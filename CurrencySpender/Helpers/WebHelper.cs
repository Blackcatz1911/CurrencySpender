using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;


namespace CurrencySpender.Helpers
{
    internal class WebHelper
    {
        public static String homeWorld = "";
        public static List<uint> lookup = new List<uint>();
        public static async void CheckPrices(Boolean forced = false)
        {
            if (!preCheck()) return;
            generateLookup(forced);
            try
            {
                var url = string.Join(",", lookup.ToArray());
                //Service.Log.Verbose(url);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://universalis.app/api/v2/aggregated/" + homeWorld + "/" + url);
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
                // Iterate through C.Items and update BuyableItem properties
                foreach (var item in C.Items)
                {
                    // Find a matching entry in itemPrices for the current item's ItemId
                    var priceInfo = itemPrices.FirstOrDefault(p => p.ItemId == item.ItemId);

                    if (priceInfo != default)
                    {
                        item.CurrentPrice = priceInfo.WorldPrice;
                        item.LastChecked = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    }
                }
            }
            catch(Exception e) { Service.Log.Error(e.ToString()); }
        }
        public static async void CheckSales(Boolean forced = false)
        {
            if (!preCheck()) return;
            generateLookup(forced);
            try
            {
                var url = string.Join(",", lookup.ToArray());
                //Service.Log.Verbose(url);
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
                foreach (var item in C.Items)
                {
                    // Find a matching entry in itemPrices for the current item's ItemId
                    var priceInfo = itemSales.FirstOrDefault(p => p.ItemId == item.ItemId);

                    if (priceInfo != default)
                    {
                        item.HasSoldWeek = priceInfo.Sales;
                    }
                }
            }
            catch (Exception e) { Service.Log.Error(e.ToString()); }
        }
        public static bool IsTimestampOlderThan(uint unixTimestamp, int minutes)
        {
            DateTime savedTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            return (DateTime.UtcNow - savedTime).TotalMinutes > minutes;
        }

        public static Boolean preCheck()
        {
            if (Service.ClientState.LocalPlayer == null)
            {
                Service.Log.Verbose("WebHelper early return");
                Service.Log.Verbose("LocalPlayer: " + (Service.ClientState.LocalPlayer == null));
                return false;
            }
            if (Service.ClientState.LocalPlayer != null)
            {
                homeWorld = Service.DataManager.Excel.GetSheet<Lumina.Excel.Sheets.World>().GetRow(
                    Service.ClientState.LocalPlayer.CurrentWorld.RowId).Name.ExtractText();
                if (homeWorld == "")
                {
                    Service.Log.Verbose("WebHelper early return");
                    Service.Log.Verbose("P.homeWorld: " + homeWorld);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static void generateLookup(Boolean forced = false)
        {
            lookup = [];
            foreach (var item in C.Items)
            {
                if ((item.LastChecked == 0 || IsTimestampOlderThan(item.LastChecked, 10) || forced) && !lookup.Contains(item.ItemId) && lookup.Count < 99)
                {
                    lookup.Add(item.ItemId);
                }
            }
        }
    }
}
