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
    internal interface IKeyValueRepository
    {
        /// <summary>
        /// Set a item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task CreateItemAsync(MKeyValue item);

        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task UpdateItemAsync(MKeyValue item);

        /// <summary>
        /// remove the item
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveItemAsync(string key);

        /// <summary>
        /// get the item
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<MKeyValue> GetItemAsync(string key);



    }
}
