<template>
  <v-container>
    <v-card
      v-if="createdGame && template"
      :loading="loading"
      class="mx-auto my-12"
      max-width="374"
    >
      <v-img :src="templateImage" />
      <v-card-title>Waiting for other players...</v-card-title>
      <v-card-text>
        <div class="my-4 text-subtitle-1">
          Selected card set: {{ template.name }}
        </div>
        <div class="my-4 text-subtitle-1">
          Game size: [{{ createdGame.rows }} x {{ createdGame.columns }}]
        </div>
        <div class="my-4 text-subtitle-1">
          Created by: {{ createdGame.createdBy }}
        </div>
        <div class="my-4 text-subtitle-1">Players joined the game:</div>
          <div
              class="d-flex flex-wrap flex-row"
          >
            <div
                v-for="player in createdGame.players"
                :key="player.id"
                class="pr-2 pb-2"
            >
              <player-avatar
                  :player="player"
                  size="32"
              />
            </div>
          </div>
      </v-card-text>
      <v-card-actions>
        <v-btn @click="onStartClicked">Start game</v-btn>
        <v-btn @click="onLeaveClicked">Leave game</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script>
import CreatedGame from "@/models/CreatedGame";
import CardTemplate from "@/models/CardTemplate";
import PlayerAvatar from "@/components/PlayerAvatar";

export default {
  name: "CreatedGameView",
  components: { PlayerAvatar },
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
    async onStartClicked(){
      console.log("Starting game")
    },
    async onLeaveClicked(){
      await this.$router.push({"name":"Home"});
    },
    async leaveGame()
    {
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
    },
    players() {
      const players = [];
      for (let i = 0; i < 13; i++) {
        players.push({
          id: i,
          name: "player" + i,
          color: "#FF0000"
        });
      }
      
      return players;
    }
  },
  async beforeRouteLeave(to, from, next)
  {
    if (to.name === "Home")
    {
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
  },
};
</script>

<style scoped></style>
