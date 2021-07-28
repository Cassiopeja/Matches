<template>
  <v-container v-if="game" fluid>
    <div class="d-flex flex-wrap">
      <div>
        <PlayersList :game="game" class="justify-start" />
      </div>
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
            @click="onCardClicked(r, c)"
          >
            <v-fab-transition>
              <v-hover v-slot="{ hover }">
                <flipper
                  :flipped="isCardFlipped(r, c)"
                  v-show="isCardVisible(r, c)"
                  duration="0.3s"
                >
                  <div
                    slot="front"
                    :style="imageStyle(game.boardState.backImageUrl)"
                    :class="{ 'elevation-2': !hover, 'elevation-4': hover }"
                  />
                  <div
                    slot="back"
                    :style="imageStyleFront(r, c)"
                    :class="{ 'elevation-2': !hover, 'elevation-4': hover }"
                  />
                </flipper>
              </v-hover>
            </v-fab-transition>
          </div>
        </div>
      </div>
    </div>
  </v-container>
</template>

<script>
import Game from "@/models/Game";
import Flipper from "vue-flipper";
import { mapGetters } from "vuex";
import PlayersList from "@/components/PlayersList";
import "vue-flipper/dist/vue-flipper.css";
import GameScore from "@/models/GameScore";

export default {
  name: "GameVue",
  props: {
    id: {
      required: true
    }
  },
  data() {
    return {
      canClick: false
    };
  },
  components: {
    PlayersList,
    Flipper
  },
  methods: {
    delay(ms) {
      return new Promise(res => setTimeout(res, ms));
    },
    async onCardClicked(row, column) {
      if (this.game.currentPlayer.id !== this.currentPlayer.id) {
        return;
      }
      if (!this.canClick) {
        return;
      }
      try {
        this.canClick = false;
        await this.$gameHub.client.invoke(
          "PlayerOpenedCard",
          this.id,
          this.currentPlayer,
          this.index(row, column)
        );
      } catch (e) {
        this.$notify({ title: e });
        console.error(e);
        this.canClick = true;
      }
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
        borderRadius: "5px"
      };
    },
    imageStyleFront(row, column) {
      if (!this.isCardFlipped(row, column)) {
        return null;
      }

      const index = this.index(row, column);
      let image = this.game.secondMove?.cardImageUrl;
      if (this.game.firstMove?.index === index) {
        image = this.game.firstMove.cardImageUrl;
      }
      return this.imageStyle(image);
    },
    index(row, column) {
      return (row - 1) * this.game.boardState.columns + column - 1;
    },
    isCardVisible(row, column) {
      return !this.game.boardState.openedCardsIndexes.includes(
        this.index(row, column)
      );
    },
    isCardFlipped(row, column) {
      const index = this.index(row, column);
      let flipped = false;

      if (
        this.game.firstMove?.index === index ||
        this.game.secondMove?.index === index
      ) {
        flipped = true;
      }

      return flipped;
    },
    enableMovesIfCurrentPlayerIsPlaying() {
      if (this.isCurrentPlayerTurn) {
        this.canClick = true;
      }
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    game() {
      const game = Game.find(this.id);
      return game;
    },
    isCurrentPlayerTurn() {
      return this.game.currentPlayer.id === this.currentPlayer.id;
    }
  },
  async beforeMount() {
    await Game.refresh(this.id);
    this.enableMovesIfCurrentPlayerIsPlaying();

    this.$gameHub.client.on("GroupPlayerOpenedCard", async (player, move) => {
      const game = Game.find(this.id);
      if (game.firstMove === null) {
        await Game.update({
          where: this.id,
          data: { firstMove: move }
        });
        this.$notify({ title: `Player ${player.name} opened first card` });
        this.enableMovesIfCurrentPlayerIsPlaying();
        return;
      }

      await Game.update({
        where: this.id,
        data: { secondMove: move }
      });
      this.$notify({ title: `Player ${player.name} opened second card` });
    });

    this.$gameHub.client.on(
      "GroupPlayerOpenedTwoEqualsCards",
      async (player, openedIndexes) => {
        const game = Game.find(this.id);
        game.boardState.openedCardsIndexes = game.boardState.openedCardsIndexes.concat(
          openedIndexes
        );
        const scorePlayer = game.players.find(pl => pl.id === player.id);
        scorePlayer.score++;
        await Game.update({
          where: this.id,
          data: {
            boardState: game.boardState,
            firstMove: null,
            secondMove: null,
            players: game.players
          }
        });
        this.$notify({
          title: `Player ${player.name} opened two equals cards`
        });
      }
    );

    this.$gameHub.client.on("GroupNextPlayer", async player => {
      await this.delay(3000);
      await Game.update({
        where: this.id,
        data: { currentPlayer: player, firstMove: null, secondMove: null }
      });
      this.$notify({ title: `Next player is ${player.name}` });
      this.enableMovesIfCurrentPlayerIsPlaying();
    });
    this.$gameHub.client.on(
      "GroupGameIsFinished",
      async (orderedByScorePlayers, winners) => {
        await GameScore.insert({
          data: {
            id: this.id,
            players: orderedByScorePlayers,
            winners: winners
          }
        });
        await this.$router.push({ name: "GameScoreView", params: { id: this.id }});
      }
    );
  },
  beforeDestroy() {
    this.$gameHub.client.off("GroupPlayerOpenedCard");
    this.$gameHub.client.off("GroupPlayerOpenedTwoEqualsCards");
    this.$gameHub.client.off("GroupNextPlayer");
    this.$gameHub.client.off("GroupGameIsFinished");
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
