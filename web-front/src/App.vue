<template>
  <v-app>
    <AppBar />
    <Drawer />
    <v-content>
      <router-view />
      <Snackbar />
    </v-content>
  </v-app>
</template>

<script>
import AppBar from './layouts/AppBar'
import Drawer from './layouts/Drawer'
import Snackbar from './layouts/Snackbar'
import axios from 'axios'

export default {
  name: 'App',
  components: {
    AppBar,
    Drawer,
    Snackbar
  },
  data: () => ({
    //
  }),
  mounted() {
    axios.get('https://localhost:5001/api/info').then(response => {
      this.$store.commit('changeSettings',{
        appName : response.data.appName,
        webName : response.data.webName,
        navImg : response.data.navImg
      })
      document.title = response.data.webName;
    })
  },
};
</script>
