<template>
  <v-app-bar 
      app
      color="blue darken-4"
      dark
    >
    <v-app-bar-title class="text-uppercase">
      <div>
        <router-link to="/" class="text-decoration-none">
          <span>PEXESO</span>
        </router-link>
      </div>
    </v-app-bar-title>
    <v-spacer/>
    <player-avatar v-if="currentPlayer" :player="currentPlayer"></player-avatar>
  </v-app-bar>

</template>

<script>
import PlayerAvatar from "@/components/PlayerAvatar";
import {mapGetters} from "vuex";

export default {
  name: "Navbar",
  components: {PlayerAvatar},
  computed: {
    ...mapGetters(['currentPlayer'])
  }
}
</script>

<style scoped>

</style>