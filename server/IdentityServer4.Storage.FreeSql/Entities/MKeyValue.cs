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
    [Table(Name = "tbl_system_keyvalue")]
    [Index("idx_kv_key", "item_key", true)]
    public class MKeyValue : IModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 配置 key
        /// </summary>
        [Column(Name = "item_key", StringLength = 63, IsNullable = false, CanUpdate = false)]
        public string Key { get; set; }

        /// <summary>
        /// 配置 value
        /// </summary>
        [Column(Name = "item_value", StringLength = -1, IsNullable = false)]
        public string Value { get; set; }


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

        ///// <summary>
        ///// 过期时间
        ///// </summary>
        //[Column(Name = "expire_time", IsNullable = false)]
        //public DateTime ExpireTime { get; set; }
    }
}
