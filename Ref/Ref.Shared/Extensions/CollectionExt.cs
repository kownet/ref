using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Extensions
{
    public static class CollectionExt
    {
        public static bool AnyAndNotNull<T>(this IEnumerable<T> source)
            => source != null && source.Any();

        public static TSource SecondLast<TSource>(this IEnumerable<TSource> source)
            => source.Reverse().Skip(1).Take(1).FirstOrDefault();
    }
}