const fs = require("node:fs/promises");
const path = require("node:path");
const { execFileSync } = require("node:child_process");
const { createHttp, hash, requireValue, metadataBytes, validateHttpResponse } = require("../toolchain-io.cjs");
const { resolveLatestStableSdk, readRevisionToolchain, selectArchive } = require("../toolchains.cjs");
const { inspectRevisionLayout } = require("../workspace.cjs");
const { createFixture } = require("./smoke.cjs");
const { seal } = require("./contracts.cjs");

const base = "8c0e444de18218ecec91475a6022872e93299e10";
const sourcePath = "src/Compiler/Interactive/fsi.fs";
const samplePath = "regression-trial/repro.fsx";
const sample = 'printfn "TRIAL_SAMPLE"\n';
const sentinel = '        fsiConsoleOutput.Out.WriteLine("SYNTHETIC_ACCEPTANCE_SENTINEL")\n';
const platform = { os: "linux", architecture: "x64", rid: "linux-x64" };
const policy = require("../../../.github/policies/regression-bisect.json");
const repository = path.resolve(__dirname, "..", "..", "..");

async function freeze(directory, executable, injected = {}) {
    const signal = AbortSignal.timeout(300000);
    requireValue(path.isAbsolute(directory) && path.isAbsolute(executable), "absolute new directory and Git executable required");
    await fs.mkdir(directory);
    requireValue(await fs.realpath(directory) === directory, "fixture directory cannot resolve through links");
    const mirror = path.join(directory, "history.git");
    const env = { ...(process.platform === "win32" ? { SystemRoot: process.env.SystemRoot } : {}),
        GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(directory, "no-config"), GIT_ALLOW_PROTOCOL: "file",
        GIT_NO_REPLACE_OBJECTS: "1", GIT_INDEX_FILE: path.join(directory, "index"),
        GIT_AUTHOR_NAME: "Synthetic acceptance fixture", GIT_AUTHOR_EMAIL: "fixture@example.invalid",
        GIT_COMMITTER_NAME: "Synthetic acceptance fixture", GIT_COMMITTER_EMAIL: "fixture@example.invalid",
        GIT_AUTHOR_DATE: "2026-09-22T00:00:00Z", GIT_COMMITTER_DATE: "2026-09-22T00:00:00Z" };
    const trace = [];
    const git = (args, input, cwd = mirror) => {
        const start = Date.now();
        const raw = execFileSync(executable, ["-c", "core.hooksPath=", "-c", "credential.helper=", ...args],
            { cwd, env, input, encoding: "utf8", shell: false, timeout: 60000, maxBuffer: 4 * 1024 * 1024 });
        trace.push({ args, exitCode: 0, durationMs: Date.now() - start });
        return raw;
    };
    git(["clone", "--quiet", "--bare", "--shared", "--no-hardlinks", repository, mirror], undefined, directory);
    const stock = git(["show", `${base}:${sourcePath}`]);
    const anchor = '    member x.Run() =\n        progress <- isEnvVarSet "FSHARP_INTERACTIVE_PROGRESS"';
    requireValue(stock.split(anchor).length === 2 && !stock.includes(sentinel.trim()), "reviewed FsiEvaluationSession.Run symbol changed");
    const goodSource = stock.replace(anchor, anchor.replace("\n", `\n${sentinel}`));
    const blob = text => git(["hash-object", "-w", "--stdin"], text).trim();
    const index = (name, id) => git(["update-index", "--add", "--cacheinfo", `100644,${id},${name}`]);
    git(["read-tree", base]);
    index(samplePath, blob(sample));
    index(sourcePath, blob(goodSource));
    const good = git(["commit-tree", git(["write-tree"]).trim(), "-p", base],
        "SYNTHETIC ACCEPTANCE ONLY: add FSI sentinel and frozen sample\n").trim();
    index(sourcePath, blob(stock));
    const candidate = git(["commit-tree", git(["write-tree"]).trim(), "-p", good],
        "SYNTHETIC ACCEPTANCE ONLY: remove FSI sentinel\n").trim();
    const main = git(["commit-tree", git(["write-tree"]).trim(), "-p", candidate],
        "SYNTHETIC ACCEPTANCE ONLY: no-op bad main\n").trim();
    git(["update-ref", "refs/heads/main", main]);
    const files = [];
    const save = async (role, name, raw) => {
        const file = path.join(directory, name);
        const bytes = typeof raw === "string" ? raw : JSON.stringify(raw, null, 2);
        await fs.writeFile(file, bytes, { flag: "wx" });
        files.push({ role, path: file, sha256: hash(bytes) });
    };
    await save("good-patch", "good.patch", git(["diff", "--no-ext-diff", "--no-textconv", "--binary", base, good]));
    await save("introducing-patch", "introducing.patch", git(["diff", "--no-ext-diff", "--no-textconv", "--binary", good, candidate]));
    const declarations = path.join(directory, "declarations");
    const names = ["global.json", "eng/TargetFrameworks.props", "build.sh", "Build.cmd", "eng/build.sh", "eng/Build.ps1",
        "eng/build-utils.ps1", "proto.proj", "NuGet.config", "src/fsi/fsiProject/fsi.fsproj", "src/fsc/fscProject/fsc.fsproj",
        "src/FSharp.Core/FSharp.Core.fsproj", "src/Microsoft.FSharp.Compiler/Microsoft.FSharp.Compiler.fsproj"];
    for (const name of names) {
        const file = path.join(declarations, name);
        await fs.mkdir(path.dirname(file), { recursive: true });
        await fs.writeFile(file, git(["show", `${base}:${name}`]));
    }
    const layout = { sourceDirectory: declarations, evidenceRoot: directory, evidence: [], buildInstanceId: "layout",
        options: { request: { platform } } };
    await inspectRevisionLayout(layout);
    const declared = await readRevisionToolchain({ policy, sourceDirectory: declarations, sourceSha: base, platform });
    requireValue(declared.status === "resolved" && declared.toolchain.sdkVersion === "10.0.301",
        "exact reviewed source SDK declaration required");
    const latest = await resolveLatestStableSdk({ policy, platform, signal }, injected);
    requireValue(latest.status === "resolved", latest.error?.message);
    for (const [i, record] of latest.toolchain.metadata.entries())
        await save(`metadata-${i}`, `metadata-${i}.json`, metadataBytes(record).toString("utf8"));
    const channel = latest.toolchain.metadata.map(m => JSON.parse(metadataBytes(m)))
        .find(m => m["channel-version"] === "10.0");
    const sourceRelease = channel.releases.find(r => [r.sdk, ...(r.sdks ?? [])].some(s => s?.version === declared.toolchain.sdkVersion));
    const sdk = [sourceRelease?.sdk, ...(sourceRelease?.sdks ?? [])].find(s => s?.version === declared.toolchain.sdkVersion);
    const compilerRuntime = { name: "Microsoft.NETCore.App", version: sdk?.["runtime-version"] ?? sourceRelease?.runtime?.version };
    requireValue(/^\d+\.\d+\.\d+$/.test(compilerRuntime.version), "source SDK lacks an exact publisher runtime version");
    const runtime = channel.releases.find(r => r.runtime?.version === "10.0.0")?.runtime;
    const archives = [];
    for (const [product, record] of [["sdk", sdk], ["dotnet", runtime]]) {
        const archive = selectArchive(record, platform, policy, product);
        const response = await (injected.http ?? createHttp(policy, fs, signal))(archive.url, { method: "HEAD" });
        validateHttpResponse(archive.url, response, policy);
        requireValue(response.status === 200, "reviewed Linux archive unavailable");
        archives.push({ product, version: record.version, archive, availability: { status: response.status,
            url: response.url, redirects: response.redirects, checkedAt: new Date().toISOString() } });
    }
    await save("source-toolchain", "source-toolchain.json", { declaration: declared.toolchain, archives, adapter: layout.adapter });
    const stable = { sdkVersion: latest.toolchain.sdkVersion, archive: latest.toolchain.archive,
        metadata: latest.toolchain.metadata.map(({ url, sha256 }) => ({ url, sha256 })) };
    await save("stable-toolchain", "stable-toolchain.json", stable);
    const fixtureDirectory = path.join(directory, "sample");
    const manifest = await createFixture(fixtureDirectory, { kind: "fsi", tfm: "net10.0" });
    manifest.issueNumber = 123;
    manifest.sources = [{ path: samplePath, sha256: hash(sample) }];
    manifest.assertions = { pass: { exitCode: 0, stdout: "SYNTHETIC_ACCEPTANCE_SENTINEL\nTRIAL_SAMPLE\n", stderr: "" },
        failure: { exitCode: 0, stdout: "TRIAL_SAMPLE\n", stderr: "" } };
    manifest.matrix[0] = { ...manifest.matrix[0], os: "linux", architecture: "x64", command: ["fsi", samplePath] };
    manifest.searchHints.sourceShas = [good];
    seal(manifest);
    await fs.unlink(path.join(fixtureDirectory, "repro.fsx"));
    await fs.mkdir(path.join(fixtureDirectory, "regression-trial"));
    await save("sample", path.join("sample", samplePath), sample);
    await fs.writeFile(path.join(fixtureDirectory, "manifest.json"), JSON.stringify(manifest, null, 2));
    await save("assertion", "frozen-manifest.json", manifest);
    const result = { purpose: "SYNTHETIC ACCEPTANCE ONLY", source: { directory: mirror, base, good, candidate, main, compilerRuntime },
        stable, files, plan: { sourceBuilds: 5, workerUnits: 6, reservedMinutes: 660, totalMinutes: 720,
            attempts: { [good]: 2, [candidate]: 2, [main]: 1 }, actualProductRuns: 0 },
        approval: "Reviewed exact FsiEvaluationSession.Run sentinel only; issue/discussion and physical environment approval still required." };
    await save("graph", "source-graph.json", result);
    await save("trace", "git-trace.json", trace);
    await fs.unlink(path.join(directory, "index"));
    return result;
}

if (require.main === module) {
    const [directory, executable, extra] = process.argv.slice(2);
    if (extra || !directory || !executable) throw new Error("Usage: node fixtures\\trial-source.cjs <absolute-new-directory> <absolute-git>");
    freeze(directory, executable).then(result => console.log(JSON.stringify(result)))
        .catch(error => { console.error(error.message); process.exitCode = 1; });
}
module.exports = { freeze, base, sourcePath, samplePath, sample, sentinel };
