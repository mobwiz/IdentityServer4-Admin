<style lang="less" scoped></style>
<template>
  <div>
    <div class="title-cont">
      <h2>Api Scopes</h2>
    </div>
    <div class="search-box">
      <a-row>
        <a-col :span="18">
          <a-form layout="inline">
            <a-form-item label="Keyword">
              <a-input v-model:value="so.keyword" />
            </a-form-item>
            <a-form-item>
              <a-checkbox v-model:checked="so.onlyEnabled"
                >Only Enabled</a-checkbox
              >
            </a-form-item>
            <a-form-item>
              <a-button type="info" @click="loadApiScopes(1)">Search</a-button>
            </a-form-item>
          </a-form>
        </a-col>
        <a-col :span="6" style="text-align: right">
          <a-button type="primary" @click="newApiScope()"
            >Create ApiScope</a-button
          >
        </a-col>
      </a-row>
    </div>
    <a-table
      :columns="columns"
      :data-source="dataList"
      rowKey="id"
      :pagination="false"
      :loading="loading"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.dataIndex === 'enabled'">
          <span>{{ record.enabled ? "Enabled" : "Disabled" }} </span>
        </template>

        <template v-if="column.dataIndex === 'required'">
          <span>{{ record.required ? "Y" : "N" }} </span>
        </template>
        <template v-if="column.dataIndex === 'emphasize'">
          <span>{{ record.emphasize ? "Y" : "N" }} </span>
        </template>
        <template v-if="column.dataIndex === 'claims'">
          <span> - {{ record.claims.join(",") }} </span>
        </template>
        <template v-if="column.dataIndex === 'operation'">
          <a-button type="link" @click="editItem(record)">Edit</a-button>
          <span>&nbsp;</span>
          <a-popconfirm
            placement="leftBottom"
            okType="danger"
            ok-text="Remove"
            cancel-text="Cancel"
            :title="`Are you sure to remove this item ${record.displayName}?`"
            @confirm="removeItem(record)"
          >
            <a-button type="link">Remove</a-button>
          </a-popconfirm>
        </template>
      </template>
    </a-table>
    <div style="padding: 15px">
      <a-pagination
        :current="so.pageIndex"
        :total="totalCount"
        show-size-changer
        @showSizeChange="onShowSizeChange"
        @change="onPageChange"
      />
    </div>
    <a-drawer
      :title="'Edit ApiScope '"
      :width="600"
      :visible="drawerVisible"
      @close="onEditClose"
    >
      <scope-edit :scope="currentScope" @saved="onEditClose" />
    </a-drawer>
  </div>
</template>
<script>
import ResourceApi from "@/api/apiScope";
import ScopeEdit from "./components/ApiScopeEdit.vue";
import { defineComponent } from "vue";

const columns = [
  { title: "Id", width: 100, dataIndex: "id", key: "id" },
  {
    title: "Enabled",
    width: 100,
    dataIndex: "enabled",
    key: "enabled",
    scopedSlots: { customRender: "enabled" },
  },
  {
    title: "Required",
    width: 100,
    dataIndex: "required",
    key: "required",
    scopedSlots: { customRender: "required" },
  },
  {
    title: "Emphasize",
    width: 120,
    dataIndex: "emphasize",
    key: "emphasize",
    scopedSlots: { customRender: "emphasize" },
  },
  { title: "Scope Name", dataIndex: "name", key: "name" },
  { title: "Display Name", dataIndex: "displayName", key: "displayName" },
  {
    title: "Claims",
    dataIndex: "claims",
    scopedSlots: { customRender: "claims" },
  },
  {
    title: "Operations",
    dataIndex: "operation",
    scopedSlots: { customRender: "operation" },
    width: 180,
  },
];

export default defineComponent({
  name: "ApiScope",
  components: { ScopeEdit },
  // setup() {
  //   const so = ref({
  //     pageIndex: 1,
  //     keyword: "",
  //     pageSizes: 10,
  //     onlyEnabled: false,
  //   });

  //   return {
  //     so,
  //   };
  // },
  data() {
    return {
      dataList: [],
      totalCount: 0,
      columns: columns,
      drawerVisible: false,
      currentScope: null,
      so: {
        pageIndex: 1,
        keyword: "",
        pageSizes: 10,
        onlyEnabled: false,
      },
      loading: false
    };
  },
  created() {
    this.loadApiScopes(1);
  },
  methods: {
    onShowSizeChange(current, size) {
      this.so.pageSize = size;
      this.loadApiScopes(current);
    },
    newApiScope() {
      this.drawerVisible = true;
      this.currentScope = null;
    },
    loadApiScopes(pn) {
      this.pageIndex = pn;
      console.log(this.so);
      this.loading = true;
      ResourceApi.getApiScopes({
        pageIndex: this.so.pageIndex,
        pageSize: this.so.pageSize,
        onlyEnabled: this.so.onlyEnabled,
        keyword: this.so.keyword,
      }).then((resp) => {
        if (resp.code === 0) {
          this.dataList = resp.data.items;
          this.totalCount = resp.data.totalCount;
        } else {
          this.$message.error(resp.message);
        }
        this.loading = false;
      });
    },
    editItem(item) {
      //
      console.log(item);
      this.currentScope = item;
      this.drawerVisible = true;
    },
    removeItem(item) {
      ResourceApi.removeApiScope(item.id).then((resp) => {
        if (resp.code === 0) {
          this.loadApiScopes();
          this.$message.success("Remove success!");
        } else {
          this.$message.error(resp.message);
        }
      });
    },
    onEditClose() {
      this.drawerVisible = false;
      this.currentScope = null;
      this.loadApiScopes();
    },
    onPageChange(e) {
      this.pageIndex = e;
      this.loadApiScopes(e);
    },
  },
});
</script>
