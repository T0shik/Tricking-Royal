<template>
    <div v-resize="onResize">
        <v-parallax
                src="https://cdn.trickingroyal.com/static/main_full.png"
                :height="windowSize.y"
                px-0
        >
            <div class="d-flex flex-column align-center">
                <div>
                    <button v-for="entry in languages" :key="entry.name" @click="setLang(entry.locale)">
                        <flag :iso="entry.icon" v-bind:squared="false"/>
                        {{entry.name}}
                    </button>
                </div>
                <v-spacer/>
                <h1 class="font-weight-bold primary--text mb-3 font-rock"
                    :class="windowSize.x < 600 ? 'display-1' : 'display-3'">Tricking Royal</h1>
                <h4 class="title">{{$t('landing.description')}}</h4>
                <v-btn color="info" @click="login">{{$t('misc.signIn')}}</v-btn>
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
                    <h4 class="text-center subtitle-1">{{$t('landing.partners')}}</h4>
                    <a class="kojos-tricklab" href="https://www.kojostricklab.com/?wpam_id=93" target="_blank_">
                        <img :src="`${process.env.VUE_APP_CDN}/static/kojos-tricklab.png`"/>
                    </a>
                </div>
                <v-spacer/>
                <v-btn large floating icon color="white" @click="$vuetify.goTo($refs.info, options)">
                    <v-icon size="46px">{{iconChevronDown}}</v-icon>
                </v-btn>
            </div>
        </v-parallax>

        <v-row class="mx-2" ref="info">
            <v-col cols="6" sm="6" md="4" v-for="(v, n) in blocks">
                <div class="d-flex flex-column align-center">
                    <div>
                        <v-icon size="50">{{v}}</v-icon>
                    </div>
                    <p class="title mb-0">{{$t(`landing.block.${n}.title`)}}</p>
                    <p class="body-2 text-center">{{$t(`landing.block.${n}.text`)}}</p>
                </div>
            </v-col>
        </v-row>
        <div class="d-flex flex-column align-center" ref="battles">
            <div>
                <h1>{{$t(`landing.battlesTitle`)}}</h1>
            </div>
            <div class="main-card mb-5" v-if="matches && matches.length > 0">
                <MatchPlayer v-for="match in matches" :key="match.key" :disabled="true" :match="match"></MatchPlayer>
                <div class="text-center">
                    <v-btn color="primary" v-if="!endReached" @click="loadMatches">{{$t(`misc.loadMore`)}}</v-btn>
                </div>
            </div>
            <div v-else>{{$t(`landing.battlesEmpty`)}}</div>
        </div>

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
        <Connecting :connecting="connecting"/>
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
        mdiPackageVariant, mdiGithubBox
    } from "@mdi/js";
    import axios from "axios";
    import {languages} from '@/lang/languages.json'
    import {loadLanguageAsync} from "@/plugins/i18n";
    import {STORAGE_KEYS} from "../data/enum";

    export default {
        data: () => ({
            blocks: {
                battles: mdiSwordCross,
                community: mdiAccountGroup,
                tribunal: mdiScaleBalance,
                statistics: mdiChartBar,
                social: mdiEarth,
                contribute: mdiPackageVariant
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
            languages,
        }),
        created() {
            this.onResize();
            this.loadMatches();
        },
        methods: {
            setLang(code) {
                loadLanguageAsync(code);
            },
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
            },
            plugs() {
                return [
                    {
                        name: this.$t(`landing.plugs.social.title`),
                        description: this.$t(`landing.plugs.social.text`),
                        links: [
                            {href: "https://www.facebook.com/trickingroyal", icon: mdiFacebook},
                            {href: "https://www.instagram.com/tricking_royal", icon: mdiInstagram},
                        ]
                    },
                    {
                        name: this.$t(`landing.plugs.support.title`),
                        description: this.$t(`landing.plugs.support.text`),
                        links: [
                            {
                                href: "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WRNCKRU9VZH24&source=url",
                                icon: mdiPaypal
                            },
                            {
                                href: "https://www.patreon.com/raw_coding",
                                icon: mdiPatreon
                            },
                            {href: 'https://github.com/T0shik/Tricking-Royal', icon: mdiGithubBox}
                        ]
                    }
                ]
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