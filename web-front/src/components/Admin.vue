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
                    <v-btn text href="https://localhost:5001/api/admin/bind/url">认证</v-btn>
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
                                    <v-btn text @click="openEditDriveDialog(item.nickName)">修改名称</v-btn>
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
                                <v-text-field label="导航栏背景图片" hint="左侧导航栏背景图片，留空则不显示" v-model="settings.navImg"></v-text-field>
                                <v-text-field label="设置主驱动器" hint="填写创建 SharePoint 站点时输入的名称，将被设置为打开网站默认显示的驱动器" v-model="settings.defaultDrive">
                                </v-text-field>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-form>
                <v-card-actions>
                    <v-btn @click="updateSettings" text>提交</v-btn>
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
                修改驱动器名称
            </v-card-title>
            <v-card-text>
                <v-form>
                    <v-text-field label="新名称" v-model="newDriveName">
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
export default {
    data() {
        return {
            drives: [],
            settings: {
                officeName: undefined,
                officeType: undefined,
                appName: undefined,
                webName: undefined,
                navImg: undefined,
                defaultDrive: undefined,
                accountStatus: undefined
            },
            newBind: {
                siteName: undefined,
                nickName: undefined
            },
            dialog: {
                newBindDialog: false,
                editDriveDialog: false
            },
            toEditDriveName: undefined,
            newDriveName: undefined
        }
    },
    mounted() {
        helper.getWithToken("https://localhost:5001/api/admin/info", null, response => {
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
                navImg: response.data.navImg,
                defaultDrive: response.data.defaultDrive,
                accountStatus: response.data.accountStatus
            }
        })
    },
    methods: {
        updateSettings: function () {
            helper.postWithToken("https://localhost:5001/api/admin/setting", this.settings, response => {
                if (!response.data.error) {
                    this.$store.commit('openSnackBar', '更新成功！')
                    //刷新此页
                    this.$router.go(0)
                }
            })
        },
        addSite: function () {
            helper.postWithToken("https://localhost:5001/api/admin/site", this.newBind, response => {
                if (!response.data.error) {
                    this.$store.commit('openSnackBar', '绑定成功！')
                    this.dialog.newBindDialog = false
                    //刷新此页
                    this.$router.go(0)
                } else {
                    this.$store.commit('openSnackBar', '绑定失败：' + response.data.message)
                }
            })
        },
        unbind: function (nickName) {
            helper.deleteWithToken("https://localhost:5001/api/admin/site", {
                nickName: nickName
            }, response => {
                if (!response.data.error) {
                    this.$store.commit('openSnackBar', '已解除绑定')
                    this.dialog.newBindDialog = false
                    //刷新此页
                    this.$router.go(0)
                } else {
                    this.$store.commit('openSnackBar', '操作失败：' + response.data.message)
                }
            })
        },
        openEditDriveDialog: function (nickName) {
            this.toEditDriveName = nickName
            this.dialog.editDriveDialog = true
        },
        editDriveName: function () {
            helper.postWithToken("https://localhost:5001/api/admin/site/rename", {
                oldName: this.toEditDriveName,
                nickName: this.newDriveName
            }, response => {
                if (!response.data.error) {
                    this.$store.commit('openSnackBar', '已更新驱动器名称')
                    this.dialog.newBindDialog = false
                    //刷新此页
                    this.$router.go(0)
                } else {
                    this.$store.commit('openSnackBar', '操作失败：' + response.data.message)
                }
            })
        }
    },
}
</script>

<style>

</style>
