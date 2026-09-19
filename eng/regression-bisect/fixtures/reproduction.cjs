const fs = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const { hash } = require("../toolchain-io.cjs");
const { manifest, seal, archiveReceipt } = require("./contracts.cjs");

async function fixture(t, { kind = "source", executable = false, tfms = ["net10.0"], configuration = "Release" } = {}) {
    const root = await fs.realpath(await fs.mkdtemp(path.join(os.tmpdir(), "repro-contract-")));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const file = async (relative, contents = relative) => {
        const location = path.join(root, relative);
        await fs.mkdir(path.dirname(location), { recursive: true });
        await fs.writeFile(location, contents);
        return { path: await fs.realpath(location), sha256: hash(contents) };
    };
    const sdkRoot = path.join(root, "dotnet");
    const host = await file("dotnet/dotnet.exe");
    const sdkBasePath = path.join(sdkRoot, "sdk", "10.0.100");
    const runtime = await file("dotnet/shared/Microsoft.NETCore.App/10.0.0/System.Private.CoreLib.dll");
    const productRoot = kind === "source" ? path.join(root, "local") : path.join(sdkBasePath, "FSharp");
    const prefix = path.relative(root, productRoot);
    const files = [];
    for (const [role, name] of Object.entries({
        fsi: executable ? "fsi.exe" : "fsi.dll", compiler: "fsc.dll", tasks: "FSharp.Build.dll",
        props: "Microsoft.FSharp.NetSdk.props", targets: "Microsoft.FSharp.Targets",
        sdkTargets: "Microsoft.FSharp.NetSdk.targets", overrides: "Microsoft.FSharp.Overrides.NetSdk.targets",
        core: "FSharp.Core.dll", corePackage: "FSharp.Core.10.0.100.nupkg",
    })) {
        files.push({ role, ...await file(path.join(prefix, name)), ...(/\.(dll|exe)$/.test(name) ? { assemblyVersion: "10.0.0.0" } : {}) });
    }
    const config = await file(path.join(prefix, "fsi.runtimeconfig.json"), JSON.stringify({
        runtimeOptions: { tfm: "net10.0", framework: { name: "Microsoft.NETCore.App", version: "10.0.0" } },
    }));
    files.find(f => f.role === "fsi").runtimeConfig = config;
    const m = manifest();
    m.sources = [{ path: "repro.fsx", ...(await file("source/repro.fsx", 'printfn "ok"')) }];
    m.sources = m.sources.map(s => ({ path: "repro.fsx", sha256: s.sha256 }));
    m.packages = [];
    m.producer = { name: "no-producer", version: "0.0.0", sha256: hash("") };
    m.matrix = tfms.map((tfm, i) => ({
        id: `cell-${i}`, os: "windows", architecture: "x64", project: null, tfm, configuration,
        languageVersion: "10.0", command: ["fsi", "repro.fsx"],
    }));
    seal(m);
    const product = {
        kind, root: productRoot, sourceSha: "a".repeat(40), buildInstanceId: "clean-build",
        runtime: { name: "Microsoft.NETCore.App", version: "10.0.0" }, files,
        corePackageVersion: "10.0.100",
    };
    const toolchain = {
        root: sdkRoot, hostPath: host.path, hostSha256: host.sha256, sdkVersion: "10.0.100", sdkBasePath,
        architecture: "x64", rid: "win-x64",
        runtimes: [{ name: "Microsoft.NETCore.App", version: "10.0.0",
            path: path.dirname(path.dirname(runtime.path)), sha256: runtime.sha256 }],
    };
    const url = "https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.100/dotnet-sdk-10.0.100-win-x64.zip";
    toolchain.archive = archiveReceipt({ name: "sdk.zip", url, rid: "win-x64", architecture: "x64",
        provenance: { kind: "publisher-checksum" } }, (await file("sdk.zip", "archive")).path, "archive");
    toolchain.archive.sha512 = toolchain.archive.actualSha512;
    toolchain.requested = {
        kind: "released", platform: { os: "windows", architecture: "x64", rid: "win-x64" },
        installationRoot: sdkRoot, sdkVersion: "10.0.100", toolsVersion: "10.0.100",
        bundledRuntime: { version: "10.0.0" }, runtimeRequirements: [], archive: toolchain.archive,
    };
    toolchain.sdks = [{ version: "10.0.100", path: path.join(sdkRoot, "sdk") }];
    toolchain.compilerRuntime = { name: "Microsoft.NETCore.App", version: "10.0.0" };
    toolchain.files = {};
    for (const [name, assemblyName] of Object.entries({ compiler: "fsc", fsi: "fsi", core: "FSharp.Core" })) {
        toolchain.files[name] = {
            ...await file(path.join("dotnet", "sdk", "10.0.100", "FSharp", `${assemblyName}.dll`)),
            assemblyName, assemblyVersion: "10.0.0.0", fileVersion: "10.0.0",
        };
    }
    return { root, file, product, toolchain, manifest: m, sourceDirectory: path.join(root, "source") };
}

module.exports = { fixture };
