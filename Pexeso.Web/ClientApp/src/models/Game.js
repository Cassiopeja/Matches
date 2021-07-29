import { Model } from "@vuex-orm/core";

export default class Game extends Model {
  static entity = "games";

  static fields() {
    return {
      id: this.string(),
      gameState: this.number(),
      boardState: this.attr({}),
      currentPlayer: this.attr({}),
      players: this.attr([]),
      firstMove: this.attr({}).nullable(),
      secondMove: this.attr({}).nullable()
    };
  }

  static async refresh(id) {
    await this.api().get(`/games/${id}`, {
      persistBy: "insertOrUpdate"
    });
  }

  static async require(id) {
    if (!this.find(id)) await this.refresh(id);
  }

  isFirstMove() {
    return this.firstMove === null;
  }

  async doFirstMove(move) {
    await this.$update({
      firstMove: move
    });
  }

  async doSecondMove(move) {
    await this.$update({
      secondMove: move
    });
  }

  async playerOpenedTwoEqualsCards(player, openedIndexes) {
    const boardState = this.boardState;
    boardState.openedCardsIndexes = boardState.openedCardsIndexes.concat(
      openedIndexes
    );
    const scorePlayer = this.players.find(pl => pl.id === player.id);
    scorePlayer.score++;
    await this.$update({
      boardState: boardState,
      firstMove: null,
      secondMove: null,
      players: this.players
    });
  }

  async setNextPlayer(player) {
    await this.$update({
      currentPlayer: player,
      firstMove: null,
      secondMove: null
    });
  }
}
