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
}
