// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Storage.KeyManager;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class DbSigningCredentialStoreImpl : ISigningCredentialStore
    {
        private RotateKeyManager _keyManager;

        public DbSigningCredentialStoreImpl(RotateKeyManager keyManager)
        {
            _keyManager = keyManager;
        }


        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var keys = await _keyManager.GetKeys();
            var keyInfo = keys.SigningKey.ToSecurityKey();

            return new SigningCredentials(keyInfo.Key, keyInfo.SigningAlgorithm);
        }

    }

    internal class DbValidationKeysStoreImpl : IValidationKeysStore
    {
        private RotateKeyManager _keyManager;

        public DbValidationKeysStoreImpl(RotateKeyManager keyManager)
        {
            _keyManager = keyManager;
        }

        public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var keys = await _keyManager.GetKeys();

            return keys.ValidationKeys.Select(x => x.ToSecurityKey());
        }
    }

    //public interface ISecurityKeyInfoStore
    //{
    //    Task<SecurityKeyInfo> GetKey(string name);
    //}

    //public class KeyStoreOptions
    //{
    //    public ISecurityKeyInfoStore KeyStoreInstance { get; set; }

    //    public string KeyName { get; set; }
    //}
}
