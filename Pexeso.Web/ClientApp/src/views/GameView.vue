<template>
  <v-container v-if="game" fluid>
    <div class="grid-container">
    <div
      v-for="r in game.boardState.rows"
      :key="r"
      class="d-flex flex-grow-1 justify-center"
      style="width: 100%"
    >
      <v-card
        hover
        v-for="c in game.boardState.columns"
        :key="c"
        class="flex-grow-1"
        style="margin: 3px"
      >
        <div :style="imageStyle"></div>
      </v-card>
    </div>
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

.grid-container {
  width: min(80vw, 80vh);
  height: min(80vw, 80vh);
  margin: auto;
  display: flex;
  flex-direction: column;

}

</style>
