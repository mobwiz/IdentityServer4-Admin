import { get, post } from "@/utils/request";

export default {
  getApiResources({ pageIndex, pageSize, keyword, onlyEnabled }) {
    return get("apiresource/list", {
      pageIndex,
      pageSize,
      keyword: keyword || "",
      onlyEnabled,
    });
  },
  removeApiResource(id) {
    return post("apiresource/remove", { id });
  },
  saveApiResource({ id, enabled, name, displayName, secrets, claims, scopes }) {
    return post("apiresource/save", {
      id,
      enabled,
      name,
      displayName,
      secrets,
      claims,
      scopes,
    });
  },
  getAvaiableScopes({ keyword }) {
    return get("identityResource/getAvaibleScopes", {
      keyword: keyword || "",
    });
  },
};
