import Vue from "vue";
import Vuex from "vuex"
import axios from 'axios'

Vue.use(Vuex);

export default {
    state: {
        user: null,
        history: [],
        active: [],
        open: [],
        loadingMatches: false
    },
    getters: {
        GET_USER: state => {
            return state.user;
        },
        GET_USER_HISTORY: state => {
            return state.history
        },
        GET_USER_ACTIVE: state => {
            return state.active
        },
        GET_USER_OPEN: state => {
            return state.open
        },
        GET_USER_LOADING_M: state => {
            return state.loadingMatches
        }
    },
    mutations: {
        UPDATE_USER(state, payload) {
            state.user = payload
        },
        UPDATE_USER_MATCHES(state, payload) {
            let { matches, type } = payload;
            if (type === 'history') {
                state.history = matches
            }
            else if (type === 'active') {
                state.active = matches
            }
            else if (type === 'open') {
                state.open = matches
            }
        },
        CLEAR_USER(state) {
            state.user = null;
        },
        CLEAR_USER_MATCHES(state) {
            state.history = [];
            state.active = [];
            state.open = [];
        }
    },
    actions: {
        SET_USER({ commit }, payload) {
            const {router, displayName, redirect} = payload;
            axios.get(`/users/${displayName}`)
                .then(res => {
                    const {data, status} = res;
                    if (status === 200) {
                        commit('CLEAR_USER_MATCHES');
                        commit('UPDATE_USER', data);
                        if (redirect) {
                            router.push(`/user/${displayName}`)
                        }
                    }
                    else if (status === 204) {
                        commit('UPDATE_USER', {})
                    }
                })
                .catch(err => {
                    console.log("TODO: Handle getting user error", err)
                })
        },
        LOAD_USER_MATCHSE({ commit, getters }, payload) {
            let { type } = payload;
            let { displayName } = getters.GET_USER;
            axios.get(`/matches?filter=${type}&displayName=${displayName}`)
                .then(res => {
                    commit('UPDATE_USER_MATCHES', { matches: res.data, type: type })
                })
                .catch(err => {
                    console.log("TODO: Handle getting user error", err)
                })
        }
    }
}