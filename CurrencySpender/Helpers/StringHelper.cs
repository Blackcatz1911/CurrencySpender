using System.Text;

namespace CurrencySpender.Helpers
{
    internal static class StringHelper
    {
        public static string FormatString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder();
            int count = 0;
            string[] items = { "", ".", "," };
            string insert = items[C.Seperator];

            // Iterate the string in reverse to add separators
            for (int i = str.Length - 1; i >= 0; i--)
            {
                sb.Insert(0, str[i]); // Insert at the beginning
                count++;
                if (count % 3 == 0 && i > 0) // Add separator every 3 digits, except at the start
                {
                    sb.Insert(0, insert);
                }
            }

            return sb.ToString();
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
