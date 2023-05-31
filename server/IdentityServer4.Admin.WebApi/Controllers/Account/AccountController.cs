// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4.Admin.WebApi.Utils;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Laiye.SaasMp.WebApi.Integration;
using Mobwiz.Common.BaseTypes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace IdentityServer4.Admin.WebApi.Controllers.Account
{
    [ApiController]
    [Route("api/admin/account")]
    public class AccountController : Controller, IApiResult
    {
        private readonly IDbAdminUserService _adminUserService;
        private readonly SecurityChecker _securityChecker;

        public AccountController(IDbAdminUserService adminUserService,
            SecurityChecker securityChecker)
        {
            _adminUserService = adminUserService;
            _securityChecker = securityChecker;
        }

        [HttpPost("webLogin")]
        public async Task<BaseResult> WebLogin([FromBody] LoginRequest request)
        {

            var (isFreezed, failedCount, freezeTime) = await _securityChecker.CheckIsFreezed(request.Username);

            if (isFreezed)
            {
                return new BaseResult(400402, $"User is freezed, please try again after {freezeTime}");
            }

            var adminUser = await _adminUserService.ValidateUserAsync(new ValidateUserRequest
            {
                Username = request.Username,
                Password = request.Password,
            });

            if (adminUser == null)
            {
                await _securityChecker.RecordFailedLogin(request.Username);

                return new BaseResult(400400, "Username or password is wrong");
            }

            if (adminUser.IsFreezed == 1)
            {
                await _securityChecker.RecordFailedLogin(request.Username);

                return new BaseResult(400403, "User is freezed");
            }

            await _securityChecker.ClearFailedLogin(request.Username);

            // succeed logined.
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtClaimTypes.Name, adminUser.Username),
                new Claim(JwtClaimTypes.Subject, adminUser.Id.ToString()),
            }, "idsadmin"));

            await HttpContext.SignInAsync(principal);

            await _adminUserService.UpdateLoginInfoAsync(new UpdateLoginInfoRequest
            {
                Id = adminUser.Id,
                LoginIp = HttpContext.GetClientIp()
            });

            return new BaseResult();
        }

        [HttpGet("webLogout")]
        public async Task<BaseResult> WebLogout()
        {
            var result = await this.HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                await this.HttpContext.SignOutAsync();
            }

            return this.OkResult();
        }

        [Authorize]
        [HttpGet("getUserInfo")]
        public BaseResult GetUserInfo()
        {
            var result = new Dictionary<string, string>
            {
                { "name", User.Identity?.Name ?? "-" },
                { "role", "admin" }
            };

            return this.OkResult(result);
        }
    }

    //public class UserSession
    //{
    //    public long UserId { get; set; }
    //    public string Username { get; set; }
    //    public string SessionKey { get; set; }
    //    public string UserRole { get; set; }
    //}


    //public class SessionManager
    //{
    //    public const string SessionCookieName = ".idsadmin.session";

    //    private ICacheHelper CacheHelper { get; set; }

    //    private TimeSpan _slidingTimespan = TimeSpan.FromMinutes(30);

    //    private const string CategoryKey = "ids:session";

    //    public SessionManager(ICacheHelper cacheHelper)
    //    {
    //        CacheHelper = cacheHelper;
    //    }

    //    public async Task<UserSession?> GetSessionAsync(string sessionKey)
    //    {
    //        var obj = await CacheHelper.GetCacheValueAsync<UserSession>(CategoryKey, sessionKey);
    //        if (obj != null)
    //        {
    //            await CacheHelper.ExpireCacheAsync(CategoryKey, sessionKey, _slidingTimespan);
    //        }

    //        return obj;
    //    }

    //    public async Task SetSessinAsync(string sessionKey, UserSession session)
    //    {
    //        var json = JsonConvert.SerializeObject(session);
    //        await CacheHelper.SetCacheValueAsync(CategoryKey, sessionKey, json, _slidingTimespan);
    //    }
    //}
}
