// Wrap the pinned stock harness, including its retries and proxy reflection.
const { spawn } = require("node:child_process");
const path = require("node:path");

const shutdownMs = 40000;
const deadline = Number(process.env.COMPILER_PERF_EXECUTION_DEADLINE_MS);
const remaining = deadline - Date.now() - shutdownMs;
if (!Number.isSafeInteger(deadline) || remaining <= 0 || remaining > 15000000 - shutdownMs) {
  console.error("Protected runtime execution deadline exhausted or invalid");
  process.exit(2);
}

const grouped = process.platform !== "win32";
const child = spawn(process.execPath, [path.join(__dirname, "copilot_harness.cjs"), ...process.argv.slice(2)],
                    { stdio: "inherit", detached: grouped });
let stopped = 0;
let killTimer;
function signalOwned(signal) {
  if (child.exitCode !== null || child.signalCode !== null) return;
  if (grouped) process.kill(-child.pid, signal);
  else child.kill(signal);
}
function stop(code) {
  if (stopped) return;
  stopped = code;
  console.error("Protected runtime stopping owned stock harness");
  signalOwned("SIGTERM");
  killTimer = setTimeout(() => signalOwned("SIGKILL"), 20000);
}
const timer = setTimeout(() => stop(124), remaining);
let last = Date.now();
const wallTimer = setInterval(() => {
  const now = Date.now();
  if (now < last || now >= deadline - shutdownMs) stop(124);
  last = now;
}, 1000);
process.on("SIGTERM", () => stop(143));
process.on("SIGINT", () => stop(130));
function finish(code) {
  clearTimeout(timer);
  clearTimeout(killTimer);
  clearInterval(wallTimer);
  process.exit(code);
}
child.on("error", error => {
  console.error(`Stock harness launch failed: ${error.message}`);
  finish(2);
});
child.on("exit", (code, signal) => finish(stopped || (signal ? 130 : code ?? 2)));
