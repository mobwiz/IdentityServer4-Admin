import { createApp } from "vue";
import Antd from "ant-design-vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import { createI18n } from "vue-i18n";

import messageEn from "./locales/en-us.json";
import messageZh from "./locales/zh-cn.json";

import { getLanguage } from "./utils/common.js";

const i18n = createI18n({
  locale: getLanguage(),
  messages: {
    "en-us": messageEn,
    "zh-cn": messageZh,
  },
});

import "ant-design-vue/dist/antd.css";

const app = createApp(App);

app.use(Antd).use(i18n).use(store).use(router).mount("#app");
