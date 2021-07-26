<template>
  <v-container v-if="game" fluid class="fill-height d-flex flex-column">
    <div
      v-for="r in game.boardState.rows"
      :key="r"
      class="d-flex flex-grow-1 justify-center"
      style="width: 80vw"
    >
      <v-card
        hover
        flat
        v-for="c in game.boardState.columns"
        :key="c"
        class="flex-grow-1 mr-1 mb-1"
      >
        <div :style="imageStyle"></div>
      </v-card>
    </div>
  </v-container>
</template>

<script>
import Game from "@/models/Game";

export default {
  name: "GameVue",
  props: {
    id: {
      required: true
    }
  },
  computed: {
    backImageUrl() {
      const game = Game.find(this.id);
      const url = window.location.origin + "/" + game.boardState.backImageUrl;
      return url;
    },
    imageStyle() {
      return {
        width: "100%",
        height: "100%",
        backgroundImage: `url(${this.backImageUrl})`,
        backgroundSize: "contain",
        backgroundRepeat: "no-repeat",
        backgroundPosition: "50% 50%"
      };
    },
    game() {
      const game = Game.find(this.id);
      return game;
    }
  },
  async beforeMount() {
    await Game.refresh(this.id);
  }
};
</script>

<style scoped>
body {
  margin: 0;
}
</style>
