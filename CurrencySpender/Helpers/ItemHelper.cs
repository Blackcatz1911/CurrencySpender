using CurrencySpender.Classes;
using CurrencySpender.Data;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Exd;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace CurrencySpender.Helpers
{
    public class ItemHelper
    {
        public static bool Debug = false;

        private static readonly Dictionary<uint, List<uint>> Containers = new()
        {
            // Bronze Triad Card 
            { 10128, new List<uint> { 9782, 9809, 9797, 9796, 9779, 16762, 9783, 16760, 16759, 9776, 9798, 9775, 9795, 16765, 15621 } },
            // Silver Triad Card 
            { 10129, new List<uint> { 9785, 14199, 9813, 9814, 9811, 9786, 9788, 9828, 9827, 9792, 9787, 9790, 9812, 9821 } },
            // Gold Triad Card
            { 10130, new List<uint> { 9800, 9829, 9805, 14192, 9837, 9825, 9836, 9799, 9801, 9824, 9838, 9826, 9822, 9839, 9847 } },
            // Mythril Triad Card 
            { 13380, new List<uint> { 9843, 14193, 13368, 9810, 9823, 9841, 13372, 9844, 13367 } },
            // Imperial Triad Card 
            { 17702, new List<uint> { 17686, 16775, 17681, 17682, 16774, 13378 } },
            // Dream Triad Card    
            { 28652, new List<uint> { 28661, 26767, 28657, 28653, 28655, 26772, 28658, 28660, 26765, 26768, 26766 } },
            // Platinum Triad Card 
            { 10077, new List<uint> { 9830, 9842, 9840, 14208, 15872, 9828, 9851, 9831, 9834, 9826, 9822, 9848 } },
            // Materiel Container 3.0 
            { 36635, new List<uint>
                {
                    9350, 12051, 6187, 15441, 6175, 7564, 6186, 6203, 6177, 17525, 15440, 14098, 6003, 12055, 6199, 6205,
                    16570, 16568, 6189, 15447, 8193, 9347, 14103, 12054, 8194, 12061, 6191, 12069, 13279, 6179, 12058, 13283,
                    12056, 9348, 7568, 6004, 8196, 8201, 7566, 10071, 6204, 6173, 14100, 9349, 8200, 8205, 16564, 8202, 12052,
                    12057, 13275, 7559, 6192, 16572, 6208, 6195, 12062, 7567, 6188, 6174, 8199, 6185, 8195, 12053, 12049, 6005,
                    6213, 6200, 6190, 16573, 17527, 14093, 13284, 13276, 14095, 6214, 15436, 15437, 14094, 6184, 14083, 6183, 6198,
                    8192, 6209, 6178
                } },
            // Materiel Container 4.0 
            { 36636, new List<uint>
                {
                    24902, 21921, 21063, 20529, 20530, 21920, 24002, 20524, 24635, 23027, 24001, 23023, 20533, 24219, 24630, 21052,
                    20542, 24903, 20538, 21064, 20541, 21058, 20536, 23032, 23998, 20525, 21916, 20531, 21193, 23989, 24634, 21059,
                    21922, 21919, 20528, 21911, 20547, 20539, 24000, 21918, 21055, 20544, 20546, 21915, 21060, 21917, 20537, 21057,
                    23030, 21065, 20545, 23028, 24639, 23036, 24640
                } }
        };

        public static Dictionary<uint, (uint, uint)> ContainerUnlocked = new()
        {
            { 10128, (0,0) },
            { 10129, (0,0) },
            { 10130, (0,0) },
            { 13380, (0,0) },
            { 17702, (0,0) },
            { 28652, (0,0) },
            { 10077, (0,0) },
            { 36635, (0,0) },
            { 36636, (0,0) },
        };

        public static uint GetItemIDFromString(string arg)
        {
            var ret = Service.DataManager.GetExcelSheet<Item>().FirstOr0(x => x.Name == arg);
            if (ret.RowId != 0) { return ret.RowId; }
            return 0;
        }

        public static unsafe bool IsUnlocked(uint id)
        {
            Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(id);
            //if(item.RowId == 44936) PluginLog.Debug($"{item.Name.ExtractText()} - {item.RowId}");
            if (Containers.ContainsKey(id))
            {
                if (ContainerUnlocked.TryGetValue(id, out (uint, uint) tuple))
                {
                    if (tuple.Item2 > tuple.Item1) return false;
                    if (tuple.Item2 != 0 && tuple.Item1 != 0 && tuple.Item2 == tuple.Item1) return true;
                }
                uint unlocked = 0;
                uint max = 0;
                if (Containers.TryGetValue(id, out List<uint>? values))
                {
                    foreach (var value in values)
                    {
                        if (IsUnlocked(value)) unlocked++;
                        else
                        {
                            var missingItem =  Service.DataManager.GetExcelSheet<Item>().GetRow(value);
                            PluginLog.Debug($"{missingItem.Name} - {missingItem.RowId} not unlocked");
                        }
                        max++;
                    }
                }
                ContainerUnlocked[id] = (unlocked, max);
                PluginLog.Debug($"{item.Name.ExtractText()} - {item.RowId} - {unlocked}/{max}");
                if (max == unlocked) return true;
                return false;
            }
            if (item.ItemUICategory.RowId == 94 && item.Name.ExtractText().Contains("Faded"))
            {
                if (Debug) PluginLog.Verbose("Item is Faded Copy of Orchestration Roll");
                var new_name = item.Name.ExtractText().Replace("Faded Copy of ", "") + " Orchestrion Roll";
                var rowId = GetItemIDFromString(new_name);
                if (Debug) PluginLog.Verbose("new_name: '" + new_name + "'");
                if (Debug) PluginLog.Verbose("row: " + rowId.ToString());
                if (rowId != 0)
                {
                    var new_item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(rowId);
                    var new_additionalData = new_item.AdditionalData.RowId;
                    return UIState.Instance()->PlayerState.IsOrchestrionRollUnlocked(new_additionalData);
                }
            }

            if (item.ItemAction.RowId == 0)
                return false;

            switch ((ItemActionType)item.ItemAction.Value.Action.RowId)
            {
                case ItemActionType.Companion:
                    return UIState.Instance()->IsCompanionUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.BuddyEquip:
                    return UIState.Instance()->Buddy.CompanionInfo.IsBuddyEquipUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.Mount:
                    return PlayerState.Instance()->IsMountUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.SecretRecipeBook:
                    return PlayerState.Instance()->IsSecretRecipeBookUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.UnlockLink:
                    // PluginLog.Information($"{item.Name.ExtractText()} - {item.ItemAction.RowId} - {(ItemActionType)item.ItemAction.Value.Type} - {UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0])}");
                    return UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.TripleTriadCard when item.AdditionalData.Is<TripleTriadCard>():
                    return UIState.Instance()->IsTripleTriadCardUnlocked((ushort)item.AdditionalData.RowId);

                case ItemActionType.FolkloreTome:
                    return PlayerState.Instance()->IsFolkloreBookUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.OrchestrionRoll when item.AdditionalData.Is<Orchestrion>():
                    return PlayerState.Instance()->IsOrchestrionRollUnlocked(item.AdditionalData.RowId);

                case ItemActionType.FramersKit:
                    return PlayerState.Instance()->IsFramersKitUnlocked(item.AdditionalData.RowId);

                case ItemActionType.Ornament:
                    return PlayerState.Instance()->IsOrnamentUnlocked(item.ItemAction.Value.Data[0]);

                case ItemActionType.Glasses:
                    return PlayerState.Instance()->IsGlassesUnlocked((ushort)item.AdditionalData.RowId);
            }

            var row = ExdModule.GetItemRowById(item.RowId);
            return row != null && UIState.Instance()->IsItemActionUnlocked(row) == 1;
        }
        public enum ItemActionType : ushort
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
        public static ItemType GetItemTypes(uint id)
        {
            Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(id);
            if (item.RowId == 21072)
            {
                return ItemType.Venture;
            }

            if (item.RowId == 50058)
            {
                return ItemType.None;
            }

            var cat = item.ItemUICategory.RowId;
            var name = item.Name.ExtractText();
            var untradable = item.IsUntradable;
            ItemType curType = ItemType.None;
            if (Containers.ContainsKey(id)) curType |= ItemType.Collectable;
            //if (item_.ItemAction.RowId != 0) curType |= ItemType.Collectable;
            if (P.Currencies.Where(cur => cur.ItemId == item.RowId).ToList().Count > 0) curType |= ItemType.Currency;
            
            // if(item.RowId == 46321)
            //     PluginLog.Information($"46321: {item.Name} cat:{cat} item.ItemAction.Value.Type:{item.ItemAction.Value.Type}");
            
            if(name.Contains("Ballroom Etiquette") || name.Contains("Framer's Kit") || name.Contains("Battlefield Etiquette") ||
                name.Contains("The Faces We Wear") || name.Contains("Modern Aesthetics") || name.Contains("Maxims of Mahjong"))
            {
                curType |= ItemType.Collectable;
            }
            if(cat == 63)
            {
                if(name.Contains("Barding") || item.ItemAction.Value.Action.RowId == 1322 || item.ItemAction.Value.Action.RowId == 29459 ||
                    item.ItemAction.Value.Action.RowId == 2633 || item.ItemAction.Value.Action.RowId == 2136) //2633 Riding Map
                {
                    curType |= ItemType.Collectable;
                }
            }
            if(cat == 81 || cat == 86 || cat == 94)
            {
                curType |= ItemType.Collectable;
            }
            if(!untradable)
                curType |= ItemType.Tradeable;
            if (curType == ItemType.None && item.ItemAction.Value.Action.RowId == 20086)
            {
                curType |= ItemType.Collectable;
            }
            return curType;
        }
        public static CollectableType GetCollectableType(Item item, ItemType itemTypes)
        {
            return GetCollectableTypeInternal(item, itemTypes);
        }

        public static CollectableType GetCollectableType(RowRef<Item> item, ItemType itemTypes)
        {
            return GetCollectableTypeInternal(item.Value, itemTypes);
        }

        private static CollectableType GetCollectableTypeInternal(Item item, ItemType itemTypes)
        {
            if (!itemTypes.HasFlag(ItemType.Collectable)) { return CollectableType.None; }
    
            var cat = item.ItemUICategory.RowId;
            var name = item.Name.ExtractText();
    
            if (Containers.ContainsKey(item.RowId)) return CollectableType.Container;
    
            if (name.Contains("Ballroom Etiquette") || name.Contains("Battlefield Etiquette"))
            {
                return CollectableType.Scroll;
            }
    
            if (name.Contains("Framer's Kit")) return CollectableType.FramersKit;
            if (name.Contains("Maxims of Mahjong")) return CollectableType.Mahjong;
            if (name.Contains("The Faces We Wear")) return CollectableType.Facewear;
            if (name.Contains("Modern Aesthetics")) return CollectableType.Hairstyle;
    
            if (cat == 63)
            {
                if (name.Contains("Barding")) return CollectableType.Barding;
                if (item.ItemAction.Value.Action.RowId == 1322) return CollectableType.Mount;
                if (item.ItemAction.Value.Action.RowId == 29459) return CollectableType.FramersKit;
                if (item.ItemAction.Value.Action.RowId == 2633) return CollectableType.RidingMap;
                if (item.ItemAction.Value.Action.RowId == 2136) return CollectableType.MasterRecipes;
            }
            if (item.ItemAction.Value.Action.RowId == 20086) return CollectableType.FashionAccessory;
            if (cat == 81) return CollectableType.Minion;
            if (cat == 86) return CollectableType.TTCard;
            if (cat == 94) return CollectableType.Scroll;
    
            if (Debug) DuoLog.Debug("Collectable Type not found!");
            return CollectableType.None;
        }
        public static Dictionary<uint, uint> TribeByCurrency = new();
        public static void InitTribes()
        {
            TribeByCurrency.Clear();
            foreach (var tribe in Service.DataManager.GetExcelSheet<BeastTribe>())
            {
                if (tribe.RowId == 0 || tribe.CurrencyItem.RowId == 0) continue;
                TribeByCurrency[tribe.CurrencyItem.RowId] = tribe.RowId;
            }
            PluginLog.Debug($"InitTribes: mapped {TribeByCurrency.Count} tribe currencies");
        }

        // BeastReputationRank scale: Neutral=1, Recognized=2, Friendly=3, Trusted=4,
        // Respected=5, Honored=6, Sworn=7, Bloodsworn=8, Allied=9 (rank-up quests for
        // HW/SB+ tribes reach rank 8 = field+1; the special Allied quests reach rank 9)
        public const int RepNeutral = 1;
        public const int RepRecognized = 2;
        public const int RepFriendly = 3;
        public const int RepTrusted = 4;
        public const int RepRespected = 5;
        public const int RepHonored = 6;
        public const int RepSworn = 7;
        public const int RepBloodsworn = 8;
        public const int RepAllied = 9;
        
        private static readonly Dictionary<(uint ItemId, uint TribeId), int> RequiredReputation = new()
        {
            { (52254, 0), RepRecognized }, // Standard Spectrum Dye
            { (7621, 0), RepTrusted },     // Glamour Dispeller

            // Amalj'aa (1)
            { (16800, 1), RepFriendly },   // Smoulder Orchestrion Roll
            { (6686, 1), RepTrusted },     // Amalj'aa Supply Carriage
            { (6687, 1), RepTrusted },     // Amalj'aa Pavis Shield

            // Sylph (2)
            { (7028, 2), RepRecognized },  // Sylphic Silk
            { (5360, 2), RepRecognized },  // Wildfowl Feather
            { (17620, 2), RepFriendly },   // Flibbertigibbet Orchestrion Roll (quest derives Recognized, actual Friendly)
            { (6495, 2), RepTrusted },     // Sylphic Lamp Tree
            { (6496, 2), RepTrusted },     // Sylphic Lamppost

            // Kobold (3)
            { (7126, 3), RepTrusted },     // Automaton Digger
            { (7127, 3), RepTrusted },     // Kobold Furnace

            // Sahagin (4)
            { (7122, 4), RepTrusted },     // Sahagin Living Lamp
            { (7123, 4), RepTrusted },     // Sahagin Hanging Larder

            // Ixal (5)
            { (5684, 5), RepRespected },   // Gatherer's Guerdon Materia I
            { (5685, 5), RepRespected },   // Gatherer's Guerdon Materia II
            { (5686, 5), RepRespected },   // Gatherer's Guerdon Materia III
            { (5689, 5), RepRespected },   // Gatherer's Guile Materia I
            { (5690, 5), RepRespected },   // Gatherer's Guile Materia II
            { (5691, 5), RepRespected },   // Gatherer's Guile Materia III
            { (5694, 5), RepRespected },   // Gatherer's Grasp Materia I
            { (5695, 5), RepRespected },   // Gatherer's Grasp Materia II
            { (5696, 5), RepRespected },   // Gatherer's Grasp Materia III
            { (5699, 5), RepRespected },   // Craftsman's Competence Materia I
            { (5700, 5), RepRespected },   // Craftsman's Competence Materia II
            { (5701, 5), RepRespected },   // Craftsman's Competence Materia III
            { (5704, 5), RepRespected },   // Craftsman's Cunning Materia I
            { (5705, 5), RepRespected },   // Craftsman's Cunning Materia II
            { (5706, 5), RepRespected },   // Craftsman's Cunning Materia III
            { (5709, 5), RepRespected },   // Craftsman's Command Materia I
            { (5710, 5), RepRespected },   // Craftsman's Command Materia II
            { (5711, 5), RepRespected },   // Craftsman's Command Materia III

            // Vanu Vanu (6)
            { (12586, 6), RepRecognized }, // Birch Branch
            { (12723, 6), RepRecognized }, // Starflower
            { (16801, 6), RepRecognized }, // Coming Home Orchestrion Roll
            { (12735, 6), RepFriendly },   // Whiteloom
            { (12891, 6), RepFriendly },   // Birch Sap
            { (17486, 6), RepAllied },     // Zundu Head
            { (17487, 6), RepAllied },     // Zundu Body
            { (17488, 6), RepAllied },     // Zundu Arms
            { (17489, 6), RepAllied },     // Zundu Waist
            { (17618, 6), RepAllied },     // Zundu Legs

            // Vath (7)
            { (21072, 7), RepFriendly },   // Venture
            { (13582, 7), RepFriendly },   // Unidentifiable Bone
            { (13584, 7), RepFriendly },   // Unidentifiable Shell
            { (13586, 7), RepFriendly },   // Unidentifiable Ore
            { (13588, 7), RepFriendly },   // Unidentifiable Seeds
            { (4868, 7), RepFriendly },    // Gysahl Greens
            { (7895, 7), RepFriendly },    // Sylkis Bud
            { (7897, 7), RepFriendly },    // Mimett Gourd
            { (7898, 7), RepFriendly },    // Tantalplant
            { (7900, 7), RepFriendly },    // Pahsana Fruit
            { (17621, 7), RepFriendly },   // Piece of Mind Orchestrion Roll
            { (17490, 7), RepAllied },     // Gnath Thorax

            // Kojin (9)
            { (20038, 9), RepFriendly },   // Zekki Grouper
            { (20199, 9), RepFriendly },   // Amberjack
            { (21072, 9), RepFriendly },   // Venture
            { (24618, 9), RepAllied },     // Kojin Material Supplier Permit (q68700 field-0 quest)
            { (24619, 9), RepAllied },     // Kojin Junkmonger Permit
            { (24620, 9), RepAllied },     // Kojin Mender Permit
            { (24621, 9), RepAllied },     // Kojin Manservant Permit
            { (24638, 9), RepAllied },     // Wind-up Redback
            { (24901, 9), RepAllied },     // Zephyrous Zabuton

            // Ananta (10)
            { (21072, 10), RepFriendly },  // Venture
            { (22361, 10), RepFriendly },  // False Nails
            { (21840, 10), RepFriendly },  // Stuffed Ananta
            { (24637, 10), RepAllied },    // Wind-up Qalyana (q68700 field-0 quest)
            { (24617, 10), RepAllied },    // Ananta Metalworks

            // Namazu (11)
            { (21072, 11), RepFriendly },  // Venture
            { (23178, 11), RepFriendly },  // Stormsap
            { (22564, 11), RepFriendly },  // Stuffed Namazu
            { (22567, 11), RepFriendly },  // Basket of Steamed Buns
            { (24537, 11), RepAllied },    // Big One Festival Float (q68700 field-0 quest)
            { (24164, 11), RepAllied },    // Namazu Mask

            // Pixies (12)
            { (25186, 12), RepFriendly },  // Piety Materia VII
            { (25187, 12), RepFriendly },  // Heavens' Eye Materia VII
            { (25188, 12), RepFriendly },  // Savage Aim Materia VII
            { (25189, 12), RepFriendly },  // Savage Might Materia VII
            { (25190, 12), RepFriendly },  // Battledance Materia VII
            { (25197, 12), RepFriendly },  // Quickarm Materia VII
            { (25198, 12), RepFriendly },  // Quicktongue Materia VII
            { (26727, 12), RepFriendly },  // Piety Materia VIII
            { (26728, 12), RepFriendly },  // Heavens' Eye Materia VIII
            { (26729, 12), RepFriendly },  // Savage Aim Materia VIII
            { (26730, 12), RepFriendly },  // Savage Might Materia VIII
            { (26731, 12), RepFriendly },  // Battledance Materia VIII
            { (26738, 12), RepFriendly },  // Quickarm Materia VIII
            { (26739, 12), RepFriendly },  // Quicktongue Materia VIII

            // Qitari (13)
            { (25191, 13), RepFriendly },  // Gatherer's Guerdon Materia VII
            { (25192, 13), RepFriendly },  // Gatherer's Guile Materia VII
            { (25193, 13), RepFriendly },  // Gatherer's Grasp Materia VII
            { (26732, 13), RepFriendly },  // Gatherer's Guerdon Materia VIII
            { (26733, 13), RepFriendly },  // Gatherer's Guile Materia VIII
            { (26734, 13), RepFriendly },  // Gatherer's Grasp Materia VIII

            // Dwarves (14)
            { (31320, 14), RepFriendly },  // Slithersand
            { (25194, 14), RepFriendly },  // Craftsman's Competence Materia VII
            { (25195, 14), RepFriendly },  // Craftsman's Cunning Materia VII
            { (25196, 14), RepFriendly },  // Craftsman's Command Materia VII
            { (26735, 14), RepFriendly },  // Craftsman's Competence Materia VIII
            { (26736, 14), RepFriendly },  // Craftsman's Cunning Materia VIII
            { (26737, 14), RepFriendly },  // Craftsman's Command Materia VIII

            // Arkasodara (15)
            { (33917, 15), RepFriendly },  // Piety Materia IX
            { (33918, 15), RepFriendly },  // Heavens' Eye Materia IX
            { (33919, 15), RepFriendly },  // Savage Aim Materia IX
            { (33920, 15), RepFriendly },  // Savage Might Materia IX
            { (33921, 15), RepFriendly },  // Battledance Materia IX
            { (33928, 15), RepFriendly },  // Quickarm Materia IX
            { (33929, 15), RepFriendly },  // Quicktongue Materia IX
            { (33930, 15), RepFriendly },  // Piety Materia X
            { (33931, 15), RepFriendly },  // Heavens' Eye Materia X
            { (33932, 15), RepFriendly },  // Savage Aim Materia X
            { (33933, 15), RepFriendly },  // Savage Might Materia X
            { (33934, 15), RepFriendly },  // Battledance Materia X
            { (33941, 15), RepFriendly },  // Quickarm Materia X
            { (33942, 15), RepFriendly },  // Quicktongue Materia X

            // Omicrons (16)
            { (33922, 16), RepFriendly },  // Gatherer's Guerdon Materia IX
            { (33923, 16), RepFriendly },  // Gatherer's Guile Materia IX
            { (33924, 16), RepFriendly },  // Gatherer's Grasp Materia IX
            { (33935, 16), RepFriendly },  // Gatherer's Guerdon Materia X
            { (33936, 16), RepFriendly },  // Gatherer's Guile Materia X
            { (33937, 16), RepFriendly },  // Gatherer's Grasp Materia X

            // Loporrits (17)
            { (39595, 17), RepFriendly },  // Gripgel
            { (33925, 17), RepFriendly },  // Craftsman's Competence Materia IX
            { (33926, 17), RepFriendly },  // Craftsman's Cunning Materia IX
            { (33927, 17), RepFriendly },  // Craftsman's Command Materia IX
            { (33938, 17), RepFriendly },  // Craftsman's Competence Materia X
            { (33939, 17), RepFriendly },  // Craftsman's Cunning Materia X
            { (33940, 17), RepFriendly },  // Craftsman's Command Materia X

            // Pelupelu (18)
            { (41757, 18), RepFriendly },  // Piety Materia XI
            { (41758, 18), RepFriendly },  // Heavens' Eye Materia XI
            { (41759, 18), RepFriendly },  // Savage Aim Materia XI
            { (41760, 18), RepFriendly },  // Savage Might Materia XI
            { (41761, 18), RepFriendly },  // Battledance Materia XI
            { (41768, 18), RepFriendly },  // Quickarm Materia XI
            { (41769, 18), RepFriendly },  // Quicktongue Materia XI
            { (41770, 18), RepFriendly },  // Piety Materia XII
            { (41771, 18), RepFriendly },  // Heavens' Eye Materia XII
            { (41772, 18), RepFriendly },  // Savage Aim Materia XII
            { (41773, 18), RepFriendly },  // Savage Might Materia XII
            { (41774, 18), RepFriendly },  // Battledance Materia XII
            { (41781, 18), RepFriendly },  // Quickarm Materia XII
            { (41782, 18), RepFriendly },  // Quicktongue Materia XII

            // Mamool Ja (19)
            { (41762, 19), RepFriendly },  // Gatherer's Guerdon Materia XI
            { (41763, 19), RepFriendly },  // Gatherer's Guile Materia XI
            { (41764, 19), RepFriendly },  // Gatherer's Grasp Materia XI
            { (41775, 19), RepFriendly },  // Gatherer's Guerdon Materia XII
            { (41776, 19), RepFriendly },  // Gatherer's Guile Materia XII
            { (41777, 19), RepFriendly },  // Gatherer's Grasp Materia XII

            // Yok Huy (20)
            { (46252, 20), RepFriendly },  // Mason's Abrasive
            { (41765, 20), RepFriendly },  // Craftsman's Competence Materia XI
            { (41766, 20), RepFriendly },  // Craftsman's Cunning Materia XI
            { (41767, 20), RepFriendly },  // Craftsman's Command Materia XI
            { (41778, 20), RepFriendly },  // Craftsman's Competence Materia XII
            { (41779, 20), RepFriendly },  // Craftsman's Cunning Materia XII
            { (41780, 20), RepFriendly },  // Craftsman's Command Materia XII
        };

        public static int MapItemToRequiredReputation(uint itemId, uint questId = 0, uint tribeId = 0)
        {
            if (RequiredReputation.TryGetValue((itemId, tribeId), out var ret) ||
                RequiredReputation.TryGetValue((itemId, 0), out ret))
            {
                var name = Service.DataManager.GetExcelSheet<Item>().GetRow(itemId).Name;
                PluginLog.Verbose($"ID: {itemId} | Name: {name} | Reputation: {ret} (override)");
                return ret;
            }
            if (questId != 0 && tribeId != 0)
            {
                var quest = Service.DataManager.GetExcelSheet<Quest>().GetRow(questId);
                if (quest.BeastTribe.RowId == tribeId && quest.BeastReputationRank.RowId != 0)
                {
                    ret = (int)quest.BeastReputationRank.RowId + 1;
                    var name = Service.DataManager.GetExcelSheet<Item>().GetRow(itemId).Name;
                    PluginLog.Verbose($"ID: {itemId} | Name: {name} | Reputation: {ret} (quest-derived)");
                    return ret;
                }
            }
            return RepNeutral;
        }
        public static unsafe bool IsReputationReached(uint itemId, uint tribeId, uint questId = 0)
        {
            var tribe = Service.DataManager.GetExcelSheet<BeastTribe>().GetRow(tribeId);
            var itemRep = MapItemToRequiredReputation(itemId, questId, tribeId);
            var playerRep = GetPlayerTribeRank(tribeId);
            PluginLog.Verbose($"Tribe {tribe.Name} | itemRep: {itemRep} | playerRep: {playerRep}");
            return itemRep <= playerRep;
        }

        public static unsafe byte GetPlayerTribeRank(uint tribeId)
        {
            byte rank = PlayerState.Instance()->GetBeastTribeRank((byte)tribeId);
            var tribe = Service.DataManager.GetExcelSheet<BeastTribe>().GetRow(tribeId);
            if (tribe.Expansion.RowId != 0
                && tribe.IntersocietalQuest.IsValid
                && QuestManager.IsQuestComplete(tribe.IntersocietalQuest.RowId))
            {
                rank++;
            }
            return rank;
        }
    }
}
