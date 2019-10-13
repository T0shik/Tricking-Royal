import Vue from "vue";
import Vuex from "vuex"

Vue.use(Vuex);

const initialState = () => ({
    match: null,
});

export default {
    namespaced: true,
    state: initialState(),
    getters: {
        open: state => state.match !== null
    },
    mutations: {
        openMenu(state, match) {
            state.match = match;
        },
        closeMenu(state) {
            Object.assign(state, initialState());
        }
    }
}