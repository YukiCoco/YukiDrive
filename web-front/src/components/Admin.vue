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
                </v-card-text>
                <v-card-actions>
                    <v-btn text href="https://localhost:5001/api/admin/bind/url">登录</v-btn>
                    <v-btn text>取消登录</v-btn>
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
                                    {{ item.name }}
                                </v-card-title>
                                <v-card-text class="d-flex justify-center">
                                    <v-progress-circular size="200" width="20" color="primary" :value="item.percent">
                                        {{item.showUsed}} / {{item.showTotal}}
                                    </v-progress-circular>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn text>修改名称</v-btn>
                                    <v-btn text>解绑</v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-col>
                    </v-row>
                </v-container>
                <v-card-actions>
                    <v-btn text>添加站点</v-btn>
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
                navImg: undefined
            }
        }
    },
    mounted() {
        helper.getWithToken("https://localhost:5001/api/admin/info", null, response => {
            response.data.driveInfo.forEach(element => {
                element.showTotal = helper.bytesToSize(element.quota.total)
                element.showUsed = helper.bytesToSize(element.quota.used)
                element.percent = (element.quota.used / element.quota.total) * 100
                element.name = element.nickName
                this.drives.push(element)
            });
            this.settings = {
                officeName: response.data.officeName,
                officeType: response.data.officeType,
                appName: response.data.appName,
                webName: response.data.webName,
                navImg: response.data.navImg
            }
        })
    },
    methods: {
        updateSettings: function(){
            helper.postWithToken("https://localhost:5001/api/admin/setting",this.settings,response => {
                if(!response.error){
                    this.$store.commit('openSnackBar','更新成功！')
                }
            })
        }
    },
}
</script>

<style>

</style>
