<template>
  <div class="home">
    <v-container>
      <v-row dense>
        <v-col cols="12" class="d-flex justify-center">
          <v-btn @click="show.dialog=true" color="pink" dark>Create new game</v-btn
          >
        </v-col>
        <v-col cols="12"
          v-for="item in games"
          :key="item.id">
          <v-card>
            <div class="d-flex flex-no-wrap justify-space-between">
              <div>
                <v-card-title>
                 Game [{{item.rows}} x {{item.columns}}] has been created by {{item.createdBy}} 
                </v-card-title>
                <v-card-subtitle>
                  Created on {{getFormmatedDateTime(item.createdOn)}}
                </v-card-subtitle>
                <v-card-text>
                  <div>
                    {{item.players.length}} players already  joined game
                  </div>
                </v-card-text>
                <v-card-actions>
                  <v-btn
                      class="ml-2 mt-5"
                      outlined
                      rounded
                      small
                      color="success"
                  >
                    JOIN
                  </v-btn>
                </v-card-actions>
              </div>
              <v-avatar
                  class="ma-3"
                  size="125"
                  tile
              >
                <v-img :src="getBackCardImageUrl(item.cardTemplateId)"></v-img>
              </v-avatar>
            </div>
          </v-card>
        </v-col>
      </v-row>
      <create-game-dialog v-model="show.dialog"/>
    </v-container>
  </div>
</template>

<script>
// @ is an alias to /src

import CreateGameDialog from "@/components/CreateGameDialog";
export default {
  name: "Home",
  components: {CreateGameDialog},
  data() {
    return {
      games: [],
      selectedGame: null,
      templates: [],
      show: {dialog: false}
    };
  },
  methods: {
    async onCreateNewGameClicked() {
      const parameters = {
        rows: 4,
        columns: 2,
        playerName: "user",
        templateId: this.templates[0].id
      };
      try {
        const createdGame = await this.$gameHub.client.invoke(
          "CreateGame",
          parameters
        );
        this.games.push(createdGame);
      } catch (e) {
        this.$notify({ title: e });
      }
    },
    getBackCardImageUrl(templateId){
      const template = this.templates.find(t=>t.id === templateId);
      if (template === undefined)
      {
        return undefined;
      }
      
      return template.backCardImageUrl;
    },
    getFormmatedDateTime(dateTimeStr)
    {
      const date = new Date(Date.parse(dateTimeStr));
      //TODO: add locale
      return date.toLocaleString();
    }
  },
  mounted() {
    this.$http
      .get("/createdgames")
      .then(response => {
        this.games = response.data;
      })
      .catch(error => console.error(error));

    this.$http
      .get("/cardTemplates")
      .then(response => {
        this.templates = response.data;
      })
      .catch(error => console.error(error));

    this.$gameHub.client.on("GameCreated", createdGame => {
      this.games.push(createdGame);
      this.$notify({ title: "Game created by " + createdGame.createdBy });
    });
  }
};
</script>
