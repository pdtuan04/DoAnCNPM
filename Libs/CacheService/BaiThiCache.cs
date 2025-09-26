using Libs.Entity;
using Libs.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Libs.CacheService
{
    public class BaiThiCache
    {
        private readonly IDistributedCache _cache;
        private readonly IBaiThiRepository _baiThiRepository;
        private DistributedCacheEntryOptions _cacheOption;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        public BaiThiCache(IDistributedCache cache, IBaiThiRepository baiThiRepository)
        {
            _cache = cache;
            _baiThiRepository = baiThiRepository;
            _cacheOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)// thoi gian het han cache
            };
        }
        // Cache theo id
        public async Task<BaiThi> GetBaiThiByIdAsync(Guid Id)
        {
            var cacheKey = $"BaiThiById-{Id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData == null)
            {
                var baiThiData = await _baiThiRepository.GetBaiThiWithDetails(Id);
                var serializedData = JsonSerializer.Serialize(baiThiData, _serializerOptions);
                await _cache.SetStringAsync(cacheKey, serializedData, _cacheOption);
                return baiThiData;
            }
            else
            {
                var baiThiData = JsonSerializer.Deserialize<BaiThi>(cachedData, _serializerOptions);
                return baiThiData;
            }
        }
    }
}
