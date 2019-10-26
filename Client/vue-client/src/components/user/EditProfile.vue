<template>
    <v-dialog dark v-model="dialog" fullscreen hide-overlay transition="dialog-bottom-transition">
        <template v-slot:activator="{ on }">
            <v-btn text icon color="white" v-on="on">
                <v-icon>{{icons.settings}}</v-icon>
            </v-btn>
        </template>
        <v-card>
            <v-toolbar color="secondary">
                <v-btn icon @click="dialog = false">
                    <v-icon>{{icons.close}}</v-icon>
                </v-btn>
                <v-spacer></v-spacer>
                <v-toolbar-title class="white--text">Settings</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-btn icon @click="save" color="info" :disabled="!valid || loadingName">
                    <v-icon>{{icons.check}}</v-icon>
                </v-btn>
            </v-toolbar>
            <v-form class="px-4" ref="form" v-model="valid">
                <v-container>
                    <v-row>
                        <v-col>
                            <h1 class="title">Personal Information</h1>
                            <div class="d-flex align-center my-2">
                                <v-avatar size="48px" color="secondary">
                                    <img v-if="tempImage" :src="tempImage" alt="alt" style="object-fit:cover"/>
                                    <img v-else-if="profile.picture" :src="profile.picture" alt="alt"/>
                                    <v-icon v-else class="grey--text">{{icons.account}}</v-icon>
                                </v-avatar>
                                <div class="ml-2">
                                    <v-btn outlined color="info" @click="$refs.image.click()">Change profile picture
                                    </v-btn>
                                    <v-btn class="ma-0" text color="error" v-if="tempImage" @click="clearImage">Remove
                                    </v-btn>
                                    <input
                                            hidden
                                            type="file"
                                            ref="image"
                                            accept="image/*"
                                            @change="storeImage"
                                    />
                                </div>
                            </div>
                            <v-text-field
                                    v-model="profile.displayName"
                                    :rules="displayNameRules"
                                    label="Username"
                                    placeholder="Super_trix_69"
                                    :loading="loadingName"
                                    validate
                                    :counter="15"
                                    maxlength="15"
                            ></v-text-field>
                            <v-select
                                    :items="skillList"
                                    item-text="name"
                                    item-value="value"
                                    v-model="profile.skill"
                                    label="Skill"
                            ></v-select>
                            <v-textarea
                                    counter="255"
                                    v-model="profile.information"
                                    label="Bio"
                                    validate
                                    maxlength="255"
                                    placeholder="Walked through hell and fire to land this cork."
                                    hint="Say something about yourself!"
                            ></v-textarea>
                            <v-subheader class="px-0">Social Media</v-subheader>
                            <v-text-field
                                    :prepend-icon="icons.instagram"
                                    v-model="profile.instagram"
                                    label="Instagram"
                                    placeholder="anton_toshik"
                            ></v-text-field>
                            <v-text-field
                                    :prepend-icon="icons.facebook"
                                    v-model="profile.facebook"
                                    label="Facebook"
                                    placeholder="anton.wieslander"
                            ></v-text-field>
                            <v-text-field
                                    :prepend-icon="icons.youtube"
                                    v-model="profile.youtube"
                                    label="Youtube"
                                    placeholder="UCP_jWxjn__YXmo4iU7Low0g"
                            ></v-text-field>
                        </v-col>
                        <v-col>
                            <v-list two-line flat subheader>
                                <v-subheader>Notifications</v-subheader>
                                <v-list-item>
                                    <v-list-item-action>
                                        <v-icon color="green" v-if="pushEnabled">{{icons.bell}}</v-icon>
                                        <v-icon color="red" v-else>{{icons.bellOff}}</v-icon>
                                    </v-list-item-action>
                                    <v-list-item-content>
                                        <v-list-item-title>Push</v-list-item-title>
                                        <v-list-item-subtitle v-if="pushEnabled">
                                            Push notifications are enabled.
                                        </v-list-item-subtitle>
                                        <v-list-item-subtitle v-else>
                                            <v-btn small color="info" @click="enablePushNotifications">Turn On</v-btn>
                                        </v-list-item-subtitle>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-list-item v-if="!emailConfigLoading && emailConfig">
                                    <v-list-item-action>
                                        <v-btn text icon v-if="emailConfig.enabled"
                                               @click="updateEmailConfig(false)">
                                            <v-icon color="green">{{icons.emailOn}}
                                            </v-icon>
                                        </v-btn>
                                        <v-btn v-else text icon @click="updateEmailConfig(true)">
                                            <v-icon color="red">{{icons.emailOff}}</v-icon>
                                        </v-btn>
                                    </v-list-item-action>
                                    <v-list-item-content>
                                        <v-list-item-title>
                                            Email (click icon to {{emailConfig.enabled ? 'disable' : 'enable'}})
                                        </v-list-item-title>
                                        <v-list-item-subtitle>
                                            {{emailConfig.notificationId}}
                                        </v-list-item-subtitle>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-list-item v-else>
                                    <v-list-item-action>
                                        <v-icon>{{icons.email}}</v-icon>
                                    </v-list-item-action>
                                    <v-list-item-content>
                                        <v-list-item-title>Email</v-list-item-title>
                                        <v-list-item-subtitle v-if="emailConfigLoading">
                                            <v-progress-linear color="primary" indeterminate></v-progress-linear>
                                        </v-list-item-subtitle>
                                        <v-list-item-subtitle v-if="!emailConfig">
                                            <v-btn small color="info"
                                                   @click="updateEmailConfig(true)">
                                                Turn On
                                            </v-btn>
                                        </v-list-item-subtitle>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-divider></v-divider>
                            </v-list>
                            <v-subheader>Account</v-subheader>
                            <div class="d-flex">
                                <v-btn color="warning" @click="signOut">
                                    Sign Out
                                    <v-icon right>{{icons.logout}}</v-icon>
                                </v-btn>
                                <v-spacer></v-spacer>
                                <v-btn color="error" disabled>Delete Account</v-btn>
                            </div>
                        </v-col>
                    </v-row>
                </v-container>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
    import axios from "axios";
    import skillList from "../../data/skills";
    import {mapMutations, mapActions} from "vuex";
    import {
        mdiAccount, mdiBell, mdiBellOff,
        mdiCheck,
        mdiClose, mdiEmail, mdiEmailCheck, mdiEmailMinus,
        mdiFacebook,
        mdiInstagram,
        mdiLogout,
        mdiSettings,
        mdiYoutube
    } from '@mdi/js';
    import {NOTIFICATION_TYPE} from "../../data/enum";

    export default {
        data() {
            return {
                dialog: false,
                browser: false,
                email: true,

                tempImage: "",
                tempImageFile: null,

                valid: true,

                loadingName: false,
                timeout: null,

                validName: true,
                pushEnabled: false,
                displayNameRules: [
                    v => !!v || "Username is required",
                    () => this.validName || "Username already taken.",
                    v => !/\s/.test(v) || "No whitespace allowed."
                ],

                profile: {
                    displayName: "",
                    picture: "",
                    skill: 0,
                    information: "",
                    gym: "",
                    city: "",
                    country: "",
                    instagram: "",
                    facebook: "",
                    youtube: "",
                },
                emailConfig: null,
                emailConfigLoading: false
            };
        },
        watch: {
            dialog: async function (v) {
                if (v) {
                    let profile = this.$store.getters.GET_PROFILE;
                    this.profile = {
                        displayName: profile.displayName,
                        picture: profile.picture,
                        skill: this.getSkillLevel(profile.skill),
                        information: profile.information,
                        gym: profile.gym,
                        city: profile.city,
                        country: profile.country,
                        instagram: profile.instagram,
                        facebook: profile.facebook,
                        youtube: profile.youtube,
                    };

                    this.pushEnabled = await this.getPushState();
                    this.loadEmailConfig();
                }
                this.tempImage = "";
            },
            "profile.displayName": function (v) {
                if (this.timeout !== null) clearTimeout(this.timeout);

                this.loadingName = true;
                this.timeout = setTimeout(
                    function () {
                        axios
                            .get(`/users/${v}/can-use`)
                            .then(result => {
                                const {status} = result;
                                if (status === 200) {
                                    this.validName = true;
                                } else if (status === 204) {
                                    this.validName = false;
                                }
                                this.$refs.form.validate();
                            })
                            .catch(() => {
                                this.validName = false;
                            })
                            .then(() => {
                                this.loadingName = false;
                            });
                    }.bind(this),
                    500
                );
            }
        },
        methods: {
            ...mapMutations({
                updateProfileImage: "UPDATE_PROFILE_IMAGE",
            }),
            ...mapActions({
                enablePushNotifications: "notifications/showPrompt",
                getPushState: "notifications/getPushState",
                refreshProfile: "REFRESH_PROFILE",
                popup: "DISPLAY_POPUP",
                signOut: "SIGN_OUT",
            }),
            updateEmailConfig(enable) {
                if (this.emailConfigLoading) return;
                this.emailConfigLoading = true;
                let email = this.emailConfig !== null
                    ? this.emailConfig.notificationId
                    : '';

                axios.post("/notifications", {
                    notificationId: email,
                    type: NOTIFICATION_TYPE.EMAIL,
                    active: enable,
                })
                    .then(() => {
                        this.emailConfigLoading = false;
                        this.loadEmailConfig();
                    })
            },
            loadEmailConfig() {
                if (this.emailConfigLoading) return;
                this.emailConfigLoading = true;

                this.$axios.get(`/notifications/config/${NOTIFICATION_TYPE.EMAIL}`)
                    .then(({data}) => {
                        this.emailConfig = data;
                        this.emailConfigLoading = false;
                    });
            },
            getSkillLevel(name) {
                return this.skillList.filter(x => x.name === name)[0].value
            },
            clearImage() {
                this.tempImage = "";
                this.tempImageFile = null;
            },
            storeImage(e) {
                let file = e.target.files[0];
                this.tempImageFile = file;
                this.tempImage = URL.createObjectURL(file);
            },
            save() {
                this.$axios
                    .put("users", this.profile)
                    .then(({data: {message, success}}) => {
                        if (success)
                            this.refreshProfile();

                        this.popup({
                            message,
                            type: success ? "success" : "error"
                        });

                        this.dialog = false;
                    })
                    .catch(() => {
                        this.popup({
                            message: "Failed to update profile",
                            type: "error"
                        });
                    });

                if (this.tempImage && this.tempImageFile) {
                    this.uploadImage();
                }
            },
            uploadImage() {
                const formData = new FormData();
                formData.append("image", this.tempImageFile);
                let headers = {
                    headers: {
                        "Content-Type": "multipart/form-data"
                    }
                };
                axios
                    .post(
                        `${process.env.VUE_APP_CDN}/image`,
                        formData,
                        headers
                    )
                    .then(res => {
                        axios.put("/users/picture", {picture: res.data}).then(res => {
                            this.updateProfileImage(res.data.message);
                        });
                    })
                    .catch(error => {
                        this.$logger.error("ERROR UPLOADING IMAGE TODO HANDLE", error);
                    });
            }
        },
        computed: {
            skillList() {
                return skillList;
            },
            icons() {
                return {
                    settings: mdiSettings,
                    close: mdiClose,
                    check: mdiCheck,
                    email: mdiEmail,
                    emailOn: mdiEmailCheck,
                    emailOff: mdiEmailMinus,
                    bell: mdiBell,
                    bellOff: mdiBellOff,
                    account: mdiAccount,
                    facebook: mdiFacebook,
                    instagram: mdiInstagram,
                    youtube: mdiYoutube,
                    logout: mdiLogout
                }
            }
        }
    };
</script>
