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
              <a-button type="info" @click="loadUsers(1)">Search</a-button>
            </a-form-item>
          </a-form>
        </a-col>
        <a-col :span="6" style="text-align: right">
          <a-button type="primary" @click="createAdminUser()"
            >Create user</a-button
          >
        </a-col>
      </a-row>
    </div>
    <a-table
      :columns="columns"
      :data-source="dataList"
      rowKey="id"
      :pagination="false"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.dataIndex === 'isFreezed'">
          <span>{{ record.isFreezed ? "Freezed" : "-" }} </span>
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
      :title="'Edit User'"
      :width="600"
      :visible="editDrawerVisible"
      @close="onEditClose"
    >
      <AdminUserEdit :user="currentUser" @saved="onSaved" />
    </a-drawer>
    <a-drawer
      :title="'Create User'"
      :width="600"
      :visible="createDrawerVisible"
      @close="onCreateClose"
    >
      <AdminUserCreate @saved="onCreated" />
    </a-drawer>
  </div>
</template>

<script>
const columns = [
  { title: "Id", width: 100, dataIndex: "id", key: "id" },
  {
    title: "Username",
    width: 200,
    dataIndex: "username",
    key: "username",
  },
  {
    title: "IsFreezed",
    width: 120,
    dataIndex: "isFreezed",
    key: "isFreezed",
  },
  {
    title: "CreationTime",
    width: 200,
    dataIndex: "createTime",
    key: "createTime",
  },
  {
    title: "LastLoginTime",
    dataIndex: "lastLoginTime",
    key: "lastLoginTime",
  },
  {
    title: "Last Login IP",
    dataIndex: "lastLoginIp",
    key: "lastLoginIp",
  },
  {
    title: "Operations",
    dataIndex: "operation",
    scopedSlots: { customRender: "operation" },
    width: 180,
  },
];

import AdminUserApi from "@/api/adminUser";
import AdminUserEdit from "./components/AdminUserEdit.vue";
import AdminUserCreate from "./components/AdminUserCreate.vue";

export default {
  name: "AdminUser",
  components: {
    AdminUserEdit,
    AdminUserCreate,
  },
  data() {
    return {
      so: {
        pageIndex: 1,
        pageSize: 10,
        keyword: "",
      },
      columns,
      dataList: [],
      totalCount: 0,
      editDrawerVisible: false,
      currentUser: null,
      createDrawerVisible: false,
    };
  },
  mounted() {
    this.loadUsers(1);
  },
  methods: {
    loadUsers(pageIndex) {
      this.so.pageIndex = pageIndex;
      AdminUserApi.queryUsers(this.so).then((res) => {
        if (res.code === 0) {
          this.dataList = res.data.items;
          this.totalCount = res.data.totalCount;
        } else {
          this.$message.error(res.message);
        }
      });
    },
    editItem(item) {
      this.currentUser = item;
      this.editDrawerVisible = true;
    },
    onSaved() {
      this.onEditClose();
      this.loadUsers(this.so.pageIndex);
    },
    onEditClose() {
      this.editDrawerVisible = false;
    },
    createAdminUser() {
      this.createDrawerVisible = true;
    },
    onCreateClose() {
      this.createDrawerVisible = false;
    },
    onCreated() {
      this.onCreateClose();
      this.loadUsers(1);
    },
    removeItem(item) {
      AdminUserApi.removeUser(item.id).then((res) => {
        if (res.code === 0) {
          this.$message.success("Removed successfully");
          this.loadUsers(this.so.pageIndex);
        } else {
          this.$message.error(res.message);
        }
      });
    },
  },
};
</script>

<style></style>
