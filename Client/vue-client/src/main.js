import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify';
import router from './router'
import {store} from './stores/store'
import "./components/shared/index"
import Axios from 'axios'

import {loadProgressBar} from 'axios-progress-bar'
import 'axios-progress-bar/dist/nprogress.css'
import Logger from "./logger/logger";

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

Vue.use(axiosPlugin);
Vue.use(loggerPlugin);

Vue.config.productionTip = true;

new Vue({
    router,
    store,
    vuetify,
    render: h => h(App)
}).$mount('#app');
