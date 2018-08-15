using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CacheUtility;
using NUnit.Framework;

namespace CacheUtility.Tests
{
    [ExcludeFromCodeCoverage]
    public class CachingTest
    {
        [TestCase(0), Order(1)]
        public void ClearCacheTest(int output)
        {
            // clear cache
            Caching.ClearCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            Assert.IsInstanceOf(typeof(List<CacheObject>), allCache);
            Assert.AreEqual(output, allCache.Count());
        }

        [TestCase("RemoveObjectFromCacheString", "test"), Order(8)]
        public void RemoveObjectFromCacheTest(string itemName, string output)
        {
            // clear cache
            Caching.ClearCache();
            itemName.SetObjectInCache<string>(5, output);
            string cachedItem = itemName.GetObjectFromCache<string>(5, () => output);
            Assert.AreEqual(output, cachedItem);
            itemName.RemoveObjectFromCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.AreEqual(null, cachedItem);
        }

        [TestCase("RemoveObjectFromCacheString", "test", "refresh test"), Order(7)]
        public void RemoveObjectFromCacheTest(string itemName, string input, string output)
        {
            // clear cache
            Caching.ClearCache();
            // set data in the cache
            string cachedItem = itemName.GetObjectFromCache<string>(5, () => input);
            Assert.AreEqual(input, cachedItem);
            // change the cached item to something new
            itemName.RemoveObjectFromCache();
            // check cached item
            List<CacheObject> newCache = Caching.GetAllCache();
            cachedItem = newCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
        }

        [TestCase("test", "refresh test"), Order(7)]
        public void RemoveObectsFromCacheTest(string input, string output)
        {
            List<string> inputs = new List<string> { "test1", "test2", "test3" };
            // clear cache
            Caching.ClearCache();
            // set data in the cache
            foreach (string s in inputs)
                s.SetObjectInCache<string>(5, s);

            input.SetObjectInCache<string>(5, input);

            List<CacheObject> allCache = Caching.GetAllCache();
            Assert.AreEqual(inputs.Count() + 1, allCache.Count());

            // change the cached item to something new
            inputs.RemoveObjectsFromCache();
            // check cached item
            List<CacheObject> newCache = Caching.GetAllCache();
            string cachedItem = newCache.Where(w => w.Key.ToLower() == inputs.FirstOrDefault().ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
            Assert.AreEqual(newCache.Count(), 1);
        }

        [TestCase("RefreshObjectFromCacheTestString", "test", "refresh test"), Order(4)]
        public void RefreshObjectFromCacheTest(string itemName, string input, string output)
        {
            // clear cache
            Caching.ClearCache();
            // set data in the cache
            string cachedItem = itemName.GetObjectFromCache<string>(5, () => input);
            Assert.AreEqual(input, cachedItem);
            // change the cached item to something new
            itemName.RefreshObjectFromCache<string>(5, () => output);
            // check cached item
            List<CacheObject> newCache = Caching.GetAllCache();
            cachedItem = newCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.AreEqual(output, cachedItem);
        }

        [TestCase("SetObjectInCacheTestString", "test"), Order(9)]
        public void SetObjectInCacheTest(string itemName, string output)
        {
            // clear cache
            Caching.ClearCache();
            itemName.RemoveObjectFromCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            string cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
            itemName.SetObjectInCache<string>(5, output, false);
            allCache = Caching.GetAllCache();
            cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.AreEqual(output, cachedItem);
        }

        [TestCase("SetObjectInCacheTestString", "test"), Order(9)]
        public void SetObjectInCacheTestSliding(string itemName, string output)
        {
            // clear cache
            Caching.ClearCache();
            itemName.RemoveObjectFromCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            string cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
            itemName.SetObjectInCache<string>(5, output, true);
            allCache = Caching.GetAllCache();
            cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.AreEqual(output, cachedItem);
        }

        [TestCase("SetObjectInCacheTestString", "test1", "test2"), Order(9)]
        public void SetObjectInCacheTestEmpty(string itemName, string output1, string output2)
        {
            // clear cache
            Caching.ClearCache();
            itemName.SetObjectInCache<string>(5, output1, false);

            List<CacheObject> allCache = Caching.GetAllCache();
            string cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNotNull(cachedItem);
            itemName.SetObjectInCache<string>(5, output2, false);
            allCache = Caching.GetAllCache();
            cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.AreEqual(output2, cachedItem);
        }

        [TestCase("GetObjectFromCacheTestString", "test"), Order(6)]
        public void GetObjectFromCacheTest(string itemName, string output)
        {
            // clear cache
            Caching.ClearCache();
            itemName.RemoveObjectFromCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            string cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
            cachedItem = itemName.GetObjectFromCache<string>(5, () => output);
            Assert.AreEqual(output, cachedItem);
        }

        [TestCase("GetObjectFromCacheTestString", "test"), Order(6)]
        public void GetObjectFromCacheTestSliding(string itemName, string output)
        {
            // clear cache
            Caching.ClearCache();
            itemName.RemoveObjectFromCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            string cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
            cachedItem = itemName.GetObjectFromCache<string>(5, () => output, true);
            Assert.AreEqual(output, cachedItem);
        }

        [TestCase("GetObjectFromCacheTestString", "test"), Order(6)]
        public void GetObjectFromCacheTestThrow(string itemName, string output)
        {
            // clear cache
            Caching.ClearCache();
            itemName.RemoveObjectFromCache();
            List<CacheObject> allCache = Caching.GetAllCache();
            string cachedItem = allCache.Where(w => w.Key.ToLower() == itemName.ToLower()).Select(s => s.Value.ToString()).FirstOrDefault();
            Assert.IsNull(cachedItem);
            cachedItem = itemName.GetObjectFromCache<string>(5, () => throw new Exception());
            Assert.IsNull(cachedItem);
        }

        [TestCase(), Order(2)]
        public void GetAllCacheTest()
        {
            // clear cache
            Caching.ClearCache();
            var allCache = Caching.GetAllCache();
            Assert.IsInstanceOf(typeof(List<CacheObject>), allCache);
        }

        [TestCase("area1|owner1", "area2|owner2", "area3|owner3", true, "owner2", 2), Order(6)]
        public void RemoveFilteredCacheTestOWner(string itemName1, string itemName2, string itemName3, bool isOwner, string area, int output)
        {
            // clear cache
            Caching.ClearCache();

            $"{itemName1}".SetObjectInCache<string>(5, $"{itemName1}");
            $"{itemName2}".SetObjectInCache<string>(5, $"{itemName2}");
            $"{itemName3}".SetObjectInCache<string>(5, $"{itemName1}");

            int total = 3;

            List<CacheObject> allCache = Caching.GetAllCache();
            int count = allCache.Count();
            Assert.AreEqual(total, count);

            area.RemoveFilteredCache(isOwner);

            allCache = Caching.GetAllCache();
            count = allCache.Count();
            Assert.AreEqual(output, count);
        }

        [TestCase("area1|owner1", "area2|owner2", "area3|owner3", false, "area2", 2), Order(6)]
        public void RemoveFilteredCacheTestArea(string itemName1, string itemName2, string itemName3, bool isOwner, string area, int output)
        {
            // clear cache
            Caching.ClearCache();

            $"{itemName1}".SetObjectInCache<string>(5, $"{itemName1}");
            $"{itemName2}".SetObjectInCache<string>(5, $"{itemName2}");
            $"{itemName3}".SetObjectInCache<string>(5, $"{itemName1}");

            int total = 3;

            List<CacheObject> allCache = Caching.GetAllCache();
            int count = allCache.Count();
            Assert.AreEqual(total, count);

            area.RemoveFilteredCache(isOwner);

            allCache = Caching.GetAllCache();
            count = allCache.Count();
            Assert.AreEqual(output, count);
        }

        [TestCase("RemoveAllWildcard1", "RemoveAllWildcard2", "RemoveAllWildcard3", "Wildcard",
                    "test 1", "test 2", "test 3", 5, 5, 0), Order(5)]
        public void RemoveAllWildcardTest(string itemName1, string itemName2, string itemName3, string wildcard,
                                            string input1, string input2, string input3, int output1, int output2, int output3)
        {
            // clear cache
            Caching.ClearCache();

            for (int i = 0; i < 5; i++)
            {
                $"{i}{itemName1}".SetObjectInCache<string>(5, $"{i} - {input1}");
                $"{i}{itemName2}".SetObjectInCache<string>(5, $"{i} - {input2}");
                $"{i}{itemName3}".SetObjectInCache<string>(5, $"{i} - {input3}");
            }

            List<CacheObject> allCache = Caching.GetAllCache();
            int count = allCache.Where(w => w.Key.ToLower().Contains(itemName1.ToLower())).Count();
            Assert.AreEqual(output1, count);

            itemName1.RemoveAllWildcard();

            allCache = Caching.GetAllCache();
            count = allCache.Where(w => w.Key.ToLower().Contains(itemName2.ToLower())).Count();
            Assert.AreEqual(output2, count);

            wildcard.RemoveAllWildcard();

            allCache = Caching.GetAllCache();
            count = allCache.Count();
            Assert.AreEqual(output3, count);
        }

    }
    public class CacheModelsTest
    {
        [TestCase("area|owner", "area", "owner"), Order(1)]
        public void CacheObjectAreaOwnerTest1(string input, string area, string owner)
        {
            CacheObject obj = new CacheObject { Key = input };
            Assert.AreEqual(area, obj.Area);
            Assert.AreEqual(owner, obj.Owner);
        }

        [TestCase("areaowner"), Order(1)]
        public void CacheObjectAreaOwnerTest2(string input)
        {
            CacheObject obj = new CacheObject { Key = input };
            Assert.IsNull(obj.Area);
            Assert.IsNull(obj.Owner);
        }

        [TestCase(""), Order(1)]
        public void CacheObjectAreaOwnerTest3(string input)
        {
            CacheObject obj = new CacheObject { Key = input };
            Assert.IsNull(obj.Area);
            Assert.IsNull(obj.Owner);
        }

        [TestCase("", "Area", "Owner"), Order(1)]
        public void CacheObjectAreaOwnerTest4(string key, string area, string owner)
        {
            CacheObject obj = new CacheObject { Key = key, Area = area, Owner = owner };
            Assert.AreEqual(obj.Area, area);
            Assert.AreEqual(obj.Owner, owner);
        }
    }
}