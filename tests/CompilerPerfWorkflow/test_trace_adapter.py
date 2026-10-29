"""TraceEvent adapter contract; the live test traces only a tiny public child."""

import hashlib
import importlib.util
import json
import math
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import time
import unittest
import uuid

ROOT = Path(__file__).resolve().parents[2]
ADAPTER = ROOT / "eng" / "scripts" / "compiler_perf" / "AnalyzeTrace.fsx"
FIXTURE = Path(__file__).parent / "fixtures" / "TraceChild.cs"


class TraceAdapterTests(unittest.TestCase):
    def setUp(self):
        self.work = ROOT / "artifacts" / ("trace-adapter-" + uuid.uuid4().hex)
        self.work.mkdir(parents=True)
        self.addCleanup(shutil.rmtree, self.work)

    def reduce(self, trace, pid, name="reduction.json"):
        output = self.work / name
        result = subprocess.run(
            ["dotnet", "fsi", "--exec", str(ADAPTER), str(trace), str(output), str(pid)],
            cwd=ROOT, capture_output=True, text=True, timeout=180,
        )
        self.assertTrue(output.is_file(), result.stdout + result.stderr)
        self.assertLessEqual(output.stat().st_size, 16 * 1024 * 1024)
        self.assertFalse(list(self.work.glob("*.etlx")))
        self.assertFalse(list(self.work.glob("*.etlx.*")))
        return result, json.loads(output.read_text(encoding="utf-8-sig"))

    def assert_blocked(self, trace, pid, message):
        result, data = self.reduce(trace, pid)
        self.assertNotEqual(result.returncode, 0)
        self.assertEqual(data["schema_version"], 1)
        self.assertEqual(data["kind"], "trace-reduction")
        self.assertIs(data["complete"], False)
        self.assertRegex(data["error"], message)

    def build_child(self, target_framework=None):
        sdk = subprocess.run(["dotnet", "--version"], cwd=ROOT, capture_output=True,
                             text=True, check=True, timeout=30).stdout.strip()
        target_framework = target_framework or f"net{sdk.split('.')[0]}.0"
        project = self.work / "Child.csproj"
        project.write_text(
            '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>'
            f'<TargetFramework>{target_framework}</TargetFramework>'
            '<OutputType>Exe</OutputType><EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
            '</PropertyGroup><ItemGroup>'
            f'<Compile Include="{FIXTURE}" /></ItemGroup></Project>', encoding="utf-8",
        )
        build = subprocess.run(
            ["dotnet", "build", str(project), "-c", "Release",
             "-p:ImportDirectoryBuildProps=false", "-p:ImportDirectoryBuildTargets=false",
             "-p:NuGetAudit=false", "--nologo", "-v:q"],
            cwd=ROOT, capture_output=True, text=True, timeout=180,
        )
        self.assertEqual(build.returncode, 0, build.stdout + build.stderr)
        return self.work / "bin" / "Release" / target_framework / "Child.dll"

    def test_missing_malformed_and_oversize_traces_fail_closed(self):
        trace = self.work / "input.nettrace"
        self.assert_blocked(trace, 123, "(?i)exist|found")
        for payload in (b"", b"not an EventPipe trace", b"Nettrace" + bytes(100)):
            trace.write_bytes(payload)
            self.assert_blocked(trace, 123, "(?i)trace|empty|parse")
        with trace.open("wb") as stream:
            stream.truncate(512 * 1024 * 1024 + 1)
        self.assert_blocked(trace, 123, "(?i)size|large|512")

    def test_expected_pid_must_be_positive(self):
        trace = self.work / "input.nettrace"
        trace.write_bytes(b"not an EventPipe trace")
        for pid in ("0", "-1", "not-a-pid"):
            with self.subTest(pid=pid):
                self.assert_blocked(trace, pid, "(?i)pid|process")

    def test_input_output_collision_preserves_trace(self):
        trace = self.work / "input.nettrace"
        payload = b"Nettrace" + bytes(range(32))
        digest = hashlib.sha256(payload).hexdigest()
        cases = (("identical", str(trace), "123"),
                 ("relative", os.path.relpath(trace, ROOT), "123"),
                 ("invalid-pid", str(trace), "not-a-pid"))
        for name, output, pid in cases:
            with self.subTest(case=name):
                trace.write_bytes(payload)
                result = subprocess.run(
                    ["dotnet", "fsi", "--exec", str(ADAPTER), str(trace), output, pid],
                    cwd=ROOT, capture_output=True, text=True, timeout=180,
                )
                self.assertNotEqual(result.returncode, 0, result.stdout + result.stderr)
                self.assertEqual(trace.read_bytes(), payload)
                self.assertEqual(hashlib.sha256(trace.read_bytes()).hexdigest(), digest)
                self.assertIn("Trace input and output must differ", result.stderr)
                self.assertEqual(list(self.work.iterdir()), [trace])

    @unittest.skipUnless(os.environ.get("COMPILER_PERF_LIVE") == "1",
                         "set COMPILER_PERF_LIVE=1 for real EventPipe collection")
    def test_live_public_child_trace_and_negative_controls(self):
        version = subprocess.run(
            ["dotnet", "tool", "run", "dotnet-trace", "--version"],
            cwd=ROOT, capture_output=True, text=True, check=True, timeout=30,
        )
        self.assertTrue(version.stdout.startswith("8.0.547301"), version.stdout)
        child = self.build_child()

        def collect(name, providers, rundown=True):
            trace = self.work / f"{name}.nettrace"
            pid_file = self.work / f"{name}.pid"
            environment = dict(os.environ, DOTNET_EnableDiagnostics="1",
                               DOTNET_EnableDiagnostics_IPC="1", DOTNET_EnableDiagnostics_Profiler="1")
            result = subprocess.run(
                ["dotnet", "tool", "run", "dotnet-trace", "--", "collect",
                 "--providers", providers, "--buffersize", "64", "--output", str(trace),
                 "--rundown", str(rundown).lower(),
                 "--", "dotnet", str(child), str(pid_file)],
                cwd=ROOT, env=environment, capture_output=True, text=True, timeout=90,
            )
            self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
            return trace, int(pid_file.read_text(encoding="utf-8"))

        trace, pid = collect(
            "full", "Microsoft-Windows-DotNETRuntime:0x1:5,Microsoft-DotNETCore-SampleProfiler:0x0:5")
        result, data = self.reduce(trace, pid)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr + str(data))
        self.assertEqual(data["schema_version"], 1)
        self.assertEqual(data["kind"], "trace-reduction")
        self.assertIs(data["complete"], True)
        self.assertEqual(data["process_id"], pid)
        self.assertRegex(data["runtime"], r"^\d+\.\d+\.\d+")
        self.assertEqual(data["lost_events"], 0)
        self.assertIn("Microsoft-Windows-DotNETRuntime", data["providers"])
        self.assertIn("Microsoft-DotNETCore-SampleProfiler", data["providers"])
        self.assertEqual(data["cpu_sample_unit"], "samples")
        self.assertGreater(data["cpu_sample_weight"], 0)
        self.assertGreater(data["last_event_ms"], data["first_event_ms"])
        self.assertLessEqual(len(data["stacks"]), 128)
        self.assertGreater(len(data["stacks"]), 0)
        for stack in data["stacks"]:
            self.assertGreater(stack["weight"], 0)
            self.assertTrue(stack["frames"])
            self.assertTrue(all(isinstance(frame, str) and frame for frame in stack["frames"]))
        self.assertLessEqual(sum(stack["weight"] for stack in data["stacks"]), data["cpu_sample_weight"])
        self.assertIn("TraceChild", json.dumps(data["stacks"]))
        for key in ("allocation_ticks", "gc_intervals", "gc_pauses", "heap_observations"):
            self.assertTrue(data[key], key)
            for item in data[key]:
                self.assertTrue(all(not isinstance(value, (int, float)) or
                                    (math.isfinite(value) and value >= 0) for value in item.values()), item)
                if "end_ms" in item:
                    self.assertGreaterEqual(item["end_ms"], item["start_ms"])
            if key in ("gc_intervals", "gc_pauses"):
                starts = [item["start_ms"] for item in data[key]]
                self.assertEqual(starts, sorted(starts))
        self.assertTrue(all(item["bytes"] > 0 for item in data["allocation_ticks"]))
        aggregation = data["allocation_aggregation"]
        self.assertEqual(aggregation["kind"], "fixed-time-buckets")
        self.assertEqual(aggregation["bucket_width_ms"], 100)
        self.assertEqual(aggregation["timestamp"], "bucket-start")
        self.assertGreater(aggregation["source_event_count"], len(data["allocation_ticks"]))
        self.assertTrue(all(item["timestamp_ms"] % 100 == 0 for item in data["allocation_ticks"]))
        self.assertLessEqual(len(data["allocation_ticks"]), math.ceil(data["last_event_ms"] / 100) + 1)
        self.assertTrue(all(item["kind"] == "gc-heap-stats" for item in data["heap_observations"]))
        self.assert_blocked(trace, pid + 1, "(?i)pid|process")
        truncated = self.work / "truncated.nettrace"
        raw = trace.read_bytes()
        truncated.write_bytes(raw[:len(raw) // 2])
        self.assert_blocked(truncated, pid, "(?i)trace|truncat|parse|incomplete|missing")
        truncated.write_bytes(raw[:-8])
        self.assert_blocked(truncated, pid, "(?i)trace|truncat|parse|incomplete|missing")
        no_samples, child_pid = collect("no-samples", "Microsoft-Windows-DotNETRuntime:0x1:5")
        self.assert_blocked(no_samples, child_pid, "(?i)sample|stack|provider")
        no_rundown, child_pid = collect(
            "no-rundown", "Microsoft-Windows-DotNETRuntime:0x1:5,Microsoft-DotNETCore-SampleProfiler:0x0:5",
            rundown=False)
        self.assert_blocked(no_rundown, child_pid, "(?i)runtime|metadata|stack|symbol|missing")

    @unittest.skipUnless(os.environ.get("COMPILER_PERF_LIVE") == "1",
                         "set COMPILER_PERF_LIVE=1 for private public-child dump collection")
    def test_live_public_child_dump(self):
        spec = importlib.util.spec_from_file_location("collection", ADAPTER.with_name("collection.py"))
        collection = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(collection)
        child = self.build_child("net8.0")
        ipc = tempfile.TemporaryDirectory(prefix="compiler-perf-ipc-")
        self.addCleanup(ipc.cleanup)
        environment = collection.target_environment(Path(ipc.name))
        environment["DOTNET_DefaultDiagnosticPortSuspend"] = "0"
        dotnet = Path(shutil.which("dotnet"))
        pid_file = self.work / "dump-child.pid"
        process = subprocess.Popen(
            [str(dotnet), str(child), str(pid_file), "--wait"], cwd=ROOT, env=environment,
            stdin=subprocess.PIPE, stdout=subprocess.DEVNULL, stderr=subprocess.PIPE, text=True,
        )
        try:
            deadline = time.monotonic() + 20
            while not pid_file.exists() and process.poll() is None and time.monotonic() < deadline:
                time.sleep(0.1)
            startup_error = process.stderr.read() if process.poll() is not None else ""
            self.assertTrue(pid_file.exists(), "Public child did not start: " + startup_error)
            self.assertEqual(int(pid_file.read_text()), process.pid)
            time.sleep(3)
            self.assertIsNone(process.poll(), "Dump fixture must remain alive until released")
            identity = collection.snapshot_process(process.pid)
            self.assertEqual(identity["pid"], process.pid)
            self.assertIn(str(child), identity["command"])
            document = {
                "identity": identity, "profiling_replay": True, "environment": environment,
                "hypothesis": "Public fixture allocation churn may leave objects in a heap snapshot",
            }
            result = collection.collect_dump(document, "retention", self.work / "private-dump")
            self.assertEqual(result["status"], "collected", json.dumps(result, indent=2))
            self.assertEqual(result["identity"]["pid"], process.pid)
            self.assertFalse(result["published"])
            self.assertTrue(result["profiler_attached"])
            self.assertFalse(result["acceptance_eligible"])
            self.assertIsNone(result["retained_heap_bytes"])
            self.assertEqual(result["validation"]["exit_status"], 0)
            artifact = Path(result["artifact"]["path"])
            self.assertGreater(artifact.stat().st_size, 4096)
            self.assertEqual(result["artifact"]["sha256"], collection.m.file_record(artifact)["sha256"])
            self.assertIsNone(process.poll(), "Dump collection must not terminate the target")
        finally:
            try:
                process.communicate(input="\n", timeout=10)
            except subprocess.TimeoutExpired:
                process.kill()
                process.communicate(timeout=10)


if __name__ == "__main__":
    unittest.main()
