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
      autocomplete="off"
      @finish="onFinish"
      @finishFailed="onFinishFailed"
    >
      <a-form-item label="Enabled">
        <a-checkbox v-model:checked="form.enabled" />
      </a-form-item>
      <a-form-item label="Client Id" name="clientId">
        <a-input v-model:value="form.clientId" />
      </a-form-item>
      <a-form-item label="Client Name">
        <a-input v-model:value="form.clientName" />
      </a-form-item>
      <a-form-item label="Client Uri">
        <a-input v-model:value="form.clientUri" />
      </a-form-item>
      <a-form-item label="Client Description">
        <a-input v-model:value="form.clientDescription" />
      </a-form-item>
      <a-form-item label="Logo Uri">
        <a-input v-model:value="form.logoUri" />
      </a-form-item>
      <a-form-item label="GrantType" name="allowedGrantTypes">
        <!-- <string-collection v-model="form.allowedGrantTypes" /> -->
        <a-select
          v-model:value="allowedGrantTypeKey"
          @change="updateAllowedGrantTypes()"
        >
          <a-select-option
            v-for="(v, k) in AllGrantTypes"
            :key="k"
            :value="k"
            >{{ k }}</a-select-option
          >
        </a-select>
      </a-form-item>
      <a-form-item label="Redirect Uris">
        <string-collection v-model:values="form.redirectUris" />
      </a-form-item>
      <a-form-item label="Post Logout Uris">
        <string-collection v-model:values="form.postLogoutRedirectUris" />
      </a-form-item>
      <a-form-item label="Client Secrets">
        <secret-collection v-model:values="form.clientSecrets" />
      </a-form-item>
      <a-form-item label="Front Logout Uri">
        <a-input v-model:value="form.frontChannelLogoutUri" />
      </a-form-item>
      <a-form-item label="Background Logout Uri">
        <a-input v-model:value="form.backChannelLogoutUri" />
      </a-form-item>
      <a-form-item label="Require Pkce">
        <a-checkbox v-model:checked="form.requirePkce" />
      </a-form-item>
      <a-form-item label="Require Consent">
        <a-checkbox v-model:checked="form.requireConsent" />
      </a-form-item>
      <a-form-item label="Allow RememberConsent">
        <a-checkbox v-model:checked="form.allowRememberConsent" />
      </a-form-item>
      <a-form-item label="Allow Offline Access">
        <a-checkbox v-model:checked="form.allowOfflineAccess" />
      </a-form-item>
      <a-form-item label="Require ClientSecret">
        <a-checkbox v-model:checked="form.requireClientSecret" />
      </a-form-item>
      <a-form-item label="Allowed Scopes" name="allowedScopes">
        <!-- <string-collection v-model="form.allowedScopes" /> -->
        <a-row>
          <a-col :span="20">
            <a-select v-model:value="form.allowedScopes" mode="multiple">
              <a-select-option
                v-for="(item, idx) in avaiableScopes"
                :key="item.key + '-' + idx"
                :value="item.key"
              >
                {{ item.label }} ( {{ item.key }})
              </a-select-option>
            </a-select>
          </a-col>
          <a-col :span="4">
            <sync-outlined
              style="margin-left: 1em; color: green"
              @click="loadAllResources"
            ></sync-outlined>
          </a-col>
        </a-row>
      </a-form-item>
      <a-form-item label="Allowed CorsOrigins">
        <string-collection v-model:values="form.allowedCorsOrigins" />
      </a-form-item>
      <a-form-item label="AccessToken Type">
        <!-- <a-input v-model="form.accessTokenType" /> -->
        <a-select v-model:value="form.accessTokenType">
          <a-select-option :value="0">Jwt</a-select-option>
          <a-select-option :value="1">Referene</a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item label="Access token lifetime(Second)">
        <a-input-number v-model:value="form.tokenLifetime" />
      </a-form-item>
      <a-form-item label="Claims included">
        <claims-edit v-model:values="form.clientClaims" />
      </a-form-item>

      <a-form-item label=" ">
        <a-button :loading="isSaving" type="primary" html-type="submit"
          >Save Client</a-button
        >
      </a-form-item>
    </a-form>
  </div>
</template>
<script>
import ClientApi from "@/api/client";
import ResourceApi from "@/api/apiResource.js";
import StringCollection from "./StringCollection.vue";
import SecretCollection from "./SecretCollection.vue";
import ClaimsEdit from "./ClaimsEdit.vue";
import AllGrantTypes from "./GrantTypes";
import { SyncOutlined } from "@ant-design/icons-vue";

const getEmptyClient = function () {
  return {
    id: 0,
    enabled: false,
    clientId: "",
    clientName: "",
    clientUri: "",
    logoUri: "",
    clientDescription: "",
    allowedGrantTypes: [],
    redirectUris: [],
    postLogoutRedirectUris: [],
    allowedCorsOrigins: [],
    clientSecrets: [],
    allowedScopes: [],
    clientClaims: [],
    requireConsent: true,
    allowRememberConsent: true,
    requireClientSecret: false,
    requirePkce: true,
    frontChannelLogoutUri: "",
    backChannelLogoutUri: "",
    allowOfflineAccess: true,
    accessTokenType: 0,
    tokenLifetime: 3600,
  };
};

export default {
  name: "ClientEdit",
  components: { StringCollection, SecretCollection, SyncOutlined, ClaimsEdit },
  props: {
    clientId: String,
  },
  data() {
    return {
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      form: getEmptyClient(),
      rules: {
        clientId: [
          { required: true, message: "Client Id is required", trigger: "blur" },
        ],
        allowedGrantTypes: [
          {
            type: "array",
            required: true,
            message: "Grant type required",
            trigger: "change",
          },
        ],
        allowedScopes: [
          {
            type: "array",
            required: true,
            message: "Please select at least one scope",
            trigger: "change",
          },
        ],
      },
      AllGrantTypes: AllGrantTypes,
      allowedGrantTypeKey: "",
      isLoading: false,
      isSaving: false,
      avaiableScopes: [],
    };
  },
  created() {
    this.loadAllResources();
    if (this.clientId) {
      this.loadClientInfo(this.clientId);
    } else {
      this.isLoading = false;
      this.isSaving = false;
      this.form = getEmptyClient();
    }
  },
  watch: {
    clientId: function (val) {
      if (val) {
        this.loadClientInfo(val);
      } else {
        this.isLoading = false;
        this.isSaving = false;
        this.form = getEmptyClient();
      }
    },
  },
  methods: {
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
    loadClientInfo(clientId) {
      if (!clientId) return;
      this.isLoading = true;
      ClientApi.getDetail(clientId).then((resp) => {
        if (resp.code === 0) {
          const data = resp.data;
          this.form = data;
          this.allowedGrantTypeKey = this.AllGrantTypes.FindKey(
            data.allowedGrantTypes
          );
        } else {
          this.$message.error(resp.message);
        }
        this.isLoading = false;
      });
    },
    updateAllowedGrantTypes() {
      if (this.allowedGrantTypeKey) {
        const values = this.AllGrantTypes[this.allowedGrantTypeKey];
        this.form.allowedGrantTypes = values;
      }
    },
    onFinish() {
      this.isSaving = true;
      ClientApi.saveClient(this.form).then(
        (resp) => {
          if (resp.code === 0) {
            this.$emit("saved");
          } else {
            this.$message.error(resp.message);
          }
          this.isSaving = false;
        },
        (error) => {
          this.$message.error(error.message);
          this.isSaving = false;
        }
      );
    },
    onFinishFailed(error) {
      console.log(error);
    },
    // saveClient() {
    //   this.$refs.clientForm.validate((valid) => {
    //     if (valid) {
    //       this.isSaving = true;
    //       ClientApi.saveClient(this.form).then((resp) => {
    //         if (resp.code === 0) {
    //           this.$emit("saved");
    //         } else {
    //           this.$message.error(resp.message);
    //         }
    //         this.isSaving = false;
    //       });
    //     }
    //   });
    // },
  },
};
</script>
