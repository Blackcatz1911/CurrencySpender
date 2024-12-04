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

        public static string ToSemVer(string microsoftVersion)
        {
            // Split the version into parts
            var parts = microsoftVersion.Split('.');

            // SemVer only uses the first three components (Major.Minor.Patch)
            if (parts.Length >= 3)
            {
                return $"{parts[0]}.{parts[1]}.{parts[2]}";
            }

            return "";
        }
    }
}
