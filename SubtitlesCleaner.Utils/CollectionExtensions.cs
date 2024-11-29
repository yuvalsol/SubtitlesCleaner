using System.Linq;

namespace System.Collections.Generic
{
    public static partial class CollectionExtensions
    {
        #region Visual Studio Search and Replace

        // (?<lst>[a-zA-Z0-9._]+) == null \|\| \k<lst>\.(?:Count|Length)(?:\(\))? == 0
        // ${lst}.IsNullOrEmpty()

        // (?<lst>[a-zA-Z0-9._]+) != null && \k<lst>\.(?:Count|Length)(?:\(\))? > 0
        // ${lst}.HasAny()

        // (?<lst>[a-zA-Z0-9._]+) != null && \k<lst>\.(?:Count|Length)(?:\(\))? == 1
        // ${lst}.HasSingle()

        // (?<lst>[a-zA-Z0-9._]+) != null && \k<lst>\.(?:Count|Length)(?:\(\))? > 1
        // ${lst}.HasMoreThanOne()

        #endregion

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source == null || !source.Any(predicate);
        }

        public static bool IsNullOrEmpty<TSource>(this ICollection<TSource> source)
        {
            return source == null || source.Count == 0;
        }

        public static bool HasAny<TSource>(this IEnumerable<TSource> source)
        {
            return source != null && source.Any();
        }

        public static bool HasAny<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source != null && source.Any(predicate);
        }

        public static bool HasAny<TSource>(this ICollection<TSource> source)
        {
            return source != null && source.Count > 0;
        }

        public static bool HasSingle<TSource>(this IEnumerable<TSource> source)
        {
            return source != null && source.Any() && !source.Skip(1).Any();
        }

        public static bool HasSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.HasEqualsTo(1, predicate);
        }

        public static bool HasSingle<TSource>(this ICollection<TSource> source)
        {
            return source != null && source.Count == 1;
        }

        public static bool HasMoreThanOne<TSource>(this IEnumerable<TSource> source)
        {
            return source != null && source.Skip(1).Any();
        }

        public static bool HasMoreThanOne<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.HasMoreThan(1, predicate);
        }

        public static bool HasMoreThanOne<TSource>(this ICollection<TSource> source)
        {
            return source != null && source.Count > 1;
        }

        private static readonly Func<int, int, bool> EqualsTo = (itemsCount, numItems) => itemsCount == numItems;
        private static readonly Func<int, int, bool> LessThan = (itemsCount, numItems) => itemsCount < numItems;
        private static readonly Func<int, int, bool> LessThanOrEqualsTo = (itemsCount, numItems) => itemsCount <= numItems;
        private static readonly Func<int, int, bool> MoreThan = (itemsCount, numItems) => itemsCount > numItems;
        private static readonly Func<int, int, bool> MoreThanOrEqualsTo = (itemsCount, numItems) => itemsCount >= numItems;

        public static bool HasMoreThanOrEqualsTo<TSource>(this IEnumerable<TSource> source, int numItems, Func<TSource, bool> predicate)
        {
            return HasItemsCount(source, numItems, predicate, MoreThanOrEqualsTo, EqualsTo, true);
        }

        public static bool HasMoreThan<TSource>(this IEnumerable<TSource> source, int numItems, Func<TSource, bool> predicate)
        {
            return HasItemsCount(source, numItems, predicate, MoreThan, MoreThan, true);
        }

        public static bool HasEqualsTo<TSource>(this IEnumerable<TSource> source, int numItems, Func<TSource, bool> predicate)
        {
            return HasItemsCount(source, numItems, predicate, EqualsTo, MoreThan, false);
        }

        public static bool HasLessThanOrEqualsTo<TSource>(this IEnumerable<TSource> source, int numItems, Func<TSource, bool> predicate)
        {
            return HasItemsCount(source, numItems, predicate, LessThanOrEqualsTo, MoreThan, false);
        }

        public static bool HasLessThan<TSource>(this IEnumerable<TSource> source, int numItems, Func<TSource, bool> predicate)
        {
            return HasItemsCount(source, numItems, predicate, LessThan, EqualsTo, false);
        }

        private static bool HasItemsCount<TSource>(
            IEnumerable<TSource> source,
            int numItems,
            Func<TSource, bool> predicate,
            Func<int, int, bool> comparison,
            Func<int, int, bool> inLoopComparison,
            bool inLoopComparisonResult)
        {
            if (source == null || numItems < 0)
                return false;

            int itemsCount = 0;
            foreach (TSource item in source)
            {
                if (predicate(item))
                {
                    itemsCount++;
                    if (inLoopComparison(itemsCount, numItems))
                        return inLoopComparisonResult;
                }
            }

            return comparison(itemsCount, numItems);
        }
    }
}