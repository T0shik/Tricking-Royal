<template>
  <v-app>
    <MatchMenu/>
    <router-view/>
  </v-app>
</template>
<script>
import MatchMenu from "@/components/layout/modals/MatchMenu";

export default {
  components: {
    MatchMenu
  }
};
</script>
