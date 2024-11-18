using System;

namespace CurrencySpender.Helpers
{
    internal static class StringHelper
    {
        public static string FormatString(String str)
        {
            int len = str.Length;
            var reverse = Reverse(str);
            var str_ = "";
            for (int i = 0; i < len; i++)
            {
                if (i % 3 == 0 && i != 0)
                    str_ += ".";
                str_ += reverse[i];
            }
            return Reverse(str_);
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
