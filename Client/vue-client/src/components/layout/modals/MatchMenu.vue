<template>
    <v-dialog :value="open" @input="dismiss" max-width="320">
        <v-card color="secondary">
            <v-card-title>
                <span v-if="title">{{title}}</span>
                <span v-else>Menu</span>
                <v-spacer></v-spacer>
                <v-btn icon text @click="dismiss">
                    <v-icon>{{icons.close}}</v-icon>
                </v-btn>
            </v-card-title>
            <v-card-text class="overflow-y-auto" style="max-height: 360px;">
                <v-window v-model="step" touchless>
                    <v-window-item :value="0">
                        <v-list color="secondary">
                            <v-list-item v-for="button in allowedButtons" :key="button.id"
                                         @click="selectOption(button)">
                                <v-list-item-title class="text-center">
                                    {{button.name}}
                                </v-list-item-title>
                            </v-list-item>
                        </v-list>
                    </v-window-item>
                    <v-window-item :value="1">
                        <div v-if="menu">
                            <p class="title">{{menu.title}}</p>
                            <p class="subtitle">{{menu.description}}</p>
                            <v-text-field v-if="menu.comment" :counter="50" v-model="comment"
                                          label="Comment"></v-text-field>
                        </div>
                    </v-window-item>
                    <v-window-item :value="2">
                        <v-list class="pa-0" v-if="match" color="secondary">
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>Mode</v-list-item-subtitle>
                                    <v-list-item-title>{{match.mode}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>Surface</v-list-item-subtitle>
                                    <v-list-item-title>{{match.surface}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>Time per turn</v-list-item-subtitle>
                                    <v-list-item-title>{{match.turnTime}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>Time left</v-list-item-subtitle>
                                    <v-list-item-title>{{match.timeLeft}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line v-if="match.turn">
                                <v-list-item-content>
                                    <v-list-item-subtitle>Turn</v-list-item-subtitle>
                                    <v-list-item-title>{{match.turn}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>Round</v-list-item-subtitle>
                                    <v-list-item-title>{{match.round}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>Likes</v-list-item-subtitle>
                                    <v-list-item-title>{{match.likes}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list>
                    </v-window-item>
                </v-window>
            </v-card-text>
            <v-card-actions v-if="step > 0">
                <v-btn @click="back" text>back</v-btn>
                <v-btn v-if="menu"
                       text
                       class="primary--text"
                       :disabled="menu.comment && !comment"
                       @click="menu.action.fn"
                >{{menu.action.name}}
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    import {mapMutations, mapActions, mapState, mapGetters} from "vuex";
    import {mdiClose} from "@mdi/js";
    import {MATCH_TYPES} from "../../../data/enum";

    const initialState = () => ({
        step: 0,
        menu: null,
        title: "",
        comment: "",
        loading: false
    });

    export default {
        data: initialState,
        methods: {
            ...mapMutations({
                closeMenu: 'menu/closeMenu',
                setMatch: 'updateMatch/setMatch',
                clearMatches: 'matches/clearMatches'
            }),
            ...mapActions({
                setMatchType: 'matches/setType',
                popup: "DISPLAY_POPUP_DEFAULT",
                refreshProfile: "REFRESH_PROFILE"
            }),
            share() {
                const el = document.createElement("textarea");
                let id = this.match.matchId === undefined || this.match.matchId === null
                    ? this.match.id
                    : this.match.matchId;

                el.value = process.env.VUE_APP_URL + "/watch/" + id;
                el.setAttribute("readonly", "");
                el.style.position = "absolute";
                el.style.left = "-9999px";
                document.body.appendChild(el);
                el.select();
                document.execCommand("copy");
                document.body.removeChild(el);

                this.popup({
                    message: "Link copied to clipboard",
                    success: true,
                });

                this.dismiss();
            },
            forfeit() {
                this.loading = true;
                this.$axios.post(`/matches/${this.match.id}/forfeit`)
                    .then(({data}) => {
                        this.popup(data);
                        this.loading = false;

                        if (data.success) {
                            this.clearMatches(MATCH_TYPES.HISTORY);
                            this.clearMatches(MATCH_TYPES.ACTIVE);
                            this.clearMatches(MATCH_TYPES.SPECTATE);
                            this.$router.push({path: "/battles/history"});
                            this.dismiss();
                            this.refreshProfile();
                        }
                    });
            },
            flag() {
                this.loading = true;
                this.$axios
                    .post(`/matches/${this.match.id}/flag`, {
                        reason: this.comment
                    })
                    .then(({data}) => {
                        this.popup(data);
                        this.loading = false;

                        if (data.success) {
                            this.clearMatches(MATCH_TYPES.HISTORY);
                            this.clearMatches(MATCH_TYPES.ACTIVE);
                            this.clearMatches(MATCH_TYPES.SPECTATE);
                            this.$router.push({path: "/battles/active"});
                            this.dismiss();
                            this.refreshProfile();
                        }
                    });
            },
            upload() {
                this.setMatch({match: this.match, update: true});
                this.dismiss();
            },
            dismiss() {
                this.closeMenu();
                Object.assign(this.$data, initialState());
            },
            selectOption({title, menu, action}) {
                if (menu !== null && menu !== undefined) {
                    this.menu = menu;
                    this.step = 1;
                } else {
                    action();
                }
                if (title) {
                    this.title = title;
                }
            },
            setStep(s) {
                this.step = parseInt(s);
            },
            back() {
                Object.assign(this.$data, initialState());
            }
        },
        computed: {
            ...mapState('menu', {
                match: state => state.match,
            }),
            ...mapGetters('menu', ['open']),
            icons() {
                return {
                    close: mdiClose
                }
            },
            allowedButtons() {
                if (this.match === null) {
                    return [];
                }

                return [
                    {id: 1, name: "Share", action: this.share, show: true},
                    {
                        id: 2,
                        name: "Match Information",
                        title: "Match Information",
                        action: () => {
                            this.setStep(2)
                        },
                        show: true
                    },
                    {
                        id: 3,
                        name: "Forfeit",
                        title: "Forfeit Match?",
                        menu: {
                            description: "Forfeit the match and earn so and so points.",
                            comment: false,
                            action: {
                                fn: this.forfeit,
                                name: "Confirm"
                            }
                        },
                        show: this.match.canGo
                    },
                    {
                        id: 4,
                        name: "Flag",
                        title: "Flag Match?",
                        menu: {
                            description: "Flag your opponent for breaking the rules.",
                            comment: true,
                            action: {
                                fn: this.flag,
                                name: "Flag"
                            }
                        },
                        show: this.match.canFlag
                    },
                    {
                        id: 5,
                        name: "Re-Upload",
                        title: "Re-Upload?",
                        menu: {
                            description: "Re-upload your last video.",
                            comment: false,
                            action: {
                                fn: this.upload,
                                name: "Re-Upload"
                            }
                        },
                        show: this.match.canUpdate
                    },
                ].filter(x => x.show);
            }
        }
    };
</script>

