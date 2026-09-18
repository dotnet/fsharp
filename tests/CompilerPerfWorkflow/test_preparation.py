"""Controlled preparation commands and synthetic build artifacts, not measurements."""

import os
from pathlib import Path
import tempfile
import unittest
from unittest import mock
import zipfile

from test_measurement import m


class PreparationTests(unittest.TestCase):
    def test_preparation_capture_and_failures(self):
        outcomes = ("success", "sdk mismatch", "bootstrap failure", "capture failure", "wrong compiler",
                    "missing generated", "wrong Core", "dirty source", "replay failure", "no assembly")
        for outcome in outcomes:
            with self.subTest(outcome=outcome), tempfile.TemporaryDirectory(prefix="compiler perf ") as directory:
                base = Path(directory).resolve()
                repo, root = base / "repository", base / "experiment"
                dotnet = repo / ".dotnet" / ("dotnet.exe" if os.name == "nt" else "dotnet")
                sdk, tfm, sha = "11.0.100-synthetic", "net11.0", "a" * 40
                installation = root / "bootstrap" / "artifacts" / "bin" / "fsc" / "Release" / tfm
                cwd = root / "workload" / "src" / "Compiler"
                response = root / "capture" / "compiler.rsp"
                for path in (dotnet, dotnet.parent / "shared" / "Microsoft.NETCore.App" / "11.0.0" / "runtime.dll",
                             dotnet.parent / "sdk" / sdk / "FSharp" / "fsc.dll"):
                    path.parent.mkdir(parents=True, exist_ok=True)
                    path.write_text("synthetic binary")
                calls = []
                arguments = ["-o:output.dll", "--optimize+", "--noframework", "--parallelcompilation-",
                             "-r:FSharp.Core.dll", "-r:reference.dll", "--resource:resource.bin,Logical",
                             "first.fs", "second.fs", "generated.fs"]

                def run(args, working, environment, log):
                    calls.append((args, Path(working)))
                    self.assertIsInstance(args, list)
                    self.assertEqual(environment["NUGET_PACKAGES"], str(root / "packages"))
                    self.assertEqual(environment["DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER"], "1")
                    self.assertNotIn("CORECLR_ENABLE_PROFILING", environment)
                    text, status = "", 0
                    if args[:2] == ["git", "clone"]:
                        checkout = Path(args[-1])
                        checkout.mkdir()
                        m.write_json(checkout / "global.json", {"sdk": {"version": sdk}})
                    elif args[:2] == ["git", "checkout"]:
                        self.assertEqual(args[-1], sha)
                    elif args[:2] == ["git", "rev-parse"]:
                        text = sha
                    elif args[:2] == ["git", "merge-base"]:
                        self.assertEqual(args[3:], [sha, sha])
                    elif args[:2] == ["git", "ls-files"]:
                        text = "src/Compiler/first.fs\nsrc/Compiler/second.fs"
                    elif args[:2] == ["git", "status"]:
                        text = " M src/Compiler/first.fs" if outcome == "dirty source" else ""
                    elif "--version" in args:
                        text = "wrong-sdk" if outcome == "sdk mismatch" else sdk
                    elif "build.cmd" in args or "build.sh" in args:
                        self.assertEqual(Path(working), root / "bootstrap")
                        self.assertIn("Release", args)
                        self.assertTrue("-pack" in args or "--pack" in args)
                        status = 7 if outcome == "bootstrap failure" else 0
                        installation.mkdir(parents=True)
                        for name in ("fsc.dll", "FSharp.Compiler.Service.dll", "fsc.runtimeconfig.json",
                                     "fsc.deps.json", "FSharp.Core.dll", "FSharp.Build.dll"):
                            (installation / name).write_text("synthetic binary")
                        proto = root / "bootstrap" / "artifacts" / "bin" / "fsc" / "Proto" / "fsc.dll"
                        proto.parent.mkdir(parents=True)
                        proto.write_text("synthetic bootstrap")
                        package = root / "bootstrap" / "artifacts" / "packages" / "Release" / "Shipping" / "FSharp.Core.1.0.nupkg"
                        package.parent.mkdir(parents=True)
                        with zipfile.ZipFile(package, "w") as archive:
                            archive.writestr("lib/netstandard2.0/FSharp.Core.dll", b"synthetic Core")
                    elif "msbuild" in args:
                        text = tfm
                    elif "build" in args:
                        self.assertEqual(Path(working), root / "workload")
                        for flag in ("--disable-build-servers", "-p:ProvideCommandLineArgs=true",
                                     "-p:SkipCompilerExecution=false", "-p:ProduceReferenceAssembly=false"):
                            self.assertIn(flag, args)
                        self.assertEqual(environment["LocalFSharpCompilerPath"], str(root / "bootstrap"))
                        self.assertEqual(environment["RegressionLocalCoreVersion"], "1.0")
                        self.assertEqual(environment["CustomAfterDirectoryBuildTargets"], str(m.HERE / "Capture.targets"))
                        compiler = next(a.split("=", 1)[1] for a in args if a.startswith("-p:DotnetFscCompilerPath="))
                        compiler = compiler.replace("%22", '"')
                        self.assertEqual(compiler, f'"{installation / "fsc.dll"}"')
                        status = 7 if outcome == "capture failure" else 0
                        cwd.mkdir(parents=True)
                        for name in ("first.fs", "second.fs", "generated.fs", "reference.dll", "resource.bin"):
                            (cwd / name).write_text(name)
                        (cwd / "FSharp.Core.dll").write_bytes(b"wrong Core" if outcome == "wrong Core" else b"synthetic Core")
                        (cwd / "output.dll").write_text("capture output")
                        if outcome == "missing generated":
                            (cwd / "generated.fs").unlink()
                        response.write_text("\n".join(arguments) + "\n", encoding="utf-8")
                        identity = ["wrong compiler" if outcome == "wrong compiler" else compiler,
                                    "Release", str(root / "workload" / m.PROJECT), tfm]
                        response.with_name("identity.txt").write_text("\n".join(identity), encoding="utf-8")
                    elif args[1] == str(installation / "fsc.dll"):
                        self.assertEqual(args, [str(dotnet), str(installation / "fsc.dll"), "@" + str(response)])
                        self.assertEqual(Path(working), cwd)
                        self.assertFalse((cwd / "output.dll").exists())
                        self.assertEqual(response.read_text().splitlines(), arguments)
                        if outcome != "no assembly":
                            (cwd / "output.dll").write_text("synthetic replay output")
                        status = 7 if outcome == "replay failure" else 0
                    elif args[1] not in ("--list-runtimes", "--info"):
                        self.fail(f"Unexpected preparation command: {args}")
                    Path(log).write_text(text, encoding="utf-8")
                    return {"exit_status": status, "complete": status == 0}

                output = base / "manifest.json"
                with mock.patch.object(m, "REPO", repo), mock.patch.object(m, "run_process", side_effect=run):
                    if outcome != "success":
                        with self.assertRaises((ValueError, FileNotFoundError)):
                            m.prepare_workload(sha, root, output)
                        self.assertFalse(output.exists())
                        self.assertFalse((root / "baseline-compiler.json").exists())
                    else:
                        document = m.prepare_workload(sha, root, output)
                        self.assertEqual(document, m.read_json(output))
                        m.validate_manifest(document, "workload")
                        workload = document["invariants"]["workload"]
                        for key, names in (("sources", ["first.fs", "second.fs"]), ("generated", ["generated.fs"]),
                                           ("references", ["FSharp.Core.dll", "reference.dll"])):
                            self.assertEqual([Path(r["path"]).name for r in workload[key]], names)
                        self.assertEqual(workload["arguments"], arguments)
                        self.assertEqual(workload["revision"], sha)
                        self.assertEqual(workload["response_file"], m.file_record(response))
                        self.assertEqual(document["baseline_compiler"], m.read_json(root / "baseline-compiler.json"))
                        with self.assertRaisesRegex(ValueError, "must be a new"):
                            m.prepare_workload(sha, root, output)
                recorded = m.read_json(root / "preparation-commands.json")["commands"]
                self.assertEqual([entry["arguments"] for entry in recorded], [args for args, _ in calls])
                if outcome.endswith("failure"):
                    self.assertEqual(recorded[-1]["exit_status"], 7)


if __name__ == "__main__":
    unittest.main()
