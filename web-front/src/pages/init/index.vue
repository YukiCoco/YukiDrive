<template>
  <v-container class="app-container" grid-list-xs>
    <div id="setting-page" class="col-md-10 offset-md-1 col-lg-6 offset-lg-3 col-xs-12 offset-sm-1 col-sm-10">
      <!-- grid 布局开始 -->
      <div>
        <div class="setting-title">
          <div>
            <h1>初始化</h1>
            <span></span>
          </div>
        </div>
        <settings-text-area
          title="Client Id"
          v-model="clientId"
        ></settings-text-area>
        <settings-text-area
          title="Client Secret"
          v-model="clientSecret"
        ></settings-text-area>
        <settings-text-area
          title="站点 URL"
          v-model="siteUrl"
        ></settings-text-area>
        <settings-text-area
          title="SharePoint 账户类型"
          v-model="spUserType"
        ></settings-text-area>
        <settings-text-area
          title="SharePoint 账户邮箱"
          v-model="spUserEmail"
        ></settings-text-area>
        <settings-text-area
          title="管理员账号"
          v-model="adminId"
        ></settings-text-area>
        <settings-text-area
          title="管理员密码"
          v-model="adminPwd"
        ></settings-text-area>
      </div>
    </div>
  </v-container>
</template>

<script>
import SettingsTextArea from "./components/SettingsTextArea.vue";

export default {
  data: function () {
    return {
      clientId: undefined,
      clientSecret: undefined,
      siteUrl: undefined,
      spUserType: undefined,
      spUserEmail: undefined,
      adminId: undefined,
      adminPwd: undefined,
    };
  },
  components: {
    "settings-text-area": SettingsTextArea,
  },
};
</script>