<template>
    <div class="main-card">
        <v-btn @click="refreshMatches({})" color="primary" fixed bottom small left fab>
            <v-icon>{{icons.refresh}}</v-icon>
        </v-btn>
        <div class="ma-5 d-flex flex-column align-center" v-if="loading">
            Loading Matches
            <v-progress-circular color="primary" indeterminate></v-progress-circular>
        </div>
        <div v-else-if="matches.length > 0">
            <OpenMatch v-for="match in matches" :key="match.key"
                       :match="match"
                       :loading="loading"
                       @join="joinMatch(match.id)"
                       @delete="deleteMatch(match.id)"></OpenMatch>

        </div>
        <v-layout v-else column align-center justify-start>
            <v-card color="secondary" width="100%">
                <v-card-title class="justify-center">
                    <h1 class="title">No Open Matches</h1>
                </v-card-title>
                <v-card-text class="text-xs-center">
                    <span>Looks like there are no open matches, go make one!</span>
                </v-card-text>
                <v-card-actions class="justify-center">
                    <v-btn color="primary" to="/create-battle">Create Battle</v-btn>
                </v-card-actions>
            </v-card>
        </v-layout>
    </div>
</template>

<script>
    import OpenMatch from "../components/match/OpenMatch";
    import {mapState, mapGetters, mapActions} from "vuex";
    import {MATCH_TYPES} from "../data/enum";
    import {mdiRefresh} from "@mdi/js";

    export default {
        components: {
            OpenMatch
        },
        created() {
            this.setType({type: MATCH_TYPES.OPEN});
        },
        methods: mapActions('matches', [
            "setType",
            "deleteMatch",
            "joinMatch",
            "refreshMatches",
        ]),
        computed: {
            ...mapState('matches', {
                loading: state => state.loading,
                matches: state => state.open.list,
            }),
            ...mapGetters({
                hostLimit: "HOST_LIMIT_REACHED",
            }),
            icons() {
                return {
                    refresh: mdiRefresh
                }
            }
        }
    };
</script>