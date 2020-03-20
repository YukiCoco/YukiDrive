<template>
<v-container>
    <v-row>
        <v-col cols="12" sm="12" md="10" offset-md="1" align="start" justify="center">
            <v-card>
                <v-card-title>
                    <v-icon>{{ this.$store.state.show.icon }}</v-icon>{{ this.$store.state.show.name }}
                </v-card-title>
                <v-divider></v-divider>
                <v-card-text>
                    <div id="dplayer"></div>
                    <v-img v-if="this.$store.state.show.icon == 'mdi-image'" :src="this.$store.state.show.url">
                        <template v-slot:placeholder>
                            <v-row class="fill-height ma-0" align="center" justify="center">
                                <v-progress-circular size="75" indeterminate></v-progress-circular>
                            </v-row>
                        </template>
                    </v-img>
                    <v-text-field class="mt-4" hide-details readonly label="下载链接" :value="this.$store.state.show.url"></v-text-field>
                    <v-text-field v-if="this.$store.state.show.icon == 'mdi-image'" class="mt-4" hide-details readonly label="Markdown" :value="`![image](${this.$store.state.show.url})`"></v-text-field>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-btn text>下载</v-btn>
                </v-card-actions>
            </v-card>
        </v-col>
    </v-row>
    <v-btn fab dark color="cyan" class="float-btn" @click="goBack">
        <v-icon>mdi-undo-variant</v-icon>
    </v-btn>
</v-container>
</template>

<script>
import 'dplayer/dist/DPlayer.min.css';
import DPlayer from 'dplayer';
export default {
    data() {
        return {
            fab: false
        }
    },
    mounted() {
        if (this.$store.state.show.icon == 'mdi-video') {
            new DPlayer({
                container: document.getElementById('dplayer'),
                screenshot: true,
                autoplay: true,
                video: {
                    url: this.$store.state.show.url
                }
            });
        }
    },
    methods: {
        goBack:function(){
            this.$router.go(-1)
        }
    },
}
</script>

<style>
.float-btn{
    position: fixed;
    right: 16px;
    bottom: 16px;
}
</style>
