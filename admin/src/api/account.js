import { get, post } from "@/utils/request";
// import { GetKey } from "@/utils/encrypt";

export default {
  // 获取登录信息
  getLoginInfo() {
    return get("account/prelogin");
  },
  // 登录
  doLogin({ username, password }) {
    // const key = GetKey();
    // const passwordHashed = EncryptSecret(password, key);
    return post("account/webLogin", {
      username,
      password: password,
      validateCode: "1111",
    });
  },
  // 登出
  doLogout() {
    return get("account/webLogout");
  },
  // 获取用户信息
  getUserInfo() {
    return get("account/getUserInfo");
  },
};
