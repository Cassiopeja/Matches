<template>
  <div class="flex-grow-1 ma-1" @click="onClick">
    <v-fab-transition>
      <v-hover v-slot="{ hover }">
        <flipper
          :flipped="isCardFlipped"
          v-show="isCardVisible"
          duration="0.3s"
        >
          <div
            slot="front"
            :style="backCardStyle"
            :class="{ 'elevation-2': !hover, 'elevation-4': hover }"
          />
          <div
            slot="back"
            :style="frontCardStyle"
            :class="{ 'elevation-2': !hover, 'elevation-4': hover }"
          />
        </flipper>
      </v-hover>
    </v-fab-transition>
  </div>
</template>
<script>
import "vue-flipper/dist/vue-flipper.css";
import Flipper from "vue-flipper";

export default {
  name: "Card",
  components: { Flipper },
  props: {
    game: {
      required: true
    },
    index: {
      required: true
    }
  },
  methods: {
    onClick() {
      this.$emit("click");
    },
    imageStyle(image) {
      const imageFullUrl = window.location.origin + "/" + image;
      return {
        width: "100%",
        height: "100%",
        backgroundImage: `url(${imageFullUrl})`,
        backgroundSize: "contain",
        backgroundRepeat: "no-repeat",
        backgroundPosition: "50% 50%",
        borderRadius: "5px"
      };
    }
  },
  computed: {
    backCardStyle() {
      return this.imageStyle(this.game.getBackImage());
    },
    frontCardStyle() {
      return this.imageStyle(this.game.getFrontImage(this.index));
    },
    isCardFlipped() {
      return this.game.isCardFlipped(this.index);
    },
    isCardVisible() {
      return this.game.isCardVisible(this.index);
    }
  }
};
</script>
<style scoped>
body {
  margin: 0;
}
</style>
