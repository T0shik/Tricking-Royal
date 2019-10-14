<template>
    <div class="main-card">
        <TribunalVoting></TribunalVoting>
        <div class="ma-5 d-flex flex-column align-center" v-if="loading">
            Loading Matches
            <v-progress-circular color="primary" indeterminate></v-progress-circular>
        </div>
        <v-card v-else-if="evaluations.length === 0" class="mb-4" dark color="secondary" max-width="500" min-width="320"
                width="100%">
            <v-card-title class="justify-center">
                <h1 class="title">No Matches</h1>
            </v-card-title>
            <v-card-text>
                <p>Looks like there is nothing to vote for.</p>
                <p>
                    Check the history tab for your complete matches currently in the voting process
                    or the ones you voted for.
                </p>
                <p>You can also take a look at the Flag section, see if anyone's been breaking the rules.</p>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn color="primary" @click="type = 'history'">history</v-btn>
                <v-btn color="primary" @click="type = 'flag'">flag</v-btn>
            </v-card-actions>
        </v-card>
        <div v-else>
            <MatchPlayer v-for="e in evaluations" :match="e" :key="e.key"></MatchPlayer>
        </div>

        <v-bottom-navigation class="secondary" mandatory absolute v-model="type" ref="nav" grow>
            <v-btn class="pa-0" color="primary" text :to="'/tribunal/history'" value="history">
                <span>History</span>
                <v-icon>{{icons.history}}</v-icon>
            </v-btn>

            <v-btn class="pa-0" color="primary" text :to="'/tribunal/complete'" value="complete">
                <span>Vote</span>
                <v-icon>{{icons.star}}</v-icon>
            </v-btn>

            <v-btn class="pa-0" color="primary" text :to="'/tribunal/flag'" value="flag">
                <span>Flag</span>
                <v-icon>{{icons.flag}}</v-icon>
            </v-btn>
        </v-bottom-navigation>
    </div>
</template>

<script>
    import TribunalVoting from "../components/tribunal/modals/TribunalVoting";
    import {mapGetters, mapActions} from "vuex";
    import {mdiFlag, mdiHistory, mdiStar} from "@mdi/js";
    import MatchPlayer from "../components/match/MatchPlayer";

    export default {
        data() {
            return {
                type: "complete"
            };
        },
        created() {
            if (this.$route.params.type)
                this.type = this.$route.params.type;
        },
        watch: {
            type: {
                immediate: true,
                handler(newType) {
                    this.loadEvaluations({type: newType});
                }
            },
            trueType: {
                immediate: true,
                handler(newType) {
                    this.type = newType;
                }
            }
        },
        components: {
            MatchPlayer,
            TribunalVoting
        },
        methods: {
            ...mapActions({
                loadEvaluations: "LOAD_EVALUATIONS"
            })
        },
        computed: {
            ...mapGetters({
                trueType: "GET_TRIBUNAL_TYPE",
                evaluations: "GET_EVALUATIONS",
                loading: "GET_TRIBUNAL_LOADING"
            }),
            icons() {
                return {
                    history: mdiHistory,
                    star: mdiStar,
                    flag: mdiFlag
                }
            }
        }
    };
</script>

<style lang="scss" scoped>
    .v-item-group.v-bottom-nav .v-btn {
        min-width: auto;
    }
</style>