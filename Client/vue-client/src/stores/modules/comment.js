import Vue from "vue";
import Vuex from "vuex"
import axios from 'axios'

Vue.use(Vuex)

const getDefaultState = () => {
    return {
        targetId: -1,
        commenting: false,
        tag: "",
        loading: false
    }
}

const state = getDefaultState();

export default {
    state,
    getters: {
        GET_COMMENT_IS_COMMENTING: state => {
            return state.commenting
        },
        GET_COMMENT_TAG: state => {
            return state.tag
        },
        GET_COMMENT_TARGET_ID: state => {
            return state.targetId
        },
        GET_COMMENT_LOADING: state => {
            return state.loading
        },
        GET_NEW_COMMENT: state => {
            return state.newComment
        },
        GET_NEW_SUB_COMMENT: state => {
            return state.newSubComment
        }
    },
    mutations: {
        START_COMMENT(state, payload) {
            let {id, tag} = payload
            if (state.commenting && !(tag !== state.tag && tag.length > 0)) return;
            state.commenting = true
            state.targetId = id
            state.tag = tag
        },
        CLEAR_TAG(state) {
            state.tag = ""
        },
        FINISH_COMMENT(state) {
            Object.assign(state, getDefaultState())
        },
        SET_COMMENT_LOADING(state, payload) {
            state.loading = payload
        },
    },
    actions: {
        CREATE_COMMENT({commit, dispatch}, payload) {
            commit('SET_COMMENT_LOADING', true);
            let url = '/comments';

            if (payload.taggedUser) url += '/sub';

            return axios.post(url, payload).then(({data}) => {
                dispatch('DISPLAY_POPUP', {
                    message: 'Comment created.',
                    type: 'success',
                });

                return data;
            });
        }
    }
}