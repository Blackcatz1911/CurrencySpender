using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CurrencySpender.Data;


namespace CurrencySpender.Helpers
{
    internal class WebHelper
    {
        private static readonly HttpClient Client = new();
        private static string HomeWorld = "";

        public static async Task CheckPrices(List<uint> itemIds)
        {
            try
            {
                var url = $"https://universalis.app/api/v2/aggregated/{HomeWorld}/{string.Join(",", itemIds)}";
                PluginLog.Verbose(url);
                var json = await GetJson(url);
                if (json == null) return;

                var byId = new Dictionary<uint, uint>();
                foreach (var result in (JArray?)json["results"] ?? new JArray())
                {
                    uint itemId = result["itemId"]?.Value<uint>() ?? 0;
                    if (itemId == 0) continue;
                    byId[itemId] = result["nq"]?["minListing"]?["world"]?["price"]?.Value<uint>() ?? 0;
                }

                foreach (var item in Generator.items.Where(i => byId.ContainsKey(i.Id)))
                {
                    item.CurrentPrice = byId[item.Id];
                    item.GilPerCur = item.CurrentPrice / item.Price;
                    item.LastChecked = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    item.Type |= Classes.ItemType.Sellable;
                }
            }
            catch (Exception e) { PluginLog.Error(e.ToString()); }
        }

        public static async Task CheckSales(List<uint> itemIds)
        {
            try
            {
                var url = $"https://universalis.app/api/v2/history/{HomeWorld}/{string.Join(",", itemIds)}";
                var json = await GetJson(url);
                if (json == null) return;

                var byId = new Dictionary<uint, uint>();
                if (json["items"] is JObject items)
                {
                    foreach (var property in items.Properties())
                    {
                        if (uint.TryParse(property.Name, out var itemId))
                            byId[itemId] = property.Value["regularSaleVelocity"]?.Value<uint>() ?? 0;
                    }
                }

                foreach (var item in Generator.items.Where(i => byId.ContainsKey(i.Id)))
                    item.HasSoldWeek = byId[item.Id];

                // We are on an HTTP continuation thread here; mutate UI state only on the framework thread.
                Service.Framework.RunOnTick(P.spendingWindow.UpdateData);
            }
            catch (Exception e) { PluginLog.Error(e.ToString()); }
        }

        private static async Task<JObject?> GetJson(string url)
        {
            var response = await Client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                PluginLog.Error($"Request failed with status code {response.StatusCode}: {url}");
                return null;
            }
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JObject>(body);
        }

        public static bool IsTimestampOlderThan(uint unixTimestamp, int minutes)
        {
            DateTime savedTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            return (DateTime.UtcNow - savedTime).TotalMinutes > minutes;
        }

        public static bool preCheck()
        {
            if (Service.ObjectTable.LocalPlayer == null)
            {
                PluginLog.Verbose("WebHelper early return");
                PluginLog.Verbose("LocalPlayer: " + (Service.ObjectTable.LocalPlayer == null));
                return false;
            }
            HomeWorld = Service.DataManager.Excel.GetSheet<Lumina.Excel.Sheets.World>().GetRow(
                Service.ObjectTable.LocalPlayer.CurrentWorld.RowId).Name.ExtractText();
            if (HomeWorld == "")
            {
                PluginLog.Verbose("WebHelper early return");
                PluginLog.Verbose("P.homeWorld: " + HomeWorld);
                return false;
            }
            return true;
        }

        public static List<uint> generateLookup(uint currencyId, bool forced = false)
        {
            HashSet<uint> lookup = new();
            foreach (var item in Generator.items)
            {
                if ((item.LastChecked == 0 || IsTimestampOlderThan(item.LastChecked, 30) || forced)
                    && item.Type.HasFlag(Classes.ItemType.Tradeable) && item.Currency == currencyId)
                    lookup.Add(item.Id);
            }
            return lookup.ToList();
        }

        public static void CheckAll(uint currencyId, bool forced = false)
        {
            if (!preCheck()) return;
            List<uint> lookup = generateLookup(currencyId, forced);
            for (int i = 0; i < lookup.Count; i += 90)
            {
                var batch = lookup.GetRange(i, Math.Min(90, lookup.Count - i));
                P.TaskManager.Enqueue(() => _ = CheckPrices(batch));
                P.TaskManager.Enqueue(() => _ = CheckSales(batch));
            }
        }
    }
}
