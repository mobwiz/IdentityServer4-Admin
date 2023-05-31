// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.WebApi.Filters;

namespace IdentityServer4.Admin.WebApi.Controllers.User
{
    [Authorize]
    [Route("api/admin/user")]
    [ServiceFilter(typeof(DemoModeFilter))]
    public class AdminUserController : AdminControllerBase, IApiResult
    {
        private IDbAdminUserService _adminUserService;
        private IDbTicketService _ticketService;
        // inject mapper
        private IMapper _mapper;

        public AdminUserController(IDbAdminUserService adminUserService,
            IMapper mapper,
            IDbTicketService ticketService)
        {
            _adminUserService = adminUserService;
            _mapper = mapper;
            _ticketService = ticketService;
        }

        [HttpGet("query")]
        public async Task<BaseResult<PagedResult<AdminUserVo>>> QueryUsers(QueryUserRequest model)
        {
            var result = await _adminUserService.QueryAdminUsersAsync(new Storage.FreeSql.Services.Requests.QueryAdminUserRequest
            {
                Keyword = model.Keyword,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
            });

            return this.OkResult(new PagedResult<AdminUserVo>
            {
                TotalCount = result.TotalCount,
                Items = result.Items.Select(_mapper.Map<AdminUserVo>)
            });
        }

        [HttpPost("create")]
        public async Task<BaseResult> CreateAdminUser([FromBody] CreateAdminUserRequest model)
        {
            var result = await _adminUserService.CreateAdminUserAsync(new Storage.FreeSql.Services.Requests.CreateAdminUserRequest
            {
                Operator = this.UserName,
                Password = model.Password,
                UserName = model.UserName,
            });

            return this.OkResult();
        }

        [HttpPost("update")]
        public async Task<BaseResult> UpdateAdminUser([FromBody] UpdateAdminUserRequest model)
        {
            await _adminUserService.UpdateAdminUserAsync(new UpdateAdminUserRquest
            {
                Operator = this.UserName,
                Id = model.Id,
                IsFreezed = model.IsFreezed,
                Password = model.NewPassword
            });

            if (model.IsFreezed || !string.IsNullOrEmpty(model.NewPassword))
            {
                // 让用户退出
                await _ticketService.RemoveTicketByUserIdAsync(model.Id.ToString());
            }

            return this.OkResult();
        }

        [HttpPost("remove")]
        public async Task<BaseResult> DeleteAdminUser([FromBody] DeleteAdminUserRequest model)
        {
            await _adminUserService.RemoveUserAsync(new RemoveAdminUserRequest
            {
                Operator = this.UserName,
                Id = model.Id,
            });
            return this.OkResult();
        }

        [HttpPost("update-password")]
        public async Task<BaseResult> UpdatePassword([FromBody] UpdatePasswordRequest model)
        {
            await _adminUserService.ResetPasswordForUserAsync(new ResetPasswordForUserRequest
            {
                Operator = this.UserName,
                NewPassword = model.NewPassword,
                Id = model.Id,
            });

            // 让用户退出
            await _ticketService.RemoveTicketByUserIdAsync(model.Id.ToString());

            return this.OkResult();
        }

    }

    public class UpdatePasswordRequest
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 8)]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [StringLength(255, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class DeleteAdminUserRequest
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
    }

    public class CreateAdminUserRequest
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [StringLength(255, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }

    public class QueryUserRequest
    {
        public string Keyword { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;

    }

    public class AdminUserVo
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// last login ip
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 是否已冻结
        /// </summary>
        public bool IsFreezed { get; set; }
    }
}
