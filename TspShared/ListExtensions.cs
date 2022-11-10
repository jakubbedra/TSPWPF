using System;
using System.Collections.Generic;
using System.Linq;

namespace TspShared;

public static class ListExtensions
{
    private static Random _random = new Random();

    public static void AddAll<T>(this List<T> list, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            list.Add(item);
        }
    }
    
    public static T GetRandom<T>(this List<T> list, T item)
    {
        List<T> copy = new List<T>(list);
        return copy.OrderBy(i => _random.Next()).First();
    }
    
    public static T GetRandomOtherThan<T>(this List<T> list, T item)
    {
        List<T> copy = new List<T>(list);
        copy.OrderBy(i => _random.Next()).ToList();
        return copy[0].Equals(item) ? (copy.Count > 1 ? copy[1] : copy[0]) : copy[0];
    }
}