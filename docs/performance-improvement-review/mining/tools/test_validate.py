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


class FrozenFilesTests(unittest.TestCase):
    def test_release_note_preserves_the_entry_without_freezing_unrelated_additions(self):
        original = b"### Added\n\n* Frozen reviewer ([PR #20619](https://github.com/dotnet/fsharp/pull/20619))\n"
        additions = b"* Another feature ([PR #1](https://github.com/dotnet/fsharp/pull/1))\n\n### Fixed\n\n* A fix\n"
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "release-notes" / ".FSharp.Compiler.Service" / "11.0.200.md"
            path.parent.mkdir(parents=True)
            for newline in (b"\n", b"\r\n"):
                raw = (original + additions).replace(b"\n", newline)
                path.write_bytes(raw)
                self.assertEqual(validate.frozen_file_bytes(path), original)
                self.assertEqual(path.read_bytes(), raw)
            path.write_bytes(original.replace(b"Frozen reviewer", b"Changed reviewer") + additions)
            self.assertNotEqual(hashlib.sha256(validate.frozen_file_bytes(path)).digest(),
                                hashlib.sha256(original).digest())

    def test_release_note_rejects_missing_duplicate_or_reclassified_entries(self):
        entry = b"* Frozen reviewer ([PR #20619](https://github.com/dotnet/fsharp/pull/20619))\n"
        invalid = [
            b"### Added\n\n* Other feature\n",
            b"### Added\n\n" + entry + entry,
            b"### Added\n\n" + entry + b"\n### Added\n",
            b"### Added\n\n* Other feature\n\n### Fixed\n\n" + entry,
            b"### Added\n" + entry,
        ]
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "release-notes" / ".FSharp.Compiler.Service" / "11.0.200.md"
            path.parent.mkdir(parents=True)
            for raw in invalid:
                with self.subTest(raw=raw), self.assertRaises(validate.ValidationError):
                    path.write_bytes(raw)
                    validate.frozen_file_bytes(path)

    def test_other_release_note_files_still_freeze_all_bytes(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "release-notes" / ".FSharp.Core" / "11.0.200.md"
            path.parent.mkdir(parents=True)
            raw = b"### Added\n\n* Another component\n"
            path.write_bytes(raw)
            self.assertEqual(validate.frozen_file_bytes(path), raw)


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


class ScopeTests(unittest.TestCase):
    def setUp(self):
        self.discovery = validate.load_discovery(validate.ROOT / "discovery.json")
        self.rows = validate.load_index(validate.ROOT / "work-index.jsonl")
        self.protocol = validate.parse_json(validate.read_text(
            validate.ROOT / "evaluation" / "protocol.json"), "protocol")
        self.exposure = validate.parse_json(validate.read_text(
            validate.ROOT / "evaluation" / "exposure.json"), "exposure")

    def test_scope_and_exposure_freeze(self):
        validate.validate_scope(self.discovery, self.rows, self.protocol)
        validate.validate_exposure(self.discovery, self.rows, self.exposure, self.protocol)
        shards = [{r["pr"] for r in self.rows if r["shard"] == s} for s in range(6)]
        self.assertEqual([81, 81, 81, 81, 81, 80], list(map(len, shards)))
        self.assertEqual({m["pr"] for m in self.discovery["members"]}, set.union(*shards))
        for i, shard in enumerate(shards):
            self.assertTrue(all(not shard & other for other in shards[i + 1:]))

    def test_scope_mutations_fail_closed(self):
        mutations = {
            "undecided": lambda r: r["scope"].update(decision="pending"),
            "blank": lambda r: r["scope"].update(reason=" "),
            "boilerplate": lambda r: r["scope"].update(reason="Performance benchmarks added"),
            "long boilerplate": lambda r: r["scope"].update(
                reason=f"PR {r['pr']}: " + "Performance benchmarks added. " * 4),
            "source lost": lambda r: r["scope"].update(source_ids=[]),
            "source copied": lambda r: r["scope"].update(source_ids=["pr-20524#pr=20524"]),
            "identity exclusion": lambda r: r["scope"].update(
                decision="out", basis="author", reason="Excluded solely because author is a bot."),
            "identity-only reason": lambda r: r["scope"].update(
                reason=f"PR {r['pr']}: Excluded solely because the author is a bot account.",
                decision="out", basis="no-performance-claim"),
            "updatedAt only": lambda r: r["scope"].update(
                decision="in", basis="mechanism",
                event={"kind": "updatedAt", "at": validate.CUTOFF,
                       "source_id": f"pr-{r['pr']}#pr={r['pr']}", "event_id": "updated_at"}),
            "updatedAt disguised": lambda r: r["scope"].update(
                decision="in", basis="mechanism",
                event={"kind": "other", "at": validate.CUTOFF,
                       "source_id": f"activity-{r['pr']}#pr={r['pr']}", "event_id": "updatedAt"}),
            "malformed hash": lambda r: r["scope"].update(description_sha256=12),
            "malformed reconciliation": lambda r: r["scope"].update(reconciliation=[]),
            "lost original decision": lambda r: r.update(historical={}),
            "shifted ordinal": lambda r: r.update(ordinal=1),
            "shifted shard": lambda r: r.update(shard=1),
            "copied identity": lambda r: r["scope"]["historical_identity"].update(node_id="PR_copied"),
        }
        for name, mutate in mutations.items():
            with self.subTest(name=name):
                rows = copy.deepcopy(self.rows)
                mutate(rows[0])
                discovery = copy.deepcopy(self.discovery)
                discovery["scope_freeze"]["index_sha256"] = validate.digest(validate.scope_projection(rows))
                with self.assertRaises(validate.ValidationError):
                    validate.validate_scope(discovery, rows, self.protocol)

    def test_protocol_is_immutable_before_outputs(self):
        for section, key, value in (
            ("recurrence", "minimum_distinct_prs", 1),
            ("recurrence", "minimum_independent_families_for_generalization", 1),
            ("evaluation", "required_real_strata", []),
            ("authorship", "default", "verified-human"),
        ):
            with self.subTest(key=key):
                protocol = copy.deepcopy(self.protocol)
                protocol[section][key] = value
                with self.assertRaisesRegex(validate.ValidationError, "protocol"):
                    validate.validate_scope(self.discovery, self.rows, protocol)

    def test_family_renaming_does_not_erase_exposure(self):
        rows = copy.deepcopy(self.rows)
        rows[0].update(family="FAMILY-fresh-name", split="holdout")
        with self.assertRaises(validate.ValidationError):
            validate.validate_exposure(self.discovery, rows, self.exposure, self.protocol)

    def test_unknown_or_semantic_exposure_cannot_be_fresh(self):
        for kind in ("unknown", "substantive-inspection", "shared-oracle-access",
                     "reviewer-execution", "score-access", "rule-derivation"):
            with self.subTest(kind=kind):
                discovery, rows, exposure, family, record = self.metadata_only_holdout()
                validate.validate_exposure(discovery, rows, exposure, self.protocol)
                record["events"].append({
                    "kind": kind, "at": exposure["frozen_at"],
                    "source_ids": ["decisions"], "detail": f"Test access receipt: {kind}.",
                })
                family["exposure_kinds"] = sorted({e["kind"] for e in record["events"]})
                discovery["scope_freeze"]["exposure_sha256"] = validate.digest(exposure)
                with self.assertRaisesRegex(
                        validate.ValidationError,
                        f"^PR {record['pr']}/exposure: unknown or semantic exposure cannot be fresh$"):
                    validate.validate_exposure(discovery, rows, exposure, self.protocol)

    def test_central_package_management_variants_share_exposure_boundary(self):
        discovery = copy.deepcopy(self.discovery)
        rows, exposure = copy.deepcopy(self.rows), copy.deepcopy(self.exposure)
        family = next(f for f in exposure["families"] if 20083 in f["prs"])
        self.assertEqual([20083, 20084], family["prs"])
        self.assertEqual(["FAMILY-pr-20083", "FAMILY-pr-20084"], family["historical_aliases"])
        connection = next(e for e in exposure["connections"] if e["prs"] == [20083, 20084])
        self.assertTrue({"pr-20083#pr=20083", "pr-20084#pr=20084"} <= set(connection["source_ids"]))
        validate.validate_exposure(discovery, rows, exposure, self.protocol)

        family["prs"] = [20083]
        family["historical_aliases"] = ["FAMILY-pr-20083"]
        split_family = copy.deepcopy(family)
        split_family.update(id="FAMILY-pr-20084", prs=[20084],
                            historical_aliases=["FAMILY-pr-20084"])
        exposure["families"].append(split_family)
        next(r for r in rows if r["pr"] == 20084)["family"] = split_family["id"]
        next(r for r in exposure["prs"] if r["pr"] == 20084)["family"] = split_family["id"]
        for f in (family, split_family):
            f["exposure_kinds"] = sorted({e["kind"] for r in exposure["prs"]
                                         if r["pr"] in f["prs"] for e in r["events"]})
        discovery["scope_freeze"]["index_sha256"] = validate.digest(validate.scope_projection(rows))
        discovery["scope_freeze"]["exposure_sha256"] = validate.digest(exposure)
        validate.validate_scope(discovery, rows, self.protocol)
        with self.assertRaisesRegex(validate.ValidationError,
                                    "^exposure: connected/superseding variants split$"):
            validate.validate_exposure(discovery, rows, exposure, self.protocol)

    def test_freshness_requires_complete_audits(self):
        for audit in ("access_audit_complete", "connection_audit_complete"):
            with self.subTest(audit=audit):
                discovery, rows, exposure, _, record = self.metadata_only_holdout()
                validate.validate_exposure(discovery, rows, exposure, self.protocol)
                record[audit] = False
                discovery["scope_freeze"]["exposure_sha256"] = validate.digest(exposure)
                with self.assertRaisesRegex(
                        validate.ValidationError,
                        f"^PR {record['pr']}/exposure: freshness requires complete access and connection audits$"):
                    validate.validate_exposure(discovery, rows, exposure, self.protocol)

    def metadata_only_holdout(self):
        rows, exposure = copy.deepcopy(self.rows), copy.deepcopy(self.exposure)
        family = next(f for f in exposure["families"] if len(f["prs"]) == 1)
        n = family["prs"][0]
        record = next(r for r in exposure["prs"] if r["pr"] == n)
        record["events"] = [e for e in record["events"] if e["kind"] in
                            ("acquisition-only", "metadata-only-screening")]
        record.update(fresh=True, access_audit_complete=True, connection_audit_complete=True)
        family.update(fresh=True, split="holdout",
                      exposure_kinds=sorted({e["kind"] for e in record["events"]}))
        next(r for r in rows if r["pr"] == n)["split"] = "holdout"
        discovery = copy.deepcopy(self.discovery)
        discovery["scope_freeze"]["exposure_sha256"] = validate.digest(exposure)
        return discovery, rows, exposure, family, record

    def test_connected_family_cannot_straddle_splits(self):
        rows = copy.deepcopy(self.rows)
        family = next(f for f in self.exposure["families"] if len(f["prs"]) > 1)
        next(r for r in rows if r["pr"] == family["prs"][0])["split"] = "holdout"
        with self.assertRaises(validate.ValidationError):
            validate.validate_exposure(self.discovery, rows, self.exposure, self.protocol)

    def test_coordinated_family_rename_cannot_clear_history(self):
        rows, exposure = copy.deepcopy(self.rows), copy.deepcopy(self.exposure)
        family = next(f for f in exposure["families"] if len(f["prs"]) > 1)
        n = family["prs"].pop()
        row = next(r for r in rows if r["pr"] == n)
        record = next(r for r in exposure["prs"] if r["pr"] == n)
        row["family"] = record["family"] = "RENAMED-FRESH"
        exposure["families"].append({
            "id": "RENAMED-FRESH", "prs": [n], "split": row["split"],
            "historical_aliases": [row["historical"]["decision"]["family"]],
            "fresh": False, "exposure_kinds": sorted({e["kind"] for e in record["events"]}),
        })
        with self.assertRaises(validate.ValidationError):
            validate.validate_exposure(self.discovery, rows, exposure, self.protocol)

    def test_invalid_human_authorship_attestations(self):
        for name, source_index, rationale, error in (
            ("copied source", 1, "Explicit attestation for a different source.",
             "missing/copied human composition attestation"),
            ("account identity", 0, "The account is a GitHub User and replied to a review.",
             "account identity is not a composition attestation"),
        ):
            with self.subTest(name=name):
                discovery, exposure = copy.deepcopy(self.discovery), copy.deepcopy(self.exposure)
                record = exposure["prs"][0]
                source_pr = exposure["prs"][source_index]["pr"]
                subject = f"pr-{source_pr}:description"
                record["authorship"].update(
                    classification="verified-human",
                    positive_attestation={
                        "subject": subject, "enclosing_scope": subject,
                        "source_id": f"pr-{source_pr}#pr={source_pr}",
                        "rationale": rationale,
                    })
                discovery["scope_freeze"]["exposure_sha256"] = validate.digest(exposure)
                with self.assertRaisesRegex(validate.ValidationError,
                                            f"^PR {record['pr']}/exposure: {error}$"):
                    validate.validate_exposure(discovery, self.rows, exposure, self.protocol)


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
        result = self.cli("--stage", "scope")
        self.assertEqual(0, result.returncode, result.stderr)
        self.assertEqual("blocked", json.loads(result.stdout)["evaluation_acceptance"])
        for stage in ("red", "acquisition", "batch", "training",
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

    def test_scope_freeze_cannot_reseal_substituted_data(self):
        discovery = validate.load_discovery(self.root / "discovery.json")
        with tempfile.TemporaryDirectory() as temp:
            path = Path(temp) / "discovery.json"
            discovery["scope_freeze"]["protocol_sha256"] = "a" * 64
            write_json(path, discovery)
            result = self.cli("--discovery", str(path), "--stage", "scope")
            self.assertNotEqual(0, result.returncode)
            self.assertIn("scope freeze seal", result.stderr)

    def test_frozen_reviewer_and_evidence_unchanged(self):
        discovery = validate.load_discovery(self.root / "discovery.json")
        repository = self.root.parents[2]
        for entry in discovery["frozen_files"]:
            with self.subTest(path=entry["path"]):
                self.assertEqual(
                    entry["sha256"],
                    hashlib.sha256(validate.frozen_file_bytes(
                        validate.cache_path(repository, entry["path"]))).hexdigest(),
                )
        for source in discovery["scope_freeze"]["sources"].values():
            if "repository_path" in source:
                with self.subTest(path=source["repository_path"]):
                    self.assertEqual("CRLF-to-LF", source["hash_normalization"])
                    raw = validate.cache_path(repository, source["repository_path"]).read_bytes()
                    self.assertEqual(source["sha256"],
                                     hashlib.sha256(raw.replace(b"\r\n", b"\n")).hexdigest())


if __name__ == "__main__":
    unittest.main()
