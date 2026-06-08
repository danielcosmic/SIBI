const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  pages: {
    index: {
      entry: 'src/main.js',
      title: 'SIBI'
    }
  },
  devServer: {
    proxy: {
      '/api': {
        target: 'http://localhost:5025',
        secure: false,
        changeOrigin: true
      }
    }
  }
})
