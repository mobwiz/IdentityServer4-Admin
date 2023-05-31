import { get, post } from "@/utils/request";

export default {
  queryUsers({ pageIndex, pageSize, keyword }) {
    return get("user/query", {
      pageIndex,
      pageSize,
      keyword: keyword || "",
    });
  },
  removeUser(id) {
    return post("user/remove", { id });
  },
  createUser({ username, password }) {
    return post("user/create", {
      username,
      password,
    });
  },
  updateUser({ id, newPassword, isFreezed }) {
    return post("user/update", {
      id,
      newPassword,
      isFreezed,
    });
  },
};
