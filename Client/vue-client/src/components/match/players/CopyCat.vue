<template>
    <div class="p-relative">
        <div class="points" v-if="match.videos.length > 0">
            <span class="font-rock">
                {{getUserByIndex(0).points}}
                -
                {{getUserByIndex(1).points}}
            </span>
        </div>
        <v-stepper v-model="step" class="secondary">
            <v-stepper-items>
                <v-stepper-content
                        v-for="(slide, sIndex) in slides"
                        class="pa-0"
                        :step="slide.step"
                        :key="`${match.id}-${sIndex}-group`">
                    <div v-if="slide.videoPair.length > 0">
                        <v-carousel v-model="slide.value" :cycle="false" hide-delimiters>
                            <v-carousel-item v-for="(video, pIndex) in slide.videoPair"
                                             :key="`ccc-${match.id}-${sIndex}-${pIndex}`">
                                <div v-if="video.empty" class="text-center title pa-5 mt-4">
                                    {{match.participants[video.userIndex].displayName}} passed this round.
                                </div>
                                <video-player
                                        v-else
                                        :video="video.video"
                                        :thumb="video.thumb"
                                        :is-playing="(step - 1) * 2 + slide.value === video.videoIndex"
                                ></video-player>
                            </v-carousel-item>
                        </v-carousel>
                        <div v-if="match.chain[sIndex]" class="py-1 px-2">
                            Combo: {{match.chain[sIndex]}}
                        </div>
                    </div>
                    <div v-else class="pa-3 title text-center">{{match.turn}}'s turn to upload</div>
                </v-stepper-content>
            </v-stepper-items>
            <v-stepper-header class="height-40">
                <template v-for="s in steps">
                    <v-stepper-step
                            :key="`${match.id}-${s.value}-step`"
                            class="py-1"
                            :editable="s.enabled"
                            :step="s.value"
                    ></v-stepper-step>
                </template>
            </v-stepper-header>
        </v-stepper>
        <div class="d-flex flex-column justify-center align-center">
            <v-btn class="mt-2 info"
                   v-if="match.canGo"
                   @click="$emit('respond')"
                   :loading="loading"
                   :disabled="loading">
                Respond
            </v-btn>
            <v-btn class="mt-2 warning"
                   v-if="match.canPass"
                   @click="copyCatPass({id: match.id})"
                   :loading="loading"
                   :disabled="loading">
                Pass
            </v-btn>
        </div>
    </div>
</template>

<script>
    import VideoPlayer from "./VideoPlayer.vue";
    import {mapActions} from "vuex";

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
            VideoPlayer
        },
        data() {
            return {
                slides: [],
                steps: [],
                step: 0,
                maxSteps: 4
            };
        },
        watch: {
            step: {
                handler: function () {
                    this.emitUpdateVideo();
                },
                immediate: false,
            },
            slides: {
                handler: function () {
                    this.emitUpdateVideo();
                },
                immediate: false,
                deep: true,
            }
        },
        created() {
            for (let i = 1; i <= this.maxSteps; i++) {
                let slide = this.createSlide(i);
                this.slides.push(slide);
                if (slide.videoPair.length > 0) {
                    this.steps.push({
                        value: i,
                        enabled: true,
                    });
                    this.step = i;
                } else {
                    this.steps.push({
                        value: i,
                        enabled: false,
                    });
                }
            }
        },
        methods: {
            ...mapActions('confirmation', ['copyCatPass']),
            createSlide(index) {
                let offset = (index - 1) * 2;
                let videoPair = [];
                if (offset < this.match.videos.length) {
                    videoPair.push(this.match.videos[offset++]);
                }
                if (offset < this.match.videos.length) {
                    videoPair.push(this.match.videos[offset]);
                }
                return {
                    step: index,
                    value: 0,
                    videoPair
                }
            },
            emitUpdateVideo() {
                let userIndex = -1;
                if (this.match.videos.length > 0) {
                    let offSetStep = this.step - 1;
                    let index = offSetStep * 2 + parseInt(this.slides[offSetStep].value);
                    userIndex = this.match.videos[index].userIndex;
                }
                this.$emit('update-video', {userIndex})
            },
            getUserByIndex(index) {
                for (let i = 0; i < this.match.participants.length; i++) {
                    if (this.match.participants[i].role === index)
                        return this.match.participants[i];
                }
                return {};
            }
        },
    };
</script>

<style lang="scss" scoped>
    @import "../../../styles/colors";

    ::v-deep .v-stepper {
        box-shadow: none;

        .v-stepper__step__step {
            background: transparent !important;
        }

        .v-stepper__step--editable {
            .v-stepper__step__step {
                background: rgba(255, 255, 254, 0.5) !important;
            }
        }

        .v-stepper__step--active {
            .v-stepper__step__step {
                background: $primary !important;
            }
        }
    }

    .points {
        z-index: 2;
        position: absolute;
        display: flex;
        justify-content: center;
        width: 100%;
        top: 0;
        padding: 0 1rem;

        span {
            color: #fff;
            font-size: 20px;
            text-shadow: 3px 3px 0 $secondary;
        }
    }

</style>