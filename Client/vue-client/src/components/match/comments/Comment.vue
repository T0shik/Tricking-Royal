<template>
    <div>
        <router-link :to="`/user/${comment.displayName}`">
            <ProfileImage size="36px" :picture="comment.picture"></ProfileImage>
            <span class="subtitle px-2 primary--text">{{comment.displayName}}</span>
        </router-link>
        <span class="caption">
            <router-link class="px-1" v-if="comment.taggedUser"
                         :to="`/user/${comment.taggedUser}`">
                @{{comment.taggedUser}}
            </router-link>
            {{comment.message}}
        </span>
    </div>
</template>

<script>
    export default {
        name: "comment",
        props: {
            comment: {
                required: true,
                type: Object
            }
        }
    };
</script>

<style lang="scss" scoped>
    .caption {
        font-size: 13px !important;
    }
</style>