const Logger = {
    log: function () {
        if (process.env.NODE_ENV !== 'production') {
            console.log(...arguments)
        }
    },
    error: function () {
        if (process.env.NODE_ENV !== 'production') {
            console.error(...arguments)
        }
    }
};

export default Logger;