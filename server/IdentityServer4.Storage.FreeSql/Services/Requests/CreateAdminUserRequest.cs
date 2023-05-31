// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class CreateAdminUserRequest : BaseOperateRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    // 好像没啥可以 updte 的
    public class UpdateAdminUserRequest : BaseOperateRequest
    {
        public long Id { get; set; }

        public string NewPassword { get; set; }

        public bool IsFreezed { get; set; }
    }

    public class RemoveAdminUserRequest : BaseOperateRequest
    {
        public long Id { get; set; }
    }

    public class QueryAdminUserRequest
    {
        public string Keyword { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public class ValidateUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class GetAdminUserByIdRequest
    {
        public long Id { get; set; }
    }

    public class UpdatePasswordByOldRequest
    {
        public long Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public string Operator { get; set; }
    }

    public class ResetPasswordForUserRequest
    {
        public long Id { get; set; }
        public string NewPassword { get; set; }

        public string Operator { get; set; }

    }

    public class UpdateLoginInfoRequest
    {
        public long Id { get; set; }
        public string LoginIp { get; set; }
    }

    public class UpdateAdminUserRquest
    {
        public long Id { get; set; }
        public string Password { get; set; }
        public string Operator { get; set; }

        public bool IsFreezed { get; set; }
    }


}
