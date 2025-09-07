using CurrencySpender.Classes;
using CurrencySpender.Data;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Exd;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace CurrencySpender.Helpers
{
    public class ItemHelper
    {
        public static bool Debug = false;

        private static readonly Dictionary<uint, List<uint>> Containers = new()
        {
            // Bronze Triad Card 
            { 10128, new List<uint> { 9782, 9809, 9797, 9796, 9779, 16762, 9783, 16760, 16759, 9776, 9798, 9775, 9795, 16765, 15621 } },
            // Silver Triad Card 
            { 10129, new List<uint> { 9785, 14199, 9813, 9814, 9811, 9786, 9788, 9828, 9827, 9792, 9787, 9790, 9812, 9821 } },
            // Gold Triad Card
            { 10130, new List<uint> { 9800, 9829, 9805, 14192, 9837, 9825, 9836, 9799, 9801, 9824, 9838, 9826, 9822, 9839, 9847 } },
            // Mythril Triad Card 
            { 13380, new List<uint> { 9843, 14193, 13368, 9810, 9823, 9841, 13372, 9844, 13367 } },
            // Imperial Triad Card 
            { 17702, new List<uint> { 17686, 16775, 17681, 17682, 16774, 13378 } },
            // Dream Triad Card    
            { 28652, new List<uint> { 28661, 26767, 28657, 28653, 28655, 26772, 28658, 28660, 26765, 26768, 26766 } },
            // Platinum Triad Card 
            { 10077, new List<uint> { 9830, 9842, 9840, 14208, 15872, 9828, 9851, 9831, 9834, 9826, 9822, 9848 } },
            // Materiel Container 3.0 
            { 36635, new List<uint>
                {
                    9350, 12051, 6187, 15441, 6175, 7564, 6186, 6203, 6177, 17525, 15440, 14098, 6003, 12055, 6199, 6205,
                    16570, 16568, 6189, 15447, 8193, 9347, 14103, 12054, 8194, 12061, 6191, 12069, 13279, 6179, 12058, 13283,
                    12056, 9348, 7568, 6004, 8196, 8201, 7566, 10071, 6204, 6173, 14100, 9349, 8200, 8205, 16564, 8202, 12052,
                    12057, 13275, 7559, 6192, 16572, 6208, 6195, 12062, 7567, 6188, 6174, 8199, 6185, 8195, 12053, 12049, 6005,
                    6213, 6200, 6190, 16573, 17527, 14093, 13284, 13276, 14095, 6214, 15436, 15437, 14094, 6184, 14083, 6183, 6198,
                    8192, 6209, 6178
                } },
            // Materiel Container 4.0 
            { 36636, new List<uint>
                {
                    24902, 21921, 21063, 20529, 20530, 21920, 24002, 20524, 24635, 23027, 24001, 23023, 20533, 24219, 24630, 21052,
                    20542, 24903, 20538, 21064, 20541, 21058, 20536, 23032, 23998, 20525, 21916, 20531, 21193, 23989, 24634, 21059,
                    21922, 21919, 20528, 21911, 20547, 20539, 24000, 21918, 21055, 20544, 20546, 21915, 21060, 21917, 20537, 21057,
                    23030, 21065, 20545, 23028, 24639, 23036, 24640
                } }
        };

        public static Dictionary<uint, (uint, uint)> ContainerUnlocked = new()
        {
            { 10128, (0,0) },
            { 10129, (0,0) },
            { 10130, (0,0) },
            { 13380, (0,0) },
            { 17702, (0,0) },
            { 28652, (0,0) },
            { 10077, (0,0) },
            { 36635, (0,0) },
            { 36636, (0,0) },
        };

        public static uint GetItemIDFromString(string arg)
        {
            var ret = Service.DataManager.GetExcelSheet<Item>().FirstOr0(x => x.Name == arg);
            if (ret.RowId != 0) { return ret.RowId; }
            return 0;
        }

        public static unsafe bool IsUnlocked(uint id)
        {
            Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(id);
            //if(item.RowId == 44936) PluginLog.Debug($"{item.Name.ExtractText()} - {item.RowId}");
            if (Containers.ContainsKey(id))
            {
                if (ContainerUnlocked.TryGetValue(id, out (uint, uint) tuple))
                {
                    if (tuple.Item2 > tuple.Item1) return false;
                    if (tuple.Item2 != 0 && tuple.Item1 != 0 && tuple.Item2 == tuple.Item1) return true;
                }
                uint unlocked = 0;
                uint max = 0;
                if (Containers.TryGetValue(id, out List<uint>? values))
                {
                    foreach (var value in values)
                    {
                        if (IsUnlocked(value)) unlocked++;
                        else PluginLog.Debug($"{value} not unlocked");
                        max++;
                    }
                }
                ContainerUnlocked[id] = (unlocked, max);
                PluginLog.Debug($"{item.Name.ExtractText()} - {item.RowId} - {unlocked}/{max}");
                if (max == unlocked) return true;
                return false;
            }
            if (item.ItemUICategory.RowId == 94 && item.Name.ExtractText().Contains("Faded"))
            {
                if (Debug) PluginLog.Verbose("Item is Faded Copy of Orchestration Roll");
                var new_name = item.Name.ExtractText().Replace("Faded Copy of ", "") + " Orchestrion Roll";
                var rowId = GetItemIDFromString(new_name);
                if (Debug) PluginLog.Verbose("new_name: '" + new_name + "'");
                if (Debug) PluginLog.Verbose("row: " + rowId.ToString());
                if (rowId != 0)
                {
                    var new_item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(rowId);
                    var new_additionalData = new_item.AdditionalData.RowId;
                    return UIState.Instance()->PlayerState.IsOrchestrionRollUnlocked(new_additionalData);
                }
            }

            if (item.ItemAction.RowId == 0)
                return false;

            switch ((ItemActionType)item.ItemAction.Value.Type)
            {
                case ItemActionType.Companion:
                    return UIState.Instance()->IsCompanionUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.BuddyEquip:
                    return UIState.Instance()->Buddy.CompanionInfo.IsBuddyEquipUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.Mount:
                    return PlayerState.Instance()->IsMountUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.SecretRecipeBook:
                    return PlayerState.Instance()->IsSecretRecipeBookUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.UnlockLink:
                    // PluginLog.Information($"{item.Name.ExtractText()} - {item.ItemAction.RowId} - {(ItemActionType)item.ItemAction.Value.Type} - {UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0])}");
                    return UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.TripleTriadCard when item.AdditionalData.Is<TripleTriadCard>():
                    return UIState.Instance()->IsTripleTriadCardUnlocked((ushort)item.AdditionalData.RowId);

                case ItemActionType.FolkloreTome:
                    return PlayerState.Instance()->IsFolkloreBookUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.OrchestrionRoll when item.AdditionalData.Is<Orchestrion>():
                    return PlayerState.Instance()->IsOrchestrionRollUnlocked(item.AdditionalData.RowId);

                case ItemActionType.FramersKit:
                    return PlayerState.Instance()->IsFramersKitUnlocked(item.AdditionalData.RowId);

                case ItemActionType.Ornament:
                    return PlayerState.Instance()->IsOrnamentUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.Glasses:
                    return PlayerState.Instance()->IsGlassesUnlocked((ushort)item.AdditionalData.RowId);
            }

            var row = ExdModule.GetItemRowById(item.RowId);
            return row != null && UIState.Instance()->IsItemActionUnlocked(row) == 1;
        }
        public enum ItemActionType : ushort
        {
            Companion = 853,
            BuddyEquip = 1013,
            Mount = 1322,
            SecretRecipeBook = 2136,
            UnlockLink = 2633, // riding maps, blu totems, emotes/dances, hairstyles
            TripleTriadCard = 3357,
            FolkloreTome = 4107,
            OrchestrionRoll = 25183,
            FramersKit = 29459,
            // FieldNotes = 19743, // bozjan field notes (server side, but cached)
            Ornament = 20086,
            Glasses = 37312,
            CompanySealVouchers = 41120, // can use = is in grand company, is unlocked = always false
        }
        public static ItemType GetItemTypes(uint id)
        {
            Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(id);
            if (item.RowId == 21072)
            {
                return ItemType.Venture;
            }
            var cat = item.ItemUICategory.RowId;
            var name = item.Name.ExtractText();
            var untradable = item.IsUntradable;
            ItemType curType = ItemType.None;
            if (Containers.ContainsKey(id)) curType |= ItemType.Collectable;
            //if (item_.ItemAction.RowId != 0) curType |= ItemType.Collectable;
            if (P.Currencies.Where(cur => cur.ItemId == item.RowId).ToList().Count > 0) curType |= ItemType.Currency;
            if(name.Contains("Ballroom Etiquette") || name.Contains("Framer's Kit") || name.Contains("Battlefield Etiquette") ||
                name.Contains("The Faces We Wear") || name.Contains("Modern Aesthetics") || name.Contains("Maxims of Mahjong"))
            {
                curType |= ItemType.Collectable;
            }
            if(cat == 63)
            {
                if(name.Contains("Barding") || item.ItemAction.Value.Type == 1322 || item.ItemAction.Value.Type == 29459 ||
                    item.ItemAction.Value.Type == 2633) //2633 Riding Map
                {
                    curType |= ItemType.Collectable;
                }
            }
            if(cat == 81 || cat == 86 || cat == 94)
            {
                curType |= ItemType.Collectable;
            }
            if(!untradable)
                curType |= ItemType.Tradeable;
            return curType;
        }
        public static CollectableType GetCollectableType(RowRef<Item> item, ItemType item_types)
        {
            if (!item_types.HasFlag(ItemType.Collectable)) { return CollectableType.None; }
            var cat = item.Value.ItemUICategory.RowId;
            var name = item.Value.Name.ExtractText();
            if (Containers.ContainsKey(item.RowId)) return CollectableType.Container;
            if (name.Contains("Ballroom Etiquette") || name.Contains("Battlefield Etiquette"))
            {
                return CollectableType.Scroll;
            }
            if (name.Contains("Framer's Kit")) return CollectableType.FramersKit;
            if (name.Contains("Maxims of Mahjong")) return CollectableType.Mahjong;
            if (name.Contains("The Faces We Wear")) return CollectableType.Facewear;
            if (name.Contains("Modern Aesthetics")) return CollectableType.Hairstyle;
            if (cat == 63)
            {
                if (name.Contains("Barding")) return CollectableType.Barding;
                if (item.Value.ItemAction.Value.Type == 1322) return CollectableType.Mount;
                if (item.Value.ItemAction.Value.Type == 29459) return CollectableType.FramersKit;
                if (item.Value.ItemAction.Value.Type == 2633) return CollectableType.RidingMap;
            }
            if (cat == 81) return CollectableType.Minion;
            if (cat == 86) return CollectableType.TTCard;
            if (cat == 94) return CollectableType.Scroll;
            if (Debug) DuoLog.Debug("Collectable Type not found!");
            return CollectableType.None;
        }
    }
}
