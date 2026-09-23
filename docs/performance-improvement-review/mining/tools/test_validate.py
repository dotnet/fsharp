import copy
import hashlib
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

import validate


def fixture(size=3):
    members = [
        {
            "repository": validate.REPOSITORY,
            "pr": n,
            "node_id": f"PR_{n}",
            "url": f"https://github.com/dotnet/fsharp/pull/{n}",
            "author": "Copilot" if n == 1 else "T-Gro",
            "discovery_ids": ["Q01"],
            "ordinal": n,
            "shard": (n - 1) // 25 + 1,
        }
        for n in range(1, size + 1)
    ]
    discovery = {
        "schema_version": 1,
        "repository": validate.REPOSITORY,
        "start": validate.START,
        "cutoff": validate.CUTOFF,
        "original_head": "a" * 40,
        "continuation": {"branch": "idea/dm-943", "head": validate.STARTING_HEAD},
        "final_corpus_complete": False,
        "sources": {
            "decisions": {"cache_path": "archive\\decisions.json", "sha256": "a" * 64},
            "Q01": {"cache_path": "archive\\search.json", "sha256": "b" * 64},
        },
        "queries": [
            {"id": "Q01", "query": "frozen query", "total": size, "collected": size,
             "pages": 1, "incomplete_results": False, "members": list(range(1, size + 1))}
        ],
        "linked": [],
        "counts": {"inventory": size, "search_occurrences": size,
                   "search_unique": size, "linked": 0},
        "members": members,
        "frozen_files": [],
    }
    rows = [
        dict(m, schema_version=1,
             scope={"decision": "pending", "reason": "Historical decision only",
                    "source_ids": [f"decisions#pr={m['pr']}"]},
             family=f"FAMILY-{m['pr']}", split="development",
             source_status="pending", inspection_status="pending",
             receipt=None, rule_links=[], historical={})
        for m in members
    ]
    return discovery, rows


def write_json(path, data):
    path.write_text(json.dumps(data), encoding="utf-8")


def write_index(path, rows):
    path.write_text("".join(json.dumps(r) + "\n" for r in rows), encoding="utf-8")


class InventoryTests(unittest.TestCase):
    def test_frozen_fixture_passes_without_completion_credit(self):
        discovery, rows = fixture()
        self.assertIsNone(validate.validate_inventory(discovery, rows))
        for status in ("pending", "failed", "stale", "acquired"):
            with self.subTest(source_status=status):
                rows[0]["source_status"] = status
                validate.validate_inventory(discovery, rows)

    def test_identity_and_schema_mutations(self):
        discovery, original = fixture(485)
        mutations = {
            "484 rows": lambda r: r.pop(),
            "486 rows": lambda r: r.append(dict(r[-1], pr=486, node_id="PR_486")),
            "substitution": lambda r: r.__setitem__(0, dict(r[0], pr=999, node_id="PR_999")),
            "duplicate PR": lambda r: r[0].update(pr=r[1]["pr"]),
            "duplicate node": lambda r: r[0].update(node_id=r[1]["node_id"]),
            "wrong node": lambda r: r[0].update(node_id="PR_other"),
            "repository": lambda r: r[0].update(repository="other/repo"),
            "URL": lambda r: r[0].update(url="https://github.com/other/repo/pull/1"),
            "schema": lambda r: r[0].update(schema_version=2),
            "missing membership": lambda r: r[0].update(discovery_ids=[]),
            "extra membership": lambda r: r[0].update(discovery_ids=["Q01", "Q02"]),
            "duplicate membership": lambda r: r[0].update(discovery_ids=["Q01", "Q01"]),
            "author rewrite": lambda r: r[0].update(author="T-Gro"),
            "ordinal": lambda r: r[0].update(ordinal=2),
            "shard": lambda r: r[0].update(shard=2),
            "order": lambda r: r.reverse(),
            "complete flag": lambda r: r[0].update(complete=True),
            "final complete flag": lambda r: r[0].update(final_corpus_complete=True),
            "unknown status": lambda r: r[0].update(source_status="complete"),
            "acquisition is not inspection": lambda r: r[0].update(inspection_status="acquired"),
            "missing scope": lambda r: r[0].pop("scope"),
            "missing receipt field": lambda r: r[0].pop("receipt"),
            "unknown source": lambda r: r[0]["scope"].update(source_ids=["invented"]),
            "non-object scope": lambda r: r[0].update(scope=[]),
            "boolean PR": lambda r: r[0].update(pr=True),
        }
        for name, mutate in mutations.items():
            with self.subTest(name=name):
                rows = copy.deepcopy(original)
                mutate(rows)
                with self.assertRaises(validate.ValidationError):
                    validate.validate_inventory(discovery, rows)

    def test_manifest_mutations(self):
        original, rows = fixture()
        mutations = {
            "cutoff": lambda d: d.update(cutoff="2026-09-23T00:00:00Z"),
            "start": lambda d: d.update(start="2026-04-21T00:00:00Z"),
            "repository": lambda d: d.update(repository="other/repo"),
            "schema": lambda d: d.update(schema_version=2),
            "completion": lambda d: d.update(final_corpus_complete=True),
            "fabricated complete flag": lambda d: d.update(complete=True),
            "missing sources": lambda d: d.pop("sources"),
            "duplicate PR": lambda d: d["members"][0].update(pr=2),
            "duplicate node": lambda d: d["members"][0].update(node_id="PR_2"),
            "missing query member": lambda d: d["queries"][0]["members"].pop(),
            "query count": lambda d: d["queries"][0].update(total=4),
            "incomplete query": lambda d: d["queries"][0].update(incomplete_results=True),
        }
        for name, mutate in mutations.items():
            with self.subTest(name=name):
                discovery = copy.deepcopy(original)
                mutate(discovery)
                with self.assertRaises(validate.ValidationError):
                    validate.validate_inventory(discovery, rows)

    def test_loaders_and_sealed_digest(self):
        discovery, rows = fixture()
        with tempfile.TemporaryDirectory() as temp:
            path = Path(temp) / "discovery.json"
            index = Path(temp) / "work-index.jsonl"
            write_json(path, discovery)
            write_index(index, rows)
            digest = validate.digest(discovery)
            self.assertEqual(discovery, validate.load_discovery(path, expected_sha256=digest))
            self.assertEqual(rows, validate.load_index(index))
            with self.assertRaisesRegex(validate.ValidationError, "seal"):
                validate.load_discovery(path)
            discovery["queries"][0]["query"] = "new search"
            write_json(path, discovery)
            with self.assertRaisesRegex(validate.ValidationError, "seal"):
                validate.load_discovery(path, expected_sha256=digest)
            for content in ('{bad json', '{"pr": 1, "pr": 2}', '[]', '\n'):
                with self.subTest(content=content):
                    index.write_text(content, encoding="utf-8")
                    with self.assertRaises(validate.ValidationError):
                        validate.load_index(index)
            with self.assertRaisesRegex(validate.ValidationError, "missing.json"):
                validate.load_index(Path(temp) / "missing.json")


class ProductionTests(unittest.TestCase):
    root = Path(__file__).resolve().parents[1]

    def cli(self, *args):
        return subprocess.run(
            [sys.executable, "-O", str(Path(validate.__file__)), *args],
            capture_output=True, text=True,
        )

    def test_live_inventory_and_fail_closed_stages(self):
        discovery = validate.load_discovery(self.root / "discovery.json")
        rows = validate.load_index(self.root / "work-index.jsonl")
        self.assertEqual(485, len(rows))
        self.assertTrue(all(r["inspection_status"] == "pending" for r in rows))
        validate.validate_inventory(discovery, rows)
        result = self.cli("--stage", "inventory")
        self.assertEqual(0, result.returncode, result.stderr)
        self.assertIs(False, json.loads(result.stdout)["final_corpus_complete"])
        for stage in ("scope", "red", "acquisition", "batch", "training",
                      "rules", "audit", "sealed", "complete"):
            with self.subTest(stage=stage):
                args = ["--stage", stage]
                if stage == "batch":
                    args += ["--batch", "1"]
                result = self.cli(*args)
                self.assertNotEqual(0, result.returncode)
                self.assertIn("not implemented", result.stderr)
                self.assertIn("final corpus completion is false", result.stderr)
                if stage == "complete":
                    self.assertIn("485", result.stderr)
                    self.assertIn("pending", result.stderr)

    def test_cli_cannot_accept_an_injected_manifest_or_bypass_inventory(self):
        discovery, rows = fixture()
        with tempfile.TemporaryDirectory() as temp:
            path = Path(temp) / "discovery.json"
            index = Path(temp) / "index.jsonl"
            write_json(path, discovery)
            write_index(index, rows)
            result = self.cli("--discovery", str(path), "--index", str(index),
                              "--stage", "inventory")
            self.assertNotEqual(0, result.returncode)
            self.assertIn("seal", result.stderr)
            live = validate.load_index(self.root / "work-index.jsonl")
            for mutate in (
                lambda r: r.pop(),
                lambda r: r.append(dict(r[-1], pr=99999, node_id="PR_fake")),
                lambda r: r.__setitem__(0, dict(r[0], pr=99999, node_id="PR_fake")),
                lambda r: r[0].update(complete=True),
            ):
                changed = copy.deepcopy(live)
                mutate(changed)
                write_index(index, changed)
                result = self.cli("--index", str(index), "--stage", "inventory")
                self.assertNotEqual(0, result.returncode)
                self.assertNotIn("Traceback", result.stderr)
            for row in live:
                row.update(source_status="acquired", inspection_status="inspected",
                           receipt=f"unverified\\{row['pr']}.json")
            write_index(index, live)
            result = self.cli("--index", str(index), "--stage", "complete")
            self.assertNotEqual(0, result.returncode)
            self.assertIn("not implemented", result.stderr)
            self.assertIn("final corpus completion is false", result.stderr)
        for args in (["--stage", "batch"], ["--stage", "batch", "--batch", "0"],
                     ["--stage", "inventory", "--batch", "1"],
                     ["--stage", "unknown"]):
            with self.subTest(args=args):
                self.assertNotEqual(0, self.cli(*args).returncode)

    def test_frozen_reviewer_and_evidence_unchanged(self):
        discovery = validate.load_discovery(self.root / "discovery.json")
        repository = self.root.parents[2]
        for entry in discovery["frozen_files"]:
            with self.subTest(path=entry["path"]):
                self.assertEqual(
                    entry["sha256"],
                    hashlib.sha256(validate.cache_path(repository, entry["path"]).read_bytes()
                                   .replace(b"\r\n", b"\n")).hexdigest(),
                )


if __name__ == "__main__":
    unittest.main()
