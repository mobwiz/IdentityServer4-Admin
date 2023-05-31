// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Laiye.SaasMp.WebApi.Integration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.DataProtection
{
    public class CacheBasedDataProtectionOptions
    {
        public string CacheKey { get; internal set; }
    }   

    public class KeyManagementOptionsPostConfigure : IPostConfigureOptions<KeyManagementOptions>
    {
        private IServiceProvider _serviceProvider;

        public KeyManagementOptionsPostConfigure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void PostConfigure(string name, KeyManagementOptions options)
        {
            options.XmlRepository = _serviceProvider.GetRequiredService<CacheBasedXmlRepository>();

            // todo add encryptor and descripter here
            // options.XmlEncryptor
        }
    }

    /// <summary>
    /// Contains Redis-specific extension methods for modifying a <see cref="IDataProtectionBuilder"/>.
    /// </summary>
    public static class CSRedisDataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysName = "DataProtection-Keys";

        /// <summary>
        /// Configures the data protection system to persist keys to specified key in Redis database
        /// </summary>  
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="databaseFactory">The delegate used to create instances.</param>
        /// <param name="storeName">Dapr state store name</param>
        /// <param name="key">The key used to store key list.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToCacheHelper(this IDataProtectionBuilder builder, string key)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure<CacheBasedDataProtectionOptions>((opt) =>
            {
                opt.CacheKey = key;
            });

            builder.Services.AddSingleton<CacheBasedXmlRepository>();

            builder.Services.AddSingleton<IPostConfigureOptions<KeyManagementOptions>, KeyManagementOptionsPostConfigure>();

            return builder;
        }
    }
}