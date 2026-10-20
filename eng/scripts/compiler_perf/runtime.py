"""Trusted host adapters for GH AW v0.76.1; never execute this from agent mounts."""

import argparse
import asyncio
from datetime import timedelta
import os
from pathlib import Path, PurePosixPath
import re
import signal
import subprocess
import sys
import tempfile
import threading
import time

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import agent_bridge as bridge
import workflow as w

require, mem, e, m, c = w.require, w.mem, w.e, w.m, w.c
overlaps = bridge._overlap
PINS = {"gh_aw": "v0.76.1", "gh_aw_commit": "58d1bedbb7200f59c2d224151339e38fd8687d05",
        "copilot": "1.0.52", "awf": "v0.25.55", "gateway": "v0.3.19"}
SHUTDOWN_SECONDS = 2 * bridge.STOP_SECONDS
READINESS_SECONDS = w.STARTUP_SECONDS - SHUTDOWN_SECONDS - 1
AGENT_IMAGE = "ghcr.io/github/gh-aw-firewall/agent:0.25.55"
# AWF's credential-steered dummy token, not an owner credential.
PROXY_CREDENTIAL = "ghu_" + "a" * 36


def configuration():
    return {"pins": PINS, "engine": {"id": "copilot", "version": PINS["copilot"], "bare": True,
                                    "harness": "compiler_perf_harness.cjs",
                                    "env": {"COMPILER_PERF_EXECUTION_DEADLINE_MS":
                                            "${{ steps.controller.outputs.execution_deadline_ms }}"},
                                    "mcp": {"tool-timeout": "5m"}},
            "budgets": {"job": 360, "setup": 60, "execution": 250, "finalization": 40},
            "hooks": {"pre-steps": "setup", "pre-agent-steps": "start-service", "post-steps": "handoff"},
            "mcp-servers": {"controller": {"type": "http", "url": f"http://127.0.0.1:{bridge.PORT}/mcp",
                                          "headers": {"Authorization": "Bearer ${{ steps.controller.outputs.token }}"},
                                          "allowed": ["investigate"]}}}


def validate_layout(protected, workspace, home, runner_temp):
    """Stock AWF mounts both /tmp aliases and selected home/system paths."""
    reachable = [workspace, runner_temp, "/tmp", "/usr", "/bin", "/sbin", "/lib", "/lib64", "/opt", "/dev", "/sys"]
    reachable += [home + "/" + part for part in
                  (".copilot", ".cache", ".config", ".local", ".anthropic", ".claude",
                   ".cargo", ".rustup", ".npm", ".nvm", "work/_tool")]
    require(all(not overlaps(PurePosixPath(p), PurePosixPath(r)) for p in protected for r in reachable),
            "Protected custody overlaps a stock AWF mount")


def validate_container(value, protected, workspace, *, resolve_source=PurePosixPath):
    """Inspect Docker's effective inventory, including unadvertised default mounts."""
    require(type(value) is dict and value["Name"] == "/awf-agent" and value["State"]["Running"]
            and value["Config"]["Image"] == AGENT_IMAGE and mem.digest(value["Id"]),
            "Pinned AWF agent is unavailable")
    host = value["HostConfig"]
    require(host["Privileged"] is False and host["PidMode"] != "host" and host["IpcMode"] != "host"
            and host["NetworkMode"] not in ("host", "none", "bridge", "default")
            and len(value["NetworkSettings"]["Networks"]) == 1
            and {cap.removeprefix("CAP_") for cap in host.get("CapAdd") or []} <= {"SYS_CHROOT", "SYS_ADMIN"}
            and "no-new-privileges:true" in host["SecurityOpt"], "Unsafe AWF namespace/capabilities")
    mounted_workspace = False
    mounts = []
    for mount in value["Mounts"]:
        require(mount["Type"] in ("bind", "tmpfs"), "Unknown AWF mount authority")
        if mount["Type"] == "tmpfs":
            mounts.append(mount)
            continue
        source = resolve_source(mount["Source"])
        require(source.is_absolute() and all(not overlaps(source, PurePosixPath(p)) for p in protected),
                "Actual AWF mount exposes controller custody")
        require("docker.sock" not in str(source) and source.name not in
                (".git-credentials", "hosts.yml", "identity.txt"), "Actual AWF mount exposes credentials/socket")
        mounted_workspace |= source == PurePosixPath(workspace)
        mounts.append(dict(mount, Source=str(source)))
    require(mounted_workspace, "Actual AWF public workspace mount missing")
    for item in value["Config"]["Env"]:
        key, separator, data = item.partition("=")
        require(separator, "Malformed AWF environment")
        if key != "AWF_ONE_SHOT_TOKENS" and re.search(r"TOKEN|SECRET|PASSWORD|PRIVATE_KEY|CREDENTIAL|API_KEY", key):
            require(not data or key in ("COPILOT_GITHUB_TOKEN", "COPILOT_TOKEN", "COPILOT_API_KEY",
                                       "COPILOT_PROVIDER_API_KEY") and data == PROXY_CREDENTIAL,
                    "Actual AWF environment exposes a credential")
    return mounts


class Runtime:
    def __init__(self, controller, workspace):
        self.controller, self.workspace = controller, Path(workspace).absolute()
        self.protected = [controller.root, controller.repository, controller.transport.journal.repository,
                          Path(__file__).resolve().parent, Path(sys.prefix).resolve()]

    def setup(self, request, job_started_at, *, agent_home=None):
        controller = self.controller
        require(controller.session is None, "Existing owner requires reconciliation, not setup replay")
        require(e.unredirected(self.workspace) and self.workspace.is_dir()
                and not list(self.workspace.iterdir()), "Agent workspace must be an empty disposable directory")
        require(all(not overlaps(self.workspace.resolve(), Path(p).resolve()) for p in self.protected),
                "Public workspace overlaps protected custody")
        require(job_started_at <= controller.now(), "Invalid enclosing job start")
        clocks = controller.load("clocks") if (controller.root / "clocks.json").exists() else {}
        if "setup" not in clocks:
            clocks["setup"] = {"start": job_started_at.isoformat(),
                               "deadline": (job_started_at + timedelta(seconds=w.SETUP_SECONDS)).isoformat(),
                               "last_seen": job_started_at.isoformat(), "remaining_seconds": w.SETUP_SECONDS}
            controller.save("clocks", clocks)
        result = controller.preflight(request)
        if result["action"] != "reserve":
            return result
        with controller.budget("setup", w.SETUP_SECONDS):
            exported = bridge.export_source(controller.repository, request["base_sha"],
                                            self.workspace / "source", self.protected)
            controller.save("runtime", {"workspace": str(self.workspace), "export": exported,
                                         "agent_home": agent_home or str(Path.home()),
                                         "pins": PINS, "service_started_at": None, "ready": False,
                                         "inputs": {"policy": m.fingerprint_inputs(m.POLICY),
                                                    "sdk": m.file_record(controller.repository / "global.json"),
                                                    "helpers": [m.file_record(p) for p in
                                                                Path(__file__).resolve().parent.iterdir()
                                                                if p.suffix in (".py", ".cjs")]}})
        return result

    def arm(self):
        controller = self.controller
        require(controller.session["phase"] == "ready", "Prepare before starting the service")
        record = (controller.load("runtime") if (controller.root / "runtime.json").exists() else
                  {"workspace": str(self.workspace), "pins": PINS, "service_started_at": None, "ready": False})
        require(record["workspace"] == str(self.workspace) and record["pins"] == PINS, "Runtime identity changed")
        if record["service_started_at"] is None:
            record["service_started_at"] = controller.now().isoformat()
            record["execution_deadline_ms"] = int((controller.now() + timedelta(
                seconds=w.ACTIVE_SECONDS + w.STARTUP_SECONDS)).timestamp() * 1000)
            controller.session["ready_at"] = record["service_started_at"]
            controller.save("session", controller.session)
            controller.save("runtime", record)
        return record

    def agent_ready(self, probe):
        controller = self.controller
        record = controller.load("runtime")
        if record["ready"]:
            controller.poll_active()
            return
        started = mem.timestamp(record["service_started_at"])
        require(0 <= (controller.now() - started).total_seconds() < READINESS_SECONDS,
                "Agent readiness exhausted execution slack")
        try:
            probe()
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            controller.save("runtime-error", {"reason": c.sanitize(str(error))})
            raise
        require(0 <= (controller.now() - started).total_seconds() < READINESS_SECONDS,
                "Agent readiness probe exhausted execution slack")
        controller.start_active()
        record["ready"] = True
        controller.save("runtime", record)
        controller.checkpoint()

    def inspect_agent(self):
        value = subprocess.run(["docker", "inspect", "awf-agent"], capture_output=True, check=False,
                               timeout=15, env=c.tool_environment())
        require(value.returncode == 0 and len(value.stdout) <= mem.LIMITS["file_bytes"], "Denied Docker inspection")
        documents = mem.decoded(value.stdout)
        require(type(documents) is list and len(documents) == 1, "Ambiguous AWF agent")
        sources = [p["Source"] for p in documents[0]["Mounts"] if p["Type"] == "bind"]
        require(sources and all(type(p) is str and PurePosixPath(p).is_absolute() for p in sources),
                "Invalid AWF mount sources")
        # Stock sudo AWF creates root-only mount parents. Read metadata, not contents.
        resolved = subprocess.run(["/usr/bin/sudo", "-n", "--", "/usr/bin/readlink",
                                   "--canonicalize-existing", "--zero", "--", *sources],
                                  capture_output=True, check=False, timeout=15, env=c.tool_environment())
        require(resolved.returncode == 0 and len(resolved.stdout) <= mem.LIMITS["file_bytes"],
                "Denied AWF mount canonicalization")
        paths = resolved.stdout.decode("utf-8").split("\0")
        require(len(paths) == len(sources) + 1 and paths[-1] == ""
                and all(PurePosixPath(p).is_absolute() for p in paths[:-1]), "Incomplete AWF mount canonicalization")
        canonical = dict(zip(sources, map(PurePosixPath, paths[:-1])))
        mounts = validate_container(documents[0], [str(p) for p in self.protected], str(self.workspace),
                                    resolve_source=canonical.__getitem__)
        config = w.read_input(Path(os.environ["RUNNER_TEMP"]) / "gh-aw" / "awf-config.json")
        require(config["container"]["imageTag"] == PINS["awf"].lstrip("v")
                and config["apiProxy"]["enabled"] is True and config["apiProxy"]["enableTokenSteering"] is True,
                "Stock authenticated model proxy is required")
        proxy = subprocess.run(["docker", "inspect", "awf-api-proxy"], capture_output=True, check=False,
                               timeout=15, env=c.tool_environment())
        require(proxy.returncode == 0 and len(proxy.stdout) <= mem.LIMITS["file_bytes"], "Model proxy unavailable")
        proxy = mem.decoded(proxy.stdout)
        require(len(proxy) == 1 and proxy[0]["State"]["Running"] and proxy[0]["Config"]["Image"] ==
                "ghcr.io/github/gh-aw-firewall/api-proxy:0.25.55", "Pinned model proxy unavailable")
        self.controller.save("runtime-container", {"id": documents[0]["Id"],
                                                   "last_seen_at": self.controller.now().isoformat(),
                                                   "mounts": mounts, "pins": PINS,
                                                   "proxy_id": proxy[0]["Id"], "config": m.file_record(
                                                       Path(os.environ["RUNNER_TEMP"]) / "gh-aw" / "awf-config.json")})

    def observe_engine(self):
        """Use host Docker inventory, never an agent-supplied completion timestamp."""
        controller = self.controller
        if (controller.root / "runtime-engine-exit.json").exists():
            return
        record = controller.load("runtime-container")
        require(mem.digest(record["id"]), "Invalid owned engine identity")
        result = subprocess.run(["docker", "ps", "--all", "--no-trunc", "--filter", "id=" + record["id"],
                                 "--format", "{{json .}}"], capture_output=True, check=False,
                                timeout=15, env=c.tool_environment())
        require(result.returncode == 0 and len(result.stdout) <= mem.LIMITS["file_bytes"],
                "Denied engine lifetime inspection")
        rows = result.stdout.splitlines()
        require(len(rows) <= 1, "Ambiguous engine lifetime")
        state = mem.decoded(rows[0]) if rows else None
        require(state is None or type(state) is dict and state["ID"] == record["id"]
                and state["State"] in ("running", "exited", "dead", "removing"), "Invalid engine lifetime")
        require(mem.timestamp(record["last_seen_at"]) <= controller.now(), "Engine clock moved backwards")
        if state is not None and state["State"] == "running":
            record["last_seen_at"] = controller.now().isoformat()
            controller.save("runtime-container", record)
        else:
            # Conservatively include the observation gap and AWF teardown in cleanup.
            controller.save("runtime-engine-exit", {"id": record["id"], "at": record["last_seen_at"]})

    def verify_tools(self):
        for command, expected in ((["copilot", "--version"], PINS["copilot"]), (["awf", "--version"], PINS["awf"])):
            result = subprocess.run(command, capture_output=True, check=False, timeout=15, env=c.tool_environment())
            require(result.returncode == 0 and re.search(r"(?<![\w.+-])v?" + re.escape(expected.lstrip("v")) +
                    r"\.?(?=\s|$)", result.stdout.decode("utf-8")), "Installed runtime pin differs")
        record = self.controller.load("runtime")
        m.verify_files([record["inputs"]["sdk"], *record["inputs"]["helpers"]])
        require(record["inputs"]["policy"] == m.fingerprint_inputs(m.POLICY), "Frozen runtime policy changed")

    def prepare(self):
        try:
            self.verify_tools()
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            return self.controller.finish("blocked_setup", "Runtime prerequisite: " + c.sanitize(str(error)))
        return self.controller.investigate()

    def install_harness(self):
        source = Path(__file__).with_name("compiler_perf_harness.cjs")
        destination = Path(os.environ["RUNNER_TEMP"]) / "gh-aw" / "actions" / source.name
        require(e.unredirected(destination, missing_ok=True) and destination.parent.is_dir()
                and (destination.parent / "copilot_harness.cjs").is_file(), "Stock harness setup unavailable")
        if not destination.exists():
            with destination.open("xb") as stream:
                stream.write(source.read_bytes())
        require(m.file_record(source)["sha256"] == m.file_record(destination)["sha256"], "Runtime harness changed")

    def wait_service(self, child, probe):
        controller = self.controller
        while True:
            require(child.poll() is None, "Owned controller service exited before readiness")
            controller.remaining("setup", w.SETUP_SECONDS)
            if probe():
                require(child.poll() is None, "Owned controller service exited during readiness")
                return
            time.sleep(.1)

    def abort_startup(self, child, error):
        controller = self.controller
        reason = "Owned service startup failed: " + (c.sanitize(str(error)) or "cancelled")
        controller.save("runtime-error", {"reason": reason})
        try:
            try:
                remaining = controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
            finally:
                if not (controller.root / "runtime-stop.json").exists():
                    controller.save("runtime-stop", {"reason": reason, "requested_at": controller.now().isoformat()})
                if child is not None and child.poll() is None:
                    child.terminate()
            if child is not None:
                child.wait(timeout=remaining)
            with e.owned_lock(controller.root):
                require(not (controller.root / "runtime-service.json").exists()
                        or controller.load("runtime-service")["state"] != "cleanup_failed",
                        "Owned service cleanup incomplete")
                if controller.session is not None:
                    controller.session = controller.load("session")
                    require(not bridge.Bridge(controller).blocked, "Worker requires owner reconciliation")
                    owner = controller.session.get("discovery") or controller.session["attempt"]
                    if owner["outcome"] is None:
                        controller.finish("cancelled" if isinstance(error, KeyboardInterrupt) else "blocked_setup", reason)
                controller.save("runtime-service", {"state": "stopped"})
        except (KeyboardInterrupt, ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError):
            controller.save("runtime-service", {"state": "cleanup_failed"})
            raise

    def export(self, destination):
        """Publisher transport is never an optimizer-writable workspace or GH_AW_SAFE_OUTPUTS."""
        controller = self.controller
        with controller.budget("finalization", w.FINALIZATION_SECONDS):
            current, state = controller.current_attempt()
            sealed = controller.load("approval")
            source = controller.root / "output"
            require({p.name for p in source.iterdir()} == {"safe-output.json", "proposal.patch"},
                    "Unexpected publisher output")
            m.verify_files(sealed["files"])
            payload = w.read_input(source / "safe-output.json")
            patch = e.git(Path(state["tree"]), "format-patch", "--stdout", "--no-stat", "--no-signature",
                          "-1", sealed["head"])
            require((source / "proposal.patch").read_bytes() == patch, "Publication patch changed")
            binding = w.approval(current, payload, state["source_tree"],
                                 m.file_record(source / "proposal.patch")["sha256"])
            require(all(sealed[k] == value for k, value in binding.items()), "Publication seal changed")
            require(not (controller.root / "runtime-error.json").exists(), "Runtime ended unhealthy")
            # Reapply independently even if active time expired after authorization.
            with tempfile.TemporaryDirectory(prefix="handoff-", dir=controller.root) as directory:
                checked = e.prepare_candidate(controller.repository, Path(directory) / "candidate",
                                              state["request"],
                                              (controller.candidate_root() / "proposal.patch").read_bytes())
                require(checked["source_tree"] == state["source_tree"], "Handoff candidate changed")
            destination = Path(destination).absolute()
            require(e.unredirected(destination, missing_ok=True)
                    and not overlaps(destination, self.workspace), "Unsafe publisher destination")
            if (controller.root / "runtime-container.json").exists():
                mounts = controller.load("runtime-container")["mounts"]
                require(all(not overlaps(PurePosixPath(destination.as_posix()), PurePosixPath(p["Source"]))
                            for p in mounts if p["Type"] == "bind"), "Publisher destination overlaps an agent mount")
            else:
                require(controller.transport.scope == "local_fixture", "Actual runtime mount receipt is required")
            intent = {"destination": str(destination), "binding": binding}
            if (controller.root / "runtime-export.json").exists():
                require(controller.load("runtime-export") == intent, "Publisher export ownership changed")
            else:
                require(not destination.exists(), "Unowned publisher destination")
                controller.save("runtime-export", intent)
            if not destination.exists():
                c.private_directory(destination)
            require(destination.is_dir() and {p.name for p in destination.iterdir()} <=
                    {"safe-output.json", "proposal.patch"}, "Unexpected publisher files")
            for path in source.iterdir():
                target = destination / path.name
                if target.exists():
                    require(e.unredirected(target) and m.file_record(target)["sha256"] == m.file_record(path)["sha256"],
                            "Publisher export changed; preserved")
                else:
                    with target.open("xb") as stream:
                        stream.write(path.read_bytes())
            files = [m.file_record(p) for p in sorted(destination.iterdir())]
            require([r["sha256"] for r in files] == [r["sha256"] for r in sealed["files"]],
                    "Publisher copy changed")
            return {"scope": controller.transport.scope, "files": files, "binding": binding}

    def finalize(self, *, publisher=None):
        if publisher is not None:
            require(not (self.controller.root / "runtime-error.json").exists(),
                    "Runtime unhealthy; publication blocked")
        with self.controller.budget("finalization", w.FINALIZATION_SECONDS):
            return self.controller.finalize(publisher=publisher)

    def handoff(self, destination=None, *, engine_outcome="success"):
        controller = self.controller
        require(engine_outcome in ("success", "failure", "cancelled", "skipped"), "Invalid stock engine outcome")
        if engine_outcome != "success":
            controller.save("runtime-error", {"reason": "Stock engine ended " + engine_outcome})
        if not (controller.root / "runtime-stop.json").exists():
            controller.save("runtime-stop", {"reason": "Engine execution ended",
                                             "requested_at": controller.now().isoformat()})
        if controller.transport.scope != "local_fixture":
            try:
                self.observe_engine()
                require((controller.root / "runtime-engine-exit.json").exists(), "Engine has not exited")
            except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
                controller.save("runtime-error", {"reason": "Engine handoff inspection failed: " + c.sanitize(str(error))})
                if not (controller.root / "runtime-engine-exit.json").exists():
                    inspected = (controller.root / "runtime-container.json").exists()
                    record = controller.load("runtime-container" if inspected else "runtime")
                    controller.save("runtime-engine-exit", {
                        "at": record["last_seen_at" if inspected else "service_started_at"],
                        "observed": False})
        requested = mem.timestamp(controller.load("runtime-stop")["requested_at"])
        if (controller.root / "runtime-engine-exit.json").exists():
            requested = min(requested, mem.timestamp(controller.load("runtime-engine-exit")["at"]))
        reader = bridge.Bridge(controller)
        while controller.load("runtime-service")["state"] not in ("stopped", "cleanup_failed"):
            elapsed = (controller.now() - requested).total_seconds()
            require(0 <= elapsed < w.FINALIZATION_SECONDS, "Handoff cleanup deadline reached")
            clocks = controller.load("clocks")
            if "finalization" in clocks:
                reader._read_deadline(clocks["finalization"], "finalization", w.FINALIZATION_SECONDS)
            time.sleep(.1)
        require(controller.load("runtime-service")["state"] == "stopped", "Owned service cleanup incomplete")
        if (controller.root / "runtime-error.json").exists():
            raise ValueError("Runtime unhealthy: " + controller.load("runtime-error")["reason"])
        controller.session = controller.load("session")
        with controller.budget("finalization", w.FINALIZATION_SECONDS):
            result = {"status": controller.status(), "artifacts": controller.stage_evidence(), "publication": None}
            controller.checkpoint()
            if (controller.root / "approval.json").exists():
                require(destination is not None, "A protected publisher destination is required")
                result["publication"] = self.export(destination)
            return result


async def probe_service(token):
    import httpx

    try:
        async with httpx.AsyncClient(headers={"Authorization": "Bearer " + token}, timeout=2) as http:
            response = await http.post(f"http://127.0.0.1:{bridge.PORT}/mcp",
                headers={"Accept": "application/json, text/event-stream"},
                json={"jsonrpc": "2.0", "id": "readiness", "method": "initialize", "params": {
                    "protocolVersion": "2025-11-25", "capabilities": {},
                    "clientInfo": {"name": "protected-runtime-probe", "version": "1"}}})
    except (httpx.ConnectError, httpx.TimeoutException):
        return False
    require(response.status_code == 200, "Protected service rejected readiness probe")
    result = mem.decoded(response.content)
    require(result["result"]["serverInfo"]["name"] == "compiler-performance", "Unexpected readiness endpoint")
    return True


def serve(adapter, *, sockets=None, launch=bridge._launch, identify=c.snapshot_process):
    import uvicorn

    controller = adapter.controller
    with e.owned_lock(controller.root / "agent-server"):
        client = bridge.Bridge(controller, launch=launch, identify=identify,
                               ready=lambda: adapter.agent_ready(adapter.inspect_agent))
        token = bridge.endpoint_token(controller.root)

        def shutdown():
            if not (controller.root / "runtime-stop.json").exists() \
                    and not (controller.root / "runtime-engine-exit.json").exists():
                controller.save("runtime-error", {"reason": "Owned HTTP service interrupted"})
            return client.shutdown("Owned HTTP service shutdown; incomplete investigation")

        server = uvicorn.Server(uvicorn.Config(bridge.create_app(client, token, shutdown=shutdown), host="127.0.0.1",
                                port=bridge.PORT, access_log=False, log_level="warning", proxy_headers=False,
                                server_header=False, limit_concurrency=8))
        stop = threading.Event()

        def monitor():
            try:
                while not stop.wait(1):
                    with client.lock:
                        if (controller.root / "runtime-container.json").exists():
                            adapter.observe_engine()
                        client.tick()
                        if server.started and controller.load("runtime-service")["state"] == "launching":
                            controller.save("runtime-service", {"state": "serving"})
                        record = controller.load("runtime")
                        if not record["ready"] and (controller.now() - mem.timestamp(
                                record["service_started_at"])).total_seconds() >= READINESS_SECONDS:
                            client._request_finish("blocked_setup", "Agent readiness exhausted execution slack")
                        if controller.session["phase"] == "accepted" and not (controller.root / "approval.json").exists():
                            if client.worker is None:
                                require(client.receipt is None or client.receipt["operation"] != "authorize",
                                        "Trusted output authorization failed")
                                controller.poll_active()
                                controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
                                client._start("authorize", None)
                        if (controller.root / "runtime-stop.json").exists():
                            controller.load("runtime-stop")
                            break
                        if (controller.root / "runtime-engine-exit.json").exists():
                            break
                        if client.worker is None and "finalization" in controller.load("clocks"):
                            controller.remaining("finalization", w.FINALIZATION_SECONDS)
            except (KeyboardInterrupt, ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
                controller.save("runtime-error", {"reason": c.sanitize(str(error)) or "Finalization deadline"})
            finally:
                stopped = False
                try:
                    stopped = client.shutdown("Owned service shutdown; incomplete investigation")
                except (KeyboardInterrupt, ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
                    controller.save("runtime-error", {"reason": "Owned cleanup failed: " + c.sanitize(str(error))})
                controller.save("runtime-service", {"state": "stopped" if stopped else "cleanup_failed"})
                server.should_exit = True

        # Uvicorn owns SIGTERM; lifespan and the monitor share idempotent owned cleanup.
        worker = threading.Thread(target=monitor, name="compiler-perf-runtime", daemon=True)
        worker.start()
        try:
            server.run(sockets=sockets)
        finally:
            stop.set()
            worker.join(timeout=w.FINALIZATION_SECONDS)
        require(not worker.is_alive() and controller.load("runtime-service")["state"] == "stopped",
                "Owned service cleanup incomplete")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("command", choices=("configuration", "setup", "start-service", "serve", "handoff", "finalize"))
    for name in ("root", "repository", "journal", "workspace", "input", "job-started-at", "destination"):
        parser.add_argument("--" + name)
    parser.add_argument("--engine-outcome", choices=("success", "failure", "cancelled", "skipped"))
    args = parser.parse_args(argv)
    original_environment, original_temp = dict(os.environ), tempfile.tempdir
    try:
        if args.command == "configuration":
            result = configuration()
        else:
            require(sys.platform == "linux", "Protected Actions runtime requires Linux; no host fallback")
            require(sys.flags.isolated, "Protected runtime requires Python -I")
            from transport import JournalTransport
            require(all((args.root, args.repository, args.journal, args.workspace)), "Missing protected layout")
            protected = [str(bridge._local_directory(p, missing=True))
                         for p in (args.root, args.repository, args.journal,
                                   Path(__file__).resolve().parent, sys.prefix)]
            agent_home = str(Path.home())
            if args.command != "serve":
                validate_layout(protected, args.workspace, agent_home, os.environ["RUNNER_TEMP"])
            controller = w.Controller(args.root, args.repository, None, None)
            if args.command == "serve":
                validate_layout(protected, args.workspace, controller.load("runtime")["agent_home"],
                                os.environ["RUNNER_TEMP"])
            environment = bridge.trusted_environment(controller)
            if "GITHUB_OUTPUT" in os.environ:
                environment["GITHUB_OUTPUT"] = os.environ["GITHUB_OUTPUT"]
            os.environ.clear()
            os.environ.update(environment)
            tempfile.tempdir = None
            controller.transport, controller.api = JournalTransport(args.journal), w.GitHub()
            adapter = Runtime(controller, args.workspace)
            if args.command == "setup":
                require(args.input and args.job_started_at, "Setup requires fixed input and enclosing job start")
                result = adapter.setup(w.read_input(args.input), mem.timestamp(args.job_started_at), agent_home=agent_home)
            elif args.command == "start-service":
                child, starting = None, False
                previous = signal.signal(signal.SIGTERM, lambda *_: (_ for _ in ()).throw(KeyboardInterrupt()))
                try:
                    with e.owned_lock(controller.root):
                        require(not (controller.root / "runtime-service.json").exists(),
                                "Existing service requires owner reconciliation")
                        starting = True
                        controller.remaining("setup", w.SETUP_SECONDS)
                        result = adapter.prepare()
                        require(result.get("phase") == "ready", result.get("reason", "Preparation blocked"))
                        adapter.install_harness()
                        adapter.arm()
                        (controller.root / "agent-server").mkdir(mode=0o700)
                        controller.save("runtime-service", {"state": "launching"})
                        command = [sys.executable, "-I", str(Path(__file__).resolve()), "serve", "--root", args.root,
                                   "--repository", args.repository, "--journal", args.journal, "--workspace", args.workspace]
                        environment = bridge.trusted_environment(controller)
                        with (controller.root / "service.log").open("xb") as log:
                            child = subprocess.Popen(command, cwd=controller.repository, env=environment,
                                                     stdin=subprocess.DEVNULL, stdout=log, stderr=log,
                                                     start_new_session=True)
                    adapter.wait_service(child, lambda: asyncio.run(probe_service(bridge.endpoint_token(controller.root))))
                    token = bridge.endpoint_token(controller.root)
                    print("::add-mask::" + token)
                    with open(os.environ["GITHUB_OUTPUT"], "a", encoding="utf-8") as stream:
                        stream.write("token=" + token + "\nexecution_deadline_ms=" +
                                     str(controller.load("runtime")["execution_deadline_ms"]) + "\n")
                    result = {"state": "serving"}
                except BaseException as error:
                    if starting:
                        adapter.abort_startup(child, error)
                    raise
                finally:
                    signal.signal(signal.SIGTERM, previous)
            elif args.command == "serve":
                serve(adapter)
                result = controller.status()
            elif args.command == "handoff":
                require(args.engine_outcome is not None, "Handoff requires the trusted Actions engine outcome")
                result = adapter.handoff(args.destination, engine_outcome=args.engine_outcome)
            else:
                result = adapter.finalize()
        print(mem.encoded(result).decode(), end="")
        status = result["status"] if type(result.get("status")) is dict else result
        if status.get("outcome") == "cancelled":
            return 130
        return 2 if status.get("action") == "blocked" or (status.get("outcome") or "").startswith("blocked_") else 0
    except KeyboardInterrupt:
        return 130
    except (ValueError, OSError, KeyError, TypeError, ImportError, subprocess.SubprocessError) as error:
        print(mem.encoded({"outcome": "blocked_setup", "reason": c.sanitize(str(error))}).decode(), end="")
        return 2
    finally:
        os.environ.clear()
        os.environ.update(original_environment)
        tempfile.tempdir = original_temp


if __name__ == "__main__":
    sys.exit(main())
