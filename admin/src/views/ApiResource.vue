<template>
  <div>
    <div class="title-cont">
      <h2>Api Resources</h2>
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
              <a-button type="info" @click="loadApiResources(1)"
                >Search</a-button
              >
            </a-form-item>
          </a-form>
        </a-col>
        <a-col :span="6" style="text-align: right">
          <a-button type="primary" @click="newApiResource()"
            >Create ApiResource</a-button
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
        <template v-if="column.dataIndex === 'scopes'">
          <div>
            <div v-for="(str, idx) in record.scopes" :key="idx + '-' + str">
              - {{ str }}
            </div>
          </div>
        </template>
        <template v-if="column.dataIndex === 'operation'">
          <a-button type="link" @click="editItem(record)">Edit</a-button>
          <span>&nbsp;</span>
          <a-popconfirm
            placement="leftBottom"
            okType="danger"
            ok-text="Remove"
            cancel-text="Cancel"
            @confirm="removeItem(record)"
            :title="`Are your sure ApiResource: ${record.displayName}?`"
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
    <div>
      <a-drawer
        :width="600"
        title="Edit ApiResource"
        :visible="drawerVisible"
        @close="onEditClose"
      >
        <api-resource-edit
          :api-resource="currentApiResource"
          @saved="onEditClose"
        />
      </a-drawer>
    </div>
  </div>
</template>
<script>
import ResourceApi from "@/api/apiResource.js";
import ApiResourceEdit from "./components/ApiResourceEdit.vue";

const columns = [
  { title: "Id", width: 100, dataIndex: "id", key: "id" },
  {
    title: "Enabled",
    width: 100,
    dataIndex: "enabled",
    key: "enabled",
    scopedSlots: { customRender: "enabled" },
  },
  { title: "Api Name", dataIndex: "name", key: "name" },
  { title: "Display Name", dataIndex: "displayName", key: "displayName" },
  {
    title: "Scopes",
    dataIndex: "scopes",
    scopedSlots: { customRender: "scopes" },
  },
  {
    title: "Operation",
    dataIndex: "operation",
    scopedSlots: { customRender: "operation" },
    width: 180,
  },
];

export default {
  name: "ApiResource",
  components: {
    ApiResourceEdit,
  },
  data() {
    return {
      isLoading: false,
      drawerVisible: false,
      dataList: [],
      columns: columns,
      total: 0,
      so: {
        pageIndex: 1,
        pageSize: 10,
        keyword: "",
        onlyEnabled: false,
      },
      currentApiResource: undefined,
      totalCount: 0,
      loading: false,
    };
  },
  created() {
    this.loadApiResources(1);
  },
  watch: {},
  methods: {
    onShowSizeChange(current, size) {
      this.so.pageSize = size;
      this.loadApiResources(current);
    },
    onPageChange(pn) {
      this.loadApiResources(pn);
    },
    loadApiResources(pn) {
      this.so.pageIndex = pn || this.so.pageIndex;
      this.loading = true;
      ResourceApi.getApiResources({
        pageIndex: this.so.pageIndex,
        pageSize: this.so.pageSize,
        keyword: this.so.keyword,
        onlyEnabled: this.so.onlyEnabled,
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
    newApiResource() {
      this.currentApiResource = undefined;
      this.drawerVisible = true;
    },
    onEditClose() {
      this.drawerVisible = false;
      this.currentApiResource = {};
      this.loadApiResources();
    },
    removeItem(item) {
      ResourceApi.removeApiResource(item.id).then((resp) => {
        if (resp.code === 0) {
          this.$message.success("删除成功");
          this.loadApiResources();
        } else {
          this.$message.error(resp.message);
        }
      });
    },
    editItem(item) {
      this.currentApiResource = item;
      this.drawerVisible = true;
    },
  },
};
</script>
