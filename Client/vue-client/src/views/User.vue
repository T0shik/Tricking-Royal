<template>
    <div class="main-card">
        <v-card color="secondary" v-if="user">
            <v-card-title primary-title>
                <profile-img :picture="user.picture"></profile-img>
                <div>
                    <h3 class="headline pl-1 mb-0">{{user.displayName}}</h3>
                </div>
                <v-spacer></v-spacer>
                <h3
                        class="font-rock title font-weight-black"
                        :class="`${user.skill.toLowerCase()}--text`"
                >{{user.skill}}</h3>
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
                    <h1 class="body-2">Bio</h1>
                    <p class="subtitle-2">{{user.information}}</p>
                </div>

                <v-row class="score-board" dense>
                    <v-col cols="4">
                        <span class="subheading">Wins</span>
                        <div class="title">{{user.win}}</div>
                    </v-col>
                    <v-col cols="4">
                        <span class="subheading">Loss</span>
                        <div class="title">{{user.loss}}</div>
                    </v-col>
                    <v-col cols="4">
                        <span class="subheading">Draw</span>
                        <div class="title">{{user.draw}}</div>
                    </v-col>
                    <v-col cols="6">
                        <span class="subheading">Reputation</span>
                        <div class="title">{{user.reputation}}</div>
                    </v-col>
                    <v-col cols="6">
                        <span class="subheading">Style</span>
                        <div class="title">{{user.style}}</div>
                    </v-col>
                </v-row>
            </v-card-text>
        </v-card>

        <v-tabs v-model="tab" mobile-break-point="0" fixed-tabs icons-and-text>
            <v-tabs-slider color="primary"></v-tabs-slider>
            <v-tab v-for="t in tabs" :key="t.id">
                {{t.name}}
                <v-icon>{{t.icon}}</v-icon>
            </v-tab>
        </v-tabs>

        <v-tabs-items v-model="tab" class="transparent">
            <v-tab-item v-for="t in tabs" :key="t.id">
                <div v-if="t.data.length > 0">
                    <div v-if="tab === 2">
                        <OpenMatch :match="match" v-for="match in t.data" :key="match.id"></OpenMatch>
                    </div>
                    <div v-else>
                        <MatchPlayer :match="match" v-for="match in t.data" :key="match.id"></MatchPlayer>
                    </div>
                </div>
                <div class="pa-4 mt-2 title text-xs-center secondary white--text" v-else>
                    <span>{{t.empty}}</span>
                </div>
            </v-tab-item>
        </v-tabs-items>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from "vuex";
    import MatchPlayer from "../components/match/MatchPlayer";
    import OpenMatch from "../components/match/OpenMatch";
    import ProfileImg from "../components/shared/ProfileImage";
    import {mdiAccountGroup, mdiAccountSearch, mdiFacebook, mdiHistory, mdiInstagram, mdiYoutube} from "@mdi/js";

    export default {
        data() {
            return {
                tab: 1
            };
        },
        components: {
            ProfileImg,
            MatchPlayer,
            OpenMatch
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
                getMatches: "LOAD_USER_MATCHSE",
                setUser: "SET_USER"
            })
        },
        computed: {
            ...mapGetters({
                user: "GET_USER",
                history: "GET_USER_HISTORY",
                active: "GET_USER_ACTIVE",
                open: "GET_USER_OPEN"
            }),
            tabs() {
                return [
                    {
                        id: 0,
                        name: "History",
                        value: "history",
                        icon: mdiHistory,
                        data: this.history,
                        empty: `${this.user.displayName} hasn't finished any battles`
                    },
                    {
                        id: 1,
                        name: "Active",
                        value: "active",
                        icon: mdiAccountGroup,
                        data: this.active,
                        empty: `${this.user.displayName} isn't battling anyone at the moment`
                    },
                    {
                        id: 2,
                        name: "Open",
                        value: "open",
                        icon: mdiAccountSearch,
                        data: this.open,
                        empty: `${this.user.displayName} doesn't have any open matches`
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
