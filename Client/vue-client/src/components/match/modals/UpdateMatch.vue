<template>
    <div>
        <input type="file" @change="selectFile" hidden accept="video/*" ref="file"/>
        <v-dialog persistent :value="display" width="500">
            <v-card color="secondary">
                <v-card-title class="headline" primary-title>
                    <span>{{$t('updateMatch.title')}}</span>
                    <v-spacer/>
                    <v-btn text icon @click="reset">
                        <v-icon>{{icons.close}}</v-icon>
                    </v-btn>
                </v-card-title>
                <v-card-text class="pa-0">
                    <div class="error--text" v-if="error">{{error}}</div>
                    <div class="custom-container">
                        <video
                                v-show="tempFile"
                                ref="video"
                                @error="videoError"
                                @loadedmetadata="videoLoad"
                                @timeupdate="videoTimeUpdate"
                                :src="tempFile"
                                muted="muted"
                                autoplay
                                loop="loop"
                                playsinline
                        />
                    </div>
                    <v-window touchless v-model="stage">
                        <v-window-item :value="0" class="text-center pa-3">
                            <v-btn color="info" large @click="$refs.file.click()">
                                {{$t('updateMatch.uploadVideo')}}
                                <v-icon right>{{icons.upload}}</v-icon>
                            </v-btn>
                        </v-window-item>

                        <v-window-item :value="1">
                            <div class="px-4">
                                <p class="text-center title mb-4">{{$t('updateMatch.trim')}}</p>

                                <v-range-slider
                                        always-dirty
                                        thumb-label="always"
                                        v-model="trim.value"
                                        :max="trim.duration"
                                        :min="0"
                                        :step="0.1"
                                        @start="$refs.video.pause()"
                                        @end="$refs.video.play()"
                                />

                                <div class="re-upload text-center" v-if="isBlitzReUpload">
                                    <v-btn :color="index === 0 ? 'primary' : 'success'" @click="index = 0">1</v-btn>
                                    <v-btn :color="index === 1 ? 'primary' : 'success'" @click="index = 1">2</v-btn>
                                    <v-btn :color="index === 2 ? 'primary' : 'success'" @click="index = 2">3</v-btn>
                                </div>
                            </div>

                            <v-divider/>

                            <v-card-actions class="justify-center">
                                <v-btn color="info" @click="completeTrim" :disabled="needIndex">
                                    <span v-if="needTrick">{{$t('misc.next')}}</span>
                                    <span v-else-if="needIndex">{{$t('updateMatch.selectPass')}}</span>
                                    <span v-else>{{$t('misc.finish')}}</span>
                                </v-btn>
                            </v-card-actions>
                        </v-window-item>

                        <v-window-item :value="2">
                            <div class="px-4">
                                <v-text-field v-model="move" :label="trickLabel" id="trick-input"
                                              @focus="focusTrickInput"/>
                            </div>
                            <v-divider/>
                            <v-card-actions class="justify-center">
                                <v-btn color="info" :disabled="move.length < 2" @click="startMatchUpdate">
                                    {{$t('misc.finish')}}
                                </v-btn>
                            </v-card-actions>
                        </v-window-item>
                    </v-window>
                </v-card-text>
            </v-card>
        </v-dialog>
    </div>
</template>


<script>
    import {createNamespacedHelpers} from "vuex";
    import {mdiClose, mdiUpload} from "@mdi/js";
    import {MATCH_MODE, UPLOAD_STATUS} from "../../../data/enum";

    const {mapState, mapMutations, mapActions} = createNamespacedHelpers('updateMatch');

    const initialState = () => ({
        stage: 0,
        file: null,
        tempFile: null,
        move: "",
        trim: {
            value: [0, 0],
            duration: 0
        },
        index: -1,
        error: ""
    });

    export default {
        data: initialState,
        watch: {
            display: function (v) {
                if (v) {
                    this.$refs.file.click();
                } else {
                    this.clear();
                }
            },
            "trim.value": function (prev, next) {
                if (prev[0] !== next[0]) {
                    this.$refs.video.currentTime = next[0] + 0.1;
                } else if (prev[1] !== next[1]) {
                    this.$refs.video.currentTime = prev[1] - 0.1;
                }
            }
        },
        methods: {
            ...mapMutations(['hide']),
            ...mapActions(['reset', 'uploadVideo', 'startUpdate']),
            clear() {
                this.$refs.file.value = null;
                this.$refs.video.load();
                Object.assign(this.$data, initialState());
            },
            videoError() {
                if (this.$refs.video.error) {
                    this.clear();
                    this.error = `${this.$t('updateMatch.videoUploadError')}. Error: ${this.$refs.video.error.message}`;
                }
            },
            videoLoad() {
                let duration = this.$refs.video.duration.toFixed(1);
                this.trim.duration = duration;
                this.trim.value = [0, duration];
            },
            videoTimeUpdate() {
                const video = this.$refs.video;
                if (video && video.currentTime >= this.trim.value[1]) {
                    video.pause();
                    video.currentTime = this.trim.value[0];
                    video.play();
                }
            },
            selectFile() {
                this.stage = 1;
                this.file = this.$refs.file.files[0];
                this.tempFile = URL.createObjectURL(this.file);
                const formData = new FormData();
                formData.append("video", this.file);
                this.uploadVideo(formData);
            },
            focusTrickInput() {
                if (window.innerWidth < 960) {
                    document.getElementById('trick-input').scrollIntoView();
                }
            },
            safeWatch(uploadStatus, action) {
                let unwatch = null;

                let unwatchAction = () => {
                    if (this.uploadStatus === uploadStatus) {
                        if (unwatch !== null) unwatch();
                        action();
                    }
                };

                if (this.uploadStatus === uploadStatus) {
                    action();
                } else {
                    unwatch = this.$watch("uploadStatus", unwatchAction);
                }
            },
            completeTrim() {
                if (this.needTrick && !this.videoUpdate) {
                    this.stage = 2;
                } else {
                    this.startMatchUpdate();
                }
            },
            startMatchUpdate() {
                let options = {
                    move: this.move,
                    index: this.index,
                    start: this.trim.value[0],
                    end: this.trim.value[1],
                };

                this.safeWatch(UPLOAD_STATUS.INITIAL_FINISHED, () => this.startUpdate(options));
                this.hide();
            }
        },
        computed: {
            ...mapState(['display', 'match', 'uploadStatus', 'videoUpdate']),
            isBlitzReUpload() {
                return this.match && this.videoUpdate && this.match.turnType === 0;
            },
            needIndex() {
                return this.isBlitzReUpload && this.index < 0;
            },
            trickLabel() {
                return this.match
                    ? this.$t(`updateMatch.${this.match.mode === MATCH_MODE.ONE_UP ? 'trick' : 'combo'}`)
                    : "";
            },
            needTrick() {
                return (
                    this.match && (this.match.mode === MATCH_MODE.ONE_UP || this.needTrickCopyCat)
                );
            },
            needTrickCopyCat() {
                return (
                    this.match &&
                    (this.match.mode === MATCH_MODE.COPY_CAT &&
                        this.match.round !== this.match.chain.length)
                );
            },
            icons() {
                return {
                    close: mdiClose,
                    upload: mdiUpload,
                }
            }
        }
    };
</script>

<style lang="scss" scoped>
    .custom-container {
        position: relative;

        video {
            max-width: 100%;
        }
    }

    .re-upload {
        .v-btn {
            min-width: 50px;
        }
    }
</style>