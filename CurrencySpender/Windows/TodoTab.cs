using System;
using CurrencySpender;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CurrencySpender.Classes;
namespace CurrencySpender.Windows;

internal class TodoTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Whats is planned for later updates:");
        List<String> list = ["Support more currencies", "Weekly sales displayed in the Spending Guide",
            "Teleportation button", "Checking if you can get certain items", "Dont show completed bardings"];
        foreach (var item in list)
        {
            ImGui.TextWrapped("- " + item);
        }
    }
}
