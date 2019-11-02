import Vue from 'vue'
import Vuex from 'vuex'
import Oidc from 'oidc-client'
import axios from "axios";
import router from "../router";

import layout from "./modules/layout";
import menu from "./modules/menu";
import profile from "./modules/profile";
import user from "./modules/user";
import matches from "./modules/matches";
import tribunal from "./modules/tribunal";
import updateMatch from "./modules/updateMatch";
import notifications from "./modules/notifications";
import popup from "./modules/popup";
import confirmation from "./modules/confirmation";
import comment from "./modules/comment";
import voting from "./modules/voting";
import {LAYOUT} from "../data/enum";
import Logger from "../logger/logger";

axios.defaults.baseURL = process.env.VUE_APP_API;

axios.interceptors.response.use(
    response => response,
    error => {
        const {config, response: {status}} = error;
        Logger.log(error, config, status);

        if (status === 401) {
            if (!store.getters.REFRESHING_TOKEN) {
                store.commit('UPDATE_TOKEN_ACTIVITY', true);
                Logger.log("starting silent refresh on startup");
                return store.state.userMgr.signinSilent().then((user) => {
                    if (user) {
                        Logger.log("silent refresh complete, success refreshing token.");

                        let bearerToken = `bearer ${user.access_token}`;
                        axios.defaults.headers.common['Authorization'] = bearerToken;
                        config.headers['Authorization'] = bearerToken;
                        return axios.request(config);
                    } else {
                        Logger.log("silent refresh complete, failed signing out.");
                        store.commit("SIGN_OUT");
                        store.commit('layout/setLayout', LAYOUT.LOADING, {root: true});

                        router.push('landing');
                        Logger.log("do we reach this ting or we are redirected back to auth.");
                    }
                })
                    .catch(e => {
                        Logger.log("Failed to re-authenticate.", e);
                        if (e.error === 'login_required') {
                            store.state.userMgr.signinRedirect();
                        }
                    })
                    .then(result => {
                        store.commit('UPDATE_TOKEN_ACTIVITY', false);
                        return result;
                    });
            }
        }

        store.dispatch("DISPLAY_POPUP", {
            message: "Server Failed",
            type: "error"
        });
        return Promise.reject(error);
    });

Vue.use(Vuex);

const config = {
    userStore: new Oidc.WebStorageStateStore({store: window.localStorage}),
    authority: process.env.VUE_APP_AUTHORITY,
    client_id: "vue",
    redirect_uri: process.env.VUE_APP_REDIRECT,
    silent_redirect_uri: process.env.VUE_APP_SILENT_REDIRECT,
    response_type: "id_token token",
    scope: "openid profile TrickingRoyal.Api TrickingRoyal.Cdn",
    post_logout_redirect_uri: process.env.VUE_APP_POST_LOGOUT,
    automaticSilentRenew: false
};

export const store = new Vuex.Store({
    state: {
        appReady: false,
        userMgr: new Oidc.UserManager(config),
        isAuthenticated: false,
        refreshingToken: false,
        displayRules: false,
    },
    getters: {
        AUTHENTICATED: state => {
            return state.isAuthenticated;
        },
        REFRESHING_TOKEN: state => {
            return state.refreshingToken;
        }
    },
    mutations: {
        COMPLETE_INIT(state, payload) {
            state.isAuthenticated = payload;
            state.appReady = true;
        },
        SIGN_OUT: state => {
            state.isAuthenticated = false;
        },
        UPDATE_TOKEN_ACTIVITY(state, payload) {
            state.refreshingToken = payload;
        },
        SET_DISPLAY_RULES(state, {display}) {
            state.displayRules = display;
        }
    },
    actions: {
        INIT({commit, dispatch}) {
            return this.state.userMgr.getUser()
                .then(user => {
                    if (user) {
                        axios.defaults.headers.common['Authorization'] = `bearer ${user.access_token}`;
                        return true;
                    } else {
                        return false;
                    }
                })
                .then(async success => {
                    let result = {
                        success,
                        appId: '',
                        safariId: '',
                        activated: false,
                    };
                    if (success) {
                        const {data: profile} = await axios.get('users/me');
                        commit('UPDATE_PROFILE', profile);
                        result.activated = profile.activated;
                        commit('layout/setLayout', result.activated ? LAYOUT.USER : LAYOUT.VISITOR, {root: true});
                        dispatch('LOAD_TRIBUNAL_COUNT');
                        dispatch('notifications/getNotifications');
                        const {data: {appId, safariId}} = await axios.get('platform/one-signal');
                        result.appId = appId;
                        result.safariId = safariId;
                    } else {
                        commit('layout/setLayout', LAYOUT.VISITOR, {root: true});
                    }

                    commit('COMPLETE_INIT', success);
                    return result;
                })
        },
        SIGN_OUT(context) {
            this.state.userMgr.signoutRedirect();
            context.commit("SIGN_OUT");
            context.commit('layout/setLayout', LAYOUT.LOADING, {root: true});
        }
    },
    modules: {
        notifications,
        menu,
        confirmation,
        voting,
        matches,
        updateMatch,
        layout,

        //todo namespace these
        profile,
        popup,
        user,
        tribunal,
        comment
    }
});
