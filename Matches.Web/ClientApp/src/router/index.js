import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/about",
    name: "About",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/About.vue")
  },
  {
    path: "/createdGame/:id",
    name: "CreatedGameView",
    component: () => import("../views/CreatedGameView"),
    props: true
  },
  {
    path: "/game/:id",
    name: "GameView",
    component: () => import("../views/GameView"),
    props: true
  },
  {
    path: "/gameScores/:id",
    name: "GameScoreView",
    component: () => import("../views/GameScoresView"),
    props: true
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

export default router;
