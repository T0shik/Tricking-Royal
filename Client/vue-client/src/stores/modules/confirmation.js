import Vue from "vue";
import Vuex from "vuex"

Vue.use(Vuex);

const initialState = () => ({
    title: '',
    description: '',
    loading: false,
    action: null,
});

export default {
    namespaced: true,
    state: initialState(),
    getters: {
        display: state => {
            return state.title !== '';
        }
    },
    mutations: {
        set(state, payload) {
            Object.assign(state, payload);
        },
        dismiss(state) {
            Object.assign(state, initialState());
        },
        toggleLoading(state) {
            state.loading = !state.loading;
        }
    },
    actions: {
        confirm({state, commit}) {
            commit("toggleLoading");
            state.action().then(() => {
                commit("toggleLoading");
                commit('dismiss');
            });
        },
    }
}
