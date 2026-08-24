using CurrencySpender.Classes;
using CurrencySpender.Windows.Config;
using Dalamud.Interface;

namespace CurrencySpender.Windows;

internal class ConfigWizardWindow : Window
{
    private static int Step = 0;
    private static int MaxSteps = 0;
    private static string VersionFrom = "0.0.0";
    private static string VersionTo = "";

    private readonly record struct VersionStep(string Version, int StepCount, Action<int> Draw);

    private static readonly List<VersionStep> VersionSteps =
    [
        new("1.1.0", 2, DrawVersion1_1_0Steps),
        new("1.1.2", 1, DrawVersion1_1_2Steps),
        new("1.2.2", 1, DrawVersion1_2_2Steps),
        new("1.2.3", 1, DrawVersion1_2_3Steps),
        new("1.2.4", 1, DrawVersion1_2_4Steps),
        new("1.2.6", 1, DrawVersion1_2_6Steps),
        new("1.2.7", 1, DrawVersion1_2_7Steps),
        new("1.3.0", 1, DrawVersion1_3_0Steps),
        new("1.3.1", 1, DrawVersion1_3_1Steps),
    ];

    public ConfigWizardWindow() : base($"{P.Name} {P.Version} - Configuration Wizard###{P.Name}ConfigWizardWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        VersionTo = VersionHelper.GetVersion();
        VersionFrom = VersionTo;
        CalculateSteps();
        P.ws.AddWindow(this);
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} {P.Version} - Configuration Wizard###{P.Name}ConfigWizardWindow;";
    }

    public override void Draw()
    {
        Vector2 contentRegion = ImGui.GetContentRegionAvail();
        float footerHeight = ImGui.GetTextLineHeight() + 20.0f;
        ImGui.BeginChild("StepContent", new Vector2(contentRegion.X, contentRegion.Y - footerHeight), false);

        DrawStep();

        ImGui.EndChild();
        DrawFooter();
    }

    private void DrawStep()
    {
        if (Step == 0 && MaxSteps == 0)
        {
            DrawNothingNew();
            return;
        }
        if (Step == 0)
        {
            DrawWelcome();
            return;
        }

        int cumulativeSteps = 0;
        foreach (var step in IncludedSteps)
        {
            if (Step > cumulativeSteps && Step <= cumulativeSteps + step.StepCount)
            {
                ImGui.Text($"Changed in Version {step.Version}:");
                step.Draw(Step - cumulativeSteps);
                break;
            }
            cumulativeSteps += step.StepCount;
        }
    }

    private void DrawWelcome()
    {
        ImGui.TextWrapped("Welcome to the Configuration Wizard!");
        ImGui.TextWrapped("This wizard will help you configure new options added since the latest patch. You can skip this setup and modify the settings later.");
        ImGui.TextWrapped("Review the new options or skip ahead if you're ready.");
        ImGui.Separator();
    }

    private void DrawNothingNew()
    {
        ImGui.TextWrapped("There are no new options to configure.");
        ImGui.TextWrapped("You are already up to date with the latest plugin version. You can close this window.");
        ImGui.Separator();
    }

    private void DrawFooter()
    {
        Vector2 windowSize = ImGui.GetWindowSize();
        float padding = 15.0f;
        if (Step > 0)
        {
            ImGui.SetCursorPos(new Vector2(padding, windowSize.Y - ImGui.GetTextLineHeight() - padding));
            ImGui.Text($"Step {Step}/{MaxSteps}");
        }
        if(Step > 0)
            ImGui.SetCursorPos(new Vector2(windowSize.X - 190 - padding, windowSize.Y - ImGui.GetTextLineHeight() - padding));
        else
            ImGui.SetCursorPos(new Vector2(windowSize.X - 130 - padding, windowSize.Y - ImGui.GetTextLineHeight() - padding));
        if (ImGuiEx.IconButtonWithText(FontAwesomeIcon.Times, "Skip"))
        {
            P.configWizard.IsOpen = false;
        }
        ImGui.SameLine();
        if (Step > 0)
        {
            if (ImGuiEx.IconButtonWithText(FontAwesomeIcon.ArrowLeft, "Back") && Step > 0)
            {
                Step--;
                ImGui.BeginChild("StepContent");
                ImGui.SetScrollY(0.0f);
                ImGui.EndChild();
            }
            ImGui.SameLine();
        }
        if (Step == MaxSteps)
        {
            if (ImGuiEx.IconButtonWithText(FontAwesomeIcon.Magic, "Finish"))
            {
                P.configWizard.IsOpen = false;
                Step = 0;
            }
        }
        else {
            if (ImGuiEx.IconButtonWithText(FontAwesomeIcon.ArrowRight, Step == 0 ? "Start" : "Next"))
            {
                Step++;
                ImGui.BeginChild("StepContent");
                ImGui.SetScrollY(0.0f);
                ImGui.EndChild();
            }
        }
    }

    private static void DrawCurrencySelection(string version)
    {
        var currencies = P.Currencies.Where(c => c.AddedInVersion == version && !c.Child && c.Enabled).ToList();
        if (currencies.Count == 0) return;
        
        ImGui.TextWrapped("Select if you want to see the following currencies:");
        foreach (var cur in currencies)
        {
            DrawCurrencyCheckbox(cur);
        }
    }

    private static void DrawCurrencyCheckbox(TrackedCurrency cur)
    {
        bool isSelected = C.SelectedCurrencies.Contains(cur.ItemId);
        if (ImGui.Checkbox($"##{cur.ItemId}", ref isSelected))
        {
            if (isSelected)
                C.SelectedCurrencies.Add(cur.ItemId);
            else
                C.SelectedCurrencies.Remove(cur.ItemId);
            P.spendingWindow.UpdateData();
            MainTab.Update(true);
        }
        ImGui.SameLine();
        ImGui.Text(cur.Name);
    }

    private static void DrawCollectableTypeSelection(params CollectableType[] types)
    {
        if (types.Length == 0) return;
        
        ImGui.TextWrapped("Select if you consider the following as collectable:");
        foreach (var type in types)
        {
            string label = CollectableTypeLabels.TryGetValue(type, out var displayName) ? displayName : type.ToString();
            bool isSelected = C.SelectedCollectableTypes.Contains(type);
            if (ImGui.Checkbox($"##{type}", ref isSelected))
            {
                if (isSelected)
                    C.SelectedCollectableTypes.Add(type);
                else
                    C.SelectedCollectableTypes.Remove(type);
                P.spendingWindow.UpdateData();
                MainTab.Update(true);
            }
            ImGui.SameLine();
            ImGui.Text(label);
        }
    }

    private static void DrawVersion1_1_0Steps(int step)
    {
        switch (step)
        {
            case 1:
                CurrenciesTab.Draw();
                break;
            case 2:
                ImGui.TextWrapped("Shows you if you can buy collectables with it.");
                ImGui.Checkbox("Show collectables", ref C.ShowCollectables);
                if (C.ShowCollectables)
                {
                    ImGui.TextWrapped("You can have a little info in the main window when you are still missing collectables from that currency.");
                    ImGui.Checkbox("Show missing collectables in the main window", ref C.ShowMissingCollectables);
                    ImGui.TextWrapped("If you don't want to see specific item you can deselect them here and they won't show up.");
                    var allTypes = Enum.GetValues(typeof(CollectableType)).Cast<CollectableType>().Where(t => t != CollectableType.None).ToArray();
                    DrawCollectableTypeSelection(allTypes);
                }
                break;
        }
    }

    private static void DrawVersion1_1_2Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Select if you want to see sellable items:");
                ImGui.Checkbox("Show items eligible for sale", ref C.ShowSellables);
                ImGui.Separator();
                DrawCollectableTypeSelection(CollectableType.Mahjong);
                ImGui.Separator();
                DrawCurrencySelection("1.1.2");
                break;
        }
    }

    private static void DrawVersion1_2_2Steps(int step)
    {
        switch (step)
        {
            case 1:
                DrawCurrencySelection("1.2.2");
                break;
        }
    }

    private static void DrawVersion1_2_3Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Open Currency Spender automatically when you open the ingame Currency window:");
                ImGui.Checkbox("Open automatically with the Currency window", ref C.OpenAutomatically);
                break;
        }
    }

    private static void DrawVersion1_2_4Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Minimum sales for the sellable table (0 = disable)");
                ImGui.InputInt("Minimum sales", ref C.MinSales);
                DrawCurrencySelection("1.2.4");
                ImGui.Separator();
                DrawCollectableTypeSelection(CollectableType.MasterRecipes);
                break;
        }
    }

    private static void DrawVersion1_2_6Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Will hide all windows when in a loading screen.");
                ImGui.Checkbox("Hide in loading screens", ref C.HideInLoadingScreens);
                ImGui.Separator();
                ImGui.TextWrapped("Will hide all windows when in a duty.");
                ImGui.Checkbox("Hide in duties", ref C.HideInDuties);
                ImGui.Separator();
                ImGui.TextWrapped("Will hide all windows when in combat.");
                ImGui.Checkbox("Hide in combat", ref C.HideInCombat);
                ImGui.Separator();
                ImGui.TextWrapped("Will hide all windows when in a cutscene.");
                ImGui.Checkbox("Hide in cutscenes", ref C.HideInCutscenes);
                ImGui.Separator();
                DrawCollectableTypeSelection(CollectableType.FashionAccessory);
                break;
        }
    }

    private static void DrawVersion1_2_7Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Will highlight the NPC when marking the flag or teleporting.");
                ImGui.Checkbox("Highlight NPC", ref C.HighlightNpc);
                ImGui.Separator();
                ImGui.TextWrapped("Will highlight the menus where to find the item when marking the flag or teleporting.");
                ImGui.Checkbox("Highlight Menus", ref C.HighlightMenu);
                ImGui.Separator();
                ImGui.TextWrapped("Automatically add newest upgrade items and remove old ones to the \"Items of Interest\"");
                ImGui.Checkbox("Automatically add/remove upgrade items", ref C.AddUpgradeItems);
                ImGui.Separator();
                if (C.AddUpgradeItems)
                {
                    List<uint> oldItems = [43554, 43555, 46730, 46731];
                    List<uint> newItems = [49758, 49759];
                    if (C.ItemsOfInterest.ContainsAny(oldItems))
                    {
                        foreach (var oldItem in oldItems)
                            C.ItemsOfInterest.Remove(oldItem);
                    }
                    if (!C.ItemsOfInterest.ContainsAll(newItems))
                    {
                        foreach (var newItem in newItems)
                            C.ItemsOfInterest.Add(newItem);
                    }
                }
                DrawCurrencySelection("1.2.7");
                break;
        }
    }

    private static void DrawVersion1_3_0Steps(int step)
    {
        switch (step)
        {
            case 1:
                if (C.ThirdParty)
                {
                    ImGui.TextWrapped("Third party plugins detected. You can enable them here, to make use of them.");
                    ImGui.Checkbox("Enable Lifestream to teleport closer to your target.", ref C.UseLifestream);
                    ImGui.Checkbox("Enable vnavmesh to path you to your target.", ref C.UseVnavmesh);
                    ImGui.Separator();
                }
                ImGui.TextWrapped("Societal currencies now have their own dedicated tab.");
                ImGui.Checkbox("Show Societies tab", ref C.ShowSocieties);
                ImGui.Separator();
                DrawCurrencySelection("1.3.0");
                break;
        }
    }

    private static void DrawVersion1_3_1Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Will hide items that require an achievement or quest you have not completed yet.");
                ImGui.Checkbox("Hide unobtainable items", ref C.HideUnattainableItems);
                break;
        }
    }

    private static void CalculateSteps()
    {
        MaxSteps = IncludedSteps.Sum(step => step.StepCount);
    }

    private static bool VersionIncluded(string version)
    {
        return VersionHelper.CompareVersions(version, VersionFrom) > 0 && VersionHelper.CompareVersions(version, VersionTo) <= 0;
    }

    private static IEnumerable<VersionStep> OrderedVersionSteps =>
        VersionSteps.OrderBy(step => step.Version, SemVerComparer.Instance);

    private static IEnumerable<VersionStep> IncludedSteps =>
        OrderedVersionSteps.Where(step => VersionIncluded(step.Version));

    private sealed class SemVerComparer : IComparer<string>
    {
        public static readonly SemVerComparer Instance = new();
        public int Compare(string? x, string? y) => VersionHelper.CompareVersions(x, y);
    }

    public void SetVersion(string fromVersion, string toVersion)
    {
        VersionFrom = fromVersion;
        VersionTo = toVersion;
        Step = 0;
        CalculateSteps();
    }

    public void OpenForCurrentVersion()
    {
        var current = VersionHelper.GetVersion();
        var fromVersion = VersionSteps
            .Where(step => VersionHelper.CompareVersions(current, step.Version) > 0)
            .OrderByDescending(step => step.Version, SemVerComparer.Instance)
            .Select(step => step.Version)
            .FirstOrDefault() ?? current;
        SetVersion(fromVersion, current);
        IsOpen = true;
    }
}
