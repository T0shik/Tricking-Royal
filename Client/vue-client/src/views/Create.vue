<template>
    <div class="main-card">
        <v-card>
            <v-card-title>{{stageInfo.title}}</v-card-title>
            <v-card-text>{{stageInfo.description}}</v-card-text>
            <v-form ref="form" v-model="valid">
                <v-window v-model="stage">
                    <v-window-item :value="1">
                        <v-card-actions class="justify-center">
                            <v-btn @click="selectMode('One Up', 0, true)">One Up</v-btn>
                            <v-btn @click="selectMode('Three Round Pass', 1, false)">Three Round Pass</v-btn>
                            <v-btn @click="selectMode('Copy Cat', 2, true)">Copy Cat</v-btn>
                            <!-- <v-btn outline color="primary" @click="selectMode('TRICK', 3)">TRICK</v-btn> -->
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="2">
                        <v-card-actions class="justify-center">
                            <v-btn @click="selectType('Blitz', 0)">Blitz</v-btn>
                            <v-btn @click="selectType('Classic', 1)">Classic</v-btn>
                            <v-btn @click="selectType('Alternating', 2)">Alternating</v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="3">
                        <v-card-actions class="justify-center flex-wrap">
                            <v-btn @click="selectSurface('Any', 0)">Any</v-btn>
                            <v-btn @click="selectSurface('Sprung Floor', 1)">Sprung Floor</v-btn>
                            <v-btn @click="selectSurface('Grass', 2)">Grass</v-btn>
                            <v-btn @click="selectSurface('Concrete', 3)">Concrete</v-btn>
                            <v-btn @click="selectSurface('Trampoline', 4)">Trampoline</v-btn>
                            <v-btn @click="selectSurface('Tumbling Track', 5)">Tumbling Track</v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="4">
                        <v-card-actions class="flex-column">
                            <v-text-field
                                    class="align-self-stretch"
                                    label="Time per turn"
                                    :rules="turnTypeRules"
                                    required
                                    mask="##"
                                    v-model="form.turnTime"
                                    suffix="Days"
                            ></v-text-field>
                            <v-btn color="primary" @click="stage++" :disabled="!valid">pick</v-btn>
                        </v-card-actions>
                    </v-window-item>
                    <v-window-item :value="5">
                        <v-list class="secondary">
                            <v-list-item @click="edit(1)">
                                <v-list-item-title>
                                    <strong class="primary--text">Mode:</strong>
                                    {{form.mode.name}}
                                </v-list-item-title>
                            </v-list-item>

                            <v-list-item @click="edit(2)" v-if="form.turnType.value >= 0">
                                <v-list-item-title>
                                    <strong class="primary--text">Type:</strong>
                                    {{form.turnType.name}}
                                </v-list-item-title>
                            </v-list-item>

                            <v-list-item @click="edit(3)">
                                <v-list-item-title>
                                    <strong class="primary--text">Surface:</strong>
                                    {{form.surface.name}}
                                </v-list-item-title>
                            </v-list-item>

                            <v-list-item @click="edit(4)">
                                <v-list-item-title>
                                    <strong class="primary--text">Time:</strong>
                                    {{form.turnTime}} Days
                                </v-list-item-title>
                            </v-list-item>
                        </v-list>
                        <v-card-actions class="justify-center">
                            <v-btn :loading="loading" @click="create" :disabled="!valid || limitReached || loading">
                                Create
                            </v-btn>
                        </v-card-actions>
                    </v-window-item>
                </v-window>
            </v-form>
        </v-card>
        <p class="error--text text-center" v-if="limitReached">Match limit reached.</p>
        <v-card color="secondary" class="mt-3">
            <v-card-title class="display-1 justify-center">
                <span>Hosted Matches</span>
            </v-card-title>
            <div class="d-flex justify-center" v-if="loadingMatches">
                <v-progress-circular color="primary" indeterminate></v-progress-circular>
            </div>
            <v-list class="secondary" v-else>
                <v-list-item v-for="match in hosted" :key="match.id">
                    <v-list-item-title>{{match.mode}}</v-list-item-title>
                    <v-list-item-subtitle>{{match.surface}} - {{match.turnTime}}</v-list-item-subtitle>
                    <v-spacer></v-spacer>
                    <v-btn text icon color="error" @click="deleteMatch(match.id)">
                        <v-icon>{{icons.delete}}</v-icon>
                    </v-btn>
                </v-list-item>
            </v-list>
        </v-card>
    </div>

</template>

<script>
    import {mapState, mapGetters, mapMutations, mapActions} from "vuex";
    import matches from "../data/matches";
    import {mdiDelete} from "@mdi/js";
    import {createMatch} from "../data/api";
    import {MATCH_TYPES} from "../data/enum";

    export default {
        data() {
            return {
                stage: 1,
                matches: matches,
                loading: false,
                valid: false,
                editing: false,
                turnTypeRules: [
                    v => v > 0 || "Turn time needs to be higher than 0",
                    v => v <= 30 || "No longer than a month"
                ],
                form: {
                    mode: {name: "", value: -1},
                    turnType: {name: "", value: -1},
                    surface: {name: "", value: -1},
                    turnTime: 10
                }
            };
        },
        created() {
            this.setType({type: MATCH_TYPES.HOSTED});
        },
        methods: {
            ...mapMutations({
                incrementHosting: "INCREMENT_HOSTING_COUNT"
            }),
            ...mapActions('matches', ['setType', "deleteMatch", "refreshMatches"]),
            ...mapActions({
                popup: "DISPLAY_POPUP",
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
                        const {message, success} = data;
                        if (success) {
                            this.refreshMatches(MATCH_TYPES.HOSTED);
                            this.refreshMatches(MATCH_TYPES.OPEN);
                            this.$router.push("/find-battle");
                            this.incrementHosting(1);
                        }
                        this.popup({
                            message: message,
                            type: success ? "success" : "error"
                        });
                    }
                )
                    .then(() => {
                        this.loading = false;
                    });
            },
            selectMode(n, v, skip) {
                this.form.mode.name = n;
                this.form.mode.value = v;
                this.upStage(skip);
            },
            selectType(n, v) {
                this.form.turnType.name = n;
                this.form.turnType.value = v;
                this.upStage();
            },
            selectSurface(n, v) {
                this.form.surface.name = n;
                this.form.surface.value = v;
                this.upStage();
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
                } else this.stage += skip ? 2 : 1;
            },
            edit(s) {
                this.stage = s;
                this.editing = true;
            }
        },
        computed: {
            ...mapState('matches', {
                loadingMatches: state => state.loading,
                hosted: state => state.hosted.list,
            }),
            ...mapGetters({
                limitReached: "HOST_LIMIT_REACHED",
            }),
            icons() {
                return {
                    delete: mdiDelete
                }
            },
            stageInfo() {
                switch (this.stage) {
                    case 1:
                        return {title: 'Mode', description: "Select a battle mode, don't forget to read the rules."};
                    case 2:
                        return {title: 'Turn Type', description: "What turn style do you prefer?"};
                    case 3:
                        return {title: 'Surface', description: "Select a battle mode, don't forget to read the rules."};
                    case 4:
                        return {title: 'Time per turn', description: "How long do you need per turn?"};
                    case 5:
                        return {
                            title: 'Confirmation',
                            description: "You made this match, are you proud? (click to edit)"
                        };
                }
                return {title: '', description: ""};
            }
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