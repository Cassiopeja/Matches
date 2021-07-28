<template>
  <v-container v-if="gameScore" class="d-flex flex-column">
    <v-card>
      <v-card-title class="font-weight-bold">Game scores:</v-card-title>
      <v-divider></v-divider>
      <v-list denst>
        <v-list-item
          v-for="player in gameScore.players" :key="player.id"
        >
          <v-list-item-icon class="mr-2">
            <v-icon v-if="isWinner(player)" color="#cc9900">mdi-trophy</v-icon>
          </v-list-item-icon>
          <v-list-item-avatar class="mr-2">
            <PlayerAvatar :player="player" size="32"/>
          </v-list-item-avatar>
          <v-list-item-content>{{player.name}}</v-list-item-content>
          <v-list-item-content class="align-end">{{player.score}}</v-list-item-content>
        </v-list-item>
      </v-list>
    </v-card>
  </v-container>
</template>

<script>
import GameScore from "@/models/GameScore";
import { mapGetters } from "vuex";
import PlayerAvatar from "@/components/PlayerAvatar";

export default {
  name: "GameScoresView",
  props: {
    id: {
      required: true
    }
  },
  components: { PlayerAvatar },
  methods:{
    isWinner(player) {
      return this.gameScore.winners.find(pl=>pl.id === player.id) !== undefined;
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    gameScore() {
      return GameScore.find(this.id);
    }
  },
  mounted() {
    if (this.gameScore !== null && this.gameScore?.winners?.find(pl=>pl.id === this.currentPlayer.id) !== undefined)
    {
      this.$confetti.start();
    }
  },
  beforeRouteLeave(to, from, next) {
    this.$confetti.stop();
    next();
  },
};
</script>

<style scoped></style>
