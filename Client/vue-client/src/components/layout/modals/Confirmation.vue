<template>
    <v-dialog :value="display" @click:outside="dismiss" max-width="320">
        <v-card dark color="secondary">
            <v-card-title class="title">
                <span>{{title}}</span>
                <v-spacer></v-spacer>
                <v-btn text icon @click="dismiss">
                    <v-icon>{{icons.close}}</v-icon>
                </v-btn>
            </v-card-title>
            <v-card-text v-if="Array.isArray(description)">
                <p v-for="d in description" :key="createKey(d)">{{d}}</p>
            </v-card-text>
            <v-card-text v-else>
                <p>{{description}}</p>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn color="primary" @click="confirm" :loading="loading" :disabled="loading">
                    {{buttonText}}
                </v-btn>
                <v-btn text @click="dismiss">Close</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    import {createNamespacedHelpers} from "vuex";
    import {mdiClose} from "@mdi/js";

    const {mapGetters, mapActions, mapState, mapMutations} = createNamespacedHelpers('confirmation');

    export default {
        methods: {
            ...mapActions(['confirm']),
            ...mapMutations(['dismiss']),
            createKey(description) {
                return description.substring(1, 10).replace(/\s/, '-');
            }
        },
        computed: {
            ...mapState(['title', 'description', 'buttonText', 'loading']),
            ...mapGetters(['display']),
            icons() {
                return {
                    close: mdiClose
                }
            }
        }
    };
</script>

