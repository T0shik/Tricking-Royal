import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import {Scroll} from 'vuetify/lib/directives'

Vue.use(Vuetify, {
    directives: {
        Scroll
    }
});

export default new Vuetify({
    icons: {
        iconfont: 'mdiSvg',
    },
    theme: {
        dark: true,
        themes: {
            dark: {
                primary: '#9a48ab',
                secondary: '#30343f',
                success: '#212121',
                beginner: '#00e6e5',
                intermediate: '#059e02',
                skilled: '#189cea',
                expert: '#b31e0f',
                master: '#8509d2',
                goat: '#efbe00',
                white: '#F8F8FF',
            }
        }
    }
});
