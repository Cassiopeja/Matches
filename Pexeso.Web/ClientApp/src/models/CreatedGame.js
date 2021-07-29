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
    await this.deleteAll();
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

  async addPlayer(player) {
    if (this.players.find(pl => pl.id === player.id) === undefined) {
      const players = [...this.players];
      players.push(player);
      await this.$update({ players: players });
    }
  }

  async removePlayer(player) {
    const players = this.players.filter(pl => pl.id !== player.id);
    await this.$update({ players: players });
  }
}
