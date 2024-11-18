using Dalamud.Plugin.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Runtime.InteropServices.Marshalling;
using Serilog;
using System.Text.Json.Nodes;
using System.Text;
using System.Linq;
using CurrencySpender.Classes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using CurrencySpender.Classes;
using CurrencySpender.Configuration;
using CurrencySpender.Helpers;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

namespace CurrencySpender.Helpers
{
    internal class WebHelper
    {
        public record class WhitelistUser(
            string? Name = null
        );
        internal static async Task<Boolean> CheckPrices(List<uint> ItemIds, Plugin p)
        {
            if (Service.ClientState.LocalPlayer == null || ItemIds.Count == 0)
            {
                return false;
            }

            var url = string.Join(",", ItemIds.ToArray());
            //Service.Log.Verbose(url);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://universalis.app/api/v2/aggregated/Odin/"+url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

            // Extract prices from `minListing.world`
            var itemPrices = apiResponse.Results
                .Where(result => result.Nq?.MinListing?.World?.Price.HasValue == true) // Filter out null prices
                .ToDictionary(
                    result => (uint)result.ItemId,                                   // Use itemId as the key
                    result => (uint)result.Nq.MinListing.World.Price.Value    // Cast price to uint as the value
                );

            // Log or use the extracted prices
            foreach (var obj_ in itemPrices)
            {
                //Service.Log.Info($"World Price: {obj_.Value}");
            }
            foreach (var item in p.config.Items)
            {
                if (itemPrices.TryGetValue(item.ItemId, out uint newPrice))
                {
                    // Assuming you want to update the CurrentPrice (not Price, as Price is `init`)
                    item.GetType()
                        .GetProperty(nameof(BuyableItem.CurrentPrice))!
                        .SetValue(item, newPrice);
                }
            }
            //Service.Log.Verbose(json.ToString());
            //HttpClient client = new HttpClient();
            //var prices = await GetJsonHttpClient("https://universalis.app/api/v2/aggregated/Odin/19935", client);
            //Log.Verbose(prices.ToString());
            return true;


        }
        public class WorldPrice
        {
            public int? Price { get; set; }
        }

        public class MinListing
        {
            public WorldPrice World { get; set; }
        }

        public class Nq
        {
            public MinListing MinListing { get; set; }
        }

        public class Result
        {
            public int ItemId { get; set; }
            public Nq Nq { get; set; }
        }

        public class ApiResponse
        {
            public List<Result> Results { get; set; }
            public List<object> FailedItems { get; set; }
        }

        //public static async void PostStuff()
        //{
        //    HttpClient client = new HttpClient();
        //    var values = new Dictionary<string, string> { { "index", "selling" }, { "value", "1" } };
        //    string url = "https://universail.z0x.org/api/items/reset";
        //    var data = new FormUrlEncodedContent(values);
        //    var response = await client.PostAsync(url, data);
        //    PluginLog.Debug("Reset: " + response);
        //    var length = C.MarketConfigs.Count;
        //    var items = new List<uint>();
        //    foreach (var marketConfig in C.MarketConfigs)
        //    {
        //        if (!items.Exists(x => x == marketConfig.item_id) && marketConfig.item_id != 0)
        //        {
        //            items.Add(marketConfig.item_id);
        //        }
        //    }
        //    PluginLog.Debug("Items: " + string.Join(",", items));
        //    values = new Dictionary<string, string> { { "items", string.Join(",", items) } };
        //    url = "https://universail.z0x.org/api/items/selling";
        //    data = new FormUrlEncodedContent(values);
        //    response = await client.PostAsync(url, data);
        //    PluginLog.Debug("Items Response: " + response);
        //}
    }
}
