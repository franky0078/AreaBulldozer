const fs = require("fs");
const path = require("path");
const MOD = require("./mod.json");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const { CSSPresencePlugin } = require("./tools/css-presence");

function resolveCsiiUserDataPath() {
  const configuredPath = process.env.CSII_USERDATAPATH?.trim();

  if (configuredPath) {
    return configuredPath;
  }

  // Visual Studio does not always inherit user environment variables that
  // were created after Visual Studio was started. On Windows, use the normal
  // Cities: Skylines II user-data location as a reliable fallback.
  const userProfile = process.env.USERPROFILE?.trim();

  if (process.platform === "win32" && userProfile) {
    const fallbackPath = path.join(
      userProfile,
      "AppData",
      "LocalLow",
      "Colossal Order",
      "Cities Skylines II"
    );

    console.warn(
      `[AreaBulldozer] CSII_USERDATAPATH is not set. Using fallback: ${fallbackPath}`
    );

    return fallbackPath;
  }

  throw new Error(
    "CSII_USERDATAPATH is not set and the Cities: Skylines II user-data path could not be determined."
  );
}

const CSII_USERDATAPATH = resolveCsiiUserDataPath();
const OUTPUT_DIR = path.join(CSII_USERDATAPATH, "Mods", MOD.id);

fs.mkdirSync(OUTPUT_DIR, { recursive: true });

const banner = `
 * Cities: Skylines II UI Module
 *
 * Id: ${MOD.id}
 * Author: ${MOD.author}
 * Version: ${MOD.version}
 * Dependencies: ${MOD.dependencies.join(",")}
`;

module.exports = {
  mode: "production",
  stats: {
    preset: "errors-warnings",
    errorDetails: true,
    moduleTrace: true,
  },
  entry: {
    [MOD.id]: "./src/index.tsx",
  },
  externalsType: "window",
  externals: {
    react: "React",
    "react-dom": "ReactDOM",
    "cs2/modding": "cs2/modding",
    "cs2/api": "cs2/api",
    "cs2/l10n": "cs2/l10n",
    "cs2/bindings": "cs2/bindings",
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: {
          loader: "ts-loader",
          options: {
            // CS2 ships its own UI type declarations. They can temporarily
            // differ from the installed game/toolchain version. Transpile the
            // production bundle independently; use `npm run typecheck` when
            // explicit TypeScript diagnostics are needed.
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
              url: true,
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
      "mod.json": path.resolve(__dirname, "mod.json"),
    },
  },
  output: {
    filename: "[name].mjs",
    path: OUTPUT_DIR,
    library: {
      type: "module",
    },
    publicPath: "coui://ui-mods/",
    clean: false,
  },
  optimization: {
    minimize: true,
    minimizer: [
      new TerserPlugin({
        extractComments: {
          banner: () => banner,
        },
      }),
    ],
  },
  experiments: {
    outputModule: true,
  },
  plugins: [
    new MiniCssExtractPlugin(),
    new CSSPresencePlugin(),
    {
      apply(compiler) {
        compiler.hooks.done.tap("AreaBulldozerBuildResult", (stats) => {
          console.log(stats.toString({ colors: true, errorDetails: true }));
          console.log(`[AreaBulldozer] UI output: ${OUTPUT_DIR}`);
        });
      },
    },
  ],
};
