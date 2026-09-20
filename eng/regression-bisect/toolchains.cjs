const fs = require("node:fs/promises");
const path = require("node:path");
const { validatePolicy, fingerprintInput } = require("./model.cjs");
const { hash, requireValue, inside, publicUrl, createHttp, validateHttpResponse, validateDownloadProvenance, run, isolatedEnvironment, capture } = require("./toolchain-io.cjs");
const { validateArchiveReceipt } = require("./archive-provenance.cjs");

const indexUrl = "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json";
const exactVersion = /^\d+\.\d+\.\d+(?:-[0-9A-Za-z]+(?:[.-][0-9A-Za-z]+)*)?$/;
const stableVersion = /^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$/;
const runtimeProducts = { "Microsoft.NETCore.App": "dotnet", "Microsoft.AspNetCore.App": "aspnetcore", "Microsoft.WindowsDesktop.App": "windowsdesktop" };

function dependencies(options, injected) {
    validatePolicy(options.policy);
    const fileSystem = injected.fs ?? fs;
    return {
        fs: fileSystem, http: createHttp(options.policy, fileSystem, options.signal),
        run: (file, args, settings) => run(file, args, { ...settings, signal: options.signal }),
        clock: () => new Date(), capture, ...injected,
    };
}
function failure(stage, error, attempts = []) {
    return {
        status: "untestable",
        outcome: {
            schemaVersion: 1, outcome: "untestable",
            reason: ["checksum-provenance", "network-failure"].includes(error.code) ? error.code :
                stage === "sdk" && ["command-failure", "installation-failure"].includes(error.code) ? "setup-failure" :
                { identity: "wrong-identity", sdk: "unavailable-sdk", runtime: "unavailable-runtime" }[stage] ?? "setup-failure",
        },
        error: { stage, code: error.code ?? "toolchain-unavailable", message: error.message },
        attempts: attempts.map(a => a.status === "pending" ? { ...a, status: "error", error: error.message } : a),
        ...(error.acquisition ? { acquisition: error.acquisition } : {}),
    };
}
function validatePlatform(platform) {
    requireValue(platform && ["x64", "arm64"].includes(platform.architecture), "unsupported architecture");
    const prefix = { windows: "win", linux: "linux", macos: "osx" }[platform.os];
    requireValue(prefix && (platform.rid === `${prefix}-${platform.architecture}` ||
        platform.os === "linux" && platform.rid === `linux-musl-${platform.architecture}`), "OS/RID/architecture mismatch");
}
function validateResponse(url, response, policy) {
    publicUrl(url, policy);
    publicUrl(response.url, policy);
    for (const hop of response.redirects ?? []) {
        publicUrl(hop.from, policy); publicUrl(hop.to, policy);
    }
}
async function document(url, options, deps, records, attempts) {
    publicUrl(url, options.policy);
    const attempt = { url, status: "pending" };
    attempts.push(attempt);
    const response = await deps.http(url);
    attempt.status = response.status;
    validateResponse(url, response, options.policy);
    if (response.status !== 200) throw Object.assign(new Error(`metadata HTTP ${response.status}: ${url}`), { code: "network-failure" });
    const bytes = Buffer.from(response.bytes);
    records.push({ url, redirectUrl: response.url, sha256: hash(bytes), bytesBase64: bytes.toString("base64") });
    return JSON.parse(bytes.toString("utf8"));
}
async function releases(options, deps, records, attempts, channel) {
    const index = await document(indexUrl, options, deps, records, attempts);
    requireValue(Array.isArray(index["releases-index"]) && index["releases-index"].length <= 32, "invalid release index");
    const result = [];
    for (const item of index["releases-index"]) {
        requireValue(/^\d+\.\d+$/.test(item["channel-version"]), "invalid release channel");
        if (channel && item["channel-version"] !== channel) continue;
        const doc = await document(item["releases.json"], options, deps, records, attempts);
        requireValue(doc["channel-version"] === item["channel-version"] && Array.isArray(doc.releases), "invalid channel metadata");
        for (const release of doc.releases) result.push({ channel: item["channel-version"], release });
    }
    return result;
}
function selectArchive(product, platform, policy) {
    const extension = platform.os === "windows" ? ".zip" : ".tar.gz";
    const files = product.files?.filter(file => file.rid === platform.rid && file.name?.endsWith(extension) &&
        file.url?.endsWith(extension));
    requireValue(files?.length === 1, `missing or ambiguous ${platform.rid} ${extension} archive for ${product.version}`);
    const file = files[0];
    publicUrl(file.url, policy);
    if (!/^[0-9a-f]{128}$/i.test(file.hash)) throw Object.assign(new Error(`malformed SHA-512 for ${file.name}`),
        { code: "checksum-provenance" });
    return { name: file.name, url: file.url, sha512: file.hash.toLowerCase(), rid: file.rid, architecture: platform.architecture,
        provenance: { kind: "publisher-checksum" } };
}
function sdkEntries(release) {
    return [release.sdk, ...(release.sdks ?? [])].filter(Boolean);
}
function numericVersion(a, b) {
    const left = a.split(".").map(BigInt);
    const right = b.split(".").map(BigInt);
    for (let i = 0; i < 3; i++) if (left[i] !== right[i]) return left[i] > right[i] ? 1 : -1;
    return 0;
}

async function resolveLatestStableSdk(options, injected = {}) {
    const attempts = [];
    try {
        const deps = dependencies(options, injected);
        validatePlatform(options.platform);
        const now = deps.clock();
        requireValue(Number.isFinite(now.getTime()), "invalid run clock");
        const metadata = [];
        const candidates = new Map();
        for (const { channel, release } of await releases(options, deps, metadata, attempts)) {
            const date = release["release-date"];
            requireValue(/^\d{4}-\d{2}-\d{2}$/.test(date) && Number.isFinite(Date.parse(date)) &&
                new Date(date).toISOString().slice(0, 10) === date, "invalid release date");
            if (Date.parse(date) > now.getTime()) continue;
            for (const sdk of sdkEntries(release)) {
                if (!stableVersion.test(sdk.version)) continue;
                const candidate = { sdk, channel, releaseDate: date, bundledRuntime: { version: sdk["runtime-version"] ?? release.runtime?.version } };
                const entries = candidates.get(sdk.version) ?? [];
                entries.push(candidate);
                candidates.set(sdk.version, entries);
            }
        }
        const version = [...candidates.keys()].sort(numericVersion).at(-1);
        requireValue(version, "no released stable SDK in official metadata");
        const entries = candidates.get(version).sort((a, b) => a.releaseDate.localeCompare(b.releaseDate));
        const selected = entries[0];
        const archive = selectArchive(selected.sdk, options.platform, options.policy);
        for (const duplicate of entries.slice(1)) {
            requireValue(fingerprintInput(archive) === fingerprintInput(selectArchive(duplicate.sdk, options.platform, options.policy)) &&
                duplicate.bundledRuntime.version === selected.bundledRuntime.version, `conflicting SDK metadata: ${version}`);
        }
        requireValue(exactVersion.test(selected.bundledRuntime.version), "missing bundled runtime metadata");
        return {
            status: "resolved",
            toolchain: {
                kind: "released", sdkVersion: version, toolsVersion: version, platform: options.platform,
                channel: selected.channel, releaseDate: selected.releaseDate, frozenAt: now.toISOString(),
                archive,
                bundledRuntime: selected.bundledRuntime, metadata, runtimeRequirements: [],
            }, attempts,
        };
    } catch (error) { return failure("metadata", error, attempts); }
}

async function snapshot(file, deps, optional = false) {
    try {
        const real = await deps.fs.realpath(file);
        const bytes = await deps.fs.readFile(real);
        return { path: real, sha256: hash(bytes), bytesBase64: bytes.toString("base64") };
    } catch (error) {
        if (optional && error.code === "ENOENT") return null;
        throw error;
    }
}
async function readRevisionToolchain(options, injected = {}) {
    try {
        const deps = dependencies(options, injected);
        validatePlatform(options.platform);
        requireValue(/^[0-9a-f]{40}$/.test(options.sourceSha), "expected pinned source SHA");
        const sourceDirectory = await deps.fs.realpath(options.sourceDirectory);
        const globalJson = await snapshot(path.join(sourceDirectory, "global.json"), deps);
        requireValue(inside(sourceDirectory, globalJson.path), "global.json resolves outside historical source");
        const declaration = JSON.parse(Buffer.from(globalJson.bytesBase64, "base64").toString("utf8"));
        const sdkVersion = declaration.sdk?.version;
        const toolsVersion = declaration.tools?.dotnet;
        requireValue(exactVersion.test(sdkVersion) && exactVersion.test(toolsVersion), "missing exact sdk.version or tools.dotnet");
        requireValue(sdkVersion === toolsVersion, `source sdk.version ${sdkVersion} disagrees with tools.dotnet ${toolsVersion}`);
        const runtimeRequirements = [];
        const declaredRuntimes = declaration.tools.runtimes ?? {};
        requireValue(declaredRuntimes && typeof declaredRuntimes === "object" && !Array.isArray(declaredRuntimes), "invalid tools.runtimes");
        for (const [key, versions] of Object.entries(declaredRuntimes)) {
            const [product, architecture = options.platform.architecture, extra] = key.split("/");
            const name = Object.keys(runtimeProducts).find(name => runtimeProducts[name] === product);
            requireValue(name && !extra && architecture === options.platform.architecture && Array.isArray(versions),
                `unsupported historical runtime declaration: ${key}`);
            for (const version of versions) {
                requireValue(exactVersion.test(version), `historical runtime version must be exact: ${version}`);
                runtimeRequirements.push({ name, version, role: "build", config: globalJson });
            }
        }
        for (const config of options.runtimeConfigs ?? []) {
            requireValue(["compiler", "fsi", "sample"].includes(config.role), "invalid runtime requirement role");
            const record = await snapshot(config.path, deps);
            requireValue(config.role === "sample" || inside(sourceDirectory, record.path), "compiler runtimeconfig resolves outside historical source");
            const settings = JSON.parse(Buffer.from(record.bytesBase64, "base64").toString("utf8")).runtimeOptions;
            const frameworks = settings?.framework ? [settings.framework] : settings?.frameworks;
            requireValue(Array.isArray(frameworks) && frameworks.length > 0, `missing framework requirement: ${config.path}`);
            for (const framework of frameworks) {
                requireValue(runtimeProducts[framework.name] && exactVersion.test(framework.version), `unsupported runtime requirement: ${config.path}`);
                runtimeRequirements.push({ name: framework.name, version: framework.version, role: config.role, config: record });
            }
        }
        // Older sources need not have a centralized TFM file. Never invent or override it.
        const declarations = {
            targetFrameworks: await snapshot(path.join(sourceDirectory, "eng", "TargetFrameworks.props"), deps, true),
            buildSh: await snapshot(path.join(sourceDirectory, "eng", "build.sh"), deps, true),
            buildPs1: await snapshot(path.join(sourceDirectory, "eng", "Build.ps1"), deps, true),
        };
        requireValue(Object.values(declarations).every(file => file === null || inside(sourceDirectory, file.path)), "build declaration resolves outside historical source");
        return {
            status: "resolved",
            toolchain: {
                kind: "source", sourceSha: options.sourceSha, sourceDirectory, globalJson, declarations,
                sdkVersion, toolsVersion, sdkSelection: declaration.sdk, platform: options.platform, runtimeRequirements,
            },
        };
    } catch (error) { return failure("declarations", error); }
}

async function exactArchive(toolchain, product, version, options, deps, attempts) {
    const metadata = [];
    const rows = await releases(options, deps, metadata, attempts, version.split(".").slice(0, 2).join("."));
    for (const row of rows) {
        const entries = product === "sdk" ? sdkEntries(row.release)
            : [row.release[{ dotnet: "runtime", aspnetcore: "aspnetcore-runtime", windowsdesktop: "windowsdesktop" }[product]]].filter(Boolean);
        const found = entries.find(entry => entry.version === version);
        if (found) return {
            archive: selectArchive(found, toolchain.platform, options.policy), metadata,
            bundledRuntime: { version: found["runtime-version"] ?? row.release.runtime?.version },
        };
    }
    const family = { sdk: "Sdk", dotnet: "Runtime", aspnetcore: "aspnetcore/Runtime", windowsdesktop: "WindowsDesktop" }[product];
    const prefix = { sdk: "dotnet-sdk", dotnet: "dotnet-runtime", aspnetcore: "aspnetcore-runtime", windowsdesktop: "windowsdesktop-runtime" }[product];
    const name = `${prefix}-${version}-${toolchain.platform.rid}${toolchain.platform.os === "windows" ? ".zip" : ".tar.gz"}`;
    const urls = options.policy.toolchains.publicFeeds
        .filter(f => /(?:builds\.dotnet\.microsoft\.com\/dotnet|ci\.dot\.net\/public)$/.test(f))
        .map(feed => `${feed}/${family}/${version}/${name}`);
    for (const url of urls) {
        const attempt = { url: `${url}.sha512`, version, status: "pending" };
        attempts.push(attempt);
        const response = await deps.http(attempt.url);
        attempt.status = response.status;
        validateResponse(attempt.url, response, options.policy);
        if (response.status === 404) continue;
        if (response.status !== 200) throw Object.assign(new Error(`checksum HTTP ${response.status}: ${attempt.url}`), { code: "network-failure" });
        const checksum = Buffer.from(response.bytes).toString("utf8").trim().split(/\s+/)[0];
        if (!/^[a-f0-9]{128}$/i.test(checksum)) throw Object.assign(new Error(`malformed historical checksum: ${attempt.url}`),
            { code: "checksum-provenance" });
        metadata.push({ url: attempt.url, redirectUrl: response.url, sha256: hash(response.bytes), bytesBase64: Buffer.from(response.bytes).toString("base64") });
        return { archive: { name, url, sha512: checksum.toLowerCase(), rid: toolchain.platform.rid,
            architecture: toolchain.platform.architecture, provenance: { kind: "publisher-checksum" } }, metadata };
    }
    // Availability is not checksum provenance. HEAD never downloads or executes unapproved bytes.
    for (const url of urls) {
        const approval = options.policy.toolchains.measuredArchives.find(a => toolchain.kind === "source" &&
            a.sourceSha === toolchain.sourceSha && a.product === product && a.version === version &&
            a.rid === toolchain.platform.rid && a.url === url);
        const constraints = approval ? { allowedUrls: [url, ...approval.redirectUrls],
            maxBytes: approval.maxBytes, timeoutMs: approval.timeoutMs } : undefined;
        const attempt = { url, version, method: "HEAD", status: "pending" };
        attempts.push(attempt);
        const response = await deps.http(url, { method: "HEAD", ...(constraints ? { constraints } : {}) });
        attempt.status = response.status;
        validateDownloadProvenance(() => validateHttpResponse(url, response, options.policy, constraints));
        if (response.status === 404) continue;
        if (response.status === 200 && approval) return {
            archive: { name, url, rid: toolchain.platform.rid, architecture: toolchain.platform.architecture,
                provenance: { kind: "approved-official-https-measured", approvalRevision: approval.approvalRevision,
                    approvalDigest: fingerprintInput(approval) } }, metadata,
        };
        if (response.status === 200) throw Object.assign(new Error(`exact ${product} ${version} is available but lacks approved checksum provenance`),
            { code: "checksum-provenance" });
        throw Object.assign(new Error(`archive availability HTTP ${response.status}: ${url}`), { code: "network-failure" });
    }
    throw Object.assign(new Error(`exact ${product} ${version} archive unavailable from permitted public feeds`), { code: "archive-unavailable" });
}

function verifyHostRid(toolchain, identity, filePath) {
    requireValue(identity.architecture === toolchain.platform.architecture, "resolved architecture/RID mismatch");
    if (identity.rid === toolchain.platform.rid) return;
    const record = identity.runtimeGraph;
    requireValue(toolchain.kind === "source" && record?.path === filePath.join(identity.sdkBasePath, "RuntimeIdentifierGraph.json"),
        "historical RID requires the acquired SDK's runtime graph");
    const bytes = Buffer.from(record.bytesBase64, "base64");
    requireValue(hash(bytes) === record.sha256, "historical RID graph digest mismatch");
    const graph = JSON.parse(bytes).runtimes;
    const pending = [identity.rid];
    const seen = new Set();
    const portable = new Set();
    for (const rid of pending) {
        if (seen.has(rid)) continue;
        seen.add(rid);
        requireValue(typeof rid === "string", "invalid historical RID");
        if (/^(win|linux(?:-musl)?|osx)-(x64|arm64)$/.test(rid)) {
            // Stop at the ABI boundary: linux-musl imports linux, but is not glibc.
            portable.add(rid);
        } else {
            const imports = graph?.[rid]?.["#import"];
            requireValue(Array.isArray(imports), `unsupported historical RID: ${rid}`);
            pending.push(...imports.filter(r => typeof r === "string" && r.endsWith(`-${toolchain.platform.architecture}`)));
        }
    }
    requireValue(portable.size === 1 && portable.has(toolchain.platform.rid), "resolved architecture/RID mismatch");
}

function verifyToolchainIdentity({ policy, toolchain, identity }, filePath = path) {
    try {
        validatePolicy(policy);
        validatePlatform(toolchain.platform);
        const root = identity.root;
        requireValue(filePath.isAbsolute(root) && root === toolchain.installationRoot &&
            identity.hostPath === filePath.join(root, toolchain.platform.os === "windows" ? "dotnet.exe" : "dotnet"),
            "wrong selected host path");
        requireValue(identity.sdkVersion === toolchain.sdkVersion && toolchain.toolsVersion === toolchain.sdkVersion, "resolved SDK version mismatch");
        requireValue(identity.sdkBasePath === filePath.join(root, "sdk", toolchain.sdkVersion), "wrong resolved SDK base path");
        verifyHostRid(toolchain, identity, filePath);
        requireValue(identity.sdks.length === 1 && identity.sdks[0].version === toolchain.sdkVersion &&
            identity.sdks[0].path === filePath.join(root, "sdk"), "unexpected installed SDK or SDK location");
        validateArchiveReceipt({ policy, ...toolchain, version: toolchain.sdkVersion, product: "sdk" }, identity.archive, filePath);
        requireValue(Array.isArray(identity.installations), "missing installation receipts");
        const sdks = identity.installations.filter(i => i.product === "sdk");
        requireValue(sdks.length === 1 && sdks[0].version === toolchain.sdkVersion &&
            fingerprintInput(sdks[0].archive) === fingerprintInput(identity.archive), "missing or mismatched SDK installation receipt");
        const runtimes = identity.installations.filter(i => i.product !== "sdk");
        for (const installed of runtimes) {
            requireValue(toolchain.runtimeRequirements.some(r => runtimeProducts[r.name] === installed.product && r.version === installed.version),
                "unexpected supplemental installation receipt");
            validateArchiveReceipt({ policy, ...toolchain, version: installed.version, product: installed.product,
                archive: installed.archive }, installed.archive, filePath);
        }
        for (const required of toolchain.runtimeRequirements)
            requireValue(identity.installations.some(i => i.archive.url === required.archiveUrl &&
                (i.product === "sdk" || i.product === runtimeProducts[required.name] && i.version === required.version)),
            "missing required runtime archive receipt");
        for (const [name, assemblyName] of Object.entries({ compiler: "fsc", fsi: "fsi", core: "FSharp.Core" })) {
            const file = identity.files[name];
            const relative = file && filePath.relative(filePath.join(root, "sdk", toolchain.sdkVersion, "FSharp"), file.path);
            requireValue(file && relative && !relative.startsWith(`..${filePath.sep}`) && relative !== ".." &&
                !filePath.isAbsolute(relative) && filePath.basename(file.path) === `${assemblyName}.dll` && file.assemblyName === assemblyName &&
                /^[a-f0-9]{64}$/.test(file.sha256) && /^\d+\.\d+\.\d+\.\d+$/.test(file.assemblyVersion) &&
                file.fileVersion, `missing or invalid bundled F# ${name} identity`);
        }
        for (const runtime of identity.runtimes) {
            requireValue(runtime.path === filePath.join(root, "shared", runtime.name) &&
                /^[a-f0-9]{64}$/.test(runtime.sha256), "wrong runtime installation identity");
        }
        for (const required of [{ name: "Microsoft.NETCore.App", version: toolchain.bundledRuntime.version }, ...toolchain.runtimeRequirements]) {
            requireValue(identity.runtimes.some(r => r.name === required.name && r.version === required.version), `missing runtime ${required.name} ${required.version}`);
        }
        for (const required of toolchain.runtimeRequirements.filter(r => r.name === "Microsoft.NETCore.App" && ["compiler", "fsi"].includes(r.role))) {
            requireValue(identity.runtimeExecutions?.some(r => r.role === required.role &&
                r.configPath === required.config.path && r.configDigest === required.config.sha256 &&
                r.name === required.name && r.version === required.version && r.architecture === toolchain.platform.architecture),
            `unverified ${required.role} execution runtime ${required.version}`);
        }
        requireValue(identity.compilerRuntime.name === "Microsoft.NETCore.App" &&
            identity.compilerRuntime.version === toolchain.bundledRuntime.version, "wrong bundled compiler execution runtime");
        return { status: "verified" };
    } catch (error) { return failure("identity", error); }
}

async function acquireToolchain(options, injected = {}) {
    const attempts = [];
    let stage = "acquisition";
    try {
        const deps = dependencies(options, injected);
        let toolchain = structuredClone(options.toolchain);
        validatePlatform(toolchain.platform);
        requireValue(exactVersion.test(toolchain.sdkVersion) && toolchain.sdkVersion === toolchain.toolsVersion, "inconsistent requested SDK");
        requireValue(["released", "source"].includes(toolchain.kind), "unsupported toolchain kind");
        const storage = await deps.fs.realpath(options.storageDirectory);
        const work = await deps.fs.mkdtemp(path.join(storage, "toolchain-"));
        const env = isolatedEnvironment(work, toolchain.platform);
        for (const folder of [env.DOTNET_ROOT, env.HOME, env.TEMP, env.NUGET_PACKAGES, env.NUGET_HTTP_CACHE_PATH, env.APPDATA, env.LOCALAPPDATA]) {
            await deps.fs.mkdir(folder, { recursive: true });
        }
        const root = await deps.fs.realpath(env.DOTNET_ROOT);
        toolchain.installationRoot = root;
        const sampleDirectory = path.join(work, "sample");
        await deps.fs.mkdir(sampleDirectory);
        await deps.fs.writeFile(path.join(sampleDirectory, "global.json"), JSON.stringify({
            sdk: { version: toolchain.sdkVersion, rollForward: "disable", allowPrerelease: toolchain.kind === "source" },
        }), { flag: "wx" });
        const nugetConfig = path.join(toolchain.platform.os === "windows" ? env.APPDATA : path.join(env.HOME, ".nuget"), "NuGet");
        await deps.fs.mkdir(nugetConfig, { recursive: true });
        await deps.fs.writeFile(path.join(nugetConfig, "NuGet.Config"), "<configuration><packageSources><clear /></packageSources></configuration>");
        if (toolchain.kind === "source") {
            requireValue(hash(await deps.fs.readFile(toolchain.globalJson.path)) === toolchain.globalJson.sha256, "historical global.json changed");
            for (const sdkPath of toolchain.sdkSelection.paths ?? []) {
                if (sdkPath === "$host$") continue;
                requireValue(typeof sdkPath === "string" && !path.isAbsolute(sdkPath) &&
                    inside(toolchain.sourceDirectory, path.resolve(toolchain.sourceDirectory, sdkPath)), "unsupported historical SDK search path");
                try {
                    const actual = await deps.fs.realpath(path.resolve(toolchain.sourceDirectory, sdkPath));
                    requireValue(actual === root, `historical SDK search path is not isolated: ${actual}`);
                } catch (error) {
                    if (error.code !== "ENOENT") throw error;
                }
            }
            stage = "sdk";
            const exact = await exactArchive(toolchain, "sdk", toolchain.sdkVersion, options, deps, attempts);
            toolchain = { ...toolchain, ...exact };
        }
        publicUrl(toolchain.archive.url, options.policy);
        const installer = deps.installer ?? require("./toolchain-installer.cjs").installArchive;
        const installed = [];
        const install = async (product, version, archive) => {
            const result = await installer({
                helperRoot: path.join(options.helperRoot ?? (toolchain.kind === "source" ? toolchain.sourceDirectory : path.resolve(__dirname, "..", "..")),
                    "eng", "common"),
                root, archivePath: path.join(work, `${product}-${version}${toolchain.platform.os === "windows" ? ".zip" : ".tar.gz"}`),
                product, version, platform: toolchain.platform, archive, env, cwd: sampleDirectory,
                policy: options.policy, kind: toolchain.kind, sourceSha: toolchain.sourceSha, sources: structuredClone(attempts),
            }, deps);
            attempts.push(...result.attempts);
            validateArchiveReceipt({ policy: options.policy, ...toolchain, version, product, archive }, result.archive);
            installed.push({ product, version, ...result });
        };
        stage = "sdk";
        await install("sdk", toolchain.sdkVersion, toolchain.archive);
        // An unreleased SDK's runtime version is obtained from its verified archive.
        if (!toolchain.bundledRuntime?.version) {
            const config = JSON.parse(await deps.fs.readFile(path.join(root, "sdk", toolchain.sdkVersion, "FSharp", "fsi.runtimeconfig.json"), "utf8"));
            const framework = config.runtimeOptions?.framework;
            requireValue(framework?.name === "Microsoft.NETCore.App" && exactVersion.test(framework.version), "unsupported historical bundled runtime");
            const versions = await deps.fs.readdir(path.join(root, "shared", framework.name));
            requireValue(versions.length === 1 && exactVersion.test(versions[0]), "ambiguous historical bundled runtime");
            toolchain.bundledRuntime = { version: versions[0] };
        }
        stage = "runtime";
        for (const required of toolchain.runtimeRequirements) {
            const location = path.join(root, "shared", required.name, required.version);
            try { await deps.fs.access(location); } catch (error) {
                if (error.code !== "ENOENT") throw error;
                const product = runtimeProducts[required.name];
                requireValue(product && exactVersion.test(required.version), "unsupported explicit runtime requirement");
                const exact = await exactArchive(toolchain, product, required.version, options, deps, attempts);
                await install(product, required.version, exact.archive);
            }
            required.archiveUrl = (installed.find(i => i.product === runtimeProducts[required.name] &&
                i.version === required.version) ?? installed[0]).archive.url;
        }
        stage = "identity";
        const executable = path.join(root, toolchain.platform.os === "windows" ? "dotnet.exe" : "dotnet");
        const cwd = toolchain.kind === "source" ? toolchain.sourceDirectory : sampleDirectory;
        const identity = await deps.capture({ root, executable, cwd, env, toolchain }, deps);
        identity.archive = installed[0].archive;
        identity.installations = installed;
        identity.requested = toolchain;
        const verification = verifyToolchainIdentity({ policy: options.policy, toolchain, identity });
        if (verification.status !== "verified") return { ...verification, attempts };
        if (toolchain.kind === "source") {
            requireValue(hash(await deps.fs.readFile(toolchain.globalJson.path)) === toolchain.globalJson.sha256, "historical global.json changed");
        }
        const localCompilerRuntime = identity.runtimeExecutions.find(r => r.role === "compiler");
        if (toolchain.kind === "source" && localCompilerRuntime && identity.runtimeExecutions.some(r => r.role === "fsi")) {
            identity.buildToolchain = {
                sourceSha: toolchain.sourceSha, globalJsonDigest: toolchain.globalJson.sha256,
                evidenceDigest: fingerprintInput(identity), requestedSdk: toolchain.sdkVersion, requestedTools: toolchain.toolsVersion,
                requestedCompilerRuntime: toolchain.runtimeRequirements.find(r => r.role === "compiler" && r.name === "Microsoft.NETCore.App").version,
                sdk: { version: identity.sdkVersion, sha256: identity.archive.sha256, verified: true },
                archive: identity.archive,
                compilerRuntime: {
                    version: localCompilerRuntime.version,
                    sha256: identity.runtimes.find(r => r.name === localCompilerRuntime.name && r.version === localCompilerRuntime.version).sha256,
                    verified: true,
                }, verified: true, supported: true,
            };
        }
        if (toolchain.kind === "released") {
            identity.stableSdk = {
                version: identity.sdkVersion, origin: "official-release-index", latest: true, verified: true,
                releaseIndexUrl: indexUrl, releaseIndexDigest: toolchain.metadata.find(m => m.url === indexUrl).sha256,
                archives: [{ os: toolchain.platform.os, architecture: toolchain.platform.architecture, sha256: identity.archive.sha256 }],
            };
        }
        const evidencePath = path.join(work, "identity.json");
        await deps.fs.writeFile(evidencePath, JSON.stringify({ identity, attempts }), { flag: "wx" });
        return { status: "acquired", identity, evidencePath, launch: { executable, cwd, env, sampleDirectory }, attempts };
    } catch (error) { return failure(stage, error, [...attempts, ...(error.attempts ?? [])]); }
}

module.exports = { resolveLatestStableSdk, readRevisionToolchain, acquireToolchain, verifyToolchainIdentity };
