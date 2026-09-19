const fs = require("node:fs/promises");
const path = require("node:path");
const { randomUUID } = require("node:crypto");
const { validateManifest, fingerprintInput } = require("./model.cjs");
const { readRevisionToolchain, acquireToolchain } = require("./toolchains.cjs");
const { runReproduction } = require("./repro.cjs");
const { execute } = require("./repro-process.cjs");
const { hash, inside, requireValue, isolatedEnvironment, fileIdentity } = require("./toolchain-io.cjs");
const policy = require("../../.github/policies/regression-bisect.json");

const trustedRoot = path.resolve(__dirname, "..", "..");
const owned = new WeakSet();
const relative = (root, file) => path.relative(root, file).split(path.sep).join("/");
const untestable = reason => ({ schemaVersion: 1, outcome: "untestable", reason });

function validateRequest(options) {
    validateManifest(options.manifest);
    const r = options.request;
    requireValue(r?.schemaVersion === 1 && r.repository === policy.repository &&
        /^[a-f0-9]{40}$/.test(r.sourceSha), "expected trusted repository and full source SHA");
    requireValue(Object.keys(r).sort().join() ===
        "allocation,approvalDigest,manifestDigest,phase,platform,repository,schemaVersion,sourceSha", "unexpected work request fields");
    requireValue(r.manifestDigest === fingerprintInput(options.manifest) &&
        r.approvalDigest === options.manifest.approvedInputDigest, "frozen manifest/approval digest mismatch");
    requireValue(["main", "older-search", "bisect", "confirmation"].includes(r.phase), "unsupported source phase");
    const p = r.platform;
    requireValue(["windows", "linux", "macos"].includes(p?.os) && ["x64", "arm64"].includes(p.architecture) &&
        (p.rid === `${{ windows: "win", linux: "linux", macos: "osx" }[p.os]}-${p.architecture}` ||
            p.os === "linux" && p.rid === `linux-musl-${p.architecture}`) &&
        options.manifest.matrix.every(c => c.os === p.os && c.architecture === p.architecture), "unapproved platform");
    requireValue(Object.keys(r.allocation).sort().join() === "buildTimeoutMs,workerTimeoutMs" &&
        Number.isSafeInteger(r.allocation.buildTimeoutMs) && r.allocation.buildTimeoutMs > 0 &&
        r.allocation.buildTimeoutMs <= policy.limits.buildTimeoutMinutes * 60000 &&
        Number.isSafeInteger(r.allocation.workerTimeoutMs) &&
        r.allocation.workerTimeoutMs >= r.allocation.buildTimeoutMs &&
        r.allocation.workerTimeoutMs <= policy.limits.workerTimeoutMinutes * 60000, "invalid bounded allocation");
    requireValue(path.isAbsolute(options.gitExecutable), "trusted absolute Git executable required");
}

async function evidence(w, name, value) {
    requireValue(/^[a-z0-9.-]+$/.test(name), "invalid evidence name");
    const bytes = typeof value === "string" || Buffer.isBuffer(value) ? value : JSON.stringify(value, null, 2);
    const file = path.join(w.evidenceRoot, name);
    await fs.writeFile(file, bytes, { flag: "wx" });
    w.evidence.push({ path: `${w.buildInstanceId}/${name}`, sha256: hash(bytes) });
}

function failure(w, stage, error) {
    const code = w?.signal?.aborted ? (w.options.signal?.aborted ? "cancelled" : "timeout") : error.code ?? stage;
    return {
        status: "untestable", outcome: untestable(code === "timeout" ? "timeout" :
            ["checksum-provenance", "network-failure"].includes(code) ? code :
                stage === "sdk" && ["command-failure", "installation-failure"].includes(code) ? "setup-failure" :
                { build: "build-failure", identity: "wrong-identity", sdk: "unavailable-sdk", runtime: "unavailable-runtime" }[stage] ?? "setup-failure"),
        error: { stage, code, message: error.message },
    };
}

async function invoke(w, stage, executable, args, settings = {}, boundary = w.execute) {
    w.signal.throwIfAborted();
    const start = Date.now();
    const entry = { stage, executable, args, cwd: settings.cwd ?? w.sourceDirectory };
    w.commands.push(entry);
    const result = await boundary(executable, args, {
        cwd: entry.cwd, env: w.env, maxBytes: 16 * 1024 * 1024,
        ...settings, signal: w.signal,
        timeoutMs: Math.max(1, Math.min(w.deadline - start, w.options.request.allocation.buildTimeoutMs, settings.timeoutMs ?? Infinity)),
    });
    entry.durationMs = Date.now() - start;
    entry.exitCode = result.exitCode;
    entry.status = result.status ?? "completed";
    await evidence(w, `command-${w.commands.length}.json`, { ...entry, result });
    if (result.exitCode !== 0 || result.status === "timeout" || result.status === "cancelled" || result.complete === false) {
        throw Object.assign(new Error(
            `${stage} command failed (${entry.status}, exit ${result.exitCode}): ${executable} ${args.join(" ")}\n${result.error ?? result.stderr}`),
        { code: result.status === "timeout" || result.status === "cancelled" ? result.status : "command-failure" });
    }
    w.signal.throwIfAborted();
    return result.stdout.trim();
}

const gitArgs = w => ["-c", `core.hooksPath=${path.join(w.root, "hooks")}`, "-c", "core.fsmonitor=false", "-c", "core.longpaths=true",
    "-c", "credential.helper=", "-c", "submodule.recurse=false", "-c", "protocol.allow=never", "-c", "protocol.file.allow=always"];
const git = (w, args) => invoke(w, "source", w.options.gitExecutable, [...gitArgs(w), ...args]);

async function sourceUnchanged(w) {
    const after = hash(await fs.readFile(path.join(w.sourceDirectory, "global.json")));
    w.globalJson.after = after;
    requireValue(after === w.globalJson.before, "historical global.json changed");
    requireValue(await git(w, ["rev-parse", "HEAD"]) === w.actualSourceSha, "historical checkout SHA changed");
    await git(w, ["diff", "--exit-code", "HEAD", "--"]);
}

async function prepareRevisionWorkspace(options, dependencies = {}) {
    let w;
    try {
        validateRequest(options);
        const parent = await fs.realpath(options.scratchDirectory);
        const evidenceParent = await fs.realpath(options.evidenceDirectory);
        const mirror = await fs.realpath(options.mirrorDirectory);
        requireValue(parent !== mirror && !inside(mirror, parent) && !inside(parent, mirror) &&
            parent !== evidenceParent && !inside(parent, evidenceParent), "scratch, mirror and evidence must be separate");
        const buildInstanceId = options.buildInstanceId ?? randomUUID();
        requireValue(/^[A-Za-z0-9-]{1,128}$/.test(buildInstanceId), "invalid reserved build instance");
        const root = path.join(parent, `revision-${buildInstanceId}`);
        await fs.mkdir(root);
        const signal = AbortSignal.any([AbortSignal.timeout(options.request.allocation.workerTimeoutMs),
            ...(options.signal ? [options.signal] : [])]);
        w = {
            options, root, parent, buildInstanceId, signal, started: Date.now(), deadline: Date.now() + options.request.allocation.workerTimeoutMs,
            sourceDirectory: path.join(root, "source"), sampleDirectory: path.join(root, "sample"),
            evidenceRoot: path.join(evidenceParent, buildInstanceId), evidence: [], commands: [],
            execute: dependencies.execute ?? execute, env: isolatedEnvironment(root, options.request.platform),
        };
        owned.add(w);
        Object.assign(w.env, {
            GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(root, "gitconfig"),
            GIT_TERMINAL_PROMPT: "0", GCM_INTERACTIVE: "Never",
            GIT_ALLOW_PROTOCOL: "file", GIT_LFS_SKIP_SMUDGE: "1",
            NUGET_PLUGIN_PATHS: "", NUGET_FALLBACK_PACKAGES: "",
        });
        if (options.request.platform.os === "windows") Object.assign(w.env, {
            OS: "Windows_NT", PATHEXT: ".COM;.EXE;.BAT;.CMD",
            PSModulePath: path.join(w.env.SystemRoot, "System32", "WindowsPowerShell", "v1.0", "Modules"),
        });
        for (const dir of [w.sourceDirectory, w.sampleDirectory, w.evidenceRoot, path.join(root, "hooks"),
            w.env.HOME, w.env.TEMP, w.env.APPDATA, w.env.LOCALAPPDATA, w.env.NUGET_PACKAGES]) await fs.mkdir(dir, { recursive: true });
        await fs.writeFile(w.env.GIT_CONFIG_GLOBAL, "", { flag: "wx" });
        await fs.writeFile(path.join(root, "owner.json"), JSON.stringify({ buildInstanceId, parent }), { flag: "wx" });
        await evidence(w, "request.json", options.request);
        await git(w, ["init", "--template="]);
        await git(w, ["fetch", "--no-tags", "--depth=1", "--no-recurse-submodules", mirror, options.request.sourceSha]);
        const tree = await git(w, ["ls-tree", "-r", "FETCH_HEAD"]);
        requireValue(!tree.split("\n").some(line => /^(120000|160000) /.test(line) ||
            /(?:\t|\/)(?:artifacts|bin|obj|\.dotnet|Bootstrap)\//i.test(line)), "unsupported tracked link/submodule/build output");
        await git(w, ["checkout", "--detach", options.request.sourceSha]);
        w.actualSourceSha = await git(w, ["rev-parse", "HEAD"]);
        requireValue(w.actualSourceSha === options.request.sourceSha, "wrong checked-out source SHA");
        w.globalJson = { before: hash(await fs.readFile(path.join(w.sourceDirectory, "global.json"))) };
        await sourceUnchanged(w);
        for (const source of options.manifest.sources) {
            const file = await fileIdentity(path.join(options.sourceDirectory, source.path), await fs.realpath(options.sourceDirectory));
            requireValue(file.sha256 === source.sha256, "frozen reproduction bytes changed");
            const target = path.join(w.sampleDirectory, source.path);
            await fs.mkdir(path.dirname(target), { recursive: true });
            await fs.copyFile(file.path, target);
        }
        w.signal.throwIfAborted();
        w.status = "prepared";
        return w;
    } catch (error) {
        const result = failure(w, "setup", error);
        return w ? finish(w, result) : result;
    }
}

async function layout(w) {
    const files = ["build.sh", "Build.cmd", "eng/build.sh", "eng/Build.ps1", "eng/build-utils.ps1", "proto.proj", "NuGet.config",
        "src/fsi/fsiProject/fsi.fsproj", "src/fsc/fscProject/fsc.fsproj", "src/FSharp.Core/FSharp.Core.fsproj",
        "src/Microsoft.FSharp.Compiler/Microsoft.FSharp.Compiler.fsproj"];
    const declarations = {};
    for (const name of files) {
        const file = await fileIdentity(path.join(w.sourceDirectory, name), w.sourceDirectory);
        declarations[name] = { sha256: file.sha256, text: await fs.readFile(file.path, "utf8") };
    }
    await evidence(w, "layout.json", declarations);
    const ps = declarations["eng/Build.ps1"].text, sh = declarations["eng/build.sh"].text;
    requireValue(/proto\.proj/.test(declarations["eng/build-utils.ps1"].text) &&
        /Configuration/.test(declarations["proto.proj"].text), "unsupported bootstrap entrypoint");
    const windows = w.options.request.platform.os === "windows";
    requireValue(windows
        ? ["restore", "build", "pack", "bootstrap", "noVisualStudio", "msbuildEngine", "configuration"].every(s => ps.includes(`$${s}`)) &&
            ps.includes("/p:Projects=") && ps.includes("@properties")
        : ["--bootstrap", "--pack", "--product-build", "proto.proj", "Configuration=Proto"].every(s => sh.includes(s)) &&
            /--restore --build/.test(declarations["build.sh"].text), "unsupported historical build entrypoint");
    requireValue(!/<packageSourceCredentials|<config\b|<packageSourceMapping/i.test(declarations["NuGet.config"].text),
        "unsupported historical NuGet configuration");
    for (const [, url] of declarations["NuGet.config"].text.matchAll(/value="(https?:[^"]+)"/g)) {
        const parsed = new URL(url);
        requireValue(parsed.protocol === "https:" && !parsed.username && !parsed.password && !parsed.search &&
            !parsed.hash, "historical feed must be public HTTPS without credentials");
    }
    w.adapter = {
        version: 2, layout: /FSharp\.slnx/.test(windows ? ps : sh) ? "arcade-slnx-products" : "arcade-sln-products",
        mapping: "identity: nested fsi/fsc projects with evaluated artifacts/bin/<product>/Release/<tfm> outputs",
        injection: await Promise.all(["UseLocalCompiler.Directory.Build.props", "UseLocalCompiler.Directory.Build.targets"]
            .map(async name => ({ path: name, sha256: hash(await fs.readFile(path.join(trustedRoot, name))) }))),
        implementationSha256: hash(await fs.readFile(__filename)),
    };
}

async function evaluated(w, project, targetFramework) {
    const properties = ["TargetPath", "TargetFramework", "TargetFrameworks", "PackageId", "PackageVersion", "PackageOutputPath"];
    const args = ["msbuild", path.join(w.sourceDirectory, project), "-nologo", "-p:Configuration=Release",
        `-getProperty:${properties.join(",")}`, ...(targetFramework ? [`-p:TargetFramework=${targetFramework}`] : [])];
    const output = await invoke(w, "evaluation", w.toolchain.hostPath, args);
    const row = JSON.parse(output).Properties;
    requireValue(row && properties.every(p => typeof row[p] === "string"), "unsupported evaluated project properties");
    return row;
}

async function inspect(w, files) {
    const h = w.helper;
    return JSON.parse(await invoke(w, "inspection", h.executable, ["exec", "--fx-version", h.runtimeVersion, h.dll, "inspect", ...files]));
}

async function helper(w) {
    if (w.options.helper) { w.helper = w.options.helper; return; }
    const directory = path.join(w.root, "helper");
    await fs.cp(path.join(__dirname, "evidence"), directory, {
        recursive: true, filter: file => !["bin", "obj"].includes(path.basename(file)),
    });
    const runtime = w.toolchain.compilerRuntime.version;
    const tfm = `net${runtime.split(".")[0]}.0`;
    const output = path.join(directory, "output");
    await fs.writeFile(path.join(directory, "NuGet.Config"),
        '<configuration><packageSources><clear /><add key="public" value="https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json" /></packageSources></configuration>');
    await invoke(w, "helper", w.toolchain.hostPath, ["build", path.join(directory, "Evidence.csproj"),
        "-c", "Release", "-o", output, `-p:EvidenceTargetFramework=${tfm}`, "-nr:false", "--nologo"], { cwd: directory });
    w.helper = { executable: w.toolchain.hostPath, dll: path.join(output, "Evidence.dll"),
        runtimeVersion: runtime, hook: path.join(output, "Repro.StartupHook.dll") };
}

async function inventory(w, rows) {
    const files = [];
    for (const [role, row] of Object.entries(rows)) {
        if (role === "corePackage") {
            requireValue(row.PackageId === "FSharp.Core" && /^[\w.+-]+$/.test(row.PackageVersion), "wrong evaluated Core package identity");
            const location = path.resolve(w.sourceDirectory, row.PackageOutputPath, `FSharp.Core.${row.PackageVersion}.nupkg`);
            const file = await fileIdentity(location, w.sourceDirectory);
            requireValue(file.path === location, "package output redirects through a link");
            files.push({ role, ...file });
            continue;
        }
        const expectedName = role === "fsi" ? "fsi" : "fsc";
        requireValue(row.TargetPath === path.join(w.sourceDirectory, "artifacts", "bin", expectedName, "Release",
            w.targetFramework, `${expectedName}.dll`),
        "unsupported or wrong evaluated product output");
        const file = { role, ...await fileIdentity(row.TargetPath, w.sourceDirectory),
            runtimeConfig: await fileIdentity(row.TargetPath.replace(/\.dll$/, ".runtimeconfig.json"), w.sourceDirectory) };
        requireValue(file.path === row.TargetPath && file.runtimeConfig.path === row.TargetPath.replace(/\.dll$/, ".runtimeconfig.json"),
            "product output redirects through a link");
        requireValue(file.sha256 !== w.toolchain.files[role === "fsi" ? "fsi" : "compiler"].sha256, "stale bundled compiler artifact");
        const config = JSON.parse(await fs.readFile(file.runtimeConfig.path, "utf8")).runtimeOptions;
        requireValue(config?.tfm === w.targetFramework && config.framework?.name === "Microsoft.NETCore.App", "wrong product runtimeconfig");
        const runtimes = w.toolchain.runtimes.filter(r => r.name === config.framework.name &&
            r.version.split(".").slice(0, 2).join(".") === config.framework.version.split(".").slice(0, 2).join(".") &&
            Number(r.version.split(".")[2].split("-")[0]) >= Number(config.framework.version.split(".")[2].split("-")[0]));
        if (runtimes.length !== 1) throw Object.assign(new Error(`missing or ambiguous product runtime: ${config.framework.version}`),
            { code: "unavailable-runtime" });
        requireValue(!w.runtime || w.runtime.version === runtimes[0].version, "compiler and FSI require different runtimes");
        w.runtime = { name: runtimes[0].name, version: runtimes[0].version };
        files.push(file);
    }
    const fscDir = path.dirname(rows.compiler.TargetPath), fsiDir = path.dirname(rows.fsi.TargetPath);
    for (const [role, name] of Object.entries({
        core: "FSharp.Core.dll", tasks: "FSharp.Build.dll", props: "Microsoft.FSharp.NetSdk.props",
        targets: "Microsoft.FSharp.Targets", sdkTargets: "Microsoft.FSharp.NetSdk.targets", overrides: "Microsoft.FSharp.Overrides.NetSdk.targets",
    })) {
        const location = path.join(role === "core" ? fsiDir : fscDir, name);
        const file = await fileIdentity(location, w.sourceDirectory);
        requireValue(file.path === location, "product output redirects through a link");
        files.push({ role, ...file });
    }
    for (const directory of [fsiDir, fscDir]) {
        for (const name of await fs.readdir(directory, { recursive: true })) {
            const location = path.join(directory, name);
            const stat = await fs.lstat(location);
            requireValue(!stat.isSymbolicLink(), "product inventory contains a link");
            if (stat.isDirectory() || files.some(f => f.path === location)) continue;
            files.push({ role: `dependency-${files.length}`, ...await fileIdentity(location, w.sourceDirectory) });
        }
    }
    const selected = files.filter(f => ["fsi", "compiler", "core", "tasks", "corePackage"].includes(f.role));
    const inspected = await inspect(w, selected.map(f => f.path));
    for (const file of selected) {
        const actual = inspected.find(i => path.resolve(i.path) === file.path);
        requireValue(actual?.sha256 === file.sha256, "artifact inspection digest mismatch");
        if (["fsi", "compiler", "core", "tasks"].includes(file.role)) requireValue(actual.assemblyVersion && actual.fileVersion &&
            actual.assemblyName === path.basename(file.path, ".dll"), "artifact assembly identity mismatch");
        Object.assign(file, actual, { role: file.role });
    }
    const pkg = files.find(f => f.role === "corePackage");
    requireValue(pkg.id === "FSharp.Core" && pkg.version === rows.corePackage.PackageVersion &&
        pkg.entries.some(e => /^lib\/[^/]+\/FSharp\.Core\.dll$/.test(e.path) && e.sha256 === files.find(f => f.role === "core").sha256),
    "Core package identity/bytes do not match the built FSI");
    return {
        kind: "source", root: w.sourceDirectory, sourceSha: w.actualSourceSha, buildInstanceId: w.buildInstanceId,
        configuration: "Release", targetFramework: w.targetFramework, runtime: w.runtime,
        corePackageVersion: pkg.version, files,
    };
}

async function buildRevision(w, dependencies = {}) {
    let stage = "setup";
    try {
        requireValue(owned.has(w) && w.status === "prepared", "workspace is not a fresh prepared build instance");
        w.status = "building";
        await sourceUnchanged(w);
        requireValue(await git(w, ["ls-files", "--others", "--exclude-standard"]) === "" &&
            await git(w, ["ls-files", "--others", "--ignored", "--exclude-standard"]) === "",
        "unexpected state in fresh revision checkout");
        stage = "layout";
        await layout(w);
        stage = "sdk";
        const declaration = await readRevisionToolchain({
            policy, sourceDirectory: w.sourceDirectory, sourceSha: w.actualSourceSha, platform: w.options.request.platform,
        });
        requireValue(declaration.status === "resolved", declaration.error?.message ?? "unsupported source toolchain");
        w.declaration = declaration.toolchain;
        await evidence(w, "declarations.json", declaration);
        stage = "sdk";
        const acquisition = await acquireToolchain({
            policy, toolchain: declaration.toolchain, storageDirectory: w.root, helperRoot: trustedRoot, signal: w.signal,
        }, {
            ...dependencies.toolchains,
            run: async (exe, args, settings) => {
                const stdout = await invoke(w, "acquisition", exe, args, settings, dependencies.toolchains?.run ?? w.execute);
                return { exitCode: 0, stdout, stderr: "" };
            },
        });
        await evidence(w, "acquisition.json", acquisition);
        if (acquisition.status !== "acquired") {
            stage = acquisition.error.stage;
            throw Object.assign(new Error(acquisition.error.message), { code: acquisition.error.code });
        }
        w.toolchain = acquisition.identity;
        const oldRoot = w.env.DOTNET_ROOT;
        for (const [key, value] of Object.entries(w.env)) if (value === oldRoot) w.env[key] = w.toolchain.root;
        w.env.DOTNET_HOST_PATH = w.toolchain.hostPath;
        w.env.PATH = `${w.toolchain.root}${path.delimiter}${path.dirname(w.options.gitExecutable)}${path.delimiter}${w.env.PATH.split(path.delimiter).slice(1).join(path.delimiter)}`;
        // Arcade must use the verified installation, never independently select/download a host.
        w.env.DOTNET_INSTALL_DIR = w.toolchain.root;
        const sdkPaths = declaration.toolchain.sdkSelection.paths ?? [];
        for (const sdkPath of sdkPaths.filter(p => p !== "$host$")) {
            const target = path.resolve(w.sourceDirectory, sdkPath);
            requireValue(inside(w.sourceDirectory, target), "unsupported historical SDK search path");
            await fs.symlink(w.toolchain.root, target, w.options.request.platform.os === "windows" ? "junction" : "dir");
        }
        stage = "setup";
        const projects = { fsi: "src/fsi/fsiProject/fsi.fsproj", compiler: "src/fsc/fscProject/fsc.fsproj",
            corePackage: "src/FSharp.Core/FSharp.Core.fsproj" };
        const rows = {};
        for (const [role, project] of Object.entries(projects)) {
            let row = await evaluated(w, project);
            if (role !== "corePackage") {
                const frameworks = (row.TargetFrameworks || row.TargetFramework).split(";").filter(t => /^net\d+\.\d+$/.test(t));
                requireValue(frameworks.length === 1 && (!w.targetFramework || w.targetFramework === frameworks[0]), "unsupported product TFMs");
                w.targetFramework = frameworks[0];
                row = await evaluated(w, project, w.targetFramework);
                try { await fs.access(row.TargetPath); throw new Error("product exists before clean build"); }
                catch (error) { if (error.code !== "ENOENT") throw error; }
            }
            rows[role] = row;
        }
        await evidence(w, "evaluated-products.json", rows);
        stage = "build";
        const productProject = path.join(w.sourceDirectory, "src", "Microsoft.FSharp.Compiler", "Microsoft.FSharp.Compiler.fsproj");
        if (w.options.request.platform.os === "windows") {
            await invoke(w, "build", path.join(w.env.SystemRoot, "System32", "WindowsPowerShell", "v1.0", "powershell.exe"),
                ["-NoProfile", "-ExecutionPolicy", "Bypass", "-File", path.join(w.sourceDirectory, "eng", "Build.ps1"),
                    "-restore", "-build", "-c", "Release", "-pack", "-bootstrap", "-noVisualStudio", "-msbuildEngine", "dotnet",
                    `/p:Projects=${productProject}`]);
        } else {
            await invoke(w, "build", "/bin/bash", [path.join(w.sourceDirectory, "build.sh"),
                "--configuration", "Release", "--pack", "--bootstrap", "--product-build"]);
        }
        // Product graphs can pack dependencies elsewhere; repack the built Core into its evaluated standalone output.
        const packageOutput = path.resolve(w.sourceDirectory, rows.corePackage.PackageOutputPath);
        requireValue(inside(w.sourceDirectory, packageOutput), "Core package output escapes source checkout");
        await invoke(w, "build", w.toolchain.hostPath, ["pack", path.join(w.sourceDirectory, projects.corePackage),
            "--configuration", "Release", "--no-build", "--no-restore", "--output", packageOutput]);
        stage = "identity";
        await sourceUnchanged(w);
        for (const [role, project] of Object.entries(projects)) {
            const row = await evaluated(w, project, role === "corePackage" ? undefined : w.targetFramework);
            requireValue(fingerprintInput(row) === fingerprintInput(rows[role]), "evaluated product identity changed during build");
        }
        stage = "setup";
        await helper(w);
        stage = "identity";
        w.product = await inventory(w, rows);
        await evidence(w, "product.json", w.product);
        w.signal.throwIfAborted();
        w.status = "built";
        return { status: "built", product: w.product, toolchain: w.toolchain,
            buildInstanceId: w.buildInstanceId, commands: w.commands, evidence: w.evidence };
    } catch (error) {
        const result = { ...failure(w, error.code === "unavailable-runtime" ? "runtime" : stage, error),
            buildInstanceId: w?.buildInstanceId, commands: w?.commands, evidence: w?.evidence };
        return owned.has(w) && w.signal.aborted ? finish(w, result) : result;
    }
}

function finish(w, result) {
    return w.finalization ??= finalize(w, result);
}

async function finalize(w, result) {
    if (w.globalJson) {
        try { await sourceUnchanged(w); }
        catch (error) {
            result = { ...failure(w, "identity", error), ...(result.error ? { priorError: result.error } : {}) };
        }
    }
    const logs = path.join(w.sourceDirectory, "artifacts", "log");
    let names = [];
    try {
        names = await fs.readdir(logs, { recursive: true });
    } catch (error) {
        if (error.code !== "ENOENT") throw error;
    }
    for (const name of names.filter(n => /\.(binlog|log)$/i.test(n))) {
        const file = await fileIdentity(path.join(logs, name), w.sourceDirectory);
        await evidence(w, `build-log-${w.evidence.length}${path.extname(name).toLowerCase()}`, await fs.readFile(file.path));
        w.evidence.at(-1).sourcePath = relative(w.root, file.path);
    }
    const outcome = {
        ...result, buildInstanceId: w.buildInstanceId, actualSourceSha: w.actualSourceSha,
        requested: w.options.request, adapter: w.adapter, globalJson: w.globalJson,
        toolchain: w.toolchain ? { sdkVersion: w.toolchain.sdkVersion, requested: w.toolchain.requested, archive: w.toolchain.archive,
            acquisitionDigest: fingerprintInput(w.toolchain) } : w.declaration,
        product: w.product, commands: w.commands, evidence: w.evidence, durationMs: Date.now() - w.started,
    };
    let written = false;
    for (;;) {
        if (w.signal.aborted && !outcome.scratchRemoved) {
            Object.assign(outcome, failure(w, "finalization", w.signal.reason));
            requireValue(owned.has(w) && path.dirname(w.root) === w.parent &&
                path.basename(w.root) === `revision-${w.buildInstanceId}` &&
                !(await fs.lstat(w.root)).isSymbolicLink() && await fs.realpath(w.root) === w.root &&
                JSON.parse(await fs.readFile(path.join(w.root, "owner.json"))).buildInstanceId === w.buildInstanceId,
            "refusing cleanup outside owned resolved scratch tree");
            await fs.rm(w.root, { recursive: true });
            outcome.scratchRemoved = true;
        }
        const bytes = JSON.stringify(outcome, null, 2);
        await fs.writeFile(path.join(w.evidenceRoot, "observation.json"), bytes, { flag: written ? "w" : "wx" });
        written = true;
        // An abort during evidence I/O must invalidate the persisted observation too.
        if (w.signal.aborted && !outcome.scratchRemoved) continue;
        w.evidence.push({ path: `${w.buildInstanceId}/observation.json`, sha256: hash(bytes) });
        return outcome;
    }
}

async function testSourceRevision(options, dependencies = {}) {
    const w = await prepareRevisionWorkspace(options, dependencies);
    if (w.status !== "prepared") return w;
    const built = await buildRevision(w, dependencies);
    if (built.status !== "built") return finish(w, built);
    try {
        const reproduction = await runReproduction({
            manifest: options.manifest, sourceDirectory: w.sampleDirectory, workDirectory: path.join(w.root, "executions"),
            product: w.product, toolchain: options.reproductionToolchain ?? w.toolchain, helper: w.helper,
            sampleRuntimes: options.sampleRuntimes,
        }, {
            execute: (exe, args, settings) => w.execute(exe, args, {
                ...settings, signal: w.signal, timeoutMs: Math.max(1, Math.min(settings.timeoutMs, w.deadline - Date.now())),
            }),
        });
        w.signal.throwIfAborted();
        await evidence(w, "reproduction.json", reproduction);
        return finish(w, { status: "tested", outcome: reproduction.outcome, reproduction });
    } catch (error) { return finish(w, failure(w, "reproduction", error)); }
}

module.exports = { prepareRevisionWorkspace, buildRevision, testSourceRevision };
