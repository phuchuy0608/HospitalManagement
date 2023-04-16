using HospitalManagement.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace HospitalManagement.Service
{
    public class CacheBase : ICacheBase
    {
        private readonly IMemoryCache _memoryCache;

        public CacheBase(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Add<T>(string key, T cacheData)
        {
            T cacheExisted;
            if (!_memoryCache.TryGetValue(key, out cacheExisted))
            {
                cacheExisted = cacheData;

                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300));

                _memoryCache.Set(cacheExisted, cacheOptions);
            }
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public T GetOrCreate<T>(string key, TimeSpan timeExpiredCache, Func<T> cacheData)
        {
            return _memoryCache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = timeExpiredCache;
                return cacheData.Invoke();
            });
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
