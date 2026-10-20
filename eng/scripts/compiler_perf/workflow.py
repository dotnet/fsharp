"""Trusted compiler profiling lifecycle. Run only from the protected controller checkout."""

import argparse
from contextlib import contextmanager
import copy
from datetime import datetime, timedelta, timezone
import hashlib
import hmac
import os
from pathlib import Path
import platform
import re
import secrets
import signal
import subprocess
import sys
import tempfile
import threading
import time

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import experiment as e
import memory as mem
import artifacts

m, c, require = mem.m, mem.c, mem.require
ACTIVE_SECONDS = 14400
SETUP_SECONDS = 3600
STARTUP_SECONDS = 600
FINALIZATION_SECONDS = 2400
HEARTBEAT_SECONDS = 30
DEFAULT_BRANCH = "main"
AUTHOR = "github-actions[bot]"


def read_input(path):
    path = Path(path).absolute()
    require(e.unredirected(path) and path.is_file(), "Missing or redirected JSON input")
    with path.open("rb") as stream:
        data = stream.read(mem.LIMITS["file_bytes"] + 1)
    require(len(data) <= mem.LIMITS["file_bytes"], "JSON input budget exceeded")
    return mem.decoded(data)


def safe_output(attempt_id, base, head, tree, patch_hash, title, body):
    require(mem.identifier(attempt_id) and all(mem.digest(s, 40) for s in (base, head, tree))
            and mem.digest(patch_hash), "Invalid publication identity")
    require(all(mem.text(s) and len(s) <= 4096 and c.sanitize(s) == s for s in (title, body)),
            "Invalid publication text")
    return {"items": [{"type": "create_pull_request", "base": DEFAULT_BRANCH, "base_commit": base,
                       "branch": mem.WORKFLOW_MARKER + "/" + attempt_id, "draft": True,
                       "title": title, "body": body, "labels": [mem.WORKFLOW_MARKER],
                       "patch_path": "proposal.patch"}]}


def validate_payload(payload, expected):
    require(type(payload) is dict and set(payload) == {"items"} and type(payload["items"]) is list
            and len(payload["items"]) == 1, "Only one safe-output item is allowed")
    item = payload["items"][0]
    require(type(item) is dict and set(item) == {
        "type", "base", "base_commit", "branch", "draft", "title", "body", "labels", "patch_path"
    } and item["type"] == "create_pull_request" and item["draft"] is True
        and item["base"] == DEFAULT_BRANCH and item["patch_path"] == "proposal.patch",
        "Only the sealed am patch transport and one draft are allowed")
    require(mem.encoded(payload) == mem.encoded(expected), "Entire safe-output payload changed")


def approval(attempt, payload, tree, patch_hash):
    mem.acceptance_ready(attempt)
    require(mem.digest(tree, 40) and mem.digest(patch_hash), "Invalid output binding")
    checks = copy.deepcopy(attempt["correctness"])
    checks.pop("validation", None)
    for result in checks["results"]:
        result.pop("execution", None)
    return {"repository": mem.REPOSITORY, "base": attempt["source_revision"],
            "run": attempt["run"], "attempt": attempt["id"], "tree": tree,
            "patch_sha256": patch_hash, "policy": m.fingerprint_inputs(m.POLICY),
            "evidence": m.fingerprint_inputs([attempt["fingerprints"], attempt["summary"],
                                              attempt["measurement"], attempt["collection"], checks]),
            "payload": m.fingerprint_inputs(payload)}


class GitHub(mem.GitHubReader):
    """Fixed repository reads; PR bodies are never identity or instruction inputs."""

    def prs(self, page):
        response = self.get(f"repos/{mem.REPOSITORY}/pulls?state=open&per_page=100&page={page}", page=True)
        require(type(response["data"]) is list and type(response["has_next"]) is bool,
                "Malformed PR page")
        items = []
        for p in response["data"]:
            branch, author = p["head"]["ref"], p["user"]["login"]
            attempt_id = branch.removeprefix(mem.WORKFLOW_MARKER + "/")
            owned = branch.startswith(mem.WORKFLOW_MARKER + "/") and mem.identifier(attempt_id)
            head = p["head"]["sha"]
            items.append({"number": p["number"], "state": p["state"], "draft": p["draft"],
                          "base_repository": p["base"]["repo"]["full_name"], "base_ref": p["base"]["ref"],
                          "head_repository": p["head"]["repo"]["full_name"] if p["head"]["repo"] else None,
                          "head_ref": branch, "head_sha": head, "labels": [v["name"] for v in p["labels"]],
                          "author": author, "marker": mem.WORKFLOW_MARKER if owned else None,
                          "attempt_id": attempt_id if owned else None,
                          "intent_id": m.fingerprint_inputs([attempt_id, head, mem.WORKFLOW_MARKER]) if owned else None,
                          "url": p["html_url"]})
        return {"status": 200, "items": items, "complete": True,
                "next": page + 1 if response["has_next"] else None}

    def branch(self, name):
        require(name.startswith(mem.WORKFLOW_MARKER + "/") and mem.identifier(name.split("/")[1]),
                "Unexpected publication branch")
        # Denial and absence both block publishing; this reader never creates a branch.
        return self.get(f"repos/{mem.REPOSITORY}/git/ref/heads/{name}")["object"]["sha"]


def capabilities(root, repository):
    require(sys.platform == "linux" and Path("/usr/bin/bwrap").is_file(),
            "Linux bubblewrap is required; no host-execution fallback")
    dotnet = Path(repository) / ".dotnet" / "dotnet"
    require(dotnet.is_file(), "Pinned local dotnet is unavailable")
    config = read_input(Path(repository) / "global.json")
    require(config["sdk"]["version"] == e.runner.SDK, "SDK pin differs from correctness runner")
    environment = m.allowed_environment(root / "probe")
    boundary = e.Isolation(repository, [dotnet.parent])
    result = boundary.command([str(dotnet), "--version"], repository, environment, root, "capability", 60)
    require(result["status"] == "collected" and result["stdout"].strip() == e.runner.SDK,
            "Linux namespace or pinned SDK probe failed")
    script = (
        "import os,pathlib,socket,sys\n"
        "assert not any('TOKEN' in k or 'SECRET' in k for k in os.environ)\n"
        "assert int(next(l.split()[1] for l in pathlib.Path('/proc/self/status').read_text().splitlines() "
        "if l.startswith('CapEff:')),16)==0\n"
        "for name in sys.argv[1:]:\n"
        " try: pathlib.Path(name).read_bytes()\n"
        " except (FileNotFoundError,PermissionError): pass\n"
        " else: raise AssertionError('Protected custody readable')\n"
        " try: pathlib.Path(name).write_bytes(b'forged')\n"
        " except (FileNotFoundError,PermissionError): pass\n"
        " else: raise AssertionError('Protected custody writable')\n"
        "s=socket.socket(); s.settimeout(1)\n"
        "assert s.connect_ex(('1.1.1.1',443)) != 0\n"
        "print('isolated')\n")
    result = boundary.command(["/usr/bin/python3", "-c", script, str(root / "key"),
                               str(root / "host-receipt")], repository, environment, root, "custody-capability", 15)
    m.write_json(root / "isolation-probe.json", result)
    require(result["status"] == "collected" and result["stdout"].strip() == "isolated",
            "Linux credential, custody, capability or deny-network probe failed")
    c.verify_tools(str(dotnet), root)


class ActiveSession:
    """Main-thread timer interrupts owned child waits, including long pack/test commands."""

    def __init__(self, checkpoint, *, clock=time.monotonic, allowance=ACTIVE_SECONDS):
        self.checkpoint, self.clock, self.allowance = checkpoint, clock, allowance
        self.start, self.previous, self.last = None, None, None

    def poll(self, *_):
        self.checkpoint()
        now = self.clock()
        if now < self.last or now - self.start >= self.allowance:
            raise KeyboardInterrupt("Active investigation deadline reached")
        self.last = now

    def __enter__(self):
        if self.start is None:
            self.start = self.clock()
            self.last = self.start
        self.poll()
        if sys.platform == "linux":
            require(signal.getitimer(signal.ITIMER_REAL) == (0.0, 0.0), "Another timer owns this process")
            self.previous = signal.signal(signal.SIGALRM, self.poll)
            signal.setitimer(signal.ITIMER_REAL, min(HEARTBEAT_SECONDS, self.allowance),
                             min(HEARTBEAT_SECONDS, self.allowance))
        return self

    def __exit__(self, *_):
        if self.previous is not None:
            signal.setitimer(signal.ITIMER_REAL, 0)
            signal.signal(signal.SIGALRM, self.previous)


class Controller:
    def __init__(self, root, repository, transport, api, *, now=None, clock=time.monotonic):
        self.root, self.repository = Path(root).absolute(), Path(repository).resolve(strict=True)
        require(e.unredirected(self.root, missing_ok=True), "Controller storage must be unredirected")
        self.root = self.root.resolve()
        require(not self.root.is_relative_to(self.repository), "Controller storage must be outside source")
        self.transport, self.api = transport, api
        self.now = now or (lambda: datetime.now(timezone.utc))
        self.clock, self.timers, self.budgets, self.saving = clock, {}, set(), False
        self.storage_lock = threading.RLock()
        if not self.root.exists():
            c.private_directory(self.root)
            (self.root / "key").write_bytes(secrets.token_bytes(32))
            (self.root / "key").chmod(0o600)
        require(e.unredirected(self.root / "key"), "Controller custody key unavailable")
        self.key = (self.root / "key").read_bytes()
        require(len(self.key) == 32, "Invalid controller custody key")
        if os.name != "nt":
            require(self.root.stat().st_uid == os.getuid() and self.root.stat().st_mode & 0o077 == 0,
                    "Controller storage must be owner-only")
        self.session = self.load("session") if (self.root / "session.json").exists() else None
        self.busy = False

    def save(self, name, value):
        with self.storage_lock:
            previous, self.saving = self.saving, True
            try:
                require(e.unredirected(self.root / (name + ".json"), missing_ok=True) and
                        e.unredirected(self.root / (name + ".json.tmp"), missing_ok=True), "Redirected controller record")
                data = {"schema_version": 1, "value": value,
                        "seal": hmac.digest(self.key, name.encode() + mem.encoded(value), "sha256").hex()}
                require(len(mem.encoded(data)) <= mem.LIMITS["file_bytes"], "Controller record budget exceeded")
                m.write_json(self.root / (name + ".json"), data)
            finally:
                self.saving = previous

    def load(self, name):
        with self.storage_lock:
            data = read_input(self.root / (name + ".json"))
            require(set(data) == {"schema_version", "value", "seal"} and data["schema_version"] == 1
                    and isinstance(data["seal"], str) and hmac.compare_digest(
                        data["seal"], hmac.digest(self.key, name.encode() + mem.encoded(data["value"]), "sha256").hex()),
                    "Controller record custody changed")
            return data["value"]

    def pages(self, page):
        result = self.api.prs(page)
        require(type(result) is dict and type(result.get("items")) is list, "Malformed PR API response")
        for pr in result["items"]:
            mem.validate_pr(pr)
            require(mem.text(pr["author"]) and pr["base_repository"] == mem.REPOSITORY
                    and (pr["intent_id"] is None or mem.digest(pr["intent_id"])), "Invalid PR author/repository/intent")
        return result

    def reservation(self, request):
        e.validate_request(request)
        require(all(c.sanitize(request[k]) == request[k] for k in ("hypothesis", "complexity")),
                "Credential-bearing proposal text")
        base = request["base_sha"]
        paths = {p: hashlib.sha256(e.git(self.repository, "show", f"{base}:{p}")).hexdigest()
                 for p in request["compiler_paths"]}
        pin = mem.decoded(e.git(self.repository, "show", f"{base}:global.json"))
        source = e.git(self.repository, "ls-tree", "-r", base, "--", "src/Compiler", "src/FSharp.Core",
                       "eng", "Directory.Build.props", "Directory.Build.targets", "global.json")
        plan = {"source": base, "workload": hashlib.sha256(source).hexdigest(),
                "sdk": pin["sdk"]["version"], "policy": m.POLICY, "configuration": "Release",
                "project": m.PROJECT.as_posix(), "paths": paths,
                "architecture": platform.machine(), "platform": sys.platform,
                "helpers": [m.file_record(p) for p in (m.__file__, c.__file__, e.__file__, mem.__file__, __file__)]}
        planned = self.root / ("planned-inputs-" + request["attempt_id"] + ".json")
        if planned.exists():
            require(read_input(planned) == plan, "Recorded planned inputs differ")
        else:
            m.write_json(planned, plan)
        checks = e.correctness_plan(request["compiler_paths"] + [r["path"] for r in request["regressions"]] +
                                   [e.RELEASE_NOTE], request["regressions"], "net11.0")
        now = self.now()
        discovery = self.session.get("discovery") if self.session else None
        expires = (discovery["expires_at"] if discovery and discovery["run"]["id"] == request["run_id"] else
                   (now + timedelta(seconds=SETUP_SECONDS + ACTIVE_SECONDS)).isoformat())
        return {"id": request["attempt_id"], "source_revision": base,
                "hypothesis": {"id": request["attempt_id"], "description": request["hypothesis"],
                               "area": request["compiler_paths"][0].split("/")[2],
                               "operation": ";".join(sorted(request["compiler_paths"]))},
                "run": {"id": request["run_id"], "attempt": 1,
                        "url": f"https://github.com/{mem.REPOSITORY}/actions/runs/{request['run_id']}"},
                "lifecycle": "reserved", "outcome": None, "reason": "",
                "fingerprints": {"source": m.fingerprint_inputs(base), "input": m.fingerprint_inputs(plan),
                                 "policy": m.fingerprint_inputs(m.POLICY)},
                "dependencies": {"paths": paths, "workload": plan["workload"], "sdk": plan["sdk"],
                                 "collector_permission": None},
                "retry": {"when_any": [{"kind": "path", "path": p, "value": v} for p, v in paths.items()] +
                          [{"kind": "sdk", "value": plan["sdk"]}],
                          "reason": "Retry only when a recorded relevant compiler path or SDK changes"},
                "summary": {"evidence": "Reserved before bootstrap; protected planned inputs bind preparation",
                            "change": request["complexity"], "patch_sha256": hashlib.sha256(b"").hexdigest()},
                "evidence": [], "measurement": None, "collection": {"status": "pending", "reason": ""},
                "correctness": {"schema_version": 5, "changes": request["compiler_paths"] +
                                [r["path"] for r in request["regressions"]] + [e.RELEASE_NOTE],
                                "regressions": request["regressions"], "target_framework": "net11.0",
                                "plan": checks, "plan_sha256": m.fingerprint_inputs(checks),
                                "required": [p["id"] for p in checks], "results": [], "nonce": "reserved",
                                "cwd": str(self.root / "candidate" / "tree"), "tool": {},
                                "inputs": m.file_record(planned), "validation": None},
                "publication": None, "supersedes": None, "notes": [],
                "lease": {"run_id": request["run_id"], "run_attempt": 1, "checkpoint_at": now.isoformat(),
                          "expires_at": expires},
                "checkpoint": {"next_action": "Prepare frozen Release compiler-source workload",
                               "required_evidence": [], "candidate_ref": ""}}

    def persist(self, state, head, candidate):
        mem.checkpoint(self.root / "checkpoint.json", candidate)
        receipt = self.transport.write_state(state, head, candidate)
        require(type(receipt) is dict and receipt.get("acknowledged") is True
                and receipt.get("branch") == mem.STATE_BRANCH and mem.digest(receipt.get("commit"), 40)
                and receipt.get("scope") == self.transport.scope in ("github", "local_fixture")
                and receipt.get("remote") == self.transport.remote
                and receipt.get("local") == mem.GitJournal.receipt(receipt["commit"]),
                "Canonical update was not acknowledged")
        return receipt

    def remaining(self, name, allowance, *, create=False):
        """Sealed wall deadline survives restart; a live monotonic clock can only shorten it."""
        previous, self.saving = self.saving, True
        try:
            return self._remaining(name, allowance, create=create)
        finally:
            self.saving = previous

    def _remaining(self, name, allowance, *, create):
        clocks = self.load("clocks") if (self.root / "clocks.json").exists() else {}
        now, tick = self.now(), self.clock()
        if name not in clocks:
            require(create, "Protected investigation clock is missing")
            clocks[name] = {"start": now.isoformat(), "deadline": (
                now + timedelta(seconds=allowance)).isoformat(), "last_seen": now.isoformat(),
                "remaining_seconds": allowance}
            self.save("clocks", clocks)
        record = clocks[name]
        if name == "finalization" and (self.root / "runtime-engine-exit.json").exists():
            exited = mem.timestamp(self.load("runtime-engine-exit")["at"])
            if exited < mem.timestamp(record["start"]):
                record.update(start=exited.isoformat(),
                              deadline=(exited + timedelta(seconds=allowance)).isoformat())
        start, deadline, last = (mem.timestamp(record[k]) for k in ("start", "deadline", "last_seen"))
        require((deadline - start).total_seconds() == allowance, "Protected deadline differs")
        require(type(record["remaining_seconds"]) in (int, float)
                and 0 <= record["remaining_seconds"] <= allowance, "Invalid protected remaining time")
        if now < last or now < start:
            raise KeyboardInterrupt("Wall clock moved backwards; investigation incomplete")
        remaining = min((deadline - now).total_seconds(),
                        record["remaining_seconds"] - (now - last).total_seconds())
        if name not in self.timers:
            self.timers[name] = (tick, remaining)
        origin, initial = self.timers[name]
        if tick < origin:
            raise KeyboardInterrupt("Monotonic clock moved backwards")
        remaining = min(remaining, initial - (tick - origin))
        record["last_seen"] = now.isoformat()
        record["remaining_seconds"] = max(0, remaining)
        self.save("clocks", clocks)
        if remaining <= 0:
            raise KeyboardInterrupt(name + " deadline reached")
        return remaining

    @contextmanager
    def budget(self, name, allowance):
        remaining = self.remaining(name, allowance, create=name != "active")
        if name in self.budgets:
            yield
            self.remaining(name, allowance)
            return

        def pulse():
            if self.saving:
                return
            self.remaining(name, allowance)
            if self.session is not None:
                self.checkpoint()

        self.budgets.add(name)
        try:
            with ActiveSession(pulse, clock=self.clock, allowance=remaining):
                yield
                pulse()
        finally:
            self.budgets.remove(name)

    def poll_active(self):
        return self.remaining("active", ACTIVE_SECONDS)

    def start_active(self):
        require(self.session is not None and self.session["phase"] == "ready",
                "Prepared controller and ready agent required")
        if not (self.root / "clocks.json").exists() or "active" not in self.load("clocks"):
            ready = mem.timestamp(self.session["ready_at"])
            if not 0 <= (self.now() - ready).total_seconds() < STARTUP_SECONDS:
                raise ValueError("Agent readiness exhausted startup allowance")
        self.remaining("active", ACTIVE_SECONDS, create=True)

    def discover(self, request):
        try:
            allowance = self.remaining("setup", SETUP_SECONDS, create=True)
            with ActiveSession(lambda: self.remaining("setup", SETUP_SECONDS),
                               clock=self.clock, allowance=allowance):
                return self._discover(request)
        except KeyboardInterrupt:
            reason = "Aggregate setup deadline or cancellation during preflight"
            if self.session and self.session.get("discovery") and self.session["discovery"]["outcome"] is None:
                self.finish("blocked_setup", reason)
            return {"action": "blocked", "outcome": "blocked_setup", "reason": reason, "durable": False}

    def _discover(self, request):
        require(set(request) == {"schema_version", "run_id", "base_sha", "primary_metric"}
                and type(request["schema_version"]) is int and request["schema_version"] == 2
                and mem.identifier(request["run_id"]) and len(request["run_id"]) <= 96
                and mem.digest(request["base_sha"], 40) and request["primary_metric"] in m.PRIMARY_METRICS,
                "Invalid discovery request")
        result = {"action": "blocked", "outcome": "blocked_setup", "reason": "", "durable": False}
        try:
            self.remaining("setup", SETUP_SECONDS, create=True)
            self.api.verify_source({"source_ref": DEFAULT_BRANCH, "source_revision": request["base_sha"]})
            self.remaining("setup", SETUP_SECONDS)
            now = self.now()
            proposed = {"id": request["run_id"] + ".discovery",
                        "run": {"id": request["run_id"], "attempt": 1,
                                "url": f"https://github.com/{mem.REPOSITORY}/actions/runs/{request['run_id']}"},
                        "source_revision": request["base_sha"], "lifecycle": "reserved",
                        "outcome": None, "reason": "", "checkpoint_at": now.isoformat(),
                        "expires_at": (now + timedelta(seconds=SETUP_SECONDS + STARTUP_SECONDS +
                                                      ACTIVE_SECONDS + FINALIZATION_SECONDS)).isoformat()}
            for _ in range(3):
                state, head = self.transport.read()
                decision = mem.discovery_preflight(state, proposed, self.pages, self.api.run, now)
                self.remaining("setup", SETUP_SECONDS)
                if decision["state"] != state:
                    try:
                        receipt = self.persist(state, head, decision["state"])
                    except mem.Conflict:
                        continue
                    decision.update(persistence=receipt, durable=receipt["scope"] == "github")
                if decision["action"] == "reserve":
                    self.session = {"discovery_request": copy.deepcopy(request), "request": None, "attempt": None,
                                    "discovery": proposed, "persistence": receipt, "phase": "reserved",
                                    "canonical_discovery_sha256": m.fingerprint_inputs(proposed), "revisions": []}
                    self.save("session", self.session)
                self.remaining("setup", SETUP_SECONDS)
                return {k: v for k, v in decision.items() if k != "state"}
            raise mem.Conflict("Discovery conflict retry budget exhausted")
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            return dict(result, reason=c.sanitize(str(error)))

    def frozen_manifest(self):
        m.verify_files([self.session["manifest"]])
        manifest = c.read_bounded_json(self.session["manifest"]["path"])
        e.frozen_records(manifest)
        return manifest

    def public_data(self, value):
        if isinstance(value, str):
            text = c.sanitize(value)
            for path, label in ((self.root, "[controller]"), (self.repository, "[source]")):
                text = text.replace(str(path), label).replace(path.as_posix(), label)
            return text
        if isinstance(value, list):
            return [self.public_data(v) for v in value]
        if isinstance(value, dict):
            return {k: self.public_data(v) for k, v in value.items()}
        return value

    def diagnostics(self):
        m.verify_files([self.session["profile"]])
        profile = c.analyze_profile(self.session["profile"]["path"])
        require(profile["status"] == "collected", profile.get("reason", "Diagnostic collection incomplete"))
        evidence = c.read_bounded_json(self.session["profile"]["path"])
        manifest = self.frozen_manifest()
        require(evidence["compiler"] == manifest["baseline_compiler"]
                and evidence["fingerprints"] == {k: m.fingerprint_inputs(v)
                                               for k, v in manifest["invariants"].items()},
                "Diagnostic evidence belongs to different frozen inputs")
        result = {k: profile[k] for k in ("metrics", "allocation_method", "allocation_counter_observed_bytes",
                                        "unavailable_reasons", "stack_semantics")}
        result["stacks"] = sorted(profile["stacks"], key=lambda s: s["weight"], reverse=True)[:32]
        result["stack_count"] = len(profile["stacks"])
        timings = Path(evidence["timings"]["output"]["path"]).read_text(encoding="utf-8")
        result["binlog"] = {
            "projects": evidence["timings"]["projects"],
            "conclusion": timings[:8192], "truncated": len(timings) > 8192,
            "acceptance_eligible": False}
        result["profile_sha256"] = self.session["profile"]["sha256"]
        # File paths and process/environment records stay in protected custody.
        result = self.public_data(result)
        require(len(mem.encoded(result)) <= 65536, "Diagnostic summary budget exceeded")
        return result

    def status(self):
        require(self.session is not None, "Preflight reservation required")
        a, d = self.session["attempt"], self.session.get("discovery")
        record = d or a
        result = {"phase": self.session["phase"], "outcome": record["outcome"], "reason": record["reason"],
                  "diagnostics": self.session.get("diagnostics"),
                  "result": ({"outcome": a["outcome"], "reason": a["reason"]} if a else None),
                  "history": self.session.get("history", []), "revisions": self.session.get("revisions", []),
                  "dump": self.session.get("dump_result"),
                  "selection_result": self.session.get("selection_result")}
        if (self.root / "clocks.json").exists():
            result["clocks"] = self.load("clocks")
        return self.public_data(result)

    def profile(self, dump_reason=None, hypothesis=None):
        require(self.session["phase"] in ("ready", "awaiting_decision"), "Profile phase differs")
        require(dump_reason is None or (dump_reason in c.REASONS and mem.text(hypothesis)
                                       and len(hypothesis) <= 2048), "Invalid dump justification")
        phase, dump_workspace, retain = "collecting", None, None
        try:
            with self.budget("active", ACTIVE_SECONDS):
                manifest = self.frozen_manifest()
                self.session["phase"] = phase
                self.session["discovery"]["lifecycle"] = phase
                self.checkpoint()
                private = self.root / "private"
                private.mkdir(mode=0o700, exist_ok=True)
                profile_root = private / "profile"
                if dump_reason is not None:
                    dump_workspace = artifacts.create_dump_workspace(private / "dump-session",
                                                                     (self.root / "public",))
                    profile_root = dump_workspace.path / "profile"

                    def retain(receipt, text):
                        file = {"path": str(receipt["path"]), "sha256": receipt["sha256"],
                                "size_bytes": receipt["size_bytes"]}
                        result = self.stage_evidence([{"file": file, "kind": "dump", "conclusion": text}],
                                                     dump_workspace=dump_workspace)
                        owner = self.session.get("discovery") or self.session["attempt"]
                        record = next((r for r in reversed(owner.get("evidence", []))
                                       if r["type"] == "dump" and r["sha256"] == receipt["sha256"]), None)
                        if record is None:
                            record = artifacts._record("dump", receipt["sha256"], receipt["size_bytes"], text, self.now(),
                                provenance={"run_id": owner["run"]["id"],
                                            "fingerprint": m.fingerprint_inputs(owner["source_revision"])})
                            record["reason"] = "blocked_retention: " + result["status"]
                            owner.setdefault("evidence", []).append(record)
                        return record

                c.collect_profile(manifest, manifest["baseline_compiler"], profile_root,
                                  dump_reason=dump_reason, hypothesis=hypothesis)
                profile_path = profile_root / "evidence-manifest.json"
                summary = c.analyze_profile(profile_path)
                require(summary["status"] == "collected", summary.get("reason", "Diagnostic collection failed"))
                if dump_reason is not None:
                    # Raw dumps never become agent-readable/public files.
                    evidence = c.read_bounded_json(profile_path)
                    dump = evidence["dump"]
                    conclusion = ("Heap snapshot includes unreachable objects; not retained bytes.\n" +
                                  c.sanitize(dump["validation"]["stdout"])[-1600:])

                    removed = artifacts.analyze_owned_dump(dump_workspace, dump["artifact"],
                                                           lambda _: conclusion, self.now(), retain=retain)
                    self.session["dump_result"] = {"reason": dump_reason, "hypothesis": hypothesis,
                                                   "artifact": removed,
                                                   "status": "encrypted_and_removed" if removed["availability"] == "pending"
                                                   else "blocked_retention"}
                    require(removed["availability"] == "pending", removed["reason"])
                else:
                    self.session["profile"] = m.file_record(profile_path)
                    self.session["diagnostics"] = self.diagnostics()
                self.session["phase"] = "awaiting_decision"
                self.session["discovery"]["lifecycle"] = "awaiting_decision"
                state, _ = self.transport.read()
                self.session["history"] = [{"hypothesis": a["hypothesis"], "outcome": a["outcome"],
                                            "reason": a["reason"], "retry": a["retry"]}
                                           for a in state["attempts"].values()][-32:]
                self.checkpoint()
            return self.status()
        except (KeyboardInterrupt, ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            if getattr(error, "artifact", None) is not None:
                owner = self.session.get("discovery") or self.session["attempt"]
                error.artifact["provenance"] = {"run_id": owner["run"]["id"],
                                                "fingerprint": m.fingerprint_inputs(owner["source_revision"])}
                owner.setdefault("evidence", []).append(error.artifact)
                self.session["dump_result"] = {"artifact": error.artifact, "status": "blocked_retention"}
            cancelled = isinstance(error, KeyboardInterrupt)
            return self.finish("cancelled" if cancelled else "blocked_collection",
                               "Incomplete diagnostics: deadline or cancellation" if cancelled else
                               "collection: " + c.sanitize(str(error)))
        finally:
            if dump_workspace is not None:
                dump = dump_workspace.path / "profile" / "private-dump" / "heap.dmp"
                if dump.exists():
                    removed = artifacts.analyze_owned_dump(dump_workspace, m.file_record(dump),
                        lambda _: "Incomplete dump investigation; no measured conclusion", self.now(), retain=retain)
                    owner = self.session.get("discovery") or self.session["attempt"]
                    if removed not in owner.setdefault("evidence", []):
                        owner["evidence"].append(removed)
                    self.session["dump_result"] = {"artifact": removed, "status": "incomplete_encrypted"
                                                   if removed["availability"] == "pending" else "blocked_retention"}
                    self.checkpoint()

    def select_hypothesis(self, data):
        self.poll_active()
        require(self.session["phase"] == "awaiting_decision" and self.session["discovery"]["outcome"] is None,
                "Completed diagnostics and an explicit decision are required")
        require(type(data) is dict and set(data) == {
            "compiler_paths", "regressions", "hypothesis", "complexity", "hot_operation"}, "Invalid selection")
        summary = self.diagnostics()
        operation = data["hot_operation"]
        require(type(operation) is int and 0 <= operation < len(summary["stacks"]), "Select an observed hot operation")
        sequence = self.session.get("selection_sequence", 0) + 1
        require(sequence <= 128, "Selection budget exceeded")
        self.session["selection_sequence"] = sequence
        self.save("session", self.session)
        request = dict(self.session["discovery_request"], **{k: v for k, v in data.items() if k != "hot_operation"})
        request.update(schema_version=1, attempt_id=request["run_id"] + "." + str(sequence))
        proposed = self.reservation(request)
        # Keep the existing path-scoped retry identity, including version-1 history.
        proposed["summary"]["evidence"] = (
            summary["profile_sha256"] + ": observed stack " +
            m.fingerprint_inputs(summary["stacks"][operation]["frames"]))
        proposed["collection"] = {"status": "collected", "reason": ""}
        proposed["checkpoint"]["next_action"] = "Submit a bounded compiler patch, then run isolated validation"
        for _ in range(3):
            state, head = self.transport.read()
            query = {"attempt": proposed, "repository": mem.REPOSITORY, "default_branch": DEFAULT_BRANCH,
                     "source_ref": DEFAULT_BRANCH, "source_revision": request["base_sha"]}
            self.api.verify_source(query)
            decision = mem.preflight(state, query, self.pages, self.api.run, self.now(),
                                     discovery_id=self.session["discovery"]["id"])
            if decision["action"] != "reserve":
                return {k: v for k, v in decision.items() if k != "state"}
            self.poll_active()
            try:
                receipt = self.persist(state, head, decision["state"])
            except mem.Conflict:
                continue
            a = decision["state"]["attempts"][proposed["id"]]
            self.session.update(request=request, attempt=a, phase="selected", persistence=receipt,
                                canonical_attempt_sha256=m.fingerprint_inputs(a))
            self.save("session", self.session)
            self.poll_active()
            return self.status()
        raise mem.Conflict("Selection conflict retry budget exhausted")

    def select(self):
        try:
            with self.budget("active", ACTIVE_SECONDS):
                result = self.select_hypothesis(self.session["selection"])
                self.session["selection_result"] = (
                    {k: result[k] for k in ("action", "outcome", "reason")} if "action" in result else
                    {"action": "selected", "outcome": None, "reason": "Observed operation reserved"})
                self.save("session", self.session)
            return self.status()
        except KeyboardInterrupt:
            return self.finish("cancelled", "Incomplete selection: active deadline or cancellation")
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            self.session["selection_result"] = {"action": "blocked", "outcome": "blocked_setup",
                                                "reason": c.sanitize(str(error))}
            self.save("session", self.session)
            return self.status()

    def submit_patch(self, patch):
        self.poll_active()
        require(self.session["phase"] == "selected" and type(patch) is str, "Select before submitting patch")
        data = patch.encode("utf-8")
        require(0 < len(data) <= e.MAX_PATCH and b"\0" not in data, "Invalid/bounded patch")
        inbox = self.root / "inbox"
        inbox.mkdir(mode=0o700, exist_ok=True)
        path = inbox / (self.session["attempt"]["id"] + ".patch")
        require(e.unredirected(path, missing_ok=True) and not path.exists(), "Patch already submitted or redirected")
        path.write_bytes(data)
        self.session.update(patch=m.file_record(path), phase="patch_submitted")
        self.session["attempt"]["summary"]["patch_sha256"] = self.session["patch"]["sha256"]
        self.poll_active()
        self.save("session", self.session)
        return self.status()

    def candidate_root(self):
        return Path(self.session.get("candidate", self.root / "candidate"))

    def validate(self):
        require(self.session["phase"] == "patch_submitted", "A sealed submitted patch is required")
        try:
            with self.budget("active", ACTIVE_SECONDS):
                manifest = self.frozen_manifest()
                m.verify_files([self.session["patch"], self.session["profile"]])
                candidate = self.root / ("candidate" if not self.session["revisions"] else
                                         "candidate-" + str(len(self.session["revisions"]) + 1))
                self.session.update(candidate=str(candidate), phase="experimenting")
                self.session["attempt"]["lifecycle"] = "experimenting"
                self.session["attempt"]["summary"]["patch_sha256"] = self.session["patch"]["sha256"]
                self.checkpoint()
                prepared = e.prepare_candidate(self.repository, candidate, self.session["request"],
                                               Path(self.session["patch"]["path"]).read_bytes(), manifest=manifest)
                require(prepared["outcome"] is None, prepared["reason"])
                result = e.validate_candidate(candidate, Path(self.session["profile"]["path"]),
                                              checkpoint_on_cancel=True)
                require((candidate / "attempt.json").is_file(), result["reason"])
                self.poll_active()
                final = self.accept_experiment(candidate)
                self.session["revisions"].append({"id": final["id"], "outcome": final["outcome"],
                                                 "reason": final["reason"],
                                                 "hypothesis": copy.deepcopy(final["hypothesis"]),
                                                 "patch_sha256": final["summary"]["patch_sha256"]})
                if final["outcome"] == "measured_improvement":
                    self.session["phase"] = "accepted"
                else:
                    self.session["phase"] = "awaiting_decision"
                self.checkpoint()
            return self.status()
        except KeyboardInterrupt:
            return self.finish("cancelled", "Incomplete validation: deadline or cancellation; retained evidence")
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            candidate = self.candidate_root()
            if (candidate / "candidate.json").is_file():
                result = c.read_bounded_json(candidate / "candidate.json")
                if result["outcome"] == "rejected" and not result["draft_eligible"]:
                    e.discard_candidate(candidate)
                    a = self.session["attempt"]
                    a.update(outcome="rejected", reason=c.sanitize(str(error)), lifecycle="interrupted")
                    self.session["revisions"].append({"id": a["id"], "outcome": a["outcome"],
                                                     "reason": a["reason"],
                                                     "hypothesis": copy.deepcopy(a["hypothesis"]),
                                                     "patch_sha256": a["summary"]["patch_sha256"]})
                    self.session["phase"] = "awaiting_decision"
                    self.checkpoint()
                    return self.status()
            return self.finish("blocked_setup", "experiment: " + c.sanitize(str(error)))

    def preflight(self, request):
        if type(request) is dict and request.get("schema_version") == 2:
            return self.discover(request)
        proposed = self.reservation(request)
        query = {"attempt": proposed, "repository": mem.REPOSITORY, "default_branch": DEFAULT_BRANCH,
                 "source_ref": DEFAULT_BRANCH, "source_revision": request["base_sha"]}
        result = {"action": "blocked", "outcome": "blocked_setup", "reason": "", "durable": False}
        try:
            self.api.verify_source(query)
            for _ in range(3):
                state, head = self.transport.read()
                # Same-run reentry needs explicit owner reconciliation, not an expired lease.
                if proposed["id"] in state["attempts"] and state["attempts"][proposed["id"]]["outcome"] is None:
                    return dict(result, reason="Existing owner requires checkpoint reconciliation")
                decision = mem.preflight(state, query, self.pages, self.api.run, self.now())
                if decision["state"] != state:
                    try:
                        receipt = self.persist(state, head, decision["state"])
                    except mem.Conflict:
                        continue
                    decision.update(persistence=receipt, durable=receipt["scope"] == "github")
                if decision["action"] == "reserve":
                    self.session = {"request": request, "attempt": decision["state"]["attempts"][proposed["id"]],
                                    "persistence": receipt, "phase": "reserved", "evidence": [],
                                    "canonical_attempt_sha256": m.fingerprint_inputs(
                                        decision["state"]["attempts"][proposed["id"]])}
                    self.save("session", self.session)
                return {k: v for k, v in decision.items() if k != "state"}
            raise mem.Conflict("Preflight conflict retry budget exhausted")
        except (OSError, ValueError, KeyError, TypeError, subprocess.SubprocessError) as error:
            return dict(result, reason=c.sanitize(str(error)))

    def checkpoint(self):
        require(self.session is not None, "No acknowledged reservation")
        if self.busy:
            return self.session["persistence"]
        self.busy = True
        try:
            state, head = self.transport.read()
            sealed = self.load("session")
            for name, index in (("attempt", "attempts"), ("discovery", "discoveries")):
                owner = self.session.get(name)
                if owner is None:
                    continue
                prior = state.get(index, {}).get(owner["id"], {}).get("evidence", [])
                for record in owner.get("evidence", []):
                    if record.get("schema_version") == 2 and record["availability"] == "available" and record not in prior:
                        require(sealed.get(name) is not None and sealed[name]["id"] == owner["id"]
                                and record in sealed[name].get("evidence", []),
                                "Artifact acknowledgment is not in authenticated custody")
                        record["acknowledgment"]["validation"] = mem.artifact_validation(record)
            if self.session.get("discovery") is not None:
                d = self.session["discovery"]
                previous = state.get("discoveries", {}).get(d["id"])
                require(previous is not None and
                        (m.fingerprint_inputs(previous) == self.session["canonical_discovery_sha256"]
                         or previous == d), "Canonical discovery changed")
                if d["outcome"] is None:
                    d["checkpoint_at"] = max(mem.timestamp(previous["checkpoint_at"]),
                                            min(self.now(), mem.timestamp(d["expires_at"]))).isoformat()
                candidate = mem.merge_discovery(state, d, previous)
                a = self.session["attempt"]
                if a is not None:
                    old = state["attempts"].get(a["id"])
                    require(old is not None and (old == a or
                            m.fingerprint_inputs(old) == self.session["canonical_attempt_sha256"]),
                            "Canonical attempt changed")
                    a["lease"]["checkpoint_at"] = max(mem.timestamp(old["lease"]["checkpoint_at"]),
                        min(self.now(), mem.timestamp(a["lease"]["expires_at"]))).isoformat()
                    candidate = mem.merge_attempt(candidate, a, old)
                self.save("session", self.session)
                receipt = (self.session["persistence"] if candidate == state and
                           head == self.session["persistence"]["commit"] else self.persist(state, head, candidate))
                self.session.update(persistence=receipt, canonical_discovery_sha256=m.fingerprint_inputs(d))
                if a is not None:
                    self.session["canonical_attempt_sha256"] = m.fingerprint_inputs(a)
                self.save("session", self.session)
                return receipt
            a = self.session["attempt"]
            mem.validate_attempt(a)
            previous = state["attempts"].get(a["id"])
            require(previous is not None and previous["run"] == a["run"], "Reservation owner changed")
            baseline = self.session["canonical_attempt_sha256"]
            require(baseline == m.fingerprint_inputs(previous) or a == previous,
                    "Canonical attempt changed since the acknowledged checkpoint")
            a["lease"]["checkpoint_at"] = max(mem.timestamp(previous["lease"]["checkpoint_at"]),
                min(self.now(), mem.timestamp(a["lease"]["expires_at"]))).isoformat()
            candidate = mem.merge_attempt(state, a, previous)
            self.save("session", self.session)
            receipt = (self.session["persistence"] if candidate == state and
                       head == self.session["persistence"]["commit"] else self.persist(state, head, candidate))
            self.session["persistence"] = receipt
            self.session["canonical_attempt_sha256"] = m.fingerprint_inputs(a)
            self.save("session", self.session)
            return receipt
        finally:
            self.busy = False

    def finish(self, outcome, reason):
        require(outcome in mem.OUTCOMES and mem.text(reason), "Invalid lifecycle conclusion")
        if self.session.get("discovery"):
            require(self.session["discovery"]["outcome"] is None, "Discovery already finished")
            if outcome == "completed_no_finding":
                require(self.session["phase"] == "awaiting_decision" and self.session["attempt"] is None
                        and not self.session.get("revisions")
                        and self.session.get("selection_result", {}).get("action") != "blocked",
                        "No-finding requires untested completed discovery")
        with self.budget("finalization", FINALIZATION_SECONDS):
            return self._finish(outcome, reason)

    def finish_operation(self):
        request = self.session["finish_request"]
        require(set(request) == {"outcome", "reason"}, "Invalid protected finish request")
        return self.finish(request["outcome"], request["reason"])

    def _finish(self, outcome, reason):
        a = self.session["attempt"]
        if self.session.get("discovery") is not None:
            d = self.session["discovery"]
            # A rejected/noisy/failed experiment cannot be relabelled as healthy discovery.
            if outcome == "completed_no_finding":
                self.diagnostics()
            if a is not None and a["outcome"] is None:
                a.update(outcome=outcome, reason=c.sanitize(reason), lifecycle="interrupted")
            d.update(outcome=outcome, reason=c.sanitize(reason), lifecycle="interrupted")
            self.session["phase"] = "finished"
            self.stage_evidence()
            self.checkpoint()
            self.report()
            return self.status()
        a.update(outcome=outcome, reason=c.sanitize(reason), lifecycle="interrupted")
        a["checkpoint"]["next_action"] = "Reconcile owner and retained evidence before retry"
        self.stage_evidence()
        self.checkpoint()
        self.report()
        return copy.deepcopy(a)

    def report(self):
        a = self.session["attempt"] or self.session["discovery"]
        text = f"Compiler self-profile: {a['outcome'] or a['lifecycle']}\n\n{c.sanitize(a['reason'])}\n"
        digest = hashlib.sha256(text.encode()).hexdigest()
        if self.session.get("report_sha256") != digest:
            (self.root / "summary.txt").write_text(text, encoding="utf-8")
            if os.environ.get("GITHUB_STEP_SUMMARY"):
                with open(os.environ["GITHUB_STEP_SUMMARY"], "a", encoding="utf-8") as stream:
                    stream.write(text)
            self.session["report_sha256"] = digest
            self.save("session", self.session)

    def stage_evidence(self, extra_entries=(), *, dump_workspace=None):
        owner = self.session["attempt"] or self.session["discovery"]
        private = self.root / "private"
        private.mkdir(mode=0o700, exist_ok=True)
        public = self.root / "public"
        public.mkdir(mode=0o700, exist_ok=True)
        summary = {"schema_version": 1, "kind": "workflow-summary", "status": owner["outcome"] or owner["lifecycle"],
                   "attempt_id": owner["id"],
                   "conclusion": artifacts._conclusion(owner["reason"] or "Incomplete investigation", private)[:2048],
                   "profile_status": self.session["attempt"]["collection"]["status"] if self.session["attempt"]
                   else "collected" if self.session.get("profile") else "pending"}
        if owner["outcome"] == "measured_improvement":
            summary.update(status="validating", conclusion="Candidate measurements require evidence retention and publication validation")
        entries, inventory_errors = list(extra_entries), []
        existing = [r for record in (self.session["attempt"], self.session.get("discovery")) if record
                    for r in record.get("evidence", [])]
        for trace in (private / "profile" / "compiler.nettrace",
                      private / "dump-session" / "profile" / "compiler.nettrace"):
            try:
                if not trace.exists():
                    continue
                checked = artifacts._path(trace)
                sha256, size, _, _ = artifacts._read_file(checked)
                identity = {"path": str(checked), "sha256": sha256, "size_bytes": size}
                if not any(r["sha256"] == identity["sha256"] and r["availability"] in ("pending", "available")
                           for r in existing) and not any(entry["kind"] == "profile-trace"
                                                          and entry["file"]["sha256"] == identity["sha256"]
                                                          for entry in entries):
                    entries.append({"file": identity, "kind": "profile-trace",
                                    "conclusion": "Diagnostic trace; not an unprofiled acceptance measurement"})
            except (ValueError, OSError) as error:
                inventory_errors.append("Trace inventory: " + artifacts._conclusion(str(error), private)[:512])
        config_path = self.root / "retention-config.json"
        config_error = None
        try:
            encryption = read_input(config_path) if config_path.exists() else None
        except (ValueError, OSError) as error:
            encryption, config_error = None, "Owner encryption configuration: " + artifacts._conclusion(str(error), private)[:512]
        if inventory_errors or config_error:
            summary["reason"] = "; ".join(inventory_errors + ([config_error] if config_error else []))[:512]
        summary_path = private / ("summary-" + m.fingerprint_inputs(summary) + ".json")
        m.write_json(summary_path, summary)
        entries.insert(0, {"file": m.file_record(summary_path), "kind": "summary", "conclusion": summary["conclusion"]})
        provenance = {"run_id": owner["run"]["id"], "fingerprint": m.fingerprint_inputs(
            [owner["source_revision"], owner["run"], self.session.get("manifest")])}
        job_id = m.fingerprint_inputs([entries, encryption, provenance])
        jobs = self.session.setdefault("retention_jobs", {})
        if job_id not in jobs and any(not any(
                entry == prior and (entry["kind"] == "summary" or job["encryption"] == encryption)
                for job in jobs.values() for prior in job["entries"]) for entry in entries):
            require(len(jobs) < artifacts.MAX_ENTRIES, "Retention job inventory exceeded")
            jobs[job_id] = {"entries": entries, "encryption": encryption, "provenance": provenance,
                            "created_at": self.now().isoformat(),
                            "owner": (self.session.get("discovery") or owner)["id"], "dump": None}
            if dump_workspace is not None:
                jobs[job_id]["dump"] = {"path": str(dump_workspace.path),
                                         "identity": list(dump_workspace.identity),
                                         "receipt": artifacts.claim_dump(dump_workspace, extra_entries[0]["file"])}
                receipt = jobs[job_id]["dump"]["receipt"]
                receipt.update(path=str(receipt["path"]), identity=list(receipt["identity"]))
            self.save("session", self.session)
        results = []
        for identity, job in jobs.items():
            try:
                results.append(self._stage_evidence_job(identity, job, dump_workspace))
            except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
                results.append({"status": "blocked_retention", "files": [],
                                "reason": artifacts._conclusion(str(error), private)[:512]})
        states = {r["status"] for r in results}
        status = "blocked_retention" if "blocked_retention" in states or config_error or inventory_errors else (
            next(iter(states)) if len(states) == 1 else "pending")
        result = {"status": status, "staged": results, "files": [f for r in results for f in r["files"]],
                  "limits": {"total_bytes": artifacts.PUBLIC_BYTES, "compact_bytes": artifacts.COMPACT_BYTES},
                  "retention_days": artifacts.RETENTION_DAYS}
        if "reason" in summary:
            result["reason"] = summary["reason"]
        self.session["retention"] = result
        self.save("session", self.session)
        return result

    def _stage_evidence_job(self, identity, job, active_dump):
        destination = self.root / "public" / identity
        require(mem.digest(identity) and job["provenance"]["run_id"] ==
                (self.session["attempt"] or self.session["discovery"])["run"]["id"], "Retention owner differs")
        owner = next(record for record in (self.session["attempt"], self.session.get("discovery"))
                     if record is not None and record["id"] == job["owner"])
        expired = mem.timestamp(job["created_at"]) + timedelta(days=artifacts.RETENTION_DAYS) <= self.now()
        owned, dump_error = active_dump, None
        if job["dump"] is not None:
            try:
                custody = job["dump"]
                path = artifacts._path(custody["path"], exists=False)
                require(path == self.root / "private" / "dump-session", "Dump owner changed")
                if path.exists():
                    require(list(artifacts._identity(path.stat())) == custody["identity"], "Dump owner changed")
                    if owned is None:
                        owned = artifacts.DumpWorkspace(path, tuple(custody["identity"]))
                        artifacts._DUMP_WORKSPACES.add(owned)  # Restored only from authenticated controller custody.
            except (ValueError, OSError) as error:
                dump_error = error
        if destination.exists():
            manifest = read_input(destination / "manifest.json")
            require(manifest["created_at"] == job["created_at"] and all(
                r["provenance"] == job["provenance"] for r in manifest["artifacts"])
                and {(r["type"], r["source_sha256"]) for r in manifest["artifacts"]} ==
                    {(entry["kind"], entry["file"]["sha256"]) for entry in job["entries"]},
                    "Retained manifest owner or source differs")
            saved = next((s for s in self.session.get("artifacts", []) if s["root"] == str(destination)), None)
            if saved is not None:
                m.verify_files([saved["manifest"]])
            custody_name = "retention-" + identity
            if (self.root / (custody_name + ".json")).exists():
                require(self.load(custody_name) == manifest, "Retained manifest custody changed")
            else:
                require(saved is not None, "Retained manifest custody unavailable")
        elif expired:
            manifest = None
        else:
            if dump_error is not None:
                raise dump_error
            used = compact = 0
            for other in self.session["retention_jobs"]:
                path = self.root / "public" / other
                saved = next((s for s in self.session.get("artifacts", []) if s["root"] == str(path)), None)
                custody_name = "retention-" + other
                sealed = (self.root / (custody_name + ".json")).exists()
                require(path.exists() or saved is None and (other == identity or not sealed),
                        "Retained artifact inventory missing; budget unavailable")
                if saved is not None:
                    m.verify_files([saved["manifest"]])
                if not path.exists():
                    continue
                prior = read_input(path / "manifest.json")
                require(self.load(custody_name) == prior if sealed else saved is not None,
                        "Retained manifest custody changed; budget unavailable")
                inventory = [(p.suffix, artifacts._path(p).stat().st_size) for p in path.iterdir()]
                used += max(prior["total_bytes"], sum(size for _, size in inventory))
                compact += max(prior.get("compact_bytes", prior["total_bytes"]),
                               sum(size for suffix, size in inventory if suffix == ".json"))
            manifest = artifacts.stage_artifacts(self.root / "private", destination, job["entries"],
                job["created_at"], encryption=job["encryption"], provenance=job["provenance"],
                dump_workspace=owned, used_bytes=used, used_compact_bytes=compact,
                seal_manifest=lambda value: self.save("retention-" + identity, value))
        result = {"status": "expired", "files": [], "artifacts": []}
        if manifest is not None:
            result = artifacts.retention_result(destination, manifest, self.now())
            for record in manifest["artifacts"]:
                if not any(r["artifact_id"] == record["artifact_id"] and r["collected_at"] == record["collected_at"]
                           for r in owner.setdefault("evidence", [])):
                    owner["evidence"].append(record)
            staged = {"root": str(destination), "manifest": m.file_record(destination / "manifest.json")}
            if staged not in self.session.setdefault("artifacts", []):
                self.session["artifacts"].append(staged)
            self.save("session", self.session)
        for status in result["artifacts"]:
            acknowledged = next((r for r in owner["evidence"] if r["artifact_id"] == status["artifact_id"]
                                 and r["collected_at"] == manifest["created_at"]
                                 and r["availability"] == "available"), None)
            if acknowledged is not None:
                status.update(mem.artifact_availability(acknowledged, self.now()))
        if result["artifacts"] and all(s["available"] for s in result["artifacts"]):
            result["status"] = "available"
        if job["dump"] is not None and active_dump is None:
            try:
                if dump_error is not None:
                    raise dump_error
                receipt = dict(job["dump"]["receipt"])
                receipt.update(path=Path(receipt["path"]), identity=tuple(receipt["identity"]))
                if receipt["path"].exists():
                    artifacts.remove_owned_dump(owned, receipt, "Recovered analyzed dump cleanup", self.now())
            except (ValueError, OSError) as error:
                result.update(status="blocked_retention", reason=artifacts._conclusion(
                    "Owned dump cleanup: " + str(error), self.root / "private")[:512])
        return result

    def acknowledge_evidence(self, sink):
        """Protected runtime API only; agent messages cannot submit sink receipts."""
        self.checkpoint()
        for staged in self.session.get("artifacts", []):
            m.verify_files([staged["manifest"]])
            manifest = read_input(staged["manifest"]["path"])
            pending = {(r["artifact_id"], r["collected_at"])
                       for owner in (self.session["attempt"], self.session.get("discovery")) if owner
                       for r in owner.get("evidence", []) if r["availability"] == "pending"
                       and mem.artifact_availability(r, self.now())["reason"] != "expired"}
            if not any((r["artifact_id"], r["collected_at"]) in pending for r in manifest["artifacts"]):
                continue
            records = artifacts.acknowledge_artifacts(staged["root"], manifest, self.now, sink)
            for owner in (self.session["attempt"], self.session.get("discovery")):
                if owner is not None:
                    replacements = {(r["artifact_id"], r["collected_at"]): r for r in records
                                    if r["availability"] == "available"}
                    owner["evidence"] = [replacements.get((r["artifact_id"], r["collected_at"]), r)
                                         if r["availability"] == "pending" else r
                                         for r in owner.get("evidence", [])]
            self.save("session", self.session)
            self.checkpoint()
        return self.stage_evidence()

    def accept_experiment(self, candidate):
        a = self.session["attempt"]
        measured = mem.load_state(candidate / "attempt.json")
        require(measured["status"] == "ready", measured["reason"])
        final = measured["state"]["attempts"][a["id"]]
        # Planned identities remain immutable; measurements add actual frozen identities.
        final["fingerprints"].update(a["fingerprints"])
        for key in ("hypothesis", "run", "dependencies", "retry", "supersedes", "lease", "evidence"):
            final[key] = copy.deepcopy(a[key])
        final.update(publication=None, lifecycle="validating")
        if final["outcome"] == "measured_improvement":
            mem._attest_correctness(final)
        self.session.update(attempt=final, phase="validated")
        retention = self.stage_evidence()
        if final["outcome"] == "measured_improvement" and retention["status"] == "blocked_retention":
            final.update(outcome="blocked_collection", reason="Required evidence retention is blocked",
                         lifecycle="interrupted")
        self.checkpoint()
        self.report()
        return final

    def investigate(self):
        """Trusted preparation only. The ready adapter owns the subsequent interaction."""
        require(self.session is not None, "Preflight reservation required before investigation")
        try:
            with self.budget("setup", SETUP_SECONDS):
                capabilities(self.root, self.repository)
                request = self.session.get("discovery_request", self.session["request"])
                manifest_path = self.root / "workload.json"
                require(not manifest_path.exists(), "Prepared inputs already exist; resume instead")
                m.prepare_workload(request["base_sha"], self.root / "frozen", manifest_path,
                                   request["primary_metric"])
                e.frozen_records(c.read_bounded_json(manifest_path))
                self.session.update(phase="ready", manifest=m.file_record(manifest_path),
                                    ready_at=self.now().isoformat())
                if self.session.get("discovery"):
                    self.session["discovery"]["lifecycle"] = "ready"
                self.checkpoint()
            return self.status() if self.session.get("discovery") else self.session["attempt"]
        except KeyboardInterrupt:
            return self.finish("blocked_setup", "Aggregate preparation deadline or cancellation")
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            return self.finish("blocked_setup", "setup: " + c.sanitize(str(error)))

    def current_attempt(self):
        saved = self.load("session")
        require(saved == self.session, "In-memory session differs from protected custody")
        a = copy.deepcopy(saved["attempt"])
        require(a["outcome"] == "measured_improvement", "No measured candidate")
        candidate_root = self.candidate_root()
        state = e.verify_candidate(candidate_root)
        require(state["request"] == saved["request"] and state["patch_sha256"] == a["summary"]["patch_sha256"],
                "Candidate request or proposal differs")
        e.frozen_records(c.read_bounded_json(candidate_root / "workload.json"))
        m.verify_files([saved["manifest"], saved["profile"]])
        require(c.analyze_profile(saved["profile"]["path"])["status"] == "collected",
                "Diagnostic evidence unavailable")
        records = state["raw_measurements"]
        m.verify_files(records)
        baseline, candidate = [c.read_bounded_json(r["path"]) for r in records]
        require(mem.measurement_record(baseline, candidate) == a["measurement"], "Measurement receipts changed")
        # Only controller-sealed captures may be rebound after process restart. Each
        # capture is still freshly reconciled with selection, discovery and XML.
        for result in a["correctness"]["results"]:
            result["execution"] = e.execution_signature(result)
        mem._attest_correctness(a)
        mem.acceptance_ready(a)
        return a, state

    def validate_output(self, payload=None):
        try:
            return self._validate_output(payload)
        except KeyboardInterrupt:
            if self.session.get("discovery") and self.session["discovery"]["outcome"] is None:
                self.finish("cancelled", "Output validation deadline or cancellation before publisher output")
            raise

    def authorize(self):
        if self.session.get("discovery") and "finalization" not in self.load("clocks"):
            self.poll_active()
        with self.budget("finalization", FINALIZATION_SECONDS):
            return self.validate_output()

    def _validate_output(self, payload):
        clock = "finalization" if "finalization" in self.budgets else "active"
        allowance = FINALIZATION_SECONDS if clock == "finalization" else ACTIVE_SECONDS
        if self.session.get("discovery"):
            require(self.session["phase"] == "accepted" and self.session["discovery"]["outcome"] is None,
                    "Only an active accepted experiment can authorize output")
            self.remaining(clock, allowance)
        a, state = self.current_attempt()
        tree = Path(state["tree"])
        # Reapply the retained bytes in a new owned clone and inspect Git's actual index.
        with tempfile.TemporaryDirectory(prefix="output-", dir=self.root) as directory:
            verified = e.prepare_candidate(self.repository, Path(directory) / "candidate", state["request"],
                                           (self.candidate_root() / "proposal.patch").read_bytes())
            require(verified["source_tree"] == state["source_tree"], "Applied proposal tree differs")
        head = self.session.get("head")
        if head is None:
            head = e.git(tree, "-c", "user.name=Compiler performance validator",
                         "-c", "user.email=compiler-perf@example.invalid",
                         "commit-tree", state["source_tree"], "-p", a["source_revision"], "-m",
                         "Measured compiler optimization\n\nCo-authored-by: Copilot "
                         "<223556219+Copilot@users.noreply.github.com>").decode().strip()
        require(e.git(tree, "show", "-s", "--format=%T %P", head).decode().strip() ==
                state["source_tree"] + " " + a["source_revision"], "Publication commit tree/base changed")
        patch = e.git(tree, "format-patch", "--stdout", "--no-stat", "--no-signature", "-1", head)
        require(len(patch) <= e.MAX_PATCH, "Publication patch budget exceeded")
        patch_hash = hashlib.sha256(patch).hexdigest()
        intent = mem.publication_intent(a, head)
        percent = a["measurement"]["comparison"]["primary"]["improvement_percent"]
        expected = safe_output(a["id"], a["source_revision"], head, state["source_tree"], patch_hash,
                               "Improve compiler " + a["hypothesis"]["area"],
                               f"Measured {percent:.2f}% improvement on frozen Release compiler sources. "
                               f"Correctness and unprofiled A/A and A/B gates passed.\n\n"
                               f"Run: {a['run']['url']}\nIntent: {intent['intent_id']}")
        payload = expected if payload is None else payload
        validate_payload(payload, expected)
        require(not mem.workflow_prs(mem.query_prs(self.pages)), "Outstanding optimization PR")
        self.api.verify_source({"source_ref": DEFAULT_BRANCH, "source_revision": a["source_revision"]})
        if self.session.get("discovery"):
            self.remaining(clock, allowance)
        a.update(publication=intent, lifecycle="publication_pending")
        self.session.update(attempt=a, head=head)
        self.checkpoint()  # Intent acknowledgment precedes any publisher-visible output.
        if self.session.get("discovery"):
            self.remaining(clock, allowance)
        output = self.root / "output"
        require(e.unredirected(output, missing_ok=True), "Publication output redirected")
        output.mkdir(exist_ok=True)
        if list(output.iterdir()):
            require({p.name for p in output.iterdir()} == {"proposal.patch", "safe-output.json"}
                    and e.unredirected(output / "proposal.patch")
                    and m.file_record(output / "proposal.patch")["sha256"] == patch_hash,
                    "Publication output differs; preserve rather than overwrite")
            validate_payload(read_input(output / "safe-output.json"), payload)
        else:
            (output / "proposal.patch").write_bytes(patch)
            m.write_json(output / "safe-output.json", payload)
        binding = approval(a, payload, state["source_tree"], patch_hash)
        self.save("approval", dict(binding, head=head, files=[m.file_record(p) for p in sorted(output.iterdir())]))
        return payload

    def finalize(self, *, publisher=None):
        require(self.session is not None, "Missing lifecycle session")
        a = self.session["attempt"]
        require(a["publication"] is not None, "No acknowledged publication intent")
        intent = a["publication"]
        prs = mem.query_prs(self.pages)
        outstanding = mem.workflow_prs(prs)
        for pr in outstanding:
            require(pr["author"] == AUTHOR and pr["base_ref"] == DEFAULT_BRANCH and pr["draft"] is True
                    and pr["intent_id"] == intent["intent_id"]
                    and pr["head_ref"] == mem.WORKFLOW_MARKER + "/" + a["id"],
                    "Outstanding PR differs from sealed publication")
        reconciled = mem.reconcile_publication(a, lambda _: {
            "status": 200, "items": prs, "complete": True, "next": None}, self.now())
        if reconciled["action"] == "acknowledge":
            self.session["attempt"] = reconciled["attempt"]
            if self.session.get("discovery") and self.session["discovery"]["outcome"] is None:
                self.session["discovery"].update(outcome="completed", lifecycle="interrupted",
                                                 reason="Accepted experiment publication acknowledged")
                self.session["phase"] = "finished"
            self.checkpoint()
            self.report()
            return reconciled
        if publisher is not None:
            require(not outstanding, "Outstanding PR differs from sealed publication")
            current, state = self.current_attempt()
            sealed = self.load("approval")
            output = self.root / "output"
            require({p.name for p in output.iterdir()} == {"proposal.patch", "safe-output.json"},
                    "Unexpected output transport/files")
            m.verify_files(sealed["files"])
            payload = read_input(output / "safe-output.json")
            binding = approval(current, payload, state["source_tree"], m.file_record(output / "proposal.patch")["sha256"])
            require(all(sealed[k] == v for k, v in binding.items()), "Publication authority changed")
            require(publisher.branch(payload["items"][0]["branch"]) in (None, intent["head_sha"]),
                    "Unmerged publication branch differs")
            publisher.publish(payload, intent["head_sha"], intent)
            return self.finalize()
        self.session["attempt"] = reconciled["attempt"]
        self.checkpoint()
        self.report()
        return reconciled


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    sub = parser.add_subparsers(dest="command", required=True)
    for name in ("preflight", "investigate", "worker", "checkpoint", "validate-output", "finalize"):
        command = sub.add_parser(name)
        command.add_argument("--root", required=True, help="Protected controller-owned directory outside source")
        command.add_argument("--repository", required=True, help="Trusted helper/default-branch checkout")
        command.add_argument("--journal", required=True, help="Owned local Git journal transport cache")
        if name == "preflight":
            command.add_argument("--input", required=True, help="Bounded experiment request JSON")
        if name == "worker":
            command.add_argument("--operation", required=True,
                                 choices=("profile", "validate", "dump", "select", "finish", "authorize"))
    args = parser.parse_args(argv)
    try:
        from transport import JournalTransport
        controller = Controller(args.root, args.repository, JournalTransport(args.journal), GitHub())
        with e.owned_lock(controller.root):
            if args.command == "preflight":
                result = controller.preflight(read_input(args.input))
            elif args.command == "worker":
                require(sys.platform == "linux", "Task workers require Linux; no host fallback")
                signal.signal(signal.SIGTERM, lambda *_: (_ for _ in ()).throw(KeyboardInterrupt()))
                if args.operation == "dump":
                    request = controller.session["dump_request"]
                    result = controller.profile(request["reason"], request["hypothesis"])
                elif args.operation == "finish":
                    result = controller.finish_operation()
                else:
                    result = getattr(controller, args.operation)()
            elif args.command == "validate-output":
                result = controller.authorize()
            elif args.command == "finalize":
                with controller.budget("finalization", FINALIZATION_SECONDS):
                    result = controller.finalize()
            else:
                result = getattr(controller, args.command.replace("-", "_"))()
        print(mem.encoded(result).decode(), end="")
        if result.get("outcome") == "cancelled":
            return 130
        return 2 if result.get("action") == "blocked" or (result.get("outcome") or "").startswith("blocked_") else 0
    except KeyboardInterrupt:
        return 130
    except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
        print(mem.encoded({"outcome": "blocked_setup", "reason": c.sanitize(str(error))}).decode(), end="")
        return 2


if __name__ == "__main__":
    sys.exit(main())
