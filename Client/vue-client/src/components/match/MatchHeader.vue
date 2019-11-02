<template>
    <v-card-title>
        <v-container class="pa-0">
            <v-row dense>
                <v-col class="d-flex align-center" cols="5">
                    <router-link class="white--text" :to="`/user/${host.displayName}`">
                        <profile-img :class="{'playing': host.index === playingIndex}"
                                     :picture="host.picture"
                                     :level="host.level"
                                     :winner="host.winner"></profile-img>
                        <span>{{host.displayName}}</span>
                    </router-link>
                </v-col>
                <v-col class="d-flex align-center justify-center title font-rock" cols="2">
                    VS
                </v-col>
                <v-col class="d-flex align-center justify-end" cols="5">
                    <router-link class="white--text" :to="`/user/${opponent.displayName}`">
                        <span>{{opponent.displayName}}</span>
                        <profile-img :class="{'playing': opponent.index === playingIndex}"
                                     :picture="opponent.picture"
                                     :level="opponent.level"
                                     :winner="opponent.winner"></profile-img>
                    </router-link>
                </v-col>
            </v-row>
        </v-container>
    </v-card-title>
</template>

<script>
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
        padding: 0 2px;
        font-size: 12px;
    }

    .playing {
        box-shadow: 0 0 2px 2px $intermediate;
    }
</style>
