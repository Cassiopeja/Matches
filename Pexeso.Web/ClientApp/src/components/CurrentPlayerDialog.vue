<template>
  <v-dialog
      v-model="show"
      persistent
      max-width="600"
  >
    <v-card>
      <v-card-title class="headline">
        Current player
      </v-card-title>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-text-field
              v-model="player.name"
              :counter="128"
              label = "Player name"
              :rules="nameRules"
          >
          </v-text-field>
          <div class="caption">Avatar color</div>
          <v-color-picker
              class="ma-2"
              v-model="player.color"
              hide-inputs
              hide-canvas
          ></v-color-picker>
        </v-form>
      </v-card-text>
      <v-card-text>
        <div class="caption">Avatar look</div>
        <player-avatar v-bind:player="player"/>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
            color="green darken-1"
            text
            @click="show = false"
        >
          Cancel
        </v-btn>
        <v-btn
            color="green darken-1"
            text
            @click="onSave"
        >
          Save
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import {mapActions} from "vuex";
import {v4 as uuidv4} from 'uuid';
import PlayerAvatar from "@/components/PlayerAvatar";

export default {
  name: "CurrentPlayerDialog",
  components: {PlayerAvatar},
  props:{
    value: {
      default: false
    },
  },
  data(){
    return{
      valid: true,
      player: {
        name: '',
        color: {}
      },
      errors: []
    }
  },
  methods: {
    ...mapActions(['setCurrentPlayer']),
    async onSave() {
      this.errors = [];
      this.$refs.form.validate();
      if (this.valid)
      {
        this.player.id = uuidv4();
        this.setCurrentPlayer(this.player);
        this.show = false;
      }
    },
  },
  computed: {
    show: {
      get() {
        return this.value
      },
      set(v) {
        this.$emit('input', v)
      }
    },
    nameRules(){
      return [
        v => !!v || "Name field is mandatory",
        v => (v && v.length <= 128) || "Name field should not contain more that 128 symbols",
      ] 
    },
    showColor () {
      if (typeof this.player.color === 'string') return this.player.color

      return JSON.stringify(Object.keys(this.player.color).reduce((color, key) => {
        color[key] = Number(this.color[key].toFixed(2))
        return color
      }, {}), null, 2)
    }
  }
}
</script>

<style scoped>

</style>