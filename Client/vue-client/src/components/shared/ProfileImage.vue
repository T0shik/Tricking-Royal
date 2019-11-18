<template>
    <v-avatar :size="imgSize" color="secondary">
        <img v-if="picture" :src="picture" alt="alt" :key="picture"/>
        <v-icon v-else>{{icons.user}}</v-icon>
        <span v-if="level" class="level">{{level}}</span>
        <span class="winner" v-if="winner">
            <v-icon>{{icons.winner}}</v-icon>
        </span>
    </v-avatar>
</template>

<script>
    import {mdiAccountCircle, mdiTrophy} from "@mdi/js";

    export default {
        props: {
            picture: {
                required: true,
                default: ""
            },
            level: {
                required: false,
                type: Number
            },
            size: {
                required: false,
                type: String
            },
            winner: {
                required: false,
                type: Boolean
            }
        },
        data: () => ({
            icons: {
                user: mdiAccountCircle,
                winner: mdiTrophy
            }
        }),
        computed: {
            imgSize() {
                return this.size ? this.size : "48px";
            }
        }
    }
</script>

<style lang="scss" scoped>
    @import '../../styles/colors';

    span.level {
        position: absolute;
        bottom: -0.2rem;
        padding: 1px 0.25rem 0;
        background-color: $secondary;
        border-radius: 5px;
        font-size: 12px;
        box-shadow: 0 1px 2px 1px #aaa;
    }

    span.winner {
        position: absolute !important;
        top: -3px;
        right: -6px;
        font-size: 14px;

        .theme--dark.v-icon {
            color: #ffe200;
        }
    }
</style>