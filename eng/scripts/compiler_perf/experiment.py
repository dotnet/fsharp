"""Disposable compiler experiments. JSON is data; only this validator chooses commands."""

import argparse
from contextlib import contextmanager, nullcontext
from datetime import datetime, timezone
import hashlib
import hmac
import os
from pathlib import Path, PurePosixPath
import re
import select
import shutil
import stat
import subprocess
import sys
import tempfile
import threading
import uuid
import xml.etree.ElementTree as ET

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import memory as mem

c, m, require = mem.c, mem.m, mem.require
COMPONENT = "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj"
SERVICE = "tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj"
COMPATIBILITY = "tests/projects/CompilerCompat"
COMPAT_SCENARIOS = (("local", "local"), ("latest", "local"), ("local", "latest"),
                    ("latest", "latest"), ("net9", "local"), ("local", "net9"))
RELEASE_PROPERTIES = ["-p:BUILDING_USING_DOTNET=false", "-p:BuildNoRealsig=true"]
PACK_COMMAND = ["bash", "build.sh", "-c", "Release", "--pack"]
RELEASE_NOTE = "docs/release-notes/.FSharp.Compiler.Service/11.0.100.md"
MAX_PATCH = 1024 * 1024
MAX_REPORT = 16 * 1024 * 1024
PLATFORM_EXCLUSIONS = {
    "This test is only supported on Windows",
    "NETCOREAPP is not supported runtime for this kind of test, it is intended for DESKTOP only",
}
KNOWN_AREAS = {"Checking", "CodeGen", "Optimize", "Service", "TypedTree", "Driver",
               "AbstractIL", "SyntaxTree", "Facilities", "Utilities", "SymbolHelpers"}
TEST_AREAS = {
    "Checking": {"Conformance", "ConstraintSolver", "TypeChecks", "Diagnostics", "ErrorMessages", "Language", "Signatures"},
    "CodeGen": {"EmittedIL", "Conformance", "Interop", "StaticLinking"},
    "AbstractIL": {"EmittedIL", "Interop", "Import"},
    "Optimize": {"Optimizations", "EmittedIL"},
    "TypedTree": {"Conformance", "Signatures", "EmittedIL", "Optimizations"},
    "SyntaxTree": {"Conformance", "CompilerDirectives", "Diagnostics"},
}


def relative_path(value):
    require(isinstance(value, str) and len(value) <= 240 and
            re.fullmatch(r"[A-Za-z0-9_. /-]+", value) and
            all(p not in ("", ".", "..") and not p.endswith((" ", "."))
                and not re.fullmatch(r"(?i)(con|prn|aux|nul|com[0-9]|lpt[0-9])(?:\..*)?", p)
                for p in value.split("/")) and
            not PurePosixPath(value).is_absolute(), "Unsafe normalized relative path")
    return value


def validate_request(request):
    fields = {"schema_version", "attempt_id", "run_id", "base_sha", "compiler_paths",
              "regressions", "hypothesis", "primary_metric", "complexity"}
    require(type(request) is dict and set(request) == fields and
            type(request["schema_version"]) is int and request["schema_version"] == 1,
            "Invalid experiment request")
    require(mem.identifier(request["attempt_id"]) and mem.identifier(request["run_id"]) and
            mem.digest(request["base_sha"], 40), "Invalid attempt/run/base identity")
    require(request["primary_metric"] in m.PRIMARY_METRICS, "Unsupported primary metric")
    for key in ("hypothesis", "complexity"):
        require(mem.text(request[key]) and len(request[key]) <= 4096, "Missing/bounded hypothesis or complexity")
    paths = request["compiler_paths"]
    require(type(paths) is list and 0 < len(paths) <= 8 and len(set(paths)) == len(paths),
            "Declare a small compiler change")
    for path in paths:
        relative_path(path)
        require(path.startswith("src/Compiler/") and Path(path).suffix in (".fs", ".fsi"),
                "Only compiler implementation/signature source is in scope")
    areas = {path.split("/")[2] for path in paths}
    require(len(areas) == 1, "One justified compiler area per experiment")
    regressions = request["regressions"]
    require(type(regressions) is list and 0 < len(regressions) <= 8, "New regression checks required")
    for regression in regressions:
        require(type(regression) is dict and set(regression) == {"path", "method"}, "Invalid regression selector")
        path = relative_path(regression["path"])
        require(path.startswith(("tests/FSharp.Compiler.ComponentTests/",
                                 "tests/FSharp.Compiler.Service.Tests/")) and path.endswith(".fs") and
                path != "tests/FSharp.Compiler.ComponentTests/CompilerCompatibilityTests.fs",
                "Regression must be test source, not a harness")
        require(Path(path).stem not in {"Common", "TestHelpers", "CompilerTestHelpers", "XunitSetup",
                                       "TestDoubles", "AssemblyInfo"}, "Test harness edits are not regressions")
        if areas <= TEST_AREAS.keys():
            allowed = set.union(*(TEST_AREAS[area] for area in areas))
            require(path.startswith("tests/FSharp.Compiler.ComponentTests/") and
                    len(path.split("/")) >= 4 and path.split("/")[2] in allowed,
                    "Test source is unrelated to the compiler area")
        require(isinstance(regression["method"], str) and
                re.fullmatch(r"[A-Za-z0-9_][A-Za-z0-9_ .-]{0,159}", regression["method"]),
                "Regression selector must name a method, not a command or wildcard")


def git(tree, *args, data=None, receipts=None):
    environment = {k: v for k, v in os.environ.items() if k in m.INHERITED_ENVIRONMENT_KEYS}
    environment.update(GIT_CONFIG_NOSYSTEM="1", GIT_CONFIG_GLOBAL=os.devnull,
                       GIT_TERMINAL_PROMPT="0", GIT_ATTR_NOSYSTEM="1")
    command = ["git", "-c", "core.hooksPath=" + os.devnull, "-c", "core.autocrlf=false",
               "-c", "core.filemode=true", "-c", "credential.helper=", *map(str, args)]
    if receipts is None and (Path(tree).parent / "owner.json").is_file():
        receipts = Path(tree).parent
    with tempfile.TemporaryDirectory(prefix="compiler-perf-git-") as temporary:
        logs = Path(receipts) / "git-commands" if receipts else Path(temporary)
        logs.mkdir(exist_ok=True)
        result = c.run_tool(command, tree, environment, logs, "git", timeout=60, input_bytes=data)
        if receipts:
            m.write_json(logs / (uuid.uuid4().hex + ".json"), result)
        require(result["status"] == "collected", "Git rejected operation: " + (result.get("stderr") or result.get("reason", "")))
        return result["stdout"].encode("utf-8")


@contextmanager
def owned_lock(root):
    lock = Path(root) / "controller.lock"
    try:
        lock.mkdir()
    except FileExistsError as error:
        raise ValueError(f"Unproven lock owner preserved: {lock}") from error
    try:
        m.write_json(lock / "owner.json", c.snapshot_process(os.getpid()))
        yield
    finally:
        (lock / "owner.json").unlink(missing_ok=True)
        lock.rmdir()


def validate_patch(tree, request, patch):
    """Inspect Git's materialized index, not the agent's list of patch headers."""
    require(isinstance(patch, bytes) and 0 < len(patch) <= MAX_PATCH and b"\0" not in patch and
            b"GIT binary patch" not in patch and b"Binary files " not in patch, "Invalid/binary/oversized patch")
    require(not re.search(rb"(?m)^(?:(?:old|new|new file|deleted file) mode (?!100644\b)|"
                          rb"index [^\r\n]+ (?!100644\b)[0-9]{6}$|rename (?:from|to) )", patch),
            "Unsupported patch mode or rename")
    git(tree, "apply", "--index", "--whitespace=nowarn", "-", data=patch)
    raw = git(tree, "diff", "--cached", "--raw", "--no-abbrev", "--no-renames", "-z").split(b"\0")
    changes = []
    test_paths = {r["path"] for r in request["regressions"]}
    allowed = set(request["compiler_paths"]) | test_paths | {RELEASE_NOTE}
    for header, encoded_path in zip(raw[0::2], raw[1::2]):
        path = relative_path(encoded_path.decode("utf-8"))
        old, new, _, _, status = header.decode().lstrip(":").split()
        require(old in ("000000", "100644") and new == "100644" and status in ("A", "M"),
                "Deletions, renames, symlinks, submodules and executable modes are not allowed")
        registration = path in (COMPONENT, SERVICE)
        baseline = (path.endswith((".bsl", ".il", ".vsbsl")) and
                    any(path.startswith(str(PurePosixPath(p).with_suffix("")) + ".") for p in test_paths))
        fixture = (Path(path).suffix in (".fs", ".fsi", ".fsx", ".cs", ".bsl", ".il", ".vsbsl") and
                   any(path.startswith(str(PurePosixPath(p).with_suffix("")) + "/") for p in test_paths))
        require(path in allowed or baseline or fixture or registration, f"Out-of-scope patch path: {path}")
        if registration:
            before = git(tree, "show", f"HEAD:{path}").decode("utf-8").replace("\r\n", "\n")
            after = (tree / path).read_text(encoding="utf-8")
            additions = [line for line in after.splitlines(keepends=True)
                         if line not in before.splitlines(keepends=True)]
            require(additions and all(re.fullmatch(r'\s*<Compile Include="[^"]+\.fs"\s*/>\s*', line)
                                      for line in additions), "Only Compile item registration additions allowed")
            for line in additions:
                included = re.search(r'Include="([^"]+)"', line)[1].replace("\\", "/")
                require(relative_path(included) and
                        (PurePosixPath(path).parent / included).as_posix() in test_paths,
                        "Unrelated test registration")
            reduced = after
            for line in additions:
                reduced = reduced.replace(line, "", 1)
            require(reduced == before, "Test project properties/targets cannot change")
        require(not (tree / path).is_symlink(), "Symlink patch")
        changes.append(path)
    require(set(request["compiler_paths"]) <= set(changes) and test_paths <= set(changes) and
            RELEASE_NOTE in changes, "Compiler, regression and current release note changes required")
    for regression in request["regressions"]:
        added = git(tree, "diff", "--cached", "--unified=0", "--", regression["path"]).decode("utf-8")
        require(any(line.startswith("+") and not line.startswith("+++") and
                    regression["method"] in line for line in added.splitlines()),
                "Regression selector must identify an added or changed assertion")
    require(not any(line.startswith(b"-\t-\t") for line in
                    git(tree, "diff", "--cached", "--numstat").splitlines()), "Binary index change")
    return sorted(changes)


def prepare_candidate(repository, owned_root, request, patch, *, manifest=None):
    validate_request(request)
    proposed_root = Path(owned_root).absolute()
    require(not proposed_root.exists() and not proposed_root.is_symlink(), "Candidate needs a new owned root")
    root = proposed_root.resolve()
    require(root != Path(repository).resolve() and not Path(repository).resolve().is_relative_to(root),
            "Candidate cannot contain the source repository")
    c.private_directory(root)
    tree = root / "tree"
    identity = {"root": str(root), "device": root.stat().st_dev, "inode": root.stat().st_ino}
    m.write_json(root / "owner.json", identity)
    state = {"schema_version": 1, "request": request, "patch_sha256": hashlib.sha256(patch).hexdigest(),
             "tree": str(tree),
             "frozen": [m.file_record(p) for p in (__file__, mem.__file__, c.__file__, m.__file__)],
             "lifecycle": "experimenting", "outcome": None, "reason": "", "draft_eligible": False,
             "next_action": "validate", "changed_paths": [], "source_tree": None, "compiler": None}
    (root / "proposal.patch").write_bytes(patch)
    try:
        with owned_lock(root):
            if manifest is not None:
                frozen_records(manifest)
                require(request["base_sha"] == manifest["invariants"]["workload"]["revision"] and
                        request["primary_metric"] == manifest["invariants"]["experiment"]["primary_metric"],
                        "Request differs from frozen base/hypothesis")
                packages = Path(manifest["invariants"]["environment"]["variables"]["NUGET_PACKAGES"])
                package_records = manifest["invariants"]["toolchain"]["build_inputs"]["packages"]
                require({r["path"] for r in package_records} == {str(p.resolve()) for p in packages.rglob("*") if p.is_file()},
                        "Dependency cache inventory changed")
                require(package_records, "Frozen dependency cache unavailable")
                m.write_json(root / "packages.json", package_records)
                m.write_json(root / "workload.json", manifest)
                state["frozen"] += [m.file_record(root / name) for name in ("workload.json", "packages.json")]
            git(repository, "clone", "--quiet", "--local", "--no-hardlinks", "--dissociate", "--no-checkout", "--template=",
                "--", Path(repository).resolve(), tree, receipts=root)
            git(tree, "checkout", "--quiet", "--detach", request["base_sha"])
            state["changed_paths"] = validate_patch(tree, request, patch)
            state["source_tree"] = git(tree, "write-tree").decode().strip()
            state["inputs"] = [m.file_record(tree / p) for p in state["changed_paths"]]
            state["git_files"] = [m.file_record(tree / ".git" / p) for p in ("config", "HEAD", "index")]
    except (ValueError, OSError, subprocess.SubprocessError, KeyboardInterrupt) as error:
        state.update(outcome="cancelled" if isinstance(error, KeyboardInterrupt) else "rejected",
                     reason=c.sanitize(str(error)), lifecycle="interrupted", next_action="discard")
        if isinstance(error, KeyboardInterrupt):
            m.write_json(root / "candidate.json", state)
            state = discard_candidate(root)
        raise
    finally:
        m.write_json(root / "candidate.json", state)
    return state


def owned_root(path):
    require(not Path(path).is_symlink(), "Candidate root redirected")
    root = Path(path).resolve(strict=True)
    owner = m.read_json(root / "owner.json")
    require(owner == {"root": str(root), "device": root.stat().st_dev, "inode": root.stat().st_ino},
            "Unproven candidate ownership")
    tree = root / "tree"
    require(not tree.is_symlink() and tree.resolve() == tree, "Candidate tree redirected")
    return root


def verify_candidate(path):
    root = owned_root(path)
    state = m.read_json(root / "candidate.json")
    try:
        validate_request(state["request"])
        require(m.file_record(root / "proposal.patch")["sha256"] == state["patch_sha256"], "Proposed patch changed")
        tree = root / "tree"
        m.verify_files(state["git_files"] + state.get("frozen", []))
        require(git(tree, "rev-parse", "HEAD").decode().strip() == state["request"]["base_sha"] and
                git(tree, "write-tree").decode().strip() == state["source_tree"] and
                not git(tree, "diff", "--no-ext-diff", "--name-only"), "Candidate source/patch changed")
        require(not git(tree, "ls-files", "--others", "--exclude-standard"), "Untracked candidate source")
        m.verify_files(state["inputs"])
        if state["compiler"]:
            m.verify_files(list(state["compiler"]["files"].values()))
    except (ValueError, OSError, KeyError, TypeError) as error:
        state.update(draft_eligible=False, reason="Approval invalidated: " + c.sanitize(str(error)), next_action="discard")
        m.write_json(root / "candidate.json", state)
        raise
    return state


def correctness_plan(changes, regressions, target_framework):
    require(re.fullmatch(r"net\d+\.0", target_framework), "Invalid target framework")
    areas = {p.split("/")[2] for p in changes if p.startswith("src/Compiler/")}
    unknown = not areas or bool(areas - KNOWN_AREAS)
    api = any(p.endswith(".fsi") for p in changes)
    service = unknown or api or bool(areas - {"Checking", "CodeGen", "Optimize", "AbstractIL", "SyntaxTree"})
    codegen = unknown or bool(areas - {"Checking", "Service", "SyntaxTree"})
    plan = []

    def add(category, argv, result="exit", timeout=1800):
        plan.append({"id": f"{category}-{sum(p['category'] == category for p in plan)}",
                     "category": category, "argv": argv, "result": result, "timeout_seconds": timeout,
                     "platform_exclusions": result == "xunit" and category != "regression"})

    def build(project, category):
        add(category, ["build", project, "-c", "Release", "-f", target_framework,
                       "--disable-build-servers", "-p:UseSharedCompilation=false", *RELEASE_PROPERTIES])

    def test(project, category, selector=None):
        args = ["test", "--project", project, "-c", "Release", "-f", target_framework,
                "--no-build", "--results-directory", "{result_dir}", "--",
                "--minimum-expected-tests", "1", "--report-xunit", "--report-xunit-filename", "{result_name}"]
        if selector:
            args += list(selector)
        add(category, args, "xunit")

    build(m.PROJECT.as_posix(), "compiler-build")
    build(COMPONENT, "test-build")
    if service or any(r["path"].startswith("tests/FSharp.Compiler.Service.Tests/") for r in regressions):
        build(SERVICE, "test-build")
    for regression in regressions:
        test(SERVICE if regression["path"].startswith("tests/FSharp.Compiler.Service.Tests/") else COMPONENT,
             "regression", ("--filter-method", "*." + regression["method"], "--fail-skips", "on"))
        plan[-1]["regression"] = regression
    test(COMPONENT, "siblings")
    if codegen:
        test(COMPONENT, "emitted-il", ("--filter-class", "*EmittedIL*"))
        test(COMPONENT, "optimizer", ("--filter-class", "Optimizations.*"))
    if service:
        test(SERVICE, "service")
    if api:
        test(SERVICE, "surface-area", ("--filter-method", "*SurfaceArea*"))
    for lib, app in COMPAT_SCENARIOS:
        add("runtime" if (lib, app) == ("local", "local") else "compatibility",
            ["exec", "--roll-forward", "Major",
             COMPATIBILITY + "/CompilerCompatApp/bin/Release/net8.0/CompilerCompatApp.dll"], "compatibility")
        plan[-1]["scenario"] = [lib, app]
    return plan


def unredirected(path, *, missing_ok=False):
    for part in reversed((path, *path.parents)):
        try:
            info = part.lstat()
        except FileNotFoundError:
            if missing_ok:
                continue
            return False
        if stat.S_ISLNK(info.st_mode) or getattr(info, "st_file_attributes", 0) & stat.FILE_ATTRIBUTE_REPARSE_POINT:
            return False
    return True


def test_counts(path, *, platform_exclusions=False, required_test=None, allowed_skips=(), baseline=False):
    path = Path(path)
    require(unredirected(path) and path.is_file() and
            0 < path.stat().st_size <= MAX_REPORT,
            "Missing or oversized test result")
    data = path.read_bytes()
    require(b"<!DOCTYPE" not in data and b"<!ENTITY" not in data, "Invalid test XML")
    root = ET.fromstring(data)
    require(root.tag == "assemblies", "Expected xUnit result")
    assemblies = list(root.findall("assembly"))
    require(assemblies, "Missing test assemblies")
    counts = dict.fromkeys(("total", "passed", "failed", "skipped", "errors"), 0)
    all_tests = []
    for assembly in assemblies:
        tests = list(assembly.findall("./collection/test"))
        actual = {key: sum(t.get("result") == value for t in tests)
                  for key, value in (("passed", "Pass"), ("failed", "Fail"), ("skipped", "Skip"))}
        actual.update(total=len(tests), errors=len(assembly.findall("./errors/error")))
        require(all(t.get("result") in ("Pass", "Fail", "Skip") for t in tests) and
                all(int(assembly.attrib[k]) == v for k, v in actual.items()) and
                len(list(assembly.iter("test"))) == len(tests),
                "Test totals differ from individual results")
        for key, value in actual.items():
            counts[key] += value
        all_tests.extend(tests)
    require(len(list(root.iter("test"))) == len(all_tests), "Tests outside assembly collections")
    excluded = [{"name": t.get("name"), "reason": t.findtext("reason/message") or t.findtext("reason")}
                for t in all_tests if t.get("result") == "Skip"]
    require(counts["passed"] > 0 and counts["total"] == counts["passed"] + counts["skipped"] and
            counts["failed"] == counts["errors"] == 0 and counts["skipped"] == len(excluded) and
            all(mem.text(t["name"]) and mem.text(t["reason"]) and
                (baseline or t in allowed_skips or platform_exclusions and t["reason"] in PLATFORM_EXCLUSIONS)
                for t in excluded),
            "Required matched tests did not all pass")
    if required_test is not None:
        require(any(t.get("result") == "Pass" and t.get("method") == required_test["method"] and
                    t.get("source-file", "").replace("\\", "/") == required_test["path"].replace("\\", "/")
                    for t in all_tests), "Required regression source/method did not match")
    return {"matched": counts["passed"], "passed": counts["passed"], "excluded": excluded}


@contextmanager
def capture_report(path, destination, isolation):
    """A mounted FIFO cannot be replaced/truncated by a test's ProcessExit handler."""
    require(sys.platform == "linux", "Authenticated report capture requires Linux")
    path = Path(path)
    require(unredirected(path.parent) and not path.exists(), "Stale or redirected report channel")
    os.mkfifo(path, 0o600)
    descriptor = os.open(path, os.O_RDWR | os.O_NONBLOCK)
    path.chmod(0o200)
    finished = threading.Event()
    errors = []
    size = 0

    def drain():
        nonlocal size
        try:
            with Path(destination).open("xb") as output:
                while True:
                    readable = select.select([descriptor], [], [], 0.05)[0]
                    if not readable:
                        if finished.is_set():
                            break
                        continue
                    data = os.read(descriptor, 65536)
                    size += len(data)
                    if size <= MAX_REPORT:
                        output.write(data)
        except OSError as error:
            errors.append(error)

    reader = threading.Thread(target=drain, name="compiler-perf-report")
    isolation.sealed.append(path.parent)
    reader.start()
    try:
        yield
    finally:
        finished.set()
        reader.join()
        os.close(descriptor)
        isolation.sealed.remove(path.parent)
        path.unlink()
    require(not errors and 0 < size <= MAX_REPORT, "Missing, failed or oversized report capture")


def execution_signature(receipt):
    return hmac.digest(mem._VALIDATOR_KEY, b"execution\0" + mem.encoded(
        {k: v for k, v in receipt.items() if k != "execution"}), "sha256").hex()


def compact_correctness(root, checks):
    checks = dict(checks)
    require(checks["schema_version"] == 4, "Historical correctness cannot be upgraded")
    if isinstance(checks["inputs"], list):
        require(mem.identifier(checks["nonce"]), "Invalid inventory identity")
        path = Path(root) / (checks["nonce"] + "-inputs.json")
        m.write_json(path, checks["inputs"])
        checks["inputs"] = m.file_record(path)
    return checks


class Isolation:
    """Empty Linux mount namespace; no host home, network, credentials or validator files."""

    def __init__(self, tree, readonly=(), writable=(), sealed=()):
        require(sys.platform == "linux" and Path("/usr/bin/bwrap").is_file(),
                "Linux bubblewrap is required; host execution is never a fallback")
        self.tree = Path(tree).resolve(strict=True)
        self.readonly = [Path(p).resolve(strict=True) for p in readonly]
        self.writable = [Path(p).resolve(strict=True) for p in writable]
        self.sealed = [Path(p).resolve(strict=True) for p in sealed]
        self.tool = m.file_record("/usr/bin/bwrap")

    def argv(self, args, cwd, environment, accounts):
        (Path(accounts) / "passwd").write_text(
            f"compiler-perf:x:{os.getuid()}:{os.getgid()}::/tmp:/bin/sh\n")
        (Path(accounts) / "group").write_text(f"compiler-perf:x:{os.getgid()}:\n")
        command = [self.tool["path"], "--die-with-parent", "--new-session", "--unshare-all",
                   "--cap-drop", "ALL", "--clearenv", "--proc", "/proc", "--dev", "/dev",
                   "--tmpfs", "/tmp",
                   "--ro-bind", str(Path(accounts) / "passwd"), "/etc/passwd",
                   "--ro-bind", str(Path(accounts) / "group"), "/etc/group"]
        system = [Path(p) for p in ("/usr", "/bin", "/sbin", "/lib", "/lib64", "/etc/ld.so.cache",
                                   "/etc/localtime") if Path(p).exists()]
        for path in dict.fromkeys(system + self.readonly + [self.tree]):
            command += ["--ro-bind", str(path), str(path)]
        artifacts = self.tree / "artifacts"
        artifacts.mkdir(exist_ok=True)
        for path in [artifacts, *self.writable]:
            require(path != self.tree and not self.tree.is_relative_to(path), "Writable mount is too broad")
            if not path.exists():
                path.touch()
            command += ["--bind", str(path), str(path)]
        for path in self.sealed:
            command += ["--ro-bind", str(path), str(path)]
        for key, value in environment.items():
            require(key in (set(m.INHERITED_ENVIRONMENT_KEYS) | set(m.CONTROLLED_ENVIRONMENT) |
                            set(m.OWNED_ENVIRONMENT_KEYS) | m.LOCAL_ENVIRONMENT_KEYS),
                    "Non-allowlisted isolated environment")
            command += ["--setenv", key, value]
        return command + ["--chdir", str(cwd), "--", *args]

    def command(self, args, cwd, environment, receipts, phase, timeout=1800):
        started = datetime.now(timezone.utc).isoformat()
        logs = Path(receipts) / "commands"
        logs.mkdir(exist_ok=True)
        m.verify_files([self.tool])
        with tempfile.TemporaryDirectory(prefix="compiler-perf-accounts-") as accounts:
            invocation = self.argv(args, cwd, environment, accounts)
            result = c.run_tool(invocation, cwd, environment, logs, phase, timeout=timeout)
        result.update(command=args, cwd=str(cwd), started_at=started,
                      finished_at=datetime.now(timezone.utc).isoformat(), isolation=self.tool)
        return result

    def process(self, args, cwd, environment, log):
        m.verify_files([self.tool])
        with tempfile.TemporaryDirectory(prefix="compiler-perf-accounts-") as accounts:
            result = m.run_process(self.argv(args, cwd, environment, accounts), cwd, environment, log)
        result["isolation"] = self.tool
        return result


def frozen_records(manifest):
    m.validate_manifest(manifest, "workload")
    require(manifest["invariants"]["policy"] == m.POLICY, "Measurement policy changed")
    invariants = manifest["invariants"]
    root = Path(manifest["owned_root"]).resolve(strict=True)
    require(root.parent != root and Path(invariants["workload"]["checkout"]) == root / "workload" and
            all(Path(invariants["environment"]["variables"][k]) == root / v for k, v in
                (("NUGET_PACKAGES", "packages"), ("HOME", "home"), ("TMP", "tmp"))),
            "Workload/cache paths are not in their owned preparation tree")
    tool_root = Path(invariants["toolchain"]["dotnet"]["path"]).parent
    require(not root.is_relative_to(tool_root), "Tool installation would expose the validator's parent tree")
    build_inputs = invariants["toolchain"].get("build_inputs")
    require(type(build_inputs) is dict and set(build_inputs) == {"sdk", "bootstrap", "packages"} and
            all(type(v) is list and v for v in build_inputs.values()),
            "Frozen build/package identities missing; prepare a new workload, do not upgrade history")
    for directory, inventory in (
            (tool_root / "sdk", build_inputs["sdk"]),
            (tool_root / "shared" / "Microsoft.NETCore.App", invariants["toolchain"]["runtime_files"]),
            (root / "packages", build_inputs["packages"])):
        require({str(p.resolve()) for p in directory.rglob("*") if p.is_file()} == {r["path"] for r in inventory},
                "Frozen SDK/runtime/package inventory changed")
    records = c.frozen_inputs(manifest, manifest["baseline_compiler"])
    records += invariants["bootstrap"]["binaries"] + invariants["toolchain"]["sdk_compiler_files"]
    records += [invariants["toolchain"][k] for k in ("measurement", "capture_target")]
    records += [r for group in build_inputs.values() for r in group]
    m.verify_files(records)
    return records


def evaluate_candidate(baseline, candidate):
    return m.compare_measurements(baseline, candidate)


def verify_compatibility_output(output, expected):
    require("SUCCESS: All compiler compatibility tests passed" in output, "Missing runtime compatibility result")
    for section, (sdk, compiler, local) in zip(("Library", "Application"), expected):
        pattern = (re.escape(section) + r" Build Info:\s+SDK Version: ([^\r\n]+)\s+"
                   r"F# Compiler Path: ([^\r\n]+)\s+Is Local Build: (true|false)")
        found = re.search(pattern, output)
        require(found and tuple(s.strip() for s in found.groups()) ==
                (sdk, compiler, str(local).lower()), "Compatibility SDK/compiler execution differs")


def prepare_compatibility(root, manifest, compiler, isolation, environment, check, inputs):
    tree = root / "tree"
    directory = tree / COMPATIBILITY
    toolchain = manifest["invariants"]["toolchain"]
    dotnet = toolchain["dotnet"]["path"]
    historical = re.findall(r"(?m)^(9\.0\.(?:[3-9]\d\d|\d{4,}))\s", toolchain["sdks"])
    require(historical, "Required .NET 9 compatibility SDK unavailable")
    net9 = max(historical, key=lambda v: tuple(map(int, v.split("."))))
    commands, expected, files = [], [], []
    feed = directory / "local-nuget-packages"
    if feed.exists():
        require(unredirected(feed), "Redirected compatibility feed")
        shutil.rmtree(feed)
    feed.mkdir()
    package = Path(environment["NUGET_PACKAGES"]) / "compilercompatlib"
    require(not (Path(manifest["invariants"]["environment"]["variables"]["NUGET_PACKAGES"]) / "compilercompatlib").exists(),
            "Compatibility package must not be a frozen dependency")
    if package.exists():
        require(unredirected(package), "Redirected compatibility package")
        shutil.rmtree(package)
    configuration = m.read_json(tree / "global.json")
    for part, mode in zip(("Lib", "App"), check["scenario"]):
        project = directory / ("CompilerCompat" + part)
        for name in ("bin", "obj"):
            output = project / name
            if output.exists():
                require(unredirected(output), "Redirected compatibility output")
                shutil.rmtree(output)
        sdk = net9 if mode == "net9" else toolchain["sdk"]
        require(all(unredirected(directory / name, missing_ok=True) for name in ("global.json", "global.json.tmp")),
                "Redirected compatibility SDK configuration")
        configuration["sdk"] = {"version": sdk, "rollForward": "disable"}
        m.write_json(directory / "global.json", configuration)
        selected = (compiler["files"]["fsc.dll"]["path"] if mode == "local" else
                    str(Path(dotnet).parent / "sdk" / sdk / "FSharp" / "fsc.dll"))
        m.verify_files([m.file_record(selected)])
        expected.append([sdk, selected, mode == "local"])
        properties = [f"-p:LoadLocalFSharpBuild={'True' if mode == 'local' else 'False'}",
                      "-p:LocalFSharpCompilerConfiguration=Release", f"-p:LocalFSharpCompilerPath={tree}",
                      "-p:PackageVersion=1.0.1", "--disable-build-servers", "-p:UseSharedCompilation=false"]
        arguments = (["pack", "CompilerCompatLib.fsproj", "-o", str(feed)] if part == "Lib" else
                     ["build", "CompilerCompatApp.fsproj", "--source", str(feed)])
        # External packages are already frozen in the private cache; only the fixture package is new.
        for suffix in (["--version"], [*arguments, "-c", "Release", *properties]):
            frozen_records(manifest)
            m.verify_files(inputs)
            verify_candidate(root)
            result = isolation.command([dotnet, *suffix], project, environment, root,
                                       check["id"] + "-" + part)
            commands.append(result)
            m.write_json(root / (check["id"] + "-preparation.json"), commands)
            require(result["status"] == "collected" and result["exit_status"] == 0,
                    "Required compatibility preparation failed")
            if suffix == ["--version"]:
                require(result["stdout"].strip() == sdk, "Historical SDK was not selected")
            files.extend(result["logs"])
            frozen_records(manifest)
            m.verify_files(inputs)
            verify_candidate(root)
        info = project / (part + "BuildInfo.fs")
        require(unredirected(info) and info.is_file(), "Missing compatibility build identity")
        destination = root / (uuid.uuid4().hex + "-" + info.name)
        shutil.copyfile(info, destination)
        files.append(m.file_record(destination))
    (directory / "global.json").unlink()
    return {"expected": expected, "commands": commands, "files": files}


def validate_correctness(checks, candidate, policy, *, current=False, passed=True):
    require(type(checks.get("schema_version")) is int and checks["schema_version"] in (2, 3, 4) and
            (not current or checks["schema_version"] == 4),
            "Historical correctness is not a fresh validator receipt")
    plan = (checks["plan"] if checks["schema_version"] < 4 else
            correctness_plan(checks["changes"], checks["regressions"], checks["target_framework"]))
    require(checks["plan"] == plan and checks["plan_sha256"] == m.fingerprint_inputs(plan) and
            checks["required"] == [p["id"] for p in plan] and
            {"compiler-build", "test-build", "regression", "siblings", "runtime", "compatibility"} <=
            {p["category"] for p in plan}, "Correctness plan changed or weakened")
    results = checks["results"]
    require(len(results) <= len(plan) and len({r["check_id"] for r in results}) == len(results) and
            (not passed or len(results) == len(plan)),
            "Missing required correctness categories")
    cwd = Path(checks["cwd"]) if current else m.absolute_path(checks["cwd"])
    for check, result in zip(plan, results):
        path = type(cwd)(result["result_path"])
        expected = [checks["tool"]["path"], *[a.format(result_dir=str(path.parent), result_name=path.name) for a in check["argv"]]]
        require(result["check_id"] == check["id"] and result["command"] == expected and
                result["cwd"] == checks["cwd"] and result["tool"] == checks["tool"] and
                type(result["exit_status"]) is int and (not passed or result["exit_status"] == 0) and
                result["candidate"] == candidate and result["policy"] == policy and
                result["nonce"] == checks["nonce"] and result["category"] == check["category"] and
                mem.timestamp(result["finished_at"]) >= mem.timestamp(result["started_at"]),
                "Failed, forged, stale or mismatched correctness receipt")
        require(not passed or check["result"] == "exit" or
                (type(result["matched"]) is int and result["matched"] > 0), "Zero matched tests")
        excluded = result.get("excluded_count", 0)
        require(type(excluded) is int and excluded >= 0 and
                (excluded == 0 or check["platform_exclusions"] and mem.digest(result.get("excluded_sha256"))),
                "Skipped required correctness check")
        if passed and "regression" in check:
            require(result.get("regression") == dict(check["regression"],
                    path=str(cwd / check["regression"]["path"])), "Missing regression source/method receipt")
        require(result["files"], "Missing correctness output evidence")
        if current:
            m.verify_files(result["files"] + [checks["tool"]])
            if check["result"] == "xunit":
                allowed = ()
                if check["platform_exclusions"]:
                    base = result["baseline"]
                    require(base["report"] in base["files"] and base["exit_status"] == 0 and
                            base["check_id"] == check["id"], "Missing matching baseline skip evidence")
                    m.verify_files(base["files"])
                    allowed = test_counts(base["report"]["path"], baseline=True)["excluded"]
                counts = test_counts(result["report"]["path"], allowed_skips=allowed,
                                     required_test=result.get("regression"))
                require(result["report"] in result["files"] and result["matched"] == counts["matched"] and
                        result["excluded_count"] == len(counts["excluded"]) and
                        result["excluded_sha256"] == m.fingerprint_inputs(counts["excluded"]),
                        "Test receipt differs from raw results")
            elif check["result"] == "compatibility":
                evidence = result["compatibility"]
                require(len(evidence["commands"]) == 4 and
                        all(r["status"] == "collected" and r["exit_status"] == 0
                            for r in evidence["commands"]), "Incomplete compatibility execution")
                m.verify_files(evidence["files"])
                verify_compatibility_output(result["stdout"], evidence["expected"])
            require(isinstance(result.get("execution"), str) and
                    hmac.compare_digest(result["execution"], execution_signature(result)),
                    "A fresh trusted validator execution capture is required; receipt changed or stale")
    if checks["schema_version"] == 4:
        m.validate_file_record(checks["inputs"])
    if current:
        inventory = checks["inputs"]
        m.verify_files([inventory])
        records = c.read_bounded_json(inventory["path"])
        require(isinstance(records, list) and records, "Missing correctness input inventory")
        m.verify_files(records)
        m.verify_files([inventory])


def run_correctness(root, manifest, compiler, isolation, *, baseline=False):
    state = verify_candidate(root)
    tree = isolation.tree
    invariants = manifest["invariants"]
    dotnet = invariants["toolchain"]["dotnet"]
    plan = correctness_plan(state["changed_paths"], state["request"]["regressions"],
                            invariants["workload"]["target_framework"])
    nonce = uuid.uuid4().hex
    receipt_path = root / ("baseline-correctness.json" if baseline else "correctness.json")
    if receipt_path.exists():
        previous = m.read_json(receipt_path)
        shutil.copyfile(receipt_path, root / (previous["nonce"] + "-correctness.json"))
    results_dir = tree / "artifacts" / "perf-results" / nonce
    require(unredirected(results_dir, missing_ok=True), "Redirected correctness results")
    results_dir.mkdir(parents=True)
    checks = {"schema_version": 4, "changes": state["changed_paths"], "regressions": state["request"]["regressions"],
              "target_framework": invariants["workload"]["target_framework"], "plan": plan,
              "plan_sha256": m.fingerprint_inputs(plan), "required": [p["id"] for p in plan],
              "results": [], "nonce": nonce, "cwd": str(tree), "tool": dotnet,
              "inputs": state["inputs"] + state["git_files"] + state.get("frozen", []) +
                        [m.file_record(root / "proposal.patch")] + list(compiler["files"].values()), "validation": None}
    require(all(unredirected(tree / "artifacts" / "validation" / name, missing_ok=True)
                for name in ("home", "tmp", "packages")), "Redirected correctness environment")
    environment = m.allowed_environment(tree / "artifacts" / "validation")
    environment.update(DOTNET_ROOT=str(Path(dotnet["path"]).parent),
                       DOTNET_HOST_PATH=dotnet["path"],
                       DOTNET_INSTALL_DIR=str(Path(dotnet["path"]).parent),
                       PATH=str(Path(dotnet["path"]).parent) + os.pathsep + os.defpath,
                       NUGET_PACKAGES=str(tree / "artifacts" / "validation" / "packages"))
    packages = Path(environment["NUGET_PACKAGES"])
    frozen_packages = Path(invariants["environment"]["variables"]["NUGET_PACKAGES"])
    require(unredirected(packages) and all(unredirected(p) for p in packages.rglob("*")),
            "Correctness cache contains redirected paths")
    shutil.copytree(frozen_packages, packages, dirs_exist_ok=True)
    copied = [{"path": str(packages / Path(r["path"]).relative_to(frozen_packages)), "sha256": r["sha256"]}
              for r in invariants["toolchain"]["build_inputs"]["packages"]]
    m.verify_files(copied)
    checks["inputs"] += copied
    base_results = {}
    if baseline:
        build = isolation.command(PACK_COMMAND, tree, environment, root, "baseline-pack", 3600)
        m.write_json(root / (nonce + "-baseline-pack.json"), build)
        require(build["status"] == "collected" and build["exit_status"] == 0, "Baseline Release pack failed")
        compiler = m.compiler_manifest(tree / "artifacts" / "bin" / "fsc" / "Release" /
                                       invariants["workload"]["target_framework"], state["request"]["base_sha"],
                                       hashlib.sha256(b"").hexdigest(), invariants["bootstrap"], invariants["toolchain"])
        checks["compiler"] = compiler
    else:
        base_tree = tree / "artifacts" / ("baseline-" + nonce)
        git(tree, "clone", "--quiet", "--local", "--no-hardlinks", "--no-checkout", "--template=",
            "--", tree, base_tree)
        git(base_tree, "checkout", "--quiet", "--detach", state["request"]["base_sha"])
        shutil.copytree(Path(manifest["owned_root"]) / "bootstrap" / "artifacts" / "Bootstrap",
                        base_tree / "artifacts" / "Bootstrap")
        base_isolation = Isolation(base_tree, isolation.readonly)
        base_checks = run_correctness(root, manifest, compiler, base_isolation, baseline=True)
        base_results = {r["check_id"]: r for r in base_checks["results"]}
        checks["inputs"].append(m.file_record(root / "baseline-correctness.json"))
    binaries = list(compiler["files"].values())
    checks["inputs"] += binaries
    probed = set()
    for check in plan:
        if baseline and not (check["category"] in ("compiler-build", "test-build") or check["platform_exclusions"]):
            continue
        frozen_records(manifest)
        m.verify_files(binaries)
        verify_candidate(root)
        if check["argv"][0] == "test" and check["argv"][2] not in probed:
            project = check["argv"][2]
            help_result = isolation.command([dotnet["path"], "test", "--project", project, "-c", "Release",
                                             "--no-build", "--help"], tree, environment, root, "mtp-help")
            require(help_result["status"] == "collected" and help_result["exit_status"] == 0 and all(flag in help_result["stdout"] for flag in
                    ("--filter-method", "--filter-class", "--fail-skips", "--report-xunit", "--report-xunit-filename")),
                    "Pinned MTP runner does not expose required selectors/results")
            probed.add(project)
            outputs = list((tree / "artifacts" / "bin" / Path(project).stem / "Release").rglob("FSharp.Compiler.Service.dll"))
            require(outputs and (baseline or all(
                m.file_record(p)["sha256"] == compiler["files"]["FSharp.Compiler.Service.dll"]["sha256"] for p in outputs)),
                "Tests did not load candidate compiler service")
        result_path = str(results_dir / (check["id"] + ".xml"))
        require(not Path(result_path).exists(), "Stale correctness result")
        argv = [dotnet["path"], *[arg.format(result_dir=str(results_dir), result_name=Path(result_path).name) for arg in check["argv"]]]
        compatibility = (prepare_compatibility(root, manifest, compiler, isolation, environment, check, copied + binaries)
                         if check["result"] == "compatibility" else None)
        destination = root / f"{nonce}-{check['id']}.xml"
        capture = (capture_report(result_path, destination, isolation)
                   if check["result"] == "xunit" else nullcontext())
        with capture:
            result = isolation.command(argv, tree, environment, root, check["id"], check["timeout_seconds"])
            m.write_json(root / f"{nonce}-{check['id']}-command.json", result)
        receipt = {key: result[key] for key in ("command", "cwd", "exit_status", "started_at", "finished_at")}
        receipt.update(check_id=check["id"], category=check["category"], nonce=nonce, tool=dotnet,
                       candidate=m.fingerprint_inputs(compiler), policy=m.fingerprint_inputs(m.POLICY),
                       matched=None, result_path=result_path, files=result["logs"])
        checks["results"].append(receipt)
        m.write_json(receipt_path, checks)
        require(result["exit_status"] == 0 and result["status"] == "collected",
                f"Required check failed: {check['id']}")
        if check["result"] == "xunit":
            regression = (dict(check["regression"], path=str(tree / check["regression"]["path"]))
                          if "regression" in check else None)
            base = base_results.get(check["id"])
            allowed = test_counts(base["report"]["path"], baseline=True)["excluded"] if base else ()
            counts = test_counts(destination, allowed_skips=allowed, baseline=baseline, required_test=regression)
            receipt.update(matched=counts["matched"], excluded_count=len(counts["excluded"]),
                           excluded_sha256=m.fingerprint_inputs(counts["excluded"]))
            if regression:
                receipt["regression"] = regression
            receipt["report"] = m.file_record(destination)
            receipt["files"].append(receipt["report"])
            if base:
                receipt["baseline"] = base
        elif check["result"] == "compatibility":
            verify_compatibility_output(result["stdout"], compatibility["expected"])
            receipt.update(matched=1, compatibility=compatibility, stdout=result["stdout"])
        frozen_records(manifest)
        m.verify_files(copied + binaries)
        verify_candidate(root)
        receipt["execution"] = execution_signature(receipt)
        m.write_json(receipt_path, checks)
    return checks


def build_candidate(root, manifest, isolation):
    state = verify_candidate(root)
    tree = root / "tree"
    invariants = manifest["invariants"]
    bootstrap = Path(manifest["owned_root"]) / "bootstrap"
    records = frozen_records(manifest)
    copied = []
    for record in invariants["toolchain"]["build_inputs"]["bootstrap"]:
        relative = Path(record["path"]).relative_to(bootstrap)
        require(relative.parts[0] == "artifacts", "Bootstrap outside recorded build")
        target = tree / relative
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(record["path"], target)
        copied.append({"path": str(target), "sha256": record["sha256"]})
    require(all(unredirected(tree / "artifacts" / "validation" / name, missing_ok=True)
                for name in ("home", "tmp", "packages")), "Redirected build environment")
    environment = m.allowed_environment(tree / "artifacts" / "validation")
    dotnet = invariants["toolchain"]["dotnet"]["path"]
    environment.update(DOTNET_ROOT=str(Path(dotnet).parent), DOTNET_HOST_PATH=dotnet,
                       DOTNET_INSTALL_DIR=str(Path(dotnet).parent),
                       PATH=str(Path(dotnet).parent) + os.pathsep + os.defpath,
                       NUGET_PACKAGES=invariants["environment"]["variables"]["NUGET_PACKAGES"])
    for suffix, expected in ((["--version"], invariants["toolchain"]["sdk"]),
                             (["--list-sdks"], invariants["toolchain"].get("sdks", "")),
                             (["--list-runtimes"], invariants["toolchain"]["runtime"])):
        probe = isolation.command([dotnet, *suffix], tree, environment, root, "toolchain", 60)
        m.write_json(root / ("toolchain-" + suffix[0].lstrip("-") + ".json"), probe)
        require(probe["status"] == "collected" and probe["exit_status"] == 0 and
                probe["stdout"].strip() == expected.strip(), "SDK/runtime changed")
    require(any(int(version) >= 300 for version in re.findall(r"(?m)^9\.0\.(\d+)\s", invariants["toolchain"]["sdks"])),
            "Required compatibility SDK 9.0.300 or later is unavailable in the frozen toolchain")
    pinned = m.read_json(tree / "global.json")["sdk"]["version"]
    require(pinned == invariants["toolchain"]["sdk"], "Pinned SDK unavailable; global.json must not change")
    command = PACK_COMMAND
    require(any(r["arguments"] == command and r["exit_status"] == 0
                for r in invariants["bootstrap"]["commands"]), "Release pack flags differ from recorded bootstrap")
    build = isolation.command(command, tree, environment, root, "candidate-pack", 3600)
    m.write_json(root / "build.json", build)
    require(build["status"] == "collected" and build["exit_status"] == 0, "Candidate Release pack failed")
    m.verify_files(records + copied)
    verify_candidate(root)
    installation = tree / "artifacts" / "bin" / "fsc" / "Release" / invariants["workload"]["target_framework"]
    require(unredirected(installation) and
            all(unredirected(p) for p in installation.rglob("*")), "Candidate installation contains redirected paths")
    compiler = m.compiler_manifest(installation, state["request"]["base_sha"], state["patch_sha256"],
                                   invariants["bootstrap"], invariants["toolchain"])
    require(compiler["files"]["fsc.runtimeconfig.json"]["sha256"] ==
            invariants["toolchain"]["compiler_runtimeconfig_sha256"], "Candidate runtime configuration changed")
    require(compiler["files"]["FSharp.Compiler.Service.dll"]["sha256"] !=
            manifest["baseline_compiler"]["files"]["FSharp.Compiler.Service.dll"]["sha256"],
            "Candidate compiler binary is not distinct")
    state["compiler"] = compiler
    m.write_json(root / "candidate.json", state)
    return compiler


def candidate_attempt(state, manifest, profile=None, correctness=None, documents=None):
    request = state["request"]
    profile = profile if profile and profile.get("status") == "collected" else None
    invariants = manifest["invariants"]
    measurement = mem.measurement_record(documents["baseline"], documents["candidate"]) if documents else None
    now = datetime.now(timezone.utc).isoformat()
    fingerprints = (dict(measurement["fingerprints"]) if measurement else
                    {key: m.fingerprint_inputs(invariants[key]) for key in ("workload", "bootstrap", "toolchain", "policy")})
    fingerprints.update(source=m.fingerprint_inputs(request["base_sha"]), input=m.fingerprint_inputs(manifest),
                        capture=invariants["toolchain"]["measurement"]["sha256"], reducer=m.file_record(m.__file__)["sha256"])
    if state["compiler"]:
        fingerprints["candidate"] = m.fingerprint_inputs(state["compiler"])
    if correctness is None:
        plan = correctness_plan(state["changed_paths"], request["regressions"], invariants["workload"]["target_framework"])
        correctness = {"schema_version": 4, "changes": state["changed_paths"], "regressions": request["regressions"],
                       "target_framework": invariants["workload"]["target_framework"], "plan": plan,
                       "plan_sha256": m.fingerprint_inputs(plan), "required": [p["id"] for p in plan],
                       "results": [], "nonce": uuid.uuid4().hex, "cwd": state["tree"],
                       "tool": invariants["toolchain"]["dotnet"], "inputs": state["inputs"], "validation": None}
    correctness = compact_correctness(Path(state["tree"]).parent, correctness)
    outcome = measurement["comparison"]["decision"] if measurement else state["outcome"]
    reason = ("; ".join(measurement["comparison"]["reasons"]) or "Independent correctness and measurement gates passed"
              if measurement else state["reason"])
    return {
        "id": request["attempt_id"], "source_revision": request["base_sha"],
        "hypothesis": {"id": request["attempt_id"], "description": request["hypothesis"],
                       "area": request["compiler_paths"][0].split("/")[2],
                       "operation": ";".join(request["compiler_paths"])},
        "run": {"id": request["run_id"], "attempt": 1,
                "url": f"https://github.com/dotnet/fsharp/actions/runs/{request['run_id']}"},
        "lifecycle": "validating" if measurement else "interrupted", "outcome": outcome, "reason": reason,
        "fingerprints": fingerprints,
        "dependencies": {"paths": {p: m.file_record(Path(manifest["invariants"]["workload"]["checkout"]) / p)["sha256"]
                                    for p in request["compiler_paths"]
                                    if (Path(manifest["invariants"]["workload"]["checkout"]) / p).is_file()},
                         "workload": fingerprints["workload"], "sdk": manifest["invariants"]["toolchain"]["sdk"],
                         "collector_permission": True if profile else None},
        "retry": {"when_any": [{"kind": "workload", "value": fingerprints["workload"]}],
                  "reason": "Retry only after relevant frozen inputs change"},
        "summary": {"evidence": profile["evidence_manifest"]["sha256"] if profile else "No completed diagnostic evidence",
                    "change": request["complexity"],
                    "patch_sha256": state["patch_sha256"]},
        "evidence": [], "measurement": measurement,
        "collection": {"status": "collected" if profile else "blocked_collection" if outcome == "blocked_collection" else "pending",
                       "reason": reason if outcome == "blocked_collection" else ""},
        "correctness": correctness, "publication": None, "supersedes": None, "notes": [],
        "lease": {"run_id": request["run_id"], "run_attempt": 1, "checkpoint_at": now, "expires_at": now},
        "checkpoint": {"next_action": "Human complexity and draft review; no automatic publication",
                       "required_evidence": [], "candidate_ref": state["source_tree"]},
    }


def validate_candidate(path, profile_path, *, checkpoint_on_cancel=False):
    root = owned_root(path)
    phase = "setup"
    cancelled = False
    manifest = profile = None
    with owned_lock(root):
        state = m.read_json(root / "candidate.json")
        resume = state["outcome"] == "cancelled" and "checkpoint" in state
        require(state["outcome"] is None or resume, "Terminal experiments require a new attempt, not replacement")
        state.update(lifecycle="validating", draft_eligible=False)
        m.write_json(root / "candidate.json", state)
        try:
            state = verify_candidate(root)
            manifest = c.read_bounded_json(root / "workload.json")
            if resume:
                require(state["checkpoint"] == m.fingerprint_inputs(
                    [state["source_tree"], state["patch_sha256"], manifest]), "Interrupted checkpoint fingerprints differ")
            invariants = manifest["invariants"]
            frozen_records(manifest)
            require(state["request"]["base_sha"] == invariants["workload"]["revision"] and
                    state["request"]["primary_metric"] == invariants["experiment"]["primary_metric"],
                    "Frozen base/hypothesis differs")
            packages = Path(invariants["environment"]["variables"]["NUGET_PACKAGES"])
            package_records = c.read_bounded_json(root / "packages.json")
            m.verify_files(package_records)
            require({r["path"] for r in package_records} == {str(p.resolve()) for p in packages.rglob("*") if p.is_file()},
                    "Dependency cache inventory changed")
            tool_root = Path(invariants["toolchain"]["dotnet"]["path"]).parent
            tree = root / "tree"
            compatibility = tree / COMPATIBILITY
            readonly = [tool_root, packages]
            isolation = Isolation(tree, readonly, [compatibility])
            compiler = state["compiler"] or build_candidate(root, manifest, isolation)
            state = verify_candidate(root)
            m.verify_files(package_records)
            phase = "correctness"
            correctness = run_correctness(root, manifest, compiler, isolation)
            m.verify_files(package_records)
            phase = "collection"
            profile = c.analyze_profile(profile_path)
            require(profile["status"] == "collected", profile.get("reason", "Diagnostic collection incomplete"))
            evidence = c.read_bounded_json(profile_path)
            require(evidence["fingerprints"] == {k: m.fingerprint_inputs(v) for k, v in invariants.items()} and
                    evidence["compiler"] == manifest["baseline_compiler"], "Diagnostic evidence belongs to different inputs")
            m.write_json(root / "profile.json", profile)
            phase = "measurement"
            records = frozen_records(manifest)
            outputs = [Path(p) for p in invariants["workload"]["outputs"]]
            for output in outputs:
                output.parent.mkdir(parents=True, exist_ok=True)
                output.touch(exist_ok=True)
            variables = invariants["environment"]["variables"]
            writable = outputs + list(dict.fromkeys(Path(variables[k]) for k in m.OWNED_ENVIRONMENT_KEYS
                                                    if k != "NUGET_PACKAGES"))
            preparation = Path(manifest["owned_root"])
            readonly = [tool_root, packages, preparation / "bootstrap", preparation / "workload"]
            readonly += [Path(r["path"]) for r in records
                         if not any(Path(r["path"]).is_relative_to(p) for p in readonly)]
            replay = Isolation(tree, readonly, writable, [Path(compiler["files"]["fsc.dll"]["path"]).parent])
            run = root / "runs" / uuid.uuid4().hex
            run.mkdir(parents=True)
            documents = m.run_repetitions(manifest, compiler, run / "candidate-samples.json",
                                           process_runner=replay.process)
            raw_records = []
            for arm, name in (("baseline", "candidate-samples.baseline.json"), ("candidate", "candidate-samples.json")):
                raw = run / name
                require(unredirected(raw) and c.read_bounded_json(raw) == documents[arm],
                        "Raw measurement receipt is missing or differs from collection")
                raw_records.append(m.file_record(raw))
            frozen_records(manifest)
            m.verify_files(package_records)
            state = verify_candidate(root)
            state["raw_measurements"] = raw_records
            correctness["inputs"] += raw_records + [profile["evidence_manifest"]]
            phase = "approval"
            attempt = candidate_attempt(state, manifest, profile, correctness, documents)
            if attempt["outcome"] == "measured_improvement":
                mem._attest_correctness(attempt)
                mem.acceptance_ready(attempt)
                head = git(tree, "-c", "user.name=Compiler performance validator",
                           "-c", "user.email=compiler-perf@example.invalid",
                           "commit-tree", state["source_tree"], "-p", state["request"]["base_sha"],
                           "-m", "Validated disposable compiler experiment\n\n"
                           "Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>").decode().strip()
                attempt["publication"] = mem.publication_intent(attempt, head)
                attempt["lifecycle"] = "publication_pending"
            journal = mem.merge_attempt(mem.empty_state(), attempt)
            m.write_json(root / "attempt.json", journal)
            state.update(outcome=attempt["outcome"], reason=attempt["reason"],
                         draft_eligible=attempt["publication"] is not None, lifecycle=attempt["lifecycle"],
                         next_action=attempt["checkpoint"]["next_action"] if attempt["publication"] else "discard")
        except (ValueError, OSError, KeyError, TypeError, ET.ParseError,
                subprocess.SubprocessError, KeyboardInterrupt) as error:
            cancelled = isinstance(error, KeyboardInterrupt)
            outcome = ("cancelled" if cancelled else "blocked_collection" if phase in ("collection", "measurement") else
                       "rejected" if phase == "correctness" else "blocked_setup")
            state.update(outcome=outcome, reason=f"{phase}: {c.sanitize(str(error)) or outcome}",
                         lifecycle="interrupted", draft_eligible=False, next_action="discard")
            if isinstance(error, c.CollectionCancelled):
                m.write_json(root / "cancelled-command.json", error.result)
            if cancelled and checkpoint_on_cancel:
                try:
                    verified = verify_candidate(root)
                    frozen_records(manifest)
                    state.update(compiler=verified["compiler"],
                                 next_action="Resume only with matching checkpoint fingerprints",
                                 checkpoint=m.fingerprint_inputs([state["source_tree"], state["patch_sha256"], manifest]))
                except (ValueError, OSError, KeyError, TypeError) as failure:
                    state["reason"] += "; checkpoint refused: " + c.sanitize(str(failure))
                    checkpoint_on_cancel = False
            if manifest is not None and not (cancelled and checkpoint_on_cancel):
                try:
                    checks = m.read_json(root / "correctness.json") if (root / "correctness.json").exists() else None
                    attempt = candidate_attempt(state, manifest, profile, checks)
                    m.write_json(root / "attempt.json", mem.merge_attempt(mem.empty_state(), attempt))
                except (ValueError, OSError, KeyError, TypeError) as failure:
                    state["reason"] += "; journal blocked: " + c.sanitize(str(failure))
        finally:
            m.write_json(root / "candidate.json", state)
    if not state["draft_eligible"] and not (cancelled and checkpoint_on_cancel):
        discard_candidate(root)
    if cancelled:
        raise KeyboardInterrupt(state["reason"])
    return m.read_json(root / "candidate.json")


def complete_no_finding(path, profile_path):
    root = owned_root(path)
    cancelled = False
    with owned_lock(root):
        state = m.read_json(root / "candidate.json")
        require(state["outcome"] is None and not (root / "runs").exists(),
                "An existing measurement outcome cannot become a healthy no-finding")
        phase = "setup"
        try:
            state = verify_candidate(root)
            manifest = c.read_bounded_json(root / "workload.json")
            frozen_records(manifest)
            phase = "collection"
            profile = c.analyze_profile(profile_path)
            require(profile["status"] == "collected", "No-finding requires completed diagnostic collection")
            evidence = c.read_bounded_json(profile_path)
            require(evidence["fingerprints"] == {k: m.fingerprint_inputs(v) for k, v in manifest["invariants"].items()} and
                    evidence["compiler"] == manifest["baseline_compiler"], "Diagnostic evidence belongs to different inputs")
            m.write_json(root / "profile.json", profile)
            state.update(outcome="completed_no_finding", draft_eligible=False,
                         reason="Completed diagnostic collection; hypothesis withdrawn without a performance claim",
                         next_action="discard")
            m.write_json(root / "attempt.json", mem.merge_attempt(
                mem.empty_state(), candidate_attempt(state, manifest, profile)))
        except (ValueError, OSError, KeyError, TypeError, KeyboardInterrupt) as error:
            cancelled = isinstance(error, KeyboardInterrupt)
            state.update(outcome="cancelled" if cancelled else "blocked_" + phase, draft_eligible=False,
                         reason=c.sanitize(str(error)) or "cancelled", next_action="discard")
        finally:
            m.write_json(root / "candidate.json", state)
    result = discard_candidate(root)
    if cancelled:
        raise KeyboardInterrupt(result["reason"])
    return result


def discard_candidate(path):
    root = owned_root(path)
    with owned_lock(root):
        state = m.read_json(root / "candidate.json")
        require(not state["draft_eligible"], "Accepted candidate is retained for human draft review")
        tree = root / "tree"
        if tree.exists():
            require(((tree / ".git").is_dir() and not (tree / ".git").is_symlink()) or
                    (state["source_tree"] is None and not (tree / ".git").exists()),
                    "Unproven disposable Git tree")
            state["discarded_diff_sha256"] = (hashlib.sha256(
                git(tree, "diff", "HEAD", "--binary", "--no-ext-diff")).hexdigest() if state["source_tree"] else None)
            def remove_readonly(function, name, error):
                target = Path(name)
                require(os.name == "nt" and isinstance(error[1], PermissionError) and
                        not target.is_symlink() and target.resolve().is_relative_to(tree) and target.is_file(),
                        f"Cannot remove owned file: {name}")
                target.chmod(stat.S_IWRITE)
                function(name)
            shutil.rmtree(tree, onerror=remove_readonly)
        state.update(lifecycle="interrupted", next_action="Review retained failure evidence", candidate_retained=False)
        m.write_json(root / "candidate.json", state)
        return state


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="operation", required=True)
    prepare = commands.add_parser("prepare")
    prepare.add_argument("--repository", required=True)
    prepare.add_argument("--request", required=True)
    prepare.add_argument("--patch", required=True)
    prepare.add_argument("--manifest", required=True)
    validate = commands.add_parser("validate")
    validate.add_argument("--profile", required=True)
    validate.add_argument("--checkpoint-on-cancel", action="store_true")
    validate.add_argument("--no-finding", action="store_true",
                          help="Withdraw an unmeasured hypothesis only after validating complete diagnostic evidence")
    for command in (prepare, validate, commands.add_parser("discard")):
        command.add_argument("--owned-root", required=True)
    options = parser.parse_args(argv)
    try:
        if options.operation == "prepare":
            request = c.read_bounded_json(options.request)
            with Path(options.patch).open("rb") as stream:
                patch = stream.read(MAX_PATCH + 1)
            manifest = c.read_bounded_json(options.manifest)
            frozen_records(manifest)
            require(request["base_sha"] == manifest["invariants"]["workload"]["revision"] and
                    request["primary_metric"] == manifest["invariants"]["experiment"]["primary_metric"],
                    "Request differs from frozen base/hypothesis")
            prepare_candidate(options.repository, options.owned_root, request, patch, manifest=manifest)
        elif options.operation == "validate":
            if options.no_finding:
                result = complete_no_finding(options.owned_root, options.profile)
                return 0 if result["outcome"] == "completed_no_finding" else 2
            result = validate_candidate(Path(options.owned_root), options.profile,
                                        checkpoint_on_cancel=options.checkpoint_on_cancel)
            return 0 if result["draft_eligible"] else 2
        else:
            discard_candidate(options.owned_root)
    except KeyboardInterrupt:
        return 130
    except (ValueError, OSError, KeyError, TypeError, ET.ParseError, subprocess.SubprocessError) as error:
        parser.exit(2, f"error: {c.sanitize(str(error))}\n")
    return 0


if __name__ == "__main__":
    sys.exit(main())
