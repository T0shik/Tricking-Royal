<template>
    <div>
        <router-link :to="`/user/${comment.displayName}`">
            <ProfileImage :size="36" :picture="comment.picture"/>
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
    import ProfileImage from "@/components/shared/ProfileImage";
    
    export default {
        name: "comment",
        props: {
            comment: {
                required: true,
                type: Object
            }
        },
        components: {
            ProfileImage
        }
    };
</script>

<style lang="scss" scoped>
    .caption {
        font-size: 13px !important;
    }
</style>