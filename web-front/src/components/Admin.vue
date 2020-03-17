<template>
<v-container>
    <v-row>
        <v-col cols="12" sm="12" offset-md="2" md="8" align="start" justify="center">
            <v-card>
                <v-toolbar dense flat>
                    <v-toolbar-title>SharePoint 账户</v-toolbar-title>
                </v-toolbar>
                <v-card-text>
                    <p>Office 区域：China</p>
                    <p>当前账户：YukinoCoco@outlook.com</p>
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
                        <v-col cols="12" md="6" lg="4">
                            <v-card outlined>
                                <v-card-title class="d-flex justify-center">
                                    DriveName
                                </v-card-title>
                                <v-card-text class="d-flex justify-center">
                                    <v-progress-circular size="150" width="20" color="primary"></v-progress-circular>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn text>修改昵称</v-btn>
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
                                <v-text-field label="网站名称">
                                </v-text-field>
                                <v-text-field label="导航栏显示名"
                                value="Yuki Drive">
                                </v-text-field>
                                <v-text-field
                                    label="导航栏背景图片"
                                    hint="左侧导航栏背景图片，留空则不显示"
                                ></v-text-field>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-form>
                <v-card-actions>
                    <v-btn text>提交</v-btn>
                </v-card-actions>
            </v-card>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
//import Cookies from 'js-cookie'
//import { bytesToSize } from '../helpers/helper.js'
export default {

}
</script>

<style>

</style>
