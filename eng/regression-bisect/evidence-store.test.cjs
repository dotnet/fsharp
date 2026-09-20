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

test("killing the quota writer cannot leave a lock that blocks a new-process retry", { timeout: 10000 }, async t => {
    const { config } = await fixture(t);
    const input = approved();
    const modulePath = path.join(__dirname, "evidence-store.cjs");
    const child = spawn(process.execPath, ["-e", `
        const fs=require("node:fs/promises"), path=require("node:path"), {Readable}=require("node:stream");
        const open=fs.open;
        fs.open=async function(file,...args) {
            const handle=await open.call(this,file,...args);
            if(path.basename(file).startsWith("quota-")) {
                process.send("reservation-created"); setInterval(()=>{},1000);
                await new Promise(()=>{});
            }
            return handle;
        };
        require(${JSON.stringify(modulePath)}).createFileEvidenceStore(${JSON.stringify(config)})
            .writeBlob(Readable.from([Buffer.from(${JSON.stringify(payload.toString())})]),${JSON.stringify(input)})
            .catch(e=>{console.error(e);process.exit(5)});
    `], { stdio: ["ignore", "ignore", "pipe", "ipc"] });
    t.after(() => { if (child.exitCode === null) child.kill("SIGKILL"); });
    const exited = once(child, "exit");
    assert.deepEqual(await once(child, "message"), ["reservation-created", undefined]);
    child.kill("SIGKILL");
    await exited;
    const { stdout } = await promisify(execFile)(process.execPath, ["-e", `
        const {Readable}=require("node:stream");
        require(${JSON.stringify(modulePath)}).createFileEvidenceStore(${JSON.stringify(config)})
            .writeBlob(Readable.from([Buffer.from(${JSON.stringify(payload.toString())})]),${JSON.stringify(input)})
            .then(r=>process.stdout.write(JSON.stringify(r))).catch(e=>{console.error(e);process.exit(5)});
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
    let stalled;
    t.mock.method(fs, "open", async (...args) => {
        const handle = await open(...args);
        if (args[0].endsWith(".bin")) {
            stalled = new Readable({ read() {} });
            t.mock.method(handle, "createReadStream", () => stalled);
        }
        return handle;
    });
    await assert.rejects(createFileEvidenceStore({ ...config, timeoutMs: 20 }).verifyEvidence(record.url),
        error => error.name === "TimeoutError");
    assert.equal(stalled.destroyed, true);
});

for (const stalledOperation of ["source", "writeFile", "sync"]) {
    test(`staging deadline bounds stalled ${stalledOperation}`, { timeout: 2000 }, async t => {
        const { config } = await fixture(t);
        let release;
        const stalled = new Promise(resolve => { release = resolve; });
        const open = fs.open;
        t.mock.method(fs, "open", async (...args) => {
            const handle = await open(...args);
            if (path.basename(args[0]).startsWith(".upload-") && stalledOperation !== "source")
                t.mock.method(handle, stalledOperation, () => stalled);
            return handle;
        });
        const source = stalledOperation === "source" ? new Readable({ read() {} }) : Readable.from([payload]);
        const outcome = createFileEvidenceStore({ ...config, timeoutMs: 20 }).writeEvidence(source, binding)
            .then(() => "accepted", error => error.name);
        let timer;
        try {
            const result = await Promise.race([outcome,
                new Promise(resolve => { timer = setTimeout(resolve, 300, "deadline missed"); })]);
            assert.equal(result, "TimeoutError");
            assert.equal(source.destroyed, true);
            assert.deepEqual(await fs.readdir(config.directory), []);
        } finally {
            clearTimeout(timer);
            release();
            await outcome;
        }
    });
}

test("HTTP verification deadline cancels a stalled response body", { timeout: 2000 }, async t => {
    const { config, store, state, send, signed } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    let cancelled = false;
    state.hook = url => url === signed ? new Response(new ReadableStream({
        cancel() { cancelled = true; },
    })) : null;
    await assert.rejects(createActionsEvidenceStore({ ...config, timeoutMs: 20 }, { send }).verifyEvidence(record.url),
        error => error.name === "TimeoutError");
    assert.equal(cancelled, true);
});

test("metadata bodies and blob sinks also obey the transfer deadline", { timeout: 2000 }, async t => {
    const { config, store, state, send } = await actionsFixture(t);
    const record = await store.writeEvidence(payload, binding);
    let cancelled = false;
    state.hook = url => url.endsWith("/artifacts/87654") ? new Response(new ReadableStream({
        cancel() { cancelled = true; },
    })) : null;
    await assert.rejects(createActionsEvidenceStore({ ...config, timeoutMs: 20 }, { send }).verifyEvidence(record.url),
        error => error.name === "TimeoutError");
    assert.equal(cancelled, true);
    const fileStore = createFileEvidenceStore(config);
    const blob = await fileStore.writeBlob(Readable.from([payload]), approved());
    await assert.rejects(createFileEvidenceStore({ ...config, timeoutMs: 20 }).readBlob(blob.url, () => new Promise(() => {})),
        error => error.name === "TimeoutError");
    for (const timeoutMs of [0, 300001, Infinity]) assert.throws(() => createFileEvidenceStore({ ...config, timeoutMs }), /timeout/);
});
