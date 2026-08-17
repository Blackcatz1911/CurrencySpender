using ECommons.EzIpcManager;

namespace CurrencySpender
{
    internal static class IpcSubscriber
    {
        public static bool IsReady(string pluginName)
            => Service.PluginInterface.InstalledPlugins.Any(x => x.InternalName == pluginName && x.IsLoaded);
    }
    
    public static class Lifestream
    {
        static Lifestream()
        {
            EzIPC.Init(typeof(Lifestream), "Lifestream", SafeWrapper.AnyException);
        }

        internal static bool Enabled
            => IpcSubscriber.IsReady("Lifestream");

        [EzIPC] public static Action<string>? ExecuteCommand;
        
        [EzIPC] public static Func<bool>? IsBusy;
    }
    public static class Vnavmesh
    {
        static Vnavmesh()
        {
            EzIPC.Init(typeof(Vnavmesh), "vnavmesh", SafeWrapper.AnyException);
        }

        internal static bool Enabled
            => IpcSubscriber.IsReady("vnavmesh");

        [EzIPC("Nav.IsReady")]
        public static Func<bool>? NavIsReady;

        [EzIPC("Nav.PathfindCancelAll")]
        public static Action? CancelAll;
        
        [EzIPC("Path.IsRunning")]
        public static Func<bool> PathIsRunning;
        
        [EzIPC("SimpleMove.PathfindAndMoveTo")]
        public static Func<Vector3, bool, bool> SimpleMovePathfindAndMoveTo;
        
        [EzIPC("SimpleMove.PathfindInProgress")]
        public static Func<bool>? PathfindInProgress;

        [EzIPC("Path.Stop")]
        public static Action? Stop;
    }
}
