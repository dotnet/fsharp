const fs = require("node:fs/promises");
const path = require("node:path");
const { execFileSync } = require("node:child_process");
const { randomBytes } = require("node:crypto");
const { image, runContainer, validateContainerConfig, sourceBundleIdentity } = require("./container.cjs");
const { freeze } = require("./fixtures/trial-source.cjs");
const { resolveLatestStableSdk } = require("./toolchains.cjs");
const { validateExecutionUnit } = require("./worker.cjs");
const { fingerprintInput: fingerprint } = require("./model.cjs");
const { createHttp, validateHttpResponse, hash, requireValue } = require("./toolchain-io.cjs");
const { execute } = require("./repro-process.cjs");
const policy = require("../../.github/policies/regression-bisect.json");
const platform = { os: "linux", architecture: "x64", rid: "linux-x64" };
const repository = path.resolve(__dirname, "..", "..");

function sourceUnit({ revision, sourceSha, manifest, resolved, archive, compilerRuntime, now = new Date() }) {
    requireValue(archive.status === 200 && /^[a-f0-9]{64}$/.test(archive.sha256) &&
        archive.sha512 === resolved.archive.sha512, "stable archive must match publisher metadata");
    requireValue(compilerRuntime?.name === "Microsoft.NETCore.App" && /^\d+\.\d+\.\d+$/.test(compilerRuntime.version),
        "exact source compiler runtime required");
    const unit = {
        schemaVersion: 1, repository: "dotnet/fsharp", issueNumber: manifest.issueNumber,
        buildInstanceId: `source-smoke-${randomBytes(16).toString("hex")}`,
        epoch: fingerprint({ purpose: "source-worker acceptance only", revision, sourceSha }),
        deadline: new Date(now.getTime() + policy.limits.workerTimeoutMinutes * 60000).toISOString(),
        environmentFingerprint: fingerprint(Object.fromEntries(manifest.matrix.map(c =>
            [c.id, { os: c.os, architecture: c.architecture, imageDigest: image.split(":").at(-1) }]))),
        policyDigest: fingerprint(policy), policyRevision: revision, workflowRevision: revision,
        manifest, platform, request: { kind: "source", endpoint: "good", phase: "confirmation", target: { sourceSha } },
        resolved: { ...resolved, runtimeRequirements: [...resolved.runtimeRequirements, { ...compilerRuntime, role: "sample" }] },
        stableSdk: { version: resolved.sdkVersion, origin: "official-release-index", latest: true, verified: true,
            releaseIndexUrl: resolved.metadata[0].url, releaseIndexDigest: resolved.metadata[0].sha256,
            archives: [{ os: "linux", architecture: "x64", sha256: archive.sha256 }] },
    };
    validateExecutionUnit(unit);
    return unit;
}

async function main(args = process.argv.slice(2)) {
    const [opt, configurationFile, directory, containmentDirectory, extra] = args;
    requireValue((opt === "--run" && !containmentDirectory || opt === "--controller" && path.isAbsolute(containmentDirectory ?? "")) &&
        configurationFile && directory && !extra &&
        path.isAbsolute(configurationFile) && path.isAbsolute(directory),
    "usage: node eng\\regression-bisect\\source-worker.smoke.cjs <--run|--controller> <absolute-reviewed-config.json> <absolute-new-evidence-directory> [absolute-containment-directory]");
    requireValue(process.platform === "linux" && process.arch === "x64",
        "source-worker smoke requires the authorized disposable Linux x64 runtime");
    const stat = await fs.lstat(configurationFile);
    requireValue(stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.size < 48 * 1024 &&
        await fs.realpath(configurationFile) === configurationFile, "config must be a bounded resolved regular file");
    const configBytes = await fs.readFile(configurationFile);
    const config = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(configBytes));
    validateContainerConfig(config);
    await fs.mkdir(directory);
    const root = await fs.realpath(directory);
    requireValue(root === path.resolve(directory), "source-worker evidence directory cannot resolve through links");
    const report = {
        schemaVersion: 1, purpose: "source-worker acceptance only", status: "incomplete",
        configSha256: hash(configBytes), sourceSha256: hash(await fs.readFile(__filename)),
        startedAt: new Date().toISOString(), controllerAcceptance: false, projectMatrixAcceptance: false,
        command: [process.execPath, ...process.argv.slice(1)],
        immutableProducerAcceptance: false, fullTrialAcceptance: false,
        allocation: { sourceBuilds: 1, workerUnits: opt === "--controller" ? 2 : 1,
            reservedMinutes: policy.limits.workerTimeoutMinutes * (opt === "--controller" ? 2 : 1) },
    };
    const cancellation = new AbortController();
    const cancel = () => cancellation.abort(new Error("source-worker smoke cancelled"));
    for (const name of ["SIGTERM", "SIGINT"]) process.once(name, cancel);
    try {
        const frozen = await freeze(path.join(root, "inputs"), "/usr/bin/git");
        const manifest = JSON.parse(await fs.readFile(path.join(root, "inputs", "frozen-manifest.json"), "utf8"));
        const resolved = await resolveLatestStableSdk({ policy, platform, signal: cancellation.signal });
        requireValue(resolved.status === "resolved", `stable SDK resolution failed: ${resolved.error?.message}`);
        requireValue(fingerprint(frozen.stable) === fingerprint({
            sdkVersion: resolved.toolchain.sdkVersion, archive: resolved.toolchain.archive,
            metadata: resolved.toolchain.metadata.map(({ url, sha256 }) => ({ url, sha256 })),
        }), "official stable metadata changed during fixture preparation");
        const archive = await createHttp(policy, fs, cancellation.signal)(resolved.toolchain.archive.url,
            { destination: path.join(root, "stable.archive") });
        validateHttpResponse(resolved.toolchain.archive.url, archive, policy);
        const revision = execFileSync("/usr/bin/git", ["rev-parse", "HEAD"],
            { cwd: repository, encoding: "utf8", timeout: 60000 }).trim();
        const unit = sourceUnit({ revision, sourceSha: frozen.source.good, manifest, resolved: resolved.toolchain, archive,
            compilerRuntime: frozen.source.compilerRuntime });
        report.revision = revision;
        report.unit = unit;
        const sourceBundle = path.join(root, "source.bundle");
        execFileSync("/usr/bin/git", ["-C", frozen.source.directory, "bundle", "create", sourceBundle, "refs/heads/main"],
            { timeout: 60000, maxBuffer: 1024 * 1024 });
        const sourceBundleSha256 = (await sourceBundleIdentity(sourceBundle, cancellation.signal)).sha256;
        report.sourceBundleSha256 = sourceBundleSha256;
        if (opt === "--controller") {
            Object.assign(report, await require("./controller.smoke.cjs").acceptController({
                root, config, configBytes, containmentDirectory, revision, frozen, unit, key: randomBytes(32),
                sourceDirectory: path.join(root, "inputs", "sample"), sourceBundle, sourceBundleSha256, signal: cancellation.signal,
            }));
            report.status = "passed";
            return;
        }
        const evidenceDirectory = path.join(root, "launch");
        await fs.mkdir(evidenceDirectory);
        const key = randomBytes(32);
        console.log(JSON.stringify({ phase: "source-worker", buildInstanceId: unit.buildInstanceId,
            sourceSha: unit.request.target.sourceSha, deadline: unit.deadline, evidenceDirectory }));
        const output = await runContainer(config, {
            operation: "execution", input: unit, evidenceDirectory, sourceDirectory: path.join(root, "inputs", "sample"),
            sourceBundle, sourceBundleSha256, receiptKey: key,
            signal: AbortSignal.any([cancellation.signal, AbortSignal.timeout(policy.limits.workerTimeoutMinutes * 60000)]),
        });
        report.transportStatus = output.status;
        report.transportReason = output.reason;
        requireValue(output.status === "collected", `source-worker transfer incomplete: ${output.reason}`);
        const reread = await execute(process.execPath, ["-e", `
const { readCollectedExecution } = require("./eng/regression-bisect/container.cjs");
const { fingerprintInput } = require("./eng/regression-bisect/model.cjs");
readCollectedExecution(process.argv[1], {
    keys: { smoke: Buffer.from(process.env.RB_SMOKE_KEY, "hex") }, keyId: "smoke",
}).then(value => console.log(JSON.stringify({
    resultDigest: fingerprintInput(value.result), launchId: value.launch.launchId,
    cleanup: value.receipt.cleanup, files: value.files.length, blobs: value.blobs.length,
}))).catch(error => { console.error(error.message); process.exitCode = 1; });
`, evidenceDirectory], {
            cwd: repository, env: { PATH: "/usr/bin:/bin", RB_SMOKE_KEY: key.toString("hex") },
            timeoutMs: 300000, maxBytes: 16384,
        });
        requireValue(reread.complete && reread.exitCode === 0, `fresh retained-evidence authentication failed: ${reread.stderr}`);
        report.freshReader = JSON.parse(reread.stdout);
        requireValue(report.freshReader.resultDigest === fingerprint(output.result) &&
            report.freshReader.launchId === output.receipt.launchId &&
            report.freshReader.cleanup.length === 4 && report.freshReader.cleanup.every(r => r.removed),
        "fresh retained evidence or owned cleanup differs from the physical launch");
        report.outcome = output.result.outcome;
        report.workerError = output.result.error;
        requireValue(output.result.actualSourceSha === unit.request.target.sourceSha &&
            output.result.status === "tested" && output.result.outcome.outcome === "pass",
        `clean source/FSI smoke incomplete: ${output.result.error?.message ?? output.result.outcome?.reason}`);
        report.status = "passed";
    } catch (error) {
        report.reason = error.message;
        process.exitCode = 2;
    } finally {
        for (const name of ["SIGTERM", "SIGINT"]) process.removeListener(name, cancel);
        report.completedAt = new Date().toISOString();
        await fs.writeFile(path.join(root, "source-worker.json"), JSON.stringify(report, null, 2), { flag: "wx" });
    }
    console.log(JSON.stringify({ status: report.status, reason: report.reason, evidenceDirectory: root }));
}

if (require.main === module) main().catch(error => { console.error(error.message); process.exitCode = 2; });
module.exports = { sourceUnit, main };
