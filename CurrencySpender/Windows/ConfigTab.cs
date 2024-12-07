namespace CurrencySpender.Windows;

internal class ConfigTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Shows you if you can buy ventures with it.");
        ImGui.Checkbox($"Show Ventures", ref C.ShowVentures);
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
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.25f, 0.25f, 1.0f)); // RGBA for red
        ImGui.TextWrapped("Dont turn it on, unless you know what you are doing...");
        ImGui.Checkbox($"Debug Mode", ref C.Debug);
        ImGui.PopStyleColor();
    }
}
