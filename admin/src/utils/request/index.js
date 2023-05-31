import axios from "axios";
import { message } from "ant-design-vue";
import { VueAxios } from "./axios";

const defaultLang = (
  navigator.browserLanguage || navigator.language
).toLowerCase();

const getHeader = function () {
  const header = {
    "Accept-Language": localStorage.getItem("lang") || defaultLang,
  };
  // const csrfToken = Cookies.get("XSRF-TOKEN");
  // if (csrfToken) {
  //   header["X-XSRF-TOKEN"] = csrfToken;
  // }
  return header;
};

// 创建 axios 实例
const service = axios.create({
  baseURL: process.env.VUE_APP_API_BASE_URL, // api base_url
  timeout: 30000, // 请求超时时间
  headers: getHeader(),
  withCredentials: true, // 允许携带cookie
});

const err = (error) => {
  // console.log(error)
  if (error.response) {
    if (error.response.status === 400) {
      message.error(`错误代码：${error.response.status}，请求出错`);
    } else if (error.response.status === 401) {
      // 跳转登录页面了
      // const curUrl = window.location.toString()
      window.location = `/#/login`;
    } else {
      // const data = error.response.data;
      message.error(`错误代码：${error.response.status}，请求出错`);
    }
  }
  return Promise.reject(error);
};

// service.interceptors.request.use((config) => {
//     const token = Vue.ls.get('ACCESS_TOKEN');
//     if (token) {
//         config.headers['Access-Token'] = token; // 让每个请求携带自定义 token 请根据实际情况自行修改
//     }
//     return config;
// }, err);

service.interceptors.response.use((response) => {
  return response.data;
}, err);

const installer = {
  vm: {},
  install(Vue) {
    Vue.use(VueAxios, service);
  },
};

const get = (url, params) =>
  service({
    method: "get",
    url: url,
    params: params,
  });

const post = (url, data) =>
  service({
    method: "post",
    url: url,
    data: data,
  });

const getAbs = (url, params) => {
  const request = axios.create({
    baseURL: "/", // api base_url
    timeout: 30000, // 请求超时时间
    headers: { "Accept-Language": localStorage.getItem("lang") || "zh-CN" },
    withCredentials: true, // 允许携带cookie
  });
  request.interceptors.response.use((response) => {
    return response.data;
  }, err);

  return request({
    method: "get",
    url: url,
    params: params,
  });
};

export { installer as VueAxios, service as axios, get, post, getAbs };
