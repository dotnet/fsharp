const path = require("node:path");
const { spawn } = require("node:child_process");
const { constants } = require("node:os");
const policy = require("../../.github/policies/regression-bisect.json");

function execute(executable, args, { cwd, env, timeoutMs, signal: cancellation, maxBytes = policy.evidenceBytesPerStream }) {
    return new Promise(resolve => {
        if (cancellation?.aborted) {
            resolve({ status: "cancelled", complete: false, exitCode: null, stdout: "", stderr: "" });
            return;
        }
        const child = spawn(executable, args, { cwd, env, shell: false, windowsHide: true, detached: process.platform !== "win32" });
        const streams = { stdout: [], stderr: [] };
        const lengths = { stdout: 0, stderr: 0 };
        let timedOut = false, cancelled = false, terminating = false, truncated = false, launchError = null, killing = Promise.resolve();
        for (const name of Object.keys(streams)) child[name].on("data", bytes => {
            const remaining = Math.max(0, maxBytes - lengths[name]);
            if (bytes.length > remaining) truncated = true;
            if (remaining > 0) streams[name].push(bytes.subarray(0, remaining));
            lengths[name] += bytes.length;
        });
        child.on("error", error => { launchError = error.message; });
        const terminate = () => {
            if (!child.pid || terminating) return;
            terminating = true;
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
        };
        const abort = () => { cancelled = true; terminate(); };
        cancellation?.addEventListener("abort", abort, { once: true });
        const timer = setTimeout(() => { timedOut = true; terminate(); }, timeoutMs);
        child.on("close", async (code, signal) => {
            clearTimeout(timer);
            cancellation?.removeEventListener("abort", abort);
            await killing;
            const stdout = Buffer.concat(streams.stdout).toString("utf8").replace(/\r\n/g, "\n");
            const stderr = Buffer.concat(streams.stderr).toString("utf8").replace(/\r\n/g, "\n");
            const managedException = signal === "SIGABRT" && /^Unhandled exception\./.test(stderr);
            resolve({
                status: cancelled ? "cancelled" : timedOut ? "timeout" : launchError || signal && !managedException ? "exception" : "completed",
                complete: !truncated && !launchError && !cancelled && !timedOut, exitCode: managedException ? 128 + constants.signals[signal] : code,
                stdout, stderr,
                ...(launchError ? { error: launchError } : {}),
            });
        });
    });
}

module.exports = { execute };
