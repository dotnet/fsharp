const { validateManifest, validatePolicy, validateOutcome, fingerprintInput: fingerprint } = require("./model.cjs");
const toolchains = require("./toolchains.cjs");
const { runReproduction } = require("./repro.cjs");
const { testSourceRevision } = require("./workspace.cjs");
const { execute } = require("./repro-process.cjs");
const { requireValue, hash } = require("./toolchain-io.cjs");
const fs = require("node:fs/promises");
const path = require("node:path");
const { executionFileNames, validateExecutionFiles } = require("./worker-output.cjs");

function validateExecutionUnit(unit) {
    requireValue(unit && Object.keys(unit).sort().join(" ") ===
        "buildInstanceId deadline environmentFingerprint epoch issueNumber manifest platform policyDigest policyRevision repository request resolved schemaVersion stableSdk workflowRevision",
    "invalid public execution unit fields");
    fingerprint(unit);
    validateManifest(unit.manifest);
    requireValue(unit.schemaVersion === 1 && unit.repository === "dotnet/fsharp" &&
        Number.isSafeInteger(unit.issueNumber) && unit.issueNumber > 0 &&
        unit.manifest.repository === unit.repository && unit.manifest.issueNumber === unit.issueNumber &&
        typeof unit.buildInstanceId === "string" && /^[a-zA-Z0-9_-]{1,128}$/.test(unit.buildInstanceId) &&
        typeof unit.deadline === "string" && Number.isFinite(Date.parse(unit.deadline)) &&
        [unit.epoch, unit.environmentFingerprint, unit.policyDigest].every(v => typeof v === "string" && /^[a-f0-9]{64}$/.test(v)) &&
        [unit.policyRevision, unit.workflowRevision].every(v => typeof v === "string" && /^[a-f0-9]{40}$/.test(v)),
    "invalid public execution identity");
    const r = unit.request;
    requireValue(r && Object.keys(r).sort().join(" ") === "endpoint kind phase target" &&
        ["released", "source"].includes(r.kind) && [null, "stable", "main", "good"].includes(r.endpoint) &&
        ["stable", "main", "good-search", "admission", "bisect", "confirmation"].includes(r.phase) &&
        r.target && Object.keys(r.target).join(" ") === (r.kind === "source" ? "sourceSha" : "sdk") &&
        (r.kind === "source" ? typeof r.target.sourceSha === "string" && /^[a-f0-9]{40}$/.test(r.target.sourceSha) :
            fingerprint(r.target.sdk) === fingerprint(unit.stableSdk)), "invalid public execution request");
    const p = unit.platform;
    requireValue(p && Object.keys(p).sort().join(" ") === "architecture os rid" &&
        ["linux", "windows", "macos"].includes(p.os) && ["x64", "arm64"].includes(p.architecture) &&
        [ `${{ linux: "linux", windows: "win", macos: "osx" }[p.os]}-${p.architecture}`,
            ...(p.os === "linux" ? [`linux-musl-${p.architecture}`] : []) ].includes(p.rid) &&
        unit.manifest.matrix.every(c => c.os === p.os && c.architecture === p.architecture),
    "execution unit requires one supported platform");
    return true;
}

// This is the physical runner, not a containment adapter. Its dependencies belong inside the worker.
async function runExecutionUnit(unit, deps, { signal } = {}) {
    validateExecutionUnit(unit);
    validatePolicy(deps.policy);
    requireValue(fingerprint(deps.policy) === unit.policyDigest, "worker policy bundle mismatch");
    const clock = deps.clock ?? (() => new Date());
    const remaining = Date.parse(unit.deadline) - clock().getTime();
    requireValue(remaining > 0 && remaining <= deps.policy.limits.workerTimeoutMinutes * 60000, "reservation deadline exhausted or invalid");
    signal = AbortSignal.any([AbortSignal.timeout(remaining), ...(signal ? [signal] : [])]);
    const checkDeadline = () => {
        signal.throwIfAborted();
        requireValue(clock().getTime() < Date.parse(unit.deadline), "reservation deadline exhausted");
    };
    const { request, manifest, buildInstanceId, platform } = unit;
    checkDeadline();
    if (request.kind === "source") {
        requireValue(deps.worker?.reproductionToolchain, "frozen reproduction toolchain required separately from revision build SDK");
        const result = await (deps.testSourceRevision ?? testSourceRevision)({
            ...deps.worker, manifest, signal, buildInstanceId,
            request: { schemaVersion: 1, repository: unit.repository, sourceSha: request.target.sourceSha,
                phase: request.phase === "good-search" ? "older-search" : request.phase,
                manifestDigest: fingerprint(manifest), approvalDigest: manifest.approvedInputDigest,
                platform, allocation: { buildTimeoutMs: deps.policy.limits.buildTimeoutMinutes * 60000,
                    workerTimeoutMs: deps.policy.limits.workerTimeoutMinutes * 60000 } },
        }, deps.workspaceDependencies);
        checkDeadline();
        return result;
    }
    const acquired = await (deps.acquireToolchain ?? toolchains.acquireToolchain)({
        policy: deps.policy, toolchain: unit.resolved, storageDirectory: deps.worker.storageDirectory, signal,
    }, deps.toolchainDependencies);
    checkDeadline();
    if (acquired.status !== "acquired") return acquired;
    const verified = (deps.verifyToolchainIdentity ?? toolchains.verifyToolchainIdentity)({
        policy: deps.policy, toolchain: acquired.identity.requested, identity: acquired.identity,
    });
    requireValue(verified.status === "verified", "stable acquisition identity rejected");
    requireValue(acquired.identity.archive.sha256 === unit.stableSdk.archives.find(a =>
        a.os === platform.os && a.architecture === platform.architecture)?.sha256, "stable archive identity changed");
    checkDeadline();
    const product = await deps.inventoryReleased(acquired.identity, buildInstanceId);
    checkDeadline();
    const result = await (deps.runReproduction ?? runReproduction)({
        ...deps.worker, manifest, product, toolchain: acquired.identity,
    }, { execute: (exe, args, settings) => (deps.workerExecute ?? execute)(exe, args, { ...settings, signal }) });
    checkDeadline();
    return result;
}

function readExecutionOutput(files, unit, launch) {
    validateExecutionUnit(unit);
    requireValue(launch && Object.keys(launch).sort().join(" ") === "imageDigest launchId unitDigest" &&
        Object.values(launch).every(v => typeof v === "string" && /^[a-f0-9]{64}$/.test(v)) &&
        launch.unitDigest === fingerprint(unit), "invalid trusted launch binding");
    requireValue(Array.isArray(files), "collected result files required");
    const results = files.filter(f => f.path === "result.json");
    requireValue(results.length === 1, "exactly one collected result file required");
    const file = results[0];
    requireValue(Buffer.isBuffer(file.bytes) && file.bytes.length <= 16 * 1024 * 1024 && file.size === file.bytes.length,
        "invalid result file size");
    requireValue(hash(file.bytes) === file.sha256, "collected result digest mismatch");
    const payload = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(file.bytes));
    requireValue(payload && Object.keys(payload).sort().join(" ") === "imageDigest launchId result schemaVersion unitDigest" &&
        payload.schemaVersion === 1 && ["imageDigest", "launchId", "unitDigest"].every(k => payload[k] === launch[k]),
    "worker output launch binding mismatch");
    requireValue(payload.result && Object.getPrototypeOf(payload.result) === Object.prototype, "invalid worker output schema");
    fingerprint(payload.result);
    validateOutcome(payload.result.outcome);
    return payload.result;
}

async function executionEvidence(unit, launch, result, { acquisition = null, commands = [], evidenceDirectory, scratchDirectory }) {
    validateExecutionUnit(unit);
    const read = async (file, root, optional = false) => {
        try {
            const relative = path.relative(root, file);
            requireValue(relative && !relative.startsWith("..") && !path.isAbsolute(relative) &&
                await fs.realpath(file) === path.resolve(file), "raw evidence path escapes worker scratch or contains links");
            const stat = await fs.lstat(file);
            requireValue(stat.isFile() && stat.nlink === 1 && stat.size <= 16 * 1024 * 1024, "raw evidence must be bounded regular unlinked data");
            const bytes = await fs.readFile(file);
            requireValue(bytes.length === stat.size, "raw evidence changed during export");
            return bytes;
        } catch (error) {
            if (optional && error.code === "ENOENT") return Buffer.from("null");
            throw error;
        }
    };
    const source = unit.request.kind === "source";
    const reproduction = source ? result.reproduction ?? null : result;
    const buildRoot = path.join(evidenceDirectory, unit.buildInstanceId);
    const declarations = source ? await read(path.join(buildRoot, "declarations.json"), evidenceDirectory, true) : Buffer.from("null");
    const buildAcquisition = source ? await read(path.join(buildRoot, "acquisition.json"), evidenceDirectory, true) : Buffer.from("null");
    const identities = reproduction?.identities
        ? await read(path.join(reproduction.workDirectory, "identities.json"), scratchDirectory) : Buffer.from("null");
    if (reproduction?.identities) requireValue(hash(identities) === reproduction.identities.sha256 &&
        reproduction.identities.path === path.join(reproduction.workDirectory, "identities.json"), "raw identity binding mismatch");
    const sourceCommands = [];
    for (let i = 0; i < (source ? result.commands?.length ?? 0 : 0); i++)
        sourceCommands.push(JSON.parse(await read(path.join(buildRoot, `command-${i + 1}.json`), evidenceDirectory)));
    const product = JSON.parse(identities)?.product ?? result.product ?? null;
    const rawProduct = {};
    for (const role of ["fsi", "compiler"]) {
        const config = product?.files.find(f => f.role === role)?.runtimeConfig;
        rawProduct[`${role}.runtimeconfig.json`] = config ? await read(config.path, scratchDirectory) : null;
        if (config) requireValue(hash(rawProduct[`${role}.runtimeconfig.json`]) === config.sha256, "raw runtimeconfig digest mismatch");
    }
    const global = source ? JSON.parse(buildAcquisition)?.identity?.requested.globalJson : null;
    rawProduct["global.json"] = global ? await read(global.path, scratchDirectory) : null;
    if (global) requireValue(hash(rawProduct["global.json"]) === global.sha256, "raw source declaration digest mismatch");
    const values = {
        "result.json": { schemaVersion: 1, ...launch, result }, "unit.json": unit,
        "request.json": source ? result.requested ?? unit.request : unit.request,
        "source.json": source ? result : null, "declarations.json": declarations,
        "acquisition.json": acquisition, "build-acquisition.json": buildAcquisition,
        "product.json": product, ...rawProduct,
        "reproduction.json": reproduction, "identities.json": identities,
        "commands.json": { source: sourceCommands, execution: commands },
    };
    const files = executionFileNames.map(name => {
        const bytes = Buffer.isBuffer(values[name]) ? values[name] : Buffer.from(JSON.stringify(values[name]));
        return { path: name, bytes, size: bytes.length, sha256: hash(bytes) };
    });
    validateExecutionFiles(files.map(({ path, size, sha256 }) => ({ path, size, sha256 })));
    const blobs = [];
    for (const [scope, acquired] of [["reproduction", acquisition], ["build", JSON.parse(buildAcquisition)]]) {
        if (acquired?.status !== "acquired") continue;
        const identity = acquired.identity;
        for (const installation of identity.installations) {
            const { product, version, archive } = installation;
            blobs.push({ slot: `archive-${blobs.length}`, scope, kind: identity.requested.kind,
                sourceSha: identity.requested.sourceSha ?? null, product, version, platform: unit.platform, archive });
        }
    }
    return { files, blobs };
}

module.exports = { validateExecutionUnit, runExecutionUnit, readExecutionOutput, executionEvidence };
