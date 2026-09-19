const fs = require("node:fs/promises");
const path = require("node:path");
const { resolveLatestStableSdk, acquireToolchain } = require("./toolchains.cjs");

async function main() {
    const args = process.argv.slice(2);
    if (args.length !== 3 || args[0] !== "--run" || args[1] !== "--storage") {
        throw new Error("Opt in with: node eng\\regression-bisect\\toolchains.smoke.cjs --run --storage <existing-temporary-directory>");
    }
    const storage = await fs.realpath(args[2]);
    const temporary = await fs.mkdtemp(path.join(storage, "fsharp-sdk-smoke-"));
    const os = { win32: "windows", linux: "linux", darwin: "macos" }[process.platform];
    const architecture = process.arch;
    const platform = { os, architecture, rid: `${{ windows: "win", linux: "linux", macos: "osx" }[os]}-${architecture}` };
    const policy = JSON.parse(await fs.readFile(path.resolve(__dirname, "..", "..", ".github", "policies", "regression-bisect.json")));
    const options = { policy, platform, signal: AbortSignal.timeout(600000) };
    let report;
    try {
        const resolved = await resolveLatestStableSdk(options);
        if (resolved.status !== "resolved") {
            report = resolved;
        } else {
            const result = await acquireToolchain({ ...options, toolchain: resolved.toolchain, storageDirectory: temporary });
            if (result.status !== "acquired") {
                report = { ...result, requestedSdk: resolved.toolchain.sdkVersion };
            } else {
                const identity = result.identity;
                report = {
                    status: result.status, requestedSdk: resolved.toolchain.sdkVersion, resolvedSdk: identity.sdkVersion,
                    platform, frozenAt: resolved.toolchain.frozenAt,
                    metadata: resolved.toolchain.metadata.map(({ bytesBase64, ...record }) => record),
                    archive: identity.archive, installers: identity.installations.map(i => i.installer),
                    hostPath: identity.hostPath, hostSha256: identity.hostSha256,
                    architecture: identity.architecture, rid: identity.rid, runtimes: identity.runtimes,
                    compilerRuntime: identity.compilerRuntime, files: identity.files, evidence: identity.evidence,
                };
            }
        }
    } finally {
        // Only the uniquely created child is ours, never the caller's storage root.
        const real = await fs.realpath(temporary);
        if (path.dirname(real) !== storage || !path.basename(real).startsWith("fsharp-sdk-smoke-")) {
            throw new Error(`refusing cleanup of unexpected storage: ${real}`);
        }
        await fs.rm(real, { recursive: true, force: true });
    }
    console.log(JSON.stringify({ ...report, cleaned: true }, null, 2));
    process.exitCode = report.status === "acquired" ? 0 : 2;
}

main().catch(error => {
    console.error(JSON.stringify({ status: "untestable", error: error.message }));
    process.exitCode = 2;
});
