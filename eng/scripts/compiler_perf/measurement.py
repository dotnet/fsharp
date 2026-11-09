"""Frozen compiler-source replay and paired decisions. Python 3 standard library only."""

import argparse
from fractions import Fraction
import hashlib
import importlib.util
import json
import math
import os
from pathlib import Path, PurePosixPath, PureWindowsPath
import platform
import random
import re
import shutil
import signal
import socket
import statistics
import subprocess
import sys
import time
import uuid
import zipfile


if os.name == "nt":
    job_spec = importlib.util.spec_from_file_location("windows_job", Path(__file__).with_name("windows_job.py"))
    job_module = importlib.util.module_from_spec(job_spec)
    job_spec.loader.exec_module(job_module)
    WindowsJob = job_module.WindowsJob


POLICY = {
    "pairs": 5, "control_pairs": 5, "warmups": 2, "seed": 910,
    "minimum_improvement_percent": 3, "maximum_aa_difference_percent": 1,
    "maximum_relative_mad_percent": 2, "maximum_time_regression_percent": 1,
    "bootstrap_resamples": 10000, "interval_percent": 95,
}
SECTIONS = ("workload", "bootstrap", "toolchain", "environment", "policy", "experiment")
METRICS = ("elapsed_ms", "process_cpu_ms", "process_peak_rss_bytes")
PRIMARY_METRICS = ("elapsed_ms", "process_peak_rss_bytes")
PROJECT = Path("src") / "Compiler" / "FSharp.Compiler.Service.fsproj"
HERE = Path(__file__).resolve().parent
REPO = HERE.parents[2]
LOCAL_ENVIRONMENT_KEYS = {
    "LoadLocalFSharpBuild", "LocalFSharpCompilerPath", "LocalFSharpCompilerConfiguration",
    "CustomAfterDirectoryBuildProps", "CustomAfterDirectoryBuildTargets", "RegressionLocalCore",
    "RegressionLocalCoreVersion", "RegressionLocalCorePackagesDir",
    "DOTNET_ROOT", "DOTNET_HOST_PATH", "DOTNET_INSTALL_DIR",
}
CACHE_POLICY = "restore-once; warm-dependencies-and-filesystem; no-page-cache-flush; fresh-process; no-build-server"
RUNNER_KEYS = ("ImageOS", "ImageVersion", "RUNNER_NAME", "RUNNER_OS", "RUNNER_ARCH",
               "GITHUB_RUN_ID", "GITHUB_RUN_ATTEMPT", "GITHUB_JOB")
INHERITED_ENVIRONMENT_KEYS = ("PATH", "SystemRoot", "COMSPEC", "PATHEXT", "SystemDrive",
                              "ProgramFiles", "ProgramFiles(x86)", "ProgramW6432", "ProgramData")
CONTROLLED_ENVIRONMENT = {
    "DOTNET_CLI_TELEMETRY_OPTOUT": "1", "DOTNET_NOLOGO": "1",
    "DOTNET_MULTILEVEL_LOOKUP": "0", "DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER": "1",
    "DOTNET_EnableDiagnostics": "0", "MSBUILDDISABLENODEREUSE": "1",
    "BUILDING_USING_DOTNET": "true", "LANG": "C.UTF-8", "LC_ALL": "C.UTF-8",
}
OWNED_ENVIRONMENT_KEYS = ("HOME", "USERPROFILE", "DOTNET_CLI_HOME", "NUGET_PACKAGES", "TMP", "TEMP", "TMPDIR")


def require(condition, message):
    if not condition:
        raise ValueError(message)


def fingerprint_inputs(value):
    return hashlib.sha256(json.dumps(value, sort_keys=True, separators=(",", ":"),
                                     ensure_ascii=True, allow_nan=False).encode()).hexdigest()


def file_record(path):
    path = Path(path).resolve(strict=True)
    with path.open("rb") as stream:
        digest = hashlib.file_digest(stream, "sha256").hexdigest()
    return {"path": str(path), "sha256": digest}


def read_json(path):
    def unique(pairs):
        result = {}
        for key, value in pairs:
            require(key not in result, f"Duplicate JSON key: {key}")
            result[key] = value
        return result

    def invalid(value):
        raise ValueError(f"Nonfinite JSON value: {value}")

    return json.loads(Path(path).read_text(encoding="utf-8-sig"),
                      object_pairs_hook=unique, parse_constant=invalid)


def write_json(path, value):
    path = Path(path)
    temporary = path.with_suffix(path.suffix + ".tmp")
    temporary.write_text(json.dumps(value, indent=2, allow_nan=False) + "\n", encoding="utf-8")
    temporary.replace(path)


def absolute_path(value):
    require(isinstance(value, str) and value and "\0" not in value, "Missing absolute path")
    path = PureWindowsPath(value) if PureWindowsPath(value).is_absolute() else PurePosixPath(value)
    require(path.is_absolute() and ".." not in path.parts, f"Path must be absolute and normalized: {value}")
    return path


def validate_file_record(record):
    require(type(record) is dict and isinstance(record.get("sha256"), str)
            and re.fullmatch("[0-9a-f]{64}", record["sha256"]), "Invalid file hash")
    return absolute_path(record.get("path"))


def validate_workload(workload):
    required = {"revision", "default_branch_revision", "configuration", "target", "target_framework",
                "parallel_compilation", "cwd", "checkout", "sources", "generated", "references",
                "resources", "auxiliary", "outputs", "assembly_output", "core", "core_package",
                "core_package_asset", "arguments", "arguments_sha256", "response_file"}
    require(type(workload) is dict and required <= workload.keys(), "Missing workload identity")
    require(workload["configuration"] == "Release", "Only Release is an acceptance workload")
    require(workload["target"] == PROJECT.as_posix(), "Not the compiler-source workload")
    for key in ("revision", "default_branch_revision"):
        require(isinstance(workload[key], str) and re.fullmatch("[0-9a-f]{40}", workload[key]),
                f"Invalid workload {key}")
    require(isinstance(workload["target_framework"], str)
            and re.fullmatch(r"net\d+\.0", workload["target_framework"]), "Invalid workload target framework")
    checkout, cwd = (absolute_path(workload[key]) for key in ("checkout", "cwd"))
    require(cwd == checkout / "src" / "Compiler", "Workload cwd is not src/Compiler")
    for key in ("sources", "generated", "references", "resources", "auxiliary"):
        records = workload[key]
        require(type(records) is list and (records or key in ("resources", "auxiliary")),
                f"Missing workload {key}")
        for record in records:
            path = validate_file_record(record)
            if key in ("sources", "generated"):
                require(path.is_relative_to(checkout) and path.suffix in (".fs", ".fsi"),
                        f"Source outside frozen checkout: {path}")
    require(any(absolute_path(r["path"]).is_relative_to(cwd) for r in workload["sources"]),
            "No tracked src/Compiler sources")
    for key in ("core", "core_package", "response_file"):
        validate_file_record(workload[key])
    core = [r for r in workload["references"] if absolute_path(r["path"]).name == "FSharp.Core.dll"]
    require(core == [workload["core"]], "Core identity differs from workload reference")
    asset = workload["core_package_asset"]
    require(isinstance(asset, str) and asset.startswith("lib/") and asset.endswith("/FSharp.Core.dll"),
            "Missing local Core package asset")
    outputs = workload["outputs"]
    require(type(outputs) is list and outputs and workload["assembly_output"] in outputs,
            "Missing workload output assembly")
    inputs = {r["path"] for key in ("sources", "generated", "references", "resources", "auxiliary")
              for r in workload[key]}
    for output in outputs:
        path = absolute_path(output)
        require(path.is_relative_to(checkout) and path != checkout and output not in inputs,
                f"Unsafe workload output: {output}")
    arguments = workload["arguments"]
    require(type(arguments) is list and all(isinstance(a, str) and a and a == a.strip()
            and not any(c in a for c in "\r\n\0") and not a.startswith("@") for a in arguments),
            "Invalid workload arguments")
    require(workload["arguments_sha256"] == fingerprint_inputs(arguments), "Argument hash mismatch")
    # Inspect option names, not reference/resource paths; include legacy and slash spellings.
    options = [(a, a.lstrip("-/").partition(":")) for a in arguments
               if a.startswith("-") or (a.startswith("/") and "/" not in a[1:])]
    diagnostics = {"times", "statistics", "progress", "resolutions", "showextensionresolution",
                   "pause", "ast", "parseonly", "typecheckonly", "debug-parse", "simulateexception",
                   "dumpallcommandlineoptions"}
    require(not any(name.lower().rstrip("+-") in diagnostics or name.lower().startswith(("test", "typedtree", "tokenize"))
                    for _, (name, _, _) in options),
            "Compiler instrumentation is not unprofiled acceptance evidence")
    require(type(workload["parallel_compilation"]) is bool, "Invalid parallel-compilation identity")
    parallel = "--parallelcompilation+" if workload["parallel_compilation"] else "--parallelcompilation-"
    disabled_optimization = {"localoptimize-", "crossoptimize-", "jit-",
                             "no-local-optimize", "no-cross-optimize", "no-jit-optimize"}
    require([a for a, (name, _, _) in options
             if name.lower().rstrip("+-") in ("optimize", "ooff") or name.rstrip("+-") == "O"] == ["--optimize+"]
            and "--noframework" in arguments
            and [a for a, (name, _, _) in options if name.lower().rstrip("+-") == "parallelcompilation"] == [parallel]
            and not any(name.lower() in disabled_optimization
                        or (name.lower() in ("detuple", "tlr", "finalsimplify", "inlinethreshold")
                            and not (re.fullmatch(r"\s*\+?\d+\s*", value) and int(value) > 0))
                        for _, (name, _, value) in options),
            "Release/parallel flags differ from workload identity")


def validate_provenance(invariants):
    for section in SECTIONS:
        require(type(invariants[section]) is dict and invariants[section], f"Missing {section} identity")
    bootstrap, toolchain, environment = (invariants[k] for k in ("bootstrap", "toolchain", "environment"))
    for key in ("revision", "source_tree"):
        require(isinstance(bootstrap.get(key), str) and re.fullmatch("[0-9a-f]{40}", bootstrap[key]),
                f"Invalid bootstrap {key}")
    commands = bootstrap.get("commands")
    require(type(commands) is list and commands, "Missing bootstrap commands")
    for command in commands:
        require(type(command) is dict and type(command.get("exit_status")) is int
                and command["exit_status"] == 0, "Unsuccessful bootstrap command")
        args = command.get("arguments")
        require(type(args) is list and args and all(isinstance(a, str) and a and "\0" not in a for a in args),
                "Missing bootstrap command arguments")
        for key in ("cwd", "log"):
            absolute_path(command.get(key))
    for owner, key in ((bootstrap, "binaries"), (toolchain, "runtime_files"), (toolchain, "sdk_compiler_files")):
        records = owner.get(key)
        require(type(records) is list and records, f"Missing provenance {key}")
        for record in records:
            validate_file_record(record)
    for key in ("dotnet", "measurement", "capture_target"):
        validate_file_record(toolchain.get(key))
    for key in ("sdk", "runtime", "info", "python", "git"):
        require(isinstance(toolchain.get(key), str) and toolchain[key].strip(), f"Missing toolchain {key}")
    runtime_hash = toolchain.get("compiler_runtimeconfig_sha256")
    require(isinstance(runtime_hash, str) and re.fullmatch("[0-9a-f]{64}", runtime_hash),
            "Missing compiler runtime configuration hash")
    for key in ("os", "architecture", "hostname"):
        require(isinstance(environment.get(key), str) and environment[key].strip(), f"Missing environment {key}")
    unavailable = environment.get("unavailable_reasons")
    require(type(unavailable) is dict and all(isinstance(v, str) and v.strip() for v in unavailable.values()),
            "Missing environment availability reasons")
    runner = environment.get("runner")
    require(type(runner) is dict and set(runner) == set(RUNNER_KEYS), "Missing runner identity")
    for key, value in list(runner.items()) + [(k, environment.get(k)) for k in
                                             ("logical_cpus", "affinity", "cpu_topology", "processor")]:
        reason_key = f"runner.{key}" if key in runner else key
        require(key in runner or key in environment, f"Missing environment {key}")
        if value is None:
            require(reason_key in unavailable, f"Missing unavailability reason: {reason_key}")
        elif key == "logical_cpus":
            require(type(value) is int and value > 0, "Invalid CPU count")
        elif key == "affinity":
            require(type(value) is list and value and all(type(x) is int and x >= 0 for x in value),
                    "Invalid CPU affinity")
        elif key == "cpu_topology":
            require(type(value) is dict and value and all(isinstance(v, str) and v for v in value.values()),
                    "Invalid CPU topology")
        elif key == "processor" and type(value) is list:
            require(value and all(isinstance(v, str) and v.strip() for v in value), "Invalid processor identity")
        else:
            require(isinstance(value, str) and value.strip(), f"Invalid environment {key}")
    variables = environment.get("variables")
    allowed = (set(INHERITED_ENVIRONMENT_KEYS) | set(CONTROLLED_ENVIRONMENT) | set(OWNED_ENVIRONMENT_KEYS)
               | LOCAL_ENVIRONMENT_KEYS | {"OS", "APPDATA", "LOCALAPPDATA"})
    require(type(variables) is dict and set(variables) <= allowed
            and all(isinstance(v, str) and v and "\0" not in v for v in variables.values()),
            "Invalid or non-allowlisted environment variables")
    require(all(variables.get(k) == v for k, v in CONTROLLED_ENVIRONMENT.items()),
            "Diagnostics/cache execution controls changed")
    for key in OWNED_ENVIRONMENT_KEYS:
        absolute_path(variables.get(key))
    require(environment.get("cache_policy") == CACHE_POLICY, "Missing or unsupported cache policy")
    experiment = invariants["experiment"]
    require(isinstance(experiment.get("id"), str) and experiment["id"].strip(), "Missing experiment identity")
    for key in ("primary_metric", "collector"):
        require(isinstance(experiment.get(key), str) and experiment[key].strip(), f"Missing experiment {key}")
    require(type(experiment.get("seed")) is int, "Missing ordering seed")


def validate_manifest(document, kind):
    require(type(document) is dict and type(document.get("schema_version")) is int
            and document["schema_version"] == 1, "Unsupported or missing schema_version")
    require(document.get("kind") == kind, f"Expected {kind} manifest")
    fingerprint_inputs(document)
    if kind == "compiler":
        require(document.get("configuration") == "Release", "Compiler installation must be Release")
        require(re.fullmatch("[0-9a-f]{40}", document.get("source_revision", "")), "Invalid compiler revision")
        for key in ("patch_sha256", "bootstrap_fingerprint", "toolchain_fingerprint"):
            require(re.fullmatch("[0-9a-f]{64}", document.get(key, "")), f"Invalid {key}")
        require(type(document.get("files")) is dict, "Missing compiler installation")
        for name in ("fsc.dll", "FSharp.Compiler.Service.dll", "fsc.runtimeconfig.json",
                     "fsc.deps.json", "FSharp.Core.dll"):
            require(name in document.get("files", {}), f"Missing compiler binary: {name}")
        installation = validate_file_record(document["files"]["fsc.dll"]).parent
        for name, record in document["files"].items():
            relative = PurePosixPath(name)
            require(name and not relative.is_absolute() and ".." not in relative.parts and "\\" not in name
                    and not PureWindowsPath(name).drive and str(relative) == name, "Invalid compiler installation entry")
            require(validate_file_record(record) == installation / relative,
                    f"Compiler companion is outside its installation or misnamed: {name}")
    elif kind in ("workload", "samples"):
        require(set(document.get("invariants", {})) == set(SECTIONS), "Missing invariant section")
        validate_provenance(document["invariants"])
        validate_workload(document["invariants"]["workload"])
        if kind == "workload":
            validate_manifest(document["baseline_compiler"], "compiler")
            baseline, invariants = document["baseline_compiler"], document["invariants"]
            require(all(baseline[f"{k}_fingerprint"] == fingerprint_inputs(invariants[k])
                        for k in ("bootstrap", "toolchain")), "Baseline build provenance differs")
            require(baseline["files"]["fsc.runtimeconfig.json"]["sha256"] ==
                    invariants["toolchain"]["compiler_runtimeconfig_sha256"], "Baseline runtime configuration differs")
        else:
            require(document.get("arm") in ("baseline", "candidate"), "Invalid sample arm")
            for key in ("compiler_id", "control_compiler_id"):
                require(re.fullmatch("[0-9a-f]{64}", document.get(key, "")), f"Invalid {key}")
            require(type(document.get("samples")) is list, "Missing samples")
            require(type(document.get("collection_complete")) is bool, "Missing collection completion status")
            require(isinstance(document.get("collection_id"), str) and document["collection_id"],
                    "Missing collection identity")
            require("compiler" in document, "Missing compiler manifest")
            validate_manifest(document["compiler"], "compiler")
            for sample in document["samples"]:
                require(isinstance(sample.get("sample_id"), str) and sample["sample_id"], "Missing sample ID")
                require(type(sample.get("unavailable_reasons")) is dict, "Missing metric availability reasons")
                for metric in METRICS:
                    value = sample.get(metric)
                    if value is None:
                        reason = sample["unavailable_reasons"].get(metric)
                        require(metric != "elapsed_ms" and isinstance(reason, str) and reason.strip(),
                                f"Missing value/reason for {metric}")
                    else:
                        require(type(value) in (int, float) and math.isfinite(value) and value >= 0,
                                f"Invalid {metric}")
                require(type(sample.get("complete")) is bool and
                        type(sample.get("profiler_attached")) is bool, "Missing completion/profiler status")
                status = sample.get("exit_status")
                require(type(status) is int or (status is None and not sample["complete"]), "Invalid exit status")


def allowed_environment(work_dir):
    root = Path(work_dir).resolve()
    environment = {key: os.environ[key] for key in INHERITED_ENVIRONMENT_KEYS
                   if key in os.environ}
    environment.update({
        "HOME": str(root / "home"), "USERPROFILE": str(root / "home"),
        "DOTNET_CLI_HOME": str(root / "home"), "NUGET_PACKAGES": str(root / "packages"),
        "TMP": str(root / "tmp"), "TEMP": str(root / "tmp"), "TMPDIR": str(root / "tmp"),
        **CONTROLLED_ENVIRONMENT,
    })
    for name in ("home", "tmp", "packages"):
        (root / name).mkdir(parents=True, exist_ok=True)
    if os.name == "nt":
        environment["OS"] = "Windows_NT"
        for key in ("APPDATA", "LOCALAPPDATA"):
            path = root / "home" / key
            path.mkdir(exist_ok=True)
            environment[key] = str(path)
    return environment


def run_process(arguments, cwd, environment, log):
    require(isinstance(arguments, list) and all(isinstance(x, str) for x in arguments), "Use argument arrays")
    start = time.perf_counter_ns()
    started_at = time.time_ns()
    cpu = rss = None
    unavailable = {}
    with Path(log).open("wb") as output:
        isolation = ({"creationflags": subprocess.CREATE_NEW_PROCESS_GROUP | 0x4} if sys.platform == "win32"
                     else {"start_new_session": True})
        job = process = None
        try:
            if sys.platform == "win32":
                job = WindowsJob()
            process = subprocess.Popen(arguments, cwd=cwd, env=environment, stdin=subprocess.DEVNULL,
                                       stdout=output, stderr=subprocess.STDOUT, **isolation)
            if job is not None:
                try:
                    job.assign_and_resume(process)
                except BaseException:
                    process.kill()
                    raise
            if sys.platform == "linux":
                _, status, usage = os.wait4(process.pid, 0)
                process.returncode = os.waitstatus_to_exitcode(status)
                cpu = (usage.ru_utime + usage.ru_stime) * 1000
                rss = usage.ru_maxrss * 1024
            else:
                process.wait()
                unavailable = {metric: "Per-process rusage collection requires Linux"
                               for metric in METRICS[1:]}
        except BaseException:
            # Kill descendants before reaping the wrapper; never target the caller's group.
            if process is not None and job is None:
                try:
                    os.killpg(process.pid, signal.SIGKILL)
                except ProcessLookupError:
                    pass  # The entire owned group has already exited.
            raise
        finally:
            if job is not None:
                job.close()
            if process is not None and process.returncode is None:
                process.wait()
    return {"elapsed_ms": (time.perf_counter_ns() - start) / 1_000_000,
            "command": arguments, "cwd": str(cwd), "pid": process.pid, "started_at_unix_ns": started_at,
            "process_cpu_ms": cpu, "process_peak_rss_bytes": rss,
            "unavailable_reasons": unavailable, "exit_status": process.returncode,
            "complete": process.returncode == 0}


def capture_inputs(arguments, cwd, checkout, tracked):
    cwd, checkout = Path(cwd).resolve(), Path(checkout).resolve()
    result = {key: [] for key in ("sources", "generated", "references", "resources", "auxiliary", "outputs")}
    input_options = {"-r": "references", "--reference": "references", "--resource": "resources",
                     "--linkresource": "resources", "--embed": "auxiliary", "--sourcelink": "auxiliary",
                     "--keyfile": "auxiliary", "--win32res": "auxiliary", "--win32manifest": "auxiliary",
                     "--win32icon": "auxiliary", "--versionfile": "auxiliary"}
    output_options = ("-o", "--out", "--refout", "--doc", "--pdb", "--sig")
    for arg in arguments:
        require(arg and arg == arg.strip() and not any(c in arg for c in "\r\n\0"),
                "Response arguments must be nonempty single lines")
        require(not arg.startswith("@"), "Nested response files are not a materialized workload")
        option, separator, value = arg.partition(":")
        if separator and option in output_options:
            output = (cwd / value).resolve()
            require(output.is_relative_to(checkout), f"Output outside owned checkout: {output}")
            result["outputs"].append(str(output))
            if option in ("-o", "--out"):
                result["assembly_output"] = str(output)
        elif separator and option in input_options:
            path = value.split(",")[0] if option in ("--resource", "--linkresource") else value
            result[input_options[option]].append(file_record(cwd / path))
        elif not arg.startswith("-"):
            path = (cwd / arg).resolve()
            require(path.suffix in (".fs", ".fsi"), f"Unexpected source: {arg}")
            relative = path.relative_to(checkout).as_posix()
            category = "sources" if relative in tracked else "generated"
            result[category].append(file_record(path))
        else:
            require(not option.startswith(("--compilertool", "--analyzer", "--use", "--load")),
                    f"Unsupported additional compiler input: {arg}")
    require(result["sources"] or result["generated"], "No compiler sources captured")
    require(any(Path(x["path"]).is_relative_to(checkout / "src" / "Compiler")
                for x in result["sources"] + result["generated"]), "Not compiling src/Compiler")
    core = [x for x in result["references"] if Path(x["path"]).name == "FSharp.Core.dll"]
    require(len(core) == 1, "Exactly one materialized Core reference is required")
    result["core"] = core[0]
    # The compiler implicitly writes a portable PDB next to its output assembly.
    for path in list(result["outputs"]):
        if Path(path).suffix == ".dll":
            result["outputs"].append(str(Path(path).with_suffix(".pdb")))
    result["outputs"] = list(dict.fromkeys(result["outputs"]))
    require("assembly_output" in result, "No owned output assembly captured")
    return result


def reset_outputs(outputs, owned_root, inputs):
    root = Path(owned_root).resolve()
    protected = {Path(x["path"]).resolve() for x in inputs}
    paths = [Path(x) for x in outputs]
    for path in paths:
        require(path.resolve().is_relative_to(root) and path.resolve() != root and not path.is_symlink()
                and path.resolve() not in protected and not path.is_dir(), f"Unsafe output cleanup: {path}")
    for path in paths:
        path.unlink(missing_ok=True)


def verify_files(records):
    for record in records:
        require(file_record(record["path"]) == record, f"Input changed: {record['path']}")


def verify_collector(toolchain):
    require(file_record(__file__) == toolchain["measurement"], "Executing measurement collector differs from toolchain")
    require(platform.python_version() == toolchain["python"], "Executing Python version differs from toolchain")


def compiler_manifest(installation, source_revision, patch_sha256, bootstrap, toolchain):
    installation = Path(installation).resolve()
    result = {"schema_version": 1, "kind": "compiler", "configuration": "Release", "source_revision": source_revision,
              "patch_sha256": patch_sha256, "bootstrap_fingerprint": fingerprint_inputs(bootstrap),
              "toolchain_fingerprint": fingerprint_inputs(toolchain),
              "files": {p.relative_to(installation).as_posix(): file_record(p)
                        for p in sorted(installation.rglob("*")) if p.is_file()}}
    validate_manifest(result, "compiler")
    return result


def packed_core_asset(reference, package):
    with zipfile.ZipFile(package) as archive:
        for name in archive.namelist():
            if name.startswith("lib/") and name.endswith("/FSharp.Core.dll"):
                with archive.open(name) as stream:
                    if hashlib.file_digest(stream, "sha256").hexdigest() == reference["sha256"]:
                        return name
    raise ValueError("Workload Core reference does not match any asset in the local Release package")


def local_core_package(directory):
    packages = [p for p in sorted(Path(directory).rglob("FSharp.Core.*.nupkg"))
                if not p.name.endswith(".symbols.nupkg")]
    shipping = [p for p in packages if p.parent.name == "Shipping"]
    packages = shipping or packages
    require(len(packages) == 1, f"Expected one local Shipping Core package, got {len(packages)}")
    return packages[0]


def schedule():
    events = []
    rng = random.Random(POLICY["seed"])
    for phase, role, count in (("ab", "warmup", POLICY["warmups"]),
                               ("aa", "measured", POLICY["control_pairs"]),
                               ("ab", "measured", POLICY["pairs"])):
        for pair in range(count):
            arms = ["baseline", "candidate"]
            rng.shuffle(arms)
            for arm in arms:
                events.append({"phase": phase, "role": role, "pair": pair,
                               "arm": arm, "ordinal": len(events)})
    return events


def percent_difference(baseline, candidate):
    # JSON decimal values, not binary floating-point cancellation, define policy boundaries.
    a, b = Fraction(str(baseline)), Fraction(str(candidate))
    return (a - b) * 100 / a


def paired_summary(baseline, candidate):
    effects = [percent_difference(a, b) for a, b in zip(baseline, candidate)]
    rng = random.Random(POLICY["seed"])
    boot = sorted(statistics.median(rng.choices(effects, k=len(effects)))
                  for _ in range(POLICY["bootstrap_resamples"]))
    return {
        "baseline_median": statistics.median(baseline), "candidate_median": statistics.median(candidate),
        "baseline_relative_mad_percent": relative_mad(baseline),
        "candidate_relative_mad_percent": relative_mad(candidate),
        "improvement_percent": statistics.median(effects),
        "interval_95_percent": [boot[math.ceil(len(boot) * .025) - 1], boot[math.ceil(len(boot) * .975) - 1]],
    }


def relative_mad(values):
    values = [Fraction(str(x)) for x in values]
    median = statistics.median(values)
    return statistics.median(abs(x - median) for x in values) / median * 100


def compare_measurements(baseline, candidate):
    for document in (baseline, candidate):
        validate_manifest(document, "samples")
    invariants = baseline["invariants"]
    primary = invariants["experiment"].get("primary_metric")
    result = {"schema_version": 1, "kind": "comparison", "decision": "inconclusive",
              "primary_metric": primary, "raw_sample_ids": [s["sample_id"] for d in (baseline, candidate)
                                                            for s in d["samples"]],
              "invariant_mismatches": [k for k in SECTIONS if invariants[k] != candidate["invariants"][k]],
              "primary": None, "elapsed": None, "aa": None, "aa_primary": None, "evidence": {}, "reasons": []}
    reasons = result["reasons"]
    if result["invariant_mismatches"]:
        reasons.append("Comparison invariants differ")
        return result
    if invariants["policy"] != POLICY or invariants["experiment"].get("seed") != POLICY["seed"]:
        reasons.append("Policy differs from the checked-in production policy")
    if primary not in PRIMARY_METRICS or invariants["experiment"].get("collector") != "process-rusage-v1":
        reasons.append("Unsupported predeclared primary metric or collector")
    if baseline["collection_id"] != candidate["collection_id"]:
        reasons.append("Samples came from separate collection invocations")
    if baseline["control_compiler_id"] != baseline["compiler_id"] or candidate["control_compiler_id"] != baseline["compiler_id"]:
        reasons.append("A/A control compiler differs from baseline")
    ids = result["raw_sample_ids"]
    if len(set(ids)) != len(ids):
        reasons.append("Duplicate sample IDs")
    fingerprints = {k: fingerprint_inputs(v) for k, v in invariants.items()}
    for arm, document in (("baseline", baseline), ("candidate", candidate)):
        expected = [s for s in schedule() if s["arm"] == arm]
        samples = document["samples"]
        if not document["collection_complete"]:
            reasons.append(f"{arm}: collection was not finalized")
        if fingerprint_inputs(document["compiler"]) != document["compiler_id"]:
            reasons.append(f"{arm}: compiler manifest fingerprint mismatch")
        if any(document["compiler"][f"{key}_fingerprint"] != fingerprint_inputs(invariants[key])
               for key in ("bootstrap", "toolchain")):
            reasons.append(f"{arm}: compiler build provenance differs")
        if document["compiler"]["files"]["fsc.runtimeconfig.json"]["sha256"] != invariants["toolchain"].get("compiler_runtimeconfig_sha256"):
            reasons.append(f"{arm}: compiler runtime configuration differs from toolchain")
        if document["arm"] != arm or len(samples) != len(expected):
            reasons.append(f"{arm}: incomplete repetition/warm-up schedule")
        for sample, event in zip(samples, expected):
            if any(sample.get(k) != v for k, v in event.items()):
                reasons.append(f"{arm}: non-interleaved or incomplete schedule")
            compiler = document["control_compiler_id"] if event["phase"] == "aa" else document["compiler_id"]
            if sample.get("compiler_id") != compiler or sample.get("fingerprints") != fingerprints:
                reasons.append(f"{arm}: sample identity mismatch")
            if not sample["complete"] or sample["exit_status"] != 0 or sample["elapsed_ms"] <= 0:
                reasons.append(f"{arm}: failed, cancelled or incomplete compilation")
            if sample["profiler_attached"]:
                result["decision"] = "rejected"
                reasons.append(f"{arm}: profiled runs are not acceptance samples")
    if reasons:
        result["reasons"] = list(dict.fromkeys(reasons))
        return result

    def series(document, phase, metric):
        return [s[metric] for s in sorted(document["samples"], key=lambda x: x["pair"])
                if s["phase"] == phase and s["role"] == "measured"]

    aa = [series(d, "aa", "elapsed_ms") for d in (baseline, candidate)]
    aa_primary = [series(d, "aa", primary) for d in (baseline, candidate)]
    elapsed = [series(d, "ab", "elapsed_ms") for d in (baseline, candidate)]
    values = [series(d, "ab", primary) for d in (baseline, candidate)]
    if any(x is None or x <= 0 for arm in values + aa_primary for x in arm):
        reasons.append("Primary metric unavailable or zero")
        return result
    result["aa"] = paired_summary(*aa)
    result["aa_primary"] = paired_summary(*aa_primary)
    result["elapsed"] = paired_summary(*elapsed)
    result["primary"] = paired_summary(*values)
    for metric in METRICS:
        evidence = [series(d, "ab", metric) for d in (baseline, candidate)]
        if all(x is not None and x > 0 for arm in evidence for x in arm):
            result["evidence"][metric] = {"summary": paired_summary(*evidence), "unavailable_reason": None}
        else:
            result["evidence"][metric] = {"summary": None, "unavailable_reason": "Incomplete or zero-valued evidence"}
    aa_difference = abs(percent_difference(statistics.median(aa[0]), statistics.median(aa[1])))
    result["aa"]["absolute_median_difference_percent"] = aa_difference
    if aa_difference > POLICY["maximum_aa_difference_percent"]:
        reasons.append("A/A median difference exceeds 1%")
    if primary != "elapsed_ms":
        primary_aa_difference = abs(percent_difference(statistics.median(aa_primary[0]), statistics.median(aa_primary[1])))
        if primary_aa_difference > POLICY["maximum_aa_difference_percent"]:
            reasons.append("Primary metric A/A median difference exceeds 1%")
    if any(relative_mad(v) > POLICY["maximum_relative_mad_percent"] for v in aa + elapsed):
        reasons.append("Timing relative MAD exceeds 2%")
    if not reasons:
        effect = result["primary"]
        if effect["improvement_percent"] < POLICY["minimum_improvement_percent"] or effect["interval_95_percent"][0] <= 0:
            reasons.append("Primary improvement is below 3% or its paired interval includes zero")
        # Regression is relative to baseline, so its upper bound is minus improvement's lower bound.
        if primary != "elapsed_ms" and -result["elapsed"]["interval_95_percent"][0] >= POLICY["maximum_time_regression_percent"]:
            reasons.append("Upper elapsed regression bound must be strictly below 1%")
        result["decision"] = "rejected" if reasons else "measured_improvement"
    for summary in [result[k] for k in ("aa", "aa_primary", "elapsed", "primary")] + [
            e["summary"] for e in result["evidence"].values() if e["summary"] is not None]:
        for key, value in summary.items():
            summary[key] = ([float(x) for x in value] if isinstance(value, list)
                            else float(value) if isinstance(value, Fraction) else value)
    return result


def host_identity(environment):
    topology = {}
    processor = os.environ.get("PROCESSOR_IDENTIFIER")
    if sys.platform == "linux":
        cpuinfo = Path("/proc/cpuinfo")
        if cpuinfo.is_file():
            processor = sorted({line.partition(":")[2].strip() for line in cpuinfo.read_text().splitlines()
                                if line.partition(":")[0].strip() in ("model name", "Hardware", "CPU implementer")})
        for path in sorted(Path("/sys/devices/system/cpu").glob("cpu[0-9]*/topology/*")):
            if path.name in ("physical_package_id", "core_id", "thread_siblings_list"):
                topology[str(path)] = path.read_text().strip()
    result = {"os": f"{platform.system()} {platform.release()} {platform.version()}",
            "architecture": platform.machine(), "hostname": socket.gethostname(),
            "logical_cpus": os.cpu_count(), "affinity": sorted(os.sched_getaffinity(0)) if hasattr(os, "sched_getaffinity") else None,
            "cpu_topology": topology or None, "processor": processor or None,
            "runner": {key: os.environ.get(key) or None for key in RUNNER_KEYS}, "variables": environment,
            "cache_policy": CACHE_POLICY, "unavailable_reasons": {}}
    for key in ("logical_cpus", "affinity", "cpu_topology", "processor"):
        if result[key] is None:
            result["unavailable_reasons"][key] = f"Not exposed by {platform.system()} host APIs"
    for key, value in result["runner"].items():
        if value is None:
            result["unavailable_reasons"][f"runner.{key}"] = "Not provided by the runner environment"
    return result


def prepare_workload(source_sha, work_dir, output, primary_metric="elapsed_ms", parallel=False):
    require(re.fullmatch("[0-9a-f]{40}", source_sha), "--source-sha must be a full lowercase Git SHA")
    require(primary_metric in PRIMARY_METRICS, "Unsupported primary metric")
    root = Path(work_dir).resolve()
    require(not root.exists(), "--work-dir must be a new, exclusively owned directory")
    require(sys.platform in ("linux", "win32"), "Preparation supports Linux and Windows only; collection requires Linux")
    root.mkdir(parents=True)
    environment = allowed_environment(root)
    commands = []

    def run(args, cwd):
        log = root / f"prepare-{len(commands):02}.log"
        result = run_process([str(x) for x in args], cwd, environment, log)
        commands.append({"arguments": [str(x) for x in args], "cwd": str(cwd),
                         "exit_status": result["exit_status"], "log": str(log)})
        write_json(root / "preparation-commands.json",
                   {"schema_version": 1, "kind": "preparation", "commands": commands})
        require(result["complete"], f"Preparation failed ({result['exit_status']}): see {log}")
        return log.read_text(encoding="utf-8-sig").strip()

    bootstrap_dir, workload_dir = root / "bootstrap", root / "workload"
    default_branch_revision = run(["git", "rev-parse", "refs/remotes/origin/main"], REPO)
    run(["git", "merge-base", "--is-ancestor", source_sha, default_branch_revision], REPO)
    for checkout in (bootstrap_dir, workload_dir):
        run(["git", "clone", "--quiet", "--shared", "--no-checkout", str(REPO), str(checkout)], REPO)
        run(["git", "checkout", "--quiet", "--detach", source_sha], checkout)
    dotnet_name = "dotnet.exe" if os.name == "nt" else "dotnet"
    local_dotnet = REPO / ".dotnet" / dotnet_name
    dotnet = (local_dotnet if local_dotnet.is_file() else Path(shutil.which(dotnet_name) or dotnet_name)).resolve()
    require(dotnet.is_file(), "dotnet is missing; provision the pinned SDK with eng/common/dotnet")
    environment["PATH"] = str(dotnet.parent) + os.pathsep + environment.get("PATH", os.defpath)
    environment["DOTNET_ROOT"] = str(dotnet.parent)
    environment["DOTNET_HOST_PATH"] = str(dotnet)
    environment["DOTNET_INSTALL_DIR"] = str(dotnet.parent)
    pinned = read_json(bootstrap_dir / "global.json")["sdk"]["version"]
    sdk = run([dotnet, "--version"], bootstrap_dir)
    require(sdk == pinned, f"SDK mismatch: expected {pinned}, got {sdk}")
    if os.name == "nt":
        run([os.environ["COMSPEC"], "/d", "/c", "build.cmd", "-c", "Release", "-pack", "-noVisualStudio"], bootstrap_dir)
    else:
        run(["bash", "build.sh", "-c", "Release", "--pack"], bootstrap_dir)
    toolchain = {"dotnet": file_record(dotnet), "sdk": sdk,
                 "sdks": run([dotnet, "--list-sdks"], bootstrap_dir),
                 "runtime": run([dotnet, "--list-runtimes"], bootstrap_dir),
                 "info": run([dotnet, "--info"], bootstrap_dir), "python": platform.python_version(),
                 "git": run(["git", "--version"], bootstrap_dir), "measurement": file_record(__file__),
                 "capture_target": file_record(HERE / "Capture.targets")}
    toolchain["runtime_files"] = [file_record(p) for p in sorted((dotnet.parent / "shared" / "Microsoft.NETCore.App").rglob("*"))
                                  if p.is_file()]
    toolchain["sdk_compiler_files"] = [file_record(p) for p in sorted((dotnet.parent / "sdk" / sdk / "FSharp").rglob("*"))
                                      if p.is_file()]
    require(toolchain["runtime_files"], "Runtime installation was not materialized")
    require(toolchain["sdk_compiler_files"], "SDK bootstrap compiler was not materialized")
    tfm = run([dotnet, "msbuild", str(PROJECT), "-nologo", "-p:Configuration=Release",
               "-getProperty:FSharpNetCoreProductTargetFramework"], bootstrap_dir)
    require(re.fullmatch(r"net\d+\.0", tfm), f"Unexpected product framework: {tfm}")
    installation = bootstrap_dir / "artifacts" / "bin" / "fsc" / "Release" / tfm
    proto_files = [file_record(p) for p in sorted((bootstrap_dir / "artifacts").rglob("*"))
                   if p.is_file() and ("Proto" in p.parts or "Bootstrap" in p.parts)
                   and p.suffix in (".dll", ".json", ".exe")]
    require(proto_files, "Bootstrap compiler binaries were not found")
    bootstrap = {"revision": source_sha, "source_tree": run(["git", "rev-parse", "HEAD^{tree}"], bootstrap_dir),
                 "binaries": proto_files, "commands": list(commands)}
    baseline = compiler_manifest(installation, source_sha, hashlib.sha256(b"").hexdigest(), bootstrap, toolchain)
    toolchain["compiler_runtimeconfig_sha256"] = baseline["files"]["fsc.runtimeconfig.json"]["sha256"]
    baseline["toolchain_fingerprint"] = fingerprint_inputs(toolchain)
    package = local_core_package(bootstrap_dir / "artifacts" / "packages" / "Release")
    version = package.name[len("FSharp.Core."):-len(".nupkg")]
    require(re.fullmatch(r"[0-9][A-Za-z0-9.+-]*", version), "Invalid local Core package version")
    cached_core = (Path(environment["NUGET_PACKAGES"]) / "fsharp.core" / version.lower()).resolve()
    require(cached_core.parent == root / "packages" / "fsharp.core", "Core cache is not owned by this experiment")
    if cached_core.is_dir():
        shutil.rmtree(cached_core)
    capture = root / "capture"
    capture.mkdir()
    environment.update({
        "LoadLocalFSharpBuild": "True", "LocalFSharpCompilerPath": str(bootstrap_dir),
        "LocalFSharpCompilerConfiguration": "Release",
        "CustomAfterDirectoryBuildProps": str(bootstrap_dir / "UseLocalCompiler.Directory.Build.props"),
        "CustomAfterDirectoryBuildTargets": str(HERE / "Capture.targets"),
        "RegressionLocalCore": "true", "RegressionLocalCoreVersion": version,
        "RegressionLocalCorePackagesDir": str(package.parent),
    })
    build_args = [dotnet, "build", str(PROJECT), "-c", "Release", "-f", tfm, "--disable-build-servers",
                  "-p:FSHARPCORE_USE_PACKAGE=true", "-p:UseSharedCompilation=false",
                  "-p:ProduceReferenceAssembly=false", "-p:ProduceOnlyReferenceAssembly=false", "-p:ProvideCommandLineArgs=true",
                  "-p:SkipCompilerExecution=false", f"-p:ParallelCompilation={str(parallel).lower()}",
                  f"-p:DotnetFscCompilerPath=%22{installation / 'fsc.dll'}%22",
                  f"-p:FSharpBuildTasksAssembly={installation / 'FSharp.Build.dll'}",
                  f"-p:FscToolPath={dotnet.parent}", f"-p:FscToolExe={dotnet.name}",
                  f"-p:CompilerPerfCaptureDir={capture}", f"-bl:{root / 'workload.binlog'}"]
    run(build_args, workload_dir)
    metadata = (capture / "identity.txt").read_text(encoding="utf-8-sig").splitlines()
    require(metadata == [f'"{installation / "fsc.dll"}"', "Release", str(workload_dir / PROJECT), tfm],
            f"Captured compiler/project identity mismatch: {metadata}")
    response = capture / "compiler.rsp"
    arguments = response.read_text(encoding="utf-8-sig").splitlines()
    # The local-compiler props inject --times; keep capture evidence but never replay its instrumentation.
    arguments = [arg for arg in arguments if arg != "--times"]
    response = capture / "replay.rsp"
    response.write_text("\n".join(arguments) + "\n", encoding="utf-8")
    tracked = set(run(["git", "ls-files"], workload_dir).splitlines())
    inputs = capture_inputs(arguments, (workload_dir / PROJECT).parent, workload_dir, tracked)
    inputs["core_package"] = file_record(package)
    inputs["core_package_asset"] = packed_core_asset(inputs["core"], package)
    require(not run(["git", "status", "--porcelain", "--untracked-files=no"], workload_dir),
            "Frozen compiler source was changed during preparation")
    workload = dict(inputs, revision=source_sha, default_branch_revision=default_branch_revision,
                    target=PROJECT.as_posix(), configuration="Release",
                    target_framework=tfm, parallel_compilation=parallel, arguments=arguments,
                    arguments_sha256=fingerprint_inputs(arguments), response_file=file_record(response),
                    cwd=str((workload_dir / PROJECT).parent), checkout=str(workload_dir))
    invariants = {"workload": workload, "bootstrap": bootstrap, "toolchain": toolchain,
                  "environment": host_identity(environment), "policy": dict(POLICY),
                  "experiment": {"id": str(uuid.uuid4()), "primary_metric": primary_metric,
                                 "collector": "process-rusage-v1", "seed": POLICY["seed"]}}
    document = {"schema_version": 1, "kind": "workload", "owned_root": str(root),
                "invariants": invariants, "baseline_compiler": baseline, "preparation_commands": commands}
    validate_manifest(document, "workload")
    # This smoke replay also works on Windows; its unavailable rusage is not acceptance evidence.
    reset_outputs(workload["outputs"], workload_dir, sum((inputs[k] for k in ("sources", "generated", "references", "resources", "auxiliary")), []))
    run([dotnet, installation / "fsc.dll", "@" + str(response)], workload["cwd"])
    assembly = Path(workload["assembly_output"])
    require(assembly.is_file() and assembly.stat().st_size > 0, "Smoke replay produced no assembly")
    toolchain["build_inputs"] = {
        "packages": [file_record(p) for p in sorted(Path(environment["NUGET_PACKAGES"]).rglob("*")) if p.is_file()],
        "sdk": [file_record(p) for p in sorted((dotnet.parent / "sdk").rglob("*")) if p.is_file()],
        "bootstrap": [file_record(p) for p in sorted((bootstrap_dir / "artifacts").rglob("*"))
                      if p.is_file() and ("Proto" in p.parts or "Bootstrap" in p.parts)],
    }
    baseline["toolchain_fingerprint"] = fingerprint_inputs(toolchain)
    document["preparation_commands"] = commands
    write_json(root / "baseline-compiler.json", baseline)
    write_json(output, document)
    return document


def run_repetitions(manifest, compiler, output, *, process_runner=None):
    require(sys.platform == "linux", "Live acceptance collection requires Linux (wait4 per-process CPU/peak RSS)")
    validate_manifest(manifest, "workload")
    validate_manifest(compiler, "compiler")
    invariants = manifest["invariants"]
    require(invariants["policy"] == POLICY, "Policy changed since preparation")
    require(invariants["experiment"]["collector"] == "process-rusage-v1"
            and invariants["experiment"]["primary_metric"] in PRIMARY_METRICS,
            "Unsupported predeclared primary metric or collector")
    verify_collector(invariants["toolchain"])
    require(compiler["bootstrap_fingerprint"] == fingerprint_inputs(invariants["bootstrap"]), "Candidate bootstrap mismatch")
    require(compiler["toolchain_fingerprint"] == fingerprint_inputs(invariants["toolchain"]), "Candidate toolchain mismatch")
    environment = invariants["environment"]["variables"]
    safe = allowed_environment(manifest["owned_root"])
    require(set(environment) <= set(safe) | LOCAL_ENVIRONMENT_KEYS, "Non-allowlisted execution environment")
    require(all(environment.get(k) == v for k, v in safe.items() if k != "PATH"),
            "Execution environment/cache policy changed")
    require(host_identity(environment) == invariants["environment"], "Runner/environment changed since preparation")
    root = Path(manifest["owned_root"])
    workload = invariants["workload"]
    records = sum((workload[k] for k in ("sources", "generated", "references", "resources", "auxiliary")), [])
    records += [workload["response_file"], workload["core_package"]] + invariants["bootstrap"]["binaries"]
    records += [invariants["toolchain"][k] for k in ("dotnet", "measurement", "capture_target")]
    records += invariants["toolchain"]["runtime_files"] + invariants["toolchain"]["sdk_compiler_files"]
    installations = {"baseline": manifest["baseline_compiler"], "candidate": compiler}
    for installation in installations.values():
        require(installation["files"]["fsc.runtimeconfig.json"]["sha256"] ==
                invariants["toolchain"]["compiler_runtimeconfig_sha256"], "Compiler runtime configuration changed")
        records += list(installation["files"].values())
    verify_files(records)
    require(Path(workload["response_file"]["path"]).read_text(encoding="utf-8-sig").splitlines() == workload["arguments"],
            "Response file differs from arguments")
    tracked = {Path(record["path"]).relative_to(Path(workload["checkout"]).resolve()).as_posix()
               for record in workload["sources"]}
    captured = capture_inputs(workload["arguments"], workload["cwd"], workload["checkout"], tracked)
    require(all(captured[key] == workload[key] for key in captured), "Arguments differ from frozen input inventory")
    lock = root / "measurement.lock"
    lock.mkdir()
    documents = {}
    paths = {"candidate": Path(output), "baseline": Path(output).with_suffix(".baseline.json")}
    try:
        require(all(not path.exists() for path in paths.values()), "Refusing to overwrite collected evidence")
        collection_id = str(uuid.uuid4())
        logs = root / collection_id
        logs.mkdir()
        for arm in installations:
            documents[arm] = {"schema_version": 1, "kind": "samples", "arm": arm, "invariants": invariants,
                              "collection_id": collection_id, "compiler": installations[arm],
                              "collection_complete": False,
                              "compiler_id": fingerprint_inputs(installations[arm]),
                              "control_compiler_id": fingerprint_inputs(installations["baseline"]), "samples": []}
        for event in schedule():
            installation = installations["baseline" if event["phase"] == "aa" else event["arm"]]
            reset_outputs(workload["outputs"], workload["checkout"], records)
            sample = dict(event, sample_id=str(uuid.uuid4()), compiler_id=fingerprint_inputs(installation),
                          fingerprints={k: fingerprint_inputs(v) for k, v in invariants.items()},
                          profiler_attached=False)
            result = (process_runner or run_process)([invariants["toolchain"]["dotnet"]["path"], installation["files"]["fsc.dll"]["path"],
                                  "@" + workload["response_file"]["path"]],
                                 workload["cwd"], environment, logs / f"sample-{event['ordinal']:02}.log")
            sample.update(result)
            assembly = Path(workload["assembly_output"])
            sample["complete"] = result["complete"] and assembly.is_file() and assembly.stat().st_size > 0
            documents[event["arm"]]["samples"].append(sample)
            for arm in documents:
                write_json(paths[arm], documents[arm])
            require(sample["complete"], f"Compiler failed or produced no assembly (exit {result['exit_status']}): {logs}")
        verify_files(records)
        verify_collector(invariants["toolchain"])
        if sum(len(d["samples"]) for d in documents.values()) == len(schedule()) and result["complete"]:
            for arm, document in documents.items():
                document["collection_complete"] = True
                write_json(paths[arm], document)
    except BaseException:
        for arm, document in documents.items():
            document["collection_complete"] = False
            for sample in document["samples"]:
                sample["complete"] = False
            write_json(paths[arm], document)
        raise
    finally:
        lock.rmdir()
    return documents


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    prepare = commands.add_parser("prepare", help="Build Release baseline, freeze and smoke-replay compiler source")
    prepare.add_argument("--source-sha", required=True, help="Full default-branch revision available in the local Git object store")
    prepare.add_argument("--work-dir", required=True, help="New exclusively owned disposable directory (must not exist)")
    prepare.add_argument("--primary-metric", choices=PRIMARY_METRICS, default="elapsed_ms", help="Predeclare time or peak-RSS retention hypothesis")
    prepare.add_argument("--parallel", action="store_true", help="Separate parallel-compilation-on experiment cell")
    measure = commands.add_parser("measure", help="Linux: collect A/A and interleaved A/B; also write OUTPUT.baseline.json")
    measure.add_argument("--manifest", required=True, help="Frozen workload JSON from prepare")
    measure.add_argument("--compiler", required=True, help="Candidate compiler identity JSON; baseline identity is in workload")
    compare = commands.add_parser("compare", help="Decide measured_improvement, rejected or inconclusive; never authorize publication")
    compare.add_argument("--baseline", required=True, help="Baseline samples JSON from the same measure invocation")
    compare.add_argument("--candidate", required=True, help="Candidate samples JSON from that measure invocation")
    for command in (prepare, measure, compare):
        command.add_argument("--output", required=True, help="Destination versioned JSON file")
    options = parser.parse_args(argv)
    try:
        if options.command == "prepare":
            prepare_workload(options.source_sha, options.work_dir, options.output, options.primary_metric, options.parallel)
        elif options.command == "measure":
            run_repetitions(read_json(options.manifest), read_json(options.compiler), options.output)
        else:
            write_json(options.output, compare_measurements(read_json(options.baseline), read_json(options.candidate)))
    except (ValueError, OSError, KeyError, TypeError) as error:
        parser.exit(2, f"error: {error}\n")
    return 0


if __name__ == "__main__":
    sys.exit(main())
