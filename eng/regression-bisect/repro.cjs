const fs = require("node:fs/promises");
const path = require("node:path");
const { execute } = require("./repro-process.cjs");
const { validateManifest, classifyAttempts, fingerprintInput } = require("./model.cjs");
const { verifyToolchainIdentity } = require("./toolchains.cjs");
const { verifyRetainedArchive } = require("./toolchain-installer.cjs");
const { hash, inside, requireValue, isolatedEnvironment } = require("./toolchain-io.cjs");
const policy = require("../../.github/policies/regression-bisect.json");

const untestable = reason => ({ schemaVersion: 1, outcome: "untestable", reason });
const role = (product, name) => {
    const files = product.files.filter(f => f.role === name);
    requireValue(files.length === 1, `missing or ambiguous ${name} artifact`);
    return files[0];
};
const samePath = (a, b) => process.platform === "win32"
    ? path.resolve(a).toLowerCase() === path.resolve(b).toLowerCase() : path.resolve(a) === path.resolve(b);

async function checkedFile(record, root) {
    requireValue(record && path.isAbsolute(record.path), "expected absolute artifact path");
    const real = await fs.realpath(record.path);
    requireValue(inside(await fs.realpath(root), real), `artifact escapes expected root: ${record.path}`);
    requireValue(hash(await fs.readFile(real)) === record.sha256, `artifact bytes changed: ${record.path}`);
    return { ...record, path: real };
}

async function checkedRuntime(toolchain, required) {
    const matches = toolchain.runtimes.filter(r => r.name === required.name && r.version === required.version);
    requireValue(matches.length === 1, `missing or ambiguous runtime ${required.name} ${required.version}`);
    const runtime = matches[0];
    requireValue(samePath(runtime.path, path.join(toolchain.root, "shared", runtime.name)), "wrong runtime root");
    await checkedFile({ path: path.join(runtime.path, runtime.version, "System.Private.CoreLib.dll"), sha256: runtime.sha256 }, toolchain.root);
    return runtime;
}

async function selectFsiCommand({ product, toolchain, cell }) {
    requireValue(["source", "released"].includes(product.kind), "unknown product kind");
    requireValue(toolchain.architecture === cell.architecture, "wrong host architecture");
    requireValue(toolchain.rid.startsWith({ windows: "win-", linux: "linux-", macos: "osx-" }[cell.os]), "wrong host OS");
    const host = await checkedFile({ path: toolchain.hostPath, sha256: toolchain.hostSha256 }, toolchain.root);
    requireValue(samePath(host.path, path.join(toolchain.root, cell.os === "windows" ? "dotnet.exe" : "dotnet")), "wrong selected host");
    const fsi = await checkedFile(role(product, "fsi"), product.root);
    const core = await checkedFile(role(product, "core"), product.root);
    requireValue(fsi.assemblyVersion && core.assemblyVersion, "missing FSI/Core assembly versions");
    if (product.kind === "released") {
        requireValue(samePath(product.root, path.join(toolchain.sdkBasePath, "FSharp")), "released FSI must be bundled with the acquired SDK");
    } else {
        requireValue(!inside(toolchain.root, fsi.path) && /^[a-f0-9]{40}$/.test(product.sourceSha) &&
            product.buildInstanceId, "local FSI must belong to the tested source build");
    }
    if (path.basename(fsi.path).toLowerCase() === "fsi.exe") {
        requireValue(cell.os === "windows", "framework FSI executable requires Windows");
        let runtime;
        if (product.runtime.name === "Microsoft.NETCore.App") runtime = await checkedRuntime(toolchain, product.runtime);
        else {
            requireValue(product.runtime.name === ".NETFramework", "unsupported executable runtime");
            runtime = await checkedFile(product.runtime, product.runtime.root);
        }
        return { executable: fsi.path, args: [], identity: { fsi, core, runtime, architecture: cell.architecture } };
    }
    requireValue(path.basename(fsi.path) === "fsi.dll", "local FSI cannot be host dotnet fsi");
    const config = await checkedFile(fsi.runtimeConfig, product.root);
    const options = JSON.parse(await fs.readFile(config.path, "utf8")).runtimeOptions;
    const frameworks = options?.framework ? [options.framework] : options?.frameworks;
    requireValue(frameworks?.length === 1 && frameworks[0].name === product.runtime.name, "unsupported FSI runtimeconfig");
    const required = frameworks[0].version;
    const selected = product.runtime.version;
    requireValue(required.split(".").slice(0, 2).join(".") === selected.split(".").slice(0, 2).join(".") &&
        Number(selected.split(".")[2].split("-")[0]) >= Number(required.split(".")[2].split("-")[0]),
    "explicit runtime is incompatible with FSI runtimeconfig");
    const runtime = await checkedRuntime(toolchain, product.runtime);
    return {
        executable: host.path, args: ["exec", "--fx-version", runtime.version, fsi.path],
        identity: { host, fsi, core, runtime, runtimeConfig: config, architecture: cell.architecture },
    };
}

const compileAssertion = manifest => /(?:^|\n)[^\n]+\(\d+,\d+(?:,\d+,\d+)?\): error FS\d{4}: [^\n]+/.test(manifest.assertions.failure.stderr);
const attemptFor = ({ status, complete, exitCode, stdout, stderr }) => ({
    schemaVersion: 1, checks: { restore: true, build: true, setup: true, identity: true, sdk: true, runtime: true, checksumProvenance: true, network: true },
    execution: { status, complete, exitCode, stdout, stderr },
});

function evaluateAssertion(manifest, execution, { phase = "runtime", checks = {} } = {}) {
    validateManifest(manifest);
    if (phase !== (compileAssertion(manifest) ? "compile" : "runtime")) return untestable("assertion-mismatch");
    const attempt = attemptFor(execution);
    Object.assign(attempt.checks, checks);
    if (/You must install or update .NET|Failed to (?:load|initialize) (?:CoreCLR|the runtime)|hostfxr.*(?:not found|failed)/i.test(execution.stderr)) {
        attempt.checks.runtime = false;
    }
    return classifyAttempts(Array.from({ length: policy.limits.assertionRepetitions }, () => attempt), manifest);
}

async function verifyProjectIdentities({ manifest, product, toolchain, sourceDirectory, packageRoot, evidence, producerRecords = [] }) {
    try {
        validateManifest(manifest);
        requireValue(evidence.length === manifest.matrix.length, "missing or extra project/TFM/configuration evidence");
        const expected = Object.fromEntries(await Promise.all(
            ["compiler", "tasks", "props", "targets", "sdkTargets", "overrides", "core", "corePackage"]
                .map(async name => [name, await checkedFile(role(product, name), product.root)])));
        requireValue(product.kind === "source"
            ? /^[a-f0-9]{40}$/.test(product.sourceSha) && product.buildInstanceId &&
                Object.values(expected).every(f => !inside(toolchain.root, f.path))
            : product.kind === "released" && samePath(product.root, path.join(toolchain.sdkBasePath, "FSharp")),
        "project artifact origin mismatch");
        const verified = [];
        const seen = new Set();
        for (const cell of manifest.matrix) {
            const project = await fs.realpath(path.join(sourceDirectory, cell.project));
            const rows = evidence.filter(e => samePath(e.project, project) &&
                e.targetFramework === cell.tfm && e.configuration === cell.configuration);
            requireValue(rows.length === 1 && !seen.has(rows[0]), `missing or ambiguous inner build: ${cell.id}`);
            const row = rows[0];
            seen.add(row);
            requireValue(row.complete && row.fsc.length === 1, `no unique executed Fsc task: ${cell.id}`);
            requireValue(row.sdkVersion === toolchain.sdkVersion && samePath(row.hostPath, toolchain.hostPath), "SDK/host fallback");
            requireValue(row.languageVersion === cell.languageVersion, "language version changed");
            requireValue(row.projectReferences.every(p => manifest.matrix.some(c => c.project &&
                samePath(path.resolve(path.dirname(project), p), path.join(sourceDirectory, c.project)))),
            "undeclared project reference (immutable producers must not be rebuilt)");
            const imports = await Promise.all(row.imports.map(f => fs.realpath(f)));
            for (const name of ["Sdk.props", "Sdk.targets"]) {
                const sdkImport = await fs.realpath(path.join(toolchain.sdkBasePath, "Sdks", "Microsoft.NET.Sdk", "Sdk", name));
                requireValue(imports.some(f => samePath(f, sdkImport)), "SDK imports fallback");
            }
            for (const name of ["props", "targets", "sdkTargets", "overrides"]) {
                requireValue(imports.some(f => samePath(f, expected[name].path)), `${name} fallback`);
            }
            const fsc = row.fsc[0];
            for (const [field, file] of [["compilerPath", expected.compiler], ["taskAssembly", expected.tasks], ["targetsPath", expected.targets]]) {
                requireValue(samePath(await fs.realpath(fsc[field]), file.path), `${field} fallback or late override`);
            }
            requireValue(samePath(fsc.hostPath, toolchain.hostPath) && fsc.commandLine, "wrong Fsc host or missing invocation");
            requireValue(fsc.languageVersion === cell.languageVersion && fsc.optimize === row.optimize && fsc.nullable === row.nullable &&
                fingerprintInput([...fsc.defines].sort()) === fingerprintInput([...row.defines].sort()), "late compiler flag override");
            const core = row.core;
            requireValue(core.package.id === "FSharp.Core" && core.package.version === product.corePackageVersion, "Core package version fallback");
            requireValue(samePath(await fs.realpath(core.package.root), await fs.realpath(packageRoot)), "shared package cache");
            await checkedFile({ ...core.package, sha256: expected.corePackage.sha256 }, packageRoot);
            for (const kind of ["compile", "runtime"]) {
                const asset = core[kind];
                const packageDirectory = path.join(packageRoot, "fsharp.core", product.corePackageVersion.toLowerCase());
                const relative = path.relative(packageDirectory, asset.path).split(path.sep).join("/");
                const digest = product.coreAssets
                    ? product.coreAssets.find(e => e.path === relative)?.sha256 : expected.core.sha256;
                requireValue(digest, `Core ${kind} asset absent from approved package`);
                await checkedFile({ ...asset, sha256: digest }, packageDirectory);
            }
            const references = await Promise.all(fsc.references.filter(f => path.basename(f).toLowerCase() === "fsharp.core.dll").map(f => fs.realpath(f)));
            requireValue(references.length === 1 && samePath(references[0], await fs.realpath(core.compile.path)), "Fsc did not reference selected Core");
            verified.push(row);
        }
        for (const producer of producerRecords) {
            requireValue(evidence.every(e => !e.targetPath || path.basename(e.targetPath) !== path.basename(producer.dll.path)),
                "immutable producer was rebuilt");
            const references = [...new Set(evidence.flatMap(e => e.fsc.flatMap(f => f.references))
                .filter(f => path.basename(f) === path.basename(producer.dll.path)))];
            requireValue(references.length > 0, "consumer did not reference immutable producer");
            for (const reference of references) {
                const real = await fs.realpath(reference);
                requireValue((inside(await fs.realpath(sourceDirectory), real) || inside(await fs.realpath(packageRoot), real)) &&
                    hash(await fs.readFile(real)) === producer.dll.sha256, "consumer did not reference immutable producer bytes");
            }
        }
        return { status: "verified", cells: verified };
    } catch (error) {
        return { status: "untestable", outcome: untestable("wrong-identity"), error: error.message };
    }
}

async function verifySources(manifest, sourceDirectory) {
    for (const source of manifest.sources) await checkedFile({ ...source, path: path.join(sourceDirectory, source.path) }, sourceDirectory);
}

async function prepareEnvironment(root, toolchain, platform) {
    const env = isolatedEnvironment(root, platform);
    if (platform.os === "windows") Object.assign(env, {
        PATHEXT: ".COM;.EXE;.BAT;.CMD", OS: "Windows_NT",
        ProgramFiles: path.join(root, "program-files"),
        "ProgramFiles(x86)": path.join(root, "program-files-x86"),
    });
    for (const folder of [env.HOME, env.TEMP, env.APPDATA, env.LOCALAPPDATA, env.NUGET_PACKAGES]) await fs.mkdir(folder, { recursive: true });
    const isolatedDotnet = env.DOTNET_ROOT;
    for (const [name, value] of Object.entries(env)) {
        if (value === isolatedDotnet) env[name] = toolchain.root;
    }
    env.PATH = `${toolchain.root}${path.delimiter}${env.PATH.split(path.delimiter).slice(1).join(path.delimiter)}`;
    env.DOTNET_HOST_PATH = toolchain.hostPath;
    return env;
}

async function invokeHelper(helper, args, settings, run) {
    requireValue(helper?.executable && helper.dll && helper.runtimeVersion, "evidence helper is required");
    const result = await run(helper.executable, ["exec", "--fx-version", helper.runtimeVersion, helper.dll, ...args],
        { ...settings, maxBytes: 4 * 1024 * 1024 });
    requireValue(result.status === "completed" && result.complete && result.exitCode === 0,
        `evidence helper failed: ${result.error ?? result.stderr}`);
    return JSON.parse(result.stdout);
}

async function inspectProduct(product, inspect) {
    for (const file of product.files) await checkedFile(file, product.root);
    const files = product.files.filter(f => ["fsi", "compiler", "core", "tasks", "corePackage"].includes(f.role));
    const inspected = await inspect(files.map(f => f.path));
    for (const file of files.filter(f => f.assemblyVersion)) {
        const actual = inspected.find(f => samePath(f.path, file.path));
        requireValue(actual?.sha256 === file.sha256 && actual.assemblyVersion === file.assemblyVersion, "artifact assembly identity mismatch");
    }
    const pkg = inspected.find(f => samePath(f.path, role(product, "corePackage").path));
    requireValue(pkg?.id === "FSharp.Core" && pkg.version === product.corePackageVersion, "package identity mismatch");
    return { ...product, coreAssets: pkg.entries.filter(e => /^lib\/[^/]+\/FSharp\.Core\.dll$/i.test(e.path)) };
}

async function producers(manifest, sourceDirectory, inspect) {
    const records = [];
    for (const source of manifest.sources.filter(s => s.path.endsWith(".producer.json"))) {
        const record = JSON.parse(await fs.readFile(path.join(sourceDirectory, source.path), "utf8"));
        requireValue(record.role === "immutable-producer" && record.compiler?.sha256 && record.compiler.version &&
            Array.isArray(record.flags) && record.tfm && Array.isArray(record.packages), "incomplete immutable producer provenance");
        requireValue(record.dll.sha256 === manifest.producer.sha256, "producer does not match frozen identity");
        for (const item of [record.dll, ...(record.package ? [record.package] : [])]) {
            requireValue(manifest.sources.some(s => s.path === item.path && s.sha256 === item.sha256), "producer asset must be a reviewed source");
            await checkedFile({ ...item, path: path.join(sourceDirectory, item.path) }, sourceDirectory);
        }
        if (record.package) requireValue(manifest.packages.some(p => p.id === record.package.id &&
            p.version === record.package.version && p.sha256 === record.package.sha256), "producer package not frozen");
        const [dll] = await inspect([path.join(sourceDirectory, record.dll.path)]);
        requireValue(fingerprintInput(dll.resources) === fingerprintInput(record.dll.resources) &&
            dll.resources.some(r => /^FSharp(Signature|Optimization)(Compressed)?DataB?\./.test(r.name)), "producer metadata resource changed or missing");
        records.push(record);
    }
    requireValue(records.length <= 1, "ambiguous immutable producer role");
    requireValue(records.length === 1 || manifest.producer.name === "no-producer" &&
        manifest.producer.version === "0.0.0" && manifest.producer.sha256 === hash(""), "missing immutable producer declaration");
    return records;
}

async function coreAssets(row, packageRoot, product, manifest) {
    const assets = JSON.parse(await fs.readFile(row.assetsFile, "utf8"));
    const keys = Object.keys(assets.targets).filter(k => k === row.targetFramework);
    requireValue(keys.length === 1, "missing or ambiguous assets TFM");
    const target = assets.targets[keys[0]];
    const identity = `FSharp.Core/${product.corePackageVersion}`;
    requireValue(target[identity] && assets.libraries[identity]?.type === "package", "Core not resolved as expected package");
    requireValue(Object.keys(assets.packageFolders).length === 1 &&
        samePath(await fs.realpath(Object.keys(assets.packageFolders)[0]), await fs.realpath(packageRoot)), "nonisolated assets package root");
    const directory = path.join(packageRoot, assets.libraries[identity].path);
    for (const [key, library] of Object.entries(assets.libraries)) {
        if (key === identity || library.type !== "package") continue;
        const [id, version] = key.split("/");
        const frozen = manifest.packages.find(p => p.id.toLowerCase() === id.toLowerCase() && p.version === version);
        requireValue(frozen, `unfrozen package: ${key}`);
        await checkedFile({ path: path.join(packageRoot, library.path, `${id.toLowerCase()}.${version.toLowerCase()}.nupkg`),
            sha256: frozen.sha256 }, packageRoot);
    }
    const selected = {};
    for (const kind of ["compile", "runtime"]) {
        const entries = Object.keys(target[identity][kind] ?? {}).filter(p => /(?:^|\/)FSharp\.Core\.dll$/i.test(p));
        requireValue(entries.length === 1, `missing or ambiguous Core ${kind} asset`);
        const location = path.join(directory, entries[0]);
        selected[kind] = { path: location, sha256: hash(await fs.readFile(location)) };
    }
    const nupkg = path.join(directory, `fsharp.core.${product.corePackageVersion.toLowerCase()}.nupkg`);
    return {
        ...selected, package: { id: "FSharp.Core", version: product.corePackageVersion, root: packageRoot,
            path: nupkg, sha256: hash(await fs.readFile(nupkg)) },
        assetsFile: { path: row.assetsFile, sha256: hash(await fs.readFile(row.assetsFile)) },
    };
}

async function loadedCore(file, expected, runtime, architecture, allowedPaths = [expected.path]) {
    const records = (await fs.readFile(file, "utf8")).trim().split(/\r?\n/).map(line => JSON.parse(line));
    requireValue(records.length === 1, "missing or ambiguous loaded FSharp.Core");
    const actual = records[0];
    requireValue(actual.complete === true && actual.assemblyName === "FSharp.Core", "incomplete loaded Core evidence");
    requireValue(allowedPaths.some(p => samePath(p, actual.path)), "wrong loaded Core path");
    requireValue(samePath(await fs.realpath(actual.path), actual.path), "loaded Core path redirects through a link");
    requireValue(actual.sha256 === expected.sha256 && actual.assemblyVersion === expected.assemblyVersion &&
        actual.runtimeVersion === runtime.version && actual.architecture.toLowerCase() === architecture, "wrong loaded Core/runtime/architecture");
    requireValue(samePath(actual.runtimePath, path.join(runtime.path, runtime.version)) &&
        hash(await fs.readFile(path.join(actual.runtimePath, "System.Private.CoreLib.dll"))) === runtime.sha256, "wrong loaded runtime path/bytes");
    requireValue(hash(await fs.readFile(actual.path)) === expected.sha256, "loaded Core bytes changed");
    return actual;
}

async function probeFrameworkFsi(selected, flags, directory, command, expected, architecture) {
    const probe = path.join(directory, ".runner", "core-probe.fsx");
    await fs.writeFile(probe, [
        "open System",
        "open System.IO",
        "let core = typeof<list<int>>.Assembly",
        'let info = typeof<obj>.Assembly.GetType("System.Runtime.InteropServices.RuntimeInformation")',
        'let arch = if isNull info then (if IntPtr.Size = 8 then "x64" else "x86") else info.GetProperty("ProcessArchitecture").GetValue(null).ToString().ToLowerInvariant()',
        'printfn "CORE|%s|%O|%s|%s|%s" core.Location (core.GetName().Version) (Path.GetFileName(Path.GetDirectoryName(typeof<obj>.Assembly.Location))) arch typeof<obj>.Assembly.Location',
    ].join("\n"));
    const result = await command(selected.executable, [...selected.args, ...flags, "--exec", probe]);
    requireValue(result.status === "completed" && result.complete && result.exitCode === 0, "framework FSI identity probe failed");
    const lines = result.stdout.trim().split("\n");
    requireValue(lines.length === 1 && lines[0].startsWith("CORE|"), "ambiguous framework FSI probe");
    const [, location, assemblyVersion, runtimeVersion, actualArchitecture, runtimeLocation] = lines[0].split("|");
    const runtime = selected.identity.runtime;
    const runtimePath = runtime.name === ".NETFramework" ? runtime.path : path.join(runtime.path, runtime.version, "System.Private.CoreLib.dll");
    requireValue(samePath(await fs.realpath(location), expected.path) &&
        hash(await fs.readFile(location)) === expected.sha256 && assemblyVersion === expected.assemblyVersion &&
        actualArchitecture === architecture && runtimeVersion === selected.identity.runtime.version, "wrong loaded framework Core/runtime");
    requireValue(samePath(await fs.realpath(runtimeLocation), runtimePath) &&
        hash(await fs.readFile(runtimeLocation)) === runtime.sha256, "wrong loaded executable runtime bytes");
    return { path: location, sha256: expected.sha256, assemblyVersion, runtimeVersion, runtimePath, architecture };
}

function diagnosticsExecution(result, diagnostics, root, filePath = path) {
    const errors = diagnostics.filter(d => d.severity === "error");
    return {
        ...result, stdout: "", stderr: errors.map(d => {
            const span = d.endLine > 0 && d.endColumn > 0 ? `${d.line},${d.column},${d.endLine},${d.endColumn}` : `${d.line},${d.column}`;
            return `${filePath.relative(root, d.file).split(filePath.sep).join("/")}(${span}): error ${d.code}: ${d.message}\n`;
        }).join(""),
    };
}

function expectedBuildManifest(manifest, cell, rows, directory) {
    const matrix = [cell];
    for (let i = 0; i < matrix.length; i++) {
        const current = matrix[i];
        const matches = rows.filter(r => samePath(r.project, path.join(directory, current.project)) &&
            r.targetFramework === current.tfm && r.configuration === current.configuration);
        requireValue(matches.length === 1, `missing or ambiguous expected inner build: ${current.project} ${current.tfm}`);
        for (const reference of matches[0].projectReferences) {
            const absolute = path.resolve(path.dirname(matches[0].project), reference);
            const relative = path.relative(directory, absolute).split(path.sep).join("/");
            requireValue(manifest.sources.some(s => s.path === relative && s.path.endsWith(".fsproj")),
                "undeclared project reference (immutable producers must not be rebuilt)");
            const declared = manifest.matrix.filter(c => c.project === relative && c.configuration === cell.configuration);
            const expected = declared.find(c => c.tfm === cell.tfm) ?? (declared.length === 1 ? declared[0] : null);
            requireValue(declared.length === 0 || expected, "ambiguous referenced project TFM");
            const next = expected ?? { ...cell, project: relative, command: ["run-project", relative] };
            if (!matrix.some(c => c.project === next.project && c.tfm === next.tfm && c.configuration === next.configuration)) {
                matrix.push({ ...next, id: `${cell.id}-dependency-${matrix.length}` });
            }
        }
    }
    const { approvedInputDigest, ...input } = manifest;
    const result = { ...input, matrix };
    return { ...result, approvedInputDigest: fingerprintInput(result) };
}

async function runReproduction(options, dependencies = {}) {
    const cells = [];
    let stage = "setup";
    let work, pending;
    try {
        const { manifest, toolchain, helper } = options;
        validateManifest(manifest);
        await verifySources(manifest, options.sourceDirectory);
        stage = "identity";
        const verification = verifyToolchainIdentity({ policy, toolchain: toolchain.requested, identity: toolchain });
        requireValue(verification.status === "verified", verification.error?.message ?? "unverified acquisition");
        await verifyRetainedArchive(toolchain.archive);
        for (const installation of toolchain.installations)
            if (installation.product !== "sdk") await verifyRetainedArchive(installation.archive);
        await checkedFile({ path: toolchain.hostPath, sha256: toolchain.hostSha256 }, toolchain.root);
        requireValue(["source", "released"].includes(options.product.kind) && options.product.buildInstanceId, "unidentified product build");
        if (options.product.kind === "source") requireValue(/^[0-9a-f]{40}$/.test(options.product.sourceSha),
            "source build revision missing");
        const platform = toolchain.requested.platform;
        requireValue(manifest.matrix.every(c => c.architecture === toolchain.architecture &&
            c.os === platform.os), "matrix requires a different worker platform");
        const run = dependencies.execute ?? execute;
        await fs.mkdir(options.workDirectory, { recursive: true });
        work = await fs.realpath(await fs.mkdtemp(path.join(options.workDirectory, "repro-")));
        const env = await prepareEnvironment(work, toolchain, platform);
        const settings = { cwd: work, env, timeoutMs: manifest.timeoutSeconds * 1000 };
        const inspect = files => invokeHelper(helper, ["inspect", ...files], settings, run);
        stage = "identity";
        const product = await inspectProduct(options.product, inspect);
        const frozenProducers = await producers(manifest, options.sourceDirectory, inspect);
        const productFingerprint = fingerprintInput(product);
        await fs.writeFile(path.join(work, "identities.json"), JSON.stringify({
            product, acquisitionDigest: fingerprintInput(toolchain),
            sdk: { version: toolchain.sdkVersion, basePath: toolchain.sdkBasePath, archive: toolchain.archive },
            host: { path: toolchain.hostPath, sha256: toolchain.hostSha256 },
            architecture: toolchain.architecture, runtimes: toolchain.runtimes, producers: frozenProducers,
        }, null, 2), { flag: "wx" });
        const repetitions = policy.limits.assertionRepetitions;
        for (const cell of manifest.matrix) {
            const attempts = [], evidence = [];
            for (let repetition = 0; repetition < repetitions; repetition++) {
                stage = "setup";
                const directory = path.join(work, `cell-${cells.length}-${repetition}`);
                await fs.mkdir(directory);
                for (const source of manifest.sources) {
                    const destination = path.join(directory, source.path);
                    await fs.mkdir(path.dirname(destination), { recursive: true });
                    await fs.copyFile(path.join(options.sourceDirectory, source.path), destination);
                }
                await fs.writeFile(path.join(directory, "global.json"), JSON.stringify({
                    sdk: { version: toolchain.sdkVersion, rollForward: "disable", allowPrerelease: product.kind === "source" },
                }), { flag: "wx" });
                const attemptEnv = await prepareEnvironment(path.join(directory, ".runner"), toolchain, platform);
                const launch = { cwd: directory, env: attemptEnv, timeoutMs: settings.timeoutMs };
                const record = { commands: [], productFingerprint, producers: frozenProducers };
                pending = { cell: cell.id, repetition, evidence: record };
                const command = async (executable, args, extraEnv = {}) => {
                    const entry = { executable, args };
                    record.commands.push(entry);
                    entry.result = await run(executable, args, { ...launch, env: { ...attemptEnv, ...extraEnv } });
                    return entry.result;
                };
                let execution;
                if (manifest.kind === "fsi") {
                    stage = "identity";
                    const selected = await selectFsiCommand({ product, toolchain, cell });
                    record.selection = selected.identity;
                    const coreFile = path.join(directory, ".runner", "fsi-core.jsonl");
                    const flags = ["--nologo", "--noninteractive", `--langversion:${cell.languageVersion}`,
                        `--optimize${cell.configuration === "Release" ? "+" : "-"}`];
                    const framework = path.extname(selected.executable).toLowerCase() === ".exe" && selected.args.length === 0;
                    if (framework) record.loadedCore = await probeFrameworkFsi(selected, flags, directory, command, role(product, "core"), cell.architecture);
                    else requireValue(helper.hook, "runtime Core load hook missing");
                    stage = "runtime";
                    execution = await command(selected.executable, [...selected.args, ...flags, "--exec", path.join(directory, cell.command[1])],
                        framework ? {} : { DOTNET_STARTUP_HOOKS: helper.hook, REPRO_CORE_EVIDENCE: coreFile });
                    stage = "identity";
                    if (!framework && execution.status === "completed") record.loadedCore = await loadedCore(coreFile,
                        role(product, "core"), selected.identity.runtime, cell.architecture);
                    if (compileAssertion(manifest)) execution = {
                        ...execution, stderr: execution.stderr.split(`${directory}${path.sep}`).join("").replace(/^\n+/, ""),
                    };
                } else {
                    const reviewedConfig = manifest.sources.find(s => s.path.toLowerCase() === "nuget.config");
                    const restoreConfig = reviewedConfig ? path.join(directory, reviewedConfig.path) : path.join(directory, ".runner", "NuGet.Config");
                    if (!reviewedConfig) await fs.writeFile(restoreConfig,
                        "<configuration><packageSources><clear /></packageSources></configuration>", { flag: "wx" });
                    const targetImport = path.join(directory, ".runner", "Directory.Build.targets");
                    const imports = [path.join(__dirname, "ReproEvidence.targets")];
                    if (product.kind === "source") imports.unshift(path.join(__dirname, "..", "..", "UseLocalCompiler.Directory.Build.targets"));
                    const xml = text => text.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;");
                    await fs.writeFile(targetImport, `<Project>${imports.map(file => `<Import Project="${xml(file)}" />`).join("")}</Project>`);
                    attemptEnv.CustomAfterDirectoryBuildTargets = targetImport;
                    if (product.kind === "source") {
                        const config = await checkedFile(role(product, "compiler").runtimeConfig, product.root);
                        const runtimeOptions = JSON.parse(await fs.readFile(config.path, "utf8")).runtimeOptions;
                        requireValue(/^net\d+\.\d+$/.test(runtimeOptions.tfm) &&
                            ["Debug", "Release"].includes(product.configuration), "missing built compiler layout/configuration");
                        Object.assign(attemptEnv, {
                            LoadLocalFSharpBuild: "True", LocalFSharpCompilerPath: product.root,
                            LocalFSharpCompilerConfiguration: product.configuration,
                            FSharpNetCoreProductTargetFramework: runtimeOptions.tfm,
                            CustomAfterDirectoryBuildProps: path.join(__dirname, "..", "..", "UseLocalCompiler.Directory.Build.props"),
                            RegressionLocalCore: "true", RegressionLocalCoreVersion: product.corePackageVersion,
                            RegressionLocalCorePackagesDir: path.dirname(role(product, "corePackage").path),
                        });
                    }
                    const binlog = path.join(directory, ".runner", "build.binlog");
                    const compilerRuntime = await checkedRuntime(toolchain, product.runtime);
                    const compatible = toolchain.runtimes.filter(r => r.name === product.runtime.name &&
                        r.version.split(".").slice(0, 2).join(".") === product.runtime.version.split(".").slice(0, 2).join("."));
                    requireValue(compatible.length === 1, "ambiguous compiler runtime; cannot pin MSBuild's compiler child");
                    record.compilerRuntimeSelection = { runtime: compilerRuntime, rollForward: "LatestPatch" };
                    stage = "build";
                    const build = await command(toolchain.hostPath, ["build", path.join(directory, cell.project),
                        "--no-incremental", "--nologo", "-v:quiet", "-nr:false", `-bl:${binlog}`,
                        `-p:TargetFramework=${cell.tfm}`, `-p:Configuration=${cell.configuration}`, `-p:LangVersion=${cell.languageVersion}`,
                        `-p:RestoreConfigFile=${restoreConfig}`,
                        "-p:UseSharedCompilation=false"], {
                        DOTNET_ROLL_FORWARD: "LatestPatch", DOTNET_ROLL_FORWARD_TO_PRERELEASE: "1",
                        DOTNET_STARTUP_HOOKS: helper.hook,
                        REPRO_CORE_EVIDENCE: path.join(directory, ".runner", "compiler-core-{pid}.jsonl"),
                    });
                    record.build = build;
                    if (build.status !== "completed") execution = build;
                    else {
                        const structured = await invokeHelper(helper, ["binlog", binlog], launch, run);
                        record.diagnostics = structured.diagnostics;
                        record.binlog = { path: binlog, sha256: hash(await fs.readFile(binlog)) };
                        if (structured.diagnostics.some(d => d.severity === "error" &&
                            (/^NU\d+$/.test(d.code) || path.basename(d.file ?? "").toLowerCase() === "nuget.targets"))) {
                            const failed = attemptFor(build);
                            failed.checks.restore = false;
                            attempts.push(failed); evidence.push(record);
                            continue;
                        }
                        const rows = structured.cells.map(row => ({
                            ...row, fsc: row.fscTasks,
                            languageVersion: row.expectedLanguageVersion,
                            optimize: row.expectedOptimize,
                            nullable: row.expectedNullable,
                            defines: row.expectedDefines,
                            assetsFile: path.resolve(path.dirname(row.project), row.assetsFile),
                            targetPath: path.resolve(path.dirname(row.project), row.targetPath),
                        }));
                        stage = "identity";
                        for (const row of rows) row.core = await coreAssets(row, attemptEnv.NUGET_PACKAGES, product, manifest);
                        const cellManifest = expectedBuildManifest(manifest, cell, rows, directory);
                        const verified = await verifyProjectIdentities({
                            manifest: cellManifest, product, toolchain, sourceDirectory: directory,
                            packageRoot: attemptEnv.NUGET_PACKAGES, evidence: rows, producerRecords: frozenProducers,
                        });
                        requireValue(verified.status === "verified", verified.error);
                        record.projects = rows;
                        const compiler = role(product, "compiler");
                        const coreFiles = [];
                        for (const name of await fs.readdir(path.join(directory, ".runner"))) {
                            if (!/^compiler-core-\d+\.jsonl$/.test(name)) continue;
                            const file = path.join(directory, ".runner", name);
                            const loads = (await fs.readFile(file, "utf8")).trim().split(/\r?\n/).map(line => JSON.parse(line));
                            if (loads.some(load => load.entryAssembly && samePath(load.entryAssembly, compiler.path))) coreFiles.push(file);
                        }
                        requireValue(coreFiles.length === rows.reduce((count, row) => count + row.fsc.length, 0),
                            "missing actual compiler runtime/Core load evidence");
                        record.compilerCoreLoads = [];
                        for (const file of coreFiles) record.compilerCoreLoads.push(await loadedCore(file, role(product, "core"),
                            compilerRuntime, cell.architecture, [path.join(path.dirname(compiler.path), "FSharp.Core.dll")]));
                        if (compileAssertion(manifest)) execution = diagnosticsExecution(build, structured.diagnostics, directory);
                        else if (build.exitCode !== 0) {
                            const failed = attemptFor(build);
                            failed.checks.build = false;
                            attempts.push(failed); evidence.push(record);
                            continue;
                        } else {
                            const row = rows.find(r => samePath(r.project, path.join(directory, cell.project)) &&
                                r.targetFramework === cell.tfm && r.configuration === cell.configuration);
                            requireValue(inside(directory, await fs.realpath(row.targetPath)), "runtime output outside reproduction");
                            record.runtimeProducers = [];
                            for (const producer of frozenProducers) {
                                const location = path.join(path.dirname(row.targetPath), path.basename(producer.dll.path));
                                record.runtimeProducers.push(await checkedFile({ path: location, sha256: producer.dll.sha256 }, directory));
                            }
                            const runtime = options.sampleRuntimes?.[cell.tfm] ?? product.runtime;
                            const verifiedRuntime = await checkedRuntime(toolchain, runtime);
                            const coreFile = path.join(directory, ".runner", "app-core.jsonl");
                            stage = "runtime";
                            execution = await command(toolchain.hostPath, ["exec", "--fx-version", runtime.version, row.targetPath], {
                                DOTNET_STARTUP_HOOKS: helper.hook, REPRO_CORE_EVIDENCE: coreFile,
                            });
                            stage = "identity";
                            if (execution.status === "completed") {
                                const [core] = await inspect([row.core.runtime.path]);
                                record.loadedCore = await loadedCore(coreFile, core, verifiedRuntime, cell.architecture,
                                    [row.core.runtime.path, path.join(path.dirname(row.targetPath), "FSharp.Core.dll")]);
                            }
                        }
                    }
                }
                stage = "identity";
                await verifySources(manifest, directory);
                await verifySources(manifest, options.sourceDirectory);
                for (const file of product.files) await checkedFile(file, product.root);
                await checkedFile({ path: toolchain.hostPath, sha256: toolchain.hostSha256 }, toolchain.root);
                for (const file of record.runtimeProducers ?? []) await checkedFile(file, directory);
                const attempt = attemptFor(execution);
                const outcome = evaluateAssertion(manifest, execution, { phase: compileAssertion(manifest) ? "compile" : "runtime" });
                if (outcome.reason === "unavailable-runtime") attempt.checks.runtime = false;
                attempts.push(attempt); evidence.push(record);
            }
            cells.push({ id: cell.id, attempts, evidence, outcome: classifyAttempts(attempts, manifest) });
        }
        const failure = cells.find(c => c.outcome.outcome === "untestable");
        const outcome = failure?.outcome ?? (cells.every(c => c.outcome.outcome === cells[0].outcome.outcome)
            ? cells[0].outcome : untestable("flaky"));
        const identitiesPath = path.join(work, "identities.json");
        const result = {
            outcome, cells, manifestDigest: fingerprintInput(manifest), productFingerprint, workDirectory: work,
            identities: { path: identitiesPath, sha256: hash(await fs.readFile(identitiesPath)) },
        };
        await fs.writeFile(path.join(work, "result.json"), JSON.stringify(result, null, 2), { flag: "wx" });
        return result;
    } catch (error) {
        const result = {
            outcome: untestable({ identity: "wrong-identity", build: "build-failure", runtime: "unavailable-runtime" }[stage] ?? "setup-failure"),
            cells, error: error.message, ...(pending ? { pending } : {}), ...(work ? { workDirectory: work } : {}),
        };
        if (work) await fs.writeFile(path.join(work, "result.json"), JSON.stringify(result, null, 2), { flag: "wx" });
        return result;
    }
}

module.exports = { runReproduction, selectFsiCommand, verifyProjectIdentities, evaluateAssertion, compileAssertion, diagnosticsExecution };
