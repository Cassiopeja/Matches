import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify'
import router from './router'
import Axios from 'axios';

Vue.prototype.$http = Axios;
Vue.config.productionTip = false

new Vue({
  vuetify,
  router,
  render: h => h(App),
  created() {
    this.$http.interceptors.request.use(config => {
      config.baseURL = `${process.env.VUE_APP_BACKEND_API}/api`;
      return config;
    });
  }

}).$mount('#app')
