using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Managers
{
    public static unsafe class TeleportManager
    {
        public static bool Teleport(TeleportInfo info)
        {
            if (Service.ClientState.LocalPlayer == null)
                return false;
            var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, 5);
            if (status != 0)
            {
                var msg = GetLogMessage(status);
                return false;
            }

            if (Service.ClientState.LocalPlayer.CurrentWorld.RowId != Service.ClientState.LocalPlayer.HomeWorld.RowId)
            {
                if (AetheryteManager.IsHousingAetheryte(info.AetheryteId, info.Plot, info.Ward, info.SubIndex))
                {
                    //Service.LogChat($"Unable to Teleport to {AetheryteManager.GetAetheryteName(info)} while visiting other Worlds.", true);
                    return false;
                }
            }

            return Telepo.Instance()->Teleport(info.AetheryteId, info.SubIndex);
        }


        private static string GetLogMessage(uint id)
        {
            var sheet = Service.DataManager.GetExcelSheet<LogMessage>();
            if (sheet == null) return string.Empty;
            var row = sheet.GetRow(id);
            return row.Text.ToString();
        }
    }
}
