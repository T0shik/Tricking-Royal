<template>
    <v-navigation-drawer
            color="secondary"
            app
            right
            :value="show"
            @input="$emit('input', $event)"
            mobile-break-point="960"
            v-if="profile"
    >
        <v-container class="px-4">
            <v-row class="justify-center" v-if="profile.levelUpPoints > 0">
                <v-btn class="black--text" color="yellow accent-3" @click="levelUp = true">{{$t('misc.levelUp')}}
                </v-btn>
            </v-row>
            <v-row class="align-center">
                <ProfileImage :class="{'level-up': profile.levelUpPoints}" :picture="profile.picture"
                              :level="profile.level"/>
                <v-col class="px-1">
                    <span class="white--text subtitle-2">{{profile.displayName}}</span>
                    <div>
                        <v-progress-linear height="14" rounded :value="expBar">
                            <span class="body-2">EXP {{this.profile.experience}} / {{this.profile.experienceNeed}}</span>
                        </v-progress-linear>
                    </div>
                </v-col>
                <EditProfile/>
            </v-row>
            <v-row class="score-board" dense>
                <v-col cols="4">
                    <span class="subheading">{{$t('layout.win')}}</span>
                    <div class="title">{{profile.win}}</div>
                </v-col>
                <v-col cols="4">
                    <span class="subheading">{{$t('layout.loss')}}</span>
                    <div class="title">{{profile.loss}}</div>
                </v-col>
                <v-col cols="4">
                    <span class="subheading">{{$t('layout.draw')}}</span>
                    <div class="title">{{profile.draw}}</div>
                </v-col>
                <v-col cols="6">
                    <span class="subheading">{{$t('layout.reputation')}}</span>
                    <div class="title">{{profile.reputation}}</div>
                </v-col>
                <v-col cols="6">
                    <span class="subheading">{{$t('layout.style')}}</span>
                    <div class="title">{{profile.style}}</div>
                </v-col>
            </v-row>
        </v-container>
        <p class="title ml-2 white--text">{{$t('layout.menu')}}</p>
        <v-list dense class="pt-0">
            <v-list-item-group color="primary">
                <v-list-item to="/battles">
                    <v-list-item-icon>
                        <v-icon>{{icons.battle}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>{{$t('layout.sideNav.battles')}}</v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item to="/create-battle">
                    <v-list-item-icon>
                        <v-icon>{{icons.add}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>{{$t('layout.sideNav.create')}}</v-list-item-title>
                    </v-list-item-content>
                    <span>{{profile.hosting}}/{{profile.hostingLimit}}</span>
                </v-list-item>
                <v-list-item to="/find-battle">
                    <v-list-item-icon>
                        <v-icon>{{icons.search}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>{{$t('layout.sideNav.find')}}</v-list-item-title>
                    </v-list-item-content>
                    <span>{{profile.joined}}/{{profile.joinedLimit}}</span>
                </v-list-item>
                <v-list-item to="/tribunal">
                    <v-list-item-icon>
                        <v-icon>{{icons.gavel}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>{{$t('layout.sideNav.tribunal')}}</v-list-item-title>
                    </v-list-item-content>
                    <span>
                    <v-progress-circular v-if="loadingCount" :size="20" :width="2" indeterminate/>
                    <span v-else>{{tribunalCount}}</span>
                </span>
                </v-list-item>
            </v-list-item-group>
        </v-list>
        <v-dialog v-model="levelUp" :persistent="loading" max-width="500px">
            <v-card color="secondary">
                <v-card-title>
                    {{$t('layout.levelUp')}}
                </v-card-title>
                <v-card-text class="py-0">
                    <span v-if="selectedPerk">{{$t(`layout.levelPerks.${selectedPerk.name}.description`)}}</span>
                    <span v-else>{{$t('layout.selectPerk')}}</span>
                    <div class="d-flex flex-wrap justify-center">
                        <v-btn class="ma-1 grey" :class="{'primary': p.selected}" @click="selectPerk(p)"
                               v-for="p in perks"
                               :key="`perk-${p.id}`">
                            {{$t(`layout.levelPerks.${p.name}.title`)}}
                            <v-icon right>{{p.icon}}</v-icon>
                        </v-btn>
                    </div>
                    <p class="ma-0 py-2 text-center" v-if="selectedPerk">
                        {{$t(`layout.levelPerks.${selectedPerk.name}.current`, [profile[selectedPerk.prop]])}}
                    </p>
                </v-card-text>
                <v-card-actions v-if="selectedPerk" class="justify-center">
                    <v-btn color="primary" @click="submitLevelUp" :loading="loading" :disabled="loading">
                        {{$t('misc.select')}}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-navigation-drawer>
</template>


<script>
    import EditProfile from "./modals/EditProfile";
    import {mapActions, mapGetters} from "vuex";
    import {
        mdiClose,
        mdiAccount,
        mdiSwordCross,
        mdiPlus,
        mdiGavel,
        mdiAccountSearch,
        mdiViewGridPlus,
        mdiDoorOpen,
        mdiScale
    } from "@mdi/js";
    import ProfileImage from "../shared/ProfileImage";

    const initialState = () => ({
        levelUp: false,
        loading: false,
        selectedPerk: null,
        perks: [
            {
                id: 0,
                name: 'host',
                icon: mdiViewGridPlus,
                selected: false,
                prop: 'hostingLimit'
            },
            {
                id: 1,
                name: 'guest',
                icon: mdiDoorOpen,
                selected: false,
                prop: 'joinedLimit'
            },
            {
                id: 2,
                name: 'voting',
                icon: mdiScale,
                selected: false,
                prop: 'votingPower'
            }
        ]
    });

    export default {
        data: () => initialState(),
        props: {
            show: {
                required: true
            }
        },
        components: {
            EditProfile,
            ProfileImage
        },
        methods: {
            ...mapActions({
                popup: "DISPLAY_POPUP_DEFAULT",
                refreshProfile: "REFRESH_PROFILE"
            }),
            selectPerk(perk) {
                if (this.selectedPerk) {
                    this.selectedPerk.selected = false;
                }
                perk.selected = true;
                this.selectedPerk = perk;
            },
            submitLevelUp() {
                this.loading = true;
                this.$axios.put('/users/level-up', {
                    type: this.selectedPerk.id
                })
                    .then(({data}) => {
                        this.popup(data);
                    })
                    .catch(err => {
                        this.$logger.error(err)
                    })
                    .then(() => {
                        this.refreshProfile();
                        Object.assign(this.$data, initialState());
                    })
            }
        },
        computed: {
            ...mapGetters({
                profile: "GET_PROFILE",
                tribunalCount: "GET_TRIBUNAL_COUNT",
                loadingCount: "GET_TRIBUNAL_LOADING_COUNT",
            }),
            icons() {
                return {
                    close: mdiClose,
                    account: mdiAccount,
                    battle: mdiSwordCross,
                    add: mdiPlus,
                    search: mdiAccountSearch,
                    gavel: mdiGavel,
                };
            },
            expBar() {
                return (parseInt(this.profile.experience) * 100 / parseInt(this.profile.experienceNeed)).toFixed(0)
            }
        }
    };
</script>

<style lang="scss" scoped>

</style>