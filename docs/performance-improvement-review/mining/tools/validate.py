"""Validate the frozen inventory, not the substantive inspection of its sources."""

import argparse
import hashlib
import json
from pathlib import Path, PureWindowsPath
import sys

REPOSITORY = "dotnet/fsharp"
START = "2026-04-22T00:00:00Z"
CUTOFF = "2026-09-22T09:28:34Z"
STARTING_HEAD = "8d21ff13acda3eff7b26cfdde8b2dbb4db40955a"
SEALED_SHA256 = "591530729c4265e397e74e8b852380cabb666437b49d65e30df9a41ff0a53b79"
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


def load_discovery(path, *, expected_sha256=SEALED_SHA256):
    discovery = parse_json(read_text(path), str(path))
    require(digest(discovery) == expected_sha256, f"{path}: discovery seal mismatch")
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
    require(set(discovery) - {"historical_counts"} == fields,
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
    sources = discovery["sources"]
    for n, row in indexed.items():
        channel = f"PR {n}/index"
        require(set(row) == fields, f"{channel}: missing/unknown fields {sorted(set(row) ^ fields)}")
        require(type(row["schema_version"]) is int and row["schema_version"] == 1,
                f"{channel}: unsupported schema_version")
        for key, value in members[n].items():
            require(type(row[key]) is type(value) and row[key] == value,
                    f"{channel}: changed frozen {key}")
        scope = row["scope"]
        require(isinstance(scope, dict) and set(scope) == {"decision", "reason", "source_ids"},
                f"{channel}: invalid scope")
        require(scope["decision"] in ("pending", "included", "excluded"),
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


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--discovery", type=Path, default=ROOT / "discovery.json")
    parser.add_argument("--index", type=Path, default=ROOT / "work-index.jsonl")
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
        if args.stage != "inventory":
            pending = sum(row["inspection_status"] == "pending" for row in rows)
            raise ValidationError(
                f"stage {args.stage}: not implemented; {pending} PR inspections pending; "
                "final corpus completion is false")
        print(json.dumps({"stage": "inventory", "inventory": len(rows),
                          "final_corpus_complete": False}))
        return 0
    except ValidationError as error:
        print(f"validation failed: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
