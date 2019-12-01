<template>
    <v-card class="mb-5" color="secondary" width="100%">
        <match-header :users="match.participants" :playingIndex="playingIndex"></match-header>
        <component
                v-bind:is="mode"
                :match="match"
                :loading="loading || match.updating"
                @lock-in="lockIn(match)"
                @update-video="setPlayingIndex"
                @respond="respond({match, isReuplaod: false})"
        ></component>
        <div class="d-flex flex-row my-1">
            <v-spacer></v-spacer>
            <v-chip small>{{match.mode}}</v-chip>
            <v-chip v-if="!match.finished" small>{{match.timeLeft}}</v-chip>
        </div>
        <div class="text-center">
            <v-btn v-if="match.canVote" @click="startVote(match)" color="info">vote</v-btn>
        </div>
        <v-card-actions class="justify-space-between">
            <div>
                <v-btn @click="like(match)"
                       :class="{'red--text darken-1': !match.canLike}"
                       :disabled="disabled"
                       text
                       icon
                >
                    <v-icon>{{icons.fire}}</v-icon>
                </v-btn>
                <v-btn @click="openComments = !openComments"
                       :disabled="disabled"
                       text
                       icon
                >
                    <v-icon v-if="!openComments">{{icons.comment}}</v-icon>
                    <v-icon v-else>{{icons.less}}</v-icon>
                </v-btn>
            </div>
            <div class="text-right">
                <v-btn class="white--text" text icon @click="openMenu(match)">
                    <v-icon>{{icons.more}}</v-icon>
                </v-btn>
            </div>
        </v-card-actions>
        <v-divider></v-divider>
        <CommentSection :match="match" :open="openComments"></CommentSection>
    </v-card>
</template>

<script>
    import MatchHeader from "./MatchHeader.vue";
    import OneUp from "./players/OneUp.vue";
    import ThreeRoundPass from "./players/ThreeRoundPass.vue";
    import CopyCat from "./players/CopyCat.vue";
    import CommentSection from "./comments/CommentSection.vue";
    import {mapGetters, mapMutations, mapActions} from "vuex";
    import {mdiChevronUp, mdiComment, mdiFire, mdiMore} from "@mdi/js";

    export default {
        props: {
            match: {
                type: Object,
                required: true
            },
            disabled: {
                required: false,
                type: Boolean
            }
        },
        data() {
            return {
                openComments: false,
                playingIndex: -1,
            };
        },
        components: {
            MatchHeader,
            OneUp,
            ThreeRoundPass,
            CopyCat,
            CommentSection,
        },
        created() {
            let matchHasComments = this.match.comments && this.match.comments.length > 0;
            if (matchHasComments) {
                this.openComments = true;
            }
        },
        methods: {
            ...mapMutations({
                openMenu: "menu/openMenu",
                respond: "updateMatch/setMatch",
                startVote: "voting/setEvaluation",
            }),
            ...mapActions({
                popup: "DISPLAY_POPUP_DEFAULT",
                lockIn: "updateMatch/lockIn",
                like: "updateMatch/like",
            }),
            setPlayingIndex({userIndex}) {
                this.playingIndex = userIndex;
            }
        },
        computed: {
            ...mapGetters({
                loading: 'updateMatch/loading'
            }),
            mode() {
                return this.match.mode.toLowerCase().replace(/ /g, "-");
            },
            icons() {
                return {
                    comment: mdiComment,
                    less: mdiChevronUp,
                    fire: mdiFire,
                    more: mdiMore
                }
            }
        }
    };
</script>

<style scoped lang="scss">
    .v-sheet {
        border-radius: 0;
    }
</style>
