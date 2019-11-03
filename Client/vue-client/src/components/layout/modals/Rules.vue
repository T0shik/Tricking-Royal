<template>
    <v-dialog :value="displayRules" persistent max-width="400">
        <v-card dark color="secondary">
            <v-card-title class="title">Rules - {{title}}</v-card-title>
            <v-window v-model="section">
                <v-window-item v-for="(match, index) in matchRules" :value="index + 1" :key="`match-rules-${index}`">
                    <v-card-subtitle>
                        {{match.description}}
                    </v-card-subtitle>
                    <v-card-text>
                        <p v-for="(rule, index) in match.rules" :key="`rules-${section}-${index}`">{{rule}}</p>
                    </v-card-text>
                </v-window-item>
            </v-window>
            <v-card-actions class="justify-center">
                <v-btn :disabled="section === 1" text @click="section--">
                    Back
                </v-btn>
                <v-spacer></v-spacer>
                <v-btn :disabled="section === matchRules.length" color="info" depressed @click="section++">
                    Next
                </v-btn>
            </v-card-actions>
            <v-card-actions v-if="endReached" class="justify-center">
                <v-btn color="primary" @click="confirm">
                    I have read the rules
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    import matchRules from "../../../data/matchRules";
    import {mapState} from 'vuex';
    import {STORAGE_KEYS} from "../../../data/enum";

    export default {
        data: () => ({
            section: 1,
            endReached: false,
        }),
        watch: {
            displayRules: function (display) {
                if (display) {
                    this.endReached = localStorage.getItem(STORAGE_KEYS.RULES_READ) === 'yes';
                }
            },
            section: function (section) {
                if (section === this.matchRules.length) {
                    this.endReached = true
                }
            }
        },
        created() {
            if (!localStorage.getItem(STORAGE_KEYS.RULES_READ)) {
                this.$store.commit('SET_DISPLAY_RULES', {display: true});
            }
        },
        methods: {
            confirm() {
                localStorage.setItem(STORAGE_KEYS.RULES_READ, "yes");
                this.$store.commit('SET_DISPLAY_RULES', {display: false});
            }
        },
        computed: {
            ...mapState(['displayRules']),
            matchRules() {
                return matchRules
            },
            title() {
                return matchRules[this.section - 1].name;
            }
        }
    }
</script>

<style scoped>

</style>