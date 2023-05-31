const Implicit = "implicit";
const Hybrid = "hybrid";
const AuthorizationCode = "authorization_code";
const ClientCredentials = "client_credentials";
const ResourceOwnerPassword = "password";

const GrantTypes = {
  // single
  ClientCredentials: [ClientCredentials],
  Code: [AuthorizationCode],
  CodeAndClientCredentials: [AuthorizationCode, ClientCredentials],
  Hybrid: [Hybrid],
  HybridAndClientCredentials: [Hybrid, ClientCredentials],
  Implicit: [Implicit],
  ImplicitAndClientCredentials: [Implicit, ClientCredentials],
  ResourceOwnerPassword: [ResourceOwnerPassword],
  ResourceOwnerPasswordAndClientCredentials: [
    ResourceOwnerPassword,
    ClientCredentials,
  ],
  // 2 combine

  FindKey(grantTypes) {
    if (grantTypes && grantTypes.length == 1) {
      const val0 = grantTypes[0];
      if (val0 === Implicit) return "Implicit";
      else if (val0 === Hybrid) return "Hybrid";
      else if (val0 === AuthorizationCode) return "Code";
      else if (val0 === ClientCredentials) return "ClientCredentials";
      else if (val0 === ResourceOwnerPassword) return "ResourceOwnerPassword";
    }
    if (grantTypes && grantTypes.length == 2) {
      const inStr = grantTypes.join(",");

      if (this.ImplicitAndClientCredentials.join(",") == inStr) {
        return "ImplicitAndClientCredentials";
      }

      if (this.CodeAndClientCredentials.join(",") == inStr) {
        return "CodeAndClientCredentials";
      }

      if (this.HybridAndClientCredentials.join(",") == inStr) {
        return "HybridAndClientCredentials";
      }

      if (this.ResourceOwnerPasswordAndClientCredentials.join(",") == inStr) {
        return "ResourceOwnerPasswordAndClientCredentials";
      }
    }
    return "";
  },
};

export default GrantTypes;
