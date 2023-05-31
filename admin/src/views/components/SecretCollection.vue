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
      <div v-for="(str, idx) in collection" :key="'sstr-' + idx">
        <a-row>
          <a-col :span="18">
            <a-input-password
              :id="'somevalue_' + idx"
              autocomplete="new-password"
              v-model:value="collection[idx]"
              @change="onChange()"
            />
          </a-col>
          <a-col :span="6">
            <file-add-outlined class="operate-icon-green" @click="addOne" />
            <a-popconfirm
              v-if="collection.length > 1"
              placement="top"
              ok-text="Yes"
              cancel-text="No"
              title="Are you sure to delete this item?"
              @confirm="remove(idx)"
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
  name: "SecretCollection",
  components: {
    FileAddOutlined,
    DeleteOutlined,
  },
  props: {
    values: {
      type: Array,
      default() {
        return [];
      },
    },
  },
  data() {
    return {
      collection: [],
    };
  },
  created() {
    this.collection = this.values;
    if (!this.collection.length) {
      this.collection = [""];
    }
  },
  watch: {
    values: function (val) {
      this.collection = val;
      if (!this.collection.length) {
        this.collection = [""];
      }
    },
  },
  emits: ["update:values"],
  methods: {
    onChange() {
      this.$emit("update:values", this.collection);
    },
    addOne() {
      this.collection.push("");
      this.$emit("update:values", this.collection);
    },
    remove(idx) {
      this.collection.splice(idx, 1);
      this.$emit("update:values", this.collection);
    },
  },
};
</script>
