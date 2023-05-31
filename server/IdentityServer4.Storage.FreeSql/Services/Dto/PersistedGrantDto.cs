using FreeSql.DataAnnotations;

namespace IdentityServer4.Storage.FreeSql.Entities
{ 
    public class PersistedGrantDto : IModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// KEY
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Subject Id
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Session Id
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Company Id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Consumed 时间
        /// </summary>
        public DateTime? ConsumedTime { get; set; }

        /// <summary>
        /// JSON Data
        /// </summary>
        public string Data { get; set; }
    }
}
