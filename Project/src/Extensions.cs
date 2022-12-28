﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Golfscript
{
    static class Extensions
    {
        public static IEnumerable<T> Difference<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            foreach (var item in left)
            {
                if (!right.Contains(item))
                    yield return item;
            }
        }

        public static IEnumerable<int> FindAll<T>(this List<T> left, List<T> right, int offset = 0)
        {
            int match = 0;
            for (int i = 0; i < left.Count; i++)
            {
                if (match == right.Count)
                {
                    yield return i - right.Count;
                    match = 0;
                }

                match = left[i].Equals(right[match]) ? match + 1 : 0;
            }
        }


        public static IEnumerable<IEnumerable<T>> Split<T>(this List<T> left, List<T> right)
        {
            int start = 0;
            foreach (var index in left.FindAll(right))
            {
                yield return left.GetRange(start, index - start);
                start = index + right.Count;
            }
            yield return left.GetRange(start, left.Count - start);
        }

        public static IEnumerable<T> SymmetricDifference<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            var union = left.Union(right);
            var intersection = left.Intersect(right);
            return union.Except(intersection);
        }

        public static string Union(this string left, string right)
        {
            return string.Join("", (left + right).Distinct());
        }

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

        public static string Format<T>(this IEnumerable<T> array)
        {
            if (!array.Any())
                return "[]";

            if (array.Count() == 1)
                return $"[{array.First()}]";

            var sb = new StringBuilder();
            sb.Append('[');

            foreach (var item in array)
                sb.Append(item).Append(' ');

            sb.Length--;
            sb.Append(']');

            return sb.ToString();
        }
    }
}
