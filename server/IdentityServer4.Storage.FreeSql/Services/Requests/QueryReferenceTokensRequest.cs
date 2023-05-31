// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class QueryReferenceTokensRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string ClientId { get; set; }
        public string SubjectId { get; set; }
        public string SessionId { get; set; }
        public string Handle { get; set; }
    }
}