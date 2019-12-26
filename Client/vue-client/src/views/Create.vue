<template>
    <div class="main-card">
        <Rules/>
        <v-card>
            <v-card-title>
                <span>{{stageInfo.title}}</span>
                <v-spacer/>
                <v-btn color="info" icon @click="$store.commit('SET_DISPLAY_RULES', {display: true})">
                    <v-icon>{{icons.info}}</v-icon>
                </v-btn>
            </v-card-title>
            <v-card-text>{{stageInfo.description}}</v-card-text>
            <v-form ref="form" v-model="valid">
                <v-window v-model="stage" touchless>
                    <v-window-item :value="1">
                        <v-card-actions class="justify-center">
                            <v-btn v-for="(m, index) in matches" :key="`mm-${m.value}`"
                                   @click="selectOption('mode', index, m.turnTypes)">
                                {{$t(`match.modes[${index}].name`)}}
                            </v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="2">
                        <v-card-actions class="justify-center flex-wrap">
                            <v-btn v-for="t in turnTypes" :key="`mtt-${t}`" @click="selectOption('turnType', t, {})">
                                {{$t(`match.turnTypes[${t}]`)}}
                            </v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="3">
                        <v-card-actions class="justify-center flex-wrap">
                            <v-btn v-for="s in surfaces" :key="`ms-${s}`" @click="selectOption('surface', s, {})">
                                {{$t(`match.surfaces[${s}]`)}}
                            </v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="4">
                        <v-card-actions class="flex-column">
                            <v-text-field
                                    class="align-self-stretch"
                                    :label="$t('create.stage.time.title')"
                                    :rules="turnTimeRules"
                                    required
                                    mask="##"
                                    v-model="form.turnTime"
                                    suffix="Days"
                            />
                            <v-btn color="primary" @click="stage++" :disabled="!valid">{{$t('misc.select')}}</v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="5">
                        <v-list class="secondary">
                            <v-list-item @click="edit(1)">
                                <v-list-item-title>
                                    <strong class="primary--text">{{$t('create.stage.mode.title')}}:</strong>
                                    {{$t(`match.modes[${form.mode.value}].name`)}}
                                </v-list-item-title>
                            </v-list-item>

                            <v-list-item @click="edit(2)" v-if="form.turnType.value >= 0">
                                <v-list-item-title>
                                    <strong class="primary--text">{{$t('create.stage.turnType.title')}}:</strong>
                                    {{$t(`match.turnTypes[${form.turnType.value}]`)}}
                                </v-list-item-title>
                            </v-list-item>

                            <v-list-item @click="edit(3)">
                                <v-list-item-title>
                                    <strong class="primary--text">{{$t('create.stage.surface.title')}}:</strong>
                                    {{$t(`match.surfaces[${form.surface.value}]`)}}
                                </v-list-item-title>
                            </v-list-item>

                            <v-list-item @click="edit(4)">
                                <v-list-item-title>
                                    <strong class="primary--text">{{$t('create.stage.time.title')}}:</strong>
                                    {{form.turnTime}} {{$t('misc.days')}}
                                </v-list-item-title>
                            </v-list-item>
                        </v-list>
                        <v-card-actions class="justify-center">
                            <v-btn :loading="loading" @click="create" :disabled="!valid || limitReached || loading">
                                {{$t('misc.create')}}
                            </v-btn>
                        </v-card-actions>
                    </v-window-item>
                </v-window>
                <p class="error--text text-center" v-if="limitReached">{{$t('create.limitReached')}}</p>
            </v-form>
        </v-card>
        <v-card color="secondary" class="mt-3">
            <v-card-title class="display-1 justify-center">
                <span>{{$t('create.hostedMatches')}}</span>
            </v-card-title>
            <div class="d-flex justify-center" v-if="loadingMatches">
                <v-progress-circular color="primary" indeterminate/>
            </div>
            <v-list class="secondary">
                <v-list-item v-for="match in hosted" :key="match.id">
                    <v-list-item-title>{{match.mode}}</v-list-item-title>
                    <v-list-item-subtitle>{{$t(`match.surfaces[${match.surface}]`)}} - {{match.turnTime}}
                    </v-list-item-subtitle>
                    <v-spacer/>
                    <v-btn text icon color="error"
                           :loading="loadingDelete"
                           :disabled="loadingDelete"
                           @click="deleteMatch({type: hostMatchType, matchId: match.id})">
                        <v-icon>{{icons.delete}}</v-icon>
                    </v-btn>
                </v-list-item>
            </v-list>
        </v-card>
    </div>
</template>

<script>
    import {mapState, mapGetters, mapMutations, mapActions} from "vuex";
    import {mdiDelete, mdiInformation} from "@mdi/js";
    import {createMatch} from "../data/api";
    import {MATCH_TYPES} from "../data/enum";
    import {matches, surfaces} from "../data/shared";
    import Rules from "../components/layout/modals/Rules";

    const initialForm = () => ({
        mode: {value: -1},
        turnType: {value: -1},
        surface: {value: -1},
        turnTime: 10
    });

    export default {
        data: () => ({
            stage: 1,
            loading: false,
            valid: false,
            editing: false,
            form: initialForm(),
            turnTypes: null
        }),
        created() {
            this.setType({type: MATCH_TYPES.HOSTED});
        },
        methods: {
            ...mapMutations({
                incrementHosting: "INCREMENT_HOSTING_COUNT"
            }),
            ...mapActions('matches', ['setType', "deleteMatch", "refreshMatches"]),
            ...mapActions({
                popup: "DISPLAY_POPUP_DEFAULT",
            }),
            create() {
                if (this.loading) return;
                this.loading = true;
                createMatch({
                    mode: this.form.mode.value,
                    turnType: this.form.turnType.value,
                    surface: this.form.surface.value,
                    turnTime: this.form.turnTime
                }).then(({data}) => {
                        if (data.success) {
                            this.refreshMatches({type: MATCH_TYPES.HOSTED, toggle: false});
                            this.$router.push("/find-battle");
                            this.incrementHosting(1);
                        }
                        this.popup(data);
                    }
                )
                    .then(() => {
                        this.loading = false;
                    });
            },
            selectOption(key, value, args) {
                this.form[key].value = value;
                if (args && key === 'mode') {
                    this.turnTypes = args;
                }
                this.upStage(args === null)
            },
            upStage(skip = false) {
                if (this.editing) {
                    if (this.form.mode.value === 1 && this.form.turnType.value === -1) {
                        this.stage += 1;
                    } else {
                        if (this.form.mode.value !== 1) {
                            this.form.turnType.value = -1;
                        }
                        this.stage = 5;
                    }
                } else {
                    this.stage += skip ? 2 : 1;
                }
            },
            edit(s) {
                this.stage = s;
                this.editing = true;
            }
        },
        computed: {
            ...mapState('matches', {
                hosted: state => state.hosted.list,
                loadingDelete: state => state.loadingDelete,
            }),
            ...mapGetters({
                limitReached: "HOST_LIMIT_REACHED",
                loadingMatches: 'matches/loading'
            }),
            icons() {
                return {
                    delete: mdiDelete,
                    info: mdiInformation
                }
            },
            hostMatchType() {
                return MATCH_TYPES.HOSTED;
            },
            surfaces() {
                return [0, 1, 2, 3, 4, 5]
            },
            stageInfo() {
                switch (this.stage) {
                    case 1:
                        return {
                            title: this.$t('create.stage.mode.title'),
                            description: this.$t('create.stage.mode.description')
                        };
                    case 2:
                        return {
                            title: this.$t('create.stage.turnType.title'),
                            description: this.$t('create.stage.turnType.description')
                        };
                    case 3:
                        return {
                            title: this.$t('create.stage.surface.title'),
                            description: this.$t('create.stage.surface.description')
                        };
                    case 4:
                        return {
                            title: this.$t('create.stage.time.title'),
                            description: this.$t('create.stage.time.description')
                        };
                    case 5:
                        return {
                            title: this.$t('create.stage.confirm.title'),
                            description: this.$t('create.stage.confirm.description')
                        };
                }
                return {title: '', description: ""};
            },
            matches() {
                return matches;
            },
            turnTimeRules() {
                return [
                    v => v > 0 || this.$t('create.validation.turnTimeRequired'),
                    v => v <= 30 || this.$t('create.validation.turnTimeMaxMonth')
                ]
            }
        },
        components: {
            Rules
        }
    };
</script>

<style lang="scss" scoped>
    @import "../styles/colors";

    .v-card {
        background-color: $secondary;

        .v-card__actions .v-btn.v-btn {
            margin-top: 0.25rem;
            background-color: $primary;
        }
    }
</style>