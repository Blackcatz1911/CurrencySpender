using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Exd;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Classes;

[Flags]
public enum ItemType
{
    None = 0,
    Tradeable = 1,       // 2^0
    Sellable = 2,        // 2^1
    Collectable = 4,     // 2^2
    Venture = 8,         // 2^3
    Currency = 16,       // 2^4
}
public enum CollectableType
{
    None,
    Mount,
    Minion,
    Scroll,
    Emote,
    Hairstyle,
    Barding,
    RidingMap,
    Facewear,
    FramersKit,
    TTCard,
    Mahjong,
    Container,
    MasterRecipes
}
public unsafe class ShopItem
{
    public ItemType Type { get; set; }
    public CollectableType CollectableType { get; set; }
    public uint Id { get; set; }
    [JsonIgnore] public string Name => Service.DataManager.GetExcelSheet<Item>()!.GetRow(Id).Name.ExtractText() ?? "Unable to read name";
    public uint Category { get; set; }
    public uint Price { get; set; }
    public uint Currency { get; set; }
    public uint ShopId { get; set; }
    public required Shop Shop { get; set; }

    public bool Disabled = false;
    public uint? RequiredRank;

    public List<int>? ContainerUnlocks { get; set; }

    public override string ToString()
    {
        var cur_name = Service.DataManager.GetExcelSheet<Item>().GetRow(Currency).Name.ExtractText();
        return $"Id: {Id}, Name: {Name}, Category: {Category}, Type: ({FormatFlags(Type)}), Price: {Price}, Currency: {cur_name}, ShopId: {ShopId}";
    }

    private string FormatFlags(ItemType type)
    {
        var flags = Enum.GetValues(typeof(ItemType))
            .Cast<ItemType>()
            .Where(flag => type.HasFlag(flag) && flag != ItemType.None);

        return string.Join(", ", flags);
    }
    public uint CurrentPrice { get; set; }
    public uint LastChecked { get; set; }
    public uint AmountCanBuy => (uint)Math.Floor((double)P.Currencies.First(cur => cur.ItemId == Currency).CurrentCount / Price);
    public uint Profit { get; set; }
    public float GilPerCur { get; set; }
    public uint HasSoldWeek { get; set; }

    public unsafe bool IsUnlocked(Item item)
    {
        if (item.ItemAction.RowId == 0)
            return false;

        switch ((ItemActionAction)item.ItemAction.Value.Action.RowId)
        {
            case ItemActionAction.Companion:
                return UIState.Instance()->IsCompanionUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.BuddyEquip:
                return UIState.Instance()->Buddy.CompanionInfo.IsBuddyEquipUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.Mount:
                return PlayerState.Instance()->IsMountUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.SecretRecipeBook:
                return PlayerState.Instance()->IsSecretRecipeBookUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.UnlockLink:
                return UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.TripleTriadCard when item.AdditionalData.Is<TripleTriadCard>():
                return UIState.Instance()->IsTripleTriadCardUnlocked((ushort)item.AdditionalData.RowId);

            case ItemActionAction.FolkloreTome:
                return PlayerState.Instance()->IsFolkloreBookUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.OrchestrionRoll when item.AdditionalData.Is<Orchestrion>():
                return PlayerState.Instance()->IsOrchestrionRollUnlocked(item.AdditionalData.RowId);

            case ItemActionAction.FramersKit:
                return PlayerState.Instance()->IsFramersKitUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.Ornament:
                return PlayerState.Instance()->IsOrnamentUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionAction.Glasses:
                return PlayerState.Instance()->IsGlassesUnlocked((ushort)item.AdditionalData.RowId);
        }

        var row = ExdModule.GetItemRowById(item.RowId);
        return row != null && UIState.Instance()->IsItemActionUnlocked(row) == 1;
    }
    public enum ItemActionAction : ushort
    {
        Companion = 853,
        BuddyEquip = 1013,
        Mount = 1322,
        SecretRecipeBook = 2136,
        UnlockLink = 2633, // riding maps, blu totems, emotes/dances, hairstyles
        TripleTriadCard = 3357,
        FolkloreTome = 4107,
        OrchestrionRoll = 25183,
        FramersKit = 29459,
        // FieldNotes = 19743, // bozjan field notes (server side, but cached)
        Ornament = 20086,
        Glasses = 37312,
        CompanySealVouchers = 41120, // can use = is in grand company, is unlocked = always false
    }
}

