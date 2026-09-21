const assert = require("node:assert/strict");
const { execFile, spawn } = require("node:child_process");
const { createHash } = require("node:crypto");
const { once } = require("node:events");
const fs = require("node:fs/promises");
const path = require("node:path");
const { Readable } = require("node:stream");
const { test } = require("node:test");
const { promisify } = require("node:util");
const { createFileEvidenceStore, createActionsEvidenceStore, validateEvidenceUrl, isControlledEvidenceUrl } = require("./evidence-store.cjs");
const { archiveReceipt, policy } = require("./fixtures/contracts.cjs");
const { fingerprintInput } = require("./model.cjs");

const hash = (bytes, algorithm = "sha256") => createHash(algorithm).update(bytes).digest("hex");
const binding = { workId: "work-1", launchId: "launch-1", unitDigest: hash("unit"), imageDigest: hash("image") };
const payload = Buffer.from("physical evidence, not a worker URL");

async function fixture(t) {
    const directory = await fs.mkdtemp(path.join(__dirname, ".evidence-test-"));
    t.after(() => fs.rm(directory, { recursive: true, force: true }));
    const config = { directory, repository: "dotnet/fsharp", runId: "12345", attempt: 2 };
    return { config, store: createFileEvidenceStore(config) };
}

function deadlineAtFault(t) {
    // Arm the real timer at the injected stall, not during unrelated fixture I/O.
    const timeout = AbortSignal.timeout;
    const controller = new AbortController();
    t.mock.method(AbortSignal, "timeout", () => controller.signal);
    return () => {
        const signal = timeout(20);
        signal.addEventListener("abort", () => controller.abort(signal.reason), { once: true });
    };
}

test("durable required bytes survive a fresh process without worker scratch or memory", async t => {
    const { config, store } = await fixture(t);
    const record = await store.writeEvidence(payload, binding);
    assert.deepEqual(Object.keys(record).sort(), ["required", "sha256", "url"]);
    assert.equal(record.sha256, hash(payload));
    assert.equal(record.required, true);
    const { stdout } = await promisify(execFile)(process.execPath, ["-e",
        `const {createFileEvidenceStore}=require(${JSON.stringify(path.join(__dirname, "evidence-store.cjs"))});
        createFileEvidenceStore(${JSON.stringify(config)}).readEvidence(${JSON.stringify(record.url)})
        .then(b=>{if(!b)process.exit(4);process.stdout.write(b.toString("hex"))}).catch(e=>{console.error(e);process.exit(5)});`,
    ]);
    assert.equal(stdout, payload.toString("hex"));
});

test("lost evidence acknowledgement is discoverable in a fresh process by reserved work identity", async t => {
    const { config, store } = await fixture(t);
    const record = await store.writeEvidence(payload, binding);
    await store.writeEvidence(Buffer.from("another work"), { ...binding, workId: "other-work" });
    await store.writeBlob(Readable.from([payload]), approved());
    const query = { runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId };
    const { stdout } = await promisify(execFile)(process.execPath, ["-e",
        `const {createFileEvidenceStore}=require(${JSON.stringify(path.join(__dirname, "evidence-store.cjs"))});
        createFileEvidenceStore(${JSON.stringify(config)}).findEvidence(${JSON.stringify(query)})
        .then(records=>process.stdout.write(JSON.stringify(records))).catch(e=>{console.error(e);process.exit(5)});`,
    ]);
    assert.deepEqual(JSON.parse(stdout), [record]);
    assert.deepEqual(await store.findEvidence({ ...query, buildInstanceId: "not-completed" }), []);
});

test("bounded byte streams are measured and replay writes are immutable", async t => {
    const { store } = await fixture(t);
    const a = await store.writeEvidence(Readable.from([payload.subarray(0, 3), payload.subarray(3)]), binding);
    const b = await store.writeEvidence(payload, binding);
    assert.deepEqual(a, b);
    assert.deepEqual(await store.readEvidence(a.url), payload);
    await assert.rejects(store.writeEvidence(Readable.from([Buffer.alloc(64 * 1024 * 1024 + 1)]), binding), /limit/);
});

test("worker-selected URLs and filenames cannot cross the store authority", async t => {
    const { store } = await fixture(t);
    const record = await store.writeEvidence(payload, binding);
    assert.equal(isControlledEvidenceUrl(record.url), true);
    assert.equal(validateEvidenceUrl(record.url), true);
    for (const url of ["https://attacker.invalid/a", "file:///etc/passwd", record.url + "#fragment",
        record.url.replace("github.com", "github.com.attacker.invalid"), record.url.replace("/12345/", "/12346/"),
        record.url.replace("dotnet/fsharp", "other/repository"), record.url + "&url=https://attacker.invalid"]) {
        assert.equal(isControlledEvidenceUrl(url), false);
        assert.throws(() => validateEvidenceUrl(url), /URL/);
        await assert.rejects(store.readEvidence(url), /URL|identity|binding|artifact/);
    }
    await assert.rejects(store.writeEvidence(payload, { ...binding, url: "https://attacker.invalid/a" }), /binding/);
});

test("missing bytes return null, corruption and linked objects throw", async t => {
    const { config, store } = await fixture(t);
    const record = await store.writeEvidence(payload, binding);
    const files = (await fs.readdir(config.directory)).filter(name => name.endsWith(".bin"));
    assert.equal(files.length, 1);
    const file = path.join(config.directory, files[0]);
    await fs.writeFile(file, Buffer.alloc(payload.length));
    await assert.rejects(store.readEvidence(record.url), /digest|checksum/);
    await fs.writeFile(file, payload);
    await fs.link(file, path.join(config.directory, "linked"));
    await assert.rejects(store.readEvidence(record.url), /regular|link/);
    await fs.rm(path.join(config.directory, "linked"));
    await fs.rm(file);
    assert.equal(await store.readEvidence(record.url), null);
});

function approved(bytes = payload) {
    const p = policy();
    const a = p.toolchains.measuredArchives[0];
    const archive = { name: "sdk-win-x64", rid: "win-x64", architecture: "x64", url: a.url,
        provenance: { kind: "approved-official-https-measured", approvalRevision: a.approvalRevision,
            approvalDigest: fingerprintInput(a) } };
    const request = { policy: p, kind: "source", sourceSha: a.sourceSha, product: a.product, version: a.version,
        platform: { os: "win", architecture: "x64", rid: "win-x64" }, archive };
    return { binding, request, receipt: archiveReceipt(archive, path.join(__dirname, "retained-sdk.zip"), bytes) };
}

test("archive blob lane validates acquisition receipt and independently measures both hashes", async t => {
    const { store } = await fixture(t);
    const input = approved();
    const record = await store.writeBlob(Readable.from([payload]), input);
    assert.equal(record.sha256, hash(payload));
    assert.deepEqual(await store.verifyEvidence(record.url),
        { sha256: hash(payload), sha512: hash(payload, "sha512"), bytes: payload.length });
    const chunks = [];
    await store.readBlob(record.url, bytes => chunks.push(Buffer.from(bytes)));
    assert.deepEqual(Buffer.concat(chunks), payload);
    await assert.rejects(store.readEvidence(record.url), /blob|stream/);
    for (const change of [
        o => { o.receipt.sha256 = hash("forged"); o.receipt.retained.sha256 = o.receipt.sha256; },
        o => { o.receipt.actualSha512 = hash("forged", "sha512"); o.receipt.retained.sha512 = o.receipt.actualSha512; },
        o => { o.receipt.bytesReceived++; o.receipt.retained.size++; },
        o => { o.request.sourceSha = "f".repeat(40); },
        o => { o.receipt.bytesReceived = 1073741825; o.receipt.retained.size = o.receipt.bytesReceived; },
    ]) {
        const changed = approved();
        change(changed);
        await assert.rejects(store.writeBlob(Readable.from([payload]), changed));
    }
});

test("launch archive reservations are durable and stop before the fourth GiB", async t => {
    const { config, store } = await fixture(t);
    for (let index = 0; index < 4; index++) {
        const input = approved(Buffer.from(`sdk-${index}`));
        input.receipt.bytesReceived = 1024 * 1024 * 1024;
        input.receipt.retained.size = input.receipt.bytesReceived;
        const current = index === 3 ? createFileEvidenceStore(config) : store;
        await assert.rejects(current.writeBlob(Readable.from([]), input), index === 3 ? /launch blob byte limit/ : /checksum/);
    }
    assert.equal((await fs.readdir(config.directory)).filter(n => n.startsWith(".upload")).length, 0);
});

for (const fault of ["quota", "evidence-publication", "blob-publication"])
test(`killing the ${fault} writer permits a new-process retry under the whole-issue lock`, { timeout: 10000 }, async t => {
    const { config } = await fixture(t);
    const input = approved();
    const modulePath = path.join(__dirname, "evidence-store.cjs");
    const write = fault === "evidence-publication"
        ? `.writeEvidence(Buffer.from(${JSON.stringify(payload.toString())}),${JSON.stringify(binding)})`
        : `.writeBlob(Readable.from([Buffer.from(${JSON.stringify(payload.toString())})]),${JSON.stringify(input)})`;
    const child = spawn(process.execPath, ["-e", `
        const fs=require("node:fs/promises"), path=require("node:path"), {Readable}=require("node:stream");
        for (const operation of ${JSON.stringify(fault === "quota" ? ["open"] : ["link", "rename"])}) {
            const original=fs[operation];
            fs[operation]=async function(file,...args) {
                const result=await original.call(this,file,...args);
                if(operation!=="open" || path.basename(file).startsWith("quota-")) {
                    process.send("fault-reached"); setInterval(()=>{},1000);
                    await new Promise(()=>{});
                }
                return result;
            };
        }
        require(${JSON.stringify(modulePath)}).createFileEvidenceStore(${JSON.stringify(config)})${write}
            .catch(e=>{console.error(e);process.exit(5)});
    `], { stdio: ["ignore", "ignore", "pipe", "ipc"] });
    t.after(() => { if (child.exitCode === null) child.kill("SIGKILL"); });
    const exited = once(child, "exit");
    assert.deepEqual(await once(child, "message"), ["fault-reached", undefined]);
    child.kill("SIGKILL");
    await exited;
    const { stdout } = await promisify(execFile)(process.execPath, ["-e", `
        const {Readable}=require("node:stream");
        const store=require(${JSON.stringify(modulePath)}).createFileEvidenceStore(${JSON.stringify(config)});
        (async()=>{
            const record=await store${write};
            await store.verifyEvidence(record.url);
            const found=await store.findEvidence(${JSON.stringify({ runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId })});
            if(${fault === "evidence-publication"} && found.length!==1) throw Error("lost published evidence");
            process.stdout.write(JSON.stringify(record));
        })().catch(e=>{console.error(e);process.exit(5)});
    `]);
    assert.equal(JSON.parse(stdout).sha256, hash(payload));
});

test("concurrent local reservations cannot overspend the durable launch quota", async t => {
    const { config } = await fixture(t);
    const results = await Promise.allSettled(Array.from({ length: 4 }, (_, index) => {
        const input = approved(Buffer.from(`concurrent-sdk-${index}`));
        input.receipt.bytesReceived = 1024 * 1024 * 1024;
        input.receipt.retained.size = input.receipt.bytesReceived;
        return createFileEvidenceStore(config).writeBlob(Readable.from([]), input);
    }));
    assert.equal(results.filter(r => r.status === "rejected" && /launch blob byte limit/.test(r.reason.message)).length, 1);
    assert.equal(results.filter(r => r.status === "rejected" && /checksum/.test(r.reason.message)).length, 3);
    assert.equal((await fs.readdir(config.directory)).filter(name => name.startsWith("quota-")).length, 3);
});

for (const fault of ["enumeration", "open-before", "open-after", "sync", "close"])
test(`expired quota ${fault} retains ownership until a fresh-process successor is safe`, { timeout: 10000 }, async t => {
    const { config } = await fixture(t);
    const input = approved(Buffer.from("expired-owner"));
    input.receipt.bytesReceived = input.receipt.retained.size = 1024 ** 3;
    const expire = deadlineAtFault(t);
    let entered, release, settled = false;
    const reached = new Promise(resolve => { entered = resolve; });
    const barrier = new Promise(resolve => { release = resolve; });
    const pause = async () => { entered(); await barrier; };
    const readdir = fs.readdir, open = fs.open;
    t.mock.method(fs, "readdir", async (...args) => {
        const names = await readdir(...args);
        if (fault === "enumeration") await pause();
        return names;
    });
    t.mock.method(fs, "open", async (...args) => {
        if (!path.basename(args[0]).startsWith("quota-")) return open(...args);
        if (fault === "open-before") await pause();
        const handle = await open(...args);
        if (fault === "open-after") await pause();
        if (["sync", "close"].includes(fault)) {
            const original = handle[fault].bind(handle);
            t.mock.method(handle, fault, async () => { await pause(); return original(); });
        }
        return handle;
    });
    const writer = createFileEvidenceStore(config);
    const outcome = writer.writeBlob(Readable.from([]), input).catch(error => error.name);
    let handoff, early, successor;
    try {
        await reached;
        expire();
        assert.equal(await outcome, "TimeoutError");
        handoff = (writer.settleQuota?.() ?? Promise.resolve()).then(() => { settled = true; });
        await new Promise(resolve => setImmediate(resolve));
        early = settled;
        if (!early) { release(); await handoff; }
        const { stdout } = await promisify(execFile)(process.execPath, ["-e", `
            const {Readable}=require("node:stream"), crypto=require("node:crypto");
            const store=require(${JSON.stringify(path.join(__dirname, "evidence-store.cjs"))})
                .createFileEvidenceStore(${JSON.stringify(config)});
            (async()=>{
                const results=[];
                for(let i=0;i<3;i++){
                    const input=${JSON.stringify(input)};
                    input.receipt.sha256=input.receipt.retained.sha256=crypto.createHash("sha256").update("successor-"+i).digest("hex");
                    try { await store.writeBlob(Readable.from([]), input); results.push("accepted"); }
                    catch(e) { results.push(e.message); }
                }
                process.stdout.write(JSON.stringify(results));
            })().catch(e=>{console.error(e);process.exit(5)});
        `], { timeout: 5000 });
        successor = JSON.parse(stdout);
    } finally {
        release();
        await outcome;
        await handoff;
        // Drain the original queue even on the unfixed baseline.
        t.mock.restoreAll();
        await writer.writeBlob(Readable.from([]), input).catch(() => {});
    }
    const markers = (await fs.readdir(config.directory)).filter(n => n.startsWith("quota-"));
    const total = markers.reduce((sum, n) => sum + Number(n.split("-").at(-1)), 0);
    t.diagnostic(JSON.stringify({ fault, earlyHandoff: early, markers, total, successor, realLaunches: 0 }));
    assert.ok(total <= 3 * 1024 ** 3, "expired owner must not overspend durable accounting");
    assert.equal(early, false, "whole-issue ownership cannot pass an outstanding quota operation");
    assert.equal(markers.length, 3);
    await assert.rejects(writer.writeBlob(Readable.from([]), input),
        fault === "enumeration" ? /launch blob byte limit/ : /checksum/,
        "a charged same-blob retry must remain admissible at the cap");
});

test("expired quota queue wait cancels before enumeration or mutation", { timeout: 5000 }, async t => {
    const { config, store } = await fixture(t);
    const expire = deadlineAtFault(t);
    let entered, release, enumerations = 0;
    const reached = new Promise(resolve => { entered = resolve; });
    const barrier = new Promise(resolve => { release = resolve; });
    const readdir = fs.readdir;
    t.mock.method(fs, "readdir", async (...args) => {
        enumerations++;
        const names = await readdir(...args);
        entered(); await barrier;
        return names;
    });
    const first = store.writeBlob(Readable.from([payload]), approved()).catch(error => error.name);
    await reached;
    const second = createFileEvidenceStore(config).writeBlob(Readable.from([payload]), approved(Buffer.from("queued")))
        .catch(error => error.name);
    expire();
    try { assert.deepEqual(await Promise.all([first, second]), ["TimeoutError", "TimeoutError"]); }
    finally { release(); await store.settleQuota(); }
    t.mock.restoreAll();
    assert.equal(enumerations, 1, "expired queued work cannot begin a quota transaction");
    assert.deepEqual(await fs.readdir(config.directory), []);
});

for (const fault of ["lost-open-acknowledgement", "sync", "close"])
test(`expired quota accounting retains charges and acknowledged retry after ${fault}`, async t => {
    const { config, store } = await fixture(t);
    const open = fs.open;
    let injected = false;
    t.mock.method(fs, "open", async (...args) => {
        const handle = await open(...args);
        if (injected || !path.basename(args[0]).startsWith("quota-")) return handle;
        injected = true;
        if (fault === "lost-open-acknowledgement") { await handle.close(); throw new Error(fault); }
        const original = handle[fault].bind(handle);
        t.mock.method(handle, fault, async () => { await original(); throw new Error(fault); });
        return handle;
    });
    await assert.rejects(store.writeBlob(Readable.from([payload]), approved()), new RegExp(fault));
    await store.settleQuota();
    t.mock.restoreAll();
    const before = await fs.readdir(config.directory);
    assert.equal(before.length, 1);
    assert.ok(before[0].endsWith(`-${payload.length}`));
    const retry = createFileEvidenceStore(config);
    const record = await retry.writeBlob(Readable.from([payload]), approved());
    await store.settleQuota();
    assert.equal((await fs.readdir(config.directory)).filter(n => n.startsWith("quota-")).length, 1);
    assert.equal((await retry.verifyEvidence(record.url)).sha256, hash(payload));
});

test("expired quota controller reports bounded cleanup without releasing whole-issue ownership", { timeout: 10000 }, async t => {
    const { store, config } = await fixture(t);
    const { h, deps } = await require("./fixtures/execution.cjs").fixture();
    const { createObservationCollector } = require("./collector.cjs");
    const { runController } = require("./controller.cjs");
    const expire = deadlineAtFault(t);
    let entered, release, unlocked, depth = 0;
    const reached = new Promise(resolve => { entered = resolve; });
    const barrier = new Promise(resolve => { release = resolve; });
    const released = new Promise(resolve => { unlocked = resolve; });
    const open = fs.open;
    t.mock.method(fs, "open", async (...args) => {
        if (path.basename(args[0]).startsWith("quota-")) { entered(); await barrier; }
        return open(...args);
    });
    deps.withLock = action => h.lock(async () => {
        depth++;
        try { return await action(); } finally { depth--; unlocked(); }
    });
    deps.runIsolated = async () => ({ outcome: { outcome: "untestable" } });
    const collector = createObservationCollector({ evidenceDirectory: config.directory, store });
    deps.collectObservation = Object.assign(async () => store.writeBlob(Readable.from([payload]), approved()),
        { settleQuota: collector.settleQuota });
    const running = runController({ repository: "dotnet/fsharp", issueNumber: 123 }, deps);
    let successor = false, next;
    try {
        await reached;
        expire();
        const result = await running;
        assert.equal(result.status, "blocked");
        assert.match(result.reason, /ownership cleanup pending/);
        assert.equal(depth, 1, "bounded cleanup reporting must retain the actual lock callback");
        next = h.lock(() => { successor = true; });
        await new Promise(resolve => setImmediate(resolve));
        assert.equal(successor, false);
    } finally {
        release();
        await running;
        await released;
        await next;
    }
    t.mock.restoreAll();
    assert.equal(depth, 0);
    assert.equal(successor, true);
    assert.equal((await fs.readdir(config.directory)).filter(n => n.startsWith("quota-")).length, 1);
    const record = await store.writeBlob(Readable.from([payload]), approved());
    assert.equal((await store.verifyEvidence(record.url)).sha256, hash(payload));
});

async function actionsFixture(t) {
    const { config } = await fixture(t);
    const claims = { scp: `Actions.Results:${"1".repeat(8)}-1111-1111-1111-${"1".repeat(12)}:${"2".repeat(8)}-2222-2222-2222-${"2".repeat(12)}` };
    Object.assign(config, { token: "trusted-read-token", runtimeToken: `header.${Buffer.from(JSON.stringify(claims)).toString("base64url")}.signature` });
    const state = { calls: [], artifacts: [], zip: null, hook: null };
    const api = "https://api.github.com/repos/dotnet/fsharp";
    const signed = "https://evidence123.blob.core.windows.net/container/owned-artifact?sv=2023-11-03&sr=b&sig=owned-signature";
    const response = (data, options) => new Response(JSON.stringify(data), options);
    const send = async (url, options) => {
        state.calls.push({ url, options });
        assert.equal(options.redirect, "manual");
        const headers = new Headers(options.headers);
        assert.equal(headers.get("Authorization"), url.startsWith(`${api}/`) ? `Bearer ${config.token}` :
            url.startsWith("https://results-receiver.actions.githubusercontent.com/") ? `Bearer ${config.runtimeToken}` : null);
        assert.equal(headers.get("Cookie"), null);
        assert.equal(headers.get("Proxy-Authorization"), null);
        if (state.hook) {
            const result = await state.hook(url, options);
            if (result) return result;
        }
        if (url.includes("/attempts/")) return response({ id: 12345, run_attempt: 2,
            repository: { full_name: "dotnet/fsharp", id: 777 }, head_repository: { full_name: "dotnet/fsharp", id: 777 } });
        if (url.includes("/artifacts?")) return response({ total_count: state.artifacts.length, artifacts: state.artifacts });
        if (url.endsWith("/CreateArtifact")) {
            const body = JSON.parse(options.body);
            assert.deepEqual(Object.keys(body).sort(), ["expires_at", "name", "version", "workflow_job_run_backend_id", "workflow_run_backend_id"]);
            assert.equal(body.version, 4);
            assert.ok(Date.parse(body.expires_at) > Date.now());
            assert.equal(options.headers.Authorization, `Bearer ${config.runtimeToken}`);
            state.name = body.name;
            return response({ ok: true, signed_upload_url: signed });
        }
        if (url === signed && options.method === "PUT") {
            assert.equal(options.headers.Authorization, undefined);
            assert.equal(options.headers["x-ms-blob-type"], "BlockBlob");
            const chunks = [];
            for await (const chunk of options.body) chunks.push(Buffer.from(chunk));
            state.zip = Buffer.concat(chunks);
            assert.equal(state.zip.length, Number(options.headers["Content-Length"]));
            return new Response(null, { status: 201 });
        }
        if (url.endsWith("/FinalizeArtifact")) {
            const body = JSON.parse(options.body);
            assert.equal(body.hash, `sha256:${hash(state.zip)}`);
            assert.equal(body.size, String(state.zip.length));
            const metadata = { id: 87654, name: state.name, size_in_bytes: state.zip.length, expired: false,
                expires_at: "2026-12-01T00:00:00Z", digest: body.hash,
                workflow_run: { id: 12345, repository_id: 777, head_repository_id: 777 },
                archive_download_url: `${api}/actions/artifacts/87654/zip` };
            state.artifacts = [metadata];
            if (state.finalizeFault) await state.finalizeFault();
            return response({ ok: true, artifact_id: "87654" });
        }
        if (url === `${api}/actions/artifacts/87654`) return response(state.artifacts[0]);
        if (url === `${api}/actions/artifacts/87654/zip`)
            return new Response(null, { status: 302, headers: { location: signed } });
        if (url === signed) {
            assert.equal(options.headers.Authorization, undefined);
            return new Response(state.zip, { headers: { "Content-Length": String(state.zip.length) } });
        }
        throw new Error(`unexpected mock request: ${url}`);
    };
    return { config, state, signed, response, store: createActionsEvidenceStore(config, { send }), send };
}

test("Actions uses the concrete create, signed ZIP upload, finalize, authenticated read protocol", async t => {
    const { store, state, config, send } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    assert.ok(state.calls.every(c => c.options.signal === state.calls[0].options.signal),
        "every write request must share the original deadline");
    assert.deepEqual(await store.readEvidence(record.url), payload);
    assert.deepEqual(await createActionsEvidenceStore(config, { send }).writeEvidence(payload, binding), record);
    assert.equal(state.calls.filter(c => c.url.endsWith("/CreateArtifact")).length, 1);
    assert.equal(state.calls.filter(c => c.options.method === "PUT").length, 1);
    assert.equal(state.zip.subarray(42, 42 + payload.length).toString(), payload.toString());
    assert.equal((await fs.readdir(config.directory)).length, 0);
});

test("Actions discovers an acknowledged-lost artifact using only reservation owner and build identity", async t => {
    const { store, config, send } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    const replay = createActionsEvidenceStore(config, { send });
    assert.deepEqual(await replay.findEvidence({ runId: config.runId, attempt: config.attempt,
        buildInstanceId: binding.workId }), [record]);
    assert.deepEqual(await replay.findEvidence({ runId: config.runId, attempt: config.attempt,
        buildInstanceId: "not-completed" }), []);
});

test("Actions recovers and verifies owned evidence on the second discovery page", async t => {
    const { store, config, state, send, response } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    const pages = [];
    state.hook = url => {
        if (!url.includes("/artifacts?")) return null;
        const page = Number(new URL(url).searchParams.get("page"));
        pages.push(page);
        assert.ok(page === 1 || page === 2);
        return response({ total_count: 101, artifacts: page === 1
            ? Array.from({ length: 100 }, (_, i) => ({ id: i + 1, name: `unrelated-${i}` }))
            : state.artifacts });
    };
    const replay = createActionsEvidenceStore(config, { send });
    assert.deepEqual(await replay.findEvidence({
        runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId,
    }), [record]);
    assert.deepEqual(pages, [1, 2]);
    assert.deepEqual(await replay.readEvidence(record.url), payload);
    assert.equal(state.calls.filter(c => c.options.method === "PUT").length, 1);
});

for (const method of ["readEvidence", "verifyEvidence", "readBlob", "findEvidence"])
for (const fault of ["present", "absent", "expired", "revalidation-404", "revalidation-410",
    ...(method === "findEvidence" ? ["pages", "candidates"] : [])])
test(`Actions operation deadline covers cumulative ${method} ${fault}`, { timeout: 5000 }, async t => {
    const { config, store, state, send, response, signed } = await actionsFixture(t);
    const record = method === "readBlob" ? await store.writeBlob(Readable.from([payload]), approved())
        : await store.writeEvidence(payload, binding);
    const query = { runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId };
    const timeline = [];
    let missing = false, cancelled = false, listed = false, downloaded = false;
    // Advance only injected service latency, never unrelated host scheduling.
    t.mock.timers.enable({ apis: ["setTimeout", "Date"], now: Date.now() });
    t.mock.method(AbortSignal, "timeout", ms => {
        const controller = new AbortController();
        setTimeout(() => controller.abort(new DOMException("operation deadline", "TimeoutError")), ms);
        return controller.signal;
    });
    const delay = async signal => {
        const waiting = require("node:timers/promises").setTimeout(45, undefined, { signal });
        t.mock.timers.tick(45);
        await waiting;
    };
    state.hook = async (url, options) => {
        timeline.push({ url, at: Date.now() });
        const run = url.includes("/attempts/"), listing = url.includes("/artifacts?");
        const candidate = method !== "findEvidence" || listed;
        const absence = ["absent", "expired"].includes(fault) && candidate;
        const delayed = absence || fault === "present" && !run && !listing ||
            fault.startsWith("revalidation") && (missing || url.endsWith("/zip")) ||
            fault === "pages" && (run || listing) ||
            fault === "candidates" && (url.endsWith("/zip") || url === signed || downloaded && run);
        if (delayed) await delay(options.signal);
        if (listing) listed = true;
        if (url === signed) downloaded = true;
        if (url.includes("/attempts/")) {
            const bytes = Buffer.from(JSON.stringify({ id: 12345, run_attempt: 2,
                repository: { full_name: "dotnet/fsharp", id: 777 }, head_repository: { full_name: "dotnet/fsharp", id: 777 } }));
            return new Response(new ReadableStream({
                async start(controller) {
                    try { if (absence) await delay(options.signal); controller.enqueue(bytes); controller.close(); }
                    catch (error) { controller.error(error); }
                },
                cancel() { cancelled = true; },
            }));
        }
        if (url.endsWith("/zip") && fault.startsWith("revalidation")) {
            missing = true;
            return new Response(null, { status: Number(fault.split("-")[1]) });
        }
        if (url.endsWith("/artifacts/87654")) {
            if (fault === "absent" || missing) return response({}, { status: 404 });
            if (fault === "expired") return response({ ...state.artifacts[0], expired: true });
        }
        if (url.includes("/artifacts?") && fault === "pages")
            return response({ total_count: 101, artifacts: url.endsWith("page=1")
                ? Array.from({ length: 100 }, (_, i) => ({ id: i + 1, name: `unrelated-${i}` })) : state.artifacts });
        if (url.includes("/artifacts?") && fault === "candidates")
            return response({ total_count: 2, artifacts: [state.artifacts[0], { ...state.artifacts[0], id: 87655 }] });
        if (url.endsWith("/artifacts/87655")) return response({}, { status: 404 });
        return null;
    };
    const start = Date.now(), first = state.calls.length;
    const reader = createActionsEvidenceStore({ ...config, timeoutMs: 120 }, { send });
    const result = await reader[method](method === "findEvidence" ? query : record.url, () => {})
        .then(() => "accepted", error => error.name);
    t.diagnostic(JSON.stringify({ method, fault, result, elapsed: Date.now() - start, timeline, cancelled, realLaunches: 0 }));
    assert.ok(["TimeoutError", "AbortError"].includes(result), "expiry must not return success, absence or partial discovery");
    const signals = state.calls.slice(first).map(c => c.options.signal);
    assert.ok(signals.length >= 2, "must cross individually affordable request stages");
    assert.ok(signals.every(signal => signal === signals[0]), "all requests share the public operation deadline");
    assert.equal(signals[0].aborted, true);
    assert.equal(Date.now() - start, 135, "three affordable 45 ms stages must exceed one 120 ms operation");
    if (fault.startsWith("revalidation")) assert.equal(missing, true, "must reach authenticated download revalidation");
    if (fault === "pages") assert.ok(timeline.some(call => call.url.endsWith("page=2")));
    if (fault === "candidates") {
        assert.equal(downloaded, true, "first candidate must finish before expiry in the next candidate");
        assert.equal(timeline.filter(call => call.url.includes("/attempts/")).length, 3);
    }
});

for (const method of ["readEvidence", "verifyEvidence", "readBlob", "findEvidence"])
for (const fault of ["ignored-abort", "response-close", "metadata-body", "blob-body", ...(method === "readBlob" ? ["sink"] : [])])
test(`Actions operation deadline bounds ${method} ${fault} and cleans late responses`, { timeout: 5000 }, async t => {
    const { config, store, state, send, signed } = await actionsFixture(t);
    const record = method === "readBlob" ? await store.writeBlob(Readable.from([payload]), approved())
        : await store.writeEvidence(payload, binding);
    const expire = deadlineAtFault(t);
    let entered, release, closed, cancellations = 0;
    const reached = new Promise(resolve => { entered = resolve; });
    const barrier = new Promise(resolve => { release = resolve; });
    const cleanup = new Promise(resolve => { closed = resolve; });
    state.hook = async url => {
        if (fault === "sink") return null;
        if (fault.endsWith("-body")) {
            if (!(fault === "blob-body" ? url === signed : url.endsWith("/artifacts/87654"))) return null;
            return new Response(new ReadableStream({
                start() { entered(); },
                cancel() { cancellations++; closed(); },
            }));
        }
        if (url !== signed && fault !== "ignored-abort" && !url.endsWith("/zip")) return null;
        if (fault === "ignored-abort") { entered(); await barrier; }
        if (fault === "response-close" && !url.endsWith("/zip")) return null;
        return new Response(new ReadableStream({
            async cancel() {
                cancellations++;
                if (fault === "response-close") { entered(); await barrier; }
                closed();
            },
        }), { status: 302, headers: { location: signed } });
    };
    const reader = createActionsEvidenceStore(config, { send });
    const query = { runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId };
    const outcome = reader[method](method === "findEvidence" ? query : record.url, async () => {
        entered(); await barrier; closed();
    }).then(() => "accepted", error => error.name);
    let result, timer, calls;
    try {
        await reached;
        expire();
        result = await Promise.race([outcome, new Promise(resolve => { timer = setTimeout(resolve, 500, "deadline missed"); })]);
        calls = state.calls.length;
    } finally {
        clearTimeout(timer);
        release();
        await outcome;
        await cleanup;
    }
    assert.equal(result, "TimeoutError");
    assert.equal(await outcome, "TimeoutError");
    assert.equal(state.calls.length, calls, "late completion cannot start another request");
    assert.equal(cancellations, fault === "sink" ? 0 : 1);
});

for (const lane of ["evidence", "blob"])
test(`Actions recovers ${lane} after losing the finalization response without uploading again`, async t => {
    const { config, state, send, store } = await actionsFixture(t);
    const write = store => lane === "blob" ? store.writeBlob(Readable.from([payload]), approved())
        : store.writeEvidence(payload, binding);
    state.finalizeFault = () => { throw new Error("lost finalization response"); };
    await assert.rejects(write(store), /lost finalization response/);
    assert.deepEqual((await fs.readdir(config.directory)).filter(n => n.startsWith(".upload-")), []);
    const artifactId = state.artifacts[0].id;
    state.finalizeFault = null;
    const replay = createActionsEvidenceStore(config, { send });
    const record = await write(replay);
    assert.ok(new URL(record.url).pathname.endsWith(`/artifacts/${artifactId}`));
    assert.equal((await replay.verifyEvidence(record.url)).sha256, hash(payload));
    if (lane === "evidence") assert.deepEqual(await replay.findEvidence({
        runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId,
    }), [record]);
    for (const operation of ["CreateArtifact", "FinalizeArtifact"])
        assert.equal(state.calls.filter(c => c.url.endsWith(`/${operation}`)).length, 1);
    assert.equal(state.calls.filter(c => c.options.method === "PUT").length, 1);
});

for (const status of [404, 410])
for (const target of ["archive", "blob"])
for (const confirmation of target === "archive" ? ["present", "expired", 404, 410, 401, 403, 429, "network"] : ["present"])
test(`Actions download ${target} ${status} requires authenticated absence: ${confirmation}`, async t => {
    const { store, state, response, signed } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    let missing = false, rechecks = 0;
    state.hook = url => {
        if (target === "archive" ? url.endsWith("/zip") : url === signed) {
            missing = true;
            return response({}, { status });
        }
        if (!missing || !url.endsWith("/artifacts/87654")) return null;
        rechecks++;
        if (confirmation === "network") throw new Error("offline network failure");
        if (typeof confirmation === "number") return response({}, { status: confirmation });
        return response({ ...state.artifacts[0], expired: confirmation === "expired" });
    };
    if (["expired", 404, 410].includes(confirmation)) assert.equal(await store.readEvidence(record.url), null);
    else await assert.rejects(store.readEvidence(record.url), /confirmed expiry|metadata request failed|offline network|blob request failed/);
    assert.equal(rechecks, target === "archive" ? 1 : 0);
});

test("Actions recovery rejects corrupt bytes, incomplete listings and excessive discovery", async t => {
    const { store, state, config, response } = await actionsFixture(t);
    await store.writeEvidence(payload, binding);
    const query = { runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId };
    state.zip[42] ^= 1;
    await assert.rejects(store.findEvidence(query), /ZIP|digest/);
    for (const list of [
        { total_count: 501, artifacts: [] },
        { total_count: 2, artifacts: state.artifacts },
        { total_count: 2, artifacts: [state.artifacts[0], state.artifacts[0]] },
    ]) {
        state.hook = url => url.includes("/artifacts?") ? response(list) : null;
        await assert.rejects(store.findEvidence(query), /discovery/);
    }
});

test("Actions confirms artifact absence only after authenticating the owning run", async t => {
    const { store, state, response } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    state.hook = url => url.endsWith("/artifacts/87654") ? response({}, { status: 404 }) : null;
    assert.equal(await store.readEvidence(record.url), null);
    for (const status of [401, 403, 429, 500]) {
        state.hook = url => url.endsWith("/artifacts/87654") ? response({}, { status }) : null;
        await assert.rejects(store.readEvidence(record.url), /metadata request failed/);
    }
    state.hook = url => url.includes("/attempts/") ? response({}, { status: 404 }) : null;
    await assert.rejects(store.readEvidence(record.url), /metadata request failed/);
    state.hook = null;
    state.artifacts[0].expired = true;
    assert.equal(await store.readEvidence(record.url), null);
});

test("hostile artifact metadata cannot redirect or substitute repository, attempt, name, digest, or bounds", async t => {
    const { store, state, response } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    const original = structuredClone(state.artifacts[0]);
    for (const mutate of [
        m => { m.id++; }, m => { m.name += "-evil"; }, m => { m.workflow_run.id++; },
        m => { m.workflow_run.repository_id++; }, m => { m.workflow_run.head_repository_id++; },
        m => { m.archive_download_url = "https://attacker.invalid/zip"; },
        m => { m.size_in_bytes++; }, m => { m.digest = "bogus"; },
    ]) {
        state.artifacts[0] = structuredClone(original);
        mutate(state.artifacts[0]);
        await assert.rejects(store.readEvidence(record.url), /metadata ownership/);
    }
    state.artifacts[0] = original;
    state.hook = url => url.includes("/attempts/") ? response({ id: 12345, run_attempt: 1,
        repository: { full_name: "dotnet/fsharp", id: 777 }, head_repository: { full_name: "dotnet/fsharp", id: 777 } }) : null;
    await assert.rejects(store.readEvidence(record.url), /run identity/);
});

test("signed redirects are exact owned HTTPS blob objects and never receive GitHub credentials", async t => {
    const { store, state, signed } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    for (const target of ["https://attacker.invalid/blob", "http://127.0.0.1/blob",
        signed.replace("blob.core.windows.net", "blob.core.windows.net.attacker.invalid"),
        signed.replace("https://", "https://user:password@"), signed.replace("/owned-artifact", "/../secret"),
        signed + "#fragment", signed + "&sig=duplicate", signed.replace("sr=b", "sr=c")]) {
        state.hook = url => url.endsWith("/zip") ? new Response(null, { status: 302, headers: { location: target } }) : null;
        await assert.rejects(store.readEvidence(record.url), /signed URL/);
        assert.equal(state.calls.at(-1).url.endsWith("/zip"), true);
    }
    state.hook = url => url === signed ? new Response(null, { status: 302, headers: { location: "https://attacker.invalid/" } }) : null;
    await assert.rejects(store.readEvidence(record.url), /blob request failed/);
    assert.equal(state.calls.at(-1).options.headers.Authorization, undefined);
});

test("ZIP is read only as one bounded data file, never as an extraction archive", async t => {
    const { store, state } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    const original = Buffer.from(state.zip);
    for (const mutate of [
        zip => { zip.write("../evil.bin", 30); },
        zip => { zip.writeUInt16LE(8, 8); },
        zip => { zip.writeUInt32LE(0xa1ff0000, 42 + payload.length + 38); },
        zip => { zip.writeUInt32LE(0x81a40000, 42 + payload.length + 38); },
        zip => { zip[42] ^= 1; },
        zip => { zip.writeUInt16LE(2, zip.length - 14); },
    ]) {
        state.zip = Buffer.from(original);
        mutate(state.zip);
        // Even a substituted transport digest cannot authorize unsafe ZIP metadata.
        state.artifacts[0].digest = `sha256:${hash(state.zip)}`;
        await assert.rejects(store.readEvidence(record.url), /ZIP|digest/);
    }
    for (const zip of [original.subarray(0, -1), Buffer.concat([original, Buffer.from("x")])]) {
        state.zip = zip;
        await assert.rejects(store.readEvidence(record.url), /byte limit/);
    }
});

test("upload failures and unapproved results/upload authorities fail closed", async t => {
    const { store, state, config, send, response, signed } = await actionsFixture(t);
    state.hook = url => url.endsWith("/CreateArtifact") ?
        response({ ok: true, signed_upload_url: "https://attacker.invalid/upload" }) : null;
    await assert.rejects(store.writeEvidence(payload, binding), /signed URL/);
    assert.equal(state.calls.some(c => c.options.method === "PUT"), false);
    state.hook = url => url === signed ? new Response(null, { status: 403 }) : null;
    await assert.rejects(store.writeEvidence(payload, binding), /upload failed/);
    await assert.rejects(createActionsEvidenceStore({ ...config, resultsUrl: "https://attacker.invalid" }, { send })
        .writeEvidence(payload, binding), /results authority/);
    assert.equal((await fs.readdir(config.directory)).length, 0);
});

test("artifact metadata truncation and oversized bodies throw without becoming absent evidence", async t => {
    const { store, state, response } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    for (const result of [
        () => response(state.artifacts[0], { headers: { "content-length": "1" } }),
        () => response({}, { headers: { "content-length": "1048577" } }),
        () => new Response(Buffer.alloc(1048577)),
        () => response({}, { headers: { "content-encoding": "gzip" } }),
    ]) {
        state.hook = url => url.endsWith("/artifacts/87654") ? result() : null;
        await assert.rejects(store.readEvidence(record.url), /metadata.*(length|failed)|byte limit/);
    }
    state.hook = () => { throw new Error("offline network failure"); };
    await assert.rejects(store.readEvidence(record.url), /offline network failure/);
});

test("data-only ZIP validation survives split headers and rejects unadvertised truncation and extra bytes", async t => {
    const { store, state, signed } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    const responseStream = bytes => new Response(new ReadableStream({
        start(controller) {
            for (let i = 0; i < bytes.length; i += 7) controller.enqueue(bytes.subarray(i, i + 7));
            controller.close();
        },
    }));
    state.hook = url => url === signed ? responseStream(state.zip) : null;
    assert.deepEqual(await store.readEvidence(record.url), payload);
    const original = state.zip;
    for (const bytes of [original.subarray(0, -1), Buffer.concat([original, Buffer.from("x")])]) {
        state.zip = bytes;
        await assert.rejects(store.readEvidence(record.url), /ZIP|byte limit/);
    }
});

test("Actions archive blobs retain publisher versus measured provenance without buffering reads", async t => {
    const { store } = await actionsFixture(t);
    const input = approved();
    const record = await store.writeBlob(Readable.from([payload]), input);
    assert.deepEqual(await store.verifyEvidence(record.url), { bytes: payload.length,
        sha256: hash(payload), sha512: hash(payload, "sha512") });
    assert.equal(input.receipt.provenance.kind, "approved-official-https-measured");
    assert.equal(input.receipt.sha512, undefined);
    await assert.rejects(store.readEvidence(record.url), /blob requires streaming/);
});

test("blob receipt paths describe worker provenance, never a host filename", async t => {
    const { store } = await fixture(t);
    const input = approved();
    input.receipt.retained.path = "/scratch/toolchains/consumed-sdk.zip";
    const record = await store.writeBlob(Readable.from([payload]), input);
    assert.equal((await store.verifyEvidence(record.url)).sha256, hash(payload));
});

test("rejected receipt destroys the transfer stream before accepting any bytes", async t => {
    const { store } = await fixture(t);
    const input = approved();
    input.receipt.sha256 = "bad";
    const stream = Readable.from([payload]);
    await assert.rejects(store.writeBlob(stream, input), /checksum|provenance/);
    assert.equal(stream.destroyed, true);
});

test("file store rejects linked roots and leaves no accepted object after partial input failure", async t => {
    const { config, store } = await fixture(t);
    async function* fail() { yield payload; throw new Error("lost worker pipe"); }
    await assert.rejects(store.writeEvidence(Readable.from(fail()), binding), /lost worker pipe/);
    await assert.rejects(store.writeEvidence(Readable.from(["not raw data"]), binding), /raw bytes/);
    assert.deepEqual(await fs.readdir(config.directory), []);
    const actual = path.join(config.directory, "actual"), linked = path.join(config.directory, "linked");
    await fs.mkdir(actual);
    await fs.symlink(actual, linked, process.platform === "win32" ? "junction" : "dir");
    await assert.rejects(createFileEvidenceStore({ ...config, directory: linked }).writeEvidence(payload, binding), /not be a link/);
});

test("file verification deadline destroys a stalled stream", { timeout: 2000 }, async t => {
    const { config, store } = await fixture(t);
    const record = await store.writeEvidence(payload, binding);
    const open = fs.open;
    const expire = deadlineAtFault(t);
    let stalled, entered;
    const reached = new Promise(resolve => { entered = resolve; });
    t.mock.method(fs, "open", async (...args) => {
        const handle = await open(...args);
        if (args[0].endsWith(".bin")) {
            stalled = new Readable({ read() { entered(); } });
            t.mock.method(handle, "createReadStream", () => stalled);
        }
        return handle;
    });
    const outcome = createFileEvidenceStore(config).verifyEvidence(record.url);
    const rejected = assert.rejects(outcome, error => error.name === "TimeoutError");
    await reached;
    expire();
    await rejected;
    assert.equal(stalled.destroyed, true);
});

for (const method of ["readEvidence", "verifyEvidence", "readBlob", "findEvidence"])
for (const operation of ["mkdir", "lstat", "realpath", "open", "stat", "close", "failed-stat-close",
    ...(method === "findEvidence" ? ["readdir"] : [])])
test(`file ${method} deadline bounds ${operation} and rejects late verification`, { timeout: 5000 }, async t => {
    const { config, store } = await fixture(t);
    const blob = method === "readBlob";
    const record = blob ? await store.writeBlob(Readable.from([payload]), approved()) : await store.writeEvidence(payload, binding);
    const expire = deadlineAtFault(t);
    let release, entered, closed, handle, streams = 0;
    const stalled = new Promise(resolve => { release = resolve; });
    const reached = new Promise(resolve => { entered = resolve; });
    const cleanup = new Promise(resolve => { closed = resolve; });
    const pause = async () => { entered(); await stalled; };
    const open = fs.open;
    t.mock.method(fs, "open", async (...args) => {
        if (operation === "open") await pause();
        handle = await open(...args);
        const stat = handle.stat.bind(handle), close = handle.close.bind(handle);
        t.mock.method(handle, "stat", async () => {
            if (operation === "stat") await pause();
            if (operation === "failed-stat-close") throw new Error("lost stat");
            return stat();
        });
        t.mock.method(handle, "close", async () => {
            if (["close", "failed-stat-close"].includes(operation)) await pause();
            await close(); closed();
        });
        const stream = handle.createReadStream.bind(handle);
        t.mock.method(handle, "createReadStream", (...args) => { streams++; return stream(...args); });
        return handle;
    });
    if (["mkdir", "lstat", "realpath", "readdir"].includes(operation)) {
        const original = fs[operation];
        t.mock.method(fs, operation, async (...args) => { await pause(); return original(...args); });
    }
    const reader = createFileEvidenceStore({ ...config, timeoutMs: 100 });
    const query = { runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId };
    const outcome = reader[method](method === "findEvidence" ? query : record.url, () => {})
        .then(() => "accepted", error => error.name);
    let timer, result;
    try {
        await reached;
        expire();
        result = await Promise.race([outcome,
            new Promise(resolve => { timer = setTimeout(resolve, 500, "deadline missed"); })]);
    } finally {
        clearTimeout(timer);
        release();
        await outcome;
        // Drain the uncancellable filesystem operation, including its eventual handle cleanup.
        while (handle ? handle.fd !== -1 : !["mkdir", "lstat", "realpath", "readdir"].includes(operation))
            await new Promise(resolve => setImmediate(resolve));
        if (handle) await cleanup;
    }
    assert.equal(result, "TimeoutError");
    assert.equal(await outcome, "TimeoutError");
    assert.equal(streams, operation === "close" ? 1 : 0, "expired opens must not start streaming");
    t.mock.restoreAll();
    assert.deepEqual(await store.verifyEvidence(record.url), {
        bytes: payload.length, sha256: hash(payload), sha512: hash(payload, "sha512"),
    });
});

for (const provider of ["file", "Actions"])
for (const lane of ["evidence", "blob"])
for (const stalledOperation of ["source", "open", "writeFile", "sync", "close", "failed-write-close"]) {
    test(`${provider} ${lane} staging deadline bounds stalled ${stalledOperation}`, { timeout: 5000 }, async t => {
        const { config, send } = await (provider === "file" ? fixture(t) : actionsFixture(t));
        const expire = deadlineAtFault(t);
        let release, entered, cleaned;
        const stalled = new Promise(resolve => { release = resolve; });
        const reached = new Promise(resolve => { entered = resolve; });
        const cleanup = new Promise(resolve => { cleaned = resolve; });
        const open = fs.open;
        t.mock.method(fs, "open", async (...args) => {
            if (!path.basename(args[0]).startsWith(".upload-")) return open(...args);
            if (stalledOperation === "open") { entered(); await stalled; }
            const handle = await open(...args);
            if (["writeFile", "sync", "close", "failed-write-close"].includes(stalledOperation)) {
                const operation = stalledOperation === "failed-write-close" ? "close" : stalledOperation;
                const original = handle[operation].bind(handle);
                t.mock.method(handle, operation, async (...args) => { entered(); await stalled; return original(...args); });
                if (stalledOperation === "writeFile") {
                    const close = handle.close.bind(handle);
                    t.mock.method(handle, "close", async () => { await stalled; return close(); });
                }
                if (stalledOperation === "failed-write-close")
                    t.mock.method(handle, "writeFile", async () => { throw new Error("lost write"); });
            }
            return handle;
        });
        const rm = fs.rm;
        t.mock.method(fs, "rm", async (file, ...args) => {
            const result = await rm(file, ...args);
            if (path.basename(file).startsWith(".upload-")) cleaned();
            return result;
        });
        const source = stalledOperation === "source" ? new Readable({ read() { entered(); } }) : Readable.from([payload]);
        const create = options => provider === "file" ? createFileEvidenceStore(options) : createActionsEvidenceStore(options, { send });
        const store = create({ ...config, timeoutMs: 250 });
        const outcome = (lane === "blob" ? store.writeBlob(source, approved()) : store.writeEvidence(source, binding))
            .then(() => "accepted", error => error.name);
        let timer;
        try {
            await reached;
            expire();
            const result = await Promise.race([outcome,
                new Promise(resolve => { timer = setTimeout(resolve, 1000, "deadline missed"); })]);
            assert.equal(result, "TimeoutError");
            assert.equal(source.destroyed, true);
        } finally {
            clearTimeout(timer);
            release();
            await outcome;
            await cleanup;
        }
        t.mock.restoreAll();
        assert.deepEqual((await fs.readdir(config.directory)).filter(n => !n.startsWith("quota-")), []);
        const replay = create(config);
        assert.deepEqual(await replay.findEvidence({
            runId: config.runId, attempt: config.attempt, buildInstanceId: binding.workId,
        }), []);
        const record = lane === "blob" ? await replay.writeBlob(Readable.from([payload]), approved())
            : await replay.writeEvidence(payload, binding);
        assert.equal((await replay.verifyEvidence(record.url)).sha256, hash(payload));
    });
}

for (const lane of ["evidence", "blob"])
for (const fault of ["rename-before", "rename-after", "verification", "cleanup", "failed-rename-cleanup", "replay-cleanup"])
test(`file ${lane} post-staging deadline preserves a fresh-process retry before late ${fault} completion`, { timeout: 10000 }, async t => {
    const { config, store } = await fixture(t);
    const write = store => lane === "blob" ? store.writeBlob(Readable.from([payload]), approved())
        : store.writeEvidence(payload, binding);
    const existing = fault === "replay-cleanup" ? await write(store) : null;
    const expire = deadlineAtFault(t);
    let entered, release, cleaned, published = false;
    const reached = new Promise(resolve => { entered = resolve; });
    const stalled = new Promise(resolve => { release = resolve; });
    const cleanup = new Promise(resolve => { cleaned = resolve; });
    const pause = async () => { entered(); await stalled; };
    const rename = fs.rename, rm = fs.rm, open = fs.open, removals = [];
    t.mock.method(fs, "rename", async (...args) => {
        if (fault === "rename-before") await pause();
        if (fault === "failed-rename-cleanup") throw new Error("lost rename");
        await rename(...args);
        published = true;
        if (fault === "rename-after") await pause();
    });
    t.mock.method(fs, "open", async (...args) => {
        if (fault === "verification" && published && args[0].endsWith(".bin")) await pause();
        return open(...args);
    });
    t.mock.method(fs, "rm", async (file, ...args) => {
        if (fault.endsWith("cleanup") && path.basename(file).startsWith(".upload-")) await pause();
        const removal = rm(file, ...args);
        removals.push(removal);
        await removal;
        setImmediate(cleaned);
    });
    const outcome = write(createFileEvidenceStore({ ...config, timeoutMs: 100 }))
        .then(() => "accepted", error => error.name);
    let timer, result, record;
    try {
        await reached;
        expire();
        result = await Promise.race([outcome,
            new Promise(resolve => { timer = setTimeout(resolve, 500, "deadline missed"); })]);
        assert.equal(result, "TimeoutError");
        const { stdout } = await promisify(execFile)(process.execPath, ["-e", `
            const {Readable}=require("node:stream");
            const store=require(${JSON.stringify(path.join(__dirname, "evidence-store.cjs"))})
                .createFileEvidenceStore(${JSON.stringify(config)});
            (async()=>{
                const record=await store${lane === "blob"
                    ? `.writeBlob(Readable.from([Buffer.from(${JSON.stringify(payload.toString())})]),${JSON.stringify(approved())})`
                    : `.writeEvidence(Buffer.from(${JSON.stringify(payload.toString())}),${JSON.stringify(binding)})`};
                process.stdout.write(JSON.stringify({record, verified: await store.verifyEvidence(record.url)}));
            })().catch(e=>{console.error(e);process.exit(5)});
        `], { timeout: 5000 });
        const replay = JSON.parse(stdout);
        record = replay.record;
        assert.deepEqual(replay.verified, { bytes: payload.length, sha256: hash(payload), sha512: hash(payload, "sha512") });
        if (existing) assert.deepEqual(record, existing);
    } finally {
        clearTimeout(timer);
        release();
        await outcome;
    }
    assert.equal(result, "TimeoutError");
    assert.equal(await outcome, "TimeoutError");
    assert.equal(AbortSignal.timeout.mock.callCount(), 1, "the complete write must share one deadline");
    try {
        assert.equal(await Promise.race([cleanup.then(() => "cleaned"),
            new Promise(resolve => { timer = setTimeout(resolve, 500, "cleanup missed"); })]), "cleaned");
    } finally { clearTimeout(timer); }
    await Promise.all(removals);
    t.mock.restoreAll();
    const files = (await fs.readdir(config.directory)).filter(n => !n.startsWith("quota-"));
    assert.equal(files.length, 1, "late cleanup must preserve the acknowledged retry");
    assert.deepEqual(await store.verifyEvidence(record.url),
        { bytes: payload.length, sha256: hash(payload), sha512: hash(payload, "sha512") });
    t.diagnostic(JSON.stringify({ lane, fault, result, bytes: payload.length, sha256: hash(payload), realLaunches: 0 }));
});

for (const lane of ["evidence", "blob"])
for (const fault of ["open", "stat", "read", "close", "failed-stat-close", "cleanup", "replay-cleanup"])
test(`Actions ${lane} post-staging deadline bounds ${fault}`, { timeout: 5000 }, async t => {
    const { config, store, state, send } = await actionsFixture(t);
    const write = store => lane === "blob" ? store.writeBlob(Readable.from([payload]), approved())
        : store.writeEvidence(payload, binding);
    const existing = fault === "replay-cleanup" ? await write(store) : null;
    const expire = deadlineAtFault(t);
    let entered, release, cleaned, stream;
    const reached = new Promise(resolve => { entered = resolve; });
    const stalled = new Promise(resolve => { release = resolve; });
    const cleanup = new Promise(resolve => { cleaned = resolve; });
    const pause = async () => { entered(); await stalled; };
    const open = fs.open, rm = fs.rm;
    t.mock.method(fs, "open", async (...args) => {
        if (!path.basename(args[0]).startsWith(".upload-") || args[1] === "wx") return open(...args);
        if (fault === "open") await pause();
        const handle = await open(...args), stat = handle.stat.bind(handle), close = handle.close.bind(handle);
        t.mock.method(handle, "stat", async () => {
            if (fault === "stat") await pause();
            if (fault === "failed-stat-close") throw new Error("lost ZIP stat");
            return stat();
        });
        t.mock.method(handle, "close", async () => {
            if (["close", "failed-stat-close"].includes(fault)) await pause();
            return close();
        });
        if (fault === "read") t.mock.method(handle, "createReadStream", () => {
            stream = Readable.from((async function* () { await pause(); yield payload; })());
            return stream;
        });
        return handle;
    });
    t.mock.method(fs, "rm", async (...args) => {
        if (fault.endsWith("cleanup")) await pause();
        await rm(...args); cleaned();
    });
    const outcome = write(createActionsEvidenceStore({ ...config, timeoutMs: 100 }, { send }))
        .then(() => "accepted", error => error.name);
    let timer, result, calls;
    try {
        await reached;
        expire();
        result = await Promise.race([outcome,
            new Promise(resolve => { timer = setTimeout(resolve, 500, "deadline missed"); })]);
        calls = state.calls.length;
    } finally {
        clearTimeout(timer);
        release();
        await outcome;
        await cleanup;
    }
    assert.equal(result, "TimeoutError");
    assert.equal(await outcome, "TimeoutError");
    assert.equal(state.calls.length, calls, "expired hashing must not start artifact publication");
    if (stream) assert.equal(stream.destroyed, true);
    t.mock.restoreAll();
    assert.deepEqual((await fs.readdir(config.directory)).filter(n => !n.startsWith("quota-")), []);
    const replay = await write(store);
    if (existing) assert.deepEqual(replay, existing);
    assert.equal((await store.verifyEvidence(replay.url)).sha256, hash(payload));
    assert.equal(state.calls.filter(c => c.options.method === "PUT").length, 1);
});

for (const target of ["blob", "metadata"])
test(`HTTP verification deadline cancels a stalled ${target} body`, { timeout: 2000 }, async t => {
    const { config, store, state, send, signed } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    let cancelled = false;
    state.hook = url => (target === "blob" ? url === signed : url.endsWith("/artifacts/87654")) ? new Response(new ReadableStream({
        cancel() { cancelled = true; },
    })) : null;
    await assert.rejects(createActionsEvidenceStore({ ...config, timeoutMs: 20 }, { send }).verifyEvidence(record.url),
        error => error.name === "TimeoutError");
    assert.equal(cancelled, true);
});

test("blob sinks also obey the transfer deadline", { timeout: 2000 }, async t => {
    const { config } = await fixture(t);
    const fileStore = createFileEvidenceStore(config);
    const blob = await fileStore.writeBlob(Readable.from([payload]), approved());
    await assert.rejects(createFileEvidenceStore({ ...config, timeoutMs: 20 }).readBlob(blob.url, () => new Promise(() => {})),
        error => error.name === "TimeoutError");
    for (const timeoutMs of [0, 300001, Infinity]) assert.throws(() => createFileEvidenceStore({ ...config, timeoutMs }), /timeout/);
});
