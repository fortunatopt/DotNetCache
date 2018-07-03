# DotNetCache

## Description:

This is a .NET class library for caching. It includes methods for general cache management.

## Methods:

1.  ClearCache()
2.  RemoveObjectFromCache(string cacheItemName)
3.  SetObjectInCache<T>(string cacheItemName, int cacheTimeInMinutes, T newCacheObject)
4.  GetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool slidingExpiration = false)
5.  GetAllCache()
6.  RemoveFilteredCache(string filter, bool isOwner = false)

## Descriptions:

<table class="editorDemoTable">

<thead>

<tr>

<td>Name of the method</td>

<td>Explaination</td>

<td>Return</td>

</tr>

</thead>

<tbody>

<tr>

<td>ClearCache</td>

<td>Removes everything from the cache, but leaves the cache object intact.</td>

<td>void</td>

</tr>

<tr>

<td>RemoveObjectFromCache</td>

<td>Removes an individual item from the cache by name</td>

<td>void</td>

</tr>

<tr>

<td>SetObjectInCache</td>

<td><span id="demoId">Sets a typed item in the cache for a set number of minutes</span></td>

<td>void</td>

</tr>

<tr>

<td>GetObjectFromCache</td>

<td>Gets an item from the cache.</td>

<td> Object T</td>

</tr>

<tr>

<td>GetAllCache</td>

<td>Gets the entire cache</td>

<td>List<CacheObject)</td>

</tr>

<tr>

<td>RemoveFilteredCache</td>

<td>Removes a cached item by either the Area or the Owner</td>

<td>bool</td>

</tr>

</tbody>

</table>

## Usage:

The CacheObject above is a custom class that has the following properties:

public class CacheObject {

public string Key { get; set; } 

public object Value { get; set; } 

public string Area { get; set; } 

public string Owner { get; set; }

}

The Area and Owner are derived from the Key. For this reason, the Key should be structure as such:

area_owner ie. UserProfile_jsmith

**Here are some usage examples for the Options:**

Caching.ClearCache();

Caching.RemoveObjectFromCache("UserProfile_jsmith");

Caching.SetObjectInCache<vwGetOrgProfile>("UserProfile_jsmith", 5, jSmith);

var allCache = Caching.GetAllCache()

var success = Caching.RemoveFilteredCache(jsmith, true);

User data = Caching.GetObjectFromCache<User>("UserProfile_jsmith", 5, () => GetUserMethod("jSmith"));

Understanding the GetObjectFromCache method:

The GetObjectFromCache method takes a function as the final parameter. This function is to get the item from the original source in the event that it does not exist in the cache. There is nothing special about that function, other than the types of the two must match.

**What's coming?** 

Implementation of Sliding Expiration instead of Absolute as an option throughout the utility.