<template>
<v-footer class="font-weight-medium mt-2">
    <v-col class="text-center" cols="12">
        <div class="footer" id="footer"></div>
    </v-col>
</v-footer>
</template>

<script>
import {
    markdown
} from 'markdown'
export default {
    data() {
        return {
            footer : this.$store.state.settings.footer
        }
    },
    methods: {
        updateFooter: function(){
            document.getElementById('footer').innerHTML = markdown.toHTML(this.$store.state.settings.footer)
        }
    },
}
</script>

<style>
.footer p {
    margin-bottom: 0px !important;
}
</style>
