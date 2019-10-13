<template>
    <div>
        <component v-bind:is="layout"></component>
    </div>
</template>

<script>
    import VisitorLayout from "./components/layout/VisitorLayout";
    import UserLayout from "./components/layout/UserLayout";
    import {mapGetters} from "vuex";
    import {NOTIFICATION_TYPE} from "./data/enum";

    export default {
        name: "App",
        created() {
            this.$store.dispatch("INIT")
                .then(({success, appId}) => {
                    if (!success) return;

// OneSignal instance is loaded in index.html
// https://cdn.onesignal.com/sdks/OneSignalSDK.js"
// at the moment this lives here because I couldn't figure out a better
// place to put it, and still have sane access to vuex & axios
// probably want to wrap this
// todo: move this fucker.
                    const OneSignal = window.OneSignal || [];

                    const createOneSignalEventListener = () => {
                        OneSignal.addListenerForNotificationOpened(({data}) => {
                            let {id, navigation, type} = data;
                            this.$store.dispatch('notifications/touchNotificationById', id);
                            this.$store.dispatch('matches/refreshMatches');
                            this.$store.dispatch('REFRESH_PROFILE');
                            this.open(navigation, type);
                            createOneSignalEventListener();
                        });
                    };
                    OneSignal.push(() => {
                        OneSignal.init({
                            appId,
                            notificationClickHandlerMatch: 'origin',
                            notificationClickHandlerAction: 'focus'
                        });
                        OneSignal.on('subscriptionChange', (subscribed) => {
                            OneSignal.getUserId().then(notificationId => {
                                this.$axios.post("/notifications", {
                                    notificationId,
                                    type: NOTIFICATION_TYPE.WEB,
                                    active: subscribed,
                                }).catch(err => {
                                    console.error(err);
                                })
                            });
                            this.$store.commit('notifications/setPushState', subscribed)
                        });
                        OneSignal.on('notificationDisplay', () => {
                            this.$store.dispatch('notifications/resetNotifications');
                        });
                        createOneSignalEventListener();
                    });

                    this.$store.dispatch('notifications/setPushState');
                });
        },
        methods: {
            open(navigation, type) {
                if (type === 'Empty') return;
                //todo: this needs to be cleaned up
                //todo: duplicate functionality in Navbar.vue -> open() ~137
                if (type === 'TribunalHistory') {
                    this.$router.push({
                        path: `/tribunal/history/${navigation[0]}`,
                    })
                }
                if (type === 'MatchActive') {
                    this.$router.push({
                        path: `/battles/active/${navigation[0]}`,
                    })
                } else if (type === 'MatchHistory') {
                    this.$router.push({
                        path: `/battles/history/${navigation[0]}`,
                    })
                } else if (type === 'Comment') {
                    this.$router.push({
                        path: `/watch/${navigation[0]}`,
                        query: {comment: navigation[1]}
                    })
                } else if (type === 'SubComment') {
                    this.$router.push({
                        path: `/watch/${navigation[0]}`,
                        query: {comment: navigation[1], subComment: navigation[2]}
                    })
                }
            }
        },
        components: {
            VisitorLayout,
            UserLayout
        },
        computed: {
            ...mapGetters({
                layout: "GET_LAYOUT"
            })
        }
    };
</script>

<style lang="scss">
    @import "./styles/main.scss";
</style>