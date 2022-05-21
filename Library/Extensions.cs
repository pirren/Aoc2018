using System.Drawing;

namespace Aoc2018.Library
{
    public static partial class Ext
    {
        public static int ToInt(this string str)
            => int.Parse(str);

        public static int ToInt(this char ch)
            => int.Parse(ch.ToString());

        public static float ToFloat(this string str)
            => float.Parse(str);

        public static T PopAt<T>(this List<T> list, int idx)
        {
            T r = list[idx];
            list.RemoveAt(idx);
            return r;
        }

        public static T? PopLast<T>(this List<T> list)
        {
            T? r = list.LastOrDefault();
            list.RemoveAt(list.Count - 1);
            return r;
        }

        public static T[,] Populate<T>(this T[,] arr, T val)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
                for (int y = 0; y < arr.GetLength(1); y++)
                    arr[x, y] = val;
            return arr;
        }

        public static IEnumerable<T> Flatten<T>(this T[,] map)
        {
            for (int y = 0; y < map.GetLength(0); y++)
                for (int x = 0; x < map.GetLength(1); x++)
                    yield return map[y, x];
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
            => enumerable.Where(x => x != null).Select(s => s!);

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source) action(item);
        }

        public static int Manhattan(this (int x, int y) p, (int x, int y) other)
            => Math.Abs(other.x - p.x) + Math.Abs(other.y - p.y);
    }
}
