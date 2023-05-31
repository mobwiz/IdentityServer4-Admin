// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("IdentityServer4.Storage.FreeSql.Tests")]
namespace IdentityServer4.Storage.FreeSql.Storage.KeyManager
{
    internal class RotateKeyManager
    {
        private IDbKeyValueService _keyValueService;
        private ILogger<RotateKeyManager> _logger;
        private IDataProtectionProvider _dataProtectionProvider;

        private static KeyCollection KeyCollection { get; set; }

        public RotateKeyManager(IDbKeyValueService keyValueService, IDataProtectionProvider dataProtectionProvider, ILogger<RotateKeyManager> logger)
        {
            _keyValueService = keyValueService;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        private const string KeyItemName = "_IdentityServer4:SigningCredentia";
        private const int MaxValidationKey = 6;

        internal static TimeSpan KeyExpiration = TimeSpan.FromDays(30);

        private static SemaphoreSlim LockObject = new SemaphoreSlim(1, 1);

        // for test class use
        internal static void Reset()
        {
            KeyCollection = null;
        }

        public async Task<KeyCollection> GetKeys()
        {
            await LockObject.WaitAsync(3000);
            {
                if (KeyCollection != null && KeyCollection.SigningKey.ExpireTime > DateTime.UtcNow)
                {
                    LockObject.Release();
                    return KeyCollection;
                }

                var dp = _dataProtectionProvider.CreateProtector("IdentityServer4.Storage.FreeSql.Storage.KeyManager.RotateKeyManager");

                try
                {
                    var item = _keyValueService.GetItemAsync(KeyItemName).Result;
                    if (item != null)
                    {
                        var decryptedJson = dp.Unprotect(item.Value);
                        var existedCollection = JsonSerializer.Deserialize<KeyCollection>(decryptedJson);
                        if (existedCollection != null)
                        {
                            if (existedCollection.SigningKey.ExpireTime < DateTime.UtcNow)
                            {
                                // create a new key
                                existedCollection = RenewCollection(existedCollection);
                                // save the key

                                var encryptedJson = dp.Protect(JsonSerializer.Serialize(existedCollection));
                                await _keyValueService.SetItemAsync(new Services.Requests.SetKeyValueItemRequest
                                {
                                    Key = KeyItemName,
                                    Operator = "System",
                                    Value = encryptedJson
                                });
                            }

                            KeyCollection = existedCollection;

                            LockObject.Release();
                            return KeyCollection;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Can't load the signing key from store");
                }

                {
                    var newKey = CreateNewKeyInfo();
                    var collection = new KeyCollection()
                    {
                        SigningKey = newKey,
                        ValidationKeys = new List<KeyInfo>() { newKey }
                    };
                    var encryptedJson = dp.Protect(JsonSerializer.Serialize(collection));
                    await _keyValueService.SetItemAsync(new Services.Requests.SetKeyValueItemRequest
                    {
                        Key = KeyItemName,
                        Operator = "System",
                        Value = encryptedJson
                    });

                    KeyCollection = collection;
                    LockObject.Release();

                    return collection;
                }
            }
        }


        private KeyCollection RenewCollection(KeyCollection keyCollection)
        {

            if (keyCollection.ValidationKeys.Count >= MaxValidationKey)
            {
                keyCollection.ValidationKeys = keyCollection.ValidationKeys.OrderByDescending(x => x.ExpireTime).Take(MaxValidationKey - 1).ToList();
            }

            var keyInfo = CreateNewKeyInfo();
            keyCollection.ValidationKeys.Insert(0, keyInfo);
            keyCollection.SigningKey = keyInfo;

            return keyCollection;
        }

        private KeyInfo CreateNewKeyInfo()
        {
            var newSigningKey = CryptoHelper.CreateRsaSecurityKey();
            var alg = IdentityServerConstants.RsaSigningAlgorithm.RS256.ToString();
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(newSigningKey);
            jwk.Alg = alg;


            var keyInfo = new KeyInfo
            {
                JsonWebKey = jwk,
                ExpireTime = DateTime.UtcNow.Add(KeyExpiration)
            };
            return keyInfo;
        }
    }

    internal class KeyCollection
    {
        public KeyInfo SigningKey { get; set; }

        public IList<KeyInfo> ValidationKeys { get; set; }

        public KeyCollection()
        {
            ValidationKeys = new List<KeyInfo>();
        }
    }

    internal class KeyInfo
    {
        public Microsoft.IdentityModel.Tokens.JsonWebKey JsonWebKey { get; set; }

        public DateTime ExpireTime { get; set; } // = DateTime.UtcNow.AddMonths(1); // 默认一个月过期

        public SecurityKeyInfo ToSecurityKey()
        {
            return new SecurityKeyInfo()
            {
                Key = JsonWebKey,
                SigningAlgorithm = JsonWebKey.Alg
            };
        }
    }
}
