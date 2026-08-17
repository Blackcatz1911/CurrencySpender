using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Managers
{
    public static unsafe class TeleportManager
    {
        public static bool Teleport(TeleportInfo info)
        {
            if (Service.ObjectTable.LocalPlayer == null)
            {
                PluginLog.Debug("Teleport skipped: LocalPlayer is null");
                return false;
            }
            var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, 5);
            if (status != 0)
            {
                var msg = GetLogMessage(status);
                PluginLog.Debug($"Teleport skipped: status={status} ({msg}), aetheryte={info.AetheryteId}/{info.SubIndex}");
                return false;
            }

            if (Service.ObjectTable.LocalPlayer.CurrentWorld.RowId != Service.ObjectTable.LocalPlayer.HomeWorld.RowId)
            {
                if (AetheryteManager.IsHousingAetheryte(info.AetheryteId, info.Plot, info.Ward, info.SubIndex))
                {
                    //Service.LogChat($"Unable to Teleport to {AetheryteManager.GetAetheryteName(info)} while visiting other Worlds.", true);
                    PluginLog.Debug("Teleport skipped: housing aetheryte while visiting another world");
                    return false;
                }
            }

            var result = Telepo.Instance()->Teleport(info.AetheryteId, info.SubIndex);
            PluginLog.Debug($"Teleport to aetheryte {info.AetheryteId}/{info.SubIndex}: result={result}");
            return result;
        }


        private static string GetLogMessage(uint id)
        {
            var sheet = Service.DataManager.GetExcelSheet<LogMessage>();
            var row = sheet.GetRow(id);
            return row.Text.ToString();
        }
    }
}
