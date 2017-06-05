using System;
using System.Runtime.Caching;

public class InMemoryCache : ICacheService
{
	public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
	{
		T item = MemoryCache.Default.Get(cacheKey) as T;
		if (item == null)
		{
			item = getItemCallback();
			//MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(10));
			MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddHours(2));
		}
		return item;
	}

	public void Remove(string cacheKey) 
	{
		if (MemoryCache.Default.Contains(cacheKey))
		{
			MemoryCache.Default.Remove(cacheKey);
		}
	}
}

public interface ICacheService
{
	T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class;
	void Remove(string cacheKey);
}