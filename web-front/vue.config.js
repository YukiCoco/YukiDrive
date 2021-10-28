//const HtmlWebpackPlugin = require('html-webpack-plugin')
module.exports = {
  pages: {
    index: {
      // page 的入口
      entry: 'src/main.js',
      // 模板来源
      template: 'public/index.html',
      // 在 dist/index.html 的输出
      filename: 'index.html',
      // 当使用 title 选项时，
      // template 中的 title 标签需要是 <title><%= htmlWebpackPlugin.options.title %></title>
      title: 'Yuki Drive',
      // 在这个页面中包含的块，默认情况下会包含
      // 提取出来的通用 chunk 和 vendor chunk。
      //chunks: ['chunk-vendors', 'chunk-common', 'index']
    },
    initPage: {
      entry: 'src/pages/init/main.js',
      template: 'public/init.html',
      title: '引导页面',
      filename: 'init.html'
    }
  },
  "transpileDependencies": [
    "vuetify"
  ],
  // configureWebpack: {
  //   plugins: [
  //     new HtmlWebpackPlugin({
  //       title: 'Yuki Drive',
  //       template: 'public/index.html'
  //     })
  //   ]
  // }
}