import Vue from 'vue'
import App from './App.vue'
import router from './router'
import {store} from './stores/store'
import vuetify from './plugins/vuetify';
import {i18n} from "./plugins/i18n";
import FlagIcon from 'vue-flag-icon';
import Axios from 'axios'
import Logger from "./logger/logger";
import "./registerServiceWorker"


import {loadProgressBar} from 'axios-progress-bar'
import 'axios-progress-bar/dist/nprogress.css'

loadProgressBar();

const axiosPlugin = {
    install(Vue) {
        Vue.prototype.$axios = Axios;
    }
};

const loggerPlugin = {
    install(Vue) {
        Vue.prototype.$logger = Logger;
    }
};

Vue.use(FlagIcon);
Vue.use(axiosPlugin);
Vue.use(loggerPlugin);

Vue.config.productionTip = true;

new Vue({
    router,
    store,
    vuetify,
    i18n,
    render: h => h(App)
}).$mount('#app');
