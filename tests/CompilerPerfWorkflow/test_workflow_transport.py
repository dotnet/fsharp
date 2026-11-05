"""Transport fixtures use only local bare repositories, never a network remote."""

import os
from pathlib import Path
import shutil
import stat
import subprocess
import sys
import threading
import unittest
from unittest import mock
import uuid

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "eng" / "scripts" / "compiler_perf"))
import memory as mem
try:
    import transport
except ModuleNotFoundError:
    transport = None


def attempt(identifier="one", *, active=False):
    fingerprint = "a" * 64
    return {
        "id": identifier,
        "hypothesis": {"id": identifier, "description": "Fixture", "area": "Checking", "operation": identifier},
        "run": {"id": "100", "attempt": 1, "url": "https://github.com/dotnet/fsharp/actions/runs/100"},
        "source_revision": "a" * 40, "lifecycle": "reserved" if active else "interrupted",
        "outcome": None if active else "rejected", "reason": "" if active else "Fixture rejection",
        "fingerprints": dict.fromkeys(mem.FINGERPRINTS, fingerprint),
        "dependencies": {"paths": {"src/Test.fs": fingerprint}, "workload": fingerprint,
                         "sdk": "11.0.100", "collector_permission": False},
        "retry": {"when_any": [{"kind": "path", "path": "src/Test.fs", "value": fingerprint}],
                  "reason": "Changed source"},
        "summary": {"evidence": "", "change": "", "patch_sha256": fingerprint},
        "evidence": [], "measurement": None, "collection": {"status": "pending", "reason": ""},
        "correctness": {"required": [["dotnet", "test", "-c", "Release"]], "results": []},
        "publication": None, "supersedes": None, "notes": [],
        "lease": {"run_id": "100", "run_attempt": 1, "checkpoint_at": "2026-09-19T00:00:00Z",
                  "expires_at": "2026-09-20T00:00:00Z"},
        "checkpoint": {"next_action": "Collect", "required_evidence": [], "candidate_ref": "candidate/one"},
    }


class JournalTransportTests(unittest.TestCase):
    def setUp(self):
        self.assertIsNotNone(transport, "trusted journal transport is not implemented")
        self.root = ROOT / "artifacts" / ("transport-fixture-" + uuid.uuid4().hex)
        self.root.mkdir(parents=True)
        def remove_readonly(function, path, _):
            os.chmod(path, stat.S_IWRITE)
            function(path)

        self.addCleanup(shutil.rmtree, self.root, onexc=remove_readonly)
        self.remote = self.root / "remote.git"
        run = subprocess.run

        def local_only(command, **kwargs):
            if command[0] == "git" and any(operation in command for operation in ("clone", "ls-remote", "fetch", "push")):
                self.assertIn(str(self.remote), command, "Tests must never contact a network remote")
            return run(command, **kwargs)

        patch = mock.patch.object(subprocess, "run", side_effect=local_only)
        patch.start()
        self.addCleanup(patch.stop)
        self.git("init", "--quiet", "--bare", str(self.remote))
        self.one = self.clone("one")
        self.two = self.clone("two")

    def git(self, *args, data=None):
        env = mem.c.tool_environment()
        env.update(GIT_CONFIG_NOSYSTEM="1", GIT_CONFIG_GLOBAL=os.devnull,
                   GIT_AUTHOR_NAME="Fixture", GIT_AUTHOR_EMAIL="fixture@example.invalid",
                   GIT_COMMITTER_NAME="Fixture", GIT_COMMITTER_EMAIL="fixture@example.invalid")
        return subprocess.run(["git", *args], input=data, capture_output=True,
                              check=True, timeout=30, env=env).stdout.decode().strip()

    def clone(self, name):
        path = self.root / (name + ".git")
        self.git("-c", "protocol.file.allow=always", "clone", "--quiet", "--bare", str(self.remote), str(path))
        return transport.JournalTransport(path, local_remote=self.remote)

    def write(self, writer, record, snapshot=None):
        base, head = snapshot or writer.read()
        return writer.write_state(base, head, mem.merge_attempt(base, record, base["attempts"].get(record["id"])))

    def assert_ack(self, receipt):
        self.assertFalse(receipt["local"]["remote_acknowledged"])
        self.assertEqual(receipt, {
            "scope": "local_fixture", "acknowledged": True, "commit": receipt["commit"],
            "branch": mem.STATE_BRANCH, "remote": str(self.remote.resolve()),
            "local": mem.GitJournal.receipt(receipt["commit"]),
        })
        self.assertEqual(self.git("--git-dir", str(self.remote), "rev-parse", "refs/heads/" + mem.STATE_BRANCH),
                         receipt["commit"])

    def test_interrupted_operation_admission_releases_busy_state(self):
        assign = transport.JournalTransport.__setattr__

        def interrupt(owner, name, value):
            assign(owner, name, value)
            if owner is self.one and name == "busy" and value:
                raise KeyboardInterrupt()

        with mock.patch.object(transport.JournalTransport, "__setattr__", interrupt):
            with self.assertRaises(KeyboardInterrupt):
                self.one.read()
        self.assertFalse(self.one.busy)
        self.assertFalse(self.one.lock_path.exists())
        self.assertEqual(self.one.read(), (mem.empty_state(), None))

    def test_verified_absence_and_exact_branch_round_trip(self):
        self.assertEqual(self.one.read(), (mem.empty_state(), None))
        receipt = self.write(self.one, attempt())
        self.assert_ack(receipt)
        state, head = self.two.read()
        self.assertEqual(state["attempts"], {"one": attempt()})
        self.assertEqual(head, receipt["commit"])
        self.assert_ack(self.two.write_state(state, head, state))
        self.assertEqual(self.git("--git-dir", str(self.remote), "for-each-ref", "--format=%(refname)"),
                         "refs/heads/" + mem.STATE_BRANCH)

    def test_actual_push_cas_race_reloads_and_preserves_other_attempt(self):
        snapshot = self.one.read()
        original = self.one._push
        results = []

        def race(expected, commit):
            if not results:
                self.write(self.two, attempt("two"), snapshot)
            result = original(expected, commit)
            results.append(result.returncode)
            return result

        with mock.patch.object(self.one, "_push", side_effect=race):
            receipt = self.write(self.one, attempt(), snapshot)
        self.assert_ack(receipt)
        self.assertNotEqual(results[0], 0)
        self.assertEqual(results[-1], 0)
        self.assertEqual(set(self.two.read()[0]["attempts"]), {"one", "two"})
        self.assertEqual(self.git("--git-dir", str(self.remote), "rev-list", "--count", receipt["commit"]), "2")

    def test_concurrent_reservations_preserve_loser_candidate(self):
        snapshots = [self.one.read(), self.two.read()]
        barrier = threading.Barrier(2)
        receipts, errors = [], []

        def reserve(writer, identifier, snapshot):
            original = writer._push
            first = True

            def synchronized(expected, commit):
                nonlocal first
                if first:
                    first = False
                    barrier.wait(timeout=20)
                return original(expected, commit)

            try:
                with mock.patch.object(writer, "_push", side_effect=synchronized):
                    receipts.append(self.write(writer, attempt(identifier, active=True), snapshot))
            except Exception as error:
                errors.append((writer, error))

        threads = [threading.Thread(target=reserve, args=args) for args in
                   ((self.one, "one", snapshots[0]), (self.two, "two", snapshots[1]))]
        for thread in threads:
            thread.start()
        for thread in threads:
            thread.join(timeout=40)
            self.assertFalse(thread.is_alive())
        self.assertEqual(len(receipts), 1)
        self.assertEqual(len(errors), 1)
        writer, failure = errors[0]
        self.assertIsInstance(failure, mem.Conflict)
        self.assertTrue(failure.candidate_ref)
        proposed = mem.decoded(writer.journal.git("show", failure.candidate_ref + ":state.json").stdout)
        stored = self.one.read()[0]
        self.assertEqual(len(stored["attempts"]), 1)
        self.assertNotEqual(set(proposed["attempts"]), set(stored["attempts"]))

    def test_denied_listing_is_not_verified_absence(self):
        run = subprocess.run

        def denied(command, **kwargs):
            if "ls-remote" in command:
                self.assertIn(str(self.remote), command)
                return subprocess.CompletedProcess(command, 128, b"", b"access denied")
            return run(command, **kwargs)

        with mock.patch.object(subprocess, "run", side_effect=denied):
            with self.assertRaisesRegex(ValueError, "denied"):
                self.one.read()
        self.assertIsNone(self.one.journal.head())

    def test_empty_or_corrupt_remote_tree_never_initializes(self):
        for files in ({}, {"state.json": b'{"schema_version":'},
                      {"other.json": b"{}"}, {"state.json": mem.encoded(dict(mem.empty_state(), schema_version=2))}):
            with self.subTest(files=files):
                commit = self.one.journal.commit(files, None)
                self.git("--git-dir", str(self.one.journal.repository), "-c", "protocol.file.allow=always",
                         "push", "--quiet", "--force", str(self.remote), commit + ":refs/heads/" + mem.STATE_BRANCH)
                with self.assertRaises(ValueError):
                    self.one.read()
                self.assertEqual(self.git("--git-dir", str(self.remote), "rev-parse", mem.STATE_BRANCH), commit)

    def test_denied_push_preserves_candidate_without_acknowledgment(self):
        snapshot = self.one.read()
        denied = subprocess.CompletedProcess([], 1, b"", b"permission denied")
        with mock.patch.object(self.one, "_push", return_value=denied) as push:
            with self.assertRaises(mem.Conflict) as caught:
                self.write(self.one, attempt(), snapshot)
        self.assertEqual(push.call_count, 1)
        self.assertTrue(caught.exception.candidate_ref)
        self.assertEqual(self.two.read(), snapshot)
        self.assertEqual(mem.decoded(self.one.journal.git(
            "show", caught.exception.candidate_ref + ":state.json").stdout)["attempts"], {"one": attempt()})

    def test_readback_failure_never_acknowledges_successful_push(self):
        original = self.one._push

        def accepted_then_unreadable(expected, commit):
            result = original(expected, commit)
            self.one._list_head = mock.Mock(side_effect=ValueError("readback denied"))
            return result

        with mock.patch.object(self.one, "_push", side_effect=accepted_then_unreadable):
            with self.assertRaises(mem.Conflict) as caught:
                self.write(self.one, attempt())
        self.assertTrue(caught.exception.candidate_ref)
        self.assertEqual(self.two.read()[0]["attempts"], {"one": attempt()})

    def test_changed_readback_is_not_exact_commit_acknowledgment(self):
        original = self.one._push

        def accepted_then_advanced(expected, commit):
            result = original(expected, commit)
            self.write(self.two, attempt("two"))
            return result

        with mock.patch.object(self.one, "_push", side_effect=accepted_then_advanced):
            with self.assertRaises(mem.Conflict):
                self.write(self.one, attempt())
        self.assertEqual(set(self.two.read()[0]["attempts"]), {"one", "two"})

    def test_local_lock_is_owned_and_never_reclaimed(self):
        with self.one._locked():
            with self.assertRaises(mem.Conflict):
                self.one.read()
        self.assertEqual(self.one.read(), (mem.empty_state(), None))
        self.one.lock_path.write_text("unproven lock")
        with self.assertRaises(mem.Conflict):
            self.one.read()
        self.assertEqual(self.one.lock_path.read_text(), "unproven lock")

    def test_transport_rejects_nonlocal_or_nonbare_fixture_paths(self):
        for remote in ("https://github.com/dotnet/fsharp.git", "file:///remote.git", "host:repo",
                       r"\\server\share\repo.git", "//server/share/repo.git", str(self.root)):
            with self.subTest(remote=remote), self.assertRaises((ValueError, OSError)):
                transport.JournalTransport(self.one.journal.repository, local_remote=remote)

    def test_fixture_symlink_or_junction_is_rejected(self):
        link = self.root / "linked.git"
        if os.name == "nt":
            subprocess.run(["cmd", "/c", "mklink", "/J", str(link), str(self.remote)],
                           check=True, capture_output=True)
            self.addCleanup(lambda: os.rmdir(link) if link.exists() else None)
        else:
            link.symlink_to(self.remote, target_is_directory=True)
        with self.assertRaises(ValueError):
            transport.JournalTransport(self.one.journal.repository, local_remote=link)

    def test_untrusted_global_config_and_git_environment_are_ignored(self):
        poison = self.root / "poison.gitconfig"
        poison.write_text('[url "https://invalid.invalid/"]\n\tinsteadOf = ' + str(self.remote).replace("\\", "/") +
                          '\n[core]\n\thooksPath = unexpected-hooks\n')
        env = {"GIT_CONFIG_GLOBAL": str(poison), "GIT_DIR": str(self.remote),
               "GIT_CONFIG_COUNT": "1", "GIT_CONFIG_KEY_0": "protocol.file.allow", "GIT_CONFIG_VALUE_0": "never",
               "GIT_SSH_COMMAND": "unexpected-command", "GIT_ASKPASS": "unexpected-command",
               "GH_TOKEN": "fixture-must-not-leak"}
        original = subprocess.run
        seen = []

        def inspect(*args, **kwargs):
            seen.append(kwargs["env"])
            return original(*args, **kwargs)

        with mock.patch.dict(os.environ, env), mock.patch.object(transport.subprocess, "run", side_effect=inspect):
            self.assert_ack(self.write(self.one, attempt()))
        self.assertTrue(seen)
        for environment in seen:
            self.assertNotIn("GH_TOKEN", environment)
            self.assertNotIn("GIT_DIR", environment)
            self.assertNotIn("GIT_SSH_COMMAND", environment)
            self.assertEqual(environment["GIT_CONFIG_GLOBAL"], os.devnull)

    def test_untrusted_repository_config_cannot_rewrite_remote(self):
        self.git("--git-dir", str(self.one.journal.repository), "config",
                 "url.https://invalid.invalid/.insteadOf", str(self.remote))
        with self.assertRaises(ValueError):
            self.one.read()

    def test_local_and_remote_hooks_are_disabled(self):
        for repository, name in ((self.one.journal.repository, "pre-push"), (self.remote, "pre-receive")):
            hook = repository / "hooks" / name
            hook.write_bytes(b"#!/bin/sh\nexit 1\n")
            hook.chmod(0o755)
        self.assert_ack(self.write(self.one, attempt()))

    def test_non_fast_forward_push_is_rejected_before_remote_command(self):
        first = self.write(self.one, attempt())
        unrelated = self.one.journal.commit({"state.json": mem.encoded(mem.empty_state())}, None)
        with self.assertRaisesRegex(ValueError, "fast-forward"):
            self.one._push(first["commit"], unrelated)
        self.assertEqual(self.two.read()[1], first["commit"])

    def test_cas_retry_budget_is_bounded_and_retains_candidate(self):
        original = self.one._push
        count = 0

        def keep_racing(expected, commit):
            nonlocal count
            count += 1
            self.write(self.two, attempt("other-" + str(count)))
            return original(expected, commit)

        with mock.patch.object(self.one, "_push", side_effect=keep_racing):
            with self.assertRaisesRegex(mem.Conflict, "retry budget") as caught:
                self.write(self.one, attempt())
        self.assertEqual(count, 5)
        self.assertTrue(caught.exception.candidate_ref)
        candidate = mem.decoded(self.one.journal.git("show", caught.exception.candidate_ref + ":state.json").stdout)
        self.assertIn("one", candidate["attempts"])
        self.assertNotIn("one", self.two.read()[0]["attempts"])

    def test_fixed_production_identity_requires_credentials_without_network(self):
        production = transport.JournalTransport(self.one.journal.repository)
        self.assertEqual(production.remote, "https://github.com/dotnet/fsharp.git")
        self.assertEqual(production.scope, "github")
        with mock.patch.dict(os.environ, {"GH_TOKEN": "", "GITHUB_TOKEN": ""}), \
                mock.patch.object(transport.subprocess, "run", wraps=subprocess.run) as run:
            with self.assertRaisesRegex(ValueError, "credential|token"):
                production.read()
        self.assertFalse(any("ls-remote" in call.args[0] for call in run.call_args_list))

    def test_production_credential_is_confined_to_network_git_environment(self):
        production = transport.JournalTransport(self.one.journal.repository)
        with mock.patch.dict(os.environ, {"GH_TOKEN": "fixture-token"}):
            ordinary = production._environment()
            network = production._environment(network=True)
            compiler = mem.c.tool_environment()
        for environment in (ordinary, network, compiler):
            self.assertNotIn("GH_TOKEN", environment)
            self.assertNotIn("GITHUB_TOKEN", environment)
        self.assertFalse(any("AUTHORIZATION:" in value for value in ordinary.values()))
        self.assertEqual(sum("AUTHORIZATION:" in value for value in network.values()), 1)
        self.assertFalse(any("AUTHORIZATION:" in value for value in compiler.values()))


if __name__ == "__main__":
    unittest.main()
