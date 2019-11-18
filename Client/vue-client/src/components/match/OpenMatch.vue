<template>
    <v-card class="my-3" max-width="600px" width="100%" dark color="secondary">
        <v-card-title>
            <router-link class="white--text" :to="`/user/${user.displayName}`">
                <ProfileImage :picture="user.picture" :level="user.level"></ProfileImage>
                <span class="white--text subheading px-2">{{user.displayName}}</span>
            </router-link>
            <v-spacer></v-spacer>
            <span
                    class="font-rock font-weight-black"
                    :class="`${user.skill.toLowerCase()}--text`"
            >{{user.skill}}</span>
        </v-card-title>
        <v-card-text>
            <div class="d-flex white--text">
                <div>
                    <h1 class="body-1 mb-0">Mode</h1>
                    <h1 class="body-2">{{match.mode}}</h1>
                </div>
                <v-spacer></v-spacer>
                <div class="text-xs-right mr-3">
                    <h1 class="body-1 mb-0">Surface</h1>
                    <h1 class="body-2">{{match.surface}}</h1>
                </div>
                <div class="text-xs-right">
                    <h1 class="body-1 mb-0">Turn Time</h1>
                    <h1 class="body-2">{{match.turnTime}}</h1>
                </div>
            </div>
        </v-card-text>
        <v-card-actions class="justify-center">
            <v-btn color="info" v-if="match.canJoin" @click="$emit('join')" :loading="loading" :disabled="loading">
                Join
            </v-btn>
            <v-btn color="error"
                   v-else-if="match.canClose"
                   @click="$emit('delete')"
                   :loading="loading"
                   :disabled="loading"
            >Remove
            </v-btn>
            <span v-else>Can't join this match</span>
        </v-card-actions>
    </v-card>
</template>

<script>
    import ProfileImage from "../shared/ProfileImage";

    export default {
        props: {
            match: {
                required: true,
                type: Object
            },
            loading: {
                required: true,
                type: Boolean
            }
        },
        components: {
            ProfileImage
        },
        computed: {
            user() {
                return this.match.participants[0];
            }
        }
    };
</script>

<style lang="scss" scoped>
    h1.body-1 {
        border-bottom: 1px solid #9a48ab;
    }
</style>
