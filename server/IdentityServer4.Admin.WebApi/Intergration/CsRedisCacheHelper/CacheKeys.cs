// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.Admin.WebApi.Intergration.CsRedisCacheHelper
{
    public static class CacheKeys
    {
        public static string RedisKeyPrefix = "{idsadmin}";
        public static void SetKeyPrefix(string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                RedisKeyPrefix = prefix;
            }
        }
        public static string DataProtectionKey()
        {
            return $"{RedisKeyPrefix}:dataprotect:key";
        }
    }
}
