using System;
using System.Collections.Generic;

namespace CacheVisualization
{
    class CacheConversion
    {
        private readonly Dictionary<string, Func<object, string>> _cacheConversions;

        public CacheConversion()
        {
            _cacheConversions = new Dictionary<string, Func<object, string>>();
        }

        public void Add(string cacheKey, Func<object, string> conversion)
        {
            if (!_cacheConversions.ContainsKey(cacheKey))
            {
                _cacheConversions.Add(cacheKey, conversion);
            }
        }

        public string GetValue(string cacheKey, object cacheValue)
        {
            foreach (var cacheConversion in _cacheConversions)
            {
                if (cacheKey.StartsWith(cacheConversion.Key))
                {
                    return cacheConversion.Value(cacheValue);
                }
            }

            return cacheValue.ToString();
        }
    }
}
