<template>
  <div class="py-2">
    <comment :comment="comment" :id="`main-${comment.id}`" />
    <footer class="px-5">
      <a class="white--text mx-1" @click="startComment(comment.displayName)">{{$t('comments.reply')}}</a>
      <a
        v-if="comment.hasReplies && !hasSubComments && !loading"
        class="mx-3"
        @click="loadReplies"
      >{{$t('comments.viewReplies')}}</a>
    </footer>
    <div class="ml-3" v-if="hasSubComments">
      <sub-comment
        v-for="subComment in comment.subComments"
        :comment="subComment"
        :key="`c-${comment.id}-${subComment.id}`"
        @reply="startComment(subComment.displayName)"
      />
    </div>
    <div class="text-xs-center">
      <v-progress-circular v-if="loading" color="primary" indeterminate/>
      <v-btn
        text
        icon
        color="primary"
        v-else-if="!empty && comment.hasReplies && hasSubComments"
        @click="loadReplies"
      >
        <v-icon>{{icons.expand}}</v-icon>
      </v-btn>
    </div>
  </div>
</template>

<script>
import Comment from "./Comment";
import SubComment from "./SubComment";
import {mdiExpandAll} from "@mdi/js";

export default {
  name: "main-comment",
  props: {
    comment: {
      required: true,
      type: Object
    }
  },
  data() {
    return {
      loading: false,
      empty: false
    };
  },
  created(){
    if(this.comment.empty) {
      this.empty = true;
    }
  },
  methods: {
    startComment(displayName) {
      this.$store.commit("START_COMMENT", {
        id: this.comment.id,
        tag: displayName
      });
    },
    loadReplies() {
      this.loading = true;
      let { id, index } = this.comment;
      let url = `/comments/sub?commentId=${id}&index=${index}`;
      this.$axios.get(url).then(res => {
        this.$emit("load-sub", { id, subComments: res.data });
        this.empty = res.data.length < 5;
        this.loading = false;
      });
    }
  },
  computed: {
    hasSubComments() {
      return this.comment.subComments.length > 0;
    },
    icons() {
      return {
        expand: mdiExpandAll
      }
    }
  },
  components: {
    Comment,
    SubComment
  }
};
</script>

<style>
</style>
