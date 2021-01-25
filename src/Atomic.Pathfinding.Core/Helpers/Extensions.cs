using System.Collections.Generic;

namespace Atomic.Pathfinding.Core.Helpers
{
    public static class Extensions
    {
        //TryGetValue is faster than ContainsKey
        public static bool HasKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            return dict.TryGetValue(key, out var result);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if(dict.TryGetValue(key, out var result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
