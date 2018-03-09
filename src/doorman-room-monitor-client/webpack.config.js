const path = require('path');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

const extractLess = new ExtractTextPlugin({
    filename: '[name].css'
});

module.exports = {
    entry: './src/client/main.js',
    output: {
        path: path.resolve(__dirname, 'dist', 'public'),
        filename: 'main.js'
    },
    module: {
        loaders: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: 'babel-loader',
                query: {
                    presets: ['es2015', 'react']
                }
            },
            {
                test: /(client\.config\.js)|(\.(html)$)/,
                use: {
                    loader: 'file-loader',
                    options: {
                        name: '[name].[ext]'
                    }
                }
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                use: {
                    loader: 'file-loader',
                    options: {
                        publicPath: 'fonts',
                        outputPath: 'fonts',
                        name: '[name].[hash].[ext]'
                    }
                }
            },
            {
                test: /\.less$/,
                use: extractLess.extract({
                    use: [{
                        loader: 'css-loader'
                    }, {
                        loader: 'less-loader'
                    }]
                })
            }
        ]
    },
    plugins: [
        extractLess
    ]
};
