  
<template>
  <v-container
        class="fill-height"
        fluid
      >
        <v-row
          align="center"
          justify="center"
        >
          <v-col
            cols="12"
            sm="8"
            md="4"
          >
            <v-card class="elevation-12">
              <v-toolbar
                color="primary"
                dark
                flat
              >
                <v-toolbar-title>进入后台</v-toolbar-title>
                <v-spacer />
              </v-toolbar>
              <v-card-text>
                <v-form>
                  <v-text-field
                    label="用户名"
                    name="login"
                    prepend-icon="mdi-account"
                    type="text"
                    v-model="UserName"
                  />
                  <v-text-field
                    id="password"
                    label="密码"
                    name="password"
                    prepend-icon="mdi-lock"
                    type="password"
                    v-model="UserPassword"
                  />
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer />
                <v-btn color="primary" v-on:click="Login">登录</v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
</template>

<script>
import axios from 'axios'
import Cookies from 'js-cookie'
  export default {
      data() {
          return {
              UserName:null,
              UserPassword:null
          }
      },
      methods: {
          Login : function(){
              axios.post('https://localhost:5001/api/user/authenticate',{
                  username:this.UserName,
                  password:this.UserPassword
              }).then(response => {
                  //登录成功
                  if(!response.data.error){
                  Cookies.set('token',response.data.token,{
                    expires: 7
                  })
                  this.$store.commit('openSnackBar','登录成功')
                  this.$router.push('admin')
                  } else {
                      this.$store.commit('openSnackBar',response.data.message)
                  }
              })
          }
      },
  }
</script>