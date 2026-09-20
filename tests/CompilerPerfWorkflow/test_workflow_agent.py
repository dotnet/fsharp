"""Portable agent interactions use synthetic diagnostics, never compiler speedups."""

from contextlib import contextmanager
import asyncio
import copy
from datetime import timedelta
import json
import os
from pathlib import Path
import socket
import subprocess
import tempfile
import threading
import time
from types import SimpleNamespace
import unittest
from unittest import mock

from test_collection import artifact_profile
import test_workflow as fixtures
from test_experiment import Repository, SOURCE
from experiment_state_cases import candidate_fixture
import agent_bridge as bridge
import workflow as w


def message(operation, data=None, identifier=None):
    return w.mem.encoded({"schema_version": 1, "id": identifier or operation,
                          "operation": operation, "data": data or {}}).decode()


class Worker:
    pid = 424242

    def __init__(self, code=0):
        self.code, self.signals = code, []

    def poll(self):
        return self.code

    def wait(self, timeout=None):
        if self.code is None:
            raise subprocess.TimeoutExpired("synthetic owned worker", timeout)
        return self.code

    def send_signal(self, value):
        self.signals.append(value)
        self.code = 130


def identify(pid):
    return {"pid": pid, "parent_pid": os.getpid(), "start_time": "synthetic"}


class MemoryTransport:
    """Unit-test acknowledgments only; ownership/restart/candidate cases use real bare Git."""

    scope, remote = "local_fixture", "synthetic"

    def __init__(self, root):
        self.state, self.head = w.mem.empty_state(), None
        self.journal = SimpleNamespace(repository=root / "synthetic-journal")
        self.journal.repository.mkdir()

    def read(self):
        return copy.deepcopy(self.state), self.head

    def write_state(self, base, head, candidate):
        if (base, head) != self.read():
            raise w.mem.Conflict("Synthetic CAS conflict")
        w.mem.validate_state(candidate)
        self.state = copy.deepcopy(candidate)
        self.head = w.m.fingerprint_inputs(candidate)[:40]
        return {"scope": self.scope, "acknowledged": True, "commit": self.head,
                "branch": w.mem.STATE_BRANCH, "remote": self.remote,
                "local": w.mem.GitJournal.receipt(self.head)}


class AgentTests(unittest.TestCase):
    fixture = fixtures.WorkflowTests.fixture

    @contextmanager
    def discovery(self, *, canonical=False):
        with tempfile.TemporaryDirectory() as directory:
            repo, controller, api, transport = self.fixture(
                Path(directory), transport_factory=None if canonical else MemoryTransport)
            request = {"schema_version": 2, "run_id": "run-1", "base_sha": repo.sha,
                       "primary_metric": "elapsed_ms"}
            self.assertEqual(controller.preflight(request)["action"], "reserve")
            yield repo, controller, api, transport

    @contextmanager
    def ready(self, *, canonical=False):
        with self.discovery(canonical=canonical) as fixture:
            controller = fixture[1]
            manifest = {"baseline_compiler": {}, "invariants": {}}

            def collect(_, __, root, **kwargs):
                root.mkdir()
                evidence = artifact_profile(root)
                evidence.update(compiler={}, fingerprints={})
                w.m.write_json(root / "evidence-manifest.json", evidence)

            controller.session.update(phase="ready", ready_at=controller.now().isoformat())
            controller.checkpoint()
            with mock.patch.object(w.Controller, "frozen_manifest", return_value=manifest), \
                    mock.patch.object(w.c, "collect_profile", side_effect=collect):
                yield fixture

    @staticmethod
    def selection(repo):
        return {**{k: v for k, v in repo.request().items()
                   if k in ("compiler_paths", "regressions", "hypothesis", "complexity")},
                "hot_operation": 0}

    @staticmethod
    def launch(arguments, controller):
        operation = arguments[-1]
        getattr(controller, "finish_operation" if operation == "finish" else operation)()
        return Worker()

    def test_fresh_discovery_has_no_invented_hypothesis(self):
        with self.discovery(canonical=True) as (_, controller, _, transport):
            self.assertIsNone(controller.session["attempt"])
            self.assertEqual(transport.read()[0]["attempts"], {})

    def test_empty_inbox_waits_after_delivering_complete_diagnostics(self):
        with self.discovery() as (_, controller, api, _):
            private = controller.root / "private"
            private.mkdir()
            profile = private / "profile"
            profile.mkdir()
            evidence = artifact_profile(profile)
            evidence.update(compiler={}, fingerprints={})
            w.m.write_json(profile / "evidence-manifest.json", evidence)
            controller.session.update(phase="ready", manifest={"path": "synthetic"},
                                      ready_at=controller.now().isoformat())
            with mock.patch.object(controller, "frozen_manifest", return_value={
                    "baseline_compiler": {}, "invariants": {}}), mock.patch.object(w.c, "collect_profile"):
                controller.start_active()
                controller.profile()
            status = controller.status()
            self.assertEqual(status["phase"], "awaiting_decision")
            self.assertIsNone(status["outcome"])
            self.assertTrue(status["diagnostics"]["stacks"])
            self.assertIn("allocated_bytes_estimate", status["diagnostics"]["metrics"])
            self.assertIn("binlog", status["diagnostics"])
            self.assertFalse((controller.root / "output").exists())
            self.assertEqual(api.effects, [])

    def test_explicit_no_finding_and_failed_collection_are_distinct(self):
        for case in ("valid", "missing", "failed", "tampered"):
            with self.subTest(case=case), self.ready() as (_, controller, api, transport):
                controller.start_active()
                if case == "failed":
                    with mock.patch.object(w.c, "collect_profile", side_effect=ValueError("tool missing")):
                        self.assertEqual(controller.profile()["outcome"], "blocked_collection")
                elif case != "missing":
                    controller.profile()
                    if case == "tampered":
                        Path(controller.session["profile"]["path"]).write_text("{}")
                if case == "valid":
                    result = controller.finish("completed_no_finding", "Sampled hot work is already shared")
                    self.assertEqual(result["outcome"], "completed_no_finding")
                else:
                    with self.assertRaises(ValueError):
                        controller.finish("completed_no_finding", "Ignore missing evidence")
                self.assertEqual(transport.read()[0]["attempts"], {})
                self.assertEqual(api.effects, [])

    def test_selection_requires_profile_and_an_observed_operation(self):
        with self.ready() as (repo, controller, _, _):
            controller.start_active()
            with self.assertRaises(ValueError):
                controller.select_hypothesis(self.selection(repo))
            controller.profile()
            for operation in (-1, 10000, True, "command"):
                with self.subTest(operation=operation), self.assertRaises(ValueError):
                    controller.select_hypothesis(dict(self.selection(repo), hot_operation=operation))
            result = controller.select_hypothesis(self.selection(repo))
            self.assertEqual(result["phase"], "selected")
            self.assertEqual(controller.session["attempt"]["collection"]["status"], "collected")
            with self.assertRaises(ValueError):
                controller.finish("completed_no_finding", "Cannot erase selected work")

    def test_rejected_revision_can_select_another_eligible_scope_not_repeat_itself(self):
        with tempfile.TemporaryDirectory() as directory:
            repo, controller, _, transport = self.fixture(Path(directory), transport_factory=MemoryTransport)
            other = "src/Compiler/CodeGen/Other.fs"
            (repo.root / other).write_text("module Other\nlet answer = 0\n")
            repo.git("add", other)
            repo.git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid",
                     "commit", "-qm", "second eligible scope")
            repo.sha = repo.git("rev-parse", "HEAD").decode().strip()
            self.assertEqual(controller.preflight({
                "schema_version": 2, "run_id": "run-1", "base_sha": repo.sha,
                "primary_metric": "elapsed_ms"})["action"], "reserve")
            controller.session.update(phase="ready", ready_at=controller.now().isoformat())
            controller.start_active()
            controller.session["phase"] = "awaiting_decision"
            deadline = controller.load("clocks")["active"]["deadline"]
            with mock.patch.object(controller, "diagnostics", return_value={
                    "stacks": [{"frames": ["Test.answer"], "weight": 1}], "profile_sha256": "a" * 64}):
                controller.select_hypothesis(self.selection(repo))
                a = controller.session["attempt"]
                a.update(outcome="rejected", reason="Synthetic unchanged timing", lifecycle="interrupted")
                controller.session.update(phase="awaiting_decision", revisions=[{"id": a["id"], "outcome": "rejected"}])
                controller.checkpoint()
                self.assertEqual(controller.select_hypothesis(self.selection(repo))["action"], "skip")
                result = controller.select_hypothesis(dict(self.selection(repo), compiler_paths=[other]))
                self.assertEqual(result["phase"], "selected")
            self.assertEqual(controller.load("clocks")["active"]["deadline"], deadline)
            records = transport.read()[0]["attempts"]
            self.assertEqual(len(records), 2)
            self.assertEqual(records[a["id"]]["reason"], "Synthetic unchanged timing")
            self.assertEqual(controller.session["attempt"]["id"], "run-1.3")

    def test_restart_each_idle_phase_keeps_deadline_and_patch_custody(self):
        with self.ready(canonical=True) as (repo, controller, api, transport):
            controller.start_active()
            deadline = controller.load("clocks")["active"]["deadline"]
            for phase in ("ready", "awaiting_decision", "selected", "patch_submitted"):
                if phase == "awaiting_decision":
                    controller.profile()
                elif phase == "selected":
                    controller.select_hypothesis(self.selection(repo))
                elif phase == "patch_submitted":
                    controller.submit_patch(repo.patch().decode())
                restarted = w.Controller(controller.root, repo.root, transport, api, now=controller.now)
                self.assertEqual(restarted.status()["phase"], phase)
                self.assertEqual(restarted.load("clocks")["active"]["deadline"], deadline)
                self.assertLessEqual(restarted.poll_active(), w.ACTIVE_SECONDS)
                if phase == "patch_submitted":
                    w.m.verify_files([restarted.session["patch"]])
                controller = restarted
            with self.assertRaises(ValueError):
                controller.submit_patch("late replacement")
            self.assertFalse((controller.root / "output").exists())

    def test_setup_and_near_active_deadline_save_truthful_partial_state(self):
        for case in ("setup", "readiness", "active"):
            with self.subTest(case=case), self.ready() as (_, controller, _, transport):
                now = controller.now()
                if case == "setup":
                    controller.session["phase"] = "reserved"
                    controller.now = lambda: now + timedelta(seconds=w.SETUP_SECONDS + 1)
                    with mock.patch.object(w, "capabilities") as capability:
                        self.assertEqual(controller.investigate()["outcome"], "blocked_setup")
                    capability.assert_not_called()
                elif case == "readiness":
                    controller.now = lambda: now + timedelta(seconds=w.STARTUP_SECONDS)
                    client = bridge.Bridge(controller, launch=self.launch, identify=identify)
                    client.investigate(message("start_profile"))
                    client.tick()
                    self.assertEqual(controller.status()["outcome"], "blocked_setup", controller.status())
                else:
                    controller.start_active()
                    controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS)
                    self.assertEqual(controller.profile()["outcome"], "cancelled")
                self.assertFalse((controller.root / "output").exists())
                state = transport.read()[0]
                self.assertIsNotNone(state["discoveries"]["run-1.discovery"]["outcome"])

    def test_setup_preflight_probe_uses_the_same_aggregate_deadline(self):
        with tempfile.TemporaryDirectory() as directory:
            repo, controller, api, transport = self.fixture(Path(directory), transport_factory=MemoryTransport)
            now = [controller.now()]
            controller.now = lambda: now[0]

            def slow_probe(_):
                now[0] += timedelta(seconds=w.SETUP_SECONDS)

            with mock.patch.object(api, "verify_source", side_effect=slow_probe):
                result = controller.preflight({"schema_version": 2, "run_id": "run-1",
                                               "base_sha": repo.sha, "primary_metric": "elapsed_ms"})
            self.assertEqual(result["outcome"], "blocked_setup")
            self.assertEqual(transport.read()[0], w.mem.empty_state())
            self.assertIsNone(controller.session)

    def test_canonical_discovery_blocks_other_owner_and_page_two_pr(self):
        from test_memory import pr
        for case in ("owner", "page-two"):
            with self.subTest(case=case), self.discovery(canonical=True) as (repo, controller, api, transport):
                other = w.Controller(controller.root.parent / "other", repo.root, transport, api,
                                     now=controller.now)
                if case == "page-two":
                    controller.finish("cancelled", "Owner cancelled explicitly")
                    api.items = [dict(pr(number=1), labels=[], marker=None, attempt_id=None,
                                      head_ref="unrelated", author="contributor", intent_id=None),
                                 dict(pr(), author=w.AUTHOR, intent_id=None)]
                decision = other.preflight(dict(controller.session["discovery_request"], run_id="other"))
                self.assertEqual(decision["action"], "blocked")
                self.assertEqual(len(transport.read()[0]["discoveries"]), 1)

    def test_data_only_parser_rejects_injection_corruption_and_bounds(self):
        valid = json.loads(message("status"))
        for field in ("command", "path", "url", "env", "credentials", "deadline", "policy",
                      "passed_tests", "accepted", "timeout"):
            for target in ("envelope", "data"):
                altered = copy.deepcopy(valid)
                (altered if target == "envelope" else altered["data"])[field] = "$(whoami); hostile PR"
                with self.subTest(field=field, target=target), self.assertRaises(ValueError):
                    bridge.parse_message(json.dumps(altered))
        for invalid in (b"\xff", "{", '{"schema_version":1,"schema_version":1}', "[[[[" * 2000,
                        "x" * (w.e.MAX_PATCH + 65536), message("accept", {"accepted": True}),
                        message("request_dump", {"reason": "shell", "hypothesis": "run command"}),
                        message("request_dump", {"reason": "retention", "hypothesis": ""}),
                        message("submit_patch", {"patch": "x" * (w.e.MAX_PATCH + 1)})):
            with self.subTest(invalid=str(invalid)[:80]), self.assertRaises(ValueError):
                bridge.parse_message(invalid)
        for reason in w.c.REASONS:
            parsed = bridge.parse_message(message("request_dump", {
                "reason": reason, "hypothesis": "Test retention against observed GC and RSS"}))
            self.assertEqual(parsed["data"]["reason"], reason)

    def test_source_export_and_mounts_cannot_expose_controller_or_credentials(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "controller-source")
            protected = [repo.root, root / "custody", root / "journal", root / "frozen"]
            for path in protected[1:]:
                path.mkdir()
            (repo.root / "private.key").write_text("must not enter exported source")
            source = root / "agent-source"
            bridge.export_source(repo.root, repo.sha, source, protected)
            self.assertTrue((source / SOURCE).is_file())
            for excluded in (".git", "global.json", "eng", ".github", "private.key"):
                self.assertFalse((source / excluded).exists(), excluded)
            mounts = [{"type": "bind", "source": str(source), "target": "/workspace"},
                      {"type": "tmpfs", "target": "/agent-tmp"}]
            bridge.validate_mounts(source, protected, mounts)
            command = bridge.agent_command(source, protected, image="registry.invalid/agent@sha256:" + "a" * 64,
                                           network="compiler-perf-fixture")
            self.assertIn("--read-only", command)
            self.assertIn("/usr/local/bin/copilot", command)
            for dangerous in ("--privileged", "--pid=host", "--network=host", "docker.sock", "--env-file"):
                self.assertNotIn(dangerous, " ".join(command))
            for extra in (
                    {"type": "bind", "source": str(root), "target": "/tmp"},
                    {"type": "bind", "source": str(protected[0]), "target": "/repo"},
                    {"type": "bind", "source": str(protected[1]), "target": "/custody"},
                    {"type": "bind", "source": str(root / "docker.sock"), "target": "/var/run/docker.sock"},
                    {"type": "bind", "source": str(root / ".git-credentials"), "target": "/credentials"}):
                with self.subTest(mount=extra), self.assertRaises(ValueError):
                    bridge.validate_mounts(source, protected, [*mounts, extra])
            for path in ("eng/scripts/compiler_perf/workflow.py", ".git/config", ".git-credentials", "docker.sock"):
                target = source / path
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_text("agent-controlled")
                with self.subTest(path=path), self.assertRaises(ValueError):
                    bridge.validate_mounts(source, protected, mounts)
                target.unlink()
                parent = target.parent
                while parent != source and not any(parent.iterdir()):
                    parent.rmdir()
                    parent = parent.parent

    def test_scripted_agent_profiles_waits_and_explicitly_finishes(self):
        with self.ready() as (_, controller, api, transport):
            client = bridge.Bridge(controller, launch=self.launch, identify=identify)
            start = message("start_profile")
            first = client.investigate(start)
            self.assertTrue(first["ok"], first)
            self.assertEqual(client.investigate(start), first)
            status = client.investigate(message("status"))
            self.assertEqual(status["status"]["phase"], "awaiting_decision")
            diagnostics = status["status"]["diagnostics"]
            self.assertTrue(diagnostics["stacks"])
            self.assertEqual(diagnostics["metrics"]["allocated_bytes_estimate"], 300000)
            self.assertIsNone(diagnostics["metrics"]["retained_heap_bytes"])
            self.assertFalse(diagnostics["binlog"]["acceptance_eligible"])
            changed = client.investigate(message("cancel", {"reason": "changed ID"}, "start_profile"))
            self.assertFalse(changed["ok"])
            result = client.investigate(message("finish_no_finding", {"reason": "No bounded useful change"}))
            self.assertTrue(result["ok"], result)
            late = client.investigate(message("start_profile", identifier="late"))
            self.assertFalse(late["ok"])
            client.close()
            self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"],
                             "completed_no_finding")
            self.assertEqual(api.effects, [])

    def test_worker_early_exit_restart_and_near_deadline_cancel(self):
        for case in ("early", "restart", "deadline"):
            with self.subTest(case=case), self.ready() as (_, controller, _, _):
                worker = Worker(0 if case == "early" else None)

                def launch(arguments, owner):
                    return self.launch(arguments, owner) if arguments[-1] == "finish" else worker

                client = bridge.Bridge(controller, launch=launch, identify=identify)
                result = client.investigate(message("start_profile"))
                self.assertTrue(result["ok"], result)
                if case == "restart":
                    restarted = bridge.Bridge(controller, launch=mock.Mock(), identify=identify)
                    state = restarted.investigate(message("status"))
                    self.assertTrue(state["recovery_required"], state)
                    self.assertFalse(restarted.investigate(message("start_profile", identifier="second"))["ok"])
                    self.assertEqual(worker.signals, [])
                elif case == "deadline":
                    now = controller.now()
                    controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS)
                client.tick()
                if case != "restart":
                    self.assertIn(controller.status()["outcome"], ("cancelled", "blocked_setup"))
                    self.assertFalse((controller.root / "output").exists())
                client.close()

    def test_scripted_candidate_rejection_and_synthetic_improvement(self):
        for mode in ("correctness-failure", "synthetic-improvement"):
            with self.subTest(mode=mode), candidate_fixture() as (root, repo, original, patch):
                _, controller, api, transport = self.fixture(root / "interaction", repo)
                self.assertEqual(controller.preflight({
                    "schema_version": 2, "run_id": "run-1", "base_sha": repo.sha,
                    "primary_metric": "elapsed_ms"})["action"], "reserve")
                manifest = w.c.read_bounded_json(original / "workload.json")
                w.m.write_json(controller.root / "workload.json", manifest)
                controller.session.update(phase="ready", ready_at=controller.now().isoformat(),
                                          manifest=w.m.file_record(controller.root / "workload.json"))
                controller.checkpoint()

                def collect(_, __, destination, **kwargs):
                    destination.mkdir()
                    w.m.write_json(destination / "evidence-manifest.json", w.c.read_bounded_json(root / "profile.json"))

                client = bridge.Bridge(controller, launch=self.launch, identify=identify)
                with mock.patch.object(w.c, "collect_profile", side_effect=collect):
                    self.assertTrue(client.investigate(message("start_profile"))["ok"])
                received = client.investigate(message("status"))["status"]["diagnostics"]
                self.assertIn("FSharp.Compiler.Service", received["binlog"]["projects"])
                self.assertTrue(client.investigate(message("select_hypothesis", self.selection(repo)))["ok"])
                self.assertTrue(client.investigate(message("submit_patch", {"patch": patch.decode()}))["ok"])
                if mode == "correctness-failure":
                    with mock.patch.object(w.e, "run_correctness",
                                           side_effect=ValueError("Required check failed: regression-0")):
                        result = client.investigate(message("start_validation"))
                else:
                    result = client.investigate(message("start_validation"))
                self.assertTrue(result["ok"], result)
                result = client.investigate(message("status"))["status"]
                expected = "measured_improvement" if mode == "synthetic-improvement" else "rejected"
                self.assertEqual(result["result"]["outcome"], expected, result)
                self.assertEqual(result["revisions"][0]["outcome"], expected)
                if mode == "synthetic-improvement":
                    payload = controller.validate_output()
                    self.assertEqual(len(payload["items"]), 1)
                    self.assertTrue(payload["items"][0]["draft"])
                    self.assertEqual(len([a for a in transport.read()[0]["attempts"].values()
                                          if a["publication"]]), 1)
                else:
                    self.assertFalse(client.investigate(message("finish_no_finding", {"reason": "Hide failure"}))["ok"])
                    self.assertTrue(client.investigate(message(
                        "select_hypothesis", self.selection(repo), identifier="retry-unchanged"))["ok"])
                    skipped = client.investigate(message("status"))["status"]["selection_result"]
                    self.assertEqual(skipped["action"], "skip", skipped)
                    self.assertTrue(client.investigate(message("finish_rejected", {"reason": "Retain failed revision"}))["ok"])
                    self.assertFalse((controller.root / "output").exists())
                    self.assertFalse((controller.candidate_root() / "tree").exists())
                    self.assertEqual((controller.candidate_root() / "proposal.patch").read_bytes(), patch)
                self.assertEqual(api.effects, [])
                client.close()

    def test_real_http_sdk_client_receives_profile_and_rejects_unsafe_requests(self):
        try:
            import httpx
            import uvicorn
            from mcp import ClientSession
            from mcp.client.streamable_http import streamable_http_client
        except ImportError:
            self.skipTest("Install the isolated requirements-agent.txt to exercise HTTP MCP")
        with self.ready() as (_, controller, _, _):
            controller.start_active()
            controller.profile()
            client = bridge.Bridge(controller, launch=self.launch, identify=identify)
            token = "a" * 64
            with self.assertNoLogs("uvicorn.error", level="ERROR"), socket.socket() as listener:
                listener.bind(("127.0.0.1", 0))
                port = listener.getsockname()[1]
                app = bridge.create_app(client, token, allowed_hosts=[f"127.0.0.1:{port}"])
                server = uvicorn.Server(uvicorn.Config(app, log_level="error", lifespan="on"))
                thread = threading.Thread(target=server.run, kwargs={"sockets": [listener]})
                thread.start()
                try:
                    end = time.monotonic() + 10
                    while not server.started and thread.is_alive() and time.monotonic() < end:
                        time.sleep(.02)
                    self.assertTrue(server.started, "Owned HTTP server did not become responsive")

                    async def script():
                        url = f"http://127.0.0.1:{port}/mcp"
                        async with httpx.AsyncClient(headers={"Authorization": "Bearer " + token},
                                                     timeout=60) as http:
                            denied = await http.post(url, headers={"Authorization": "Bearer wrong"}, content="{}")
                            self.assertEqual(denied.status_code, 401)
                            oversized = await http.post(url, content=b"x" * (w.e.MAX_PATCH * 8))
                            self.assertEqual(oversized.status_code, 413)
                            async with streamable_http_client(url, http_client=http) as (read, write, _):
                                async with ClientSession(read, write) as session:
                                    await session.initialize()
                                    tools = await session.list_tools()
                                    self.assertEqual([tool.name for tool in tools.tools], ["investigate"])
                                    result = await session.call_tool("investigate", {"message": message("status")})
                                    data = result.structuredContent
                                    self.assertIsNotNone(data, result)
                                    self.assertTrue(data["ok"], data)
                                    self.assertTrue(data["status"]["diagnostics"]["stacks"])
                                    extra = await http.post(url, json={
                                        "jsonrpc": "2.0", "id": 17, "method": "tools/call",
                                        "params": {"name": "investigate", "arguments": {
                                            "message": message("status"), "command": "whoami"}}})
                                    self.assertEqual(extra.status_code, 400)
                    asyncio.run(script())
                    self.assertTrue(client.investigate(message(
                        "finish_no_finding", {"reason": "Profile reviewed; no change"}))["ok"])
                finally:
                    server.should_exit = True
                    thread.join(timeout=10)
                    self.assertFalse(thread.is_alive(), "Owned HTTP server failed to stop")


if __name__ == "__main__":
    unittest.main()
