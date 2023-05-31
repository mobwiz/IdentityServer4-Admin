// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Repositories.Impl;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using Mobwiz.Common.HashAlg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class AdminUserServiceImpl : InnerBaseRepository, IDbAdminUserService
    {

        private readonly IAdminUserRepository _adminUserRepository;
        // IMapper
        private readonly IMapper _mapper;


        public AdminUserServiceImpl(DatabaseConnectionManager connectionManager,
            IAdminUserRepository adminUserRepository,
            IMapper mapper)
            : base(connectionManager)
        {
            _adminUserRepository = adminUserRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateAdminUserAsync(CreateAdminUserRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.UserName)) throw new ArgumentNullException(nameof(request.UserName));
            if (string.IsNullOrWhiteSpace(request.Password)) throw new ArgumentNullException(nameof(request.Password));

            // check the same name!
            if (await _adminUserRepository.GetAdminUserByUsernameAsync(request.UserName) != null)
                throw new BllException(500, $"User \"{request.UserName}\" is already existed");

            var muser = new MAdminUser
            {
                CreatedBy = request.Operator,
                CreateTime = DateTime.Now,
                Nonce = Guid.NewGuid().ToString("N"),
                Username = request.UserName,
                UpdateBy = "",
                UpdateTime = DateTime.MinValue,
                LastLoginIp = "",
                LastLoginTime = DateTime.MinValue
            };

            // hash password using pbkdf2
            muser.PasswordHash = GetHashAlgorithm().ComputeHash(muser.Nonce, request.Password);

            var id = await _adminUserRepository.CreateAdminUserAsync(muser);

            return id;
        }

        public async Task<AdminUserDto?> GetAdminUserAsync(GetAdminUserByIdRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var obj = await _adminUserRepository.GetAdminUserByIdAsync(request.Id);

            return _mapper.Map<AdminUserDto?>(obj);
        }

        public async Task<PagedResult<AdminUserDto>> QueryAdminUsersAsync(QueryAdminUserRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var queryResult = await _adminUserRepository.QueryAdminUsersAsync(request.Keyword, request.PageIndex, request.PageSize);

            return new PagedResult<AdminUserDto>
            {
                TotalCount = queryResult.TotalCount,
                Items = queryResult.Items.Select(p => _mapper.Map<AdminUserDto>(p))
            };
        }

        public async Task RemoveUserAsync(RemoveAdminUserRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));

            await _adminUserRepository.RemoveAdminUserAsync(request.Id);
        }

        public async Task<bool> ResetPasswordForUserAsync(ResetPasswordForUserRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var obj = await _adminUserRepository.GetAdminUserByIdAsync(request.Id);

            if (obj == null)
            {
                throw new BllException(404, $"User by id {request.Id} not found");
            }

            obj.PasswordHash = GetHashAlgorithm().ComputeHash(obj.Nonce, request.NewPassword);

            await _adminUserRepository.UpdateAdminUserAsync(obj);

            return true;
        }

        public async Task UpdateAdminUserAsync(UpdateAdminUserRquest adminUser)
        {
            // check the parameter
            if (adminUser == null) throw new ArgumentNullException(nameof(adminUser));

            // check the user is exist
            var obj = await _adminUserRepository.GetAdminUserByIdAsync(adminUser.Id);
            if (obj == null)
            {
                throw new BllException(404, $"User by id {adminUser.Id} not found");
            }

            // if the password is not null, then update the password
            if (!string.IsNullOrWhiteSpace(adminUser.Password))
            {
                obj.PasswordHash = GetHashAlgorithm().ComputeHash(obj.Nonce, adminUser.Password);
            }

            // update the user

            obj.UpdateBy = adminUser.Operator;
            obj.UpdateTime = DateTime.Now;
            obj.IsFreezed = adminUser.IsFreezed ? 1 : 0;

            await _adminUserRepository.UpdateAdminUserAsync(obj);
        }

        public async Task UpdateLoginInfoAsync(UpdateLoginInfoRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var obj = await _adminUserRepository.GetAdminUserByIdAsync(request.Id);

            if (obj == null)
            {
                throw new BllException(404, $"User by id {request.Id} not found");
            }

            obj.LastLoginIp = request.LoginIp;
            obj.LastLoginTime = DateTime.Now;
            await _adminUserRepository.UpdateAdminUserAsync(obj);
        }

        public async Task<bool> UpdatePasswordByOldAsync(UpdatePasswordByOldRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var obj = await _adminUserRepository.GetAdminUserByIdAsync(request.Id);

            if (obj == null)
            {
                throw new BllException(404, $"User by id {request.Id} not found");
            }

            IHashAlgorithm hashAlg = GetHashAlgorithm();

            var oldPasswordHash = hashAlg.ComputeHash(obj.Nonce, request.OldPassword);
            if (oldPasswordHash != obj.PasswordHash) return false;

            obj.PasswordHash = hashAlg.ComputeHash(obj.Nonce, request.NewPassword);

            await _adminUserRepository.UpdateAdminUserAsync(obj);

            return true;
        }

        public async Task<AdminUserDto?> ValidateUserAsync(ValidateUserRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var obj = await _adminUserRepository.GetAdminUserByUsernameAsync(request.Username);

            if (obj == null) return null;

            // check the input password is equal to the obj.paswordHash
            IHashAlgorithm hashAlg = GetHashAlgorithm();

            var inputPaswordHash = hashAlg.ComputeHash(obj.Nonce, request.Password);

            if (inputPaswordHash != obj.PasswordHash) return null;

            return _mapper.Map<AdminUserDto>(obj);
        }



        private IHashAlgorithm GetHashAlgorithm()
        {
            return new Pbkdf2HashTransform();
        }
    }
}
