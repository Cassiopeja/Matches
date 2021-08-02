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
          <Card
            v-for="index in game.indexesInRow(r)"
            :key="index"
            :game="game"
            :index="index"
            @click="onCardClicked(index)"
          />
        </div>
      </div>
    </div>
  </v-container>
</template>

<script>
import Game from "@/models/Game";
import { mapGetters } from "vuex";
import PlayersList from "@/components/PlayersList";
import GameScore from "@/models/GameScore";
import Card from "@/components/Card";

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
    Card,
    PlayersList
  },
  methods: {
    delay(ms) {
      return new Promise(res => setTimeout(res, ms));
    },
    enableMovesIfCurrentPlayerIsPlaying() {
      if (this.isCurrentPlayerTurn) {
        this.canClick = true;
      }
    },
    async gameReload() {
      await Game.refresh(this.id);
      this.enableMovesIfCurrentPlayerIsPlaying();
    },
    async connectToGame() {
      try {
        await this.$gameHub.client.invoke("ConnectToGame", this.id);
      } catch (e) {
        console.error(e);
        this.$notify({ title: e });
      }
    },
    async onCardClicked(index) {
      if (!(this.isCurrentPlayerTurn && this.canClick)) {
        return;
      }
      try {
        this.canClick = false;
        await this.$gameHub.client.invoke(
          "PlayerOpenedCard",
          this.id,
          this.currentPlayer,
          index
        );
      } catch (e) {
        this.canClick = true;
      }
    },
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    game() {
      return Game.find(this.id);
    },
    isCurrentPlayerTurn() {
      return this.game.currentPlayer.id === this.currentPlayer.id;
    },
  },
  async beforeMount() {
    this.$gameHub.on("ConnectedToHub", () => this.connectToGame());
    await this.gameReload();

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
      await this.gameReload();
      await this.connectToGame();
    });
  },
  beforeDestroy() {
    this.$gameHub.client.off("GroupPlayerOpenedCard");
    this.$gameHub.client.off("GroupPlayerOpenedTwoEqualsCards");
    this.$gameHub.client.off("GroupNextPlayer");
    this.$gameHub.client.off("GroupGameIsFinished");
    this.$gameHub.off("ConnectedToHub", () => this.connectToGame());
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
