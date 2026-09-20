const fs = require("node:fs/promises");
const path = require("node:path");
const { randomBytes } = require("node:crypto");
const { runContainer, validateContainerConfig } = require("./container.cjs");
const { hash, requireValue } = require("./toolchain-io.cjs");

async function main() {
    const [opt, configurationFile, directory, extra] = process.argv.slice(2);
    requireValue(opt === "--run" && configurationFile && directory && !extra &&
        path.isAbsolute(configurationFile) && path.isAbsolute(directory),
    "usage: node eng\\regression-bisect\\containment.smoke.cjs --run <absolute-approved-config.json> <absolute-new-evidence-directory>");
    const stat = await fs.lstat(configurationFile);
    requireValue(stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.size < 48 * 1024 &&
        await fs.realpath(configurationFile) === configurationFile, "config must be a bounded resolved regular file");
    const bytes = await fs.readFile(configurationFile);
    const config = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(bytes));
    validateContainerConfig(config);
    await fs.mkdir(directory);
    const root = await fs.realpath(directory);
    const canary = randomBytes(32).toString("hex");
    const outer = path.join(root, "outer-canary");
    await fs.mkdir(outer);
    const outerPaths = ["home-canary", "git-config-canary", "artifact-token-canary"].map(name => path.join(outer, name));
    for (const file of outerPaths) await fs.writeFile(file, canary, { flag: "wx", mode: 0o600 });
    const previous = process.env.RB_OUTER_CANARY;
    process.env.RB_OUTER_CANARY = canary;
    const report = {
        schemaVersion: 1, status: "incomplete", image: config.image, configSha256: hash(bytes),
        sourceSha256: hash(await fs.readFile(__filename)), startedAt: new Date().toISOString(), cases: [],
    };
    const cancellation = new AbortController();
    const cancel = () => cancellation.abort(new Error("containment cancelled"));
    for (const name of ["SIGTERM", "SIGINT"]) process.once(name, cancel);
    const signal = AbortSignal.any([AbortSignal.timeout(13 * 60000), cancellation.signal]);
    try {
        for (const mode of ["inspect", "disk", "pids", "cpu", "memory", "malformed", "timeout"]) {
            signal.throwIfAborted();
            const evidenceDirectory = path.join(root, mode);
            await fs.mkdir(evidenceDirectory);
            const result = await runContainer({
                ...config, limits: { ...config.limits, timeoutMs: mode === "timeout" ? 3000 : Math.min(config.limits.timeoutMs, 120000),
                    cpus: 1, scratchMiB: 8, memoryMiB: 256, pids: 32 },
            }, {
                operation: "canary",
                input: { mode, outerPaths: [...outerPaths, `/proc/${process.pid}/environ`],
                    canaryDigest: hash(canary), hostAddresses: ["172.17.0.1"] },
                evidenceDirectory, signal,
            });
            const expectedDenial = ["malformed", "timeout", "memory"].includes(mode);
            const cleanupComplete = result.receipt.resources.length === 4 &&
                result.receipt.cleanup.length === 4 && result.receipt.cleanup.every(r => r.removed);
            const passed = result.receipt.workerStarted && cleanupComplete &&
                (expectedDenial ? result.status === "blocked" &&
                    (mode === "memory" ? result.receipt.cleanup.find(c => c.name.endsWith("-worker"))?.state?.OOMKilled === true :
                        mode === "malformed" ? /binding/.test(result.reason) : /timeout/i.test(result.reason))
                    : result.status === "collected" && result.result.mode === mode);
            report.cases.push({ mode, passed, status: result.status, reason: result.reason ?? null, receipt: `${mode}/receipt.json` });
            console.log(`${mode}: ${passed ? "passed" : "incomplete"} (${result.reason ?? result.status})`);
            requireValue(passed, `containment ${mode} incomplete: ${result.reason ?? "missing enforcement evidence"}`);
        }
        report.status = "passed";
    } catch (error) {
        report.reason = error.message;
        process.exitCode = 2;
    } finally {
        for (const name of ["SIGTERM", "SIGINT"]) process.removeListener(name, cancel);
        if (previous === undefined) delete process.env.RB_OUTER_CANARY;
        else process.env.RB_OUTER_CANARY = previous;
        for (const file of outerPaths) await fs.unlink(file);
        await fs.rmdir(outer);
        report.completedAt = new Date().toISOString();
        await fs.writeFile(path.join(root, "containment.json"), JSON.stringify(report, null, 2), { flag: "wx" });
    }
    console.log(JSON.stringify({ status: report.status, cases: report.cases.length, evidenceDirectory: root }));
}

if (require.main === module) main().catch(error => { console.error(error.message); process.exitCode = 2; });
