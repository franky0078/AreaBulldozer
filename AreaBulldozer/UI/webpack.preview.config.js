const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const PREVIEW_OUTPUT = path.resolve(__dirname, "dist-preview");

module.exports = {
  mode: "development",
  devtool: "source-map",
  stats: {
    preset: "errors-warnings",
    errorDetails: true,
    moduleTrace: true,
  },
  entry: {
    preview: "./src/preview.tsx",
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: {
          loader: "ts-loader",
          options: {
            transpileOnly: true,
          },
        },
        exclude: /node_modules/,
      },
      {
        test: /\.s?css$/,
        include: path.join(__dirname, "src"),
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: "css-loader",
            options: {
              // coui:// resources only exist inside the game. Keep their URLs
              // untouched so the browser can fall back to normal system fonts.
              url: {
                filter: (url) => !url.startsWith("coui://"),
              },
              importLoaders: 1,
              modules: {
                auto: true,
                exportLocalsConvention: "camelCase",
                localIdentName: "[local]_[hash:base64:3]",
              },
            },
          },
          "sass-loader",
        ],
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
    modules: ["node_modules", path.join(__dirname, "src")],
    alias: {
      "cs2/api$": path.resolve(__dirname, "src/preview/cs2-api.ts"),
      "cs2/l10n$": path.resolve(__dirname, "src/preview/cs2-l10n.ts"),
      "mod.json$": path.resolve(__dirname, "mod.json"),
    },
  },
  output: {
    filename: "preview.js",
    path: PREVIEW_OUTPUT,
    publicPath: "/",
    clean: true,
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: "preview.css",
    }),
  ],
  devServer: {
    host: "127.0.0.1",
    port: 8080,
    open: true,
    hot: false,
    liveReload: true,
    compress: true,
    static: {
      directory: path.resolve(__dirname, "preview"),
      watch: true,
    },
    devMiddleware: {
      publicPath: "/",
      writeToDisk: false,
    },
    client: {
      overlay: {
        errors: true,
        warnings: false,
      },
    },
  },
};
