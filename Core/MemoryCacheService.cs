using System;
using System.Runtime.Caching;

namespace Core
{
    public class MemoryCacheService : ICacheService
    {
        private ObjectCache Cache => MemoryCache.Default;

        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Set(new CacheItem(key, data), policy);
        }


        /// <summary>
        /// Variable (lock) to support thread-safe
        /// </summary>
        private static readonly object SyncObject = new object();


        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public T Get<T>(string key, int cacheTime, Func<T> acquire)
        {
            lock (SyncObject)
            {
                if (Cache.Contains(key))
                {
                    return Get<T>(key);
                }

                var result = acquire();
                if (cacheTime > 0)
                    Set(key, result, cacheTime);
                return result;
            }
        }
    }
}