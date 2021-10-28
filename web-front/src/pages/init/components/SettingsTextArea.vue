<template>
  <div>
    <p class="text-subtitle-1">{{ title }}</p>
          <v-text-field
            dense
            single-line
            outlined
            :value="value"
            v-on:change="$emit('change', $event)"
            hide-details
          ></v-text-field>
  </div>
</template>

<script>
export default {
    props:{
        title:String,
        value:String
    },
    model: {
      prop:"value",
      event:"change"
    }
}
</script>

<style>

</style>