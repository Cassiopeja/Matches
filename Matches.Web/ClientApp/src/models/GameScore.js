import { Model } from "@vuex-orm/core";

export default class GameScore extends Model {
  static entity = "gameScores";

  static fields() {
    return {
      id: this.string(),
      players: this.attr([]),
      winners: this.attr([])
    };
  }
}
