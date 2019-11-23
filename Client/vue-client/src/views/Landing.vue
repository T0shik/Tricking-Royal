<template>
    <div v-resize="onResize">
        <v-parallax
                src="https://cdn.trickingroyal.com/static/main_full.png"
                :height="windowSize.y"
                px-0
        >
            <v-layout align-center column justify-center>
                <v-spacer></v-spacer>
                <h1 class="font-weight-bold primary--text mb-3 font-rock"
                    :class="windowSize.x < 600 ? 'display-1' : 'display-3'">Tricking Royal</h1>
                <h4 class="title">Online Tricking Battles</h4>
                <v-btn color="info" @click="login">Sign In</v-btn>
                <!--                <a-->
                <!--                        class="google-link"-->
                <!--                        href="https://play.google.com/store/apps/details?id=aw.trickingroyal&pcampaignid=MKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1"-->
                <!--                >-->
                <!--                    <img-->
                <!--                            alt="Get it on Google Play"-->
                <!--                            src="https://play.google.com/intl/en_gb/badges/images/generic/en_badge_web_generic.png"-->
                <!--                    />-->
                <!--                </a>-->
                <div class="pt-4">
                    <h4 class="text-center subtitle-1">Partners</h4>
                    <a class="kojos-tricklab" href="https://www.kojostricklab.com/?wpam_id=93" target="_blank_">
                        <img :src="logos.kojo"/>
                    </a>
                </div>
                <v-spacer></v-spacer>
                <v-btn large floating icon color="white" @click="$vuetify.goTo($refs.info, options)">
                    <v-icon size="46px">{{iconChevronDown}}</v-icon>
                </v-btn>
                <v-spacer></v-spacer>
            </v-layout>
        </v-parallax>

        <v-row class="mx-2" ref="info">
            <v-col cols="6" sm="6" md="4">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{icons.battles}}</v-icon>
                    </div>
                    <p class="title mb-0">Battles</p>
                    <p class="body-2 text-center">
                        Battle trickers from accross the world in 3 different battle modes: One Up, Three Round Pass,
                        Copy Cat.
                    </p>
                </div>
            </v-col>
            <v-col cols="6" sm="6" md="4">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{icons.community}}</v-icon>
                    </div>
                    <p class="title mb-0">Community</p>
                    <p class="body-2 text-center">
                        Meet new trickers, advance your skills and help the tricking comunity grow stronger.
                    </p>
                </div>
            </v-col>
            <v-col cols="6" sm="6" md="4">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{icons.tribunal}}</v-icon>
                    </div>
                    <p class="title mb-0">Tribunal</p>
                    <p class="body-2 text-center">
                        Be the judge! The comunity votes on who wins battles and who is breaking the rules.
                    </p>
                </div>
            </v-col>
            <v-col cols="6" sm="6" md="4">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{icons.social}}</v-icon>
                    </div>
                    <p class="title mb-0">Social</p>
                    <p class="body-2 text-center">
                        Share your social media channels, so people can follow you.
                    </p>
                </div>
            </v-col>
            <v-col cols="6" sm="6" md="4">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{icons.stats}}</v-icon>
                    </div>
                    <p class="title mb-0">Statistics</p>
                    <p class="body-2 text-center">
                        Track your win-loss history, gain repuatation and gradually build up your level to unlock new
                        perks.
                    </p>
                </div>
            </v-col>
            <v-col cols="6" sm="6" md="4">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{icons.open}}</v-icon>
                    </div>
                    <p class="title mb-0">Contribute</p>
                    <p class="body-2 text-center">
                        TrickingRoyal is an open source project. Visit the <a href="https://github.com/T0shik/Tricking-Royal">github project</a>
                        page to find out more!
                    </p>
                </div>
            </v-col>
        </v-row>

        <v-layout mb-5 column align-center justify-center ref="battles">
            <div>
                <h1>New Way to Battle</h1>
            </div>
            <div class="main-card" v-if="matches && matches.length > 0">
                <MatchPlayer v-for="match in matches" :key="match.key" :disabled="true" :match="match"></MatchPlayer>
                <div class="text-center">
                    <v-btn color="primary" v-if="!endReached" @click="loadMatches">load more</v-btn>
                </div>
            </div>
            <div v-else>No Matches to show</div>
        </v-layout>

        <v-footer class="py-4 white--text flex-column" color="secondary">
            <div v-for="p in plugs" :key="p.name" class="text-center">
                <h1 class="title">{{p.description}}</h1>
                <div>
                    <v-btn color="white" v-for="link in p.links" :href="link.href" target="_blank" :key="link.href"
                           class="mx-3" icon>
                        <v-icon size="24px">{{link.icon}}</v-icon>
                    </v-btn>
                </div>
            </div>
            <div>
                &copy;2019 —
                <strong>Tricking Royal</strong>
            </div>
        </v-footer>
        <Connecting :connecting="connecting"></Connecting>
    </div>
</template>


<script>
    import MatchPlayer from "@/components/match/MatchPlayer";
    import Connecting from "@/components/layout/modals/Connecting";
    import {
        mdiChevronDown,
        mdiPaypal,
        mdiPatreon,
        mdiFacebook,
        mdiInstagram,
        mdiSwordCross,
        mdiAccountGroup,
        mdiScaleBalance,
        mdiChartBar,
        mdiEarth,
        mdiPackageVariant
    } from "@mdi/js";
    import axios from "axios";

    export default {
        data: () => ({
            icons: {
                battles: mdiSwordCross,
                community: mdiAccountGroup,
                tribunal: mdiScaleBalance,
                stats: mdiChartBar,
                social: mdiEarth,
                open: mdiPackageVariant
            },
            logos: {
                kojo: `${process.env.VUE_APP_CDN}/static/kojos-tricklab.png`
            },
            connecting: false,
            heightSet: false,
            windowSize: {
                x: 0,
                y: 0
            },
            matches: [],
            loading: true,
            index: 0,
            endReached: false,
            plugs: [
                {
                    name: "Social",
                    description: "Love the project? Check out our social media!",
                    links: [
                        {href: "https://www.facebook.com/trickingroyal", icon: mdiFacebook},
                        {href: "https://www.instagram.com/tricking_royal", icon: mdiInstagram}
                    ]
                },
                {
                    name: "Support",
                    description: "Help support this project!",
                    links: [
                        {href: "paypallink", icon: mdiPaypal},
                        {
                            href: "https://www.patreon.com/raw_coding",
                            icon: mdiPatreon
                        }]
                }
            ]
        }),
        created() {
            this.onResize();
            this.loadMatches();
        },
        methods: {
            loadMatches() {
                axios.get(`/anon/matches?index=${this.index}`).then(res => {
                    if (res.data && res.data.length === 0) this.endReached = true;
                    this.matches = this.matches.concat(res.data);
                    this.loading = false;
                    this.index++;
                });
            },
            login() {
                this.connecting = true;
                this.$store.state.userMgr.signinRedirect();
            },
            onResize() {
                if (this.heightSet) {
                    this.windowSize = {x: window.innerWidth, y: this.windowSize.y};
                } else {
                    this.heightSet = true;
                    this.windowSize = {x: window.innerWidth, y: window.innerHeight};
                }
            }
        },
        computed: {
            options() {
                return {
                    duration: 800,
                    offset: -10,
                    easing: "easeInCubic"
                };
            },
            iconChevronDown() {
                return mdiChevronDown;
            }
        },
        components: {
            MatchPlayer,
            Connecting
        }
    };
</script>

<style lang="scss" scoped>
    .v-parallax__content {
        padding: 0 0 !important;
        height: 100vh;
    }

    .font-rock {
        text-shadow: 0 0 8px black;
    }

    .google-link img {
        height: 65px;
    }

    .kojos-tricklab {
        display: flex;
        background-color: #fff;
        border-radius: 8px;
        padding: 0.5rem;

        img {
            height: 36px;
        }
    }
</style>