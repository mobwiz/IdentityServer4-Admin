import { get, post } from "@/utils/request";

export default {
  getClients: function ({ keyword, onlyEnabled, pageIndex, pageSize }) {
    return get("client/list", { keyword, onlyEnabled, pageIndex, pageSize });
  },
  getDetail: function (clientId) {
    return get("client/get", { clientId });
  },
  saveClient: function (clientInfo) {
    return post("client/save", clientInfo);
  },
  removeClient: function ({ id, clientId }) {
    return post("client/remove", { id, clientId });
  },
};
