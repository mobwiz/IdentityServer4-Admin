// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories
{
    internal interface IClientClaimRepository
    {
        void SaveClaims(long clientId, IEnumerable<MClientClaim> claims);

        void RemoveClaims(long clientId);

        IEnumerable<MClientClaim> GetClaims(long clientId);
    }
}
