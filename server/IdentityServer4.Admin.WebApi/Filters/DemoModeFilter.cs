// Copyright (c) Mobwiz Lu. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.Filters;
using Mobwiz.Common.Exceptions;

namespace IdentityServer4.Admin.WebApi.Filters
{
    public class DemoModeFilter : IActionFilter
    {

        private IConfiguration Configuration { get; }

        private bool IsDemoMode => Configuration.GetValue<bool>("DemoMode");

        public DemoModeFilter(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // 如果是 demo mode，仅允许 Get 请求，否则返回 403 错误
            if (IsDemoMode && context.HttpContext.Request.Method != "GET")
            {
                throw new BllException(403, "Demo mode is not allowed to modify data.");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {            
        }
    }
}
