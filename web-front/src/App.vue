<template>
<v-app>
    <AppBar />
    <Drawer />
    <v-content>
        <router-view />
        <Snackbar />
    </v-content>
    <Footer ref="footer"/>
</v-app>
</template>

<script>
import AppBar from './layouts/AppBar'
import Drawer from './layouts/Drawer'
import Snackbar from './layouts/Snackbar'
import axios from 'axios'
import Footer from './layouts/Footer'

export default {
    name: 'App',
    components: {
        AppBar,
        Drawer,
        Snackbar,
        Footer
    },
    data: () => ({
        //
    }),
    mounted() {
        axios.get('/api/info').then(response => {
            this.$store.commit('changeSettings', {
                appName: response.data.appName,
                webName: response.data.webName,
                navImg: response.data.navImg,
                defaultDrive: response.data.defaultDrive,
                footer: response.data.footer
            })
            //更新网页 Title
            document.title = response.data.webName
            //没有指定驱动器 跳转到默认驱动器
            if (this.$route.path == '/') {
                this.$router.push(this.$store.state.settings.defaultDrive)
            }
            //更新 Footer
            this.$refs.footer.updateFooter()
        })
    },
};
</script>
