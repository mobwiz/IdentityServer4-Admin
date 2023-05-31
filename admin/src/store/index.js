import { createStore } from "vuex";

export default createStore({
  state() {
    return {
      user: null,
    };
  },
  mutations: {
    setUserinfo(state, payload) {
      state.user = payload;
    },
  },
  actions: {
    setUserinfo({ commit }, payload) {
      commit("setUserinfo", payload);
    },
  },
  modules: {},
});
