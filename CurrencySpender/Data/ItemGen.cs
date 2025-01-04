using CurrencySpender.Classes;
using ECommons;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Data
{
    internal class ItemGen
    {
        internal static bool FateShopsDone = false;
        internal static bool GCShopsDone = false;
        public static List<uint> enabled_currencies = new List<uint>();
        public static void init()
        {
            PluginLog.Debug("ItemGen init");
            foreach(var currency in C.Currencies)
            {
                if (currency.Enabled) enabled_currencies.Add(currency.ItemId);
            }
            PluginLog.Verbose($"SpecialShops: {Generator.shops.Where(shop => shop.Type == ShopType.SpecialShop).ToList().Count}");
            PluginLog.Verbose($"GCShops: {Generator.shops.Where(shop => shop.Type == ShopType.GCShop).ToList().Count}");
            PluginLog.Verbose($"FateShops: {Generator.shops.Where(shop => shop.Type == ShopType.FateShop).ToList().Count}");
            foreach (var shop in Generator.shops)
            {
                if(shop.Type == ShopType.SpecialShop)
                {
                    specialShop(shop);
                }
                else if (shop.Type == ShopType.GCShop)
                {
                    GCShop(shop);
                }
                else if (shop.Type == ShopType.FateShop)
                {
                    specialShop(shop);
                }
                //PluginLog.Verbose($"{shop}");
            }
            if (PlayerHelper.SharedFateRanksCreated)
            {
                PluginLog.Debug("Starting fateShops");
                fateShops();
            } else PluginLog.Debug("Not starting fateShops");
            if (PlayerHelper.GCRanksCreated) GCShops();
            PluginLog.Debug("ItemGen init finished");

            //foreach (var item in Generator.items)
            //{
            //    PluginLog.Verbose(item.ToString());
            //}
        }

        internal static void specialShop(Shop shop)
        {
            var shop_ = Service.DataManager.GetExcelSheet<SpecialShop>().GetRow(shop.ShopId);
            var itemCol = shop_.Item;
            foreach (var itemCol_ in itemCol)
            {
                var temp = new List<(uint, uint)>();
                //PluginLog.Verbose(itemCol_.ItemCosts.ToString());
                for (int i = 0; i < itemCol_.ReceiveItems.Count; i++)
                {
                    if (i >= itemCol_.ItemCosts.Count) continue;
                    if (itemCol_.ItemCosts[i].ItemCost.RowId == 0) continue;
                    if (itemCol_.ReceiveItems[i].Item.RowId == 0) continue;

                    var item_types = ItemHelper.GetItemTypes(itemCol_.ReceiveItems[i].Item);
                    //PluginLog.Verbose(types.ToString());
                    var costItemId = itemCol_.ItemCosts[i].ItemCost.RowId;
                    var cur = ConvertCurrencyId(shop_.RowId, costItemId, shop_.UseCurrencyType);
                    var cur_item = Service.DataManager.GetExcelSheet<Item>().GetRow(cur);
                    //if (!enabled_currencies.Contains(cur)) continue;
                    // if(itemCol_.ReceiveItems[i].Item.RowId == 43590)
                    //     PluginLog.Verbose($"{cur}-{cur_item.Name}-{shop.NpcName}-{shop.ShopId}-{ itemCol_.ReceiveItems[i].Item.Value.Name.ToString()}");
                    var existing_item = Generator.items.FirstOrDefault(it => it.Id == itemCol_.ReceiveItems[i].Item.RowId && it.ShopId == shop.ShopId); //it.Shop.NpcId == shop.NpcId);
                    if(existing_item == default)
                    {
                        if(costItemId == 26807)
                        {
                            //itemCol_
                        }
                        ShopItem shopItem = new ShopItem
                        {
                            Id = itemCol_.ReceiveItems[i].Item.RowId,
                            ShopId = shop_.RowId,
                            Price = itemCol_.ItemCosts[i].CurrencyCost,
                            Currency = cur,
                            Category = itemCol_.ReceiveItems[i].Item.Value.ItemUICategory.RowId,
                            Type = item_types,
                            Shop = shop
                        };
                        Generator.items.Add(shopItem);
                        shop.Items.Add(shopItem);
                    }
                    //PluginLog.Verbose($"{i}/{itemCol_.ItemCosts.Count}");
                }
                //PluginLog.Verbose($"{itemCol_.ToString()}");
            }
        }

        internal static void GCShop(Shop shop)
        {
            var GCShopSheet = Service.DataManager.GetExcelSheet<GCShop>();
            var GCScripShopCategorySheet = Service.DataManager.GetExcelSheet<GCScripShopCategory>();
            var GCScripShopItemSheet = Service.DataManager.GetSubrowExcelSheet<GCScripShopItem>();
            var GCItem = Service.DataManager.GetExcelSheet<Item>().GetRow(20);

            foreach (var gcShop in GCShopSheet)
            {
                var gcShopCategories = GCScripShopCategorySheet.Where(i => i.GrandCompany.RowId == shop.GC).ToList();
                if (gcShopCategories.Count == 0)
                {
                    return;
                }

                foreach (var category in gcShopCategories)
                {
                    //PluginLog.Verbose(GCScripShopItemSheet.TotalSubrowCount.ToString());
                    for (var i = 0u; i < GCScripShopItemSheet.TotalSubrowCount; i++)
                    {
                        //PluginLog.Verbose(GCScripShopItemSheet.TotalSubrowCount.ToString());
                        var GCScripShopItem = GCScripShopItemSheet.GetSubrow(category.RowId, (ushort)i);
                        //PluginLog.Verbose(GCScripShopItem.Item.RowId.ToString());
                        if (GCScripShopItem.RowId == 0)
                        {
                            break;
                        }

                        //var item = Service.DataManager.GetExcelSheet<Item>().GetRow(GCScripShopItem.Item.RowId);
                        var item = GCScripShopItem.Item.Value;
                        var item_ref = GCScripShopItem.Item;
                        if (item.RowId == 0)
                        {
                            break;
                        }
                        var cat = item.ItemUICategory.RowId;
                        var types = ItemHelper.GetItemTypes(item_ref);

                        var existing_item = Generator.items.FirstOrDefault(existing_item => existing_item.Id == item.RowId && existing_item.ShopId == shop.ShopId);
                        if (existing_item == default)
                        {
                            uint requiredRank = GCScripShopItem.RequiredGrandCompanyRank.RowId;
                            ShopItem shopItem = new ShopItem
                            {
                                Id = item.RowId,
                                ShopId = shop.ShopId,
                                Price = GCScripShopItem.CostGCSeals,
                                Currency = shop.Currency,
                                Category = item.ItemUICategory.RowId,
                                Type = types,
                                Shop = shop,
                                RequiredRank = requiredRank,
                            };
                            Generator.items.Add(shopItem);
                            shop.Items.Add(shopItem);
                        }
                    }
                }
            }
        }

        internal static void fateShops()
        {
            if (FateShopsDone)
            {
                PluginLog.Debug("FateShopDone");
                return;
            }
            PluginLog.Debug("FateShop init");
            // Assuming `Generator.shops` is a list of Shop objects
            var shops = Generator.shops.Where(shop => shop.Type == ShopType.FateShop).ToList();
            PluginLog.Verbose($"shops: {shops.Count}");

            // Group FateShops by NpcId
            var groupedShops = shops.GroupBy(shop => shop.NpcId);

            // Create a dictionary to store the shop with the most items for each NpcId
            var shopsWithMaxItems = new Dictionary<uint, Shop>();

            // Step 1: Find the shop with the maximum number of items for each NpcId
            foreach (var group in groupedShops)
            {
                // Find the shop with the most items in this group using the ItemCount property
                var shopWithMaxItems = group.OrderByDescending(shop => shop.ItemCount).First();
                shopsWithMaxItems[shopWithMaxItems.NpcId] = shopWithMaxItems;
            }

            // Step 2: Iterate through the shops and disable items in shops that have fewer items
            foreach (var shop in shops)
            {
                if (!shopsWithMaxItems.ContainsKey(shop.NpcId) || shopsWithMaxItems[shop.NpcId] != shop)
                {
                    // This shop has fewer items than the shop with the maximum, disable its items and the shop itself
                    foreach (var item in shop.Items)
                    {
                        item.Disabled = true; // Disable the items in this shop
                    }
                    shop.Disabled = true; // Disable the shop itself
                }
            }

            Dictionary<uint, (int Rank, List<uint> TerritoryIds)> ranks = new Dictionary<uint, (int, List<uint>)>
            {
                { 1027998, (3, [813, 814, 815, 816, 817, 818]) },
                { 1027538, (3, [813, 814, 815, 816, 817, 818]) },

                { 1037055, (3, [956, 957, 958, 959, 960, 961]) },
                { 1037304, (3, [956, 957, 958, 959, 960, 961]) },

                { 1048383, (4, [1187, 1188, 1189, 1190, 1191, 1192]) },
                { 1049082, (4, [1187, 1188, 1189, 1190, 1191, 1192]) },
            };
            Dictionary<uint, List<List<uint>>> item_ids = new Dictionary<uint, List<List<uint>>>
            {
                { 1027497, [[29709, 27962, 27850, 27798, 6141, 17837, 7621, 21800], [28881,
                    25186, 25187, 25188, 25189, 25190, 25197, 25198, 26727, 26728, 26729, 26730, 26731, 26738, 26739],
                    [27896, 26769]] },
                { 1027892, [[27963, 27852, 27756, 27735, 6141, 17837, 7621, 21800], [28882,
                    25186, 25187, 25188, 25189, 25190, 25197, 25198, 26727, 26728, 26729, 26730, 26731, 26738, 26739],
                    [27897, 26792]] },
                { 1027385, [[29706, 28999, 29000, 27961, 27732, 27763, 27764, 6141, 17837, 7621, 21800], [28880,
                    25186, 25187, 25188, 25189, 25190, 25197, 25198, 26727, 26728, 26729, 26730, 26731, 26738, 26739],
                    [27895, 27989]] },
                { 1027665, [[29704, 33332, 39370, 33269, 27964, 27851, 27797, 27733, 6141, 17837, 7621, 21800], [28883, 30264, 32232,
                    25186, 25187, 25188, 25189, 25190, 25197, 25198, 26727, 26728, 26729, 26730, 26731, 26738, 26739],
                    [27898, 27276, 30090]] },
                { 1027709, [[29710, 27965, 27734, 27774, 27773, 6141, 17837, 7621, 21800], [28884, 28635,
                    25186, 25187, 25188, 25189, 25190, 25197, 25198, 26727, 26728, 26729, 26730, 26731, 26738, 26739],
                    [27899, 26804]] },
                { 1027766, [[29713, 33274, 28972, 27966, 27736, 27799, 27800, 6141, 17837, 7621, 21800], [28885, 30263,
                    25186, 25187, 25188, 25189, 25190, 25197, 25198, 26727, 26728, 26729, 26730, 26731, 26738, 26739],
                    [27900, 27313]] },

                { 1037484, [[36243, 36254, 36261], [35962], [35799, 37424]] },
                { 1037635, [[36242, 36245, 36253, 36264], [35963], [36362, 37425, 35807, 37342]] },
                { 1037724, [[36255, 36203, 36244], [35964], [36363, 37426, 37427, 38650, 38651, 37389, 35805, 38599 ]] },
                { 1037793, [[36257, 36258, 36259], [35965], [36364, 37429, 35800]] },
                { 1037909, [[36246, 36256, 36630, 36260], [35967], [36365, 37428, 36280, 38438, 41141, 38627]] },
                { 1038004, [[36262], [356966], [36366, 36267, 37341, 35801, 40628, 35971]] },

                { 1048628, [[44063, 44067, 44053], [43607], [44114], [44312, 45009, 43571]] },
                { 1048778, [[44064, 44054, 44068, 44069], [43608], [44115], [44313, 45010, 41819]] },
                { 1048933, [[44065, 44055, 44070], [43609], [44121], [44314, 45011, 43574]] },
                { 1049283, [[44066, 44106, 44027, 44071], [43610], [44117], [44315, 45012, 43601, 44479]] },
                { 1049438, [[44056, 44072], [43611], [44118], [44316, 45013, 43873]] },
                { 1049528, [[44057], [43612], [44122], [44317, 45014, 43874, 44480]] },
            };
            foreach (var shop in Generator.shops.Where(shop => shop.Type == ShopType.FateShop && !shop.Disabled))
            {
                if (ranks.ContainsKey(shop.NpcId))
                {
                    // Get the rank and territory IDs for this NpcId
                    var rankInfo = ranks[shop.NpcId];
                    int requiredRank = rankInfo.Rank; // The rank required
                    List<uint> territoryIds = rankInfo.TerritoryIds; // List of territory IDs for which this rank applies
                    bool unlocked = true;
                    foreach (var territoryId in territoryIds)
                    {
                        if (PlayerHelper.SharedFateRanks.ContainsKey(territoryId))
                        {
                            var playerRank = PlayerHelper.SharedFateRanks[territoryId];
                            PluginLog.Debug($"PlayerRank: {playerRank} for {territoryId}, Required: {requiredRank}");

                            // If the player's rank matches the required rank for this NpcId, perform actions
                            if (playerRank != requiredRank)
                            {
                                unlocked = false;
                            }
                        }
                    }
                    if(!unlocked)
                    {
                        PluginLog.Information($"{shop.NpcName} not unlocked!");
                        foreach (var item in shop.Items)
                        {
                            item.Disabled = true; // Disable the items in this shop
                        }
                        shop.Disabled = true; // Disable the shop itself
                    }
                }


                //PluginLog.Verbose(shop.ToString());
                if (item_ids.TryGetValue(shop.NpcId, out var rankGroups))
                {
                    if (PlayerHelper.SharedFateRanks.TryGetValue(shop.Location.TerritoryId, out var playerRank))
                    {
                        // Flatten all visible items up to the player's current rank
                        var visibleItems = rankGroups
                            .Take((int)playerRank) // Include only ranks up to the player's current rank
                            .SelectMany(group => group) // Flatten into a single list of item IDs
                            .ToHashSet(); // Use HashSet for quick lookups

                        // Iterate through the shop's items
                        var items = Generator.items.Where(item => item.Shop.ShopId == shop.ShopId).ToList();
                        foreach (var item in items)
                        {
                            // If the item is not in the visible list, disable it
                            item.Disabled = !visibleItems.Contains(item.Id);
                        }
                    }
                }
                //PluginLog.Verbose("---");
                //PluginLog.Verbose(shop.ToString());
                //var items_ = Generator.items.Where(item => item.Shop.ShopId == shop.ShopId && !item.Disabled).ToList();
                //foreach (var item in items_)
                //{
                //    PluginLog.Verbose(item.ToString());
                //}
                //PluginLog.Verbose("---");
            }
            FateShopsDone = true;
            PluginLog.Verbose("FateShop init finished");
        }

        internal static void GCShops()
        {
            if (GCShopsDone) return;
            PluginLog.Verbose("GCShops init");
            var shops = Generator.shops.Where(shop => shop.Type == ShopType.GCShop).ToList();

            foreach (var shop in shops)
            {
                foreach (var item in shop.Items)
                {
                    if(shop.GC != null && item.RequiredRank > PlayerHelper.GCRanks[(uint)shop.GC])
                        item.Disabled = true;
                }
            }
            GCShopsDone = true;
            PluginLog.Verbose("GCShops init finished");
        }

        private static Dictionary<uint, uint> Currencies_Dict = new Dictionary<uint, uint>()
        {
            { 1, 10309 },
            { 2, 33913 }, // Unlimited Crafters{  scrip
            { 3, 10311 },
            { 4, 33914 }, // Unlimited Gatherers{  scrip
            { 5, 10307 },
            { 6, 41784 }, // Limited Crafters{  scrip
            { 7, 41785 }, // Limited Gatherers{  scrip
            { 8, 21072 },
            { 9, 21073 },
            { 10, 21074 },
            { 11, 21075 },
            { 12, 21076 },
            { 13, 21077 },
            { 14, 21078 },
            { 15, 21079 },
            { 16, 21080 },
            { 17, 21081 },
            { 18, 21172 },
            { 19, 21173 },
            { 20, 21935 },
            { 21, 22525 },
            { 22, 26533 },
            { 23, 26807 },
            { 24, 28063 },
            { 25, 28186 },
            { 26, 28187 },
            { 27, 28188 },
            { 28, 30341 }
        };
        private static Dictionary<uint, uint> TomeStones_Dict = new Dictionary<uint, uint>() {
            { 1, 28 },
            { 2, 46 },
            { 3, 47 },
        };
        public static uint ConvertCurrencyId(uint specialShopId, uint itemId, ushort useCurrencyType)
        {
            if (specialShopId == 1770637)
            {
                if (Currencies_Dict.TryGetValue(itemId, out var currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            if (specialShopId == 1770446 || (specialShopId == 1770699 && itemId < 10))
            {
                if (Currencies_Dict.TryGetValue(itemId, out var currencyValue) || TomeStones_Dict.TryGetValue(itemId, out currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            if (useCurrencyType == 2 && itemId < 10)
            {
                if (TomeStones_Dict.TryGetValue(itemId, out var tomestoneValue))
                {
                    return tomestoneValue;
                }
                return itemId;
            }

            if ((useCurrencyType == 16 || useCurrencyType == 4) && itemId < 10)
            {
                if (TomeStones_Dict.TryGetValue(itemId, out var currencyValue) || Currencies_Dict.TryGetValue(itemId, out currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            return itemId;
        }
    }
}
