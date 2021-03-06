<template>
    <div class="main-card">
        <v-card color="secondary" v-if="user">
            <v-card-title class="px-2">
                <ProfileImage :picture="user.picture" :level="user.level"/>
                <div>
                    <h3 class="subtitle-1 pl-1 mb-0">{{user.displayName}}</h3>
                </div>
                <v-spacer/>
                <h3
                        class="font-rock skill-text"
                        :class="`${user.skill.toLowerCase()}--text`"
                >{{$t(`skills.${user.skill}.title`)}}</h3>
            </v-card-title>
            <v-card-text class="white--text py-0">
                <v-row class="justify-space-around my-1">
                    <v-btn
                            text
                            icon
                            v-if="user.instagram"
                            :href="`https://instagram.com/${user.instagram}`"
                            target="_blank"
                    >
                        <v-icon size="32">{{icons.instagram}}</v-icon>
                    </v-btn>
                    <v-btn
                            text
                            icon
                            v-if="user.facebook"
                            :href="`https://facebook.com/${user.facebook}`"
                            target="_blank"
                    >
                        <v-icon size="32">{{icons.facebook}}</v-icon>
                    </v-btn>
                    <v-btn
                            text
                            icon
                            v-if="user.youtube"
                            :href="`https://youtube.com/channel/${user.youtube}`"
                            target="_blank"
                    >
                        <v-icon size="32">{{icons.youtube}}</v-icon>
                    </v-btn>
                </v-row>
                <div class="my-2" v-if="user.information">
                    <h1 class="body-2">{{$t('user.bio')}}</h1>
                    <p class="subtitle-2">{{user.information}}</p>
                </div>

                <v-row class="score-board" dense>
                    <v-col cols="4">
                        <span class="subheading">{{$t('misc.win')}}</span>
                        <div class="title">{{user.win}}</div>
                    </v-col>
                    <v-col cols="4">
                        <span class="subheading">{{$t('misc.loss')}}</span>
                        <div class="title">{{user.loss}}</div>
                    </v-col>
                    <v-col cols="4">
                        <span class="subheading">{{$t('misc.draw')}}</span>
                        <div class="title">{{user.draw}}</div>
                    </v-col>
                    <v-col cols="6">
                        <span class="subheading">{{$t('misc.reputation')}}</span>
                        <div class="title">{{user.reputation}}</div>
                    </v-col>
                    <v-col cols="6">
                        <span class="subheading">{{$t('misc.style')}}</span>
                        <div class="title">{{user.style}}</div>
                    </v-col>
                </v-row>
            </v-card-text>
        </v-card>

        <v-tabs v-model="tab" mobile-break-point="0" fixed-tabs icons-and-text>
            <v-tabs-slider color="primary"/>
            <v-tab v-for="t in tabs" :key="t.id">
                {{t.name}}
                <v-icon>{{t.icon}}</v-icon>
            </v-tab>
        </v-tabs>

        <v-tabs-items v-model="tab" class="transparent">
            <v-tab-item v-for="t in tabs" :key="t.id">
                <div v-if="t.data.length > 0">
                    <component v-bind:is="t.component" v-for="match in t.data" :key="match.key"
                               :loading="openLoading" :match="match"/>
                </div>
                <div class="pa-4 mt-2 title text-xs-center secondary white--text" v-else>
                    <span>{{t.empty}}</span>
                </div>
            </v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script>
    import {mapState, mapGetters, mapActions} from "vuex";
    import MatchPlayer from "../components/match/MatchPlayer";
    import OpenMatch from "../components/match/OpenMatch";
    import {mdiAccountGroup, mdiAccountSearch, mdiFacebook, mdiHistory, mdiInstagram, mdiYoutube} from "@mdi/js";
    import ProfileImage from "../components/shared/ProfileImage";
    import {MATCH_TYPES} from "../data/enum";

    export default {
        data: () => ({
            tab: 1
        }),
        components: {
            MatchPlayer,
            OpenMatch,
            ProfileImage
        },
        watch: {
            tab: {
                immediate: false,
                handler(v) {
                    if (v < 0) return;
                    let data = this.tabs[v].data;

                    if (!data || data.length === 0) {
                        this.getMatches({type: this.tabs[v].value});
                    }
                }
            },
            "$route.params.id": {
                immediate: true,
                handler(v) {
                    if (this.user === null || this.user.displayName !== v) {
                        this.setUser({
                            router: this.$router,
                            displayName: this.$route.params.id,
                            redirect: false
                        });
                        this.tab = -1;
                    }
                }
            },
            user: {
                immediate: false,
                handler(user) {
                    if (user) {
                        this.tab = 1;
                    }
                }
            }
        },
        methods: {
            ...mapActions({
                getMatches: "LOAD_USER_MATCHES",
                setUser: "SET_USER"
            })
        },
        computed: {
            ...mapState('matches', {
                openLoading: state => state.open.loading
            }),
            ...mapGetters({
                user: "GET_USER",
                history: "GET_USER_HISTORY",
                active: "GET_USER_ACTIVE",
                open: "GET_USER_OPEN",
            }),
            tabs() {
                return this.user === null
                    ? []
                    : [
                        {
                            id: 0,
                            name: this.$t('battles.history'),
                            value: MATCH_TYPES.HISTORY,
                            icon: mdiHistory,
                            data: this.history,
                            component: MatchPlayer,
                            empty: this.$t('user.historyEmpty')
                        },
                        {
                            id: 1,
                            name: this.$t('battles.active'),
                            value: MATCH_TYPES.ACTIVE,
                            icon: mdiAccountGroup,
                            data: this.active,
                            component: MatchPlayer,
                            empty: this.$t('user.activeEmpty')
                        },
                        {
                            id: 2,
                            name: this.$t('battles.open'),
                            value: MATCH_TYPES.OPEN,
                            icon: mdiAccountSearch,
                            data: this.open,
                            component: OpenMatch,
                            empty: this.$t('user.openEmpty')
                        }
                    ];
            },
            icons() {
                return {
                    facebook: mdiFacebook,
                    instagram: mdiInstagram,
                    youtube: mdiYoutube
                }
            }
        }
    };
</script>

<style lang="scss" scoped>
    div.subheading {
        border-bottom: 1px solid #9a48ab;
    }
</style>
