// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class QueryIdentityResourcesRequest
    {
        public BoolCondition Enabled { get; set; } = BoolCondition.All;
        public string Keyword { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}