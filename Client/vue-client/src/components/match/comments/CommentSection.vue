<template>
    <v-card-text v-show="open">
        <main-comment
                v-for="comment in comments"
                :comment="comment"
                :key="`c-${comment.id}`"
                @load-sub="loadSubComments"
        />
        <div class="text-center">
            <v-progress-circular v-if="loading" color="primary" indeterminate></v-progress-circular>
            <v-btn text icon color="primary" v-else-if="!empty" @click="loadComments">
                <v-icon>{{icons.expand}}</v-icon>
            </v-btn>
        </div>
        <comment-input :matchId="match.id" @main="addMainComment" @sub="addSubComment"/>
    </v-card-text>
</template>

<script>
    import Axios from "axios";
    import MainComment from "./MainComment";
    import CommentInput from "./CommentInput";
    import {mdiChevronDown} from "@mdi/js";

    export default {
        props: {
            open: {
                required: true,
                type: Boolean
            },
            match: {
                required: true,
                type: Object
            }
        },
        components: {
            MainComment,
            CommentInput
        },
        data() {
            return {
                loading: false,
                comments: [],
                index: 0,
                empty: false
            };
        },
        created() {
            if (this.match.comments && this.match.comments.length > 0) {
                this.comments = this.match.comments.map(x => ({
                    ...x.mainComment,
                    empty: true,
                    subComments: x.subComments
                }));
                this.empty = true;
            }
        },
        watch: {
            open: {
                immediate: true,
                handler(v) {
                    if (v && this.comments.length === 0 && !this.match.comments) this.loadComments();
                }
            }
        },
        methods: {
            loadComments() {
                this.loading = true;
                let url = `/comments?matchId=${this.match.id}&index=${this.index}`;
                Axios.get(url).then(res => {
                    let newComments = res.data.map(x => this.convertMainComment(x));
                    this.comments = this.comments.concat(newComments);
                    this.index++;
                    this.empty = res.data.length < 5;
                    this.loading = false;
                });
            },
            loadSubComments(payload) {
                let comment = this.comments.filter(x => x.id === payload.id)[0];
                comment.subComments = comment.subComments.concat(payload.subComments);
                comment.index++;
            },
            addMainComment(comment) {
                this.comments.unshift(this.convertMainComment(comment));
            },
            addSubComment(payload) {
                this.comments
                    .filter(x => x.id === payload.id)[0]
                    .subComments.unshift(payload.comment);
            },
            convertMainComment(comment) {
                return {
                    ...comment,
                    index: 0,
                    subComments: []
                };
            }
        },
        computed: {
            icons() {
                return {
                    expand: mdiChevronDown
                }
            }
        }
    };
</script>