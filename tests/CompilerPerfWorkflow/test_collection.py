"""Synthetic reducer/process fixtures; live diagnostics tests are separately opt-in."""

import copy
import importlib.util
import json
import os
from pathlib import Path
import socket
import subprocess
import sys
import tempfile
from concurrent.futures import ThreadPoolExecutor
import unittest
from unittest import mock

ROOT = Path(__file__).resolve().parents[2]
HERE = ROOT / "eng" / "scripts" / "compiler_perf"
sys.path.insert(0, str(HERE))
spec = importlib.util.spec_from_file_location("collection", HERE / "collection.py")
c = importlib.util.module_from_spec(spec)
spec.loader.exec_module(c)
FIXTURES = Path(__file__).parent / "fixtures"


def profile():
    return json.loads((FIXTURES / "profile.json").read_text())


class CollectionTests(unittest.TestCase):
    def test_normalized_metric_meanings(self):
        result = c.normalize_evidence(profile())
        self.assertEqual(result["status"], "collected")
        metrics = result["metrics"]
        self.assertEqual(metrics["elapsed_ms"], 4000)
        self.assertEqual(metrics["process_cpu_ms"], 2700)
        self.assertEqual(metrics["cpu_sample_weight"], 30)
        self.assertEqual(metrics["cpu_sample_unit"], "samples")
        self.assertEqual(metrics["allocated_bytes_estimate"], 300000)
        self.assertIn("sample", result["allocation_method"].lower())
        self.assertEqual(metrics["gc_pause_ms"], 400)
        self.assertEqual(metrics["gc_counts"], {"0": 1, "1": 0, "2": 1})
        self.assertEqual(metrics["managed_heap_bytes"], 12000000)
        self.assertEqual(metrics["peak_rss_bytes"], 90000000)
        self.assertIsNone(metrics["retained_heap_bytes"])
        self.assertIn("retained_heap_bytes", result["unavailable_reasons"])
        self.assertEqual(result["allocation_rate_integral_estimate_bytes"], 1300)
        self.assertEqual(result["stacks"][0]["weight"], 20)
        self.assertFalse(result["acceptance_eligible"])
        self.assertTrue(result["profiler_attached"])

    def test_invalid_profiles_fail_closed(self):
        mutations = [
            ("schema", lambda p: p.update(schema_version=2)),
            ("boolean schema", lambda p: p.update(schema_version=True)),
            ("reduction schema", lambda p: p["trace"].update(schema_version=True)),
            ("empty", lambda p: p.clear()),
            ("wrong pid", lambda p: p["trace"].update(process_id=43)),
            ("wrong runtime", lambda p: p["trace"].update(runtime="9.0.1")),
            ("provider", lambda p: p["trace"].update(providers=[])),
            ("truncated", lambda p: p["trace"].update(complete=False)),
            ("data loss", lambda p: p["trace"].update(lost_events=1)),
            ("cpu absent", lambda p: p["trace"].pop("cpu_sample_weight")),
            ("cpu no stacks", lambda p: p["trace"].update(stacks=[])),
            ("cpu negative", lambda p: p["trace"].update(cpu_sample_weight=-1)),
            ("allocation absent", lambda p: p["trace"].update(allocation_ticks=[])),
            ("allocation ordering", lambda p: p["trace"]["allocation_ticks"].reverse()),
            ("gc absent", lambda p: p["trace"].update(gc_intervals=[])),
            ("pause absent", lambda p: p["trace"].update(gc_pauses=[])),
            ("gc generation", lambda p: p["trace"]["gc_intervals"][0].update(generation=9)),
            ("gc reversed", lambda p: p["trace"]["gc_pauses"][0].update(end_ms=0)),
            ("heap units", lambda p: p["counters"]["observations"][-1].update(unit="MiB?")),
            ("counter cumulative", lambda p: p["counters"]["observations"][0].update(semantics="cumulative")),
            ("counters missing", lambda p: p["counters"].update(observations=[])),
            ("counter process", lambda p: p["counters"].update(process_id=9)),
            ("counter provider", lambda p: p["counters"].update(provider="Other")),
            ("counter ordering", lambda p: p["counters"]["observations"].reverse()),
            ("cpu nan", lambda p: p["process"].update(process_cpu_ms=float("nan"))),
            ("exit", lambda p: p["process"].update(exit_status=1)),
            ("pid reused", lambda p: p["process"]["end"].update(start_time="101")),
            ("wrong parent", lambda p: p["process"]["end"].update(parent_pid=1)),
            ("late attach", lambda p: p["coverage"].update(trace_before_resume=False)),
            ("short trace", lambda p: p["trace"].update(last_event_ms=10)),
            ("tool failed", lambda p: p["commands"][0].update(exit_status=1)),
            ("missing hash", lambda p: p.update(source_hashes=[])),
            ("acceptance", lambda p: p.update(acceptance_eligible=True)),
        ]
        for label, mutate in mutations:
            with self.subTest(case=label):
                evidence = profile()
                mutate(evidence)
                result = c.normalize_evidence(evidence)
                self.assertEqual(result["status"], "blocked_collection")
                self.assertTrue(result["reason"])
                self.assertNotIn("metrics", result)

    def test_single_failure_classification_table(self):
        for phase, code, stderr, issue in json.loads((FIXTURES / "failures.json").read_text()):
            with self.subTest(phase=phase, issue=issue, code=code):
                result = c.classify_failure(phase, code, stderr, issue)
                self.assertEqual(result["schema_version"], 1)
                self.assertEqual(result["kind"], "collection-failure")
                self.assertEqual(result["status"], "blocked_collection")
                self.assertEqual(result["phase"], phase)
                self.assertEqual(result["exit_status"], code)
                self.assertTrue(result["reason"])
                self.assertEqual(result["stderr"], stderr)

    def test_full_process_identity_and_ambiguity(self):
        expected = profile()["process"]["start"]
        self.assertEqual(c.select_compiler_process([expected], expected), expected)
        for field in expected:
            wrong = copy.deepcopy(expected)
            wrong[field] = ["wrong"] if field == "command" else "wrong"
            with self.subTest(field=field), self.assertRaises(ValueError):
                c.select_compiler_process([wrong], expected)
        for candidates in ([], [expected, expected]):
            with self.subTest(count=len(candidates)), self.assertRaises(ValueError):
                c.select_compiler_process(candidates, expected)
        with self.assertRaises(ValueError):
            c.select_compiler_process([{"pid": 42}], {"pid": 42})

    def test_counter_adapter_and_irregular_intervals(self):
        counters = c.read_counters(FIXTURES / "counters.csv", 42)
        self.assertEqual(counters, profile()["counters"])
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "bad.csv"
            for contents in ("{}", "Timestamp,Provider\n", "x,y,z\n1,2,3\n"):
                path.write_text(contents)
                with self.assertRaises(ValueError):
                    c.read_counters(path, 42)
            for timestamp in ("9/18/2026 10:00:00 AM", "09/18/2026 10:00:00"):
                path.write_text((FIXTURES / "counters.csv").read_text().replace(
                    "2026-09-18T10:00:00.0000000Z", timestamp))
                self.assertEqual(c.read_counters(path, 42)["observations"][0]["timestamp_ms"], 0)

    def test_counter_coverage_uses_trace_interval_not_tool_startup_delay(self):
        evidence = profile()
        evidence["coverage"]["duration_ms"] = 6900
        evidence["trace"]["last_event_ms"] = 5000
        evidence["counters"]["observations"][2]["timestamp_ms"] = 3500
        self.assertEqual(c.normalize_evidence(evidence)["status"], "collected")
        evidence["counters"]["observations"][2]["timestamp_ms"] = 2900
        self.assertEqual(c.normalize_evidence(evidence)["status"], "blocked_collection")

    def test_environment_and_log_redaction(self):
        with tempfile.TemporaryDirectory() as directory, mock.patch.dict(os.environ, {
            "GITHUB_TOKEN": "sensitive-value-123", "COPILOT_TOKEN": "other-secret",
            "AWS_SECRET_ACCESS_KEY": "private-key", "DOTNET_STARTUP_HOOKS": "evil.dll",
            "DOTNET_EnableDiagnostics": "0", "COMPlus_EnableDiagnostics": "0",
        }):
            env = c.target_environment(Path(directory))
            self.assertEqual(env["DOTNET_EnableDiagnostics"], "1")
            self.assertEqual(env["DOTNET_DefaultDiagnosticPortSuspend"], "1")
            for key in ("GITHUB_TOKEN", "COPILOT_TOKEN", "AWS_SECRET_ACCESS_KEY", "DOTNET_STARTUP_HOOKS",
                        "COMPlus_EnableDiagnostics"):
                self.assertNotIn(key, env)
            tooling = c.tool_environment(env)
            self.assertEqual(tooling["TMPDIR"], env["TMPDIR"])
            self.assertNotIn("GITHUB_TOKEN", tooling)
            cleaned = c.sanitize("GITHUB_TOKEN=sensitive-value-123 Bearer other-secret https://name:pass@host/a")
            for value in ("sensitive-value-123", "other-secret", "name:pass"):
                self.assertNotIn(value, cleaned)

    def test_bounds_and_owned_cleanup(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            owned = root / "owned"
            owned.mkdir()
            binary = owned / "bad.nettrace"
            binary.write_bytes(b"partial")
            other = root / "unrelated"
            other.write_bytes(b"keep")
            record = c.remove_incomplete(binary, owned)
            self.assertEqual(record["sha256"], c.m.hashlib.sha256(b"partial").hexdigest())
            self.assertEqual(record["status"], "removed_incomplete")
            self.assertFalse(binary.exists())
            with self.assertRaises(ValueError):
                c.remove_incomplete(other, owned)
            self.assertTrue(other.exists())
            with mock.patch.dict(c.LIMITS, artifact_bytes=3):
                with self.assertRaises(ValueError):
                    c.check_budget(root)

    def test_tool_timeout_failure_and_bounded_output(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            env = dict(os.environ)
            for script, expected in (
                ("import sys; print('failed',file=sys.stderr); sys.exit(7)", "tool_failure"),
                ("import time; time.sleep(30)", "timeout"),
                ("print('x' * 200000)", "budget_exceeded"),
            ):
                with self.subTest(expected=expected), mock.patch.dict(c.LIMITS, log_bytes=10000):
                    result = c.run_tool([sys.executable, "-c", script], root, env, root,
                                        "fixture", timeout=0.2 if expected == "timeout" else 5)
                    self.assertEqual(result["status"], "blocked_collection")
                    self.assertEqual(result["reason"], expected)
                    self.assertIn("exit_status", result)

    def test_tool_timeout_reaps_only_owned_descendants(self):
        with tempfile.TemporaryDirectory() as directory, socket.socket() as server:
            root = Path(directory)
            server.bind(("127.0.0.1", 0))
            server.listen()
            server.settimeout(15)
            port = server.getsockname()[1]
            client = (f"import socket,time; s=socket.create_connection(('127.0.0.1',{port}));"
                      "s.sendall(b'1'); time.sleep(60)")
            unrelated = subprocess.Popen([sys.executable, "-c", client])
            other, _ = server.accept()
            other.settimeout(.1)
            self.assertEqual(other.recv(1), b"1")
            owned = []
            try:
                parent = f"import subprocess,sys; subprocess.Popen([sys.executable,'-c',{client!r}]); {client}"
                with ThreadPoolExecutor(max_workers=1) as executor:
                    future = executor.submit(c.run_tool, [sys.executable, "-c", parent], root,
                                             dict(os.environ), root, "owned-tree", 5)
                    for _ in range(2):
                        connection, _ = server.accept()
                        connection.settimeout(10)
                        self.assertEqual(connection.recv(1), b"1")
                        owned.append(connection)
                    result = future.result(timeout=15)
                self.assertEqual(result["reason"], "timeout")
                for connection in owned:
                    try:
                        closed = connection.recv(1) == b""
                    except ConnectionResetError:
                        closed = True  # Windows reports terminated peers as reset rather than EOF.
                    self.assertTrue(closed)
                self.assertIsNone(unrelated.poll())
            finally:
                for connection in owned:
                    connection.close()
                other.close()
                unrelated.terminate()
                unrelated.wait(timeout=10)

    def test_dump_requires_hypothesis_and_verified_alive_identity(self):
        process = profile()["process"]["start"]
        for reason in ("", "cpu", None):
            with tempfile.TemporaryDirectory() as directory:
                result = c.collect_dump({"identity": process}, reason, Path(directory) / "dump")
                self.assertEqual(result["status"], "blocked_collection")
                self.assertEqual(result["phase"], "hypothesis")
        for problem in (PermissionError("denied"), ProcessLookupError("exited")):
            with tempfile.TemporaryDirectory() as directory, mock.patch.object(c, "snapshot_process", side_effect=problem):
                result = c.collect_dump({"identity": process, "hypothesis": "Live heap increases between full GCs",
                                         "profiling_replay": True}, "heap-growth", Path(directory) / "dump")
                self.assertEqual(result["status"], "blocked_collection")
                self.assertFalse(result["published"])
                self.assertIn("reason", result)

    def test_dump_tool_and_artifact_failures_preserve_evidence(self):
        identity = profile()["process"]["start"]
        document = {"identity": identity, "hypothesis": "RSS grows while managed heap remains stable",
                    "profiling_replay": True}
        for issue in ("permission_denied", "timeout", "tool_failure", "empty_artifact", "corrupt_artifact"):
            with self.subTest(issue=issue), tempfile.TemporaryDirectory() as directory:
                root = Path(directory) / "private"

                def run(arguments, *args, **kwargs):
                    if issue in ("empty_artifact", "corrupt_artifact"):
                        (root / "heap.dmp").write_bytes(b"" if issue == "empty_artifact" else b"bad!" * 2000)
                        return {"status": "collected", "exit_status": 0, "stderr": "tool exited zero"}
                    return c.classify_failure("dump", 17, "diagnostic failure", issue)

                with mock.patch.object(c, "snapshot_process", return_value=identity), \
                        mock.patch.object(c, "preflight", return_value={}), \
                        mock.patch.object(c, "verify_tools", return_value={"dotnet-dump": {"command": ["dotnet-dump"]}}), \
                        mock.patch.object(c, "run_tool", side_effect=run):
                    result = c.collect_dump(document, "unexplained-rss", root)
                self.assertEqual(result["status"], "blocked_collection")
                self.assertEqual(result["reason"], issue)
                self.assertFalse(result["published"])
                self.assertEqual(result["exit_status"], 0 if "artifact" in issue else 17)
                self.assertTrue((root / "dump-evidence.json").exists())
                self.assertFalse((root / "heap.dmp").exists())
                if "artifact" in issue:
                    self.assertEqual(result["incomplete_artifact"]["status"], "removed_incomplete")

    def test_preflight_fails_closed_without_budget_or_diagnostics(self):
        with tempfile.TemporaryDirectory() as directory:
            env = c.target_environment(Path(directory))
            with mock.patch.object(c.shutil, "disk_usage", return_value=type("Disk", (), {"free": 0})()):
                with self.assertRaisesRegex(ValueError, "disk"):
                    c.preflight(directory, env)
            for key, value in (("DOTNET_EnableDiagnostics", "0"), ("CORECLR_ENABLE_PROFILING", "1")):
                with self.subTest(key=key), self.assertRaises(ValueError):
                    c.preflight(directory, dict(env, **{key: value}))

    def test_collection_does_not_remove_another_collectors_lock(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            lock = root / "measurement.lock"
            lock.mkdir()
            manifest = {"owned_root": str(root), "invariants": {
                "workload": {}, "toolchain": {"dotnet": {"path": sys.executable}}}}
            with mock.patch.object(c, "frozen_inputs", return_value=[]), \
                    mock.patch.object(c.m, "file_record", return_value={"path": str(root), "sha256": "a" * 64}):
                result = c.collect_profile(manifest, {}, root / "profile")
            self.assertEqual(result["status"], "blocked_collection")
            self.assertTrue(lock.is_dir())

    def test_analyze_cannot_turn_blocked_collection_into_healthy_metrics(self):
        evidence = profile()
        evidence["status"] = "blocked_collection"
        self.assertEqual(c.normalize_evidence(evidence)["status"], "blocked_collection")
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "manifest.json"
            path.write_text(json.dumps(profile()))
            self.assertEqual(c.analyze_profile(path)["status"], "blocked_collection")

    def test_timing_output_requires_project_and_numeric_phase(self):
        valid = "=== FSharp.Compiler.Service ===\n  |Import mscorlib and FSharp.Core| 0.123| 0.123| 1|\n"
        self.assertEqual(c.validate_timings(valid), ["FSharp.Compiler.Service"])
        for text in ("", "No Fsc tasks found in binlog.",
                     "Fsc tasks found but no timing data present. Was --times flag set?",
                     "=== Unknown Project ===\n|phase|1.2|", "=== FSharp.Compiler.Service ===\n|header|time|"):
            with self.subTest(text=text), self.assertRaises(ValueError):
                c.validate_timings(text)

    def test_analyze_cli_corrupt_input_is_persistent_blocker(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            input_path, output = root / "bad.json", root / "summary.json"
            input_path.write_text('{"schema_version":')
            result = subprocess.run([sys.executable, str(HERE / "collection.py"), "analyze",
                                     "--input", str(input_path), "--output", str(output)],
                                    capture_output=True, text=True)
            self.assertNotEqual(result.returncode, 0)
            self.assertEqual(json.loads(output.read_text())["status"], "blocked_collection")


if __name__ == "__main__":
    unittest.main()
