using System;
using System.Collections.Generic;
using System.Linq;
using SuperGameSystemBasic.Basic;

namespace SuperGameSystemBasic.Utils
{
    public static class Utilities
    {
        public static List<T> Shuffle<T>(this IEnumerable<T> list) => list.ToList().Shuffle();
        public static List<TValue> Shuffle<TValue>(this List<TValue> values)
        {
            var randomList = new List<TValue>();
            if (values == null || values.Count == 0) return randomList;
            while (values.Count > 0)
            {
                var randomIndex = (int)(BuiltInFunctions.Random.NextDouble() * values.Count);
                randomList.Add(values[randomIndex]); //add it to the new, random list
                values.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            return randomList; //return the new random list
        }

        public static int LevenshteinDistance<T>(T[] left, T[] right) where T : IEquatable<T>
        {
            var n = left.Length;
            var m = right.Length;
            var d = new int[n + 1, m + 1];

            // shortcut calculation for zero-length strings
            if (n == 0) return m;
            if (m == 0) return n;

            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }
            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (var i = 1; i <= n; i++)
            for (var j = 1; j <= m; j++)
            {
                var cost = right[j - 1].Equals(left[i - 1]) ? 0 : 1;

                d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }

            return d[n, m];
        }
    }
}
