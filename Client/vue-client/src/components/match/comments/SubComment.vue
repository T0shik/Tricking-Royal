<template>
  <div class="my-2">
    <comment :comment="comment" :id="`sub-${comment.id}`"/>
    <footer class="px-5">
      <a class="white--text mx-1" @click="$emit('reply')">{{$t('comments.reply')}}</a>
    </footer>
  </div>
</template>

<script>
import Comment from "./Comment";
export default {
  name: "sub-comment",
  props: {
    comment: {
      required: true,
      type: Object
    }
  },
  components: {
    Comment
  }
};
</script>

<style>
</style>
