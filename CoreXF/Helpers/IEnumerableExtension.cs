
using System.Collections;

namespace CoreXF
{
    public static class IEnumerableExtension
    {
        public static int Count(this IEnumerable coll)
        {
            int i = 0;
            foreach (var elm in coll)
                i++;

            return i;
        }
    }
}
