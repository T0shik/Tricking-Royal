<template>
    <v-card color="transparent" elevation="0" max-width="500" width="100%" v-if="evaluation">
        <v-card-title class="headline pt-0">
            <v-spacer></v-spacer>
            <v-btn text icon @click="$emit('close')">
                <v-icon>{{icons.close}}</v-icon>
            </v-btn>
        </v-card-title>

        <v-card-text class="d-flex flex-column align-center">
            <div class="d-flex flex-row align-center mb-2">
                <ProfileImage :picture="users[evaluation.target].picture"
                             :level="users[evaluation.target].level"
                             size="68px"></ProfileImage>
                <span class="pl-2 title">{{users[evaluation.target].displayName}}</span>
            </div>
            <h1 class="title text--white">Reason</h1>
            <h1 class="subtitle text--primary">{{evaluation.reason}}</h1>
        </v-card-text>

        <v-card-actions class="justify-center">
            <div class="text-center" v-if="loadingResults">
                <p>Loading Results</p>
                <v-progress-circular color="primary" indeterminate></v-progress-circular>
            </div>
            <VotingResults :schema="voteResultSchema" v-else-if="results"></VotingResults>
            <div v-else>
                <v-btn color="ma-1 red darken-4" @click="judge({vote: 0})">punish</v-btn>
                <v-btn color="ma-1 amber darken-3" @click="judge({vote: 1})">forgive</v-btn>
            </div>
        </v-card-actions>
    </v-card>
</template>

<script>
    import {mapActions, mapState} from "vuex";
    import {mdiThumbDown, mdiThumbUp, mdiClose} from "@mdi/js";
    import VotingResults from "./VotingResults";
    import ProfileImage from "../../shared/ProfileImage";

    export default {
        props: {
            evaluation: {
                required: true,
                type: Object
            }
        },
        methods: {
            ...mapActions({
                judge: "voting/vote"
            })
        },
        components: {
            VotingResults,
            ProfileImage
        },
        computed: {
            ...mapState('voting', {
                results: state => state.results,
                loading: state => state.loading,
                loadingResults: state => state.loadingResults,
            }),
            voteResultSchema() {
                return {
                    left: {
                        icon: this.icons.thumbDown,
                        colour: 'green',
                    },
                    right: {
                        icon: this.icons.thumbUp,
                        colour: 'red',
                    }
                }
            },
            users() {
                if (this.evaluation) return this.evaluation.participants;
                else return null;
            },
            icons() {
                return {
                    close: mdiClose,
                    thumbDown: mdiThumbDown,
                    thumbUp: mdiThumbUp,
                }
            }
        }
    };
</script>

<style>
</style>
