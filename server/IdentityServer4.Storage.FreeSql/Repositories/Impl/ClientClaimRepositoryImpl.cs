// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class ClientClaimRepositoryImpl : IClientClaimRepository
    {
        private IFreeSql _conn;

        public ClientClaimRepositoryImpl(IFreeSql conn)
        {
            _conn = conn;
        }

        public IEnumerable<MClientClaim> GetClaims(long clientId)
        {
            using var repos = _conn.GetRepository<MClientClaim>();

            return repos.Select.Where(p => p.ClientId == clientId).ToList();
        }

        public void RemoveClaims(long clientId)
        {
            _conn.Delete<MClientClaim>().Where(p => p.ClientId == clientId).ExecuteAffrows();
        }

        public void SaveClaims(long clientId, IEnumerable<MClientClaim> claims)
        {
            using var repos = _conn.GetRepository<MClientClaim>();

            repos.Delete(p => p.ClientId == clientId);

            foreach (var claim in claims)
            {
                claim.ClientId = clientId;
                repos.Insert(claim);
            }
        }
    }
}
