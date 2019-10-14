<template>
  <v-dialog v-model="connecting" persistent width="300">
    <div class="secondary pa-3">
      <span class="title white--text">Connecting</span>
      <v-progress-linear indeterminate color="primary" class="mb-0"></v-progress-linear>
    </div>
  </v-dialog>
</template>

<script>
export default {
  props: {
    connecting: {
      required: true,
      type: Boolean
    }
  }
};
</script>

<style>
</style>
