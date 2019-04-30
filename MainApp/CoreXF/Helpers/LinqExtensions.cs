
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreXF
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this List<T> source, Action<T> action)
        {
            for(int i = 0;i < source.Count(); i++)
            {
                action(source[i]);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
