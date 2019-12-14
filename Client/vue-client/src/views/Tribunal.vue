<template>
    <div class="main-card">
        <TribunalVoting/>
        <div class="ma-5 d-flex flex-column align-center" v-if="loading">
            {{$t('misc.loadingMatches')}}
            <v-progress-circular color="primary" indeterminate/>
        </div>
        <v-card v-else-if="evaluations.length === 0" class="mb-4" color="secondary" width="100%">
            <v-card-title class="justify-center">
                <h1 class="title">{{$t('tribunal.empty.title')}}</h1>
            </v-card-title>
            <v-card-text>
                <p>{{$t('tribunal.empty.description1')}}</p>
                <p>{{$t('tribunal.empty.description2')}}</p>
                <p>{{$t('tribunal.empty.description3')}}</p>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn color="primary" v-if="type !== 'history'" :to="'/tribunal/history'">{{$t('tribunal.history')}}
                </v-btn>
                <v-btn color="primary" v-if="type !== 'flag'" :to="'/tribunal/flag'">{{$t('tribunal.flag')}}</v-btn>
                <v-btn color="primary" v-if="type !== 'complete'" :to="'/tribunal/complete'">{{$t('tribunal.vote')}}
                </v-btn>
            </v-card-actions>
        </v-card>
        <div v-else>
            <MatchPlayer v-for="e in evaluations" :match="e" :key="e.key"/>
        </div>

        <v-bottom-navigation class="secondary" mandatory absolute v-model="type" ref="nav" grow>
            <v-btn class="pa-0" color="primary" text :to="'/tribunal/history'" value="history">
                <span>{{$t('tribunal.history')}}</span>
                <v-icon>{{icons.history}}</v-icon>
            </v-btn>

            <v-btn class="pa-0" color="primary" text :to="'/tribunal/complete'" value="complete">
                <span>{{$t('tribunal.vote')}}</span>
                <v-icon>{{icons.star}}</v-icon>
            </v-btn>

            <v-btn class="pa-0" color="primary" text :to="'/tribunal/flag'" value="flag">
                <span>{{$t('tribunal.flag')}}</span>
                <v-icon>{{icons.flag}}</v-icon>
            </v-btn>
        </v-bottom-navigation>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from "vuex";
    import {mdiFlag, mdiHistory, mdiStar} from "@mdi/js";
    import MatchPlayer from "@/components/match/MatchPlayer";
    import TribunalVoting from "@/components/tribunal/modals/TribunalVoting";

    export default {
        data() {
            return {
                type: "complete"
            };
        },
        created() {
            if (this.$route.params.type) {
                this.type = this.$route.params.type;
            }
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