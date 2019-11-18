<template>
    <v-app-bar app clipped-left color="secondary">
        <router-link class="d-flex align-center" to="/battles">
            <v-avatar size="48">
                <img src="https://cdn.trickingroyal.com/static/logo_final.png"/>
            </v-avatar>
            <span
                    class="headline font-rock font-weight-black ml-1 primary--text hidden-sm-and-down"
            >Tricking Royal</span>
        </router-link>
        <v-spacer></v-spacer>
        <v-autocomplete
                class="px-2"
                dark
                :search-input.sync="search"
                :items="users"
                :loading="searching"
                label="Search for other trickers"
                item-avatar="picture"
                item-text="displayName"
                item-value="displayName"
                hide-details
                hide-no-data
                clearable
                @change="selectUser"
        >
            <template v-slot:item="data">
                <template>
                    <v-list-item-avatar>
                        <ProfileImage :picture="data.item.picture"></ProfileImage>
                    </v-list-item-avatar>
                    <v-list-item-content>
                        <v-list-item-title v-html="data.item.displayName"></v-list-item-title>
                    </v-list-item-content>
                </template>
            </template>
        </v-autocomplete>
        <v-spacer></v-spacer>
        <v-menu
                :close-on-content-click="false"
                :value="notificationsMenu"
                :nudge-width="200"
                offset-x
        >
            <template v-slot:activator="{ on }">
                <v-badge overlap left color="red">
                    <template v-slot:badge v-if="notificationCount > 0">{{notificationCount}}</template>
                    <template>
                        <v-btn text icon v-on="on" @click="toggleNotifications">
                            <v-icon>{{icons.bell}}</v-icon>
                        </v-btn>
                    </template>
                </v-badge>
            </template>

            <v-card color="secondary">
                <v-card-title>
                    <v-btn text small @click="clearNotifications">clear all</v-btn>
                    <v-spacer></v-spacer>
                    <v-btn text icon small @click="toggleNotifications">
                        <v-icon>{{icons.close}}</v-icon>
                    </v-btn>
                </v-card-title>
                <v-divider></v-divider>
                <v-list class="py-0 overflow-y-auto" height="340" dense>
                    <v-list-item :class="{'blue': n.new}" @click="open(n)" two-line v-for="n in notifications"
                                 :key="`notification-${n.id}`">
                        <v-list-item-content>
                            <v-list-item-title>{{n.message}}</v-list-item-title>
                            <v-list-item-subtitle>{{n.timeStamp}}</v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>
                </v-list>
                <v-divider></v-divider>
                <v-card-actions class="justify-center">
                    <v-btn small text @click="getNotifications" v-if="!empty">Load More</v-btn>
                </v-card-actions>
            </v-card>
        </v-menu>
        <v-btn class="hidden-md-and-up" text icon @click="$emit('toggle')">
            <v-icon color="white">{{icons.menu}}</v-icon>
        </v-btn>
    </v-app-bar>
</template>

<script>
    import {mapMutations, mapActions, mapState} from "vuex";
    import {mdiAccount, mdiBell, mdiClose, mdiMenu} from "@mdi/js";
    import ProfileImage from "../shared/ProfileImage";

    export default {
        name: "App",
        components: {
            ProfileImage
        },
        data() {
            return {
                users: [],
                search: "",
                timeout: null,
                searching: false,
            };
        },
        watch: {
            search: function (v) {
                if (v === undefined || v === null || v === "") return;
                if (this.timeout !== null) clearTimeout(this.timeout);

                this.searching = true;
                this.timeout = setTimeout(
                    function () {
                        this.$axios
                            .get(`/users?search=${v}`)
                            .then(({data}) => {
                                this.users = data;
                            })
                            .catch(err => {
                                this.$logger.error("TODO remove this", err);
                            })
                            .then(() => {
                                this.searching = false;
                            });
                    }.bind(this),
                    500
                );
            }
        },
        methods: {
            ...mapMutations('notifications', ['toggleNotifications']),
            ...mapActions('notifications', ['touchNotification', 'getNotifications', 'clearNotifications']),
            selectUser(displayName) {
                if (displayName && displayName !== this.$route.params.id) {
                    this.$store.dispatch("SET_USER", {
                        router: this.$router,
                        displayName: displayName,
                        redirect: true
                    });
                }
            },
            open(notification) {
                this.toggleNotifications();
                this.touchNotification(notification);
                const {navigation, type} = notification;

                if (type === 'Empty') return;
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
        computed: {
            ...mapState('notifications', {
                notificationsMenu: state => state.open,
                notifications: state => state.notifications,
                empty: state => state.empty,
            }),
            notificationCount() {
                return this.notifications.filter(x => x.new).length;
            },
            icons() {
                return {
                    account: mdiAccount,
                    menu: mdiMenu,
                    bell: mdiBell,
                    close: mdiClose,
                }
            }
        }
    };
</script>


<style lang="scss" scoped>
    ::v-deep .v-badge--overlap.v-badge--left .v-badge__badge {
        top: 0;
        left: 0;
    }
</style>