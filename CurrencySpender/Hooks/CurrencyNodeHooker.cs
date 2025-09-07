using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dalamud.Game.Addon.Events;
using CurrencySpender.Hooks;
using Dalamud.Game.Addon.Events.EventDataTypes;

namespace CurrencySpender.Hooks;

public class CurrencyNodeHooker
{

    private List<IAddonEventHandle?>? eventHandles;

    public CurrencyNodeHooker()
    {
    }

    public unsafe void Enable()
    {
        eventHandles = [];
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Currency", OnCurrencySetup);
        // Service.AddonLifecycle.RegisterListener(AddonEvent.PreRefresh, "Currency", OnCurrencyRefresh);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "Currency", OnCurrencyRefresh);
        // Service.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "Currency", OnCurrencyRefresh);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "Currency", OnCurrencyFinalize);
    }

    public void Disable()
    {
        Service.AddonLifecycle.UnregisterListener(OnCurrencySetup);

        foreach (var handle in eventHandles ?? [])
        {
            if (handle is not null)
            {
                Service.AddonEventManager.RemoveEvent(handle);
            }
        }
    }

    private unsafe void OnCurrencyRefresh(AddonEvent eventType, AddonArgs args) { PluginLog.Debug("Refresh currency"); }

    private unsafe void OnCurrencySetup(AddonEvent eventType, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon.Address;
        if (addon == null) return; // Always check!
        PluginLog.Information("OnCurrencySetup Hooked");
        for (uint i = 0; i < addon->UldManager.NodeListCount; i++) {
            var node = addon->UldManager.NodeList[i];
            if (node->NodeId != 20 && node->NodeId < 200000) continue;
            if (node is null || node->GetNodeType() != NodeType.Component) continue;

            PluginLog.Debug($"Index: {i}, Node: {node->NodeId}, Node: {node->GetNodeType()} IsVisible: {node->IsVisible()}");
            
            // var child = node

            // Make node respond to mouse
            node->NodeFlags |= NodeFlags.RespondToMouse | NodeFlags.HasCollision;

            // Attach events
            eventHandles?.AddRange([
                Service.AddonEventManager.AddEvent((nint)addon, (nint)node, AddonEventType.MouseOver, (eventType, data) => OnNodeMouseOver(node, eventType, data)),
                Service.AddonEventManager.AddEvent((nint)addon, (nint)node, AddonEventType.MouseOut, (eventType, data) =>OnNodeMouseOut(node, eventType, data)),
                Service.AddonEventManager.AddEvent((nint)addon, (nint)node, AddonEventType.MouseClick, (eventType, data) => OnNodeClick(node, eventType, data)),
            ]);
        }
    }
    private unsafe bool IsNodeVisibleAndInteractive(AtkResNode* node)
    {
        if (node == null) return false;

        // 1. Check if node is explicitly hidden
        if (!node->IsVisible()) return false;

        // 2. Check if node has zero size
        if (node->Width <= 0 || node->Height <= 0) return false;

        // ✅ Node is visible and on-screen
        return true;
    }

    private void OnCurrencyFinalize(AddonEvent type, AddonArgs args)
    {
        foreach (var handle in eventHandles ?? [])
        {
            if (handle is not null)
            {
                Service.AddonEventManager.RemoveEvent(handle);
            }
        }
    }

    private unsafe void OnNodeClick(AtkResNode* node, AddonEventType atkEventType, AddonEventData data)
    {
        if (node->IsVisible())
        {
            PluginLog.Debug($"{node->NodeId} has been clicked");
            // foreach (var cur in P.Currencies)
            // {
            //     if (cur.ItemId == index) P.ToggleSpendingUI(cur);
            // }
            // PluginLog.Debug($"{index} has been clicked {data}");
        }
    }

    private unsafe void OnNodeMouseOver(AtkResNode* node, AddonEventType atkEventType, AddonEventData data)
    {
        if(node->IsVisible()) Service.AddonEventManager.SetCursor(AddonCursorType.Clickable);
    }

    private unsafe void OnNodeMouseOut(AtkResNode* node, AddonEventType atkEventType, AddonEventData data)
    {
        if(node->IsVisible()) Service.AddonEventManager.ResetCursor();
    }
}
