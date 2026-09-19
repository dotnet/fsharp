"""Synthetic reducer/process fixtures; live diagnostics tests are separately opt-in."""

import copy
from contextlib import ExitStack
import importlib.util
import itertools
import json
import os
from pathlib import Path
import socket
import subprocess
import sys
import tempfile
from types import SimpleNamespace
from concurrent.futures import ThreadPoolExecutor
import unittest
from unittest import mock

from test_measurement import measurements

ROOT = Path(__file__).resolve().parents[2]
HERE = ROOT / "eng" / "scripts" / "compiler_perf"
sys.path.insert(0, str(HERE))
spec = importlib.util.spec_from_file_location("collection", HERE / "collection.py")
c = importlib.util.module_from_spec(spec)
spec.loader.exec_module(c)
FIXTURES = Path(__file__).parent / "fixtures"
TIMINGS = "=== FSharp.Compiler.Service ===\n  |Import mscorlib and FSharp.Core| 0.123| 0.123| 1|\n"


def profile():
    return json.loads((FIXTURES / "profile.json").read_text())


def artifact_profile(root):
    evidence = profile()
    for name, text in (("compiler.nettrace", "synthetic trace, not real EventPipe"),
                       ("trace-reduction.json", json.dumps(evidence["trace"])),
                       ("counters.csv", (FIXTURES / "counters.csv").read_text()),
                       ("diagnostic.binlog", "synthetic binlog"),
                       ("timings.log", TIMINGS)):
        (root / name).write_text(text, encoding="utf-8")
    evidence["artifacts"] = [c.m.file_record(root / name) for name in
                             ("compiler.nettrace", "trace-reduction.json", "counters.csv")]
    evidence["commands"][0]["stdout"] = "Trace completed."
    evidence["timings"] = {
        "status": "collected", "acceptance_eligible": False,
        "build": {"status": "collected", "exit_status": 0},
        "extraction": {"status": "collected", "exit_status": 0, "stdout": TIMINGS,
                       "logs": [c.m.file_record(root / "timings.log")]},
        "binlog": c.m.file_record(root / "diagnostic.binlog"),
        "extractor": c.m.file_record(ROOT / "eng" / "scripts" / "ExtractTimingsFromBinlog.fsx"),
        "output": c.m.file_record(root / "timings.log"),
        "output_sha256": c.m.file_record(root / "timings.log")["sha256"],
        "projects": ["FSharp.Compiler.Service"],
    }
    return evidence


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
            ("huge integer", lambda p: p["process"].update(process_cpu_ms=10**400)),
            ("heap overflow", lambda p: p["counters"]["observations"][-1].update(value=1e308)),
            ("allocation overflow", lambda p: [t.update(bytes=1e308) for t in p["trace"]["allocation_ticks"]]),
            ("rate overflow", lambda p: [o.update(value=1e308) for o in p["counters"]["observations"][:3]]),
            ("future counters", lambda p: [o.update(timestamp_ms=o["timestamp_ms"] + 100000)
                                            for o in p["counters"]["observations"]]),
            ("future heap", lambda p: p["counters"]["observations"][-1].update(timestamp_ms=100000)),
            ("future trace", lambda p: p["trace"].update(last_event_ms=100000)),
            ("before trace", lambda p: p["trace"].update(first_event_ms=150)),
            ("missing trace heap", lambda p: p["trace"].pop("heap_observations")),
            ("future trace heap", lambda p: p["trace"]["heap_observations"][0].update(timestamp_ms=100000)),
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

    def test_allocation_bucket_may_start_before_first_event(self):
        evidence = profile()
        evidence["trace"]["first_event_ms"] = 20
        evidence["trace"]["allocation_ticks"][0]["timestamp_ms"] = 0
        evidence["trace"]["allocation_aggregation"] = {
            "kind": "fixed-time-buckets", "bucket_width_ms": 100,
            "timestamp": "bucket-start", "source_event_count": 3}
        self.assertEqual(c.normalize_evidence(evidence)["status"], "collected")
        evidence["trace"]["first_event_ms"] = 120
        self.assertEqual(c.normalize_evidence(evidence)["status"], "blocked_collection")

    def test_artifact_backed_analysis_and_disagreements(self):
        cases = (
            ("valid", lambda e, r: None),
            ("counter mismatch", lambda e, r: e["counters"]["observations"][-1].update(value=13)),
            ("missing timings", lambda e, r: e.update(timings={"status": "collected"})),
            ("missing binlog", lambda e, r: (r / "diagnostic.binlog").unlink()),
            ("changed output", lambda e, r: (r / "timings.log").write_text("No Fsc tasks found in binlog.")),
            ("failed build", lambda e, r: e["timings"]["build"].update(exit_status=1)),
            ("failed extraction", lambda e, r: e["timings"]["extraction"].update(status="blocked_collection")),
            ("wrong output hash", lambda e, r: e["timings"].update(output_sha256="a" * 64)),
            ("wrong projects", lambda e, r: e["timings"].update(projects=["Other"])),
            ("empty trace", lambda e, r: (r / "compiler.nettrace").write_bytes(b"")),
            ("changed trace", lambda e, r: (r / "compiler.nettrace").write_bytes(b"changed")),
            ("reduction mismatch", lambda e, r: e["trace"]["allocation_ticks"][0].update(bytes=200000)),
            ("missing trace completion", lambda e, r: e["commands"][0].update(stdout="")),
            ("duplicate artifact", lambda e, r: e["artifacts"].append(e["artifacts"][0])),
            ("trace budget", lambda e, r: None),
            ("total budget", lambda e, r: None),
        )
        for label, mutate in cases:
            with self.subTest(case=label), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                evidence = artifact_profile(root)
                mutate(evidence, root)
                path = root / "evidence-manifest.json"
                c.m.write_json(path, evidence)
                limits = {"trace_bytes": 1} if label == "trace budget" else (
                    {"total_bytes": 1} if label == "total budget" else {})
                with mock.patch.dict(c.LIMITS, limits):
                    result = c.analyze_profile(path)
                self.assertEqual(result["status"], "collected" if label == "valid" else "blocked_collection")
                if label == "valid":
                    self.assertEqual(result["metrics"], c.normalize_evidence(evidence)["metrics"])
                else:
                    self.assertNotIn("metrics", result)

    def test_rehashed_invalid_timing_output_is_not_evidence(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            evidence = artifact_profile(root)
            text = "No Fsc tasks found in binlog."
            (root / "timings.log").write_text(text)
            record = c.m.file_record(root / "timings.log")
            evidence["timings"].update(output=record, output_sha256=record["sha256"])
            evidence["timings"]["extraction"].update(stdout=text, logs=[record])
            path = root / "evidence-manifest.json"
            c.m.write_json(path, evidence)
            self.assertEqual(c.analyze_profile(path)["status"], "blocked_collection")

    def test_analysis_verifies_requested_dump_evidence(self):
        for problem in (None, "missing", "hash", "size", "boolean size", "identity", "validation", "status-only"):
            with self.subTest(problem=problem), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                evidence = artifact_profile(root)
                dump = root / "heap.dmp"
                dump.write_bytes(b"MDMP" + bytes(5000))
                evidence["dump"] = {
                    "status": "collected", "published": False, "identity": evidence["process"]["start"],
                    "artifact": dict(c.m.file_record(dump), size_bytes=dump.stat().st_size),
                    "reason_for_dump": "heap-growth",
                    "hypothesis": "Heap grows across full GCs",
                    "command": {"status": "collected", "exit_status": 0},
                    "validation": {"status": "collected", "exit_status": 0}}
                if problem == "missing":
                    dump.unlink()
                elif problem == "hash":
                    dump.write_bytes(b"MDMP" + b"x" + bytes(4999))
                elif problem in ("size", "boolean size"):
                    evidence["dump"]["artifact"]["size_bytes"] = True if problem == "boolean size" else 5005
                elif problem == "identity":
                    evidence["dump"]["identity"] = dict(evidence["process"]["start"], start_time="999")
                elif problem == "validation":
                    evidence["dump"]["validation"]["exit_status"] = 1
                elif problem == "status-only":
                    evidence["dump"] = {"status": "collected"}
                path = root / "evidence-manifest.json"
                c.m.write_json(path, evidence)
                self.assertEqual(c.analyze_profile(path)["status"], "blocked_collection" if problem else "collected")

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
            (owned / "first.bin").write_bytes(b"1234")
            (owned / "second.bin").write_bytes(b"5678")
            with mock.patch.dict(c.LIMITS, artifact_bytes=4, total_bytes=7):
                with self.assertRaisesRegex(ValueError, "budget"):
                    c.check_budget(owned)

    def test_private_directory_and_dump_inherit_owner_only_access(self):
        with tempfile.TemporaryDirectory() as directory:
            root = c.private_directory(Path(directory) / "private")
            dump = root / "heap.dmp"
            dump.write_bytes(b"private")
            if os.name == "nt":
                result = subprocess.run(
                    ["powershell.exe", "-NoProfile", "-NonInteractive", "-Command",
                     f"$ErrorActionPreference='Stop';$acl=Get-Acl -LiteralPath '{root}';"
                     f"$file=Get-Acl -LiteralPath '{dump}';"
                     "@{protected=$acl.AreAccessRulesProtected;owner=$acl.Owner;"
                     "directory=@($acl.Access|%{$_.IdentityReference.Value});"
                     "file=@($file.Access|%{$_.IdentityReference.Value})}|ConvertTo-Json"],
                    capture_output=True, text=True, check=True, timeout=15, env=c.tool_environment())
                acl = json.loads(result.stdout)
                self.assertTrue(acl["protected"])
                self.assertEqual(acl["directory"], [acl["owner"]])
                self.assertEqual(acl["file"], [acl["owner"]])
            else:
                self.assertEqual(root.stat().st_mode & 0o777, 0o700)
                self.assertEqual(root.stat().st_uid, os.getuid())

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

    def test_tool_monitor_aborts_collection_and_preserves_command(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            monitor = mock.Mock(side_effect=[None, ValueError("target duration budget exceeded")])
            result = c.run_tool([sys.executable, "-c", "import time; time.sleep(30)"], root,
                                dict(os.environ), root, "dump", monitor=monitor)
            self.assertEqual(result["status"], "blocked_collection")
            self.assertEqual(result["reason"], "target duration budget exceeded")
            self.assertIsNotNone(result["exit_status"])
            self.assertIn("profiler_pid", result)

    def test_windows_stop_race_only_ignores_an_already_exited_child(self):
        for exited in (True, False):
            with self.subTest(exited=exited), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                command = c.Command([sys.executable, "-c", "import sys; sys.exit(7)"],
                                    root, dict(os.environ), root, "fixture")
                command.process.wait(timeout=10)
                error = subprocess.CalledProcessError(128, ["taskkill"], stderr=b"Process not found")
                try:
                    with mock.patch.object(c.os, "name", "nt"), \
                            mock.patch.object(command.process, "poll", side_effect=[None, 7 if exited else None]), \
                            mock.patch.object(c.subprocess, "run", side_effect=error) as kill:
                        if exited:
                            command.stop()
                        else:
                            with self.assertRaises(subprocess.CalledProcessError) as raised:
                                command.stop()
                            self.assertIs(raised.exception, error)
                        self.assertEqual(kill.call_args.args[0],
                                         ["taskkill", "/PID", str(command.process.pid), "/T", "/F"])
                finally:
                    result = command.finish("budget_exceeded")
                self.assertEqual(result["status"], "blocked_collection")
                self.assertEqual(result["exit_status"], 7)
                self.assertEqual(result["reason"], "budget_exceeded")
                self.assertFalse(list(root.glob("*.raw")))
                self.assertTrue(all(stream.closed for stream in command.streams))

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

    def test_dump_success_and_aggregate_budget(self):
        identity = profile()["process"]["start"]
        document = {"identity": identity, "hypothesis": "Heap grows across full GCs", "profiling_replay": True}
        for limit in (100000, 9000, 10010):
            with self.subTest(limit=limit), tempfile.TemporaryDirectory() as directory:
                root = Path(directory) / "private"
                limited = limit != 100000

                def run(arguments, *args, **kwargs):
                    (root / "heap.dmp").write_bytes(b"MDMP" + bytes(5000))
                    (root / "other.log").write_bytes(bytes(5000))
                    return {"status": "collected", "exit_status": 0, "stderr": "", "stdout": "Total 10 objects"}

                with mock.patch.object(c, "snapshot_process", return_value=identity), \
                        mock.patch.object(c, "preflight", return_value={}), \
                        mock.patch.object(c, "verify_tools", return_value={"dotnet-dump": {"command": ["dotnet-dump"]}}), \
                        mock.patch.object(c, "run_tool", side_effect=run), \
                        mock.patch.dict(c.LIMITS, total_bytes=limit):
                    result = c.collect_dump(document, "heap-growth", root)
                self.assertEqual(result["status"], "blocked_collection" if limited else "collected")
                self.assertFalse(result["published"])
                self.assertEqual((root / "heap.dmp").exists(), not limited)
                if limited:
                    self.assertEqual(result["reason"], "budget_exceeded")
                    self.assertEqual(result["incomplete_artifact"]["sha256"],
                                     c.m.hashlib.sha256(b"MDMP" + bytes(5000)).hexdigest())
                else:
                    self.assertEqual(result["artifact"]["size_bytes"], 5004)
                    self.assertEqual(result["artifact"]["sha256"], c.m.file_record(root / "heap.dmp")["sha256"])
                    evidence = artifact_profile(root)
                    evidence["dump"] = c.m.read_json(root / "dump-evidence.json")
                    path = root / "evidence-manifest.json"
                    c.m.write_json(path, evidence)
                    analyzed = c.analyze_profile(path)
                    self.assertEqual(analyzed["status"], "collected", analyzed)

    def test_collection_lifecycle_and_failures(self):
        for problem in (None, "trace", "trace-incomplete", "counters", "reduce", "timings", "count", "duration",
                        "dump", "dump-failure", "dump-early-exit"):
            with self.subTest(problem=problem), tempfile.TemporaryDirectory() as directory, ExitStack() as patches:
                base = Path(directory)
                root = base / "profile"
                runtime = base / "fsc.runtimeconfig.json"
                c.m.write_json(runtime, {"runtimeOptions": {"framework": {"version": "8.0.1"}}})
                compiler = {"files": {"fsc.dll": {"path": str(base / "fsc.dll")},
                                     "fsc.runtimeconfig.json": c.m.file_record(runtime)}}
                assembly = base / "output.dll"
                source = base / "public.fs"
                source.write_text("module Public")
                dotnet = c.m.file_record(sys.executable)
                workload = {"sources": [c.m.file_record(source)], "generated": [], "outputs": [str(assembly)],
                            "checkout": str(base), "cwd": str(base), "assembly_output": str(assembly),
                            "response_file": {"path": str(base / "replay.rsp")}}
                manifest = {"owned_root": str(base), "invariants": {
                    "workload": workload, "toolchain": {"dotnet": dotnet}}}
                args = [dotnet["path"], compiler["files"]["fsc.dll"]["path"], "@" + workload["response_file"]["path"]]
                identity = dict(profile()["process"]["start"], executable=dotnet["path"],
                                binary_sha256=dotnet["sha256"], command=args, parent_pid=os.getpid(),
                                session_id=42 if sys.platform == "linux" else 7)
                calls, commands = [], []
                ticks = itertools.count(0, .1)

                class FakeCommand:
                    def __init__(self, arguments, cwd, environment, owned, phase):
                        self.phase, self.arguments = phase, arguments
                        self.start = c.time.monotonic()
                        self.process = SimpleNamespace(pid=42 if phase == "target" else 100 + len(commands),
                                                       returncode=None, poll=self.poll)
                        self.polls = 0
                        commands.append(self)
                        if phase == "trace":
                            (root / "compiler.nettrace").write_bytes(b"synthetic trace, not real EventPipe")
                        if phase == "counters":
                            (root / "counters.csv").write_text((FIXTURES / "counters.csv").read_text())

                    def poll(self):
                        self.polls += 1
                        if self.phase != "target" and commands[0].process.returncode is None:
                            return None
                        if self.phase == "target" and self.polls < (2 if problem == "dump-early-exit" else 40):
                            return None
                        if self.phase == "target":
                            assembly.write_bytes(b"synthetic assembly")
                        self.process.returncode = 7 if problem == self.phase else 0
                        return self.process.returncode

                    def check(self):
                        c.check_budget(root)

                    def output(self):
                        return f"Output File    : {(root / 'compiler.nettrace').resolve()}"

                    def finish(self, issue=None):
                        calls.append(self.phase)
                        return {"status": "blocked_collection" if problem == self.phase or issue else "collected",
                                "phase": self.phase, "exit_status": 7 if problem == self.phase else 0,
                                "stdout": "" if problem == "trace-incomplete" else "Trace completed.",
                                "stderr": "", "arguments": self.arguments,
                                "target_pid" if self.phase == "target" else "profiler_pid": self.process.pid}

                def timings(*unused):
                    calls.append("timings")
                    result = artifact_profile(root)["timings"]
                    if problem == "timings":
                        result = {"status": "collected"}
                    return result

                def run(arguments, cwd, environment, owned, phase, **kwargs):
                    if phase in ("dump", "dump-validate"):
                        calls.append(phase)
                        if problem == "dump-failure":
                            return c.classify_failure(phase, 17, "Dump failed", "tool_failure")
                        (Path(owned) / "heap.dmp").write_bytes(b"MDMP" + bytes(5000))
                        return {"status": "collected", "exit_status": 0, "stdout": "Total 10 objects", "stderr": ""}
                    c.m.write_json(root / "trace-reduction.json", profile()["trace"])
                    return {"status": "blocked_collection" if problem == "reduce" else "collected",
                            "phase": "reduce", "exit_status": 7 if problem == "reduce" else 0, "stderr": ""}

                for name, value in (
                    ("Command", FakeCommand), ("frozen_inputs", lambda *a: []),
                    ("preflight", lambda *a, **kw: {}),
                    ("verify_tools", lambda *a, **kw: {name: {"command": [name]} for name in
                                                 ("dotnet-trace", "dotnet-counters", "dotnet-dump")}),
                    ("snapshot_process", lambda pid: identity if pid == 42 else
                     dict(identity, pid=os.getpid(), session_id=7)),
                    ("windows_accounting", lambda p: {"process_cpu_ms": 2700, "peak_rss_bytes": 90000000}),
                    ("diagnostic_timings", timings), ("run_tool", run),
                ):
                    patches.enter_context(mock.patch.object(c, name, value))
                patches.enter_context(mock.patch.object(c.time, "monotonic", side_effect=lambda: next(ticks)))
                patches.enter_context(mock.patch.object(c.time, "sleep"))
                if sys.platform == "linux":
                    patches.enter_context(mock.patch.object(c.os, "wait4", side_effect=lambda pid, flags:
                        (0, 0, None) if commands[0].poll() is None else
                        (pid, 0, SimpleNamespace(ru_utime=2, ru_stime=.7, ru_maxrss=90000))))
                if problem == "count":
                    patches.enter_context(mock.patch.dict(c.LIMITS, collections=0))
                if problem == "duration":
                    patches.enter_context(mock.patch.dict(c.LIMITS, duration_seconds=1))
                requested_dump = problem is not None and problem.startswith("dump")
                result = c.collect_profile(manifest, compiler, root,
                                           dump_reason="heap-growth" if requested_dump else None,
                                           hypothesis="Heap grows across full GCs" if requested_dump else None)
                success = problem in (None, "dump")
                self.assertEqual(result["status"], "collected" if success else "blocked_collection", result)
                self.assertFalse(result["acceptance_eligible"])
                self.assertFalse((base / "measurement.lock").exists())
                saved = c.m.read_json(root / "evidence-manifest.json")
                self.assertEqual(saved["status"], result["status"])
                self.assertEqual(c.m.read_json(root / "profile-summary.json"), result)
                if problem == "count":
                    self.assertFalse(commands)
                if requested_dump:
                    self.assertIn("dump", saved)
                    self.assertEqual(saved["dump"]["status"], "collected" if success else "blocked_collection")
                    self.assertFalse(saved["dump"]["published"])
                    if problem == "dump-early-exit":
                        self.assertEqual(saved["dump"]["reason"], "early_exit")
                        self.assertNotIn("dump", calls)
                    else:
                        self.assertIn("dump", calls)
                        self.assertEqual(saved["dump"]["command"]["exit_status"], 0 if success else 17)
                if success:
                    self.assertEqual(c.analyze_profile(root / "evidence-manifest.json")["metrics"], result["metrics"])
                    self.assertEqual(commands[0].arguments, args)
                    self.assertEqual(calls, (["dump", "dump-validate"] if requested_dump else [])
                                     + ["target", "trace", "counters", "timings"])
                    self.assertEqual(saved["process"]["start"], saved["process"]["end"])
                    self.assertIn("--resume-runtime:false", commands[1].arguments)
                    self.assertIn("--resume-runtime:true", commands[2].arguments)
                    self.assertNotEqual(commands[0].process.pid, commands[1].process.pid)

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

    def test_frozen_input_boundary_rejects_changes_before_launch(self):
        cases = (None, "schema", "source", "response", "inventory", "revision", "untracked", "bootstrap", "runtime")
        for problem in cases:
            with self.subTest(problem=problem), tempfile.TemporaryDirectory() as directory:
                base = Path(directory).resolve()
                sample = measurements()[0]
                invariants, compiler = sample["invariants"], sample["compiler"]
                workload, toolchain = invariants["workload"], invariants["toolchain"]
                for key in ("checkout", "cwd"):
                    workload[key] = str(base / Path(workload[key]).relative_to(ROOT))
                records = [r for key in ("sources", "generated", "references", "resources", "auxiliary")
                           for r in workload[key]]
                records += [workload[k] for k in ("response_file", "core_package")]
                records += invariants["bootstrap"]["binaries"] + list(compiler["files"].values())
                records += [toolchain[k] for k in ("dotnet", "measurement", "capture_target")]
                records += toolchain["runtime_files"] + toolchain["sdk_compiler_files"]
                for record in records:
                    path = base / Path(record["path"]).relative_to(ROOT)
                    path.parent.mkdir(parents=True, exist_ok=True)
                    path.write_text("synthetic public input", encoding="utf-8")
                    record.update(c.m.file_record(path))
                runtime = Path(compiler["files"]["fsc.runtimeconfig.json"]["path"])
                c.m.write_json(runtime, {"runtimeOptions": {"framework": {"version": "8.0.1"}}})
                compiler["files"][runtime.name] = c.m.file_record(runtime)
                toolchain["compiler_runtimeconfig_sha256"] = compiler["files"][runtime.name]["sha256"]
                workload["arguments"][4] = "-r:" + workload["references"][0]["path"]
                response = Path(workload["response_file"]["path"])
                response.write_text("\n".join(workload["arguments"]), encoding="utf-8")
                workload.update(response_file=c.m.file_record(response),
                                arguments_sha256=c.m.fingerprint_inputs(workload["arguments"]))
                tracked = [Path(r["path"]).relative_to(workload["checkout"]).as_posix() for r in workload["sources"]]
                workload.update(c.m.capture_inputs(workload["arguments"], workload["cwd"], workload["checkout"], set(tracked)))

                def git(*args):
                    return subprocess.run(["git", *args], cwd=workload["checkout"], env=c.tool_environment(),
                                          capture_output=True, text=True, check=True, timeout=30).stdout.strip()

                git("init", "-q")
                git("add", "--", *tracked)
                git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid",
                    "-c", "commit.gpgsign=false", "-c", "core.hooksPath=" + str(base / "no-hooks"),
                    "commit", "-qm", "Synthetic public sources")
                workload["revision"] = git("rev-parse", "HEAD")
                for section in ("bootstrap", "toolchain"):
                    compiler[f"{section}_fingerprint"] = c.m.fingerprint_inputs(invariants[section])
                manifest = {"schema_version": 1, "kind": "workload", "owned_root": str(base),
                            "baseline_compiler": copy.deepcopy(compiler), "invariants": invariants}
                if problem == "schema":
                    manifest["schema_version"] = 2
                elif problem == "source":
                    Path(workload["sources"][0]["path"]).write_text("changed source")
                elif problem == "response":
                    response.write_text("\n".join(reversed(workload["arguments"])))
                    workload["response_file"] = c.m.file_record(response)
                elif problem == "inventory":
                    workload["sources"].reverse()
                elif problem == "revision":
                    workload["revision"] = "a" * 40
                elif problem == "untracked":
                    git("update-index", "--force-remove", "--", tracked[0])
                elif problem == "bootstrap":
                    compiler["bootstrap_fingerprint"] = "b" * 64
                elif problem == "runtime":
                    compiler["files"][runtime.name]["sha256"] = "b" * 64
                if problem is None:
                    self.assertTrue(c.frozen_inputs(manifest, compiler))
                else:
                    with mock.patch.object(c, "Command") as command:
                        result = c.collect_profile(manifest, compiler, base / "profile")
                    self.assertEqual(result["status"], "blocked_collection", result)
                    self.assertEqual(result["phase"], "validate", result)
                    command.assert_not_called()
                    self.assertFalse((base / "measurement.lock").exists())

    def test_diagnostic_timing_boundary_checks_build_and_extractor_output(self):
        cases = (None, "build", "extract", "no-fsc", "no-times")
        for problem in cases:
            with self.subTest(problem=problem), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                sample = measurements()[0]
                manifest = {"invariants": sample["invariants"]}
                calls = []

                def run(arguments, cwd, environment, owned, phase, **kwargs):
                    calls.append((phase, arguments, environment))
                    if phase == "diagnostic-build":
                        (root / "diagnostic.binlog").write_bytes(b"synthetic binlog")
                        if problem == "build":
                            return c.classify_failure(phase, 7, "Build failed")
                        return {"status": "collected", "exit_status": 0, "stderr": ""}
                    if problem == "extract":
                        return c.classify_failure(phase, 9, "Extractor failed")
                    text = {"no-fsc": "No Fsc tasks found in binlog.",
                            "no-times": "Fsc tasks found but no timing data present. Was --times flag set?"}.get(problem, TIMINGS)
                    output = root / "timings.log"
                    output.write_text(text)
                    return {"status": "collected", "exit_status": 0, "stdout": text, "stderr": "",
                            "logs": [c.m.file_record(output)]}

                with mock.patch.object(c, "run_tool", side_effect=run):
                    result = c.diagnostic_timings(manifest, sample["compiler"], root, sys.executable)
                self.assertEqual(result["status"], "blocked_collection" if problem else "collected", result)
                self.assertFalse(result["acceptance_eligible"])
                self.assertEqual([phase for phase, _, _ in calls],
                                 ["diagnostic-build"] + ([] if problem == "build" else ["timings-extract"]))
                arguments, environment = calls[0][1:]
                self.assertIn("-p:OtherFlags=--times", arguments)
                self.assertEqual(arguments[arguments.index("-c") + 1], "Release")
                self.assertEqual(environment["DOTNET_EnableDiagnostics"], "0")
                self.assertIn("-p:DotnetFscCompilerPath=%22" + sample["compiler"]["files"]["fsc.dll"]["path"] + "%22",
                              arguments)
                if problem != "build":
                    self.assertEqual(calls[1][1], [sys.executable, "fsi",
                                     str(ROOT / "eng" / "scripts" / "ExtractTimingsFromBinlog.fsx"),
                                     str(root / "diagnostic.binlog")])
                if problem is None:
                    self.assertEqual(result["projects"], ["FSharp.Compiler.Service"])
                    self.assertEqual(result["binlog"], c.m.file_record(root / "diagnostic.binlog"))
                    self.assertEqual(result["output_sha256"], c.m.file_record(root / "timings.log")["sha256"])
                elif problem in ("no-fsc", "no-times"):
                    self.assertEqual(result["exit_status"], 0)
                    self.assertIn("Missing project-tagged Fsc timing data", result["reason"])

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

    def test_analyze_cli_invalid_input_is_persistent_blocker(self):
        for problem in ("corrupt-json", "numeric-overflow"):
            with self.subTest(problem=problem), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                input_path, output = root / "input.json", root / "summary.json"
                if problem == "corrupt-json":
                    input_path.write_text('{"schema_version":')
                else:
                    evidence = artifact_profile(root)
                    evidence["process"]["process_cpu_ms"] = 10**400
                    c.m.write_json(input_path, evidence)
                result = subprocess.run([sys.executable, str(HERE / "collection.py"), "analyze",
                                         "--input", str(input_path), "--output", str(output)],
                                        capture_output=True, text=True)
                self.assertEqual(result.returncode, 2, result.stderr)
                self.assertTrue(output.exists(), result.stderr)
                self.assertEqual(json.loads(output.read_text())["status"], "blocked_collection")


if __name__ == "__main__":
    unittest.main()
