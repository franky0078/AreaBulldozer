const { RawSource } = require("webpack").sources;

exports.CSSPresencePlugin = class CSSPresencePlugin {
  apply(compiler) {
    compiler.hooks.compilation.tap("CSSPresencePlugin", (compilation) => {
      compilation.hooks.processAssets.tap(
        {
          name: "CSSPresencePlugin",
          stage: compilation.PROCESS_ASSETS_STAGE_ADDITIONS,
        },
        () => {
          const cssFiles = Object.keys(compilation.assets).filter((asset) =>
            asset.endsWith(".css")
          );
          const hasCSS = cssFiles.length > 0;

          for (const chunk of compilation.chunks) {
            for (const file of chunk.files) {
              if (!file.endsWith(".mjs")) {
                continue;
              }

              const asset = compilation.getAsset(file);
              if (!asset) {
                continue;
              }

              const rawSource = asset.source.source();
              const source = Buffer.isBuffer(rawSource)
                ? rawSource.toString("utf8")
                : String(rawSource);

              const updatedSource = source.replace(
                "export {",
                `const hasCSS = ${hasCSS}; export { hasCSS, `
              );

              compilation.updateAsset(file, new RawSource(updatedSource));
            }
          }
        }
      );
    });
  }
};
