<template>
  <v-app>
    <Popup />
    <MatchMenu/>
    <router-view/>
  </v-app>
</template>
<script>
  import Popup from "@/components/layout/modals/Popup";
  import MatchMenu from "@/components/layout/modals/MatchMenu";

export default {
  components: {
    Popup,
    MatchMenu
  }
};
</script>
