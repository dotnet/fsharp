"""Pinned runtime adapters: local Git/HTTP only, never hosted containment proof."""

from datetime import timedelta
import asyncio
from contextlib import contextmanager, nullcontext
import io
import json
import os
from pathlib import Path
from pathlib import PurePosixPath
import subprocess
import sys
import socket
import tempfile
import threading
import time
from types import SimpleNamespace
import unittest
from unittest import mock

import test_workflow_agent as fixtures
import test_trace_adapter
import agent_bridge as bridge
import workflow as w
import runtime


RUNNER_IDENTITY = {
    "ImageOS": "ubuntu24", "ImageVersion": "20261020.1.0", "RUNNER_NAME": "fixture-runner",
    "RUNNER_OS": "Linux", "RUNNER_ARCH": "X64", "GITHUB_RUN_ID": "123456789",
    "GITHUB_RUN_ATTEMPT": "2", "GITHUB_JOB": "agent",
    "PROCESSOR_IDENTIFIER": "portable fixture processor",
}


def container():
    return {
        "Id": "a" * 64, "Name": "/awf-agent", "State": {"Running": True},
        "Config": {"Image": "ghcr.io/github/gh-aw-firewall/agent:0.25.55",
                   # AWF 39b8a59: constants/placeholders.ts and tool-specific-environment.ts.
                   "Env": [key + "=ghu_" + "a" * 36 for key in
                           ("COPILOT_GITHUB_TOKEN", "COPILOT_TOKEN", "COPILOT_API_KEY",
                            "COPILOT_PROVIDER_API_KEY")] + ["COPILOT_MODEL=gpt-5.6-sol",
                           "AWF_ONE_SHOT_TOKENS=GITHUB_TOKEN,GH_TOKEN"]},
        "HostConfig": {"Privileged": False, "PidMode": "", "IpcMode": "private",
                       "NetworkMode": "awf-network", "CapAdd": ["SYS_CHROOT", "SYS_ADMIN"],
                       "SecurityOpt": ["no-new-privileges:true"]},
        "NetworkSettings": {"Networks": {"awf-network": {}}},
        "Mounts": [{"Type": "bind", "Source": source, "Destination": target, "RW": writable}
                   for source, target, writable in [
                       ("/tmp", "/host/tmp", True), ("/usr", "/host/usr", False),
                       ("/opt", "/host/opt", False),
                       ("/home/runner/.copilot", "/host/home/runner/.copilot", True),
                       ("/work/public", "/host/work/public", True)]],
    }


class RuntimeTests(unittest.TestCase):
    ready = fixtures.AgentTests.ready
    discovery = fixtures.AgentTests.discovery
    fixture = fixtures.AgentTests.fixture
    candidate = fixtures.fixtures.PublicationTests.candidate

    @contextmanager
    def serving(self, adapter, launch, *, expected_error=None):
        controller, errors = adapter.controller, []

        class SynchronousSession(w.ActiveSession):
            # Synthetic workers run inline on service threads, not in owned processes.
            def __enter__(self):
                self.start = self.last = self.clock()
                self.poll()
                return self

        with socket.socket() as listener:
            listener.bind(("127.0.0.1", 0))
            port = listener.getsockname()[1]
            (controller.root / "agent-server").mkdir()
            controller.save("runtime-service", {"state": "launching"})
            token = bridge.endpoint_token(controller.root)
            def run():
                try:
                    runtime.serve(adapter, sockets=[listener], launch=launch, identify=fixtures.identify)
                except BaseException as error:
                    errors.append(error)
            with mock.patch.object(bridge, "PORT", port), mock.patch.object(adapter, "inspect_agent") as probe, \
                    mock.patch.object(w, "ActiveSession", SynchronousSession):
                thread = threading.Thread(target=run)
                thread.start()
                try:
                    end = time.monotonic() + 15
                    while not asyncio.run(runtime.probe_service(token)) and time.monotonic() < end:
                        self.assertTrue(thread.is_alive(), errors)
                        time.sleep(.05)
                    yield port, token, probe
                finally:
                    controller.save("runtime-stop", {"reason": "Owned test cleanup",
                                                     "requested_at": controller.now().isoformat()})
                    thread.join(timeout=30)
                    self.assertFalse(thread.is_alive(), errors)
                if expected_error is None:
                    self.assertEqual(errors, [])
                else:
                    self.assertEqual(len(errors), 1, errors)
                    self.assertRegex(str(errors[0]), expected_error)

    def test_pinned_configuration_uses_stock_hooks_and_full_envelopes(self):
        import jsonschema

        config = runtime.configuration()
        self.assertEqual(config["pins"], {
            "gh_aw": "v0.76.1", "gh_aw_commit": "58d1bedbb7200f59c2d224151339e38fd8687d05",
            "copilot": "1.0.52", "awf": "v0.25.55", "gateway": "v0.3.19"})
        # v0.3.19 rejects the renderer's sessionTimeout property; its default is 6h.
        self.assertEqual(config["engine"]["mcp"], {"tool-timeout": "5m"})
        # GH AW 58d1bed, mcp_renderer.go emits camelCase gateway properties.
        rendered = {"port": 8080, "domain": "host.docker.internal", "apiKey": "fixture"}
        for key, value in config["engine"]["mcp"].items():
            rendered[{"session-timeout": "sessionTimeout", "tool-timeout": "toolTimeout"}[key]] = (
                int(value[:-1]) * 60 if key == "tool-timeout" else value)
        schema = json.loads((Path(__file__).parent / "fixtures" / "mcpg-v0.3.19-gateway.schema.json").read_text())
        jsonschema.validate(rendered, schema)
        with self.assertRaises(jsonschema.ValidationError):
            jsonschema.validate(dict(rendered, sessionTimeout="6h"), schema)
        self.assertEqual(config["engine"]["harness"], "compiler_perf_harness.cjs")
        self.assertEqual(config["budgets"], {"job": 360, "setup": 60, "execution": 250, "finalization": 40})
        self.assertEqual(config["hooks"], {"pre-steps": "setup", "pre-agent-steps": "start-service",
                                          "post-steps": "handoff"})
        command = bridge.worker_command(SimpleNamespace(root=Path("root"), repository=Path("source"),
            transport=SimpleNamespace(journal=SimpleNamespace(repository=Path("journal")))), "profile")
        self.assertEqual(command[:3], [sys.executable, "-I", str(Path(w.__file__).resolve())])
        self.assertNotIn("docker run", str(config))
        result = subprocess.run([sys.executable, runtime.__file__, "configuration"], capture_output=True, timeout=15)
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertEqual(json.loads(result.stdout), config)

    def test_service_probe_handles_refused_connection_without_starting_active_time(self):
        with socket.socket() as listener:
            listener.bind(("127.0.0.1", 0))
            port = listener.getsockname()[1]
        with mock.patch.object(bridge, "PORT", port):
            self.assertFalse(asyncio.run(runtime.probe_service("a" * 64)))

    @unittest.skipUnless(sys.platform == "linux" and os.environ.get("COMPILER_PERF_LIVE") == "1",
                         "set COMPILER_PERF_LIVE=1 for actual Linux collector IPC")
    def test_profile_ipc_scratch_is_protected_separate_and_removed(self):
        """Real collector/CLR/tools; only compiler workload schema and binlog timings are synthetic."""
        with self.discovery() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            builder = test_trace_adapter.TraceAdapterTests()
            builder.work = controller.root / "child-build"
            builder.work.mkdir()
            child = builder.build_child("net8.0")
            response, output = controller.root / "replay.rsp", controller.root / "output.dll"
            response.write_text(f"{controller.root / 'child.pid'}\n{output}\n")
            compiler = {"files": {"fsc.dll": w.m.file_record(child),
                "fsc.runtimeconfig.json": w.m.file_record(child.with_suffix(".runtimeconfig.json"))}}
            workload = {"sources": [w.m.file_record(test_trace_adapter.FIXTURE)], "generated": [],
                        "outputs": [str(output)], "assembly_output": str(output),
                        "checkout": str(controller.root), "cwd": str(controller.root),
                        "response_file": w.m.file_record(response)}
            manifest = {"owned_root": str(controller.root), "baseline_compiler": compiler,
                        "invariants": {"workload": workload, "toolchain": {
                            "dotnet": w.m.file_record(os.environ["COMPILER_PERF_DOTNET"])}}}
            inputs = [*compiler["files"].values(), workload["response_file"], *workload["sources"]]
            timing_root = controller.root / "synthetic-timings"
            timing_root.mkdir()
            timings = fixtures.artifact_profile(timing_root)["timings"]
            controller.session.update(phase="ready", ready_at=controller.now().isoformat())
            controller.start_active()
            with mock.patch.object(controller, "frozen_manifest", return_value=manifest), \
                    mock.patch.object(w.c, "frozen_inputs", return_value=inputs), \
                    mock.patch.object(w.c, "diagnostic_timings", return_value=timings), \
                    mock.patch.object(w.c, "collect_profile", wraps=w.c.collect_profile) as collect:
                controller.profile()
            self.assertEqual(controller.session["phase"], "awaiting_decision",
                             w.c.read_bounded_json(controller.root / "private/profile/profile-summary.json"))
            scratch = collect.call_args.kwargs["ipc_root"]
            self.assertEqual(scratch.parent, controller.root.parent / "ipc")
            self.assertIn(scratch.parent, adapter.protected)
            self.assertFalse(scratch.exists())
            self.assertFalse(scratch.is_relative_to(controller.root / "private"))
            evidence = w.c.read_bounded_json(controller.session["profile"]["path"])
            self.assertEqual(evidence["status"], "collected")
            self.assertEqual(Path(evidence["preflight"]["socket"]).parent, scratch / "tmp")
            self.assertGreater(evidence["trace"]["cpu_sample_weight"], 0)
            self.assertTrue(evidence["counters"]["observations"])
            self.assertFalse((controller.root / "measurement.lock").exists())

    def test_sealed_record_reads_serialize_with_background_checkpoints(self):
        with self.discovery() as (_, controller, _, _):
            controller.save("snapshot", {"state": "before"})
            reading, release, writing, completed = (threading.Event() for _ in range(4))
            read_input, values = w.read_input, []
            def read(path):
                value = read_input(path)
                reading.set()
                self.assertTrue(release.wait(10))
                return value
            def write():
                writing.set()
                controller.save("snapshot", {"state": "after"})
                completed.set()
            with mock.patch.object(w, "read_input", side_effect=read):
                reader = threading.Thread(target=lambda: values.append(controller.load("snapshot")))
                writer = threading.Thread(target=write)
                reader.start()
                try:
                    self.assertTrue(reading.wait(10))
                    writer.start()
                    self.assertTrue(writing.wait(10))
                    self.assertFalse(completed.wait(.1))
                finally:
                    release.set()
                    reader.join(10)
                    if writer.ident is not None:
                        writer.join(10)
            self.assertFalse(reader.is_alive() or writer.is_alive())
            self.assertTrue(completed.is_set())
            self.assertEqual(values, [{"state": "before"}])
            self.assertEqual(controller.load("snapshot"), {"state": "after"})
            path = controller.root / "snapshot.json"
            path.write_bytes(path.read_bytes().replace(b"after", b"forged"))
            with self.assertRaisesRegex(ValueError, "custody"):
                controller.load("snapshot")

    def test_installed_pin_denial_precedes_any_bootstrap(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            for result in (subprocess.CompletedProcess([], 1, b"", b"denied"),
                           subprocess.CompletedProcess([], 0, b"1.0.999", b"")):
                with self.subTest(result=result), mock.patch.object(runtime.subprocess, "run", return_value=result), \
                        self.assertRaisesRegex(ValueError, "pin"):
                    adapter.verify_tools()
            with mock.patch.object(adapter, "verify_tools", side_effect=ValueError("Denied pinned AWF")), \
                    mock.patch.object(controller, "investigate") as prepare:
                result = adapter.prepare()
            self.assertEqual(result["outcome"], "blocked_setup")
            self.assertIn("Denied pinned AWF", result["reason"])
            prepare.assert_not_called()

    def test_exact_pinned_cli_stdout_and_near_version_denial(self):
        with self.discovery() as (repo, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            controller.save("runtime", {"inputs": {"policy": w.m.fingerprint_inputs(w.m.POLICY),
                            "sdk": w.m.file_record(repo.root / "global.json"), "helpers": []}})
            # Checksum-verified v1.0.52 release stdout; AWF uses Commander .version().
            copilot = b"GitHub Copilot CLI 1.0.52.\nRun 'copilot update' to check for updates.\n"
            for version in (b"1.0.52", b"1.0.520", b"11.0.52", b"1.0.52.1", b"1.0.52-rc.1"):
                for tool in ("copilot", "awf"):
                    outputs = [copilot.replace(b"1.0.52", version), b"0.25.55\n"] if tool == "copilot" else [
                        copilot, version.replace(b"1.0.52", b"0.25.55") + b"\n"]
                    with self.subTest(tool=tool, version=version), mock.patch.object(
                            runtime.subprocess, "run", side_effect=[
                                subprocess.CompletedProcess([], 0, output, b"") for output in outputs]):
                        if version == b"1.0.52":
                            adapter.verify_tools()
                        else:
                            with self.assertRaisesRegex(ValueError, "pin"):
                                adapter.verify_tools()

    def test_linux_capabilities_probe_custody_and_network_through_existing_isolation(self):
        with self.ready() as (_, controller, _, _):
            is_file = Path.is_file
            for denied in (False, True):
                with self.subTest(denied=denied), mock.patch.object(w.sys, "platform", "linux"), \
                        mock.patch.object(Path, "is_file", lambda p:
                                          True if p.name in ("bwrap", "dotnet") else is_file(p)), \
                        mock.patch.object(w.e, "Isolation") as isolation, mock.patch.object(w.c, "verify_tools") as tools:
                    isolation.return_value.command.side_effect = [
                        {"status": "collected", "stdout": w.e.runner.SDK},
                        {"status": "blocked_collection" if denied else "collected", "stdout": "isolated"}]
                    if denied:
                        with self.assertRaisesRegex(ValueError, "deny-network"):
                            w.capabilities(controller.root, controller.repository)
                        tools.assert_not_called()
                    else:
                        w.capabilities(controller.root, controller.repository)
                        tools.assert_called_once()
                    command = isolation.return_value.command.call_args.args[0]
                    self.assertEqual(command[:2], ["/usr/bin/python3", "-c"])
                    self.assertEqual(command[3:], [str(controller.root / "key"), str(controller.root / "host-receipt")])
                    compile(command[2], "<isolated-capability-probe>", "exec")
                    self.assertIn("CapEff:", command[2])
                    self.assertIn("connect_ex", command[2])

    def test_actual_mount_and_credential_inventory_fails_closed(self):
        protected = ["/var/lib/compiler-perf/source", "/var/lib/compiler-perf/custody",
                     "/var/lib/compiler-perf/journal.git", "/var/lib/compiler-perf/publisher"]
        runtime.validate_container(container(), protected, "/work/public")
        for key in ("COPILOT_GITHUB_TOKEN", "COPILOT_TOKEN", "COPILOT_API_KEY",
                    "COPILOT_PROVIDER_API_KEY", "GITHUB_TOKEN"):
            value = container()
            value["Config"]["Env"] = [item for item in value["Config"]["Env"] if not item.startswith(key + "=")]
            value["Config"]["Env"].append(key + "=gho_" + "a" * 36)
            with self.subTest(credential=key), self.assertRaisesRegex(ValueError, "credential"):
                runtime.validate_container(value, protected, "/work/public")
        for mutation in ("custody", "broad", "socket", "credentials", "host-network", "host-pid",
                         "privileged", "wrong-image", "extra-network", "stopped"):
            value = container()
            if mutation in ("custody", "broad", "socket"):
                value["Mounts"].append({"Type": "bind", "Source": {
                    "custody": protected[1], "broad": "/var", "socket": "/var/run/docker.sock"}[mutation],
                    "Destination": "/escape", "RW": False})
            elif mutation == "credentials":
                value["Config"]["Env"].append("GITHUB_TOKEN=publisher-secret")
            elif mutation == "host-network":
                value["HostConfig"]["NetworkMode"] = "host"
            elif mutation == "host-pid":
                value["HostConfig"]["PidMode"] = "host"
            elif mutation == "privileged":
                value["HostConfig"]["Privileged"] = True
            elif mutation == "wrong-image":
                value["Config"]["Image"] = "unprovisioned:latest"
            elif mutation == "extra-network":
                value["NetworkSettings"]["Networks"]["other"] = {}
            else:
                value["State"]["Running"] = False
            with self.subTest(mutation=mutation), self.assertRaises(ValueError):
                runtime.validate_container(value, protected, "/work/public")
        value = container()
        value["Mounts"].append({"Type": "bind", "Source": "/tmp/custody-alias",
                                "Destination": "/escape", "RW": False})
        with self.assertRaisesRegex(ValueError, "custody"):
            runtime.validate_container(value, protected, "/work/public",
                resolve_source=lambda source: PurePosixPath(protected[1] if source == "/tmp/custody-alias" else source))

    def test_default_mounts_reject_protected_workspace_home_and_temp(self):
        for protected in ("/tmp/custody", "/opt/controller", "/usr/controller",
                          "/home/runner/.config/key", "/work/public/controller"):
            with self.subTest(protected=protected), self.assertRaises(ValueError):
                runtime.validate_layout([protected], "/work/public", "/home/runner", "/runner/_temp")
        runtime.validate_layout(["/var/lib/compiler-perf"], "/work/public", "/home/runner", "/runner/_temp")

    def test_protected_entry_points_refuse_nonisolated_python_before_controller_access(self):
        with mock.patch.object(runtime, "sys", SimpleNamespace(platform="linux", flags=SimpleNamespace(isolated=0))), \
                mock.patch.object(w, "Controller") as controller, mock.patch.object(w, "GitHub") as api:
            for command in ("setup", "start-service", "serve", "handoff", "finalize"):
                with self.subTest(command=command), mock.patch("sys.stdout", new_callable=io.StringIO) as output:
                    self.assertEqual(runtime.main([command]), 2)
                    self.assertEqual(json.loads(output.getvalue()), {
                        "outcome": "blocked_setup", "reason": "Protected runtime requires Python -I"})
            controller.assert_not_called()
            api.assert_not_called()

    def test_interpreter_prefix_custody_covers_entry_point_layout_and_actual_mounts(self):
        helpers = str(Path(runtime.__file__).resolve().parent)
        paths = {"root": "/var/lib/compiler-perf/custody", "repository": "/var/lib/compiler-perf/source",
                 "journal": "/var/lib/compiler-perf/journal", helpers: "/var/lib/compiler-perf/helpers"}
        for prefix in ("/usr/python", "/opt/python", "/home/runner/.local/python",
                       "/work/public/python", "/runner/_temp/python", "/tmp/python"):
            with self.subTest(prefix=prefix), mock.patch.object(runtime, "sys", SimpleNamespace(
                    platform="linux", prefix=prefix, flags=SimpleNamespace(isolated=1))), \
                    mock.patch.object(bridge, "_local_directory", side_effect=lambda path, **_:
                                      PurePosixPath(paths.get(str(path), str(path)))), \
                    mock.patch.object(runtime.Path, "home", return_value=PurePosixPath("/home/runner")), \
                    mock.patch.dict(os.environ, {"RUNNER_TEMP": "/runner/_temp"}), \
                    mock.patch.object(w, "Controller") as controller, \
                    mock.patch("sys.stdout", new_callable=io.StringIO) as output:
                self.assertEqual(runtime.main(["finalize", "--root", "root", "--repository", "repository",
                                              "--journal", "journal", "--workspace", "/work/public"]), 2)
                self.assertIn("Protected custody overlaps", json.loads(output.getvalue())["reason"])
                controller.assert_not_called()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory).resolve()
            owner = SimpleNamespace(root=root / "custody", repository=root / "source",
                transport=SimpleNamespace(journal=SimpleNamespace(repository=root / "journal")))
            prefix = root / "python"
            with mock.patch.object(runtime, "sys", SimpleNamespace(prefix=str(prefix))):
                adapter = runtime.Runtime(owner, root / "visible")
            self.assertIn(prefix.resolve(), adapter.protected)
            # Model Linux custody independently of the host's temporary and checkout paths.
            protected = [str(PurePosixPath("/var/lib/compiler-perf", p.name)) for p in adapter.protected]
            self.assertIn("/var/lib/compiler-perf/python", protected)
            runtime.validate_container(container(), protected, "/work/public")
            for source, denied in (("python", True), ("python/lib", True), ("other/lib", False)):
                with self.subTest(source=source):
                    value = container()
                    value["Mounts"].append({"Type": "bind", "Source": "/var/lib/compiler-perf/" + source,
                                            "Destination": "/interpreter", "RW": False})
                    with self.assertRaisesRegex(ValueError, "custody") if denied else nullcontext():
                        runtime.validate_container(value, protected, "/work/public")

    def test_setup_preflights_before_preparation_and_exports_no_controller(self):
        for denied in (False, True):
            with self.subTest(denied=denied), tempfile.TemporaryDirectory() as directory:
                repo, controller, api, _ = self.fixture(Path(directory), transport_factory=fixtures.MemoryTransport)
                workspace = Path(directory) / "visible"
                workspace.mkdir()
                api.failure = "403 denied" if denied else None
                adapter = runtime.Runtime(controller, workspace)
                request = {"schema_version": 2, "run_id": "run-1", "base_sha": repo.sha,
                           "primary_metric": "elapsed_ms"}
                with mock.patch.object(controller, "investigate") as prepare:
                    result = adapter.setup(request, controller.now() - timedelta(seconds=120))
                self.assertEqual(result["action"], "blocked" if denied else "reserve")
                prepare.assert_not_called()
                if not denied:
                    self.assertTrue((workspace / "source" / fixtures.SOURCE).is_file())
                    self.assertFalse((workspace / "source" / "eng").exists())
                    self.assertEqual(controller.load("runtime")["agent_home"], str(Path.home()))
                    clock = controller.load("clocks")["setup"]
                    self.assertEqual(w.mem.timestamp(clock["start"]),
                                     controller.now() - timedelta(seconds=120))
                self.assertNotIn("active", controller.load("clocks"))

    def test_readiness_begins_at_first_authenticated_agent_call_not_service_install(self):
        with self.ready() as (_, controller, _, _):
            now = controller.now()
            controller.now = lambda: now + timedelta(seconds=300)
            controller.clock = lambda: 0
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            probe = mock.Mock()
            client = bridge.Bridge(controller, launch=fixtures.AgentTests.launch,
                                   identify=fixtures.identify, ready=lambda: adapter.agent_ready(probe))
            self.assertNotIn("active", controller.load("clocks"))
            controller.now = lambda: now + timedelta(seconds=700)
            response = client.investigate(fixtures.message("status"))
            self.assertTrue(response["ok"], response)
            probe.assert_called_once()
            clock = controller.load("clocks")["active"]
            self.assertEqual(w.mem.timestamp(clock["start"]), controller.now())
            self.assertEqual((w.mem.timestamp(clock["deadline"]) - controller.now()).total_seconds(), 14400)
            client.investigate(fixtures.message("status"))
            self.assertEqual(probe.call_count, 1)
            self.assertTrue(client.close())

    def test_denied_runtime_probe_cannot_start_profile_or_export(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            client = bridge.Bridge(controller, launch=fixtures.AgentTests.launch,
                                   identify=fixtures.identify, ready=lambda: adapter.agent_ready(
                                       mock.Mock(side_effect=ValueError("Denied Docker inspection"))))
            self.assertFalse(client.investigate(fixtures.message("start_profile"))["ok"])
            self.assertNotIn("active", controller.load("clocks"))
            self.assertEqual(controller.load("runtime-error")["reason"], "Denied Docker inspection")
            self.assertFalse((controller.root / "output").exists())
            client.close()

    def test_startup_shutdown_share_one_ten_minute_slack_and_restart_never_renews(self):
        with self.ready() as (repo, controller, api, transport):
            now = controller.now()
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            before = controller.load("runtime")
            restarted = w.Controller(controller.root, repo.root, transport, api, now=lambda: now)
            runtime.Runtime(restarted, adapter.workspace).arm()
            self.assertEqual(restarted.load("runtime"), before)
            controller.now = lambda: now + timedelta(seconds=runtime.READINESS_SECONDS)
            with self.assertRaisesRegex(ValueError, "readiness"):
                adapter.agent_ready(mock.Mock())
            self.assertEqual(runtime.READINESS_SECONDS + runtime.SHUTDOWN_SECONDS + 1, 600)
            self.assertNotIn("active", controller.load("clocks"))

    def test_stock_harness_wrapper_bounds_readiness_retries_and_shutdown_without_new_allowance(self):
        source = Path(runtime.__file__).with_name("compiler_perf_harness.cjs")
        for mode in ("success", "expired", "deadline"):
            with self.subTest(mode=mode), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                wrapper = root / source.name
                wrapper.write_bytes(source.read_bytes())
                (root / "copilot_harness.cjs").write_text(
                    "setInterval(() => {}, 1000);" if mode == "deadline" else
                    "console.log(JSON.stringify(process.argv.slice(2)));")
                deadline = (time.time() + (-1 if mode == "expired" else runtime.SHUTDOWN_SECONDS + .3
                                           if mode == "deadline" else 15000)) * 1000
                environment = dict(w.c.tool_environment(),
                                   COMPILER_PERF_EXECUTION_DEADLINE_MS=str(int(deadline)))
                result = subprocess.run(["node", str(wrapper), "/usr/local/bin/copilot", "--no-ask-user"],
                                        capture_output=True, timeout=6, env=environment)
                self.assertEqual(result.returncode, {"success": 0, "expired": 2, "deadline": 124}[mode],
                                 result.stderr)
                if mode == "success":
                    self.assertEqual(json.loads(result.stdout), ["/usr/local/bin/copilot", "--no-ask-user"])

    def test_supported_harness_installation_reuses_stock_files_and_rejects_replacement(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            actions = controller.root.parent / "runner-temp" / "gh-aw" / "actions"
            actions.mkdir(parents=True)
            stock = actions / "copilot_harness.cjs"
            stock.write_text("fixed stock fixture")
            with mock.patch.dict(os.environ, {"RUNNER_TEMP": str(actions.parents[1])}):
                adapter.install_harness()
                adapter.install_harness()
                self.assertEqual(stock.read_text(), "fixed stock fixture")
                (actions / "compiler_perf_harness.cjs").write_text("tampered")
                with self.assertRaisesRegex(ValueError, "harness changed"):
                    adapter.install_harness()

    def test_owned_shutdown_checkpoints_incomplete_and_cannot_invent_no_finding(self):
        with self.ready(canonical=True) as (_, controller, _, transport):
            controller.start_active()
            worker = fixtures.Worker(None)

            def launch(arguments, owner):
                return fixtures.AgentTests.launch(arguments, owner) if arguments[-1] == "finish" else worker

            client = bridge.Bridge(controller, launch=launch, identify=fixtures.identify)
            client.investigate(fixtures.message("start_profile"))
            self.assertTrue(client.shutdown("Owned service SIGTERM"))
            self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "cancelled")
            self.assertFalse((controller.root / "output").exists())
            self.assertTrue(worker.signals)
        with self.ready() as (_, controller, _, transport):
            controller.session["phase"] = "accepted"
            controller.save("session", controller.session)
            client = bridge.Bridge(controller, launch=fixtures.AgentTests.launch, identify=fixtures.identify)
            self.assertTrue(client.shutdown("Authorization had not completed"))
            self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "cancelled")

    def test_shutdown_monitors_slow_owned_finalization_until_completion_or_existing_deadline(self):
        for mode in ("cancelling", "finalizing", "expired"):
            with self.subTest(mode=mode), self.ready(canonical=True) as (_, controller, api, transport):
                elapsed, now = [0.0], controller.now()
                controller.now = lambda: now + timedelta(seconds=elapsed[0])
                controller.clock = lambda: elapsed[0]
                controller.start_active()
                worker = fixtures.Worker(None)

                def interrupt(value):
                    worker.signals.append(value)
                    if "finalization" not in controller.load("clocks"):
                        controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)

                def wait(timeout=None):
                    if worker.code is None:
                        elapsed[0] += min(timeout, max(0, 60 - elapsed[0])) if mode != "expired" else timeout
                        if mode == "expired" or elapsed[0] < 60:
                            raise subprocess.TimeoutExpired("slow owned cleanup", timeout)
                        controller.finish("cancelled", "Owned worker retained incomplete evidence")
                        worker.code = 130
                    return worker.code

                worker.send_signal, worker.wait = interrupt, wait
                launch = mock.Mock(return_value=worker)
                client = bridge.Bridge(controller, launch=launch, identify=fixtures.identify)
                self.assertTrue(client.investigate(fixtures.message("start_profile"))["ok"])
                if mode != "cancelling":
                    controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
                if mode == "expired":
                    elapsed[0] = w.FINALIZATION_SECONDS - 60
                deadline = (now + timedelta(seconds=w.FINALIZATION_SECONDS)).isoformat()

                def advance(seconds):
                    elapsed[0] += 5
                    if mode != "expired" and elapsed[0] >= 60 and worker.code is None:
                        wait(0)

                with mock.patch.object(bridge, "time", SimpleNamespace(sleep=advance)):
                    stopped = client.shutdown("Owned SIGTERM")
                self.assertEqual(stopped, mode != "expired")
                self.assertEqual(controller.load("clocks")["finalization"]["deadline"], deadline)
                self.assertEqual(len(worker.signals), 0 if mode == "finalizing" else 1)
                self.assertEqual(launch.call_count, 1)
                outcome = transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"]
                if mode == "expired":
                    self.assertIsNone(outcome)
                    self.assertEqual(client.receipt["state"], "cleanup_failed")
                    self.assertLessEqual(elapsed[0], w.FINALIZATION_SECONDS)
                    self.assertIs(client.worker, worker)
                    expired_at = elapsed[0]
                    self.assertFalse(client.shutdown("Cleanup remains unacknowledged"))
                    self.assertEqual(elapsed[0], expired_at)
                else:
                    self.assertEqual(outcome, "cancelled")
                    self.assertEqual(client.receipt["state"], "completed")
                    self.assertEqual(client.receipt["exit_status"], 130)
                    self.assertIsNone(client.worker)
                    self.assertEqual(worker.poll(), 130)
                    self.assertGreaterEqual(elapsed[0], 60)
                    self.assertTrue(client.shutdown("Idempotent owned shutdown"))
                self.assertFalse((controller.root / "output").exists())
                self.assertEqual(api.effects, [])

    def test_authorization_worker_uses_existing_finalization_deadline_not_active_expiry(self):
        with self.ready() as (_, controller, _, _):
            controller.clock = lambda: 0
            controller.start_active()
            controller.session["phase"] = "accepted"
            controller.save("session", controller.session)
            controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
            worker = fixtures.Worker(None)
            launch = mock.Mock(return_value=worker)
            client = bridge.Bridge(controller, launch=launch, identify=fixtures.identify)
            client._start("authorize", None)
            self.assertEqual(launch.call_args.args[0][-1], "authorize")
            now = controller.now()
            # Active may have started much earlier than finalization.
            clocks = controller.load("clocks")
            clocks["active"]["start"] = (now - timedelta(seconds=w.ACTIVE_SECONDS)).isoformat()
            clocks["active"]["deadline"] = now.isoformat()
            clocks["active"]["last_seen"] = now.isoformat()
            clocks["active"]["remaining_seconds"] = 0
            controller.save("clocks", clocks)
            client.tick()
            self.assertEqual(worker.signals, [])
            controller.save("runtime-engine-exit", {"at": (now - timedelta(seconds=30)).isoformat(), "id": "a" * 64})
            controller.now = lambda: now + timedelta(seconds=w.FINALIZATION_SECONDS - 30)
            client.tick()
            self.assertEqual(len(worker.signals), 1)
            self.assertEqual(client.receipt["state"], "worker_failed")
            self.assertFalse((controller.root / "approval.json").exists())
            client.close()

    def test_closing_with_pending_cleanup_cannot_acknowledge_a_later_shutdown(self):
        with self.ready() as (_, controller, _, transport):
            worker = fixtures.Worker(None)
            client = bridge.Bridge(controller, launch=lambda *_: worker, identify=fixtures.identify)
            self.assertTrue(client.investigate(fixtures.message("start_profile"))["ok"])
            controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
            self.assertFalse(client.close())
            self.assertFalse(client.shutdown("Cleanup remains unacknowledged"))
            self.assertEqual(client.receipt["state"], "cleanup_failed")
            self.assertEqual(worker.signals, [])
            self.assertIs(client.worker, worker)
            self.assertIsNone(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"])

    def test_authorization_cannot_begin_after_active_expiry(self):
        with self.ready() as (_, controller, _, _):
            controller.start_active()
            controller.session["phase"] = "accepted"
            now = controller.now()
            controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS)
            with self.assertRaises(KeyboardInterrupt):
                controller.authorize()
            self.assertNotIn("finalization", controller.load("clocks"))
            self.assertFalse((controller.root / "approval.json").exists())

    def test_runtime_inspection_requires_actual_model_proxy_and_unmodified_config(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            # Linux Docker paths are a fixture, not a claim about this Windows host.
            config = {"container": {"imageTag": "0.25.55"},
                      "apiProxy": {"enabled": True, "enableTokenSteering": True}}
            proxy = {"Id": "b" * 64, "State": {"Running": True},
                     "Config": {"Image": "ghcr.io/github/gh-aw-firewall/api-proxy:0.25.55"}}
            for denied in (False, True):
                value = container()
                proxy["State"]["Running"] = not denied
                results = [subprocess.CompletedProcess([], 0, data, b"") for data in (
                    w.mem.encoded([value]),
                    ("\0".join(p["Source"] for p in value["Mounts"]) + "\0").encode(),
                    w.mem.encoded([proxy]))]
                read = w.read_input
                with self.subTest(denied=denied), mock.patch.object(runtime.subprocess, "run", side_effect=results), \
                        mock.patch.object(runtime, "validate_container", return_value=value["Mounts"]) as validate, \
                        mock.patch.object(w, "read_input", side_effect=lambda path:
                                          config if Path(path).name == "awf-config.json" else read(path)), \
                        mock.patch.object(w.m, "file_record", return_value={"path": "fixture", "sha256": "c" * 64}), \
                        mock.patch.dict(runtime.os.environ, {"RUNNER_TEMP": str(controller.root.parent)}):
                    if denied:
                        with self.assertRaisesRegex(ValueError, "proxy"):
                            adapter.inspect_agent()
                    else:
                        adapter.inspect_agent()
                        self.assertEqual(controller.load("runtime-container")["proxy_id"], "b" * 64)
                    validate.assert_called_once()

    def test_root_owned_mounts_use_bounded_credential_free_canonical_metadata_probe(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.workspace = PurePosixPath("/work/public")
            adapter.protected = ["/var/lib/compiler-perf"]
            value = container()
            value["Mounts"].append({"Type": "bind", "Source": "/tmp/awf-root/agent-logs",
                                    "Destination": "/host/home/runner/.copilot/logs", "RW": True})
            sources = [p["Source"] for p in value["Mounts"]]
            config = controller.root.parent / "gh-aw" / "awf-config.json"
            config.parent.mkdir()
            w.m.write_json(config, {"container": {"imageTag": "0.25.55"},
                                   "apiProxy": {"enabled": True, "enableTokenSteering": True}})
            proxy = {"Id": "b" * 64, "State": {"Running": True},
                     "Config": {"Image": "ghcr.io/github/gh-aw-firewall/api-proxy:0.25.55"}}
            for case, uid in ((case, uid) for case in ("valid", "denied", "incomplete", "custody-alias")
                              for uid in (0, 1001)):
                resolved = list(sources)
                if case == "custody-alias":
                    resolved[-1] = "/var/lib/compiler-perf/key"
                data = ("\0".join(resolved) + "\0").encode()
                if case == "incomplete":
                    data = data.removesuffix(b"\0")
                results = [subprocess.CompletedProcess([], 0, w.mem.encoded([value]), b""),
                           subprocess.CompletedProcess([], 1 if case == "denied" else 0, data, b"denied"),
                           subprocess.CompletedProcess([], 0, w.mem.encoded([proxy]), b"")]
                with self.subTest(case=case, uid=uid), mock.patch.dict(os.environ, {
                        "RUNNER_TEMP": str(config.parents[1]), "GH_TOKEN": "must-not-reach-sudo"}), \
                        mock.patch.object(runtime.os, "geteuid", return_value=uid, create=True), \
                        mock.patch.object(runtime.subprocess, "run", side_effect=results) as run:
                    if case == "valid":
                        adapter.inspect_agent()
                        self.assertEqual(controller.load("runtime-container")["mounts"], value["Mounts"])
                    else:
                        with self.assertRaises(ValueError):
                            adapter.inspect_agent()
                    self.assertGreaterEqual(len(run.call_args_list), 2)
                    command = run.call_args_list[1]
                    metadata = ["/usr/bin/readlink", "--canonicalize-existing", "--zero", "--", *sources]
                    self.assertEqual(command.args[0], metadata if uid == 0 else ["/usr/bin/sudo", "-n", "--", *metadata])
                    self.assertEqual(command.kwargs["timeout"], 15)
                    self.assertNotIn("GH_TOKEN", command.kwargs["env"])

    def test_failed_engine_blocks_even_previously_authorized_handoff(self):
        with self.candidate() as (controller, api, transport):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            controller.validate_output()
            intent = controller.session["attempt"]["publication"]
            controller.save("runtime-service", {"state": "stopped"})
            for outcome in ("failure", "cancelled", "skipped"):
                with self.subTest(outcome=outcome):
                    with self.assertRaisesRegex(ValueError, "Stock engine ended " + outcome):
                        adapter.handoff(engine_outcome=outcome)
                    with self.assertRaisesRegex(ValueError, "Runtime unhealthy"):
                        adapter.finalize(publisher=api)
                    self.assertEqual(api.effects, [])
                    self.assertEqual(transport.read()[0]["attempts"][intent["attempt_id"]]["publication"], intent)
                    self.assertTrue((controller.root / "approval.json").is_file())
            # Read-only lost-ack reconciliation remains available without publisher authority.
            adapter.finalize()

    def test_service_readiness_detects_early_exit_without_adopting_pid(self):
        for code in (0, 2):
            with self.subTest(code=code), self.ready() as (_, controller, _, _):
                adapter = runtime.Runtime(controller, controller.root.parent / "visible")
                child = fixtures.Worker(code)
                with self.assertRaisesRegex(ValueError, "exited"):
                    adapter.wait_service(child, mock.Mock(return_value=False))
                self.assertEqual(child.signals, [])

    def test_startup_cleanup_shares_finalization_bound_and_preserves_unacknowledged_work(self):
        for state in ("stopped", "stuck", "expired", "worker-unacknowledged"):
            with self.subTest(state=state), self.ready() as (_, controller, _, transport):
                adapter = runtime.Runtime(controller, controller.root.parent / "visible")
                controller.clock = lambda: 0
                now = controller.now()
                controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
                controller.now = lambda: now + timedelta(seconds=2400 if state == "expired" else 2390)
                controller.save("runtime-service", {"state": "launching"})
                child = mock.Mock()
                child.poll.return_value = None
                if state == "stuck":
                    child.wait.side_effect = subprocess.TimeoutExpired("owned service", 10)
                if state == "worker-unacknowledged":
                    controller.save("worker", {"state": "running", "operation": "profile"})
                if state == "stopped":
                    adapter.abort_startup(child, ValueError("Denied readiness"))
                    self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "blocked_setup")
                else:
                    with self.assertRaises(KeyboardInterrupt if state == "expired" else
                                           ValueError if state == "worker-unacknowledged" else subprocess.TimeoutExpired):
                        adapter.abort_startup(child, ValueError("Denied readiness"))
                    self.assertEqual(controller.load("runtime-service")["state"], "cleanup_failed")
                    self.assertIsNone(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"])
                child.terminate.assert_called_once_with()
                if state != "expired":
                    child.wait.assert_called_once_with(timeout=10)
                self.assertEqual(controller.load("clocks")["finalization"]["start"], now.isoformat())
                self.assertFalse((controller.root / "approval.json").exists())

    def test_engine_exit_charges_stock_collection_to_shared_finalization_after_restart(self):
        with self.ready() as (repo, controller, api, transport):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            now = controller.now()
            controller.clock = lambda: 0
            adapter.arm()
            adapter.agent_ready(lambda: None)
            controller.save("runtime-container", {"id": "a" * 64, "last_seen_at": now.isoformat()})
            result = subprocess.CompletedProcess([], 0, b"", b"")
            with mock.patch.object(runtime.subprocess, "run", return_value=result):
                adapter.observe_engine()
            controller.save("runtime-service", {"state": "stopped"})
            controller.now = lambda: now + timedelta(seconds=900)
            adapter.handoff()
            self.assertEqual(controller.remaining("finalization", w.FINALIZATION_SECONDS), 1500)
            restarted = w.Controller(controller.root, repo.root, transport, api,
                                     now=lambda: now + timedelta(seconds=2400), clock=lambda: 0)
            with self.assertRaises(KeyboardInterrupt):
                runtime.Runtime(restarted, adapter.workspace).handoff()
            self.assertEqual(restarted.load("clocks")["finalization"]["start"], now.isoformat())

    def test_engine_observation_preserves_earlier_finalization_and_denies_failed_inventory(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            now = controller.now()
            controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
            controller.save("runtime-container", {"id": "a" * 64, "last_seen_at": now.isoformat()})
            for code, data in ((1, b""), (0, b"not-json"), (0, w.mem.encoded({"ID": "b" * 64, "State": "running"}))):
                with self.subTest(code=code, data=data), \
                        mock.patch.object(runtime.subprocess, "run", return_value=
                                          subprocess.CompletedProcess([], code, data, b"denied")), \
                        self.assertRaises(ValueError):
                    adapter.observe_engine()
                self.assertFalse((controller.root / "runtime-engine-exit.json").exists())
            controller.now = lambda: now + timedelta(seconds=20)
            with mock.patch.object(runtime.subprocess, "run", return_value=subprocess.CompletedProcess(
                    [], 0, w.mem.encoded({"ID": "a" * 64, "State": "running"}), b"")):
                adapter.observe_engine()
            controller.now = lambda: now + timedelta(seconds=30)
            with mock.patch.object(runtime.subprocess, "run", return_value=subprocess.CompletedProcess([], 0, b"", b"")):
                adapter.observe_engine()
            self.assertEqual(controller.remaining("finalization", w.FINALIZATION_SECONDS), 2370)
            self.assertEqual(controller.load("runtime-engine-exit")["at"], (now + timedelta(seconds=20)).isoformat())

    def test_monitor_checkpoints_engine_exit_before_stock_post_steps(self):
        with self.ready() as (_, controller, _, transport):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            adapter.agent_ready(lambda: None)
            controller.save("runtime-container", {"id": "a" * 64, "last_seen_at": controller.now().isoformat()})
            run = subprocess.run
            def inspect(command, *args, **kwargs):
                return (subprocess.CompletedProcess(command, 0, b"", b"") if command[:2] == ["docker", "ps"]
                        else run(command, *args, **kwargs))
            with mock.patch.object(runtime.subprocess, "run", side_effect=inspect), \
                    self.serving(adapter, fixtures.AgentTests.launch):
                end = time.monotonic() + 15
                while controller.load("runtime-service")["state"] != "stopped" and time.monotonic() < end:
                    time.sleep(.05)
                self.assertEqual(controller.load("runtime-service")["state"], "stopped")
                self.assertTrue((controller.root / "runtime-engine-exit.json").exists())
                self.assertFalse((controller.root / "runtime-error.json").exists())
                self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "cancelled")
                self.assertFalse((controller.root / "output").exists())

    def test_frozen_sdk_helpers_and_policy_are_checked_before_preparation(self):
        for changed in ("sdk", "helpers", "policy"):
            with self.subTest(changed=changed), self.discovery() as (repo, controller, _, _):
                adapter = runtime.Runtime(controller, controller.root.parent / "visible")
                helper = controller.root / "frozen.py"
                helper.write_text("frozen helper")
                inputs = {"policy": w.m.fingerprint_inputs(w.m.POLICY),
                          "sdk": w.m.file_record(repo.root / "global.json"), "helpers": [w.m.file_record(helper)]}
                if changed == "policy":
                    inputs["policy"] = "0" * 64
                else:
                    Path(inputs["sdk"]["path"] if changed == "sdk" else str(helper)).write_text("changed")
                controller.save("runtime", {"inputs": inputs})
                with mock.patch.object(runtime.subprocess, "run", side_effect=[
                        subprocess.CompletedProcess([], 0, b"1.0.52", b""),
                        subprocess.CompletedProcess([], 0, b"v0.25.55", b"")]), \
                        mock.patch.object(controller, "investigate") as investigate:
                    result = adapter.prepare()
                self.assertEqual(result["outcome"], "blocked_setup")
                self.assertIn("changed", result["reason"])
                investigate.assert_not_called()

    def test_start_service_entry_point_installs_harness_and_emits_authenticated_handoff(self):
        import transport
        for failure in (None, "launch-denied", "early-exit", "probe-denied", "cancelled", "sigterm", "output-denied"):
            with self.subTest(failure=failure), self.ready() as (_, controller, api, journal):
                previous = runtime.signal.getsignal(runtime.signal.SIGTERM)
                cancelled = failure in ("cancelled", "sigterm")
                actions = controller.root.parent / "runner-temp" / "gh-aw" / "actions"
                actions.mkdir(parents=True)
                (actions / "copilot_harness.cjs").write_text("stock fixture")
                output, log = controller.root / "actions-output", io.StringIO()
                arguments = ["start-service", "--root", str(controller.root), "--repository", str(controller.repository),
                             "--journal", str(journal.journal.repository), "--workspace", str(controller.root.parent / "visible")]
                popen = subprocess.Popen
                owned = fixtures.Worker(2 if failure == "early-exit" else None)
                owned.terminate = lambda: owned.send_signal("SIGTERM")
                if failure == "output-denied":
                    output.mkdir()
                def child(command, *args, **kwargs):
                    if command[3:4] == ["serve"] and failure == "launch-denied":
                        raise OSError("Denied service launch")
                    return owned if command[3:4] == ["serve"] else popen(command, *args, **kwargs)
                async def probe(token):
                    self.assertFalse((controller.root / "controller.lock").exists())
                    if failure == "sigterm":
                        runtime.signal.raise_signal(runtime.signal.SIGTERM)
                    if failure in ("probe-denied", "cancelled"):
                        raise KeyboardInterrupt() if failure == "cancelled" else ValueError("Denied service probe")
                    return True
                with mock.patch.object(runtime, "sys", SimpleNamespace(
                        platform="linux", executable=sys.executable, prefix=sys.prefix,
                        flags=SimpleNamespace(isolated=1))), \
                        mock.patch.object(runtime, "validate_layout"), \
                        mock.patch.object(w, "Controller", return_value=controller), \
                        mock.patch.object(w, "GitHub", return_value=api), \
                        mock.patch.object(transport, "JournalTransport", return_value=journal), \
                        mock.patch.object(runtime.Runtime, "prepare", return_value={"phase": "ready"}), \
                        mock.patch.object(runtime, "probe_service", side_effect=probe), \
                        mock.patch.object(runtime.subprocess, "Popen", side_effect=child) as launch, \
                        mock.patch.dict(os.environ, {"RUNNER_TEMP": str(actions.parents[1]), "GITHUB_OUTPUT": str(output),
                                                    "PUBLISHER_SECRET": "must-not-inherit",
                                                    "GITHUB_UNTRUSTED_INPUT": "must-not-inherit", **RUNNER_IDENTITY}), \
                        mock.patch("sys.stdout", log):
                    frozen_identity = w.m.host_identity({})
                    self.assertEqual(runtime.main(arguments), 0 if failure is None else
                                     130 if cancelled else 2, log.getvalue())
                    if failure is None:
                        armed = controller.load("runtime")
                        self.assertEqual(runtime.main(arguments), 2, log.getvalue())
                        self.assertEqual(controller.load("runtime"), armed)
                        self.assertFalse((controller.root / "runtime-error.json").exists())
                        self.assertEqual(owned.signals, [])
                self.assertEqual(runtime.signal.getsignal(runtime.signal.SIGTERM), previous)
                if failure is not None:
                    self.assertEqual(owned.signals, [] if failure in ("early-exit", "launch-denied") else ["SIGTERM"])
                    self.assertEqual(controller.load("runtime-service")["state"], "stopped")
                    self.assertTrue((controller.root / "runtime-error.json").exists())
                    self.assertEqual(journal.read()[0]["discoveries"]["run-1.discovery"]["outcome"],
                                     "cancelled" if cancelled else "blocked_setup")
                    self.assertFalse((controller.root / "approval.json").exists())
                    continue
                services = [call for call in launch.call_args_list if call.args[0][3:4] == ["serve"]]
                self.assertEqual(len(services), 1)
                command = services[0].args[0]
                self.assertEqual(command[:4], [sys.executable, "-I", str(Path(runtime.__file__).resolve()), "serve"])
                self.assertEqual(command[4:], arguments[1:])
                environment = services[0].kwargs["env"]
                self.assertEqual({key: environment.get(key) for key in RUNNER_IDENTITY}, RUNNER_IDENTITY)
                for key in (*w.m.OWNED_ENVIRONMENT_KEYS, "APPDATA", "LOCALAPPDATA"):
                    self.assertTrue(Path(environment[key]).is_relative_to(controller.root), key)
                with mock.patch.dict(os.environ, environment, clear=True):
                    self.assertEqual(w.m.host_identity({}), frozen_identity)
                for key in ("PUBLISHER_SECRET", "GITHUB_UNTRUSTED_INPUT", "GITHUB_OUTPUT"):
                    self.assertNotIn(key, environment)
                self.assertTrue(services[0].kwargs["start_new_session"])
                self.assertTrue((actions / "compiler_perf_harness.cjs").is_file())
                self.assertIn("token=" + bridge.endpoint_token(controller.root), output.read_text())
                self.assertIn("execution_deadline_ms=", output.read_text())
                self.assertNotIn("active", controller.load("clocks"))

    def test_service_reentry_validates_frozen_agent_home_not_private_controller_home(self):
        import transport
        with self.ready() as (_, controller, api, journal):
            workspace = controller.root.parent / "visible"
            controller.save("runtime", {"agent_home": "frozen-agent-home"})
            environment = bridge.trusted_environment(controller)
            environment["RUNNER_TEMP"] = str(controller.root.parent)
            with mock.patch.object(runtime, "sys", SimpleNamespace(
                    platform="linux", prefix=sys.prefix, flags=SimpleNamespace(isolated=1))), \
                    mock.patch.object(runtime, "validate_layout") as layout, \
                    mock.patch.object(w, "Controller", return_value=controller), \
                    mock.patch.object(w, "GitHub", return_value=api), \
                    mock.patch.object(transport, "JournalTransport", return_value=journal), \
                    mock.patch.object(runtime, "serve"), mock.patch.dict(os.environ, environment, clear=True), \
                    mock.patch("sys.stdout", new_callable=io.StringIO):
                self.assertEqual(runtime.main(["serve", "--root", str(controller.root), "--repository",
                    str(controller.repository), "--journal", str(journal.journal.repository),
                    "--workspace", str(workspace)]), 0)
            self.assertEqual(layout.call_args.args[2], "frozen-agent-home")

    def test_service_sigterm_unwinds_and_restores_the_original_handler(self):
        import transport
        with self.ready() as (_, controller, api, journal):
            controller.save("runtime", {"agent_home": "frozen-agent-home"})
            previous = runtime.signal.signal(runtime.signal.SIGTERM, mock.Mock())
            expected = runtime.signal.getsignal(runtime.signal.SIGTERM)
            try:
                with mock.patch.object(runtime, "sys", SimpleNamespace(
                        platform="linux", prefix=sys.prefix, flags=SimpleNamespace(isolated=1))), \
                        mock.patch.object(runtime, "validate_layout"), \
                        mock.patch.object(w, "Controller", return_value=controller), \
                        mock.patch.object(w, "GitHub", return_value=api), \
                        mock.patch.object(transport, "JournalTransport", return_value=journal), \
                        mock.patch.object(runtime, "serve", side_effect=lambda _: runtime.signal.raise_signal(
                            runtime.signal.SIGTERM)), \
                        mock.patch.dict(os.environ, {"RUNNER_TEMP": str(controller.root.parent)}), \
                        mock.patch("sys.stdout", new_callable=io.StringIO):
                    self.assertEqual(runtime.main(["serve", "--root", str(controller.root), "--repository",
                        str(controller.repository), "--journal", str(journal.journal.repository),
                        "--workspace", str(controller.root.parent / "visible")]), 130)
                self.assertIs(runtime.signal.getsignal(runtime.signal.SIGTERM), expected)
                expected.assert_not_called()
            finally:
                runtime.signal.signal(runtime.signal.SIGTERM, previous)

    def test_worker_preserves_host_identity_without_broadening_candidate_environment(self):
        script = (
            "import json,os,pathlib,subprocess,sys\n"
            "sys.path.insert(0,sys.argv[1])\n"
            "import measurement as m,collection as c\n"
            "root=pathlib.Path(sys.argv[2])\n"
            "(root/'worker.json').write_text(json.dumps({'identity':m.host_identity({}),"
            "'isolated':sys.flags.isolated,'prefix':sys.prefix,"
            "'environment':dict(os.environ),'candidate':m.allowed_environment(root/'candidate'),"
            "'diagnostic':c.target_environment(root/'diagnostic'),'tool':c.tool_environment(),"
            "'git_config':subprocess.run(['git','config','--global','--get','compilerperf.poison'],"
            "capture_output=True,env=c.tool_environment()).returncode}))\n")
        for available in (True, False):
            with self.subTest(available=available), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                hostile = root / "agent-home"
                hostile.mkdir()
                (hostile / ".gitconfig").write_text("[compilerperf]\npoison = agent-controlled\n")
                (hostile / "sitecustomize.py").write_text("raise RuntimeError('Agent Python configuration loaded')\n")
                environment = {key: value for key, value in os.environ.items() if key not in RUNNER_IDENTITY}
                environment.update(GH_TOKEN="trusted-journal-token", GITHUB_TOKEN="trusted-api-token",
                                   GITHUB_STEP_SUMMARY=str(root / "summary"),
                                   PUBLISHER_SECRET="must-not-inherit", GITHUB_UNTRUSTED_INPUT="must-not-inherit",
                                   RUNNER_UNTRUSTED_INPUT="must-not-inherit",
                                   PYTHONHOME=str(hostile), PYTHONPATH=str(hostile))
                environment.update({key: str(hostile) for key in
                                    (*w.m.OWNED_ENVIRONMENT_KEYS, "APPDATA", "LOCALAPPDATA",
                                     "XDG_CONFIG_HOME", "GH_CONFIG_DIR")})
                if available:
                    environment.update(RUNNER_IDENTITY)
                with mock.patch.dict(os.environ, environment, clear=True):
                    frozen_identity = w.m.host_identity({})
                    bridge.trusted_environment(SimpleNamespace(root=root, repository=root))
                    with mock.patch.object(bridge, "sys", SimpleNamespace(
                            platform="linux", executable=sys.executable)), \
                            mock.patch.dict(os.environ, {"PATH": str(hostile)}):
                        trusted = bridge.trusted_environment(SimpleNamespace(root=root, repository=root))
                        self.assertNotIn(str(hostile), trusted["PATH"].split(os.pathsep))
                    owner = SimpleNamespace(root=root, repository=root,
                                            transport=SimpleNamespace(journal=SimpleNamespace(repository=root)))
                    command = bridge.worker_command(owner, "profile")
                    with bridge._launch([*command[:2], "-c", script, str(Path(runtime.__file__).parent),
                                         str(root)], owner) as child:
                        self.assertEqual(child.wait(timeout=30), 0)
                result = json.loads((root / "worker.json").read_text())
                self.assertEqual(result["isolated"], 1)
                self.assertEqual(result["prefix"], sys.prefix)
                self.assertEqual(result["identity"], frozen_identity)
                self.assertEqual(result["git_config"], 1, "Trusted tool read agent-controlled Git configuration")
                for key in (*w.m.OWNED_ENVIRONMENT_KEYS, "APPDATA", "LOCALAPPDATA"):
                    self.assertTrue(Path(result["environment"][key]).is_relative_to(root / "host"), key)
                for key in ("GH_TOKEN", "GITHUB_TOKEN", "GITHUB_STEP_SUMMARY"):
                    self.assertEqual(result["environment"][key], environment[key])
                for key in ("PUBLISHER_SECRET", "GITHUB_UNTRUSTED_INPUT", "RUNNER_UNTRUSTED_INPUT"):
                    self.assertNotIn(key, result["environment"])
                for key in ("PYTHONHOME", "PYTHONPATH"):
                    self.assertNotIn(key, result["environment"])
                for target in ("candidate", "diagnostic", "tool"):
                    self.assertFalse(set(result[target]) & (set(RUNNER_IDENTITY) | {
                        "GH_TOKEN", "GITHUB_TOKEN", "GITHUB_STEP_SUMMARY", "PUBLISHER_SECRET",
                        "GITHUB_UNTRUSTED_INPUT", "RUNNER_UNTRUSTED_INPUT"}), target)

    def test_skipped_engine_cli_reaches_owned_handoff(self):
        import transport
        with self.ready() as (_, controller, api, journal):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            controller.save("runtime-service", {"state": "stopped"})
            with mock.patch.object(runtime, "sys", SimpleNamespace(platform="linux", prefix=sys.prefix,
                    flags=SimpleNamespace(isolated=1))), \
                    mock.patch.object(runtime, "validate_layout"), \
                    mock.patch.object(w, "Controller", return_value=controller), \
                    mock.patch.object(w, "GitHub", return_value=api), \
                    mock.patch.object(transport, "JournalTransport", return_value=journal), \
                    mock.patch.dict(os.environ, {"RUNNER_TEMP": str(controller.root.parent)}), \
                    mock.patch("sys.stdout", new_callable=io.StringIO) as output:
                code = runtime.main(["handoff", "--root", str(controller.root), "--repository",
                                     str(controller.repository), "--journal", str(journal.journal.repository),
                                     "--workspace", str(adapter.workspace), "--engine-outcome", "skipped"])
            self.assertEqual(code, 2)
            self.assertIn("Stock engine ended skipped", output.getvalue())
            self.assertTrue((controller.root / "runtime-stop.json").exists())
            self.assertEqual(api.effects, [])

    def test_handoff_rechecks_seal_and_preserves_one_publication_after_lost_ack(self):
        with self.candidate() as (controller, api, transport):
            root = controller.root
            adapter = runtime.Runtime(controller, root / "visible")
            controller.validate_output()
            for tamper in ("payload", "patch", "extra"):
                output = controller.root / "output"
                saved = {p: p.read_bytes() for p in output.iterdir()}
                if tamper == "extra":
                    (output / "untrusted").write_text("not a sealed transport")
                else:
                    (output / ("safe-output.json" if tamper == "payload" else "proposal.patch")).write_text("changed")
                with self.subTest(tamper=tamper), self.assertRaises(ValueError):
                    adapter.export(root / ("rejected-" + tamper))
                for path in output.iterdir():
                    if path not in saved:
                        path.unlink()
                for path, data in saved.items():
                    path.write_bytes(data)
            destination = root / "publisher"
            inventory = adapter.export(destination)
            self.assertEqual({Path(f["path"]).name for f in inventory["files"]},
                             {"safe-output.json", "proposal.patch"})
            self.assertEqual(adapter.export(destination), inventory)
            (destination / "proposal.patch").unlink()
            self.assertEqual(adapter.export(destination), inventory)
            api.failure = "after-pr"
            with self.assertRaises(RuntimeError):
                adapter.finalize(publisher=api)
            api.failure = None
            adapter.finalize(publisher=api)
            adapter.finalize(publisher=api)
            self.assertEqual(api.effects, ["branch", "pr"])
            self.assertEqual(len(transport.read()[0]["attempts"]), 1)

    def test_runtime_http_readiness_and_owned_handoff_preserve_incomplete_evidence(self):
        import httpx
        from mcp import ClientSession
        from mcp.client.streamable_http import streamable_http_client

        with self.ready() as (_, controller, api, transport):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            launches = []

            def launch(arguments, owner):
                launches.append(arguments[-1])
                if arguments[-1] == "profile":
                    time.sleep(.1)
                return fixtures.AgentTests.launch(arguments, owner)

            with self.serving(adapter, launch) as (port, token, probe):
                self.assertNotIn("active", controller.load("clocks"))

                async def script():
                    async with httpx.AsyncClient(headers={"Authorization": "Bearer " + token}, timeout=300) as http:
                        url = f"http://127.0.0.1:{port}/mcp"
                        denied = await http.post(url, headers={"Authorization": "Bearer invalid"}, content="{}")
                        self.assertEqual(denied.status_code, 401)
                        self.assertNotIn("active", controller.load("clocks"))
                        async with streamable_http_client(url, http_client=http) as streams:
                            async with ClientSession(streams[0], streams[1]) as session:
                                await session.initialize()
                                request = {"message": fixtures.message("start_profile")}
                                with mock.patch.object(bridge, "DISPATCH_SECONDS", .01):
                                    pending = await session.call_tool("investigate", request)
                                self.assertEqual(pending.structuredContent["error"], "dispatch_pending")
                                result = await session.call_tool("investigate", request)
                                self.assertTrue(result.structuredContent["ok"], result)
                                status = await session.call_tool("investigate", {
                                    "message": fixtures.message("status")})
                                self.assertTrue(status.structuredContent["status"]["diagnostics"]["stacks"])
                asyncio.run(script())
                result = adapter.handoff()
                self.assertIsNone(result["publication"])
                self.assertEqual(result["status"]["outcome"], "cancelled")
                self.assertTrue(result["artifacts"]["files"])
                self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "cancelled")
                probe.assert_called_once()
                self.assertEqual(launches.count("profile"), 1)
                self.assertEqual(api.effects, [])

    def test_engine_exit_before_agent_readiness_still_requests_owned_http_cleanup(self):
        for outcome in ("success", "failure", "cancelled", "skipped", "inspection-denied"):
            with self.subTest(outcome=outcome), self.ready() as (_, controller, api, transport):
                adapter = runtime.Runtime(controller, controller.root.parent / "visible")
                started = controller.now()
                controller.now, controller.clock = lambda: started, lambda: 0
                adapter.arm()
                # This is still a fake/local effect sink, not GitHub durability.
                transport.scope = "github"
                controller.now = lambda: started + timedelta(seconds=120)
                if outcome == "inspection-denied":
                    controller.save("runtime-container", {"id": "a" * 64, "last_seen_at": started.isoformat()})
                with self.serving(adapter, fixtures.AgentTests.launch) as (_, _, probe):
                    with mock.patch.object(adapter, "observe_engine", side_effect=ValueError("Denied Docker read")) \
                            if outcome == "inspection-denied" else nullcontext():
                        with self.assertRaisesRegex(ValueError, "Runtime unhealthy"):
                            adapter.handoff(engine_outcome="success" if outcome == "inspection-denied" else outcome)
                    self.assertTrue((controller.root / "runtime-stop.json").exists())
                    self.assertTrue((controller.root / "runtime-error.json").exists())
                    self.assertEqual(controller.load("runtime-service")["state"], "stopped")
                    self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "cancelled")
                    self.assertNotIn("active", controller.load("clocks"))
                    self.assertFalse(controller.load("runtime-engine-exit")["observed"])
                    self.assertNotIn("id", controller.load("runtime-engine-exit"))
                    self.assertEqual(controller.remaining("finalization", w.FINALIZATION_SECONDS), 2280)
                    self.assertFalse((controller.root / "output").exists())
                    probe.assert_not_called()
                    self.assertEqual(api.effects, [])

    def test_shutdown_failure_reports_cleanup_failed_without_releasing_outstanding_work(self):
        with self.ready() as (_, controller, api, transport):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            def denied(*_):
                raise OSError("Denied owned finalizer launch")
            with self.serving(adapter, denied, expected_error="Owned service cleanup incomplete"):
                with self.assertRaisesRegex(ValueError, "Owned service cleanup incomplete"):
                    adapter.handoff()
                self.assertEqual(controller.load("runtime-service")["state"], "cleanup_failed")
                self.assertIn("Denied owned finalizer", controller.load("runtime-error")["reason"])
                self.assertIsNone(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"])
                self.assertFalse((controller.root / "output").exists())
                self.assertEqual(api.effects, [])

    def test_service_automatically_authorizes_accepted_candidate_across_active_expiry(self):
        from mcp import ClientSession
        from mcp.client.streamable_http import streamable_http_client
        import httpx

        with fixtures.candidate_fixture() as (root, repo, original, patch):
            _, controller, api, transport = self.fixture(root / "interaction", repo)
            self.assertEqual(controller.preflight({"schema_version": 2, "run_id": "run-1",
                             "base_sha": repo.sha, "primary_metric": "elapsed_ms"})["action"], "reserve")
            manifest = controller.root / "workload.json"
            w.m.write_json(manifest, w.c.read_bounded_json(original / "workload.json"))
            controller.session.update(phase="ready", manifest=w.m.file_record(manifest),
                                      ready_at=controller.now().isoformat())
            controller.checkpoint()
            adapter = runtime.Runtime(controller, root / "visible")
            adapter.arm()
            controller.clock = lambda: 0
            now = controller.now()
            controller.save("runtime-container", {"id": "a" * 64, "last_seen_at": now.isoformat(), "mounts": []})
            engine_running, run = threading.Event(), subprocess.run
            engine_running.set()
            def inspect(command, *args, **kwargs):
                if command[:2] == ["docker", "ps"]:
                    data = w.mem.encoded({"ID": "a" * 64, "State": "running"}) if engine_running.is_set() else b""
                    return subprocess.CompletedProcess(command, 0, data, b"")
                return run(command, *args, **kwargs)

            def collect(_, __, destination, **kwargs):
                destination.mkdir()
                w.m.write_json(destination / "evidence-manifest.json", w.c.read_bounded_json(root / "profile.json"))

            launches = []
            def launch(arguments, owner):
                launches.append(arguments[-1])
                # The monitor runs in a thread; production workers run in the main thread.
                if arguments[-1] == "authorize":
                    current = controller.current_attempt
                    def cross_active_expiry():
                        controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS + 1)
                        return current()
                    with mock.patch.object(controller, "current_attempt", side_effect=cross_active_expiry):
                        return fixtures.AgentTests.launch(arguments, owner)
                result = fixtures.AgentTests.launch(arguments, owner)
                if arguments[-1] == "validate":
                    controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS - 1)
                return result

            with mock.patch.object(w.c, "collect_profile", side_effect=collect), \
                    mock.patch.object(runtime.subprocess, "run", side_effect=inspect), \
                    self.serving(adapter, launch) as (port, token, _):
                async def script():
                    async with httpx.AsyncClient(headers={"Authorization": "Bearer " + token}, timeout=300) as http:
                        async with streamable_http_client(f"http://127.0.0.1:{port}/mcp", http_client=http) as streams:
                            async with ClientSession(streams[0], streams[1]) as session:
                                await session.initialize()
                                for operation, data in (
                                        ("start_profile", None),
                                        ("select_hypothesis", fixtures.AgentTests.selection(repo)),
                                        ("submit_patch", {"patch": patch.decode()}),
                                        ("start_validation", None)):
                                    response = await session.call_tool("investigate", {
                                        "message": fixtures.message(operation, data)})
                                    self.assertTrue(response.structuredContent["ok"], response)
                                    while True:
                                        response = await session.call_tool("investigate", {
                                            "message": fixtures.message("status")})
                                        self.assertTrue(response.structuredContent["ok"], response)
                                        if not response.structuredContent["worker"]:
                                            break
                                        await asyncio.sleep(.05)
                asyncio.run(script())
                end = time.monotonic() + 15
                while not (controller.root / "approval.json").exists() and time.monotonic() < end:
                    time.sleep(.05)
                self.assertTrue((controller.root / "approval.json").exists(), controller.load("runtime-service"))
                engine_running.clear()
                end = time.monotonic() + 15
                while controller.load("runtime-service")["state"] != "stopped" and time.monotonic() < end:
                    time.sleep(.05)
                self.assertEqual(controller.load("runtime-service")["state"], "stopped")
                self.assertFalse((controller.root / "runtime-error.json").exists())
                result = adapter.handoff(root / "publisher")
                self.assertEqual(len(result["publication"]["files"]), 2)
                self.assertEqual(launches.count("authorize"), 1)
                self.assertEqual(len([a for a in transport.read()[0]["attempts"].values() if a["publication"]]), 1)
                api.failure = "after-pr"
                with self.assertRaises(RuntimeError):
                    adapter.finalize(publisher=api)
                api.failure = None
                adapter.finalize(publisher=api)
                adapter.finalize(publisher=api)
                self.assertEqual(api.effects, ["branch", "pr"])


if __name__ == "__main__":
    unittest.main()
