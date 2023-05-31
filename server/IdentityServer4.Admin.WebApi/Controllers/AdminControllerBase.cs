// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using Mobwiz.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.WebApi.Controllers
{
    public class AdminControllerBase : ControllerBase
    {
        /// <summary>
        /// 用户 Id
        /// </summary>
        public long UserId => long.Parse(GetStringClaimValue(JwtClaimTypes.Subject));

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName => GetStringClaimValue(JwtClaimTypes.Name);


        /// <summary>
        /// 获取指定 Claim 值
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        /// <exception cref="LaiyeException"></exception>
        private string GetStringClaimValue(string claimType)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                if (User.HasClaim(p => p.Type == claimType))
                {
                    return User.Claims.FirstOrDefault(p => p.Type == claimType)?.Value ?? "";
                }
                //throw new ArgumentOutOfRangeException(nameof(claimType));
            }

            throw new BllException(401, "Not authenticated, cliam not found: " + claimType);
        }

    }
}
