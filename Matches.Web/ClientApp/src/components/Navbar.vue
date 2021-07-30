<template>
  <v-app-bar app color="blue darken-4" dark dense>
    <v-app-bar-title class="text-uppercase text--white">
      <span style="cursor: pointer" @click="onGoHome">
        MATCHES
      </span>
    </v-app-bar-title>
    <v-spacer />
    <div style="cursor: pointer" @click="createPlayer">
      <player-avatar
        v-if="currentPlayer"
        :player="currentPlayer"
        size="32"
      ></player-avatar>
      <div v-else>Create player</div>
    </div>
    <current-player-dialog v-model="show.playerDialog" />
  </v-app-bar>
</template>

<script>
import PlayerAvatar from "@/components/PlayerAvatar";
import CurrentPlayerDialog from "@/components/CurrentPlayerDialog";
import { mapGetters } from "vuex";

export default {
  name: "Navbar",
  components: { PlayerAvatar, CurrentPlayerDialog },
  data() {
    return{
      show: {
        playerDialog: false
      },
    }
  },
  methods: {
    onGoHome() {
      this.$router.push("/").catch(e => {
        console.error(e);
      });
    },
    createPlayer() {
      this.show.playerDialog = !this.show.playerDialog;
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
  }
};
</script>

<style scoped></style>
