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
        <v-container class="px-5">
            <v-row class="justify-center" v-if="profile.levelUpPoints > 0">
                <v-btn class="black--text" color="yellow accent-3" @click="levelUp = true">Level Up</v-btn>
            </v-row>
            <v-row class="align-center">
                <profile-img :class="{'level-up': profile.levelUpPoints}" :picture="profile.picture"
                             :level="profile.level"></profile-img>
                <v-col class="px-1">
                    <span class="white--text subheading">{{profile.displayName}}</span>
                    <div>
                        <v-progress-linear height="14" rounded :value="expBar">
                            <span class="body-2">EXP {{this.profile.experience}} / {{this.profile.experienceNeed}}</span>
                        </v-progress-linear>
                    </div>
                </v-col>
                <EditProfile></EditProfile>
            </v-row>
            <v-row class="score-board" dense>
                <v-col cols="4">
                    <span class="subheading">Wins</span>
                    <div class="title">{{profile.win}}</div>
                </v-col>
                <v-col cols="4">
                    <span class="subheading">Loss</span>
                    <div class="title">{{profile.loss}}</div>
                </v-col>
                <v-col cols="4">
                    <span class="subheading">Draw</span>
                    <div class="title">{{profile.draw}}</div>
                </v-col>
                <v-col cols="6">
                    <span class="subheading">Reputation</span>
                    <div class="title">{{profile.reputation}}</div>
                </v-col>
                <v-col cols="6">
                    <span class="subheading">Style</span>
                    <div class="title">{{profile.style}}</div>
                </v-col>
            </v-row>
        </v-container>
        <p class="title ml-2 white--text">Menu</p>
        <v-list dense class="pt-0">
            <v-list-item-group color="primary">
                <v-list-item to="/battles">
                    <v-list-item-icon>
                        <v-icon>{{icons.battle}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>Battles</v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item to="/create-battle">
                    <v-list-item-icon>
                        <v-icon>{{icons.add}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>Create Battle</v-list-item-title>
                    </v-list-item-content>
                    <span>{{profile.hosting}}/{{profile.hostingLimit}}</span>
                </v-list-item>
                <v-list-item to="/find-battle">
                    <v-list-item-icon>
                        <v-icon>{{icons.search}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>Find Battle</v-list-item-title>
                    </v-list-item-content>
                    <span>{{profile.joined}}/{{profile.joinedLimit}}</span>
                </v-list-item>
                <v-list-item to="/tribunal">
                    <v-list-item-icon>
                        <v-icon>{{icons.gavel}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>Tribunal</v-list-item-title>
                    </v-list-item-content>
                    <span>
                    <v-progress-circular v-if="loadingCount" :size="20" :width="2" indeterminate></v-progress-circular>
                    <span v-else>{{tribunalCount}}</span>
                </span>
                </v-list-item>
            </v-list-item-group>
        </v-list>
        <v-dialog v-model="levelUp" :persistent="loading" max-width="500px">
            <v-card color="secondary">
                <v-card-title>
                    Level Up
                </v-card-title>
                <v-card-text>
                    <span v-if="selectedPerk">{{selectedPerk.description}}</span>
                    <span v-else>Select your perk.</span>
                </v-card-text>
                <v-card-text class="d-flex flex-wrap justify-center">
                    <v-btn class="ma-1 grey" :class="{'primary': p.selected}" @click="selectPerk(p)" v-for="p in perks"
                           :key="`perk-${p.id}`">
                        {{p.name}}
                        <v-icon right>{{p.icon}}</v-icon>
                    </v-btn>
                </v-card-text>
                <v-card-actions v-if="selectedPerk" class="justify-center">
                    <v-btn color="primary" @click="submitLevelUp" :loading="loading" :disabled="loading">select
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-navigation-drawer>
</template>


<script>
    import EditProfile from "../user/EditProfile";
    import {mapActions, mapGetters} from "vuex";

    import {
        mdiClose,
        mdiAccount,
        mdiSwordCross,
        mdiPlus,
        mdiGavel,
        mdiAccountSearch
    } from "@mdi/js";
    import perks from "../../data/perks";

    const initialState = () => ({
        levelUp: false,
        perks: perks.map(x => ({
            ...x,
            selected: false
        })),
        loading: false
    });

    export default {
        props: {
            show: {
                required: true
            }
        },
        components: {
            EditProfile
        },
        data: initialState,
        methods: {
            ...mapActions({
                popup: "DISPLAY_POPUP_DEFAULT",
                refreshProfile: "REFRESH_PROFILE"
            }),
            selectPerk(newPerk) {
                for (let i = 0; i < this.perks.length; i++) {
                    this.perks[i].selected = newPerk.id === this.perks[i].id;
                }
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
                        console.error(err)
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
            selectedPerk() {
                return this.perks.filter(x => x.selected)[0];
            },
            expBar() {
                return (parseInt(this.profile.experience) * 100 / parseInt(this.profile.experienceNeed)).toFixed(0)
            }
        }
    };
</script>

<style lang="scss" scoped>

</style>