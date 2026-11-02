import copy
from contextlib import redirect_stderr, redirect_stdout
import hashlib
import io
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

import collect
import validate
from test_validate import fixture, write_json


class CollectionTests(unittest.TestCase):
    def inputs(self):
        discovery, _ = fixture()
        inventory = [
            {"number": m["pr"], "node_id": m["node_id"], "html_url": m["url"],
             "repository_url": "https://api.github.com/repos/dotnet/fsharp",
             "user": {"login": m["author"]}, "queries": ["Q01"]}
            for m in discovery["members"]
        ]
        decisions = [
            {"number": r["number"], "author": r["user"]["login"],
             "family": "FAMILY-test", "split": "holdout", "included": True,
             "tags": "performance", "reason": "Prior decision", "queries": r["queries"]}
            for r in inventory
        ]
        inventory[-1]["queries"] = decisions[-1]["queries"] = ["linked"]
        queries = [
            (dict(discovery["queries"][0], total=2, collected=2),
             [{"total_count": 2, "incomplete_results": False,
               "items": copy.deepcopy(inventory[:-1])}])
        ]
        return inventory, decisions, queries, [1, 2, 3]

    def test_reconcile_preserves_authors_and_linked_membership(self):
        inventory, decisions, queries, numbers = self.inputs()
        members, reconciled = collect.reconcile(inventory, decisions, queries, numbers)
        self.assertEqual([1, 2, 3], [m["pr"] for m in members])
        self.assertEqual("Copilot", members[0]["author"])
        self.assertEqual(["linked"], members[-1]["discovery_ids"])
        self.assertEqual([1, 2], reconciled[0]["members"])
        rows = collect.make_index(members, decisions, {}, {}, {})
        for row in rows:
            self.assertEqual("pending", row["scope"]["decision"])
            self.assertEqual("pending", row["source_status"])
            self.assertEqual("pending", row["inspection_status"])
            self.assertIsNone(row["receipt"])
            self.assertEqual([], row["rule_links"])
            self.assertTrue(row["historical"]["decision"]["included"])

    def test_original_inventory_ignores_marker_mentions_in_prose(self):
        table = "<!-- BEGIN INVENTORY -->\n| [PR-1](url) | author |\n<!-- END INVENTORY -->"
        mention = "\nExample: '(?s)<!-- BEGIN INVENTORY -->(.*?)<!-- END INVENTORY -->'\n"
        self.assertEqual([1], collect.corpus_numbers(table + mention))
        for text in ("", mention, table + "\n" + table):
            with self.subTest(text=text):
                with self.assertRaises(validate.ValidationError):
                    collect.corpus_numbers(text)

    def test_reconciliation_mutations(self):
        mutations = {
            "lost inventory": lambda i, d, q, n: i.pop(),
            "duplicate inventory": lambda i, d, q, n: i.append(i[0]),
            "lost decision": lambda i, d, q, n: d.pop(),
            "duplicate decision": lambda i, d, q, n: d.append(d[0]),
            "original corpus substitution": lambda i, d, q, n: n.__setitem__(0, 99),
            "decision membership": lambda i, d, q, n: d[0].update(queries=["linked"]),
            "author": lambda i, d, q, n: d[0].update(author="T-Gro"),
            "search node": lambda i, d, q, n: q[0][1][0]["items"][0].update(node_id="fake"),
            "search repository": lambda i, d, q, n: q[0][1][0]["items"][0].update(
                repository_url="https://api.github.com/repos/other/repo"),
            "truncated pages": lambda i, d, q, n: q[0][1][0]["items"].pop(),
            "incomplete": lambda i, d, q, n: q[0][1][0].update(incomplete_results=True),
            "duplicate search": lambda i, d, q, n: q[0][1][0]["items"].__setitem__(
                0, q[0][1][0]["items"][1]),
        }
        for name, mutate in mutations.items():
            with self.subTest(name=name):
                args = self.inputs()
                mutate(*args)
                with self.assertRaises(validate.ValidationError):
                    collect.reconcile(*args)

    def test_cache_read_is_relative_hashed_and_never_downloaded(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            write_json(root / "receipt.json", {"prior": "acquired"})
            sources = {}
            result = collect.read_source(root, "receipt.json", "receipt", sources)
            self.assertEqual({"prior": "acquired"}, result)
            before = (root / "receipt.json").read_bytes()
            collect.read_source(root, "receipt.json", "receipt", sources)
            self.assertEqual(before, (root / "receipt.json").read_bytes())
            self.assertEqual("receipt.json", sources["receipt"]["cache_path"])
            for path in ("missing.json", "..\\outside.json", "C:\\private.json"):
                with self.subTest(path=path):
                    with self.assertRaises(validate.ValidationError):
                        collect.read_source(root, path, "bad", sources)
            (root / "broken.json").write_text("{", encoding="utf-8")
            with self.assertRaises(validate.ValidationError):
                collect.read_source(root, "broken.json", "bad", sources)

    def test_historical_inspection_is_not_promoted(self):
        members, _ = collect.reconcile(*self.inputs())
        decisions = self.inputs()[1]
        holdout = {1: {"pr": 1, "inspection_status": "focused selected-case inspection",
                       "channels": {"reviews": {"count": 3, "reconciled": True}},
                       "source_errors": [], "unresolved_api_reconciliation": []}}
        rows = collect.make_index(members, decisions, {1: {"reviews": 2}}, {}, holdout)
        self.assertEqual("pending", rows[0]["inspection_status"])
        self.assertEqual("pending", rows[0]["source_status"])
        self.assertIsNone(rows[0]["receipt"])
        self.assertEqual(holdout[1]["inspection_status"],
                         rows[0]["historical"]["holdout"]["inspection_status"])

    def test_write_reuses_identical_outputs_but_never_discards_edits(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            collect.save_outputs(root, {"discovery.json": "frozen\n", "work-index.jsonl": "pending\n"})
            (root / "discovery.json").write_bytes(b"frozen\r\n")
            collect.save_outputs(root, {"discovery.json": "frozen\n", "work-index.jsonl": "pending\n"})
            self.assertEqual(b"frozen\r\n", (root / "discovery.json").read_bytes())
            with self.assertRaisesRegex(validate.ValidationError, "overwrite"):
                collect.save_outputs(root, {"discovery.json": "changed\n", "new.json": "new\n"})
            self.assertEqual("frozen\n", (root / "discovery.json").read_text())
            self.assertFalse((root / "new.json").exists())

    def archive(self, root):
        repository = root / "repository"
        sources = root / "sources"
        inventory, decisions, _, numbers = self.inputs()
        inventory[1]["queries"] = decisions[1]["queries"] = ["Q01", "Q02"]
        ledger = []
        files = {}
        for i in range(29):
            items = inventory[:2] if i == 0 else inventory[1:2] if i == 1 else []
            pages = [items[:1], items[1:]] if i == 0 else [items]
            ledger.append({"id": f"Q{i + 1:02}", "query": f"repo:dotnet/fsharp test-{i}",
                           "total": len(items), "collected": len(items),
                           "pages": len(pages), "incomplete_results": False})
            files[f"{collect.ORIGINAL}\\search-{i:02}-test.json"] = [
                {"total_count": len(items), "incomplete_results": False, "items": page}
                for page in pages
            ]
        for name, value in {
            "snapshot": {"repository": validate.REPOSITORY, "start": validate.START,
                         "cutoff": validate.CUTOFF, "checkout": "a" * 40},
            "discovery-ledger": ledger, "inventory-raw": inventory, "decisions": decisions,
            "validation-counts": {"inventory": 3, "development": [{"number": 2, "reviews": 4}]},
        }.items():
            files[f"{collect.ORIGINAL}\\{name}.json"] = value
        files[f"{collect.HOLDOUT}\\reserve.json"] = [{"number": 3, "family": "FAMILY-test"}]
        files[f"{collect.HOLDOUT}\\per-pr-ledger.json"] = [
            {"pr": 3, "inspection_status": "focused selected-case inspection",
             "source_errors": [], "unresolved_api_reconciliation": [],
             "channels": {"reviews": {"count": 2, "pages": 1, "reconciled": True}}}
        ]
        files[f"{collect.HOLDOUT}\\raw\\3\\ledger.json"] = {"number": 3}
        for identifier, value in files.items():
            path = validate.cache_path(sources, identifier)
            path.parent.mkdir(parents=True, exist_ok=True)
            write_json(path, value)
        for name in collect.FROZEN_FILES:
            path = validate.cache_path(repository, name)
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_bytes(b"preserved\r\n")
        release_note = repository / "docs" / "release-notes" / ".FSharp.Compiler.Service" / "11.0.200.md"
        release_note.write_bytes(
            b"### Added\r\n\r\n"
            b"* Preserved reviewer ([PR #20619](https://github.com/dotnet/fsharp/pull/20619))\r\n"
        )
        corpus = repository / "docs" / "performance-improvement-review" / "corpus.md"
        corpus.write_text("<!-- BEGIN INVENTORY -->\n" + "".join(
            f"| [PR-{n}](url) | author |\n" for n in reversed(numbers)
        ) + "<!-- END INVENTORY -->\n", encoding="utf-8")
        return sources, repository

    def collector_cli(self, sources, repository, seal, output):
        stdout, stderr = io.StringIO(), io.StringIO()
        mining = repository / "docs" / "performance-improvement-review" / "mining"
        with patch.object(collect, "ROOT", mining), patch.object(collect, "SEALED_SHA256", seal):
            with redirect_stdout(stdout), redirect_stderr(stderr):
                code = collect.main(["--source-root", str(sources), "--output", str(output)])
        return code, stdout.getvalue(), stderr.getvalue()

    def test_recover_and_cli_reuse_complete_temporary_archive(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            sources, repository = self.archive(root)
            before = {p: (p.read_bytes(), p.stat().st_mtime_ns)
                      for p in root.rglob("*") if p.is_file()}
            discovery, rows = collect.recover(sources, repository)
            self.assertEqual({"inventory": 3, "search_occurrences": 3,
                              "search_unique": 2, "linked": 1}, discovery["counts"])
            self.assertEqual(("a" * 40, validate.START, validate.CUTOFF),
                             (discovery["original_head"], discovery["start"], discovery["cutoff"]))
            self.assertEqual([f"Q{i:02}" for i in range(1, 30)],
                             [q["id"] for q in discovery["queries"]])
            self.assertEqual([[1, 2], [2]] + [[]] * 27,
                             [q["members"] for q in discovery["queries"]])
            self.assertEqual([3], discovery["linked"])
            self.assertEqual([1, 2, 3], [r["pr"] for r in rows])
            self.assertEqual("Copilot", rows[0]["author"])
            self.assertEqual(["Q01", "Q02"], rows[1]["discovery_ids"])
            self.assertEqual(4, rows[1]["historical"]["development_counts"]["reviews"])
            self.assertEqual(3, rows[2]["historical"]["reserve"]["number"])
            self.assertEqual(2, rows[2]["historical"]["holdout"]["channels"]["reviews"]["count"])
            self.assertEqual("focused selected-case inspection",
                             rows[2]["historical"]["holdout"]["inspection_status"])
            self.assertEqual({"inventory": 3}, discovery["historical_counts"])
            self.assertEqual(37, len(discovery["sources"]))
            for source in discovery["sources"].values():
                self.assertEqual(hashlib.sha256(
                    validate.cache_path(sources, source["cache_path"]).read_bytes()
                ).hexdigest(), source["sha256"])
            for frozen in discovery["frozen_files"]:
                self.assertEqual(hashlib.sha256(
                    validate.cache_path(repository, frozen["path"]).read_bytes()
                    .replace(b"\r\n", b"\n")
                ).hexdigest(), frozen["sha256"])
            for row in rows:
                self.assertEqual(("pending", "pending", "pending", None, []),
                                 (row["scope"]["decision"], row["source_status"],
                                  row["inspection_status"], row["receipt"], row["rule_links"]))
            output = root / "output"
            for _ in range(2):
                code, stdout, stderr = self.collector_cli(
                    sources, repository, validate.digest(discovery), output)
                self.assertEqual((0, ""), (code, stderr))
                self.assertEqual({"inventory": 3, "network_requests": 0,
                                  "final_corpus_complete": False}, json.loads(stdout))
                self.assertEqual(discovery, validate.load_discovery(
                    output / "discovery.json", expected_sha256=validate.digest(discovery)))
                self.assertEqual(rows, validate.load_index(output / "work-index.jsonl"))
                for path, state in before.items():
                    self.assertEqual(state, (path.read_bytes(), path.stat().st_mtime_ns))
                before.update({p: (p.read_bytes(), p.stat().st_mtime_ns) for p in output.iterdir()})

    def test_recover_and_cli_reject_invalid_temporary_archives(self):
        cases = (
            ("snapshot.json", None, "snapshot"),
            ("snapshot.json", "{", "snapshot"),
            ("snapshot.json", '{"repository":"other/repo","start":"","cutoff":""}', "snapshot"),
            ("discovery-ledger.json", "[]", "Q01-Q29"),
            ("search-00-test.json", None, "Q01"),
            ("search-00-duplicate.json", "[]", "Q01"),
            ("decisions.json", "[]", "identities differ"),
            ("validation-counts.json", '{"development":[{"number":99}]}', "unknown PR"),
            (f"{collect.HOLDOUT}\\reserve.json", "[]", "holdout"),
            (f"{collect.HOLDOUT}\\raw\\3\\ledger.json", '{"number":99}', "PR 3/holdout"),
        )
        for identifier, content, diagnostic in cases:
            with self.subTest(source=identifier, content=content), tempfile.TemporaryDirectory() as temp:
                root = Path(temp)
                sources, repository = self.archive(root)
                seal = validate.digest(collect.recover(sources, repository)[0])
                path = validate.cache_path(
                    sources, identifier if "\\" in identifier else f"{collect.ORIGINAL}\\{identifier}")
                if content is None:
                    path.unlink()
                else:
                    path.write_text(content, encoding="utf-8")
                with self.assertRaisesRegex(validate.ValidationError, diagnostic):
                    collect.recover(sources, repository)
                code, stdout, stderr = self.collector_cli(sources, repository, seal, root / "output")
                self.assertEqual((1, ""), (code, stdout))
                self.assertIn(diagnostic, stderr)
                self.assertNotIn("Traceback", stderr)
                self.assertFalse((root / "output").exists())

    def test_cli_rejects_changed_source_seal_before_writing(self):
        for existing_output in (False, True):
            with self.subTest(existing_output=existing_output), tempfile.TemporaryDirectory() as temp:
                root = Path(temp)
                sources, repository = self.archive(root)
                seal = validate.digest(collect.recover(sources, repository)[0])
                output = root / "output"
                if existing_output:
                    self.assertEqual(0, self.collector_cli(sources, repository, seal, output)[0])
                snapshot = validate.cache_path(sources, f"{collect.ORIGINAL}\\snapshot.json")
                snapshot.write_bytes(snapshot.read_bytes() + b"\n")
                self.assertNotEqual(seal, validate.digest(collect.recover(sources, repository)[0]))
                before = {p: (p.read_bytes(), p.stat().st_mtime_ns)
                          for p in root.rglob("*") if p.is_file()}
                code, stdout, stderr = self.collector_cli(sources, repository, seal, output)
                self.assertEqual((1, ""), (code, stdout))
                self.assertIn("discovery seal mismatch; saved sources changed", stderr)
                self.assertEqual(existing_output, output.exists())
                self.assertEqual(before, {p: (p.read_bytes(), p.stat().st_mtime_ns)
                                          for p in root.rglob("*") if p.is_file()})


if __name__ == "__main__":
    unittest.main()
