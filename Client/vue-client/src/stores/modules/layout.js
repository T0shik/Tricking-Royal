import Vue from "vue";
import Vuex from "vuex"

Vue.use(Vuex)
//---Layouts
//visitor-layout
//user-layout
export default {
    state: {
        layout: 'visitor-layout',
        navbar: false
    },
    getters: {
        GET_LAYOUT: state => {
            return state.layout;
        },
        HAS_NAVBAR: state => {
            return state.navbar
        }
    },
    mutations: {
        SET_LAYOUT(state, payload) {
            state.navbar = payload === 'user-layout'
            state.layout = payload
        }
    }
}