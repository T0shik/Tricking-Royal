import Vue from "vue";
import Vuex from "vuex";
import axios from 'axios'
import {UPLOAD_STATUS} from "../../data/enum";

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
        uploadInitial({state, commit, dispatch}, formData) {
            commit("setUploadStatus", UPLOAD_STATUS.INITIAL_STARTED);
            let ext = state.videoUpdate ? `update` : `init`;

            axios.post(`${process.env.VUE_APP_CDN}/video/${ext}/${state.match.id}`,
                formData, MULTIPART_HEADER_OPTIONS).then(({data}) => {
                commit('setInitialVideoName', data.video);
                commit("setUploadStatus", UPLOAD_STATUS.INITIAL_FINISHED);
            }).catch(error => {
                console.error("Error Uploading Video", error);

                dispatch('DISPLAY_POPUP', {
                    message: "Error uploading video.",
                    type: 'error'
                }, {root: true});

                commit('reset');
            })
        },
        uploadTrimOptions({state, commit, dispatch}, {start, end}) {
            commit("setUploadStatus", UPLOAD_STATUS.TRIM_STARTED);

            axios.post(
                `${process.env.VUE_APP_CDN}/video/trim/${state.match.id}`,
                {
                    video: state.initialVideoName,
                    start: start,
                    end: end
                }).then(({data}) => {
                let {video, thumb} = data;
                commit('setTrimmingResult', {
                    video,
                    thumb
                });
                commit("setUploadStatus", UPLOAD_STATUS.TRIM_FINISHED);
            }).catch(error => {
                console.error("Error Editing Video", error);
                dispatch('DISPLAY_POPUP', {
                    message: "Error editing video.",
                    type: 'error'
                }, {root: true});

                commit('reset');
            })
        },
        updateMatch({state, commit, dispatch}, {move, index}) {
            let {video, thumb} = state.trimResult;
            let ext = state.videoUpdate ? "video" : "update";

            axios.put(`/matches/${state.match.id}/${ext}`, {
                video, thumb, move, index
            }).then(({data}) => {
                dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                dispatch('matches/refreshMatches', null, {root: true})
            }).catch(error => {
                console.error("Error Uploading Video", error);
                dispatch('DISPLAY_POPUP', {
                    message: "Error uploading video.",
                    type: 'error'
                }, {root: true});
            }).then(() => {
                commit('reset');
                dispatch('REFRESH_PROFILE', null, {root: true})
            })
        },
        lockIn({commit, dispatch}, {id}) {
            axios.post(`/matches/${id}/three-round-pass/ready`).then(({data}) => {
                dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                dispatch('matches/refreshMatches', null, {root: true})
            }).catch(error => {
                console.error("ERROR UPDATING MATCH", error)
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