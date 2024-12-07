using CurrencySpender.Classes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Exd;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Helpers
{
    public class ItemHelper
    {
        public static bool Debug = false;
        internal static List<uint> hairstyles = new List<uint>();
        public static unsafe bool CheckUnlockStatus(uint id)
        {
            //return false;
            var item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(id);
            if (Debug) PluginLog.Verbose("---");
            if (Debug) PluginLog.Verbose("item.RowId: " + item.RowId.ToString());
            var ItemUICategory = Service.DataManager.GetExcelSheet<ItemUICategory>()!.GetRow(item.ItemUICategory.RowId);
            if(Debug) PluginLog.Verbose("ItemUICategory: " + item.ItemUICategory.RowId + " : " + ItemUICategory.Name.ToString());
            if (item.RowId == 0)
                return false;

            if (Debug) PluginLog.Verbose("item.ItemAction.RowId: " + item.ItemAction.RowId.ToString());
            //PluginLog.Verbose("item.ItemAction: " + item.ItemAction.);
            //if (item.ItemAction.RowId == 0)
            //    return false;

            var action = item.ItemAction.Value;
            var additionalData = item.AdditionalData.RowId;
            var instance = UIState.Instance();
            //var ItemUICategory = Service.DataManager.GetExcelSheet<HairMakeType>()!.GetRow(item.ItemUICategory.RowId);
            //PluginLog.Verbose("Test: " + instance->IsTripleTriadCardUnlocked());

            // Orchestration Rolls
            if (item.ItemUICategory.RowId == 94 && instance->PlayerState.IsOrchestrionRollUnlocked(additionalData))
            {
                if (Debug) PluginLog.Verbose("IsOrchestrionRollUnlocked: " + instance->PlayerState.IsOrchestrionRollUnlocked(additionalData).ToString());
                return true;
            }

            // Faded Copy of Orchestration Rolls
            if (item.ItemUICategory.RowId == 94 && item.Name.ExtractText().Contains("Faded"))
            {
                if (Debug) PluginLog.Verbose("Item is Faded Copy of Orchestration Roll");
                var new_name = item.Name.ExtractText().Replace("Faded Copy of ", "") + " Orchestrion Roll";
                var row = GetItemIDFromString(new_name);
                if (Debug) PluginLog.Verbose("new_name: '"+ new_name+"'");
                if (Debug) PluginLog.Verbose("row: " + row.ToString());
                if (row != 0)
                {
                    var new_item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(row);
                    var new_additionalData = new_item.AdditionalData.RowId;
                    return instance->PlayerState.IsOrchestrionRollUnlocked(new_additionalData);
                }
            }

            // Emotes
            if (item.ItemUICategory.RowId == 61 && instance->IsEmoteUnlocked(action.Data[2]))
            {
                if (Debug) PluginLog.Verbose("IsEmoteUnlocked: " + instance->IsEmoteUnlocked(action.Data[2]));
                return true;
            }

            // Hairstyles
            if (item.ItemUICategory.RowId == 61 && hairstyles.Contains(item.RowId))
            {
                if (Debug) PluginLog.Verbose("IsHairstyleUnlocked: " + hairstyles.Contains(item.RowId));
                return true;
            }

            //Framer's Kit
            if (item.ItemUICategory.RowId == 61 && item.Name.ExtractText().Contains("Framer's Kit") && instance->PlayerState.IsFramersKitUnlocked(additionalData))
            {
                if (Debug) PluginLog.Verbose("IsFramersKitUnlocked: " + instance->PlayerState.IsFramersKitUnlocked(additionalData));
                return true;
            }

            // TT Card
            if (item.ItemUICategory.RowId == 86 && instance->IsTripleTriadCardUnlocked(action.Data[0]))
            {
                if (Debug) PluginLog.Verbose("IsTripleTriadCardUnlocked: " + instance->IsTripleTriadCardUnlocked(action.Data[0]));
                return true;
            }

            // Minion
            if (item.ItemUICategory.RowId == 81 && instance->IsCompanionUnlocked(action.Data[0]))
            {
                if (Debug) PluginLog.Verbose("IsCompanionUnlocked: " + instance->IsCompanionUnlocked(action.Data[0]));
                return true;
            }

            // Barding
            if (item.ItemUICategory.RowId == 63 && item.Name.ExtractText().Contains("Barding") && instance->Buddy.CompanionInfo.IsBuddyEquipUnlocked(action.Data[0]))
            {
                if (Debug) PluginLog.Verbose("IsBuddyEquipUnlocked: " + instance->Buddy.CompanionInfo.IsBuddyEquipUnlocked(action.Data[0]));
                return true;
            }

            //Mount
            if (item.ItemUICategory.RowId == 63 && action.Type == 1322 && instance->PlayerState.IsMountUnlocked(action.Data[0]))
            {
                if (Debug) PluginLog.Verbose("IsMountUnlocked: " + instance->PlayerState.IsMountUnlocked(action.Data[0]));
                return true;
            }

            if (Debug) PluginLog.Verbose("---");
            return false;
        }
        public static uint GetItemIDFromString(string arg)
        {
            var ret = Service.DataManager.GetExcelSheet<Item>().FirstOr0(x => x.Name == arg);
            if (ret.RowId != 0) { return ret.RowId; }
            return 0;
        }

        public static void initHairStyles()
        {
            var yas = Service.DataManager.GetExcelSheet<CharaMakeCustomize>().AsParallel()
            //ExcelCache<CharaMakeCustomize>.GetSheet().AsParallel()
            .Where(entry => entry.IsPurchasable && (entry.RowId < 100 || (entry.RowId >= 2050 && entry.RowId < 2100)))
            //.Select(entry => (ICollectible)CollectibleCache<HairstyleCollectible, CharaMakeCustomize>.Instance.GetObject(entry))
            //.OrderByDescending(c => c.IsFavorite())
            //.OrderByDescending(c => c)
            .ToList();
            foreach (var item in yas)
            {
                hairstyles.Add(item.HintItem.RowId);
            }
        }

        public static unsafe bool IsUnlocked(uint id)
        {
            Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(id);
            if (item.ItemAction.RowId == 0)
                return false;

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
                    return UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.TripleTriadCard when item.AdditionalData.Is<TripleTriadCard>():
                    return UIState.Instance()->IsTripleTriadCardUnlocked((ushort)item.AdditionalData.RowId);

                case ItemActionType.FolkloreTome:
                    return PlayerState.Instance()->IsFolkloreBookUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.OrchestrionRoll when item.AdditionalData.Is<Orchestrion>():
                    return PlayerState.Instance()->IsOrchestrionRollUnlocked(item.AdditionalData.RowId);

                case ItemActionType.FramersKit:
                    return PlayerState.Instance()->IsFramersKitUnlocked(item.ItemAction.Value.Data[0]);

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
        public static ItemType GetItemTypes(RowRef<Item> item)
        {
            if (item.RowId == 21072) return ItemType.Venture;
            var cat = item.Value.ItemUICategory.RowId;
            var name = item.Value.Name.ExtractText();
            var untradable = item.Value.IsUntradable;
            ItemType curType = ItemType.None;
            if(C.Currencies.Where(cur => cur.ItemId == item.RowId).ToList().Count > 0) curType |= ItemType.Currency;
            if (cat == 61)
            {
                if(name.Contains("Ballroom Etiquette") || name.Contains("Ballroom Etiquette") || name.Contains("Framer's Kit"))
                {
                    curType |= ItemType.Collectable;
                }
            }
            if(cat == 63)
            {
                if(name.Contains("Barding") || item.Value.ItemAction.Value.Type == 1322 || item.Value.ItemAction.Value.Type == 29459)// ||
                //    item.Value.ItemAction.Value.Type == 2633)
                {
                    //2633 Riding Map
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
    }
}
