<template>
  <v-card dark color="transparent" text max-width="500" width="100%" v-if="evaluation">
    <v-card-title class="headline pt-0" primary-title>
      <v-spacer></v-spacer>
      <v-btn text icon @click="$emit('close')">
        <v-icon>close</v-icon>
      </v-btn>
    </v-card-title>

    <v-card-text>
      <v-layout mb-3 row align-center justify-center>
        <v-avatar size="68px" color="secondary">
          <img v-if="users[evaluation.target].picture" :src="users[evaluation.target].picture">
          <v-icon v-else class="grey--text" style="font-size: 40px">person</v-icon>
        </v-avatar>
        <span class="pl-2 title">{{users[evaluation.target].displayName}}</span>
      </v-layout>
    </v-card-text>

    <v-card-actions class="justify-center mb-5 pb-5" v-if="!results">
      <v-layout py-1 px-1 column align-center>
        <h1 class="title">Reason</h1>
        <h1 class="subtitle">{{evaluation.reason}}</h1>
        <div class="py-4 text-xs-center" v-if="!loading">
          <v-btn color="ma-1 red darken-4" @click="judge(0)">punish</v-btn>
          <v-btn color="ma-1 amber darken-3" @click="judge(1)">forgive</v-btn>
        </div>
        <div class="py-5" height="100px" v-else>
          Voting
          <v-progress-circular color="primary" indeterminate></v-progress-circular>
        </div>
      </v-layout>
    </v-card-actions>

    <v-card-actions v-else>
      <div class="results">
        <v-layout row align-center>
          <v-flex class="text-xs-right title" xs2>{{results.hostPercent}}%</v-flex>
          <v-flex class="px-1" xs8>
            <v-progress-linear
              class="align-center black--text"
              height="30px"
              :background-color="background"
              :color="color"
              :value="results.hostPercent"
            >
              <v-layout px-2 fill-height row align-center>
                <span class="title">{{results.hostVotes}}</span>
                <v-spacer></v-spacer>
                <span class="title">{{results.opponentVotes}}</span>
              </v-layout>
            </v-progress-linear>
          </v-flex>
          <v-flex class="title" xs2>{{results.opponentPercent}}%</v-flex>
        </v-layout>
      </div>
    </v-card-actions>
  </v-card>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
export default {
  props: {
    evaluation: {
      required: true,
      type: Object
    }
  },
  methods: {
    ...mapActions({
      judge: "voting/vote"
    })
  },
  computed: {
    ...mapGetters({
      results: "GET_VOTE_RESULT",
      loading: "GET_VOTE_LOADING"
    }),
    background() {
      if (!this.results) return "";
      return this.results.hostPercent > this.results.opponentPercent
        ? "red darken-2"
        : "green darken-2";
    },
    color() {
      if (!this.results) return "";
      return this.results.hostPercent > this.results.opponentPercent
        ? "green darken-2"
        : "red darken-2";
    },
    users() {
      if (this.evaluation) return this.evaluation.participants;
      else return null;
    }
  }
};
</script>

<style>
</style>
