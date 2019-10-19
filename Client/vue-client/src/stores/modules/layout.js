import {LAYOUT} from "../../data/enum";

export default {
    namespaced: true,
    state: {
        layout: LAYOUT.LOADING,
    },
    mutations: {
        setLayout(state, payload) {
            state.layout = payload
        }
    }
}