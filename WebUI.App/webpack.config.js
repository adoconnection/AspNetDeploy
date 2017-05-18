var path = require('path');
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = {
    context: path.join(__dirname, 'src'),
    entry: "./index",
    output: {
        path: path.join(__dirname.replace("WebUI.App", path.join("WebUI","Resources")), 'App'),
        publicPath: "./",
        filename: "bundle.js"
    },
    devtool: 'source-map',
    module: {
        loaders: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                query: {
                    presets: ['react', 'es2015', 'stage-1']
                }
            },
            {test: /\.css$/, loader: ExtractTextPlugin.extract({ fallback: 'style-loader', use: 'css-loader' }), exclude: /flexboxgrid/},
            {test: /\.(png|jpg|jpeg|gif|woff|woff2)$/, loader: 'url-loader?limit=8192'},
            {test: /\.(otf|eot|ttf)$/, loader: "file-loader?prefix=font/&name=fonts/[name].[ext]"},
            {test: /\.svg$/, loader: "file-loader"},
            {
                test: /\.css$/,
                loader: 'style!css?modules',
                include: /flexboxgrid/,
            }
        ]
    },
    resolve: {
        extensions: ['*', '.js', '.jsx'],
    },
    plugins: [
        new ExtractTextPlugin('bundle.css')
    ]
};