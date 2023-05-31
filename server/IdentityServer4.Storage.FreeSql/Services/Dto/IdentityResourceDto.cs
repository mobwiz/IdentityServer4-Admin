// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Org.BouncyCastle.Crypto;
using System.Xml.Linq;

namespace IdentityServer4.Storage.FreeSql.Services.Dto
{
    public class IdentityResourceDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Reqquired
        /// </summary>
        public byte Required { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte Emphasize { get; set; }

        /// <summary>
        /// enabled
        /// </summary>
        public byte Enabled { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public IList<string> Claims { get; set; } = new List<string>();

        public int IsPreset { get; set; }
    }
}