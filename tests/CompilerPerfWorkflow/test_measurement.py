"""Synthetic fixtures, not compiler performance measurements."""

import copy
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
from types import SimpleNamespace
import unittest
from unittest import mock
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
MODULE = ROOT / "eng" / "scripts" / "compiler_perf" / "measurement.py"
spec = importlib.util.spec_from_file_location("measurement", MODULE)
m = importlib.util.module_from_spec(spec)
spec.loader.exec_module(m)


def measurements(candidate_ms=90, primary="elapsed_ms"):
    """All identities and numbers in this fixture are synthetic."""
    invariants = {
        "workload": {"revision": "a" * 40, "configuration": "Release",
                     "default_branch_revision": "a" * 40, "target_framework": "synthetic-tfm",
                     "parallel_compilation": False, "cwd": "synthetic-cwd", "checkout": "synthetic-checkout",
                     "target": "src/Compiler/FSharp.Compiler.Service.fsproj",
                     "sources": ["first", "second"], "generated": ["generated"],
                     "references": ["reference"], "resources": ["resource"],
                     "auxiliary": ["sourcelink"], "outputs": ["output"], "assembly_output": "output",
                     "core": "core", "core_package": "package", "core_package_asset": "asset",
                     "arguments_sha256": "a" * 64, "response_file": "synthetic-response",
                     "arguments": ["--optimize+", "--parallelcompilation-"]},
        "bootstrap": {"revision": "a" * 40, "source_tree": "a" * 40,
                      "binaries": "bootstrap", "commands": []},
        "toolchain": {"sdk": "synthetic", "runtime": "synthetic", "dotnet": "synthetic",
                      "info": "synthetic", "python": "synthetic", "git": "synthetic",
                      "measurement": "synthetic", "capture_target": "synthetic",
                      "runtime_files": ["runtime"], "sdk_compiler_files": ["sdk-compiler"],
                      "compiler_runtimeconfig_sha256": "a" * 64},
        "environment": {"runner": "synthetic", "cpu": "synthetic",
                        "os": "synthetic", "architecture": "synthetic", "hostname": "synthetic",
                        "logical_cpus": 1, "affinity": [0], "cpu_topology": {}, "processor": "synthetic",
                        "cache_policy": "warm", "variables": {}},
        "policy": copy.deepcopy(m.POLICY),
        "experiment": {"id": "synthetic-experiment", "primary_metric": primary,
                       "collector": "process-rusage-v1", "seed": 910},
    }
    documents = []
    for arm in ("baseline", "candidate"):
        compiler = {"schema_version": 1, "kind": "compiler", "configuration": "Release", "source_revision": "a" * 40,
                    "patch_sha256": m.fingerprint_inputs(arm),
                    "bootstrap_fingerprint": m.fingerprint_inputs(invariants["bootstrap"]),
                    "toolchain_fingerprint": m.fingerprint_inputs(invariants["toolchain"]),
                    "files": {name: {"path": str(ROOT / "synthetic" / arm / name),
                                     "sha256": m.fingerprint_inputs(arm + name)}
                              for name in ("fsc.dll", "FSharp.Compiler.Service.dll", "fsc.runtimeconfig.json",
                                           "fsc.deps.json", "FSharp.Core.dll")}}
        doc = {"schema_version": 1, "kind": "samples", "arm": arm,
               "collection_id": "synthetic-collection",
               "collection_complete": True,
               "invariants": copy.deepcopy(invariants),
               "compiler": compiler, "compiler_id": m.fingerprint_inputs(compiler),
               "control_compiler_id": documents[0]["compiler_id"] if documents else m.fingerprint_inputs(compiler),
               "samples": []}
        for event in m.schedule():
            if event["arm"] != arm:
                continue
            sample = dict(event)
            sample.update(
                sample_id=f"synthetic-{event['ordinal']}",
                compiler_id=doc["control_compiler_id"] if event["phase"] == "aa" else doc["compiler_id"],
                fingerprints={k: m.fingerprint_inputs(v) for k, v in invariants.items()},
                profiler_attached=False, elapsed_ms=100 if event["phase"] == "aa" or arm == "baseline" else candidate_ms,
                process_cpu_ms=80, process_peak_rss_bytes=1000 if event["phase"] == "aa" or arm == "baseline" else 900,
                unavailable_reasons={}, exit_status=0, complete=True,
            )
            doc["samples"].append(sample)
        documents.append(doc)
    return documents


class MeasurementTests(unittest.TestCase):
    def test_canonical_and_ordered_hashes(self):
        self.assertEqual(m.fingerprint_inputs({"a": 1, "b": 2}),
                         m.fingerprint_inputs({"b": 2, "a": 1}))
        for key in ("sources", "generated", "references", "arguments"):
            with self.subTest(key=key):
                self.assertNotEqual(m.fingerprint_inputs({key: ["a", "b"]}),
                                    m.fingerprint_inputs({key: ["b", "a"]}))
        for value in (float("nan"), float("inf"), -float("inf")):
            with self.subTest(value=value), self.assertRaises(ValueError):
                m.fingerprint_inputs(value)

    def test_comparison_boundaries(self):
        for candidate, decision in ((90, "measured_improvement"),
                                    (96.999, "measured_improvement"),
                                    (97, "measured_improvement"),
                                    (97.001, "rejected"), (100, "rejected"), (110, "rejected")):
            with self.subTest(candidate=candidate):
                baseline, changed = measurements(candidate)
                result = m.compare_measurements(baseline, changed)
                self.assertEqual(result["decision"], decision, result)
                self.assertEqual(len(result["raw_sample_ids"]), 24)
                self.assertIn("interval_95_percent", result["primary"])
                self.assertNotEqual(baseline["compiler_id"], changed["compiler_id"])

    def test_invariants_cannot_change(self):
        baseline, candidate = measurements()
        paths = [(section, key) for section, values in baseline["invariants"].items()
                 for key in values]
        for section, key in paths:
            with self.subTest(section=section, key=key):
                changed = copy.deepcopy(candidate)
                changed["invariants"][section][key] = "changed"
                result = m.compare_measurements(baseline, changed)
                self.assertEqual(result["decision"], "inconclusive")
                self.assertIn(section, result["invariant_mismatches"])

    def test_incomplete_or_contaminated_runs(self):
        cases = {
            "empty": lambda d: d.update(samples=[]),
            "missing pair": lambda d: d["samples"].pop(),
            "missing warmup": lambda d: d["samples"].pop(0),
            "failed": lambda d: d["samples"][-1].update(exit_status=1),
            "cancelled": lambda d: d["samples"][-1].update(complete=False),
            "profiled": lambda d: d["samples"][-1].update(profiler_attached=True),
            "wrong compiler": lambda d: d["samples"][-1].update(compiler_id="f" * 64),
            "wrong fingerprint": lambda d: d["samples"][-1]["fingerprints"].update(workload="f" * 64),
            "duplicate id": lambda d: d["samples"][-1].update(sample_id=d["samples"][0]["sample_id"]),
            "wrong order": lambda d: d["samples"][-1].update(ordinal=0),
            "wrong AA": lambda d: d.update(control_compiler_id="f" * 64),
            "different invocation": lambda d: d.update(collection_id="different-collection"),
            "killed before finalization": lambda d: d.update(collection_complete=False),
        }
        for label, mutate in cases.items():
            for arm in (0, 1):
                with self.subTest(label=label, arm=arm):
                    docs = measurements()
                    mutate(docs[arm])
                    self.assertNotEqual(m.compare_measurements(*docs)["decision"], "measured_improvement")

    def test_noise_boundaries(self):
        for delta, expected in ((0.999, "measured_improvement"), (1, "measured_improvement"),
                                (1.001, "inconclusive")):
            with self.subTest(aa_delta=delta):
                docs = measurements()
                for sample in docs[1]["samples"]:
                    if sample["phase"] == "aa":
                        sample["elapsed_ms"] = 100 + delta
                self.assertEqual(m.compare_measurements(*docs)["decision"], expected)
        for spread, expected in ((1.999, "measured_improvement"), (2, "measured_improvement"),
                                 (2.001, "inconclusive")):
            with self.subTest(mad=spread):
                docs = measurements()
                samples = [s for s in docs[0]["samples"] if s["phase"] == "ab" and s["role"] == "measured"]
                for s, v in zip(samples, (100-spread, 100-spread, 100, 100+spread, 100+spread)):
                    s["elapsed_ms"] = v
                self.assertEqual(m.compare_measurements(*docs)["decision"], expected)

    def test_memory_predeclaration_and_time_guard(self):
        for elapsed, decision in ((100, "measured_improvement"), (100.999, "measured_improvement"),
                                  (101, "rejected"), (101.001, "rejected")):
            with self.subTest(elapsed=elapsed):
                docs = measurements(elapsed, "process_peak_rss_bytes")
                self.assertEqual(m.compare_measurements(*docs)["decision"], decision)
        for missing in (True, False):
            with self.subTest(missing=missing):
                docs = measurements(100, "process_peak_rss_bytes")
                if missing:
                    docs[1]["samples"][-1]["process_peak_rss_bytes"] = None
                    docs[1]["samples"][-1]["unavailable_reasons"]["process_peak_rss_bytes"] = "not supported"
                else:
                    docs[1]["invariants"]["experiment"]["collector"] = "different-collector"
                self.assertNotEqual(m.compare_measurements(*docs)["decision"], "measured_improvement")
        for rss, decision in ((969, "measured_improvement"), (970, "measured_improvement"),
                              (971, "rejected")):
            with self.subTest(rss=rss):
                docs = measurements(100, "process_peak_rss_bytes")
                for sample in docs[1]["samples"]:
                    if sample["phase"] == "ab":
                        sample["process_peak_rss_bytes"] = rss
                self.assertEqual(m.compare_measurements(*docs)["decision"], decision)
        for rss, decision in ((1009, "measured_improvement"), (1010, "measured_improvement"),
                              (1011, "inconclusive")):
            with self.subTest(control_rss=rss):
                docs = measurements(100, "process_peak_rss_bytes")
                for sample in docs[1]["samples"]:
                    if sample["phase"] == "aa":
                        sample["process_peak_rss_bytes"] = rss
                self.assertEqual(m.compare_measurements(*docs)["decision"], decision)

    def test_uncertainty_cannot_be_replaced_by_median(self):
        for slow in (100, 110):
            with self.subTest(slow=slow):
                docs = measurements()
                values = (slow, slow, 90, 90, 90)
                for sample, elapsed in zip(
                        [s for s in docs[1]["samples"] if s["phase"] == "ab" and s["role"] == "measured"], values):
                    sample["elapsed_ms"] = elapsed
                result = m.compare_measurements(*docs)
                self.assertEqual(result["primary"]["improvement_percent"], 10)
                self.assertLessEqual(result["primary"]["interval_95_percent"][0], 0)
                self.assertEqual(result["decision"], "rejected")

    def test_policy_cannot_be_tuned_in_both_arms(self):
        for key in m.POLICY:
            with self.subTest(key=key):
                docs = measurements()
                for doc in docs:
                    doc["invariants"]["policy"][key] = 0
                    for sample in doc["samples"]:
                        sample["fingerprints"]["policy"] = m.fingerprint_inputs(doc["invariants"]["policy"])
                self.assertNotEqual(m.compare_measurements(*docs)["decision"], "measured_improvement")

    def test_invalid_json_and_metrics(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "input.json"
            for text in ('{"schema_version":', '{"x":NaN}', '{"x":1,"x":2}'):
                with self.subTest(text=text):
                    path.write_text(text, encoding="utf-8")
                    with self.assertRaises(ValueError):
                        m.read_json(path)
        for value in (-1, float("nan"), float("inf"), "100", True):
            for metric in ("elapsed_ms", "process_cpu_ms", "process_peak_rss_bytes"):
                with self.subTest(value=value, metric=metric):
                    doc = measurements()[0]
                    doc["samples"][-1][metric] = value
                    with self.assertRaises(ValueError):
                        m.validate_manifest(doc, "samples")
        for value in (0, 2, "1", True):
            with self.subTest(version=value):
                doc = measurements()[0]
                doc["schema_version"] = value
                with self.assertRaises(ValueError):
                    m.validate_manifest(doc, "samples")
        doc = measurements()[0]
        doc["samples"][-1]["process_cpu_ms"] = None
        with self.assertRaises(ValueError):
            m.validate_manifest(doc, "samples")

    def test_input_capture_and_owned_cleanup(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            cwd = root / "src" / "Compiler"
            cwd.mkdir(parents=True)
            for name in ("one.fs", "two.fs", "generated.fs", "FSharp.Core.dll", "r.dll", "res.bin"):
                (cwd / name).write_text(name, encoding="utf-8")
            arguments = ["-o:output.dll", "--optimize+", "--parallelcompilation-",
                         "-r:FSharp.Core.dll", "-r:r.dll", "--resource:res.bin,Logical",
                         "one.fs", "two.fs", "generated.fs"]
            captured = m.capture_inputs(arguments, cwd, root, {"src/Compiler/one.fs", "src/Compiler/two.fs"})
            self.assertEqual([Path(x["path"]).name for x in captured["sources"]], ["one.fs", "two.fs"])
            self.assertEqual([Path(x["path"]).name for x in captured["generated"]], ["generated.fs"])
            self.assertEqual([Path(x["path"]).name for x in captured["references"]], ["FSharp.Core.dll", "r.dll"])
            (cwd / "generated.fs").unlink()
            with self.assertRaises((ValueError, FileNotFoundError)):
                m.capture_inputs(arguments, cwd, root, set())
            output = cwd / "output.dll"
            output.write_text("old output")
            m.reset_outputs([str(output)], root, captured["sources"])
            self.assertFalse(output.exists())
            for path in (cwd / "one.fs", root.parent / "unowned-output.dll", cwd):
                with self.subTest(path=path), self.assertRaises(ValueError):
                    m.reset_outputs([str(path)], root, captured["sources"])

    def test_local_core_package_can_have_different_runtime_assets(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            package = root / "FSharp.Core.synthetic.nupkg"
            with zipfile.ZipFile(package, "w") as archive:
                archive.writestr("lib/netstandard2.0/FSharp.Core.dll", b"runtime Core")
                archive.writestr("lib/net11.0/FSharp.Core.dll", b"reference Core")
            core = root / "FSharp.Core.dll"
            for content, asset in ((b"reference Core", "lib/net11.0/FSharp.Core.dll"),
                                   (b"runtime Core", "lib/netstandard2.0/FSharp.Core.dll"),
                                   (b"published Core", None)):
                with self.subTest(content=content):
                    core.write_bytes(content)
                    if asset is None:
                        with self.assertRaises(ValueError):
                            m.packed_core_asset(m.file_record(core), package)
                    else:
                        self.assertEqual(m.packed_core_asset(m.file_record(core), package), asset)

    def test_shipping_core_selection_matches_local_compiler_helper(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for variant in ("Shipping", "Release", "PreRelease"):
                folder = root / "Dependency" / variant
                folder.mkdir(parents=True)
                (folder / "FSharp.Core.11.0.101-dev.nupkg").touch()
            shipping = root / "Dependency" / "Shipping"
            (shipping / "FSharp.Core.11.0.101-dev.symbols.nupkg").touch()
            self.assertEqual(m.local_core_package(root), shipping / "FSharp.Core.11.0.101-dev.nupkg")
            (shipping / "FSharp.Core.11.0.102-dev.nupkg").touch()
            with self.assertRaises(ValueError):
                m.local_core_package(root)

    def test_process_arguments_environment_and_failure(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            script = root / "fixture.py"
            script.write_text(
                "import json, os, sys\n"
                "print(json.dumps([sys.argv[1:], os.getcwd(), os.environ.get('CORECLR_ENABLE_PROFILING')]))\n"
                "sys.exit(int(sys.argv[1]))\n", encoding="utf-8")
            injected = {"CORECLR_ENABLE_PROFILING": "1", "DOTNET_STARTUP_HOOKS": "untrusted",
                        "MSBuildSDKsPath": "untrusted", "NUGET_PACKAGES": "shared-cache"}
            with mock.patch.dict(os.environ, injected):
                environment = m.allowed_environment(root)
            for key in ("CORECLR_ENABLE_PROFILING", "DOTNET_STARTUP_HOOKS", "MSBuildSDKsPath"):
                self.assertNotIn(key, environment)
            self.assertTrue(Path(environment["NUGET_PACKAGES"]).is_relative_to(root.resolve()))
            if os.name == "nt":
                self.assertEqual(environment["OS"], "Windows_NT")
                self.assertTrue(environment["APPDATA"])
                self.assertTrue(environment["LOCALAPPDATA"])
            for status in (0, 7):
                with self.subTest(status=status):
                    result = m.run_process([sys.executable, str(script), str(status), "space ; & argument"],
                                           root, environment, root / f"{status}.log")
                    args, cwd, profiler = json.loads((root / f"{status}.log").read_text())
                    self.assertEqual(args, [str(status), "space ; & argument"])
                    self.assertEqual(Path(cwd), root)
                    self.assertIsNone(profiler)
                    self.assertEqual(result["exit_status"], status)
                    self.assertEqual(result["complete"], status == 0)
                    self.assertGreater(result["elapsed_ms"], 0)

    def test_capture_target_and_cli(self):
        target = ET.parse(MODULE.with_name("Capture.targets")).getroot()
        capture = target.find("Target[@Name='CaptureCompilerPerf']")
        self.assertEqual(capture.get("AfterTargets"), "CoreCompile")
        self.assertTrue(any("@(FscCommandLineArgs)" in x.get("Lines", "")
                            for x in capture.findall("WriteLinesToFile")))
        with tempfile.TemporaryDirectory() as directory:
            for command in ("prepare", "measure", "compare"):
                with self.subTest(command=command):
                    result = subprocess.run([sys.executable, str(MODULE), command, "--help"],
                                            cwd=ROOT, env=m.allowed_environment(directory),
                                            capture_output=True, text=True)
                    self.assertEqual(result.returncode, 0, result.stderr)
                    self.assertIn("--output", result.stdout)

    def test_compare_cli_writes_decision_or_explicit_error(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for valid in (True, False):
                with self.subTest(valid=valid):
                    for arm, doc in zip(("baseline", "candidate"), measurements()):
                        m.write_json(root / f"{arm}.json", doc)
                    if not valid:
                        (root / "candidate.json").write_text('{"schema_version":')
                    output = root / f"comparison-{valid}.json"
                    result = subprocess.run(
                        [sys.executable, str(MODULE), "compare", "--baseline", str(root / "baseline.json"),
                         "--candidate", str(root / "candidate.json"), "--output", str(output)],
                        cwd=ROOT, env=m.allowed_environment(root), capture_output=True, text=True)
                    if valid:
                        self.assertEqual(result.returncode, 0, result.stderr)
                        decision = m.read_json(output)
                        self.assertEqual(decision["schema_version"], 1)
                        self.assertEqual(decision["decision"], "measured_improvement")
                    else:
                        self.assertEqual(result.returncode, 2)
                        self.assertIn("error:", result.stderr)
                        self.assertFalse(output.exists())

    def test_unsupported_live_platform_is_explicit(self):
        for platform_name in ("win32", "darwin"):
            with self.subTest(platform=platform_name), \
                 mock.patch.object(m, "sys", SimpleNamespace(platform=platform_name)):
                with self.assertRaisesRegex(ValueError, "requires Linux"):
                    m.run_repetitions({}, {}, "unused.json")

    def test_collection_replays_and_persists_failures(self):
        """Real child processes; synthetic compiler files, identities and timings."""
        for outcome in ("success", "failure", "no output", "cancelled", "changed input"):
            with self.subTest(outcome=outcome), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                cwd = root / "src" / "Compiler"
                cwd.mkdir(parents=True)
                for name in ("source.fs", "generated.fs", "FSharp.Core.dll"):
                    (cwd / name).write_text(name)
                compiler_dir = root / "compiler"
                compiler_dir.mkdir()
                for name in ("fsc.dll", "FSharp.Compiler.Service.dll", "fsc.runtimeconfig.json",
                             "fsc.deps.json", "FSharp.Core.dll"):
                    (compiler_dir / name).write_text("synthetic binary")
                write_output = "" if outcome == "no output" else "Path('output.dll').write_text('synthetic output')"
                (compiler_dir / "fsc.dll").write_text(
                    "from pathlib import Path\n"
                    "import sys\n"
                    "assert sys.argv[1].startswith('@')\n"
                    "assert not Path('output.dll').exists()\n"
                    f"{write_output}\n"
                    f"sys.exit({7 if outcome == 'failure' else 0})\n")
                arguments = ["-o:output.dll", "--optimize+", "--parallelcompilation-",
                             "-r:FSharp.Core.dll", "source.fs", "generated.fs"]
                response = root / "compile.rsp"
                response.write_text("\n".join(arguments) + "\n")
                inputs = m.capture_inputs(arguments, cwd, root, {"src/Compiler/source.fs"})
                environment = m.allowed_environment(root)
                bootstrap = {"revision": "a" * 40, "binaries": [], "commands": []}
                toolchain = {"dotnet": m.file_record(sys.executable),
                             "measurement": m.file_record(MODULE),
                             "capture_target": m.file_record(MODULE.with_name("Capture.targets")),
                             "runtime_files": [], "sdk_compiler_files": [],
                             "compiler_runtimeconfig_sha256": m.file_record(compiler_dir / "fsc.runtimeconfig.json")["sha256"]}
                compiler = m.compiler_manifest(compiler_dir, "a" * 40, "b" * 64, bootstrap, toolchain)
                workload = dict(inputs, revision="a" * 40, target=m.PROJECT.as_posix(),
                                configuration="Release", arguments=arguments,
                                arguments_sha256=m.fingerprint_inputs(arguments),
                                response_file=m.file_record(response), core_package=m.file_record(cwd / "FSharp.Core.dll"),
                                cwd=str(cwd), checkout=str(root))
                manifest = {"schema_version": 1, "kind": "workload", "owned_root": str(root),
                            "baseline_compiler": compiler,
                            "invariants": {"workload": workload, "bootstrap": bootstrap, "toolchain": toolchain,
                                           "environment": m.host_identity(environment), "policy": copy.deepcopy(m.POLICY),
                                           "experiment": {"id": "synthetic", "primary_metric": "elapsed_ms",
                                                          "collector": "process-rusage-v1", "seed": 910}}}
                original_run = m.run_process
                calls = []

                def controlled_run(args, working, env, log):
                    calls.append(args)
                    if outcome == "cancelled" and len(calls) == 3:
                        raise KeyboardInterrupt()
                    with mock.patch.object(m, "sys", sys):
                        result = original_run(args, working, env, log)
                    if outcome == "changed input":
                        (cwd / "source.fs").write_text("changed during replay")
                    result.update(elapsed_ms=100, process_cpu_ms=80, process_peak_rss_bytes=1000,
                                  unavailable_reasons={})
                    return result

                output = root / "samples.json"
                with mock.patch.object(m, "sys", SimpleNamespace(platform="linux")), \
                     mock.patch.object(m, "host_identity", return_value=manifest["invariants"]["environment"]), \
                     mock.patch.object(m, "run_process", side_effect=controlled_run):
                    if outcome != "success":
                        with self.assertRaises(KeyboardInterrupt if outcome == "cancelled" else ValueError):
                            m.run_repetitions(manifest, compiler, output)
                    else:
                        m.run_repetitions(manifest, compiler, output)
                docs = [m.read_json(output.with_suffix(".baseline.json")), m.read_json(output)]
                for doc in docs:
                    m.validate_manifest(doc, "samples")
                self.assertEqual(docs[0]["collection_id"], docs[1]["collection_id"])
                self.assertFalse((root / "measurement.lock").exists())
                self.assertTrue(all(args == [str(Path(sys.executable).resolve()),
                                            str((compiler_dir / "fsc.dll").resolve()),
                                            "@" + str(response.resolve())] for args in calls))
                if outcome == "success":
                    self.assertEqual(len(calls), 24)
                    self.assertEqual(m.compare_measurements(*docs)["decision"], "rejected")
                else:
                    self.assertNotEqual(m.compare_measurements(*docs)["decision"], "measured_improvement")


if __name__ == "__main__":
    unittest.main()
