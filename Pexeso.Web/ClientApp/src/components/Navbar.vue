<template>
  <v-app-bar app color="blue darken-4" dark dense>
    <v-app-bar-title class="text-uppercase text--white">
      <span style="cursor: pointer" @click="onGoHome">
        PEXESO
      </span>
    </v-app-bar-title>
    <v-spacer />
    <player-avatar v-if="currentPlayer" :player="currentPlayer" size="32"></player-avatar>
  </v-app-bar>
</template>

<script>
import PlayerAvatar from "@/components/PlayerAvatar";
import { mapGetters } from "vuex";

export default {
  name: "Navbar",
  components: { PlayerAvatar },
  methods: {
    onGoHome() {
      this.$router.push("/").catch(e => {
        console.error(e);
      });
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"])
  }
};
</script>

<style scoped></style>
