// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.Storage.FreeSql.Services.Dto
{
    public class AdminUserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// nonce, salt
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// last login ip
        /// </summary>
        public string LastLoginIp { get; set; }

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

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 是否已冻结
        /// </summary>
        public int IsFreezed { get; set; }
    }
}
