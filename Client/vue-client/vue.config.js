module.exports = {
    devServer: {
        public: 'https://localhost:8080'
    },
    pwa: {
        workboxPluginMode: 'InjectManifest',
        workboxOptions: {
            swSrc: 'src/service-worker.js',
        }
    }
}