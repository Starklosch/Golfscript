using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golfscript
{
    static class Extensions
    {
        public static string Format<T>(this IEnumerable<T> array)
        {
            if (array.Count() <= 0)
                return "[]";

            if (array.Count() == 1)
                return $"[{array.First()}]";

            var sb = new StringBuilder();
            sb.Append("[");

            foreach (var item in array)
                sb.Append(item).Append(' ');

            sb.Length--;
            sb.Append("]");

            return sb.ToString();
        }
    }
}
