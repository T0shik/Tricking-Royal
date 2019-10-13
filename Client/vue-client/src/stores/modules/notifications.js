import axios from "axios";

const initialState = () => ({
    open: false,
    notifications: [],
    index: 0,
    empty: false,
    pushEnabled: false,
});

export default {
    namespaced: true,
    state: initialState(),
    mutations: {
        toggleNotifications(state) {
            state.open = !state.open;
        },
        addNotifications(state, {notifications, index}) {
            if (notifications.length > 0) {
                state.notifications = state.notifications.concat(notifications);
                state.index = index + 1;
            } else {
                state.empty = true;
            }
        },
        touchNotification(state, notification) {
            const index = state.notifications.indexOf(notification);
            state.notifications[index].new = false;
        },
        touchNotificationById(state, id) {
            for (let i = 0; i < state.notifications.length; i++) {
                if (state.notifications[i].id === id) {
                    state.notifications[i].new = false;
                }
            }
        },
        clearNotifications(state) {
            state.notifications.forEach(x => x.new = false);
            state.open = false;
        },
        setPushState(state, pushState) {
            state.pushEnabled = pushState;
        },
        resetNotifications(state) {
            Object.assign(state, initialState());
        }
    },
    actions: {
        getNotifications({state, commit}) {
            axios.get(`/notifications?index=${state.index}`)
                .then(({data}) => {
                    commit('addNotifications', {
                        notifications: data,
                        index: state.index
                    });
                })
        },
        async setPushState({commit}) {
            // eslint-disable-next-line
            let pushState = await OneSignal.isPushNotificationsEnabled();
            commit('setPushState', pushState);
        },
        resetNotifications({commit, dispatch}) {
            commit('resetNotifications');
            dispatch('setPushState');
            dispatch('getNotifications');
        },
        touchNotification({commit}, notification) {
            if (notification.new) {
                commit('touchNotification', notification);
                axios.put(`/notifications/${notification.id}/touch`, null);
            }
        },
        touchNotificationById({commit}, id) {
            commit('touchNotificationById', id);
            axios.put(`/notifications/${id}/touch`, null);
        },
        clearNotifications({commit, dispatch}) {
            commit('clearNotifications');
            axios.put('/notifications/clear-all', null)
                .then(({data}) => {
                    dispatch('DISPLAY_POPUP_DEFAULT', data, {root: true});
                });
        },
    }
}