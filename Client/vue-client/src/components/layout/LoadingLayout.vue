<template>
    <v-app>
        <div class="d-flex flex-column justify-center align-center fill-height">
            <div>
                <img :src="logo">
            </div>
            <v-progress-linear indeterminate></v-progress-linear>
        </div>
    </v-app>
</template>

<script>
    export default {
        data: () => ({
            logo: `${process.env.VUE_APP_CDN}/static/main_192x192.png`
        })
    }
</script>

<style scoped>

</style>