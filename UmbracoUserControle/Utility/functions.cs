using System.Collections.Generic;

namespace UmbracoUserControl.Utility
{
    public static class Functions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var cur in enumerable)
            {
                collection.Add(cur);
            }
        }
    }
}