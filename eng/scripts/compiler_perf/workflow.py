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
import time

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import experiment as e
import memory as mem
import artifacts

m, c, require = mem.m, mem.c, mem.require
ACTIVE_SECONDS = 14400
SETUP_SECONDS = 7200
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
    c.verify_tools(str(dotnet), root)


class ActiveSession:
    """Main-thread timer interrupts owned child waits, including long pack/test commands."""

    def __init__(self, checkpoint, *, clock=time.monotonic, allowance=ACTIVE_SECONDS):
        self.checkpoint, self.clock, self.allowance = checkpoint, clock, allowance
        self.start, self.previous = None, None

    def poll(self, *_):
        self.checkpoint()
        if self.clock() - self.start >= self.allowance:
            raise KeyboardInterrupt("Active investigation deadline reached")

    def __enter__(self):
        self.start = self.clock()
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
    def __init__(self, root, repository, transport, api, *, now=None):
        self.root, self.repository = Path(root).absolute(), Path(repository).resolve(strict=True)
        require(e.unredirected(self.root, missing_ok=True), "Controller storage must be unredirected")
        self.root = self.root.resolve()
        require(not self.root.is_relative_to(self.repository), "Controller storage must be outside source")
        self.transport, self.api = transport, api
        self.now = now or (lambda: datetime.now(timezone.utc))
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
        require(e.unredirected(self.root / (name + ".json"), missing_ok=True) and
                e.unredirected(self.root / (name + ".json.tmp"), missing_ok=True), "Redirected controller record")
        data = {"schema_version": 1, "value": value,
                "seal": hmac.digest(self.key, name.encode() + mem.encoded(value), "sha256").hex()}
        require(len(mem.encoded(data)) <= mem.LIMITS["file_bytes"], "Controller record budget exceeded")
        m.write_json(self.root / (name + ".json"), data)

    def load(self, name):
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
        m.write_json(self.root / "planned-inputs.json", plan)
        checks = e.correctness_plan(request["compiler_paths"] + [r["path"] for r in request["regressions"]] +
                                   [e.RELEASE_NOTE], request["regressions"], "net11.0")
        now = self.now()
        return {"id": request["attempt_id"], "source_revision": base,
                "hypothesis": {"id": request["attempt_id"], "description": request["hypothesis"],
                               "area": request["compiler_paths"][0].split("/")[2],
                               "operation": ";".join(request["compiler_paths"])},
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
                "summary": {"evidence": "Reserved before bootstrap; planned-inputs.json binds preparation",
                            "change": request["complexity"], "patch_sha256": hashlib.sha256(b"").hexdigest()},
                "evidence": [], "measurement": None, "collection": {"status": "pending", "reason": ""},
                "correctness": {"schema_version": 5, "changes": request["compiler_paths"] +
                                [r["path"] for r in request["regressions"]] + [e.RELEASE_NOTE],
                                "regressions": request["regressions"], "target_framework": "net11.0",
                                "plan": checks, "plan_sha256": m.fingerprint_inputs(checks),
                                "required": [p["id"] for p in checks], "results": [], "nonce": "reserved",
                                "cwd": str(self.root / "candidate" / "tree"), "tool": {},
                                "inputs": m.file_record(self.root / "planned-inputs.json"), "validation": None},
                "publication": None, "supersedes": None, "notes": [],
                "lease": {"run_id": request["run_id"], "run_attempt": 1, "checkpoint_at": now.isoformat(),
                          "expires_at": (now + timedelta(seconds=SETUP_SECONDS + ACTIVE_SECONDS)).isoformat()},
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

    def preflight(self, request):
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
            a = self.session["attempt"]
            mem.validate_attempt(a)
            state, head = self.transport.read()
            previous = state["attempts"].get(a["id"])
            require(previous is not None and previous["run"] == a["run"], "Reservation owner changed")
            baseline = self.session["canonical_attempt_sha256"]
            require(baseline == m.fingerprint_inputs(previous) or a == previous,
                    "Canonical attempt changed since the acknowledged checkpoint")
            a["lease"]["checkpoint_at"] = self.now().isoformat()
            candidate = mem.merge_attempt(state, a, previous)
            receipt = self.persist(state, head, candidate)
            self.session["persistence"] = receipt
            self.session["canonical_attempt_sha256"] = m.fingerprint_inputs(a)
            self.save("session", self.session)
            return receipt
        finally:
            self.busy = False

    def finish(self, outcome, reason):
        require(outcome in mem.OUTCOMES and mem.text(reason), "Invalid lifecycle conclusion")
        a = self.session["attempt"]
        a.update(outcome=outcome, reason=c.sanitize(reason), lifecycle="interrupted")
        a["checkpoint"]["next_action"] = "Reconcile owner and retained evidence before retry"
        self.stage_evidence()
        self.checkpoint()
        self.report()
        return copy.deepcopy(a)

    def report(self):
        a = self.session["attempt"]
        text = f"Compiler self-profile: {a['outcome'] or a['lifecycle']}\n\n{c.sanitize(a['reason'])}\n"
        digest = hashlib.sha256(text.encode()).hexdigest()
        if self.session.get("report_sha256") != digest:
            (self.root / "summary.txt").write_text(text, encoding="utf-8")
            if os.environ.get("GITHUB_STEP_SUMMARY"):
                with open(os.environ["GITHUB_STEP_SUMMARY"], "a", encoding="utf-8") as stream:
                    stream.write(text)
            self.session["report_sha256"] = digest
            self.save("session", self.session)

    def stage_evidence(self):
        a = self.session["attempt"]
        private = self.root / "private"
        private.mkdir(mode=0o700, exist_ok=True)
        public = self.root / "public"
        public.mkdir(mode=0o700, exist_ok=True)
        summary = {"schema_version": 1, "kind": "workflow-summary", "status": a["outcome"] or a["lifecycle"],
                   "attempt_id": a["id"], "conclusion": c.sanitize(a["reason"] or "Incomplete investigation")[:2048],
                   "profile_status": a["collection"]["status"]}
        summary_path = private / "summary.json"
        m.write_json(summary_path, summary)
        entries = [{"file": m.file_record(summary_path), "kind": "summary", "conclusion": summary["conclusion"]}]
        trace = private / "profile" / "compiler.nettrace"
        if trace.is_file():
            entries.append({"file": m.file_record(trace), "kind": "profile-trace",
                            "conclusion": "Diagnostic trace; not an unprofiled acceptance measurement"})
        destination = public / hashlib.sha256(mem.encoded(summary)).hexdigest()
        if not destination.exists():
            manifest = artifacts.stage_artifacts(private, destination, entries, self.now())
            a["evidence"].extend(manifest["artifacts"])
            self.session.setdefault("artifacts", []).append(
                {"root": str(destination), "manifest": m.file_record(destination / "manifest.json")})
        for staged in self.session.get("artifacts", []):
            m.verify_files([staged["manifest"]])
            artifacts.verify_artifacts(Path(staged["root"]), read_input(staged["manifest"]["path"]), self.now())

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
        self.stage_evidence()
        self.checkpoint()
        self.report()
        return final

    def investigate(self):
        require(self.session is not None, "Preflight reservation required before investigation")
        a, request = self.session["attempt"], self.session["request"]
        require(a["outcome"] is None and a["publication"] is None, "Completed work cannot restart")
        state, _ = self.transport.read()
        require(state["attempts"].get(a["id"]) == a and self.session["persistence"]["acknowledged"],
                "Reservation changed or was not acknowledged")
        phase = "setup"
        try:
            capabilities(self.root, self.repository)
            with ActiveSession(self.checkpoint, allowance=SETUP_SECONDS):
                manifest_path = self.root / "workload.json"
                m.prepare_workload(request["base_sha"], self.root / "frozen", manifest_path,
                                   request["primary_metric"])
                manifest = c.read_bounded_json(manifest_path)
                e.frozen_records(manifest)
            self.session.update(phase="collecting", manifest=m.file_record(manifest_path))
            a["lifecycle"] = "collecting"
            self.checkpoint()
            with ActiveSession(self.checkpoint):
                phase = "collection"
                private = self.root / "private"
                private.mkdir(mode=0o700, exist_ok=True)
                profile_root = private / "profile"
                c.collect_profile(manifest, manifest["baseline_compiler"], profile_root)
                profile_path = profile_root / "evidence-manifest.json"
                profile = c.analyze_profile(profile_path)
                require(profile["status"] == "collected", profile.get("reason", "Diagnostic collection failed"))
                a["collection"] = {"status": "collected", "reason": ""}
                self.session["profile"] = m.file_record(profile_path)
                self.checkpoint()
                proposal = self.root / "inbox" / "proposal.patch"
                require(e.unredirected(proposal, missing_ok=True), "Proposal redirected")
                if not proposal.exists():
                    return self.finish("completed_no_finding", "Completed diagnostics; no proposed compiler change")
                phase = "experiment"
                with proposal.open("rb") as stream:
                    patch = stream.read(e.MAX_PATCH + 1)
                candidate = self.root / "candidate"
                e.prepare_candidate(self.repository, candidate, request, patch, manifest=manifest)
                a["lifecycle"] = "experimenting"
                self.session["phase"] = phase
                self.checkpoint()
                result = e.validate_candidate(candidate, profile_path, checkpoint_on_cancel=True)
                require((candidate / "attempt.json").exists(), result["reason"])
                return self.accept_experiment(candidate)
        except KeyboardInterrupt:
            return self.finish("cancelled", "Incomplete investigation: owned children stopped; retained checkpoint")
        except (ValueError, OSError, KeyError, TypeError, subprocess.SubprocessError) as error:
            if phase == "collection":
                a["collection"] = {"status": "blocked_collection", "reason": c.sanitize(str(error))}
            return self.finish("blocked_collection" if phase == "collection" else "blocked_setup",
                               f"{phase}: {c.sanitize(str(error))}")

    def current_attempt(self):
        saved = self.load("session")
        require(saved == self.session, "In-memory session differs from protected custody")
        a = copy.deepcopy(saved["attempt"])
        require(a["outcome"] == "measured_improvement", "No measured candidate")
        state = e.verify_candidate(self.root / "candidate")
        require(state["request"] == saved["request"] and state["patch_sha256"] == a["summary"]["patch_sha256"],
                "Candidate request or proposal differs")
        e.frozen_records(c.read_bounded_json(self.root / "candidate" / "workload.json"))
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
        a, state = self.current_attempt()
        tree = Path(state["tree"])
        # Reapply the retained bytes in a new owned clone and inspect Git's actual index.
        with tempfile.TemporaryDirectory(prefix="output-", dir=self.root) as directory:
            verified = e.prepare_candidate(self.repository, Path(directory) / "candidate", state["request"],
                                           (self.root / "candidate" / "proposal.patch").read_bytes())
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
        a.update(publication=intent, lifecycle="publication_pending")
        self.session.update(attempt=a, head=head)
        self.checkpoint()  # Intent acknowledgment precedes any publisher-visible output.
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
    for name in ("preflight", "investigate", "checkpoint", "validate-output", "finalize"):
        command = sub.add_parser(name)
        command.add_argument("--root", required=True, help="Protected controller-owned directory outside source")
        command.add_argument("--repository", required=True, help="Trusted helper/default-branch checkout")
        command.add_argument("--journal", required=True, help="Owned local Git journal transport cache")
        if name == "preflight":
            command.add_argument("--input", required=True, help="Bounded experiment request JSON")
    args = parser.parse_args(argv)
    try:
        from transport import JournalTransport
        controller = Controller(args.root, args.repository, JournalTransport(args.journal), GitHub())
        with e.owned_lock(controller.root):
            if args.command == "preflight":
                result = controller.preflight(read_input(args.input))
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
