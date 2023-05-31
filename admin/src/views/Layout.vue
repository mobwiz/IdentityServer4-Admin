<style lang="less">
#_left-side .logo {
  height: 55px;
  margin: 15px 10px;
  background: rgba(255, 255, 255, 1);
  background-image: url("../assets/idsadmui.png");
  background-size: cover;
  background-repeat: no-repeat;
}

.sep-box {
  height: 0px;
  border-bottom: 1px solid #efefef;
}

.site-layout .site-layout-background {
  background: #fff;
}
// [data-theme="dark"] .site-layout .site-layout-background {
//   background: #141414;
// }

.main-content {
  // position: absolute;
  // height: 100%;
  // left: 200px;
  padding-top: 15px;

  .search-box {
    padding: 20px;
    // border: 1px solid;
    background: #fefefe;
    // border-top: 1px solid #efefef;
    // border-bottom: 1px solid #ccc;
    margin-bottom: 20px;
  }
  .title-cont {
    line-height: 48px;
    font-size: 20px;
    margin: 12px 0;
    // border-bottom: 1px solid #ccc;
    // background: #141c24;
    h2 {
      font-size: 20px;
      // color: white;
      margin: 0;
      padding: 0;
      // padding-left: 15px;
    }
    h2:before {
      content: "";
    }
    display: none;
  }
}
</style>
<template>
  <a-layout style="min-height: 100vh">
    <a-layout-sider
      id="_left-side"
      theme="light "
      v-model:collapsed="collapsed"
      collapsible
    >
      <div class="logo"></div>
      <div class="sep-box"></div>
      <a-menu v-model:selectedKeys="selectedKeys" mode="inline">
        <a-sub-menu key="m-configure">
          <template #icon>
            <setting-outlined />
          </template>
          <template #title>
            <span>Configure</span>
          </template>
          <a-menu-item
            key="m-identityresource"
            @click="$router.push('/identity-resource')"
            >Identity Resources</a-menu-item
          >
          <a-menu-item key="m-apiscope" @click="$router.push('/apiscope')"
            >Api Scopes</a-menu-item
          >
          <a-menu-item
            key="m-apiresource"
            @click="$router.push('/api-resource')"
            >Api Resources</a-menu-item
          >
          <a-menu-item key="m-clients" @click="$router.push('/client')"
            >Clients</a-menu-item
          >
        </a-sub-menu>

        <a-sub-menu key="m-debug">
          <template #title>
            <span>
              <bug-outlined />
              <span>Analyse</span>
            </span>
          </template>
          <a-menu-item
            key="m-accesstoken"
            @click="$router.push('/access-tokens')"
            >Access Tokens</a-menu-item
          >
        </a-sub-menu>

        <a-sub-menu>
          <template #title>
            <span>
              <user-outlined />
              <span>Account</span>
            </span>
          </template>
          <a-menu-item key="m-admin-users" @click="$router.push('/admin-user')"
            >Users</a-menu-item
          >
        </a-sub-menu>

        <!-- <a-menu-item key="m-logout" @click="doLogout()">
          <logout-outlined />
          <span>Logout</span>
        </a-menu-item> -->
      </a-menu>
    </a-layout-sider>
    <a-layout>
      <a-layout-header style="background: #fff; padding: 0">
        <a-row>
          <a-col :span="16">
            <a-page-header
              :title="currentTitle"
              :back-icon="false"
              :footer="false"
            ></a-page-header>
          </a-col>
          <a-col :span="8" style="text-align: right">
            <a-button @click="doLogout()" style="margin-right: 15px">
              <logout-outlined />
              <span>Logout</span>
            </a-button>
          </a-col>
        </a-row>
      </a-layout-header>
      <a-layout-content style="margin: 0 16px">
        <!-- <a-breadcrumb style="margin: 16px 0"> -->
        <!-- <a-breadcrumb-item>User</a-breadcrumb-item> -->
        <!-- <a-breadcrumb-item>Bill</a-breadcrumb-item> -->
        <!-- </a-breadcrumb> -->
        <div class="main-content">
          <router-view />
        </div>
      </a-layout-content>
      <a-layout-footer style="text-align: center">
        Ant Design ©2018 Created by Ant UED
      </a-layout-footer>
    </a-layout>
  </a-layout>
</template>

<script>
import AccountApi from "@/api/account";
import {
  LogoutOutlined,
  SettingOutlined,
  BugOutlined,
  UserOutlined,
} from "@ant-design/icons-vue";

const titleDict = {
  "/client": "Clients",
  "/apiscope": "Api Scopes",
  "/api-resource": "Api Resources",
  "/access-tokens": "Access Tokens",
  "/identity-resource": "Identity Resources",
};

export default {
  name: "Layout",
  components: {
    LogoutOutlined,
    SettingOutlined,
    BugOutlined,
    UserOutlined,
  },
  data() {
    return {
      collapsed: false,
      selectedKeys: [],
      currentTitle: "Home",
    };
  },
  watch: {
    $route(to, from) {
      console.log(to);
      console.log(from);
      // this.selectedKeys = [to.name];
      const path = to.path;
      if (path in titleDict) {
        this.currentTitle = titleDict[path];
      } else {
        this.currentTitle = "Home";
      }
    },
  },
  created() {
    AccountApi.getUserInfo()
      .then((resp) => {
        if (resp.code === 0) {
          this.$store.commit("setUserinfo", resp.data);
        } else {
          this.$router.push("/login");
        }
      })
      .catch((err) => {
        console.log(err);
        this.$router.push("/login");
      });

    // if (this.$store.state.user == null) {
    //   this.$router.push("/login");
    // }
  },
  methods: {
    doLogout() {
      this.$confirm({
        title: "Confirmation",
        content: "Sure to logout？",
        okText: "Logout",
        cancelText: "Cancel",
        onOk: function () {
          return new Promise((resolve, reject) => {
            AccountApi.doLogout().then((resp) => {
              if (resp.code === 0) {
                // this.$router.push("/login");
                window.location = "#/login";
                resolve();
              } else {
                this.$message.error(resp.message);
                reject();
              }
            });
          });
        },
      });
    },
  },
};
</script>
