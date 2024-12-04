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
            foreach (var item in Generator.items.Where(item => item.Shop.NpcId == 1037055).ToList())
            {
                ImGui.TableNextColumn();
                //ImGui.Text("text1");
                ImGui.Text(item.ShopId.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(item.Shop.RequiredLevel.ToString());
                ImGui.TableNextColumn();
                //ImGui.Text("text2");
                ImGui.Text(item.Name);
                //ImGui.TableNextColumn();
                //ImGui.Text("text3");
                //ImGui.Text(shop.NpcId.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text("text4");
                //ImGui.Text(shop.NpcName);
                //ImGui.TableNextColumn();
                //ImGui.Text(shop.Location.NpcName.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text(item.ENpcData[0].fields.ToString());
            }
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            foreach (var item in Generator.items.Where(item => item.ShopId == 1770754).ToList())
            {
                ImGui.TableNextColumn();
                //ImGui.Text("text1");
                ImGui.Text(item.ShopId.ToString());
                ImGui.TableNextColumn();
                //ImGui.Text("text2");
                ImGui.Text(item.Name);
                //ImGui.TableNextColumn();
                //ImGui.Text("text3");
                //ImGui.Text(shop.NpcId.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text("text4");
                //ImGui.Text(shop.NpcName);
                //ImGui.TableNextColumn();
                //ImGui.Text(shop.Location.NpcName.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text(item.ENpcData[0].fields.ToString());
            }
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            foreach (var item in Generator.items.Where(item => item.ShopId == 1770755).ToList())
            {
                ImGui.TableNextColumn();
                //ImGui.Text("text1");
                ImGui.Text(item.ShopId.ToString());
                ImGui.TableNextColumn();
                //ImGui.Text("text2");
                ImGui.Text(item.Name);
                //ImGui.TableNextColumn();
                //ImGui.Text("text3");
                //ImGui.Text(shop.NpcId.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text("text4");
                //ImGui.Text(shop.NpcName);
                //ImGui.TableNextColumn();
                //ImGui.Text(shop.Location.NpcName.ToString());
                //ImGui.TableNextColumn();
                //ImGui.Text(item.ENpcData[0].fields.ToString());
            }
            //}
            //PluginLog.Verbose("Starting ImGui EndTable rendering...");
            ImGui.EndTable();
        }
        ImGui.Separator();
    }
}
