<template>
  <div>
    <h2>xxxx</h2>
    <div>
      <table class="balls">
        <tr v-for="(item, idx) in lottoData.data" :key="'xxxx_' + idx">
          <th>{{ idx }}</th>
          <td
            v-for="k in redBalls"
            :class="getTdClass(item, k)"
            :key="'redblock-' + k"
          ></td>
        </tr>
      </table>
    </div>
  </div>
</template>
<script>
import LottoData from "../data/lotto.json";

export default {
  name: "Test",
  data() {
    return {
      lottoData: LottoData,
      redBalls: [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
        21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33,
      ],
      blueBalls: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15],
    };
  },
  methods: {
    getTdClass(row, idx) {
      for (let i = 0; i < 6; i++) {
        if (row[i] === idx) {
          return "red";
        }
      }
      return "empty";
    },
  },
};
</script>
<style lang="less">
.balls {
  border-top: 1px solid #efefef;
  border-left: 1px solid #efefef;
  td {
    width: 24px;
    height: 24px;
    padding: 0;
    margin: 0;
    border-bottom: 1px solid #efefef;
    border-right: 1px solid #efefef;
    &.empty {
      background-color: white;
    }

    &.red {
      background-color: red;
    }

    &.blue {
      background-color: blue;
    }
  }
}
</style>
