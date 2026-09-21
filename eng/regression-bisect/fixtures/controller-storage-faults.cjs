const fs = require("node:fs");
const fsp = require("node:fs/promises");
const path = require("node:path");
const { setTimeout: delay } = require("node:timers/promises");
const contracts = require("./contracts.cjs");
const { fingerprintInput: fingerprint } = require("../model.cjs");
const stores = require("../evidence-store.cjs");

if (process.env.RB_LOCAL_TEST === "explicit-offline-fixture" && !process.env.GITHUB_ACTIONS) {
    const file = process.env.RB_LOCAL_TEST_FILE;
    const read = () => JSON.parse(fs.readFileSync(file));
    const record = entry => fs.appendFileSync(`${file}.ledger`, JSON.stringify({ pid: process.pid, ...entry }) + "\n");
    const advanceClock = point => {
        const db = read();
        if (db.faults.clockAt !== point) return;
        db.now = db.faults.clockAfter;
        delete db.faults.clockAt;
        fs.writeFileSync(file, JSON.stringify(db));
        record({ kind: "clock", point, now: db.now });
    };
    const open = fsp.open;
    fsp.open = async (...args) => {
        if (read().faults.pauseQuota && path.basename(args[0]).startsWith("quota-")) {
            record({ kind: "quota-entered" });
            while (read().faults.pauseQuota) await delay(25);
            record({ kind: "quota-released" });
        }
        const handle = await open(...args);
        if (path.basename(args[0]) === "unit.json") advanceClock("unit");
        return handle;
    };
    const rename = fsp.rename;
    fsp.rename = async (from, to) => {
        await rename(from, to);
        if (to.endsWith(".bin") && read().faults.durableWriteAck) {
            record({ kind: "durable-write", file: path.basename(to) });
            throw new Error("durable upload acknowledgement lost");
        }
    };
    const create = stores.createFileEvidenceStore;
    stores.createFileEvidenceStore = config => {
        const store = create(config), write = store.writeEvidence, verify = store.verifyEvidence;
        return { ...store,
            verifyEvidence: async (...args) => {
                const result = await verify(...args);
                advanceClock("verify");
                return result;
            },
            writeEvidence: async (bytes, binding) => {
                if (read().faults.pauseQuota) {
                    const policy = contracts.policy(), a = policy.toolchains.measuredArchives[0];
                    const archive = { name: "sdk-win-x64", rid: "win-x64", architecture: "x64", url: a.url,
                        provenance: { kind: "approved-official-https-measured", approvalRevision: a.approvalRevision,
                            approvalDigest: fingerprint(a) } };
                    const payload = Buffer.from("quota fixture");
                    try {
                        await store.writeBlob(payload, { binding,
                            request: { policy, kind: "source", sourceSha: a.sourceSha, product: a.product, version: a.version,
                                platform: { os: "win", architecture: "x64", rid: "win-x64" }, archive },
                            receipt: contracts.archiveReceipt(archive, path.join(config.directory, "sdk.zip"), payload) });
                    } catch (error) {
                        record({ kind: "quota-timeout", error: error.name });
                        throw error;
                    }
                }
                return write(bytes, binding);
            },
        };
    };
}
