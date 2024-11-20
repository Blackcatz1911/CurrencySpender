using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Helpers
{
    public class ItemHelper
    {
        static List<uint> hairstyles = new List<uint>();
        public static unsafe bool CheckUnlockStatus(uint id)
        {
            var item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(id);
            Service.Log.Verbose("---");
            Service.Log.Verbose("item.RowId: " + item.RowId.ToString());
            var ItemUICategory = Service.DataManager.GetExcelSheet<ItemUICategory>()!.GetRow(item.ItemUICategory.RowId);
            Service.Log.Verbose("ItemUICategory: " + item.ItemUICategory.RowId + " : " + ItemUICategory.Name.ToString());
            if (item.RowId == 0)
                return false;

            Service.Log.Verbose("item.ItemAction.RowId: " + item.ItemAction.RowId.ToString());
            //Service.Log.Verbose("item.ItemAction: " + item.ItemAction.);
            //if (item.ItemAction.RowId == 0)
            //    return false;

            var action = item.ItemAction.Value;
            var additionalData = item.AdditionalData.RowId;
            var instance = UIState.Instance();
            //var ItemUICategory = Service.DataManager.GetExcelSheet<HairMakeType>()!.GetRow(item.ItemUICategory.RowId);
            //Service.Log.Verbose("Test: " + instance->IsTripleTriadCardUnlocked());

            // Orchestration Rolls
            if (item.ItemUICategory.RowId == 94 && instance->PlayerState.IsOrchestrionRollUnlocked(additionalData))
            {
                Service.Log.Verbose("IsOrchestrionRollUnlocked: " + instance->PlayerState.IsOrchestrionRollUnlocked(additionalData).ToString());
                return true;
            }

            // Faded Copy of Orchestration Rolls
            if (item.ItemUICategory.RowId == 94 && item.Name.ExtractText().Contains("Faded"))
            {
                Service.Log.Verbose("Item is Faded Copy of Orchestration Roll");
                var new_name = item.Name.ExtractText().Replace("Faded Copy of ", "") + " Orchestrion Roll";
                var row = GetItemIDFromString(new_name);
                Service.Log.Verbose("new_name: '"+ new_name+"'");
                Service.Log.Verbose("row: " + row.ToString());
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
                Service.Log.Verbose("IsEmoteUnlocked: " + instance->IsEmoteUnlocked(action.Data[2]));
                return true;
            }

            // Hairstyles
            if (item.ItemUICategory.RowId == 61 && hairstyles.Contains(item.RowId))
            {
                Service.Log.Verbose("IsHairstyleUnlocked: " + hairstyles.Contains(item.RowId));
                return true;
            }

            // TT Card
            if (item.ItemUICategory.RowId == 86 && instance->IsTripleTriadCardUnlocked(action.Data[0]))
            {
                Service.Log.Verbose("IsTripleTriadCardUnlocked: " + instance->IsTripleTriadCardUnlocked(action.Data[0]));
                return true;
            }

            // Minion
            if (item.ItemUICategory.RowId == 81 && instance->IsCompanionUnlocked(action.Data[0]))
            {
                Service.Log.Verbose("IsCompanionUnlocked: " + instance->IsCompanionUnlocked(action.Data[0]));
                return true;
            }

            // Barding
            if (item.ItemUICategory.RowId == 63 && item.Name.ExtractText().Contains("Barding") && instance->Buddy.CompanionInfo.IsBuddyEquipUnlocked(action.Data[0]))
            {
                Service.Log.Verbose("IsBuddyEquipUnlocked: " + instance->Buddy.CompanionInfo.IsBuddyEquipUnlocked(action.Data[0]));
                return true;
            }

            //Mount
            if (item.ItemUICategory.RowId == 63 && instance->PlayerState.IsMountUnlocked(action.Data[0]))
            {
                Service.Log.Verbose("IsMountUnlocked: " + instance->PlayerState.IsMountUnlocked(action.Data[0]));
                return true;
            }

            Service.Log.Verbose("---");
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
    }
}
