using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using CurrencySpender.Data;


namespace CurrencySpender.Helpers
{
    internal class WebHelper
    {
        public static string homeWorld = "";
        //public static List<uint> lookup = new List<uint>();
        public static async void CheckPrices(List<uint> lookup, bool forced = false)
        {
            try
            {
                var url = string.Join(",", lookup.ToArray());
                url = "https://universalis.app/api/v2/aggregated/" + homeWorld + "/" + url;
                PluginLog.Verbose(url);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    PluginLog.Error($"Request failed with status code {response.StatusCode}");
                    return;
                }
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(responseBody);

                var itemPrices = new List<(uint ItemId, uint WorldPrice)>();

                // Parse the "results" array
                var results = json["results"];
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        // Extract the itemId
                        uint itemId = result["itemId"]?.Value<uint>() ?? 0;

                        // Extract the world price from "minListing" -> "world"
                        uint worldPrice = result["nq"]?["minListing"]?["world"]?["price"]?.Value<uint>() ?? 0;

                        // Add it to the list
                        itemPrices.Add((itemId, worldPrice));
                    }
                }
                foreach (var item in Generator.items)
                {
                    //PluginLog.Verbose($"Item: {item.Name}");
                    // Find a matching entry in itemPrices for the current item's ItemId
                    var priceInfo = itemPrices.FirstOrDefault(p => p.ItemId == item.Id);

                    if (priceInfo != default)
                    {
                        item.CurrentPrice = priceInfo.WorldPrice;
                        item.GilPerCur = item.CurrentPrice / item.Price;
                        item.LastChecked = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        item.Type |= Classes.ItemType.Sellable;
                        //PluginLog.Verbose($"Item was changed: {item.Name}");
                    } else
                    {
                        //PluginLog.Verbose($"Item was default: {item.Name}");
                    }
                }
            }
            catch(Exception e) { PluginLog.Error(e.ToString()); }
        }
        public static async void CheckSales(List<uint> lookup, bool forced = false)
        {
            try
            {
                var url = string.Join(",", lookup.ToArray());
                //PluginLog.Verbose(url);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://universalis.app/api/v2/history/" + homeWorld + "/" + url);
                if (!response.IsSuccessStatusCode)
                {
                    PluginLog.Error($"Request failed with status code {response.StatusCode}");
                    return;
                }
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(responseBody);

                var itemSales = new List<(uint ItemId, uint Sales)>();

                // Access the "items" object in the JSON
                var items = json["items"] as JObject; // "items" is a JSON object
                if (items != null)
                {
                    foreach (var item in items.Properties()) // Iterate over the properties of the JObject
                    {
                        // Extract the itemId from the property name
                        uint itemId = uint.Parse(item.Name);

                        // Extract the sales count from "stackSizeHistogram" -> "1"
                        uint sales = item.Value["regularSaleVelocity"]?.Value<uint>() ?? 0;

                        // Add it to the list
                        itemSales.Add((itemId, sales));
                    }
                }

                // Iterate through C.Items and update BuyableItem properties
                foreach (var item in Generator.items)
                {
                    // Find a matching entry in itemPrices for the current item's ItemId
                    var priceInfo = itemSales.FirstOrDefault(p => p.ItemId == item.Id);

                    if (priceInfo != default)
                    {
                        item.HasSoldWeek = priceInfo.Sales;
                    }
                }
                P.spendingWindow.UpdateData();
            }
            catch (Exception e) { PluginLog.Error(e.ToString()); }
        }
        public static async void CheckMarketable(List<uint> lookup, bool forced = false)
        {
            try
            {
                var url = string.Join(",", lookup.ToArray());
                //PluginLog.Verbose(url);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://universalis.app/api/v2/history/" + homeWorld + "/" + url);
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(responseBody);

                var itemSales = new List<(uint ItemId, uint Sales)>();

                // Access the "items" object in the JSON
                var items = json["items"] as JObject; // "items" is a JSON object
                if (items != null)
                {
                    foreach (var item in items.Properties()) // Iterate over the properties of the JObject
                    {
                        // Extract the itemId from the property name
                        uint itemId = uint.Parse(item.Name);

                        // Extract the sales count from "stackSizeHistogram" -> "1"
                        uint sales = item.Value["regularSaleVelocity"]?.Value<uint>() ?? 0;

                        // Add it to the list
                        itemSales.Add((itemId, sales));
                    }
                }

                // Iterate through C.Items and update BuyableItem properties
                foreach (var item in Generator.items)
                {
                    // Find a matching entry in itemPrices for the current item's ItemId
                    var priceInfo = itemSales.FirstOrDefault(p => p.ItemId == item.Id);

                    if (priceInfo != default)
                    {
                        item.HasSoldWeek = priceInfo.Sales;
                    }
                }
            }
            catch (Exception e) { PluginLog.Error(e.ToString()); }
        }
        public static bool IsTimestampOlderThan(uint unixTimestamp, int minutes)
        {
            DateTime savedTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            return (DateTime.UtcNow - savedTime).TotalMinutes > minutes;
        }

        public static bool preCheck()
        {
            if (Service.ClientState.LocalPlayer == null)
            {
                PluginLog.Verbose("WebHelper early return");
                PluginLog.Verbose("LocalPlayer: " + (Service.ClientState.LocalPlayer == null));
                return false;
            }
            if (Service.ClientState.LocalPlayer != null)
            {
                homeWorld = Service.DataManager.Excel.GetSheet<Lumina.Excel.Sheets.World>().GetRow(
                    Service.ClientState.LocalPlayer.CurrentWorld.RowId).Name.ExtractText();
                if (homeWorld == "")
                {
                    PluginLog.Verbose("WebHelper early return");
                    PluginLog.Verbose("P.homeWorld: " + homeWorld);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static List<uint> generateLookup(uint currencyId, bool forced = false)
        {
            List<uint> lookup = new List<uint>();
            foreach (var item in Generator.items)
            {
                if ((item.LastChecked == 0 || IsTimestampOlderThan(item.LastChecked, 30) || forced) && !lookup.Contains(item.Id)
                    && item.Type.HasFlag(Classes.ItemType.Tradeable) && item.Currency == currencyId)
                    lookup.Add(item.Id);
            }
            return lookup;
        }
        public static void CheckAll(uint currencyId, bool forced = false)
        {
            if (!preCheck()) return;
            List<uint> lookup = generateLookup(currencyId, forced);
            //if (C.Debug) return;
            for (int i = 0; i < lookup.Count; i += 90)
            {
                int max = Math.Min(lookup.Count, (i + 90))-1;
                int range = max - i + 1;
                //PluginLog.Verbose($"From {i} to {max}, range: {range}");
                List<uint> list = lookup.GetRange(i, range);
                P.TaskManager.Enqueue(() => CheckPrices(list));
                P.TaskManager.Enqueue(() => CheckSales(list));
            }
        }
    }
}
