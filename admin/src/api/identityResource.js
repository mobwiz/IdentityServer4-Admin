import { get, post } from "@/utils/request";

export default {
  getIdentityResources({ pageIndex, pageSize, keyword, onlyEnabled }) {
    return get("identityResource/list", {
      pageIndex,
      pageSize,
      keyword: keyword || "",
      onlyEnabled,
    });
  },
  removeIdentityResource(id) {
    return post("identityResource/remove", { id });
  },
  saveIdentityResource({ id, enabled, name, displayName, emphasize, claims }) {
    return post("identityResource/save", {
      id,
      enabled,
      name,
      displayName,
      emphasize,
      claims,
    });
  },
};
