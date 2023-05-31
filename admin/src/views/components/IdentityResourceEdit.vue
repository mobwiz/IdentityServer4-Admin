<style lang="less" scoped>
.ant-form-item {
  margin-bottom: 10px;
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
      <a-form-item label=" " name="enabled">
        <a-checkbox v-model:checked="form.enabled">Enabled</a-checkbox>
      </a-form-item>
      <a-form-item label=" " name="required">
        <a-checkbox v-model:checked="form.required">Required</a-checkbox>
      </a-form-item>
      <a-form-item label=" " name="emphasize">
        <a-checkbox v-model:checked="form.emphasize">Emphasize</a-checkbox>
      </a-form-item>
      <a-form-item label="Resource name" name="name">
        <a-input v-model:value="form.name" />
      </a-form-item>
      <a-form-item label="Display Name" name="displayName">
        <a-input v-model:value="form.displayName" />
      </a-form-item>
      <a-form-item label="Claims included" name="claims">
        <string-collection v-model:values="form.claims" />
      </a-form-item>
      <a-form-item label=" ">
        <a-button type="primary" html-type="submit" :loading="isSaving"
          >Save Identity Resource</a-button
        >
      </a-form-item>
    </a-form>
  </div>
</template>
<script>
import StringCollection from "./StringCollection.vue";
import ResourceApi from "@/api/identityResource.js";

export default {
  name: "IdentityResourceEdit",
  components: { StringCollection },
  props: {
    resource: Object,
  },
  data() {
    return {
      isLoading: false,
      isSaving: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      form: {
        enabled: true,
        required: false,
        emphasize: false,
        name: "",
        displayName: "",
        claims: [],
        id: 0,
      },
      rules: {
        name: [{ required: true, message: "Resource name is required" }],
        displayName: [{ required: true, message: "Display name is required" }],
        claims: [
          {
            type: "array",
            required: true,
            message: "Please select at least one claim",
            trigger: "change",
          },
        ],
      },
    };
  },
  created() {
    this.applyScope(this.resource);
  },
  watch: {
    resource: function (value) {
      this.applyScope(value);
    },
  },
  methods: {
    onFinish() {
      //this.saveScope();
      // callbapi
      this.isSaving = true;
      ResourceApi.saveIdentityResource(this.form).then(
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
    applyScope(value) {
      if (value) {
        this.form = value;
      } else {
        this.form = {
          claims: [],
          displayName: "",
          emphasize: false,
          enabled: true,
          id: 0,
          name: "",
        };
      }
    },
  },
};
</script>
