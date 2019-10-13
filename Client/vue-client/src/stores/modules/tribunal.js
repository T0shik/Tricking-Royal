import Vue from "vue";
import Vuex from "vuex"
import axios from "axios";

Vue.use(Vuex);

export default {
    state: {
        evaluations: null,
        type: 'complete',
        loading: false,
        count: 0,
        loadingCount: false,
    },
    getters: {
        GET_EVALUATIONS: state => {
            return state.evaluations;
        },
        GET_TRIBUNAL_TYPE: state => {
            return state.type;
        },
        GET_TRIBUNAL_LOADING: state => {
            return state.loading;
        },
        GET_TRIBUNAL_COUNT: state => {
            return state.count;
        },
        GET_TRIBUNAL_LOADING_COUNT: state => {
            return state.loadingCount;
        }
    },
    mutations: {
        SET_TRIBUNAL_EVALUATIONS(state, payload) {
            state.evaluations = payload;
        },
        SET_TRIBUNAL_TYPE(state, payload) {
            state.type = payload;
        },
        SET_TRIBUNAL_LOADING(state, payload) {
            state.loading = payload;
        },
        SET_TRIBUNAL_COUNT(state, payload) {
            state.count = payload;
        },
        SET_TRIBUNAL_LOADING_COUNT(state, payload) {
            state.loadingCount = payload;
        }
    },
    actions: {
        LOAD_EVALUATIONS({ commit }, payload) {
            let { type } = payload;
            commit('SET_TRIBUNAL_LOADING', true)
            commit('SET_TRIBUNAL_TYPE', type)
            axios.get(`/evaluations?type=${type}`)
                .then(res => {
                    commit('SET_TRIBUNAL_EVALUATIONS', res.data)
                    commit('SET_TRIBUNAL_LOADING', false)
                })
                .catch(err => {
                    console.error("ERROR GETTING EVALUATIONS", err)
                })
        },
        REFRESH_EVALUATIONS({ commit, getters }) {
            commit('SET_TRIBUNAL_LOADING', true)
            axios.get(`/evaluations?type=${getters.GET_TRIBUNAL_TYPE}`)
                .then(({data}) => {
                    commit('SET_TRIBUNAL_EVALUATIONS', data);
                    commit('SET_TRIBUNAL_LOADING', false);
                })
                .catch(err => {
                    console.error("ERROR GETTING EVALUATIONS", err);
                })
        },
        LOAD_TRIBUNAL_COUNT({ commit }) {
            commit('SET_TRIBUNAL_LOADING_COUNT', true);
            axios.get('/evaluations/count')
                .then(res => {
                    commit('SET_TRIBUNAL_COUNT', res.data);
                    commit('SET_TRIBUNAL_LOADING_COUNT', false);
                })
        }
    }
}