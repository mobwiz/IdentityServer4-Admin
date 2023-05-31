// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services.Requests
{
    public class QueryClientsRequest
    {
        /// <summary>
        /// Keyword to search
        /// </summary>
        public string? Keyword { get; set; }
        /// <summary>
        /// Page index, start at 1
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Enabled or not
        /// </summary>
        public BoolCondition Enabled { get; set; } = BoolCondition.All;
    }
}