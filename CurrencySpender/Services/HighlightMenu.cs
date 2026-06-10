using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.Expressions;
using Lumina.Text.ReadOnly;
using System;
using System.Linq;
using CurrencySpender.Classes;
using static System.Net.Mime.MediaTypeNames;
using Dalamud.Interface.Colors;

namespace CurrencySpender.Services;
internal class HighlightMenu : IDisposable
{
    private List<ShopItem> items = new List<ShopItem>();
    private Vector4 color = ImGuiColors.DalamudRed;
    public HighlightMenu()
    {
        Service.Framework.Update += Framework_OnUpdate;
    }

    private unsafe void Framework_OnUpdate(IFramework framework)
    {
        if (!C.HighlightMenu || items.Count == 0)
        {
            return;
        }

        HighlightShopAddon();
        HighlightSelectIconStringAddon();
        HighlightSelectStringAddon();
        HighlightInclusionShopAddon();
        HighlightShopExchangeCurrencyAddon();
        HighlightShopExchangeItemAddon();
        HighlightCollectablesShopAddon();
    }

    private unsafe void HighlightShopAddon()
    {
        if (items.Count == 0)
        {
            return;
        }

        var item = items[0];
        var shopAddonPtr = Service.GameGui.GetAddonByName("Shop");
        if (shopAddonPtr == nint.Zero)
        {
            return;
        }


        var shopAddon = (AtkUnitBase*)shopAddonPtr.Address;

        var itemList = (AtkComponentList*)shopAddon->GetComponentByNodeId(16);

        var bestMatchIndex = uint.MaxValue;

        foreach (uint index in Enumerable.Range(0, itemList->ListLength))
        {
            var listItemRenderer = itemList->ItemRendererList[index].AtkComponentListItemRenderer;

            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(3);
            if (text == null)
            {
                continue;
            }
            var itemName = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            // I use a partial matching because I guess item names can be concatenated. I don't think what I came up with
            // is foolproof, but it's good enough for now. I'm trying to figure out if I can use the agent for exact name
            // matches, but what I'm seeing doesn't quite match up with what I see in CS. So until I figure that out, I'm
            // going with this.
            if (string.Equals(item.Name, itemName))
            {
                // if we ever find an exact match, that must be it, so highlight it and return.
                text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(ImGuiColors.DalamudRed);
                text->SetText(itemName);
                return;
            }
            else if (itemName.EndsWith("..."))
            {
                if (item.Name.StartsWith(itemName.TrimEnd('.')))
                {
                    bestMatchIndex = index;
                }
            }
        }

        if (bestMatchIndex != uint.MaxValue)
        {
            var listItemRenderer = itemList->ItemRendererList[bestMatchIndex].AtkComponentListItemRenderer;
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(3);
            var itemName = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(ImGuiColors.DalamudRed);
            // strangely, it doesn't seem like the list gets its color updated until we set the text below
            text->SetText(itemName);
        }
    }

    private unsafe void HighlightSelectIconStringAddon()
    {
        if (items.Count == 0) return;
        var item = items[0];
        var selectIconStringAddonPtr = Service.GameGui.GetAddonByName("SelectIconString");

        if (selectIconStringAddonPtr == nint.Zero)
        {
            return;
        }

        var selectIconStringAddon = (AtkUnitBase*)selectIconStringAddonPtr.Address;

        var componentList = selectIconStringAddon->GetComponentListById(3);

        if (componentList == null)
        {
            return;
        }

        foreach (uint index in Enumerable.Range(0, componentList->ListLength))
        {
            var listItemRenderer = componentList->ItemRendererList[index].AtkComponentListItemRenderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(2);
            if (text == null)
            {
                continue;
            }
            try
            {
                if (item.Shop.ShopName != null && item.Shop.ShopName.Equals(text->NodeText.ToString()))
                {
                    text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
                    return;
                }
            }
            catch (NullReferenceException)
            {
                continue;
            }
        }
    }

    private unsafe void HighlightSelectStringAddon()
    {
        if (items.Count == 0) return;
        var item = items[0];
        
        var selectIconStringAddonPtr = Service.GameGui.GetAddonByName("SelectString");

        if (selectIconStringAddonPtr == nint.Zero)
        {
            return;
        }

        var selectIconStringAddon = (AtkUnitBase*)selectIconStringAddonPtr.Address;

        var componentList = selectIconStringAddon->GetComponentListById(3);

        if (componentList == null)
        {
            return;
        }

        foreach (uint index in Enumerable.Range(0, componentList->ListLength))
        {
            var listItemRenderer = componentList->ItemRendererList[index].AtkComponentListItemRenderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(2);
            if (text == null)
            {
                continue;
            }
            try
            {
                if (item.Shop.ShopName != null && item.Shop.ShopName.Equals(text->NodeText.ToString()))
                {
                    text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(ImGuiColors.DalamudRed);
                    return;
                }
            }
            catch (NullReferenceException)
            {
                continue;
            }
        }
    }

    private unsafe void HighlightInclusionShopAddon()
    {
        if (items.Count == 0) return;
        var item = items[0];
        
        var inclusionShopAddonPtr = Service.GameGui.GetAddonByName("InclusionShop");

        if (inclusionShopAddonPtr == nint.Zero)
        {
            return;
        }

        var inclusionShopAddon = (AtkUnitBase*)inclusionShopAddonPtr.Address;

        var category = (AtkComponentDropDownList*)inclusionShopAddon->GetComponentByNodeId(7);
        var subcategory = (AtkComponentDropDownList*)inclusionShopAddon->GetComponentByNodeId(9);
        var itemList = (AtkComponentTreeList*)inclusionShopAddon->GetComponentByNodeId(19);

        if (category == null || subcategory == null)
        {
            return;
        }

        foreach (uint index in Enumerable.Range(0, category->List->ListLength))
        {
            var listItemRenderer = category->List->ItemRendererList[index].AtkComponentListItemRenderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(4);
            if (text == null)
            {
                continue;
            }
            var textValue = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            try
            {
                if (item.Shop.ShopName != null && item.Shop.ShopName.Equals(textValue))
                {
                    text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(ImGuiColors.DalamudRed);
                    return;
                }
            }
            catch (NullReferenceException)
            {
                continue;
            }
        }
        foreach (uint index in Enumerable.Range(0, subcategory->List->ListLength))
        {
            var listItemRenderer = subcategory->List->ItemRendererList[index].AtkComponentListItemRenderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(4);
            if (text == null)
            {
                continue;
            }
            var textValue = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            try
            {
                if (item.Shop.ShopName != null && item.Shop.ShopName.Equals(textValue))
                {
                    text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(ImGuiColors.DalamudRed);
                    return;
                }
            }
            catch (NullReferenceException)
            {
                continue;
            }
        }

        if (itemList == null)
        {
            return;
        }

        foreach (var i in itemList->Items)
        {
            var listItemRenderer = i.Value->Renderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(5);
            if (text == null)
            {
                continue;
            }
            var itemName = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            if (item.Name.Equals(itemName))
            {
                text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(ImGuiColors.DalamudRed);
                text->SetText(itemName);
                return;
            }
        }
    }

    private unsafe void HighlightShopExchangeCurrencyAddon()
    {
        if (items.Count == 0) return;
        var item = items[0];
        var shopExchangeCurrencyAddonPtr = Service.GameGui.GetAddonByName("ShopExchangeCurrency");

        if (shopExchangeCurrencyAddonPtr == nint.Zero)
        {
            return;
        }

        var shopExchangeCurrencyAddon = (AtkUnitBase*)shopExchangeCurrencyAddonPtr.Address;


        // highlight tab
        var tabs = (AtkResNode*)shopExchangeCurrencyAddon->GetNodeById(7);

        if (tabs != null)
        {
            AtkResNode* othersTab = tabs->ChildNode;
            AtkResNode* accessoriesTab = othersTab->PrevSiblingNode;
            AtkResNode* armorTab = accessoriesTab->PrevSiblingNode;
            AtkResNode* weaponsTab = armorTab->PrevSiblingNode;
            // if (othersTab != null && item.Shop.SpecialShopCategory == 4)
            // {
            //     othersTab->GetAsAtkComponentRadioButton()->GetTextNodeById(2)->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
            // }
            // if (accessoriesTab != null && _itemInfo?.SpecialShopCategory == 3)
            // {
            //     accessoriesTab->GetAsAtkComponentRadioButton()->GetTextNodeById(2)->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
            // }
            // if (armorTab != null && _itemInfo?.SpecialShopCategory == 2)
            // {
            //     armorTab->GetAsAtkComponentRadioButton()->GetTextNodeById(2)->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
            // }
            // if (weaponsTab != null && _itemInfo?.SpecialShopCategory == 1)
            // {
            //     weaponsTab->GetAsAtkComponentRadioButton()->GetTextNodeById(2)->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
            // }
        }

        // highlight item in list
        var itemList = (AtkComponentTreeList*)shopExchangeCurrencyAddon->GetComponentByNodeId(19);

        if (itemList == null)
        {
            itemList = (AtkComponentTreeList*)shopExchangeCurrencyAddon->GetComponentByNodeId(20);
        }

        if (itemList == null)
        {
            return;
        }

        foreach (var i in itemList->Items)
        {
            var listItemRenderer = i.Value->Renderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(3);
            if (text == null)
            {
                text = (AtkTextNode*)listItemRenderer->GetTextNodeById(8);
            }
            if (text == null)
            {
                continue;
            }
            var itemName = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            if (item.Name.Equals(itemName))
            {
                text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
                text->SetText(itemName);
                return;
            }
        }
    }

    private unsafe void HighlightShopExchangeItemAddon()
    {
        if (items.Count == 0) return;
        var item = items[0];
        
        var shopExchangeItemAddonPtr = Service.GameGui.GetAddonByName("ShopExchangeItem");

        if (shopExchangeItemAddonPtr == nint.Zero)
        {
            return;
        }

        var shopExchangeItemAddon = (AtkUnitBase*)shopExchangeItemAddonPtr.Address;

        var itemList = (AtkComponentTreeList*)shopExchangeItemAddon->GetComponentByNodeId(20);

        if (itemList == null)
        {
            return;
        }

        foreach (var i in itemList->Items)
        {
            var listItemRenderer = i.Value->Renderer;
            if (listItemRenderer == null)
            {
                continue;
            }
            var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(7);
            if (text == null)
            {
                continue;
            }
            var itemName = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText();
            if (item.Name.Equals(itemName))
            {
                text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
                text->SetText(itemName);
                return;
            }
        }
    }

    private unsafe void HighlightCollectablesShopAddon()
    {
        if (items.Count == 0) return;
        var item = items[0];
        
        var collectablesShopAddonPtr = Service.GameGui.GetAddonByName("CollectablesShop");

        if (collectablesShopAddonPtr == nint.Zero)
        {
            return;
        }

        var collectablesShopAddon = (AtkUnitBase*)collectablesShopAddonPtr.Address;

        try
        {
            var shop = item.Shop.ShopName?.Contains("Oddly Specific Materials Exchange");
            var shopType = item.Shop.ShopName?.Split("\n")[1].Split("Oddly Specific Materials Exchange (")[1].Split(")")[0];
            // var index = Enum.GetValues<CollectablesShopIconIndex>()[Enum.GetNames<CollectablesShopIconIndex>().ToList().FindIndex(e => e == shopType)];
            // var itemCost = shop.Costs[0].Item2.Split(" min ")[0];
        
            // var radioButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId((uint)index);
            // radioButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
        
            var itemList = (AtkComponentTreeList*)collectablesShopAddon->GetComponentByNodeId(28);
        
            if (itemList == null)
            {
                return;
            }
        
            foreach (var i in itemList->Items)
            {
                var listItemRenderer = i.Value->Renderer;
                if (listItemRenderer == null)
                {
                    continue;
                }
                var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(4);
                if (text == null)
                {
                    continue;
                }
                var itemName = ((ReadOnlySeStringSpan)text->NodeText.AsSpan()).ExtractText().Split(" ")[0];
                if (item.Name.Equals(itemName))
                {
                    text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(color);
                    text->SetText(itemName);
                    return;
                }
            }
        }
        catch (InvalidOperationException)
        {
            return;
        }

        //var carpenterButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(3);
        //carpenterButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var blacksmithButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(4);
        //blacksmithButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var armorerButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(5);
        //armorerButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var goldsmithButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(6);
        //goldsmithButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var leatherworkerButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(7);
        //leatherworkerButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var weaverButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(8);
        //weaverButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var alchemistButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(9);
        //alchemistButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var culinarianButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(10);
        //culinarianButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var minerButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(11);
        //minerButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var botanistButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(12);
        //botanistButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //var fisherButton = (AtkComponentRadioButton*)collectablesShopAddon->GetComponentByNodeId(13);
        //fisherButton->ButtonBGNode->Color = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);


        //var itemList = (AtkComponentTreeList*)collectablesShopAddon->GetComponentByNodeId(28);

        //if (itemList == null)
        //{
        //    return;
        //}

        //foreach (var item in itemList->Items)
        //{
        //    var listItemRenderer = item.Value->Renderer;
        //    if (listItemRenderer == null)
        //    {
        //        continue;
        //    }
        //    var text = (AtkTextNode*)listItemRenderer->GetTextNodeById(7);
        //    if (text == null)
        //    {
        //        continue;
        //    }
        //    var itemName = SeString.Parse(text->GetText()).TextValue;
        //    if (itemName == _itemName)
        //    {
        //        text->TextColor = Dalamud.Utility.Numerics.VectorExtensions.ToByteColor(Service.Configuration.ShopHighlightColor);
        //        // strangely, it doesn't seem like the list gets its color updated until we set the text below
        //        text->SetText(SeString.Parse(text->GetText()).TextValue);
        //    }
        //}
    }

    public void SetItemInfo(ShopItem item)
    {
        if (items.Count != 0)
            items = new List<ShopItem>();
        items.Add(item);
        PluginLog.Debug("Added: " + item.Name);
        foreach (var i in items)
            PluginLog.Verbose(i.Name);
    }

    public void ClearAllInfo()
    {
        items = [];
    }

    public void Dispose()
    {
        Service.Framework.Update -= Framework_OnUpdate;
    }
}
