// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Mobwiz.Common.BaseTypes;
using System.Drawing.Printing;

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class QueryApiScopesRequest
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public BoolCondition Enabled { get; set; } = BoolCondition.All;
    }
}