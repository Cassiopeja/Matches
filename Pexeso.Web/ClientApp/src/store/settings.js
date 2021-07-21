export default {
  state: {
    currentPlayer: null
  },
  getters: {
    currentPlayer(state) {
      return state.currentPlayer;
    }
  },
  mutations: {
    setCurrentPlayer(state, player) {
      state.currentPlayer = player;
    }
  },
  actions: {
    setCurrentPlayer(context, player) {
      context.commit("setCurrentPlayer", player);
    }
  }
};
