import GameHub from "./gameHub"

export default {
    install(Vue) {
        GameHub.start();
        Vue.prototype.$gameHub = GameHub;
    }
}