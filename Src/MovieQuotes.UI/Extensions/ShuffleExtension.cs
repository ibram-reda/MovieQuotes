using System;
using System.Collections.Generic;

namespace MovieQuotes.UI.Extensions;

public static class ShuffleExtension
{
    public static IList<T> Sort<T>(this IList<T> c, Comparison<T> comparison) where T : class
    {
        List<T> sortableList = new(c);
        sortableList.Sort(comparison);
        for (int i = 0; i < sortableList.Count; i++)
        {
            T d = sortableList[i];
            if (c[i] != d)
                c[i] = d;
        }
        return c;
    }
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}