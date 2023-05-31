// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories
{
    internal interface ITicketDataRepository
    {
        /// <summary>
        /// 创建 tiket
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task CreateTicketDataAsync(MTicketData data);

        /// <summary>
        /// 更新 ticket
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task UpdateTicketDataAsync(MTicketData data);

        /// <summary>
        /// 删除 ticket
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveTicketDataByKeyAsync(string key);

        /// <summary>
        /// 获取 ticket
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<MTicketData> GetTicketDataByKeyAsync(string key);

        /// <summary>
        /// 删除用户的 ticket
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveTicketDataByUserIdAsync(string userId);

        /// <summary>
        /// 获取过期 ticket
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MTicketData>> GetExpiredTicketDataAsync(int batchSize);
    }
}
