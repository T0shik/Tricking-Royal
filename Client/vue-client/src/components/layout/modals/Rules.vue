<template>
    <v-dialog :value="displayRules" persistent max-width="400">
        <v-card dark color="secondary">
            <v-card-title class="title">{{$t("misc.rules")}} - {{$t(`match.modes[${section}].name`)}}</v-card-title>
            <v-window v-model="section">
                <v-window-item v-for="m in matches" :value="m.value" :key="`rules-${m.value}`">
                    <v-card-subtitle>
                        {{$t(`match.modes[${m.value}].description`)}}
                    </v-card-subtitle>
                    <v-card-text>
                        <p v-for="r in m.rules" :key="`rules-${m.value}-${r}`">
                            {{$t(`match.modes[${m.value}].rules[${r}]`)}}
                        </p>
                    </v-card-text>
                </v-window-item>
            </v-window>
            <v-card-actions class="justify-center">
                <v-btn :disabled="section === 0" text @click="section--">
                    {{$t("misc.back")}}
                </v-btn>
                <v-spacer/>
                <v-btn :disabled="section === matches.length - 1" color="info" depressed @click="section++">
                    {{$t("misc.next")}}
                </v-btn>
            </v-card-actions>
            <v-card-actions v-if="endReached" class="justify-center">
                <v-btn color="primary" @click="confirm">
                    {{$t("misc.rulesRead")}}
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    import {mapState} from 'vuex';
    import {STORAGE_KEYS} from "../../../data/enum";
    import {matches} from "../../../data/shared";

    export default {
        data: () => ({
            section: 0,
            endReached: false,
            matches
        }),
        watch: {
            displayRules: function (display) {
                if (display) {
                    this.endReached = localStorage.getItem(STORAGE_KEYS.RULES_READ) === 'yes';
                }
            },
            section: function (section) {
                if (section === this.matches.length - 1) {
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
        computed: mapState(['displayRules'])
    }
</script>

<style scoped>

</style>