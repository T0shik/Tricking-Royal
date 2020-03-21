import axios from "axios";

export default {
    state: {
        profile: null,
    },
    getters: {
        GET_PROFILE: state => {
            return state.profile;
        },
        HOST_LIMIT_REACHED: state => {
            return state.profile === null || state.profile.hosting >= state.profile.hostingLimit
        },
        JOIN_LIMIT_REACHED: state => {
            return state.profile === null || state.profile.joined >= state.profile.joinedLimit;
        }
    },
    mutations: {
        UPDATE_PROFILE(state, payload) {
            state.profile = payload
        },
        SET_PROFILE_LANGUAGE(state, {langauge}) {
            state.profile.language = langauge
        },
        CLEAR_PROFILE(state) {
            state.profile = null
        },
        UPDATE_PROFILE_IMAGE(state, payload) {
            state.profile.picture = payload;
        },
        INCREMENT_HOSTING_COUNT(state, payload) {
            state.profile.hosting += payload;
        }
    },
    actions: {
        UPDATE_PROFILE({commit}, payload) {
            commit('UPDATE_PROFILE', payload);
        },
        REFRESH_PROFILE({commit}) {
            axios.get('users/me')
                .then(res => {
                    commit('UPDATE_PROFILE', res.data);
                })
        },
        UPDATE_LANGUAGE({commit}, payload) {
            return axios.put('/users/language', payload)
                .then(res => {
                    if (res.data.success) {
                        commit('SET_PROFILE_LANGUAGE', payload)
                    }
                });
        }
    }
}