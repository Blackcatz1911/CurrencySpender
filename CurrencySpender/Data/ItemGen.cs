using CurrencySpender.Classes;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Data
{
    internal class ItemGen
    {
        internal static bool FateShopsDone = false;
        internal static bool GCShopsDone = false;

        // Shared-FATE gemstone vendors: NPC → (required shared FATE rank, applicable territories).
        private static readonly Dictionary<uint, (int Rank, List<uint> TerritoryIds)> SharedFateRanks = new()
        {
            { 1027998, (3, [813, 814, 815, 816, 817, 818]) },
            { 1027538, (3, [813, 814, 815, 816, 817, 818]) },

            { 1037055, (3, [956, 957, 958, 959, 960, 961]) },
            { 1037304, (3, [956, 957, 958, 959, 960, 961]) },

            { 1048383, (4, [1187, 1188, 1189, 1190, 1191, 1192]) },
            { 1049082, (4, [1187, 1188, 1189, 1190, 1191, 1192]) },
        };

        // Shared-FATE vendors: NPC → items per rank tier; tiers above the player's rank are disabled.
        private static readonly Dictionary<uint, List<List<uint>>> SharedFateItems = new()
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

        /// <summary>"Orbitingway Gamba" endgame credit shops: NPC → (currency item id, item ids).</summary>
        private static readonly Dictionary<uint, (uint Currency, uint[] Items)> CustomShops = new()
        {
            { 1052612, (45691, [44505, 44509, 47966, 48154, 48160, 48210, 48220, 48221]) }, // Lunar Credit
            { 1052642, (48146, [47973, 46795, 46782, 46840, 46155]) },                      // Phaenna Credit
            { 1052652, (48147, [50803, 50455, 50458, 50441, 50323]) },                      // Oizys Credit
            { 1056826, (48148, [52359, 52648, 52449, 52267, 52275]) },                      // Auxesia Credit
        };
        
        public static void init()
        {
            PluginLog.Debug("ItemGen init");
            ItemHelper.InitTribes();
            
            List<uint> npcIds = [1052612, 1052642, 1052652, 1056826];
            foreach (uint npcId in npcIds)
            {
                Location location = Location.GetLocation(npcId);
                Generator.shops.Add(new Shop
                {
                    ShopId = (420U * 100000U) + npcId, NpcId = npcId, Type = ShopType.CustomShop, Location = location,
                });
            }
            
            PluginLog.Debug($"SpecialShops: {Generator.shops.Where(shop => shop.Type == ShopType.SpecialShop).ToList().Count}");
            PluginLog.Debug($"GCShops: {Generator.shops.Where(shop => shop.Type == ShopType.GCShop).ToList().Count}");
            PluginLog.Debug($"FateShops: {Generator.shops.Where(shop => shop.Type == ShopType.FateShop).ToList().Count}");
            PluginLog.Debug($"CustomShops: {Generator.shops.Where(shop => shop.Type == ShopType.CustomShop).ToList().Count}");
            foreach (var shop in Generator.shops)
            {
                //PluginLog.Verbose($"{shop}");
                switch (shop.Type)
                {
                    case ShopType.SpecialShop or ShopType.FateShop:
                        specialShop(shop);
                        break;
                    case ShopType.GCShop:
                        GCShop(shop);
                        break;
                    case ShopType.CustomShop:
                        customShop(shop);
                        break;
                }
            }
            if (PlayerHelper.SharedFateRanksCreated)
            {
                PluginLog.Debug("Starting fateShops");
                fateShops();
            } else PluginLog.Debug("Not starting fateShops");
            if (PlayerHelper.GCRanksCreated) GCShops();
            
            Generator.ItemsFinished = true;
            if(Generator.ShopsFinished && Generator.ItemsFinished) Generator.AllFinished = true;
            
            PluginLog.Debug("ItemGen init finished");

            // List<String> names = ["Western Reach Framer's Kit", "Eastern Reach Framer's Kit", "Mahjong Mastery Framer's Kit"];
            // foreach (var item in Generator.items)
            // {
            //     if (names.Contains(item.Name))
            //     {
            //         DuoLog.Information($"{item.Id}");
            //     }
            // }
        }

        internal static void specialShop(Shop shop)
        {
            var shop_ = Service.DataManager.GetExcelSheet<SpecialShop>().GetRow(shop.ShopId);
            shop.ShopName = shop_.Name.ExtractText();
            // if(shop.ShopId == 1770638) PluginLog.Verbose($"SpecialShop: {shop.ShopId}-{shop.ShopName}-{shop.NpcName}");
            var itemCol = shop_.Item;
            foreach (var itemCol_ in itemCol)
            {
                for (int i = 0; i < itemCol_.ReceiveItems.Count; i++)
                {
                    if (i >= itemCol_.ItemCosts.Count) continue;
                    if (itemCol_.ItemCosts[i].ItemCost.RowId == 0) continue;
                    if (itemCol_.ReceiveItems[i].Item.RowId == 0) continue;

                    var costItemId = itemCol_.ItemCosts[i].ItemCost.RowId;
                    var cur = ConvertCurrencyId(shop_.RowId, costItemId, shop_.UseCurrencyType);
                    // if (shop.ShopId == 1770638) PluginLog.Verbose($"SpecialShop Item: {itemCol_.ReceiveItems[i].Item.RowId}-{itemCol_.ReceiveItems[i].Item.Value.Name}-Cur:{cur}-{shop.NpcName}-{shop.ShopId}-CurrencyId:{costItemId}");

                    if (P.Currencies.Where(c => c.Enabled && c.ItemId == cur).ToList().Count() == 0) continue;

                    var item_types = ItemHelper.GetItemTypes(itemCol_.ReceiveItems[i].Item.RowId);
                    var CollectableType = ItemHelper.GetCollectableType(itemCol_.ReceiveItems[i].Item, item_types);
                    //if (CollectableType == CollectableType.Container) PluginLog.Debug($"specialShop Container: {itemCol_.ReceiveItems[i].Item.RowId}, shop: {shop.NpcName}");
                    //if (CollectableType == CollectableType.Hairstyle) PluginLog.Debug($"specialShop Hairstyle: {itemCol_.ReceiveItems[i].Item.RowId}, shop: {shop.NpcName}");
                    //PluginLog.Verbose(item_types.ToString());
                    if (C.Debug && itemCol_.ReceiveItems[i].Item.RowId == 5089)
                    {
                        // PluginLog.Verbose($"{cur}-{cur_item.Name}-{shop.NpcName}-{shop.ShopId}-CurrencyId:{costItemId}-{ itemCol_.ReceiveItems[i].Item.Value.Name.ToString()}");
                        // PluginLog.Verbose($"{itemCol_.ReceiveItems[i].Item.Value.}");
                    }

                    var existingItem = Generator.items.FirstOrDefault(it => it.Id == itemCol_.ReceiveItems[i].Item.RowId && it.Shop.NpcId == shop.NpcId);
                    if(existingItem == null)
                    {
                        ShopItem shopItem = new ShopItem
                        {
                            Id = itemCol_.ReceiveItems[i].Item.RowId,
                            ShopId = shop_.RowId,
                            Price = itemCol_.ItemCosts[i].CurrencyCost,
                            Currency = cur,
                            Category = itemCol_.ReceiveItems[i].Item.Value.ItemUICategory.RowId,
                            Type = item_types,
                            CollectableType = CollectableType,
                            Shop = shop,
                            PreReq = itemCol_.Quest.RowId != 0 || itemCol_.AchievementUnlock.RowId != 0,
                            QuestId = itemCol_.Quest.RowId != 0 ? itemCol_.Quest.RowId : null,
                            AchievementId = itemCol_.AchievementUnlock.RowId != 0 ? itemCol_.AchievementUnlock.RowId : null,
                        };
                        if (ItemHelper.TribeByCurrency.TryGetValue(cur, out var tribeId))
                        {
                            if (!ItemHelper.IsReputationReached(shopItem.Id, tribeId, itemCol_.Quest.RowId)) shopItem.Disabled = true;
                        }
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

            foreach (var gcShop in GCShopSheet)
            {
                var gcShopCategories = GCScripShopCategorySheet.Where(i => i.GrandCompany.RowId == shop.GC).ToList();
                if (gcShopCategories.Count == 0)
                {
                    //PluginLog.Debug($"gcShopCategories.Count: {gcShopCategories.Count}");
                    return;
                }

                foreach (var category in gcShopCategories)
                {
                    //PluginLog.Verbose(GCScripShopItemSheet.TotalSubrowCount.ToString());
                    for (var i = 0; i < GCScripShopItemSheet.TotalSubrowCount; i++)
                    {
                        //PluginLog.Debug($"TotalSubrowCount: {GCScripShopItemSheet.TotalSubrowCount}, {i}");
                        var GCScripShopItem = GCScripShopItemSheet.GetSubrow(category.RowId, (ushort)i);
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
                        var types = ItemHelper.GetItemTypes(item_ref.RowId);
                        var CollectableType = ItemHelper.GetCollectableType(item_ref, types);
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
                                CollectableType = CollectableType,
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

            foreach (var shop in Generator.shops.Where(shop => shop.Type == ShopType.FateShop && !shop.Disabled))
            {
                if (SharedFateRanks.TryGetValue(shop.NpcId, out var rankInfo))
                {
                    int requiredRank = rankInfo.Rank;
                    List<uint> territoryIds = rankInfo.TerritoryIds;
                    bool unlocked = true;
                    foreach (var territoryId in territoryIds)
                    {
                        if (PlayerHelper.SharedFateRanks.TryGetValue(territoryId, out var playerRank)
                            && playerRank != (uint)requiredRank)
                        {
                            unlocked = false;
                        }
                        //PluginLog.Debug($"PlayerRank: {playerRank} for {territoryId}, Required: {requiredRank}");
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

                if (SharedFateItems.TryGetValue(shop.NpcId, out var rankGroups))
                {
                    if (PlayerHelper.SharedFateRanks.TryGetValue(shop.Location.TerritoryId, out var playerRank2))
                    {
                        // Flatten all visible items up to the player's current rank
                        var visibleItems = rankGroups
                            .Take((int)playerRank2) // Include only ranks up to the player's current rank
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
            PluginLog.Debug("GCShops init");
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
            PluginLog.Debug("GCShops init finished");
        }
        
        internal static void customShop(Shop shop)
        {
            if (!CustomShops.TryGetValue(shop.NpcId, out var shopData)) return;
            var cur = shopData.Currency;
            shop.ShopName = "Orbitingway Gamba";
            foreach (uint item_id in shopData.Items)
            {
                if (P.Currencies.Where(c => c.Enabled && c.ItemId == cur).ToList().Count() == 0) continue;

                Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(item_id);
                var item_types = ItemHelper.GetItemTypes(item_id);
                var CollectableType = ItemHelper.GetCollectableType(item, item_types);
                // PluginLog.Debug($"item: {item.RowId}, shop: {shop.NpcName}, CollectableType:{CollectableType}");
                // if(C.Debug && item.RowId == 45988)
                //     PluginLog.Verbose($"{cur}-{shop.NpcName}-{shop.ShopId}-CurrencyId:{cur}-{item.Name.ToString()}");
                var existingItem = Generator.items.FirstOrDefault(it => it.Id == item.RowId && it.Shop.NpcId == shop.NpcId);
                if(existingItem == null)
                {
                    ShopItem shopItem = new ShopItem
                    {
                        Id = item.RowId,
                        ShopId = shop.ShopId,
                        Price = 1000,
                        Gamba = true,
                        Currency = cur,
                        Category = item.ItemUICategory.RowId,
                        Type = item_types,
                        CollectableType = CollectableType,
                        Shop = shop
                    };
                    Generator.items.Add(shopItem);
                    shop.Items.Add(shopItem);
                    // if(C.Debug) PluginLog.Verbose(shopItem.ToString());
                }
            }
        }

        private static Dictionary<uint, uint> Currencies_Dict = new Dictionary<uint, uint>()
        {
            { 1, 10309 },
            { 2, 33913 }, // Unlimited Crafters  scrip
            { 3, 10311 },
            { 4, 33914 }, // Unlimited Gatherers  scrip
            { 5, 10307 },
            { 6, 41784 }, // Limited Crafters  scrip
            { 7, 41785 }, // Limited Gatherers  scrip
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
            { 2, Service.DataManager.GetExcelSheet<TomestonesItem>().First(item => item.Tomestones.RowId is 2).Item.RowId },
            { 3, Service.DataManager.GetExcelSheet<TomestonesItem>().First(item => item.Tomestones.RowId is 3).Item.RowId },
        };
        public static uint ConvertCurrencyId(uint specialShopId, uint itemId, ushort useCurrencyType)
        {
            if (specialShopId == 1770637)
            {
                return Currencies_Dict.TryGetValue(itemId, out var currencyValue) ? currencyValue : itemId;
            }

            if (specialShopId == 1770638)
            {
                return 33914;
            }

            if (specialShopId == 1770446 || (specialShopId == 1770699 && itemId < 10))
            {
                if (Currencies_Dict.TryGetValue(itemId, out var currencyValue) || TomeStones_Dict.TryGetValue(itemId, out currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            if (itemId < 10)
            {
                switch (useCurrencyType)
                {
                    case 2 or 4:
                        return TomeStones_Dict.TryGetValue(itemId, out var tomestoneValue) ? tomestoneValue : itemId;
                    case 16:
                        return Currencies_Dict.TryGetValue(itemId, out var creditValue) ? creditValue : itemId;
                }
            }

            return itemId;
        }
    }
}
