/// <binding ProjectOpened='Watch - Development' />
const path = require('path')
const MiniCssExtractPlugin = require('mini-css-extract-plugin')

module.exports = {
  mode: 'development',
  devtool: 'source-map',
  entry: './src/calendar.js',
  resolve: {
    extensions: [ '.js' ]
  },
  module: {
    rules: [
      {
        test: /\.css$/,
        use: [
          { loader: MiniCssExtractPlugin.loader },
          { loader: 'css-loader', options: { importLoaders: 1 } }
        ]
      }
    ]
  },
  output: {
    path: path.resolve(__dirname, 'wwwroot'),
    publicPath: '/',
    filename: 'js/calendar.js'
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: 'css/calendar.css'
    })
  ]
}
