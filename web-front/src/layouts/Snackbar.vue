<template>
<v-snackbar v-model="snackbar" right top>
    {{ this.$store.state.snackbar.message }}
    <v-btn color="pink" text @click="snackbar = false">
        关闭
    </v-btn>
</v-snackbar>
</template>

<script>
export default {
    data() {
        return {
            text: this.$store.state.snackbar.message
        }
    },
    computed: {
        snackbar: {
            set: function (value) {
                this.$store.commit('changeSnackBarStatus', value)
            },
            get: function(){
                return this.$store.state.snackbar.isOpen
            }
        }
    },
}
</script>

<style>

</style>
