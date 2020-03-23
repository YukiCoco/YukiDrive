<template>
<v-container>
    <v-row>
        <v-col cols="12" sm="12" offset-md="2" md="8" align="start" justify="center">
            <v-card>
                <v-toolbar dense flat>
                    <v-toolbar-title>SharePoint 账户</v-toolbar-title>
                </v-toolbar>
                <v-card-text>
                    <p>账户类型：{{ settings.officeType }}</p>
                    <p>当前账户：{{ settings.officeName }}</p>
                    <p>当前状态：{{ settings.accountStatus }}</p>
                </v-card-text>
                <v-card-actions>
                    <v-btn text :href="this.$store.state.settings.baseUrl + '/api/admin/bind/url'">认证</v-btn>
                </v-card-actions>
            </v-card>
            <v-card class="mt-4">
                <v-toolbar dense flat>
                    <v-toolbar-title>SharePoint & Onedrive 站点</v-toolbar-title>
                </v-toolbar>
                <v-container>
                    <v-row>
                        <v-col cols="12" md="6" lg="4" v-for="(item, index) in drives" :key="index">
                            <v-card outlined>
                                <v-card-title class="d-flex justify-center">
                                    {{ item.nickName }}
                                </v-card-title>
                                <v-card-subtitle class="d-flex justify-center">{{ item.name }}</v-card-subtitle>
                                <v-card-text class="d-flex justify-center">
                                    <v-progress-circular size="200" width="20" color="primary" :value="item.percent">
                                        {{item.showUsed}} / {{item.showTotal}}
                                    </v-progress-circular>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn text @click="openEditDriveDialog(item)">设置</v-btn>
                                    <v-btn text @click="unbind(item.nickName)">解绑</v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-col>
                    </v-row>
                </v-container>
                <v-card-actions>
                    <v-btn @click="dialog.newBindDialog = true" text>添加站点</v-btn>
                </v-card-actions>
            </v-card>
            <v-card class="mt-4">
                <v-toolbar dense flat>
                    <v-toolbar-title>基本设置</v-toolbar-title>
                </v-toolbar>
                <v-form>
                    <v-container>
                        <v-row>
                            <v-col cols="12" xs="12">
                                <v-text-field label="网站名称" v-model="settings.webName">
                                </v-text-field>
                                <v-text-field label="导航栏显示名" hint="左侧导航栏头部显示的文字" v-model="settings.appName">
                                </v-text-field>
                                <v-text-field label="设置主驱动器" hint="填写创建 SharePoint 站点时输入的名称，将被设置为打开网站默认显示的驱动器（输入 onedrive 则为 OneDrive ）" v-model="settings.defaultDrive">
                                </v-text-field>
                                <v-text-field label="页脚" hint="设置网站页脚，支持 Markdown，建议保留 Power by YukiDrive" v-model="settings.footer">
                                </v-text-field>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-form>
                <v-card-actions>
                    <v-btn @click="updateSettings" text>提交</v-btn>
                </v-card-actions>
            </v-card>
            <v-card class="mt-4">
                <v-toolbar dense flat>
                    <v-toolbar-title>首页 README</v-toolbar-title>
                </v-toolbar>
                <div class="markdown pr-3 pl-3">
                    <Markdown v-model="markdownText" />
                </div>
                <v-card-actions>
                    <v-btn @click="updateReadme" text>提交</v-btn>
                </v-card-actions>
            </v-card>
        </v-col>
    </v-row>
    <v-dialog v-model="dialog.newBindDialog" width="500">
        <v-card>
            <v-card-title class="headline" primary-title>
                新增绑定
            </v-card-title>
            <v-card-text>
                <v-form>
                    <v-text-field label="站点名称" hint="创建 SharePoint 站点时输入的名称" v-model="newBind.siteName">
                    </v-text-field>
                    <v-text-field label="显示名" hint="这将用于在左边导航中显示" v-model="newBind.nickName">
                    </v-text-field>
                </v-form>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="secondary" text @click="dialog.newBindDialog = false">
                    取消
                </v-btn>
                <v-btn color="primary" text @click="addSite">
                    提交
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
    <v-dialog v-model="dialog.editDriveDialog" width="500">
        <v-card>
            <v-card-title class="headline" primary-title>
                修改驱动器设置
            </v-card-title>
            <v-card-text>
                <v-form>
                    <v-text-field label="名称" v-model="editDriveSettings.nickName">
                    </v-text-field>
                    <v-text-field label="隐藏文件夹" hint="填写需要隐藏的文件夹，文件夹名称间用 , 隔开，仅管理员可查看被隐藏的文件夹" v-model="editDriveSettings.hiddenFolders">
                    </v-text-field>
                </v-form>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="secondary" text @click="dialog.editDriveDialog = false">
                    取消
                </v-btn>
                <v-btn color="primary" text @click="editDriveName">
                    提交
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</v-container>
</template>

<script>
import * as helper from '../helpers/helper.js'
import Markdown from 'vue-meditor'
import Cookies from 'js-cookie'

export default {
    components: {
        Markdown,
    },
    data() {
        return {
            drives: [],
            settings: {
                officeName: undefined,
                officeType: undefined,
                appName: undefined,
                webName: undefined,
                defaultDrive: undefined,
                accountStatus: undefined,
                footer: undefined
            },
            newBind: {
                siteName: undefined,
                nickName: undefined
            },
            dialog: {
                newBindDialog: false,
                editDriveDialog: false
            },
            editDriveSettings: {
                siteName: '',
                nickName : '',
                hiddenFolders : ''
            },
            markdownText : ''
        }
    },
    mounted() {
        //判断是否登录
        if(Cookies.get("token") == null) {
            this.$router.push('login')
        }
        helper.get(this.$store.state.settings.baseUrl + "/api/admin/info", null, response => {
            //计算驱动器容量
            response.data.driveInfo.forEach(element => {
                element.showTotal = helper.bytesToSize(element.quota.total)
                element.showUsed = helper.bytesToSize(element.quota.used)
                element.percent = (element.quota.used / element.quota.total) * 100
                this.drives.push(element)
            })
            this.settings = {
                officeName: response.data.officeName,
                officeType: response.data.officeType,
                appName: response.data.appName,
                webName: response.data.webName,
                defaultDrive: response.data.defaultDrive,
                accountStatus: response.data.accountStatus,
                footer : response.data.footer
            }
            this.markdownText = response.data.readme
        })
    },
    methods: {
        updateSettings: function () {
            helper.postWithToken(this.$store.state.settings.baseUrl +  "/api/admin/settings", this.settings, () => {
                this.$store.commit('openSnackBar', '更新成功！')
                //刷新此页
                this.$router.go(0)
            })
        },
        addSite: function () {
            helper.postWithToken(this.$store.state.settings.baseUrl + "/api/admin/sites", this.newBind, () => {
                this.$store.commit('openSnackBar', '绑定成功！')
                this.dialog.newBindDialog = false
                //刷新此页
                this.$router.go(0)
            })
        },
        unbind: function (nickName) {
            helper.deleteWithToken(this.$store.state.settings.baseUrl + "/api/admin/sites", {
                nickName: nickName
            }, () => {
                this.$store.commit('openSnackBar', '已解除绑定')
                this.dialog.newBindDialog = false
                //刷新此页
                this.$router.go(0)
            })
        },
        openEditDriveDialog: function (driveInfo) {
            this.editDriveSettings.nickName = driveInfo.nickName
            this.editDriveSettings.hiddenFolders = driveInfo.hiddenFolders.join()
            this.editDriveSettings.siteName = driveInfo.name
            this.dialog.editDriveDialog = true
        },
        editDriveName: function () {
            helper.postWithToken(this.$store.state.settings.baseUrl + "/api/admin/sites/settings",this.editDriveSettings, () => {
                this.$store.commit('openSnackBar', '已更新驱动器设置')
                this.dialog.newBindDialog = false
                //刷新此页
                this.$router.go(0)
            })
        },
        updateReadme: function () {
            helper.postWithToken(this.$store.state.settings.baseUrl + '/api/admin/readme', {
                text: this.markdownText
            }, () => {
                this.$store.commit('openSnackBar', '已更新 readme')
                this.dialog.newBindDialog = false
                //刷新此页
                this.$router.go(0)
            })
        }
    },
}
</script>

<style>

</style>
