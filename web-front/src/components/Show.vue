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
                    <v-text-field class="mt-4" hide-details readonly label="下载链接" v-model="this.$store.state.show.url"></v-text-field>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-btn text>下载</v-btn>
                </v-card-actions>
            </v-card>
        </v-col>
    </v-row>
</v-container>
</template>

<script>
import 'dplayer/dist/DPlayer.min.css';
import DPlayer from 'dplayer';
export default {
    data() {
        return {}
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
}
</script>

<style>

</style>
