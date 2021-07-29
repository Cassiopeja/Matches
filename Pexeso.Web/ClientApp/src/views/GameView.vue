<template>
  <v-container v-if="game" fluid>
    <div class="d-flex flex-wrap">
      <div>
        <PlayersList :game="game" class="justify-start" />
      </div>
      <div class="grid-container">
        <div
          :style="{ visibility: isCurrentPlayerTurn ? 'visible' : 'hidden' }"
          class="font-weight-light text-center"
        >
          Your turn
        </div>
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
import {mapGetters} from "vuex";
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
    async gameReload() {
      await Game.refresh(this.id);
      this.enableMovesIfCurrentPlayerIsPlaying();
    },
    async fullGameAndHubReload() {
      await Game.refresh(this.id);
      try {
        await this.$gameHub.client.invoke("ConnectToGame", this.id);
      } catch (e) {
        console.error(e);
        this.$notify({ title: e });
      }
      this.enableMovesIfCurrentPlayerIsPlaying();
    },
    async reconnectAndReload() {
      if (this.$gameHub.client.state === "Connecting") {
        while (
          this.$gameHub.client.state !== "Connected" &&
          this.$gameHub.client.state !== "Disconnected"
        ) {
          await this.delay(1000);
        }
      }
      if (this.$gameHub.client.state === "Disconnected") {
        console.error("Can not connect to hub");
      } else {
        await this.fullGameAndHubReload();
      }
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
      return window.location.origin + "/" + imagePath;
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
      return Game.find(this.id);
    },
    isCurrentPlayerTurn() {
      return this.game.currentPlayer.id === this.currentPlayer.id;
    }
  },
  async beforeMount() {
    if (this.$gameHub.client.state === "Connected") {
      await this.gameReload();
    } else {
      await this.reconnectAndReload();
    }

    this.$gameHub.client.on("GroupPlayerOpenedCard", async (player, move) => {
      if (this.game.isFirstMove()) {
        await this.game.doFirstMove(move);
        this.enableMovesIfCurrentPlayerIsPlaying();
      } else {
        await this.game.doSecondMove(move);
      }
    });

    this.$gameHub.client.on(
      "GroupPlayerOpenedTwoEqualsCards",
      async (player, openedIndexes) => {
        await this.delay(1000);
        await this.game.playerOpenedTwoEqualsCards(player, openedIndexes);
      }
    );

    this.$gameHub.client.on("GroupNextPlayer", async player => {
      await this.delay(1500);
      await this.game.setNextPlayer(player);
      this.enableMovesIfCurrentPlayerIsPlaying();
    });
    this.$gameHub.client.on(
      "GroupGameIsFinished",
      async (orderedByScorePlayers, winners) => {
        await this.delay(1000);
        await GameScore.insert({
          data: {
            id: this.id,
            players: orderedByScorePlayers,
            winners: winners
          }
        });
        await Game.delete(this.id);
        await this.$router.push({
          name: "GameScoreView",
          params: { id: this.id }
        });
      }
    );
    this.$gameHub.client.onreconnected(async () => {
      await this.fullGameAndHubReload();
    });
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
