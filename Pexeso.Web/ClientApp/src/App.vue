<template>
  <v-app>
    <Navbar />
    <v-main class="grey lighten-4">
      <router-view />
    </v-main>
    <notifications position="bottom right"/>
  </v-app>
</template>

<script>
import Navbar from "@/components/Navbar";

export default {
  name: 'App',
  components: {Navbar},
};
</script>

<style lang="scss">
a {
  text-decoration: none;
}
</style>
