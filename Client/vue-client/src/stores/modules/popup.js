import Vue from "vue";
import Vuex from "vuex"

Vue.use(Vuex);

const initialState = () => ({
    message: "",
    progress: false,
    display: false,
    type: "",
});

const showPopup = ({state, commit}, action) =>{
    if (state.display) {
        commit('HIDE_POPUP');
        setTimeout(function () {
            action(commit);
        }, 100);
    } else {
        action(commit);
    }
};

export default {
    state: initialState(),
    mutations: {
        SET_POPUP(state, payload) {
            let {message, type, progress} = payload;
            state.message = message;
            state.type = type;
            state.progress = progress;
            state.display = true
        },
        HIDE_POPUP(state) {
            Object.assign(state, initialState());
        },
    },
    actions: {
        DISPLAY_POPUP(context, payload) {
            let createPopUp = (commit) => commit('SET_POPUP', payload);

            showPopup(context, createPopUp);
        },
        DISPLAY_POPUP_DEFAULT(context, {message, success}) {
            let createPopUp = (commit) => commit('SET_POPUP', {
                message,
                type: success ? 'success' : 'error',
            });
            
            showPopup(context, createPopUp);
        },
    }
}