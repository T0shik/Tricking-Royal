module.exports = {
    devServer: {
        public: 'https://localhost:8080'
    },
    pwa: {
        name: 'Tricking Royal',
        appleMobileWebAppCapable: 'yes',
        appleMobileWebAppStatusBarStyle: '#9a48ab',
        msTileColor: '#9a48ab',
        themeColor: '#9a48ab',
        iconPaths: {
            favicon16: 'https://cdn.trickingroyal.com/static/main_16x16.png',
            favicon32: 'https://cdn.trickingroyal.com/static/main_32x32.png',
            appleTouchIcon: 'https://cdn.trickingroyal.com/static/main_152x152.png',
            msTileImage: 'https://cdn.trickingroyal.com/static/main_144x144.png'
        },
        workboxOptions: {
            cacheId: 'tr-v1',
            skipWaiting: true,
        },
    }
};