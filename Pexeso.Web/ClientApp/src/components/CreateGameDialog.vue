<template>
  <v-dialog
      v-model="show"
      persistent
      max-width="600"
  >
    <v-card>
      <v-card-title class="headline">
        Create new game
      </v-card-title>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
            color="green darken-1"
            text
            @click="show = false"
        >
           Cancel 
        </v-btn>
        <v-btn
            color="green darken-1"
            text
            @click="onCreate"
        >
           Create
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  name: "CreateGameDialog",
  props: {
    templates:[],
    value: {
      default: false
    },
  },
  data() {
    return {
      valid: true,
      formSubscription: {},
      errors: [],
    }
  },
  methods: {
    async onCreate() {
      console.log("creating");
    },
  },
  computed: {
    show: {
      get() {
        return this.value
      },
      set(v) {
        this.$emit('input', v)
      }
    }
  }
}
</script>

<style scoped>

</style>