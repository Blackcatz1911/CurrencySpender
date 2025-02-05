using CurrencySpender.Classes;
using ECommons.Configuration;

namespace CurrencySpender.Configuration;

[Serializable]
public class Config: IEzConfig
{
    public string Version { get; set; } = "0.0.0";

    public List<TrackedCurrency> Currencies = [];
    public int Seperator = 0;
    //public Dictionary<uint, uint> FateRanks = [];
    public List<uint> ItemsOfInterest = [43554, 43555];

    public bool ShowVentures = true;
    public bool ShowCollectables = true;
    public HashSet<CollectableType> SelectedCollectableTypes { get; set; } = new HashSet<CollectableType>();
    public HashSet<uint> SelectedCurrencies { get; set; } = new HashSet<uint>();
    public bool HideEmptyCurrencies = true;
    public bool ShowItemsOfInterest = true;
    public bool ShowMissingCollectables = true;
    public bool ShowSellables = true;

    public bool Debug = false;
}
