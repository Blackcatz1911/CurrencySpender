using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace CurrencySpender.Services;

internal class HighlightNpc : IDisposable
{
    private uint targetNpcDataId = 0;
    private DateTime _lastUpdateTime = DateTime.Now;

    public HighlightNpc()
    {
        Service.Framework.Update += Framework_OnUpdate;
    }

    private void Framework_OnUpdate(IFramework framework)
    {
        //we want to update every 500 ms
        if (DateTime.Now - _lastUpdateTime <= TimeSpan.FromMilliseconds(500))
        {
            return;
        }

        _lastUpdateTime = DateTime.Now;

        if (!C.HighlightNpc || targetNpcDataId == 0)
        {
            return;
        }

        ToggleHighlight(true);
    }

    public void SetNpcId(uint npcId)
    {
        _ = Service.Framework.Run(() =>
        {
            // before we update, we want to know if the previous npc object is still valid
            if (targetNpcDataId != 0)
            {
                ToggleHighlight(false);
            }
            PluginLog.Debug($"Setting npc id for HighlightObject. {npcId}");
            targetNpcDataId = npcId;
        });
    }

    public void ClearNpcInfo()
    {
        ToggleHighlight(false);
        targetNpcDataId = 0;
    }

    public unsafe void ToggleHighlight(bool on)
    {
        if (targetNpcDataId == 0)
        {
            return;
        }

        var gameObjects = Service.ObjectTable.Where(i =>
        {
            if (!i.IsValid())
                return false;
            var obj = (GameObject*)i.Address;
            return targetNpcDataId == obj->BaseId;
        });

        var enumerable = gameObjects as IGameObject[] ?? gameObjects.ToArray();
        if (!enumerable.Any())
        {
            return;
        }

        foreach (var obj in enumerable)
        {
            ((GameObject*)obj.Address)->Highlight(on ? ObjectHighlightColor.Red : ObjectHighlightColor.None);
        }
    }

    public void Dispose()
    {
        Service.Framework.Update -= Framework_OnUpdate;
    }
}
