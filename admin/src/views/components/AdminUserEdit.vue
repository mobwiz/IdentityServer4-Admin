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
      <a-form-item label="Current Username">
        <a-input v-model:value="currentUsername" disabled readonly />
      </a-form-item>
      <a-form-item label="New password" name="newPassword">
        <a-input-password
          aria-autocomplete="new-password"
          v-model:value="form.newPassword"
        />
      </a-form-item>
      <a-form-item label=" " name="isFreezed">
        <a-checkbox v-model:checked="form.isFreezed">Freezed</a-checkbox>
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
  props: {
    user: {
      type: Object,
      required: true,
      default: () => {
        return {
          id: 0,
          username: null,
        };
      },
    },
  },
  data() {
    return {
      isLoading: false,
      isSaving: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      form: {
        id: 0,
        newPassword: null,
        isFreezed: false,
      },
      rules: {
        newPassword: [
          {
            min: 8,
            message: "Password must have a length greater or equal than 8",
          },
        ],
      },
      currentUsername: null,
    };
  },
  mounted() {
    const { id, username } = this.user;
    this.currentUsername = username;
    this.form.id = id;
  },
  watch: {
    user: {
      handler: function (val) {
        console.log(val);
        const { id, username } = val;
        this.currentUsername = username;
        this.form.id = id;
        this.form.newPassword = null;
      },
      deep: true,
    },
  },
  methods: {
    onFinish() {
      //this.saveScope();
      // callbapi
      this.isSaving = true;
      AdminUerApi.updateUser(this.form).then(
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
};
</script>
