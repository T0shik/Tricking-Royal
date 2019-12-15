<template>
    <v-snackbar :value="display" top multi-line right
                @input="hide" @click="hide" timeout="3000">
        <span class="pr-2">{{ message }}</span>
        <v-icon :class="iconType.color">{{iconType.icon}}</v-icon>
    </v-snackbar>
</template>

<script>
    import {mapState, mapMutations} from "vuex";
    import {mdiAlertBox, mdiCheckCircle, mdiShieldAlert} from "@mdi/js";

    export default {
        methods: mapMutations({
            hide: "HIDE_POPUP"
        }),
        computed: {
            ...mapState({
                display: state => state.popup.display,
                message: state => state.popup.message,
                type: state => state.popup.type,
            }),
            iconType() {
                switch (this.type) {
                    case 'success':
                        return {color: 'green--text', icon: mdiCheckCircle};
                    case 'warning':
                        return {color: 'orange--text', icon: mdiAlertBox};
                    case 'error':
                        return {color: 'red--text', icon: mdiShieldAlert};
                    default:
                        return {color: '', icon: null}
                }
            }
        }
    };
</script>
