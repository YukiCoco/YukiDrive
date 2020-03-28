<template>
<v-navigation-drawer app v-model="isDrawerOpen">
    <v-list-item>
        <v-list-item-content>
            <v-list-item-title class="title">
                {{ this.$store.state.settings.appName }}
            </v-list-item-title>
        </v-list-item-content>
    </v-list-item>
    <v-divider></v-divider>
    <v-list dense nav>
        <v-list-group
            prepend-icon="mdi-microsoft-onedrive"
            value="true"
        >
            <template v-slot:activator>
            <v-list-item-content>
              <v-list-item-title>驱动器</v-list-item-title>
            </v-list-item-content>
          </template>
          <v-list-item v-for="site in sites" :key="site.name" link :to="'/' + site.name">
            <v-list-item-content>
                <v-list-item-title>{{ site.nickName }}</v-list-item-title>
            </v-list-item-content>
        </v-list-item>
        </v-list-group>

        <v-list-item link :to="item.to" v-for="(item, index) in actions" :key="index">
            <v-list-item-icon>
                <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-content>
                <v-list-item-title>{{ item.title }}</v-list-item-title>
            </v-list-item-content>
        </v-list-item>
    </v-list>
</v-navigation-drawer>
</template>

<script>
import axios from 'axios'

export default {
    data() {
        return {
            sites: [],
            actions: []
        }
    },
    mounted() {
        axios.get(this.$store.state.settings.baseUrl + "/api/sites").then(response => {
            this.sites = response.data
        })
    },
    computed: {
        isDrawerOpen : {
            get () {
                return this.$store.state.isDrawerOpen
            },
            set (value){
                this.$store.commit('changeDrawerState',value)
            }
        }
    },
    methods: {
        updateActions: function(payload) {
            this.actions.push(payload)
        }
    },
}
</script>

<style>

</style>
