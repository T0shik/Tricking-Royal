<template>
    <div>
        <v-carousel v-model="slide" v-if="match.videos.length > 0" hide-delimiters
                    :show-arrows="match.videos.length > 1">
            <v-carousel-item v-for="(item, index) in match.videos" :value="index" :key="item.video">
                <video-player :video="item.video" :thumb="item.thumb" :isPlaying="index === slide"/>
                <div class="round-tag">{{$t('battles.round')}} {{index + 1}}</div>
            </v-carousel-item>
        </v-carousel>
        <div v-else class="pa-3 title text-center">{{match.turn}} {{$t('battles.turnToUpload')}}</div>
        <div v-if="match.chain[0]" class="pa-1">{{match.chain[0]}}</div>
        <div v-if="match.canGo" class="text-center">
            <v-btn color="info"
                   @click="$emit('respond')"
                   :loading="loading"
                   :disabled="loading">{{$t('battles.respond')}}
            </v-btn>
        </div>
    </div>
</template>

<script>
    import VideoPlayer from "./VideoPlayer.vue";

    export default {
        props: {
            match: {
                required: true,
                type: Object,
            },
            loading: {
                required: true,
                type: Boolean,
            }
        },
        components: {
            VideoPlayer,
        },
        data: () => ({
            slide: 0,
        }),
        created() {
            if (this.match.videos) {
                this.slide = this.match.videos.length - 1;
            }
        },
        watch: {
            slide: {
                handler: function (value) {
                    if (!this.match.videos) {
                        return
                    }
                    let userIndex = -1;
                    if (this.match.videos.length > 0) {
                        userIndex = this.match.videos[value].userIndex;
                    }
                    this.$emit('update-video', {userIndex})
                },
                immediate: true
            }
        },
    };
</script>

<style lang="scss" scoped>
    .round-tag {
        position: absolute;
        bottom: 1rem;
        left: 50%;
        font-size: 25px;
        font-weight: bold;
        text-shadow: 0 0 4px #000;
        transform: translateX(-50%);
        opacity: 0.7;
    }
</style>