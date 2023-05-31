// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_user")]
    [Index("idx_unique_name", "username", true)]
    public class MAdminUser : IModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// username
        /// </summary>
        [Column(Name = "username", StringLength = 63, IsNullable = false)]
        public string Username { get; set; }

        /// <summary>
        /// nonce, salt
        /// </summary>
        [Column(Name = "nonce", StringLength = 63, IsNullable = false)]
        public string Nonce { get; set; }

        /// <summary>
        /// password hashed
        /// </summary>
        [Column(Name = "password_hash", StringLength = 71, IsNullable = false)]
        public string PasswordHash { get; set; }

        /// <summary>
        /// last login ip
        /// </summary>
        [Column(Name = "last_login_ip", StringLength = 63, IsNullable = false)]
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        [Column(Name = "create_time", IsNullable = false, CanUpdate = false)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column(Name = "create_by", IsNullable = false, CanUpdate = false, StringLength = 63)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column(Name = "update_by", IsNullable = false, CanUpdate = true, StringLength = 63)]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 创建时间，timestamp，秒
        /// </summary>
        [Column(Name = "update_time", IsNullable = false, CanUpdate = true)]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Column(Name = "last_login_time", IsNullable = false)]
        public DateTime LastLoginTime { get; set; }


        /// <summary>
        /// 是否冻结
        /// </summary>
        [Column(Name = "is_freezed", IsNullable = false)]
        public int IsFreezed { get; set; }
    }
}
