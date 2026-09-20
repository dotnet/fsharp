"""Portable controller state tests; toy commands and timings are not compiler evidence."""

from contextlib import contextmanager
from datetime import datetime, timezone
import copy
import os
from pathlib import Path
import shutil
import stat
import sys
import tempfile
from unittest import mock

import experiment as e
from test_collection import artifact_profile
from test_measurement import measurements, refresh_identities


class PortableTool:
    def __init__(self, tree, readonly=(), writable=(), sealed=()):
        self.tree, self.readonly = tree, readonly

    def command(self, args, cwd, environment, receipts, phase, timeout=1800):
        tool = Path(__file__).parent / "fixtures" / "ExperimentTool.py"
        actual = [sys.executable, str(tool), *(["pack"] if args[0] == "bash" else args[1:])]
        started = datetime.now(timezone.utc).isoformat()
        logs = receipts / "commands"
        logs.mkdir(exist_ok=True)
        result = e.c.run_tool(actual, cwd, environment, logs, phase, timeout=timeout)
        result.update(command=args, cwd=str(cwd), started_at=started,
                      finished_at=datetime.now(timezone.utc).isoformat())
        return result

    def process(self, *args):
        raise AssertionError("Portable state tests must not execute a compiler replay")


@contextmanager
def report_file(path, destination, isolation):
    yield
    if Path(path).is_file():
        shutil.copyfile(path, destination)


def snapshot(repo):
    index = repo.git("ls-files", "--stage", "-z")
    head = repo.git("rev-parse", "HEAD")
    status = repo.git("status", "--porcelain=v1", "--untracked-files=all")
    files = {}
    for path in repo.root.rglob("*"):
        if ".git" not in path.relative_to(repo.root).parts:
            files[str(path.relative_to(repo.root))] = (
                stat.S_IMODE(path.stat().st_mode), path.read_bytes() if path.is_file() else None)
    return files, index, head, status


@contextmanager
def candidate_fixture():
    from test_experiment import Repository, SOURCE, frozen_fixture

    with tempfile.TemporaryDirectory() as directory:
        root = Path(directory).resolve()
        repo = Repository(root / "frozen" / "workload")
        manifest = frozen_fixture(root / "frozen", repo)
        manifest["invariants"]["toolchain"]["sdks"] = manifest["invariants"]["toolchain"]["sdks"].replace("\n", os.linesep)
        manifest["baseline_compiler"]["toolchain_fingerprint"] = e.m.fingerprint_inputs(manifest["invariants"]["toolchain"])
        patch = repo.patch()
        repo.git("worktree", "add", "--quiet", "--detach", str(root / "source"), repo.sha)
        repo.root = root / "source"
        owned = root / "candidate"
        e.prepare_candidate(repo.root, owned, repo.request(), patch, manifest=manifest)
        evidence = artifact_profile(root)
        evidence.update(compiler=manifest["baseline_compiler"],
                        fingerprints={k: e.m.fingerprint_inputs(v) for k, v in manifest["invariants"].items()})
        e.m.write_json(root / "profile.json", evidence)

        def collect(frozen, compiler, output, **kwargs):
            baseline, candidate = measurements()
            baseline["compiler"] = copy.deepcopy(frozen["baseline_compiler"])
            candidate["compiler"] = copy.deepcopy(compiler)
            for document in (baseline, candidate):
                document["invariants"] = copy.deepcopy(frozen["invariants"])
            refresh_identities([baseline, candidate])
            mode = (owned / "mode").read_text() if (owned / "mode").exists() else ""
            for document in (baseline, candidate):
                for sample in document["samples"]:
                    if mode == "identical":
                        sample["elapsed_ms"] = 100
                    elif mode == "noise" and sample["phase"] == "aa":
                        sample["elapsed_ms"] = 80 + 10 * sample["pair"]
            if mode == "incompatible":
                candidate["invariants"]["toolchain"]["sdk"] = "different"
                refresh_identities([baseline, candidate])
            e.m.write_json(output, candidate)
            e.m.write_json(output.with_name("candidate-samples.baseline.json"), baseline)
            return {"baseline": baseline, "candidate": candidate}

        with mock.patch.object(e, "Isolation", PortableTool), \
                mock.patch.object(e, "capture_report", report_file), \
                mock.patch.object(e.m, "run_repetitions", side_effect=collect):
            yield root, repo, owned, patch


class ExperimentStateCases:
    def test_dirty_source_preservation_matrix(self):
        from test_experiment import SOURCE
        from test_memory import attempt, improvement, state

        for dirty in ("staged", "unstaged", "untracked-file", "untracked-directory", "mixed"):
            with self.subTest(dirty=dirty), candidate_fixture() as (root, repo, owned, patch):
                if dirty in ("staged", "mixed"):
                    (repo.root / SOURCE).write_bytes(b"staged user change\r\n")
                    repo.git("add", SOURCE)
                if dirty in ("unstaged", "mixed"):
                    (repo.root / SOURCE).write_bytes(b"unstaged user change\n")
                if dirty in ("untracked-file", "mixed"):
                    (repo.root / "user.txt").write_bytes(b"\x00untracked\xff")
                if dirty in ("untracked-directory", "mixed"):
                    (repo.root / "user-directory").mkdir()
                    (repo.root / "user-directory" / "nested.txt").write_bytes(b"keep me")
                repo.git("config", "status.showUntrackedFiles", "no")
                before = snapshot(repo)
                other = root / "other"
                e.prepare_candidate(repo.root, other, repo.request(), patch,
                                    manifest=e.m.read_json(owned / "workload.json"))
                if dirty == "mixed":
                    result = e.validate_candidate(other, root / "profile.json")
                    self.assertTrue(result["draft_eligible"], result)
                    with self.assertRaisesRegex(ValueError, "Accepted candidate is retained"):
                        e.discard_candidate(other)
                other_before = {p: (other / p).read_bytes() for p in ("proposal.patch", "candidate.json")}
                other_repo = copy.copy(repo)
                other_repo.root = other / "tree"
                other_tree = snapshot(other_repo)
                journal = root / "canonical.json"
                accepted = (e.mem.load_state(other / "attempt.json")["state"]["attempts"]["attempt-1"]
                            if dirty == "mixed" else improvement())
                if dirty != "mixed":
                    accepted["publication"] = e.mem.publication_intent(accepted, repo.sha)
                    accepted["lifecycle"] = "publication_pending"
                negative = attempt("negative", "rejected")
                e.mem.checkpoint(journal, state(accepted, negative))
                journal_before = journal.read_bytes()

                def checks(target, manifest, compiler, isolation):
                    current = e.verify_candidate(target)
                    receipt = e.candidate_attempt(current, manifest)["correctness"]
                    receipt["inputs"] = current["inputs"]
                    return receipt

                for outcome in ("no-finding", "test", "identical", "noise", "incompatible", "interrupt", "discard"):
                    with self.subTest(outcome=outcome):
                        target = owned if outcome == "no-finding" else root / outcome
                        if target != owned:
                            e.prepare_candidate(repo.root, target, repo.request(), patch,
                                                manifest=e.m.read_json(owned / "workload.json"))
                        (owned / "mode").write_text(outcome)
                        if outcome == "no-finding":
                            result = e.complete_no_finding(target, root / "profile.json")
                        elif outcome == "discard":
                            result = e.discard_candidate(target)
                        elif outcome == "interrupt":
                            with mock.patch.object(e, "run_correctness", side_effect=KeyboardInterrupt), \
                                    self.assertRaises(KeyboardInterrupt):
                                e.validate_candidate(target, root / "profile.json")
                            result = e.m.read_json(target / "candidate.json")
                        else:
                            effect = ValueError("Required check failed: regression-0") if outcome == "test" else checks
                            with mock.patch.object(e, "run_correctness", side_effect=effect):
                                result = e.validate_candidate(target, root / "profile.json")
                        expected = {"no-finding": "completed_no_finding", "test": "rejected",
                                    "identical": "rejected", "noise": "inconclusive",
                                    "incompatible": "inconclusive", "interrupt": "cancelled", "discard": None}[outcome]
                        self.assertEqual(result["outcome"], expected, result)
                        self.assertFalse(result["draft_eligible"])
                        self.assertFalse((target / "tree").exists())
                        self.assertEqual((target / "proposal.patch").read_bytes(), patch)
                        self.assertEqual(snapshot(repo), before)
                        self.assertEqual(journal.read_bytes(), journal_before)
                        self.assertTrue((other / "tree").is_dir())
                        self.assertEqual(snapshot(other_repo), other_tree)
                        self.assertEqual({p: (other / p).read_bytes() for p in other_before}, other_before)

    def test_controller_checkpoint_resume_matrix(self):
        from test_experiment import SOURCE

        cases = ("unchanged", "patch", "missing-patch", "digest", "body", "checkpoint-body", "source", "policy", "workload",
                 "sdk", "compiler", "sample", "added-sample", "run", "owner", "missing-state", "truncated-state",
                 "missing-checkpoint", "truncated-checkpoint", "patch-link", "tree-path", "orphan-checkpoint",
                 "frozen-source", "sdk-file", "compiler-file", "missing-sample")
        for change in cases:
            with self.subTest(change=change), candidate_fixture() as (root, repo, owned, patch):
                def interrupt(*args, **kwargs):
                    run = owned / "runs" / "interrupted"
                    run.mkdir(parents=True)
                    e.m.write_json(run / "partial.json", {"samples": [1, 2]})
                    raise KeyboardInterrupt

                with mock.patch.object(e, "run_correctness", side_effect=interrupt), self.assertRaises(KeyboardInterrupt):
                    e.validate_candidate(owned, root / "profile.json", checkpoint_on_cancel=True)
                state_path = owned / "candidate.json"
                saved = e.m.read_json(state_path)
                self.assertEqual(saved["outcome"], "cancelled")
                self.assertIn("checkpoint", saved)
                original = state_path.read_bytes()
                checkpoint_path = owned / "checkpoint.json"
                retained = checkpoint_path.read_bytes() if checkpoint_path.exists() else None
                reason = "Frozen restart inputs"
                if change == "patch":
                    (owned / "proposal.patch").write_bytes(patch + b"\n")
                    reason = "Proposed patch changed"
                elif change == "missing-patch":
                    (owned / "proposal.patch").unlink()
                    reason = "proposal.patch"
                elif change == "digest":
                    saved["checkpoint"]["sha256"] = "f" * 64
                    reason = "checkpoint missing or changed"
                elif change == "body":
                    saved["request"]["hypothesis"] = "altered hypothesis"
                elif change == "checkpoint-body":
                    previous = e.mem.load_state(checkpoint_path)["state"]
                    previous["attempts"]["attempt-1"]["reason"] = "changed cancellation"
                    e.m.write_json(checkpoint_path, previous)
                    reason = "checkpoint missing or changed"
                elif change == "tree-path":
                    saved["tree"] = str(repo.root)
                    reason = "tree path redirected"
                elif change == "orphan-checkpoint":
                    saved.pop("checkpoint")
                    saved["outcome"] = None
                    reason = "checkpoint is not bound"
                elif change == "source":
                    (owned / "tree" / SOURCE).write_text("altered")
                    reason = "source/patch changed"
                elif change in ("policy", "workload", "sdk"):
                    manifest = e.m.read_json(owned / "workload.json")
                    if change == "policy":
                        manifest["invariants"]["policy"]["pairs"] = 1
                    elif change == "workload":
                        manifest["invariants"]["workload"]["arguments"].append("other.fs")
                    else:
                        manifest["invariants"]["toolchain"]["sdk"] = "other"
                    e.m.write_json(owned / "workload.json", manifest)
                    reason = "Input changed"
                elif change == "compiler":
                    saved["compiler"]["source_revision"] = "f" * 40
                elif change in ("frozen-source", "sdk-file", "compiler-file"):
                    manifest = e.m.read_json(owned / "workload.json")
                    target = (Path(manifest["invariants"]["workload"]["checkout"]) / SOURCE if change == "frozen-source"
                              else Path(manifest["invariants"]["toolchain"]["sdk_compiler_files"][0]["path"])
                              if change == "sdk-file" else Path(saved["compiler"]["files"]["fsc.dll"]["path"]))
                    target.write_text("changed frozen bytes")
                    reason = "Restart dependencies differ" if change == "frozen-source" else "Input changed"
                elif change in ("sample", "added-sample"):
                    e.m.write_json(owned / "runs" / "interrupted" /
                                   ("partial.json" if change == "sample" else "extra.json"), {"samples": [3]})
                elif change == "missing-sample":
                    (owned / "runs" / "interrupted" / "partial.json").unlink()
                elif change == "run":
                    saved["request"]["run_id"] = "other-run"
                elif change == "owner":
                    owner = e.m.read_json(owned / "owner.json")
                    owner["inode"] += 1
                    e.m.write_json(owned / "owner.json", owner)
                    reason = "ownership"
                elif change in ("missing-state", "truncated-state", "missing-checkpoint", "truncated-checkpoint"):
                    target = state_path if change.endswith("state") else checkpoint_path
                    if change.startswith("missing"):
                        target.unlink(missing_ok=True)
                    else:
                        target.write_bytes(b'{"schema_version":')
                    reason = ""
                elif change == "patch-link":
                    target = root / "retained.patch"
                    (owned / "proposal.patch").rename(target)
                    try:
                        (owned / "proposal.patch").symlink_to(target)
                    except OSError as error:
                        self.skipTest(f"Symlink creation unavailable: {error}")
                    reason = "redirect"
                if change in ("digest", "body", "compiler", "run", "tree-path", "orphan-checkpoint"):
                    e.m.write_json(state_path, saved)
                with mock.patch.object(e.mem, "publication_intent", wraps=e.mem.publication_intent) as publish:
                    if change == "unchanged":
                        result = e.validate_candidate(owned, root / "profile.json")
                        self.assertTrue(result["draft_eligible"], result)
                        publish.assert_called_once()
                        e.mem.acceptance_ready(e.mem.load_state(owned / "attempt.json")["state"]["attempts"]["attempt-1"])
                    else:
                        try:
                            result = e.validate_candidate(owned, root / "profile.json")
                        except (ValueError, OSError) as error:
                            self.assertIn(reason.lower(), str(error).lower())
                        else:
                            self.assertFalse(result["draft_eligible"], result)
                            self.assertIn(reason.lower(), result["reason"].lower())
                        publish.assert_not_called()
                if retained is not None and change not in ("missing-checkpoint", "truncated-checkpoint", "checkpoint-body"):
                    self.assertEqual(checkpoint_path.read_bytes(), retained)
                if change == "missing-state":
                    self.assertFalse(state_path.exists())
                elif change == "truncated-state":
                    self.assertEqual(state_path.read_bytes(), b'{"schema_version":')
                self.assertIn(b'"cancelled"', original)

    def test_discard_rejects_substituted_tree_and_redirected_root(self):
        from test_experiment import Repository

        for redirect in ("tree", "ancestor"):
            with self.subTest(redirect=redirect), candidate_fixture() as (root, repo, owned, patch):
                unrelated = Repository(root / "unrelated")
                before = snapshot(unrelated)
                if redirect == "tree":
                    (owned / "tree").rename(owned / "retained-tree")
                    unrelated.root.rename(owned / "tree")
                    unrelated.root = owned / "tree"
                    target = owned
                else:
                    alias = root / "alias"
                    if os.name == "nt":
                        import _winapi
                        _winapi.CreateJunction(str(root), str(alias))
                    else:
                        alias.symlink_to(root, target_is_directory=True)
                    target = alias / owned.name
                with self.assertRaisesRegex(ValueError, "ownership|redirect"):
                    e.discard_candidate(target)
                self.assertEqual(snapshot(unrelated), before)

    def test_interrupted_checkpoint_write_preserves_original(self):
        with candidate_fixture() as (root, repo, owned, patch):
            with mock.patch.object(e, "run_correctness", side_effect=KeyboardInterrupt), self.assertRaises(KeyboardInterrupt):
                e.validate_candidate(owned, root / "profile.json", checkpoint_on_cancel=True)
            checkpoint = owned / "checkpoint.json"
            original = checkpoint.read_bytes()
            replace = os.replace

            def interrupt_write(source, target):
                if Path(target) == checkpoint:
                    raise OSError("interrupted checkpoint")
                return replace(source, target)

            with mock.patch.object(e, "run_correctness", side_effect=KeyboardInterrupt), \
                    mock.patch.object(e.mem.os, "replace", side_effect=interrupt_write), \
                    self.assertRaises(KeyboardInterrupt):
                e.validate_candidate(owned, root / "profile.json", checkpoint_on_cancel=True)
            self.assertEqual(checkpoint.read_bytes(), original)
            self.assertEqual(list(owned.glob("checkpoint.json.*")), [])
            result = e.m.read_json(owned / "candidate.json")
            self.assertIn("checkpoint refused: interrupted checkpoint", result["reason"])
            self.assertFalse(result["draft_eligible"])
            self.assertFalse((owned / "tree").exists())

    def test_interruption_between_checkpoint_and_candidate_writes_blocks_resume(self):
        for operation in ("validate", "no-finding"):
            for truncated in (False, True):
                with self.subTest(operation=operation, truncated=truncated), \
                        candidate_fixture() as (root, repo, owned, patch):
                    write = e.m.write_json

                    def interrupt_binding(path, value):
                        if Path(path) == owned / "candidate.json" and value.get("outcome") == "cancelled" and "checkpoint" in value:
                            raise OSError("interrupted candidate binding")
                        return write(path, value)

                    with mock.patch.object(e, "run_correctness", side_effect=KeyboardInterrupt), \
                            mock.patch.object(e.m, "write_json", side_effect=interrupt_binding), \
                            self.assertRaisesRegex(OSError, "interrupted candidate binding"):
                        e.validate_candidate(owned, root / "profile.json", checkpoint_on_cancel=True)
                    self.assertEqual(e.mem.load_state(owned / "checkpoint.json")["status"], "ready")
                    self.assertFalse((owned / "runs").exists())
                    saved = e.m.read_json(owned / "candidate.json")
                    self.assertIsNone(saved["outcome"])
                    self.assertNotIn("checkpoint", saved)
                    if truncated:
                        (owned / "checkpoint.json").write_bytes(b'{"schema_version":')
                    checkpoint = (owned / "checkpoint.json").read_bytes()
                    with mock.patch.object(e.mem, "publication_intent") as publish:
                        if operation == "no-finding":
                            exit_code = e.main(["validate", "--owned-root", str(owned),
                                               "--profile", str(root / "profile.json"), "--no-finding"])
                            result = e.m.read_json(owned / "candidate.json")
                        else:
                            result = e.validate_candidate(owned, root / "profile.json")
                        self.assertIn("checkpoint is not bound", result["reason"])
                        self.assertEqual(result["outcome"], "blocked_setup")
                        self.assertFalse(result["draft_eligible"])
                        publish.assert_not_called()
                        if operation == "no-finding":
                            self.assertEqual(exit_code, 2)
                            self.assertFalse((owned / "attempt.json").exists())
                    self.assertFalse((owned / "tree").exists())
                    self.assertEqual((owned / "checkpoint.json").read_bytes(), checkpoint)
                    self.assertEqual((owned / "proposal.patch").read_bytes(), patch)
