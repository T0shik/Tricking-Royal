<template>
    <v-app>
        <Popup></Popup>
        <UpdateMatch></UpdateMatch>
        <MatchMenu></MatchMenu>
        <Confirmation></Confirmation>
        <Navbar @toggle="sideMenu = !sideMenu"></Navbar>
        <SideMenu :show="sideMenu" @input="toggleMenu"></SideMenu>
        <v-content>
            <v-container fill-height fluid class="px-0 justify-center align-start overflow-x-hidden" id="container">
                <StatusBar></StatusBar>
                <router-view></router-view>
            </v-container>
        </v-content>
    </v-app>
</template>

<script>
    import SideMenu from "./SideMenu";
    import Popup from "./modals/Popup";
    import Confirmation from "./modals/Confirmation";
    import MatchMenu from "./modals/MatchMenu";
    import UpdateMatch from "../match/modals/UpdateMatch";
    import Navbar from "./navigation/Navbar";
    import StatusBar from "./navigation/StatusBar";

    export default {
        data() {
            return {
                sideMenu: null
            };
        },
        components: {
            SideMenu,
            Popup,
            Navbar,
            UpdateMatch,
            MatchMenu,
            Confirmation,
            StatusBar
        },
        methods: {
            toggleMenu(e) {
                this.sideMenu = e;
            }
        }
    };
</script>

<style lang="scss" scoped>
    .v-content {
        max-height: 100vh;
        height: 100vh;

        .container.fill-height {
            overflow: auto;
        }
    }
</style>
