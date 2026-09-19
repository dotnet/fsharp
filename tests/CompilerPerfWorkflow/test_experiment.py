"""Controller fixtures use synthetic timings, never evidence of a compiler speedup."""

import copy
from contextlib import ExitStack, nullcontext
import hashlib
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import unittest
from unittest import mock
import zipfile

from test_measurement import measurements, refresh_identities
from test_collection import artifact_profile
import memory as mem
import experiment as e


SOURCE = "src/Compiler/Checking/Test.fs"
TEST = "tests/FSharp.Compiler.ComponentTests/Conformance/Regression.fs"
NOTE = "docs/release-notes/.FSharp.Compiler.Service/11.0.100.md"


class Repository:
    def __init__(self, root):
        self.root = root
        root.mkdir(parents=True)
        self.git("init", "-q")
        for name, content in ((SOURCE, "module Test\nlet answer = 0\n"),
                              (TEST, "module Regression\nlet regression () = 42\n"),
                              (NOTE, "# Changes\n"), (".gitignore", "artifacts/\ngenerated.fs\n*.dll\n"),
                              ("tests/projects/CompilerCompat/.gitkeep", ""),
                              (e.COMPONENT, "<Project>\n  <ItemGroup>\n  </ItemGroup>\n</Project>\n"),
                              ("global.json", '{"sdk":{"version":"11.0.100-synthetic"}}')):
            path = root / name
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text(content)
        self.git("add", ".")
        self.git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid",
                 "commit", "-qm", "base")
        self.sha = self.git("rev-parse", "HEAD").decode().strip()

    def git(self, *args):
        return subprocess.check_output(["git", "-c", "core.autocrlf=false", *args],
                                       cwd=self.root, stderr=subprocess.PIPE,
                                       env=mem.m.allowed_environment(self.root / "artifacts"))

    def patch(self, extra=None):
        changes = {SOURCE: "module Test\nlet answer = 42\n",
                   TEST: "module Regression\nlet regression () = assert (Test.answer = 42)\n",
                   NOTE: "# Changes\n- Avoid repeated work.\n"}
        changes.update(extra or {})
        for name, content in changes.items():
            path = self.root / name
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text(content)
        self.git("add", ".")
        patch = self.git("diff", "--cached", "--binary")
        self.git("restore", "--source=HEAD", "--staged", "--worktree", ".")
        return patch

    def request(self):
        return {"schema_version": 1, "attempt_id": "attempt-1", "run_id": "run-1",
                "base_sha": self.sha, "compiler_paths": [SOURCE],
                "regressions": [{"path": TEST, "method": "regression"}],
                "hypothesis": "Avoid repeated work", "primary_metric": "elapsed_ms",
                "complexity": "One expression, no new mechanism"}


class ExperimentTests(unittest.TestCase):
    def test_isolated_cli_imports_and_historical_correctness_schema(self):
        from test_memory import improvement, state

        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            memory = root / "memory.json"
            mem.m.write_json(memory, state(improvement()))
            for script, args, expected in (
                    ("experiment.py", ["--help"], "prepare"),
                    ("memory.py", ["summarize", "--input", str(memory)], "measured_improvement")):
                with self.subTest(script=script):
                    result = subprocess.run([sys.executable, "-I", str(mem.m.HERE / script), *args],
                                            cwd=root, env=mem.m.allowed_environment(root),
                                            capture_output=True, text=True, timeout=30)
                    self.assertEqual(result.returncode, 0, result.stderr)
                    self.assertIn(expected, result.stdout)

    def test_actual_git_diff_and_owned_discard(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            before = repo.git("status", "--porcelain")
            sentinel = root / "sentinel"
            sentinel.write_text("unrelated")
            candidate = e.prepare_candidate(repo.root, root / "owned", repo.request(), repo.patch())
            self.assertEqual(candidate["changed_paths"], sorted([SOURCE, TEST, NOTE]))
            self.assertEqual(candidate["patch_sha256"], hashlib.sha256(repo.patch()).hexdigest())
            self.assertFalse(candidate["draft_eligible"])
            e.discard_candidate(root / "owned")
            self.assertFalse((root / "owned" / "tree").exists())
            self.assertTrue((root / "owned" / "candidate.json").is_file())
            self.assertEqual(repo.git("status", "--porcelain"), before)
            self.assertEqual(sentinel.read_text(), "unrelated")

    def test_allowlist_attacks_are_rejected_after_git_application(self):
        attacks = [".github/workflows/test.yml", "eng/scripts/compiler_perf/memory.py",
                   "global.json", "Directory.Build.props", "NuGet.Config",
                   "src/FSharp.Core/prim-types.fs", "src/Compiler/Service/Other.fs",
                   "tests/FSharp.Test.Utilities/Compiler.fs",
                   "tests/FSharp.Compiler.ComponentTests/Directory.Build.targets",
                   "docs/release-notes/.FSharp.Compiler.Service/10.0.100.md",
                   "src/Compiler/Checking/evil.targets", ".gitmodules", ".gitattributes"]
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            for index, name in enumerate(attacks):
                with self.subTest(path=name), self.assertRaises(ValueError):
                    e.prepare_candidate(repo.root, root / f"owned-{index}", repo.request(),
                                        repo.patch({name: "untrusted\n"}))

    def test_patch_modes_binary_rename_and_traversal(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            patch = repo.patch()
            attacks = [
                patch + b"\ndiff --git a/x b/x\nnew file mode 120000\n",
                patch + b"\nGIT binary patch\nliteral 1\nA\n",
                patch.replace(b"b/src/Compiler/Checking/Test.fs", b"b/../escape.fs"),
                patch.replace(b"b/src/Compiler/Checking/Test.fs", b"b/C:/escape.fs"),
                patch.replace(b"100644", b"100755"),
                patch + b"\ndiff --git a/x b/x\nnew file mode 160000\n",
                patch + b"\ndiff --git a/global.json b/src/Compiler/Checking/New.fs\n"
                        b"similarity index 100%\nrename from global.json\n"
                        b"rename to src/Compiler/Checking/New.fs\n",
            ]
            for index, bad in enumerate(attacks):
                with self.subTest(index=index), self.assertRaises(ValueError):
                    e.prepare_candidate(repo.root, root / f"owned-{index}", repo.request(), bad)

    def test_registration_only_adds_the_related_compile_item(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            for index, line in enumerate((
                    '<Compile Include="Conformance\\Regression.fs" />',
                    '<Compile Include="Conformance\\Other.fs" />',
                    '<Compile Include="Conformance\\Regression.fs" Condition="true" />',
                    '<Compile Include="..\\FSharp.Test.Utilities\\Compiler.fs" />',
                    '<Target Name="Injected" BeforeTargets="Build" />',
                    '<PropertyGroup><FscToolExe>injected</FscToolExe></PropertyGroup>')):
                project = "<Project>\n  <ItemGroup>\n    " + line + "\n  </ItemGroup>\n</Project>\n"
                patch = repo.patch({e.COMPONENT: project})
                with self.subTest(line=line):
                    if index == 0:
                        state = e.prepare_candidate(repo.root, root / str(index), repo.request(), patch)
                        self.assertIn(e.COMPONENT, state["changed_paths"])
                        e.discard_candidate(root / str(index))
                    else:
                        with self.assertRaises(ValueError):
                            e.prepare_candidate(repo.root, root / str(index), repo.request(), patch)

    def test_bounded_data_not_commands(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            for key, value in (("command", ["cmd", "/c", "exit 0"]),
                               ("base_sha", "HEAD"), ("primary_metric", "sampled_allocation"),
                               ("regressions", []), ("compiler_paths", ["../escape"]),
                               ("hypothesis", "x" * 4097)):
                request = repo.request()
                request[key] = value
                with self.subTest(key=key), self.assertRaises(ValueError):
                    e.validate_request(request)

    def test_materialized_workload_bootstrap_cache_and_runtime_drift(self):
        for part in ("source", "generated", "response", "bootstrap", "package", "compiler", "profiling", "sdk-addition"):
            with self.subTest(part=part), tempfile.TemporaryDirectory() as directory:
                root = Path(directory).resolve()
                repo = Repository(root / "frozen" / "workload")
                manifest = frozen_fixture(root / "frozen", repo)
                e.frozen_records(manifest)
                invariants = manifest["invariants"]
                records = {"source": invariants["workload"]["sources"][0],
                           "generated": invariants["workload"]["generated"][0],
                           "response": invariants["workload"]["response_file"],
                           "bootstrap": invariants["bootstrap"]["binaries"][0],
                           "package": invariants["toolchain"]["build_inputs"]["packages"][0],
                           "compiler": manifest["baseline_compiler"]["files"]["fsc.dll"]}
                if part == "profiling":
                    invariants["environment"]["variables"]["DOTNET_EnableDiagnostics"] = "1"
                elif part == "sdk-addition":
                    (Path(invariants["toolchain"]["dotnet"]["path"]).parent / "sdk" / "injected.targets").write_text("changed")
                else:
                    Path(records[part]["path"]).write_text("changed owned fixture")
                with self.assertRaises(ValueError):
                    e.frozen_records(manifest)

    def test_conservative_area_plans_and_explicit_projects(self):
        for area in ("Checking", "CodeGen", "Optimize", "Service", "TypedTree", "FutureArea"):
            with self.subTest(area=area):
                plan = e.correctness_plan([f"src/Compiler/{area}/Test.fs", TEST, NOTE],
                                         [{"path": TEST, "method": "regression"}], "net11.0")
                categories = {check["category"] for check in plan}
                self.assertTrue({"compiler-build", "test-build", "regression", "siblings",
                                 "runtime", "compatibility"} <= categories)
                if area in ("CodeGen", "Optimize", "FutureArea"):
                    self.assertTrue({"emitted-il", "optimizer"} <= categories)
                if area in ("Service", "TypedTree", "FutureArea"):
                    self.assertIn("service", categories)
                for check in plan:
                    argv = check["argv"]
                    if argv[0] == "test":
                        self.assertIn("--project", argv)
                        self.assertIn("--no-build", argv)
                        self.assertIn("--minimum-expected-tests", argv)
                    self.assertNotEqual(argv, ["test", "-c", "Release"])

    def test_fresh_result_requires_matched_passes(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "result.xml"
            for total, passed, failed, skipped, accepted in (
                    (1, 1, 0, 0, True), (0, 0, 0, 0, False),
                    (2, 1, 1, 0, False), (2, 1, 0, 1, False)):
                path.write_text(f'<assemblies><assembly total="{total}" passed="{passed}" '
                                f'failed="{failed}" skipped="{skipped}" errors="0" /></assemblies>')
                with self.subTest(total=total, failed=failed, skipped=skipped):
                    if accepted:
                        self.assertEqual(e.test_counts(path)["matched"], 1)
                    else:
                        with self.assertRaises(ValueError):
                            e.test_counts(path)

    def test_platform_exclusions_do_not_excuse_required_or_unknown_skips(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "results.xml"
            for reason, applicable, allowed in (
                    ("This test is only supported on Windows", 1, True),
                    ("NETCOREAPP is not supported runtime for this kind of test, it is intended for DESKTOP only", 1, True),
                    ("Required regression was skipped", 1, False),
                    ("This test is only supported on Windows", 0, False)):
                path.write_text(
                    f'<assemblies><assembly total="{applicable + 1}" passed="{applicable}" failed="0" skipped="1" errors="0">'
                    f'<collection><test name="NonApplicable" result="Skip"><reason><message>{reason}</message></reason>'
                    '</test></collection></assembly></assemblies>')
                with self.subTest(reason=reason, applicable=applicable):
                    with self.assertRaises(ValueError):
                        e.test_counts(path)
                    if allowed:
                        result = e.test_counts(path, platform_exclusions=True)
                        self.assertEqual(result["matched"], 1)
                        self.assertEqual(result["excluded"], [{"name": "NonApplicable", "reason": reason}])
                    else:
                        with self.assertRaises(ValueError):
                            e.test_counts(path, platform_exclusions=True)
                    path.write_text(path.read_text().replace("<message>", "").replace("</message>", ""))
                    if allowed:
                        self.assertEqual(e.test_counts(path, platform_exclusions=True)["matched"], 1)
                    else:
                        with self.assertRaises(ValueError):
                            e.test_counts(path, platform_exclusions=True)

    def test_regression_match_is_bound_to_its_source_and_method(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            path = root / "results.xml"
            source = str(root / "Regression.fs")
            required = {"path": source, "method": "regression"}
            for actual_source, method, accepted in (
                    (source, "regression", True), (source, "unrelated", False),
                    (str(root / "Unrelated.fs"), "regression", False), ("", "regression", False)):
                path.write_text('<assemblies><assembly total="1" passed="1" failed="0" skipped="0" errors="0">'
                                f'<collection><test result="Pass" method="{method}" source-file="{actual_source}" />'
                                '</collection></assembly></assemblies>')
                with self.subTest(source=actual_source, method=method):
                    if accepted:
                        self.assertEqual(e.test_counts(path, required_test=required)["matched"], 1)
                    else:
                        with self.assertRaises(ValueError):
                            e.test_counts(path, required_test=required)

    def test_independent_comparison_and_post_approval_tampering(self):
        for candidate_ms, expected in ((90, "measured_improvement"), (100, "rejected")):
            documents = measurements(candidate_ms)
            self.assertEqual(e.evaluate_candidate(*documents)["decision"], expected)
        for mutate in (lambda d: d[1]["samples"][0].update(profiler_attached=True),
                       lambda d: d[1].update(collection_complete=False),
                       lambda d: d[1]["invariants"]["toolchain"].update(sdk="other"),
                       lambda d: d[1]["invariants"]["workload"].update(arguments_sha256="a" * 64),
                       lambda d: d[1]["invariants"]["policy"].update(pairs=1)):
            documents = measurements()
            mutate(documents)
            with self.subTest(mutate=mutate):
                try:
                    result = e.evaluate_candidate(*documents)
                except ValueError:
                    continue
                self.assertNotEqual(result["decision"], "measured_improvement")
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            e.prepare_candidate(repo.root, root / "owned", repo.request(), repo.patch())
            (root / "owned" / "tree" / SOURCE).write_text("changed after approval")
            with self.assertRaises(ValueError):
                e.verify_candidate(root / "owned")

    def test_unknown_lock_owner_preserved(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            e.prepare_candidate(repo.root, root / "owned", repo.request(), repo.patch())
            lock = root / "owned" / "controller.lock"
            lock.mkdir()
            (lock / "owner.json").write_text('{"pid":1,"start_time":"unproven"}')
            with self.assertRaises(ValueError):
                e.discard_candidate(root / "owned")
            self.assertTrue(lock.is_dir())

    @unittest.skipUnless(sys.platform == "linux" and shutil.which("bwrap"),
                         "Live isolation fixture requires Linux bubblewrap")
    def test_real_isolated_red_green_and_rollback(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "repo")
            sentinel = root / "authority.json"
            sentinel.write_text("trusted")
            for success in (False, True):
                owned = root / str(success)
                e.prepare_candidate(repo.root, owned, repo.request(), repo.patch())
                tree = owned / "tree"
                fixture = tree / "artifacts"
                fixture.mkdir()
                credential = root / "credential"
                credential.write_text("isolated fake credential")
                runner = e.Isolation(tree, [Path(sys.executable).resolve().parent.parent, sentinel])
                script = f"""import os, pathlib
assert not pathlib.Path({str(credential)!r}).exists()
assert not any(k in os.environ for k in ('GITHUB_TOKEN', 'GITHUB_OUTPUT', 'GITHUB_ENV'))
for path in (pathlib.Path({str(sentinel)!r}), pathlib.Path({SOURCE!r})):
    try:
        path.write_text('malicious overwrite')
    except OSError:
        pass
    else:
        raise AssertionError('trusted input was writable')
assert '42' in pathlib.Path({SOURCE!r}).read_text()
assert pathlib.Path('artifacts/guard').read_text() == 'green'
"""
                (fixture / "guard").write_text("red")
                with mock.patch.dict(os.environ, {"GITHUB_TOKEN": "fake-private-token",
                                                  "GITHUB_OUTPUT": str(sentinel), "GITHUB_ENV": str(sentinel)}):
                    environment = mem.m.allowed_environment(fixture)
                red = runner.command([sys.executable, "-c", script], tree, environment, owned, "red")
                self.assertNotEqual(red["exit_status"], 0)
                (fixture / "guard").write_text("green" if success else "red")
                green = runner.command([sys.executable, "-c", script], tree, environment, owned, "green")
                self.assertEqual(green["exit_status"] == 0, success, green)
                self.assertEqual(e.evaluate_candidate(*measurements())["decision"], "measured_improvement")
                if not success:
                    e.discard_candidate(owned)
                    self.assertFalse(tree.exists())
                else:
                    e.verify_candidate(owned)
                    self.assertTrue(tree.exists())
                self.assertEqual(sentinel.read_text(), "trusted")
                self.assertEqual(repo.git("status", "--porcelain"), b"")

    @unittest.skipUnless(sys.platform == "linux" and shutil.which("bwrap"),
                         "Live controller fixture requires Linux bubblewrap")
    def test_real_controller_retains_only_independently_approved_fixture(self):
        for failure in ("", "test", "fsi", "identical", "noise", "no-finding", "missing-samples", "altered-samples", "cancel-setup",
                        "cancel-build", "cancel-correctness", "cancel-collection", "cancel-measurement",
                        "cancel-approval", "cancel-child", "checkpoint"):
            with self.subTest(failure=failure), tempfile.TemporaryDirectory() as directory, ExitStack() as lifetime:
                root = Path(directory)
                keepalive = e.c.Command([sys.executable, "-c", "import time; time.sleep(600)"],
                                       root, mem.m.allowed_environment(root / "sentinel-owned"), root, "sentinel")
                lifetime.callback(keepalive.finish)
                frozen = root / "frozen"
                repo = Repository(frozen / "workload")
                manifest = frozen_fixture(frozen, repo)
                owned = root / "candidate"
                e.prepare_candidate(repo.root, owned, repo.request(), repo.patch(), manifest=manifest)
                tree = owned / "tree"
                artifacts = tree / "artifacts"
                artifacts.mkdir()
                shutil.copyfile(Path(__file__).parent / "fixtures" / "ExperimentTool.py", artifacts / "tool.py")
                (artifacts / "fail").write_text(failure if failure in ("test", "fsi") else "")
                evidence = artifact_profile(root)
                evidence.update(compiler=manifest["baseline_compiler"],
                                fingerprints={k: mem.m.fingerprint_inputs(v) for k, v in manifest["invariants"].items()})
                mem.m.write_json(root / "profile.json", evidence)
                sentinel = root / "authority"
                sentinel.write_text("unchanged")

                class FixtureIsolation(e.Isolation):
                    def command(self, args, cwd, environment, receipts, phase, timeout=1800):
                        # Only toolchain executables are substituted; isolation, receipts,
                        # Git ownership, plan execution, reducers and approval are real.
                        arguments = ["pack"] if args[0] == "bash" else args[1:]
                        interruption = (mock.patch.object(e.c.Command, "check", side_effect=KeyboardInterrupt)
                                        if failure == "cancel-child" and phase == "regression-0" else nullcontext())
                        with interruption:
                            result = super().command([sys.executable, str(artifacts / "tool.py"), *arguments],
                                                     cwd, environment, receipts, phase, timeout)
                        result["command"] = args
                        return result

                def synthetic_samples(manifest, compiler, output, **unused):
                    documents = measurements(100 if failure == "identical" else 90)
                    for arm, doc in enumerate(documents):
                        doc["invariants"] = copy.deepcopy(manifest["invariants"])
                        doc["compiler"] = compiler if arm else manifest["baseline_compiler"]
                    refresh_identities(documents)
                    if failure == "noise":
                        for sample in documents[0]["samples"]:
                            if sample["phase"] == "aa":
                                sample["elapsed_ms"] = 80 + 10 * sample["pair"]
                    if failure != "missing-samples":
                        mem.m.write_json(Path(output).with_suffix(".baseline.json"), documents[0])
                        saved = copy.deepcopy(documents[1])
                        if failure == "altered-samples":
                            saved["samples"][0]["elapsed_ms"] = 1
                        mem.m.write_json(output, saved)
                    return dict(zip(("baseline", "candidate"), documents))

                with ExitStack() as patches:
                    patches.enter_context(mock.patch.object(e, "Isolation", FixtureIsolation))
                    patches.enter_context(mock.patch.object(e.m, "run_repetitions", side_effect=synthetic_samples))
                    cancellation = {"cancel-setup": (e, "frozen_records"), "cancel-build": (e, "build_candidate"),
                                    "cancel-correctness": (e, "run_correctness"),
                                    "cancel-collection": (e.c, "analyze_profile"),
                                    "cancel-measurement": (e.m, "run_repetitions"),
                                    "cancel-approval": (mem, "_attest_correctness"), "checkpoint": (e.m, "run_repetitions")}
                    if failure in cancellation or failure == "cancel-child":
                        with ExitStack() as interruption:
                            if failure in cancellation:
                                module, name = cancellation[failure]
                                interruption.enter_context(mock.patch.object(module, name, side_effect=KeyboardInterrupt))
                            with self.assertRaises(KeyboardInterrupt):
                                e.validate_candidate(owned, root / "profile.json",
                                                     checkpoint_on_cancel=failure == "checkpoint")
                        result = mem.m.read_json(owned / "candidate.json")
                        self.assertEqual(result["outcome"], "cancelled")
                        if failure == "checkpoint":
                            self.assertTrue(tree.exists())
                            self.assertIn("checkpoint", result)
                            result = e.validate_candidate(owned, root / "profile.json")
                    elif failure == "no-finding":
                        result = e.complete_no_finding(owned, root / "profile.json")
                    else:
                        result = e.validate_candidate(owned, root / "profile.json")
                eligible = failure in ("", "checkpoint")
                expected = ("measured_improvement" if eligible else "inconclusive" if failure == "noise" else
                            "completed_no_finding" if failure == "no-finding" else
                            "blocked_collection" if failure in ("missing-samples", "altered-samples") else
                            "cancelled" if failure.startswith("cancel-") else "rejected")
                self.assertEqual(result["draft_eligible"], eligible, result)
                self.assertEqual(result["outcome"], expected, result)
                self.assertEqual(tree.exists(), eligible)
                journal = mem.load_state(owned / "attempt.json")
                self.assertEqual(journal["status"], "ready", journal)
                self.assertEqual(journal["state"]["attempts"]["attempt-1"]["outcome"], expected)
                self.assertEqual(sentinel.read_text(), "unchanged")
                self.assertIsNone(keepalive.process.poll(), "Cancellation killed the unrelated owned sentinel")
                self.assertEqual(repo.git("status", "--porcelain", "--untracked-files=no"), b"")
                if eligible:
                    journal = mem.m.read_json(owned / "attempt.json")
                    mem.acceptance_ready(journal["attempts"]["attempt-1"])
                    with self.assertRaises(ValueError):
                        e.discard_candidate(owned)
                    (tree / SOURCE).write_text("tampered")
                    with self.assertRaises(ValueError):
                        e.verify_candidate(owned)
                    self.assertFalse(mem.m.read_json(owned / "candidate.json")["draft_eligible"])


def frozen_fixture(root, repo):
    """Materialize the existing synthetic measurement contract, not a speedup."""
    m = mem.m
    invariants = measurements()[0]["invariants"]
    cwd = repo.root / "src" / "Compiler"
    core = cwd / "FSharp.Core.dll"
    core.write_text("core")
    (cwd / "generated.fs").write_text("module Generated")
    args = ["-o:output.dll", "--optimize+", "--noframework", "--parallelcompilation-",
            "-r:" + str(core), "Checking/Test.fs", "generated.fs"]
    inputs = m.capture_inputs(args, cwd, repo.root, {SOURCE})
    response = root / "replay.rsp"
    response.write_text("\n".join(args) + "\n")
    environment = m.allowed_environment(root)
    package = root / "packages" / "core.nupkg"
    with zipfile.ZipFile(package, "w") as archive:
        archive.writestr("lib/netstandard2.0/FSharp.Core.dll", b"core")
    workload = dict(inputs, revision=repo.sha, default_branch_revision=repo.sha, configuration="Release",
                    target=m.PROJECT.as_posix(), target_framework="net11.0", parallel_compilation=False,
                    cwd=str(cwd), checkout=str(repo.root), arguments=args, arguments_sha256=m.fingerprint_inputs(args),
                    response_file=m.file_record(response), core_package=m.file_record(package),
                    core_package_asset="lib/netstandard2.0/FSharp.Core.dll")
    bootstrap = root / "bootstrap"
    boot = bootstrap / "artifacts" / "Bootstrap" / "fsc" / "fsc.dll"
    boot.parent.mkdir(parents=True)
    boot.write_text("bootstrap")
    provenance = {"revision": repo.sha, "source_tree": repo.git("rev-parse", "HEAD^{tree}").decode().strip(),
                  "binaries": [m.file_record(boot)],
                  "commands": [{"arguments": ["bash", "build.sh", "-c", "Release", "--pack"],
                                "cwd": str(bootstrap), "exit_status": 0, "log": str(root / "bootstrap.log")}]}
    toolchain = invariants["toolchain"]
    tool_root = root / "toolchain"
    sdk_file = tool_root / "sdk" / "11.0.100-synthetic" / "FSharp" / "fsc.dll"
    runtime_file = tool_root / "shared" / "Microsoft.NETCore.App" / "11.0.0-synthetic" / "runtime.dll"
    for path in (sdk_file, runtime_file):
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text("synthetic toolchain")
    tool = tool_root / "dotnet"
    shutil.copyfile(sys.executable, tool)
    toolchain.update(dotnet=m.file_record(tool), measurement=m.file_record(m.__file__),
                     capture_target=m.file_record(e.c.HERE / "Capture.targets"),
                     sdk="11.0.100-synthetic", runtime="synthetic runtime",
                     sdks="9.0.300 [synthetic]\n11.0.100-synthetic [synthetic]",
                     runtime_files=[m.file_record(runtime_file)], sdk_compiler_files=[m.file_record(sdk_file)],
                     build_inputs={"packages": [m.file_record(package)], "sdk": [m.file_record(sdk_file)],
                                   "bootstrap": [m.file_record(boot)]})
    installation = bootstrap / "artifacts" / "bin" / "fsc" / "Release" / "net11.0"
    installation.mkdir(parents=True)
    for name, text in (("fsc.dll", "baseline"), ("FSharp.Compiler.Service.dll", "baseline"),
                       ("FSharp.Core.dll", "core"), ("fsc.runtimeconfig.json", "runtime"), ("fsc.deps.json", "{}")):
        (installation / name).write_text(text)
    toolchain["compiler_runtimeconfig_sha256"] = m.file_record(installation / "fsc.runtimeconfig.json")["sha256"]
    invariants.update(workload=workload, bootstrap=provenance, environment=m.host_identity(environment))
    compiler = m.compiler_manifest(installation, repo.sha, hashlib.sha256(b"").hexdigest(), provenance, toolchain)
    return {"schema_version": 1, "kind": "workload", "owned_root": str(root),
            "baseline_compiler": compiler, "invariants": invariants}


if __name__ == "__main__":
    unittest.main()
