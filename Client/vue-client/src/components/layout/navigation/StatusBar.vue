<template>
    <div class="sys-bar" :class="{'hidden': display}">
        <span>{{message}}</span>
    </div>
</template>

<script>
    import {mapState} from 'vuex'

    export default {
        computed: mapState('statusBar', ["display", "message"])
    }
</script>

<style scoped lang="scss">
    .sys-bar {
        position: absolute;
        text-align: center;
        top: 0;
        left: 0;
        z-index: 10;
        background-color: #0a0a0a;
        width: 100%;

        .hidden {
            display: none;
        }
    }
</style>