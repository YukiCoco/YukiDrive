<template>
<v-navigation-drawer app v-model="isDrawerOpen">
    <v-list-item>
        <v-list-item-content>
            <v-list-item-title class="title">
                Application
            </v-list-item-title>
        </v-list-item-content>
    </v-list-item>
    <v-divider></v-divider>
    <v-list dense nav>
        <v-list-item v-for="site in sites" :key="site.name" link :to="'/' + site.name">
            <v-list-item-icon>
                <v-icon>mdi-microsoft-onedrive</v-icon>
            </v-list-item-icon>
            <v-list-item-content>
                <v-list-item-title>{{ site.name }}</v-list-item-title>
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
            sites: []
        }
    },
    mounted() {
        axios.get("https://localhost:5001/api/site").then(response => {
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
}
</script>

<style>

</style>
