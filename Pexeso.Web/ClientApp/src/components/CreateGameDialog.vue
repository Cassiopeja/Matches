<template>
  <v-dialog v-model="show" persistent max-width="600">
    <v-card>
      <v-card-title class="headline">
        Create new game
      </v-card-title>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <div class="caption">
            Card set
          </div>
          <v-chip-group v-model="selectedTemplateId" column mandatory>
            <v-chip
              v-for="t in templates"
              :key="t.id"
              :value="t.id"
              active-class="indigo white--text"
              label
              dark
            >
              <v-avatar tile left>
                <v-img :src="t.backCardImageUrl" />
              </v-avatar>
              {{ t.name }}
            </v-chip>
          </v-chip-group>
          <v-select
            :items="sizes"
            :item-text="sizeName"
            item-value="id"
            v-model="selectedSize"
            return-object
            label="Game size"
            :rules="sizeRules"
          >
          </v-select>
        </v-form>
        <v-alert
          v-show="errors"
          text
          color="error"
          border="left"
          v-for="error in errors"
          :key="error.name"
        >
          <div v-html="error.value" />
        </v-alert>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="green darken-1" text @click="show = false">
          Cancel
        </v-btn>
        <v-btn color="green darken-1" text @click="onCreate">
          Create
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { mapGetters } from "vuex";

export default {
  name: "CreateGameDialog",
  props: {
    templates: [],
    value: {
      default: false
    }
  },
  data() {
    return {
      valid: true,
      errors: [],
      selectedTemplateId: null,
      selectedSize: null,
      sizes: [
        { id: 1, rows: 4, columns: 4 },
        { id: 3, rows: 6, columns: 6 },
        { id: 5, rows: 8, columns: 8 }
      ]
    };
  },
  methods: {
    async onCreate() {
      this.errors = [];
      this.$refs.form.validate();
      if (this.valid) {
        const parameters = {
          rows: this.selectedSize.rows,
          columns: this.selectedSize.columns,
          playerName: this.currentPlayer.name,
          templateId: this.selectedTemplateId
        };
        try {
          const createdGame = await this.$gameHub.client.invoke(
            "CreateGame",
            parameters
          );
          console.log(createdGame);
          //TODO: redirect to started game view
          // this.show = false;
        } catch (e) {
          this.$notify({ title: e });
        }
      }
    },
    sizeName(item) {
      return `${item.rows} x ${item.columns}`;
    }
  },
  computed: {
    ...mapGetters(["currentPlayer"]),
    show: {
      get() {
        return this.value;
      },
      set(v) {
        this.$emit("input", v);
      }
    },
    sizeRules() {
      return [v => v !== null || "Game size field is mandatory"];
    }
  }
};
</script>

<style scoped></style>
