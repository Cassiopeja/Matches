<template>
  <v-avatar
      :color="player.color ? player.color.hex : '#eee'"
      :size="size"
  >
    <span class="white--text text-h5">{{initials}}</span>
  </v-avatar>
</template>

<script>
export default {
  name: "PlayerAvatar",
  props: {
    player: {
      required: true
    },
    size: {
      required: false,
      value: 48
    }
  },
  computed:{
    initials() {
      return this.player.name.split(' ').map(w => w[0]).join('').toUpperCase();
    }
  }
}
</script>

<style scoped>

</style>