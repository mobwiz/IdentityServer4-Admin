namespace IdentityServer4.Storage.FreeSql.Types
{
    public enum EStringType
    {
        Unknown = 0,
        IdentityResourceClaim = 11,

        ApiResourceClaim = 21,
        ApiResourceScope = 22,
        ApiResourceSecret = 23,

        ApiScopeUserClaim = 31,

        ClientSecret = 32,
        ClientAllowedScope = 33,
        ClientAllowedGrantType = 34,
        ClientRedirectUri = 35,
        ClientPostLogoutRedirectUri = 36,
        ClientCorsOrigin = 37
    }

    public enum EAccessTokenType
    {
        Jwt = 0,
        Reference = 1
    }

}
