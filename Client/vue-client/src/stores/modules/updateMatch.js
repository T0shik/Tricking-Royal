import Vue from "vue";
import Vuex from "vuex";
import axios from 'axios'
import {UPLOAD_STATUS} from "../../data/enum";
import Logger from "../../logger/logger";

Vue.use(Vuex);

const MULTIPART_HEADER_OPTIONS = {
    headers: {
        "Content-Type": "multipart/form-data"
    }
};

const getDefaultState = () => {
    return {
        display: false,
        match: null,
        uploadStatus: UPLOAD_STATUS.NOT_STARTED,
        initialVideoName: '',
        trimResult: null,
        videoUpdate: false,
    }
};

export default {
    namespaced: true,
    state: getDefaultState(),
    getters: {
        loading: state => {
            return state.uploadStatus > 0
        },
    },
    mutations: {
        setMatch(state, {match, update}) {
            state.display = true;
            state.match = match;
            state.videoUpdate = update
        },
        setUploadStatus(state, status) {
            state.uploadStatus = status;
        },
        setInitialVideoName(state, name) {
            state.initialVideoName = name
        },
        setTrimmingResult(state, {video, thumb}) {
            state.trimResult = {
                video, thumb
            };
        },
        hide(state) {
            state.display = false;
        },
        reset(state) {
            Object.assign(state, getDefaultState())
        },
    },
    actions: {
        uploadVideo({state, commit, dispatch}, formData) {
            commit("setUploadStatus", UPLOAD_STATUS.INITIAL_STARTED);
            let uploadTask = state.videoUpdate ? `update` : `upload`;

            axios.post(`${process.env.VUE_APP_CDN}/video/${state.match.id}/${uploadTask}`,
                formData, MULTIPART_HEADER_OPTIONS).then(({data}) => {
                commit('setInitialVideoName', data);
                commit("setUploadStatus", UPLOAD_STATUS.INITIAL_FINISHED);
            }).catch(error => {
                Logger.error("Error Uploading Video", error);

                dispatch('DISPLAY_POPUP', {
                    message: "Error uploading video.",
                    type: 'error'
                }, {root: true});

                commit('reset');
            })
        },
        startUpdate({state, commit, dispatch}, {start, end, move, index}) {
            let data = {
                video: state.initialVideoName,
                start,
                end,
                move,
                index,
                videoUpdate: state.videoUpdate
            };

            axios.put(`/matches/${state.match.id}/update`, data).then(({data}) => {
                dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                dispatch('matches/refreshMatches', {}, {root: true})
            }).catch(error => {
                Logger.error("Error Uploading Video", error);
                dispatch('DISPLAY_POPUP', {
                    message: "Error uploading video.",
                    type: 'error'
                }, {root: true});
            })
                .then(() => {
                    commit('reset');
                    dispatch('REFRESH_PROFILE', null, {root: true})
                })
        },
        lockIn({commit, dispatch}, {id}) {
            axios.post(`/matches/${id}/three-round-pass/ready`).then(({data}) => {
                dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                dispatch('matches/refreshMatches', {}, {root: true})
            }).catch(error => {
                Logger.error("ERROR UPDATING MATCH", error)
            }).then(() => {
                commit('reset');
                dispatch('REFRESH_PROFILE', null, {root: true});
            })
        },
        like({dispatch}, match) {
            if (match.canLike) {
                axios.post(`/likes/${match.id}`).then(({data}) => {
                    dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});

                    if (data.success) {
                        match.canLike = false;
                    }
                });
            }
        }
    }
}