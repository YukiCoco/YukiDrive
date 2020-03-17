import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    custom: {
      appName: 'YukiDrive',
    },
    isDrawerOpen : null
  },
  mutations: {
    changeDrawerState(state,payload){
      state.isDrawerOpen = payload ? payload : !state.isDrawerOpen
    }
  },
  actions: {
  },
  modules: {
  }
})
