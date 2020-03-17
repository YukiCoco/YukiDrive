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
};
</script>
