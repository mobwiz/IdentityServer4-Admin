// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Xml.Linq;

namespace IdentityServer4.Storage.FreeSql.Services.Dto
{
    public class ReferenceTokenDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Token Handle
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Subject Id
        /// </summary>
        public string SubjectId { get; set; }


        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// JSON Data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; } = 3600;

        /// <summary>
        /// SessionId
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}