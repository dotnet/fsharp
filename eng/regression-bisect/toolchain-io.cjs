const fs = require("node:fs/promises");
const path = require("node:path");
const { createHash } = require("node:crypto");
const { execFile } = require("node:child_process");

const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const requireValue = (condition, message) => { if (!condition) throw new Error(message); };
const inside = (root, file) => {
    const relative = path.relative(root, file);
    return relative !== "" && !relative.startsWith(`..${path.sep}`) && relative !== ".." && !path.isAbsolute(relative);
};

function publicUrl(url, policy) {
    const parsed = new URL(url);
    requireValue(parsed.protocol === "https:" && !parsed.username && !parsed.password && !parsed.search &&
        !parsed.hash && !parsed.port && policy.toolchains.publicFeeds.some(feed =>
            parsed.href === feed || parsed.href.startsWith(`${feed}/`)), `unapproved public download URL: ${url}`);
    return url;
}

function createHttp(policy, fileSystem = fs, signal) {
    return async (url, { destination, method = "GET" } = {}) => {
        const redirects = [];
        let current = publicUrl(url, policy);
        for (let hop = 0; hop <= 5; hop++) {
            const response = await fetch(current, {
                method, redirect: "manual",
                signal: signal ? AbortSignal.any([signal, AbortSignal.timeout(300000)]) : AbortSignal.timeout(300000),
            });
            if ([301, 302, 303, 307, 308].includes(response.status)) {
                await response.body?.cancel();
                const next = publicUrl(new URL(response.headers.get("location"), current).href, policy);
                redirects.push({ from: current, to: next, status: response.status });
                current = next;
                continue;
            }
            const sha256 = createHash("sha256");
            const sha512 = createHash("sha512");
            const chunks = [];
            let size = 0;
            const file = destination && response.ok ? await fileSystem.open(destination, "wx") : null;
            try {
                for await (const chunk of response.body ?? []) {
                    size += chunk.length;
                    requireValue(size <= (file ? 1024 * 1024 * 1024 : 16 * 1024 * 1024), `download exceeds byte limit: ${current}`);
                    sha256.update(chunk); sha512.update(chunk);
                    if (file) await file.writeFile(chunk);
                    else chunks.push(chunk);
                }
            } finally {
                await file?.close();
            }
            return {
                status: response.status, url: current, redirects,
                ...(file ? {} : { bytes: Buffer.concat(chunks) }),
                sha256: sha256.digest("hex"), sha512: sha512.digest("hex"), size,
            };
        }
        throw new Error(`too many redirects: ${url}`);
    };
}

function run(executable, args, { cwd, env, timeoutMs = 120000, signal } = {}) {
    return new Promise((resolve, reject) => {
        execFile(executable, args, { cwd, env, timeout: timeoutMs, signal, maxBuffer: 4 * 1024 * 1024, windowsHide: true },
            (error, stdout, stderr) => {
                if (error && (error.killed || typeof error.code !== "number")) reject(error);
                else resolve({ stdout, stderr, exitCode: error?.code ?? 0 });
            });
    });
}

function isolatedEnvironment(root, platform) {
    const home = path.join(root, "home");
    const dotnet = path.join(root, "dotnet");
    const systemRoot = process.env.SystemRoot || "C:\\Windows";
    const systemPath = platform.os === "windows"
        ? [path.join(systemRoot, "System32"), systemRoot, path.join(systemRoot, "System32", "WindowsPowerShell", "v1.0")].join(path.delimiter)
        : "/usr/bin:/bin:/usr/sbin:/sbin";
    return {
        ...(platform.os === "windows" ? { SystemRoot: systemRoot, WINDIR: systemRoot, ComSpec: path.join(systemRoot, "System32", "cmd.exe") } : {}),
        PATH: `${dotnet}${path.delimiter}${systemPath}`,
        HOME: home, USERPROFILE: home, APPDATA: path.join(home, "AppData", "Roaming"), LOCALAPPDATA: path.join(home, "AppData", "Local"),
        TMP: path.join(root, "tmp"), TEMP: path.join(root, "tmp"), TMPDIR: path.join(root, "tmp"),
        DOTNET_ROOT: dotnet, [`DOTNET_ROOT_${platform.architecture.toUpperCase()}`]: dotnet,
        DOTNET_INSTALL_DIR: dotnet, DOTNET_GLOBAL_INSTALL_DIR: dotnet,
        DOTNET_HOST_PATH: path.join(dotnet, platform.os === "windows" ? "dotnet.exe" : "dotnet"),
        DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR: dotnet,
        DOTNET_CLI_HOME: home, DOTNET_MULTILEVEL_LOOKUP: "0", DOTNET_ROLL_FORWARD: "Disable",
        DOTNET_CLI_TELEMETRY_OPTOUT: "1", DOTNET_SKIP_FIRST_TIME_EXPERIENCE: "1", DOTNET_NOLOGO: "1",
        DOTNET_CLI_UI_LANGUAGE: "en-US", DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE: "true",
        DOTNET_INSTALL_SKIP_TAR: "1", MSBUILDDISABLENODEREUSE: "1",
        NUGET_PACKAGES: path.join(root, "nuget", "packages"), NUGET_HTTP_CACHE_PATH: path.join(root, "nuget", "http"),
        NUGET_PLUGINS_CACHE_PATH: path.join(root, "nuget", "plugins"),
        NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED: "false", LANG: "C.UTF-8",
    };
}

async function fileIdentity(file, root, fileSystem = fs) {
    const real = await fileSystem.realpath(file);
    requireValue(inside(root, real), `identity path escapes isolated installation: ${real}`);
    const bytes = await fileSystem.readFile(real);
    return { path: real, sha256: hash(bytes) };
}

// No reproduction is executed: this fixed probe only inspects the acquired product.
const probe = `
open System
open System.IO
open System.Reflection
open System.Diagnostics
open System.Runtime.InteropServices
open System.IO.Compression
open System.Xml.Linq
printfn "ARCH|%s" (RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant())
printfn "RUNTIME|%s" (Path.GetFileName(Path.GetDirectoryName(typeof<obj>.Assembly.Location)))
for file in fsi.CommandLineArgs |> Array.skip 1 do
    if Path.GetExtension(file) = ".nupkg" then
        use archive = ZipFile.OpenRead(file)
        let entry = archive.Entries |> Seq.find (fun e -> e.FullName.EndsWith(".nuspec", StringComparison.Ordinal))
        use stream = entry.Open()
        let xml = XDocument.Load(stream)
        let value name = xml.Descendants() |> Seq.find (fun e -> e.Name.LocalName = name) |> fun e -> e.Value
        printfn "PACKAGE|%s|%s|%s" (Path.GetFileName(file)) (value "id") (value "version")
    else
        let assembly = AssemblyName.GetAssemblyName(file)
        let info = FileVersionInfo.GetVersionInfo(file)
        printfn "FILE|%s|%s|%O|%s" (Path.GetFileName(file)) assembly.Name assembly.Version info.FileVersion
`;

async function capture(request, deps) {
    const { root, executable, cwd, env, toolchain } = request;
    const command = async args => {
        const result = await deps.run(executable, args, { cwd, env, timeoutMs: 120000 });
        requireValue(result.exitCode === 0, `${args.join(" ")} failed (${result.exitCode}): ${result.stderr || result.stdout}`);
        return result.stdout.trim();
    };
    const info = await command(["--info"]);
    const sdkList = await command(["--list-sdks"]);
    const runtimeList = await command(["--list-runtimes"]);
    const sdkVersion = await command(["--version"]);
    const sdkBasePath = path.resolve(/^\s*Base Path:\s*(.+)/m.exec(info)?.[1]?.trim() ?? "");
    requireValue(sdkBasePath === path.join(root, "sdk", toolchain.sdkVersion), "SDK resolver selected a path outside the acquired SDK");
    const sdks = sdkList.split(/\r?\n/).filter(Boolean).map(line => {
        const match = /^(\S+) \[(.+)\]$/.exec(line);
        requireValue(match, `unrecognized SDK identity: ${line}`);
        return { version: match[1], path: match[2] };
    });
    const runtimes = [];
    for (const line of runtimeList.split(/\r?\n/).filter(Boolean)) {
        const match = /^(\S+) (\S+) \[(.+)\]$/.exec(line);
        requireValue(match, `unrecognized runtime identity: ${line}`);
        const [name, version, location] = match.slice(1);
        const marker = name === "Microsoft.NETCore.App" ? "System.Private.CoreLib.dll"
            : name === "Microsoft.AspNetCore.App" ? "Microsoft.AspNetCore.dll" : "PresentationCore.dll";
        const file = await fileIdentity(path.join(location, version, marker), root, deps.fs);
        runtimes.push({ name, version, path: await deps.fs.realpath(location), sha256: file.sha256 });
    }
    const fsharp = path.join(root, "sdk", toolchain.sdkVersion, "FSharp");
    const files = {};
    for (const [name, file] of Object.entries({ compiler: "fsc.dll", fsi: "fsi.dll", core: "FSharp.Core.dll" })) {
        files[name] = await fileIdentity(path.join(fsharp, file), root, deps.fs);
    }
    const config = JSON.parse(await deps.fs.readFile(path.join(fsharp, "fsi.runtimeconfig.json"), "utf8"));
    const framework = config.runtimeOptions?.framework;
    requireValue(framework?.name === "Microsoft.NETCore.App", "unsupported bundled FSI runtimeconfig");
    const runtime = runtimes.find(r => r.name === framework.name && r.version === toolchain.bundledRuntime.version);
    requireValue(runtime, "bundled compiler runtime is missing");
    const probeFile = path.join(env.DOTNET_CLI_HOME, "identity.fsx");
    await deps.fs.writeFile(probeFile, probe, { flag: "wx" });
    let packageFile = null;
    const libraryPacks = path.join(fsharp, "library-packs");
    try {
        const names = (await deps.fs.readdir(libraryPacks)).filter(n => /^FSharp\.Core\..+\.nupkg$/i.test(n));
        requireValue(names.length === 1, "ambiguous bundled FSharp.Core package");
        packageFile = await fileIdentity(path.join(libraryPacks, names[0]), root, deps.fs);
    } catch (error) {
        if (error.code !== "ENOENT") throw error;
    }
    // Pin the runtime explicitly; SDKs often declare a lower minimum patch.
    const probeOutput = await command(["exec", "--fx-version", runtime.version, files.fsi.path,
        "--nologo", "--exec", probeFile, ...Object.values(files).map(f => f.path), ...(packageFile ? [packageFile.path] : [])]);
    const architecture = /^ARCH\|(.+)$/m.exec(probeOutput)?.[1]?.trim();
    const compilerRuntime = { name: framework.name, version: /^RUNTIME\|(.+)$/m.exec(probeOutput)?.[1]?.trim() };
    const runtimeExecutions = [];
    for (const required of toolchain.runtimeRequirements.filter(r => r.name === "Microsoft.NETCore.App" && ["compiler", "fsi"].includes(r.role))) {
        requireValue(hash(await deps.fs.readFile(required.config.path)) === required.config.sha256, "runtimeconfig changed after acquisition request");
        requireValue(required.config.path.endsWith(".runtimeconfig.json"), "unsupported local compiler runtimeconfig name");
        const compiler = await deps.fs.realpath(required.config.path.replace(/\.runtimeconfig\.json$/, ".dll"));
        requireValue(inside(toolchain.sourceDirectory, compiler), "local compiler path escapes historical source");
        const traceDirectory = await deps.fs.mkdtemp(path.join(env.DOTNET_CLI_HOME, `${required.role}-`));
        const trace = path.join(traceDirectory, "host-trace.txt");
        const result = await deps.run(executable, ["exec", "--fx-version", required.version, compiler, "--help"], {
            cwd, env: { ...env, COREHOST_TRACE: "1", COREHOST_TRACEFILE: trace, DOTNET_HOST_TRACE: "1", DOTNET_HOST_TRACEFILE: trace },
            timeoutMs: 120000,
        });
        requireValue(result.exitCode === 0, `local ${required.role} identity probe failed (${result.exitCode}): ${result.stderr}`);
        const output = await deps.fs.readFile(trace, "utf8");
        const expected = path.join(root, "shared", required.name, required.version);
        const selected = [...output.matchAll(/Chose FX version \[([^\]]+)\]/g)].map(m => path.normalize(m[1]));
        requireValue(selected.includes(expected), `local ${required.role} host trace did not verify ${expected}`);
        runtimeExecutions.push({
            role: required.role, configPath: required.config.path, configDigest: required.config.sha256, name: required.name,
            version: required.version, architecture, hostTrace: { path: trace, sha256: hash(output) },
            compiler: { path: compiler, sha256: hash(await deps.fs.readFile(compiler)) },
        });
    }
    for (const file of Object.values(files)) {
        const line = probeOutput.split(/\r?\n/).find(l => l.startsWith(`FILE|${path.basename(file.path)}|`));
        requireValue(line, `assembly identity absent: ${file.path}`);
        [, , file.assemblyName, file.assemblyVersion, file.fileVersion] = line.split("|");
    }
    const depsFile = await fileIdentity(path.join(fsharp, "fsi.deps.json"), root, deps.fs);
    const packages = JSON.parse(await deps.fs.readFile(depsFile.path, "utf8")).libraries;
    const corePackage = Object.keys(packages).find(p => p.startsWith("FSharp.Core/")) ?? null;
    files.core.packageIdentity = corePackage;
    if (packageFile) {
        const packageLine = probeOutput.split(/\r?\n/).find(l => l.startsWith(`PACKAGE|${path.basename(packageFile.path)}|`));
        requireValue(packageLine, "bundled FSharp.Core nuspec identity absent");
        const [, , id, version] = packageLine.split("|");
        requireValue(id === "FSharp.Core" && /^\d+\.\d+\.\d+(?:[-+].+)?$/.test(version), "invalid FSharp.Core package identity");
        files.core.packageIdentity = `${id}/${version}`;
        files.core.package = { ...packageFile, id, version };
    }
    const host = await fileIdentity(executable, root, deps.fs);
    const rid = /^\s*RID:\s*(\S+)/m.exec(info)?.[1];
    let runtimeGraph;
    if (rid !== toolchain.platform.rid) {
        runtimeGraph = await fileIdentity(path.join(sdkBasePath, "RuntimeIdentifierGraph.json"), root, deps.fs);
        runtimeGraph.bytesBase64 = (await deps.fs.readFile(runtimeGraph.path)).toString("base64");
    }
    return {
        root, hostPath: host.path, hostSha256: host.sha256, sdkVersion, sdkBasePath, sdks, runtimes,
        architecture, rid, compilerRuntime, runtimeExecutions, files, depsFile,
        ...(runtimeGraph ? { runtimeGraph } : {}),
        evidence: { info, sdkList, runtimeList, probeOutput },
    };
}

module.exports = { hash, requireValue, inside, publicUrl, createHttp, run, isolatedEnvironment, fileIdentity, capture };
