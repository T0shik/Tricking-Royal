<template>
    <div class="video-container">
        <video
                ref="video"
                :src="video"
                :poster="thumb"
                muted="muted"
                preload="none"
                loop="loop"
                playsinline
                @click="pause()"
        />
        <div class="play-button" @click="play()" v-if="!playing">
            <v-icon size="60px">{{icons.play}}</v-icon>
        </div>
        <div class="play-button" v-if="loading">
            <v-progress-circular :size="50" color="primary" indeterminate/>
        </div>
    </div>
</template>


<script>
    import {mdiPlay} from "@mdi/js";

    export default {
        props: {
            video: {
                type: String,
                required: true
            },
            thumb: {
                type: String,
                required: true
            },
            isPlaying: {
                type: Boolean,
                required: true
            }
        },
        data() {
            return {
                playing: false,
                loading: false,
                icons: {
                    play: mdiPlay
                }
            };
        },
        watch: {
            isPlaying: {
                immediate: false,
                handler(v) {
                    if (v) {
                        this.play();
                    } else {
                        this.pause();
                    }
                }
            }
        },
        mounted() {
            this.$refs.video.addEventListener(
                "playing",
                function () {
                    this.loading = false;
                }.bind(this)
            );
        },
        beforeDestroy() {
            this.$refs.video.removeEventListener(
                "playing",
                function () {
                    this.loading = false;
                }.bind(this)
            );
        },
        methods: {
            play() {
                this.playing = true;
                this.$refs.video.play();
                if (this.$refs.video.readyState !== 4) {
                    this.loading = true;
                }
            },
            pause() {
                this.playing = false;
                this.$refs.video.pause();
            }
        },
    };
</script>


<style lang="scss" scoped>
    .video-container {
        position: relative;

        video {
            max-width: 100%;
        }

        .play-button {
            position: absolute;
            top: 0;
            width: 100%;
            height: 100%;
            display: flex;
            justify-content: center;
            align-items: center;

            .v-icon {
                font-size: 90px;
            }
        }

        .play-button:hover {
            cursor: pointer;
        }
    }
</style>
