<template>
    <v-layout class="pa-2 lighten-2" column justify-center align-center fill-height>
        <h1 class="display-3 hidden-xs-only">{{$t("skills.title")}}</h1>
        <h1 class="display-1 hidden-sm-and-up">{{$t("skills.title")}}</h1>
        <h1 class="title mb-3 text-xs-center" style="max-width: 500px"
        >{{$t("skills.description")}}</h1>
        <v-card color="secondary" class="mx-auto" max-width="500" min-width="300" width="100%">
            <v-window v-model="skill" touchless>
                <v-window-item v-for="(s, index) in skills" :key="s" :value="index">
                    <v-card-title class="display-1 font-rock font-weight-bold justify-space-between">
                        <span :class="`${s}--text`">{{$t(`skills.${s}.title`)}}</span>
                    </v-card-title>
                    <v-card-text class="subheading white--text">{{$t(`skills.${s}.text`)}}</v-card-text>
                </v-window-item>
            </v-window>

            <v-divider></v-divider>

            <v-card-actions>
                <v-btn class="white--text" v-show="skill !== 0" text @click="skill--">{{$t("misc.back")}}</v-btn>
                <v-spacer></v-spacer>
                <v-btn v-show="skill !== skills.length - 1" depressed @click="skill++">{{$t("misc.next")}}</v-btn>
            </v-card-actions>
        </v-card>
        <v-btn class="mt-4" color="info" @click="choose" :loading="loading" :disabled="loading">{{$t("misc.select")}}</v-btn>
    </v-layout>
</template>

<script>
    import {LAYOUT} from "../data/enum";

    export default {
        data() {
            return {
                skills: ['beginner', 'intermediate', 'skilled', 'expert', 'master', 'goat'],
                skill: 0,
                loading: false
            };
        },
        methods: {
            choose() {
                if (this.loading) return;
                this.loading = true;
                this.$axios
                    .post("/users", {skill: this.skill})
                    .then(({data}) => {
                        this.$store.dispatch("UPDATE_PROFILE", data);
                        this.$store.commit('layout/setLayout', LAYOUT.USER, {root: true});
                        this.$store.dispatch('confirmation/notificationsPrompt', {}, {root: true});
                        this.$router.replace("/battles");
                    })
                    .catch(err => {
                        this.$logger.log("TODO handle skill error", err);
                    });
            }
        }
    };
</script>