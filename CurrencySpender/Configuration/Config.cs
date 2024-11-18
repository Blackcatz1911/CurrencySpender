using CurrencySpender.Classes;
using ECommons.Configuration;

namespace CurrencySpender.Configuration;

[Serializable]
public class Config: IEzConfig
{
    public String Version { get; set; } = "0.0.0.0";

    public List<TrackedCurrency> Currencies = [];
    public List<BuyableItem> Items = [];

    public Boolean debug = false;

    // the below exist just to make saving less cumbersome
}
