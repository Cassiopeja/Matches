<template>
  <v-card>
    <v-list subheader>
      <v-subheader>Players:</v-subheader>
      <v-list-item v-for="player in game.players" :key="player.id">
        <v-list-item-icon class="mr-2">
          <v-icon v-if="isCurrentPlayer(player)">mdi-arrow-right-bold</v-icon>
        </v-list-item-icon>
        <v-list-item-avatar>
          <PlayerAvatar :player="player" size="32" />
        </v-list-item-avatar>
        <v-list-item-content v-text="player.score"> </v-list-item-content>
      </v-list-item>
    </v-list>
  </v-card>
</template>

<script>
import PlayerAvatar from "@/components/PlayerAvatar";
export default {
  name: "PlayersList",
  components: { PlayerAvatar },
  props: {
    game: {
      required: true
    }
  },
  methods: {
    isCurrentPlayer(player) {
      return player.id === this.game.currentPlayer.id;
    }
  }
};
</script>

<style scoped></style>
