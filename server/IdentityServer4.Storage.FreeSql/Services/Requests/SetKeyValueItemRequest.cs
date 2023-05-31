// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class SetKeyValueItemRequest : BaseOperateRequest
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
