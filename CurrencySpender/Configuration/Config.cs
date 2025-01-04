using CurrencySpender.Classes;
using ECommons.Configuration;

namespace CurrencySpender.Configuration;

[Serializable]
public class Config: IEzConfig
{
    public string Version { get; set; } = "0.0.0.0";

    public List<TrackedCurrency> Currencies = [];
    public int Seperator = 0;
    //public Dictionary<uint, uint> FateRanks = [];
    public List<uint> ItemsOfInterest = [43554, 43555];

    public bool ShowVentures = true;
    public bool ShowItemsOfInterest = true;

    public bool Debug = false;
}
