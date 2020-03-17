<template>
  <v-app>
    <!-- <AppBar /> -->
    <Drawer />
    <v-content>
      <router-view />
    </v-content>
  </v-app>
</template>

<script>
//import AppBar from './layouts/AppBar';
import Drawer from './layouts/Drawer'

export default {
  name: 'App',
  components: {
    //AppBar,
    Drawer
  },
  data: () => ({
    //
  }),
};
</script>
