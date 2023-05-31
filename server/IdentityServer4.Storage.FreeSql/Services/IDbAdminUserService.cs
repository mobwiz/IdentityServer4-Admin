// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{

    public interface IDbAdminUserService
    {
        /// <summary>
        /// Create admin user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<long> CreateAdminUserAsync(CreateAdminUserRequest request);

        /// <summary>
        /// Update admin user
        /// </summary>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        Task UpdateAdminUserAsync(UpdateAdminUserRquest adminUser);

        /// <summary>
        /// Get admin user by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AdminUserDto?> GetAdminUserAsync(GetAdminUserByIdRequest request);

        /// <summary>
        /// Validate user by username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AdminUserDto?> ValidateUserAsync(ValidateUserRequest request);

        /// <summary>
        /// remove user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RemoveUserAsync(RemoveAdminUserRequest request);

        /// <summary>
        /// query users
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagedResult<AdminUserDto>> QueryAdminUsersAsync(QueryAdminUserRequest request);

        /// <summary>
        /// update password by old
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> UpdatePasswordByOldAsync(UpdatePasswordByOldRequest request);

        /// <summary>
        /// reset password for user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> ResetPasswordForUserAsync(ResetPasswordForUserRequest request);

        /// <summary>
        /// update the login info
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateLoginInfoAsync(UpdateLoginInfoRequest request);

    }

}
