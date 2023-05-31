// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class UpdateIdentityResourceRequest : BaseOperateRequest
    {
        public IdentityResourceDto Resource { get; set; }
    }
}