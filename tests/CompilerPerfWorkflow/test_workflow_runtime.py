"""Pinned runtime adapters: local Git/HTTP only, never hosted containment proof."""

from datetime import timedelta
import asyncio
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
import unittest
from unittest import mock

import test_workflow_agent as fixtures
import agent_bridge as bridge
import workflow as w
import runtime


def container():
    return {
        "Id": "a" * 64, "Name": "/awf-agent", "State": {"Running": True},
        "Config": {"Image": "ghcr.io/github/gh-aw-firewall/agent:0.25.55",
                   "Env": ["COPILOT_GITHUB_TOKEN=" + runtime.PROXY_CREDENTIAL,
                           "COPILOT_API_KEY=" + runtime.PROXY_CREDENTIAL, "COPILOT_MODEL=gpt-5.6-sol",
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

    def test_pinned_configuration_uses_stock_hooks_and_full_envelopes(self):
        config = runtime.configuration()
        self.assertEqual(config["pins"], {
            "gh_aw": "v0.76.1", "gh_aw_commit": "58d1bedbb7200f59c2d224151339e38fd8687d05",
            "copilot": "1.0.52", "awf": "v0.25.55", "gateway": "v0.3.19"})
        self.assertEqual(config["engine"]["mcp"], {"session-timeout": "6h", "tool-timeout": "5m"})
        self.assertEqual(config["engine"]["harness"], "compiler_perf_harness.cjs")
        self.assertEqual(config["budgets"], {"job": 360, "setup": 60, "execution": 250, "finalization": 40})
        self.assertEqual(config["hooks"], {"pre-steps": "setup", "pre-agent-steps": "start-service",
                                          "post-steps": "handoff"})
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
            controller.now = lambda: now + timedelta(seconds=w.FINALIZATION_SECONDS)
            client.tick()
            self.assertEqual(len(worker.signals), 1)
            self.assertEqual(client.receipt["state"], "worker_failed")
            self.assertFalse((controller.root / "approval.json").exists())
            client.close()

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
                value["Mounts"].append({"Type": "bind", "Source": adapter.workspace.as_posix(),
                                        "Destination": "/host/workspace", "RW": True})
                proxy["State"]["Running"] = not denied
                results = [subprocess.CompletedProcess([], 0, w.mem.encoded(v), b"") for v in ([value], [proxy])]
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

    def test_failed_engine_blocks_even_previously_authorized_handoff(self):
        with self.ready() as (_, controller, _, _):
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            controller.save("runtime-service", {"state": "stopped"})
            with self.assertRaisesRegex(ValueError, "Stock engine ended failure"):
                adapter.handoff(engine_outcome="failure")
            self.assertFalse((controller.root / "output").exists())

    def test_service_readiness_detects_early_exit_without_adopting_pid(self):
        for code in (0, 2):
            with self.subTest(code=code), self.ready() as (_, controller, _, _):
                adapter = runtime.Runtime(controller, controller.root.parent / "visible")
                child = fixtures.Worker(code)
                with self.assertRaisesRegex(ValueError, "exited"):
                    adapter.wait_service(child, mock.Mock(return_value=False))
                self.assertEqual(child.signals, [])

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

        with self.ready() as (_, controller, api, transport), socket.socket() as listener:
            listener.bind(("127.0.0.1", 0))
            port = listener.getsockname()[1]
            adapter = runtime.Runtime(controller, controller.root.parent / "visible")
            adapter.arm()
            (controller.root / "agent-server").mkdir()
            controller.save("runtime-service", {"state": "launching"})
            token = bridge.endpoint_token(controller.root)
            errors, launches = [], []

            def launch(arguments, owner):
                launches.append(arguments[-1])
                if arguments[-1] == "profile":
                    time.sleep(.1)
                return fixtures.AgentTests.launch(arguments, owner)

            def run():
                try:
                    runtime.serve(adapter, sockets=[listener], launch=launch,
                                  identify=fixtures.identify)
                except BaseException as error:
                    errors.append(error)

            with mock.patch.object(bridge, "PORT", port), mock.patch.object(adapter, "inspect_agent") as probe:
                thread = threading.Thread(target=run)
                thread.start()
                try:
                    end = time.monotonic() + 15
                    while not asyncio.run(runtime.probe_service(token)) and time.monotonic() < end:
                        self.assertTrue(thread.is_alive(), errors)
                        time.sleep(.05)
                    self.assertNotIn("active", controller.load("clocks"))

                    async def script():
                        async with httpx.AsyncClient(headers={"Authorization": "Bearer " + token}) as http:
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
                finally:
                    controller.save("runtime-stop", {"reason": "Owned test cleanup",
                                                     "requested_at": controller.now().isoformat()})
                    thread.join(timeout=30)
                    self.assertFalse(thread.is_alive(), "Owned runtime did not stop")
                self.assertEqual(errors, [])


if __name__ == "__main__":
    unittest.main()
