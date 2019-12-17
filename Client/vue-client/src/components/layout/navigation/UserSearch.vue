<template>
    <v-autocomplete
            class="px-2 mw300"
            :search-input.sync="search"
            :items="users"
            :loading="loading"
            :label="$t('layout.search.label')"
            item-avatar="picture"
            item-text="displayName"
            item-value="displayName"
            hide-details
            hide-no-data
            clearable
            @change="selectUser"
    >
        <template v-slot:item="data">
            <template>
                <div class="d-flex align-center justify-center w100">
                    <ProfileImage :picture="data.item.picture" :size="'36px'" />
                    <span class="ml-2 body-2">{{data.item.displayName}}</span>
                    <span class="level body-2">{{data.item.level}}</span>
                </div>
            </template>
        </template>
    </v-autocomplete>
</template>

<script>
    import ProfileImage from "../../shared/ProfileImage";

    export default {
        data: () => ({
            users: [],
            search: "",
            loading: false,
            timeout: null
        }),
        watch: {
            search: function (v) {
                if (v === undefined || v === null || v === "") return;
                if (this.timeout !== null) clearTimeout(this.timeout);

                this.loading = true;
                this.timeout = setTimeout(
                    function () {
                        this.$axios
                            .get(`/users?search=${v}`)
                            .then(({data}) => {
                                this.users = data;
                                this.loading = false;
                            })
                    }.bind(this),
                    500
                );
            }
        },
        methods: {
            selectUser(displayName) {
                if (displayName && displayName !== this.$route.params.id) {
                    this.$store.dispatch("SET_USER", {
                        router: this.$router,
                        displayName: displayName,
                        redirect: true
                    });
                }
            },
        },
        components: {
            ProfileImage
        }
    }
</script>

<style scoped lang="scss">
    .mw300 {
        max-width: 300px;
    }

    .w100 {
        width: 100%;
    }

    .level {
        background-color: #000;
        padding: 0 4px;
        margin-left: auto;
        border-radius: 5px;
        box-shadow: 0 1px 2px 1px #777;
    }
</style>