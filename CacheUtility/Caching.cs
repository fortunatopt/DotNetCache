using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace CacheUtility
{
    /// <summary>
    /// Caching Utilities
    /// </summary>
    public static class Caching
    {
        /// <summary>
        /// Clear all items in the Cache
        /// </summary>
        public static void ClearCache()
        {
            // This will remove all items from the cache, but leave the cache object
            ObjectCache cache = MemoryCache.Default;
            List<string> keys = cache.Select(s => s.Key).ToList();
            foreach (string key in keys)
                cache.Remove(key.ToLower());
        }
        /// <summary>
        /// Remove a single item from the Cache
        /// </summary>
        /// <param name="cacheItemName"></param>
        public static void RemoveObjectFromCache(this string cacheItemName)
        {
            // This removes an item from the cache by name
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(cacheItemName.ToLower());
        }
        public static void RemoveObjectsFromCache(this List<string> cacheItems)
        {
            // This removes items from the cache by name
            ObjectCache cache = MemoryCache.Default;
            foreach (string cacheItem in cacheItems)
                cache.Remove(cacheItem.ToLower());
        }
        public static void RefreshObjectFromCache<T>(this string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool slidingExpiration = false)
        {
            cacheItemName.RemoveObjectFromCache();
            var data = cacheItemName.GetObjectFromCache<T>(cacheTimeInMinutes, objectSettingFunction, slidingExpiration);
            string test = "test";
        }
        /// <summary>
        /// Add an item to the Cache
        /// </summary>
        /// <typeparam name="T">Type of the item to be added to the Cache</typeparam>
        /// <param name="cacheItemName">Name for the item</param>
        /// <param name="cacheTimeInMinutes">Minutes before expiration</param>
        /// <param name="newCacheObject">Data for the item to be added to the Cache</param>
        public static void SetObjectInCache<T>(this string cacheItemName, int cacheTimeInMinutes, T newCacheObject, bool slidingExpiration = false)
        {
            // This sets an item in the cache
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[cacheItemName.ToLower()];
            // remove cached item if it already exists
            if (cachedObject != null)
                cache.Remove(cacheItemName.ToLower());
            // set cache expiration time
            CacheItemPolicy policy = new CacheItemPolicy();
            if (slidingExpiration == true)
            {
                policy.SlidingExpiration = TimeSpan.FromMinutes(cacheTimeInMinutes);
            }
            else
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
            }
            // set item in cache
            cache.Set(cacheItemName.ToLower(), newCacheObject, policy);
        }
        /// <summary>
        /// Funtion that takes a generic type to get and set an item in the cache
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="cacheItemName">Name if cache item</param>
        /// <param name="cacheTimeInMinutes">Cache item expiration</param>
        /// <param name="objectSettingFunction">Function to be called to set the item if it doesn't exist</param>
        /// <param name="slidingExpiration">This allows for the use of Sliding Expiration instead of Absolute so the cache object will persist longer</param>
        /// <returns>Returns the object of the type specified</returns>
        public static T GetObjectFromCache<T>(this string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool slidingExpiration = false)
        {
            // get the item from the cache
            ObjectCache cache = MemoryCache.Default;
            T cachedObject = (T)cache.Get(cacheItemName.ToLower(), null);
            // if the item doesn't exist, this will get and set it
            if (cachedObject == null)
            {
                // set the expiration policy
                CacheItemPolicy policy = new CacheItemPolicy();
                if (slidingExpiration == true)
                {
                    policy.SlidingExpiration = TimeSpan.FromMinutes(cacheTimeInMinutes);
                }
                else
                {
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
                }
                // try catch to make sure the item is not set to an exception
                try
                {
                    cachedObject = objectSettingFunction();
                }
                catch { }
                if (cachedObject != null)
                    cache.Set(cacheItemName.ToLower(), cachedObject, policy);
            }
            return cachedObject;
        }
        /// <summary>
        /// Get the entire Cache
        /// </summary>
        /// <returns>A list of the CacheObject data</returns>
        public static List<CacheObject> GetAllCache()
        {
            List<CacheObject> c = new List<CacheObject>();
            ObjectCache cache = MemoryCache.Default;

            c = cache.Select(s => new CacheObject { Key = s.Key.ToLower(), Value = s.Value }).ToList();
            return c;
        }
        /// <summary>
        /// Remove an item from the Cache by either Area or Owner
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="isOwner"></param>
        /// <returns></returns>
        public static bool RemoveFilteredCache(this string filter, bool isOwner = false)
        {
            // get the cache
            List<CacheObject> c = new List<CacheObject>();
            ObjectCache cache = MemoryCache.Default;

            // get the item by either the owner or the area
            if (isOwner == true)
            {
                c = cache.Select(s => new CacheObject { Key = s.Key.ToLower(), Value = s.Value }).ToList().Where(w => w.Owner.ToLower() == filter.ToLower()).ToList();
            }
            else
            {
                c = cache.Select(s => new CacheObject { Key = s.Key.ToLower(), Value = s.Value }).ToList().Where(w => w.Area.ToLower() == filter.ToLower()).ToList();
            }

            // remove the item
            foreach (var f in c)
                f.Key.ToLower().RemoveObjectFromCache();

            return true;
        }
        public static void RemoveAllWildcard(this string cacheItemName)
        {
            // This will remove all items from the cache, but leave the cache object
            ObjectCache cache = MemoryCache.Default;
            List<string> keys = cache.Where(w => w.Key.ToLower().Contains(cacheItemName.ToLower())).Select(s => s.Key).ToList();
            foreach (string key in keys)
                cache.Remove(key);
        }
    }
}
