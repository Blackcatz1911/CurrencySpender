namespace CurrencySpender.Windows;
internal class ConfigCurrenciesTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("If you don't want to see specific currencies you can deselect them here and they won't show up.");
        ImGui.Checkbox("Hide currencies you currently don't own", ref C.HideEmptyCurrencies);
        ImGui.TextWrapped("Select which currencies you want to see:");
        foreach (var cur in C.Currencies.Where(cur => cur.Child == false && cur.Enabled).ToList() )
        {
            bool isSelected = C.SelectedCurrencies.Contains(cur.ItemId);
            if (ImGui.Checkbox($"##{cur.ItemId}", ref isSelected))
            {
                if (isSelected)
                {
                    C.SelectedCurrencies.Add(cur.ItemId);
                }
                else
                {
                    C.SelectedCurrencies.Remove(cur.ItemId);
                }
                P.spendingWindow.UpdateData();
                MainTab.update(true);
            }
            ImGui.SameLine();
            ImGui.Text(cur.Name);
        }
    }
}
