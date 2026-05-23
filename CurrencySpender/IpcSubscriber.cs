using ECommons.EzIpcManager;

namespace CurrencySpender
{
    internal static class IpcSubscriber
    {
        public static bool IsReady(string pluginName)
            => ReflectionHelper.TryGetDalamudPlugin(pluginName, out _);
    }
    internal static class Lifestream
    {
        static Lifestream()
        {
            EzIPC.Init(typeof(Lifestream), "Lifestream");
        }

        internal static bool Enabled
            => IpcSubscriber.IsReady("Lifestream");

        [EzIPC("Lifestream.ExecuteCommand", applyPrefix: false)]
        internal static readonly Action<string> ExecuteCommand;

        [EzIPC("Lifestream.IsBusy", applyPrefix: false)]
        internal static readonly Func<bool> IsBusy;

        [EzIPC("Lifestream.Abort", applyPrefix: false)]
        internal static readonly Action Abort;

        [EzIPC("Lifestream.AethernetTeleport", applyPrefix: false)]
        internal static readonly Func<string, bool> AethernetTeleport;
        
        [EzIPC("Lifestream.ChangeCharacter", applyPrefix: false)]
        internal static readonly Func<string, string, int> ChangeCharacter;
    }
}
