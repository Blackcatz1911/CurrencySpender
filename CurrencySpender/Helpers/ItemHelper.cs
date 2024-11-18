using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencySpender.Helpers
{
    public class ItemHelper
    {
        public static unsafe bool CheckUnlockStatus(uint id)
        {
            Item item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(id);
            Service.Log.Verbose("item.RowId: " + item.RowId.ToString());
            if (item.RowId == 0)
                return false;

            Service.Log.Verbose("item.ItemAction.RowId: " + item.ItemAction.RowId.ToString());
            //if (item.ItemAction.RowId == 0)
            //    return false;

            var action = item.ItemAction.Value;
            var additionalData = item.AdditionalData.RowId;
            var instance = UIState.Instance();
            if (instance->PlayerState.IsOrchestrionRollUnlocked(additionalData)) return true;
            Service.Log.Verbose("IsOrchestrionRollUnlocked: " + instance->PlayerState.IsOrchestrionRollUnlocked(additionalData).ToString());
            return action.Type switch
            {
                1447 => instance->PlayerState.IsOrchestrionRollUnlocked(additionalData),
                1322 => instance->PlayerState.IsMountUnlocked(action.Data[0]),
                853 => instance->IsCompanionUnlocked(action.Data[0]),
                _ => false
            };
        }
    }
}
