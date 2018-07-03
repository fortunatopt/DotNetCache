using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace CacheUtility
{
    /// <summary>
    /// Caching Utilities
    /// </summary>
    public class Caching
    {
        /// <summary>
        /// Clear all items in the Cache
        /// </summary>
        public static void ClearCache()
        {
            ObjectCache cache = MemoryCache.Default;
            List<string> keys = cache.Select(s => s.Key).ToList();
            foreach (string key in keys)
                cache.Remove(key);
        }
        /// <summary>
        /// Remove a single item from the Cache
        /// </summary>
        /// <param name="cacheItemName"></param>
        public static void RemoveObjectFromCache(string cacheItemName)
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(cacheItemName);
        }
        /// <summary>
        /// Add an item to the Cache
        /// </summary>
        /// <typeparam name="T">Type of the item to be added to the Cache</typeparam>
        /// <param name="cacheItemName">Name for the item</param>
        /// <param name="cacheTimeInMinutes">Minutes before expiration</param>
        /// <param name="newCacheObject">Data for the item to be added to the Cache</param>
        public static void SetObjectInCache<T>(string cacheItemName, int cacheTimeInMinutes, T newCacheObject)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[cacheItemName];
            if (cachedObject != null)
                cache.Remove(cacheItemName);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
            cache.Set(cacheItemName, newCacheObject, policy);
        }
        /// <summary>
        /// A generic function for getting and setting objects to the memory cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <param name="cacheItemName">The name to be used when storing this object in the cache.</param>
        /// <param name="cacheTimeInMinutes">How long to cache this object for.</param>
        /// <param name="objectSettingFunction">A parameterless function to call if the object isn't in the cache and you need to set it.</param>
        /// <param name="slidingExpiration">This allows for the use of Sliding Expiration instead of Absolute so the cache object will persist longer</param>
        /// <returns>An object of the type you asked for</returns>
        public static T GetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool slidingExpiration = false)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[cacheItemName];
            if (cachedObject == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                if (slidingExpiration == true)
                {
                    policy.SlidingExpiration = TimeSpan.FromMinutes(cacheTimeInMinutes);
                }
                else
                {
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
                }
                cachedObject = objectSettingFunction();
                if (cachedObject != null)
                    cache.Set(cacheItemName, cachedObject, policy);
            }
            return cachedObject;
        }
        /// <summary>
        /// Class to pull data from the Cache
        /// </summary>
        public class CacheObject
        {
            /// <summary>
            /// Name of the Cache item
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Cache data
            /// </summary>
            public object Value { get; set; }
            private string _area;
            /// <summary>
            /// The area for the Cache item
            /// </summary>
            public string Area
            {
                get
                {
                    if (Key != null && Key != string.Empty)
                    {
                        _area = Key.Substring(0, Key.LastIndexOf('_'));
                    }
                    return _area;
                }
                set { _area = value; }
            }
            private string _owner;
            /// <summary>
            /// Owner of the Cache item
            /// </summary>
            public string Owner
            {
                get
                {
                    if (Key != null && Key != string.Empty)
                    {
                        int lio = Key.LastIndexOf('_') + 1;
                        _owner = Key.Substring(lio, Key.Length - lio);
                    }
                    return _owner;
                }
                set { _owner = value; }
            }
        }
        /// <summary>
        /// Get the entire Cache
        /// </summary>
        /// <returns></returns>
        public static List<CacheObject> GetAllCache()
        {
            List<CacheObject> c = new List<CacheObject>();
            ObjectCache cache = MemoryCache.Default;

            c = cache.Select(s => new CacheObject { Key = s.Key, Value = s.Value }).ToList();
            return c;
        }
        /// <summary>
        /// Remove an item from the Cache by either Area or Owner
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="isOwner"></param>
        /// <returns></returns>
        public static bool RemoveFilteredCache(string filter, bool isOwner = false)
        {
            List<CacheObject> c = new List<CacheObject>();
            ObjectCache cache = MemoryCache.Default;

            if (isOwner == true)
            {
                c = cache.Select(s => new CacheObject { Key = s.Key, Value = s.Value }).ToList().Where(w => w.Owner == filter).ToList();
            }
            else
            {
                c = cache.Select(s => new CacheObject { Key = s.Key, Value = s.Value }).ToList().Where(w => w.Area == filter).ToList();
            }

            foreach (var f in c)
                Caching.RemoveObjectFromCache(f.Key);

            return true;
        }
    }
}
