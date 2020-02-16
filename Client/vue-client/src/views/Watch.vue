<template>
    <div class="main-card">
        <v-layout v-if="signedIn" class="my-2 pb-4" column align-center justify-start>
            <MatchPlayer v-if="match" :match="match" :key="match.key"/>
            <v-btn color="primary" to="/battles">{{$t('watch.return')}}</v-btn>
        </v-layout>
        <v-layout v-else class="my-2 pb-4" column align-center justify-start>
            <h1
                    class="display-3 hidden-xs-only font-weight-bold primary--text mb-3 font-rock"
            >Tricking Royal</h1>
            <h1
                    class="display-1 hidden-sm-and-up font-weight-bold primary--text mb-3 font-rock"
            >Tricking Royal</h1>
            <h4 class="title">{{$t('landing.description')}}</h4>
            <MatchPlayer v-if="match" :disabled="true" :match="match"/>

            <h4 v-if="pending" class="title">{{$t('watch.matchInTribunal')}}</h4>

            <v-btn v-if="pending" color="info" @click="login">{{$t('watch.signInToVote')}}</v-btn>
            <v-btn v-else color="info" @click="login">{{$t('misc.signIn')}}</v-btn>

            <Connecting :connecting="connecting"/>
        </v-layout>
    </div>
</template>

<script>
    import MatchPlayer from "@/components/match/MatchPlayer.vue";
    import Connecting from "@/components/layout/modals/Connecting";
    import {mapGetters, mapMutations} from "vuex";

    export default {
        data: () => ({
            connecting: false,
            match: null,
            loaded: false
        }),
        watch: {
            match: function (v) {
                if (v !== null) {
                    setTimeout(function () {
                        let {query} = this.$route;
                        if (query.comment) {
                            this.goToComment(`main-${query.comment}`);
                        } else if (query.subComment) {
                            this.goToComment(`sub-${query.subComment}`)
                        }
                    }.bind(this), 250);
                }
            }
        },
        mounted() {
            let id = this.$route.params.id;
            this.$axios.get(`/matches/${id}`).then(({status, data}) => {
                if (status === 200) {
                    this.match = data;
                } else if (status === 204) {
                    this.popup({
                        message: "Match not found",
                        type: "error",
                    });
                    this.$router.push({name: 'battles'});
                }
            });
        },
        methods: {
            ...mapMutations({
                popup: "SET_POPUP"
            }),
            goToComment(selector) {
                let el = document.getElementById(selector);
                el.scrollIntoView({
                    behavior: 'smooth',
                    block: 'center'
                });
                el.classList.add('blue-grey', 'darken-3');
            },
            login() {
                this.connecting = true;
                this.$store.state.userMgr.signinRedirect();
            }
        },
        components: {
            MatchPlayer,
            Connecting
        },
        computed: {
            ...mapGetters({
                signedIn: "AUTHENTICATED"
            }),
            pending() {
                if (this.match === null) return false;
                return this.match.status === 3;
            }
        }
    };
</script>