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

export default {
  name: "CreatedGameView",
  components: { GameCard },
  props: {
    id: {
      required: true
    }
  },
  data() {
    return {
      createdGame: null,
      loading: true
    };
  },
  methods: {
    async onStartClicked() {
      console.log("Starting game");
      try{
        const startedGame = await this.$gameHub.client.invoke("StartGame", this.id);
        console.log("startedgame", startedGame)
      }
      catch (e) {
        console.error(e);
       this.$notify({title:e});
      }
    },
    async onLeaveClicked() {
      await this.$router.push({ name: "Home" });
    },
    async leaveGame() {
      await this.$gameHub.client.send("LeaveCreatedGame", this.id);
    }
  },
  computed: {
    template() {
      return CardTemplate.find(this.createdGame.cardTemplateId);
    },
    templateImage() {
      const url = window.location.origin + "/" + this.template.backCardImageUrl;
      return url;
    }
  },
  async beforeRouteLeave(to, from, next) {
    if (to.name === "Home") {
      await this.leaveGame();
    }
    next();
  },
  async beforeMount() {
    this.loading = true;
    await CreatedGame.refresh(this.id);
    this.createdGame = CreatedGame.find(this.id);
    await CardTemplate.require(this.createdGame.cardTemplateId);
    this.loading = false;

    this.$gameHub.client.on("GroupPlayerJoinedCreatedGame", (player) => {
      if (this.createdGame.players.find(pl=>pl.id === player.id) === undefined)
      {
        this.createdGame.players.push(player);
        this.$notify({ title: player.name + " joined the game" });
      }
    });
    
    this.$gameHub.client.on("GroupPlayerLeftCreatedGame", (player) => {
      this.createdGame.players = this.createdGame.players.filter(pl=> pl.id !== player.id);
      this.$notify({ title: player.name + " left the game" });
    });
  },
  beforeDestroy() {
    this.$gameHub.client.off("GroupPlayerJoinedCreatedGame");
    this.$gameHub.client.off("GroupPlayerLeftCreatedGame");
  }
};
</script>

<style scoped></style>
