// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Admin.WebApi.Utils;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace IdentityServer4.Admin.WebApi.Filters
{
    public class BllExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }

            BaseResult result;
            var exceptionType = context.Exception.GetType();
            var exception = context.Exception;
            if (exceptionType == typeof(BllException))
            {
                var exObj = exception as BllException;

                if (exObj != null)
                {
                    result = new BaseResult(500, $"{exObj.Message}");

                    context.ExceptionHandled = true;
                    var response = context.HttpContext.Response;
                    response.StatusCode = 200; // (int)status;
                    response.ContentType = "application/json";
                    response.WriteAsync(JsonSerializer.Serialize(result, options: Helper.GetJsonSerializeOptions()));
                }
            }
        }
    }
}
