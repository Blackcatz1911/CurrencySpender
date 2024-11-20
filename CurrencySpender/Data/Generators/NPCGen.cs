using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using System.Xml.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using ECommons;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel;
using FFXIVClientStructs.FFXIV.Client.UI;
using CurrencySpender.Classes;
using System.Collections.Immutable;

namespace CurrencySpender.Data.Generators
{
    public static class NPCGen
    {
        public static List<Shop> shops = new List<Shop> ();

        

        public static void init()
        {
            foreach (ENpcBase npc in Service.DataManager.GetExcelSheet<ENpcBase>())
            {
                foreach (var variable in npc.ENpcData)
                {
                    EvalulateRowRef(npc, variable);
                }
            }

            //foreach (var shop in shops)
            //{
            var shop_ = Service.DataManager.GetExcelSheet<SpecialShop>().GetRow(1769730);
            var itemCol = shop_.Item;
            foreach (var itemCol_ in itemCol)
            {
                //Service.Log.Verbose(itemCol_.ItemCosts.ToString());
                foreach (var item in itemCol_.ItemCosts)
                {
                    Service.Log.Verbose(item.ItemCost.Value.Name.ToString());
                    if (item.ItemCost.RowId == 0) continue;
                    //Service.Log.Verbose(item.CurrencyCost.ToString());
                    //var item_ = Service.DataManager.GetExcelSheet<Item>().GetRow(item.ItemCost.RowId);
                    //Service.Log.Verbose(item_.Name.ToString());
                }
                foreach (var item in itemCol_.ReceiveItems)
                {
                    if (item.Item.RowId == 0) continue;
                    var item_ = Service.DataManager.GetExcelSheet<Item>().GetRow(item.Item.RowId);
                    //Service.Log.Verbose(item_.Name.ToString());
                    //Service.Log.Verbose(item.Item.RowId.ToString());
                    //Service.Log.Verbose("IsCollectable: "+item_.IsCollectable.ToString());
                    //Service.Log.Verbose("IsUntradable: " + item_.IsUntradable.ToString());
                    //Service.Log.Verbose("---");
                    //break;
                }
                //break;
            }
            //Service.Log.Verbose(itemCol.Count.ToString());
            //Service.Log.Verbose(shop_.Item.ToString());
            //var items = shop_.Item.ToArray;
            //foreach (var item in items)
            //{
            //    foreach (var item in items[re])
            //        Service.Log.Verbose(item);
            //    break;
            //}
        }
        public static void EvalulateRowRef(ENpcBase npcBase, RowRef rowRef)
        {
            if (rowRef.Is<FccShop>())
            {
                shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.FccShop });
            }
            else if (rowRef.Is<GCShop>())
            {
                //var shop = Service.DataManager.GetExcelSheet<GCShop>()?.GetRow(rowRef.RowId);
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GCShop));
                shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.GCShop });
            }
            else if (rowRef.Is<GilShop>())
            {
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                //var gilShop = Service.DataManager.GetExcelSheet<GilShop>()?.GetRow(rowRef.RowId);
                //list.Add((gilShop.Value.Name.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                // shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.GilShop });
            }
            else if (rowRef.Is<SpecialShop>())
            {
                //var npc = Service.DataManager.GetExcelSheet<ENpcResident>()?.GetRow(npcBase.RowId);
                //list.Add((npc.Value.Singular.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.SpecialShop));
                //var gilShop = Service.DataManager.GetExcelSheet<GilShop>()?.GetRow(rowRef.RowId);
                //list.Add((gilShop.Value.Name.ExtractText(), rowRef.RowId, npcBase.RowId, ShopType.GilShop));
                List<uint> blacklist = [1006004, 1006005, 1006006]; //Calamatiy Salvager
                if (!blacklist.Contains(npcBase.RowId))
                    shops.Add(new Shop { ShopId = rowRef.RowId, NpcId = npcBase.RowId, Type = ShopType.SpecialShop });
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
