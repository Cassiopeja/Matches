<template>
  <div class="home">
    <v-container>
      <v-row dense>
        <v-col cols="12" class="d-flex justify-center">
          <v-btn @click="onCreateNewGameClicked" color="pink" dark
            >Create new game</v-btn
          >
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="3"
          v-for="game in games"
          :key="game.id"
        >
          <GameCard :created-game="game">
            <template v-slot:actions>
              <v-btn text @click="onJoinGameClicked(game)">
                <v-icon left>mdi-login-variant</v-icon>
                Join
              </v-btn>
            </template>
          </GameCard>
        </v-col>
      </v-row>
      <create-game-dialog v-model="show.gameDialog" />
      <current-player-dialog v-model="show.playerDialog" />
    </v-container>
  </div>
</template>

<script>
// @ is an alias to /src

import CreateGameDialog from "@/components/CreateGameDialog";
import { mapGetters } from "vuex";
import CurrentPlayerDialog from "@/components/CurrentPlayerDialog";
import CardTemplate from "@/models/CardTemplate";
import CreatedGame from "@/models/CreatedGame";
import GameCard from "@/components/GameCard";

export default {
  name: "Home",
  components: { GameCard, CurrentPlayerDialog, CreateGameDialog },
  data() {
    return {
      selectedGame: null,
      show: {
        gameDialog: false,
        playerDialog: false
      },
      showGameDialog: {
        dialog: false
      },
      showPlayerDialog: {
        dialog: false
      }
    };
  },
  methods: {
    async onCreateNewGameClicked() {
      if (this.currentPlayer === null) {
        this.showCreatePlayerDialog();
        return;
      }

      this.show.gameDialog = true;
    },
    showCreatePlayerDialog() {
      this.$notify("There is no player selected");
      this.show.playerDialog = true;
    },
    async onJoinGameClicked(game) {
      if (this.currentPlayer === null) {
        this.showCreatePlayerDialog();
        return;
      }

      try {
        await this.$gameHub.client.invoke(
          "JoinCreatedGame",
          game.id,
          this.currentPlayer
        );
        await this.$router.push({
          name: "CreatedGameView",
          params: { id: game.id }
        });
      } catch (e) {
        this.$notify({ title: e });
        console.error(e);
      }
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    games() {
      return CreatedGame.all();
    }
  },
  async beforeMount() {
    await CardTemplate.reload();
    await CreatedGame.reload();

    this.$gameHub.client.on("GameCreated", createdGame => {
      CreatedGame.insert({ data: createdGame });
      this.$notify({ title: `Game created by ${createdGame.createdBy}` });
    });

    this.$gameHub.client.on("PlayerJoinedCreatedGame", (gameId, player) => {
      const game = CreatedGame.find(gameId);
      if (game === null) {
        return;
      }
      const players = [...game.players];
      players.push(player);
      CreatedGame.update({ where: gameId, data: { players: players } });
      this.$notify({ title: player.name + " joined the game" });
    });

    this.$gameHub.client.on("PlayerLeftCreatedGame", (gameId, player) => {
      const game = CreatedGame.find(gameId);
      if (game === null) {
        return;
      }
      const players = game.players.filter(pl => pl.id !== player.id);
      CreatedGame.update({ where: gameId, data: { players: players } });
      this.$notify({ title: player.name + " left the game" });
    });
  },
  beforeDestroy() {
    this.$gameHub.client.off("GameCreated");
    this.$gameHub.client.off("PlayerJoinedCreatedGame");
    this.$gameHub.client.off("PlayerLeftCreatedGame");
  }
};
</script>
