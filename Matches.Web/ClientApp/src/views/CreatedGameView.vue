<template>
  <v-container>
    <GameCard v-if="createdGame" :created-game="createdGame">
      <template v-slot:actions>
        <v-btn text class="success" @click="onStartClicked">
          <v-icon left>mdi-play-circle-outline</v-icon>
          Start game
        </v-btn>
        <v-btn text class="warning" @click="onLeaveClicked">
          <v-icon left>mdi-logout-variant</v-icon>
          Leave game
        </v-btn>
      </template>
    </GameCard>
  </v-container>
</template>

<script>
import CreatedGame from "@/models/CreatedGame";
import CardTemplate from "@/models/CardTemplate";
import GameCard from "@/components/GameCard";
import {mapGetters} from "vuex";

export default {
  name: "CreatedGameView",
  components: { GameCard },
  props: {
    id: {
      required: true
    }
  },
  methods: {
    async reloadCreatedGame() {
      await CreatedGame.refresh(this.id);
      await CardTemplate.require(this.createdGame.cardTemplateId);
      try {
        await this.$gameHub.client.invoke("ConnectToCreatedGame", this.id);
      } catch (e) {
        console.error(e);
        this.$notify({ title: e });
      }
    },
    onGameStart()
    {
        CreatedGame.delete(this.id);
        this.$router.push({
          name: "GameView",
          params: { id: this.id }
        });
    },
    async onStartClicked() {
      try{
        await this.$gameHub.client.invoke("StartGame", this.id, this.currentPlayer.id);
        this.onGameStart();
      }
      catch (e) {
        console.error(e);
        this.$notify({title:e});
      }
    },
    async onLeaveClicked() {
      try{
        await this.$router.push({ name: "Home" });
      }
      catch (e) {
       console.log(e);
      }
    },
    async leaveGame() {
      await this.$gameHub.client.invoke("LeaveCreatedGame", this.id, this.currentPlayer.id);
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    template() {
      return CardTemplate.find(this.createdGame.cardTemplateId);
    },
    templateImage() {
      return window.location.origin + "/" + this.template.backCardImageUrl;
    },
    createdGame() {
      return CreatedGame.find(this.id);
    }
  },
  async beforeRouteLeave(to, from, next) {
      if (to.name === "Home") {
        await this.leaveGame();
      }
      next();
  },
  async beforeMount() {
    await this.reloadCreatedGame();
    this.$gameHub.client.on("GroupPlayerJoinedCreatedGame", async (player) => {
      await this.createdGame.addPlayer(player);
      this.$notify({ title: player.name + " joined the game" });
    });

    this.$gameHub.client.on("GroupPlayerLeftCreatedGame", async (player) => {
      await this.createdGame.removePlayer(player);
      this.$notify({ title: player.name + " left the game" });
    });
    
    this.$gameHub.client.on("GroupGameStarted", () => {
      this.onGameStart();
    });
  },
  beforeDestroy() {
    this.$gameHub.client.off("GroupPlayerJoinedCreatedGame");
    this.$gameHub.client.off("GroupPlayerLeftCreatedGame");
    this.$gameHub.client.off("GroupGameStarted");
  }
};
</script>

<style scoped></style>
