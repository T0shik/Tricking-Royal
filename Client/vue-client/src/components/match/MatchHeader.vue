<template>
    <v-card-title class="pa-0 py-2 px-1">
        <div class="d-flex justify-space-between align-center fullwidth">
            <div class="d-flex flex-column flex-1">
                <router-link class="white--text" :to="`/user/${host.displayName}`">
                    <ProfileImage v-if="host" :class="{'playing': host.index === playingIndex}"
                                  :picture="host.picture"
                                  :level="host.level"
                                  :winner="host.winner"
                                  left/>
                    <span class="pl-1">{{host.displayName}}</span>
                </router-link>
            </div>
            <div class="font-rock">
                VS
            </div>
            <div class="d-flex flex-column flex-1">
                <router-link class="white--text text-right" :to="`/user/${opponent.displayName}`">
                    <span class="pr-1">{{opponent.displayName}}</span>
                    <ProfileImage v-if="opponent" :class="{'playing': opponent.index === playingIndex}"
                                  :picture="opponent.picture"
                                  :level="opponent.level"
                                  :winner="opponent.winner"
                                  right/>
                </router-link>
            </div>
        </div>
    </v-card-title>
</template>

<script>
    import ProfileImage from "../shared/ProfileImage";

    export default {
        props: {
            users: {
                type: Array,
                required: true
            },
            playingIndex: {
                type: Number,
                required: true
            }
        },
        components: {
            ProfileImage
        },
        computed: {
            host() {
                return this.users.filter(x => x.role === 0)[0];
            },
            opponent() {
                return this.users.filter(x => x.role === 1)[0];
            }
        }
    };
</script>

<style lang="scss" scoped>
    @import '../../styles/colors';

    .v-card__title {
        padding: 2px 4px;
    }

    span {
        font-size: 0.8rem;
    }

    .playing {
        box-shadow: 0 0 2px 2px $intermediate;
    }
</style>
