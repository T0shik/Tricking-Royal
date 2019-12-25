<template>
    <div class="main-card" ref="root" v-scroll:#container="onScroll">
        <div class="ma-5 d-flex flex-column align-center" v-if="loading">
            {{$t('battles.loading')}}
            <v-progress-circular color="primary" indeterminate/>
        </div>
        <v-card v-else-if="matches.length === 0" color="secondary" width="100%">
            <v-card-title class="justify-center">
                <span class="title" v-if="type === 'active'">{{$t('battles.empty.active.title')}}</span>
                <span class="title" v-else-if="type === 'history'">{{$t('battles.empty.history.title')}}</span>
                <span class="title" v-else-if="type === 'spectate'">{{$t('battles.empty.spectate.title')}}</span>
            </v-card-title>
            <v-card-text class="text-xs-center">
                <span v-if="type === 'active'">{{$t('battles.empty.active.text')}}</span>
                <span v-else-if="type === 'history'">{{$t('battles.empty.history.text')}}</span>
                <span v-else-if="type === 'spectate'">{{$t('battles.empty.spectate.text')}}</span>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn color="primary" to="/find-battle">{{$t('battles.find')}}</v-btn>
                <v-btn color="primary" v-if="type !== 'active'" to="/create-battle">{{$t('battles.create')}}</v-btn>
            </v-card-actions>
        </v-card>
        <div v-else>
            <MatchPlayer v-for="match in matches" :match="match" :key="match.key"/>
            <v-btn class="refresh-button" @click="refreshMatches({})" color="primary" fixed bottom small left fab>
                <v-icon>{{icons.refresh}}</v-icon>
            </v-btn>
        </div>

        <v-bottom-navigation class="secondary" mandatory v-model="type" ref="nav" grow>
            <v-btn class="pa-0" color="primary" text :to="'/battles/history'" value="history">
                <span>{{$t('battles.history')}}</span>
                <v-icon>{{icons.history}}</v-icon>
            </v-btn>

            <v-btn class="pa-0" color="primary" text :to="'/battles/active'" value="active">
                <span>{{$t('battles.active')}}</span>
                <v-icon>{{icons.group}}</v-icon>
            </v-btn>

            <v-btn class="pa-0" color="primary" text :to="'/battles/spectate'" value="spectate">
                <span>{{$t('battles.spectate')}}</span>
                <v-icon>{{icons.globe}}</v-icon>
            </v-btn>
        </v-bottom-navigation>
    </div>
</template>

<script>
    import MatchPlayer from "../components/match/MatchPlayer";
    import {mapState, mapGetters, mapActions} from "vuex";
    import {mdiAccountGroup, mdiEarth, mdiHistory, mdiRefresh} from "@mdi/js";
    import {MATCH_TYPES} from "../data/enum";

    export default {
        watch: {
            '$route.params': {
                handler: function({type}, oldValue) {
                    if(!type){
                        type = MATCH_TYPES.ACTIVE;
                    }
                    let force = oldValue && oldValue.type === type; 
                    
                    this.setType({type, force});
                },
                immediate: true
            }
        },
        methods: {
            ...mapActions('matches', [
                'setType',
                'loadMoreMatches',
                'refreshMatches'
            ]),
            onScroll(e) {
                if (this.loadingMore || this.endReached) return;

                let {scrollTop, scrollHeight, clientHeight} = e.target;
                if (scrollTop + clientHeight + 400 - scrollHeight > 0) {
                    this.loadMoreMatches();
                }
            },
        },
        computed: {
            ...mapState('matches', {
                type: state => state.type,
                loadingMore: state => state.loadingMore,
            }),
            ...mapGetters('matches', ['matches', 'endReached', 'loading']),
            icons() {
                return {
                    history: mdiHistory,
                    globe: mdiEarth,
                    group: mdiAccountGroup,
                    refresh: mdiRefresh,
                }
            }
        },
        components: {
            MatchPlayer
        },
    };
</script>