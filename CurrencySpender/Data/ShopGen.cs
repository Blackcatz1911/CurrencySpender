using Lumina.Excel.Sheets;
using Lumina.Excel;
using CurrencySpender.Classes;
using System.Runtime.ConstrainedExecution;

namespace CurrencySpender.Data
{
    public static class ShopGen
    {
        public static List<uint> fateShops = new List<uint>();
        public static List<uint> EventShops = [
            1770595, 1770645, 1770729, // Event Shops
            1769474, 1769747, 1769875, 1769881, 1769882, 1769952, 1769953, 1769954, 1769955, // Campaign Attendant
            1770283, 1770284, 1770336, 1770341, // Campaign Attendant
            1770596, 1770646, 1770730, // Seasonal Event 
            1769951, //Ironworks Vendor
        ];

        public static List<uint> OldShops = new List<uint> ();

        public static void init()
        {
            PluginLog.Verbose("ShopGen init");
            OldShops.AddRange(ListRange(1770434, 1770438)); // Aphorism & Astronomy
            OldShops.Add(1770446); // Astronomy
            OldShops.Add(1770267); // Astronomy
            //if(C.Shops ==  null || C.Shops.Count == 0 || C.Debug)
            foreach (var fs in Service.DataManager.GetExcelSheet<FateShop>())
            {
                if (fs.RowId == 0) continue;
                //fateShops.Add(fs.RowId);
                //PluginLog.Verbose(fs.RowId.ToString());
                var specialShops = fs.SpecialShop.ToList();
                for (int i = 0; i < fs.SpecialShop.Count; i++)
                {
                    var shop_ = fs.SpecialShop.ToList()[i];
                    if (shop_.RowId == 0) continue;
                    //PluginLog.Verbose(shop.RowId.ToString());
                    Location loc_ = Location.GetLocation(fs.RowId);
                    Generator.shops.Add(new Shop { ShopId = shop_.RowId, NpcId = fs.RowId, Type = ShopType.FateShop, Location = loc_ });
                }
                //var shop = fs.SpecialShop.ToList()[fs.SpecialShop.Count - 1];
                //if (shop.RowId == 0) continue;
                //Location loc = Location.GetLocation(fs.RowId);
                //Generator.shops.Add(new Shop { ShopId = shop.RowId, NpcId = fs.RowId, Type = ShopType.FateShop, Location = loc });
                //PluginLog.Verbose(fs.SpecialShop.First().RowId.ToString());
            }
            foreach (var npc in Service.DataManager.GetExcelSheet<ENpcBase>())
            {
                foreach (var variable in npc.ENpcData)
                {
                    EvalulateRowRef(npc, variable);
                }
            }
            //foreach (var shop in Service.DataManager.GetExcelSheet<InclusionShop>())
            //{
            //    if (shop.RowId == 0) continue;
            //    PluginLog.Information();
            //}
            foreach (var shop in Service.DataManager.GetExcelSheet<SpecialShop>())
            {
                if (shop.Item[0].ReceiveItems[0].Item.RowId == 4551 && shop.Item[1].ReceiveItems[0].Item.RowId == 0) continue;
                if (OldShops.Contains(shop.RowId)) continue;
                uint converted_cur = 0;
                foreach (var currency in shop.Item.First().ItemCosts)
                {
                    if (currency.ItemCost.RowId == 0) continue;
                    var costItemId = currency.ItemCost.RowId;
                    costItemId = ItemGen.ConvertCurrencyId(shop.RowId, costItemId, shop.UseCurrencyType);
                    if (costItemId != 0)
                    {
                        converted_cur = costItemId;
                        break;
                    }
                    //var costItem = Service.DataManager.GetExcelSheet<Item>().GetRow(costItemId);
                    //DuoLog.Information($"Currency: {costItem.Name}");
                }
                Item converted_cur_item = Service.DataManager.GetExcelSheet<Item>().GetRow(converted_cur);
                var item_cost = shop.Item.First().ItemCosts.First().ItemCost.RowId;
                //var converted_cur = ItemGen.ConvertCurrencyId(shop.RowId, item_cost, shop.UseCurrencyType);
                //PluginLog.Information($"{converted_cur}");
                if (C.Currencies.Where(cur => cur.Enabled && (cur.ItemId == item_cost || cur.ItemId == converted_cur)).ToList().Count > 0)
                {
                    var cur = ItemGen.ConvertCurrencyId(shop.RowId, shop.Item.First().ReceiveItems.First().Item.RowId, shop.UseCurrencyType);
                    if (Generator.shops.Where(s => s.ShopId == shop.RowId).ToList().Count == 0)
                    {
                        uint NpcId = 0;
                        Dictionary<List<uint>, uint> npcMapping = new()
                        {
                            { ListRange(1770112, 1770114), 1003633 }, // Scrip Exchange
                            { ListRange(1770183, 1770196), 1003633 }, // Scrip Exchange
                            { ListRange(1770264, 1770268), 1003633 }, // Scrip Exchange
                            { ListRange(1770272, 1770278), 1003633 }, // Scrip Exchange
                            { ListRange(1770420, 1770431), 1003633 }, // Scrip Exchange

                            { ListRange(1770477, 1770510), 1003633 }, // Scrip Exchange
                            { ListRange(1770518, 1770535), 1003633 }, // Scrip Exchange
                            { ListRange(1770625, 1770631), 1003633 }, // Scrip Exchange
                            { ListRange(1770706, 1770707), 1003633 }, // Scrip Exchange
                            { ListRange(1770781, 1770794), 1003633 }, // Scrip Exchange
                            { ListRange(1770868, 1770882), 1003633 }, // Scrip Exchange
                            { ListRange(1770907, 1770907), 1003633 }, // Scrip Exchange
                            { new List<uint> { 1769577, 1769578 }, 1012225 }, // Ardolain
                            { new List<uint> { 1769790, 1769791, 1769819, 1769814, 1769854, 1769883, 1769873, 1769940, 1769807 }, 1019451 }, // Eschina
                            { new List<uint> { 1769743, 1769744, 1770537 }, 1018655 }, // Disreputable Priest
                            { ListRange(1770551, 1770589), 1005244 }, // Mark Quartermaster
                            { new List<uint> { 1770888, 1770889 }, 1005244 }, // Mark Quartermaster
                            //{ new List<uint> { 1770041, 1770281, 1770301 }, 1031680 }, // Enie
                            
                            { new List<uint> { 1770601, 1770602, 1770659, 1770660 }, 1043463 }, // Horrendous Hoarder
                            { new List<uint> { 1770604, 1770643, 1770662, 1770709 }, 1043465 }, // Produce Producer

                            { new List<uint> { 1769957 }, 1027998 }, // Gramsol
                            { new List<uint> { 1769958 }, 1027538 }, // Pedronille
                            { new List<uint> { 1769959 }, 1027385 }, // Siulmet
                            { new List<uint> { 1769960 }, 1027497 }, // Zumutt
                            { new List<uint> { 1769961 }, 1027892 }, // Halden
                            { new List<uint> { 1769962 }, 1027665 }, // Sul Lad
                            { new List<uint> { 1769963 }, 1027709 }, // Nacille
                            { new List<uint> { 1769964 }, 1027766 }, // Goushs Ooan

                            { new List<uint> { 1770904 }, 1044839 }, // Dibourdier
                        };
                        //DuoLog.Information($"{npcMapping[1013397]}");
                        if (EventShops.Contains(shop.RowId)) continue;

                        NpcId = npcMapping.FirstOrDefault(kv => kv.Key.Contains(shop.RowId)).Value;

                        if (NpcId != 0)
                        {
                            Location loc = Location.GetLocation(NpcId);
                            if(loc == Location.locations[0] && C.Debug) { PluginLog.Error($"Missing location: {NpcId}"); }
                            List<uint> blacklist_shops = [1770595, 1770645, 1770729];
                            if (!blacklist_shops.Contains(shop.RowId))
                            {
                                List<uint> gemstones = [1027385, 1027497, 1027892, 1027665, 1027709, 1027766, 1027998, 1027538];
                                if (gemstones.Contains(NpcId))
                                {
                                    Generator.shops.Add(new Shop { ShopId = shop.RowId, NpcId = NpcId, Type = ShopType.FateShop, Location = loc });
                                }
                                if (NpcId == 1003633)
                                {
                                    Generator.shops.Add(new Shop { ShopId = shop.RowId, NpcId = NpcId, Type = ShopType.SpecialShop, Location = loc,
                                        NpcVariants = [1001617, 1003077]
                                    });
                                }
                                else
                                {
                                    Generator.shops.Add(new Shop { ShopId = shop.RowId, NpcId = NpcId, Type = ShopType.SpecialShop, Location = loc });
                                }
                            }
                        }
                        if (NpcId == 0)
                        {
                            if (C.Debug) PluginLog.Information($"Missing NpcId: {shop.RowId}-{shop.Name}-{converted_cur_item.Name}-{converted_cur}");
                            if (C.Debug) PluginLog.Information($"First Item: {shop.Item.First().ReceiveItems.First().Item.RowId}-{shop.Item.First().ReceiveItems.First().Item.Value.Name}");
                            if (C.Debug) PluginLog.Information($"Second Item: {shop.Item[1].ReceiveItems.First().Item.RowId}-{shop.Item[1].ReceiveItems.First().Item.Value.Name}");
                        }
                    }
                }
                else
                {
                    if (C.Debug)
                    {
                        //PluginLog.Information($"Other currency: {item_cost}");
                        //PluginLog.Information($"Missing NpcId: {shop.RowId}-{shop.Name}-{shop.Item.First().ItemCosts.First().ItemCost.Value.Name}");
                        //PluginLog.Information($"First Item: {shop.Item.First().ReceiveItems.First().Item.RowId}-{shop.Item.First().ReceiveItems.First().Item.Value.Name}");
                    }
                }
            }
            PluginLog.Verbose("ShopGen init finished");
        }
        public static void EvalulateRowRef(ENpcBase npcBase, RowRef rowRef)
        {
            ReadOnlySpan<Type> customTalkTypes = [typeof(FateShop), typeof(FccShop), typeof(SpecialShop), typeof(InclusionShop)];
            var customTalkTypeHash = RowRef.CreateTypeHash(customTalkTypes);
            var npcName = Service.DataManager.GetExcelSheet<ENpcResident>()!.GetRow(npcBase.RowId).Singular.ExtractText();
            //List<string> names = ["Enie"];
            //if (names.Contains(npcName))
            //{
            //    if (C.Debug)
            //    {
            //        if (rowRef.Is<SpecialShop>() || rowRef.Is<FccShop>() || rowRef.Is<GilShop>() || rowRef.Is<InclusionShop>() ||
            //            rowRef.Is<CustomTalk>() || rowRef.Is<TopicSelect>() || rowRef.Is<PreHandler>())
            //            DuoLog.Information($"FOUND: {npcBase.RowId}-{npcName}");
            //        if (rowRef.Is<SpecialShop>()) DuoLog.Information($"Specialshop: {rowRef.Is<SpecialShop>()} - {rowRef.RowId}");
            //        if (rowRef.Is<FccShop>()) DuoLog.Information($"FccShop: {rowRef.Is<FccShop>()} - {rowRef.RowId}");
            //        if (rowRef.Is<GilShop>()) DuoLog.Information($"GilShop: {rowRef.Is<GilShop>()} - {rowRef.RowId}");
            //        if (rowRef.Is<CustomTalk>()) DuoLog.Information($"CustomTalk: {rowRef.Is<CustomTalk>()} - {rowRef.RowId}");
            //        if (rowRef.Is<TopicSelect>()) DuoLog.Information($"TopicSelect: {rowRef.Is<TopicSelect>()} - {rowRef.RowId}");
            //        if (rowRef.Is<PreHandler>()) DuoLog.Information($"PreHandler: {rowRef.Is<PreHandler>()} - {rowRef.RowId}");
            //        if (rowRef.Is<InclusionShop>()) DuoLog.Information($"InclusionShop: {rowRef.Is<InclusionShop>()} - {rowRef.RowId}");
            //    }
            //}
            Location loc = Location.GetLocation(npcBase.RowId);
            //var loc = Location.locations.FirstOrDefault(loc => loc.Name.Equals(npcName, StringComparison.OrdinalIgnoreCase));
            //if (loc == default) { loc = Location.locations[0]; }
            if (npcName.ToLower() == "mark quartermaster")
            {
                //PluginLog.Verbose($"FOUND: {npcBase.RowId}-{npcName}");
                //PluginLog.Verbose($"{rowRef.Is<FccShop>()}-{rowRef.Is<GCShop>()}-{rowRef.Is<GilShop>()}-{rowRef.Is<SpecialShop>()}-{rowRef.Is<CustomTalk>()}");
                //PluginLog.Verbose($"{rowRef.Is<TopicSelect>()}-{rowRef.Is<PreHandler>()}-{rowRef.RowId}");
            }
            if (rowRef.Is<FccShop>())
            {
                Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.FccShop, Location = loc });
            }
            else if (rowRef.Is<GCShop>())
            {
                //var shop = Service.DataManager.GetExcelSheet<GCShop>()?.GetRow(rowRef.RowId);
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GCShop));
                uint gc = 1; if (npcName.Contains("flame")) gc = 2; if (npcName.Contains("serpent")) gc = 3;
                uint cur = gc + 19;

                Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.GCShop, Location = loc,
                    GC = gc, Currency = cur });
            }
            else if (rowRef.Is<GilShop>())
            {
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                //var gilShop = Service.DataManager.GetExcelSheet<GilShop>()?.GetRow(rowRef.RowId);
                //list.Add((gilShop.Value.Name.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                // Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.GilShop });
            }
            else if (rowRef.Is<SpecialShop>())
            {
                //if(rowRef.RowId == 
                //if (loc == Location.locations[0] && C.Debug) { DuoLog.Error($"Missing location: {npcBase.RowId}"); }
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.SpecialShop));
                //var gilShop = Service.DataManager.GetExcelSheet<GilShop>()?.GetRow(rowRef.RowId);
                //list.Add((gilShop.Value.Name.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                List<uint> blacklist_npcs = [1006004, 1006005, 1006006]; //Calamatiy Salvager
                blacklist_npcs.Add(1028254); // Ironworks Vendor
                blacklist_npcs.AddRange(new List<uint> { 1019797, 1026074, 1028250, 1034489, 1036894, 1042833, 1044880, 1046491 }); // Campaign Attendant
                blacklist_npcs.AddRange(new List<uint> { 1016294, 1016296 }); // Triple Triad Trader
                blacklist_npcs.Add(1031691); //Enie
                List<uint> blacklist_shops = [1770595, 1770645, 1770729];
                if (!blacklist_npcs.Contains(npcBase.RowId) && !blacklist_shops.Contains(rowRef.RowId))
                {
                    // if (rowRef.RowId == 1770477) DuoLog.Information("Found");
                    Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.SpecialShop, Location = loc });
                }
            }
            else if (rowRef.Is<InclusionShop>())
            {
                //Shop newShop = new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.SpecialShop, Location = loc };
                //DuoLog.Information($"newShop");
                //Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.SpecialShop, Location = loc });
            }
            else if (rowRef.Is<CustomTalk>())
            {
                var customTalk = Service.DataManager.GetExcelSheet<CustomTalk>()?.GetRow(rowRef.RowId);
                //if (customTalk.Value.SpecialLinks.RowId != 0)
                //{
                //    PluginLog.Information($"customTalk.Value.SpecialLinks: {customTalk.Value.SpecialLinks.RowId}");
                //    if (customTalk.Value.SpecialLinks.Is<CustomTalk>()) PluginLog.Information($"CustomTalk");
                //    if (customTalk.Value.SpecialLinks.Is<SpecialShop>()) PluginLog.Information($"SpecialShop");
                //    if (customTalk.Value.SpecialLinks.Is<InclusionShop>()) PluginLog.Information($"InclusionShop");
                //    if (customTalk.Value.SpecialLinks.Is<FateShop>()) PluginLog.Information($"FateShop");
                //    if (customTalk.Value.SpecialLinks.Is<FccShop>()) PluginLog.Information($"FccShop");
                //    if (customTalk.Value.SpecialLinks.Is<TopicSelect>()) PluginLog.Information($"TopicSelect");
                //    if (customTalk.Value.SpecialLinks.Is<PreHandler>()) PluginLog.Information($"PreHandler");
                //}
                
                EvalulateRowRef(npcBase, customTalk.Value.SpecialLinks);
                foreach (var scriptStruct in customTalk.Value.Script)
                {
                    if (scriptStruct.ScriptArg == 0)
                    {
                        continue;
                    }
                    //PluginLog.Information($"scriptStruct.ScriptArg: {scriptStruct.ScriptArg}");
                    var customTalkRef = RowRef.GetFirstValidRowOrUntyped(
                        Service.DataManager.Excel,
                        scriptStruct.ScriptArg,
                        customTalkTypes,
                        customTalkTypeHash,
                        Lumina.Data.Language.English);
                    //PluginLog.Information($"customTalkRef: {customTalkRef}");
                    //if(customTalkRef.Is<SpecialShop>()) PluginLog.Information($"SpecialShop {customTalkRef.RowId}");
                    //if (customTalkRef.Is<InclusionShop>()) PluginLog.Information($"InclusionShop");
                    //if (customTalkRef.Is<FateShop>()) PluginLog.Information($"FateShop {customTalkRef.RowId}");
                    //if (customTalkRef.Is<FccShop>()) PluginLog.Information($"FccShop {customTalkRef.RowId}");
                    EvalulateRowRef(npcBase, customTalkRef);
                }
            }
            if (rowRef.Is<TopicSelect>())
            {
                var topicSelect = Service.DataManager.GetExcelSheet<TopicSelect>()?.GetRow(rowRef.RowId);
                foreach (var topicShop in topicSelect.Value.Shop)
                {
                    EvalulateRowRef(npcBase, topicShop);
                }
            }
            else if (rowRef.Is<PreHandler>())
            {
                var preHandler = Service.DataManager.GetExcelSheet<PreHandler>()?.GetRow(rowRef.RowId);
                EvalulateRowRef(npcBase, preHandler.Value.Target);
            }
        }
        public static List<uint> ListRange(int start, int end)
        {
            return new List<uint>(Enumerable.Range(start, end - start + 1).Select(i => (uint)i));
        }
    }
}
