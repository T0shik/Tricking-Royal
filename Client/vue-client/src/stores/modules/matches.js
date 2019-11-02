import Vue from "vue";
import Vuex from "vuex"
import router from "../../router";
import {MATCH_TYPES} from "../../data/enum";
import {deleteMatch, getMatches, joinMatch} from "../../data/api";
import Logger from "../../logger/logger";

Vue.use(Vuex);

const createMatchContainer = (list) => ({
    list,
    index: 0,
    endReached: false,
    stale: false,
    timestamp: new Date(),
    loading: false
});

const initialState = () => ({
    hosted: createMatchContainer([]),
    open: createMatchContainer([]),
    history: createMatchContainer([]),
    active: createMatchContainer([]),
    spectate: createMatchContainer([]),
    loadingDelete: false,
    loadingMore: false,
    loadingJoin: false,
    type: MATCH_TYPES.NONE,
});

const getters = {
    endReached: state => state.type === MATCH_TYPES.ACTIVE || (state[state.type] && state[state.type].endReached),
    matches: state => state[state.type] && state[state.type].list,
    loading: state => state[state.type] && state[state.type].loading,
    stale: state => state[state.type] && state[state.type].stale,
    old: state => state[state.type] && (new Date() - state[state.type].timestamp) > 60000,
    typeAndIndex: state => ({
        type: state.type,
        index: state[state.type] && state[state.type].index,
    })
};

export default {
    namespaced: true,
    state: initialState(),
    getters,
    mutations: {
        setMatches(state, {matches, type}) {
            let container = createMatchContainer(matches);
            container.endReached = matches.length === 0;
            container.index++;
            state[type] = container;
        },
        addMatches(state, matches) {
            if (matches.length > 0) {
                state[state.type].list = state[state.type].list.concat(matches);
                state[state.type].index++;
            } else {
                state[state.type].endReached = true;
            }
        },
        setMatchLoader(state, {type, value}) {
            state[type].loading = value;
        },
        setLoading(state, {loader, value}) {
            Logger.log(`toggling loader ${loader}, current value: ${state[loader]}, new state: ${value}`);
            state[loader] = value;
        },
        saveType(state, type) {
            state.type = type;
        },
        clearMatches(state, type) {
            state[type] = createMatchContainer([]);
        },
        setStale(state) {
            state[MATCH_TYPES.HISTORY].stale = true;
            state[MATCH_TYPES.ACTIVE].stale = true;
            state[MATCH_TYPES.SPECTATE].stale = true;
            state[MATCH_TYPES.HOSTED].stale = true;
            state[MATCH_TYPES.OPEN].stale = true;
        }
    },
    actions: {
        setType({getters, commit, dispatch}, {type, force}) {
            commit('saveType', type);

            if (force || getters.stale || getters.old || getters.matches.length === 0) {
                dispatch('loadMatches', {type});
            }
        },
        loadMatches({state, commit}, {type}) {
            commit('setMatchLoader', {type, value: true});

            getMatches(type, 0)
                .then(({data}) => {
                    commit('setMatches', {
                        matches: data,
                        type: type
                    })
                })
                .catch(error => {
                    Logger.error("ERROR GETTING MATCHES", error)
                })
                .then(() => {
                    commit('setMatchLoader', {type, value: false});
                })
        },
        loadMoreMatches({getters, commit}) {
            commit('setLoading', {loader: 'loadingMore', value: true});
            let {type, index} = getters.typeAndIndex;

            getMatches(type, index)
                .then(({data}) => {
                    commit('addMatches', data)
                })
                .catch(error => {
                    Logger.error("ERROR GETTING MATCHES", error)
                })
                .then(() => {
                    commit('setLoading', {loader: 'loadingMore', value: false});
                })
        },
        refreshMatches({state, commit, dispatch}, {type}) {
            commit("setStale");
            let payload = {
                type: type === null || type === undefined ? state.type : type,
            };

            dispatch("loadMatches", payload);
        },
        deleteMatch({dispatch, commit}, {type, matchId}) {
            commit('setLoading', {loader: 'loadingDelete', value: true});

            deleteMatch(matchId).then(({data}) => {
                dispatch("DISPLAY_POPUP_DEFAULT", data, {root: true});

                if (data.success) {
                    dispatch("REFRESH_PROFILE", {}, {root: true});
                    dispatch("refreshMatches", {type});
                    commit('setLoading', {loader: 'loadingDelete', value: false});
                }
            });
        },
        joinMatch({dispatch, commit}, id) {
            commit('setLoading', {loader: 'loadingJoin', value: false});

            joinMatch(id).then(({data}) => {
                dispatch("DISPLAY_POPUP_DEFAULT", data, {root: true});
                commit('setStale');
                if (data.success) {
                    router.push({path: '/battles/active'});
                }
                commit('setLoading', {loader: 'loadingJoin', value: false});
            });
        },
    }
}