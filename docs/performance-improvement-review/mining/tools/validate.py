"""Validate the frozen inventory, scope and exposure without claiming inspection completion."""

import argparse
from datetime import datetime
import hashlib
import json
from pathlib import Path, PureWindowsPath
import re
import sys

REPOSITORY = "dotnet/fsharp"
START = "2026-04-22T00:00:00Z"
CUTOFF = "2026-09-22T09:28:34Z"
STARTING_HEAD = "8d21ff13acda3eff7b26cfdde8b2dbb4db40955a"
SEALED_SHA256 = "591530729c4265e397e74e8b852380cabb666437b49d65e30df9a41ff0a53b79"
SCOPE_SHA256 = "3c5e158dc1752786e821c4564465363079fbf838494436181d851fe5723bce5e"
POLICY_VERSION = "scope-exposure-1"
CORPUS_STATUS = (
    "<!-- BEGIN CURRENT SCOPE STATUS -->\n"
    "**Current scope freeze:** [485-PR index and policy](mining/README.md), "
    "[exposure ledger](mining/evaluation/exposure.json), and "
    "[evaluation protocol](mining/evaluation/protocol.json). "
    "Historical sections below are preserved; previous holdout labels do not establish freshness. "
    "Real-source availability gaps block evaluation acceptance, not inventory accounting.\n"
    "<!-- END CURRENT SCOPE STATUS -->\n\n"
)
ROOT = Path(__file__).resolve().parents[1]
STAGES = ("inventory", "scope", "red", "acquisition", "batch", "training",
          "rules", "audit", "sealed", "complete")


class ValidationError(ValueError):
    pass


def require(condition, message):
    if not condition:
        raise ValidationError(message)


def digest(value):
    return hashlib.sha256(json.dumps(
        value, sort_keys=True, separators=(",", ":"), ensure_ascii=True
    ).encode("utf-8")).hexdigest()


def unique_fields(pairs):
    result = {}
    for key, value in pairs:
        require(key not in result, f"duplicate JSON field {key!r}")
        result[key] = value
    return result


def parse_json(text, channel):
    try:
        return json.loads(text, object_pairs_hook=unique_fields)
    except (ValueError, RecursionError) as error:
        raise ValidationError(f"{channel}: {error}") from error


def read_text(path):
    try:
        return Path(path).read_text(encoding="utf-8-sig")
    except (OSError, UnicodeError) as error:
        raise ValidationError(f"{path}: {error}") from error


def frozen_file_bytes(path):
    raw = Path(path).read_bytes().replace(b"\r\n", b"\n")
    if Path(path).name == "corpus.md":
        raw = raw.replace(CORPUS_STATUS.encode("utf-8"), b"", 1)
    if Path(path).parts[-3:] == ("release-notes", ".FSharp.Compiler.Service", "11.0.200.md"):
        lines = raw.splitlines(keepends=True)
        headings = [i for i, line in enumerate(lines) if line == b"### Added\n"]
        marker = b"([PR #20619](https://github.com/dotnet/fsharp/pull/20619))"
        require(len(headings) == 1 and raw.count(marker) == 1,
                "release note: expected one Added section and one PR20619 entry")
        start = headings[0]
        end = next((i for i in range(start + 1, len(lines)) if lines[i].startswith(b"#")), len(lines))
        entry = next(i for i, line in enumerate(lines) if marker in line)
        require(start + 1 < entry < end and lines[start + 1] == b"\n"
                and lines[entry].startswith(b"* "), "release note: PR20619 must remain an Added entry")
        raw = lines[start] + lines[start + 1] + lines[entry]
    return raw


def load_discovery(path, *, expected_sha256=SEALED_SHA256):
    discovery = parse_json(read_text(path), str(path))
    require(isinstance(discovery, dict), f"{path}: expected discovery object")
    original = {k: v for k, v in discovery.items() if k != "scope_freeze"}
    require(digest(original) == expected_sha256, f"{path}: discovery seal mismatch")
    if "scope_freeze" in discovery:
        require(digest(discovery["scope_freeze"]) == SCOPE_SHA256,
                f"{path}: additive scope freeze seal mismatch")
    check_discovery(discovery)
    return discovery


def load_index(path):
    rows = []
    for line, text in enumerate(read_text(path).splitlines(), 1):
        row = parse_json(text, f"{path}: line {line}")
        require(isinstance(row, dict), f"{path}: line {line}: expected PR object")
        rows.append(row)
    require(rows, f"{path}: empty index")
    return rows


def unique_by(rows, key, channel):
    require(isinstance(rows, list), f"{channel}: expected array")
    result = {}
    for row in rows:
        require(isinstance(row, dict), f"{channel}: expected object")
        value = row.get(key)
        valid = type(value) is int and value > 0 if key in ("pr", "number") else (
            isinstance(value, str) and bool(value))
        require(valid, f"{channel}: invalid {key} {value!r}")
        require(value not in result, f"{channel}: duplicate {key} {value}")
        result[value] = row
    return result


def cache_path(root, identifier):
    require(isinstance(identifier, str) and bool(identifier), "cache: missing relative identifier")
    relative = PureWindowsPath(identifier)
    require(not relative.drive and not relative.root and ".." not in relative.parts,
            f"cache: expected relative identifier, got {identifier!r}")
    root = Path(root).resolve()
    path = root.joinpath(*relative.parts).resolve()
    require(path.is_relative_to(root), f"cache: identifier escapes source root: {identifier}")
    return path


def check_discovery(discovery):
    require(isinstance(discovery, dict), "discovery: expected object")
    fields = {"schema_version", "repository", "start", "cutoff", "original_head",
              "continuation", "final_corpus_complete", "sources", "queries",
              "linked", "counts", "members", "frozen_files"}
    require(set(discovery) - {"historical_counts", "scope_freeze"} == fields,
            "discovery: missing/unknown fields; completion flags cannot be added")
    require(isinstance(discovery["sources"], dict), "discovery: expected source manifest map")
    for key, value in (("repository", REPOSITORY), ("start", START), ("cutoff", CUTOFF)):
        require(discovery.get(key) == value, f"discovery: wrong {key}; expected {value}")
    require(type(discovery.get("schema_version")) is int and discovery["schema_version"] == 1,
            "discovery: unsupported schema_version")
    require(discovery.get("final_corpus_complete") is False,
            "discovery: final corpus completion is false; completion cannot be asserted")
    members = unique_by(discovery.get("members"), "pr", "discovery")
    unique_by(discovery["members"], "node_id", "discovery")
    queries = unique_by(discovery.get("queries"), "id", "discovery queries")
    memberships = {n: [] for n in members}
    occurrences = 0
    for qid, query in queries.items():
        numbers = query.get("members")
        require(isinstance(numbers, list) and all(type(n) is int for n in numbers),
                f"{qid}: invalid member list")
        require(len(numbers) == len(set(numbers)) and numbers == sorted(numbers),
                f"{qid}: duplicated or unsorted members")
        require(query.get("incomplete_results") is False, f"{qid}: incomplete search")
        require(query.get("total") == query.get("collected") == len(numbers),
                f"{qid}: search count mismatch")
        for n in numbers:
            require(n in members, f"PR {n}/{qid}: not in frozen inventory")
            memberships[n].append(qid)
        occurrences += len(numbers)
    linked = discovery.get("linked")
    require(isinstance(linked, list) and all(type(n) is int for n in linked),
            "discovery: invalid linked membership")
    require(linked == sorted(set(linked)), "discovery: duplicate or unsorted linked membership")
    search_unique = sum(bool(ids) for ids in memberships.values())
    for n in linked:
        require(n in members and not memberships[n], f"PR {n}/linked: invalid expansion member")
        memberships[n].append("linked")
    for ordinal, (n, member) in enumerate(sorted(members.items()), 1):
        require(member.get("repository") == REPOSITORY, f"PR {n}/discovery: wrong repository")
        require(member.get("url") == f"https://github.com/{REPOSITORY}/pull/{n}",
                f"PR {n}/discovery: wrong URL")
        require(memberships[n] and member.get("discovery_ids") == memberships[n],
                f"PR {n}/discovery: missing or changed discovery membership")
        require(member.get("ordinal") == ordinal and member.get("shard") == (ordinal - 1) // 25 + 1,
                f"PR {n}/discovery: changed ordinal/shard")
    require(discovery.get("counts") == {
        "inventory": len(members), "search_occurrences": occurrences,
        "search_unique": search_unique, "linked": len(linked),
    }, "discovery: reconciled counts do not match")
    return members


def validate_inventory(discovery, rows):
    members = check_discovery(discovery)
    indexed = unique_by(rows, "pr", "index")
    unique_by(rows, "node_id", "index")
    missing, extra = sorted(members.keys() - indexed.keys()), sorted(indexed.keys() - members.keys())
    require(not missing and not extra, f"index: exact identity mismatch; missing PRs {missing}; extra PRs {extra}")
    require(list(indexed) == sorted(indexed), "index: PR rows must be sorted")
    fields = {"schema_version", "repository", "pr", "node_id", "url", "author",
              "discovery_ids", "scope", "family", "ordinal", "shard", "split",
              "source_status", "inspection_status", "receipt", "rule_links", "historical"}
    scoped = "scope_freeze" in discovery
    sources = {**discovery["sources"], **discovery.get("scope_freeze", {}).get("sources", {})}
    for n, row in indexed.items():
        channel = f"PR {n}/index"
        require(set(row) == fields, f"{channel}: missing/unknown fields {sorted(set(row) ^ fields)}")
        require(type(row["schema_version"]) is int and row["schema_version"] == (2 if scoped else 1),
                f"{channel}: unsupported schema_version")
        for key, value in members[n].items():
            if scoped and key in ("ordinal", "shard"):
                value = member_ordinal = members[n]["ordinal"] - 1
                if key == "shard":
                    value = member_ordinal % 6
            require(type(row[key]) is type(value) and row[key] == value,
                    f"{channel}: changed frozen {key}")
        scope = row["scope"]
        require(isinstance(scope, dict) and {"decision", "reason", "source_ids"} <= set(scope),
                f"{channel}: invalid scope")
        require(scoped or set(scope) == {"decision", "reason", "source_ids"},
                f"{channel}: unknown legacy scope fields")
        require(scope["decision"] in (("in", "out") if scoped else ("pending", "included", "excluded")),
                f"{channel}: invalid scope decision")
        require(isinstance(scope["reason"], str) and scope["reason"].strip(),
                f"{channel}: missing scope reason")
        refs = scope["source_ids"]
        require(isinstance(refs, list) and refs and all(
            isinstance(ref, str) and ref.split("#")[0] in sources for ref in refs),
            f"{channel}: missing/unknown scope source IDs")
        require(isinstance(row["family"], str) and row["family"], f"{channel}: missing family")
        require(row["split"] in ("pending", "development", "control", "holdout"),
                f"{channel}: invalid split")
        require(row["source_status"] in ("pending", "failed", "stale", "acquired"),
                f"{channel}: invalid source_status")
        require(row["inspection_status"] in ("pending", "failed", "stale", "inspected"),
                f"{channel}: acquisition is not inspection; invalid inspection_status")
        require(row["receipt"] is None or isinstance(row["receipt"], str),
                f"{channel}: invalid receipt")
        if row["receipt"] is not None:
            cache_path(ROOT, row["receipt"])
        if row["inspection_status"] == "inspected":
            require(row["source_status"] == "acquired" and row["receipt"],
                    f"{channel}: inspected requires acquired sources and an inspection receipt")
        require(isinstance(row["rule_links"], list) and all(
            isinstance(link, str) and link for link in row["rule_links"]),
            f"{channel}: invalid rule_links")
        require(isinstance(row["historical"], dict), f"{channel}: invalid historical record")


def timestamp(value, channel):
    require(isinstance(value, str), f"{channel}: missing timestamp")
    try:
        result = datetime.fromisoformat(value.replace("Z", "+00:00"))
    except ValueError as error:
        raise ValidationError(f"{channel}: invalid timestamp {value!r}") from error
    require(result.tzinfo is not None and result.utcoffset().total_seconds() == 0,
            f"{channel}: timestamp must be UTC")
    return result


def source_refs(refs, sources, channel, pr=None):
    require(isinstance(refs, list) and refs, f"{channel}: missing source")
    for ref in refs:
        require(isinstance(ref, str) and ref.split("#")[0] in sources,
                f"{channel}: unknown source {ref!r}")
        if pr is not None:
            require(ref.endswith(f"#pr={pr}"), f"{channel}: copied/wrong PR source")


def scope_identity(row):
    return {"pr": row["pr"], "node_id": row["node_id"], "author": row["author"],
            "created_at": row["scope"]["created_at"]}


def scope_projection(rows):
    return [{k: r[k] for k in ("pr", "ordinal", "shard", "scope", "family", "split", "historical")}
            for r in rows]


def validate_scope(discovery, rows, protocol):
    validate_inventory(discovery, rows)
    freeze = discovery.get("scope_freeze")
    require(isinstance(freeze, dict), "scope: missing additive provenance freeze")
    require(isinstance(protocol, dict) and digest(protocol) == freeze.get("protocol_sha256"),
            "scope: protocol changed after predeclaration")
    require(protocol.get("policy_version") == POLICY_VERSION
            and protocol.get("start") == START and protocol.get("cutoff") == CUTOFF
            and protocol.get("final_corpus_complete") is False,
            "scope: invalid policy or historical bounds")
    require(not set(discovery["sources"]).intersection(freeze["sources"]),
            "scope: additive provenance cannot shadow original sources")
    sources = {**discovery["sources"], **freeze["sources"]}
    for sid, source in sources.items():
        require(isinstance(source, dict) and isinstance(source.get("sha256"), str)
                and re.fullmatch(r"[0-9a-f]{64}", source["sha256"]),
                f"{sid}: missing immutable source hash")
    allowed = {"performance-claim", "measurement", "mechanism",
               "no-performance-claim", "context-only", "outside-window"}
    start, cutoff = timestamp(START, "start"), timestamp(CUTOFF, "cutoff")
    for ordinal, row in enumerate(rows):
        n, scope = row["pr"], row["scope"]
        channel = f"PR {n}/scope"
        require(set(scope) == {
            "decision", "reason", "basis", "anchor", "source_ids", "created_at", "event",
            "title", "labels", "description_sha256", "changed_files", "reconciliation",
            "policy_version", "historical_identity",
        }, f"{channel}: missing/unknown scope fields")
        require(row["ordinal"] == ordinal and row["shard"] == ordinal % 6,
                f"{channel}: reassigned ordinal/shard")
        require(scope.get("policy_version") == POLICY_VERSION, f"{channel}: wrong decision policy")
        require(scope.get("historical_identity") == scope_identity(row),
                f"{channel}: changed historical identity")
        reason, anchor = scope.get("reason"), scope.get("anchor")
        require(isinstance(reason, str) and bool(reason.strip())
                and isinstance(anchor, str) and bool(anchor.strip())
                and str(n) in reason, f"{channel}: blank/boilerplate-only reason or missing PR anchor")
        reason_text = re.sub(rf"^PR\s+{n}\s*:\s*", "", reason, flags=re.IGNORECASE)
        require(reason_text.strip() and not re.fullmatch(
            r"(?i)(?:(?:performance benchmarks added(?: where appropriate)?|no performance claim|"
            r"in.scope|out.of.scope|reviewed)[\s.;,!?\[\]x-]*)+", reason_text),
            f"{channel}: boilerplate-only reason")
        require(not re.search(r"(?i)(?:excluded?|out.of.scope)\s+(?:solely\s+)?"
                              r"(?:because|due to)\s+(?:the\s+)?(?:author|bot|account)", reason),
                f"{channel}: identity-only exclusion")
        require(scope.get("basis") in allowed, f"{channel}: invalid content-based scope basis")
        require(scope["decision"] == ("in" if scope["basis"] in
                ("performance-claim", "measurement", "mechanism") else "out"),
                f"{channel}: scope decision contradicts eligibility basis")
        source_refs(scope["source_ids"], sources, channel)
        require(f"pr-{n}#pr={n}" in scope["source_ids"],
                f"{channel}: missing complete preserved description source")
        require(isinstance(scope.get("title"), str) and scope["title"].strip()
                and isinstance(scope.get("description_sha256"), str)
                and re.fullmatch(r"[0-9a-f]{64}", scope["description_sha256"])
                and isinstance(scope.get("labels"), list),
                f"{channel}: missing description/label screening receipt")
        files = scope.get("changed_files")
        require(isinstance(files, dict) and files.get("availability") == "complete"
                and isinstance(files.get("paths"), list), f"{channel}: missing changed-file metadata")
        source_refs(files.get("source_ids"), sources, f"{channel}/paths", n)
        created = timestamp(scope.get("created_at"), channel)
        event = scope.get("event")
        if event is not None:
            require(isinstance(event, dict) and event.get("kind") in
                    ("creation", "comment", "review", "commit", "merged", "closed",
                     "reopened", "labeled", "other"), f"{channel}: updatedAt is not temporal evidence")
            at = timestamp(event.get("at"), channel)
            require(start <= at <= cutoff and at >= created, f"{channel}: event outside window")
            source_refs([event.get("source_id")], sources, f"{channel}/event", n)
            require(isinstance(event.get("event_id"), str) and event["event_id"],
                    f"{channel}: missing immutable event identity")
            require(not re.search(r"(?i)updated_?at", event["event_id"]),
                    f"{channel}: updatedAt is not temporal evidence")
            if created < start:
                require(event["source_id"] == f"activity-{n}#pr={n}",
                        f"{channel}: older creation requires a preserved activity receipt")
            if event["kind"] == "creation":
                require(at == created, f"{channel}: fabricated creation event")
        if scope["decision"] == "in":
            require(created <= cutoff and (created >= start or event is not None),
                    f"{channel}: older creation needs qualifying in-window event, not updatedAt")
        previous = row["historical"].get("decision")
        require(isinstance(previous, dict) and type(previous.get("included")) is bool,
                f"{channel}: missing original decision")
        old = "in" if previous["included"] else "out"
        reconciliation = scope.get("reconciliation", {})
        require(isinstance(reconciliation, dict) and reconciliation.get("previous") == old
                and reconciliation.get("changed") is (old != scope["decision"])
                and isinstance(reconciliation.get("reason"), str)
                and bool(reconciliation["reason"].strip()),
                f"{channel}: missing decision reconciliation")
    require(digest(scope_projection(rows)) == freeze.get("index_sha256"),
            "scope: frozen adjudication, history or partition changed")


def validate_exposure(discovery, rows, exposure, protocol):
    require(isinstance(exposure, dict), "exposure: expected object")
    freeze = discovery["scope_freeze"]
    sources = {**discovery["sources"], **freeze["sources"]}
    require(exposure.get("policy_version") == POLICY_VERSION
            and exposure.get("start") == START and exposure.get("cutoff") == CUTOFF,
            "exposure: policy or historical window changed")
    records = unique_by(exposure.get("prs"), "pr", "exposure")
    indexed = unique_by(rows, "pr", "index")
    require(set(records) == set(indexed), "exposure: exact original PR coverage required")
    require(all(records[n].get("node_id") == indexed[n]["node_id"] for n in records),
            "exposure: stable node identity changed")
    families = unique_by(exposure.get("families"), "id", "exposure families")
    seen = set()
    for family_id, family in families.items():
        numbers = family.get("prs")
        require(isinstance(numbers, list) and numbers and all(type(n) is int for n in numbers)
                and numbers == sorted(set(numbers))
                and set(numbers) <= set(indexed) and not seen.intersection(numbers),
                f"{family_id}: missing, duplicate or unknown family membership")
        seen.update(numbers)
        require(all(indexed[n]["family"] == family_id and records[n]["family"] == family_id
                    for n in numbers), f"{family_id}: family renamed or split to hide contamination")
        splits = {indexed[n]["split"] for n in numbers}
        require(len(splits) == 1 and family.get("split") in splits,
                f"{family_id}: connected family straddles training/control/holdout")
        aliases = sorted({indexed[n]["historical"]["decision"]["family"] for n in numbers})
        require(family.get("historical_aliases") == aliases, f"{family_id}: lost historical aliases")
    require(seen == set(indexed), "exposure: family coverage mismatch")
    historical_families = {}
    for row in rows:
        alias = row["historical"]["decision"]["family"]
        historical_families.setdefault(alias, set()).add(row["family"])
    require(all(len(ids) == 1 for ids in historical_families.values()),
            "exposure: historical connected family split under new names")
    for edge in exposure.get("connections", []):
        require(isinstance(edge, dict), "exposure: invalid connection")
        numbers = edge.get("prs", [])
        require(isinstance(numbers, list) and len(numbers) >= 2
                and all(type(n) is int and n in indexed for n in numbers)
                and len({indexed[n]["family"] for n in numbers}) == 1,
                "exposure: connected/superseding variants split")
        source_refs(edge.get("source_ids"), sources, "family connection")
        require(bool(edge.get("reason")), "exposure: connection needs mechanism rationale")
    categories = set(protocol["exposure"]["categories"])
    semantic = categories - {"acquisition-only", "metadata-only-screening"}
    supplemental = unique_by(exposure.get("supplemental"), "pr", "supplemental exposure")
    require(not set(supplemental).intersection(indexed),
            "exposure: supplemental validation must not change/duplicate the original universe")
    for n, record in {**records, **supplemental}.items():
        channel = f"PR {n}/exposure"
        events = record.get("events")
        require(isinstance(events, list) and events, f"{channel}: missing access history")
        for event in events:
            require(isinstance(event, dict) and event.get("kind") in categories,
                    f"{channel}: invalid exposure category")
            timestamp(event.get("at"), channel)
            source_refs(event.get("source_ids"), sources, channel)
            require(isinstance(event.get("detail"), str) and event["detail"].strip(),
                    f"{channel}: missing access rationale")
        require(type(record.get("fresh")) is bool, f"{channel}: missing freshness decision")
        if record["fresh"]:
            require(not any(e["kind"] in semantic for e in events),
                    f"{channel}: unknown or semantic exposure cannot be fresh")
            require(record.get("access_audit_complete") is True
                    and record.get("connection_audit_complete") is True,
                    f"{channel}: freshness requires complete access and connection audits")
        authorship = record.get("authorship", {})
        subject = f"pr-{n}:description"
        require(isinstance(authorship, dict) and authorship.get("subject") == subject
                and authorship.get("enclosing_scope") == subject
                and authorship.get("classification") in protocol["authorship"]["classifications"]
                and bool(authorship.get("rationale")),
                f"{channel}: missing source-specific authorship provenance")
        source_refs(authorship.get("source_ids"), sources, f"{channel}/authorship", n)
        if authorship["classification"] == "verified-human":
            attestation = authorship.get("positive_attestation")
            require(isinstance(attestation, dict) and attestation.get("subject") == subject
                    and attestation.get("enclosing_scope") == subject
                    and bool(attestation.get("rationale")),
                    f"{channel}: missing/copied human composition attestation")
            source_refs([attestation.get("source_id")], sources, channel, n)
            source = sources[attestation["source_id"].split("#")[0]]
            require(source.get("attested_subject") == subject,
                    f"{channel}: account identity is not a composition attestation")
        if n in supplemental:
            require(record.get("split") == "validation-only"
                    and record.get("qualification") == "unverified"
                    and record["fresh"] is False,
                    f"{channel}: metadata lead cannot be promoted to real holdout")
    for family in families.values():
        kinds = sorted({e["kind"] for n in family["prs"] for e in records[n]["events"]})
        fresh = all(records[n]["fresh"] for n in family["prs"])
        require(family.get("exposure_kinds") == kinds and family.get("fresh") is fresh,
                f"{family['id']}: family exposure does not propagate across all members")
        if family["split"] == "holdout":
            require(fresh, f"{family['id']}: contaminated or unknown family cannot be holdout")
    require(exposure.get("evaluation_acceptance") == "blocked"
            and exposure.get("new_reviewer_runs") == 0,
            "exposure: scope freeze does not authorize reviewer execution or acceptance")
    gaps = unique_by(exposure.get("availability_gaps"), "stratum", "availability gaps")
    require(set(gaps) == set(protocol["evaluation"]["required_real_strata"])
            and all(g.get("blocking") is True and g.get("reason") for g in gaps.values()),
            "exposure: required real-source gaps cannot be waived or hidden")
    require(digest(exposure) == freeze.get("exposure_sha256"),
            "exposure: immutable access history changed")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--discovery", type=Path, default=ROOT / "discovery.json")
    parser.add_argument("--index", type=Path, default=ROOT / "work-index.jsonl")
    parser.add_argument("--protocol", type=Path, default=ROOT / "evaluation" / "protocol.json")
    parser.add_argument("--exposure", type=Path, default=ROOT / "evaluation" / "exposure.json")
    parser.add_argument("--stage", choices=STAGES, required=True)
    parser.add_argument("--batch", type=int)
    args = parser.parse_args(argv)
    try:
        require((args.stage == "batch" and args.batch is not None and args.batch > 0)
                or (args.stage != "batch" and args.batch is None),
                "--batch N (positive integer) is required only for --stage batch")
        discovery = load_discovery(args.discovery)
        require(discovery["counts"] == {
            "inventory": 485, "search_occurrences": 1205, "search_unique": 475, "linked": 10,
        }, "discovery: production requires the sealed 485-member corpus")
        rows = load_index(args.index)
        validate_inventory(discovery, rows)
        if args.stage == "scope":
            protocol = parse_json(read_text(args.protocol), "protocol")
            exposure = parse_json(read_text(args.exposure), "exposure")
            validate_scope(discovery, rows, protocol)
            validate_exposure(discovery, rows, exposure, protocol)
        elif args.stage != "inventory":
            pending = sum(row["inspection_status"] == "pending" for row in rows)
            raise ValidationError(
                f"stage {args.stage}: not implemented; {pending} PR inspections pending; "
                "final corpus completion is false")
        print(json.dumps({"stage": args.stage, "inventory": len(rows),
                          "in_scope": sum(r["scope"]["decision"] == "in" for r in rows),
                          "evaluation_acceptance": "blocked",
                          "final_corpus_complete": False}))
        return 0
    except ValidationError as error:
        print(f"validation failed: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
