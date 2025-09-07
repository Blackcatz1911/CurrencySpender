using CurrencySpender.Classes;

namespace CurrencySpender.Windows;

internal class ConfigTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Opens the config wizard again for the last version.");
        if (ImGui.Button("Open config wizard"))
        {
            VersionHelper.OpenConfigWizard();
        }
        ImGui.Separator();
        ImGui.TextWrapped("Shows you if you can buy ventures with it.");
        ImGui.Checkbox("Show ventures", ref C.ShowVentures);
        ImGui.Separator();
        ImGui.TextWrapped("Shows you if you can buy collectables with it.");
        ImGui.Checkbox("Show collectables", ref C.ShowCollectables);
        if(C.ShowCollectables)
        {
            ImGui.TextWrapped("You can have a little info in the main window when you are still missing collectables from that currency.");
            ImGui.Checkbox("Show missing collectables in the main window", ref C.ShowMissingCollectables);
            ImGui.TextWrapped("If you don't want to see specific item you can deselect them here and they won't show up.");
            ImGui.TextWrapped("Select which items you see as collectables:");
            foreach (CollectableType type in Enum.GetValues(typeof(CollectableType)))
            {
                if (type == CollectableType.None || type == CollectableType.Container) continue; // Skip 'None'
                string label = CollectableTypeLabels.TryGetValue(type, out var displayName) ? displayName : type.ToString();
                bool isSelected = C.SelectedCollectableTypes.Contains(type);
                if (ImGui.Checkbox($"##{type}", ref isSelected))
                {
                    if (isSelected)
                    {
                        C.SelectedCollectableTypes.Add(type);
                    }
                    else
                    {
                        C.SelectedCollectableTypes.Remove(type);
                    }
                    P.spendingWindow.UpdateData();
                    MainTab.update(true);
                }
                ImGui.SameLine();
                ImGui.Text(label);
            }
        }
        ImGui.Separator();
        ImGui.Checkbox("Show items eligible for sale", ref C.ShowSellables);
        ImGui.Separator();
        ImGui.TextWrapped("Select the thousand seperator");
        string[] items = { "None", "Seperator .", "Seperator ," };
        if (ImGui.BeginCombo("Select an Option", items[C.Seperator]))
        {
            for (int i = 0; i < items.Length; i++)
            {
                bool isSelected = (C.Seperator == i);
                if (ImGui.Selectable(items[i], isSelected))
                {
                    C.Seperator = i; // Update the selected item
                }

                // Set the initial focus when opening the combo box
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }
        ImGui.Separator();
        ImGui.Checkbox("Open automatically with the Currency window", ref C.OpenAutomatically);
        ImGui.Separator();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.25f, 0.25f, 1.0f)); // RGBA for red
        ImGui.TextWrapped("Dont turn it on, unless you know what you are doing...");
        ImGui.Checkbox("Debug Mode", ref C.Debug);
        ImGui.PopStyleColor();
    }
}
