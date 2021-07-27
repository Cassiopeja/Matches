<template>
  <v-container v-if="game" fluid>
    <div class="grid-container">
    <div
      v-for="r in game.boardState.rows"
      :key="r"
      class="d-flex flex-grow-1 justify-center"
      style="width: 100%"
    >
      <div
          v-for="c in game.boardState.columns"
          :key="c"
          class="flex-grow-1 ma-1"
      >
        <flipper
            :flipped="isFlipped"
            @click="onClick"
          >
          <div slot="front" :style="imageStyle(game.boardState.backImageUrl)">
          </div>
          <div slot="back" :style="imageStyle('templates/nature/IMG_1057.png')">
          </div>
        </flipper>
      </div>
    </div>
    </div>
  </v-container>
</template>

<script>
import Game from "@/models/Game";
import Flipper from 'vue-flipper';

export default {
  name: "GameVue",
  props: {
    id: {
      required: true
    }
  },
  data(){
    return{
      isFlipped: false  
    }
  },
  components:{
    Flipper
  },
  methods:{
    onClick(){
      this.isFlipped = !this.isFlipped;
    },
    imageUrl(imagePath) {
      const url = window.location.origin + "/" + imagePath;
      return url;
    },
    imageStyle(image) {
      const imageFullUrl = this.imageUrl(image);
      return {
        width: "100%",
        height: "100%",
        backgroundImage: `url(${imageFullUrl})`,
        backgroundSize: "contain",
        backgroundRepeat: "no-repeat",
        backgroundPosition: "50% 50%",
      };
    },
  },
  computed: {
    game() {
      const game = Game.find(this.id);
      return game;
    }
  },
  async beforeMount() {
    await Game.refresh(this.id);
  },
};
</script>

<style src="vue-flipper/dist/vue-flipper.css" />
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
