using FreeSql.DataAnnotations;
using IdentityServer4.Storage.FreeSql.Types;

/*
 * 2022年5月8日 需要做一些改动，把这个表拆成两个
 * 1. AdminUI 可以修改的字段
 * 2. 系统接入可以修改的内容
 *      clientName
 *      增加 create_by, create_time 字段
 *      增加 标识 inner_system 内部系统，仅允许内部编辑
 *      
*/
namespace IdentityServer4.Storage.FreeSql.Entities
{

    [Table(Name = "tbl_admin_client")]
    [Index("idx_ids_client_client_id", "client_id", true)]
    public class MClient : IModel
    {
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        [Column(Name = "enabled")]
        public byte Enabled { get; set; }

        [Column(Name = "client_id", IsNullable = false, StringLength = 127)]
        public string ClientId { get; set; }

        [Column(Name = "client_name", IsNullable = false, StringLength = 127)]
        public string ClientName { get; set; }

        [Column(Name = "client_uri", IsNullable = false, StringLength = 255)]
        public string ClientUri { get; set; }

        [Column(Name = "logo_uri", IsNullable = false, StringLength = 255)]
        public string LogoUri { get; set; }

        [Column(Name = "client_desc", StringLength = 3900)]
        public string ClientDescription { get; set; }

        [Column(Name = "require_consent", IsNullable = false)]
        public byte RequireConsent { get; set; }

        [Column(Name = "allow_remember_consent", IsNullable = false)]
        public byte AllowRememberConsent { get; set; }

        [Column(Name = "require_pkce", IsNullable = false)]
        public byte RequirePkce { get; set; }

        [Column(Name = "front_channel_logout_uri", StringLength = 255, IsNullable = false)]
        public string FrontChannelLogoutUri { get; set; }

        [Column(Name = "back_channel_logout_uri", StringLength = 255, IsNullable = false)]
        public string BackChannelLogoutUri { get; set; }

        [Column(Name = "allow_offline_access", IsNullable = false)]
        public byte AllowOfflineAccess { get; set; }

        [Column(Name = "require_client_secret", IsNullable = false)]
        public byte RequireClientSecret { get; set; }

        [Column(Name = "access_token_type", IsNullable = false, MapType = typeof(byte))]
        public EAccessTokenType AccessTokenType { get; set; }

        [Column(Name = "token_lifetime", IsNullable = false)]
        public int TokenLifetime { get; set; }


        /// <summary>
        /// 显示顺序
        /// </summary>
        [Column(Name = "display_order")]
        public int DisplayOrder { get; set; }


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


        [Column(IsIgnore = true)] public ICollection<string> AllowedGrantTypes { get; set; } = new string[] { };
        [Column(IsIgnore = true)] public ICollection<string> RedirectUris { get; set; } = new string[] { };
        [Column(IsIgnore = true)] public ICollection<string> PostLogoutRedirectUris { get; set; } = new string[] { };
        [Column(IsIgnore = true)] public ICollection<string> ClientSecrets { get; set; } = new string[] { };
        [Column(IsIgnore = true)] public ICollection<string> AllowedScopes { get; set; } = new string[] { };
        [Column(IsIgnore = true)] public ICollection<string> AllowedCorsOrigins { get; set; } = new string[] { };
        [Column(IsIgnore = true)] public ICollection<MClientClaim> ClientClaims { get; set; } = new MClientClaim[] { };
    }

    //public class ClientClaim
    //{
    //    public string Type { get; set; }
    //    public string Value { get; set; }
    //    public string ValueType { get; set; }
    //}
}
