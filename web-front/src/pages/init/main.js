import './style.scss';
import Vue from 'vue'
import '../../plugins/axios'
import vuetify from '../../plugins/vuetify'
import index from './index.vue'

Vue.config.productionTip = false

new Vue({
  vuetify,
  render: h => h(index)
}).$mount('#app')
