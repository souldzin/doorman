var path = require('path');

module.exports = {
    entry: "./src/client/main.js",
    output: {
        path: path.resolve(__dirname, 'dist', 'public'),
        filename: 'main.js'
    },
    module: {
        loaders: [
            {
                test: /client-config(\..*)?.js$/,
                use: {
                    loader: 'file-loader',
                    options: {
                        name: "client-config.[ext]"
                    }
                }
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: "babel-loader",
                query: {
                    presets: ['es2015', 'react']
                }
            },
            {
                test: /\.(html)$/,
                use: {
                    loader: 'file-loader',
                    options: {
                        name: "[name].[ext]"
                    }
                }
            }
        ]
    }
};
