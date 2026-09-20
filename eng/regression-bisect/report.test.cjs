const { test } = require("node:test");
const assert = require("node:assert/strict");
const f = require("./fixtures/contracts.cjs");
const { harness } = require("./fixtures/checkpoint.cjs");
const model = require("./model.cjs");
const state = require("./state.cjs");
const intake = require("./intake.cjs");
const { preparePublication, renderReport, completePublication } = require("./report.cjs");

async function fixture({ outcome = "product-failure", evidence = [], producerName = "repro", matrixCount = 1,
    archiveKind = "publisher-checksum" } = {}) {
    const h = harness(), calls = [], comments = [];
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
    for (const [name, change] of Object.entries(changes)) await t.test(name, async () => {
        const h = await fixture(); change(h);
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
    for (const mutation of ["none", "id", "writer", "app", "run", "markdown", "digest"]) await t.test(mutation, async () => {
        const h = await fixture();
        const ready = await h.prepare();
        assert.equal(ready.status, "ready", ready.reason);
        const context = { ...h.context, commentId: 789 };
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
        if (mutation === "markdown") comment.body = "# invented blame\n\n" + comment.body;
        h.comments.push(comment);
        const recovered = await h.prepare();
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

for (const matrixCount of [1, 2]) test(`six affordable observations and ${matrixCount} matrix cells fit without dropping evidence`, async () => {
    const h = await fixture({ matrixCount });
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
    assert.equal(result.status, "ready", result.reason);
    assert.equal(result.report.observations.length, 6);
    for (let i = 1; i < 6; i++) assert.ok(result.body.includes(`search-${i}`));
    const signed = state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
        payload: { schemaVersion: 1, digest: result.digest, report: result.report } }, { ...h.context, commentId: 789 });
    assert.ok(Buffer.byteLength(signed) < 48 * 1024);
});

async function confirmedFixture({ omitReleased = [] } = {}) {
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
        candidate, classification: "verified-introducing-source-boundary", reason: "independent exact failure and passing parent",
        interval: { goodSha: parent, badSha: candidate, remaining: [candidate] },
        parents: [{ sourceSha: parent, comparable: true, observationDigest: ids[1] }],
        candidateObservationDigest: ids[0], searchObservationDigest: start.state.endpointEvidence.main,
        history: { commits: [{ sha: candidate, parents: [parent], messageDigest: f.digest("change"),
            changedPaths: ["src/Compiler/Checking/example.fs"], patchId: null, reverts: [], cherryPicks: [] }],
        monotonicity: "not-contradicted", toolchainCoupled: false },
        associations: { schemaVersion: 1, repository: "dotnet/fsharp", sourceSha: candidate,
            status: "none", reason: "no verified associated pull request", pulls: [] }, latentRootCause: "unproven",
    };
    const saved = await state.recordConfirmation(h.transport, h.context, { generation: cp.generation, confirmation });
    assert.equal(saved.state.phase, "complete");
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
