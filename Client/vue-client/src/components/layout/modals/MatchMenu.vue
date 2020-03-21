<template>
    <v-dialog :value="open" @input="dismiss" max-width="320">
        <v-card color="secondary">
            <input type="text" ref="clipboard" style="position: fixed; left: -9999px"/>
            <v-card-title>
                <span v-if="title">{{title}}</span>
                <span v-else>{{$t('menu.title')}}</span>
                <v-spacer/>
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
                                          label="Comment"/>
                        </div>
                    </v-window-item>
                    <v-window-item :value="2">
                        <v-list class="pa-0" v-if="match" color="secondary">
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('create.stage.mode.title')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{matchModeNames[match.mode]}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('create.stage.surface.title')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{$t(`match.surfaces[${match.surface}]`)}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('create.stage.time.title')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{match.turnTime}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('menu.timeLeft')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{match.timeLeft}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line v-if="match.turn">
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('menu.turn')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{match.turn}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('battles.round')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{match.round}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item two-line>
                                <v-list-item-content>
                                    <v-list-item-subtitle>{{$t('misc.likes')}}</v-list-item-subtitle>
                                    <v-list-item-title>{{match.likes}}</v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list>
                    </v-window-item>
                </v-window>
            </v-card-text>
            <v-card-actions class="justify-center" v-if="step > 0">
                <v-btn v-if="menu"
                       color="primary"
                       :disabled="menu.comment && !comment"
                       @click="menu.action.fn"
                >{{menu.action.name}}
                </v-btn>
                <v-btn @click="back" text>{{$t('misc.back')}}</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    import {mapMutations, mapActions, mapState, mapGetters} from "vuex";
    import {mdiClose} from "@mdi/js";
    import {MATCH_TYPES} from "../../../data/enum";
    import mode from "../../../mixins/mode";

    const initialState = () => ({
        step: 0,
        menu: null,
        title: "",
        comment: "",
        loading: false,
        icons: {
            close: mdiClose
        }
    });

    export default {
        data: initialState,
        mixins: [mode],
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

                this.$refs.clipboard.value = process.env.VUE_APP_URL + "/watch/" + id;
                this.$refs.clipboard.select();
                let copyResult = document.execCommand("copy");
                this.$logger.log("[MatchMenu.vue].share copy to clipboard value", el.value, copyResult);

                this.popup({
                    message: this.$t('menu.copiedToClipboard'),
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
            },
        },
        computed: {
            ...mapState('menu', {
                match: state => state.match,
            }),
            ...mapGetters('menu', ['open']),
            allowedButtons() {
                if (this.match === null) {
                    return [];
                }

                let rep = (this.match.round | 0) + 5;
                let exp = (this.match.round | 0) + 7;

                return [
                    {id: 1, name: this.$t('menu.share'), action: this.share, show: true},
                    {
                        id: 2,
                        name: this.$t('menu.matchInformation'),
                        title: this.$t('menu.matchInformation'),
                        action: () => this.setStep(2),
                        show: true
                    },
                    {
                        id: 3,
                        name: this.$t('menu.forfeit.name'),
                        title: this.$t('menu.forfeit.title'),
                        menu: {
                            description: this.$t('menu.forfeit.description', [rep, exp]),
                            comment: false,
                            action: {
                                fn: this.forfeit,
                                name: this.$t('misc.confirm')
                            }
                        },
                        show: this.match.canGo
                    },
                    {
                        id: 4,
                        name: this.$t('menu.flag.name'),
                        title: this.$t('menu.flag.title'),
                        menu: {
                            description: this.$t('menu.flag.description'),
                            comment: true,
                            action: {
                                fn: this.flag,
                                name: this.$t('misc.confirm')
                            }
                        },
                        show: this.match.canFlag
                    },
                    {
                        id: 5,
                        name: this.$t('menu.reUpload.name'),
                        title: this.$t('menu.reUpload.title'),
                        menu: {
                            description: this.$t('menu.reUpload.description'),
                            comment: false,
                            action: {
                                fn: this.upload,
                                name: this.$t('misc.confirm')
                            }
                        },
                        show: this.match.canUpdate
                    },
                ].filter(x => x.show);
            }
        }
    };
</script>

