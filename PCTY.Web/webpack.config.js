let path = require('path');
let terserPlugin = require('terser-webpack-plugin');
let webpack = require('webpack');
let vueloader = require('vue-loader');
let cssloader = require('css-loader')
let sassloader = require('sass-loader')
let PROD = (process.env.NODE_ENV === 'production');

var plugins = [
  new vueloader.VueLoaderPlugin()
];

if (PROD) {
  plugins.push(new terserPlugin({
    sourceMap: true,
    include: /\.min\.js$/
  }));
}

module.exports = {
  entry: {
    '3rdparty': './scripts/3rdparty.js',
    'main': './scripts/main.js',
    'styles-default': './styles/default.scss',
    'login-loader': './scripts/apps/login-loader.js',
    'home-loader': './scripts/apps/home-loader.js',
  },
  mode: PROD ? 'production' : 'development',
  devtool: 'source-map',
  output: {
    filename: '[name].min.js',
    path: path.resolve(__dirname, 'wwwroot/dist')
  },
  resolve: {
    alias: {
      'vue$': 'vue/dist/vue.esm.js',
      'vue-router$': 'vue-router/dist/vue-router.esm.js',
      'bootstrap-vue$': 'bootstrap-vue/dist/bootstrap-vue.min.js',
      'bootstrap-vue.css$': 'bootstrap-vue/dist/bootstrap-vue.css',
      'bootstrap$': 'bootstrap/scss/bootstrap.scss'
    }
  },
  plugins: plugins,
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: 'vue-loader',
        exclude: /(node_modules)/,
        options: {
          loaders: {
            'css': [
              'vue-style-loader',
              'css-loader'
            ],
            'scss': [
              'vue-style-loader',
              'css-loader',
              'sass-loader'
            ],
            'sass': [
              'vue-style-loader',
              'css-loader',
              'sass-loader?indentedSyntax'
            ]
          }
        }
      },
      {
        test: /(\.css)$/,
        use: [
          { loader: 'vue-style-loader' },
          { loader: 'css-loader' }
        ],
        include: /(styles|bootstrap|v-suggestions|components|apps)/
      },
      {
        test: /(\.scss)$/,
        use: [
          { loader: 'vue-style-loader' },
          { loader: 'css-loader' },
          { loader: 'sass-loader' }
        ],
        include: /(styles|awesome|components|apps)/
      },
      {
        test: /\.(ttf|eot|svg|woff(2)?)(\?[a-z0-9=&.]+)?$/,
        use: [
          { loader: 'file-loader', options: {
            name: 'dist/fonts/[name].[ext]'
          }},
        ],
        include: /(awesome)/
      },
      {
        test: /\.js$/,
        exclude: /(node_modules)/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [ "@babel/preset-env" ],
            cacheDirectory: true
          }
        }
      }
    ]
  },
  optimization: {
    splitChunks: {}
  }
};
