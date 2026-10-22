const fs = require("node:fs/promises");
const path = require("node:path");
const { createGithubApi } = require("./github.cjs");
const { createFileEvidenceStore } = require("./evidence-store.cjs");
const { createObservationCollector } = require("./collector.cjs");
const { bootstrapController, runController } = require("./controller.cjs");
const { withIssueLock, containment } = require("./controller-cli.cjs");
const { inspectRuntime, runContainer } = require("./container.cjs");
const { resolveLatestStableSdk } = require("./toolchains.cjs");
const { readIssueSnapshot } = require("./intake.cjs");
const state = require("./state.cjs");
const { fingerprintInput: fingerprint } = require("./model.cjs");
const { hash, requireValue } = require("./toolchain-io.cjs");
const { execute } = require("./repro-process.cjs");
const { bootstrapFixture } = require("./fixtures/execution.cjs");
const { seal } = require("./fixtures/contracts.cjs");
const { connect, writeDatabase } = require("./fixtures/controller-service.cjs");
const assert = require("node:assert/strict");
const { Readable } = require("node:stream");
const { collectWorkerArchive, workerArchive } = require("./worker-output.cjs");
const { readExecutionOutput } = require("./worker.cjs");

async function rejectOutputAttacks(output, unit) {
    const descriptors = output.files.map(({ path, size, sha256 }) => ({ path, size, sha256 }));
    const chunks = [];
    for await (const bytes of workerArchive(output.files)) chunks.push(bytes);
    const archive = Buffer.concat(chunks), receipts = [];
    for (const mode of ["traversal", "symlink", "oversized"]) {
        const bytes = Buffer.from(archive), files = structuredClone(descriptors);
        if (mode === "oversized") files[0].size = 16 * 1024 * 1024 + 1;
        else {
            if (mode === "traversal") { bytes.fill(0, 0, 100); bytes.write("../escape"); }
            else bytes[156] = 50;
            bytes.fill(32, 148, 156);
            bytes.write(bytes.subarray(0, 512).reduce((sum, b) => sum + b, 0).toString(8).padStart(6, "0") + "\0 ", 148);
        }
        await assert.rejects(collectWorkerArchive(Readable.from([bytes]), files), /path|regular file|file size/);
        receipts.push({ mode, rejected: true });
    }
    for (const mode of ["malformed", "control-spoof"]) {
        const files = output.files.map(file => ({ ...file }));
        const file = files.find(file => file.path === "result.json");
        const payload = JSON.parse(file.bytes);
        file.bytes = mode === "malformed" ? Buffer.from("{") : Buffer.from(JSON.stringify({ ...payload, verified: true }));
        file.size = file.bytes.length; file.sha256 = hash(file.bytes);
        assert.throws(() => readExecutionOutput(files, unit, output.receipt.launch));
        receipts.push({ mode, rejected: true });
    }
    return receipts;
}

async function acceptController({ root, config, configBytes, containmentDirectory, revision, frozen, unit,
    sourceDirectory, sourceBundle, sourceBundleSha256, key, signal }) {
    const modes = ["inspect", "disk", "pids", "cpu", "memory", "malformed", "timeout"];
    const reportBytes = await fs.readFile(path.join(containmentDirectory, "containment.json"));
    await fs.writeFile(path.join(containmentDirectory, "container.json"), configBytes, { flag: "wx" });
    await containment({ container: config, containment: { configSha256: hash(configBytes), reportSha256: hash(reportBytes),
        receiptSha256: Object.fromEntries(await Promise.all(modes.map(async mode =>
            [mode, hash(await fs.readFile(path.join(containmentDirectory, mode, "receipt.json")))]))) } },
    containmentDirectory, await inspectRuntime(config));
    const { h, approval, data, result: resultSlot } = await bootstrapFixture({
        now: new Date().toISOString(), deadline: new Date(Date.now() + 720 * 60000).toISOString(),
    });
    const context = Object.assign(h.context, {
        keys: { smoke: key }, keyId: "smoke", workflowPath: ".github/workflows/regression-bisect-controller.yml",
        workflowRevision: revision, policyRevision: revision, trustedWorkflowRevisions: [revision],
        checkpointAnchorSha: revision, pinnedMainSha: frozen.source.main, stableSdk: unit.stableSdk,
        manifest: structuredClone(unit.manifest), policy: require("../../.github/policies/regression-bisect.json"),
    });
    context.manifest.discussionFingerprint = context.issueFingerprint;
    seal(context.manifest);
    context.authorization = { repository: context.repository, issueNumber: 123,
        approvalPath: ".github/regression-bisect/approved/123.json", approvalRevision: revision,
        manifestDigest: fingerprint(context.manifest), inputDigest: context.manifest.approvedInputDigest };
    context.environmentFingerprint = fingerprint(Object.fromEntries(context.manifest.matrix.map(c =>
        [c.id, { os: c.os, architecture: c.architecture, imageDigest: config.image.split(":").at(-1) }])));
    const registration = { ...approval.bootstrap, allowanceId: `acceptance-${unit.buildInstanceId}`,
        checkpointAnchorSha: revision };
    h.comment.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Reserved checkpoint slot", payload: {
        schemaVersion: 1, purpose: "checkpoint-slot", repository: context.repository, issueNumber: 123,
        checkpointAnchorSha: revision, allowanceId: registration.allowanceId, deadline: registration.deadline,
    } }, context);
    resultSlot.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Reserved result slot", payload: {
        schemaVersion: 1, purpose: "result-slot", repository: context.repository, issueNumber: 123, checkpointCommentId: 456,
    } }, { ...context, commentId: 457 });
    context.bootstrapDigest = registration.checkpointBootstrapDigest = fingerprint(h.comment.body);
    registration.resultBootstrapDigest = fingerprint(resultSlot.body);
    const apiFile = path.join(root, "acceptance-api.json"), launches = path.join(root, "controller-launches");
    await fs.mkdir(launches);
    const run = { id: Number(context.owner.runId), run_attempt: 1, path: context.workflowPath,
        head_sha: revision, head_branch: "main", repository: { full_name: context.repository },
        head_repository: { full_name: context.repository }, status: "in_progress", conclusion: null };
    writeDatabase(apiFile, { ...data, comments: [h.comment, resultSlot], actor: context.actor,
        statuses: [], runs: { [`${context.owner.runId}/1`]: run }, faults: {}, nextId: 1000,
        files: {}, revision, anchorSha: revision, mirrorDirectory: frozen.source.directory });
    const service = await connect(apiFile);
    const storeConfig = { repository: context.repository, runId: context.owner.runId, attempt: 1, directory: path.join(root, "durable") };
    const store = createFileEvidenceStore(storeConfig);
    const transport = createGithubApi({ repository: context.repository, issueNumber: 123,
        checkpointCommentId: 456, resultCommentId: 457, anchorSha: revision, token: "offline-controller-token" }, service.send);
    Object.assign(transport, { readEvidence: store.readEvidence, verifyEvidence: store.verifyEvidence });
    const approvalReader = {
        readApproval: async () => Buffer.from(JSON.stringify({ schemaVersion: 1, manifest: context.manifest, bootstrap: registration })),
        readSource: source => fs.readFile(path.join(sourceDirectory, source.path)),
        authenticateRecord: async comment =>
            (await state.authenticateDecodedRecord(transport, { ...context, commentId: comment.id }, comment)) !== null,
        readTrustedCheckpoints: async () => [],
    };
    const deps = { context, transport, approval: approvalReader, platform: unit.platform, signal,
        environments: Object.fromEntries(context.manifest.matrix.map(c =>
            [c.id, { os: c.os, architecture: c.architecture, imageDigest: config.image.split(":").at(-1) }])),
        history: { gitExecutable: "/usr/bin/git", scratchDirectory: root, mirrorDirectory: frozen.source.directory, mainRef: "refs/heads/main" },
        clock: () => new Date(), withLock: action => withIssueLock(path.join(root, "controller-lock"), context, transport, action),
        execute: async (...args) => {
            service.recordGit(args[1]);
            return execute(...args);
        },
        toolchains: { resolveLatestStableSdk: async options => {
            const resolved = await resolveLatestStableSdk(options);
            requireValue(resolved.status === "resolved", resolved.error?.message);
            resolved.toolchain.runtimeRequirements = unit.resolved.runtimeRequirements;
            return resolved;
        } },
        collectObservation: createObservationCollector({ evidenceDirectory: launches, store }),
        runIsolated: async (selected, { signal }) => {
            const evidenceDirectory = path.join(launches, selected.buildInstanceId);
            await fs.mkdir(evidenceDirectory);
            console.log(JSON.stringify({ phase: selected.request.phase, buildInstanceId: selected.buildInstanceId, deadline: selected.deadline }));
            const output = await runContainer(config, { operation: selected.request.kind === "source" ? "acceptance" : "execution",
                input: selected, evidenceDirectory, sourceDirectory, sourceBundle, sourceBundleSha256, receiptKey: key, signal });
            requireValue(output.status === "collected", `controller physical transfer failed: ${output.reason}`);
            if (selected.request.kind === "source") await fs.writeFile(path.join(root, "collector-negatives.json"),
                JSON.stringify(await rejectOutputAttacks(output, selected), null, 2), { flag: "wx" });
            return output.result;
        } };
    const entry = { repository: context.repository, issueNumber: 123 };
    const bootstrap = await bootstrapController(entry, deps);
    requireValue(bootstrap.status === "initialized", `controller bootstrap failed: ${bootstrap.reason}`);
    const outcomes = [];
    for (let i = 0; i < 2; i++) {
        const result = await runController(entry, deps);
        outcomes.push(result);
        await fs.writeFile(path.join(root, "controller-results.json"), JSON.stringify(outcomes, null, 2));
        requireValue(result.status === "observed" && result.outcome.outcome === "product-failure",
            `controller endpoint incomplete: ${result.reason ?? JSON.stringify(result)}`);
    }
    const snapshot = await readIssueSnapshot({ ...approvalReader, transport, repository: context.repository,
        issueNumber: 123, now: new Date().toISOString(), reviewedManifest: context.manifest });
    requireValue(snapshot.status === "complete", "controller complete-discussion reread failed");
    const checkpoint = (await state.loadCheckpoint(transport, { ...context, now: new Date().toISOString() })).checkpoint;
    await fs.writeFile(path.join(root, "reader.json"), JSON.stringify({ context: { ...context, keys: undefined }, storeConfig,
        reservations: checkpoint.records, launches, apiFile }));
    await fs.rm(launches, { recursive: true });
    const reader = await execute(process.execPath, [__filename, "--read", path.join(root, "reader.json")], {
        cwd: path.resolve(__dirname, "..", ".."), env: { PATH: "/usr/bin:/bin", RB_SMOKE_KEY: key.toString("hex") },
        timeoutMs: 300000, maxBytes: 16384,
    });
    requireValue(reader.complete && reader.exitCode === 0, `durable new-process authentication failed: ${reader.stderr}`);
    const freshReader = JSON.parse(reader.stdout);
    requireValue(freshReader.matrix?.status === "passed", `contained project matrix incomplete: ${freshReader.matrix?.error}`);
    return { bootstrap, outcomes, freshReader, budget: checkpoint.state.budget.consumed,
        controllerAcceptance: true, projectMatrixAcceptance: true, immutableProducerAcceptance: true };
}

async function readRetained(file) {
    const { context, storeConfig, reservations, launches, apiFile } = JSON.parse(await fs.readFile(file, "utf8"));
    context.keys = { smoke: Buffer.from(process.env.RB_SMOKE_KEY, "hex") };
    const store = createFileEvidenceStore(storeConfig, { readOnly: true });
    const collect = createObservationCollector({ evidenceDirectory: launches, store });
    const observations = [];
    let matrix;
    for (const reservation of reservations) {
        const collected = await collect({ reservation, manifest: context.manifest, context });
        observations.push({ outcome: collected.observation.outcome, evidence: collected.evidence });
        const envelope = JSON.parse(await store.readEvidence(collected.evidence[0].url));
        const payload = JSON.parse(Buffer.from(envelope.files.find(f => f.path === "result.json").bytesBase64, "base64"));
        if (payload.result.acceptance) {
            matrix = payload.result.acceptance;
            for (const file of matrix.retained ?? [])
                requireValue(hash(Buffer.from(file.bytesBase64, "base64")) === file.sha256, "retained producer bytes changed");
        }
    }
    const ledger = (await fs.readFile(`${apiFile}.ledger`, "utf8")).trim().split("\n").map(JSON.parse);
    requireValue(!ledger.some(e => e.kind === "git" && e.args.includes("bisect")), "acceptance must not start a trial bisect");
    console.log(JSON.stringify({ observations, matrix: matrix && { status: matrix.status, error: matrix.error,
        cases: matrix.results.length, producer: matrix.producer?.identity, retained: matrix.retained?.map(({ bytesBase64, ...f }) => f) },
        apiSha256: hash(await fs.readFile(apiFile)), launchScratchDeleted: true }));
}

if (require.main === module) {
    requireValue(process.argv[2] === "--read" && process.argv.length === 4, "fixed retained-reader command required");
    readRetained(process.argv[3]).catch(error => { console.error(error.message); process.exitCode = 2; });
}
module.exports = { acceptController };
