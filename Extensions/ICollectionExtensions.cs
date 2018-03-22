using System.Collections.Generic;

namespace ReinventionBot.Extensions
{
    internal static class ICollectionExtensions
    {
        internal static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
        {
            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }
        }
    }
}