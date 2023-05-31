<style lang="less" scoped>
.operate-icon-green {
  height: 32px;
  color: green;
  display: table-cell;
  vertical-align: middle;
  width: 32px;
}
.operate-icon-red {
  height: 32px;
  color: red;
  display: table-cell;
  vertical-align: middle;
  width: 32px;
}
</style>
<template>
  <div>
    <a-form-item-rest>
      <div v-for="(str, idx) in collection" :key="'pstr-' + idx">
        <a-row>
          <a-col :span="9">
            <a-input
              :key="'claimtype_' + idx"
              placeholder="Claim Type"
              v-model:value="collection[idx].type"
              @change="onChange()"
            />
          </a-col>
          <a-col :span="9">
            <a-input
              :key="'claimvalue' + idx"
              placeholder="Claim Value"
              v-model:value="collection[idx].value"
              @change="onChange()"
            />
          </a-col>
          <a-col :span="6">
            <file-add-outlined class="operate-icon-green" @click="addClaim" />
            <a-popconfirm
              v-if="collection.length > 1"
              placement="top"
              ok-text="Yes"
              cancel-text="No"
              title="Are you sure to delete this item?"
              @confirm="removeClaim(idx)"
            >
              <delete-outlined class="operate-icon-red"></delete-outlined>
            </a-popconfirm>
          </a-col>
        </a-row>
      </div>
    </a-form-item-rest>
  </div>
</template>
<script>
import { FileAddOutlined, DeleteOutlined } from "@ant-design/icons-vue";

export default {
  name: "ClaimsEdit",
  components: {
    FileAddOutlined,
    DeleteOutlined,
  },
  data() {
    return {
      collection: [],
    };
  },
  props: {
    values: {
      type: Array,
      default: () => [],
    },
  },
  created() {
    this.collection = this.values;
    if (!this.values.length) {
      this.collection = [{ type: "", value: "" }];
    }
  },
  watch: {
    claims: function (val) {
      if (!val.length) {
        this.collection = [{ type: "", value: "" }];
      } else {
        this.collection = val;
      }
    },
  },
  emits: ["update:values"],
  methods: {
    addClaim() {
      this.collection.push({ type: "", value: "" });
    },
    removeClaim(index) {
      this.collection.splice(index, 1);
      this.$emit("update:values", this.collection);
    },
    onChange() {
      this.$emit("update:values", this.collection);
    },
  },
};
</script>
