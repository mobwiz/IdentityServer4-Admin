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
      name="apiResourceForm"
      :model="form"
      :rules="rules"
      :label-col="labelCol"
      :wrapper-col="wrapperCol"
      :colon="false"
      autocomplete="off"
      @finish="onFinish"
      @finishFailed="onValidateFailed"
    >
      <a-form-item label=" " name="enabled">
        <a-checkbox v-model:checked="form.enabled">Enabled</a-checkbox>
      </a-form-item>
      <a-form-item label="Api Name" name="name">
        <a-input v-model:value="form.name" />
      </a-form-item>
      <a-form-item label="Display Name" name="displayName">
        <a-input
          v-model:value="form.displayName"
          aria-autocomplete="none"
          autocomplete="off"
        />
      </a-form-item>
      <a-form-item label="Api Secrets" required name="secrets">
        <secret-collection v-model:values="form.secrets" />
      </a-form-item>
      <a-form-item label="Scopes allows" required name="scopes">
        <!-- <string-collection v-model="form.scopes" /> -->
        <a-row>
          <a-col :span="20">
            <a-select v-model:value="form.scopes" mode="multiple">
              <a-select-option
                v-for="(item, idx) in avaiableScopes"
                :key="item.key + '-' + idx"
                :value="item.key"
              >
                {{ item.label }} ({{ item.key }})
              </a-select-option>
            </a-select>
          </a-col>
          <a-col :span="4">
            <sync-outlined
              class="operate-icon-green"
              @click="loadAllResources"
            ></sync-outlined>
          </a-col>
        </a-row>
      </a-form-item>
      <a-form-item label="User Claims required" name="claims">
        <string-collection v-model:values="form.claims" />
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
import StringCollection from "./StringCollection.vue";
import SecretCollection from "./SecretCollection.vue";
import ResourceApi from "@/api/apiResource.js";
import { SyncOutlined } from "@ant-design/icons-vue";

export default {
  name: "ApiResourceEdit",
  components: { StringCollection, SecretCollection, SyncOutlined },
  props: {
    apiResource: Object,
  },
  data() {
    return {
      isLoading: false,
      isSaving: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      form: {
        name: "",
        displayName: "",
        enabled: true,
        id: 0,
        claims: [],
        scopes: [],
        secrets: [],
      },
      avaiableScopes: [],
      rules: {
        name: [{ required: true, message: "Scope name is required" }],
        displayName: [{ required: true, message: "Display name is required" }],
        scopes: [
          {
            type: "array",
            required: true,
            message: "Please select at least one scope",
            trigger: "change",
          },
        ],
        secrets: [
          {
            type: "array",
            required: true,
            message: "Please input at least one secret",
            trigger: "change",
          },
        ],
      },
    };
  },
  created() {
    this.applyValue(this.apiResource);
  },
  mounted() {
    this.loadAllResources();
  },
  watch: {
    apiResource: function (value) {
      this.applyValue(value);
    },
  },
  methods: {
    onFinish(values) {
      console.log(values);
      // callbapi

      // let hasSecret = this.form.secrets.filter((x) => x.trim() != "");
      // if (!hasSecret.length) {
      //   this.$message.error("Please enter at least one secret");
      //   return false;
      // }

      // let hasScope = this.form.scopes.filter((x) => x.trim() != "");
      // if (!hasScope.length) {
      //   this.$message.error("Please enter at least one scope");
      //   return false;
      // }

      this.isSaving = true;
      ResourceApi.saveApiResource(this.form).then(
        (resp) => {
          if (resp.code === 0) {
            this.$emit("saved");
            this.$message.success("Save success");
          } else {
            this.$message.error(resp.message);
          }
          this.isSaving = false;
        },
        (err) => {
          console.log(err);
          this.isSaving = false;
        }
      );
    },
    onValidateFailed() {},

    applyValue(value) {
      console.log(value);
      if (value) {
        this.form = value;
      } else {
        this.form = {
          name: "",
          displayName: "",
          enabled: true,
          id: 0,
          claims: [],
          scopes: [],
          secrets: [],
        };
      }
    },
    loadAllResources() {
      this.isLoading = true;
      ResourceApi.getAvaiableScopes({
        keyword: "",
      }).then(
        (resp) => {
          this.avaiableScopes = [];
          if (resp.code === 0) {
            const data = resp.data.avaiableScopes;
            for (let scope of data) {
              // if (scope.enabled) {
              this.avaiableScopes.push({
                key: scope.name,
                label: scope.description,
              });
              // }
            }
          } else {
            this.$message.error(resp.message);
          }
          this.isLoading = false;
        },
        (err) => {
          console.log(err);
          this.isLoading = false;
        }
      );
    },
  },
};
</script>
