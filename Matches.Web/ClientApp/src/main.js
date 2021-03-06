import Vue from "vue";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import router from "./router";
import Axios from "axios";
import gameHubPlugin from "@/plugins/gameHubPlugin";
import Notification from "vue-notification";
import store from "./store";
import VueConfetti from "vue-confetti";

Vue.use(VueConfetti);
Vue.use(gameHubPlugin);
Vue.use(Notification);
Vue.prototype.$http = Axios;
Vue.config.productionTip = false;

new Vue({
  vuetify,
  router,
  render: h => h(App),
  store,

  created() {
    this.$http.interceptors.request.use(config => {
      config.baseURL = `/api`;
      return config;
    });
  }
}).$mount("#app");
