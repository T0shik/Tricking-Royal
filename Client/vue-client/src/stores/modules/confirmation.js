import Vue from "vue";
import Vuex from "vuex"
import axios from "axios";
import {STORAGE_KEYS} from "../../data/enum";
import Logger from "../../logger/logger";

Vue.use(Vuex);

const initialState = ({
                          title = '',
                          description = '',
                          buttonText = '',
                          action = null
                      }) => ({
    title,
    description,
    buttonText,
    loading: false,
    action,
});

export default {
    namespaced: true,
    state: initialState({}),
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
            Logger.log("[confirmation.dismiss] dismissing.");
            Object.assign(state, initialState({}));
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
                    title: 'Update',
                    description: 'New version of the app is available, refresh to update.',
                    buttonText: "Refresh",
                    action: () => {
                        window.location.reload(true);
                    }
                }));
                localStorage.setItem(STORAGE_KEYS.UPDATE_PROMPT, now);
            }
        },
        copyCatPass({commit, dispatch}, {id}) {
            commit('set', initialState({
                title: 'Pass?',
                description: [
                    "Are you sure you want to pass this round?",
                    "Pass this round if you can't repeat the combo."
                ],
                buttonText: "Pass",
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
                            title: 'Allow Notifications?',
                            description: 'Let TrickingRoyal send you notifications when your matches are updated.',
                            buttonText: "Turn On",
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