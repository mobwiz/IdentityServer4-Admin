// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Admin.WebApi.Controllers.Client;
using IdentityServer4.Admin.WebApi.Controllers.Resource;
using IdentityServer4.Admin.WebApi.Controllers.User;
using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Admin.WebApi.Config
{
    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApiScopeDto, ApiScopeVo>()
                .ForMember(s => s.Required, opt => opt.MapFrom(d => d.Required == 1))
                .ForMember(s => s.Emphasize, opt => opt.MapFrom(d => d.Emphasize == 1))
                .ForMember(s => s.Enabled, opt => opt.MapFrom(d => d.Enabled == 1));

            CreateMap<ApiResourceDto, ApiResourceVo>()
                .ForMember(s => s.Enabled, opt => opt.MapFrom(d => d.Enabled == 1));

            CreateMap<IdentityResourceDto, IdentityResourceVo>()
                .ForMember(s => s.Required, opt => opt.MapFrom(d => d.Required == 1))
                .ForMember(s => s.Emphasize, opt => opt.MapFrom(d => d.Emphasize == 1))
                .ForMember(s => s.Enabled, opt => opt.MapFrom(d => d.Enabled == 1));

            CreateMap<ClientDto, ClientVo>()
               .ForMember(s => s.Enabled, opt => opt.MapFrom(d => d.Enabled == 1))
               .ForMember(s => s.RequirePkce, opt => opt.MapFrom(d => d.RequirePkce == 1))
               .ForMember(s => s.RequireConsent, opt => opt.MapFrom(d => d.RequireConsent == 1))
               .ForMember(s => s.RequireClientSecret, opt => opt.MapFrom(d => d.RequireClientSecret == 1))
               .ForMember(s => s.AllowOfflineAccess, opt => opt.MapFrom(d => d.AllowOfflineAccess == 1))
               .ForMember(s => s.AllowRememberConsent, opt => opt.MapFrom(d => d.AllowRememberConsent == 1));


            CreateMap<AdminUserDto, AdminUserVo>()
                .ForMember(s => s.IsFreezed, opt => opt.MapFrom(d => d.IsFreezed == 1));
        }
    }
}
