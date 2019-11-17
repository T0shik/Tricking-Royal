import Vue from 'vue'
import ProfileImage from "./ProfileImage";

[
    ProfileImage,
].forEach(c => {
    Vue.component(c.name, c);
});