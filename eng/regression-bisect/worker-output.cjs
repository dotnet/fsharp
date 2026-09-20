const { validateRepositoryPath } = require("./model.cjs");
const { hash, requireValue } = require("./toolchain-io.cjs");
const fs = require("node:fs/promises");
const { createHash } = require("node:crypto");

const MAX_FILE_BYTES = 16 * 1024 * 1024;
const MAX_TOTAL_BYTES = 64 * 1024 * 1024;
const padded = size => Math.ceil(size / 512) * 512;

// Only the uncompressed, regular-file USTAR subset is accepted. Nothing is extracted.
async function collectWorkerArchive(stream, files, { signal, timeoutMs = 300000 } = {}) {
    requireValue(Number.isSafeInteger(timeoutMs) && timeoutMs > 0 && timeoutMs <= 300000, "invalid archive timeout");
    signal = AbortSignal.any([AbortSignal.timeout(timeoutMs), ...(signal ? [signal] : [])]);
    requireValue(Array.isArray(files) && files.length > 0 && files.length <= 64, "expected 1..64 worker files");
    const expected = new Map();
    let limit = 10240;
    let total = 0;
    for (const file of files) {
        requireValue(file && Object.keys(file).sort().join(" ") === "path sha256 size", "unexpected worker file fields");
        validateRepositoryPath(file.path);
        requireValue(typeof file.sha256 === "string" && /^[a-f0-9]{64}$/.test(file.sha256), "invalid expected file digest");
        requireValue(Number.isSafeInteger(file.size) && file.size >= 0 && file.size <= MAX_FILE_BYTES, "invalid expected file size");
        const key = file.path.toLowerCase();
        requireValue(!expected.has(key), "duplicate expected worker file");
        expected.set(key, { ...file });
        total += file.size;
        limit += 512 + padded(file.size);
    }
    requireValue(total <= MAX_TOTAL_BYTES, "expected worker files exceed total size limit");
    requireValue(stream && typeof stream[Symbol.asyncIterator] === "function" && typeof stream.destroy === "function",
        "bounded worker archive stream required");
    const chunks = [];
    let length = 0;
    const abort = () => stream.destroy(signal.reason);
    try {
        signal.throwIfAborted();
        signal.addEventListener("abort", abort, { once: true });
        for await (const chunk of stream) {
            signal.throwIfAborted();
            requireValue(Buffer.isBuffer(chunk), "worker archive must contain raw bytes");
            length += chunk.length;
            requireValue(length <= limit, "worker archive exceeds byte limit");
            chunks.push(Buffer.from(chunk));
        }
    } finally {
        signal.removeEventListener("abort", abort);
        stream.destroy();
    }
    signal.throwIfAborted();
    const archive = Buffer.concat(chunks, length);
    requireValue(length >= 1024 && length % 512 === 0, "truncated worker archive");
    const result = [];
    const seen = new Set();
    let offset = 0;
    let terminated = false;
    const string = bytes => {
        const end = bytes.indexOf(0);
        requireValue(bytes.every(b => b < 128) && (end < 0 || bytes.subarray(end).every(b => b === 0)),
            "invalid archive header name or padding");
        return bytes.subarray(0, end < 0 ? bytes.length : end).toString("ascii");
    };
    const number = bytes => {
        requireValue(bytes.every(b => b < 128) && /^ *[0-7]+[\0 ]*$/.test(bytes.toString("ascii")), "invalid archive header number");
        const value = parseInt(bytes.toString("ascii"), 8);
        requireValue(Number.isSafeInteger(value), "invalid archive header number");
        return value;
    };
    while (offset < length) {
        const header = archive.subarray(offset, offset + 512);
        if (header.every(b => b === 0)) {
            requireValue(length - offset >= 1024 && archive.subarray(offset).every(b => b === 0),
                "invalid archive terminator or trailing padding");
            terminated = true;
            break;
        }
        const checksum = number(header.subarray(148, 156));
        requireValue(header.reduce((sum, b, i) => sum + (i >= 148 && i < 156 ? 32 : b), 0) === checksum,
            "archive header checksum mismatch");
        requireValue(header.subarray(257, 265).equals(Buffer.from("ustar\0" + "00")) &&
            header.subarray(500).every(b => b === 0), "unsupported archive header");
        requireValue(header[156] === 48 || header[156] === 0, "worker archive entry must be a regular file");
        requireValue(header.subarray(157, 257).every(b => b === 0), "archive link destination forbidden");
        const name = string(header.subarray(0, 100));
        const prefix = string(header.subarray(345, 500));
        const filePath = prefix ? `${prefix}/${name}` : name;
        validateRepositoryPath(filePath);
        const file = expected.get(filePath.toLowerCase());
        requireValue(file?.path === filePath, "unexpected worker archive entry");
        requireValue(!seen.has(filePath), "duplicate worker archive entry");
        seen.add(filePath);
        const size = number(header.subarray(124, 136));
        requireValue(size === file.size, "worker archive file size mismatch");
        offset += 512;
        requireValue(offset + padded(size) <= length, "truncated worker archive file");
        const bytes = archive.subarray(offset, offset + size);
        requireValue(archive.subarray(offset + size, offset + padded(size)).every(b => b === 0), "invalid archive data padding");
        requireValue(hash(bytes) === file.sha256, "worker archive file digest mismatch");
        result.push({ ...file, bytes: Buffer.from(bytes) });
        offset += padded(size);
    }
    requireValue(terminated, "missing worker archive terminator");
    requireValue(result.length === expected.size, "missing worker archive files");
    return result;
}

const executionFileNames = Object.freeze([
    "result.json", "unit.json", "request.json", "source.json", "declarations.json", "acquisition.json",
    "build-acquisition.json", "product.json", "reproduction.json", "identities.json", "commands.json",
    "global.json", "fsi.runtimeconfig.json", "compiler.runtimeconfig.json",
]);

function validateExecutionFiles(files) {
    requireValue(Array.isArray(files) && files.length === executionFileNames.length &&
        executionFileNames.every(name => files.filter(f => f.path === name).length === 1),
    "fixed execution evidence file set required");
    let total = 0;
    for (const file of files) {
        requireValue(Object.keys(file).sort().join(" ") === "path sha256 size" &&
            /^[a-f0-9]{64}$/.test(file.sha256) && Number.isSafeInteger(file.size) &&
            file.size >= 0 && file.size <= MAX_FILE_BYTES, "invalid execution evidence descriptor");
        total += file.size;
    }
    requireValue(total <= MAX_TOTAL_BYTES, "execution evidence exceeds total size limit");
    return total;
}

// The producer shares the reader's narrow subset: fixed regular files, no tar program or extraction.
async function* workerArchive(files) {
    validateExecutionFiles(files.map(({ path, size, sha256 }) => ({ path, size, sha256 })));
    for (const file of files) {
        requireValue(Buffer.isBuffer(file.bytes) && file.bytes.length === file.size &&
            hash(file.bytes) === file.sha256, "worker evidence changed before export");
        const header = Buffer.alloc(512);
        header.write(file.path);
        for (const [offset, length, value] of [[100, 8, 0o444], [108, 8, 0], [116, 8, 0],
            [124, 12, file.size], [136, 12, 0]])
            header.write(value.toString(8).padStart(length - 1, "0") + "\0", offset);
        header.write("0", 156);
        header.write("ustar\0" + "00", 257);
        header.fill(32, 148, 156);
        header.write(header.reduce((sum, byte) => sum + byte, 0).toString(8).padStart(6, "0") + "\0 ", 148);
        yield header;
        yield file.bytes;
        yield Buffer.alloc(padded(file.size) - file.size);
    }
    yield Buffer.alloc(1024);
}

async function collectWorkerBlob(stream, archive, destination, { signal, timeoutMs = 300000 } = {}) {
    requireValue(Number.isSafeInteger(timeoutMs) && timeoutMs > 0 && timeoutMs <= 300000, "invalid blob timeout");
    const expected = archive?.retained;
    requireValue(expected && Number.isSafeInteger(expected.size) && expected.size > 0 && expected.size <= 1073741824 &&
        /^[a-f0-9]{64}$/.test(expected.sha256) && /^[a-f0-9]{128}$/.test(expected.sha512) &&
        archive.sha256 === expected.sha256 && archive.actualSha512 === expected.sha512 &&
        archive.bytesReceived === expected.size, "invalid bounded archive identity");
    requireValue(stream && typeof stream[Symbol.asyncIterator] === "function" && typeof stream.destroy === "function",
        "bounded archive blob stream required");
    signal = AbortSignal.any([AbortSignal.timeout(timeoutMs), ...(signal ? [signal] : [])]);
    const abort = () => stream.destroy(signal.reason);
    const partial = destination + ".partial";
    let file, size = 0, complete = false;
    let created = false;
    const sha256 = createHash("sha256"), sha512 = createHash("sha512");
    try {
        signal.throwIfAborted();
        file = await fs.open(partial, "wx", 0o600);
        created = true;
        signal.addEventListener("abort", abort, { once: true });
        signal.throwIfAborted();
        for await (const chunk of stream) {
            signal.throwIfAborted();
            requireValue(Buffer.isBuffer(chunk), "archive blob requires raw bytes");
            size += chunk.length;
            requireValue(size <= expected.size, "archive blob exceeds byte limit");
            sha256.update(chunk); sha512.update(chunk);
            await file.writeFile(chunk);
        }
        signal.throwIfAborted();
        requireValue(size === expected.size && sha256.digest("hex") === expected.sha256 &&
            sha512.digest("hex") === expected.sha512, "archive blob digest or size mismatch");
        await file.sync();
        await file.close(); file = null;
        await fs.link(partial, destination);
        await fs.unlink(partial);
        complete = true;
        return { path: destination, size, sha256: expected.sha256, sha512: expected.sha512 };
    } finally {
        signal.removeEventListener("abort", abort);
        stream.destroy();
        await file?.close();
        if (created && !complete) await fs.rm(partial, { force: true });
    }
}

module.exports = { collectWorkerArchive, executionFileNames, validateExecutionFiles, workerArchive, collectWorkerBlob };
