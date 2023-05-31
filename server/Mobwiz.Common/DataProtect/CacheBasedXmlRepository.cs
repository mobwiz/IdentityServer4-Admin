// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Laiye.SaasMp.WebApi.Integration;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Xml.Linq;

namespace Microsoft.AspNetCore.DataProtection
{
    public class CacheBasedXmlRepository : IXmlRepository
    {

        private ICacheHelper _cacheHelper;
        private CacheBasedDataProtectionOptions _options;

        private const string CacheCategory = "_cachedDpKeys";
        private string CacheKey = "dataprotectkey";


        public CacheBasedXmlRepository(IOptions<CacheBasedDataProtectionOptions> options, ICacheHelper cacheHelper)
        {
            _options = options.Value;
            CacheKey = _options.CacheKey;
            _cacheHelper = cacheHelper;
        }


        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var list = _cacheHelper.GetCacheValueAsync<List<string>>(CacheCategory, CacheKey).GetAwaiter().GetResult();

            var outResult = new List<XElement>();

            if (list?.Any() == true)
            {
                foreach (var str in list)
                {
                    outResult.Add(XElement.Parse(str));
                }
            }

            return outResult;
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var strValue = element.ToString(SaveOptions.DisableFormatting);

            var existed = _cacheHelper.ExistsAsync(CacheCategory, CacheKey).GetAwaiter().GetResult();

            var list = new List<string>();

            if (existed)
            {
                list = _cacheHelper.GetCacheValueAsync<List<string>>(CacheCategory, CacheKey).GetAwaiter().GetResult();
            }

            list.Add(strValue);

            _cacheHelper.SetCacheValueAsync(CacheCategory, CacheKey, JsonSerializer.Serialize(list), TimeSpan.FromHours(24 * 30 * 12))
                .GetAwaiter().GetResult();
        }
    }
}
