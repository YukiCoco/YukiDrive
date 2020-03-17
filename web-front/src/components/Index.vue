<template>
<v-container>
    <v-row>
        <v-col cols="12" sm="12" offset-md="2" md="8" align="start" justify="center">
            <v-card>
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
                                    <v-list-item-subtitle v-text="item.createdTime"></v-list-item-subtitle>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list-item-group>
                    </div>
                    <div v-if="files.length != 0">
                        <v-subheader>文件</v-subheader>
                        <v-list-item-group color="primary">
                            <v-list-item v-for="(item, i) in files" :key="i">
                                <v-list-item-avatar>
                                    <v-icon large>{{ item.icon }}</v-icon>
                                </v-list-item-avatar>
                                <v-list-item-content>
                                    <v-list-item-title v-text="item.name"></v-list-item-title>
                                    <v-list-item-subtitle v-text="item.createdTime"></v-list-item-subtitle>
                                </v-list-item-content>
                                <v-list-item-action>
                                    <v-btn icon>
                                        <v-icon>mdi-download</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                            </v-list-item>
                        </v-list-item-group>
                    </div>
                </v-list>
            </v-card>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
import axios from 'axios'
export default {
    data() {
        return {
            files: [],
            folders: [],
            router: [],
            currentSiteName: this.$route.params.siteName
        }
    },
    mounted() {
        if(this.$route.params.folderPath){
            this.changeRouter()
            this.show(this.$route.params.folderPath)
        } else{
            this.show()
        }
    },
    watch: {
        '$route'() {
            this.show(this.$route.params.folderPath)
            if(this.$route.params.folderPath){
                this.changeRouter()
            } else {
                this.router.splice(0)
            }
        }
    },
    methods: {
        show: function (path = "") {
            this.folders.splice(0)
            this.files.splice(0)
            console.log(`https://localhost:5001/api/show/${this.currentSiteName}/${path}`)
            axios.get(`https://localhost:5001/api/show/${this.currentSiteName}/${path}`).then(response => {
                response.data.forEach(element => {
                    element.createdTime = (new Date(element.createdTime)).toLocaleString()
                    if (element.downloadUrl == null) {
                        this.folders.push(element)
                    } else {
                        element.icon = getIcon(element.name)
                        this.files.push(element)
                    }
                });
            })
        },
        changeRouter: function () {
            this.router.splice(0)
            let folders = this.$route.params.folderPath.split('/')
            folders.forEach(element => {
                //正则获取路径字符串
                let regex = new RegExp(`${element}.*`)
                let path = this.$route.params.folderPath.replace(regex,element)
                console.log(`/${this.$route.params.siteName}/${path}`)
                this.router.push({
                    text: element,
                    href: `#/${this.$route.params.siteName}/${path}`,
                    disabled: false
                })
            });
            this.router[this.router.length - 1].disabled = true
        }
    },
}

function getIcon(filename) {
    let index = filename.lastIndexOf(".")
    let suffix = filename.substr(index + 1)
    let imageArray = ['png', 'jpg', 'jpeg', 'bmp', 'gif', 'webp', 'psd', 'svg', 'tiff']
    let videoArray = ['wmv', 'asf', 'asx', 'rm', 'rmvb', 'mpg', 'mpeg', 'mpe', '3gp', 'mov', 'mp4', 'm4v', 'avi', 'dat', 'mkv', 'flv', 'vob']
    let audioArray = ['mp3', 'wav', 'wma', 'ape', 'flac', 'aac']
    if (imageArray.indexOf(suffix.toLowerCase()) !== -1) {
        return 'mdi-image'
    }
    if (videoArray.indexOf(suffix.toLowerCase()) !== -1) {
        return 'mdi-video'
    }
    if (audioArray.indexOf(suffix.toLowerCase()) !== -1) {
        return 'mdi-music'
    }
    return 'mdi-file'
}
</script>

<style>

</style>
