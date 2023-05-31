// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class RemoveReferenceTokensRequest
    {
        public string ClientId { get; set; }
        public string SubjectId { get; set; }
        public string SessionId { get; set; }
    }
}