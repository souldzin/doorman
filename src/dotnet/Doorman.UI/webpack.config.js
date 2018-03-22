var path = require('path');

module.exports = {
    entry: {
        main: './Scripts/main'
    },
    externals: {
        jquery: 'jQuery'
    },
    output: {
        publicPath: '/js/',
        path: path.join(__dirname, '/wwwroot/js/'),
        filename: 'main.bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.mustache$/,
                loader: 'mustache-loader'
            }
        ]
    }
};
