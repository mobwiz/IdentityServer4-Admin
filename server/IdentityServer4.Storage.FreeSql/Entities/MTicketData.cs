// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_ticket")]
    [Index("idx_ticket_key", "data_key", true)]
    [Index("idx_expire_time", "expire_time")]
    public class MTicketData : IModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        [Column(Name = "data_key", StringLength = 63, IsNullable = false, CanUpdate = false)]
        public string Key { get; set; }

        /// <summary>
        /// userid
        /// </summary>

        [Column(Name = "subject_id", IsNullable = false, CanUpdate = false)]
        public string SubjectId { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        [Column(Name = "last_activity", IsNullable = false)]
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Column(Name = "expire_time", IsNullable = false)]
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [Column(Name = "data", StringLength = -1, IsNullable = false)]
        public string Base64Data { get; set; }
    }
}
