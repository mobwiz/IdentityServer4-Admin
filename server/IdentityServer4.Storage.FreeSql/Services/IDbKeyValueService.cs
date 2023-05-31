// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services
{
    public interface IDbKeyValueService
    {
        Task SetItemAsync(SetKeyValueItemRequest request);

        Task RemoveItemAsync(string key);

        Task<KeyValueDto> GetItemAsync(string key);
    }
}
