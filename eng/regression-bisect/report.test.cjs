const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const { createFileEvidenceStore } = require("./evidence-store.cjs");
const f = require("./fixtures/contracts.cjs");
const { harness } = require("./fixtures/checkpoint.cjs");
const model = require("./model.cjs");
const state = require("./state.cjs");
const intake = require("./intake.cjs");
const { preparePublication, renderReport, completePublication } = require("./report.cjs");
const { publishController } = require("./controller.cjs");

function resultComment(h, ready, id = 789) {
    const body = state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
        payload: { schemaVersion: 1, digest: ready.digest, report: ready.report } }, { ...h.context, commentId: id });
    return { ...structuredClone(h.comment), id, body };
}

async function authenticatedDisplay(h, ready) {
    const comment = resultComment(h, ready);
    const decoded = await state.authenticateDecodedRecord(h.transport, { ...h.context, commentId: comment.id }, comment);
    assert.ok(decoded, "result must authenticate before any rendered-output assertion");
    assert.deepEqual(decoded.payload.report, ready.report);
    return comment.body.slice(0, comment.body.indexOf("\n\nRegression investigation result\n\n```json\n"));
}

async function fixture({ outcome = "product-failure", evidence = [], producerName = "repro", matrixCount = 1,
    archiveKind = "publisher-checksum", evidenceStore } = {}) {
    const h = harness(), calls = [], comments = [];
    if (evidenceStore) Object.assign(h.transport, {
        readEvidence: evidenceStore.readEvidence, verifyEvidence: evidenceStore.verifyEvidence,
    });
    const issue = { ...h.context.issue, id: 900, title: "Regression", body: "Reviewed reproduction",
        user: { id: 20 }, updated_at: h.context.now };
    const statusRequest = h.transport.request;
    h.transport.request = async (route, args) => {
        calls.push({ route, args });
        if (route.includes("/statuses")) return statusRequest(route, args);
        if (route.endsWith("/issues/{issue_number}")) return { data: structuredClone(issue) };
        if (route.endsWith("/comments")) return {
            data: structuredClone(args.page === 1 ? comments : h.comment.body.startsWith("unused") ? [] : [h.comment]),
            headers: { link: args.page === 1 ? '<https://api.github.com/next>; rel="next"' : "" },
        };
        if (route.endsWith("/timeline")) return { data: [], headers: { link: "" } };
        if (route.endsWith("/pulls/{pull_number}") && [1, 2].includes(args.pull_number)) return {
            data: { number: args.pull_number, state: "closed", merged_at: h.context.now, updated_at: h.context.now },
        };
        if (route.includes("/actions/runs/")) return { data: await h.transport.getRun(args.run_id, args.attempt_number) };
        throw new Error(`unexpected route ${route}`);
    };
    const initial = await intake.readIssueSnapshot({ repository: h.context.repository, issueNumber: 123, transport: h.transport });
    assert.equal(initial.status, "complete", initial.reason);
    h.context.issueFingerprint = initial.snapshot.discussionFingerprint;
    h.context.manifest.discussionFingerprint = h.context.issueFingerprint;
    h.context.manifest.producer.name = producerName;
    if (archiveKind === "approved-official-https-measured") {
        h.context.manifest.matrix[0].id = "windows-x64";
        h.context.manifest.matrix[0].os = "windows";
        h.context.pinnedMainSha = f.policy().toolchains.measuredArchives[0].sourceSha;
    }
    if (matrixCount === 2) h.context.manifest.matrix.push({
        ...h.context.manifest.matrix[0], id: "windows-x64", os: "windows",
    });
    f.seal(h.context.manifest);
    h.context.stableSdk = f.observation("released", "product-failure", h.context.manifest).target.sdk;
    h.context.authorization.manifestDigest = model.fingerprintInput(h.context.manifest);
    h.context.authorization.inputDigest = h.context.manifest.approvedInputDigest;
    const environments = Object.fromEntries(h.context.manifest.matrix.map(c =>
        [c.id, { os: c.os, architecture: c.architecture, imageDigest: f.digest("runner image") }]));
    h.context.environmentFingerprint = model.fingerprintInput(environments);
    await h.start();
    const pending = await state.reserveWork(h.transport, h.context, { generation: 0, attemptId: "report-main",
        request: { kind: "source", target: { sourceSha: h.context.pinnedMainSha }, endpoint: "main", phase: "main" } });
    const observation = f.observation("source", outcome, h.context.manifest, h.context.pinnedMainSha);
    observation.buildInstanceId = pending.id;
    for (const cell of observation.cells) {
        for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = pending.id;
        cell.buildToolchain = f.buildToolchain(observation.target.sourceSha, cell.environment.os, archiveKind);
    }
    await state.commitObservation(h.transport, h.context, {
        generation: pending.generation, attemptId: pending.id, observation, evidence,
    });
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    cp.state.phase = "blocked";
    h.checkpoint(cp);
    await state.loadCheckpoint(h.transport, h.context);
    const fresh = { manifest: structuredClone(h.context.manifest), authorization: structuredClone(h.context.authorization),
        policy: f.policy(), policyRevision: h.context.policyRevision, workflowRevision: h.context.workflowRevision,
        pinnedMainSha: h.context.pinnedMainSha, stableSdk: structuredClone(h.context.stableSdk),
        environmentFingerprint: h.context.environmentFingerprint, readSource: async () => Buffer.from('printfn "ok"') };
    let reads = 0, lockDepth = 0;
    const deps = { context: h.context, transport: h.transport,
        withLock: fn => h.lock(async () => { lockDepth++; try { return await fn(); } finally { lockDepth--; } }),
        readCurrentContext: async () => { assert.equal(lockDepth, 1); reads++; return fresh; } };
    const generation = (await state.loadCheckpoint(h.transport, h.context)).checkpoint.generation;
    return Object.assign(h, { calls, comments, issue, fresh, deps, generation, getReads: () => reads,
        prepare: options => preparePublication({ now: h.context.now, generation, ...options }, deps) });
}

test("controlled durable artifacts publish with streaming archive verification and bounded signed evidence", async t => {
    const directory = await fs.realpath(await fs.mkdtemp(path.join(os.tmpdir(), "report-evidence-")));
    t.after(() => fs.rm(directory, { recursive: true, force: true }));
    const store = createFileEvidenceStore({ directory, repository: "dotnet/fsharp", runId: "12345", attempt: 1 });
    const binding = { workId: "report-main", launchId: f.digest("launch"),
        unitDigest: f.digest("unit"), imageDigest: f.digest("runner image") };
    const small = await store.writeEvidence(Buffer.from("durable evidence"), binding);
    const build = f.buildToolchain(f.sha("main"), "linux");
    const blob = await store.writeBlob(Buffer.from("archive"), { binding, request: {
        policy: f.policy(), kind: "source", sourceSha: build.sourceSha, product: "sdk", version: build.sdk.version,
        platform: { os: "linux", architecture: "x64", rid: "linux-x64" }, archive: build.archive,
    }, receipt: build.archive });
    const h = await fixture({ evidence: [small, blob], evidenceStore: store });
    h.transport.readEvidence = () => assert.fail("publication must not buffer immutable archive blobs");
    const result = await h.prepare();
    assert.equal(result.status, "ready", result.reason);
    assert.ok(result.report.observations[0].evidence.every(e => e.available));
    assert.ok(Buffer.byteLength(result.body) < 32768);
    assert.ok(Buffer.byteLength(resultComment(h, result).body) < 48 * 1024);
    const name = new URL(blob.url).searchParams.get("name");
    await fs.rm(path.join(directory, `${name}.bin`));
    const missing = await h.prepare();
    assert.equal(missing.status, "blocked");
    assert.match(missing.reason, /required evidence/);
    t.diagnostic(JSON.stringify({ realLaunches: 0, displayBytes: Buffer.byteLength(result.body),
        signedBytes: Buffer.byteLength(resultComment(h, result).body), evidence: [small, blob] }));
});

test("fresh complete intake produces a bounded deterministic pending intent, never a report write", async () => {
    const h = await fixture();
    const result = await h.prepare();
    assert.equal(result.status, "ready", JSON.stringify(result));
    assert.equal(h.getReads(), 1);
    assert.ok(h.calls.some(c => c.route.endsWith("/comments") && c.args.page === 2));
    assert.ok(h.calls.some(c => c.route.endsWith("/timeline")));
    assert.ok(h.calls.some(c => c.route.includes("/actions/runs/")));
    assert.equal(result.repository, "dotnet/fsharp");
    assert.equal(result.issueNumber, 123);
    assert.equal(result.body, renderReport(result.report));
    assert.equal(result.digest, model.fingerprintInput(result.report));
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(cp.publicationStatus, "pending");
    assert.deepEqual(cp.state.publication, { digest: result.digest, commentId: null });
    assert.equal(cp.state.budget.consumed.workerUnits, 1);
    assert.ok(h.calls.every(c => !c.route.startsWith("POST ") || c.route.includes("/statuses/")));
    const duplicate = await h.prepare();
    assert.equal(duplicate.status, "pending");
    assert.equal(duplicate.digest, result.digest);
    assert.equal(duplicate.generation, result.generation);
});

test("guarded publisher writes only its authenticated issue-specific slot and recovers across owning runs", async t => {
    for (const crash of ["none", "before-write", "after-write"]) await t.test(crash, async () => {
        const h = await fixture();
        const slotId = 789;
        h.comments.push({ ...structuredClone(h.comment), id: slotId,
            body: state.serializeWorkflowRecord({ kind: "proposal", status: "Regression investigation result slot",
                payload: { schemaVersion: 1, purpose: "result-slot", repository: h.context.repository,
                    issueNumber: h.context.issueNumber, checkpointCommentId: h.context.commentId } },
            { ...h.context, commentId: slotId }) });
        h.deps.resultCommentId = slotId;
        h.deps.clock = () => new Date(h.context.now);
        const get = h.transport.getComment, update = h.transport.updateComment;
        const writes = [];
        h.transport.getComment = async id => id === slotId ? structuredClone(h.comments[0]) : get(id);
        let interrupted = false;
        h.transport.updateComment = async (id, body) => {
            if (id !== slotId) return update(id, body);
            if (!interrupted && crash === "before-write") {
                interrupted = true;
                throw new Error("cancelled before report write");
            }
            writes.push({ id, body });
            h.comments[0].body = body;
            if (!interrupted && crash === "after-write") {
                interrupted = true;
                throw new Error("report response lost");
            }
            return structuredClone(h.comments[0]);
        };
        const input = { repository: "dotnet/fsharp", issueNumber: 123 };
        const first = await publishController(input, h.deps);
        assert.equal(first.status, crash === "none" ? "published" : "blocked", first.reason);
        h.nextRun();
        const recovered = await publishController(input, h.deps);
        assert.equal(recovered.status, "published", recovered.reason);
        assert.equal(recovered.commentId, slotId);
        assert.equal(writes.length, 1);
        assert.equal((await publishController(input, h.deps)).status, "published");
        assert.equal(writes.length, 1);
        const decoded = await state.authenticateDecodedRecord(h.transport, { ...h.context, commentId: slotId }, h.comments[0]);
        assert.equal(decoded.kind, "result");
        assert.equal(decoded.payload.digest, recovered.digest);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.state.publication.commentId, slotId);
        assert.equal(cp.state.budget.consumed.reservedMinutes, 110);
    });
});

test("publisher rechecks current inputs immediately before write and rejects forged slots", async t => {
    for (const mutation of ["discussion", "main", "slot", "destination", "markdown", "slot-owner"]) await t.test(mutation, async () => {
        const h = await fixture();
        h.deps.resultCommentId = 789;
        h.deps.clock = () => new Date(h.context.now);
        const payload = { schemaVersion: 1, purpose: "result-slot", repository: h.context.repository,
            issueNumber: h.context.issueNumber, checkpointCommentId: mutation === "slot" ? 999 : h.context.commentId };
        const slot = { ...structuredClone(h.comment), id: 789,
            body: state.serializeWorkflowRecord({ kind: "proposal", status: "Regression investigation result slot",
                payload }, { ...h.context, commentId: 789 }) };
        h.comments.push(slot);
        const get = h.transport.getComment, update = h.transport.updateComment;
        let writes = 0;
        h.transport.getComment = async id => {
            if (id !== 789) return get(id);
            if (mutation === "discussion") h.comments.push({ id: 888, body: "new correction", user: { id: 21 } });
            if (mutation === "main") h.fresh.pinnedMainSha = f.sha("moved main after preparation");
            const value = structuredClone(slot);
            if (mutation === "slot-owner") value.user.id++;
            return value;
        };
        h.transport.updateComment = async (id, body) => {
            if (id === 789) { writes++; throw new Error("forbidden report write"); }
            return update(id, body);
        };
        const input = { repository: "dotnet/fsharp", issueNumber: 123,
            ...(mutation === "destination" ? { issueNumber: 999 } : {}),
            ...(mutation === "markdown" ? { markdown: "# fabricated" } : {}) };
        const result = await publishController(input, h.deps);
        assert.ok(["blocked", "requeue"].includes(result.status), JSON.stringify(result));
        assert.equal(writes, 0);
        assert.equal((await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed.reservedMinutes, 110);
    });
});

test("pending publication ownership transfers only from a completed authenticated run", async t => {
    for (const written of [false, true]) for (const previous of ["in_progress", "completed", "unknown"])
        await t.test(`${previous}, written=${written}`, async () => {
        const h = await fixture();
        const pending = await h.prepare();
        assert.equal(pending.status, "ready", pending.reason);
        if (written) h.comments.push(resultComment(h, pending));
        h.nextRun();
        if (previous === "unknown") h.runs.delete("12345/1");
        else {
            h.runs.get("12345/1").status = previous;
            h.runs.get("12345/1").conclusion = previous === "completed" ? "cancelled" : null;
        }
        const before = h.comment.body;
        const result = await h.prepare({ generation: pending.generation });
        assert.equal(result.status, previous === "completed" ? written ? "published" : "pending" : "blocked", result.reason);
        if (previous === "completed") {
            assert.equal(result.digest, pending.digest);
            assert.equal(result.generation, pending.generation + 1);
            const loaded = await state.loadCheckpoint(h.transport, h.context);
            assert.deepEqual(loaded.owner, h.context.owner);
            assert.equal(loaded.checkpoint.state.budget.consumed.reservedMinutes, 110);
        } else if (previous === "unknown") assert.equal(h.comment.body, before);
    });
});

test("reports expose assertions, matrix, identities, provenance, uncertainty and charged resume allowance", async () => {
    const h = await fixture();
    const result = await h.prepare();
    assert.equal(result.status, "ready", result.reason);
    for (const text of ["Expected assertions", "Approved commands", "net10", "Release", "language",
        "producer", "SDK", "runtime", "compiler", "tasks", "targets", "core", "global", "requested",
        "resolved", "publisher", "report-main", "Endpoint", "Parent", "interval", "unconfirmed",
        "latent root cause", "PR", "Budget", "110", "Resume", "archive receipt", "not recorded"]) {
        assert.ok(result.body.toLowerCase().includes(text.toLowerCase()), text);
    }
    assert.ok(result.body.includes(h.context.pinnedMainSha));
    assert.ok(result.body.includes(h.context.manifest.approvedInputDigest));
    assert.ok(result.body.includes(f.digest("producer")));
});

test("measured HTTPS archives never become claimed publisher checksums", async () => {
    const h = await fixture({ archiveKind: "approved-official-https-measured" });
    const result = await h.prepare();
    assert.equal(result.status, "ready", result.reason);
    assert.match(result.body, /official-HTTPS measured archive hash, NOT a publisher-provided checksum/);
    assert.ok(!result.body.includes("- Publisher-provided SHA-512"));
});

test("ineligible or stale fresh context blocks publication without refund", async t => {
    const changes = {
        closed: h => { h.issue.state = "closed"; },
        unlabelled: h => { h.issue.labels = []; },
        old: h => { h.issue.created_at = "2026-08-01T00:00:00Z"; },
        discussion: h => { h.comments.push({ id: 800, body: "correction", user: { id: 21 } }); },
        title: h => { h.issue.title = "corrected"; },
        reproduction: h => { h.fresh.readSource = async () => Buffer.from("changed"); },
        manifest: h => { h.fresh.manifest.timeoutSeconds--; f.seal(h.fresh.manifest); },
        approval: h => { h.fresh.authorization.approvalRevision = f.sha("new-approval"); },
        policy: h => { h.fresh.policyRevision = f.sha("new-policy"); },
        workflow: h => { h.fresh.workflowRevision = f.sha("new-workflow"); },
        main: h => { h.fresh.pinnedMainSha = f.sha("new-main"); },
        environment: h => { h.fresh.environmentFingerprint = f.digest("changed image"); },
        missingEnvironment: h => { delete h.fresh.environmentFingerprint; },
    };
    for (const [name, change] of Object.entries(changes)) for (const written of [false, true]) await t.test(`${name}, written=${written}`, async () => {
        const h = await fixture();
        if (written) h.comments.push(resultComment(h, await h.prepare()));
        change(h);
        const result = await h.prepare();
        assert.ok(["blocked", "requeue"].includes(result.status), JSON.stringify(result));
        assert.equal(result.body, undefined);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.publicationStatus, "blocked");
        assert.equal(cp.state.budget.consumed.workerUnits, 1);
        assert.equal(cp.state.budget.consumed.reservedMinutes, 110);
    });
});

test("fresh reader, lock, bounded intent, and exact generation are mandatory", async t => {
    for (const change of [
        h => { delete h.deps.readCurrentContext; },
        h => { delete h.deps.withLock; },
        h => { h.deps.readCurrentContext = async () => { throw new Error("approval unavailable"); }; },
    ]) await t.test("trusted seam missing", async () => {
        const h = await fixture(); change(h);
        assert.equal((await h.prepare()).status, "blocked");
    });
    const h = await fixture();
    for (const args of [{ generation: h.generation - 1 }, { markdown: "# forged" },
        { issueNumber: 912 }, { repository: "attacker/repo" }]) {
        assert.equal((await h.prepare(args)).status, "blocked");
    }
});

test("incomplete intake blocks rather than publishing a partial discussion", async () => {
    const h = await fixture(), request = h.transport.request;
    h.transport.request = async (route, args) => route.endsWith("/timeline")
        ? { data: [], headers: { link: '<https://api.github.com/next>; rel="next"' } } : request(route, args);
    const result = await h.prepare();
    assert.equal(result.status, "blocked");
    assert.match(result.reason, /incomplete|page limit/);
});

test("signed lost-response results recover only at exact writer/app/comment/run and body", async t => {
    for (const mutation of ["none", "id", "writer", "app", "run", "owner", "repository", "issue", "signature",
        "duplicate", "conflicting-digest", "missing-intent", "markdown", "digest"]) await t.test(mutation, async () => {
        const h = await fixture();
        const ready = await h.prepare();
        assert.equal(ready.status, "ready", ready.reason);
        const context = { ...h.context, commentId: 789 };
        if (mutation === "owner") {
            h.runs.set("12346/1", { ...h.runs.get("12345/1"), id: 12346 });
            context.owner = { runId: "12346", attempt: 1 };
        }
        const resultPayload = { schemaVersion: 1, digest: ready.digest, report: ready.report };
        if (mutation === "digest") resultPayload.digest = f.digest("another");
        let body;
        try { body = state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result", payload: resultPayload }, context); }
        catch (error) {
            assert.equal(mutation, "digest");
            assert.match(error.message, /digest/);
            return;
        }
        const comment = { ...structuredClone(h.comment), id: 789, body };
        if (mutation === "id") comment.id++;
        if (mutation === "writer") comment.user.id++;
        if (mutation === "app") comment.performed_via_github_app.id++;
        if (mutation === "run") h.runs.get("12345/1").head_sha = f.sha("forged");
        if (mutation === "repository") comment.issue_url = "https://api.github.com/repos/foreign/repo/issues/123";
        if (mutation === "issue") comment.issue_url = "https://api.github.com/repos/dotnet/fsharp/issues/124";
        if (mutation === "signature") comment.body = comment.body.replace(/"signature":"[a-f0-9]{64}"/,
            `"signature":"${"0".repeat(64)}"`);
        if (mutation === "markdown") comment.body = "# invented blame\n\n" + comment.body;
        h.comments.push(comment);
        if (mutation === "duplicate") h.comments.push(resultComment(h, ready, 790));
        if (mutation === "conflicting-digest") {
            const report = { ...ready.report, deadline: "2026-10-02T00:00:00Z" };
            h.comments.push(resultComment(h, { report, digest: model.fingerprintInput(report) }, 790));
        }
        let generation = h.generation;
        if (mutation === "missing-intent") {
            const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
            cp.state.publication = { digest: null, commentId: null };
            cp.publicationStatus = "unpublished";
            h.checkpoint(cp);
            generation = cp.generation;
        }
        const recovered = await h.prepare({ generation });
        if (mutation === "none") {
            assert.equal(recovered.status, "published", JSON.stringify(recovered));
            assert.equal(recovered.commentId, 789);
            const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
            assert.equal(cp.publicationStatus, "published");
            const retry = await completePublication({ now: h.context.now, generation: ready.generation,
                digest: ready.digest, commentId: 789 }, h.deps);
            assert.equal(retry.status, "published", JSON.stringify(retry));
        } else {
            assert.ok(["blocked", "requeue"].includes(recovered.status), JSON.stringify(recovered));
            assert.notEqual(recovered.status, "published");
        }
    });
});

test("completion refreshes inputs again immediately before acknowledging a result", async () => {
    const h = await fixture(), ready = await h.prepare();
    const body = state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
        payload: { schemaVersion: 1, digest: ready.digest, report: ready.report } }, { ...h.context, commentId: 789 });
    h.comments.push({ ...structuredClone(h.comment), id: 789, body });
    h.issue.body += " corrected since preparation";
    const result = await completePublication({ now: h.context.now, generation: ready.generation,
        digest: ready.digest, commentId: 789 }, h.deps);
    assert.ok(["blocked", "requeue"].includes(result.status));
    assert.equal(h.getReads(), 2);
});

test("unsafe evidence origins and paths are rejected before any evidence read", async t => {
    for (const url of ["https://attacker.invalid/log", "https://api.github.com/repos/other/repo/actions/artifacts/1/zip",
        "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/../1/zip",
        "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/%31/zip",
        "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/1/zip?token=secret"]) {
        await t.test(url, async () => {
            const h = await fixture({ evidence: [{ url, sha256: f.digest("durable evidence"), required: true }] });
            let reads = 0;
            h.transport.readEvidence = async () => { reads++; return Buffer.from("durable evidence"); };
            assert.equal((await h.prepare()).status, "blocked");
            assert.equal(reads, 0);
        });
    }
    const h = await fixture({ evidence: [{ url: "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/12/zip",
        sha256: f.digest("durable evidence"), required: true }] });
    assert.equal((await h.prepare()).status, "ready");
});

test("renderer rejects arbitrary fields and escapes all untrusted display text", async () => {
    const h = await fixture({ producerName: "<script>@everyone [click](https://evil.invalid) ```" }), result = await h.prepare();
    assert.equal(result.status, "ready", result.reason);
    assert.throws(() => renderReport({ ...result.report, markdown: "# injected" }), /schema|field/);
    assert.throws(() => renderReport({ ...result.report, issueNumber: 912 }), /schema|field/);
    const report = structuredClone(result.report);
    report.state.manifest.producer.name = "different producer";
    f.seal(report.state.manifest);
    assert.throws(() => renderReport(report), /mismatch|identity|manifest|approval/);
    assert.ok(!result.body.includes("<script>"));
    assert.ok(!result.body.includes("@everyone"));
    assert.ok(!result.body.includes("[click](https://evil.invalid)"));
    assert.ok(result.body.includes("&lt;script&gt;"));
    assert.ok(result.body.includes("&#64;everyone"));
    assert.ok(Buffer.byteLength(result.body) < 48 * 1024);
});

test("lost pending-write acknowledgement is recovered without another intent or reservation", async () => {
    const h = await fixture(), update = h.transport.updateComment;
    h.transport.updateComment = async (...args) => { await update(...args); throw new Error("response lost"); };
    assert.equal((await h.prepare()).status, "blocked");
    h.transport.updateComment = update;
    const retry = await h.prepare();
    assert.equal(retry.status, "pending", JSON.stringify(retry));
    assert.equal((await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed.workerUnits, 1);
});

test("missing evidence is disclosed when optional and blocks when required", async t => {
    for (const required of [true, false]) await t.test(`${required}`, async () => {
        const h = await fixture({ evidence: [{ url: "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/12/zip",
            sha256: f.digest("durable evidence"), required }] });
        h.transport.readEvidence = async () => null;
        const result = await h.prepare();
        assert.equal(result.status, required ? "blocked" : "ready", result.reason);
        if (!required) assert.match(result.body, /unavailable or expired/);
    });
});

test("publication refuses a concurrent authenticated checkpoint generation change", async () => {
    const h = await fixture(), request = h.transport.request;
    let advanced = false;
    h.transport.request = async (route, args) => {
        if (route.endsWith("/timeline") && !advanced) {
            advanced = true;
            h.checkpoint((await state.loadCheckpoint(h.transport, h.context)).checkpoint);
        }
        return request(route, args);
    };
    assert.equal((await h.prepare()).status, "blocked");
});

for (const [matrixCount, products] of [[1, "shared"], [2, "shared"], [1, "unequal"], [2, "oversized"], [2, "envelope"]]) test(`six affordable observations and ${matrixCount} matrix cells with distinct receipts: ${products}`, async t => {
    const h = await fixture({ matrixCount, producerName: "\u03bb".repeat(80),
        evidence: products === "envelope" ? [12, 13, 14, 15].map(id => ({
            url: `https://api.github.com/repos/dotnet/fsharp/actions/artifacts/${id}/zip`,
            sha256: f.digest("durable evidence"), required: false,
        })) : [],
    });
    for (let i = 1; i < 6; i++) {
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        const pending = await state.reserveWork(h.transport, h.context, { generation: cp.generation,
            attemptId: `search-${i}`, request: { kind: "source", target: { sourceSha: f.sha(`search-${i}`) },
                endpoint: null, phase: "bisect" } });
        const observation = f.observation("source", "pass", h.context.manifest, pending.request.target.sourceSha);
        observation.buildInstanceId = pending.id;
        for (const cell of observation.cells) {
            for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = pending.id;
            cell.buildToolchain = f.buildToolchain(observation.target.sourceSha, cell.environment.os);
            cell.buildToolchain.evidenceDigest = f.digest(`receipt-${i}-${cell.id}`);
            cell.buildToolchain.archive.receivedAt = `2026-09-19T17:30:0${i}.000Z`;
            cell.buildToolchain.archive.retained.path += `-${i}`;
            if (products === "oversized" || products === "unequal" && i === 5) {
                cell.buildToolchain.requestedCompilerRuntime = `11.0.${i}`;
                cell.buildToolchain.compilerRuntime.version = `11.0.${i}`;
            }
        }
        await state.commitObservation(h.transport, h.context, {
            generation: pending.generation, attemptId: pending.id, observation, evidence: [],
        });
    }
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    cp.state.phase = "blocked";
    h.checkpoint(cp);
    const generation = (await state.loadCheckpoint(h.transport, h.context)).checkpoint.generation;
    const result = await h.prepare({ generation });
    if (products === "oversized" || products === "envelope") {
        assert.equal(result.status, "blocked", result.reason);
        assert.match(result.reason, /observations and saved interval retained in authenticated checkpoint 456/);
        assert.match(result.reason, /48 KiB/);
        const retained = await state.loadCheckpoint(h.transport, h.context);
        assert.equal(retained.checkpoint.publicationStatus, "blocked");
        assert.equal(Object.keys(retained.checkpoint.observations).length, 6);
        assert.deepEqual(retained.checkpoint.state.budget, cp.state.budget);
        for (let i = 1; i < 6; i++) {
            const o = Object.values(retained.checkpoint.observations).find(o => o.data.buildInstanceId === `search-${i}`);
            assert.ok(o.data.cells.every(c => c.buildToolchain.compilerRuntime.version ===
                (products === "oversized" ? `11.0.${i}` : "11.0.0")));
        }
        t.diagnostic(`${products}: ${result.reason}`);
        return;
    }
    assert.equal(result.status, "ready", result.reason);
    assert.equal(result.report.observations.length, 6);
    assert.ok(Buffer.byteLength(result.body, "utf8") < 32768);
    if (products === "unequal") {
        assert.match(result.body, /resolved SDK\/runtime .*\/11\.0\.0/);
        assert.match(result.body, /resolved SDK\/runtime .*\/11\.0\.5/);
        assert.equal((result.body.match(/Revision toolchain T\d+ \(/g) ?? []).length, 2);
    }
    for (let i = 1; i < 6; i++) assert.ok(result.body.includes(`search-${i}`));
    const signed = state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
        payload: { schemaVersion: 1, digest: result.digest, report: result.report } }, { ...h.context, commentId: 789 });
    assert.ok(Buffer.byteLength(signed) < 48 * 1024);
    assert.ok(Buffer.byteLength(result.body, "utf8") > result.body.length);
    t.diagnostic(`displayBytes=${Buffer.byteLength(result.body, "utf8")} signedBytes=${Buffer.byteLength(signed, "utf8")}`);
    const decoded = await state.authenticateDecodedRecord(h.transport, { ...h.context, commentId: 789 },
        { ...structuredClone(h.comment), id: 789, body: signed });
    assert.deepEqual(decoded.payload.report, result.report);
    for (const record of decoded.payload.report.observations) {
        assert.ok(signed.includes(record.digest));
        for (const cell of record.data.cells) assert.ok(cell.buildToolchain.archive.retained.path);
    }
    for (let i = 1; i < 6; i++) {
        const observation = decoded.payload.report.observations.find(o => o.data.buildInstanceId === `search-${i}`).data;
        for (const cell of observation.cells) {
            assert.equal(cell.buildToolchain.evidenceDigest, f.digest(`receipt-${i}-${cell.id}`));
            assert.ok(cell.buildToolchain.archive.retained.path.endsWith(`-${i}`));
            assert.equal(cell.buildToolchain.archive.receivedAt, `2026-09-19T17:30:0${i}.000Z`);
        }
    }
});

test("lost result acknowledgement recovers the pending digest after optional evidence changes", async t => {
    for (const availability of ["expired", "hash-mismatch"]) for (const required of [false, true]) await t.test(`${availability}, required=${required}`, async () => {
        const h = await fixture({ evidence: [{
            url: "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/12/zip",
            sha256: f.digest("durable evidence"), required,
        }] });
        const ready = await h.prepare();
        assert.equal(ready.status, "ready", ready.reason);
        const body = state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
            payload: { schemaVersion: 1, digest: ready.digest, report: ready.report } }, { ...h.context, commentId: 789 });
        h.comments.push({ ...structuredClone(h.comment), id: 789, body });
        const before = (await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget;
        h.transport.readEvidence = async () => availability === "expired" ? null : Buffer.from("changed bytes");
        const recovered = await h.prepare();
        assert.equal(recovered.status, required ? "blocked" : "published", recovered.reason);
        if (!required) {
            assert.equal(recovered.digest, ready.digest);
            assert.equal(recovered.commentId, 789);
        } else assert.match(recovered.reason, /required evidence/);
        assert.equal(h.comments.length, 1);
        assert.equal(h.comments[0].body, body);
        assert.deepEqual((await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget, before);
        assert.ok(h.calls.every(c => !c.route.startsWith("POST ") || c.route.includes("/statuses/")));
        t.diagnostic(JSON.stringify({ availability, required, status: recovered.status, commentId: recovered.commentId,
            digest: recovered.digest, resultWrites: h.comments.length, consumed: before.consumed }));
    });
});

test("pending result recovery checks required evidence without rereading optional availability", async t => {
    for (const written of [false, true]) await t.test(`written=${written}`, async () => {
        const h = await fixture({ evidence: [
            { url: "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/12/zip",
                sha256: f.digest("durable evidence"), required: false },
            { url: "https://api.github.com/repos/dotnet/fsharp/actions/artifacts/13/zip",
                sha256: f.digest("durable evidence"), required: true },
        ] });
        const ready = await h.prepare();
        assert.equal(ready.status, "ready", ready.reason);
        if (written) h.comments.push(resultComment(h, ready));
        const reads = [];
        h.transport.readEvidence = async url => {
            reads.push(url);
            if (url.includes("/12/")) throw new Error("optional artifact transport unavailable");
            return Buffer.from("durable evidence");
        };
        const recovered = await h.prepare();
        assert.equal(recovered.status, written ? "published" : "blocked", recovered.reason);
        assert.ok(reads.some(url => url.includes("/13/")), "required evidence must be checked before adoption");
        assert.equal(reads.some(url => url.includes("/12/")), !written);
        if (written) {
            assert.equal(recovered.digest, ready.digest);
            assert.equal(recovered.commentId, 789);
            const acknowledged = await completePublication({ now: h.context.now, generation: ready.generation,
                digest: ready.digest, commentId: 789 }, h.deps);
            assert.equal(acknowledged.status, "published", acknowledged.reason);
            assert.equal(acknowledged.generation, recovered.generation);
            assert.equal(reads.some(url => url.includes("/12/")), false);
        } else assert.match(recovered.reason, /optional artifact transport unavailable/);
    });
});

test("publication crash points preserve a single result and the charged allowance", async t => {
    for (const crash of ["before-intent", "after-intent", "after-result", "after-acknowledgement"]) await t.test(crash, async () => {
        const h = await fixture(), update = h.transport.updateComment;
        const before = (await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget;
        if (crash.endsWith("intent")) {
            h.transport.updateComment = async (...args) => {
                if (crash === "after-intent") await update(...args);
                throw new Error("interrupted intent response");
            };
            assert.equal((await h.prepare()).status, "blocked");
            h.transport.updateComment = update;
        }
        const ready = await h.prepare();
        assert.equal(ready.status, crash === "after-intent" ? "pending" : "ready", ready.reason);
        if (crash === "after-result") {
            await assert.rejects(async () => {
                h.comments.push(resultComment(h, ready));
                throw new Error("result write response lost");
            }, /response lost/);
        } else h.comments.push(resultComment(h, ready));
        if (crash === "after-acknowledgement") {
            h.transport.updateComment = async (...args) => { await update(...args); throw new Error("ack response lost"); };
            assert.equal((await h.prepare()).status, "blocked");
            h.transport.updateComment = update;
        }
        const recovered = await h.prepare();
        assert.equal(recovered.status, "published", recovered.reason);
        assert.equal(recovered.digest, ready.digest);
        assert.equal(recovered.commentId, 789);
        assert.equal(h.comments.length, 1);
        assert.deepEqual((await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget, before);
        assert.ok(h.calls.every(c => !c.route.startsWith("POST ") || c.route.includes("/statuses/")));
    });
});

async function confirmedFixture({ omitReleased = [], classification = "verified-introducing-source-boundary",
    associationCount = 0, intervalCount = 1, incomplete = null } = {}) {
    const h = await fixture();
    if (!omitReleased.includes("endpoint")) {
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        const pending = await state.reserveWork(h.transport, h.context, { generation: cp.generation,
            attemptId: "released-stable", request: { kind: "released", target: { sdk: h.context.stableSdk },
                endpoint: "stable", phase: "stable" } });
        const observation = f.observation("released", "product-failure", h.context.manifest);
        observation.buildInstanceId = pending.id;
        for (const cell of observation.cells) {
            for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = pending.id;
            const rid = `${{ linux: "linux", windows: "win", macos: "osx" }[cell.environment.os]}-${cell.environment.architecture}`;
            const name = `dotnet-sdk-${cell.sdk.version}-${rid}.tar.gz`;
            const archive = f.archiveReceipt({ name,
                url: `https://builds.dotnet.microsoft.com/dotnet/Sdk/${cell.sdk.version}/${name}`,
                rid, architecture: cell.environment.architecture, provenance: { kind: "publisher-checksum" },
            }, "/retained/released-sdk.tar.gz", `SDK archive ${cell.environment.os}-${cell.environment.architecture}`);
            archive.sha512 = archive.actualSha512;
            if (!omitReleased.includes("toolchain")) cell.releasedToolchain = {
                sdk: structuredClone(cell.sdk), compilerRuntime: structuredClone(cell.runtime), archive,
            };
            for (const [name, version] of [["compiler", "14.0.100.0"], ["tasks", "14.0.0.0"], ["core", "10.0.0.0"]]) {
                if (!omitReleased.includes(name)) cell.artifacts[name].assemblyVersion = version;
            }
        }
        await state.commitObservation(h.transport, h.context, {
            generation: pending.generation, attemptId: pending.id, observation, evidence: [],
        });
    }
    const start = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    const candidate = h.context.pinnedMainSha, parent = f.sha("parent");
    const ids = [];
    for (const [sourceSha, outcome] of [[candidate, "product-failure"], [parent, "pass"]]) {
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        const pending = await state.reserveWork(h.transport, h.context, { generation: cp.generation,
            attemptId: `confirm-${sourceSha}`, request: { kind: "source", target: { sourceSha }, endpoint: null, phase: "confirmation" } });
        const execution = await state.beginWork(h.transport, h.context, { generation: pending.generation, attemptId: pending.id });
        const observation = f.observation("source", outcome, h.context.manifest, sourceSha);
        observation.buildInstanceId = pending.id;
        for (const cell of observation.cells) {
            for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = pending.id;
            cell.buildToolchain = f.buildToolchain(sourceSha, cell.environment.os);
        }
        await state.commitObservation(h.transport, h.context, {
            generation: execution.generation, attemptId: pending.id, observation, evidence: [],
        });
        ids.push(model.fingerprintInput(observation));
    }
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    const confirmation = {
        schemaVersion: 1, epoch: cp.state.epoch, manifestDigest: cp.state.approvalFingerprint, startGeneration: start.generation,
        candidate, classification, reason: "independent exact failure and passing parent",
        interval: { goodSha: parent, badSha: candidate,
            remaining: [...Array.from({ length: intervalCount - 1 }, (_, i) => f.sha(`remaining-${i}`)), candidate] },
        parents: [{ sourceSha: parent, comparable: true, observationDigest: ids[1] }],
        candidateObservationDigest: ids[0], searchObservationDigest: start.state.endpointEvidence.main,
        history: { commits: [{ sha: candidate, parents: [parent], messageDigest: f.digest("change"),
            changedPaths: ["src/Compiler/Checking/example.fs"], patchId: null, reverts: [], cherryPicks: [] }],
        monotonicity: "not-contradicted", toolchainCoupled: classification === "verified-triggering-change" },
        associations: { schemaVersion: 1, repository: "dotnet/fsharp", sourceSha: candidate,
            status: associationCount > 1 ? "ambiguous" : associationCount ? "unique" : "none",
            reason: associationCount > 1 ? "multiple verified associations; uniqueness is unproven" :
                associationCount ? "verified commit association" : "no verified associated pull request",
            pulls: Array.from({ length: associationCount }, (_, i) => ({
                number: i + 1, url: `https://github.com/dotnet/fsharp/pull/${i + 1}`, relation: "merge-commit",
                mergeSha: candidate, baseSha: parent, headSha: candidate, baseRef: "main", onLineage: true,
            })) }, latentRootCause: "unproven",
    };
    if (incomplete === "paths") {
        confirmation.history.toolchainCoupled = true;
        confirmation.history.commits[0].changedPathsSummary = {
            encoding: "git-quoted", totalCount: 257, sha256: f.digest("complete quoted paths"),
            truncated: true, toolchainCoupled: true,
        };
    } else if (incomplete === "remaining") {
        confirmation.interval.remainingSummary = {
            totalCount: 4096, sha256: f.digest("complete remaining SHAs"), truncated: true,
        };
    } else if (incomplete === "unknown") {
        confirmation.interval.remaining = null;
    } else if (incomplete === "commits") {
        confirmation.history.commitsSummary = { totalCount: 129, sha256: f.digest("history commits"), truncated: true };
    }
    const saved = await state.recordConfirmation(h.transport, h.context, { generation: cp.generation, confirmation });
    assert.equal(saved.state.phase, classification === "unconfirmed-candidate" ? "blocked" : "complete");
    const duplicate = await state.recordConfirmation(h.transport, h.context, { generation: cp.generation, confirmation });
    assert.equal(duplicate.generation, saved.generation);
    await assert.rejects(state.recordConfirmation(h.transport, h.context, {
        generation: saved.generation, confirmation: { ...confirmation, startGeneration: saved.generation + 1 },
    }), /stale|confirmation/);
    return Object.assign(h, { generation: saved.generation });
}

test("confirmed boundaries retain distinct parent/candidate evidence and become unpublishable on head movement", async () => {
    const h = await confirmedFixture(), { generation } = h;
    const ready = await h.prepare({ generation });
    assert.equal(ready.status, "ready", ready.reason);
    assert.match(ready.body, /verified introducing source boundary/);
    assert.ok(ready.body.includes(f.sha("parent")));
    assert.match(ready.body, /PR association: none/);
    h.fresh.pinnedMainSha = f.sha("moved");
    const stale = await h.prepare({ generation: ready.generation });
    assert.equal(stale.status, "requeue");
    assert.equal(stale.body, undefined);
    const blocked = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(blocked.publicationStatus, "blocked");
    assert.equal(blocked.state.budget.consumed.workerUnits, 4);
});

test("authenticated output retains the exact boundary classification and full relevant SHAs", async t => {
    for (const classification of ["verified-introducing-source-boundary", "verified-triggering-change", "unconfirmed-candidate"]) {
        await t.test(classification, async () => {
            const h = await confirmedFixture({ classification });
            const ready = await h.prepare({ generation: h.generation });
            assert.equal(ready.status, "ready", ready.reason);
            const body = await authenticatedDisplay(h, ready);
            assert.ok(body.includes(`Classification: ${classification.replaceAll("-", " ")}`));
            assert.ok(body.includes(`Candidate: ${h.context.pinnedMainSha}`));
            assert.ok(body.includes(`Tested pinned main: ${h.context.pinnedMainSha}`));
            assert.ok(body.includes(f.sha("parent")));
            assert.match(body, /latent root cause is unproven/);
            assert.match(body, /does not establish semantic causation/);
            assert.match(body, /PR association: none/);
        });

    }
});

test("authenticated association output includes every verified URL and explicit ambiguity or absence", async t => {
    for (const associationCount of [0, 1, 2]) await t.test(`${associationCount} associations`, async () => {
        const h = await confirmedFixture({ associationCount });
        const ready = await h.prepare({ generation: h.generation });
        assert.equal(ready.status, "ready", ready.reason);
        const body = await authenticatedDisplay(h, ready);
        for (let i = 1; i <= associationCount; i++) assert.ok(body.includes(`https://github.com/dotnet/fsharp/pull/${i}`));
        assert.ok(body.includes(`PR association: ${["none", "unique", "ambiguous"][associationCount]}`));
        if (associationCount > 1) assert.match(body, /uniqueness is unproven/);
        if (associationCount === 0) assert.match(body, /no verified associated pull request/);
    });
});

test("renderer enforces UTF-8 bytes even when Unicode display length fits", async t => {
    const h = await confirmedFixture({ associationCount: 2 });
    const ready = await h.prepare({ generation: h.generation });
    assert.equal(ready.status, "ready", ready.reason);
    const report = structuredClone(ready.report), associations = report.state.confirmation.associations;
    const pull = associations.pulls[0];
    for (let count = 2; count <= 100; count++) {
        associations.pulls = Array.from({ length: count }, (_, i) => ({
            ...pull, number: i + 1, url: `https://github.com/dotnet/fsharp/pull/${i + 1}`, baseRef: "x".repeat(64),
        }));
        const ascii = renderReport(report), unicodeBytes = Buffer.byteLength(ascii, "utf8") + count * 64;
        if (unicodeBytes < 32768) continue;
        assert.ok(ascii.length < 32768, "the same number of display characters fits");
        for (const p of associations.pulls) p.baseRef = "\u03bb".repeat(64);
        assert.throws(() => renderReport(report), /rendered report exceeds bounded display size/);
        t.diagnostic(`characters=${ascii.length} unicodeBytes=${unicodeBytes} associationCount=${count}`);
        return;
    }
    assert.fail("fixture must cross the UTF-8 bound before the character bound");
});

test("compact intervals and skipped reasons survive in the authenticated result", async () => {
    const h = await confirmedFixture({ classification: "unconfirmed-candidate", intervalCount: 200 });
    const ready = await h.prepare({ generation: h.generation });
    assert.equal(ready.status, "ready", ready.reason);
    const body = await authenticatedDisplay(h, ready);
    assert.match(body, /Complete interval reference: signed state\.confirmation\.interval/);
    assert.ok(body.includes(model.fingerprintInput(ready.report.state.confirmation.interval)));
    assert.deepEqual(ready.report.state.confirmation.interval.remaining,
        [...Array.from({ length: 199 }, (_, i) => f.sha(`remaining-${i}`)), h.context.pinnedMainSha]);
    const skipped = await fixture({ outcome: "untestable" });
    const result = await skipped.prepare();
    assert.equal(result.status, "ready", result.reason);
    const rendered = await authenticatedDisplay(skipped, result);
    assert.ok(result.report.state.skips.length > 0);
    for (const skip of result.report.state.skips) assert.ok(rendered.includes(`Skipped ${skip.sourceSha}: ${skip.reason}`));
});

test("truncated path evidence is disclosed and missing complete remaining evidence blocks publication", async t => {
    for (const incomplete of ["paths", "commits", "unknown", "remaining"]) await t.test(incomplete, async () => {
        const h = await confirmedFixture({ classification: "unconfirmed-candidate", incomplete });
        const before = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        const ready = await h.prepare({ generation: h.generation });
        if (incomplete !== "remaining") {
            assert.equal(ready.status, "ready", ready.reason);
            const body = await authenticatedDisplay(h, ready);
            if (incomplete === "unknown") {
                assert.match(body, /Saved interval boundaries\/decisions reference/);
                assert.ok(!body.includes("Complete interval reference"));
            } else {
                assert.match(body, incomplete === "paths" ? /total changed paths 257/ : /total commits 129/);
                assert.ok(body.includes(f.digest(incomplete === "paths" ? "complete quoted paths" : "history commits")));
                assert.match(body, /truncated true; retained sample only/);
            }
            assert.match(body, /Classification: unconfirmed candidate/);
        } else {
            assert.equal(ready.status, "blocked", ready.reason);
            assert.match(ready.reason, /complete remaining interval unavailable/);
            assert.equal(ready.body, undefined);
            const after = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
            assert.deepEqual(after.observations, before.observations);
            assert.deepEqual(after.state.confirmation, before.state.confirmation);
            assert.deepEqual(after.state.budget, before.state.budget);
        }
    });
});

test("saved escaped rendering mutations are killed by authenticated semantic assertions", async t => {
    const fs = require("node:fs"), Module = require("node:module"), path = require("node:path");
    const filename = require.resolve("./report.cjs"), source = fs.readFileSync(filename, "utf8");
    for (const [name, setup, before, after, check] of [
        ["trigger", () => confirmedFixture({ classification: "verified-triggering-change" }),
            'c.classification.replaceAll("-", " ")', '"classification omitted"',
            body => assert.match(body, /Classification: verified triggering change/)],
        ["PR links", () => confirmedFixture({ associationCount: 1 }),
            '...(c?.associations?.pulls ?? []).map', '...[].map',
            body => assert.ok(body.includes("https://github.com/dotnet/fsharp/pull/1"))],
        ["skips", () => fixture({ outcome: "untestable" }), '...s.skips.map', '...[].map',
            body => assert.ok(body.includes(`Skipped ${f.sha("main")}:`))],
    ]) await t.test(name, async () => {
        const h = await setup(), ready = await h.prepare({ generation: h.generation });
        assert.equal(ready.status, "ready", ready.reason);
        check(await authenticatedDisplay(h, ready));
        assert.ok(source.includes(before), "mutation must edit its intended semantic seam");
        const mutant = new Module(filename, module);
        mutant.filename = filename;
        mutant.paths = Module._nodeModulePaths(path.dirname(filename));
        mutant._compile(source.replace(before, after), filename);
        const original = require.cache[filename].exports;
        try {
            require.cache[filename].exports = mutant.exports;
            const mutatedBody = await authenticatedDisplay(h, ready);
            assert.throws(() => check(mutatedBody), { code: "ERR_ASSERTION" });
            t.diagnostic(`${name}: valid output passed; authenticated semantic mutant killed (module loaded)`);
        } finally { require.cache[filename].exports = original; }
    });
});

test("changing evidence or search phase clears confirmation without refunding prior runs", async t => {
    for (const transition of ["main", "approval", "search", "reserve"]) await t.test(transition, async () => {
        const h = await confirmedFixture();
        let cp;
        if (transition === "main" || transition === "approval") {
            if (transition === "main") h.context.pinnedMainSha = f.sha("moved");
            else h.context.authorization = null;
            cp = await state.invalidateStaleEvidence(h.transport, h.context, { generation: h.generation });
        } else if (transition === "search") {
            cp = await state.recordSearchProgress(h.transport, h.context, {
                generation: h.generation, phase: "bisect", interval: null,
            });
        } else {
            await state.reserveWork(h.transport, h.context, { generation: h.generation, attemptId: "new-search",
                request: { kind: "source", target: { sourceSha: f.sha("new-search") }, endpoint: null, phase: "bisect" } });
            cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        }
        assert.equal(cp.state.confirmation, undefined);
        assert.equal(cp.state.budget.consumed.workerUnits, transition === "reserve" ? 5 : 4);
    });
});

test("signed checkpoints cannot elevate an ordinary search observation to fresh confirmation", async () => {
    const h = await confirmedFixture();
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    cp.state.confirmation.candidateObservationDigest = cp.state.confirmation.searchObservationDigest;
    h.checkpoint(cp);
    await assert.rejects(state.loadCheckpoint(h.transport, h.context), /independent|confirmation/);
});

test("returning to confirmation work cannot publish a previously verified claim mid-run", async () => {
    const h = await confirmedFixture();
    const cp = await state.recordSearchProgress(h.transport, h.context, {
        generation: h.generation, phase: "confirmation", interval: null,
    });
    const result = await h.prepare({ generation: cp.generation });
    assert.equal(result.status, "blocked");
    assert.equal(result.body, undefined);
});

test("verified reports display the actual released archive, compiler runtime and bundled assembly versions", async () => {
    const h = await confirmedFixture();
    const result = await h.prepare({ generation: h.generation });
    assert.equal(result.status, "ready", result.reason);
    for (const expected of ["Released archive receipt", "Released compiler runtime", "assembly version 14.0.100.0",
        "assembly version 10.0.0.0", "assembly version 14.0.0.0", "dotnet-sdk-10\\.0\\.100-linux-x64",
        "Publisher-provided SHA-512"]) assert.ok(result.body.includes(expected), expected);
    assert.ok(!result.body.includes("not recorded in the observation contract"));
});

test("verified success requires complete released endpoint provenance without refund", async t => {
    for (const missing of ["endpoint", "toolchain", "compiler", "core"]) await t.test(missing, async () => {
        const h = await confirmedFixture({ omitReleased: [missing] });
        const result = await h.prepare({ generation: h.generation });
        assert.ok(["blocked", "requeue"].includes(result.status), JSON.stringify(result));
        assert.match(result.reason, /released.*archive.*runtime.*assembly/i);
        assert.equal(result.body, undefined);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.publicationStatus, "blocked");
        assert.equal(cp.state.budget.consumed.workerUnits, missing === "endpoint" ? 3 : 4);
    });
});

test("legacy unconfirmed reports disclose missing released receipts and assembly versions", async () => {
    const h = await confirmedFixture({ omitReleased: ["toolchain", "compiler", "tasks", "core"] });
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    cp.state.confirmation.classification = "unconfirmed-candidate";
    cp.state.confirmation.reason = "historical released evidence incomplete";
    cp.state.phase = "blocked";
    h.checkpoint(cp);
    const result = await h.prepare({ generation: cp.generation });
    assert.equal(result.status, "ready", result.reason);
    assert.match(result.body, /Released SDK archive receipt and compiler runtime: not recorded/);
    assert.match(result.body, /assembly version not recorded/);
});

test("unsafe or mismatched released identity is rejected before transport evidence reads", async t => {
    for (const mutation of ["url", "sdk", "rid", "source-kind"]) await t.test(mutation, async () => {
        const h = await confirmedFixture();
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        const released = cp.observations[cp.state.endpointEvidence.stable].data.cells[0];
        if (mutation === "url") released.releasedToolchain.archive.url = "https://attacker.invalid/archive.zip";
        if (mutation === "sdk") released.releasedToolchain.sdk.version = "10.0.200";
        if (mutation === "rid") released.releasedToolchain.archive.rid = "win-arm64";
        if (mutation === "source-kind") cp.observations[cp.state.endpointEvidence.main].data.cells[0].releasedToolchain =
            structuredClone(released.releasedToolchain);
        h.checkpoint(cp);
        let reads = 0;
        h.transport.readEvidence = async () => { reads++; return Buffer.from("durable evidence"); };
        const result = await h.prepare({ generation: cp.generation });
        assert.equal(result.status, "blocked");
        assert.equal(reads, 0);
    });
});
