// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Repositories;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class KeyValueServiceImpl : IDbKeyValueService
    {
        private IKeyValueRepository _keyValueRepository;
        private IMapper _mapper;

        public KeyValueServiceImpl(IKeyValueRepository keyValueRepository, IMapper mapper)
        {
            _keyValueRepository = keyValueRepository;
            _mapper = mapper;
        }

        public async Task<KeyValueDto> GetItemAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var obj = await _keyValueRepository.GetItemAsync(key);

            return _mapper.Map<KeyValueDto>(obj);
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            await _keyValueRepository.RemoveItemAsync(key);
        }

        public async Task SetItemAsync(SetKeyValueItemRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Key)) throw new ArgumentNullException(nameof(request.Key));

            var obj = await _keyValueRepository.GetItemAsync(request.Key);

            if (obj == null)
            {
                await _keyValueRepository.CreateItemAsync(new Entities.MKeyValue
                {
                    Key = request.Key,
                    Value = request.Value,
                    CreatedBy = request.Operator,
                    CreateTime = DateTime.Now,
                    UpdateBy = request.Operator ?? string.Empty,
                    UpdateTime = DateTime.Now,
                });
            }
            else
            {
                obj.Value = request.Value;
                obj.UpdateTime = DateTime.Now;
                obj.UpdateBy = request.Operator ?? string.Empty;
                //obj.ExpireTime = DateTime.MaxValue;
                await _keyValueRepository.UpdateItemAsync(obj);
            }
        }
    }
}
