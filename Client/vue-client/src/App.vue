<template>
    <div>
        <component v-bind:is="layout"/>
    </div>
</template>

<script>
    import VisitorLayout from "./components/layout/VisitorLayout";
    import UserLayout from "./components/layout/UserLayout";
    import LoadingLayout from "./components/layout/LoadingLayout";
    import {mapState} from "vuex";
    import {NOTIFICATION_TYPE, STORAGE_KEYS} from "./data/enum";
    import {loadLanguageAsync} from "./plugins/i18n";

    export default {
        name: "App",
        created() {
            this.$store.dispatch("INIT")
                .then(({success, activated, appId, safariId}) => {
                    if (!success) return;

// OneSignal instance is loaded in index.html
// https://cdn.onesignal.com/sdks/OneSignalSDK.js"
// at the moment this lives here because I couldn't figure out a better
// place to put it, and still have sane access to vuex & axios
// probably want to wrap this
// todo: move this fucker.
                    this.$logger.log("[(App.vue).created] starting  OneSignal init.");
                    const OneSignal = window.OneSignal || [];

                    const createOneSignalEventListener = () => {
                        OneSignal.addListenerForNotificationOpened(({data}) => {
                            let {id, navigation, type} = data;
                            this.$store.dispatch('notifications/touchNotificationById', id);
                            this.$store.dispatch('matches/refreshMatches', {});
                            this.$store.dispatch('REFRESH_PROFILE');
                            this.open(navigation, type);
                            createOneSignalEventListener();
                        });
                    };
                    OneSignal.push(() => {
                        OneSignal.init({
                            appId,
                            safari_web_id: safariId,
                            notificationClickHandlerMatch: 'origin',
                            notificationClickHandlerAction: 'focus'
                        });
                        OneSignal.on('subscriptionChange', (subscribed) => {
                            this.$logger.log("[(App.vue).created] subscribed? =>", subscribed);
                            OneSignal.getUserId().then(notificationId => {
                                this.$axios.post("/notifications", {
                                    notificationId,
                                    type: NOTIFICATION_TYPE.WEB,
                                    active: subscribed,
                                }).catch(err => {
                                    this.$logger.error(err);
                                })
                            });
                        });
                        OneSignal.on('notificationDisplay', () => {
                            this.$store.dispatch('notifications/resetNotifications');
                        });
                        createOneSignalEventListener();

                        if (activated) {
                            this.$store.dispatch('confirmation/notificationsPrompt', {}, {root: true});
                        }
                    });
                    this.$logger.log("[(App.vue).created] finished OneSignal init.");
                });

            let lang = localStorage.getItem(STORAGE_KEYS.LANGUAGE);
            if (lang === null) {
                localStorage.setItem(STORAGE_KEYS.LANGUAGE, 'en');
            } else {
                loadLanguageAsync(lang);
            }
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
            UserLayout,
            LoadingLayout,
        },
        computed: mapState('layout', ['layout'])
    };
</script>

<style lang="scss">
    @import "./styles/main.scss";
</style>