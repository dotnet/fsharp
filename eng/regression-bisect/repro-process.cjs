const path = require("node:path");
const { spawn } = require("node:child_process");
const { constants } = require("node:os");
const { randomUUID } = require("node:crypto");
const policy = require("../../.github/policies/regression-bisect.json");

function execute(executable, args, { cwd, env, timeoutMs, signal: cancellation, maxBytes = policy.evidenceBytesPerStream }) {
    return new Promise(resolve => {
        if (cancellation?.aborted) {
            resolve({ status: "cancelled", complete: false, exitCode: null, stdout: "", stderr: "" });
            return;
        }
        if (!env) {
            resolve({ status: "exception", complete: false, exitCode: null, stdout: "", stderr: "", error: "explicit process environment is required" });
            return;
        }
        const windows = process.platform === "win32";
        const errorMarker = `REPRO_PROCESS_${randomUUID()}:`;
        let child;
        if (windows) {
            const systemRoot = process.env.SystemRoot;
            if (!systemRoot) {
                resolve({ status: "exception", complete: false, exitCode: null, stdout: "", stderr: "",
                    error: "Windows supervisor requires the host SystemRoot environment variable" });
                return;
            }
            const system32 = path.join(systemRoot, "System32");
            const powershell = path.join(system32, "WindowsPowerShell", "v1.0");
            // Keep PowerShell's OS bootstrap separate from the sample's explicit environment.
            const bootstrap = Object.fromEntries([
                "SystemRoot", "SystemDrive", "windir", "USERPROFILE", "APPDATA", "LOCALAPPDATA", "TEMP", "TMP",
                "ProgramFiles", "ProgramFiles(x86)", "ProgramData", "ComSpec", "__PSLockDownPolicy",
            ].filter(name => process.env[name] !== undefined).map(name => [name, process.env[name]]));
            bootstrap.PSModulePath = path.join(powershell, "Modules");
            bootstrap.PATH = [powershell, system32, systemRoot].join(";");
            child = spawn(path.join(powershell, "powershell.exe"),
                ["-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-File", path.join(__dirname, "repro-process.ps1"), errorMarker],
                { cwd, env: bootstrap, shell: false, windowsHide: true });
        } else child = spawn(executable, args, { cwd, env, shell: false, detached: true });
        const streams = { stdout: [], stderr: [] };
        const lengths = { stdout: 0, stderr: 0 };
        let timedOut = false, cancelled = false, terminating = false, truncated = false, launchError = null;
        let terminationTimer, finished = false, errorTail = "";
        const helperError = new RegExp(`${errorMarker}([A-Za-z0-9+/=]+)\\r?\\n`);
        for (const name of Object.keys(streams)) child[name].on("data", bytes => {
            if (windows && name === "stderr") {
                errorTail = (errorTail + bytes.toString("ascii")).slice(-8192);
                const error = helperError.exec(errorTail);
                if (error) launchError = Buffer.from(error[1], "base64").toString("utf8");
            }
            const remaining = Math.max(0, maxBytes - lengths[name]);
            if (bytes.length > remaining) truncated = true;
            if (remaining > 0) streams[name].push(bytes.subarray(0, remaining));
            lengths[name] += bytes.length;
        });
        child.on("error", error => { launchError = error.message; });
        if (windows) {
            child.stdin.on("error", error => { if (!terminating) launchError = error.message; });
            child.stdin.end(JSON.stringify({
                executable, args, environment: Object.entries(env).filter(([, value]) => value !== undefined).map(([key, value]) => `${key}=${value}`),
            }) + "\n");
        }
        const terminate = () => {
            if (!child.pid || terminating) return;
            terminating = true;
            try {
                if (windows) {
                    if (!child.kill("SIGKILL")) throw new Error("supervisor could not be terminated");
                } else process.kill(-child.pid, "SIGKILL");
            } catch (error) {
                launchError = `process-tree termination failed: ${error.message}`;
            }
            terminationTimer = setTimeout(() => {
                launchError ??= "process-tree termination did not close its streams within 5 seconds";
                child.stdout.destroy();
                child.stderr.destroy();
                child.stdin.destroy();
                finish(null, null);
            }, 5000);
        };
        const abort = () => { cancelled = true; terminate(); };
        cancellation?.addEventListener("abort", abort, { once: true });
        const timer = setTimeout(() => { timedOut = true; terminate(); }, timeoutMs);
        const finish = (code, signal) => {
            if (finished) return;
            finished = true;
            clearTimeout(timer);
            clearTimeout(terminationTimer);
            cancellation?.removeEventListener("abort", abort);
            const stdout = Buffer.concat(streams.stdout).toString("utf8").replace(/\r\n/g, "\n");
            const stderr = Buffer.concat(streams.stderr).toString("utf8").replace(helperError, "").replace(/\r\n/g, "\n");
            const managedException = signal === "SIGABRT" && /^Unhandled exception\./.test(stderr);
            resolve({
                status: cancelled ? "cancelled" : timedOut ? "timeout" : launchError || signal && !managedException ? "exception" : "completed",
                complete: !truncated && !launchError && !cancelled && !timedOut, exitCode: managedException ? 128 + constants.signals[signal] : code,
                stdout, stderr,
                ...(launchError ? { error: launchError } : {}),
            });
        };
        child.on("close", finish);
    });
}

module.exports = { execute };
