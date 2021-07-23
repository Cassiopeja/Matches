import { Model } from "@vuex-orm/core";

export default class CreatedGame extends Model {
  static entity = "createdGames";

  static fields() {
    return {
      id: this.string(),
      cardTemplateId: this.string(),
      rows: this.number(),
      columns: this.number(),
      createdBy: this.string(),
      createdOn: this.string(),
      players: this.attr([])
    };
  }

  static async reload() {
    await this.api().get("/createdgames");
  }

  static async refresh(id) {
    await this.api().get(`/createdgames/${id}`, {
      persistBy: "insertOrUpdate"
    });
  }

  static async require(id) {
    if (!this.find(id)) await this.refresh(id);
  }
}
