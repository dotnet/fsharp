const http = require("node:http");
const net = require("node:net");
const dns = require("node:dns/promises");
const { readFileSync } = require("node:fs");
const path = require("node:path");
const { requireValue } = require("./toolchain-io.cjs");

const policy = JSON.parse(readFileSync(path.join(__dirname, "..", "..", ".github", "policies", "regression-bisect.json")));
const hosts = new Set([
    ...policy.toolchains.publicFeeds.map(url => new URL(url).hostname),
    "api.nuget.org", "globalcdn.nuget.org", "pkgs.dev.azure.com",
]);
const limits = Object.freeze({
    active: 128, requests: 2048, uploadBytes: 1024 * 1024, earlyUploadBytes: 64 * 1024,
    downloadBytes: 1024 * 1024 * 1024, totalBytes: 8 * 1024 * 1024 * 1024,
    tunnelMs: 300000, headerMs: 10000, idleMs: 30000,
});

function publicAddress(address) {
    if (net.isIP(address) !== 4) return false;
    const [a, b, c] = address.split(".").map(Number);
    return !(a === 0 || a === 10 || a === 127 || a >= 224 ||
        a === 100 && b >= 64 && b <= 127 || a === 169 && b === 254 ||
        a === 172 && b >= 16 && b <= 31 ||
        a === 192 && (b === 0 && [0, 2].includes(c) || b === 88 && c === 99 || b === 168) ||
        a === 198 && ([18, 19].includes(b) || b === 51 && c === 100) ||
        a === 203 && b === 0 && c === 113);
}

function createEgressBroker(options = {}) {
    requireValue(options && Object.getPrototypeOf(options) === Object.prototype &&
        Object.keys(options).every(k => ["limits", "resolve", "connect", "audit"].includes(k)),
        "unexpected egress option");
    requireValue(options.limits === undefined || options.limits && Object.getPrototypeOf(options.limits) === Object.prototype,
        "invalid egress limits");
    const cap = { ...limits };
    for (const [key, value] of Object.entries(options.limits ?? {})) {
        requireValue(Object.hasOwn(limits, key) && Number.isSafeInteger(value) && value > 0 && value <= limits[key],
            "egress limits may only be lowered");
        cap[key] = value;
    }
    const resolve = options.resolve ?? dns.lookup;
    const connect = options.connect ?? net.connect;
    const emit = options.audit ?? (event => process.stderr.write(`${JSON.stringify(event)}\n`));
    requireValue(["resolve", "connect", "audit"].every(key => options[key] === undefined || typeof options[key] === "function"),
        "invalid egress callback");
    const sockets = new Set(), tunnels = new Map(), pending = new Map(), ids = new WeakMap();
    const denied = {};
    let requests = 0, totalBytes = 0, peakActive = 0, peakPending = 0, idleReclaimed = 0, stopped = false;
    const audit = event => {
        if (event.event === "denied") denied[event.reason] = (denied[event.reason] ?? 0) + 1;
        emit(event);
    };
    const admitPending = () => {
        while (!stopped && tunnels.size < cap.active && pending.size) {
            const [id, next] = pending.entries().next().value;
            pending.delete(id);
            next.start();
        }
    };
    const server = http.createServer({ maxHeaderSize: 8192, requestTimeout: cap.headerMs, headersTimeout: cap.headerMs },
        (request, response) => {
            audit({ event: "denied", reason: "method" });
            response.writeHead(403, { Connection: "close", "Content-Length": "0" });
            response.end();
        });
    server.on("connection", socket => {
        ids.set(socket, ++requests);
        sockets.add(socket);
        const timer = setTimeout(() => {
            audit({ event: "denied", reason: "header-timeout" });
            socket.destroy();
        }, cap.headerMs);
        socket.on("close", () => { clearTimeout(timer); sockets.delete(socket); });
        socket.on("error", () => socket.destroy());
        socket.once("egress-tunnel", () => clearTimeout(timer));
        if (requests > cap.requests) {
            stopped = true;
            audit({ event: "denied", reason: "request-capacity" });
            socket.pause();
            socket.end("HTTP/1.1 503 Service Unavailable\r\nConnection: close\r\nContent-Length: 0\r\n\r\n", () => socket.destroy());
            server.close();
        } else if (stopped || sockets.size > cap.active * 2 + 4) {
            audit({ event: "denied", reason: "socket-capacity" });
            socket.destroy();
        }
    });
    server.on("clientError", (error, socket) => {
        audit({ event: "denied", reason: "http-parser" });
        socket.destroy();
    });
    server.on("connect", (request, socket, head) => {
        const id = ids.get(socket);
        const host = request.url.slice(0, -4);
        const headerNames = request.rawHeaders.filter((_, i) => i % 2 === 0).map(h => h.toLowerCase());
        const reject = (reason, status = "403 Forbidden") => {
            audit({ event: "denied", id, reason });
            socket.end(`HTTP/1.1 ${status}\r\nConnection: close\r\nContent-Length: 0\r\n\r\n`, () => socket.destroy());
        };
        if (stopped || id > cap.requests || totalBytes >= cap.totalBytes)
            return reject("capacity", "503 Service Unavailable");
        const packageCdn = /^[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.vsblob\.vsassets\.io$/.test(host);
        if (!(hosts.has(host) || packageCdn) || request.url !== `${host}:443`) return reject("authority");
        // Node omits the default HTTPS port from Host, but not the CONNECT authority.
        if ((request.headers.host !== request.url && request.headers.host !== host) || new Set(headerNames).size !== headerNames.length ||
            headerNames.some(h => !["host", "connection", "proxy-connection", "user-agent"].includes(h)))
            return reject("headers");
        socket.emit("egress-tunnel");
        socket.pause();
        let upstream, idleTimer, established = false, done = false;
        const buffered = [];
        const counts = { uploadBytes: 0, downloadBytes: 0 };
        const finish = (reason, status) => {
            if (done) return;
            done = true;
            clearTimeout(timer);
            clearTimeout(idleTimer);
            buffered.length = 0;
            pending.delete(id);
            tunnels.delete(id);
            if (reason === "idle-pressure") idleReclaimed++;
            upstream?.destroy();
            if (status) reject(reason, status);
            else socket.destroy();
            audit({ event: "closed", id, host, reason, ...counts });
            admitPending();
        };
        const timer = setTimeout(() => finish("timeout"), cap.tunnelMs);
        const refreshIdle = () => {
            clearTimeout(idleTimer);
            if (done) return;
            idleTimer = setTimeout(() => {
                if (pending.size) finish("idle-pressure");
                else refreshIdle();
            }, cap.idleMs);
        };
        socket.on("close", () => finish("client-closed"));
        socket.on("error", () => finish("client-error"));
        socket.on("end", () => established ? upstream.end() : finish("client-closed"));
        const charge = (bytes, kind) => {
            if (done) return false;
            counts[kind] += bytes.length;
            totalBytes += bytes.length;
            if (counts[kind] > cap[kind]) { finish(kind); return false; }
            if (totalBytes > cap.totalBytes) { finish("totalBytes"); return false; }
            if (kind === "uploadBytes" && !established && counts.uploadBytes > cap.earlyUploadBytes) {
                finish("early-upload");
                return false;
            }
            return true;
        };
        socket.on("data", bytes => {
            if (!charge(bytes, "uploadBytes")) return;
            if (!established) buffered.push(bytes);
            else {
                refreshIdle();
                if (!upstream.write(bytes)) socket.pause();
            }
        });
        if (head.length) {
            if (!charge(head, "uploadBytes")) return;
            buffered.push(head);
        }
        const start = async () => {
            if (done) return;
            if (stopped || totalBytes >= cap.totalBytes) return finish("capacity", "503 Service Unavailable");
            tunnels.set(id, finish);
            peakActive = Math.max(peakActive, tunnels.size);
            try {
                const addresses = await resolve(host, { all: true, family: 4, verbatim: true });
                if (done) return;
                if (!Array.isArray(addresses) || !addresses.length || addresses.length > 16 ||
                    addresses.some(a => a?.family !== 4 || !publicAddress(a.address))) {
                    finish("private-address", "403 Forbidden");
                    return;
                }
                // Dial only the validated numeric address; a second DNS lookup would reopen rebinding.
                upstream = connect({ host: addresses[0].address, port: 443, family: 4 });
                upstream.on("error", () => finish("upstream-error"));
                upstream.on("close", () => finish("upstream-closed"));
                upstream.once("connect", () => {
                    if (done) return upstream.destroy();
                    established = true;
                    socket.write("HTTP/1.1 200 Connection Established\r\n\r\n");
                    audit({ event: "connected", id, host, address: addresses[0].address });
                    refreshIdle();
                    upstream.on("data", bytes => {
                        if (!charge(bytes, "downloadBytes")) return;
                        refreshIdle();
                        if (!socket.write(bytes)) upstream.pause();
                    });
                    socket.on("drain", () => upstream.resume());
                    upstream.on("drain", () => socket.resume());
                    upstream.on("end", () => socket.end());
                    let writable = true;
                    for (const bytes of buffered) if (!upstream.write(bytes)) writable = false;
                    buffered.length = 0;
                    if (!writable) socket.pause();
                });
            } catch {
                if (done) return;
                if (!established) finish("resolution-or-connect", "403 Forbidden");
                else finish("upstream-error");
            }
        };
        if (tunnels.size < cap.active) start();
        else if (pending.size < cap.active) {
            pending.set(id, { start, finish });
            peakPending = Math.max(peakPending, pending.size);
            audit({ event: "queued", id, host });
        } else finish("queue-capacity", "503 Service Unavailable");
        if (!done) socket.resume();
    });
    let closing;
    return {
        server,
        snapshot: () => ({ requests, active: tunnels.size, pending: pending.size, peakActive, peakPending,
            idleReclaimed, totalBytes, stopped, denied: { ...denied } }),
        close: () => closing ??= new Promise((resolve, reject) => {
            stopped = true;
            for (const { finish } of pending.values()) finish("shutdown");
            for (const finish of tunnels.values()) finish("shutdown");
            for (const socket of sockets) socket.destroy();
            if (!server.listening) return resolve();
            server.close(error => error ? reject(error) : resolve());
        }),
    };
}

module.exports = { createEgressBroker, publicAddress, limits };
