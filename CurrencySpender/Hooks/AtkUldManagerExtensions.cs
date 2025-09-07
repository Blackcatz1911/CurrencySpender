using FFXIVClientStructs.FFXIV.Component.GUI;

namespace CurrencySpender.Hooks;

public static class AtkUldManagerExtensions {
    public static unsafe T* SearchNodeById<T>(this AtkUldManager atkUldManager, uint nodeId) where T : unmanaged {
        foreach (var node in atkUldManager.Nodes) {
            if (node.Value is not null) {
                if (node.Value->NodeId == nodeId)
                    return (T*) node.Value;
            }
        }

        return null;
    }

    public static unsafe AtkResNode* SearchNodeById(this AtkUldManager atkUldManager, uint nodeId)
        => atkUldManager.SearchNodeById<AtkResNode>(nodeId);
}
