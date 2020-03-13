import Vue from "vue";
import Vuex from "vuex"
import axios from "axios";
import {STORAGE_KEYS} from "../../data/enum";
import Logger from "../../logger/logger";

Vue.use(Vuex);

const initialState = ({type = '', descriptions = 0, action = null, display = true}) => ({
    display,
    type,
    descriptions,
    loading: false,
    action,
});

export default {
    namespaced: true,
    state: initialState({display: false}),
    mutations: {
        set(state, payload) {
            Object.assign(state, payload);
        },
        dismiss(state) {
            Object.assign(state, initialState({display: false}));
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
        pwaRefresh({commit}) {
            let lastPrompt = localStorage.getItem(STORAGE_KEYS.UPDATE_PROMPT);
            let now = new Date();
            //6 days cooldown
            if (now - lastPrompt > 518400000) {
                commit('set', initialState({
                    type: 'pwa',
                    descriptions: 1,
                    action: () => {
                        window.location.reload(true);
                    }
                }));
                localStorage.setItem(STORAGE_KEYS.UPDATE_PROMPT, now);
            }
        },
        copyCatPass({commit, dispatch}, {id}) {
            commit('set', initialState({
                type: 'pass',
                descriptions: 2,
                action: () => {
                    return axios.post(`/matches/${id}/pass`)
                        .then(({data}) => {
                            dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                            dispatch('matches/refreshMatches', {}, {root: true});
                        });
                }
            }))
        },
        notificationsPrompt({commit, dispatch}) {
            dispatch('notifications/getPushState', {}, {root: true})
                .then(enabled => {
                    if (enabled) {
                        Logger.log("[confirmation.notificationsPrompt] notifications enabled, skipping prompt");
                        return;
                    }

                    let lastPrompt = localStorage.getItem(STORAGE_KEYS.NOTIFICATION_PROMPT);
                    let now = new Date();
                    //3 days cooldown
                    if (now - lastPrompt > 259200000) {
                        Logger.log("[confirmation.notificationsPrompt] displaying notification prompt");
                        commit('set', initialState({
                            type: 'notify',
                            descriptions: 1,
                            action: () => {
                                return dispatch('notifications/showPrompt', {}, {root: true});
                            }
                        }));
                        localStorage.setItem(STORAGE_KEYS.NOTIFICATION_PROMPT, now);
                    } else {
                        Logger.log("[confirmation.notificationsPrompt] displayPrompt on cooldown.");
                    }
                });
        },
    }
}