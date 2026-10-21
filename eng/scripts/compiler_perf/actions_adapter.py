"""Fixed-destination Actions provisioning and transports. Host only, Python -I."""

import argparse
from datetime import datetime, timezone
import hashlib
import os
from pathlib import Path
import re
import subprocess
import sys
import tarfile
import tempfile
from urllib.request import urlopen

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import runtime

w, require = runtime.w, runtime.require
PYTHON_URL = ("https://github.com/astral-sh/python-build-standalone/releases/download/20250409/"
              "cpython-3.12.10%2B20250409-x86_64-unknown-linux-gnu-install_only_stripped.tar.gz")
PYTHON_SHA = "8c59b9ac6bff2dc3934181d7bc82594f9f59a613afed8d72c9e89d7194e790ee"
AGE_URL = "https://github.com/FiloSottile/age/releases/download/v1.2.1/age-v1.2.1-linux-amd64.tar.gz"
AGE_SHA = "7df45a6cc87d4da11cc03a539a7470c15b1041ab2b396af088fe9990f7c79d50"


def owner_configuration():
    require(os.environ.get("GH_TOKEN"), "blocked_setup: COMPILER_PERF_OWNER_TOKEN is required")
    login, recipient = (os.environ.get(key, "") for key in
                        ("COMPILER_PERF_OWNER_LOGIN", "COMPILER_PERF_AGE_RECIPIENT"))
    require(re.fullmatch(r"[A-Za-z0-9-]+(?:\[bot\])?", login), "blocked_setup: COMPILER_PERF_OWNER_LOGIN is required")
    require(re.fullmatch(r"age1[023456789acdefghjklmnpqrstuvwxyz]{58}", recipient),
            "blocked_setup: COMPILER_PERF_AGE_RECIPIENT must be a native age recipient")
    return {"login": login, "recipient": recipient}


def validate_python(record, root):
    root = Path(root).resolve()
    require(set(record) == {"executable", "prefix", "base_prefix", "stdlib"}
            and all(Path(p).is_absolute() and Path(p).resolve().is_relative_to(root) for p in record.values()),
            "Private interpreter prefix is exposed")


def verify_retention(controller, config):
    with tempfile.TemporaryDirectory(prefix="retention-probe-", dir=controller.root) as directory:
        root = Path(directory)
        source = root / "probe"
        source.write_bytes(b"compiler-perf provisioning probe\n")
        record = dict(w.m.file_record(source), size_bytes=source.stat().st_size, type="trace")
        w.artifacts._encrypt(source, record, config, root, 1024 * 1024)


def verify_inventory(root, plan):
    root = Path(root)
    expected = {Path(record["path"]).name: record for record in plan["files"]}
    require(w.e.unredirected(root) and len(expected) == len(plan["files"])
            and set(expected) == {p.name for p in root.iterdir()}, "Artifact readback inventory differs")
    for name, record in expected.items():
        path = root / name
        require(w.e.unredirected(path) and path.is_file() and path.stat().st_nlink == 1
                and path.stat().st_size == record["size_bytes"]
                and w.m.file_record(path)["sha256"] == record["sha256"], "Artifact readback bytes differ")


def archive_bound(size, count):
    # zlib's conservative deflateBound plus ZIP headers and directory records.
    return size + (size >> 12) + (size >> 14) + (size >> 25) + 65536 + 1024 * count


class ActionsArtifactSink:
    def __init__(self, controller, *, environment=None, transport=None):
        self.controller, self.environment = controller, environment or {}
        self.transport = transport or self.transfer

    def transfer(self, plan, destination):
        controller = self.controller
        environment = dict(w.c.tool_environment(), **{
            key: self.environment[key] for key in ("ACTIONS_RUNTIME_TOKEN", "ACTIONS_RESULTS_URL")
            if key in self.environment})
        require(environment.get("ACTIONS_RUNTIME_TOKEN") and re.fullmatch(
            r"https://[A-Za-z0-9.-]+\.actions\.githubusercontent\.com/?",
            environment.get("ACTIONS_RESULTS_URL", "")), "Trusted current-run Actions artifact credentials unavailable")
        owner = controller.session["attempt"] or controller.session["discovery"]
        require(self.environment.get("GITHUB_RUN_ID") == owner["run"]["id"]
                and self.environment.get("GITHUB_REPOSITORY") == w.mem.REPOSITORY,
                "Artifact runtime belongs to another run or repository")
        environment["COMPILER_PERF_ARTIFACT_MODULE"] = str(controller.root.parent / "transport" /
                                                          "node_modules" / "@actions" / "artifact")
        environment["TZ"] = "UTC"
        result = subprocess.run(["node", str(Path(__file__).with_name("actions_artifact.cjs"))],
            input=w.mem.encoded({"name": "compiler-perf-" + plan["manifest_sha256"], "files": plan["files"],
                                "receipt_path": str(controller.root / ("artifact-upload-" + plan["manifest_sha256"] + ".json")),
                                "destination": str(destination)}), capture_output=True, env=environment,
            timeout=controller.remaining("finalization", w.FINALIZATION_SECONDS))
        require(result.returncode == 0 and len(result.stdout) <= 1024,
                "Actions artifact upload/readback failed; pending inventory preserved")
        return w.mem.decoded(result.stdout)

    def __call__(self, plan):
        controller = self.controller
        require(plan["files"] and plan["status"] not in ("expired", "blocked_retention")
                and plan["retention_days"] == 14 and controller.now() < w.mem.timestamp(plan["expires_at"]),
                "Artifact inventory expired or blocked")
        # Bound ZIP framing as well as every staged manifest, including expired ones.
        total, compact = 0, 0
        for staged in controller.session.get("artifacts", []):
            w.m.verify_files([staged["manifest"]])
            manifest = w.read_input(staged["manifest"]["path"])
            size = manifest["total_bytes"]
            bound = archive_bound(size, len(manifest["artifacts"]) + 1)
            total += bound
            compact += manifest.get("compact_bytes", size) + bound - size
        require(total <= plan["limits"]["total_bytes"] and compact <= plan["limits"]["compact_bytes"],
                "Artifact archive overhead exceeds retention budget")
        with tempfile.TemporaryDirectory(prefix="artifact-readback-", dir=controller.root) as directory:
            destination = Path(directory)
            transfer = self.transport(plan, destination)
            require(type(transfer) is dict and set(transfer) == {"id", "retention_started_at"}
                    and type(transfer["id"]) is int and transfer["id"] > 0, "Invalid Actions artifact receipt")
            identifier = transfer["id"]
            verify_inventory(destination, plan)
        metadata = controller.api.get(f"repos/{w.mem.REPOSITORY}/actions/artifacts/{identifier}")
        owner = controller.session["attempt"] or controller.session["discovery"]
        require(metadata["id"] == identifier and metadata["name"] == "compiler-perf-" + plan["manifest_sha256"]
                and metadata["expired"] is False and metadata["workflow_run"]["id"] == int(owner["run"]["id"])
                and type(metadata["size_in_bytes"]) is int and 0 < metadata["size_in_bytes"]
                    <= archive_bound(plan["total_bytes"], len(plan["files"]))
                and w.mem.timestamp(metadata["created_at"]) <= controller.now()
                and w.mem.timestamp(transfer["retention_started_at"]) <= controller.now()
                and w.mem.artifact_retention(dict(metadata, retention_started_at=transfer["retention_started_at"]),
                                            plan["created_at"]), "Actions artifact owner, expiry or retention differs")
        return {"scope": "actions" if controller.transport.scope == "github" else "local_fixture",
                "acknowledged": True, "manifest_sha256": plan["manifest_sha256"],
                "artifact_id": str(identifier),
                "location": f"https://github.com/{w.mem.REPOSITORY}/actions/runs/{owner['run']['id']}/artifacts/{identifier}",
                "retention_days": 14, "created_at": metadata["created_at"], "expires_at": metadata["expires_at"],
                "retention_started_at": transfer["retention_started_at"]}


class ActionsPublisher:
    def __init__(self, adapter, destination):
        self.adapter, self.destination = adapter, Path(destination)

    def branch(self, name):
        require(name.startswith(w.mem.WORKFLOW_MARKER + "/") and w.mem.identifier(name.split("/")[1]),
                "Unexpected publication branch")
        if self.adapter.controller.transport.scope == "local_fixture":
            return self.adapter.controller.api.branch(name)
        result = self.request(f"git/ref/heads/{name}", absent=True)
        require(result is None or w.mem.digest(result["object"]["sha"], 40), "Invalid publication branch head")
        return None if result is None else result["object"]["sha"]

    def request(self, endpoint, payload=None, *, absent=False):
        controller = self.adapter.controller
        result = subprocess.run(["gh", "api", "--include", f"repos/{w.mem.REPOSITORY}/{endpoint}",
                                  *(["--method", "POST", "--input", "-"] if payload is not None else [])],
            input=w.mem.encoded(payload) if payload is not None else None, capture_output=True,
            timeout=min(60, controller.remaining("finalization", w.FINALIZATION_SECONDS)))
        require(len(result.stdout) <= w.mem.LIMITS["file_bytes"], "Publisher API response exceeds bounds")
        parts = re.split(rb"\r?\n\r?\n", result.stdout, maxsplit=1)
        require(len(parts) == 2, "Incomplete publisher API response")
        if absent and re.match(rb"HTTP/\S+ 404\b", parts[0]):
            return None
        require(result.returncode == 0 and re.match(rb"HTTP/\S+ 20[01]\b", parts[0]),
                "Publisher API denied; intent and branch preserved")
        return w.mem.decoded(parts[1])

    def publish(self, payload, head, intent):
        controller = self.adapter.controller
        exported = self.adapter.export(self.destination)
        w.validate_payload(w.read_input(self.destination / "safe-output.json"), payload)
        sealed = controller.load("approval")
        require(head == sealed["head"] == intent["head_sha"] and exported["binding"]["payload"]
                == w.m.fingerprint_inputs(payload), "Publisher identity differs")
        _, state = controller.current_attempt()
        source = Path(state["tree"])
        with tempfile.TemporaryDirectory(prefix="publisher-am-", dir=controller.root) as directory:
            tree = Path(directory) / "tree"
            w.e.git(controller.repository, "clone", "--quiet", "--no-hardlinks", "--no-checkout", str(source), str(tree))
            w.e.git(tree, "checkout", "--quiet", "--detach", sealed["base"])
            w.e.git(tree, "-c", "user.name=Compiler performance validator",
                    "-c", "user.email=compiler-perf@example.invalid", "am", "--keep-cr",
                    str(self.destination / "proposal.patch"))
            require(w.e.git(tree, "show", "-s", "--format=%T %P", "HEAD").decode().strip()
                    == sealed["tree"] + " " + sealed["base"], "am tree or parent differs from approval")
        if controller.transport.scope == "local_fixture":
            return controller.api.publish(payload, head, intent)
        item = payload["items"][0]
        controller.api.verify_source({"source_ref": item["base"], "source_revision": item["base_commit"]})
        require(not w.mem.workflow_prs(w.mem.query_prs(controller.pages)), "Outstanding draft prevents publication")
        prior = self.branch(item["branch"])
        require(prior in (None, head), "Outstanding branch differs; preserved")
        if prior is None:
            ref = "refs/heads/" + item["branch"]
            result = subprocess.run(["git", "-C", str(source), "push", "--porcelain", "--no-verify",
                "--force-with-lease=" + ref + ":", controller.transport.remote, head + ":" + ref],
                capture_output=True, env=controller.transport._environment(network=True),
                timeout=min(60, controller.remaining("finalization", w.FINALIZATION_SECONDS)))
            require(result.returncode == 0, "Draft branch publication failed; intent preserved")
        require(self.branch(item["branch"]) == head, "Published branch acknowledgment differs")
        result = self.request("pulls", {key: item[key] for key in ("title", "body", "base", "draft")} |
                              {"head": item["branch"]})
        require(result["draft"] is True and result["head"]["sha"] == head
                and result["user"]["login"] == controller.load("publisher-owner")["login"],
                "Created draft identity differs; publication needs reconciliation")


def provision(base, job_started_at):
    owner = owner_configuration()
    base = Path(base)
    require(sys.platform == "linux" and sys.flags.isolated and re.fullmatch(
        r"/var/lib/compiler-perf/[0-9]+-[0-9]+", str(base)) and w.e.unredirected(base)
        and base.is_dir() and base.stat().st_mode & 0o077 == 0, "Invalid protected Actions root")
    repository, controller = base / "source", base / "controller"
    started = w.mem.timestamp(job_started_at)
    deadline = started + w.timedelta(seconds=w.SETUP_SECONDS)
    require(os.environ["GITHUB_REPOSITORY"] == w.mem.REPOSITORY
            and w.mem.digest(os.environ["GITHUB_SHA"], 40)
            and w.e.git(repository, "rev-parse", "HEAD").decode().strip() == os.environ["GITHUB_SHA"],
            "Trusted default-branch checkout differs")
    require(w.GitHub.get("user")["login"] == owner["login"],
            "blocked_setup: COMPILER_PERF_OWNER_LOGIN differs from the authenticated token owner")

    def remaining():
        seconds = (deadline - datetime.now(timezone.utc)).total_seconds()
        require(0 < seconds <= w.SETUP_SECONDS, "blocked_setup: aggregate setup deadline")
        return seconds

    def run(command, *, environment=None):
        subprocess.run(command, cwd=repository, check=True, timeout=remaining(),
                       env=w.c.tool_environment() if environment is None else environment,
                       stdout=subprocess.DEVNULL)

    def install(url, sha, name):
        archive = base / (name + ".tar.gz")
        with urlopen(url, timeout=min(60, remaining())) as response, archive.open("xb") as output:
            digest, size = hashlib.sha256(), 0
            while chunk := response.read(1024 * 1024):
                remaining()
                size += len(chunk)
                require(size < 100 * 1024 * 1024, "Tool archive exceeds budget")
                digest.update(chunk)
                output.write(chunk)
        require(digest.hexdigest() == sha, "Pinned tool release checksum differs")
        with tarfile.open(archive) as bundle:
            bundle.extractall(base, filter="data")
        archive.unlink()

    install(PYTHON_URL, PYTHON_SHA, "python")
    python = base / "python" / "bin" / "python3"
    probe = subprocess.run([str(python), "-I", "-c",
        "import json,sys,sysconfig; from pathlib import Path; print(json.dumps({"
        "'executable':str(Path(sys.executable).resolve()),'prefix':str(Path(sys.prefix).resolve()),"
        "'base_prefix':str(Path(sys.base_prefix).resolve()),'stdlib':str(Path(sysconfig.get_path('stdlib')).resolve())}))"],
        capture_output=True, check=True, timeout=remaining(), env=w.c.tool_environment())
    validate_python(w.mem.decoded(probe.stdout), base / "python")
    run(["git", "init", "--bare", "--quiet", str(base / "journal.git")])
    request = {"schema_version": 2, "run_id": os.environ["GITHUB_RUN_ID"],
               "base_sha": os.environ["GITHUB_SHA"], "primary_metric": "elapsed_ms"}
    w.m.write_json(base / "input.json", request)
    arguments = ["--root", str(controller), "--repository", str(repository),
                 "--journal", str(base / "journal.git"), "--workspace", os.environ["GITHUB_WORKSPACE"]]
    run([str(python), "-I", str(Path(runtime.__file__)), "setup", *arguments,
         "--input", str(base / "input.json"), "--job-started-at", job_started_at], environment=dict(os.environ))
    # Expensive dependencies and compiler bootstrap follow canonical reservation.
    from transport import JournalTransport
    host = w.Controller(controller, repository, JournalTransport(base / "journal.git"), w.GitHub())
    host.save("publisher-owner", {"login": owner["login"]})
    try:
        install(AGE_URL, AGE_SHA, "age")
        tool = base / "age" / "age"
        retention = {"recipient": owner["recipient"], "tool": w.m.file_record(tool)}
        verify_retention(host, retention)
        w.m.write_json(controller / "retention-config.json", retention)
        environment = runtime.bridge.trusted_environment(host)
        run([str(python), "-I", "-m", "pip", "install", "--disable-pip-version-check",
             "-r", str(Path(__file__).with_name("requirements-agent.txt"))])
        run(["npm", "install", "--prefix", str(base / "transport"), "--ignore-scripts",
             "--no-audit", "--no-fund", "--save-exact", "@actions/artifact@2.3.2"])
        require(Path("/usr/bin/bwrap").is_file(), "blocked_setup: runner requires bubblewrap")
        run(["bash", "-c", 'set -e; export DOTNET_INSTALL_DIR="$2"; source "$1"; '
             'InitializeDotNetCli true; InstallDotNetSdk "$2" "$3"',
             "compiler-perf-sdks", str(repository / "eng" / "common" / "tools.sh"),
             str(repository / ".dotnet"), "9.0.300"], environment=environment)
    except (ValueError, OSError, subprocess.SubprocessError) as error:
        host.finish("blocked_setup", "Provisioning: " + w.c.sanitize(str(error)))
        raise


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("command", choices=("provision",))
    parser.add_argument("--base", required=True)
    parser.add_argument("--job-started-at", required=True)
    args = parser.parse_args()
    try:
        provision(args.base, args.job_started_at)
        return 0
    except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
        print(w.mem.encoded({"outcome": "blocked_setup", "reason": w.c.sanitize(str(error))}).decode(), end="")
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
