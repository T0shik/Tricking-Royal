<template>
    <v-avatar :size="imgSize" color="secondary">
        <img v-if="picture" :src="picture" alt="alt" :key="picture"/>
        <v-icon v-else>{{userIcon}}</v-icon>
        <span v-if="level" class="level">{{level}}</span>
    </v-avatar>
</template>

<script>
    import {mdiAccountCircle} from "@mdi/js";

    export default {
        name: "profile-img",
        props: {
            picture: {
                required: true,
            },
            level: {
                required: false,
                type: Number
            },
            size: {
                required: false,
                type: String
            }
        },
        data: () => ({
            userIcon: mdiAccountCircle
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
</style>