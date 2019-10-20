<template>
    <div>
        <input type="file" @change="selectFile" hidden accept="video/*" ref="file"/>
        <v-dialog persistent :value="display" width="500">
            <v-card color="secondary">
                <v-card-title class="headline" primary-title>
                    <span>Update Match</span>
                    <v-spacer></v-spacer>
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
                        ></video>
                    </div>
                    <v-window touchless v-model="stage">
                        <v-window-item :value="0" class="text-center pa-3">
                            <v-btn color="info" large @click="$refs.file.click()">
                                Upload Video
                                <v-icon right>{{icons.upload}}</v-icon>
                            </v-btn>
                        </v-window-item>

                        <v-window-item :value="1">
                            <div class="px-4">
                                <p class="text-center title mb-4">Trim</p>

                                <v-range-slider
                                        always-dirty
                                        thumb-label="always"
                                        v-model="trim.value"
                                        :max="trim.duration"
                                        :min="0"
                                        :step="0.1"
                                        @start="$refs.video.pause()"
                                        @end="$refs.video.play()"
                                ></v-range-slider>

                                <div class="re-upload text-center" v-if="isBlitzReUpload">
                                    <v-btn :color="index === 0 ? 'primary' : 'success'" @click="index = 0">1</v-btn>
                                    <v-btn :color="index === 1 ? 'primary' : 'success'" @click="index = 1">2</v-btn>
                                    <v-btn :color="index === 2 ? 'primary' : 'success'" @click="index = 2">3</v-btn>
                                </div>
                            </div>

                            <v-divider></v-divider>

                            <v-card-actions class="justify-center">
                                <v-btn color="info" @click="completeTrim" :disabled="needIndex">
                                    <span v-if="needTrick">Next</span>
                                    <span v-else-if="needIndex">select a pass</span>
                                    <span v-else>Complete</span>
                                </v-btn>
                            </v-card-actions>
                        </v-window-item>

                        <v-window-item :value="2">
                            <div class="px-4">
                                <v-text-field v-model="move" :label="trickLabel" id="trick-input"
                                              @focus="focusTrickInput"></v-text-field>
                            </div>

                            <v-divider></v-divider>

                            <v-card-actions class="justify-center">
                                <v-btn color="info" :disabled="move.length < 3" @click="startMatchUpdate">Finish</v-btn>
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
            ...mapMutations(['reset', 'hide']),
            ...mapActions(['uploadInitial', 'uploadTrimOptions', 'updateMatch']),
            clear() {
                this.$refs.file.value = null;
                this.$refs.video.load();
                Object.assign(this.$data, initialState());
            },
            videoError() {
                if (this.$refs.video.error) {
                    this.clear();
                    this.error = `Failed to load video, please try another video. Error: ${this.$refs.video.error.message}`;
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
                this.uploadInitial(formData);
            },
            focusTrickInput() {
                if (window.innerWidth < 960) {
                    this.$logger.log("Screen should scroll to input");
                    // this.$vuetify.goTo(this.$refs.trick, {container: '#match-update-modal'});
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
                let trimOptions = {
                    start: this.trim.value[0],
                    end: this.trim.value[1]
                };

                this.safeWatch(UPLOAD_STATUS.INITIAL_FINISHED, () =>
                    this.uploadTrimOptions(trimOptions));

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
                };

                this.safeWatch(UPLOAD_STATUS.TRIM_FINISHED, () =>
                    this.updateMatch(options));

                this.$store.dispatch('DISPLAY_POPUP', {
                    message: "Match update in progress, please do NOT close",
                    type: "success",
                    progress: true,
                });

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
                if (this.match) return this.match.mode === MATCH_MODE.ONE_UP ? "Trick" : "Combo";
                else return "";
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