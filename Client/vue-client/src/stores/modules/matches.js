import Vue from "vue";
import Vuex from "vuex"
import router from "../../router";
import {MATCH_TYPES} from "../../data/enum";
import {deleteMatch, getMatches, joinMatch} from "../../data/api";

Vue.use(Vuex);

const createMatchContainer = (list) => ({
    list,
    index: 0,
    endReached: false,
    stale: false,
    timestamp: new Date(),
});

const initialState = () => ({
    hosted: createMatchContainer([]),
    open: createMatchContainer([]),
    history: createMatchContainer([]),
    active: createMatchContainer([]),
    spectate: createMatchContainer([]),
    loading: false,
    loadingMore: false,
    loadingJoin: false,
    type: MATCH_TYPES.NONE,
});

const getters = {
    endReached: state => state.type === MATCH_TYPES.ACTIVE || (state[state.type] && state[state.type].endReached),
    matches: state => state[state.type] && state[state.type].list,
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
        toggleLoading(state, loader) {
            state[loader] = !state[loader];
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
                dispatch('loadMatches', type);
            }
        },
        loadMatches({commit}, type) {
            commit('toggleLoading', 'loading');
            getMatches(type, 0)
                .then(({data}) => {
                    commit('setMatches', {
                        matches: data,
                        type: type
                    })
                })
                .catch(error => {
                    console.error("ERROR GETTING MATCHES", error)
                })
                .then(() => {
                    commit('toggleLoading', 'loading');
                })
        },
        loadMoreMatches({getters, commit}) {
            commit('toggleLoading', 'loadingMore');
            let {type, index} = getters.typeAndIndex;

            getMatches(type, index)
                .then(({data}) => {
                    commit('addMatches', data)
                })
                .catch(error => {
                    console.error("ERROR GETTING MATCHES", error)
                })
                .then(() => {
                    commit('toggleLoading', 'loadingMore');
                })
        },
        refreshMatches({state, commit, dispatch}, type) {
            commit("setStale");
            dispatch("loadMatches", type === null || type === undefined ? state.type : type);
        },
        deleteMatch({dispatch, commit}, matchId) {
            commit('toggleLoading', 'loading');

            deleteMatch(matchId).then(({data}) => {
                dispatch("DISPLAY_POPUP_DEFAULT", data, {root: true});

                if (data.success) {
                    commit("INCREMENT_HOSTING_COUNT", -1, {root: true});
                    commit('toggleLoading', 'loading');
                    dispatch("refreshMatches", MATCH_TYPES.HOSTED);
                    dispatch("refreshMatches", MATCH_TYPES.OPEN);
                }
            });
        },
        joinMatch({dispatch, commit}, id) {
            commit('toggleLoading', 'loadingJoin');

            joinMatch(id).then(({data}) => {
                dispatch("DISPLAY_POPUP_DEFAULT", data, {root: true});
                if (data.success) {
                    router.push({path: '/battles/active'});
                }
                commit('toggleLoading', 'loadingJoin');
            });
        },
    }
}