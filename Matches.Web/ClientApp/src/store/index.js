import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";
import VuexORM from "@vuex-orm/core";
import VuexORMAxios from "@vuex-orm/plugin-axios";
import settings from "@/store/settings";
import createPersistedState from "vuex-persistedstate";
import CardTemplate from "@/models/CardTemplate";
import CreatedGame from "@/models/CreatedGame";
import Game from "@/models/Game";
import GameScore from "@/models/GameScore";

VuexORM.use(VuexORMAxios, { axios });
const database = new VuexORM.Database();
database.register(CardTemplate);
database.register(CreatedGame);
database.register(Game);
database.register(GameScore);

Vue.use(Vuex);

export default new Vuex.Store({
  state: {},
  mutations: {},
  actions: {},
  modules: {
    settings
  },
  plugins: [
    createPersistedState({
      paths: ["settings"]
    }),
    VuexORM.install(database)
  ]
});
