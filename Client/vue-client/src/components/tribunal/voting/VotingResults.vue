<template>
    <v-row class="align-center" dense>
        <v-col cols="2">
            {{results.hostPercent}}%
            <v-icon v-if="schema.left.icon">
                {{schema.left.icon}}
            </v-icon>
        </v-col>
        <v-col cols="8">
            <v-progress-linear
                    class="align-center black--text"
                    height="30px"
                    :background-color="background"
                    :color="color"
                    :value="results.hostPercent"
            >
                <v-row class="px-4">
                    <span class="title">{{results.hostVotes}}</span>
                    <v-spacer/>
                    <span class="title">{{results.opponentVotes}}</span>
                </v-row>
            </v-progress-linear>
        </v-col>
        <v-col class="text-right" cols="2">
            <v-icon v-if="schema.right.icon">
                {{schema.right.icon}}
            </v-icon>
            {{results.opponentPercent}}%
        </v-col>
    </v-row>
</template>

<script>
    import {mapState} from 'vuex'

    const defaultSchema = () => ({
        left: {
            icon: null,
            colour: 'purple darken-2',
        },
        right: {
            icon: null,
            colour: 'lime darken-2',
        }
    });

    export default {
        props: {
            schema: {
                type: Object,
                required: false,
                default: defaultSchema
            }
        },
        computed: {
            ...mapState('voting', {
                results: state => state.results,
            }),
            background() {
                if (!this.results) return "";

                return this.results.hostPercent > this.results.opponentPercent
                    ? this.schema.left.colour
                    : this.schema.right.colour;
            },
            color() {
                if (!this.results) return "";

                return this.results.hostPercent > this.results.opponentPercent
                    ? this.schema.right.colour
                    : this.schema.left.colour;
            },
        }
    }
</script>

<style scoped>

</style>