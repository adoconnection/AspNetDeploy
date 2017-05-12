var path = require('path');
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
    context: path.join(__dirname, 'src'),
    entry: "./index",
    output: {
        path: path.join(__dirname, 'assets'),
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
            {test: /\.css$/, loader: ExtractTextPlugin.extract('style-loader', 'css-loader'), exclude: /flexboxgrid/},
            {test: /\.(png|jpg|jpeg|gif|woff|woff2)$/, loader: 'url?limit=8192'},
            {test: /\.(otf|eot|ttf)$/, loader: "file?prefix=font/&name=fonts/[name].[ext]"},
            {test: /\.svg$/, loader: "file"},
            {
                test: /\.css$/,
                loader: 'style!css?modules',
                include: /flexboxgrid/,
            }
        ]
    },
    resolve: {
        extensions: ['', '.js', '.jsx'],
    },
    plugins: [
        new webpack.optimize.DedupePlugin(),
        new ExtractTextPlugin('bundle.css'),
        new CopyWebpackPlugin(
            [
                {
                    from: '.././assets',
                    to: '../../WebUI/Resources/SinglePageApplicationAssets',
                    force: true,
                    copyUnmodified: true
                }
            ]
        )
    ]
};