import Vue from 'vue'
import Router from 'vue-router'
import {store} from './stores/store'
import Landing from "./views/Landing";
import SkillSelection from "./views/SkillSelection";
import Battles from "./views/Battles";
import Watch from "./views/Watch";
import Tribunal from "./views/Tribunal";
import Create from "./views/Create";
import Find from "./views/Find";
import User from "./views/User";

Vue.use(Router);

const router = new Router({
    mode: 'history',
    base: process.env.BASE_URL,
    routes: [
        {
            path: '/',
            name: 'landing',
            meta: {
                auth: false,
                profile: false,
            },
            component: Landing, // () => import(/* webpackChunkName: "landing" */ './views/Landing.vue')
        },
        {
            path: '/skill-selection',
            name: 'skill',
            meta: {
                auth: true,
                profile: false,
            },
            component: SkillSelection, // () => import(/* webpackChunkName: "skill" */ './views/SkillSelection.vue')
        },
        {
            path: '/battles/:type?/:id?',
            name: 'battles',
            alias: "/index.html",
            meta: {
                auth: true,
                profile: true,
            },
            component: Battles, // () => import(/* webpackChunkName: "battle" */ './views/Battles.vue')
        },
        {
            path: '/watch/:id',
            name: 'watch',
            meta: {
                auth: false,
                profile: false,
            },
            component: Watch, // () => import(/* webpackChunkName: "watch" */ './views/Watch.vue')
        },
        {
            path: '/tribunal/:type?/:id?',
            name: 'tribunal',
            meta: {
                auth: true,
                profile: true,
            },
            component: Tribunal, // () => import(/* webpackChunkName: "tribunal" */ './views/Tribunal.vue')
        },
        {
            path: '/create-battle',
            name: 'create',
            meta: {
                auth: true,
                profile: true,
            },
            component: Create, // () => import(/* webpackChunkName: "create" */ './views/Create.vue')
        },
        {
            path: '/find-battle',
            name: 'find',
            meta: {
                auth: true,
                profile: true,
            },
            component: Find, // () => import(/* webpackChunkName: "find" */ './views/Find.vue')
        },
        {
            path: '/user/:id',
            name: 'user',
            meta: {
                auth: true,
                profile: true,
            },
            component: User, // () => import(/* webpackChunkName: "user" */ './views/User.vue')
        },
    ]
});

router.beforeEach((to, from, next) => {
    function proceed() {
        if (to.meta.auth === undefined) {
            return next();
        }
        if (!store.getters.AUTHENTICATED && to.meta.auth) {
            localStorage.setItem("RETURN_PATH", to.path);
            store.dispatch("SIGN_IN");
        } else if (store.getters.AUTHENTICATED && !to.meta.auth) {
            return to.name === 'watch'
                ? next()
                : next('/battles')
        }
        if (to.meta.profile !== undefined) {
            const user = store.getters.GET_PROFILE;
            if (to.meta.profile && user && !user.activated) {
                return next('/skill-selection')
            } else if (!to.meta.profile && user && user.activated) {
                return next('/battles')
            }
        }
        return next()
    }

    if (!store.state.appReady) {
        store.watch(
            (state) => state.appReady,
            (value) => {
                if (value) {
                    proceed()
                }
            }
        )
    } else {
        proceed()
    }
});

export default router;