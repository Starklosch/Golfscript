using System.Linq;
using System.Text;

namespace Golfscript.Helpers
{
    internal static class StringHelpers
    {

        public static string Difference(this string left, string right)
        {
            var sb = new StringBuilder();
            foreach (var item in left)
            {
                if (!right.Contains(item))
                    sb.Append(item);
            }
            return sb.ToString();
        }

        public static string Intersect(this string left, string right)
        {
            var sb = new StringBuilder();
            foreach (var item in left)
            {
                if (right.Contains(item))
                    sb.Append(item);
            }
            return sb.ToString();
        }

        public static string SymmetricDifference(this string left, string right)
        {
            var union = left.Union(right);
            var intersection = left.Intersect(right);
            return union.Difference(intersection);
        }

        public static string Union(this string left, string right)
        {
            return string.Join("", (left + right).Distinct());
        }
    }
}