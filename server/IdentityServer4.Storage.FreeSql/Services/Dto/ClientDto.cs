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
namespace IdentityServer4.Storage.FreeSql.Services.Dto
{
    public class ClientDto
    {
        public long Id { get; set; }

        public byte Enabled { get; set; }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string ClientUri { get; set; }

        public string LogoUri { get; set; }

        public string ClientDescription { get; set; }

        public byte RequireConsent { get; set; }

        public byte AllowRememberConsent { get; set; }

        public byte RequirePkce { get; set; }

        public string FrontChannelLogoutUri { get; set; }

        public string BackChannelLogoutUri { get; set; }

        public byte AllowOfflineAccess { get; set; }

        public byte RequireClientSecret { get; set; }

        public EAccessTokenType AccessTokenType { get; set; }

        public int TokenLifetime { get; set; }


        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }


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


        [Column(IsIgnore = true)] public IList<string> AllowedGrantTypes { get; set; } = new List<string>();
        [Column(IsIgnore = true)] public IList<string> RedirectUris { get; set; } = new List<string>();
        [Column(IsIgnore = true)] public IList<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        [Column(IsIgnore = true)] public IList<string> ClientSecrets { get; set; } = new List<string>();
        [Column(IsIgnore = true)] public IList<string> AllowedScopes { get; set; } = new List<string>();
        [Column(IsIgnore = true)] public IList<string> AllowedCorsOrigins { get; set; } = new List<string>();
        [Column(IsIgnore = true)] public IList<ClientClaim> ClientClaims { get; set; } = new List<ClientClaim>();
    }

    public class ClientClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}
