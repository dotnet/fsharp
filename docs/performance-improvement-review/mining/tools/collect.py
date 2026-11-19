"""Recover the original corpus offline. Missing/invalid caches fail; no network fallback."""

import argparse
import hashlib
import json
from pathlib import Path
import re
import sys
from urllib.parse import urlencode

from validate import (CUTOFF, REPOSITORY, ROOT, SEALED_SHA256, START, STARTING_HEAD,
                      ValidationError, cache_path, digest, frozen_file_bytes, parse_json, read_text,
                      require, unique_by, validate_inventory)

ORIGINAL = "c8f75c7a-5c38-4380-a60f-80b1f04b4d69\\files"
HOLDOUT = "36233952-cdbf-4056-984e-1fe93e32d7a3\\files\\holdout-study"
FROZEN_FILES = (
    ".github\\agents\\performance-improvement-review.md",
    ".github\\skills\\reviewing-compiler-prs\\SKILL.md",
    "docs\\performance-improvement-review\\corpus.md",
    "docs\\performance-improvement-review\\evidence.md",
    "docs\\performance-improvement-review\\validation.md",
    "docs\\release-notes\\.FSharp.Compiler.Service\\11.0.200.md",
)


def read_source(root, identifier, source_id, sources, url=None):
    path = cache_path(root, identifier)
    try:
        raw = path.read_bytes()
        value = parse_json(raw.decode("utf-8-sig"), f"{source_id}/{identifier}")
    except (OSError, UnicodeError) as error:
        raise ValidationError(f"{source_id}/{identifier}: {error}") from error
    sources[source_id] = {
        "cache_path": identifier, "sha256": hashlib.sha256(raw).hexdigest(), "url": url,
    }
    return value


def reconcile(inventory, decisions, queries, corpus_numbers):
    raw = unique_by(inventory, "number", "inventory-raw")
    unique_by(inventory, "node_id", "inventory-raw")
    prior = unique_by(decisions, "number", "decisions")
    require(set(raw) == set(prior) == set(corpus_numbers)
            and len(corpus_numbers) == len(raw),
            "inventory: saved decisions, raw inventory and original corpus identities differ")
    memberships = {n: [] for n in raw}
    reconciled = []
    for entry, pages in queries:
        qid = entry["id"]
        require(isinstance(pages, list) and pages, f"{qid}: missing search pages")
        require(len(pages) == entry["pages"] and entry["incomplete_results"] is False,
                f"{qid}: page count/incomplete search mismatch")
        items = []
        for page in pages:
            require(page["incomplete_results"] is False and page["total_count"] == entry["total"],
                    f"{qid}: incomplete search or inconsistent totals")
            items.extend(page["items"])
        found = unique_by(items, "number", qid)
        require(len(found) == entry["total"] == entry["collected"], f"{qid}: search count mismatch")
        for n, item in found.items():
            require(n in raw, f"PR {n}/{qid}: missing from saved inventory")
            for key in ("node_id", "html_url", "repository_url"):
                require(item[key] == raw[n][key], f"PR {n}/{qid}: changed {key}")
            memberships[n].append(qid)
        reconciled.append(dict(entry, members=sorted(found)))
    members = []
    for ordinal, (n, item) in enumerate(sorted(raw.items()), 1):
        ids = memberships[n] or ["linked"]
        require(item["queries"] == prior[n]["queries"] == ids,
                f"PR {n}/discovery: source memberships disagree")
        require(item["repository_url"] == f"https://api.github.com/repos/{REPOSITORY}"
                and item["html_url"] == f"https://github.com/{REPOSITORY}/pull/{n}",
                f"PR {n}/inventory: wrong repository or URL")
        require(prior[n]["author"] == item["user"]["login"],
                f"PR {n}/decisions: author differs from actual author")
        members.append({
            "repository": REPOSITORY, "pr": n, "node_id": item["node_id"],
            "url": item["html_url"], "author": item["user"]["login"],
            "discovery_ids": ids, "ordinal": ordinal, "shard": (ordinal - 1) // 25 + 1,
        })
    return members, reconciled


def make_index(members, decisions, development, reserve, holdout):
    prior = unique_by(decisions, "number", "decisions")
    rows = []
    for member in members:
        n = member["pr"]
        decision = prior[n]
        historical = {"decision": decision}
        if n in development:
            historical["development_counts"] = development[n]
            historical["development_source_id"] = f"validation-counts#pr={n}"
        if n in reserve:
            historical["reserve"] = reserve[n]
            historical["reserve_source_id"] = f"reserve#pr={n}"
        if n in holdout:
            old = holdout[n]
            historical["holdout"] = {
                key: old[key] for key in ("inspection_status", "source_errors",
                                          "unresolved_api_reconciliation")
            }
            historical["holdout"]["channels"] = {
                name: {k: v for k, v in channel.items()
                       if k in ("count", "total", "pages", "reconciled")}
                for name, channel in old["channels"].items()
            }
            historical["holdout_source_ids"] = [f"holdout-ledger#pr={n}", f"holdout-{n}"]
        rows.append(dict(
            member, schema_version=1,
            scope={"decision": "pending", "reason": decision["reason"],
                   "source_ids": [f"decisions#pr={n}"]},
            family=decision["family"], split=decision["split"],
            source_status="pending", inspection_status="pending", receipt=None,
            rule_links=[], historical=historical,
        ))
    return rows


def corpus_numbers(text):
    tables = re.findall(
        r"^<!-- BEGIN INVENTORY -->\r?\n(.*?)^<!-- END INVENTORY -->\r?$",
        text, re.MULTILINE | re.DOTALL,
    )
    require(len(tables) == 1, "corpus: expected one delimited inventory table")
    numbers = [int(n) for n in re.findall(r"^\| \[PR-(\d+)\]", tables[0], re.MULTILINE)]
    require(numbers, "corpus: empty inventory")
    return numbers


def recover(source_root, repository):
    sources = {}

    def original(name):
        return read_source(source_root, f"{ORIGINAL}\\{name}.json", name, sources)

    snapshot = original("snapshot")
    require((snapshot["repository"], snapshot["start"], snapshot["cutoff"]) ==
            (REPOSITORY, START, CUTOFF), "snapshot: wrong repository or historical bounds")
    ledger = original("discovery-ledger")
    inventory = original("inventory-raw")
    decisions = original("decisions")
    counts = original("validation-counts")
    queries = []
    require([q["id"] for q in ledger] == [f"Q{n:02}" for n in range(1, 30)],
            "discovery-ledger: expected original Q01-Q29")
    for i, entry in enumerate(ledger):
        matches = list(cache_path(source_root, ORIGINAL).glob(f"search-{i:02}-*.json"))
        require(len(matches) == 1, f"{entry['id']}: expected one saved query receipt")
        identifier = f"{ORIGINAL}\\{matches[0].name}"
        url = "https://api.github.com/search/issues?" + urlencode(
            {"q": entry["query"], "per_page": 100})
        queries.append((entry, read_source(source_root, identifier, entry["id"], sources, url)))
    corpus = read_text(repository / "docs" / "performance-improvement-review" / "corpus.md")
    numbers = corpus_numbers(corpus)
    members, queries = reconcile(inventory, decisions, queries, numbers)
    reserve = unique_by(read_source(source_root, f"{HOLDOUT}\\reserve.json",
                                   "reserve", sources), "number", "reserve")
    holdout = unique_by(read_source(source_root, f"{HOLDOUT}\\per-pr-ledger.json",
                                   "holdout-ledger", sources), "pr", "holdout-ledger")
    require(set(reserve) == set(holdout) <= set(numbers), "holdout: saved member sets disagree")
    for n in holdout:
        receipt = read_source(source_root, f"{HOLDOUT}\\raw\\{n}\\ledger.json",
                              f"holdout-{n}", sources, f"https://github.com/{REPOSITORY}/pull/{n}")
        require(receipt["number"] == n, f"PR {n}/holdout: wrong receipt identity")
    development = unique_by(counts["development"], "number", "validation-counts")
    require(set(development) <= set(numbers), "validation-counts: unknown PR")
    frozen = []
    for name in FROZEN_FILES:
        path = repository.joinpath(*name.split("\\"))
        frozen.append({
            "path": name, "sha256": hashlib.sha256(frozen_file_bytes(path)).hexdigest(),
            "url": f"https://github.com/{REPOSITORY}/blob/{STARTING_HEAD}/" + name.replace("\\", "/"),
        })
    discovery = {
        "schema_version": 1, "repository": REPOSITORY, "start": START, "cutoff": CUTOFF,
        "original_head": snapshot["checkout"],
        "continuation": {"branch": "idea/dm-943", "head": STARTING_HEAD},
        "final_corpus_complete": False,
        "sources": sources, "queries": queries,
        "linked": [m["pr"] for m in members if m["discovery_ids"] == ["linked"]],
        "counts": {"inventory": len(members), "search_occurrences": sum(q["total"] for q in queries),
                   "search_unique": len({n for q in queries for n in q["members"]}),
                   "linked": sum(m["discovery_ids"] == ["linked"] for m in members)},
        "historical_counts": {k: v for k, v in counts.items()
                              if k not in ("development", "search_checks")},
        "members": members, "frozen_files": frozen,
    }
    rows = make_index(members, decisions, development, reserve, holdout)
    validate_inventory(discovery, rows)
    return discovery, rows


def save_outputs(output, files):
    output = Path(output)
    for name, content in files.items():
        path = output / name
        require(not path.exists() or path.read_bytes().replace(b"\r\n", b"\n") == content.encode("utf-8"),
                f"{path}: refusing to overwrite existing edits")
    output.mkdir(parents=True, exist_ok=True)
    for name, content in files.items():
        path = output / name
        if not path.exists():
            with path.open("x", encoding="utf-8", newline="\n") as stream:
                stream.write(content)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source-root", type=Path, required=True)
    parser.add_argument("--output", type=Path, default=ROOT)
    args = parser.parse_args(argv)
    try:
        discovery, rows = recover(args.source_root, ROOT.parents[2])
        require(digest(discovery) == SEALED_SHA256, "discovery seal mismatch; saved sources changed")
        save_outputs(args.output, {
            "discovery.json": json.dumps(discovery, indent=2, ensure_ascii=True) + "\n",
            "work-index.jsonl": "".join(json.dumps(row, ensure_ascii=True) + "\n" for row in rows),
        })
        print(json.dumps({"inventory": len(rows), "network_requests": 0,
                          "final_corpus_complete": False}))
        return 0
    except (ValidationError, OSError, UnicodeError, KeyError, TypeError) as error:
        print(f"collection failed: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
