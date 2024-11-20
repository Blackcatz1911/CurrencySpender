//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Lumina.Excel.Sheets;
//using Lumina.Extensions;
//using System.Xml.Linq;
//using FFXIVClientStructs.FFXIV.Client.Game.Event;
//using ECommons;

//namespace CurrencySpender.Data.Generators
//{
//    public static class SpecialShopGen
//    {
//        public static List<int> list = new List<int>();

//        public static async void init()
//        {
//            foreach (SpecialShop SpecialShop_ in Service.DataManager.GetExcelSheet<SpecialShop>())
//            {
//                foreach (var cur in C.Currencies)
//                {
//                    if(cur.ItemId == SpecialShop_.UseCurrencyType)
//                    {
//                        list.Add(SpecialShop_);
//                    }
//                }
//            }

//            foreach (CollectablesShop CollectablesShop_ in Service.DataManager.GetExcelSheet<CollectablesShop>())
//            {
//                foreach (var cur in C.Currencies)
//                {
//                    if (cur.ItemId == CollectablesShop_.)
//                    {
//                        list.Add(SpecialShop_);
//                    }
//                }
//            }
//        }
//    }
//}
