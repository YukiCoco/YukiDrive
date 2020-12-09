<template>
<v-footer class="font-weight-medium">
    <v-col class="text-center" cols="12">
        <div class="footer" id="footer"></div>
    </v-col>
</v-footer>
</template>

<script>
import { markdownIt } from '../helpers/helper'
export default {
    data() {
        return {
            footer : this.$store.state.settings.footer
        }
    },
    watch: {
        footer : function(){
            this.updateFooter()
        }
    },
    methods: {
        updateFooter: function(){
            document.getElementById('footer').innerHTML = markdownIt(this.$store.state.settings.footer)
        }
    },
}
</script>

<style>
.footer p {
    margin-bottom: 0px !important;
}
</style>
