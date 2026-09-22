"""Local-only publication policy tests; all scratch files stay inside the checkout."""

import copy
from datetime import datetime, timedelta, timezone
import hashlib
import importlib
import json
import os
from pathlib import Path
import shutil
import stat
import sys
from types import SimpleNamespace
import unittest
from unittest import mock
import uuid

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "eng" / "scripts" / "compiler_perf"))
try:
    a = importlib.import_module("artifacts")
except ModuleNotFoundError:
    a = None

NOW = datetime(2026, 9, 20, 10, 0, tzinfo=timezone.utc)


class ArtifactTests(unittest.TestCase):
    def setUp(self):
        self.assertIsNotNone(a, "artifact staging helper has not been implemented")
        self.root = ROOT / (".artifact-test-" + uuid.uuid4().hex)
        self.root.mkdir()
        self.addCleanup(shutil.rmtree, self.root)
        self.private = self.root / "private"
        self.private.mkdir()
        self.public = self.root / "public"

    def entry(self, name="summary.json", kind="summary", content=None, conclusion="Bounded conclusion"):
        if content is None:
            content = {"schema_version": 1, "kind": "workflow-summary",
                       "status": "completed_no_finding", "conclusion": conclusion}
        data = json.dumps(content).encode() if isinstance(content, dict) else content
        path = self.private / name
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(data)
        return {"file": {"path": str(path), "sha256": hashlib.sha256(data).hexdigest(),
                         "size_bytes": len(data)}, "kind": kind, "conclusion": conclusion}

    def stage(self, entries, public=None):
        return a.stage_artifacts(self.private, public or self.public, entries, NOW)

    def test_fixed_production_policy_and_memory_schema(self):
        self.assertEqual(a.PUBLIC_BYTES, 512 * 1024**2)
        self.assertEqual(a.COMPACT_BYTES, 20 * 1024**2)
        self.assertEqual(a.RETENTION_DAYS, 14)
        result = self.stage([self.entry()])
        self.assertEqual(result["kind"], "artifact-manifest")
        self.assertEqual(result["retention_days"], 14)
        self.assertEqual(a.mem.timestamp(result["expires_at"]), NOW + timedelta(days=14))
        record = result["artifacts"][0]
        a.mem.validate_artifact(record)
        self.assertEqual(record["access"], "public")
        self.assertEqual(record["type"], "summary")
        self.assertEqual(record["availability"], "available")
        self.assertEqual(record["collected_at"], result["created_at"])
        self.assertEqual(json.loads((self.public / "manifest.json").read_text()), result)
        self.assertEqual(result["total_bytes"], sum(p.stat().st_size for p in self.public.iterdir()))

    def test_read_file_uses_platform_timestamp_semantics(self):
        entry = self.entry("heap.dmp", "dump", b"MDMPunchanged")
        path = Path(entry["file"]["path"])
        info = path.stat()
        fields = {key: getattr(info, key) for key in
                  ("st_dev", "st_ino", "st_size", "st_nlink", "st_mtime_ns", "st_ctime_ns")}
        fields["st_birthtime_ns"] = getattr(info, "st_birthtime_ns", info.st_ctime_ns)
        for platform, changed, accepted in (
                ("nt", None, True), ("nt", "st_ctime_ns", False),
                ("nt", "st_birthtime_ns", False), ("nt", "st_mtime_ns", False),
                ("nt", "st_ino", False), ("posix", None, True), ("posix", "st_ctime_ns", False)):
            with self.subTest(platform=platform, changed=changed):
                opened, after = SimpleNamespace(**fields), SimpleNamespace(**fields)
                if platform == "nt":
                    opened.st_ctime_ns += 1000
                    after.st_ctime_ns += 1000
                if changed:
                    setattr(after, changed, getattr(after, changed) + 100)
                system = SimpleNamespace(**dict(vars(os), name=platform))
                with mock.patch.object(a, "os", system), mock.patch.object(
                        system, "fstat", side_effect=[opened, after]):
                    if accepted:
                        digest, size, _, identity = a._read_file(path)
                        self.assertEqual((digest, size), (entry["file"]["sha256"], entry["file"]["size_bytes"]))
                        self.assertEqual(identity, a._identity(info))
                    else:
                        with self.assertRaisesRegex(ValueError, "Artifact changed while reading"):
                            a._read_file(path)

    def test_timestamp_preserving_rewrite_cannot_return_stale_hash_or_delete_dump(self):
        with mock.patch.object(a, "_outside_git"):
            owned = a.create_dump_workspace(self.root / "dump", [self.public])
        path = owned.path / "heap.dmp"
        fstat = os.fstat
        for operation in ("read", "remove"):
            with self.subTest(operation=operation):
                path.write_bytes(b"MDMPoriginal")
                receipt = a.claim_dump(owned, a.c.m.file_record(path))
                before = path.stat()
                calls = 0

                def rewrite_after_read(descriptor):
                    nonlocal calls
                    calls += 1
                    if calls == 2:
                        path.write_bytes(b"MDMPmodified")
                        os.utime(path, ns=(before.st_atime_ns, before.st_mtime_ns))
                    return fstat(descriptor)

                with mock.patch.object(a.os, "fstat", side_effect=rewrite_after_read):
                    with self.assertRaisesRegex(ValueError, "Artifact changed while reading"):
                        if operation == "read":
                            a._read_file(path)
                        else:
                            a.remove_owned_dump(owned, receipt, "Conclusion", NOW)
                self.assertEqual(calls, 2)
                self.assertEqual(path.read_bytes(), b"MDMPmodified")
                self.assertEqual(path.stat().st_mtime_ns, before.st_mtime_ns)
                self.assertEqual(a._identity(path.stat()), a._identity(before))

    def test_output_is_deterministic_and_only_explicit_files_are_staged(self):
        first = self.entry("first.json", conclusion="First")
        second = self.entry("second.json", conclusion="Second")
        (self.private / "credentials.env").write_text("PASSWORD=must-not-publish")
        left = self.stage([first, second])
        other = self.root / "other"
        right = self.stage([second, first], other)
        self.assertEqual(left, right)
        self.assertEqual({p.name: p.read_bytes() for p in self.public.iterdir()},
                         {p.name: p.read_bytes() for p in other.iterdir()})
        self.assertEqual(len(list(other.iterdir())), 3)

    def test_reuses_independent_sanitization_for_summary_and_conclusion(self):
        text = f"password=hunter2 Bearer secret-bearer github_pat_abcdef {self.private} top-secret-value"
        with mock.patch.dict(os.environ, {"TEST_API_TOKEN": "top-secret-value"}):
            result = self.stage([self.entry(conclusion=text)])
        text = "".join(p.read_text() for p in self.public.iterdir())
        for forbidden in ("hunter2", "secret-bearer", "github_pat_abcdef", str(self.private), "top-secret-value"):
            self.assertNotIn(forbidden, text)
        self.assertIn("[REDACTED]", text)
        self.assertNotEqual(result["artifacts"][0]["sha256"], result["artifacts"][0]["source_sha256"])

    def test_basic_auth_private_keys_and_access_key_credentials_are_redacted(self):
        values = ("Authorization: Basic dXNlcjpwYXNz",
                  "-----BEGIN PRIVATE KEY-----\nprivate-key-material\n-----END PRIVATE KEY-----",
                  "access key AKIA1234567890ABCDEF")
        for index, text in enumerate(values):
            with self.subTest(text=text):
                target = self.root / f"public-{index}"
                self.stage([self.entry(conclusion=text)], target)
                published = "".join(p.read_text() for p in target.iterdir())
                for secret in ("dXNlcjpwYXNz", "private-key-material", "AKIA1234567890ABCDEF"):
                    self.assertNotIn(secret, published)

    def test_json_is_not_an_arbitrary_environment_or_input_container(self):
        for key in ("environment", "env", "inputs", "command", "arguments", "content", "PASSWORD"):
            with self.subTest(key=key):
                value = {"schema_version": 1, "kind": "workflow-summary", "status": "inconclusive",
                         "conclusion": "Safe", key: {"PATH": "private-input"}}
                with self.assertRaises(ValueError):
                    self.stage([self.entry(content=value)])
                self.assertFalse(self.public.exists())
        for key in ("status", "attempt_id"):
            value = {"schema_version": 1, "kind": "workflow-summary", "status": "inconclusive",
                     "conclusion": "Safe", key: "AKIA1234567890ABCDEF"}
            with self.subTest(label=key), self.assertRaises(ValueError):
                self.stage([self.entry(content=value)])

    def test_unknown_kinds_and_sensitive_file_names_are_rejected(self):
        for name, kind in (("heap.dmp", "dump"), ("input.rsp", "summary"), (".env", "summary"),
                           ("environment.json", "summary"), ("private/summary.json", "summary"),
                           ("build.binlog", "binlog"), ("arbitrary.json", "json")):
            with self.subTest(name=name), self.assertRaises(ValueError):
                self.stage([self.entry(name, kind)])
        self.assertFalse(self.public.exists())

    def test_invalid_identity_size_and_extra_entry_fields_fail_before_writing(self):
        entry = self.entry()
        for mutate in (lambda e: e["file"].update(sha256="0" * 64),
                       lambda e: e["file"].update(size_bytes=10**20),
                       lambda e: e["file"].update(size_bytes=True),
                       lambda e: e.update(access="public"),
                       lambda e: e.update(kind="dump"),
                       lambda e: e.update(conclusion="x" * (a.CONCLUSION_BYTES + 1))):
            bad = copy.deepcopy(entry)
            mutate(bad)
            with self.subTest(value=bad), self.assertRaises(ValueError):
                self.stage([bad])
        self.assertFalse(self.public.exists())

    def test_traversal_outside_sources_and_overlapping_roots_are_rejected(self):
        entry = self.entry()
        bad = copy.deepcopy(entry)
        bad["file"]["path"] = str(self.private / ".." / "private" / "summary.json")
        with self.assertRaises(ValueError):
            self.stage([bad])
        if os.name == "nt":
            bad["file"]["path"] = str(self.private / "summary.json.")
            with self.assertRaises(ValueError):
                self.stage([bad])
        outside = self.root / "outside.json"
        outside.write_bytes(Path(entry["file"]["path"]).read_bytes())
        bad["file"]["path"] = str(outside)
        with self.assertRaises(ValueError):
            self.stage([bad])
        for root in (self.private, self.private / "upload", self.root):
            with self.subTest(root=root), self.assertRaises(ValueError):
                self.stage([entry], root)

    def test_hardlinked_source_is_rejected(self):
        entry = self.entry()
        os.link(entry["file"]["path"], self.private / "alias.json")
        with self.assertRaises(ValueError):
            self.stage([entry])

    def test_ancestor_symlink_and_junction_are_rejected(self):
        target = self.root / "target"
        target.mkdir()
        link = self.private / "link"
        if os.name == "nt":
            import subprocess
            subprocess.run(["cmd.exe", "/c", "mklink", "/J", str(link), str(target)],
                           check=True, capture_output=True)
            self.addCleanup(lambda: link.rmdir() if link.exists() else None)
        else:
            link.symlink_to(target, target_is_directory=True)
        path = target / "summary.json"
        data = json.dumps({"schema_version": 1, "kind": "workflow-summary",
                           "status": "inconclusive", "conclusion": "Safe"}).encode()
        path.write_bytes(data)
        entry = {"file": {"path": str(link / path.name), "sha256": hashlib.sha256(data).hexdigest()},
                 "kind": "summary", "conclusion": "Safe"}
        with self.assertRaises(ValueError):
            self.stage([entry])
        with self.assertRaises(ValueError):
            a.stage_artifacts(link, self.public, [], NOW)
        with self.assertRaises(ValueError):
            self.stage([], link / "upload")

    def test_nonempty_upload_root_is_never_recursively_reused(self):
        self.public.mkdir()
        (self.public / "private.env").write_text("secret")
        with self.assertRaises(ValueError):
            self.stage([self.entry()])
        self.assertEqual((self.public / "private.env").read_text(), "secret")

    def test_text_and_manifest_aggregate_budgets_fail_closed(self):
        entry = self.entry()
        for constant in ("COMPACT_BYTES", "PUBLIC_BYTES"):
            with self.subTest(constant=constant), mock.patch.object(a, constant, 100):
                with self.assertRaises(ValueError):
                    self.stage([entry])
                self.assertFalse(self.public.exists())
        with mock.patch.dict(a.c.LIMITS, {"json_bytes": 16}):
            with self.assertRaises(ValueError):
                self.stage([entry])

    def test_oversized_text_is_rejected_not_truncated(self):
        entry = self.entry(content=b"x" * 257)
        with mock.patch.dict(a.c.LIMITS, {"json_bytes": 256}), self.assertRaises(ValueError):
            self.stage([entry])
        self.assertFalse(self.public.exists())

    def test_binary_identity_and_conclusion_survive_without_copy_or_truncation(self):
        entry = self.entry("compiler.nettrace", "profile-trace", b"Nettrace" + b"x" * 8192)
        with mock.patch.object(a, "PUBLIC_BYTES", 4096):
            result = self.stage([entry])
        record = result["artifacts"][0]
        self.assertEqual(record["sha256"], entry["file"]["sha256"])
        self.assertEqual(record["size_bytes"], entry["file"]["size_bytes"])
        self.assertEqual(record["availability"], "unavailable")
        self.assertIn("budget", record["reason"])
        self.assertEqual(record["conclusion"], entry["conclusion"])
        self.assertEqual(list(self.public.iterdir()), [self.public / "manifest.json"])
        self.assertEqual(Path(entry["file"]["path"]).stat().st_size, 8200)

    def test_raw_trace_is_private_even_if_small(self):
        entry = self.entry("compiler.nettrace", "profile-trace", b"Nettrace\0PASSWORD=secret")
        result = self.stage([entry])
        record = result["artifacts"][0]
        self.assertEqual(record["access"], "private")
        self.assertEqual(record["availability"], "unavailable")
        self.assertNotIn(str(self.private), json.dumps(result))
        self.assertNotIn("secret", (self.public / "manifest.json").read_text())

    def test_source_inventory_reuses_collector_total_bound(self):
        entries = [self.entry(f"trace-{i}.nettrace", "profile-trace", b"Nettrace" + bytes([i]) * 64)
                   for i in range(2)]
        with mock.patch.dict(a.c.LIMITS, {"total_bytes": 100}), self.assertRaises(ValueError):
            self.stage(entries)
        self.assertFalse(self.public.exists())

    def test_expiry_checks_current_bytes_and_never_trusts_recorded_location(self):
        manifest = self.stage([self.entry()])
        self.assertTrue(a.verify_artifacts(self.public, manifest, NOW)[0]["available"])
        expires = NOW + timedelta(days=14)
        self.assertEqual(a.verify_artifacts(self.public, manifest, expires)[0]["reason"], "expired")
        record = manifest["artifacts"][0]
        path = self.public / record["location"]
        original = path.read_bytes()
        path.write_bytes(b"x" * len(original))
        self.assertFalse(a.verify_artifacts(self.public, manifest, NOW)[0]["available"])
        path.unlink()
        self.assertFalse(a.verify_artifacts(self.public, manifest, NOW)[0]["available"])
        bad = copy.deepcopy(manifest)
        bad["artifacts"][0]["location"] = str(self.private / "summary.json")
        self.assertFalse(a.verify_artifacts(self.public, bad, NOW)[0]["available"])

    def test_restart_rejects_linked_staged_content(self):
        manifest = self.stage([self.entry()])
        path = self.public / manifest["artifacts"][0]["location"]
        os.link(path, self.root / "alias.json")
        self.assertFalse(a.verify_artifacts(self.public, manifest, NOW)[0]["available"])

    def test_restart_cannot_extend_fixed_retention_or_accept_missing_manifest(self):
        manifest = self.stage([self.entry()])
        changed = copy.deepcopy(manifest)
        changed["artifacts"][0]["expires_at"] = (NOW + timedelta(days=100)).isoformat()
        with self.assertRaises(ValueError):
            a.verify_artifacts(self.public, changed, NOW)
        (self.public / "manifest.json").unlink()
        self.assertFalse(a.verify_artifacts(self.public, manifest, NOW)[0]["available"])

    def test_restart_rejects_unlisted_public_inventory(self):
        manifest = self.stage([self.entry()])
        (self.public / "injected.env").write_text("PRIVATE=value")
        self.assertFalse(a.verify_artifacts(self.public, manifest, NOW)[0]["available"])

    def test_duplicate_records_and_unbounded_entry_inventory_rejected(self):
        entry = self.entry()
        for entries in ([entry, entry], [entry] * (a.MAX_ENTRIES + 1)):
            with self.assertRaises(ValueError):
                self.stage(entries)
        self.assertFalse(self.public.exists())

    def test_malformed_and_nonfinite_json_are_rejected(self):
        for value in (b'{"schema_version":1,"schema_version":1}', b'{"x":NaN}', b"\xff"):
            with self.subTest(value=value), self.assertRaises((ValueError, UnicodeError)):
                self.stage([self.entry(content=value)])

    def test_dump_retention_has_no_plaintext_or_encryption_fallback(self):
        result = a.dump_retention_policy()
        self.assertEqual(result["status"], "unsupported")
        self.assertFalse(result["plaintext_fallback"])
        self.assertFalse(result["persistent"])
        self.assertEqual(result["default"], "ephemeral")

    def test_dump_workspace_rejects_git_and_upload_ancestors(self):
        with self.assertRaises(ValueError):
            a.create_dump_workspace(self.root / "dump", [self.public])
        self.assertFalse((self.root / "dump").exists())
        with mock.patch.object(a, "_outside_git"):
            with self.assertRaises(ValueError):
                a.create_dump_workspace(self.public / "dump", [self.public])
        self.assertFalse(self.public.exists())

    def test_owned_dump_cleanup_preserves_hash_and_never_unlinks_replacement(self):
        with mock.patch.object(a, "_outside_git"):
            owned = a.create_dump_workspace(self.root / "dump", [self.public])
        dump = owned.path / "heap.dmp"
        dump.write_bytes(b"MDMPprivate heap bytes")
        record = a.c.m.file_record(dump)
        receipt = a.claim_dump(owned, record)
        result = a.remove_owned_dump(owned, receipt, "No retained growth", NOW)
        self.assertFalse(dump.exists())
        self.assertEqual(result["sha256"], record["sha256"])
        self.assertEqual(result["availability"], "unavailable")
        self.assertEqual(result["conclusion"], "No retained growth")
        a.mem.validate_artifact(result)
        dump.write_bytes(b"MDMPnew owned dump")
        receipt = a.claim_dump(owned, a.c.m.file_record(dump))
        dump.rename(owned.path / "previous.dmp")
        dump.write_bytes(b"MDMPreplacement")
        with self.assertRaises(ValueError):
            a.remove_owned_dump(owned, receipt, "Conclusion", NOW)
        self.assertTrue(dump.exists())
        if os.name != "nt":
            self.assertEqual(stat.S_IMODE(owned.path.stat().st_mode), 0o700)

    def test_dump_cleanup_requires_owned_root_and_matching_hash(self):
        with mock.patch.object(a, "_outside_git"):
            owned = a.create_dump_workspace(self.root / "dump", [self.public])
        entry = self.entry("heap.dmp", "dump", b"MDMPnot owned")
        with self.assertRaises(ValueError):
            a.claim_dump(owned, entry["file"])
        path = owned.path / "heap.dmp"
        path.write_bytes(b"MDMPowned")
        receipt = a.claim_dump(owned, a.c.m.file_record(path))
        path.write_bytes(b"MDMPchanged")
        with self.assertRaises(ValueError):
            a.remove_owned_dump(owned, receipt, "Conclusion", NOW)
        self.assertTrue(path.exists())

    def test_dump_workspace_receipt_cannot_be_reconstructed_from_a_path(self):
        entry = self.entry("heap.dmp", "dump", b"MDMPnot created by dump workspace")
        forged = a.DumpWorkspace(self.private, a._identity(self.private.stat()))
        with self.assertRaises(ValueError):
            a.claim_dump(forged, entry["file"])

    def test_dump_analysis_cleanup_covers_failure_and_oversized_conclusions(self):
        with mock.patch.object(a, "_outside_git"):
            owned = a.create_dump_workspace(self.root / "dump", [self.public])
        path = owned.path / "heap.dmp"
        for fails in (False, True):
            with self.subTest(fails=fails):
                path.write_bytes(b"MDMPephemeral")
                record = a.c.m.file_record(path)
                analyzer = mock.Mock(side_effect=RuntimeError("private analysis failure")) if fails else (
                    mock.Mock(return_value="x" * (a.CONCLUSION_BYTES + 1)))
                if fails:
                    with self.assertRaises(RuntimeError) as raised:
                        a.analyze_owned_dump(owned, record, analyzer, NOW)
                    artifact = raised.exception.artifact
                else:
                    artifact = a.analyze_owned_dump(owned, record, analyzer, NOW)
                self.assertFalse(path.exists())
                self.assertEqual(artifact["sha256"], record["sha256"])
                self.assertLessEqual(len(artifact["conclusion"].encode()), a.CONCLUSION_BYTES)
                self.assertEqual(artifact["availability"], "unavailable")


if __name__ == "__main__":
    unittest.main()
