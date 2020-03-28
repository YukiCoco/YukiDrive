<template>
<v-app>
    <AppBar />
    <Drawer ref="drawer" />
    <v-content>
        <router-view />
        <Snackbar />
    </v-content>
    <Footer ref="footer" />
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
        axios.get(this.$store.state.settings.baseUrl + '/api/info').then(response => {
            this.$store.commit('changeSettings', {
                appName: response.data.appName,
                webName: response.data.webName,
                navImg: response.data.navImg,
                defaultDrive: response.data.defaultDrive,
                footer: response.data.footer
            })
            //更新网页 Title
            document.title = response.data.webName
            //更新 Footer
            this.$refs.footer.updateFooter()
            //没有指定驱动器 跳转到默认驱动器
            if (this.$route.path == '/') {
                this.$router.push(response.data.defaultDrive)
            }
            //更新 Footer
            this.$refs.footer.updateFooter()
            //更新 Drawer
            //添加上传文件页面
            console.log(response.data.allowUpload)
            //接口的 boolean 这么奇怪？
            if (response.data.allowUpload || (this.$store.state.token != '')) {
                this.$refs.drawer.updateActions({
                    to: 'upload',
                    icon: 'mdi-cloud-upload-outline',
                    title: '上传文件'
                })
            }
        })
    },
};
</script>
