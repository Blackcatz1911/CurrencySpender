using CurrencySpender.Classes;
using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace CurrencySpender.Configuration;

[Serializable]
public class Config: IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public List<TrackedCurrency> Currencies = [];
    public List<BuyableItem> Items = [];

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
