using FreeSql.DataAnnotations;

namespace IdentityServer4.Storage.FreeSql.Entities
{
    [Table(Name = "tbl_admin_persisted_grant")]
    public class MPersistedGrant : IModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// KEY
        /// </summary>
        [Column(Name = "key", IsNullable = false, StringLength = 255)]
        public string Key { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        [Column(Name = "type", IsNullable = false, StringLength = 127)]
        public string Type { get; set; }

        /// <summary>
        /// Subject Id
        /// </summary>
        [Column(Name = "subject_id", IsNullable = false, StringLength = 127)]
        public string SubjectId { get; set; }

        /// <summary>
        /// Session Id
        /// </summary>
        [Column(Name = "session_id", IsNullable = false, StringLength = 127)]
        public string SessionId { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        [Column(Name = "client_id", IsNullable = false, StringLength = 127)]
        public string ClientId { get; set; }

        /// <summary>
        /// Company Id
        /// </summary>
        [Column(Name = "company_id", IsNullable = false, StringLength = 127)]
        public string CompanyId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Column(Name = "description", IsNullable = false, StringLength = 1023)]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "creation_time", IsNullable = false)]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Column(Name = "expiration")]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Consumed 时间
        /// </summary>
        [Column(Name = "consumed_time")]
        public DateTime? ConsumedTime { get; set; }

        /// <summary>
        /// JSON Data
        /// </summary>
        [Column(Name = "data", IsNullable = false, StringLength = -1)]
        public string Data { get; set; }
    }
}
