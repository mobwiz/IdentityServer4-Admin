using FreeSql.DataAnnotations;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_refresh_token")]
    [Index("idx_plat_ids_refresh_token_expireTime", "expire_time", false)]
    [Index("idx_plat_ids_refresh_token_sub_client_company_sid", "subject_id,client_id,company_id,session_id", false)]
    public class MRefreshToken : IModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Token Handle
        /// </summary>
        [Column(Name = "handle", IsNullable = false, StringLength = 127)]
        public string Handle { get; set; }

        /// <summary>
        /// Subject Id
        /// </summary>
        [Column(Name = "subject_id", IsNullable = false, StringLength = 127)]
        public string SubjectId { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        [Column(Name = "client_id", IsNullable = false, StringLength = 127)]
        public string ClientId { get; set; }

        /// <summary>
        /// JSON Data
        /// </summary>
        [Column(Name = "data", IsNullable = false, StringLength = -1)]
        public string Data { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "creation_time", IsNullable = false)]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        [Column(Name = "expires_in", IsNullable = false)]
        public int ExpiresIn { get; set; } = 3600;

        /// <summary>
        /// SessionId
        /// </summary>
        [Column(Name = "session_id", IsNullable = false)]
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// 过期时间
        /// </summary>
        [Column(Name = "expire_time", IsNullable = false)]
        public DateTime ExpireTime { get; set; }
    }
}
