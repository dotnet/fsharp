const path = require("node:path");
const { spawn } = require("node:child_process");
const { constants } = require("node:os");
const policy = require("../../.github/policies/regression-bisect.json");

function execute(executable, args, { cwd, env, timeoutMs, maxBytes = policy.evidenceBytesPerStream }) {
    return new Promise(resolve => {
        const child = spawn(executable, args, { cwd, env, shell: false, windowsHide: true, detached: process.platform !== "win32" });
        const streams = { stdout: [], stderr: [] };
        const lengths = { stdout: 0, stderr: 0 };
        let timedOut = false, truncated = false, launchError = null, killing = Promise.resolve();
        for (const name of Object.keys(streams)) child[name].on("data", bytes => {
            const remaining = Math.max(0, maxBytes - lengths[name]);
            if (bytes.length > remaining) truncated = true;
            if (remaining > 0) streams[name].push(bytes.subarray(0, remaining));
            lengths[name] += bytes.length;
        });
        child.on("error", error => { launchError = error.message; });
        const timer = setTimeout(() => {
            timedOut = true;
            if (!child.pid) return;
            if (process.platform === "win32") {
                killing = new Promise(done => {
                    const killer = spawn(path.join(env.SystemRoot, "System32", "taskkill.exe"), ["/PID", String(child.pid), "/T", "/F"],
                        { windowsHide: true, stdio: "ignore", env });
                    killer.on("error", error => { launchError = `process-tree termination failed: ${error.message}`; child.kill(); done(); });
                    killer.on("close", code => { if (code !== 0) launchError = `process-tree termination failed (${code})`; done(); });
                });
            } else {
                try { process.kill(-child.pid, "SIGKILL"); } catch (error) { launchError = `process-tree termination failed: ${error.message}`; }
            }
        }, timeoutMs);
        child.on("close", async (code, signal) => {
            clearTimeout(timer);
            await killing;
            const stdout = Buffer.concat(streams.stdout).toString("utf8").replace(/\r\n/g, "\n");
            const stderr = Buffer.concat(streams.stderr).toString("utf8").replace(/\r\n/g, "\n");
            const managedException = signal === "SIGABRT" && /^Unhandled exception\./.test(stderr);
            resolve({
                status: timedOut ? "timeout" : launchError || signal && !managedException ? "exception" : "completed",
                complete: !truncated && !launchError, exitCode: managedException ? 128 + constants.signals[signal] : code,
                stdout, stderr,
                ...(launchError ? { error: launchError } : {}),
            });
        });
    });
}

module.exports = { execute };
