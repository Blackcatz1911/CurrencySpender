using Newtonsoft.Json;
using System.Net.Http;
using CurrencySpender.Classes;


namespace CurrencySpender.Helpers
{
    internal class WebHelper
    {
        public static async void CheckPrices()
        {
            List<uint> lookup = new List<uint>();
            foreach(var item in C.Items)
            {
                if((item.LastChecked == 0 || IsTimestampOlderThan(item.LastChecked, 10)) && !lookup.Contains(item.ItemId) && lookup.Count < 99)
                {
                    lookup.Add(item.ItemId);
                }
            }
            var url = string.Join(",", lookup.ToArray());
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
            foreach (var item in C.Items)
            {
                if (itemPrices.TryGetValue(item.ItemId, out uint newPrice))
                {
                    // Assuming you want to update the CurrentPrice (not Price, as Price is `init`)
                    item.GetType()
                        .GetProperty(nameof(BuyableItem.CurrentPrice))!
                        .SetValue(item, newPrice);
                    item.LastChecked = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }
            }

        }
        public static bool IsTimestampOlderThan(uint unixTimestamp, int minutes)
        {
            DateTime savedTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            return (DateTime.UtcNow - savedTime).TotalMinutes > minutes;
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
    }
}
