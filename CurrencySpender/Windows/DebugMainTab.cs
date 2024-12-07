using CurrencySpender.Data;

namespace CurrencySpender.Windows;

internal class DebugMainTab
{
    internal unsafe static void Draw()
    {
        if (ImGuiEx.Button("Generate"))
        {
            ShopGen.init();
        }
        if (ImGui.BeginTable("##NPC", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("ShopId");
            ImGui.TableSetupColumn("ShopLevel");
            ImGui.TableSetupColumn("Name");
            //ImGui.TableSetupColumn("NpcId");
            //ImGui.TableSetupColumn("NpcName");
            //ImGui.TableSetupColumn("Location-NpcName");
            //ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
            //ImGui.TableSetupColumn("Price");
            //ImGui.TableSetupColumn("");
            ImGui.TableHeadersRow();
            foreach (var shop in Generator.shops.ToList())
            {
                ImGui.TableNextColumn();
                //ImGui.Text("text1");
                ImGui.Text($"{shop.ShopId}");
                ImGui.TableNextColumn();
                ImGui.Text($"{shop.NpcId}");
                ImGui.TableNextColumn();
                //ImGui.Text("text2");
                ImGui.Text($"{shop.NpcName}");
            }
            //PluginLog.Verbose("Starting ImGui EndTable rendering...");
            ImGui.EndTable();
        }
        ImGui.Separator();
    }
}
