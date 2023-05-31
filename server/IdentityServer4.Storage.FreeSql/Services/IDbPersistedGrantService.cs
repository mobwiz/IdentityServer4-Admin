// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services
{    
    public interface IDbPersistedGrantService
    {
        Task<IEnumerable<PersistedGrantDto>> GetListByFilterAsync(string subjectId, string sessionId, string clientId, string type);

        Task<PersistedGrantDto> GetByKeyAsync(string key);

        Task RemoveAllByFilterAsync(string subjectId, string sessionId, string clientId, string type);

        Task RemoveByKeyAsync(string key);

        Task StoreGrantAsync(PersistedGrantDto grant);
    }
}
