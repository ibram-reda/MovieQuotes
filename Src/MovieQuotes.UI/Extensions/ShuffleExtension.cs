using System;
using System.Collections.Generic;

namespace MovieQuotes.UI.Extensions;

public static class ShuffleExtension
{
    public static void Shuffle<T>(this IList<T> list)
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
    }
}