<style lang="less" scoped></style>
<template>
  <div>
    <div class="title-cont">
      <h2>Identity Resources</h2>
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
              <a-button type="info" @click="loadIdentityResources(1)"
                >Search</a-button
              >
            </a-form-item>
          </a-form>
        </a-col>
        <a-col :span="6" style="text-align: right">
          <a-button type="primary" @click="newIdentityResource()"
            >Create Identity Resource</a-button
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
          <span>{{ record.enabled ? "Y" : "N" }} </span>
        </template>
        <template v-if="column.dataIndex === 'claims'">
          <div>
            <div v-for="c in record.claims" :key="c">{{ c }}</div>
          </div>
        </template>
        <template
          v-if="column.dataIndex === 'operation' && record.isPreset === 0"
        >
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
      :title="'Edit Identity Resource '"
      :width="600"
      :visible="editDrawerVisible"
      @close="onEditClose"
    >
      <identity-resource-edit
        :resource="currentIdentityResource"
        @saved="onSaved"
      />
    </a-drawer>
  </div>
</template>
<script>
import ResourceApi from "@/api/identityResource";
import IdentityResourceEdit from "./components/IdentityResourceEdit.vue";

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
    title: "Operation",
    dataIndex: "operation",
    scopedSlots: { customRender: "operation" },
    width: 180,
  },
];
export default {
  name: "IdentityResource",
  components: {
    IdentityResourceEdit,
  },
  data() {
    return {
      dataList: [],
      columns: columns,
      totalCount: 0,
      so: {
        pageIndex: 1,
        keyword: "",
        pageSizes: 10,
        onlyEnabled: false,
      },
      currentIdentityResource: null,
      editDrawerVisible: false,
      loading: false,
    };
  },
  created() {
    this.loadIdentityResources(1);
  },
  methods: {
    onShowSizeChange(current, pageSize) {
      this.so.pageSizes = pageSize;
      this.loadIdentityResources(current);
    },
    onPageChange(pageNumber) {
      this.loadIdentityResources(pageNumber);
    },
    loadIdentityResources(pn) {
      this.so.pageIndex = pn;
      this.loading = true;

      ResourceApi.getIdentityResources({
        pageIndex: this.so.pageIndex,
        pageSize: this.so.pageSizes,
        keyword: this.so.keyword,
        onlyEnabled: this.so.onlyEnabled,
      }).then((resp) => {
        if (resp.code === 0) {
          //
          this.dataList = resp.data.items;
          this.totalCount = resp.data.totalCount;
        } else {
          this.$message.error(resp.message);
        }
        this.loading = false;
      });
    },
    newIdentityResource() {
      this.currentIdentityResource = null;
      this.editDrawerVisible = true;
    },
    onEditClose() {
      this.editDrawerVisible = false;
      this.currentIdentityResource = null;
    },
    onSaved() {
      this.editDrawerVisible = false;
      this.currentIdentityResource = null;

      this.loadIdentityResources(this.so.pageIndex);
    },
    editItem(item) {
      console.log(item);
      this.currentIdentityResource = item;
      this.editDrawerVisible = true;
    },
    removeItem(item) {
      ResourceApi.removeIdentityResource(item.id).then((resp) => {
        if (resp.code === 0) {
          this.$message.success("Remove success");
          this.loadIdentityResources(this.so.pageIndex);
        } else {
          this.$message.error(resp.message);
        }
      });
    },
  },
};
</script>
