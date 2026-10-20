const { test } = require("node:test");
const assert = require("node:assert/strict");
const net = require("node:net");
const { once, EventEmitter } = require("node:events");
const { createEgressBroker, publicAddress, limits } = require("./egress.cjs");

async function fixture(t, options = {}, serve = socket => socket.pipe(socket)) {
    const events = [], resolutions = [], connections = [], admissions = new EventEmitter();
    const target = net.createServer(serve);
    const sockets = new Set();
    target.on("connection", socket => {
        sockets.add(socket);
        socket.on("close", () => sockets.delete(socket));
    });
    target.listen(0, "127.0.0.1");
    await once(target, "listening");
    let broker;
    t.after(async () => {
        await broker?.close();
        for (const socket of sockets) socket.destroy();
        await new Promise(resolve => target.close(resolve));
        t.diagnostic(JSON.stringify({ transport: "owned-loopback-with-injected-DNS-and-dialer",
            resolutions, connections, events, final: broker?.snapshot() }));
    });
    broker = createEgressBroker({
        audit: event => {
            events.push(event);
            if (["queued", "denied"].includes(event.event)) admissions.emit("event", event);
        },
        resolve: async (host, options) => {
            resolutions.push({ host, options });
            return [{ address: "13.107.246.40", family: 4 }];
        },
        connect: options => {
            connections.push(options);
            return net.connect({ host: "127.0.0.1", port: target.address().port });
        },
        ...options,
    });
    broker.server.listen(0, "127.0.0.1");
    await once(broker.server, "listening");
    const port = broker.server.address().port;
    const request = async (authority = "api.nuget.org:443", headers = "", head = "", host = authority) => {
        const socket = net.connect({ host: "127.0.0.1", port });
        socket.on("error", () => {});
        await once(socket, "connect");
        socket.write(`CONNECT ${authority} HTTP/1.1\r\nHost: ${host}\r\n${headers}\r\n${head}`);
        return socket;
    };
    return { broker, request, events, resolutions, connections, target, admissions };
}

async function queuedRequest(f, ...args) {
    const event = once(f.admissions, "event");
    const socket = await f.request(...args);
    assert.equal((await event)[0].event, "queued", "a permitted burst must wait without starting DNS or returning 503");
    return socket;
}

async function response(socket) {
    const chunks = [];
    for await (const chunk of socket) chunks.push(chunk);
    return Buffer.concat(chunks).toString();
}

test("only globally routable IPv4 addresses can be dialed", () => {
    for (const ip of ["0.0.0.0", "0.1.2.3", "10.0.0.1", "100.64.0.1", "100.127.255.254",
        "127.0.0.1", "169.254.169.254", "172.16.0.1", "172.31.255.254", "192.0.0.9",
        "192.0.2.1", "192.88.99.1", "192.168.1.1", "198.18.0.1", "198.19.255.254",
        "198.51.100.1", "203.0.113.1", "224.0.0.1", "240.0.0.1", "255.255.255.255",
        "::1", "::ffff:13.107.246.40", "2606:4700::1111", "2130706433", "127.1", "0177.0.0.1"])
        assert.equal(publicAddress(ip), false, ip);
    for (const ip of ["13.107.246.40", "20.42.64.1", "100.63.255.254", "100.128.0.1", "172.32.0.1"])
        assert.equal(publicAddress(ip), true, ip);
});

test("a legitimate bounded tunnel pins the public address and transfers bytes", async t => {
    const f = await fixture(t);
    const socket = await f.request();
    assert.match((await once(socket, "data"))[0].toString(), /^HTTP\/1.1 200 Connection Established\r\n\r\n$/);
    socket.write("bounded public download");
    assert.equal((await once(socket, "data"))[0].toString(), "bounded public download");
    socket.end();
    await once(socket, "close");
    assert.deepEqual(f.resolutions, [{ host: "api.nuget.org", options: { all: true, family: 4, verbatim: true } }]);
    assert.deepEqual(f.connections, [{ host: "13.107.246.40", port: 443, family: 4 }]);
    assert.equal(f.events.some(e => e.event === "connected" && e.host === "api.nuget.org"), true);
    assert.equal(f.broker.snapshot().active, 0);
});

test("public restore bursts fit the finite tunnel ceiling without bypassing it", async t => {
    const f = await fixture(t), sockets = [];
    for (let n = 0; n < 128; n++) {
        const socket = await f.request();
        sockets.push(socket);
        assert.match((await once(socket, "data"))[0].toString(), /^HTTP\/1.1 200 Connection Established\r\n\r\n$/,
            `public package tunnel ${n + 1} must fit the broker descriptor budget`);
    }
    assert.equal(f.broker.snapshot().active, 128);
    assert.equal(f.broker.snapshot().peakActive, 128);
    const queued = await queuedRequest(f);
    assert.equal(f.connections.length, 128);
    assert.equal(f.broker.snapshot().pending, 1);
    assert.equal(f.broker.snapshot().peakPending, 1);
    assert.equal(f.broker.snapshot().requests, 129, "the queued connection remains charged");
    assert.deepEqual(f.broker.snapshot().denied, {});
    for (const socket of sockets) {
        socket.write("bounded");
        assert.equal((await once(socket, "data"))[0].toString(), "bounded");
    }
    const admitted = once(queued, "data");
    const closed = sockets.map(socket => once(socket, "close"));
    for (const socket of sockets) socket.end();
    assert.match((await admitted)[0].toString(), /200 Connection Established/);
    await Promise.all(closed);
    queued.end();
    await once(queued, "close");
    assert.equal(f.broker.snapshot().active, 0);
    assert.equal(f.broker.snapshot().pending, 0);
    assert.equal(f.broker.snapshot().peakActive, 128);
    assert.ok(3 * limits.active + 8 < 1024, "active and pending sockets must fit the unchanged broker file limit");
});

test("admission is FIFO, bounded and charged without resolving queued requests", async t => {
    const f = await fixture(t, { limits: { active: 2 } });
    const active = [];
    for (let n = 0; n < 2; n++) {
        const socket = await f.request();
        await once(socket, "data");
        active.push(socket);
    }
    const first = await queuedRequest(f, "globalcdn.nuget.org:443");
    const second = await queuedRequest(f, "pkgs.dev.azure.com:443");
    assert.equal(f.connections.length, 2);
    assert.equal(f.resolutions.length, 2);
    assert.equal(f.broker.snapshot().pending, 2);
    assert.match(await response(await f.request()), /503 Service Unavailable/);
    assert.deepEqual(f.broker.snapshot().denied, { "queue-capacity": 1 });
    const firstAdmitted = once(first, "data");
    active[0].end();
    assert.match((await firstAdmitted)[0].toString(), /200 Connection Established/);
    assert.equal(f.resolutions.at(-1).host, "globalcdn.nuget.org");
    assert.equal(f.broker.snapshot().pending, 1);
    const secondAdmitted = once(second, "data");
    first.end();
    assert.match((await secondAdmitted)[0].toString(), /200 Connection Established/);
    assert.equal(f.resolutions.at(-1).host, "pkgs.dev.azure.com");
    assert.equal(f.broker.snapshot().pending, 0);
    assert.equal(f.broker.snapshot().peakActive, 2);
    assert.equal(f.broker.snapshot().peakPending, 2);
    assert.equal(f.broker.snapshot().requests, 5);
});

test("queued restores reclaim idle pooled tunnels without increasing the active ceiling", async t => {
    t.mock.timers.enable({ apis: ["setTimeout"] });
    assert.equal(limits.idleMs, 30000);
    const f = await fixture(t, { limits: { active: 1 } });
    const first = await f.request();
    await once(first, "data");
    const firstClosed = once(first, "close");
    const next = await queuedRequest(f);
    t.mock.timers.tick(29999);
    assert.equal(f.broker.snapshot().idleReclaimed, 0);
    assert.equal(f.broker.snapshot().pending, 1);
    const admitted = once(next, "data");
    t.mock.timers.tick(1);
    assert.match((await admitted)[0].toString(), /200 Connection Established/);
    await firstClosed;
    assert.equal(f.events.some(e => e.reason === "idle-pressure"), true);
    assert.equal(f.broker.snapshot().idleReclaimed, 1);
    assert.equal(f.broker.snapshot().peakActive, 1);
    assert.equal(f.broker.snapshot().pending, 0);
    assert.deepEqual(f.broker.snapshot().denied, {});
});

test("idle reclamation preserves uncontended tunnels and continuously transferred bytes", async t => {
    t.mock.timers.enable({ apis: ["setTimeout"] });
    const f = await fixture(t, { limits: { active: 1 } });
    const first = await f.request();
    await once(first, "data");
    t.mock.timers.tick(60000);
    assert.equal(f.broker.snapshot().active, 1, "no idle eviction without admission pressure");
    first.write("still live");
    assert.equal((await once(first, "data"))[0].toString(), "still live");
    const next = await queuedRequest(f);
    for (let n = 0; n < 8; n++) {
        t.mock.timers.tick(5000);
        first.write("bytes");
        assert.equal((await once(first, "data"))[0].toString(), "bytes");
        assert.equal(f.broker.snapshot().pending, 1);
    }
    assert.equal(f.broker.snapshot().idleReclaimed, 0);
    const admitted = once(next, "data");
    first.end();
    assert.match((await admitted)[0].toString(), /200 Connection Established/);
});

test("cancelled queued requests cannot dial later or refund their connection charge", async t => {
    const f = await fixture(t, { limits: { active: 1 } });
    const first = await f.request();
    await once(first, "data");
    const queued = await queuedRequest(f);
    queued.end();
    await once(queued, "close");
    const closed = once(first, "close");
    first.end();
    await closed;
    assert.equal(f.connections.length, 1);
    assert.equal(f.broker.snapshot().pending, 0);
    assert.equal(f.broker.snapshot().requests, 2);
    const next = await f.request();
    assert.match((await once(next, "data"))[0].toString(), /200 Connection Established/);
});

test("shutdown cancels queued admissions as well as active DNS and sockets", async t => {
    const f = await fixture(t, { limits: { active: 1 } });
    const first = await f.request();
    await once(first, "data");
    const queued = await queuedRequest(f);
    const closed = [once(first, "close"), once(queued, "close")];
    await f.broker.close();
    await Promise.all(closed);
    assert.equal(f.connections.length, 1);
    assert.equal(f.broker.snapshot().active, 0);
    assert.equal(f.broker.snapshot().pending, 0);
    assert.equal(f.broker.snapshot().requests, 2);
});

test("early queued TLS bytes are bounded and charged exactly once before forwarding", async t => {
    const f = await fixture(t, { limits: { active: 1, uploadBytes: 4 } });
    const first = await f.request();
    await once(first, "data");
    const queued = await queuedRequest(f, undefined, "", "1234");
    assert.equal(f.broker.snapshot().totalBytes, 4);
    assert.equal(f.connections.length, 1);
    first.end();
    let received = "";
    while (!received.endsWith("1234")) received += (await once(queued, "data"))[0];
    assert.match(received, /^HTTP\/1.1 200 Connection Established\r\n\r\n1234$/);
    assert.equal(f.broker.snapshot().totalBytes, 8);
    assert.equal(f.connections.length, 2);
});

for (const kind of ["uploadBytes", "earlyUploadBytes"])
test(`queued ${kind} overflow closes the request before any DNS or upstream dial`, async t => {
    assert.equal(limits.earlyUploadBytes, 65536);
    const f = await fixture(t, { limits: { active: 1, [kind]: 4 } });
    const first = await f.request();
    await once(first, "data");
    const queued = await queuedRequest(f, undefined, "", "1234");
    queued.write("5");
    assert.equal(await response(queued), "");
    assert.equal(f.connections.length, 1);
    assert.equal(f.resolutions.length, 1);
    assert.equal(f.broker.snapshot().pending, 0);
    assert.equal(f.broker.snapshot().totalBytes, 5);
    assert.equal(f.broker.snapshot().requests, 2);
    assert.ok(f.events.some(event => event.reason === (kind === "earlyUploadBytes" ? "early-upload" : kind)));
});

test("Node's default-port proxy Host still pins the approved destination", async t => {
    const f = await fixture(t);
    const socket = await f.request(undefined, "", "", "api.nuget.org");
    assert.match((await once(socket, "data"))[0].toString(), /^HTTP\/1.1 200 Connection Established\r\n\r\n$/);
    socket.end();
    await once(socket, "close");
    assert.deepEqual(f.connections, [{ host: "13.107.246.40", port: 443, family: 4 }]);
});

for (const host of ["vwvvsblobprodcus334.vsblob.vsassets.io", "public-package-cdn.vsblob.vsassets.io"])
test(`public Azure DevOps package CDN pins its public address: ${host}`, async t => {
    const f = await fixture(t);
    const socket = await f.request(`${host}:443`);
    assert.match((await once(socket, "data"))[0].toString(), /^HTTP\/1.1 200 Connection Established\r\n\r\n$/);
    socket.end();
    await once(socket, "close");
    assert.equal(f.resolutions[0].host, host);
    assert.deepEqual(f.connections, [{ host: "13.107.246.40", port: 443, family: 4 }]);
});

test("control, metadata, credentials, redirects and alternate authorities cannot reach DNS", async t => {
    const f = await fixture(t);
    for (const authority of ["api.github.com:443", "github.com:443", "169.254.169.254:443", "localhost:443",
        "host.docker.internal:443", "api.nuget.org:80", "api.nuget.org:0443", "API.NUGET.ORG:443",
        "api.nuget.org.:443", "api.nuget.org.evil.test:443", "api.nuget.org@evil.test:443",
        "https://api.nuget.org:443", "api.nuget.org:443/path", "api.nuget.org:443?token=x",
        "%61pi.nuget.org:443", "[::1]:443", "13.107.246.40:443",
        "vsblob.vsassets.io:443", "cdn.vsblob.vsassets.io.evil.test:443",
        "cdn.evil.vsblob.vsassets.io:443", "-cdn.vsblob.vsassets.io:443",
        "cdn-.vsblob.vsassets.io:443", "cdn.vsblob.vsassets.io.:443",
        "cdn.vsblob.vsassets.io:80", "private.blob.core.windows.net:443"])
        await t.test(authority, async () => assert.match(await response(await f.request(authority)), /403 Forbidden/));
    for (const header of ["Authorization: Bearer harmless-canary\r\n", "Proxy-Authorization: Basic canary\r\n",
        "Cookie: canary\r\n", "X-GitHub-Token: canary\r\n", "Content-Length: 1\r\n",
        "Transfer-Encoding: chunked\r\n", "Host: api.nuget.org:443\r\n"])
        await t.test(header.trim(), async () => assert.match(await response(await f.request(undefined, header)), /403 Forbidden/));
    for (const host of ["github.com", "api.nuget.org:80", "api.nuget.org.evil.test",
        "api.nuget.org@evil.test", "api.nuget.org:443:443", ""])
        await t.test(`Host: ${host}`, async () => assert.match(await response(await f.request(undefined, "", "", host)), /403 Forbidden/));
    assert.equal(f.resolutions.length, 0);
    assert.equal(f.connections.length, 0);
    assert.equal(JSON.stringify(f.events).includes("canary"), false);
});

test("DNS rebinding, mixed answers and malformed resolution fail closed", async t => {
    for (const answers of [
        [], [{ address: "127.0.0.1", family: 4 }], [{ address: "169.254.169.254", family: 4 }],
        [{ address: "13.107.246.40", family: 4 }, { address: "10.0.0.1", family: 4 }],
        [{ address: "13.107.246.40", family: 6 }], [{ address: "::ffff:13.107.246.40", family: 6 }],
        Array.from({ length: 17 }, () => ({ address: "13.107.246.40", family: 4 })),
    ]) await t.test(JSON.stringify(answers), async t => {
        const f = await fixture(t, { resolve: async () => answers });
        assert.match(await response(await f.request()), /403 Forbidden/);
        assert.equal(f.connections.length, 0);
    });
});

test("tunnel byte, time and concurrent/request limits are enforced, not advisory", async t => {
    for (const kind of ["uploadBytes", "downloadBytes", "totalBytes"]) await t.test(kind, async t => {
        const f = await fixture(t, { limits: { [kind]: 8 } });
        const socket = await f.request();
        await once(socket, "data");
        socket.write("more than eight bytes");
        const bytes = await response(socket);
        assert.equal(bytes.length, 0);
        assert.equal(f.events.some(e => e.reason === kind), true);
        assert.equal(f.broker.snapshot().active, 0);
    });
    await t.test("absolute tunnel timeout", async t => {
        const f = await fixture(t, { limits: { tunnelMs: 40 } });
        const socket = await f.request();
        await once(socket, "data");
        await response(socket);
        assert.equal(f.events.some(e => e.reason === "timeout"), true);
    });
    await t.test("requests", async t => {
        const f = await fixture(t, { limits: { requests: 1 } });
        const first = await f.request();
        await once(first, "data");
        assert.match(await response(await f.request()), /503 Service Unavailable/);
        first.end();
        await once(first, "close");
        await assert.rejects(f.request(), /ECONNREFUSED/);
        assert.equal(f.broker.snapshot().stopped, true);
        assert.equal(f.connections.length, 1);
    });
});

test("shutdown destroys owned tunnels and late DNS cannot start a connection", async t => {
    let finish;
    const f = await fixture(t, { resolve: () => new Promise(resolve => { finish = resolve; }) });
    const socket = await f.request();
    while (!finish) await new Promise(resolve => setImmediate(resolve));
    const closed = once(socket, "close");
    await f.broker.close();
    await closed;
    finish([{ address: "13.107.246.40", family: 4 }]);
    await new Promise(resolve => setImmediate(resolve));
    assert.equal(f.connections.length, 0);
    assert.equal(f.broker.snapshot().active, 0);
    assert.equal(f.events.some(e => e.reason === "shutdown"), true);
});

test("DNS failure, a stalled lookup and a failed TCP connection expose no successful tunnel", async t => {
    for (const mode of ["rejected", "stalled", "connect-throw", "socket-error"]) await t.test(mode, async t => {
        const f = await fixture(t, {
            limits: { tunnelMs: 50 },
            ...(mode === "rejected" ? { resolve: async () => { throw new Error("DNS lookup failed"); } } :
                mode === "stalled" ? { resolve: () => new Promise(() => {}) } :
                    mode === "connect-throw" ? { connect: () => { throw new Error("cannot connect"); } } :
                        { connect: () => {
                            const socket = new net.Socket();
                            queueMicrotask(() => socket.destroy(new Error("TCP failure")));
                            return socket;
                        } }),
        });
        assert.doesNotMatch(await response(await f.request()), /200 Connection Established/);
        assert.equal(f.events.some(e => e.event === "connected"), false);
        assert.equal(f.events.some(e => e.event === "closed" || e.event === "denied"), true);
        assert.equal(f.broker.snapshot().active, 0);
    });
});

test("early tunnel bytes are charged, and totals survive successful tunnels", async t => {
    const f = await fixture(t, { limits: { totalBytes: 8 } });
    const first = await f.request(undefined, "", "1234");
    let received = "";
    while (!received.endsWith("1234")) received += (await once(first, "data"))[0];
    assert.match(received, /^HTTP\/1.1 200 Connection Established\r\n\r\n1234$/);
    first.end();
    await once(first, "close");
    assert.equal(f.broker.snapshot().totalBytes, 8);
    assert.match(await response(await f.request()), /503 Service Unavailable/);
    assert.equal(f.connections.length, 1);
});

test("closing the broker terminates both ends of live owned tunnels", async t => {
    const f = await fixture(t);
    const socket = await f.request();
    await once(socket, "data");
    const disconnected = once(socket, "close");
    await Promise.all([f.broker.close(), f.broker.close()]);
    await disconnected;
    assert.equal(f.broker.snapshot().active, 0);
    assert.equal(f.broker.snapshot().stopped, true);
    assert.equal(f.events.some(e => e.reason === "shutdown"), true);
});

test("plain HTTP, oversized headers and stalled headers cannot become a tunnel", async t => {
    for (const raw of [
        "GET http://api.nuget.org/ HTTP/1.1\r\nHost: api.nuget.org\r\n\r\n",
        `CONNECT api.nuget.org:443 HTTP/1.1\r\nHost: api.nuget.org:443\r\nUser-Agent: ${"x".repeat(9000)}\r\n\r\n`,
        "CONNE",
    ]) await t.test(raw.slice(0, 70), async t => {
        const f = await fixture(t, { limits: { headerMs: 40 } });
        const socket = net.connect({ host: "127.0.0.1", port: f.broker.server.address().port });
        await once(socket, "connect");
        socket.write(raw);
        assert.doesNotMatch(await response(socket), /200 Connection Established/);
        assert.equal(f.resolutions.length, 0);
        assert.equal(f.connections.length, 0);
        assert.equal(f.events.some(e => e.event === "denied"), true);
    });
});

test("large bounded transfers respect stream backpressure without dropping bytes", async t => {
    const f = await fixture(t);
    const socket = await f.request();
    await once(socket, "data");
    socket.pause();
    const data = Buffer.alloc(512 * 1024, 120);
    const received = [];
    const complete = new Promise(resolve => {
        let size = 0;
        socket.on("data", bytes => {
            received.push(bytes);
            size += bytes.length;
            if (size === data.length) { socket.end(); resolve(); }
        });
    });
    socket.write(data);
    await new Promise(resolve => setTimeout(resolve, 25));
    socket.resume();
    await complete;
    await once(socket, "close");
    assert.deepEqual(Buffer.concat(received), data);
    assert.equal(f.broker.snapshot().totalBytes, data.length * 2);
});

test("upstream EOF does not discard a slow client's queued download", async t => {
    const data = Buffer.alloc(8 * 1024 * 1024, 120);
    const f = await fixture(t, {}, socket => socket.end(data));
    const socket = await f.request();
    const received = [];
    socket.on("data", bytes => {
        received.push(bytes);
        socket.pause();
        setTimeout(() => socket.resume(), 2);
    });
    await once(socket, "close");
    const bytes = Buffer.concat(received);
    const header = Buffer.from("HTTP/1.1 200 Connection Established\r\n\r\n");
    assert.deepEqual(bytes.subarray(0, header.length), header);
    assert.equal(bytes.length - header.length, data.length);
    assert.equal(bytes.subarray(header.length).equals(data), true);
});

test("policy caps cannot be expanded or configured with unknown options", () => {
    for (const [key, value] of Object.entries(limits)) {
        for (const invalid of [0, -1, 0.5, value + 1, Infinity])
            assert.throws(() => createEgressBroker({ limits: { [key]: invalid } }), /limit/);
    }
    assert.throws(() => createEgressBroker({ limits: { arbitrary: 1 } }), /limit/);
    assert.throws(() => createEgressBroker({ limits: [] }), /limit/);
    assert.throws(() => createEgressBroker({ resolve: "host-selected resolver" }), /callback/);
    assert.throws(() => createEgressBroker({ hosts: ["api.github.com"] }), /option/);
});
