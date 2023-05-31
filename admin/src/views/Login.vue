<style lang="less">
/* 背景 */
.login-container {
  position: absolute;
  width: 100%;
  height: 100%;
  // background: url("../assets/houTaiBg.png");
  background: url("../assets/bg.jpg");
  background-position: center;
  background-size: cover;

  /* Log */
  .login-title {
    color: #fff;
    text-align: center;
    margin: 0;
    margin-top: calc(40vh - 180px);
    font-size: 28px;
    font-family: Microsoft Yahei;
  }

  .login-form {
    width: 565px;
    height: 360px;
    margin: 20px auto 0 auto;
    background: rgba(0, 0, 0, 0.7);
    padding: 40px 110px;
    border-radius: 20px;

    .ant-form-item-label > label {
      // padding-left: 0;
      color: #f1f1f1;
    }
  }
  /* 登陆按钮 */
  .ant-btn-primary {
    width: 100%;
    height: 45px;
    font-size: 14px;
    margin-top: 30px;
    border-radius: 4px;
  }

  /* 输入框 */
  .inputBox {
    height: 45px;
  }
  /* 输入框内左边距50px */
  .ant-input-affix-wrapper .ant-input:not(:first-child) {
    padding-left: 50px;
  }
}

/* 用户登陆标题 */
.title {
  margin-bottom: 50px;
  color: #fff;
  font-weight: 700;
  font-size: 24px;
  font-family: Microsoft Yahei;
}
</style>
<template>
  <div class="login-container">
    <div class="login-title">Identity Server Admin</div>
    <a-form
      :model="formState"
      layout="vertical"
      class="login-form"
      autocomplete="off"
      @finish="onFinish"
      @finishFailed="onFinishFailed"
    >
      <a-form-item
        label="Username"
        name="username"
        autofocus="autofocus"
        :rules="[{ required: true, message: 'Please input your username!' }]"
      >
        <a-input
          class="inputBox"
          autocomplete="off"
          placeholder="Username"
          v-model:value="formState.username"
        />
      </a-form-item>

      <a-form-item
        label="Password"
        name="password"
        :rules="[{ required: true, message: 'Please input your password!' }]"
      >
        <a-input-password
          autocomplete="new-password"
          placeholder="Password"
          class="inputBox"
          v-model:value="formState.password"
        />
      </a-form-item>
      <!-- <a-form-item
        name="validateCode"
        :rules="[{ required: true, message: 'Please input validateCode!' }]"
      >
        <a-input class="inputBox" v-model:value="formState.validateCode" />
      </a-form-item> -->
      <a-form-item>
        <a-button
          type="primary"
          html-type="submit"
          class="submit"
          :loading="isLoading"
        >
          Login</a-button
        >
      </a-form-item>
    </a-form>
  </div>
</template>
<script>
// import { defineComponent, reactive } from "vue";
import { message } from "ant-design-vue";
import AccountApi from "@/api/account";
export default {
  data() {
    return {
      formState: {
        username: "",
        password: "",
        validateCode: "12345",
        remember: true,
      },
      isLoading: false,
    };
  },
  created() {
    console.log("login created");
    console.log(this.$store);
  },
  methods: {
    onFinish(values) {
      // console.log("Success:", values);
      console.log("go to login");
      const that = this;
      this.isLoading = true;
      AccountApi.doLogin(values).then(
        (res) => {
          if (res.code != 0) {
            message.error(res.message);
          } else {
            AccountApi.getUserInfo().then((res2) => {
              if (res2.code === 0) {
                that.$store.commit("setUserinfo", res2.data);
                that.$router.push("./");
              } else {
                that.$router.push("./login");
              }
            });
          }
          that.isLoading = false;
        },
        (err) => {
          console.log(err);
          that.isLoading = false;
        }
      );
    },
    onFinishFailed(errorInfo) {
      console.log("Failed:", errorInfo);
    },
  },
};
</script>
