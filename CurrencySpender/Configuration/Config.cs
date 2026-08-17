using CurrencySpender.Classes;

namespace CurrencySpender.Configuration;

public enum GlueSide
{
    Left,
    Right,
}

[Serializable]
public class Config
{
    public string Version { get; set; } = "0.0.0";
    public string GameVersion { get; set; } = "";

    public int Separator = 0;
    //public Dictionary<uint, uint> FateRanks = [];
    public List<uint> ItemsOfInterest = [];
    public bool AddUpgradeItems = true;

    public bool ShowVentures = true;
    public bool ShowCollectables = true;
    public HashSet<CollectableType> SelectedCollectableTypes { get; set; } = new HashSet<CollectableType>();
    public HashSet<uint> SelectedCurrencies { get; set; } = new HashSet<uint>();
    public bool HideEmptyCurrencies = true;
    public bool ShowItemsOfInterest = true;
    public bool ShowMissingCollectables = true;
    public bool ShowSellables = true;
    public int MinSales = 0;
    public bool ShowSocieties = true;

    public bool ThirdParty = false;
    public bool UseLifestream = false;
    public bool UseVnavmesh = false;

    public bool ShowButton = true;
    public bool OpenAutomatically = false;
    public bool HideInLoadingScreens = true;
    public bool HideInDuties = true;
    public bool HideInCombat = true;
    public bool HideInCutscenes = true;
    public bool HighlightNpc = true;
    public bool HighlightMenu = true;
    public bool GlueToMainWindow = false;
    public GlueSide GlueSide = GlueSide.Right;

    public bool Debug = false;
}
