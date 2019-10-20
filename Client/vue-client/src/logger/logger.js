const Logger = {
    log: function () {
        if (process.env.NODE_ENV !== 'production') {
            console.log(...arguments)
        }
    }
};

export default Logger;