const { validateManifest, validatePolicy, fingerprintInput: fingerprint } = require("./model.cjs");
const toolchains = require("./toolchains.cjs");
const { runReproduction } = require("./repro.cjs");
const { testSourceRevision } = require("./workspace.cjs");
const { execute } = require("./repro-process.cjs");
const { requireValue } = require("./toolchain-io.cjs");

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

module.exports = { validateExecutionUnit, runExecutionUnit };
