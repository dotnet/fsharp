"""Bounded, private diagnostics replays. Never produces acceptance samples."""

import argparse
import csv
import ctypes
from datetime import datetime
import importlib.util
import json
import math
import os
from pathlib import Path
import platform
import re
import shutil
import signal
import subprocess
import sys
import time
import uuid

spec = importlib.util.spec_from_file_location("measurement", Path(__file__).with_name("measurement.py"))
m = importlib.util.module_from_spec(spec)
spec.loader.exec_module(m)
if os.name == "nt":
    WindowsJob = m.WindowsJob
HERE, REPO = m.HERE, m.REPO
LIMITS = {"duration_seconds": 180, "artifact_bytes": 1024**3, "trace_bytes": 512 * 1024**2,
          "total_bytes": 2 * 1024**3, "log_bytes": 1024**2, "json_bytes": 16 * 1024**2,
          "minimum_memory_bytes": 512 * 1024**2, "collections": 1}
TOOL_VERSION = "8.0.547301"
PROVIDERS = "Microsoft-DotNETCore-SampleProfiler:0x0:4,Microsoft-Windows-DotNETRuntime:0x1:5"
IDENTITY = ("pid", "start_time", "parent_pid", "session_id", "executable", "command",
            "uid", "namespace", "binary_sha256")
REASONS = ("retention", "heap-growth", "unexplained-rss")


class CollectionCancelled(KeyboardInterrupt):
    def __init__(self, result):
        super().__init__("Collection cancelled")
        self.result = result


def sanitize(text):
    text = str(text)
    for key, value in os.environ.items():
        if re.search(r"TOKEN|SECRET|PASSWORD|CREDENTIAL|API_KEY", key, re.I) and len(value) >= 4:
            text = text.replace(value, "[REDACTED]")
    text = re.sub(r"(?i)(bearer\s+)\S+", r"\1[REDACTED]", text)
    text = re.sub(r"(https?://)[^/\s@]+@", r"\1[REDACTED]@", text)
    text = re.sub(
        r"""(?ix)((?:token|password|secret|credentials?|api[_-]?key|authorization)["']?\s*[=:]\s*)"""
        r"""("(?:\\(?:[\s\S]|$)|[^"\\])*(?:"|$)|'(?:\\(?:[\s\S]|$)|[^'\\])*(?:'|$)|[^\s,;}]+)""",
        lambda match: match[1] + ('"[REDACTED]"' if match[2].startswith('"') else
                                  "'[REDACTED]'" if match[2].startswith("'") else "[REDACTED]"), text)
    return re.sub(r"\b(?:gh[pousr]_[A-Za-z0-9_]+|github_pat_[A-Za-z0-9_]+)\b", "[REDACTED]", text)


def classify_failure(phase, exit_status=None, stderr="", issue=None):
    return {"schema_version": 1, "kind": "collection-failure",
            "status": "blocked_collection", "phase": phase, "exit_status": exit_status,
            "reason": sanitize(issue or "tool_failure"), "stderr": sanitize(stderr),
            "profiler_attached": True, "acceptance_eligible": False}


def number(value, label):
    m.require(type(value) in (int, float) and 0 <= value <= sys.float_info.max and math.isfinite(value),
              f"Invalid {label}")
    return value


def select_compiler_process(candidates, expected):
    m.require(all(key in expected and expected[key] is not None for key in IDENTITY),
              "Incomplete process identity")
    m.require(type(expected["pid"]) is int and expected["pid"] > 0
              and isinstance(expected["command"], list) and len(expected["command"]) >= 2,
              "Invalid process identity")
    m.require(type(expected["parent_pid"]) is int and expected["parent_pid"] > 0
              and type(expected["session_id"]) is int and expected["session_id"] >= 0
              and isinstance(expected["start_time"], str) and expected["start_time"].isdigit()
              and all(isinstance(expected[k], str) and expected[k] for k in ("uid", "namespace"))
              and all(isinstance(a, str) and a and not any(x in a for x in "\0\r\n")
                      for a in expected["command"]), "Invalid process metadata")
    m.validate_file_record({"path": expected["executable"], "sha256": expected["binary_sha256"]})
    matches = [p for p in candidates if all(p.get(k) == expected[k] for k in IDENTITY)]
    m.require(len(matches) == 1, "ambiguous_process" if len(matches) > 1 else "early_exit_or_identity_mismatch")
    return matches[0]


def read_bounded_json(path):
    m.require(Path(path).stat().st_size <= LIMITS["json_bytes"], "JSON size budget exceeded")
    return m.read_json(path)


def normalize_evidence(evidence):
    try:
        m.require(evidence.get("status") != "blocked_collection", "Collection previously blocked")
        m.require(type(evidence["schema_version"]) is int and evidence["schema_version"] == 1
                  and evidence["kind"] == "compiler-profile", "Unsupported profile schema")
        m.require(evidence["profiler_attached"] is True and evidence["acceptance_eligible"] is False,
                  "Profiles are not acceptance samples")
        proc, trace, counters = (evidence[k] for k in ("process", "trace", "counters"))
        select_compiler_process([proc["end"]], proc["start"])
        pid = proc["start"]["pid"]
        m.require(type(proc["exit_status"]) is int and proc["exit_status"] == 0, "Compiler process failed")
        m.require(evidence["commands"] and all(type(c["exit_status"]) is int and c["exit_status"] == 0
                  and c.get("status", "collected") == "collected" for c in evidence["commands"]), "Tool failed")
        m.require({"trace", "counters", "reduce"} <= {c["phase"] for c in evidence["commands"]}, "Missing tool results")
        m.require(evidence["source_hashes"], "Missing source hashes")
        for record in evidence["source_hashes"]:
            m.validate_file_record(record)
        m.require(type(trace["schema_version"]) is int and trace["schema_version"] == 1
                  and trace["kind"] == "trace-reduction", "Unsupported reduction schema")
        m.require(trace["complete"] is True and type(trace["lost_events"]) is int and trace["lost_events"] == 0,
                  "Incomplete trace or dropped events")
        m.require(trace["process_id"] == pid and counters["process_id"] == pid, "Wrong process")
        m.require(int(trace["runtime"].split(".")[0]) == evidence["expected_runtime_major"], "Wrong runtime")
        m.require({"Microsoft-DotNETCore-SampleProfiler", "Microsoft-Windows-DotNETRuntime"}
                  <= set(trace["providers"]), "Missing CPU/GC providers")
        coverage = evidence["coverage"]
        m.require(coverage["trace_before_resume"] is True and coverage["counters_before_resume"] is True,
                  "Collection did not cover runtime startup")
        duration = number(coverage["duration_ms"], "coverage duration")
        first, last = (number(trace[k], k) for k in ("first_event_ms", "last_event_ms"))
        m.require(duration > 0 and last > first and last <= duration + 2000
                  and last - first >= max(1, duration - 2000),
                  "Insufficient trace coverage")
        weight = number(trace["cpu_sample_weight"], "CPU samples")
        m.require(weight > 0 and trace["cpu_sample_unit"] == "samples" and trace["stacks"], "Missing CPU stacks")
        for stack in trace["stacks"]:
            m.require(stack["frames"] and all(isinstance(f, str) and f for f in stack["frames"]),
                      "Empty stack")
            number(stack["weight"], "stack weight")
        m.require(0 < sum(s["weight"] for s in trace["stacks"]) <= weight, "Inconsistent stack weights")

        def ordered(items, key, lower=first):
            m.require(items, f"Missing {key} evidence")
            times = [number(i[key], key) for i in items]
            m.require(times == sorted(times) and lower <= times[0] and times[-1] <= last + 1,
                      "Out-of-order/out-of-coverage timestamps")

        aggregation = trace.get("allocation_aggregation", {"kind": "individual-events"})
        allocation_start = first
        if aggregation["kind"] == "fixed-time-buckets":
            m.require(aggregation["bucket_width_ms"] == 100 and aggregation["timestamp"] == "bucket-start"
                      and type(aggregation["source_event_count"]) is int
                      and aggregation["source_event_count"] >= len(trace["allocation_ticks"]),
                      "Invalid allocation aggregation")
            allocation_start = math.floor(first / 100) * 100
        else:
            m.require(aggregation["kind"] == "individual-events", "Unsupported allocation aggregation")
        ordered(trace["allocation_ticks"], "timestamp_ms", allocation_start)
        allocated = number(sum(number(t["bytes"], "allocation bytes") for t in trace["allocation_ticks"]),
                           "allocation byte sum")
        m.require(allocated > 0, "Missing allocation evidence")
        counts = dict.fromkeys(("0", "1", "2"), 0)
        ordered(trace["gc_intervals"], "start_ms")
        for gc in trace["gc_intervals"]:
            m.require(type(gc["generation"]) is int and str(gc["generation"]) in counts, "Invalid GC generation")
            m.require(number(gc["end_ms"], "GC end") >= gc["start_ms"] and gc["end_ms"] <= last + 1,
                      "Invalid GC interval")
            counts[str(gc["generation"])] += 1
        ordered(trace["gc_pauses"], "start_ms")
        pause, end = 0, 0
        for gc in trace["gc_pauses"]:
            stop = number(gc["end_ms"], "pause end")
            m.require(gc["start_ms"] <= stop <= last + 1, "Invalid pause interval")
            pause += max(0, stop - max(end, gc["start_ms"]))
            end = max(end, stop)
        ordered(trace["heap_observations"], "timestamp_ms")
        for heap_observation in trace["heap_observations"]:
            number(heap_observation["bytes"], "trace heap bytes")
            m.require(heap_observation["kind"] == "gc-heap-stats", "Invalid trace heap observation")
        m.require(counters["provider"] == "System.Runtime" and counters["interval_seconds"] == 1,
                  "Wrong counter provider/interval")
        observations = counters["observations"]
        m.require(observations, "Missing counters")
        # Ordering is per counter because exporters may interleave streams.
        increments, heap = [], []
        for name in {o["name"] for o in observations}:
            stream = [o for o in observations if o["name"] == name]
            times = [number(o["timestamp_ms"], "counter time") for o in stream]
            # CSV time starts at its first observation; second-resolution exporters round at the edges.
            m.require(times == sorted(times) and times[-1] <= last - first + 1000,
                      "Out-of-order/out-of-coverage counters")
        for o in observations:
            value = number(o["value"], "counter value")
            if o["name"] == "alloc-rate":
                m.require(o["unit"] == "B / 1 sec" and o["semantics"] == "increment", "Invalid allocation increment semantics")
                increments.append(o)
            elif o["name"] == "gc-heap-size":
                m.require(o["unit"] == "MB" and o["semantics"] == "gauge", "Invalid heap units")
                heap.append(dict(o, bytes=number(value * 1_000_000, "heap bytes")))
        m.require(len(increments) >= 2 and heap, "Missing supported System.Runtime counters")
        # One unsampled interval at each edge; counter-tool startup is not target runtime.
        counter_edge_ms = 2 * counters["interval_seconds"] * 1000
        m.require(increments[0]["timestamp_ms"] <= counter_edge_ms
                  and increments[-1]["timestamp_ms"] - increments[0]["timestamp_ms"] >= max(1, last - first - counter_edge_ms),
                  "Insufficient counter coverage")
        # Pinned v8 CSV exports Increment despite the Rate label; display timestamps do not scale it.
        observed = number(sum(o["value"] for o in increments), "allocation counter observed byte sum")
        unavailable = {"retained_heap_bytes": "No forced full-GC/live-object retention observation; heap and RSS are not retention"}
        metrics = {"elapsed_ms": number(proc["elapsed_ms"], "elapsed_ms"),
                   "cpu_sample_weight": weight, "cpu_sample_unit": "samples",
                   "allocated_bytes_estimate": allocated, "gc_pause_ms": pause, "gc_counts": counts,
                   "managed_heap_bytes": heap[-1]["bytes"], "retained_heap_bytes": None}
        for key in ("process_cpu_ms", "peak_rss_bytes"):
            value = proc.get(key)
            if value is None:
                reason = proc.get("unavailable_reasons", {}).get(key)
                m.require(isinstance(reason, str) and reason.strip(), f"Missing {key} reason")
                unavailable[key] = reason
            else:
                number(value, key)
            metrics[key] = value
        return {"schema_version": 1, "kind": "profile-summary", "status": "collected",
                "profiler_attached": True, "acceptance_eligible": False, "metrics": metrics,
                "allocation_method": "Sampled GCAllocationTick byte estimate, not exact allocated bytes",
                "allocation_aggregation": aggregation,
                "allocation_counter_observed_bytes": observed,
                "heap_observation": heap[-1], "unavailable_reasons": unavailable,
                "stack_semantics": "Inclusive sampled stacks; not exclusive CPU milliseconds",
                "stacks": trace["stacks"], "source_hashes": evidence["source_hashes"]}
    except (ValueError, KeyError, TypeError, IndexError, AttributeError) as error:
        return classify_failure("normalize", issue=str(error))


def read_counters(path, pid):
    m.require(Path(path).stat().st_size <= LIMITS["json_bytes"], "Counter size budget exceeded")
    observations, origin = [], None
    names = {"Allocation Rate (B / 1 sec)": ("alloc-rate", "B / 1 sec", "increment", "Rate"),
             "GC Heap Size (MB)": ("gc-heap-size", "MB", "gauge", "Metric")}
    with Path(path).open(encoding="utf-8-sig", newline="") as stream:
        reader = csv.DictReader(stream)
        m.require(reader.fieldnames == ["Timestamp", "Provider", "Counter Name", "Counter Type", "Mean/Increment"],
                  "Unsupported dotnet-counters CSV schema")
        for row in reader:
            m.require(None not in row and all(value is not None for value in row.values()), "Malformed counter row")
            m.require(row["Provider"] == "System.Runtime", "Wrong counter provider")
            value = row["Timestamp"]
            if re.fullmatch(r"\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2}(?: [AP]M)?", value):
                pattern = "%m/%d/%Y %I:%M:%S %p" if value.endswith(("AM", "PM")) else "%m/%d/%Y %H:%M:%S"
                stamp = datetime.strptime(value, pattern)
            else:
                stamp = datetime.fromisoformat(value.replace("Z", "+00:00"))
            timestamp = stamp.timestamp() * 1000
            if origin is None:
                origin = timestamp
            if row["Counter Name"] in names:
                name, unit, semantics, kind = names[row["Counter Name"]]
                m.require(row["Counter Type"] == kind, "Wrong counter type")
                observations.append({"timestamp_ms": round(timestamp - origin, 3), "name": name,
                                     "value": number(float(row["Mean/Increment"]), "counter value"),
                                     "unit": unit, "semantics": semantics})
    m.require(observations, "Empty/unsupported counters")
    return {"process_id": pid, "provider": "System.Runtime", "interval_seconds": 1, "observations": observations}


def trace_completed(evidence):
    return any(c["phase"] == "trace" and c["exit_status"] == 0
               and "Trace completed." in c.get("stdout", "") for c in evidence["commands"])


def validate_profile_artifacts(evidence):
    m.require(trace_completed(evidence), "Missing trace completion")
    artifacts = evidence["artifacts"]
    names = [Path(r["path"]).name for r in artifacts]
    m.require(len(names) == len(set(names))
              and {"compiler.nettrace", "trace-reduction.json", "counters.csv"} <= set(names),
              "Missing or duplicate original trace, reduction or counters")
    timings = evidence["timings"]
    m.require(timings["status"] == "collected" and timings["acceptance_eligible"] is False,
              "Missing diagnostic build/timing evidence")
    commands = [timings[k] for k in ("build", "extraction")]
    records = artifacts + [timings[k] for k in ("binlog", "extractor", "output")]
    if "dump" in evidence:
        dump = evidence["dump"]
        m.require(dump["status"] == "collected" and dump["published"] is False
                  and dump["reason_for_dump"] in REASONS and isinstance(dump["hypothesis"], str)
                  and dump["hypothesis"].strip(), "Missing requested private dump evidence")
        select_compiler_process([dump["identity"]], evidence["process"]["start"])
        commands += [dump[k] for k in ("command", "validation")]
        records.append(dump["artifact"])
    for command in commands:
        m.require(command["status"] == "collected" and type(command["exit_status"]) is int
                  and command["exit_status"] == 0, "Failed timing/dump command")
    total = 0
    for record in records:
        path = Path(m.validate_file_record(record))
        limit = (LIMITS["trace_bytes"] if path.suffix == ".nettrace" else
                 LIMITS["json_bytes"] if path.suffix in (".json", ".csv") else LIMITS["artifact_bytes"])
        size = path.stat().st_size
        m.require(not path.is_symlink() and 0 < size <= limit, "Empty artifact or size budget exceeded")
        if "size_bytes" in record:
            m.require(type(record["size_bytes"]) is int and record["size_bytes"] == size,
                      "Artifact size differs from recorded evidence")
        total += size
    m.require(total <= LIMITS["total_bytes"], "Artifact total budget exceeded")
    m.verify_files([{"path": r["path"], "sha256": r["sha256"]} for r in records])
    by_name = {Path(r["path"]).name: r["path"] for r in artifacts}
    m.require(read_bounded_json(by_name["trace-reduction.json"]) == evidence["trace"],
              "Reduction differs from the recorded trace evidence")
    m.require(read_counters(by_name["counters.csv"], evidence["process"]["start"]["pid"]) == evidence["counters"],
              "Counters differ from the recorded CSV evidence")
    output = Path(timings["output"]["path"])
    m.require(output.stat().st_size <= LIMITS["log_bytes"], "Timing output size budget exceeded")
    text = output.read_text(encoding="utf-8")
    m.require(timings["output_sha256"] == timings["output"]["sha256"]
              and timings["output"] == timings["extraction"]["logs"][0]
              and text.split() == timings["extraction"]["stdout"].split()
              and validate_timings(text) == timings["projects"], "Timing output differs from recorded evidence")


def analyze_profile(path):
    try:
        evidence = read_bounded_json(path)
        result = normalize_evidence(evidence)
        if result["status"] != "collected":
            return result
        validate_profile_artifacts(evidence)
        result["evidence_manifest"] = m.file_record(path)
        return result
    except (ValueError, OSError, KeyError, TypeError, IndexError, AttributeError) as error:
        return classify_failure("analyze", issue=str(error))


def target_environment(root):
    environment = m.allowed_environment(root)
    environment.update(DOTNET_EnableDiagnostics="1", DOTNET_DefaultDiagnosticPortSuspend="1")
    return environment


def tool_environment(target=None):
    keys = (*m.INHERITED_ENVIRONMENT_KEYS, "HOME", "USERPROFILE", "APPDATA", "LOCALAPPDATA",
            "DOTNET_ROOT", "DOTNET_HOST_PATH", "NUGET_PACKAGES", "TMP", "TEMP", "TMPDIR")
    env = {k: os.environ[k] for k in keys if k in os.environ}
    env.update(DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", BUILDING_USING_DOTNET="true",
               DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="1")
    if target is not None:
        for key in ("TMP", "TEMP", "TMPDIR"):
            env[key] = target[key]
    return env


def private_directory(path):
    path = Path(path).resolve()
    path.mkdir(mode=0o700)
    if os.name == "nt":
        escaped = str(path).replace("'", "''")
        script = (
            "$ErrorActionPreference='Stop';"
            "$sid=[Security.Principal.WindowsIdentity]::GetCurrent().User;"
            "$acl=New-Object Security.AccessControl.DirectorySecurity;"
            "$acl.SetAccessRuleProtection($true,$false);"
            "$rule=New-Object Security.AccessControl.FileSystemAccessRule("
            "$sid,'FullControl','ContainerInherit,ObjectInherit','None','Allow');"
            f"$acl.AddAccessRule($rule);[IO.Directory]::SetAccessControl('{escaped}',$acl)"
        )
        subprocess.run(["powershell.exe", "-NoProfile", "-NonInteractive", "-Command", script],
                       capture_output=True, check=True, timeout=15, env=tool_environment())
    return path


def check_budget(root):
    total = 0
    for path in Path(root).rglob("*"):
        m.require(not path.is_symlink(), "Symlink in owned artifact directory")
        if path.is_file():
            size = path.stat().st_size
            limit = (LIMITS["trace_bytes"] if path.suffix == ".nettrace" else
                     LIMITS["json_bytes"] if path.suffix in (".json", ".csv") else LIMITS["artifact_bytes"])
            m.require(size <= limit, "budget_exceeded")
            total += size
    m.require(total <= LIMITS["total_bytes"], "budget_exceeded")


def remove_incomplete(path, root):
    path, root = Path(path), Path(root).resolve()
    m.require(not path.is_symlink() and path.resolve().is_relative_to(root) and path.resolve() != root
              and path.is_file(), "Cleanup is limited to owned incomplete files")
    record = dict(m.file_record(path), size_bytes=path.stat().st_size, status="removed_incomplete")
    path.unlink()
    return record


def unlink_closed(path):
    # Windows child handle teardown can lag job termination.
    deadline = time.monotonic() + 5
    while True:
        try:
            path.unlink()
            return
        except PermissionError:
            if time.monotonic() >= deadline:
                raise
            time.sleep(.05)


class Command:
    """Own a child tree and bounded private streams; persist only sanitized text."""

    def __init__(self, arguments, cwd, environment, root, phase, input_file=None):
        self.root, self.phase, self.arguments = Path(root), phase, arguments
        stem = f"{phase}-{uuid.uuid4().hex[:8]}"
        self.paths = [self.root / f"{stem}.{part}.raw" for part in ("stdout", "stderr")]
        self.streams = [p.open("wb") for p in self.paths]
        self.job, self.process, self.stopped = None, None, False
        self.started_at_unix_ns = time.time_ns()
        # CREATE_SUSPENDED closes the launch-to-job-assignment race.
        isolation = ({"creationflags": subprocess.CREATE_NEW_PROCESS_GROUP | 0x4} if os.name == "nt"
                     else {"start_new_session": True})
        try:
            if os.name == "nt":
                self.job = WindowsJob()
            self.process = subprocess.Popen(arguments, cwd=cwd, env=environment,
                                            stdin=input_file if input_file is not None else subprocess.PIPE,
                                            stdout=self.streams[0], stderr=self.streams[1], **isolation)
            if self.job is not None:
                self.job.assign_and_resume(self.process)
        except BaseException:
            if self.process is not None:
                self.process.kill()
                self.process.wait()
                if self.process.stdin is not None:
                    self.process.stdin.close()
            if self.job is not None:
                self.job.close()
            for stream in self.streams:
                stream.close()
            for path in self.paths:
                unlink_closed(path)
            raise
        self.start = time.monotonic()

    def check(self):
        m.require(all(p.stat().st_size <= LIMITS["log_bytes"] for p in self.paths), "budget_exceeded")
        check_budget(self.root)

    def output(self):
        with self.paths[0].open("rb") as stream:
            return stream.read(LIMITS["log_bytes"]).decode("utf-8", errors="replace")

    def stop(self):
        if not self.stopped:
            if self.job is not None:
                self.job.close()
            else:
                try:
                    os.killpg(self.process.pid, signal.SIGKILL)
                except ProcessLookupError:
                    pass
            self.process.wait()
            self.stopped = True

    def finish(self, issue=None):
        self.stop()
        if self.process.stdin is not None:
            self.process.stdin.close()
        for stream in self.streams:
            stream.close()
        texts = []
        logs = []
        for path in self.paths:
            with path.open("rb") as stream:
                text = sanitize(stream.read(LIMITS["log_bytes"]).decode("utf-8", errors="replace"))
            texts.append(text)
            clean = path.with_suffix(".log")
            clean.write_text(text, encoding="utf-8", newline="")
            logs.append(m.file_record(clean))
            unlink_closed(path)
        code = self.process.returncode
        result = (classify_failure(self.phase, code, texts[1], issue) if issue or code
                  else {"status": "collected", "phase": self.phase, "exit_status": code, "stderr": texts[1]})
        result.update(arguments=[sanitize(a) for a in self.arguments], stdout=texts[0], logs=logs,
                      elapsed_ms=(time.monotonic() - self.start) * 1000,
                      started_at_unix_ns=self.started_at_unix_ns)
        result["target_pid" if self.phase == "target" else "profiler_pid"] = self.process.pid
        return result


def run_tool(arguments, cwd, environment, root, phase, timeout=180, monitor=None, input_bytes=None, input_file=None):
    command = None
    try:
        m.require(input_bytes is None or input_file is None, "Only one tool input is allowed")
        command = Command(arguments, cwd, environment, root, phase,
                          *((input_file,) if input_file is not None else ()))
        if input_bytes is not None:
            remaining = memoryview(input_bytes)
            while remaining:
                written = os.write(command.process.stdin.fileno(), remaining)
                m.require(written > 0, "Child input pipe closed")
                remaining = remaining[written:]
            command.process.stdin.close()
        while True:
            command.check()
            if monitor:
                monitor()
            m.require(time.monotonic() - command.start < timeout, "timeout")
            if command.process.poll() is not None:
                break
            time.sleep(.05)
        return command.finish()
    except (OSError, ValueError, KeyboardInterrupt) as error:
        issue = "cancelled" if isinstance(error, KeyboardInterrupt) else str(error)
        result = command.finish(issue) if command else classify_failure(
            phase, stderr=str(error), issue="cancelled" if isinstance(error, KeyboardInterrupt) else "launch_failed")
        if isinstance(error, KeyboardInterrupt):
            raise CollectionCancelled(result) from error
        return result


def validate_timings(text):
    projects, current = [], None
    for line in text.splitlines():
        header = re.fullmatch(r"=== (.+) ===", line.strip())
        if header:
            current = header[1]
        elif current and current != "Unknown Project" and re.search(r"\|[^|]+\|\s*\d+(?:\.\d+)?\s*\|", line):
            if current not in projects:
                projects.append(current)
    m.require(projects and any("FSharp.Compiler.Service" in p for p in projects),
              "Missing project-tagged Fsc timing data (zero extractor exit is not evidence)")
    return projects


def snapshot_process(pid):
    m.require(type(pid) is int and pid > 0, "Invalid PID")
    if sys.platform == "linux":
        root = Path("/proc") / str(pid)
        try:
            stat = (root / "stat").read_text().rsplit(")", 1)[1].split()
            executable = str((root / "exe").resolve(strict=True))
            command = (root / "cmdline").read_bytes().decode().rstrip("\0").split("\0")
            uid = str(root.stat().st_uid)
            return {"pid": pid, "start_time": stat[19], "parent_pid": int(stat[1]),
                    "session_id": int(stat[3]), "executable": executable, "command": command,
                    "uid": uid, "namespace": os.readlink(root / "ns" / "pid"),
                    "binary_sha256": m.file_record(executable)["sha256"]}
        except FileNotFoundError as error:
            raise ProcessLookupError("Target exited before identity verification") from error
    m.require(os.name == "nt", "Unsupported process identity platform")
    script = (
        "[Console]::OutputEncoding=[Text.UTF8Encoding]::new($false);"
        f"$p=Get-CimInstance Win32_Process -Filter 'ProcessId={pid}';"
        "if (!$p) {exit 3}; $owner=Invoke-CimMethod -InputObject $p -MethodName GetOwnerSid;"
        "if ($owner.ReturnValue -ne 0 -or !$p.ExecutablePath -or !$p.CommandLine) {exit 5};"
        "@{pid=[int]$p.ProcessId;start_time=$p.CreationDate.ToUniversalTime().Ticks.ToString();"
        "parent_pid=[int]$p.ParentProcessId;session_id=[int]$p.SessionId;executable=$p.ExecutablePath;"
        "command_line=$p.CommandLine;uid=$owner.Sid;namespace=('windows-session-'+$p.SessionId)}|ConvertTo-Json -Compress"
    )
    result = subprocess.run(["powershell.exe", "-NoProfile", "-NonInteractive", "-Command", script],
                            capture_output=True, text=True, encoding="utf-8", timeout=15, env=tool_environment())
    if result.returncode == 3:
        raise ProcessLookupError("Target exited before identity verification")
    if result.returncode:
        raise PermissionError("Cannot inspect target command/owner/start time: " + sanitize(result.stderr))
    identity = json.loads(result.stdout)
    shell = ctypes.WinDLL("shell32", use_last_error=True)
    shell.CommandLineToArgvW.argtypes = [ctypes.c_wchar_p, ctypes.POINTER(ctypes.c_int)]
    shell.CommandLineToArgvW.restype = ctypes.POINTER(ctypes.c_wchar_p)
    count = ctypes.c_int()
    args = shell.CommandLineToArgvW(identity.pop("command_line"), ctypes.byref(count))
    if not args:
        raise ctypes.WinError(ctypes.get_last_error())
    try:
        identity["command"] = [args[i] for i in range(count.value)]
    finally:
        kernel = ctypes.WinDLL("kernel32")
        kernel.LocalFree.argtypes = [ctypes.c_void_p]
        kernel.LocalFree(args)
    identity["binary_sha256"] = m.file_record(identity["executable"])["sha256"]
    return identity


def windows_accounting(process):
    from ctypes import wintypes

    kernel = ctypes.WinDLL("kernel32", use_last_error=True)
    kernel.GetProcessTimes.argtypes = [wintypes.HANDLE] + [ctypes.POINTER(ctypes.c_ulonglong)] * 4
    values = [ctypes.c_ulonglong() for _ in range(4)]
    if not kernel.GetProcessTimes(int(process._handle), *(ctypes.byref(v) for v in values)):
        raise ctypes.WinError(ctypes.get_last_error())

    class MemoryCounters(ctypes.Structure):
        _fields_ = [("cb", wintypes.DWORD), ("PageFaultCount", wintypes.DWORD)] + [
            (name, ctypes.c_size_t) for name in ("PeakWorkingSetSize", "WorkingSetSize", "QuotaPeakPagedPoolUsage",
                                               "QuotaPagedPoolUsage", "QuotaPeakNonPagedPoolUsage",
                                               "QuotaNonPagedPoolUsage", "PagefileUsage", "PeakPagefileUsage")]

    counters = MemoryCounters()
    counters.cb = ctypes.sizeof(counters)
    psapi = ctypes.WinDLL("psapi", use_last_error=True)
    psapi.GetProcessMemoryInfo.argtypes = [wintypes.HANDLE, ctypes.POINTER(MemoryCounters), wintypes.DWORD]
    if not psapi.GetProcessMemoryInfo(int(process._handle), ctypes.byref(counters), counters.cb):
        raise ctypes.WinError(ctypes.get_last_error())
    return {"process_cpu_ms": (values[2].value + values[3].value) / 10000,
            "peak_rss_bytes": counters.PeakWorkingSetSize, "creation_filetime": str(values[0].value)}


def preflight(root, environment, identity=None, dump=False):
    m.require(environment.get("DOTNET_EnableDiagnostics") == "1"
              and not any(k.startswith(("COMPlus_", "CORECLR_", "COR_")) for k in environment),
              "Diagnostics disabled or injected runtime configuration")
    m.require(shutil.disk_usage(root).free >= LIMITS["total_bytes"], "Insufficient disk budget")
    if sys.platform == "linux":
        fields = dict(line.split(":", 1) for line in Path("/proc/meminfo").read_text().splitlines())
        available = int(fields["MemAvailable"].split()[0]) * 1024
    elif os.name == "nt":
        class MemoryStatus(ctypes.Structure):
            _fields_ = [("length", ctypes.c_ulong), ("load", ctypes.c_ulong)] + [
                (name, ctypes.c_ulonglong) for name in ("total", "available", "page_total", "page_available",
                                                      "virtual_total", "virtual_available", "extended")]
        status = MemoryStatus()
        status.length = ctypes.sizeof(status)
        m.require(ctypes.windll.kernel32.GlobalMemoryStatusEx(ctypes.byref(status)), "Cannot inspect memory budget")
        available = status.available
    else:
        raise ValueError("Unsupported diagnostics platform")
    m.require(available >= LIMITS["minimum_memory_bytes"], "Insufficient memory budget")
    result = {"architecture": platform.machine(), "available_memory_bytes": available,
              "free_disk_bytes": shutil.disk_usage(root).free, "eventpipe": "pending actual startup handshake"}
    if identity and sys.platform == "linux":
        m.require(identity["uid"] == str(os.getuid()), "Same-UID diagnostics attachment required")
        m.require(identity["namespace"] == os.readlink("/proc/self/ns/pid"), "namespace_unavailable")
        sockets = list(Path(environment["TMPDIR"]).glob(f"dotnet-diagnostic-{identity['pid']}-*-socket"))
        m.require(len(sockets) == 1 and sockets[0].is_socket()
                  and sockets[0].stat().st_uid == os.getuid(), "Diagnostics socket namespace/ownership unavailable")
        result["socket"] = str(sockets[0])
        ptrace = Path("/proc/sys/kernel/yama/ptrace_scope")
        result["ptrace_scope"] = ptrace.read_text().strip() if ptrace.exists() else "not exposed"
        if dump:
            m.require(result["ptrace_scope"] not in ("2", "3"), "ptrace restrictions block unprivileged dump")
    return result


def verify_tools(dotnet, root, names=("dotnet-trace", "dotnet-counters", "dotnet-dump"), monitor=None):
    pinned = m.read_json(REPO / ".config" / "dotnet-tools.json")["tools"]
    host = run_tool([dotnet, "--info"], REPO, tool_environment(), root, "diagnostics-host", monitor=monitor)
    architecture = re.search(r"Architecture:\s+(x64|x86|arm64|arm)\b", host.get("stdout", ""))
    expected = {"amd64": "x64", "x86_64": "x64", "aarch64": "arm64",
                "arm64": "arm64", "x86": "x86", "i386": "x86", "armv7l": "arm"}.get(platform.machine().lower())
    m.require(host["status"] == "collected" and architecture and architecture[1] == expected,
              "Unsupported/mismatched diagnostics host architecture")
    records = {"host": {"executable": m.file_record(dotnet), "architecture": architecture[1], "info": host["logs"]}}
    for name in names:
        m.require(pinned[name]["version"] == TOOL_VERSION and pinned[name]["rollForward"], "Unexpected tool pin")
        prefix = [dotnet, "tool", "run", name, "--"]
        version = run_tool(prefix + ["--version"], REPO, tool_environment(), root, name + "-version", monitor=monitor)
        if version["status"] != "collected":
            m.require(re.search(r"(?i)dotnet tool restore|cannot find a tool|has not been restored",
                                version.get("stdout", "") + version["stderr"]),
                      "Diagnostics tool probe failed: " + version["stderr"])
            restore = run_tool([dotnet, "tool", "restore"], REPO, tool_environment(), root, "tool-restore", monitor=monitor)
            m.require(restore["status"] == "collected", "Pinned tool restore failed: " + restore["stderr"])
            version = run_tool(prefix + ["--version"], REPO, tool_environment(), root, name + "-version", monitor=monitor)
        m.require(version["status"] == "collected" and version["stdout"].strip().startswith(TOOL_VERSION + "+"),
                  "Incompatible diagnostics tool version")
        help_result = run_tool(prefix + ["collect", "--help"], REPO, tool_environment(), root, name + "-help", monitor=monitor)
        required = ["--process-id", "--output"]
        if name != "dotnet-dump":
            required += ["--resume-runtime"]
        m.require(help_result["status"] == "collected" and all(flag in help_result["stdout"] for flag in required),
                  "Unsupported pinned tool syntax")
        records[name] = {"version": version["stdout"].strip(), "help": help_result["logs"], "command": prefix}
    return records


def collect_dump(document, reason, output_dir, monitor=None):
    root, command, dump = None, None, None
    cancelled = None
    result = {"published": False, "publication_reason": "Private local dump; controller encryption and sink acknowledgment required"}
    phase = "hypothesis"
    try:
        m.require(reason in REASONS and isinstance(document.get("hypothesis"), str)
                  and document["hypothesis"].strip() and document.get("profiling_replay") is True,
                  "A recorded retention/growth/RSS hypothesis and separate profiling replay are required")
        m.require(LIMITS["collections"] >= 1, "Collection count budget exceeded")
        phase = "select"
        identity = document["identity"]
        select_compiler_process([snapshot_process(identity["pid"])], identity)
        root = private_directory(output_dir)
        phase = "preflight"
        environment = document.get("environment", target_environment(root))
        result["preflight"] = preflight(root, environment, identity, dump=True)
        dotnet = identity["executable"]
        tools = verify_tools(dotnet, root, ("dotnet-dump",), monitor=monitor)
        result.update(reason_for_dump=reason, hypothesis=sanitize(document["hypothesis"]),
                      identity=identity, tools=tools, dump_type="Heap")
        select_compiler_process([snapshot_process(identity["pid"])], identity)
        phase = "dump"
        dump = root / "heap.dmp"
        command = run_tool(tools["dotnet-dump"]["command"] + [
            "collect", "--process-id", str(identity["pid"]), "--type", "Heap", "--output", str(dump)],
            REPO, tool_environment(environment), root, phase, timeout=120, monitor=monitor)
        result["command"] = command
        m.require(command["status"] == "collected", command["reason"] if "reason" in command else "tool_failure")
        select_compiler_process([snapshot_process(identity["pid"])], identity)
        m.require(dump.exists() and dump.stat().st_size > 4096, "empty_artifact")
        with dump.open("rb") as stream:
            m.require(stream.read(4) in (b"MDMP", b"\x7fELF"), "corrupt_artifact")
        phase = "dump-validate"
        validation = run_tool(tools["dotnet-dump"]["command"] + [
            "analyze", str(dump), "-c", "eeversion", "-c", "dumpheap -stat", "-c", "exit"],
            REPO, tool_environment(), root, phase, timeout=120, monitor=monitor)
        result["validation"] = validation
        text = validation["stdout"] + validation["stderr"]
        m.require(not re.search(r"(?i)unsupported.*debugging interface|debugging interface.*not supported", text),
                  "incompatible_runtime")
        m.require(validation["status"] == "collected", validation.get("reason", "tool_failure"))
        m.require(re.search(r"Total\s+(?:\d{1,3}(?:,\d{3})+|\d+)\s+objects", text)
                  and not re.search(r"(?i)failed to|invalid dump|could not|error:", text), "corrupt_artifact")
        check_budget(root)
        result.update(status="collected", artifact=dict(m.file_record(dump), size_bytes=dump.stat().st_size),
                      retained_heap_bytes=None,
                      retention_reason="Heap snapshot includes unreachable objects; no live/post-GC retained-byte claim")
        result.update(schema_version=1, kind="dump-evidence", profiler_attached=True, acceptance_eligible=False)
        m.write_json(root / "dump-evidence.json", result)
        check_budget(root)
    except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError, KeyboardInterrupt) as error:
        if isinstance(error, KeyboardInterrupt):
            cancelled = error
        if isinstance(error, CollectionCancelled):
            result["cancelled_command"] = error.result
        issue = ("cancelled" if cancelled is not None else
                 "permission_denied" if isinstance(error, PermissionError) else
                 "early_exit" if isinstance(error, ProcessLookupError) else str(error))
        failed = result.get("cancelled_command", result.get("validation", command))
        result.update(classify_failure(phase, failed.get("exit_status") if failed else None,
                                       failed.get("stderr", "") if failed else str(error), issue))
        if dump and dump.is_file():
            result["incomplete_artifact"] = remove_incomplete(dump, root)
            result.pop("artifact", None)
    result.update(schema_version=1, kind="dump-evidence", profiler_attached=True, acceptance_eligible=False)
    if root:
        m.write_json(root / "dump-evidence.json", result)
    if cancelled is not None:
        raise cancelled
    return result


def frozen_inputs(manifest, compiler):
    m.validate_manifest(manifest, "workload")
    m.validate_manifest(compiler, "compiler")
    invariants = manifest["invariants"]
    for section in ("bootstrap", "toolchain"):
        m.require(compiler[f"{section}_fingerprint"] == m.fingerprint_inputs(invariants[section]),
                  f"Compiler {section} differs from frozen workload")
    m.require(compiler["files"]["fsc.runtimeconfig.json"]["sha256"] ==
              invariants["toolchain"]["compiler_runtimeconfig_sha256"], "Compiler runtime changed")
    workload = invariants["workload"]
    records = [r for key in ("sources", "generated", "references", "resources", "auxiliary") for r in workload[key]]
    records += [workload["response_file"], workload["core_package"], invariants["toolchain"]["dotnet"]]
    records += list(compiler["files"].values()) + invariants["toolchain"]["runtime_files"]
    m.verify_files(records)
    m.require(Path(workload["response_file"]["path"]).read_text(encoding="utf-8-sig").splitlines()
              == workload["arguments"], "Frozen response arguments changed")
    tracked = {Path(r["path"]).relative_to(workload["checkout"]).as_posix() for r in workload["sources"]}
    captured = m.capture_inputs(workload["arguments"], workload["cwd"], workload["checkout"], tracked)
    m.require(all(workload[k] == value for k, value in captured.items()), "Frozen input inventory differs")
    env = tool_environment()
    revision = subprocess.run(["git", "rev-parse", "HEAD"], cwd=workload["checkout"], env=env,
                              capture_output=True, text=True, timeout=30, check=True)
    m.require(revision.stdout.strip() == workload["revision"], "Frozen checkout revision changed")
    tracked_result = subprocess.run(["git", "ls-files"], cwd=workload["checkout"], env=env,
                                    capture_output=True, text=True, timeout=30, check=True)
    m.require(tracked <= set(tracked_result.stdout.splitlines()), "Untracked/private sources are not a compiler workload")
    return records


def diagnostic_timings(manifest, compiler, root, dotnet):
    workload = manifest["invariants"]["workload"]
    environment = dict(manifest["invariants"]["environment"]["variables"])
    # These variables were allowlist-validated by validate_manifest; never inherit runner credentials.
    environment["DOTNET_EnableDiagnostics"] = "0"
    project = str(Path(workload["checkout"]) / m.PROJECT)
    binlog = root / "diagnostic.binlog"
    args = [dotnet, "build", project, "-c", "Release", "--no-restore", "--disable-build-servers",
            "-t:Rebuild", "-p:BuildProjectReferences=false", f"-p:TargetFramework={workload['target_framework']}",
            "-p:FSHARPCORE_USE_PACKAGE=true", "-p:UseSharedCompilation=false",
            "-p:ProduceReferenceAssembly=false", "-p:ProduceOnlyReferenceAssembly=false",
            "-p:ProvideCommandLineArgs=true", "-p:SkipCompilerExecution=false",
            f"-p:ParallelCompilation={str(workload['parallel_compilation']).lower()}",
            f"-p:CompilerPerfCaptureDir={root}",
            "-p:DotnetFscCompilerPath=%22" + compiler["files"]["fsc.dll"]["path"] + "%22",
            "-p:FSharpBuildTasksAssembly=" + str(Path(compiler["files"]["fsc.dll"]["path"]).with_name("FSharp.Build.dll")),
            "-p:FscToolPath=" + str(Path(dotnet).parent), "-p:FscToolExe=" + Path(dotnet).name,
            "-bl:" + str(binlog), "-v:quiet"]
    build = run_tool(args, workload["checkout"], environment, root, "diagnostic-build", timeout=600)
    result = {"build": build, "acceptance_eligible": False}
    if build["status"] != "collected":
        return dict(result, **classify_failure("diagnostic-build", build["exit_status"], build["stderr"]))
    extraction = run_tool([dotnet, "fsi", str(REPO / "eng" / "scripts" / "ExtractTimingsFromBinlog.fsx"), str(binlog)],
                          REPO, tool_environment(), root, "timings-extract", timeout=180)
    result["extraction"] = extraction
    try:
        m.require(extraction["status"] == "collected", "Binlog extractor failed")
        result.update(projects=validate_timings(extraction["stdout"]), binlog=m.file_record(binlog),
                      extractor=m.file_record(REPO / "eng" / "scripts" / "ExtractTimingsFromBinlog.fsx"),
                      output=extraction["logs"][0], output_sha256=extraction["logs"][0]["sha256"], status="collected")
    except ValueError as error:
        result.update(classify_failure("timings-extract", extraction["exit_status"], extraction["stderr"], str(error)))
    return result


def collect_profile(manifest, compiler, output_dir, dump_reason=None, hypothesis=None, *, ipc_root=None):
    root = private_directory(output_dir)
    evidence = {"schema_version": 1, "kind": "compiler-profile", "profiler_attached": True,
                "acceptance_eligible": False, "commands": [], "limits": dict(LIMITS),
                "collection_count": 1}
    target = trace = counters = None
    cancelled = None
    lock = None
    lock_owned = False
    phase = "validate"
    try:
        evidence.update(collector=m.file_record(__file__), reducer=m.file_record(HERE / "AnalyzeTrace.fsx"))
        m.require(LIMITS["collections"] >= evidence["collection_count"], "Collection count budget exceeded")
        m.require(dump_reason is None or (dump_reason in REASONS and isinstance(hypothesis, str)
                                         and hypothesis.strip()), "Dump requires a recorded hypothesis")
        records = frozen_inputs(manifest, compiler)
        workload = manifest["invariants"]["workload"]
        dotnet = manifest["invariants"]["toolchain"]["dotnet"]["path"]
        lock = Path(manifest["owned_root"]) / "measurement.lock"
        lock.mkdir()
        lock_owned = True
        evidence.update(compiler=compiler, fingerprints={k: m.fingerprint_inputs(v)
                        for k, v in manifest["invariants"].items()},
                        source_hashes=workload["sources"] + workload["generated"])
        runtime = m.read_json(compiler["files"]["fsc.runtimeconfig.json"]["path"])["runtimeOptions"]["framework"]["version"]
        evidence["expected_runtime_major"] = int(runtime.split(".")[0])
        phase = "preflight"
        environment = target_environment(root if ipc_root is None else ipc_root)
        evidence["preflight"] = preflight(root, environment)
        tools = verify_tools(dotnet, root)
        evidence["tools"] = tools
        evidence["settings"] = {"providers": PROVIDERS, "sample_interval": "SampleProfiler runtime default (~1 ms)",
                                "counter_interval_seconds": 1, "buffer_megabytes": 256,
                                "startup": "DefaultDiagnosticPortSuspend; trace no-resume; counters resume"}
        m.reset_outputs(workload["outputs"], workload["checkout"], records)
        args = [dotnet, compiler["files"]["fsc.dll"]["path"], "@" + workload["response_file"]["path"]]
        phase = "select"
        target = Command(args, workload["cwd"], environment, root, "target")
        identity = snapshot_process(target.process.pid)
        m.require(identity["command"] == args and Path(identity["executable"]).resolve() == Path(dotnet).resolve()
                  and identity["parent_pid"] == os.getpid()
                  and identity["binary_sha256"] == manifest["invariants"]["toolchain"]["dotnet"]["sha256"],
                  "Launched target identity differs")
        parent_identity = snapshot_process(os.getpid())
        expected_session = target.process.pid if sys.platform == "linux" else parent_identity["session_id"]
        m.require(identity["session_id"] == expected_session, "Wrong target session")
        m.require(identity["uid"] == parent_identity["uid"], "Same-user diagnostics attachment required")
        evidence["preflight"].update(preflight(root, environment, identity))
        process_document = {"schema_version": 1, "kind": "verified-process", "identity": identity,
                            "profiling_replay": True, "hypothesis": hypothesis, "environment": environment}
        m.write_json(root / "verified-process.json", process_document)
        evidence["process"] = {"start": identity}
        phase = "trace"
        trace_path = root / "compiler.nettrace"
        trace = Command(tools["dotnet-trace"]["command"] + [
            "collect", "--process-id", str(identity["pid"]), "--resume-runtime:false",
            "--providers", PROVIDERS, "--buffersize", "256", "--output", str(trace_path)],
            REPO, tool_environment(environment), root, phase)
        deadline = time.monotonic() + 30
        # In pinned v8, this redirected-output line is emitted after StartEventPipeSession.
        while not (trace_path.exists() and f"Output File    : {trace_path}" in trace.output()):
            trace.check()
            m.require(trace.process.poll() is None, "Trace exited before startup handshake")
            m.require(time.monotonic() < deadline, "Trace startup timeout / EventPipe unavailable")
            time.sleep(.05)
        select_compiler_process([snapshot_process(identity["pid"])], identity)
        phase = "counters"
        counters = Command(tools["dotnet-counters"]["command"] + [
            "collect", "--process-id", str(identity["pid"]), "--resume-runtime:true", "--refresh-interval", "1",
            "--counters", "EventCounters\\System.Runtime", "--format", "csv", "--output", str(root / "counters.csv")],
            REPO, tool_environment(environment), root, phase)
        resume = time.monotonic()
        usage = None
        dumped = False
        last_identity = identity
        profiler_exit_at = None

        def monitor():
            for command in (target, trace, counters):
                command.check()
            m.require(time.monotonic() - target.start < LIMITS["duration_seconds"], "timeout")

        while True:
            monitor()
            if sys.platform == "linux":
                waited, status, usage_now = os.wait4(target.process.pid, os.WNOHANG)
                if waited:
                    target.process.returncode = os.waitstatus_to_exitcode(status)
                    usage = usage_now
                    break
            elif target.process.poll() is not None:
                break
            if trace.process.poll() is not None or counters.process.poll() is not None:
                if profiler_exit_at is None:
                    profiler_exit_at = time.monotonic()
                # EventPipe closes during CLR shutdown, before the OS process handle signals exit.
                m.require(time.monotonic() - profiler_exit_at < 2, "Profiler exited before target")
            if dump_reason and not dumped and time.monotonic() - resume >= 2:
                evidence["dump"] = collect_dump(process_document, dump_reason, root / "private-dump", monitor=monitor)
                dumped = True
            time.sleep(.05)
        ended = time.monotonic()
        accounting = ({"process_cpu_ms": (usage.ru_utime + usage.ru_stime) * 1000,
                       "peak_rss_bytes": usage.ru_maxrss * 1024} if usage else windows_accounting(target.process))
        evidence["process"].update(end=last_identity, end_identity_proof="Owned process handle/wait4, not a PID relookup",
                                   elapsed_ms=(ended - target.start) * 1000,
                                   exit_status=target.process.returncode, **accounting)
        evidence["coverage"] = {"trace_before_resume": True, "counters_before_resume": True,
                                "duration_ms": (ended - resume) * 1000}
        if dump_reason and not dumped:
            evidence["dump"] = dict(classify_failure("dump", issue="early_exit"), published=False)
        evidence["commands"].append(target.finish())
        target = None
        for name, command in (("trace", trace), ("counters", counters)):
            phase = name
            deadline = time.monotonic() + 30
            while command.process.poll() is None and time.monotonic() < deadline:
                command.check()
                time.sleep(.1)
            result = command.finish("timeout" if command.process.poll() is None else None)
            evidence["commands"].append(result)
            if name == "trace":
                trace = None
            else:
                counters = None
        m.require(evidence["process"]["exit_status"] == 0 and Path(workload["assembly_output"]).is_file(),
                  "Compiler replay failed or produced no assembly")
        m.verify_files(records)
        evidence["artifacts"] = [m.file_record(p) for p in (trace_path, root / "counters.csv") if p.is_file()]
        phase = "timings"
        evidence["timings"] = diagnostic_timings(manifest, compiler, root, dotnet)
        phase = "reduce"
        reduction_path = root / "trace-reduction.json"
        reduction = run_tool([dotnet, "fsi", str(HERE / "AnalyzeTrace.fsx"), str(trace_path),
                              str(reduction_path), str(identity["pid"])],
                             REPO, tool_environment(), root, phase, timeout=180)
        evidence["commands"].append(reduction)
        m.require(reduction["status"] == "collected", "Trace reducer failed: " + reduction["stderr"])
        evidence["trace"] = read_bounded_json(reduction_path)
        phase = "counters-reduce"
        evidence["counters"] = read_counters(root / "counters.csv", identity["pid"])
        evidence["artifacts"].append(m.file_record(reduction_path))
        phase = "normalize"
        summary = normalize_evidence(evidence)
        if evidence["timings"]["status"] != "collected":
            summary = classify_failure("timings", issue=evidence["timings"]["reason"])
        if dump_reason and evidence["dump"]["status"] != "collected":
            summary = classify_failure("dump", issue=evidence["dump"]["reason"])
        if summary["status"] == "collected":
            phase = "artifacts"
            validate_profile_artifacts(evidence)
        evidence["status"] = summary["status"]
    except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError, KeyboardInterrupt) as error:
        if isinstance(error, KeyboardInterrupt):
            cancelled = error
        if isinstance(error, CollectionCancelled):
            evidence["commands"].append(error.result)
            phase = error.result["phase"]
        failed = next((c for c in reversed(evidence["commands"]) if c["phase"] == phase), {})
        summary = classify_failure(phase, failed.get("exit_status"), failed.get("stderr", ""),
                                   "cancelled" if isinstance(error, KeyboardInterrupt) else str(error))
        evidence.update(status="blocked_collection", failure=summary)
    finally:
        cleanup_errors = []
        for command in (counters, trace, target):
            if command is not None:
                try:
                    evidence["commands"].append(command.finish("collection_aborted"))
                except OSError as error:
                    cleanup_errors.append(sanitize(error))
        if cleanup_errors:
            evidence["cleanup_errors"] = cleanup_errors
            summary = classify_failure("cleanup", issue="; ".join(cleanup_errors))
            evidence["status"] = "blocked_collection"
        if lock_owned:
            lock.rmdir()
    trace_complete = trace_completed(evidence)
    incomplete = []
    for path in root.rglob("*"):
        if path.is_file() and path.suffix in (".nettrace", ".binlog", ".dmp", ".etlx"):
            limit = LIMITS["trace_bytes"] if path.suffix == ".nettrace" else LIMITS["artifact_bytes"]
            if path.stat().st_size > limit or (path.suffix == ".nettrace" and not trace_complete):
                incomplete.append(remove_incomplete(path, root))
    if incomplete:
        evidence["incomplete_artifacts"] = incomplete
    m.write_json(root / "evidence-manifest.json", evidence)
    m.write_json(root / "profile-summary.json", summary)
    try:
        check_budget(root)
    except ValueError as error:
        summary = classify_failure("budget", issue=str(error))
        evidence["status"] = "blocked_collection"
        evidence["incomplete_artifacts"] = evidence.get("incomplete_artifacts", []) + [
            remove_incomplete(p, root) for p in root.rglob("*")
            if p.is_file() and p.suffix in (".nettrace", ".binlog", ".dmp", ".etlx")]
    m.write_json(root / "evidence-manifest.json", evidence)
    m.write_json(root / "profile-summary.json", summary)
    if cancelled is not None:
        raise cancelled
    return summary


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    collect = commands.add_parser("collect")
    collect.add_argument("--manifest", required=True)
    collect.add_argument("--compiler", required=True)
    collect.add_argument("--output-dir", required=True)
    collect.add_argument("--dump-reason", choices=REASONS)
    collect.add_argument("--hypothesis")
    analyze = commands.add_parser("analyze")
    analyze.add_argument("--input", required=True)
    analyze.add_argument("--output", required=True)
    dump = commands.add_parser("dump")
    dump.add_argument("--process", required=True)
    dump.add_argument("--reason", choices=REASONS, required=True)
    dump.add_argument("--output-dir", required=True)
    options = parser.parse_args(argv)
    try:
        if options.command == "collect":
            result = collect_profile(read_bounded_json(options.manifest), read_bounded_json(options.compiler),
                                     options.output_dir, options.dump_reason, options.hypothesis)
        elif options.command == "dump":
            result = collect_dump(read_bounded_json(options.process), options.reason, options.output_dir)
            if not Path(options.output_dir).exists():
                private_directory(options.output_dir)
                m.write_json(Path(options.output_dir) / "dump-evidence.json", result)
        else:
            result = analyze_profile(options.input)
            m.write_json(options.output, result)
    except KeyboardInterrupt:
        print(json.dumps(classify_failure(options.command, issue="cancelled")), file=sys.stderr)
        return 130
    except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
        result = classify_failure(options.command, issue=str(error))
        print(json.dumps(result), file=sys.stderr)
        if options.command in ("collect", "dump") and not Path(options.output_dir).exists():
            root = private_directory(options.output_dir)
            m.write_json(root / ("profile-summary.json" if options.command == "collect" else "dump-evidence.json"), result)
    return 0 if result["status"] == "collected" else 2


if __name__ == "__main__":
    sys.exit(main())
