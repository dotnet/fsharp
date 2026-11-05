"""Versioned experiment journal, not instructions for the investigating agent.

state.json contains generation/base_revision and an append-only attempt index.
Version 1 remains unchanged until the first discovery adds the version-2 index.
Discovery's "completed" outcome closes the session; it does not attest an experiment.
An attempt separates immutable identities, lifecycle, terminal outcome, retry
dependencies, evidence provenance, correctness, and publication intent/receipt.
Unknown or unreadable state blocks work; only a verified absence initializes it.

Native GH AW v0.76.1 notes use memory/compiler-self-profile and
/tmp/gh-aw/repo-memory/default, with JSON-only limits below. They are untrusted
public proposals, never journal updates. The canonical branch is exclusively
for a fixed default-branch helper in a guarded contents:write job, whose token
and checkout must NOT be available to the optimizing agent. No workflow, token
grant, remote push, or native-writer validation hook is installed here.

CLI JSON input (--input file, otherwise stdin):
  preflight: {state, request, pr_pages, runs, now} for offline planning;
             --repository uses live read-only GitHub queries and commits the
             reservation with CAS. Expensive work must wait for the caller's
             acknowledged remote journal commit, not just this local receipt.
  checkpoint --output file: local state, atomic but not trusted approval.
  merge --repository repo: {base, expected_head, attempt}; guarded writer only.
  summarize: state; includes artifact availability at --now.

Git receipts distinguish local versioned commits from remote acknowledgment.
Publish them only after the guarded job successfully persists the fixed branch.
Conflicts retain candidate refs; never force-push or use native ours-win merges.
"""

import argparse
import copy
from datetime import datetime, timedelta, timezone
import difflib
import hmac
import importlib.util
import json
import os
from pathlib import Path
import re
import secrets
import subprocess
import sys
import tempfile
from urllib.parse import quote
import uuid

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

spec = importlib.util.spec_from_file_location("collection", Path(__file__).with_name("collection.py"))
c = importlib.util.module_from_spec(spec)
spec.loader.exec_module(c)
m = c.m
require = m.require
STATE_BRANCH = "memory/compiler-self-profile-state"
NOTES_BRANCH = "memory/compiler-self-profile"
NOTES_PATH = "/tmp/gh-aw/repo-memory/default"
WORKFLOW_MARKER = "compiler-self-profile"
REPOSITORY = "dotnet/fsharp"
LIMITS = {"files": 256, "file_bytes": 262144, "patch_bytes": 1048576}
OUTCOMES = {"completed_no_finding", "blocked_setup", "blocked_collection", "inconclusive",
            "rejected", "measured_improvement", "cancelled"}
DISCOVERY_OUTCOMES = (OUTCOMES - {"measured_improvement"}) | {"completed"}
LIFECYCLES = ("reserved", "collecting", "experimenting", "validating",
              "publication_pending", "published", "interrupted")
DISCOVERY_LIFECYCLES = ("reserved", "setup", "ready", "collecting", "awaiting_decision",
                       "experimenting", "interrupted")
FINGERPRINTS = ("source", "input", "workload", "compiler", "bootstrap", "sdk", "runtime",
                "toolchain", "runner", "policy", "candidate", "capture", "reducer")
_VALIDATOR_KEY = secrets.token_bytes(32)
ACCEPTANCE_METRICS = [{"name": name, "unit": "bytes" if name.endswith("bytes") else "ms",
                       "aggregation": "unprofiled-paired-samples"} for name in m.METRICS]
PROFILE_METRICS = (
    ("allocation_counter_observed_bytes", "bytes", "sum-observed-increments"),
    ("allocated_bytes_estimate", "bytes", "sampled-gc-allocation-ticks"),
)


class Conflict(ValueError):
    def __init__(self, reason, candidate_ref=None):
        super().__init__(reason)
        self.candidate_ref = candidate_ref


def encoded(value):
    return (json.dumps(value, sort_keys=True, separators=(",", ":"), ensure_ascii=True, allow_nan=False) + "\n").encode()


def decoded(data):
    def unique(pairs):
        result = {}
        for key, value in pairs:
            require(key not in result, "Duplicate JSON key")
            result[key] = value
        return result

    value = json.loads(data, object_pairs_hook=unique)
    encoded(value)  # Reject nonfinite numbers even in fields not used by this version.
    return value


def text(value):
    return isinstance(value, str) and bool(value.strip()) and "\0" not in value


def digest(value, length=64):
    return isinstance(value, str) and re.fullmatch(f"[0-9a-f]{{{length}}}", value) is not None


def identifier(value):
    return isinstance(value, str) and re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9_.-]{0,127}", value) is not None


def timestamp(value):
    require(isinstance(value, str), "Missing timestamp")
    result = datetime.fromisoformat(value.replace("Z", "+00:00"))
    require(result.tzinfo is not None, "Timestamp requires timezone")
    return result


def empty_state():
    return {"schema_version": 1, "generation": 0, "base_revision": None, "attempts": {}}


def validate_store(files, previous):
    """Bound total inventory and added-content patch bytes before changing any ref/file."""
    require(type(files) is dict and len(files) <= LIMITS["files"], "Memory file count exceeded")
    total = added = 0
    for name, data in files.items():
        require(re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9_.-]*\.json", name) is not None,
                "Only flat JSON memory files are supported")
        require(isinstance(data, bytes) and len(data) <= LIMITS["file_bytes"], "Memory file size exceeded")
        decoded(data)
        total += len(data)
        old_lines = previous.get(name, b"").splitlines(keepends=True)
        new_lines = data.splitlines(keepends=True)
        for operation, _, _, start, end in difflib.SequenceMatcher(None, old_lines, new_lines, autojunk=False).get_opcodes():
            if operation in ("replace", "insert"):
                added += sum(len(line) + 1 for line in new_lines[start:end])
    require(total <= LIMITS["files"] * LIMITS["file_bytes"], "Total memory budget exceeded")
    require(added <= LIMITS["patch_bytes"], "Added-content patch budget exceeded")


def validate_note(note):
    fields = {"schema_version", "kind", "attempt_id", "access", "hypothesis_summary",
              "evidence_summary", "insight", "next_action", "evidence_sha256"}
    require(type(note) is dict and set(note) == fields and type(note["schema_version"]) is int
            and note["schema_version"] == 1 and note["kind"] == "investigation-note"
            and note["access"] == "public" and identifier(note["attempt_id"]), "Invalid public note proposal")
    for key in ("hypothesis_summary", "evidence_summary", "insight", "next_action"):
        require(text(note[key]) and len(note[key]) <= 2048
                and c.sanitize(note[key]) == note[key], "Invalid or credential-bearing note text")
    require(type(note["evidence_sha256"]) is list and all(digest(h) for h in note["evidence_sha256"]),
            "Invalid note evidence identities")


def validate_artifact(e):
    require(type(e) is dict and digest(e["sha256"]) and type(e["size_bytes"]) is int
            and e["size_bytes"] >= 0 and text(e["type"]) and text(e["artifact_id"])
            and text(e["location"]), "Invalid evidence identity")
    require(e["access"] in ("public", "private", "restricted")
            and e["availability"] in ("available", "pending", "unavailable", "unknown", "removed_incomplete")
            and e["role"] in ("diagnostic", "acceptance", "checkpoint"), "Invalid evidence classification")
    timestamp(e["collected_at"])
    if e["expires_at"] is not None:
        require(timestamp(e["expires_at"]) >= timestamp(e["collected_at"]), "Expiry precedes collection")
    require(digest(e["capture_sha256"]) and digest(e["reducer_sha256"]), "Missing evidence provenance")
    require(e["reanalyzes"] is None or digest(e["reanalyzes"]), "Invalid original evidence reference")
    require(type(e["metrics"]) is list, "Invalid metrics")
    for metric in e["metrics"]:
        require(set(metric) == {"name", "unit", "aggregation", "value"}
                and all(text(metric[k]) for k in ("name", "unit", "aggregation")), "Missing metric semantics")
        c.number(metric["value"], "evidence metric")
    require(e.get("schema_version", 1) in (1, 2), "Unsupported evidence schema")
    if e.get("schema_version") == 2:
        require(e["type"] in ("summary", "profile-trace", "dump") and digest(e["source_sha256"])
                and timestamp(e["expires_at"]) - timestamp(e["collected_at"]) == timedelta(days=14),
                "Invalid retained evidence kind or expiry")
        provenance = e["provenance"]
        require(set(provenance) == {"run_id", "fingerprint"} and identifier(provenance["run_id"])
                and digest(provenance["fingerprint"]), "Missing retained evidence provenance")
        require(type(e["transport_size_bytes"]) is int and e["transport_size_bytes"] >= 0,
                "Invalid transport size")
        if e["availability"] in ("pending", "available"):
            require(digest(e["transport_sha256"]) and e["transport_size_bytes"] > 0,
                    "Missing ciphertext/summary transport identity")
            if e["type"] == "summary":
                require(e["encryption"] is None and e["sha256"] == e["transport_sha256"]
                        and e["size_bytes"] == e["transport_size_bytes"] and e["access"] == "public",
                        "Invalid compact transport")
            else:
                enc = e["encryption"]
                require(type(enc) is dict and set(enc) == {"method", "version", "recipient_sha256", "tool_sha256"}
                        and enc["method"] == "age-X25519" and enc["version"] == "v1.2.1"
                        and digest(enc["recipient_sha256"]) and digest(enc["tool_sha256"])
                        and e["access"] == "restricted" and e["transport_size_bytes"] > e["size_bytes"],
                        "Plaintext binary transport is forbidden")
        else:
            require(e["transport_sha256"] is None and e["transport_size_bytes"] == 0
                    and e["encryption"] is None, "Unavailable evidence has a transport")
        receipt = e["acknowledgment"]
        if e["availability"] == "available":
            require(type(receipt) is dict and receipt.get("scope") == "actions"
                    and receipt.get("acknowledged") is True and digest(receipt.get("manifest_sha256"))
                    and identifier(receipt.get("artifact_id")) and text(receipt.get("location"))
                    and receipt.get("retention_days") == 14
                    and artifact_retention(receipt, e["collected_at"])
                    and timestamp(receipt["created_at"]) < timestamp(e["expires_at"])
                    and digest(receipt.get("validation")),
                    "Remote evidence requires a matching trusted sink acknowledgment")
        else:
            require(receipt is None, "Pending evidence cannot claim acknowledgment")


def artifact_retention(receipt, collected_at):
    """The client chooses expiry before server creation; REST may round to seconds."""
    collected = timestamp(collected_at)
    created, expires = timestamp(receipt.get("created_at")), timestamp(receipt.get("expires_at"))
    if "retention_started_at" not in receipt:
        return collected <= created and expires - created == timedelta(days=14)
    started = timestamp(receipt["retention_started_at"])
    return (collected <= started and started.replace(microsecond=0) <= created
            and (started + timedelta(days=14)).replace(microsecond=0) <= expires
            <= created + timedelta(days=14))


def artifact_validation(record):
    payload = dict(record, acknowledgment={k: v for k, v in record["acknowledgment"].items() if k != "validation"})
    return hmac.digest(_VALIDATOR_KEY, encoded(payload), "sha256").hex()


def evidence_extends(previous, current):
    """Availability may advance after acknowledgment; identity and conclusions cannot change."""
    for new in current:
        if new not in previous and new.get("schema_version") == 2 and new["availability"] == "available":
            require(hmac.compare_digest(new["acknowledgment"]["validation"], artifact_validation(new)),
                    "Fresh trusted artifact acknowledgment required")
    for old in previous:
        if old in current:
            continue
        require(old.get("schema_version") == 2 and old["availability"] == "pending" and any(
            new["availability"] == "available"
            and {k: v for k, v in new.items() if k not in ("availability", "acknowledgment")} ==
                {k: v for k, v in old.items() if k not in ("availability", "acknowledgment")}
            for new in current), "Historical evidence cannot be erased")


def artifact_availability(evidence, now):
    """Identity is durable; a recorded URL/hash is not a download guarantee. Never fetch it."""
    validate_artifact(evidence)
    if evidence["expires_at"] is not None and timestamp(evidence["expires_at"]) <= now:
        return {"available": False, "reason": "expired"}
    if evidence.get("schema_version", 1) == 1 and evidence.get("reason") == "sanitized_summary":
        return {"available": False, "reason": "legacy_local_only"}
    available = evidence["availability"] == "available"
    return {"available": available, "reason": "recorded_available" if available else evidence["availability"]}


def profile_metrics(evidence):
    """Consume the actual reducer, not the obsolete timestamp-weighted CSV summary."""
    summary = c.normalize_evidence(evidence)
    require(summary["status"] == "collected", summary.get("reason", "Incomplete profiling evidence"))
    values = (summary["allocation_counter_observed_bytes"], summary["metrics"]["allocated_bytes_estimate"])
    return [dict(zip(("name", "unit", "aggregation", "value"), (*metric, value)))
            for metric, value in zip(PROFILE_METRICS, values)]


def measurement_record(baseline, candidate):
    """Called by the trusted verifier on validated raw samples, never on agent prose."""
    comparison = m.compare_measurements(baseline, candidate)
    invariants = baseline["invariants"]
    fingerprints = {key: m.fingerprint_inputs(invariants[key])
                    for key in ("workload", "bootstrap", "toolchain", "policy")}
    fingerprints.update(compiler=baseline["compiler_id"], candidate=candidate["compiler_id"],
                        runner=m.fingerprint_inputs(invariants["environment"]),
                        sdk=m.fingerprint_inputs(invariants["toolchain"]["sdk"]),
                        runtime=m.fingerprint_inputs(invariants["toolchain"]["runtime"]))
    return {"kind": "unprofiled-comparison", "comparison": comparison, "fingerprints": fingerprints,
            "capture_sha256": invariants["toolchain"]["measurement"]["sha256"],
            "reducer_sha256": m.file_record(m.__file__)["sha256"],
            "policy": copy.deepcopy(invariants["policy"]),
            "patch_sha256": candidate["compiler"]["patch_sha256"],
            "raw_sha256": [m.fingerprint_inputs(d) for d in (baseline, candidate)],
            "samples": [[{k: s[k] for k in ("sample_id", "phase", "role", *m.METRICS)} for s in d["samples"]]
                        for d in (baseline, candidate)],
            "metrics": copy.deepcopy(ACCEPTANCE_METRICS)}


def _validation_payload(a):
    checks = dict(a["correctness"], validation=None)
    return encoded([a["id"], a["source_revision"], a["fingerprints"], a["summary"],
                    a["measurement"], a["collection"], checks])


def _attest_correctness(a):
    """Trusted in-process validator only; serialized proposals cannot mint approval."""
    from experiment import validate_correctness
    validate_correctness(a["correctness"], a["fingerprints"]["candidate"], a["fingerprints"]["policy"], current=True)
    a["correctness"]["validation"] = hmac.digest(_VALIDATOR_KEY, _validation_payload(a), "sha256").hex()


def acceptance_ready(a, *, current=True):
    receipt = a["measurement"]
    require(type(receipt) is dict and receipt["kind"] == "unprofiled-comparison",
            "Unprofiled acceptance evidence required")
    require(a["collection"]["status"] == "collected", "Required diagnostic collection did not complete")
    if current:
        current_hash = m.file_record(m.__file__)["sha256"]
        require(receipt["capture_sha256"] == receipt["reducer_sha256"] == current_hash,
                "Acceptance capture/reducer identity changed")
    policy = receipt["policy"]
    require((not current or policy == m.POLICY)
            and receipt["fingerprints"]["policy"] == m.fingerprint_inputs(policy),
            "Measurement policy changed")
    require(receipt["metrics"] == ACCEPTANCE_METRICS and receipt["patch_sha256"] == a["summary"]["patch_sha256"],
            "Acceptance metric semantics or candidate patch differ")
    require(set(receipt["fingerprints"]) == {"workload", "bootstrap", "toolchain", "policy", "compiler",
                                           "candidate", "runner", "sdk", "runtime"}
            and all(a["fingerprints"].get(k) == v for k, v in receipt["fingerprints"].items()),
            "Acceptance identities differ")
    comparison = receipt["comparison"]
    require(comparison["decision"] == "measured_improvement" and not comparison["reasons"]
            and not comparison["invariant_mismatches"], "Measurement gate did not pass")
    require(comparison["primary"]["improvement_percent"] >= policy["minimum_improvement_percent"]
            and comparison["primary"]["interval_95_percent"][0] > 0, "Acceptance effect/interval did not pass")
    checks = a["correctness"]
    if checks.get("schema_version") in (2, 3, 4, 5):
        from experiment import validate_correctness
        validate_correctness(checks, a["fingerprints"]["candidate"], a["fingerprints"]["policy"], current=current)
        if current:
            require(isinstance(checks["validation"], str) and
                    hmac.compare_digest(checks["validation"],
                                        hmac.digest(_VALIDATOR_KEY, _validation_payload(a), "sha256").hex()),
                    "A fresh trusted validator is required; persisted approval is historical")
        return
    require(not current, "Historical correctness requires a fresh trusted validator")
    required, results = checks["required"], checks["results"]
    require(["dotnet", "test", "-c", "Release"] in required and all(any(r["command"] == command and type(r["exit_status"]) is int
                                and r["exit_status"] == 0 and r["candidate"] == a["fingerprints"]["candidate"]
                                and r["policy"] == a["fingerprints"]["policy"] for r in results)
                             for command in required), "Required correctness checks did not pass")


def validate_retry(a, *, recorded=True):
    dependencies = a["dependencies"]
    require(type(dependencies) is dict and set(dependencies) == {"paths", "workload", "sdk", "collector_permission"}
            and type(dependencies["paths"]) is dict
            and (dependencies["workload"] is None or digest(dependencies["workload"]))
            and (dependencies["sdk"] is None or text(dependencies["sdk"]))
            and (dependencies["collector_permission"] is None or type(dependencies["collector_permission"]) is bool),
            "Invalid retry dependencies")
    for path, value in dependencies["paths"].items():
        require(text(path) and "\\" not in path and not path.startswith("/")
                and not any(p in ("", ".", "..") for p in path.split("/")) and ":" not in path
                and digest(value), "Invalid relative dependency path/hash")
    retry = a["retry"]
    require(type(retry) is dict and set(retry) == {"when_any", "reason"}
            and type(retry["when_any"]) is list and text(retry["reason"]), "Invalid retry policy")
    for condition in retry["when_any"]:
        kind = condition["kind"]
        require(kind in ("path", "workload", "sdk", "collector_permission")
                and set(condition) == ({"kind", "path", "value"} if kind == "path" else {"kind", "value"}),
                "Retry predicates must be allowlisted data")
        value = dependencies["paths"].get(condition["path"]) if kind == "path" else dependencies[kind]
        expected = condition["value"]
        require((digest(expected) if kind in ("path", "workload") else
                 text(expected) if kind == "sdk" else type(expected) is bool)
                and (not recorded or (type(value) is type(expected) and value == expected)),
                "Retry predicate differs from recorded dependency")


def validate_attempt(a, *, prospective=False):
    required = {"id", "hypothesis", "run", "source_revision", "lifecycle", "outcome", "reason", "fingerprints",
                "dependencies", "retry", "summary", "evidence", "measurement", "collection", "correctness",
                "publication", "supersedes", "notes", "lease", "checkpoint"}
    require(type(a) is dict and set(a) == required and identifier(a["id"])
            and digest(a["source_revision"], 40), "Invalid attempt schema/source revision")
    require(a["lifecycle"] in LIFECYCLES and (a["outcome"] is None or a["outcome"] in OUTCOMES),
            "Invalid lifecycle/outcome")
    require(isinstance(a["reason"], str) and (a["outcome"] is None or text(a["reason"])),
            "Terminal decisions require a reason")
    h, run, fp = a["hypothesis"], a["run"], a["fingerprints"]
    require(set(h) == {"id", "description", "area", "operation"} and identifier(h["id"])
            and all(text(h[k]) for k in h), "Invalid hypothesis identity")
    require(set(run) == {"id", "attempt", "url"} and identifier(run["id"])
            and type(run["attempt"]) is int and run["attempt"] > 0
            and run["url"] == f"https://github.com/{REPOSITORY}/actions/runs/{run['id']}", "Invalid run owner")
    require(type(fp) is dict and {"source", "input"} <= fp.keys() and set(fp) <= set(FINGERPRINTS)
            and all(digest(value) for value in fp.values()), "Missing frozen source/input fingerprint")
    validate_retry(a, recorded=not prospective)
    require(set(a["summary"]) == {"evidence", "change", "patch_sha256"}
            and all(isinstance(a["summary"][k], str) for k in ("evidence", "change"))
            and digest(a["summary"]["patch_sha256"]), "Invalid experiment summary")
    require(type(a["evidence"]) is list and type(a["notes"]) is list, "Invalid evidence/notes")
    collection = a["collection"]
    require(type(collection) is dict and set(collection) == {"status", "reason"}
            and collection["status"] in ("pending", "collecting", "collected", "blocked_collection")
            and isinstance(collection["reason"], str)
            and (collection["status"] != "blocked_collection" or text(collection["reason"])),
            "Invalid collection status/reason")
    receipt = a["measurement"]
    if receipt is not None:
        require(type(receipt) is dict and receipt["kind"] == "unprofiled-comparison"
                and all(digest(receipt[k]) for k in ("capture_sha256", "reducer_sha256", "patch_sha256"))
                and type(receipt["fingerprints"]) is dict
                and all(k in FINGERPRINTS and digest(v) for k, v in receipt["fingerprints"].items())
                and type(receipt["raw_sha256"]) is list and len(receipt["raw_sha256"]) == 2
                and all(digest(h) for h in receipt["raw_sha256"])
                and type(receipt["samples"]) is list and len(receipt["samples"]) == 2
                and type(receipt["metrics"]) is list and type(receipt["policy"]) is dict
                and receipt["comparison"]["decision"] in ("inconclusive", "rejected", "measured_improvement"),
                "Invalid measurement receipt")
    for e in a["evidence"]:
        validate_artifact(e)
        require(e["reanalyzes"] is None or (e["reanalyzes"] != e["sha256"]
                and any(old["sha256"] == e["reanalyzes"] for old in a["evidence"])),
                "Reanalysis must retain its original evidence identity")
    for note in a["notes"]:
        validate_note(note)
        require(note["attempt_id"] == a["id"], "Note belongs to another attempt")
    require(a["supersedes"] is None or identifier(a["supersedes"]), "Invalid superseded attempt")
    checks = a["correctness"]
    if "schema_version" in checks:
        from experiment import validate_correctness
        require(type(checks["schema_version"]) is int and checks["schema_version"] in (2, 3, 4, 5),
                "Unsupported correctness schema")
        validate_correctness(checks, fp.get("candidate"), fp.get("policy"),
                             passed=a["outcome"] == "measured_improvement" or a["publication"] is not None)
    else:
        require(set(checks) == {"required", "results"} and type(checks["required"]) is list
                and ["dotnet", "test", "-c", "Release"] in checks["required"] and type(checks["results"]) is list,
                "Invalid historical correctness record")
        for command in checks["required"] + [r["command"] for r in checks["results"]]:
            require(type(command) is list and command and all(text(arg) for arg in command),
                    "Correctness commands must be argument arrays (never executed here)")
        for r in checks["results"]:
            require(set(r) == {"command", "exit_status", "candidate", "policy"} and type(r["exit_status"]) is int
                    and digest(r["candidate"]) and digest(r["policy"]), "Invalid historical correctness result")
    lease = a["lease"]
    require(set(lease) == {"run_id", "run_attempt", "checkpoint_at", "expires_at"}
            and lease["run_id"] == run["id"] and lease["run_attempt"] == run["attempt"],
            "Lease owner differs from run")
    require(timestamp(lease["checkpoint_at"]) <= timestamp(lease["expires_at"]), "Invalid lease interval")
    checkpoint = a["checkpoint"]
    require(set(checkpoint) == {"next_action", "required_evidence", "candidate_ref"}
            and text(checkpoint["next_action"]) and isinstance(checkpoint["candidate_ref"], str)
            and type(checkpoint["required_evidence"]) is list
            and all(digest(h) for h in checkpoint["required_evidence"]), "Invalid checkpoint")
    publication = a["publication"]
    if publication is not None:
        require(a["outcome"] == "measured_improvement"
                and set(publication) == {"intent_id", "marker", "attempt_id", "head_sha", "pr", "blocker",
                                     "notified_blocker_sha256"}
                and publication["marker"] == WORKFLOW_MARKER and publication["attempt_id"] == a["id"]
                and digest(publication["head_sha"], 40) and digest(publication["intent_id"])
                and isinstance(publication["blocker"], str)
                and (publication["notified_blocker_sha256"] is None or digest(publication["notified_blocker_sha256"])),
                "Invalid publication intent")
        require(publication["intent_id"] == m.fingerprint_inputs([a["id"], publication["head_sha"], WORKFLOW_MARKER]),
                "Publication intent identity differs")
        if publication["pr"] is not None:
            validate_pr(publication["pr"])
    require(a["lifecycle"] not in ("publication_pending", "published") or publication is not None,
            "Publication lifecycle requires intent")
    require(a["lifecycle"] != "published" or publication["pr"] is not None, "Publication requires acknowledgment")
    if a["outcome"] == "measured_improvement" or publication is not None:
        acceptance_ready(a, current=False)


def validate_discovery(record):
    require(type(record) is dict and set(record) == {
        "id", "run", "source_revision", "lifecycle", "outcome", "reason", "checkpoint_at", "expires_at"} |
        ({"evidence"} if "evidence" in record else set())
        and identifier(record["id"]) and digest(record["source_revision"], 40), "Invalid discovery schema/source revision")
    run = record["run"]
    require(type(run) is dict and set(run) == {"id", "attempt", "url"} and identifier(run["id"])
            and type(run["attempt"]) is int and run["attempt"] > 0
            and run["url"] == f"https://github.com/{REPOSITORY}/actions/runs/{run['id']}", "Invalid discovery run owner")
    require(record["lifecycle"] in DISCOVERY_LIFECYCLES
            and (record["outcome"] is None or record["outcome"] in DISCOVERY_OUTCOMES)
            and isinstance(record["reason"], str) and (record["outcome"] is None or text(record["reason"])),
            "Invalid discovery lifecycle/outcome/reason")
    require(timestamp(record["checkpoint_at"]) <= timestamp(record["expires_at"]), "Invalid discovery interval")
    if "evidence" in record:
        require(type(record["evidence"]) is list, "Invalid discovery evidence")
        for artifact in record["evidence"]:
            validate_artifact(artifact)
            require(artifact.get("schema_version") == 2
                    and artifact["provenance"]["run_id"] == record["run"]["id"], "Discovery evidence owner differs")


def validate_state(value):
    require(type(value) is dict and type(value.get("schema_version")) is int
            and value["schema_version"] in (1, 2, 3)
            and set(value) == {"schema_version", "generation", "base_revision", "attempts"} |
            ({"discoveries"} if value["schema_version"] >= 2 else set()),
            "Unsupported journal schema")
    require(type(value["generation"]) is int and value["generation"] >= 0
            and (value["base_revision"] is None or digest(value["base_revision"], 40))
            and type(value["attempts"]) is dict, "Invalid generation/base revision")
    for key, a in value["attempts"].items():
        require(key == a["id"], "Attempt index differs from stable ID")
        validate_attempt(a)
        if a["supersedes"] is not None:
            require(a["supersedes"] in value["attempts"] and a["supersedes"] != key,
                    "Missing superseded attempt")
            require(same_hypothesis(a, value["attempts"][a["supersedes"]]), "Retry refers to another hypothesis")
        seen, current = {key}, a
        while current["supersedes"] is not None:
            previous = current["supersedes"]
            require(previous not in seen and previous in value["attempts"], "Cycle or missing retry history")
            seen.add(previous)
            current = value["attempts"][previous]
    active = [a for a in value["attempts"].values() if a["outcome"] is None or a["lifecycle"] == "publication_pending"]
    require(len(active) <= 1, "Another canonical reservation owns active work")
    discoveries = value.get("discoveries", {})
    require(type(discoveries) is dict, "Invalid discovery index")
    owners = set()
    for key, record in discoveries.items():
        validate_discovery(record)
        require("evidence" not in record or value["schema_version"] >= 3, "Discovery evidence requires schema 3")
        require(key == record["id"], "Discovery index differs from stable ID")
        owner = (record["run"]["id"], record["run"]["attempt"])
        require(owner not in owners, "Discovery run already has a reservation")
        owners.add(owner)
    discovering = [d for d in discoveries.values() if d["outcome"] is None]
    require(len(discovering) <= 1, "Another canonical discovery owns active work")
    if discovering and active:
        require(active[0]["run"] == discovering[0]["run"]
                and active[0]["source_revision"] == discovering[0]["source_revision"],
                "Active attempt belongs to another discovery owner")
    validate_store({"state.json": encoded(value)}, {})


def load_state(path=None, *, response=None):
    """404 must be verified by the trusted reader; all other failures retain the original."""
    try:
        if response is not None:
            if response.get("status") == 404 and response.get("verified_missing") is True:
                return {"status": "initialized", "state": empty_state(), "reason": "verified_missing"}
            require(response.get("status") == 200, f"Journal read failed: {response.get('status')}")
            data = response["body"]
            require(len(data) <= LIMITS["file_bytes"], "Journal size exceeded")
            value = decoded(data)
        else:
            with Path(path).open("rb") as stream:
                data = stream.read(LIMITS["file_bytes"] + 1)
            require(len(data) <= LIMITS["file_bytes"], "Journal size exceeded")
            value = decoded(data)
        validate_state(value)
        return {"status": "ready", "state": value, "reason": ""}
    except (OSError, ValueError, TypeError, KeyError, AttributeError) as error:
        return {"status": "blocked_setup", "state": None, "reason": str(error)}


def same_hypothesis(a, b):
    """Ignore ordering only for operations encoding the complete compiler-path dependency scope."""
    if a["hypothesis"]["id"] == b["hypothesis"]["id"]:
        return True
    if a["hypothesis"]["area"] != b["hypothesis"]["area"]:
        return False
    operations = [record["hypothesis"]["operation"] for record in (a, b)]
    if operations[0] == operations[1]:
        return True
    paths = [sorted(path for path in record["dependencies"]["paths"] if path.startswith("src/Compiler/"))
             for record in (a, b)]
    return paths[0] == paths[1] and all(sorted(operation.split(";")) == recorded
                                      for operation, recorded in zip(operations, paths))


def hypothesis_heads(state, attempt):
    history = [a for a in state["attempts"].values() if same_hypothesis(a, attempt)]
    superseded = {a["supersedes"] for a in history}
    # Normalization can join independent legacy branches; retain every branch's policy.
    return sorted((a for a in history if a["id"] not in superseded), key=lambda a: a["id"])


def retry_decision(previous, current):
    validate_retry(previous)
    validate_retry(current, recorded=False)
    result = {"allowed": False, "reason": "No recorded invalidation changed", "supersedes": previous["id"]}
    if previous["outcome"] is None or previous["lifecycle"] == "publication_pending":
        return dict(result, reason="Active work cannot be bypassed")
    if previous["outcome"] == "measured_improvement":
        return dict(result, reason="Successful work is not a rejected hypothesis")
    changed = []
    for condition in previous["retry"]["when_any"]:
        kind = condition["kind"]
        value = (current["dependencies"]["paths"].get(condition["path"]) if kind == "path"
                 else current["dependencies"][kind])
        if value is None:
            return dict(result, reason="Relevant dependency is unresolved")
        if value != condition["value"]:
            changed.append(kind)
    if changed:
        return dict(result, allowed=True, reason="Recorded invalidation changed: " + ", ".join(changed))
    return result


def merge_attempt(state, attempt, base_attempt=None):
    """Three-way whole-attempt merge: unrelated attempts merge, concurrent edits conflict."""
    validate_state(state)
    validate_attempt(attempt)
    old = state["attempts"].get(attempt["id"])
    if old == attempt:
        return copy.deepcopy(state)
    evidence_extends(old["evidence"] if old else [], attempt["evidence"])
    if old != base_attempt:
        raise Conflict("Same-attempt update conflicts with canonical state")
    new_approval = (old is None or old["outcome"] != attempt["outcome"]
                    or (old["publication"] is None and attempt["publication"] is not None))
    if new_approval and (attempt["outcome"] == "measured_improvement" or attempt["publication"] is not None):
        acceptance_ready(attempt)
    if old is None:
        history = hypothesis_heads(state, attempt)
        if history:
            require(attempt["supersedes"] == history[0]["id"]
                    and all(retry_decision(a, attempt)["allowed"] for a in history),
                    "New attempt bypasses recorded retry policy")
    if old is not None:
        if old["outcome"] is not None and old["outcome"] != attempt["outcome"]:
            raise Conflict("Terminal outcome cannot be replaced; create a linked retry")
        for key in ("hypothesis", "run", "source_revision", "supersedes"):
            require(old[key] == attempt[key], f"Attempt {key} is immutable")
        require(all(attempt["dependencies"]["paths"].get(k) == v for k, v in old["dependencies"]["paths"].items())
                and all(v is None or attempt["dependencies"][k] == v
                        for k, v in old["dependencies"].items() if k != "paths"),
                "Resolved retry dependencies are immutable")
        require(all(p in attempt["retry"]["when_any"] for p in old["retry"]["when_any"])
                and old["retry"]["reason"] == attempt["retry"]["reason"], "Retry policy cannot be erased")
        require(attempt["lifecycle"] == "interrupted" or old["lifecycle"] == "interrupted"
                or LIFECYCLES.index(attempt["lifecycle"]) >= LIFECYCLES.index(old["lifecycle"]),
                "Lifecycle cannot move backwards")
        require(all(attempt["fingerprints"].get(k) == v for k, v in old["fingerprints"].items()),
                "Frozen identities cannot be replaced")
        require(all(n in attempt["notes"] for n in old["notes"]), "Historical notes cannot be erased")
        require(attempt["correctness"]["required"] == old["correctness"]["required"],
                "Correctness requirements cannot be weakened")
        results = attempt["correctness"]["results"]
        if not all(r in results for r in old["correctness"]["results"]):
            # A protected controller may renew process-local authority after restart,
            # but not change any captured command, identity, outcome or evidence.
            def captured(result):
                return {k: v for k, v in result.items() if k != "execution"}
            require(all(any(captured(r) == captured(new) for new in results)
                        for r in old["correctness"]["results"]), "Correctness results cannot be erased")
            acceptance_ready(attempt)
        if old["outcome"] is not None:
            require(all(old[k] == attempt[k] for k in ("reason", "summary", "measurement", "collection")),
                    "Terminal conclusion cannot be rewritten")
        if old["publication"] is not None:
            require(attempt["publication"] is not None and all(
                old["publication"][k] == attempt["publication"][k] for k in ("intent_id", "head_sha")),
                "Publication intent cannot be replaced")
    result = copy.deepcopy(state)
    result["attempts"][attempt["id"]] = copy.deepcopy(attempt)
    result["generation"] += 1
    validate_state(result)
    return result


def merge_discovery(state, record, base_record=None):
    """Three-way discovery merge; completing work never rewrites its identity or deadline."""
    validate_state(state)
    validate_discovery(record)
    if base_record is not None:
        validate_discovery(base_record)
    old = state.get("discoveries", {}).get(record["id"])
    if old == record:
        return copy.deepcopy(state)
    evidence_extends(old.get("evidence", []) if old else [], record.get("evidence", []))
    if old != base_record:
        raise Conflict("Same-discovery update conflicts with canonical state")
    if old is not None:
        require(old["outcome"] is None or all(old[k] == record[k] for k in old if k != "evidence"),
                "Completed discovery cannot be rewritten")
        for key in ("run", "source_revision", "expires_at"):
            require(old[key] == record[key], f"Discovery {key} is immutable")
        require(timestamp(record["checkpoint_at"]) >= timestamp(old["checkpoint_at"]),
                "Discovery checkpoint cannot move backwards")
    result = copy.deepcopy(state)
    result["schema_version"] = max(state["schema_version"], 3 if "evidence" in record else 2)
    result.setdefault("discoveries", {})[record["id"]] = copy.deepcopy(record)
    result["generation"] += 1
    validate_state(result)
    return result


def validate_pr(pr):
    require(type(pr) is dict and type(pr["number"]) is int and pr["number"] > 0
            and pr["state"] in ("open", "closed") and type(pr["draft"]) is bool
            and all(text(pr[k]) for k in ("base_repository", "base_ref", "head_ref"))
            and (pr["head_repository"] is None or text(pr["head_repository"]))
            and digest(pr["head_sha"], 40) and type(pr["labels"]) is list
            and all(text(label) for label in pr["labels"])
            and pr["url"] == f"https://github.com/{pr['base_repository']}/pull/{pr['number']}",
            "Invalid structured PR metadata")
    require(pr["marker"] is None or pr["marker"] == WORKFLOW_MARKER, "Invalid workflow marker")
    require(pr["attempt_id"] is None or identifier(pr["attempt_id"]), "Invalid PR attempt marker")


def query_prs(fetch_page):
    result, seen, page = [], set(), 1
    while True:
        response = fetch_page(page)
        require(type(response) is dict and response.get("status") == 200 and response.get("complete") is True
                and type(response.get("items")) is list and "next" in response, "PR query failed or truncated")
        for pr in response["items"]:
            validate_pr(pr)
            require(pr["number"] not in seen, "Duplicate PR across query pages")
            seen.add(pr["number"])
            result.append(pr)
        next_page = response["next"]
        if next_page is None:
            return result
        require(type(next_page) is int and next_page == page + 1, "Invalid PR pagination")
        page = next_page


def workflow_prs(prs, state=None):
    known = {a["publication"]["pr"]["number"] for a in (state or empty_state())["attempts"].values()
             if a["publication"] is not None and a["publication"]["pr"] is not None}
    return [p for p in prs if p["state"] == "open" and p["base_repository"] == REPOSITORY
            and (p["number"] in known or p["marker"] == WORKFLOW_MARKER or WORKFLOW_MARKER in p["labels"]
                 or (p["head_repository"] == REPOSITORY and p["head_ref"].startswith(WORKFLOW_MARKER + "/")))]


def resume_reason(previous, proposed, now):
    if (set(previous["fingerprints"]) != set(FINGERPRINTS) or previous["fingerprints"] != proposed["fingerprints"]
            or previous["source_revision"] != proposed["source_revision"]):
        return "Frozen restart inputs differ or remain unresolved"
    if previous["dependencies"] != proposed["dependencies"]:
        return "Restart dependencies differ"
    for identity in previous["checkpoint"]["required_evidence"]:
        matches = [e for e in previous["evidence"] if e["sha256"] == identity]
        if not matches or not all(artifact_availability(e, now)["available"] for e in matches):
            return "Required checkpoint evidence is unavailable"
        for e in matches:
            if (e["capture_sha256"] != proposed["fingerprints"]["capture"]
                    or e["reducer_sha256"] != proposed["fingerprints"]["reducer"]
                    or any(tuple(metric[k] for k in ("name", "unit", "aggregation")) not in PROFILE_METRICS
                           for metric in e["metrics"] if e["role"] == "diagnostic")
                    or any({k: metric[k] for k in ("name", "unit", "aggregation")} not in ACCEPTANCE_METRICS
                           for metric in e["metrics"] if e["role"] == "acceptance")):
                return "Required checkpoint evidence is incompatible"
    return None


def discovery_preflight(state, proposed, fetch_pr_page, fetch_run, now):
    """Reserve discovery ownership before setup, without inventing a hypothesis."""
    validate_state(state)
    validate_discovery(proposed)
    result = {"action": "blocked", "outcome": "blocked_setup", "reason": "", "durable": False,
              "state": copy.deepcopy(state)}
    try:
        require(proposed["outcome"] is None and proposed["lifecycle"] == "reserved",
                "New discovery requires a reservation")
        if workflow_prs(query_prs(fetch_pr_page), state):
            return dict(result, reason="Outstanding workflow-owned experiment PR")
        if any(a["lifecycle"] == "publication_pending" for a in state["attempts"].values()):
            return dict(result, reason="Publication must be reconciled before new work")
        discoveries = list(state.get("discoveries", {}).values())
        attempts = [a for a in state["attempts"].values() if a["outcome"] is None]
        if any(d["run"] == proposed["run"] for d in discoveries + attempts):
            return dict(result, reason="Run already owns a discovery or attempt; reentry is not a reservation")
        active = [d for d in discoveries if d["outcome"] is None]
        completed = set()
        for owner in attempts + active:
            identity = (owner["run"]["id"], owner["run"]["attempt"])
            if identity in completed:
                continue
            run = fetch_run(owner["run"])
            require(run.get("status") == 200 and run.get("id") == identity[0]
                    and run.get("attempt") == identity[1], "Actions owner query failed")
            if run.get("state") != "completed":
                return dict(result, reason="An Actions run still owns active work")
            completed.add(identity)
        working = copy.deepcopy(state)
        for records, merge in ((attempts, merge_attempt), (active, merge_discovery)):
            for owner in records:
                cancelled = dict(owner, lifecycle="interrupted", outcome="cancelled",
                                 reason="Actions owner completed without final journal")
                working = merge(working, cancelled, owner)
        working = merge_discovery(working, proposed)
        return dict(result, action="reserve", outcome=None, reason="Reservation requires canonical commit", state=working)
    except (OSError, ValueError, KeyError, TypeError, IndexError, StopIteration, subprocess.SubprocessError) as error:
        return dict(result, reason=str(error))


def preflight(state, request, fetch_pr_page, fetch_run, now, *, discovery_id=None):
    """Pure decision and candidate state. No expensive work until a CAS reservation is acknowledged."""
    validate_state(state)
    require(set(request) == {"attempt", "repository", "default_branch", "source_ref", "source_revision"}
            and request["repository"] == REPOSITORY and request["source_ref"] == request["default_branch"]
            and text(request["default_branch"]) and not request["source_ref"].startswith("refs/pull/")
            and digest(request["source_revision"], 40), "Only trusted default-branch source may be investigated")
    proposed = copy.deepcopy(request["attempt"])
    validate_attempt(proposed, prospective=True)
    result = {"action": "blocked", "outcome": "blocked_setup", "reason": "", "durable": False,
              "state": copy.deepcopy(state)}
    try:
        require(proposed["source_revision"] == request["source_revision"], "Reservation source differs from verified default branch")
        prs = query_prs(fetch_pr_page)
        if workflow_prs(prs, state):
            for a in state["attempts"].values():
                if a["lifecycle"] == "publication_pending":
                    recovered = reconcile_publication(a, lambda _: {
                        "status": 200, "items": prs, "next": None, "complete": True}, now)
                    if recovered["action"] == "acknowledge":
                        result["state"] = merge_attempt(state, recovered["attempt"], a)
            return dict(result, reason="Outstanding workflow-owned experiment PR")
        discoveries = state.get("discoveries", {})
        if discovery_id is not None:
            owner = discoveries.get(discovery_id)
            require(owner is not None and owner["outcome"] is None
                    and owner["run"] == proposed["run"] and owner["source_revision"] == proposed["source_revision"],
                    "Selection requires the matching active discovery owner and source")
        if any(d["outcome"] is None and d["id"] != discovery_id for d in discoveries.values()):
            return dict(result, reason="A discovery still owns active work")
        same = state["attempts"].get(proposed["id"])
        if same is not None and same["outcome"] is None:
            if same["run"] != proposed["run"]:
                return dict(result, reason="Attempt belongs to another run")
            reason = resume_reason(same, proposed, now)
            return dict(result, action="blocked" if reason else "resume", reason=reason or "Exact checkpoint",
                        outcome="blocked_setup" if reason else None)
        working = copy.deepcopy(state)
        for a in list(working["attempts"].values()):
            if a["lifecycle"] == "publication_pending":
                return dict(result, reason="Publication must be reconciled before new work")
            if a["outcome"] is None:
                run = fetch_run(a["run"])
                require(run.get("status") == 200 and run.get("id") == a["run"]["id"]
                        and run.get("attempt") == a["run"]["attempt"], "Actions owner query failed")
                if run.get("state") != "completed":
                    return dict(result, reason="An Actions run still owns active work")
                interrupted = copy.deepcopy(a)
                interrupted.update(lifecycle="interrupted", outcome="cancelled", reason="Actions owner completed without final journal")
                working = merge_attempt(working, interrupted, base_attempt=a)
        history = hypothesis_heads(working, proposed)
        for previous in history:
            retry = retry_decision(previous, proposed)
            if not retry["allowed"]:
                return dict(result, action="skip", outcome=previous["outcome"], reason=retry["reason"], state=working)
        if history:
            require(proposed["id"] not in working["attempts"], "Retry requires a new stable attempt ID")
            proposed["supersedes"] = history[0]["id"]
        require(proposed["outcome"] is None and proposed["lifecycle"] == "reserved"
                and proposed["publication"] is None, "New work requires a reservation")
        for condition in proposed["retry"]["when_any"]:
            kind = condition["kind"]
            condition["value"] = (proposed["dependencies"]["paths"][condition["path"]] if kind == "path"
                                  else proposed["dependencies"][kind])
        working = merge_attempt(working, proposed)
        return dict(result, action="reserve", outcome=None, reason="Reservation requires canonical commit", state=working)
    except (OSError, ValueError, KeyError, TypeError, IndexError, StopIteration, subprocess.SubprocessError) as error:
        return dict(result, reason=str(error))


def publication_intent(attempt, head_sha):
    require(attempt["outcome"] == "measured_improvement", "Publication requires the final investigation outcome")
    acceptance_ready(attempt)
    require(digest(head_sha, 40), "Invalid publication head")
    return {"intent_id": m.fingerprint_inputs([attempt["id"], head_sha, WORKFLOW_MARKER]),
            "marker": WORKFLOW_MARKER, "attempt_id": attempt["id"], "head_sha": head_sha,
            "pr": None, "blocker": "", "notified_blocker_sha256": None}


def reconcile_publication(attempt, fetch_pr_page, now, *, error=None):
    """Discover before doing anything new; never creates a PR/issue/comment."""
    a = copy.deepcopy(attempt)
    validate_attempt(a)
    intent = a["publication"]
    require(intent is not None, "Missing saved publication intent")
    try:
        outstanding = workflow_prs(query_prs(fetch_pr_page), empty_state())
        exact = [p for p in outstanding if p["marker"] == WORKFLOW_MARKER and p["attempt_id"] == a["id"]
                 and p["head_sha"] == intent["head_sha"] and p["head_repository"] == REPOSITORY]
        if len(exact) == 1 and len(outstanding) == 1:
            intent.update(pr=exact[0], blocker="")
            a["lifecycle"] = "published"
            return {"action": "acknowledge", "attempt": a, "notify": False}
        require(not outstanding, "Outstanding PR does not uniquely match publication intent")
        require(intent["pr"] is None, "Previously acknowledged PR needs closed-PR reconciliation")
        acceptance_ready(a)
        require(resume_reason(a, a, now) is None, "Publication checkpoint unavailable or incompatible")
        raise ValueError(error or "Saved intent has no remote result; preserve candidate and retry publication only")
    except (OSError, ValueError, KeyError, TypeError, subprocess.SubprocessError) as failure:
        reason = str(failure)
        blocker = m.fingerprint_inputs([intent["intent_id"], reason])
        notify = blocker != intent["notified_blocker_sha256"]
        intent["blocker"] = reason
        return {"action": "blocked", "attempt": a, "reason": reason,
                "blocker_sha256": blocker, "notify": notify}


def checkpoint(path, state):
    validate_state(state)
    path = Path(path)
    require(not path.is_symlink(), "Checkpoint must not be a symlink")
    data = encoded(state)
    previous = {path.name: path.read_bytes()} if path.exists() else {}
    validate_store({path.name: data}, previous)
    name = None
    try:
        with tempfile.NamedTemporaryFile(dir=path.parent, prefix=path.name + ".", delete=False) as stream:
            name = stream.name
            stream.write(data)
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(name, path)
        if os.name != "nt":
            descriptor = os.open(path.parent, os.O_RDONLY | os.O_DIRECTORY)
            try:
                os.fsync(descriptor)
            finally:
                os.close(descriptor)
    finally:
        if name is not None:
            Path(name).unlink(missing_ok=True)
    return {"storage": "local_checkpoint", "remote_acknowledged": False, "sha256": m.file_record(path)["sha256"]}


class GitJournal:
    """Fixed-branch local CAS writer for the guarded job. Never checks out or executes stored data."""

    def __init__(self, repository):
        self.repository = Path(repository).resolve(strict=True)
        self.location = (["-C", str(self.repository)] if (self.repository / ".git").exists()
                         else ["--git-dir", str(self.repository)])

    def git(self, *args, data=None, check=True):
        return subprocess.run(["git", *self.location, *args], input=data,
                              capture_output=True, check=check, timeout=30, env=c.tool_environment())

    def head(self, branch=STATE_BRANCH):
        refs = self.git("for-each-ref", "--format=%(refname) %(objectname)", "refs/heads/" + branch).stdout.decode().splitlines()
        matches = [line.split()[1] for line in refs if line.split()[0] == "refs/heads/" + branch]
        return matches[0] if matches else None

    def files(self, revision):
        require(digest(revision, 40), "Invalid journal revision")
        entries = self.git("ls-tree", "-z", revision).stdout.split(b"\0")
        require(len(entries) - 1 <= LIMITS["files"], "Journal file count exceeded")
        result = {}
        for entry in filter(None, entries):
            metadata, name = entry.split(b"\t", 1)
            mode, kind, oid = metadata.split()
            require(mode == b"100644" and kind == b"blob", "Only regular JSON journal files are supported")
            size = int(self.git("cat-file", "-s", oid.decode()).stdout)
            require(size <= LIMITS["file_bytes"], "Journal file size exceeded")
            result[name.decode("utf-8")] = self.git("cat-file", "blob", oid.decode()).stdout
        validate_store(result, result)
        return result

    def read(self):
        head = self.head()
        if head is None:
            return empty_state(), None
        files = self.files(head)
        if not files:
            return empty_state(), head
        require(set(files) == {"state.json"}, "Canonical branch must contain only state.json")
        value = decoded(files["state.json"])
        validate_state(value)
        return value, head

    def commit(self, files, parent):
        lines = []
        for name, data in sorted(files.items()):
            oid = self.git("hash-object", "-w", "--stdin", data=data).stdout.strip()
            lines.append(b"100644 blob " + oid + b"\t" + name.encode() + b"\n")
        tree = self.git("mktree", data=b"".join(lines)).stdout.decode().strip()
        return self.git("-c", "commit.gpgsign=false", "commit-tree", tree,
                        *(["-p", parent] if parent else []), "-m", "Record compiler performance investigation").stdout.decode().strip()

    def compare_and_swap(self, expected, commit):
        result = self.git("update-ref", "refs/heads/" + STATE_BRANCH, commit, expected or "0" * 40, check=False)
        if result.returncode:
            # A permission/IO error is not a CAS race.
            require(self.head() != expected, result.stderr.decode(errors="replace"))
            return False
        return True

    def write_state(self, base, expected_head, candidate):
        validate_state(base)
        validate_state(candidate)
        require(set(base["attempts"]) <= set(candidate["attempts"]), "Cannot delete canonical attempts")
        base_discoveries, discoveries = base.get("discoveries", {}), candidate.get("discoveries", {})
        require(set(base_discoveries) <= set(discoveries)
                and candidate["schema_version"] >= base["schema_version"], "Cannot delete canonical discoveries")
        if expected_head is not None:
            expected_files = self.files(expected_head)
            require(digest(expected_head, 40) and (expected_files == {"state.json": encoded(base)}
                    or (not expected_files and base == empty_state())),
                    "Base state differs from expected revision")
        else:
            require(base == empty_state(), "Missing base revision is only valid for first run")
        changes = [a for key, a in candidate["attempts"].items() if base["attempts"].get(key) != a]
        changes.sort(key=lambda a: a["id"] not in base["attempts"])
        discovery_changes = [d for key, d in discoveries.items() if base_discoveries.get(key) != d]
        require(changes or discovery_changes or base == candidate, "Generation-only update is not a checkpoint")
        require(candidate["schema_version"] == base["schema_version"] or discovery_changes,
                "Schema migration requires a discovery checkpoint")
        # Release old owners before reserving their successors; the CAS publishes
        # the entire validated transaction, never these intermediate states.
        updates = ([(merge_attempt, a, base["attempts"].get(a["id"])) for a in changes
                    if a["outcome"] is not None and a["lifecycle"] != "publication_pending"] +
                   [(merge_discovery, d, base_discoveries.get(d["id"])) for d in discovery_changes
                    if d["outcome"] is not None] +
                   [(merge_discovery, d, base_discoveries.get(d["id"])) for d in discovery_changes
                    if d["outcome"] is None] +
                   [(merge_attempt, a, base["attempts"].get(a["id"])) for a in changes
                    if a["outcome"] is None or a["lifecycle"] == "publication_pending"])
        branch = "refs/heads/memory/compiler-self-profile-candidates/" + uuid.uuid4().hex
        proposed = copy.deepcopy(candidate)
        proposed["base_revision"] = expected_head
        files = {"state.json": encoded(proposed)}
        validate_store(files, {"state.json": encoded(base)} if expected_head else {})
        commit = self.commit(files, expected_head)
        self.git("update-ref", branch, commit, "0" * 40)
        try:
            for _ in range(5):
                current, head = self.read()
                merged = current
                if any(a["id"] not in base["attempts"] and
                       (a["outcome"] is None or a["lifecycle"] == "publication_pending") for a in changes):
                    require(all(d["id"] in discoveries for d in current.get("discoveries", {}).values()
                                if d["outcome"] is None), "A concurrent discovery owns active work")
                if any(d["id"] not in base_discoveries and d["outcome"] is None for d in discovery_changes):
                    require(all(a["id"] in candidate["attempts"] for a in current["attempts"].values()
                                if a["outcome"] is None or a["lifecycle"] == "publication_pending"),
                            "A concurrent attempt owns active work")
                for d in discoveries.values():
                    if d["outcome"] is None and d not in discovery_changes and any(
                            a["id"] not in base["attempts"] and a["run"] == d["run"] for a in changes):
                        owner = current.get("discoveries", {}).get(d["id"])
                        require(owner is not None and owner["outcome"] is None,
                                "Selected discovery no longer owns active work")
                for merge, record, previous in updates:
                    merged = merge(merged, record, previous)
                if merged == current:
                    self.git("update-ref", "-d", branch, commit)
                    return self.receipt(head)
                merged["base_revision"] = head
                files = {"state.json": encoded(merged)}
                validate_store(files, {"state.json": encoded(current)} if head else {})
                new_commit = self.commit(files, head)
                self.git("update-ref", branch, new_commit, commit)
                commit = new_commit
                if self.compare_and_swap(head, commit):
                    self.git("update-ref", "-d", branch, commit)
                    return self.receipt(commit)
            raise Conflict("CAS retry budget exhausted")
        except (ValueError, subprocess.SubprocessError) as error:
            raise Conflict(str(error), branch) from error

    def write(self, base, expected_head, attempt):
        candidate = merge_attempt(base, attempt, base["attempts"].get(attempt["id"]))
        return self.write_state(base, expected_head, candidate)

    @staticmethod
    def receipt(commit):
        return {"storage": "canonical_git_commit", "branch": STATE_BRANCH,
                "commit": commit, "remote_acknowledged": False}


def publish_notes(repository, notes, expected_head):
    """Native-note simulation: fixed separate branch, no canonical fields/paths/deletions."""
    journal = GitJournal(repository)
    files = {}
    for name, note in notes.items():
        validate_note(note)
        require(name == note["attempt_id"] + ".json" and name != "state.json", "Invalid note filename")
        files[name] = encoded(note)
    head = journal.head(NOTES_BRANCH)
    if head != expected_head:
        raise Conflict("Native note revision changed")
    previous = journal.files(head) if head else {}
    files = dict(previous, **files)
    validate_store(files, previous)
    commit = journal.commit(files, head)
    journal.git("update-ref", "refs/heads/" + NOTES_BRANCH, commit, head or "0" * 40)
    return {"storage": "untrusted_note_commit", "branch": NOTES_BRANCH, "commit": commit,
            "remote_acknowledged": False}


class GitHubReader:
    """Read-only REST adapter. Pagination never relies on a fixed --limit."""

    @staticmethod
    def get(endpoint, *, page=False):
        result = subprocess.run(["gh", "api", endpoint, *(["--include"] if page else [])],
                                check=True, capture_output=True, timeout=60)
        if not page:
            return decoded(result.stdout)
        parts = re.split(rb"\r?\n\r?\n", result.stdout, maxsplit=1)
        require(len(parts) == 2 and re.match(rb"HTTP/\S+ 200\b", parts[0]), "Incomplete PR API response")
        links = re.findall(rb"^link:\s*(.+)$", parts[0], flags=re.I | re.M)
        return {"data": decoded(parts[1]), "has_next": any(b'rel="next"' in link for link in links)}

    def prs(self, page):
        response = self.get(f"repos/{REPOSITORY}/pulls?state=open&per_page=100&page={page}", page=True)
        records = response["data"]
        require(type(records) is list and type(response["has_next"]) is bool, "PR API did not return a complete page")
        items = []
        for p in records:
            marker = re.search(r"<!-- compiler-self-profile:([A-Za-z0-9][A-Za-z0-9_.-]{0,127}) -->", p.get("body") or "")
            items.append({"number": p["number"], "state": p["state"], "draft": p["draft"],
                          "base_repository": p["base"]["repo"]["full_name"], "base_ref": p["base"]["ref"],
                          "head_repository": p["head"]["repo"]["full_name"] if p["head"]["repo"] else None,
                          "head_ref": p["head"]["ref"], "head_sha": p["head"]["sha"],
                          "labels": [label["name"] for label in p["labels"]],
                          "marker": WORKFLOW_MARKER if marker else None,
                          "attempt_id": marker[1] if marker else None, "url": p["html_url"]})
        return {"status": 200, "items": items, "next": page + 1 if response["has_next"] else None, "complete": True}

    def run(self, owner):
        require(owner["id"].isdigit(), "Actions run ID must be numeric")
        run = self.get(f"repos/{REPOSITORY}/actions/runs/{owner['id']}/attempts/{owner['attempt']}")
        return {"status": 200, "id": str(run["id"]), "attempt": run["run_attempt"], "state": run["status"]}

    def verify_source(self, request):
        repository = self.get(f"repos/{REPOSITORY}")
        require(repository["full_name"] == REPOSITORY and repository["default_branch"] == request["source_ref"],
                "Source is not the current default branch")
        commit = self.get(f"repos/{REPOSITORY}/commits/{quote(repository['default_branch'], safe='')}")
        require(commit["sha"] == request["source_revision"], "Default-branch source revision changed")


def summarize(state, now):
    validate_state(state)
    outcomes = {}
    evidence = []
    for a in state["attempts"].values():
        outcome = a["outcome"] or "active"
        outcomes[outcome] = outcomes.get(outcome, 0) + 1
        evidence.extend(dict(artifact_availability(e, now), sha256=e["sha256"], attempt_id=a["id"])
                        for e in a["evidence"])
    for d in state.get("discoveries", {}).values():
        evidence.extend(dict(artifact_availability(e, now), sha256=e["sha256"], discovery_id=d["id"])
                        for e in d.get("evidence", []))
    return {"schema_version": 1, "generation": state["generation"], "outcomes": outcomes, "artifacts": evidence}


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    for name in ("preflight", "checkpoint", "merge", "summarize"):
        command = commands.add_parser(name)
        command.add_argument("--input")
        if name in ("preflight", "merge"):
            command.add_argument("--repository", required=name == "merge")
        if name == "checkpoint":
            command.add_argument("--output", required=True)
        if name == "summarize":
            command.add_argument("--now")
    args = parser.parse_args(argv)
    try:
        if args.input:
            with Path(args.input).open("rb") as stream:
                data = stream.read(LIMITS["patch_bytes"] + 1)
        else:
            data = sys.stdin.buffer.read(LIMITS["patch_bytes"] + 1)
        require(len(data) <= LIMITS["patch_bytes"], "CLI JSON input budget exceeded")
        value = decoded(data)
        if args.command == "checkpoint":
            result = checkpoint(args.output, value)
        elif args.command == "merge":
            require(set(value) == {"base", "expected_head", "attempt"}, "Invalid merge input")
            result = GitJournal(args.repository).write(value["base"], value["expected_head"], value["attempt"])
        elif args.command == "summarize":
            result = summarize(value, timestamp(args.now) if args.now else datetime.now(timezone.utc))
        elif args.repository:
            require(set(value) == {"request"}, "Canonical preflight requires live queries, not injected API snapshots")
            journal, api = GitJournal(args.repository), GitHubReader()
            api.verify_source(value["request"])
            base, head = journal.read()
            result = preflight(base, value["request"], api.prs, api.run, datetime.now(timezone.utc))
            if result["state"] != base or result["action"] == "resume":
                result["receipt"] = journal.write_state(base, head, result["state"])
        else:
            require(set(value) == {"state", "request", "pr_pages", "runs", "now"}, "Invalid offline preflight input")
            result = preflight(value["state"], value["request"], lambda n: value["pr_pages"][n - 1],
                               lambda owner: next(r for r in value["runs"] if r["id"] == owner["id"]
                                                  and r["attempt"] == owner["attempt"]), timestamp(value["now"]))
        print(encoded(result).decode(), end="")
        return 2 if result.get("action") == "blocked" else 0
    except (OSError, ValueError, KeyError, TypeError, AttributeError, IndexError, StopIteration,
            subprocess.SubprocessError) as error:
        result = {"outcome": "blocked_setup", "reason": str(error)}
        if isinstance(error, Conflict):
            result["candidate_ref"] = error.candidate_ref
        print(encoded(result).decode(), end="")
        return 2


if __name__ == "__main__":
    sys.exit(main())
