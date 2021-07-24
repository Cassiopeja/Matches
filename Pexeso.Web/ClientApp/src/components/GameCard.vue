<template>
  <v-card v-if="!loading" hover class="ma-3">
    <v-toolbar dark flat color="blue darken-2">
      {{ template.name }}
      [{{ createdGame.rows }} x {{ createdGame.columns }}]
      <v-spacer/>
      <div class="d-flex flex-wrap flex-row">
        <div
            v-for="player in createdGame.players"
            :key="player.id"
            class="pr-2 pb-2"
        >
          <player-avatar :player="player" size="32" />
        </div>
      </div>
    </v-toolbar>
    <v-img :src="templateImage" max-height="240" contain class="mt-2"/>
    <v-card-text>
      <div>
        <span class="caption">
            Created by:
        </span>
        {{ createdGame.createdBy }} {{ created }}
      </div>
    </v-card-text>
    <v-card-actions>
      <slot name="actions"> </slot>
    </v-card-actions>
  </v-card>
</template>

<script>
import { parseISO, formatRelative } from 'date-fns'
import PlayerAvatar from "@/components/PlayerAvatar";
import CardTemplate from "@/models/CardTemplate";

export default {
  name: "GameCard",
  props: {
    createdGame: {
      required: true
    },
    maxWidth: {
      required: false,
      default: 345
    }
  },
  components: { PlayerAvatar },
  data()  {
    return {
      loading: true,
    }
  },
  computed: {
    template() {
      return CardTemplate.find(this.createdGame.cardTemplateId);
    },
    templateImage() {
      const url = window.location.origin + "/" + this.template.backCardImageUrl;
      return url;
    },
    created() {
      return formatRelative(
          parseISO(this.createdGame.createdOn),
          new Date()
      )
    }
  },
  async beforeMount() {
    this.loading = true;
    await CardTemplate.require(this.createdGame.cardTemplateId);
    this.loading = false;
  }
};
</script>

<style scoped></style>
