const { createHash, randomUUID } = require("node:crypto");
const fs = require("node:fs/promises");
const { constants } = require("node:fs");
const path = require("node:path");
const { Readable } = require("node:stream");
const { validateArchiveReceipt } = require("./archive-provenance.cjs");
const { fingerprintInput } = require("./model.cjs");
const { requireValue, boundedWait } = require("./toolchain-io.cjs");

const EVIDENCE_LIMIT = 64 * 1024 * 1024;
const BLOB_LIMIT = 1024 * 1024 * 1024;
const LAUNCH_LIMIT = 3 * BLOB_LIMIT;
const TIMEOUT = 300000;
const quotaQueues = new Map();
const RESULTS = "https://results-receiver.actions.githubusercontent.com";
const id = value => typeof value === "string" && /^[1-9]\d{0,19}$/.test(value);
const digest = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);
const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const crcTable = Uint32Array.from({ length: 256 }, (_, n) => {
    for (let i = 0; i < 8; i++) n = n & 1 ? 0xedb88320 ^ (n >>> 1) : n >>> 1;
    return n >>> 0;
});
const crcUpdate = (crc, bytes) => {
    for (const byte of bytes) crc = crcTable[(crc ^ byte) & 255] ^ (crc >>> 8);
    return crc >>> 0;
};

function configuration(config) {
    const c = { ...config };
    requireValue(c.repository === "dotnet/fsharp" && id(c.runId) && Number.isSafeInteger(c.attempt) &&
        c.attempt > 0 && typeof c.directory === "string" && path.isAbsolute(c.directory),
    "invalid trusted evidence store configuration");
    c.timeoutMs ??= TIMEOUT;
    requireValue(Number.isSafeInteger(c.timeoutMs) && c.timeoutMs > 0 && c.timeoutMs <= TIMEOUT,
        "invalid evidence transfer timeout");
    c.directory = path.resolve(c.directory);
    return c;
}

function bindingDigest(binding) {
    requireValue(binding && Object.getPrototypeOf(binding) === Object.prototype &&
        Object.keys(binding).sort().join(" ") === "imageDigest launchId unitDigest workId" &&
        ["workId", "launchId"].every(k => typeof binding[k] === "string" && /^[\w-]{1,128}$/.test(binding[k])) &&
        digest(binding.unitDigest) && digest(binding.imageDigest), "invalid trusted evidence binding");
    return fingerprintInput(binding);
}

function reference(c, info, artifactId) {
    const name = `rb1-${c.runId}-${c.attempt}-w${info.work}-${info.lane}-${info.binding}-${info.sha256}-${info.bytes}`;
    return { url: `https://github.com/${c.repository}/actions/runs/${c.runId}/attempts/${c.attempt}` +
        `/artifacts/${artifactId}?name=${name}&sha256=${info.sha256}`, sha256: info.sha256, required: true };
}

function parseReference(url, c) {
    requireValue(typeof url === "string" && url.length < 1024, "invalid evidence URL");
    const match = /^https:\/\/github\.com\/dotnet\/fsharp\/actions\/runs\/([1-9]\d{0,19})\/attempts\/([1-9]\d*)\/artifacts\/([1-9]\d{0,19})\?name=(rb1-([1-9]\d{0,19})-([1-9]\d*)-w([a-f0-9]{64})-(e|b)-([a-f0-9]{64})-([a-f0-9]{64})-(0|[1-9]\d*))&sha256=([a-f0-9]{64})$/.exec(url);
    requireValue(match && match[1] === match[5] && match[2] === match[6] && match[10] === match[12],
        "invalid controlled evidence URL binding");
    const [, runId, attempt, artifactId, name, , , work, lane, binding, sha256, size] = match;
    const bytes = Number(size);
    requireValue(Number.isSafeInteger(Number(attempt)) && Number.isSafeInteger(bytes) &&
        bytes <= (lane === "b" ? BLOB_LIMIT : EVIDENCE_LIMIT), "invalid evidence URL size");
    return { runId, attempt: Number(attempt), artifactId, name, work, lane, binding, sha256, bytes,
        file: path.join(c.directory, `${name}.bin`) };
}

function validateEvidenceUrl(url) {
    parseReference(url, { directory: __dirname });
    return true;
}

function isControlledEvidenceUrl(url) {
    try { return validateEvidenceUrl(url); }
    catch { return false; }
}

function discoveryPrefix(query) {
    requireValue(query && Object.keys(query).sort().join(" ") === "attempt buildInstanceId runId" &&
        id(query.runId) && Number.isSafeInteger(query.attempt) && query.attempt > 0 &&
        typeof query.buildInstanceId === "string" && /^[\w-]{1,128}$/.test(query.buildInstanceId),
    "invalid trusted evidence discovery binding");
    return `rb1-${query.runId}-${query.attempt}-w${hash(query.buildInstanceId)}-`;
}

function discoveredRecord(c, query, name, artifactId) {
    const sha256 = name.split("-").at(-2);
    const url = `https://github.com/${c.repository}/actions/runs/${query.runId}/attempts/${query.attempt}` +
        `/artifacts/${artifactId}?name=${name}&sha256=${sha256}`;
    parseReference(url, c);
    return { url, sha256, required: true };
}

async function root(c) {
    await fs.mkdir(c.directory, { recursive: true, mode: 0o700 });
    const stat = await fs.lstat(c.directory);
    requireValue(stat.isDirectory() && !stat.isSymbolicLink() &&
        path.relative(await fs.realpath(c.directory), c.directory) === "", "evidence directory must not be a link");
}

async function openRegular(file) {
    const before = await fs.lstat(file);
    requireValue(before.isFile() && !before.isSymbolicLink() && before.nlink === 1, "evidence must be an unlinked regular file");
    const handle = await fs.open(file, constants.O_RDONLY | (constants.O_NOFOLLOW ?? 0));
    try {
        const after = await handle.stat();
        requireValue(after.isFile() && after.nlink === 1 && before.dev === after.dev && before.ino === after.ino,
            "evidence regular file identity changed");
        return handle;
    } catch (error) { await handle.close(); throw error; }
}

// The controller's whole-issue lock serializes processes; this queue serializes local callers.
// Durable reservations survive process loss and failed transfers never refund the quota.
async function reserveBlob(c, binding, receipt) {
    const launch = hash(`${c.runId}:${c.attempt}:${binding.launchId}`);
    const prefix = `quota-${launch}-`;
    const marker = `${prefix}${receipt.sha256}-${receipt.bytesReceived}`;
    const key = path.join(c.directory, prefix).toLowerCase();
    const pending = (quotaQueues.get(key) ?? Promise.resolve()).then(async () => {
        const names = (await fs.readdir(c.directory)).filter(n => n.startsWith(prefix) && n !== `${prefix}lock`);
        let total = 0;
        for (const name of names) {
            const parts = /^([a-f0-9]{64})-([1-9]\d*)$/.exec(name.slice(prefix.length));
            requireValue(parts && Number(parts[2]) <= BLOB_LIMIT, "corrupt durable blob quota");
            total += Number(parts[2]);
        }
        requireValue(total + (names.includes(marker) ? 0 : receipt.bytesReceived) <= LAUNCH_LIMIT, "launch blob byte limit exceeded");
        if (!names.includes(marker)) {
            const handle = await fs.open(path.join(c.directory, marker), "wx", 0o600);
            try { await handle.sync(); } finally { await handle.close(); }
        }
    });
    const settled = pending.catch(() => {});
    quotaQueues.set(key, settled);
    try { await pending; }
    finally { if (quotaQueues.get(key) === settled) quotaQueues.delete(key); }
}

async function stage(c, source, binding, blob) {
    const stream = Buffer.isBuffer(source) ? Readable.from([source]) : source;
    requireValue(stream && typeof stream[Symbol.asyncIterator] === "function" &&
        typeof stream.destroy === "function", "bounded raw evidence stream required");
    const signal = AbortSignal.timeout(c.timeoutMs);
    const abort = () => stream.destroy(signal.reason);
    const sha256 = createHash("sha256"), sha512 = createHash("sha512");
    let bytes = 0, crc = 0xffffffff, complete = false, file, handle;
    try {
        const bound = bindingDigest(binding);
        binding = { ...binding };
        if (blob) blob = structuredClone(blob);
        await root(c);
        if (blob) {
            validateArchiveReceipt(blob.request, blob.receipt,
                path.win32.parse(blob.receipt.retained?.path ?? "").root.length > 1 ? path.win32 : path.posix);
            await reserveBlob(c, binding, blob.receipt);
        }
        file = path.join(c.directory, `.upload-${randomUUID()}`);
        handle = await fs.open(file, "wx", 0o600);
        signal.throwIfAborted();
        signal.addEventListener("abort", abort, { once: true });
        for await (const chunk of stream) {
            signal.throwIfAborted();
            requireValue(Buffer.isBuffer(chunk), "evidence stream must contain raw bytes");
            bytes += chunk.length;
            requireValue(bytes <= (blob ? blob.receipt.bytesReceived : EVIDENCE_LIMIT), "evidence byte limit exceeded");
            sha256.update(chunk); sha512.update(chunk); crc = crcUpdate(crc, chunk);
            await boundedWait(handle.writeFile(chunk), signal);
        }
        signal.throwIfAborted();
        const info = { bytes, sha256: sha256.digest("hex"), sha512: sha512.digest("hex"), crc: (crc ^ 0xffffffff) >>> 0,
            work: hash(binding.workId), binding: bound, lane: blob ? "b" : "e", file };
        if (blob) requireValue(info.bytes === blob.receipt.bytesReceived && info.sha256 === blob.receipt.sha256 &&
            info.sha512 === blob.receipt.actualSha512, "retained archive byte count or checksum mismatch");
        await boundedWait(handle.sync(), signal);
        complete = true;
        return info;
    } finally {
        signal.removeEventListener("abort", abort);
        stream.destroy();
        if (handle) await handle.close();
        if (file && !complete) await fs.rm(file, { force: true });
    }
}

async function consume(stream, maxBytes, sink, signal) {
    const iterator = stream[Symbol.asyncIterator]();
    let finished = false;
    const abort = () => stream.destroy?.(signal.reason);
    let bytes = 0;
    try {
        signal.throwIfAborted();
        signal.addEventListener("abort", abort, { once: true });
        while (true) {
            const next = await boundedWait(iterator.next(), signal);
            if (next.done) { finished = true; break; }
            const chunk = next.value;
            requireValue(Buffer.isBuffer(chunk) || chunk instanceof Uint8Array, "evidence response must contain bytes");
            bytes += chunk.length;
            requireValue(bytes <= maxBytes, "evidence response exceeds byte limit");
            await boundedWait(sink(chunk), signal);
        }
        signal.throwIfAborted();
        return bytes;
    } finally {
        signal.removeEventListener("abort", abort);
        if (!finished) {
            stream.destroy?.();
            iterator.return?.().catch(() => {});
        }
    }
}

async function measure(stream, info, sink = () => {}, signal) {
    const sha256 = createHash("sha256"), sha512 = createHash("sha512");
    const bytes = await consume(stream, info.bytes, chunk => {
        sha256.update(chunk); sha512.update(chunk);
        return sink(chunk);
    }, signal);
    const result = { bytes, sha256: sha256.digest("hex"), sha512: sha512.digest("hex") };
    requireValue(bytes === info.bytes && result.sha256 === info.sha256, "evidence size or digest mismatch");
    return result;
}

function readers(c, read) {
    return {
        async readEvidence(url) {
            const info = parseReference(url, c);
            requireValue(info.lane === "e", "archive blob requires streaming verification");
            const chunks = [];
            const result = await read(info, chunk => chunks.push(Buffer.from(chunk)));
            return result === null ? null : Buffer.concat(chunks, result.bytes);
        },
        async verifyEvidence(url) { return read(parseReference(url, c)); },
        // Sink receives untrusted data only; consumers must wait for the final verified result.
        async readBlob(url, sink) {
            const info = parseReference(url, c);
            requireValue(info.lane === "b" && typeof sink === "function", "bounded blob sink required");
            return read(info, sink);
        },
    };
}

function createFileEvidenceStore(config) {
    const c = configuration(config);
    const read = async (info, sink) => {
        await root(c);
        const expectedId = (BigInt(`0x${hash(info.name).slice(0, 15)}`) + 1n).toString();
        requireValue(info.artifactId === expectedId, "file artifact identity mismatch");
        let handle;
        try { handle = await openRegular(info.file); }
        catch (error) { if (error.code === "ENOENT") return null; throw error; }
        try { return await measure(handle.createReadStream({ autoClose: false }), info, sink, AbortSignal.timeout(c.timeoutMs)); }
        finally { await handle.close(); }
    };
    const write = async (source, binding, blob) => {
        const info = await stage(c, source, binding, blob);
        const provisional = parseReference(reference(c, info, "1").url, c);
        const artifactId = (BigInt(`0x${hash(provisional.name).slice(0, 15)}`) + 1n).toString();
        const record = reference(c, info, artifactId);
        try {
            // The whole-issue lock protects this check/rename. A crash leaves either
            // unpublished staging bytes or one complete, discoverable regular file.
            if (await read(parseReference(record.url, c)) === null)
                await fs.rename(info.file, provisional.file);
            await read(parseReference(record.url, c));
            return record;
        } finally { await fs.rm(info.file, { force: true }); }
    };
    return { ...readers(c, read),
        async findEvidence(query) {
            const prefix = `${discoveryPrefix(query)}e-`;
            await root(c);
            const names = await fs.readdir(c.directory);
            requireValue(names.length <= 10000, "file evidence discovery limit exceeded");
            const candidates = names.filter(n => n.startsWith(prefix) && n.endsWith(".bin")).sort();
            requireValue(candidates.length <= 16, "work evidence discovery limit exceeded");
            const result = [];
            for (const file of candidates) {
                const name = file.slice(0, -4);
                const artifactId = (BigInt(`0x${hash(name).slice(0, 15)}`) + 1n).toString();
                const record = discoveredRecord(c, query, name, artifactId);
                if (await read(parseReference(record.url, c)) !== null) result.push(record);
            }
            return result;
        },
        writeEvidence: (bytes, binding) => write(bytes, binding),
        writeBlob: (bytes, { binding, request, receipt }) => write(bytes, binding, { request, receipt }) };
}

// The sole supported ZIP is our single STORED regular file. No paths are extracted.
function zipParts(size, crc) {
    const name = Buffer.from("evidence.bin");
    const header = Buffer.alloc(30 + name.length);
    header.writeUInt32LE(0x04034b50); header.writeUInt16LE(20, 4);
    header.writeUInt32LE(crc, 14); header.writeUInt32LE(size, 18); header.writeUInt32LE(size, 22);
    header.writeUInt16LE(name.length, 26); name.copy(header, 30);
    const tail = Buffer.alloc(46 + name.length + 22);
    tail.writeUInt32LE(0x02014b50); tail.writeUInt16LE(0x314, 4); tail.writeUInt16LE(20, 6);
    tail.writeUInt32LE(crc, 16); tail.writeUInt32LE(size, 20); tail.writeUInt32LE(size, 24);
    tail.writeUInt16LE(name.length, 28); tail.writeUInt32LE(0x81800000, 38); name.copy(tail, 46);
    const end = 46 + name.length;
    tail.writeUInt32LE(0x06054b50, end); tail.writeUInt16LE(1, end + 8); tail.writeUInt16LE(1, end + 10);
    tail.writeUInt32LE(end, end + 12); tail.writeUInt32LE(header.length + size, end + 16);
    return { header, tail };
}

async function* zipStream(info) {
    const { header, tail } = zipParts(info.bytes, info.crc);
    const handle = await openRegular(info.file);
    try { yield header; yield* handle.createReadStream({ autoClose: false }); yield tail; }
    finally { await handle.close(); }
}

async function* unzip(stream, info, zipDigest) {
    let prefix = Buffer.alloc(0), suffix = Buffer.alloc(0), offset = 0, crc = 0xffffffff;
    const sha = createHash("sha256");
    for await (const raw of stream) {
        const chunk = Buffer.from(raw);
        sha.update(chunk);
        const end = offset + chunk.length;
        requireValue(end <= info.bytes + 122, "artifact ZIP exceeds byte limit");
        if (offset < 42) prefix = Buffer.concat([prefix, chunk.subarray(0, Math.min(42 - offset, chunk.length))]);
        if (end > 42 && offset < info.bytes + 42) {
            const data = chunk.subarray(Math.max(0, 42 - offset), Math.min(chunk.length, info.bytes + 42 - offset));
            crc = crcUpdate(crc, data);
            yield data;
        }
        if (end > info.bytes + 42) suffix = Buffer.concat([suffix, chunk.subarray(Math.max(0, info.bytes + 42 - offset))]);
        offset = end;
    }
    const expected = zipParts(info.bytes, (crc ^ 0xffffffff) >>> 0);
    requireValue(offset === info.bytes + 122 && prefix.equals(expected.header) && suffix.equals(expected.tail) &&
        sha.digest("hex") === zipDigest, "invalid data-only artifact ZIP or digest");
}

function signedBlobUrl(value) {
    let url;
    try { url = new URL(value); } catch { throw new Error("invalid artifact signed URL"); }
    requireValue(url.href === value && url.protocol === "https:" && !url.port && !url.username && !url.password && !url.hash &&
        /^[a-z0-9]{3,24}\.blob\.core\.windows\.net$/.test(url.hostname) &&
        /^\/[a-zA-Z0-9_-]+\/[a-zA-Z0-9_./-]+$/.test(url.pathname) &&
        !url.pathname.includes("//") && !url.pathname.split("/").some(p => p === "." || p === "..") &&
        url.searchParams.has("sig") && url.searchParams.get("sr") === "b" &&
        [...url.searchParams.keys()].length === new Set(url.searchParams.keys()).size,
    "forbidden artifact signed URL origin or object");
    return url.href;
}

function createActionsEvidenceStore(config, { send = fetch } = {}) {
    const c = configuration(config);
    requireValue(typeof c.token === "string" && c.token.length > 0 && !/\s/.test(c.token), "trusted artifact read credential required");
    const apiBase = `https://api.github.com/repos/${c.repository}`;
    const auth = { Authorization: `Bearer ${c.token}`, Accept: "application/vnd.github+json",
        "X-GitHub-Api-Version": "2022-11-28" };
    const request = async (url, options = {}) => {
        const response = await send(url, { redirect: "manual", signal: AbortSignal.timeout(c.timeoutMs),
            ...options, headers: { "User-Agent": "dotnet-fsharp-regression-controller", "Accept-Encoding": "identity", ...options.headers } });
        try {
            requireValue(!response.redirected && (!response.url || response.url === url), "unexpected artifact response redirect");
            return response;
        } catch (error) { await close(response); throw error; }
    };
    const close = async response => {
        if (response.body && !response.body.locked) await response.body.cancel();
    };
    const json = async response => {
        try {
            requireValue(response.status === 200 && [null, "identity"].includes(response.headers.get("content-encoding")),
                `artifact metadata request failed (${response.status})`);
            const chunks = [];
            const declared = response.headers.get("content-length");
            requireValue(declared === null || /^\d+$/.test(declared) && Number(declared) <= 1024 * 1024,
                "artifact metadata content length exceeds byte limit");
            requireValue(response.body, "missing artifact metadata");
            const raw = Readable.fromWeb(response.body);
            let length;
            try {
                length = await consume(raw, 1024 * 1024, chunk => chunks.push(Buffer.from(chunk)), AbortSignal.timeout(c.timeoutMs));
            } finally { raw.destroy(); }
            requireValue(declared === null || length === Number(declared), "artifact metadata content length mismatch");
            return JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(Buffer.concat(chunks)));
        } finally { await close(response); }
    };
    const run = async info => {
        const data = await json(await request(`${apiBase}/actions/runs/${info.runId}/attempts/${info.attempt}`, { headers: auth }));
        requireValue(String(data.id) === info.runId && data.run_attempt === info.attempt &&
            data.repository?.full_name === c.repository && data.head_repository?.full_name === c.repository &&
            Number.isSafeInteger(data.repository.id) && data.repository.id > 0 &&
            data.head_repository.id === data.repository.id, "artifact workflow run identity mismatch");
        return data;
    };
    const metadata = async info => {
        const owner = await run(info);
        const response = await request(`${apiBase}/actions/artifacts/${info.artifactId}`, { headers: auth });
        if ([404, 410].includes(response.status)) { await close(response); return null; }
        const data = await json(response);
        requireValue(String(data.id) === info.artifactId && data.name === info.name &&
            String(data.workflow_run?.id) === info.runId && data.workflow_run.repository_id === owner.repository.id &&
            data.workflow_run.head_repository_id === owner.repository.id &&
            data.size_in_bytes === info.bytes + 122 && /^sha256:[a-f0-9]{64}$/.test(data.digest) &&
            typeof data.expired === "boolean" && Number.isFinite(Date.parse(data.expires_at)) &&
            data.archive_download_url === `${apiBase}/actions/artifacts/${info.artifactId}/zip`,
        "artifact metadata ownership, identity or size mismatch");
        return data.expired ? null : data;
    };
    const read = async (info, sink) => {
        const meta = await metadata(info);
        if (!meta) return null;
        const redirect = await request(meta.archive_download_url, { headers: auth });
        let url;
        try {
            if ([404, 410].includes(redirect.status)) {
                requireValue(await metadata(info) === null, "artifact download missing without confirmed expiry");
                return null;
            }
            requireValue(redirect.status === 302, `artifact download request failed (${redirect.status})`);
            url = signedBlobUrl(redirect.headers.get("location"));
        } finally { await close(redirect); }
        // The GitHub-authenticated response selects this exact blob, not a worker or caller.
        const signal = AbortSignal.timeout(c.timeoutMs);
        const response = await request(url, { signal });
        try {
            requireValue(response.status === 200 && [null, "identity"].includes(response.headers.get("content-encoding")) &&
                (response.headers.get("content-length") === null ||
                    response.headers.get("content-length") === String(info.bytes + 122)) && response.body,
            `artifact blob request failed or exceeds byte limit (${response.status})`);
            const raw = Readable.fromWeb(response.body);
            try { return await measure(unzip(raw, info, meta.digest.slice(7)), info, sink, signal); }
            finally { raw.destroy(); }
        } finally { await close(response); }
    };
    const write = async (source, binding, blob) => {
        requireValue((c.resultsUrl ?? RESULTS) === RESULTS || c.resultsUrl === `${RESULTS}/`, "forbidden Actions results authority");
        requireValue(typeof c.runtimeToken === "string" && c.runtimeToken.length < 32768 && !/\s/.test(c.runtimeToken),
            "trusted Actions runtime credential required");
        const claims = JSON.parse(Buffer.from(c.runtimeToken.split(".")[1] ?? "", "base64url").toString("utf8"));
        const scopes = typeof claims.scp === "string" ? claims.scp.split(" ").filter(s => s.startsWith("Actions.Results:")) : [];
        requireValue(scopes.length === 1 && /^Actions\.Results:[a-f0-9-]{36}:[a-f0-9-]{36}$/.test(scopes[0]),
            "invalid Actions runtime backend binding");
        const [, workflowRunBackendId, workflowJobRunBackendId] = scopes[0].split(":");
        await run(c);
        const info = await stage(c, source, binding, blob);
        const name = parseReference(reference(c, info, "1").url, c).name;
        const rpc = async (method, fields) => json(await request(`${RESULTS}/twirp/github.actions.results.api.v1.ArtifactService/${method}`,
            { method: "POST", headers: { Authorization: `Bearer ${c.runtimeToken}`, "Content-Type": "application/json" },
                body: JSON.stringify({ workflow_run_backend_id: workflowRunBackendId,
                    workflow_job_run_backend_id: workflowJobRunBackendId, name, ...fields }) }));
        try {
            const zipHash = createHash("sha256");
            for await (const chunk of zipStream(info)) zipHash.update(chunk);
            const zipDigest = zipHash.digest("hex");
            const existing = await json(await request(`${apiBase}/actions/runs/${c.runId}/artifacts?name=${name}&per_page=100`,
                { headers: auth }));
            requireValue(Number.isSafeInteger(existing.total_count) && existing.total_count >= 0 && existing.total_count <= 1 &&
                Array.isArray(existing.artifacts) && existing.artifacts.length === existing.total_count,
            "ambiguous artifact replay identity");
            if (existing.total_count === 1) {
                requireValue(id(String(existing.artifacts[0].id)), "invalid replay artifact identity");
                const record = reference(c, info, String(existing.artifacts[0].id));
                const stored = await metadata(parseReference(record.url, c));
                requireValue(stored?.digest === `sha256:${zipDigest}`, "replayed artifact expired or digest mismatch");
                return record;
            }
            const created = await rpc("CreateArtifact", { version: 4,
                expires_at: new Date(Date.now() + 90 * 86400000).toISOString() });
            requireValue(created.ok === true, "artifact creation failed");
            const url = signedBlobUrl(created.signedUploadUrl ?? created.signed_upload_url);
            const upload = Readable.from(zipStream(info));
            let response;
            try {
                response = await request(url, { method: "PUT", duplex: "half", body: upload, headers: {
                    "x-ms-blob-type": "BlockBlob", "x-ms-version": "2023-11-03",
                    "Content-Type": "application/zip", "Content-Length": String(info.bytes + 122),
                } });
                requireValue(response.status === 201, `artifact upload failed (${response.status})`);
            } finally { upload.destroy(); if (response) await close(response); }
            const finalized = await rpc("FinalizeArtifact", { size: String(info.bytes + 122), hash: `sha256:${zipDigest}` });
            const artifactId = finalized.artifactId ?? finalized.artifact_id;
            requireValue(finalized.ok === true && id(artifactId), "artifact finalization identity missing");
            const record = reference(c, info, artifactId);
            const stored = await metadata(parseReference(record.url, c));
            requireValue(stored?.digest === `sha256:${zipDigest}`, "uploaded artifact metadata digest mismatch");
            return record;
        } finally { await fs.rm(info.file, { force: true }); }
    };
    return { ...readers(c, read),
        async findEvidence(query) {
            const prefix = `${discoveryPrefix(query)}e-`;
            await run(query);
            const candidates = [], seen = new Set();
            let total;
            for (let page = 1; page <= 5; page++) {
                const list = await json(await request(`${apiBase}/actions/runs/${query.runId}/artifacts?per_page=100&page=${page}`,
                    { headers: auth }));
                total ??= list.total_count;
                requireValue(Number.isSafeInteger(total) && total >= 0 && total <= 500 && list.total_count === total &&
                    Array.isArray(list.artifacts) && list.artifacts.length === Math.min(100, total - seen.size),
                "incomplete or oversized artifact discovery");
                for (const artifact of list.artifacts) {
                    requireValue(id(String(artifact.id)) && !seen.has(String(artifact.id)) &&
                        typeof artifact.name === "string", "duplicate or malformed artifact discovery");
                    seen.add(String(artifact.id));
                    if (artifact.name.startsWith(prefix)) candidates.push(artifact);
                }
                if (seen.size === total) break;
            }
            requireValue(candidates.length <= 16, "work evidence discovery limit exceeded");
            const result = [];
            for (const artifact of candidates.sort((a, b) => a.name.localeCompare(b.name))) {
                const record = discoveredRecord(c, query, artifact.name, String(artifact.id));
                if (await read(parseReference(record.url, c)) !== null) result.push(record);
            }
            return result;
        },
        writeEvidence: (bytes, binding) => write(bytes, binding).finally(() => bytes?.destroy?.()),
        writeBlob: (bytes, { binding, request, receipt }) => write(bytes, binding, { request, receipt })
            .finally(() => bytes?.destroy?.()) };
}

module.exports = { createFileEvidenceStore, createActionsEvidenceStore, validateEvidenceUrl, isControlledEvidenceUrl };
