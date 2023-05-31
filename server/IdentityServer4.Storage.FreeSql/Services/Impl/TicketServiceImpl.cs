// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Repositories;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class TicketServiceImpl : IDbTicketService
    {
        private ITicketDataRepository _ticketDataRepository;
        private IMapper _mapper;

        public TicketServiceImpl(ITicketDataRepository ticketDataRepository,
            IMapper mapper)
        {
            _ticketDataRepository = ticketDataRepository;
            _mapper = mapper;
        }

        public async Task CreateTicketAsync(TicketDataDto ticket)
        {
            // check parameters
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            // check key
            if (string.IsNullOrWhiteSpace(ticket.Key))
            {
                throw new ArgumentNullException(nameof(ticket.Key));
            }

            await _ticketDataRepository.CreateTicketDataAsync(_mapper.Map<MTicketData>(ticket));
        }

        public async Task<IEnumerable<TicketDataDto>> GetExpiredTicketsAsync(int batchSize)
        {
            var list = await _ticketDataRepository.GetExpiredTicketDataAsync(batchSize);

            return list.Select(p => _mapper.Map<TicketDataDto>(p));
        }

        public async Task<TicketDataDto> GetTicketByKeyAsync(string key)
        {
            var obj = await _ticketDataRepository.GetTicketDataByKeyAsync(key);
            if (obj != null)
            {
                return _mapper.Map<TicketDataDto>(obj);
            }

            return null;
        }

        public async Task RemoveTicketByKeyAsync(string key)
        {
            await _ticketDataRepository.RemoveTicketDataByKeyAsync(key);
        }

        public async Task RemoveTicketByUserIdAsync(string userId)
        {
            await _ticketDataRepository.RemoveTicketDataByUserIdAsync(userId);
        }

        public async Task UpdateTicketAsync(TicketDataDto ticket)
        {
            // check parameters
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            // check key
            if (string.IsNullOrWhiteSpace(ticket.Key))
            {
                throw new ArgumentNullException(nameof(ticket.Key));
            }

            var ticketData = await _ticketDataRepository.GetTicketDataByKeyAsync(ticket.Key);

            if (ticketData == null)
            {
                throw new ArgumentException($"Can't find ticket by key \"{ticket.Key}\" ");
            }

            if (ticketData != null)
            {
                ticketData.LastActivity = ticket.LastActivity;
                ticketData.ExpireTime = ticket.ExpireTime;
                ticketData.Base64Data = ticket.Base64Data;

                await _ticketDataRepository.UpdateTicketDataAsync(ticketData);
            }
        }
    }
}
