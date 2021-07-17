<template>
  <div class="home">
    <v-card>
      
    <v-card-title>Created games:</v-card-title>
      <v-card-text>
        <v-list>
          <v-list-item-group v-model="selectedGame">
            <v-list-item
                v-for="item in games"
                :key="item.id"
            >
              <v-list-item-content>
                <v-list-item-title v-text="item.id"></v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-card-text>
      <v-card-actions>
        <v-btn>Join</v-btn>
        <v-btn @click="onCreateNewGameClicked">Create new</v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>

<script>
// @ is an alias to /src

export default {
  name: 'Home',
  components: {
  },
  data() {
    return {
      games:[],
      selectedGame:null,
      templates: []
    }
  },
  methods:{
    async onCreateNewGameClicked(){
      console.log("Create new game");
      const parameters = {
        rows: 4,
        columns: 2,
        playerName: "user",
        templateId: this.templates[0].id
      };
      try {
        await this.$gameHub.client.invoke("CreateGame", parameters);
      }
      catch (e) {
        console.error(e);
      }
    }
  },
  mounted() {
    this.$http.get("/createdgames")
    .then(response => {
      this.games = response.data
    })
    .catch(error=> console.error(error));
    
    this.$http.get("/cardTemplates")
    .then(response => {
      this.templates = response.data;
    })
        .catch(error=> console.error(error));
    
    this.$gameHub.client.on("GameCreated", (createdGame) =>{
      this.games.push(createdGame);
      console.log(this.games);
    });
  }
}
</script>
