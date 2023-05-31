// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Config
{
    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PersistedGrantDto, PersistedGrant>();

            CreateMap<MClient, ClientDto>().ReverseMap();
            CreateMap<MApiScope, ApiScopeDto>().ReverseMap();
            CreateMap<MApiResource, ApiResourceDto>().ReverseMap();
            CreateMap<MReferenceToken, ReferenceTokenDto>().ReverseMap();
            CreateMap<MRefreshToken, RefreshTokenDto>().ReverseMap();
            CreateMap<MPersistedGrant, PersistedGrantDto>().ReverseMap();
            CreateMap<MIdentityResource, IdentityResourceDto>().ReverseMap();
            CreateMap<MAdminUser, AdminUserDto>().ReverseMap();
            CreateMap<MTicketData, TicketDataDto>().ReverseMap();

            CreateMap<MKeyValue, KeyValueDto>().ReverseMap();
        }
    }
}
