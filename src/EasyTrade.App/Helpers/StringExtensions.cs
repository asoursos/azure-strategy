using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTradeCosmos
{
    public static class StringExtensions
    {
        private const string EllipsisChar = "…";

        public static string Ellipsis(this string input, int length)
        {
            if (input.Length <= length)
                return input;

            int pos = input.IndexOf(" ", length);
            if (pos >= 0)
                return input.Substring(0, pos) + EllipsisChar;

            return input.Substring(0, length) + EllipsisChar;
        }
    }
}
