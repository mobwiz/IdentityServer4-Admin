<style lang="less" scoped></style>
<template>
  <div>
    <div class="title-cont">
      <h2>Clients</h2>
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
                >Only enabled</a-checkbox
              >
            </a-form-item>
            <a-form-item>
              <a-button type="info" @click="loadClients(1)">Search</a-button>
            </a-form-item>
          </a-form>
        </a-col>
        <a-col :span="6" style="text-align: right">
          <a-button type="primary" @click="newClient()">Create Client</a-button>
        </a-col>
      </a-row>
    </div>
    <a-table
      :columns="columns"
      :data-source="dataList"
      :pagination="false"
      rowKey="id"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.dataIndex === 'enabled'">
          <span>{{ record.enabled ? "Enabled" : "Disabled" }} </span>
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
            :title="`Are your sure to remove this client: ${record.clientName} ?`"
          >
            <a-button type="link">Remove</a-button>
          </a-popconfirm>
        </template>
      </template>
    </a-table>
    <div style="padding: 15px">
      <a-pagination
        :current="so.pageIndex"
        :total="total"
        @change="onPageChange"
      />
    </div>
    <a-drawer
      :title="'Edit Client ' + currentClientName"
      :width="960"
      :visible="drawerVisible"
      @close="onEditClose"
    >
      <client-edit :clientId="currentClientId" @saved="onSaved()" />
    </a-drawer>
  </div>
</template>
<script>
import ClientApi from "@/api/client";
import ClientEdit from "./components/ClientEdit.vue";
const columns = [
  { title: "Id", width: 100, dataIndex: "id", key: "id" },
  {
    title: "Enabled",
    width: 100,
    dataIndex: "enabled",
    key: "enabled",
    scopedSlots: { customRender: "enabled" },
  },
  { title: "ClientId", dataIndex: "clientId", key: "clientId" },
  { title: "ClientName", dataIndex: "clientName", key: "clientName" },
  {
    title: "clientDescription",
    dataIndex: "clientDescription",
    key: "clientDescription",
  },
  {
    title: "Operation",
    dataIndex: "operation",
    scopedSlots: { customRender: "operation" },
    width: 180,
  },
];
export default {
  name: "Client",
  components: { ClientEdit },
  data() {
    return {
      dataList: [],
      so: {
        keyword: "",
        pageIndex: 1,
        pageSize: 10,
        onlyEnabled: false,
      },
      total: 0,
      columns,
      drawerVisible: false,
      currentClientId: "",
      currentClientName: "",
    };
  },
  created() {
    this.loadClients(1);
  },
  methods: {
    loadClients(pn) {
      this.so.pageIndex = pn;
      ClientApi.getClients(this.so).then((resp) => {
        if (resp.code == 0) {
          this.dataList = resp.data.items;
          this.total = resp.data.totalCount;
        }
      });
    },
    onPageChange(e) {
      this.loadClients(e);
    },
    editItem(item) {
      this.currentClientId = item.clientId;
      this.currentClientName = item.clientName;
      this.drawerVisible = true;
    },
    removeItem(item) {
      // console.log(item)
      ClientApi.removeClient({ id: item.id, clientId: item.clientId })
        .then((resp) => {
          if (resp.code === 0) {
            this.loadClients(this.so.pageIndex);
          } else {
            this.$message.error(resp.message);
          }
        })
        .catch((err) => {
          console.log(err);
        });
    },
    onEditClose() {
      this.currentClientId = "";
      this.currentClientName = "";
      this.drawerVisible = false;
    },
    onSaved() {
      this.loadClients(this.so.pageIndex);
      this.currentClientId = "";
      this.currentClientName = "";
      this.drawerVisible = false;
    },
    newClient() {
      this.currentClientId = "";
      this.currentClientName = "";
      this.drawerVisible = true;
    },
  },
};
</script>
