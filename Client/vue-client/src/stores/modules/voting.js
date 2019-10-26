import Vue from "vue";
import Vuex from "vuex";
import axios from 'axios'

Vue.use(Vuex);

const initialState = () => ({
    evaluation: null,
    results: null,
    loading: false,
    loadingResults: false,
    target: -1
});

export default {
    namespaced: true,
    state: initialState(),
    mutations: {
        setEvaluation(state, payload) {
            state.evaluation = payload;
        },
        setResults(state, payload) {
            state.results = payload;
        },
        setLoading(state, payload) {
            state.loading = payload;
        },
        setTarget(state, payload) {
            if (!state.results) {
                state.target = payload;
            }
        },
        setLoadingResults(state, payload) {
            state.loadingResults = payload;
        },
        close(state) {
            Object.assign(state, initialState());
        }
    },
    actions: {
        vote({state, commit, dispatch}, {vote = null}) {
            commit('setLoading', true);
            if (vote === null) {
                vote = state.target;
            }
            axios.post(`/evaluations/${state.evaluation.id}`, {
                vote
            })
                .then(({data}) => {
                    dispatch('REFRESH_EVALUATIONS', null, {root: true});
                    dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                    commit('setLoading', false);
                    commit("setLoadingResults", true);
                    dispatch('loadResults', state.evaluation.id);
                })
        },
        loadResults({commit}, id) {
            axios.get(`/evaluations/${id}/results`)
                .then(({data}) => {
                    commit("setResults", data);
                    commit("setLoadingResults", false);
                });
        }
    }
}