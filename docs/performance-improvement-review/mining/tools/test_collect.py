import copy
from pathlib import Path
import tempfile
import unittest

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


if __name__ == "__main__":
    unittest.main()
