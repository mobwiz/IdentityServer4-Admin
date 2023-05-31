<style lang="less" scoped>
.ant-form-item {
  margin-bottom: 10px;
}
.operate-icon-green {
  height: 32px;
  color: green;
  display: table-cell;
  vertical-align: middle;
  width: 32px;
}
</style>
<template>
  <div>
    <a-spin :spinning="isLoading" />
    <a-form
      v-if="!isLoading"
      :model="form"
      :rules="rules"
      :label-col="labelCol"
      :wrapper-col="wrapperCol"
      :colon="false"
      @finish="onFinish"
      @finishFailed="onFinishFailed"
    >
      <a-form-item label="Username" name="username">
        <a-input autocomplete="off" v-model:value="form.username" />
      </a-form-item>
      <a-form-item label="Password" name="password">
        <a-input-password
          aria-autocomplete="new-password"
          autocomplete="new-password"
          v-model:value="form.password"
        />
      </a-form-item>
      <a-form-item label=" ">
        <a-button type="primary" html-type="submit" :loading="isSaving"
          >Save</a-button
        >
      </a-form-item>
    </a-form>
  </div>
</template>

<script>
import AdminUerApi from "@/api/adminUser";

export default {
  props: {},
  data() {
    return {
      isLoading: false,
      isSaving: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      form: {
        username: null,
        password: null,
      },
      rules: {
        username: [
          {
            required: true,
            message: "Please input the username",
          },
          {
            min: 4,
            message: "Username must have a length greater or equal than 4",
          },
        ],
        password: [
          {
            min: 8,
            message: "Password must have a length greater or equal than 8",
          },
        ],
      },
    };
  },
  created() {
    this.form = {
      username: null,
      password: null,
    };
  },
  watch: {},
  methods: {
    onFinish() {
      //this.saveScope();
      // callbapi
      this.isSaving = true;
      AdminUerApi.createUser(this.form).then(
        (resp) => {
          if (resp.code === 0) {
            this.$emit("saved");
          } else {
            this.$message.error(resp.message);
          }
          this.isSaving = false;
        },
        (err) => {
          this.$message.error(err.message);
          this.isSaving = false;
        }
      );
    },
    onFinishFailed() {
      // this.$message.error("Please check the form");
    },
  },
  beforeUnmount() {
    this.form = {
      username: null,
      password: null,
    };
  },
};
</script>
