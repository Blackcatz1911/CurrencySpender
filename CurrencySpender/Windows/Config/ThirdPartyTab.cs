using CurrencySpender.Classes;

namespace CurrencySpender.Windows.Config;

internal class ThirdPartyTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Use Lifestream when possible.");
        ImGui.Checkbox("Use Lifestream", ref C.UseLifestream);
        ImGui.Separator();
        ImGui.TextWrapped("Use vnavmesh when possible.");
        ImGui.Checkbox("Use vnavmesh", ref C.UseVnavmesh);
    }
}
