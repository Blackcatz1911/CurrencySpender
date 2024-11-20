using CurrencySpender.Data.Generators;

namespace CurrencySpender.Windows;

internal class DebugMainTab
{
    internal unsafe static void Draw()
    {
        if (ImGuiEx.Button("Generate"))
        {
            NPCGen.init();
        }
        if (ImGui.BeginTable("##NPC", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("ShopId");
            ImGui.TableSetupColumn("ShopName");
            ImGui.TableSetupColumn("NpcId");
            ImGui.TableSetupColumn("NpcName");
            //ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
            //ImGui.TableSetupColumn("Price");
            //ImGui.TableSetupColumn("");
            ImGui.TableHeadersRow();
            foreach (var shop in NPCGen.shops)
            {
                ImGui.TableNextColumn();
                ImGui.Text(shop.ShopId.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(shop.ShopName.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(shop.NpcId.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(shop.NpcName.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text(item.ENpcData[0].fields.ToString());
            }
            //}
            //Service.Log.Verbose("Starting ImGui EndTable rendering...");
            ImGui.EndTable();
        }
        ImGui.Separator();
    }
}
