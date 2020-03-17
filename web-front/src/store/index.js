import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    custom: {
      appName: 'YukiDrive',
    },
    isDrawerOpen : null,
    snackbar:{
      message: null,
      isOpen: false
    }
  },
  mutations: {
    changeDrawerState(state,payload){
      state.isDrawerOpen = payload ? payload : !state.isDrawerOpen
    },
    openSnackBar(state,payload){
      state.snackbar.message = payload
      state.snackbar.isOpen = true
    },
    changeSnackBarStatus(state,payload){
      state.snackbar.isOpen = payload
    }
  },
  actions: {
  },
  modules: {
  }
})
