using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace CurrencySpender.Tasks;

public class MovementTask
{
    public static string Status { get; private set; } = "Idle";
    private static void SetStatus(string status)
    {
        if (Status == status) return;
        Status = status;
        PluginLog.Debug($"Status: {status}");
    }

    public static bool IsPlayerBusy => Player.Object != null && (Player.Object.IsCasting || Player.IsMoving || Player.IsAnimationLocked);
    internal static unsafe bool UseAction(ActionType actionType, uint actionId) => ActionManager.Instance()->UseAction(actionType, actionId);
    internal static unsafe bool IsActionUsable(ActionType actionType, uint actionId) => ActionManager.Instance()->GetActionStatus(actionType, actionId) == 0;

    public static void Dismount()
    {
        if (Player.Mounted) UseAction(ActionType.Mount, 0);
    }

    public static void Cancel()
    {
        Vnavmesh.Stop?.Invoke();
        Vnavmesh.CancelAll?.Invoke();
        SetStatus("Idle");
    }

    internal static unsafe bool IsInCorrectZone(uint territoryId, uint mapId)
    {
        var agent = FFXIVClientStructs.FFXIV.Client.UI.Agent.AgentMap.Instance();
        if (agent == null) return false;
        var currentTerritory = agent->CurrentTerritoryId;
        var currentMap = agent->CurrentMapId;
        var match = territoryId == currentTerritory && mapId == currentMap;
        PluginLog.Verbose($"Zone check: current={match} (territory {currentTerritory}/{territoryId}, map {currentMap}/{mapId})");
        return match;
    }

    internal static Func<bool?> WaitForZone(uint territoryId, uint mapId, string what, int timeoutMs = 30000, Func<bool>? waitWhile = null)
    {
        var deadline = Environment.TickCount64 + timeoutMs;
        return () =>
        {
            SetStatus($"{what}ing");
            if (waitWhile?.Invoke() == true)
            {
                return false;
            }
            if (Service.Condition.IsBetweenAreas())
            {
                return false;
            }
            if (IsInCorrectZone(territoryId, mapId))
            {
                PluginLog.Debug($"{what} finished, arrived in zone {territoryId}/{mapId}");
                return true;
            }
            if (Environment.TickCount64 > deadline)
            {
                PluginLog.Warning($"{what} wait timed out, not in target zone {territoryId}/{mapId}");
                return true;
            }
            return false;
        };
    }

    internal static Func<bool?> MoveTo(Vector3 position, uint territoryId, uint mapId)
    {
        PluginLog.Debug($"MoveTo start: target={position}, distance={Player.DistanceTo(position):F1}");
        if (Player.DistanceTo(position) < 1)
        {
            PluginLog.Debug("MoveTo: already at target, skipping");
            SetStatus("Idle");
            return () => true;
        }

        var phase = 0;
        var mountAttempted = false;
        var canFly = false;
        var retryAttempt = 0;
        var retryDelayUntil = 0L;
        var flyRetried = false;
        var dismountAttempts = 0;

        return () =>
        {
            switch (phase)
            {
                case 0:
                    if (Service.Condition.IsBetweenAreas())
                    {
                        return false;
                    }
                    if (!Vnavmesh.NavIsReady())
                    {
                        SetStatus("Waiting for navmesh");
                        return false;
                    }
                    if (IsPlayerBusy) return false;
                    if (!IsInCorrectZone(territoryId, mapId))
                    {
                        PluginLog.Debug($"MoveTo skipped: not in target zone (NPC zone {territoryId}/{mapId})");
                        SetStatus("Idle");
                        return true;
                    }
                    phase = 1;
                    return false;

                case 1:
                    if (Player.Mounted)
                    {
                        canFly = Player.CanFly;
                        PluginLog.Debug($"MoveTo: mounted, canFly={canFly}");
                        phase = 2;
                        return false;
                    }
                    if (Player.IsBusy) return false;
                    if (!Player.CanMount)
                    {
                        PluginLog.Debug("MoveTo: cannot mount in current territory, using ground path");
                        phase = 2;
                        return false;
                    }
                    if (!mountAttempted)
                    {
                        if (!IsActionUsable(ActionType.Mount, 1))
                        {
                            PluginLog.Debug("MoveTo: mount action not usable, using ground path");
                            phase = 2;
                            return false;
                        }
                        UseAction(ActionType.Mount, 1);
                        mountAttempted = true;
                        SetStatus("Mounting");
                        PluginLog.Debug("MoveTo: mounting...");
                    }
                    return false;

                case 2:
                    if (Environment.TickCount64 < retryDelayUntil) return false;
                    if (Vnavmesh.PathfindInProgress()) return false;
                    SetStatus("Pathfinding");
                    var result = Vnavmesh.SimpleMovePathfindAndMoveTo(position, canFly);
                    if (result)
                    {
                        PluginLog.Debug($"MoveTo: pathfind started (fly={canFly})");
                        phase = 3;
                        return false;
                    }
                    retryAttempt++;
                    if (retryAttempt >= 3)
                    {
                        if (canFly && !flyRetried)
                        {
                            PluginLog.Debug("MoveTo: fly pathfind failed, falling back to ground path");
                            canFly = false;
                            flyRetried = true;
                            retryAttempt = 0;
                            retryDelayUntil = Environment.TickCount64 + 500;
                            return false;
                        }
                        PluginLog.Error($"Could not start path to {position} (fly={canFly})");
                        SetStatus("Idle");
                        return true;
                    }
                    PluginLog.Debug($"MoveTo: pathfind failed (attempt={retryAttempt}, fly={canFly}), retrying");
                    retryDelayUntil = Environment.TickCount64 + 500;
                    return false;

                case 3:
                    if (Vnavmesh.PathfindInProgress()) return false;
                    if (Vnavmesh.PathIsRunning())
                    {
                        SetStatus("Moving");
                        phase = 4;
                        return false;
                    }
                    PluginLog.Debug("MoveTo: pathing finished");
                    if (Player.Mounted)
                    {
                        Dismount();
                        dismountAttempts = 1;
                        retryDelayUntil = Environment.TickCount64 + 1000;
                        SetStatus("Dismounting");
                        phase = 5;
                        return false;
                    }
                    SetStatus("Idle");
                    return true;

                case 4:
                    if (Vnavmesh.PathIsRunning()) return false;
                    PluginLog.Debug("MoveTo: pathing finished");
                    if (Player.Mounted)
                    {
                        Dismount();
                        dismountAttempts = 1;
                        retryDelayUntil = Environment.TickCount64 + 1000;
                        SetStatus("Dismounting");
                        phase = 5;
                        return false;
                    }
                    SetStatus("Idle");
                    return true;

                case 5:
                    if (!Player.Mounted)
                    {
                        SetStatus("Idle");
                        return true;
                    }
                    if (Environment.TickCount64 < retryDelayUntil) return false;
                    if (dismountAttempts >= 5)
                    {
                        PluginLog.Warning("MoveTo: could not dismount");
                        SetStatus("Idle");
                        return true;
                    }
                    Dismount();
                    dismountAttempts++;
                    retryDelayUntil = Environment.TickCount64 + 1000;
                    return false;

                default:
                    return true;
            }
        };
    }
}
