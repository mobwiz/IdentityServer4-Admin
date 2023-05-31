// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services
{
    public interface IDbTicketService
    {
        /// <summary>
        /// Create a ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task CreateTicketAsync(TicketDataDto ticket);

        /// <summary>
        /// Update the ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task UpdateTicketAsync(TicketDataDto ticket);

        /// <summary>
        /// Remove the ticket by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveTicketByKeyAsync(string key);

        /// <summary>
        /// Get the tiket
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TicketDataDto> GetTicketByKeyAsync(string key);

        /// <summary>
        /// Remove the ticket by subject id
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        Task RemoveTicketByUserIdAsync(string subjectId);

        /// <summary>
        /// Get the expired tickets
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task<IEnumerable<TicketDataDto>> GetExpiredTicketsAsync(int batchSize);
    }
}
