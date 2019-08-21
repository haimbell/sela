using System;

namespace Core
{
    /// <summary>
    /// Cache service we use to store and get items to cache. cache can be in memory or 3rd party service such as redis
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Item key id</param>
        /// <param name="cacheTime">Time to cache item, in minutes</param>
        /// <param name="acquire">function to load item is not in cache</param>
        /// <returns></returns>
        T Get<T>(string key, int cacheTime, Func<T> acquire);

        /// <summary>
        /// Add or update item in cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        void Set(string key, object data, int cacheTime);
    
    }
}