<template>
    <v-dialog :value="(evaluation ? 1 : 0)" persistent fullscreen transition="dialog-transition">
        <v-layout class="custom-opacity" fill-height column align-center justify-center width="100%">
            <component v-if="evaluation" :is="votingComponent" :evaluation="evaluation" @close="close"></component>
        </v-layout>
    </v-dialog>
</template>

<script>
    import Complete from "../voting/Complete.vue";
    import Flag from "../voting/Flag.vue";
    import {mapMutations, mapState} from "vuex";

    export default {
        methods: mapMutations('voting', ['close']),
        components: {
            Complete,
            Flag
        },
        computed: {
            ...mapState('voting', ['evaluation']),
            votingComponent() {
                if (this.evaluation === null) return;
                return this.evaluation.flag ? Flag : Complete;
            }
        }
    };
</script>

<style lang="scss" scoped>
    .custom-opacity {
        background-color: rgba(48, 52, 63, 0.9);
    }

    > > > .v-progress-linear__content {
        height: 100%;
    }

    > > > .results {
        width: 100%;
    }
</style>
