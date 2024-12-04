using Lumina.Excel.Sheets;
using Lumina.Excel;
using CurrencySpender.Classes;
using System.Collections.Generic;
using System.Collections;
using Serilog;

namespace CurrencySpender.Data
{
    public static class ShopGen
    {
        public static List<uint> fateShops = new List<uint>();

        public static void init()
        {
            PluginLog.Verbose("ShopGen init");
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
            foreach (var shop in Service.DataManager.GetExcelSheet<SpecialShop>())
            {
                var item_cost = shop.Item.First().ItemCosts.First().ItemCost.RowId;
                if (C.Currencies.Where(cur => cur.Enabled && cur.ItemId == item_cost).ToList().Count > 0)
                {
                    var cur = ItemGen.ConvertCurrencyId(shop.RowId, shop.Item.First().ReceiveItems.First().Item.RowId, shop.UseCurrencyType);
                    if (Generator.shops.Where(s => s.ShopId == shop.RowId).ToList().Count == 0)
                    {
                        uint NpcId = 0;
                        Dictionary<List<uint>, uint> npcMapping = new()
                        {
                            { new List<uint> { 1769577, 1769578 }, 1012225 }, // Ardolain
                            { new List<uint> { 1769743, 1769744, 1770537 }, 1018655 }, // Disreputable Priest
                            { new List<uint>(Enumerable.Range(1770551, 1770589).Select(i => (uint)i)), 1005244 }, // Mark Quartermaster

                            { new List<uint> { 1769957 }, 1027998 }, // Gramsol
                            { new List<uint> { 1769958 }, 1027538 }, // Pedronille
                            { new List<uint> { 1769959 }, 1027385 }, // Siulmet
                            { new List<uint> { 1769960 }, 1027497 }, // Zumutt
                            { new List<uint> { 1769961 }, 1027892 }, // Halden
                            { new List<uint> { 1769962 }, 1027665 }, // Sul Lad
                            { new List<uint> { 1769963 }, 1027709 }, // Nacille
                            { new List<uint> { 1769964 }, 1027766 }, // Goushs Ooan
                        };

                        NpcId = npcMapping.FirstOrDefault(kv => kv.Key.Contains(shop.RowId)).Value;

                        Location loc = Location.GetLocation(NpcId);
                        if (NpcId != 0)
                        {
                            List<uint> gemstones = [1027385, 1027497, 1027892, 1027665, 1027709, 1027766, 1027998, 1027538];
                            if (gemstones.Contains(NpcId)) {
                                Generator.shops.Add(new Shop { ShopId = shop.RowId, NpcId = NpcId, Type = ShopType.FateShop, Location = loc });
                            }
                            else
                            {
                                Generator.shops.Add(new Shop { ShopId = shop.RowId, NpcId = NpcId, Type = ShopType.SpecialShop, Location = loc });
                            }
                        }
                        if (NpcId == 0)
                        {
                            PluginLog.Verbose($"FOUND: {shop.RowId}-{shop.Name}-{shop.Item.First().ItemCosts.First().ItemCost.Value.Name}-{cur}");
                            PluginLog.Verbose($"Item: {shop.Item.First().ReceiveItems.First().Item.RowId}-{shop.Item.First().ReceiveItems.First().Item.Value.Name}");
                        }
                    }
                }
            }
            var shops = Generator.shops
                .Where(shop => shop.RequiredLevel != null)
                .ToList();
            foreach (var shop in shops)
            {
                if(shop.RequiredLevel >= 0 && shop.RequiredLevel != 99)
                {
                    if (PlayerHelper.SharedFateRanks.ContainsKey(shop.Location.TerritoryId))
                        shop.CurrentLevel = PlayerHelper.SharedFateRanks[shop.Location.TerritoryId];
                }
                //PluginLog.Verbose(shop.ToString());
            }
            PluginLog.Verbose("ShopGen init finished");
        }
        public static void EvalulateRowRef(ENpcBase npcBase, RowRef rowRef)
        {
            var npcName = Service.DataManager.GetExcelSheet<ENpcResident>()!.GetRow(npcBase.RowId).Singular.ExtractText();
            //List<string> names = ["Tepli", "Kunuhali", "Rral Wuruq", "Mitepe", "Toashana", "Clerk PX-0029", "Kajeel Ja", "Beryl"];
            //if (names.Contains(npcName))
            //{
            //    PluginLog.Verbose($"FOUND: {npcBase.RowId}-{npcName}");
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
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.SpecialShop));
                //var gilShop = Service.DataManager.GetExcelSheet<GilShop>()?.GetRow(rowRef.RowId);
                //list.Add((gilShop.Value.Name.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                List<uint> blacklist = [1006004, 1006005, 1006006]; //Calamatiy Salvager
                if (!blacklist.Contains(npcBase.RowId))
                    Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.SpecialShop, Location = loc });
            }
            else if (rowRef.Is<CustomTalk>() && fateShops.Contains(npcBase.RowId))
            {
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.SpecialShop));
                //var gilShop = Service.DataManager.GetExcelSheet<GilShop>()?.GetRow(rowRef.RowId);
                //list.Add((gilShop.Value.Name.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                //Generator.shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.FateShop, Location = loc });
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
    }
}
