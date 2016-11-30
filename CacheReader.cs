using System;
using System.Collections.Generic;
using System.Linq;

namespace CacheVisualization
{
    class CacheReader
    {
        private readonly HashSet<string> _cachePaths;
        private readonly HashSet<string> _excludedCachePaths;

        public CacheReader(HashSet<string> cachePaths, HashSet<string> excludedCachePaths)
        {
            _cachePaths = cachePaths;
            _excludedCachePaths = excludedCachePaths;
        }

        public SortedList<string, List<object>> GetGroupedCache()
        {
            var cachedItems = new SortedList<string, List<object>>();

            var enumerator = System.Web.HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = (string)enumerator.Key;
                if (ShouldExclude(key))
                {
                    continue;
                }

                key = GetMergedKey(key);
                if (key == null)
                {
                    continue;
                }

                if (!cachedItems.ContainsKey(key))
                {
                    cachedItems[key] = new List<object> { enumerator.Value };
                }
                else
                {
                    cachedItems[key].Add(enumerator.Value);
                }
            }

            return cachedItems;
        }

        public SortedList<string, List<object>> GetCache()
        {
            var cachedItems = new SortedList<string, List<object>>();

            var enumerator = System.Web.HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = (string)enumerator.Key;
                if (ShouldExclude(key))
                {
                    continue;
                }

                cachedItems[key] = new List<object> { enumerator.Value };
            }

            return cachedItems;
        }

        bool ShouldExclude(string cacheKey)
        {
            return _excludedCachePaths.Any(exludedCachePath => cacheKey.StartsWith(exludedCachePath, StringComparison.InvariantCultureIgnoreCase));
        }

        string GetMergedKey(string cacheKey)
        {
            foreach (var cachePath in _cachePaths.OrderByDescending(x => x))
            {
                if (cacheKey.StartsWith(cachePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    return cachePath;
                }
            }

            return null;
        }
    }
}