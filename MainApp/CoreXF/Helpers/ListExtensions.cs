using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CoreXF
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList list,IEnumerable<T> collection)
        {
            foreach(var elm in collection)
            {
                list.Add(elm);
            }
        }
    }
}
