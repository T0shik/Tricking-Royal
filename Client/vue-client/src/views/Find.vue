<template>
    <div class="main-card">
        <Rules />
        <v-btn @click="refreshMatches({})" color="primary" fixed bottom small left fab>
            <v-icon>{{icons.refresh}}</v-icon>
        </v-btn>
        <div class="ma-5 d-flex flex-column align-center" v-if="loading">
            {{$t('misc.loadingMatches')}}
            <v-progress-circular color="primary" indeterminate />
        </div>
        <div v-else-if="matches.length > 0">
            <OpenMatch v-for="match in matches" :key="match.key"
                       :match="match"
                       :loading="loadingAction"
                       @join="joinMatch(match.id)"
                       @delete="deleteMatch({ type: openMatchType,matchId: match.id})" />
        </div>
        <v-layout v-else column align-center justify-start>
            <v-card color="secondary" width="100%">
                <v-card-title class="justify-center">
                    <h1 class="title">{{$t('battles.empty.open.title')}}</h1>
                </v-card-title>
                <v-card-text class="text-xs-center">
                    <span>{{$t('battles.empty.open.text')}}</span>
                </v-card-text>
                <v-card-actions class="justify-center">
                    <v-btn color="primary" to="/create-battle">{{$t('battles.create')}}</v-btn>
                </v-card-actions>
            </v-card>
        </v-layout>
    </div>
</template>

<script>
    import OpenMatch from "../components/match/OpenMatch";
    import Rules from "../components/layout/modals/Rules";
    import {mapState, mapGetters, mapActions} from "vuex";
    import {MATCH_TYPES} from "../data/enum";
    import {mdiRefresh} from "@mdi/js";

    export default {
        components: {
            OpenMatch,
            Rules
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
                loadingDelete: state => state.loadingDelete,
                loadingJoin: state => state.loadingJoin,
                matches: state => state.open.list,
            }),
            ...mapGetters({
                hostLimit: "HOST_LIMIT_REACHED",
                loading: "matches/loading"
            }),
            loadingAction() {
                return this.loadingDelete || this.loadingJoin
            },
            openMatchType() {
                return MATCH_TYPES.OPEN;
            },
            icons() {
                return {
                    refresh: mdiRefresh
                }
            }
        }
    };
</script>