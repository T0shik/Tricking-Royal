import Vue from 'vue';
import VueI18n from 'vue-i18n';
import axios from "axios";
import english from "@/lang/en/_index"

Vue.use(VueI18n);

export const i18n = new VueI18n({
    locale: 'en', 
    fallbackLocale: 'en',
    messages: {
        'en': english
    }
});

const loadedLanguages = ['en'];

function setI18nLanguage (lang) {
    i18n.locale = lang;
    axios.defaults.headers.common['Accept-Language'] = lang;
    document.querySelector('html').setAttribute('lang', lang);
    return lang;
}

export function loadLanguageAsync(lang) {
    if (i18n.locale === lang) {
        return Promise.resolve(setI18nLanguage(lang))
    }

    if (loadedLanguages.includes(lang)) {
        return Promise.resolve(setI18nLanguage(lang))
    }

    return import(/* webpackChunkName: "lang-[request]" */ `@/lang/${lang}/_index.js`).then(
        messages => {
            console.log("Messages:", messages, lang);
            i18n.setLocaleMessage(lang, messages.default);
            loadedLanguages.push(lang);
            return setI18nLanguage(lang)
        }
    )
}