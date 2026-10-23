"""Trusted Actions boundaries exercised with fake sinks and local Git only."""

from datetime import timedelta
import json
import copy
import io
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
from types import SimpleNamespace
import unittest
from unittest import mock

import test_workflow_agent as fixtures
import runtime
import workflow as w
import test_workflow_compiled as compiled

try:
    import actions_adapter as actions
except ModuleNotFoundError:
    actions = None


class ActionsAdapterTests(unittest.TestCase):
    discovery = fixtures.AgentTests.discovery
    fixture = fixtures.AgentTests.fixture
    candidate = fixtures.fixtures.PublicationTests.candidate

    def setUp(self):
        self.assertIsNotNone(actions, "Executable Actions adapters are absent")

    def test_owner_configuration_is_explicit_and_not_a_read_only_token_fallback(self):
        environment = {"COMPILER_PERF_OWNER_LOGIN": "compiler-perf[bot]",
                       "COMPILER_PERF_AGE_RECIPIENT": "age1" + "a" * 58}
        with mock.patch.dict(os.environ, environment, clear=True):
            with self.assertRaisesRegex(ValueError, "COMPILER_PERF_OWNER_TOKEN"):
                actions.owner_configuration()
            with mock.patch.dict(os.environ, {"GH_TOKEN": "fixture-token"}):
                self.assertEqual(actions.owner_configuration()["login"], "compiler-perf[bot]")

    def test_provisioning_verifies_real_age_recipient_and_tool_before_active_work(self):
        self.assertTrue(hasattr(actions, "verify_retention"), "Provisioning must verify encryption before bootstrap")
        with tempfile.TemporaryDirectory() as directory, fixtures.TemporaryRecipient() as key:
            controller = SimpleNamespace(root=Path(directory))
            actions.verify_retention(controller, key.config)
            invalid = copy.deepcopy(key.config)
            invalid["recipient"] = invalid["recipient"][:-1] + ("q" if invalid["recipient"][-1] != "q" else "p")
            with self.assertRaisesRegex(ValueError, "Encryption tool failed"):
                actions.verify_retention(controller, invalid)
            self.assertEqual(list(controller.root.iterdir()), [])

    def test_private_interpreter_rejects_exposed_executable_prefix_and_stdlib(self):
        root = Path(tempfile.gettempdir()).resolve() / "protected-python-fixture"
        for field in ("executable", "prefix", "base_prefix", "stdlib"):
            with self.subTest(field=field):
                record = {key: str(root / key)
                          for key in ("executable", "prefix", "base_prefix", "stdlib")}
                actions.validate_python(record, root)
                record[field] = str(root.parent / "exposed-python")
                with self.assertRaises(ValueError):
                    actions.validate_python(record, root)

    def test_checked_artifact_inventory_rejects_extra_changed_and_linked_files(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "manifest.json").write_bytes(b"{}\n")
            plan = {"files": [dict(w.m.file_record(root / "manifest.json"), size_bytes=3, kind="manifest")]}
            actions.verify_inventory(root, plan)
            for mutation in ("changed", "extra", "directory", "hardlink"):
                with self.subTest(mutation=mutation), tempfile.TemporaryDirectory() as destination:
                    target = Path(destination)
                    shutil.copyfile(root / "manifest.json", target / "manifest.json")
                    if mutation == "changed":
                        (target / "manifest.json").write_bytes(b"changed")
                    elif mutation == "extra":
                        (target / "private.dmp").write_bytes(b"private")
                    elif mutation == "directory":
                        (target / "nested").mkdir()
                    else:
                        os.link(target / "manifest.json", target / "alias")
                    with self.assertRaises(ValueError):
                        actions.verify_inventory(target, plan)

    def test_actions_sink_recovers_upload_ack_loss_and_blocks_false_readback(self):
        with self.discovery(run_id="123") as (_, controller, _, _):
            controller.finish("blocked_setup", "Fixture owner configuration unavailable")
            staged = controller.session["artifacts"][0]
            plan = w.artifacts.retention_result(staged["root"], w.read_input(staged["manifest"]["path"]), controller.now())
            metadata = {"id": 42, "name": "compiler-perf-" + plan["manifest_sha256"], "expired": False,
                        "workflow_run": {"id": 123}, "size_in_bytes": plan["total_bytes"],
                        "created_at": controller.now().isoformat(),
                        "expires_at": (controller.now() + timedelta(days=14)).isoformat()}
            effects = []

            def transport(inventory, target):
                if not effects:
                    effects.append("upload")
                    raise OSError("upload succeeded, acknowledgment lost")
                for record in inventory["files"]:
                    shutil.copyfile(record["path"], target / Path(record["path"]).name)
                return {"id": 42, "retention_started_at": metadata["created_at"]}

            sink = actions.ActionsArtifactSink(controller, transport=transport)
            with mock.patch.object(controller.api, "get", create=True, return_value=metadata):
                with self.assertRaisesRegex(OSError, "acknowledgment lost"):
                    sink(plan)
                receipt = sink(plan)
                self.assertEqual(effects, ["upload"])
                self.assertEqual(receipt["scope"], "local_fixture")
                self.assertEqual(receipt["manifest_sha256"], plan["manifest_sha256"])
                for change in ({"expired": True}, {"workflow_run": {"id": 124}},
                               {"name": "other"}, {"expires_at": controller.now().isoformat()}):
                    with self.subTest(change=change), mock.patch.object(
                            controller.api, "get", return_value=dict(metadata, **change)), self.assertRaises(ValueError):
                        sink(plan)
            controller.now = lambda: w.mem.timestamp(plan["expires_at"])
            with self.assertRaisesRegex(ValueError, "expired"):
                sink(plan)

    def test_archive_framing_consumes_both_run_budgets_before_transport(self):
        with self.discovery(run_id="123") as (_, controller, _, _):
            controller.finish("blocked_setup", "Fixture compact conclusion")
            staged = controller.session["artifacts"][0]
            manifest = w.read_input(staged["manifest"]["path"])
            plan = w.artifacts.retention_result(staged["root"], manifest, controller.now())
            self.assertEqual(plan["limits"], {"total_bytes": 512 * 1024**2, "compact_bytes": 20 * 1024**2})
            transport = mock.Mock(side_effect=AssertionError("Transport must not start"))
            sink = actions.ActionsArtifactSink(controller, transport=transport)
            for limit, used in (("total_bytes", manifest["total_bytes"]), ("compact_bytes", manifest["compact_bytes"])):
                with self.subTest(limit=limit):
                    bounded = copy.deepcopy(plan)
                    bounded["limits"][limit] = used + 1
                    with self.assertRaisesRegex(ValueError, "archive overhead"):
                        sink(bounded)
                    # Without framing accounting the same assertion must fail.
                    with mock.patch.object(actions, "archive_bound", side_effect=lambda size, _: size):
                        with self.assertRaises(AssertionError):
                            with self.assertRaises(ValueError):
                                sink(bounded)
            self.assertEqual(transport.call_count, 2)

    def test_publisher_reconciles_partial_effects_without_another_draft(self):
        for crash in ("after-branch", "after-pr", None):
            with self.subTest(crash=crash), self.candidate() as (controller, api, transport):
                controller.validate_output()
                adapter = runtime.Runtime(controller, controller.root.parent / "public")
                destination = controller.root / "publisher"
                adapter.export(destination)
                publisher = actions.ActionsPublisher(adapter, destination)
                api.failure = crash
                if crash:
                    with self.assertRaises(RuntimeError):
                        adapter.finalize(publisher=publisher)
                    self.assertIsNotNone(transport.read()[0]["attempts"]["attempt-1"]["publication"])
                api.failure = None
                self.assertEqual(adapter.finalize(publisher=publisher)["action"], "acknowledge")
                self.assertEqual(api.effects, ["branch", "pr"])
                adapter.finalize(publisher=publisher)
                self.assertEqual(api.effects, ["branch", "pr"])

    def test_publisher_rejects_tampered_protected_export_before_effect(self):
        with self.candidate() as (controller, api, _):
            controller.validate_output()
            adapter = runtime.Runtime(controller, controller.root.parent / "public")
            destination = controller.root / "publisher"
            adapter.export(destination)
            original = {p.name: p.read_bytes() for p in destination.iterdir()}
            for mutation in ("patch", "payload", "extra"):
                with self.subTest(mutation=mutation):
                    for name, data in original.items():
                        (destination / name).write_bytes(data)
                    (destination / "other").unlink(missing_ok=True)
                    if mutation == "extra":
                        (destination / "other").write_bytes(b"untrusted")
                    else:
                        (destination / ("proposal.patch" if mutation == "patch" else "safe-output.json")).write_bytes(b"untrusted")
                    with self.assertRaises(ValueError):
                        adapter.finalize(publisher=actions.ActionsPublisher(adapter, destination))
                    self.assertEqual(api.effects, [])

    def test_publisher_api_transport_is_fixed_json_and_denial_is_not_absence(self):
        controller = SimpleNamespace(remaining=lambda *_: 60)
        publisher = actions.ActionsPublisher(SimpleNamespace(controller=controller), Path("unused"))
        for status in (200, 201, 403, 404):
            with self.subTest(status=status):
                response = subprocess.CompletedProcess([], 0 if status < 300 else 1,
                    f"HTTP/2.0 {status} fixture\r\n\r\n{{}}\n".encode(), b"")
                with mock.patch.object(actions.subprocess, "run", return_value=response) as run:
                    if status == 403:
                        with self.assertRaisesRegex(ValueError, "denied"):
                            publisher.request("pulls", absent=True)
                    elif status == 404:
                        self.assertIsNone(publisher.request("pulls", absent=True))
                    else:
                        payload = {"body": "$(not-a-command)"}
                        self.assertEqual(publisher.request("pulls", payload), {})
                        arguments = run.call_args
                        self.assertEqual(arguments.args[0], ["gh", "api", "--include",
                            "repos/dotnet/fsharp/pulls", "--method", "POST", "--input", "-"])
                        self.assertEqual(json.loads(arguments.kwargs["input"]), payload)

    def test_finalization_without_a_candidate_keeps_truthful_outcomes(self):
        for outcome in ("blocked_setup", "cancelled", "blocked_collection"):
            with self.subTest(outcome=outcome), self.discovery() as (_, controller, api, _):
                controller.finish(outcome, "Fixture material blocker")
                adapter = runtime.Runtime(controller, controller.root.parent / "public")
                result = adapter.finalize()
                self.assertEqual(result["status"]["outcome"], outcome)
                self.assertEqual(api.effects, [])

    def test_failed_evidence_acknowledgment_cannot_call_a_publisher(self):
        with self.discovery() as (_, controller, _, _):
            controller.finish("blocked_setup", "Fixture setup incomplete")
            adapter = runtime.Runtime(controller, controller.root.parent / "public")
            publisher = mock.Mock()
            with self.assertRaisesRegex(OSError, "denied"):
                adapter.finalize(publisher=publisher, sink=mock.Mock(side_effect=OSError("denied")))
            publisher.publish.assert_not_called()

    def test_finalization_cli_reports_transport_failure_not_setup_or_no_finding(self):
        with self.discovery() as (_, controller, api, transport), \
                mock.patch.object(runtime, "sys", SimpleNamespace(platform="linux", prefix=str(controller.root),
                    flags=SimpleNamespace(isolated=1))), \
                mock.patch.object(runtime, "validate_layout"), \
                mock.patch.object(w, "Controller", return_value=controller), \
                mock.patch.object(w, "GitHub", return_value=api), \
                mock.patch("transport.JournalTransport", return_value=transport), \
                mock.patch.object(runtime.Runtime, "finalize", side_effect=ValueError("Sink readback denied")), \
                mock.patch("sys.stdout", new_callable=io.StringIO) as output, \
                mock.patch.dict(os.environ, {"RUNNER_TEMP": str(controller.root / "temp")}):
            result = runtime.main(["finalize", "--root", str(controller.root),
                "--repository", str(controller.repository), "--journal", str(transport.journal.repository),
                "--workspace", str(controller.root.parent / "public"), "--destination", str(controller.root / "publisher")])
            self.assertEqual(result, 2)
            self.assertEqual(json.loads(output.getvalue()), {
                "outcome": "blocked_finalization", "reason": "Sink readback denied"})

    def test_readiness_requires_full_active_and_finalization_inside_actual_job(self):
        for elapsed, admitted in ((0, True), (4159, True), (4161, False), (-1, False)):
            with self.subTest(elapsed=elapsed), self.discovery() as (_, controller, _, _):
                adapter = runtime.Runtime(controller, controller.root.parent / "public")
                controller.session.update(phase="ready", ready_at=controller.now().isoformat())
                controller.save("session", controller.session)
                record = adapter.arm()
                record["job_started_at"] = (controller.now() - timedelta(seconds=elapsed)).isoformat()
                controller.save("runtime", record)
                if admitted:
                    adapter.agent_ready(lambda: None)
                    clocks = controller.load("clocks")
                    self.assertEqual((w.mem.timestamp(clocks["active"]["deadline"]) -
                                      controller.now()).total_seconds(), w.ACTIVE_SECONDS)
                else:
                    with self.assertRaisesRegex(ValueError, "enclosing job"):
                        adapter.agent_ready(lambda: None)
                    self.assertNotIn("active", controller.load("clocks"))

    def test_compiled_provisioning_uses_actual_metadata_and_never_executes_dispatch_text(self):
        graph = compiled.load_graph()
        harness = """
const input = JSON.parse(require('node:fs').readFileSync(0, 'utf8'));
const calls = [];
const fs = {existsSync: () => false};
const exec = {execFileSync: (...args) => calls.push(args)};
const github = {paginate: async () => input.jobs, rest: {actions: {listJobsForWorkflowRunAttempt: {}}}};
const context = {repo: {owner: 'dotnet', repo: 'fsharp'}, runId: 123};
const mockRequire = name => name === 'node:fs' ? fs : name === 'node:child_process' ? exec : null;
new (Object.getPrototypeOf(async function(){}).constructor)('require','github','context', input.script)(
  mockRequire, github, context).then(() => console.log(JSON.stringify({calls}))).catch(error => {
    console.log(JSON.stringify({calls, error: error.message}));
});
"""
        for missing in ("none", "start", "runner", "token"):
            with self.subTest(missing=missing):
                jobs = [{"name": "agent", "runner_name": "fixture-runner", "status": "in_progress",
                         "started_at": w.datetime.now(w.timezone.utc).isoformat()}]
                if missing == "start":
                    jobs[0]["started_at"] = None
                if missing == "runner":
                    jobs[0]["runner_name"] = "other"
                environment = dict(w.c.tool_environment(), GH_TOKEN="" if missing == "token" else "fixture",
                    GITHUB_RUN_ATTEMPT="2", GITHUB_SHA="a" * 40, GITHUB_JOB="agent", RUNNER_NAME="fixture-runner")
                # The script's getuid is used only as an install argument, never for host mutation.
                script = compiled.code(compiled.step(graph, "provision")).replace("process.getuid()", "'fixture-uid'")
                result = subprocess.run(["node", "-e", harness], input=json.dumps({"jobs": jobs, "script": script}),
                    text=True, capture_output=True, check=True, timeout=15, env=environment)
                value = json.loads(result.stdout)
                if missing != "none":
                    self.assertIn("blocked_setup", value["error"])
                    self.assertEqual(value["calls"], [])
                else:
                    self.assertEqual([c[0] for c in value["calls"]],
                                     ["/usr/bin/sudo", "/usr/bin/git", "/usr/bin/git", "/usr/bin/python3"])
                    self.assertEqual(value["calls"][-1][1][-1], jobs[0]["started_at"])
                    self.assertIn("-I", value["calls"][-1][1])
                    self.assertIn("a" * 40, value["calls"][2][1])

    def test_real_artifact_entrypoint_reconciles_existing_upload(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            source, receipt = root / "summary.json", root / "effects.json"
            source.write_bytes(b'{"conclusion":"synthetic"}\n')
            environment = dict(w.c.tool_environment(),
                COMPILER_PERF_ARTIFACT_MODULE=str(Path(__file__).parent / "fixtures" / "ActionsArtifact.cjs"),
                COMPILER_PERF_FIXTURE_RECEIPT=str(receipt))
            Path(str(receipt) + ".lose-ack").touch()
            for index in range(3):
                destination = root / str(index)
                destination.mkdir()
                result = subprocess.run(["node", str(Path(actions.__file__).with_name("actions_artifact.cjs"))],
                    input=json.dumps({"name": "compiler-perf-fixture", "files": [{"path": str(source)}],
                                      "receipt_path": str(root / "upload-clock.json"),
                                      "destination": str(destination)}),
                    text=True, capture_output=True, timeout=15, env=environment)
                if index == 0:
                    self.assertEqual(result.returncode, 2)
                    started = json.loads((root / "upload-clock.json").read_text())["retention_started_at"]
                    continue
                self.assertEqual(result.returncode, 0, result.stderr)
                value = json.loads(result.stdout)
                self.assertEqual(value["id"], 42)
                self.assertEqual(value["retention_started_at"], started)
                self.assertEqual((destination / source.name).read_bytes(), source.read_bytes())
            self.assertEqual(json.loads(receipt.read_text())["uploads"], 1)

    def test_compiled_quarantine_exports_only_empty_framework_bookkeeping(self):
        graph = compiled.load_graph()
        harness = """
const input = JSON.parse(require('node:fs').readFileSync(0, 'utf8'));
const value = {calls: [], files: []};
const fs = {
  existsSync: p => p === '/tmp/gh-aw' || input.mode === 'existing' && p.endsWith('/stock-logs'),
  lstatSync: () => ({isSymbolicLink: () => input.mode === 'symlink', isDirectory: () => true}),
  mkdirSync: p => {value.mkdir = p},
  writeFileSync: (...args) => value.files.push(args)
};
const exec = {execFileSync: (...args) => value.calls.push(args)};
const mockRequire = name => name === 'node:fs' ? fs : name === 'node:child_process' ? exec : null;
new (Object.getPrototypeOf(async function(){}).constructor)('require','context','core',input.script)(
  mockRequire, {runId: 123}, {info: () => {}}).then(() => console.log(JSON.stringify(value))).catch(error => {
    console.log(JSON.stringify({...value, error: error.message}));
});
"""
        for mode in ("normal", "symlink", "existing"):
            with self.subTest(mode=mode):
                environment = dict(w.c.tool_environment(), GITHUB_RUN_ATTEMPT="2")
                script = compiled.code(compiled.step(graph, "quarantine")).replace("process.getuid()", "'fixture-uid'")
                result = subprocess.run(["node", "-e", harness], input=json.dumps({"mode": mode, "script": script}),
                    text=True, capture_output=True, check=True, timeout=15, env=environment)
                value = json.loads(result.stdout)
                if mode == "normal":
                    self.assertEqual(value["calls"][1][1],
                        ["-n", "/usr/bin/mv", "--", "/tmp/gh-aw", "/var/lib/compiler-perf/123-2/stock-logs"])
                    self.assertEqual(value["files"], [["/tmp/gh-aw/agent_output.json", '{"items":[]}\n',
                                                       {"flag": "wx", "mode": 0o600}]])
                else:
                    self.assertIn("quarantine" if mode == "existing" else "Unsafe", value["error"])
                    self.assertEqual(len(value["calls"]), 1)
                    self.assertEqual(value["files"], [])


if __name__ == "__main__":
    unittest.main()
