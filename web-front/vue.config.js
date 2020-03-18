const HtmlWebpackPlugin = require('html-webpack-plugin')
module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  configureWebpack: {
    plugins: [
      new HtmlWebpackPlugin({
        title: 'Yuki Drive',
        template: 'public/index.html'
      })
    ]
  }
}