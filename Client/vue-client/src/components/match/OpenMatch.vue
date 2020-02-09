<template>
    <v-card class="my-3" max-width="600px" width="100%" dark color="secondary">
        <v-card-title class="pa-2">
            <router-link class="white--text" :to="`/user/${user.displayName}`">
                <ProfileImage :picture="user.picture" :level="user.level"/>
                <span class="white--text body-2 px-1">{{user.displayName}}</span>
            </router-link>
            <v-spacer/>
            <span
                    class="font-rock skill-text"
                    :class="`${user.skill.toLowerCase()}--text`"
            >{{user.skill}}</span>
        </v-card-title>
        <v-card-text class="px-3">
            <div class="d-flex white--text">
                <div>
                    <h1 class="body-1 mb-0">{{$t('create.stage.mode.title')}}</h1>
                    <h1 class="body-2">{{match.mode}}</h1>
                    <h1 class="body-2" v-if="mode">{{mode}}</h1>
                </div>
                <v-spacer/>
                <div class="text-xs-right mr-3">
                    <h1 class="body-1 mb-0">{{$t('create.stage.surface.title')}}</h1>
                    <h1 class="body-2">{{$t(`match.surfaces[${match.surface}]`)}}</h1>
                </div>
                <div class="text-xs-right">
                    <h1 class="body-1 mb-0">{{$t('create.stage.time.short')}}</h1>
                    <h1 class="body-2 text-right">{{match.turnTime}}</h1>
                </div>
            </div>
        </v-card-text>
        <v-card-actions class="justify-center">
            <v-btn color="info" v-if="match.canJoin" @click="$emit('join')" :loading="loading" :disabled="loading">
                {{$t('misc.join')}}
            </v-btn>
            <v-btn color="error"
                   v-else-if="match.canClose"
                   @click="$emit('delete')"
                   :loading="loading"
                   :disabled="loading"
            >{{$t('misc.remove')}}
            </v-btn>
            <span v-else>{{$t('battles.cantJoinOpen')}}</span>
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
            },
            mode() {
                return this.match.turnType >= 0
                    ? this.$t(`match.turnTypes[${this.match.turnType}]`)
                    : "";
            }
        }
    };
</script>

<style lang="scss" scoped>
    h1.body-1 {
        border-bottom: 1px solid #9a48ab;
    }

</style>
