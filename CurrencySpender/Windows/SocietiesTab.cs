namespace CurrencySpender.Windows;

internal static class SocietiesTab
{
    internal static readonly CurrencyListTab.State State = new();

    internal static void Draw()
    {
        CurrencyListTab.Draw(State, society: true);
    }

    public static void Update(bool Force = false)
    {
        CurrencyListTab.Update(State, society: true, Force);
    }
}
