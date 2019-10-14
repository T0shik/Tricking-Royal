<template>
    <v-layout class="pa-2 lighten-2" column justify-center align-center fill-height>
        <h1 class="display-3 hidden-xs-only">Skill Selection</h1>
        <h1 class="display-1 hidden-sm-and-up">Skill Selection</h1>
        <h1 class="title mb-3 text-xs-center" style="max-width: 500px"
        >Pick an option that best represents your skill level you can always change it later.</h1>
        <v-card color="secondary" class="mx-auto" max-width="500" min-width="300" width="100%">
            <v-window v-model="skill" touchless>
                <v-window-item v-for="item in skillList" :key="item.value" :value="item.value">
                    <v-card-title class="display-1 font-rock font-weight-bold justify-space-between">
                        <span :class="`${item.name.toLowerCase()}--text`">{{ item.name }}</span>
                    </v-card-title>
                    <v-card-text class="subheading white--text">{{item.text}}</v-card-text>
                </v-window-item>
            </v-window>

            <v-divider></v-divider>

            <v-card-actions>
                <v-btn class="white--text" v-show="skill !== 0" text @click="skill--">Back</v-btn>
                <v-spacer></v-spacer>
                <v-btn v-show="skill !== skillList.length - 1" depressed @click="skill++">Next</v-btn>
            </v-card-actions>
        </v-card>
        <v-btn class="mt-4" color="info" @click="choose" :loading="loading" :disabled="loading">Choose</v-btn>
    </v-layout>
</template>

<script>
    import axios from "axios";
    import skillList from "../data/skills.js";

    export default {
        data() {
            return {
                skillList: skillList,
                skill: 0,
                loading: false
            };
        },
        methods: {
            choose() {
                if (this.loading) return;
                this.loading = true;
                axios
                    .post("/users", {skill: this.skill})
                    .then(({data}) => {
                        this.$store.dispatch("UPDATE_PROFILE", data);
                        this.$router.replace("/battles");
                    })
                    .catch(err => {
                        console.log("TODO handle skill error", err);
                    });
            }
        }
    };
</script>