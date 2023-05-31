import { get, post } from "@/utils/request";

export default {
  getApiScopes({ pageIndex, pageSize, keyword, onlyEnabled }) {
    return get("apiscope/list", {
      pageIndex,
      pageSize,
      keyword: keyword || "",
      onlyEnabled,
    });
  },
  removeApiScope(id) {
    return post("apiscope/remove", { id });
  },
  saveApiScope({
    id,
    enabled,
    name,
    displayName,
    required,
    emphasize,
    claims,
  }) {
    return post("apiscope/save", {
      id,
      enabled,
      name,
      displayName,
      required,
      emphasize,
      claims,
    });
  },
};
