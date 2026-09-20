const { test } = require("node:test");
const assert = require("node:assert/strict");
const { Readable } = require("node:stream");
const { createHash } = require("node:crypto");
const fs = require("node:fs/promises");
const path = require("node:path");
const os = require("node:os");
const { execFileSync } = require("node:child_process");
const { collectWorkerArchive } = require("./worker-output.cjs");

const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const data = Buffer.from('{"outcome":"untestable","verified":true}');
const files = () => [{ path: "result.json", sha256: hash(data), size: data.length }];

function checksum(header) {
    header.fill(32, 148, 156);
    header.write([...header].reduce((sum, byte) => sum + byte, 0).toString(8).padStart(6, "0") + "\0 ", 148);
}

function entry(name = "result.json", bytes = data, type = "0") {
    const header = Buffer.alloc(512);
    header.write(name, 0, 100);
    for (const [offset, length, value] of [[100, 8, 420], [108, 8, 1000], [116, 8, 1000],
        [124, 12, bytes.length], [136, 12, 0]])
        header.write(value.toString(8).padStart(length - 1, "0") + "\0", offset);
    header.write(type, 156);
    header.write("ustar\0" + "00", 257);
    checksum(header);
    return Buffer.concat([header, bytes, Buffer.alloc((512 - bytes.length % 512) % 512)]);
}

const archive = (...entries) => Buffer.concat([...entries, Buffer.alloc(1024)]);
const collect = (bytes, expected = files()) => collectWorkerArchive(Readable.from([bytes]), expected);

test("collector reads a bounded regular-file tar as inert bytes and hashes every output", async () => {
    const result = await collect(archive(entry()));
    assert.deepEqual(result, [{ ...files()[0], bytes: data }]);
    assert.equal(Object.hasOwn(result[0], "verified"), false);
    result[0].bytes[0] = 0;
    assert.equal(data[0], 123, "returned data must not alias the untrusted stream");
});

test("collector accepts the real platform tar producer without extracting or executing output", async t => {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "worker-output-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    await fs.writeFile(path.join(root, "result.json"), data);
    await fs.writeFile(path.join(root, "inventory.cjs"), 'throw new Error("MUST NOT EXECUTE");');
    const bytes = execFileSync("tar", ["--format", "ustar", "-cf", "-", "-C", root, "result.json", "inventory.cjs"]);
    const inventory = await fs.readFile(path.join(root, "inventory.cjs"));
    const result = await collect(bytes, [...files(), { path: "inventory.cjs", sha256: hash(inventory), size: inventory.length }]);
    assert.deepEqual(result.map(f => f.path), ["result.json", "inventory.cjs"]);
    assert.deepEqual(result[1].bytes, inventory);
});

test("collector rejects hostile paths, duplicates, links and archive extensions", async t => {
    for (const name of ["../result.json", "/result.json", "C:/result.json", "//server/share", "x\\result.json",
        "./result.json", "x/../result.json", "x//result.json", "result.json:stream", "%2e%2e/result.json",
        "NUL", "aux.json", "result.", "x/\u0001.json"])
        await t.test(name, async () => {
            await assert.rejects(collect(archive(entry(name))), /path|entry/);
        });
    for (const type of ["1", "2", "3", "4", "6", "7", "x", "g", "L", "K", "S"])
        await t.test(`type ${type}`, async () => {
            await assert.rejects(collect(archive(entry("result.json", data, type))), /type|regular/);
        });
    for (const name of ["result.json", "RESULT.JSON"]) await t.test(`duplicate ${name}`, async () => {
        await assert.rejects(collect(archive(entry(), entry(name))), /duplicate|unexpected/);
    });
});

test("collector validates complete tar framing, digest, file sizes and a fixed file set", async t => {
    const cases = {
        "bad checksum": b => { b[20] = 45; },
        "bad magic": b => { b.write("other", 257); checksum(b.subarray(0, 512)); },
        "GNU numeric extension": b => { b[124] = 128; checksum(b.subarray(0, 512)); },
        "link destination": b => { b.write("/outside", 157); checksum(b.subarray(0, 512)); },
        "header trailing garbage": b => { b[511] = 1; checksum(b.subarray(0, 512)); },
        "hidden name suffix": b => { b[30] = 65; checksum(b.subarray(0, 512)); },
        "data padding": b => { b[512 + data.length] = 1; },
        "end padding": b => { b[b.length - 1] = 1; },
        "wrong digest": b => { b[512] ^= 1; },
        "wrong size": b => { b.write("00000000001\0", 124); checksum(b.subarray(0, 512)); },
    };
    for (const [name, change] of Object.entries(cases)) await t.test(name, async () => {
        const bytes = archive(entry());
        change(bytes);
        await assert.rejects(collect(bytes), /archive|header|size|digest|padding|name|link|number/);
    });
    for (const cut of [0, 1, 511, 512, 1023, 1024, 1536, 2047]) await t.test(`truncated ${cut}`, async () => {
        await assert.rejects(collect(archive(entry()).subarray(0, cut)), /archive|truncated|terminator/);
    });
    await assert.rejects(collect(archive(entry()), []), /file|expected/);
    await assert.rejects(collect(archive(entry("other.json"))), /unexpected/);
    await assert.rejects(collect(archive()), /missing/);
    await assert.rejects(collect(Buffer.from("gzip or zip is not an accepted transfer")), /archive/);
});

test("collector checks trusted expectations before consuming input", async t => {
    for (const expected of [
        [{ ...files()[0], path: "../escape" }], [{ ...files()[0], verified: true }],
        [{ ...files()[0], size: -1 }], [{ ...files()[0], size: 16 * 1024 * 1024 + 1 }],
        [{ ...files()[0], sha256: "0" }], [...files(), ...files()],
        [...files(), { ...files()[0], path: "RESULT.JSON" }],
        Array.from({ length: 65 }, (_, i) => ({ ...files()[0], path: `file-${i}` })),
    ]) await t.test(JSON.stringify(expected).slice(0, 100), async () => {
        let reads = 0;
        const stream = Readable.from((async function* () { reads++; yield archive(entry()); })());
        await assert.rejects(collectWorkerArchive(stream, expected), /path|file|expected|size|digest|duplicate/);
        assert.equal(reads, 0);
    });
});

test("collector bounds streaming bytes, cancels the source and propagates stream errors", async () => {
    const stream = Readable.from([archive(entry()), Buffer.alloc(64 * 1024)]);
    await assert.rejects(collectWorkerArchive(stream, files()), /archive.*limit/);
    assert.equal(stream.destroyed, true);
    const failure = new Error("truncated upstream transport");
    const broken = Readable.from((async function* () { yield entry(); throw failure; })());
    await assert.rejects(collectWorkerArchive(broken, files()), error => error === failure);
    assert.equal(broken.destroyed, true);
});

test("collector handles arbitrary chunk boundaries and empty files", async () => {
    const empty = { path: "empty.json", sha256: hash(Buffer.alloc(0)), size: 0 };
    const bytes = archive(entry(), entry(empty.path, Buffer.alloc(0)));
    const chunks = Array.from({ length: Math.ceil(bytes.length / 17) }, (_, i) => bytes.subarray(i * 17, (i + 1) * 17));
    const result = await collectWorkerArchive(Readable.from(chunks), [...files(), empty]);
    assert.deepEqual(result[1], { ...empty, bytes: Buffer.alloc(0) });
});

test("collection cancellation and timeout destroy stalled transfers without partial evidence", async t => {
    for (const stage of ["before", "during", "timeout"]) await t.test(stage, async () => {
        const abort = new AbortController();
        const stream = new Readable({ read() {} });
        let timer;
        if (stage === "before") abort.abort(new Error("cancelled"));
        if (stage === "during") timer = setTimeout(() => abort.abort(new Error("cancelled")), 10);
        // AbortSignal's timer is unref'ed, so keep this stalled test alive until its deadline.
        if (stage === "timeout") timer = setTimeout(() => assert.fail("collection did not time out"), 1000);
        try {
            await assert.rejects(collectWorkerArchive(stream, files(), {
                signal: abort.signal, timeoutMs: stage === "timeout" ? 10 : 1000,
            }), /cancelled|timeout/i);
            assert.equal(stream.destroyed, true);
        } finally { clearTimeout(timer); }
    });
    for (const timeoutMs of [0, -1, 300001, Infinity])
        await assert.rejects(collectWorkerArchive(Readable.from([]), files(), { timeoutMs }), /timeout/);
});
