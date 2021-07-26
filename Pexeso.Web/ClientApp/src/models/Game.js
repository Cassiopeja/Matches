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
}
