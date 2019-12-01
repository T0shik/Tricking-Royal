const getDefaultState = () => ({
    display: false,
    message: ""
});

export default {
    namespaced: true,
    state: getDefaultState(),
    mutations: {
        setStatus(state, {message}) {
            state.display = true;
            state.message = message;
        },
        reset(state){
            Object.assign(state, getDefaultState())
        }
    }
}