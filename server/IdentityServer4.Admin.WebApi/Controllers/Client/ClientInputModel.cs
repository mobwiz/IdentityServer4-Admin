using IdentityServer4.Admin.WebApi.Utils;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.WebApi.Controllers.Client
{
    public class ClientInputModel
    {
        public int id { get; set; }
        public bool enabled { get; set; }

        [Required]
        [StringLength(63)]
        public string clientId { get; set; }

        [StringLength(127)]
        [SecureText]
        public string clientName { get; set; }

        [StringLength(255)]
        public string clientUri { get; set; }

        [StringLength(255)]
        public string logoUri { get; set; }

        [StringLength(255)]
        public string clientDescription { get; set; }

        [Required]
        public string[] allowedGrantTypes { get; set; }
        public string[] redirectUris { get; set; }
        public string[] postLogoutRedirectUris { get; set; }
        public string[] allowedCorsOrigins { get; set; }
        public string[] clientSecrets { get; set; }
        public string[] allowedScopes { get; set; }
        public ClientClaimItem[] clientClaims { get; set; }
        public bool requireConsent { get; set; }
        public bool allowRememberConsent { get; set; }
        public bool requireClientSecret { get; set; }
        public bool requirePkce { get; set; }

        [StringLength(255)]
        public string frontChannelLogoutUri { get; set; }

        [StringLength(255)]
        public string backChannelLogoutUri { get; set; }
        public bool allowOfflineAccess { get; set; }
        public int accessTokenType { get; set; }
        public int tokenLifetime { get; set; }

        public class ClientClaimItem
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
