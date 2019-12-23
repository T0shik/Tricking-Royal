<template>
    <v-dialog dark v-model="dialog" fullscreen hide-overlay transition="dialog-bottom-transition">
        <template v-slot:activator="{ on }">
            <v-btn text icon color="white" v-on="on">
                <v-icon>{{icons.settings}}</v-icon>
            </v-btn>
        </template>
        <v-card>
            <v-toolbar class="toolbar" color="secondary">
                <v-btn icon @click="dialog = false">
                    <v-icon>{{icons.close}}</v-icon>
                </v-btn>
                <v-spacer/>
                <v-toolbar-title class="white--text">{{$t('editProfile.settings')}}</v-toolbar-title>
                <v-spacer/>
                <v-progress-circular color="primary" v-if="loadingName" indeterminate/>
                <v-btn v-else icon @click="save" :color="valid ? 'green' : 'red'" :disabled="!valid">
                    <v-icon>{{valid ? icons.check : icons.ban }}</v-icon>
                </v-btn>
            </v-toolbar>
            <v-form class="px-4 edit-profile" ref="form" v-model="valid">
                <v-container>
                    <v-row>
                        <v-col>
                            <h1 class="title">{{$t('editProfile.personal.title')}}</h1>
                            <div class="d-flex align-center my-2">
                                <v-avatar size="48px" color="secondary">
                                    <img v-if="tempImage" :src="tempImage" alt="alt" style="object-fit:cover"/>
                                    <img v-else-if="profile.picture" :src="profile.picture" alt="alt"/>
                                    <v-icon v-else class="grey--text">{{icons.account}}</v-icon>
                                </v-avatar>
                                <div class="ml-2">
                                    <v-btn outlined color="info" @click="$refs.image.click()">
                                        {{$t('editProfile.personal.changeImg')}}
                                    </v-btn>
                                    <v-btn class="ma-0" text color="error" v-if="tempImage" @click="clearImage">
                                        {{$t('misc.remove')}}
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
                                    :rules="validation.displayNameRules"
                                    :label="$t('editProfile.personal.username')"
                                    placeholder="Super_trix_69"
                                    :loading="loadingName"
                                    validate
                                    :counter="15"
                                    maxlength="15"
                            />
                            <v-select
                                    :items="skills"
                                    :item-text="(o) => $t(`skills.${o.name}.title`)"
                                    item-value="value"
                                    v-model="profile.skill"
                                    :label="$t('editProfile.personal.skill')"
                            />
                            <v-select
                                    :items="languages"
                                    item-text="name"
                                    item-value="locale"
                                    v-model="lang"
                                    @change="loadLanguageAsync"
                                    :label="$t('misc.language')"
                            >
                                <flag slot="prepend" :iso="languageFlag" :squared="false"/>
                            </v-select>
                            <v-textarea
                                    counter="255"
                                    v-model="profile.information"
                                    :label="$t('editProfile.personal.bio')"
                                    validate
                                    maxlength="255"
                                    :placeholder="$t('editProfile.personal.bioPlace')"
                                    :hint="$t('editProfile.personal.bioHint')"
                            />
                            <v-subheader class="px-0">{{$t('editProfile.social')}}</v-subheader>
                            <v-text-field
                                    :prepend-icon="icons.instagram"
                                    :rules="validation.socialLinks"
                                    v-model="profile.instagram"
                                    label="Instagram"
                                    placeholder="anton_toshik"
                            />
                            <v-text-field
                                    :prepend-icon="icons.facebook"
                                    :rules="validation.socialLinks"
                                    v-model="profile.facebook"
                                    label="Facebook"
                                    placeholder="anton.wieslander"
                            />
                            <v-text-field
                                    :prepend-icon="icons.youtube"
                                    :rules="validation.socialLinks"
                                    v-model="profile.youtube"
                                    label="Youtube"
                                    placeholder="UCP_jWxjn__YXmo4iU7Low0g"
                            />
                        </v-col>
                        <v-col>
                            <v-list two-line flat subheader>
                                <v-subheader>{{$t('misc.notifications')}}</v-subheader>
                                <v-list-item>
                                    <v-list-item-action>
                                        <v-icon color="green" v-if="pushEnabled">{{icons.bell}}</v-icon>
                                        <v-icon color="red" v-else>{{icons.bellOff}}</v-icon>
                                    </v-list-item-action>
                                    <v-list-item-content>
                                        <v-list-item-title>{{$t('editProfile.push')}}</v-list-item-title>
                                        <v-list-item-subtitle v-if="pushEnabled">
                                            {{$t('editProfile.pushEnabled')}}
                                        </v-list-item-subtitle>
                                        <v-list-item-subtitle v-else>
                                            <v-btn small color="info" @click="enablePushNotifications">
                                                {{$t('editProfile.turnOn')}}
                                            </v-btn>
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
                                            {{$t('editProfile.email')}} ({{$t('editProfile.clickIconTo')}}
                                            {{emailConfig.enabled ? $t('misc.disable') : $t('misc.enable')}})
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
                                        <v-list-item-title>{{$t('editProfile.email')}}</v-list-item-title>
                                        <v-list-item-subtitle v-if="emailConfigLoading">
                                            <v-progress-linear color="primary" indeterminate/>
                                        </v-list-item-subtitle>
                                        <v-list-item-subtitle v-if="!emailConfig">
                                            <v-btn small color="info"
                                                   @click="updateEmailConfig(true)">
                                                {{$t('editProfile.turnOn')}}
                                            </v-btn>
                                        </v-list-item-subtitle>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-divider/>
                            </v-list>
                            <v-subheader>{{$t('editProfile.account')}}</v-subheader>
                            <div class="d-flex">
                                <v-btn color="warning" @click="signOut">
                                    {{$t('misc.signOut')}}
                                    <v-icon right>{{icons.logout}}</v-icon>
                                </v-btn>
                                <v-spacer/>
                                <v-btn color="error" disabled>{{$t('editProfile.deleteAccount')}}</v-btn>
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
    import {NOTIFICATION_TYPE, STORAGE_KEYS} from "@/data/enum";
    import {mapMutations, mapActions} from "vuex";
    import {
        mdiAccount, mdiBell, mdiBellOff,
        mdiCheck,
        mdiClose, mdiDoNotDisturb, mdiEmail, mdiEmailCheck, mdiEmailMinus,
        mdiFacebook,
        mdiInstagram,
        mdiLogout,
        mdiSettings,
        mdiYoutube
    } from '@mdi/js';
    import {languages} from '@/lang/languages.json'
    import {loadLanguageAsync} from "@/plugins/i18n";
    import {skills} from "@/data/shared";

    export default {
        data: () => ({
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
            emailConfigLoading: false,
            skills,
            languages,
            lang: localStorage.getItem(STORAGE_KEYS.LANGUAGE),
        }),
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
                            .then(({status}) => {
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
                    500);
            },
        },
        methods: {
            loadLanguageAsync,
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
                return this.skills.filter(x => x.name === name.toLowerCase())[0].value
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
            languageFlag(){
                return this.languages.filter(x => x.locale === this.lang)[0].icon
            },
            icons() {
                return {
                    settings: mdiSettings,
                    close: mdiClose,
                    check: mdiCheck,
                    ban: mdiDoNotDisturb,
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
            },
            validation() {
                return {
                    displayNameRules: [
                        v => !!v || this.$t('editProfile.validation.usernameRequired'),
                        () => this.validName || this.$t('editProfile.validation.usernameTaken'),
                        v => !/\s/.test(v) || this.$t('editProfile.validation.noWhiteSpace'),
                    ],
                    socialLinks: [
                        v => !/^[@]|\/|:|\?|&/.test(v) || this.$t('editProfile.validation.socialLinks'),
                    ]
                }
            }
        }
    };
</script>

<style lang="scss">
    .edit-profile {
        padding-top: 56px;
    }

    .toolbar {
        position: fixed;
        width: 100%;
        top: 0;
        z-index: 5;
    }
</style>
