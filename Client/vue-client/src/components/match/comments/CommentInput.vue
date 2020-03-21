<template>
    <div
            class="comment-input secondary"
            :class="activeClass"
            @click="startComment({id: matchId, tag: ''})"
    >
        <div class="comment-tag px-2 py-1" v-if="taggedUser">
            {{$t('comments.replyingTo')}} {{taggedUser}}
            <v-btn text small>{{$t('misc.clear')}}</v-btn>
        </div>
        <v-text-field
                class="px-2 py-1"
                ref="comment"
                @blur="finishComment"
                v-model="message"
                :label="commentLabel"
                counter="255"
        >
            <template v-slot:append>
                <v-btn text icon color="info" :disabled="!canComment" @click="tryComment">
                    <v-icon>{{icons.send}}</v-icon>
                </v-btn>
            </template>
        </v-text-field>
    </div>
</template>

<script>
    import {mapGetters, mapMutations, mapActions} from "vuex";
    import {mdiSend} from "@mdi/js";

    export default {
        name: "comment-input",
        props: {
            matchId: {
                required: true,
                type: Number
            }
        },
        data: () => ({
            message: ""
        }),
        watch: {
            isCommenting: function (v) {
                if (v) this.$refs.comment.focus();
            }
        },
        methods: {
            ...mapActions({
                createComment: "CREATE_COMMENT"
            }),
            ...mapMutations({
                finishComment: "FINISH_COMMENT",
                startComment: "START_COMMENT"
            }),
            tryComment() {
                if (this.isSubComment) {
                    this.createComment({
                        commentId: this.targetId,
                        message: this.message,
                        taggedUser: this.taggedUser
                    }).then(comment => {
                        this.$emit("sub", {
                            id: this.targetId,
                            comment
                        });
                        this.finishComment();
                    });
                } else {
                    this.createComment({
                        matchId: this.targetId,
                        message: this.message
                    }).then(comment => {
                        this.$emit("main", comment);
                        this.finishComment();
                    });
                }
                this.message = "";
            }
        },
        computed: {
            commentLabel() {
                return this.taggedUser || this.$t('comments.comment');
            },
            activeClass() {
                return this.isCommenting ? "active" : "";
            },
            canComment() {
                return this.message.length > 0;
            },
            isSubComment() {
                return this.taggedUser.length > 0;
            },
            icons() {
                return {
                    send: mdiSend
                }
            },
            ...mapGetters({
                isCommenting: "GET_COMMENT_IS_COMMENTING",
                loading: "GET_COMMENT_LOADING",
                taggedUser: "GET_COMMENT_TAG",
                targetId: "GET_COMMENT_TARGET_ID"
            })
        }
    };
</script>

<style lang="scss" scoped>
    @import "../../../styles/colors";

    .comment-input.active {
        @media screen and (max-width: 900px) {
            box-shadow: 0 0 4px 0 #777;
            position: fixed;
            width: 100%;
            bottom: 0;
            left: 0;
            z-index: 99999;
        }
    }

    .comment-tag {
        border-top: 1px solid #eee;
        border-bottom: 1px solid #eee;
    }
</style>
