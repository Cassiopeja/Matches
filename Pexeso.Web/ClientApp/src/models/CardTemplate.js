import { Model } from "@vuex-orm/core";

export default class CardTemplate extends Model {
  static entity = "cardTemplates";

  static fields() {
    return {
      id: this.string(),
      name: this.string(),
      totalCount: this.number(),
      backCardImageUrl: this.string(),
      cards: this.attr()
    };
  }

  static async reload() {
    await this.api().get("/cardTemplates", {
      persistBy: "insertOrUpdate"
    });
  }

  static async refresh(templateId) {
    await this.api().get(`/cardTemplates/${templateId}`, {
      persistBy: "insertOrUpdate"
    });
  }

  static async require(templateId) {
    if (!this.find(templateId)) await this.refresh(templateId);
  }
}
