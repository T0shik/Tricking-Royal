<template>
    <v-card dark color="transparent" elevation="0" text max-width="500" width="100%" v-if="evaluation">
        <v-card-title class="headline pt-0" primary-title>
            <v-spacer></v-spacer>
            <v-btn text icon @click="$emit('close')">
                <v-icon>{{icons.close}}</v-icon>
            </v-btn>
        </v-card-title>

        <v-card-text class="px-0 d-flex justify-space-around align-center">
            <div class="d-flex flex-column" @click="setTarget(0)">
                <profile-img :class="{'selected': target === 0}"
                             :picture="users[0].picture"
                             :level="users[0].level"
                             size="68px"/>
                <span class="pt-3 title">{{users[0].displayName}}</span>
            </div>
            <div class="title font-rock text-center font-weight-black primary--text">
                VS
            </div>
            <div class="d-flex flex-column" @click="setTarget(1)">
                <profile-img :class="{'selected': target === 1}"
                             :picture="users[1].picture"
                             :level="users[0].level"
                             size="68px"/>
                <span class="pt-3 title">{{users[1].displayName}}</span>
            </div>
        </v-card-text>

        <v-card-actions class="justify-center">
            <div class="text-center" v-if="loadingResults">
                <p>Loading Results</p>
                <v-progress-circular color="primary" indeterminate></v-progress-circular>
            </div>
            <v-row class="align-center" dense v-else-if="results">
                <v-col cols="2">{{results.hostPercent}}%</v-col>
                <v-col cols="8">
                    <v-progress-linear
                            class="align-center black--text"
                            height="30px"
                            :background-color="background"
                            :color="color"
                            :value="results.hostPercent"
                    >
                        <v-row class="px-4">
                            <span class="title">{{results.hostVotes}}</span>
                            <v-spacer></v-spacer>
                            <span class="title">{{results.opponentVotes}}</span>
                        </v-row>
                    </v-progress-linear>
                </v-col>
                <v-col class="text-right" cols="2">{{results.opponentPercent}}%</v-col>
            </v-row>
            <div v-else>
                <v-btn
                        color="info"
                        @click="vote"
                        v-if="target >= 0"
                        :loading="loading"
                        :disabled="loading"
                >vote
                </v-btn>
                <span class="pa-2" v-else>Select Winner</span>
            </div>
        </v-card-actions>
    </v-card>
</template>

<script>
    import {mapMutations, mapActions, mapState} from "vuex";
    import {mdiClose} from "@mdi/js";

    export default {
        props: {
            evaluation: {
                required: true,
                type: Object
            }
        },
        methods: {
            ...mapMutations('voting', ['setTarget']),
            ...mapActions('voting', ['vote'])
        },
        computed: {
            ...mapState('voting', {
                target: state => state.target,
                results: state => state.results,
                loading: state => state.loading,
                loadingResults: state => state.loadingResults,
            }),
            background() {
                if (!this.results) return "";
                return this.results.hostPercent > this.results.opponentPercent
                    ? "purple darken-2"
                    : "lime darken-2";
            },
            color() {
                if (!this.results) return "";
                return this.results.hostPercent > this.results.opponentPercent
                    ? "lime darken-2"
                    : "purple darken-2";
            },
            users() {
                if (this.evaluation) return this.evaluation.participants;
                else return null;
            },
            icons() {
                return {
                    close: mdiClose
                }
            }
        }
    };
</script>

<style lang="scss" scoped>
    .selected {
        box-shadow: 0 0 10px 4px yellow;
        transition: 250ms ease-in;
    }
</style>