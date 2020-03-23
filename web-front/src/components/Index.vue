<template>
<v-container>
    <v-row>
        <v-col cols="12" sm="12" offset-md="2" md="8" align="start" justify="center">
            <v-card>
                <v-progress-linear indeterminate v-if="isInProgressing"></v-progress-linear>
                <div v-if="router.length != 0">
                    <v-breadcrumbs :items="router" class="pl-4 pb-0 pt-4"></v-breadcrumbs>
                    <v-divider class="ml-4 mr-4 mt-1"></v-divider>
                </div>
                <v-list two-line>
                    <div v-if="folders.length != 0">
                        <v-subheader>文件夹</v-subheader>
                        <v-list-item-group color="primary">
                            <v-list-item v-for="(item, i) in folders" :key="i" :to="item.name" append>
                                <v-list-item-avatar>
                                    <v-icon large>mdi-folder</v-icon>
                                </v-list-item-avatar>
                                <v-list-item-content>
                                    <v-list-item-title v-text="item.name"></v-list-item-title>
                                    <v-list-item-subtitle v-text="`${item.createdTime}\xa0\xa0${item.size}`">
                                    </v-list-item-subtitle>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list-item-group>
                    </div>
                    <div v-if="files.length != 0">
                        <v-subheader>文件</v-subheader>
                        <v-list-item-group color="primary">
                            <v-list-item v-for="(item, i) in files" :key="i">
                                <v-list-item-avatar>
                                    <v-icon large @click="openDetial(item)">{{ item.icon }}</v-icon>
                                </v-list-item-avatar>
                                <v-list-item-content @click="openDetial(item)">
                                    <v-list-item-title v-text="item.name"></v-list-item-title>
                                    <v-list-item-subtitle v-text="`${item.createdTime}\xa0\xa0${item.size}`"></v-list-item-subtitle>
                                </v-list-item-content>
                                <v-list-item-action>
                                    <v-btn icon class="copy-url" :data-clipboard-text="item.downloadUrl">
                                        <v-icon>mdi-content-copy</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                                <v-list-item-action class="ml-0">
                                    <v-btn icon :href="item.downloadUrl" @click="downloadFile(item.downloadUrl)">
                                        <v-icon>mdi-download</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                            </v-list-item>
                        </v-list-item-group>
                    </div>
                </v-list>
            </v-card>
            <v-card v-if="!this.$route.params.folderPath" class="mt-4">
                <div class="pa-4 readme" id="readme">
                </div>
            </v-card>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
import axios from 'axios'
import {
    bytesToSize,
    markdownIt,
    get
} from '../helpers/helper'
import ClipboardJS from 'clipboard'
export default {
    data() {
        return {
            files: [],
            folders: [],
            router: [],
            isInProgressing: false
        }
    },
    mounted() {
        //初始化复制组件
        new ClipboardJS('.copy-url')
        //存在子路径
        if (this.$route.params.folderPath) {
            this.changeRouter()
            this.show(this.$route.params.folderPath)
            //根目录
        } else if (this.$route.params.siteName) {
            this.show()
            this.loadReadme()
        }
    },
    watch: {
        '$route'() {
            this.show(this.$route.params.folderPath)
            if (this.$route.params.folderPath) {
                this.changeRouter()
            } else {
                //清除顶部导航
                this.router.splice(0)
                this.loadReadme()
            }
        }
    },
    methods: {
        loadReadme: function () {
            axios.get(`${this.$store.state.settings.baseUrl}/api/readme`).then(response => {
                document.getElementById('readme').innerHTML = markdownIt(response.data.readme)
            })
        },
        changeProgressBar: function () {
            this.isInProgressing = !this.isInProgressing
        },
        show: function (path = "") {
            this.changeProgressBar()
            this.folders.splice(0)
            this.files.splice(0)
            // currentSiteName 显示错误 需要使用 this.$route.params.siteName
            let showCallback = response => {
                response.data.forEach(element => {
                    element.createdTime = (new Date(element.createdTime)).toLocaleString('zh-CN', {
                        year: 'numeric',
                        month: 'numeric',
                        day: 'numeric'
                    })
                    element.size = bytesToSize(element.size)
                    //文件夹
                    if (element.downloadUrl == null) {
                        this.folders.push(element)
                    } else {
                        //文件
                        element.icon = getIcon(element.name)
                        //获取原始下载链接
                        element.url = element.downloadUrl
                        if (this.$route.params.folderPath) {
                            element.downloadUrl = `${this.$store.state.settings.baseUrl}/api/files/${this.$route.params.siteName}/${this.$route.params.folderPath}/${element.name}`
                        } else {
                            element.downloadUrl = `${this.$store.state.settings.baseUrl}/api/files/${this.$route.params.siteName}/${element.name}`
                        }
                        this.files.push(element)
                    }
                });
                this.changeProgressBar()
            }
            get(`${this.$store.state.settings.baseUrl}/api/sites/${this.$route.params.siteName}/${path}`,null,showCallback)
        },
        changeRouter: function () {
            this.router.splice(0)
            let folders = this.$route.params.folderPath.split('/')
            this.router.push({
                text: '根目录',
                href: `#/${this.$route.params.siteName}`,
                disabled: false
            })
            folders.forEach(element => {
                //正则获取路径字符串
                let regex = new RegExp(`${element}.*`)
                let path = this.$route.params.folderPath.replace(regex, element)
                this.router.push({
                    text: element,
                    href: `#/${this.$route.params.siteName}/${path}`,
                    disabled: false
                })
            });
            this.router[this.router.length - 1].disabled = true
        },
        openDetial: function (payload) {
            this.$store.commit('showItem', {
                name: payload.name,
                url: payload.url,
                icon: payload.icon,
                downloadUrl : payload.downloadUrl
            })
            //调用微软接口预览 Office 文件
            if (getIcon(payload.name).match('microsoft') != null) {
                window.open('https://view.officeapps.live.com/op/view.aspx?src=' + payload.downloadUrl)
            }
            this.$router.push('/show')
        }
    },
}

//返回Icon
function getIcon(filename) {
    let index = filename.lastIndexOf(".")
    let suffix = filename.substr(index + 1).toLowerCase()
    let imageArray = ['png', 'jpg', 'jpeg', 'bmp', 'gif', 'webp', 'psd', 'svg', 'tiff']
    let videoArray = ['wmv', 'asf', 'asx', 'rm', 'rmvb', 'mpg', 'mpeg', 'mpe', '3gp', 'mov', 'mp4', 'm4v', 'avi', 'dat', 'mkv', 'flv', 'vob']
    let audioArray = ['mp3', 'wav', 'wma', 'ape', 'flac', 'aac']
    let zipArray = ['zip', 'rar', '7z', 'gz', 'bz2', 'xz', 'tar', 'tar.gz', 'tar.bz2', 'tar.xz']
    let wordArray = ['doc', 'docx']
    if (imageArray.indexOf(suffix) !== -1) {
        return 'mdi-image'
    }
    if (videoArray.indexOf(suffix) !== -1) {
        return 'mdi-movie'
    }
    if (audioArray.indexOf(suffix) !== -1) {
        return 'mdi-music'
    }
    if (zipArray.indexOf(suffix) !== -1) {
        return 'mdi-folder-zip'
    }
    if (wordArray.indexOf(suffix) !== -1) {
        return 'mdi-microsoft-word'
    }
    if (suffix == 'xlsx') {
        return 'mdi-microsoft-excel'
    }
    if (suffix == 'ppt') {
        return 'mdi-microsoft-powerpoint'
    }
    if (suffix == 'pdf') {
        return 'mdi-file-pdf-box'
    }
    if (suffix == 'md') {
        return 'mdi-text-box-outline'
    }
    return 'mdi-file'
}
</script>

<style>
.readme img {
    width: 100%;
}
</style>
