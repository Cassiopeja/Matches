<template>
  <div class="home">
    <v-container>
      <v-row dense>
        <v-col cols="12" class="d-flex justify-center">
          <v-btn @click="onCreateNewGameClicked" color="pink" dark
            >Create new game</v-btn
          >
        </v-col>
        <v-col cols="12" v-for="item in games" :key="item.id">
          <v-card>
            <div class="d-flex flex-no-wrap justify-space-between">
              <div>
                <v-card-title>
                  Game [{{ item.rows }} x {{ item.columns }}] has been created
                  by {{ item.createdBy }}
                </v-card-title>
                <v-card-subtitle>
                  Created on {{ getFormattedDateTime(item.createdOn) }}
                </v-card-subtitle>
                <v-card-text>
                  <div>
                    {{ item.players.length }} players already joined game
                  </div>
                </v-card-text>
                <v-card-actions>
                  <v-btn
                    class="ml-2 mt-5"
                    outlined
                    rounded
                    small
                    color="success"
                    @click="onJoinGameClicked(item)"
                  >
                    JOIN
                  </v-btn>
                </v-card-actions>
              </div>
              <v-avatar class="ma-3" size="125" tile>
                <v-img :src="getBackCardImageUrl(item.cardTemplateId)"></v-img>
              </v-avatar>
            </div>
          </v-card>
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

export default {
  name: "Home",
  components: { CurrentPlayerDialog, CreateGameDialog },
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
    },
    getBackCardImageUrl(templateId) {
      const template = CardTemplate.find(templateId);
      return template?.backCardImageUrl;
    },
    getFormattedDateTime(dateTimeStr) {
      const date = new Date(Date.parse(dateTimeStr));
      //TODO: add locale
      return date.toLocaleString();
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    games() {
      return CreatedGame.all();
    }
  },
  async mounted() {
    await CardTemplate.reload();
    await CreatedGame.reload();

    this.$gameHub.client.on("GameCreated", createdGame => {
      CreatedGame.insert({data:createdGame});
      this.$notify({ title: `Game created by ${createdGame.createdBy}` });
    });
    

    this.$gameHub.client.on("PlayerJoinedCreatedGame", (gameId, player) => {
      const game = CreatedGame.find(gameId);
      if (game === null) return;
      game.players.push(player);
      this.$notify({ title: player.name + " joined the game" });
    });
  },
  beforeDestroy() {
    this.$gameHub.client.off("GameCreated");
    this.$gameHub.client.off("PlayerJoinedCreatedGame");
  }
};
</script>
