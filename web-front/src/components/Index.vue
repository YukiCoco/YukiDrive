<template>
<v-container>
    <v-row>
        <v-col cols="12" sm="12" offset-md="2" md="8" align="start" justify="center">
            <v-list two-line>
                <v-subheader>文件夹</v-subheader>
                <v-list-item-group color="primary">
                    <v-list-item v-for="(item, i) in items" :key="i">
                        <v-list-item-avatar>
                            <v-icon large>mdi-{{ item.icon }}</v-icon>
                        </v-list-item-avatar>
                        <v-list-item-content>
                            <v-list-item-title v-text="item.title"></v-list-item-title>
                            <v-list-item-subtitle v-text="item.subtitle"></v-list-item-subtitle>
                        </v-list-item-content>
                        <v-list-item-action>
                            <v-btn icon>
                                <v-icon>mdi-download</v-icon>
                            </v-btn>
                        </v-list-item-action>
                    </v-list-item>
                </v-list-item-group>
            </v-list>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
import axios from 'axios'
export default {
    data() {
        return {
            items: [{
                    icon: 'folder',
                    iconClass: 'grey lighten-1 white--text',
                    title: 'Photos',
                    subtitle: 'Jan 9, 2014'
                },
                {
                    icon: 'folder',
                    iconClass: 'grey lighten-1 white--text',
                    title: 'Recipes',
                    subtitle: 'Jan 17, 2014'
                },
                {
                    icon: 'folder',
                    iconClass: 'grey lighten-1 white--text',
                    title: 'Work',
                    subtitle: 'Jan 28, 2014'
                },
            ]
        }
    },
    mounted() {
        axios.get("https://localhost:5001/api/site").then(response => {
            console.log(response.data);
        })
    },
    methods: {
        
    },
}
</script>

<style>

</style>
