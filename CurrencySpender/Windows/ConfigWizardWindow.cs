using Dalamud.Interface;

namespace CurrencySpender.Windows;

internal class ConfigWizardWindow : Window
{
    private static int Step = 0;
    private static int MaxSteps = 0;
    public static string Version = "0.0.0";

    private static readonly Dictionary<string, Action<int>> VersionSteps = new()
    {
        { "1.1.0", DrawVersion110Steps },
        //{ "1.2.0", DrawVersion120Steps }
    };

    public ConfigWizardWindow() : base("ConfigWizardWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        CalculateSteps();
        P.ws.AddWindow(this);
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} {P.Version} - Configuration Wizard###ConfigWizardWindow";
    }

    public override void Draw()
    {
        Vector2 contentRegion = ImGui.GetContentRegionAvail();
        float footerHeight = ImGui.GetTextLineHeight() + 20.0f; // Reserve space for footer
        ImGui.BeginChild("StepContent", new Vector2(contentRegion.X, contentRegion.Y - footerHeight), false);

        DrawStep();

        ImGui.EndChild();
        DrawFooter();
    }

    private void DrawStep()
    {
        if (Step == 0)
        {
            DrawWelcome();
        }
        else
        {
            int cumulativeSteps = 0;
            foreach (var (version, drawSteps) in VersionSteps)
            {
                if (VersionHelper.LowerVersionThan(version, Version))
                {
                    int versionStepCount = GetVersionStepCount(version);
                    if (Step > cumulativeSteps && Step <= cumulativeSteps + versionStepCount)
                    {
                        drawSteps(Step - cumulativeSteps);
                        break;
                    }
                    cumulativeSteps += versionStepCount;
                }
            }
        }
    }

    private void DrawWelcome()
    {
        ImGui.TextWrapped("Welcome to the Configuration Wizard!");
        ImGui.TextWrapped("This wizard will help you configure new options added since the latest patch. You can skip this setup and modify the settings later.");
        ImGui.TextWrapped("Review the new options or skip ahead if you're ready.");
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

    private static void DrawVersion110Steps(int step)
    {
        switch (step)
        {
            case 1:
                ConfigCurrenciesTab.Draw();
                break;
            case 2:
                VersionHelper.DrawVersion110Step2();
                break;
        }
    }

    private static void DrawVersion120Steps(int step)
    {
        switch (step)
        {
            case 1:
                ImGui.TextWrapped("Step 1: Configure options for version 1.2.0.");
                // Add controls and logic for this step
                break;
            case 2:
                ImGui.TextWrapped("Step 2: Finalize configuration for version 1.2.0.");
                // Add controls and logic for this step
                break;
            case 3:
                ImGui.TextWrapped("Step 3: Additional options for version 1.2.0.");
                // Add controls and logic for this step
                break;
        }
    }

    private static void CalculateSteps()
    {
        MaxSteps = 0;
        foreach (var version in VersionSteps.Keys)
        {
            if (VersionHelper.LowerVersionThan(version, Version))
            {
                MaxSteps += GetVersionStepCount(version);
            }
        }
    }

    private static int GetVersionStepCount(string version)
    {
        return version switch
        {
            "1.1.0" => 2, // Number of steps for version 1.1.0
            //"1.2.0" => 3, // Number of steps for version 1.2.0
            _ => 0
        };
    }
    public void SetVersion(string version)
    {
        Version = version;
    }
}
