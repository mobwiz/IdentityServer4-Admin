// vue.config.js

/**
 * @type {import('@vue/cli-service').ProjectOptions}
 */
module.exports = {
  // 选项...
  publicPath: process.env.NODE_ENV === "production" ? "./" : "./",
  devServer: {
    port: 8088,
    proxy: {
      "/api": {
        target: "http://localhost:4321",
        ws: false,
        secure: false,
        changeOrigin: true,
      },
    },
  },
  pages: {
    index: {
      entry: "src/main.js",
      title: "Identity Admin UI",
    },
  },
};
