using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.ExternalAuth.NovellActiveDirectory.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> newValues)
        {
            var arr = newValues.ToArray();

            foreach (var value in arr)
            {
                source.Add(value);
            }
        }
    }
}
