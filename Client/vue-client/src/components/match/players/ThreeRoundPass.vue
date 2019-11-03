<template>
    <div>
        <v-window :value="selectedVideo" v-if="match.videos.length > 0">
            <v-window-item v-for="item in match.videos" :key="item.video">
                <video-player
                        :video="item.video"
                        :thumb="item.thumb"
                        :isPlaying="selectedVideo === item.videoIndex"
                ></video-player>
            </v-window-item>
        </v-window>
        <div v-else-if="match.turnType > 0" class="pa-3 title text-center">{{match.turn}}'s turn to upload</div>
        <div v-else-if="match.turnType === 0" class="pa-3 title text-center">Waiting for someone to upload.</div>
        <div class="text-center">
            <v-btn
                    color="info"
                    class="ma-0"
                    v-if="match.canLockIn"
                    :loading="loading"
                    :disabled="loading"
                    @click="$emit('lock-in')"
            >lock in
            </v-btn>
            <v-btn
                    color="info"
                    class="ma-0"
                    v-if="match.canGo"
                    :loading="loading"
                    :disabled="loading"
                    @click="$emit('respond')"
            >respond
            </v-btn>
        </div>
        <v-row v-if="match.videos.length > 0">
            <v-col class="text-center" cols="6">
                <v-btn v-for="(item, index) in hostVideos"
                       :key="item.video"
                       outlined
                       small
                       :class="isActive(item.videoIndex)"
                       @click="selectedVideo = item.videoIndex"
                       v-text="index + 1"
                ></v-btn>
            </v-col>
            <v-col class="text-center" cols="6">
                <v-btn v-for="(item, index) in opponentVideos"
                       :key="item.video"
                       small
                       outlined
                       :class="isActive(item.videoIndex)"
                       @click="selectedVideo = item.videoIndex"
                       v-text="index + 1"
                ></v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<script>
    import VideoPlayer from "./VideoPlayer.vue";

    export default {
        props: {
            match: {
                type: Object,
                required: true
            },
            loading: {
                type: Boolean,
                required: true,
            }
        },
        components: {
            VideoPlayer
        },
        data() {
            return {
                selectedVideo: 0
            }
        },
        watch: {
            selectedVideo: {
                handler: function (value) {
                    let userIndex = -1;
                    if (this.match.videos.length > 0) {
                        userIndex = this.match.videos[value].userIndex;
                    }
                    this.$emit('update-video', {userIndex})
                },
                immediate: true
            }
        },
        methods: {
            isActive(i) {
                return this.selectedVideo === i ? "primary" : "";
            },
        },
        computed: {
            hostVideos() {
                return this.match.videos.filter(x => x.userIndex === 0);
            },
            opponentVideos() {
                return this.match.videos.filter(x => x.userIndex === 1);
            }
        }
    };
</script>


<style lang="scss" scoped>

</style>
