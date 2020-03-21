<template>
    <v-dialog :value="display" @click:outside="dismiss" max-width="320">
        <v-card dark color="secondary">
            <v-card-title class="title">
                <span>{{$t(`confirmations.${type}.title`)}}</span>
                <v-spacer />
                <v-btn text icon @click="dismiss">
                    <v-icon>{{icons.close}}</v-icon>
                </v-btn>
            </v-card-title>
            <v-card-text>
                <p v-for="i in descriptions" :key="`cd-${type}-${i}`">{{$t(`confirmations.${type}.description[${i - 1}]`)}}</p>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn color="primary" @click="confirm" :loading="loading" :disabled="loading">
                    {{$t(`confirmations.${type}.button`)}}
                </v-btn>
                <v-btn text @click="dismiss">{{$t('misc.close')}}</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    import {createNamespacedHelpers} from "vuex";
    import {mdiClose} from "@mdi/js";

    const {mapActions, mapState, mapMutations} = createNamespacedHelpers('confirmation');

    export default {
        methods: {
            ...mapActions(['confirm']),
            ...mapMutations(['dismiss']),
        },
        computed: {
            ...mapState(['type', 'descriptions', 'display', 'loading']),
            icons() {
                return {
                    close: mdiClose
                }
            }
        }
    };
</script>

