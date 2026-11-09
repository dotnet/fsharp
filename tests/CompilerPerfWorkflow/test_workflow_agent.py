"""Portable agent interactions use synthetic diagnostics, never compiler speedups."""

from contextlib import contextmanager
import asyncio
import copy
from datetime import timedelta
from itertools import product
import json
import os
from pathlib import Path
import socket
import subprocess
import sys
import tempfile
import threading
import time
from types import SimpleNamespace
import unittest
from unittest import mock

from test_collection import artifact_profile
from test_workflow_artifacts import TemporaryRecipient
import test_workflow as fixtures
from test_experiment import Repository, SOURCE
from experiment_state_cases import candidate_fixture
import agent_bridge as bridge
import workflow as w


def message(operation, data=None, identifier=None):
    return w.mem.encoded({"schema_version": 1, "id": identifier or operation,
                          "operation": operation, "data": data or {}}).decode()


class Worker:
    pid = 424242

    def __init__(self, code=0):
        self.code, self.signals = code, []

    def poll(self):
        return self.code

    def wait(self, timeout=None):
        if self.code is None:
            raise subprocess.TimeoutExpired("synthetic owned worker", timeout)
        return self.code

    def send_signal(self, value):
        self.signals.append(value)
        self.code = 130


def identify(pid):
    return {"pid": pid, "parent_pid": os.getpid(), "start_time": "synthetic"}


class MemoryTransport:
    """Unit-test acknowledgments only; ownership/restart/candidate cases use real bare Git."""

    scope, remote = "local_fixture", "synthetic"

    def __init__(self, root):
        self.state, self.head = w.mem.empty_state(), None
        self.journal = SimpleNamespace(repository=root / "synthetic-journal")
        self.journal.repository.mkdir()

    def read(self):
        return copy.deepcopy(self.state), self.head

    def write_state(self, base, head, candidate):
        if (base, head) != self.read():
            raise w.mem.Conflict("Synthetic CAS conflict")
        w.mem.validate_state(candidate)
        self.state = copy.deepcopy(candidate)
        self.head = w.m.fingerprint_inputs(candidate)[:40]
        return {"scope": self.scope, "acknowledged": True, "commit": self.head,
                "branch": w.mem.STATE_BRANCH, "remote": self.remote,
                "local": w.mem.GitJournal.receipt(self.head)}


class AgentTests(unittest.TestCase):
    fixture = fixtures.WorkflowTests.fixture

    @contextmanager
    def discovery(self, *, canonical=False, run_id="run-1"):
        with tempfile.TemporaryDirectory() as directory:
            repo, controller, api, transport = self.fixture(
                Path(directory), transport_factory=None if canonical else MemoryTransport)
            request = {"schema_version": 2, "run_id": run_id, "base_sha": repo.sha,
                       "primary_metric": "elapsed_ms"}
            self.assertEqual(controller.preflight(request)["action"], "reserve")
            yield repo, controller, api, transport

    @contextmanager
    def ready(self, *, canonical=False):
        with self.discovery(canonical=canonical) as fixture:
            controller = fixture[1]
            manifest = {"baseline_compiler": {}, "invariants": {}}

            def collect(_, __, root, **kwargs):
                root.mkdir()
                evidence = artifact_profile(root)
                evidence.update(compiler={}, fingerprints={})
                w.m.write_json(root / "evidence-manifest.json", evidence)

            controller.session.update(phase="ready", ready_at=controller.now().isoformat())
            controller.checkpoint()
            with TemporaryRecipient() as key, mock.patch.object(w.Controller, "frozen_manifest", return_value=manifest), \
                    mock.patch.object(w.c, "collect_profile", side_effect=collect):
                w.m.write_json(controller.root / "retention-config.json", key.config)
                yield fixture

    @staticmethod
    def selection(repo):
        return {**{k: v for k, v in repo.request().items()
                   if k in ("compiler_paths", "regressions", "hypothesis", "complexity")},
                "hot_operation": 0}

    @staticmethod
    def launch(arguments, controller):
        operation = arguments[-1]
        if operation == "dump":
            request = controller.session["dump_request"]
            controller.profile(request["reason"], request["hypothesis"])
        else:
            getattr(controller, "finish_operation" if operation == "finish" else operation)()
        return Worker()

    def test_fresh_discovery_has_no_invented_hypothesis(self):
        with self.discovery(canonical=True) as (_, controller, _, transport):
            self.assertIsNone(controller.session["attempt"])
            self.assertEqual(transport.read()[0]["attempts"], {})

    def test_empty_inbox_waits_after_delivering_complete_diagnostics(self):
        with self.discovery() as (_, controller, api, _):
            private = controller.root / "private"
            private.mkdir()
            profile = private / "profile"
            profile.mkdir()
            evidence = artifact_profile(profile)
            evidence.update(compiler={}, fingerprints={})
            w.m.write_json(profile / "evidence-manifest.json", evidence)
            controller.session.update(phase="ready", manifest={"path": "synthetic"},
                                      ready_at=controller.now().isoformat())
            with mock.patch.object(controller, "frozen_manifest", return_value={
                    "baseline_compiler": {}, "invariants": {}}), mock.patch.object(w.c, "collect_profile"):
                controller.start_active()
                controller.profile()
            status = controller.status()
            self.assertEqual(status["phase"], "awaiting_decision")
            self.assertIsNone(status["outcome"])
            self.assertTrue(status["diagnostics"]["stacks"])
            self.assertIn("allocated_bytes_estimate", status["diagnostics"]["metrics"])
            self.assertIn("binlog", status["diagnostics"])
            self.assertFalse((controller.root / "output").exists())
            self.assertEqual(api.effects, [])

    def test_explicit_no_finding_and_failed_collection_are_distinct(self):
        for case in ("valid", "missing", "failed", "tampered"):
            with self.subTest(case=case), self.ready() as (_, controller, api, transport):
                controller.start_active()
                if case == "failed":
                    with mock.patch.object(w.c, "collect_profile", side_effect=ValueError("tool missing")):
                        self.assertEqual(controller.profile()["outcome"], "blocked_collection")
                elif case != "missing":
                    controller.profile()
                    if case == "tampered":
                        Path(controller.session["profile"]["path"]).write_text("{}")
                if case == "valid":
                    result = controller.finish("completed_no_finding", "Sampled hot work is already shared")
                    self.assertEqual(result["outcome"], "completed_no_finding")
                else:
                    with self.assertRaises(ValueError):
                        controller.finish("completed_no_finding", "Ignore missing evidence")
                self.assertEqual(transport.read()[0]["attempts"], {})
                self.assertEqual(api.effects, [])

    def test_selection_requires_profile_and_an_observed_operation(self):
        with self.ready() as (repo, controller, _, _):
            controller.start_active()
            with self.assertRaises(ValueError):
                controller.select_hypothesis(self.selection(repo))
            controller.profile()
            for operation in (-1, 10000, True, "command"):
                with self.subTest(operation=operation), self.assertRaises(ValueError):
                    controller.select_hypothesis(dict(self.selection(repo), hot_operation=operation))
            result = controller.select_hypothesis(self.selection(repo))
            self.assertEqual(result["phase"], "selected")
            self.assertEqual(controller.session["attempt"]["collection"]["status"], "collected")
            with self.assertRaises(ValueError):
                controller.finish("completed_no_finding", "Cannot erase selected work")

    def test_rejected_revision_can_select_another_eligible_scope_not_repeat_itself(self):
        with tempfile.TemporaryDirectory() as directory:
            repo, controller, _, transport = self.fixture(Path(directory), transport_factory=MemoryTransport)
            other = "src/Compiler/CodeGen/Other.fs"
            (repo.root / other).write_text("module Other\nlet answer = 0\n")
            repo.git("add", other)
            repo.git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid",
                     "commit", "-qm", "second eligible scope")
            repo.sha = repo.git("rev-parse", "HEAD").decode().strip()
            self.assertEqual(controller.preflight({
                "schema_version": 2, "run_id": "run-1", "base_sha": repo.sha,
                "primary_metric": "elapsed_ms"})["action"], "reserve")
            controller.session.update(phase="ready", ready_at=controller.now().isoformat())
            controller.start_active()
            controller.session["phase"] = "awaiting_decision"
            deadline = controller.load("clocks")["active"]["deadline"]
            with mock.patch.object(controller, "diagnostics", return_value={
                    "stacks": [{"frames": ["Test.answer"], "weight": 1}], "profile_sha256": "a" * 64}):
                controller.select_hypothesis(self.selection(repo))
                a = controller.session["attempt"]
                a.update(outcome="rejected", reason="Synthetic unchanged timing", lifecycle="interrupted")
                controller.session.update(phase="awaiting_decision", revisions=[{"id": a["id"], "outcome": "rejected"}])
                controller.checkpoint()
                self.assertEqual(controller.select_hypothesis(self.selection(repo))["action"], "skip")
                result = controller.select_hypothesis(dict(self.selection(repo), compiler_paths=[other]))
                self.assertEqual(result["phase"], "selected")
            self.assertEqual(controller.load("clocks")["active"]["deadline"], deadline)
            records = transport.read()[0]["attempts"]
            self.assertEqual(len(records), 2)
            self.assertEqual(records[a["id"]]["reason"], "Synthetic unchanged timing")
            self.assertEqual(controller.session["attempt"]["id"], "run-1.3")

    def test_restart_each_idle_phase_keeps_deadline_and_patch_custody(self):
        with self.ready(canonical=True) as (repo, controller, api, transport):
            controller.start_active()
            deadline = controller.load("clocks")["active"]["deadline"]
            for phase in ("ready", "awaiting_decision", "selected", "patch_submitted"):
                if phase == "awaiting_decision":
                    controller.profile()
                elif phase == "selected":
                    controller.select_hypothesis(self.selection(repo))
                elif phase == "patch_submitted":
                    controller.submit_patch(repo.patch().decode())
                restarted = w.Controller(controller.root, repo.root, transport, api, now=controller.now)
                self.assertEqual(restarted.status()["phase"], phase)
                self.assertEqual(restarted.load("clocks")["active"]["deadline"], deadline)
                self.assertLessEqual(restarted.poll_active(), w.ACTIVE_SECONDS)
                if phase == "patch_submitted":
                    w.m.verify_files([restarted.session["patch"]])
                controller = restarted
            with self.assertRaises(ValueError):
                controller.submit_patch("late replacement")
            self.assertFalse((controller.root / "output").exists())

    def test_setup_and_near_active_deadline_save_truthful_partial_state(self):
        for case in ("setup", "readiness", "active"):
            with self.subTest(case=case), self.ready() as (_, controller, _, transport):
                now = controller.now()
                if case == "setup":
                    controller.session["phase"] = "reserved"
                    controller.now = lambda: now + timedelta(seconds=w.SETUP_SECONDS + 1)
                    with mock.patch.object(w, "capabilities") as capability:
                        self.assertEqual(controller.investigate()["outcome"], "blocked_setup")
                    capability.assert_not_called()
                elif case == "readiness":
                    controller.now = lambda: now + timedelta(seconds=w.STARTUP_SECONDS)
                    client = bridge.Bridge(controller, launch=self.launch, identify=identify)
                    client.investigate(message("start_profile"))
                    client.tick()
                    self.assertEqual(controller.status()["outcome"], "blocked_setup", controller.status())
                else:
                    controller.start_active()
                    controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS)
                    self.assertEqual(controller.profile()["outcome"], "cancelled")
                self.assertFalse((controller.root / "output").exists())
                state = transport.read()[0]
                self.assertIsNotNone(state["discoveries"]["run-1.discovery"]["outcome"])

    def test_setup_preflight_probe_uses_the_same_aggregate_deadline(self):
        with tempfile.TemporaryDirectory() as directory:
            repo, controller, api, transport = self.fixture(Path(directory), transport_factory=MemoryTransport)
            now = [controller.now()]
            controller.now = lambda: now[0]

            def slow_probe(_):
                now[0] += timedelta(seconds=w.SETUP_SECONDS)

            with mock.patch.object(api, "verify_source", side_effect=slow_probe):
                result = controller.preflight({"schema_version": 2, "run_id": "run-1",
                                               "base_sha": repo.sha, "primary_metric": "elapsed_ms"})
            self.assertEqual(result["outcome"], "blocked_setup")
            self.assertEqual(transport.read()[0], w.mem.empty_state())
            self.assertIsNone(controller.session)

    def test_preparation_freezes_inputs_before_agent_readiness(self):
        for elapsed in (0, w.SETUP_SECONDS):
            with self.subTest(elapsed=elapsed), self.discovery() as (repo, controller, _, transport):
                now = [controller.now()]
                controller.now = lambda: now[0]
                setup = controller.load("clocks")["setup"]

                def prepare(revision, root, output, metric):
                    self.assertEqual((revision, root, metric), (repo.sha, controller.root / "frozen", "elapsed_ms"))
                    now[0] += timedelta(seconds=elapsed)
                    w.m.write_json(output, {"prepared": True})

                with mock.patch.object(w, "capabilities") as capabilities, \
                        mock.patch.object(w.m, "prepare_workload", side_effect=prepare) as preparation, \
                        mock.patch.object(w.e, "frozen_records") as freeze:
                    result = controller.investigate()
                capabilities.assert_called_once_with(controller.root, controller.repository)
                preparation.assert_called_once()
                freeze.assert_called_once_with({"prepared": True})
                self.assertEqual(controller.load("clocks")["setup"]["deadline"], setup["deadline"])
                self.assertNotIn("active", controller.load("clocks"))
                if elapsed:
                    self.assertEqual(result["outcome"], "blocked_setup")
                else:
                    self.assertEqual(result["phase"], "ready")
                    w.m.verify_files([controller.session["manifest"]])
                    self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["lifecycle"], "ready")
                self.assertFalse((controller.root / "output").exists())

    def test_backwards_clock_cancellation_preserves_canonical_checkpoints(self):
        for selected in (False, True):
            with self.subTest(selected=selected), self.ready(canonical=True) as (repo, controller, api, transport):
                controller.start_active()
                controller.profile()
                if selected:
                    controller.select_hypothesis(self.selection(repo))
                before = transport.read()[0]
                now = controller.now()
                controller.now = lambda: now - timedelta(seconds=1)
                client = bridge.Bridge(controller, launch=self.launch, identify=identify)
                client.tick()
                client.tick()
                state = transport.read()[0]
                discovery = state["discoveries"]["run-1.discovery"]
                self.assertEqual(discovery["outcome"], "cancelled")
                self.assertEqual(discovery["checkpoint_at"], before["discoveries"][discovery["id"]]["checkpoint_at"])
                if selected:
                    attempt = state["attempts"][controller.session["attempt"]["id"]]
                    self.assertEqual(attempt["outcome"], "cancelled")
                    self.assertEqual(attempt["lease"], before["attempts"][attempt["id"]]["lease"])
                restarted = w.Controller(controller.root, repo.root, transport, api, now=controller.now)
                self.assertEqual(restarted.status()["outcome"], "cancelled")
                with self.assertRaises(ValueError):
                    restarted.validate_output()
                self.assertFalse((controller.root / "output").exists())
                self.assertEqual(api.effects, [])
                self.assertTrue(client.close())

    def test_watchdog_does_not_interrupt_a_worker_entering_finalization(self):
        with self.ready() as (_, controller, _, _):
            worker = Worker(None)
            client = bridge.Bridge(controller, launch=lambda *_: worker, identify=identify)
            self.assertTrue(client.investigate(message("start_profile"))["ok"])
            read_deadline = client._read_deadline

            def enter_finalization(record, name, allowance):
                if name == "active":
                    controller.remaining("finalization", w.FINALIZATION_SECONDS, create=True)
                    raise KeyboardInterrupt
                return read_deadline(record, name, allowance)

            with mock.patch.object(client, "_read_deadline", side_effect=enter_finalization):
                client.tick()
                self.assertEqual(worker.signals, [])
                self.assertFalse(client.blocked)
                client.tick()
                self.assertEqual(worker.signals, [])
            now = controller.now()
            controller.now = lambda: now + timedelta(seconds=w.FINALIZATION_SECONDS)
            client.tick()
            self.assertEqual(len(worker.signals), 1)
            client.close()

    def test_deadline_at_output_validation_durably_cancels_without_authority(self):
        for interrupted in (False, True):
            with self.subTest(interrupted=interrupted), self.ready(canonical=True) as (_, controller, _, transport):
                controller.clock = lambda: 0
                controller.start_active()
                controller.profile()
                controller.session["phase"] = "accepted"
                controller.save("session", controller.session)
                now = controller.now()
                controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS - int(interrupted))
                with mock.patch.object(controller, "current_attempt", side_effect=KeyboardInterrupt) as validation:
                    with self.assertRaises(KeyboardInterrupt):
                        controller.validate_output()
                self.assertEqual(validation.call_count, int(interrupted))
                self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"], "cancelled")
                for name in ("approval.json", "output"):
                    self.assertFalse((controller.root / name).exists())

    def test_dump_operation_uses_owned_storage_and_preserves_profile(self):
        for reason, cancelled in [(r, False) for r in w.c.REASONS] + [("retention", True)]:
            with self.subTest(reason=reason, cancelled=cancelled), self.ready() as (_, controller, _, transport):
                controller.start_active()
                controller.profile()
                original = copy.deepcopy(controller.session["profile"])
                paths = []

                def collect(manifest, compiler, destination, **request):
                    self.assertEqual(request.pop("ipc_root").parent, controller.root.parent / "ipc")
                    self.assertEqual(request, {"dump_reason": reason, "hypothesis": "Heap grows across full GCs"})
                    destination.mkdir()
                    evidence = artifact_profile(destination)
                    dump = destination / "private-dump" / "heap.dmp"
                    dump.parent.mkdir()
                    dump.write_bytes(b"MDMP" + bytes(5000))
                    paths.append(dump)
                    evidence["dump"] = {
                        "status": "collected", "published": False, "identity": evidence["process"]["start"],
                        "artifact": dict(w.m.file_record(dump), size_bytes=dump.stat().st_size),
                        "reason_for_dump": reason, "hypothesis": request["hypothesis"],
                        "command": {"status": "collected", "exit_status": 0},
                        "validation": {"status": "collected", "exit_status": 0, "stdout": "Synthetic heap summary"}}
                    w.m.write_json(destination / "evidence-manifest.json", evidence)

                client = bridge.Bridge(controller, launch=self.launch, identify=identify)
                encrypt = w.artifacts._encrypt

                def retain(path, record, *args):
                    if cancelled and record["type"] == "dump":
                        raise KeyboardInterrupt("Interrupted encryption")
                    return encrypt(path, record, *args)

                with mock.patch.object(w.c, "collect_profile", side_effect=collect), \
                        mock.patch.object(w.artifacts, "_encrypt", side_effect=retain):
                    result = client.investigate(message("request_dump", {
                        "reason": reason, "hypothesis": "Heap grows across full GCs"}))
                self.assertTrue(result["ok"], result)
                status = client.investigate(message("status"))["status"]
                self.assertIsNotNone(status["dump"], status)
                self.assertEqual(status["dump"]["status"], "blocked_retention" if cancelled else "encrypted_and_removed", status)
                self.assertEqual(status["dump"]["artifact"]["availability"], "unavailable" if cancelled else "pending")
                self.assertEqual(status["phase"], "finished" if cancelled else "awaiting_decision")
                self.assertIn(status["dump"]["artifact"],
                              transport.read()[0]["discoveries"]["run-1.discovery"]["evidence"])
                self.assertEqual(controller.session["profile"], original)
                self.assertFalse(paths[0].exists())
                self.assertTrue(paths[0].is_relative_to(controller.root / "private"))
                self.assertFalse((controller.root / "output").exists())
                self.assertTrue(client.close())

    def test_retention_restarts_preserve_discovery_and_require_trusted_acknowledgment(self):
        for interruption in ("stage", "save", "upload", "acknowledgment"):
            with self.subTest(interruption=interruption), self.ready() as (repo, controller, api, transport):
                controller.start_active()
                controller.profile()
                if interruption in ("stage", "save"):
                    real = controller.save

                    def save(name, value):
                        if name == "session" and value.get("artifacts"):
                            raise KeyboardInterrupt("lost staging acknowledgment")
                        return real(name, value)

                    target = mock.patch.object(w.artifacts, "stage_artifacts", side_effect=KeyboardInterrupt)
                    if interruption == "save":
                        target = mock.patch.object(controller, "save", side_effect=save)
                    with target, self.assertRaises(KeyboardInterrupt):
                        controller.stage_evidence()
                else:
                    controller.stage_evidence()
                    controller.checkpoint()
                    with self.assertRaises(KeyboardInterrupt):
                        controller.acknowledge_evidence(mock.Mock(side_effect=KeyboardInterrupt(interruption)))
                restarted = w.Controller(controller.root, repo.root, transport, api, now=controller.now)
                result = restarted.stage_evidence()
                self.assertEqual(result["status"], "pending", result)
                self.assertTrue(any(f["path"].endswith(".age") for f in result["files"]))
                restarted.checkpoint()
                state = transport.read()[0]
                self.assertEqual(state["schema_version"], 3)
                self.assertFalse(state["attempts"])
                evidence = state["discoveries"]["run-1.discovery"]["evidence"]
                self.assertTrue(evidence)
                self.assertEqual({r["availability"] for r in evidence}, {"pending"})
                before = copy.deepcopy(evidence)

                def sink(plan):
                    self.assertEqual(plan["retention_days"], 14)
                    return {"scope": "local_fixture", "acknowledged": True,
                            "manifest_sha256": plan["manifest_sha256"], "artifact_id": "local-only",
                            "location": "local-fixture", "retention_days": 14,
                            "created_at": plan["created_at"], "expires_at": plan["expires_at"]}

                restarted.acknowledge_evidence(sink)
                self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["evidence"], before)
                for file in result["files"]:
                    if file["path"].endswith(".age"):
                        Path(file["path"]).unlink()
                restarted.now = lambda: fixtures.NOW + timedelta(days=15)
                expired = restarted.stage_evidence()
                self.assertEqual(expired["status"], "expired")
                self.assertFalse(expired["files"])
                self.assertTrue(all(s["reason"] == "expired" for r in expired["staged"]
                                    for s in r.get("artifacts", [])))
                self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["evidence"], before)
                self.assertEqual(api.effects, [])

    def test_cancelled_discovery_preserves_compact_evidence_when_retention_is_blocked(self):
        for failure in ("missing-recipient", "denied-read"):
            with self.subTest(failure=failure), self.ready() as (_, controller, _, transport):
                controller.start_active()
                controller.profile()
                if failure == "missing-recipient":
                    (controller.root / "retention-config.json").unlink()
                read = w.artifacts._read_file

                def checked(path, **kwargs):
                    if failure == "denied-read" and Path(path).suffix == ".nettrace":
                        raise PermissionError("trace read denied")
                    return read(path, **kwargs)

                with mock.patch.object(w.artifacts, "_read_file", side_effect=checked):
                    result = controller.finish("cancelled", "Preserve failed investigation")
                self.assertEqual(result["outcome"], "cancelled")
                retained = transport.read()[0]["discoveries"]["run-1.discovery"]
                self.assertTrue(any(r["type"] == "summary" for r in retained["evidence"]))
                self.assertEqual(controller.session["retention"]["status"], "blocked_retention")
                files = controller.session["retention"]["files"]
                self.assertTrue(files)
                self.assertTrue(all(Path(f["path"]).suffix == ".json" for f in files))
                self.assertFalse(any(r["availability"] == "available" for r in retained["evidence"]))

    def test_acknowledgment_recovery_preserves_completed_receipts(self):
        for name, interruption in product(("attempt", "discovery"),
                                          ("partial", "before-save", "before-checkpoint", "after-journal")):
            with self.subTest(owner=name, interruption=interruption), tempfile.TemporaryDirectory() as directory:
                repo, controller, api, transport = self.fixture(Path(directory))
                request = (dict(repo.request(), run_id="100") if name == "attempt" else {
                    "schema_version": 2, "run_id": "100", "base_sha": repo.sha, "primary_metric": "elapsed_ms"})
                self.assertEqual(controller.preflight(request)["action"], "reserve")
                index = "attempts" if name == "attempt" else "discoveries"
                owner_id = controller.session[name]["id"]
                controller.stage_evidence()
                controller.checkpoint()
                controller.finish("cancelled", "Retain both compact snapshots")
                self.assertEqual(len(controller.session["artifacts"]), 2)
                calls = []

                def sink(plan):
                    calls.append(plan["manifest_sha256"])
                    if interruption == "partial" and len(calls) == 2:
                        raise KeyboardInterrupt("second upload interrupted")
                    return {"scope": "actions", "acknowledged": True,
                            "manifest_sha256": plan["manifest_sha256"], "artifact_id": str(len(calls)),
                            "location": f"https://github.com/dotnet/fsharp/actions/runs/100/artifacts/{len(calls)}",
                            "retention_days": 14, "created_at": plan["created_at"], "expires_at": plan["expires_at"]}

                write = transport.write_state

                def lost_journal_ack(*args):
                    write(*args)
                    raise KeyboardInterrupt("journal acknowledgment lost")

                save, checkpoint = controller.save, controller.checkpoint

                def interrupt_save(*args):
                    if calls:
                        raise KeyboardInterrupt("acknowledgment not saved")
                    return save(*args)

                def interrupt_checkpoint():
                    if calls:
                        raise KeyboardInterrupt("acknowledgment saved before checkpoint")
                    return checkpoint()

                target = (mock.patch.object(controller, "save", side_effect=interrupt_save) if interruption == "before-save"
                          else mock.patch.object(controller, "checkpoint", side_effect=interrupt_checkpoint)
                          if interruption == "before-checkpoint" else
                          mock.patch.object(transport, "write_state", side_effect=lost_journal_ack)
                          if interruption == "after-journal" else mock.patch.object(api, "failure", None))
                with target, self.assertRaises(KeyboardInterrupt):
                    controller.acknowledge_evidence(sink)
                completed = [r for r in transport.read()[0][index][owner_id]["evidence"]
                             if r["availability"] == "available"]
                with mock.patch.object(w.mem, "_VALIDATOR_KEY", os.urandom(32)):
                    restarted = w.Controller(controller.root, repo.root, transport, api, now=controller.now)
                    restarted.checkpoint()
                    recovered = transport.read()[0][index][owner_id]["evidence"]
                    self.assertEqual({r["availability"] for r in recovered},
                                     {"pending"} if interruption == "before-save" else
                                     {"pending", "available"})
                    result = restarted.acknowledge_evidence(sink)
                    self.assertEqual(result["status"], "available", result)
                    records = transport.read()[0][index][owner_id]["evidence"]
                    self.assertTrue(all(r["availability"] == "available" for r in records))
                    self.assertTrue(all(r in records for r in completed))
                    self.assertEqual(calls.count(calls[0]), 2 if interruption == "before-save" else 1)
                    self.assertEqual(len(calls), 3 if interruption in ("partial", "before-save") else 2)
                    no_upload = mock.Mock(side_effect=AssertionError("completed manifest uploaded again"))
                    restarted.acknowledge_evidence(no_upload)
                    no_upload.assert_not_called()
                    self.assertEqual(transport.read()[0][index][owner_id]["evidence"], records)

    def test_acknowledgment_recovery_in_fresh_process(self):
        with self.discovery(canonical=True, run_id="100") as (repo, controller, api, transport):
            controller.stage_evidence()
            controller.checkpoint()

            def sink(plan):
                return {"scope": "actions", "acknowledged": True,
                        "manifest_sha256": plan["manifest_sha256"], "artifact_id": "1",
                        "location": "https://github.com/dotnet/fsharp/actions/runs/100/artifacts/1",
                        "retention_days": 14, "created_at": plan["created_at"], "expires_at": plan["expires_at"]}

            with mock.patch.object(controller, "checkpoint", side_effect=[controller.session["persistence"],
                    KeyboardInterrupt("sealed receipt before Git checkpoint")]), self.assertRaises(KeyboardInterrupt):
                controller.acknowledge_evidence(sink)
            saved = copy.deepcopy(controller.load("session")["discovery"]["evidence"])
            self.assertEqual({r["availability"] for r in saved}, {"available"})
            self.assertEqual({r["availability"] for r in
                              transport.read()[0]["discoveries"]["100.discovery"]["evidence"]}, {"pending"})
            script = """
import sys
sys.path[:0] = sys.argv[1:3]
import workflow as w
from transport import JournalTransport
from test_workflow import API, NOW
transport = JournalTransport(sys.argv[5], local_remote=sys.argv[6])
controller = w.Controller(sys.argv[3], sys.argv[4], transport, API(), now=lambda: NOW)
controller.checkpoint()
def no_upload(plan):
    raise AssertionError("Saved acknowledgment uploaded again")
assert controller.acknowledge_evidence(no_upload)["status"] == "available"
"""
            logs = controller.root / "restart-logs"
            logs.mkdir()
            result = w.c.run_tool([sys.executable, "-I", "-c", script, str(Path(w.__file__).parent),
                                  str(Path(__file__).parent), str(controller.root), str(repo.root),
                                  str(transport.journal.repository), transport.remote],
                                 repo.root, w.c.tool_environment(), logs, "fresh-process-recovery", timeout=120)
            self.assertEqual(result["status"], "collected", result)
            self.assertNotEqual(result["profiler_pid"], os.getpid())
            recovered = transport.read()[0]["discoveries"]["100.discovery"]["evidence"]
            for before, after in zip(saved, recovered, strict=True):
                self.assertNotEqual(before["acknowledgment"]["validation"], after["acknowledgment"]["validation"])
                before["acknowledgment"].pop("validation")
                after["acknowledgment"].pop("validation")
                self.assertEqual(before, after)
            self.assertEqual(api.effects, [])

    def test_late_recovery_keeps_manifest_conclusions_without_restaging(self):
        for selected, change in ((False, "intact"), (True, "intact"), (True, "missing"), (True, "changed")):
            with self.subTest(selected=selected, manifest=change), \
                    self.ready(canonical=True) as (repo, controller, api, transport):
                controller.start_active()
                controller.profile()
                if selected:
                    controller.select_hypothesis(self.selection(repo))
                    controller.session["attempt"].update(outcome="rejected", reason="Retain rejected measurements",
                                                         lifecycle="interrupted")
                controller.checkpoint()
                previous = transport.read()[0]
                save = controller.save

                def interrupt(name, value):
                    if name == "session" and value.get("artifacts"):
                        raise KeyboardInterrupt("manifest moved before session save")
                    save(name, value)

                with mock.patch.object(controller, "save", side_effect=interrupt), self.assertRaises(KeyboardInterrupt):
                    controller.stage_evidence()
                destination = next((controller.root / "public").iterdir())
                manifest = w.read_input(destination / "manifest.json")
                for path in destination.glob("*.age"):
                    path.unlink()
                if change == "missing":
                    (destination / "manifest.json").unlink()
                elif change == "changed":
                    altered = copy.deepcopy(manifest)
                    altered["artifacts"][0]["conclusion"] = "Invented conclusion"
                    w.m.write_json(destination / "manifest.json", altered)
                restarted = w.Controller(controller.root, repo.root, transport, api,
                                         now=lambda: fixtures.NOW + timedelta(days=15))
                with mock.patch.object(w.artifacts, "stage_artifacts", side_effect=AssertionError("expired restaging")):
                    result = restarted.stage_evidence()
                self.assertEqual(result["status"], "expired" if change == "intact" else "blocked_retention", result)
                self.assertEqual(result["files"], [])
                restarted.checkpoint()
                current = transport.read()[0]
                retained = current["discoveries"]["run-1.discovery"].get("evidence", [])
                self.assertEqual(retained, manifest["artifacts"] if change == "intact" else [])
                expected = copy.deepcopy(previous["attempts"])
                for record in expected.values():
                    record["lease"]["checkpoint_at"] = record["lease"]["expires_at"]
                self.assertEqual(current["attempts"], expected)
                self.assertIsNone(current["discoveries"]["run-1.discovery"]["outcome"])

    def test_blocked_retention_prevents_optimization_output(self):
        for failure in ("healthy", "missing-config", "encryption-failure", "budget-overflow", "denied-cleanup"):
            with self.subTest(failure=failure), candidate_fixture() as (root, repo, candidate, _), \
                    TemporaryRecipient() as key:
                _, controller, api, transport = self.fixture(root / "acceptance", repo)
                self.assertEqual(controller.preflight(repo.request())["action"], "reserve")
                validated = w.e.validate_candidate(candidate, root / "profile.json")
                self.assertTrue(validated["draft_eligible"], validated)
                measured = w.mem.load_state(candidate / "attempt.json")["state"]["attempts"]["attempt-1"]
                self.assertEqual(measured["outcome"], "measured_improvement")
                w.mem.acceptance_ready(measured)
                controller.session.update(candidate=str(candidate),
                                          manifest=w.m.file_record(candidate / "workload.json"),
                                          profile=w.m.file_record(root / "profile.json"))
                trace = controller.root / "private" / "profile" / "compiler.nettrace"
                trace.parent.mkdir(parents=True)
                trace.write_bytes(b"Nettrace" + bytes(8192))
                if failure != "missing-config":
                    w.m.write_json(controller.root / "retention-config.json", key.config)
                if failure == "denied-cleanup":
                    owned = w.artifacts.create_dump_workspace(controller.root / "private" / "dump-session")
                    dump = owned.path / "heap.dmp"
                    dump.write_bytes(b"MDMPowned analyzed snapshot")
                    controller.stage_evidence([{"file": w.m.file_record(dump), "kind": "dump",
                                                "conclusion": "Analyzed heap snapshot"}], dump_workspace=owned)
                encrypt = w.artifacts._encrypt
                remove = w.artifacts.remove_owned_dump
                with mock.patch.object(w.artifacts, "_encrypt", side_effect=OSError("encryptor failed")
                                       if failure == "encryption-failure" else encrypt), \
                        mock.patch.object(w.artifacts, "PUBLIC_BYTES", 8000 if failure == "budget-overflow"
                                          else w.artifacts.PUBLIC_BYTES), \
                        mock.patch.object(w.artifacts, "remove_owned_dump", side_effect=PermissionError("delete denied")
                                          if failure == "denied-cleanup" else remove):
                    final = controller.accept_experiment(candidate)
                self.assertIsNone(final["publication"])
                self.assertFalse((controller.root / "output").exists())
                self.assertFalse((controller.root / "approval.json").exists())
                if failure == "healthy":
                    self.assertEqual(final["outcome"], "measured_improvement")
                    self.assertEqual(controller.session["retention"]["status"], "pending")
                    self.assertEqual({r["availability"] for r in final["evidence"]}, {"pending"})
                    payload = controller.validate_output()
                    self.assertEqual(len(payload["items"]), 1)
                    self.assertTrue(payload["items"][0]["draft"])
                else:
                    self.assertEqual(final["outcome"], "blocked_collection", final)
                    self.assertEqual(controller.session["retention"]["status"], "blocked_retention")
                    with self.assertRaisesRegex(ValueError, "No measured candidate"):
                        controller.validate_output()
                    publisher = mock.Mock()
                    with self.assertRaisesRegex(ValueError, "No acknowledged publication intent"):
                        controller.finalize(publisher=publisher)
                    publisher.publish.assert_not_called()
                    self.assertFalse((controller.root / "output").exists())
                    self.assertFalse((controller.root / "approval.json").exists())
                    persisted = transport.read()[0]["attempts"]["attempt-1"]
                    self.assertEqual(persisted["outcome"], "blocked_collection")
                    self.assertIsNone(persisted["publication"])
                self.assertEqual(api.effects, [])

    def test_recovered_dump_cleanup_requires_original_owned_object_even_after_expiry(self):
        for change in ("unchanged", "expired", "replaced", "denied", "workspace-lost", "workspace-replaced",
                       "after-validation", "after-validation-expired"):
            with self.subTest(change=change), self.ready() as (repo, controller, api, transport):
                private = controller.root / "private"
                private.mkdir()
                owned = w.artifacts.create_dump_workspace(private / "dump-session")
                path = owned.path / "heap.dmp"
                path.write_bytes(b"MDMPowned analyzed snapshot")
                entry = {"file": w.m.file_record(path), "kind": "dump", "conclusion": "Analyzed heap snapshot"}
                save = controller.save

                def interrupt(name, value):
                    if name == "session" and value.get("artifacts"):
                        raise KeyboardInterrupt("dump staged before session save")
                    return save(name, value)

                with mock.patch.object(controller, "save", side_effect=interrupt), self.assertRaises(KeyboardInterrupt):
                    controller.stage_evidence([entry], dump_workspace=owned)
                destination = next((controller.root / "public").iterdir())
                manifest = w.read_input(destination / "manifest.json")
                retained_dump = next(r for r in manifest["artifacts"] if r["type"] == "dump")
                self.assertEqual(retained_dump["availability"], "pending", retained_dump["reason"])
                if change == "replaced":
                    path.rename(owned.path / "original.dmp")
                    path.write_bytes(b"MDMPreplacement")
                elif change == "workspace-lost":
                    path.unlink()
                    owned.path.rmdir()
                elif change == "workspace-replaced":
                    owned.path.rename(owned.path.with_name("original-dump-session"))
                    owned.path.mkdir()
                    path.write_bytes(b"MDMPreplacement workspace")
                now = ((lambda: fixtures.NOW + timedelta(days=15))
                       if change in ("expired", "after-validation-expired") else controller.now)
                restarted = w.Controller(controller.root, repo.root, transport, api, now=now)
                remove = w.artifacts.remove_owned_dump
                record = w.artifacts._record

                def replace_then_record(*args, **kwargs):
                    path.rename(owned.path / "original.dmp")
                    path.write_bytes(b"MDMPreplacement after validation")
                    return record(*args, **kwargs)

                with mock.patch.object(w.artifacts, "remove_owned_dump",
                                       side_effect=PermissionError("delete denied") if change == "denied" else remove), \
                        mock.patch.object(w.artifacts, "_record", side_effect=replace_then_record
                                          if change.startswith("after-validation") else record):
                    result = restarted.stage_evidence()
                blocked = change in ("replaced", "denied", "workspace-replaced") or change.startswith("after-validation")
                self.assertEqual(path.exists(), blocked)
                self.assertEqual(result["status"], "blocked_retention" if blocked
                                 else "expired" if change == "expired" else "pending", result)
                if blocked:
                    if change != "after-validation-expired":
                        self.assertTrue(any(f["kind"] == "dump" for f in result["files"]), result)
                    else:
                        self.assertEqual(result["files"], [])
                    self.assertTrue(any(s.get("reason") for s in result["staged"]))
                if change.startswith("after-validation"):
                    self.assertEqual(path.read_bytes(), b"MDMPreplacement after validation")
                    self.assertEqual((owned.path / "original.dmp").read_bytes(), b"MDMPowned analyzed snapshot")
                restarted.checkpoint()
                retained = transport.read()[0]["discoveries"]["run-1.discovery"].get("evidence", [])
                self.assertTrue(any(r["type"] == "dump" and r["sha256"] == entry["file"]["sha256"]
                                    and r["conclusion"] == entry["conclusion"] for r in retained))
                self.assertIn(retained_dump, retained)

    def test_cumulative_staging_charges_saved_and_unacknowledged_manifests(self):
        for bound in ("PUBLIC_BYTES", "COMPACT_BYTES"):
            for state in ("interrupted", "saved", "expired", "deleted"):
                with self.subTest(bound=bound, state=state), self.discovery() as (repo, controller, api, transport), \
                        mock.patch.object(w.artifacts, bound, 10000):
                    controller.session["discovery"]["reason"] = "x" * 1900
                    save = controller.save

                    def interrupt(name, value):
                        if state == "interrupted" and name == "session" and value.get("artifacts"):
                            raise KeyboardInterrupt("unacknowledged staged directory")
                        save(name, value)

                    with mock.patch.object(controller, "save", side_effect=interrupt):
                        if state != "interrupted":
                            controller.stage_evidence()
                        else:
                            with self.assertRaises(KeyboardInterrupt):
                                controller.stage_evidence()
                    restarted = w.Controller(controller.root, repo.root, transport, api, now=controller.now)
                    if state == "deleted":
                        destination = next((controller.root / "public").iterdir())
                        for path in destination.iterdir():
                            path.unlink()
                        destination.rmdir()
                    elif state == "expired":
                        restarted.now = lambda: fixtures.NOW + timedelta(days=15)
                    job = copy.deepcopy(next(iter(restarted.session["retention_jobs"].values())))
                    job["created_at"] = restarted.now().isoformat()
                    source = Path(job["entries"][0]["file"]["path"])
                    summary = w.read_input(source)
                    summary["conclusion"] = "y" * 1900
                    source = source.with_name("second-summary.json")
                    w.m.write_json(source, summary)
                    job["entries"] = [{"file": w.m.file_record(source), "kind": "summary",
                                       "conclusion": summary["conclusion"]}]
                    identity = w.m.fingerprint_inputs(job)
                    restarted.session["retention_jobs"][identity] = job
                    restarted.save("session", restarted.session)
                    with self.assertRaisesRegex(ValueError, "budget unavailable" if state == "deleted"
                                                else "aggregate budget exceeded"):
                        restarted._stage_evidence_job(identity, job, None)
                    self.assertFalse((controller.root / "public" / identity).exists())
                    self.assertLessEqual(sum(p.stat().st_size for p in (controller.root / "public").rglob("*")
                                             if p.is_file()), 10000)

    def test_unsealed_acknowledgments_cannot_be_restored(self):
        with self.discovery() as (_, controller, _, transport):
            controller.stage_evidence()
            controller.checkpoint()
            record = controller.session["discovery"]["evidence"][0]
            record.update(availability="available", acknowledgment={"validation": "0" * 64})
            with self.assertRaisesRegex(ValueError, "authenticated custody"):
                controller.checkpoint()
            self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["evidence"][0]["availability"], "pending")

    def test_canonical_discovery_blocks_other_owner_and_page_two_pr(self):
        from test_memory import pr
        for case in ("owner", "page-two"):
            with self.subTest(case=case), self.discovery(canonical=True) as (repo, controller, api, transport):
                other = w.Controller(controller.root.parent / "other", repo.root, transport, api,
                                     now=controller.now)
                if case == "page-two":
                    controller.finish("cancelled", "Owner cancelled explicitly")
                    api.items = [dict(pr(number=1), labels=[], marker=None, attempt_id=None,
                                      head_ref="unrelated", author="contributor", intent_id=None),
                                 dict(pr(), author=w.AUTHOR, intent_id=None)]
                decision = other.preflight(dict(controller.session["discovery_request"], run_id="other"))
                self.assertEqual(decision["action"], "blocked")
                self.assertEqual(len(transport.read()[0]["discoveries"]), 1)

    def test_data_only_parser_rejects_injection_corruption_and_bounds(self):
        valid = json.loads(message("status"))
        for field in ("command", "path", "url", "env", "credentials", "deadline", "policy",
                      "passed_tests", "accepted", "timeout"):
            for target in ("envelope", "data"):
                altered = copy.deepcopy(valid)
                (altered if target == "envelope" else altered["data"])[field] = "$(whoami); hostile PR"
                with self.subTest(field=field, target=target), self.assertRaises(ValueError):
                    bridge.parse_message(json.dumps(altered))
        for invalid in (b"\xff", "{", '{"schema_version":1,"schema_version":1}', "[[[[" * 2000,
                        "x" * (w.e.MAX_PATCH + 65536), message("accept", {"accepted": True}),
                        message("request_dump", {"reason": "shell", "hypothesis": "run command"}),
                        message("request_dump", {"reason": "retention", "hypothesis": ""}),
                        message("submit_patch", {"patch": "x" * (w.e.MAX_PATCH + 1)})):
            with self.subTest(invalid=str(invalid)[:80]), self.assertRaises(ValueError):
                bridge.parse_message(invalid)
        for reason in w.c.REASONS:
            parsed = bridge.parse_message(message("request_dump", {
                "reason": reason, "hypothesis": "Test retention against observed GC and RSS"}))
            self.assertEqual(parsed["data"]["reason"], reason)

    def test_source_export_and_mounts_cannot_expose_controller_or_credentials(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            repo = Repository(root / "controller-source")
            protected = [repo.root, root / "custody", root / "journal", root / "frozen"]
            for path in protected[1:]:
                path.mkdir()
            (repo.root / "private.key").write_text("must not enter exported source")
            repo.git("update-index", "--chmod=+x", SOURCE)
            repo.git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid",
                     "commit", "-qm", "executable source blob")
            repo.sha = repo.git("rev-parse", "HEAD").decode().strip()
            source = root / "agent-source"
            bridge.export_source(repo.root, repo.sha, source, protected)
            self.assertTrue((source / SOURCE).is_file())
            self.assertEqual((source / SOURCE).read_bytes(), repo.git("show", f"{repo.sha}:{SOURCE}"))
            if os.name != "nt":
                self.assertEqual((source / SOURCE).stat().st_mode & 0o111, 0)
            for excluded in (".git", "global.json", "eng", ".github", "private.key"):
                self.assertFalse((source / excluded).exists(), excluded)
            for mode in ("120000", "160000"):
                digest = repo.git("rev-parse", f"{repo.sha}:{SOURCE}").decode().strip() if mode == "120000" else repo.sha
                repo.git("update-index", "--cacheinfo", f"{mode},{digest},{SOURCE}")
                tree = repo.git("write-tree").decode().strip()
                with self.subTest(mode=mode), self.assertRaisesRegex(ValueError, "regular Git blobs"):
                    bridge.export_source(repo.root, tree, root / ("unsafe-" + mode), protected)
            mounts = [{"type": "bind", "source": str(source), "target": "/workspace"},
                      {"type": "tmpfs", "target": "/agent-tmp"}]
            bridge.validate_mounts(source, protected, mounts)
            for extra in (
                    {"type": "bind", "source": str(root), "target": "/tmp"},
                    {"type": "bind", "source": str(protected[0]), "target": "/repo"},
                    {"type": "bind", "source": str(protected[1]), "target": "/custody"},
                    {"type": "bind", "source": str(root / "docker.sock"), "target": "/var/run/docker.sock"},
                    {"type": "bind", "source": str(root / ".git-credentials"), "target": "/credentials"}):
                with self.subTest(mount=extra), self.assertRaises(ValueError):
                    bridge.validate_mounts(source, protected, [*mounts, extra])
            for path in ("eng/scripts/compiler_perf/workflow.py", ".git/config", ".git-credentials", "docker.sock"):
                target = source / path
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_text("agent-controlled")
                with self.subTest(path=path), self.assertRaises(ValueError):
                    bridge.validate_mounts(source, protected, mounts)
                target.unlink()
                parent = target.parent
                while parent != source and not any(parent.iterdir()):
                    parent.rmdir()
                    parent = parent.parent

    def test_scripted_agent_profiles_waits_and_explicitly_finishes(self):
        with self.ready() as (_, controller, api, transport):
            client = bridge.Bridge(controller, launch=self.launch, identify=identify)
            start = message("start_profile")
            first = client.investigate(start)
            self.assertTrue(first["ok"], first)
            self.assertEqual(client.investigate(start), first)
            status = client.investigate(message("status"))
            self.assertEqual(status["status"]["phase"], "awaiting_decision")
            diagnostics = status["status"]["diagnostics"]
            self.assertTrue(diagnostics["stacks"])
            self.assertEqual(diagnostics["metrics"]["allocated_bytes_estimate"], 300000)
            self.assertIsNone(diagnostics["metrics"]["retained_heap_bytes"])
            self.assertFalse(diagnostics["binlog"]["acceptance_eligible"])
            changed = client.investigate(message("cancel", {"reason": "changed ID"}, "start_profile"))
            self.assertFalse(changed["ok"])
            result = client.investigate(message("finish_no_finding", {"reason": "No bounded useful change"}))
            self.assertTrue(result["ok"], result)
            late = client.investigate(message("start_profile", identifier="late"))
            self.assertFalse(late["ok"])
            client.close()
            self.assertEqual(transport.read()[0]["discoveries"]["run-1.discovery"]["outcome"],
                             "completed_no_finding")
            self.assertEqual(api.effects, [])

    def test_worker_early_exit_restart_and_near_deadline_cancel(self):
        for case in ("early", "restart", "deadline"):
            with self.subTest(case=case), self.ready() as (_, controller, _, _):
                worker = Worker(0 if case == "early" else None)

                def launch(arguments, owner):
                    return self.launch(arguments, owner) if arguments[-1] == "finish" else worker

                client = bridge.Bridge(controller, launch=launch, identify=identify)
                result = client.investigate(message("start_profile"))
                self.assertTrue(result["ok"], result)
                if case == "restart":
                    restarted = bridge.Bridge(controller, launch=mock.Mock(), identify=identify)
                    state = restarted.investigate(message("status"))
                    self.assertTrue(state["recovery_required"], state)
                    self.assertFalse(restarted.investigate(message("start_profile", identifier="second"))["ok"])
                    self.assertEqual(worker.signals, [])
                elif case == "deadline":
                    now = controller.now()
                    controller.now = lambda: now + timedelta(seconds=w.ACTIVE_SECONDS)
                client.tick()
                if case != "restart":
                    self.assertIn(controller.status()["outcome"], ("cancelled", "blocked_setup"))
                    self.assertFalse((controller.root / "output").exists())
                client.close()

    def test_scripted_candidate_rejection_and_synthetic_improvement(self):
        for mode in ("correctness-failure", "synthetic-improvement", "retention-failure"):
            with self.subTest(mode=mode), candidate_fixture() as (root, repo, original, patch):
                _, controller, api, transport = self.fixture(root / "interaction", repo)
                self.assertEqual(controller.preflight({
                    "schema_version": 2, "run_id": "run-1", "base_sha": repo.sha,
                    "primary_metric": "elapsed_ms"})["action"], "reserve")
                manifest = w.c.read_bounded_json(original / "workload.json")
                w.m.write_json(controller.root / "workload.json", manifest)
                controller.session.update(phase="ready", ready_at=controller.now().isoformat(),
                                          manifest=w.m.file_record(controller.root / "workload.json"))
                controller.checkpoint()

                def collect(_, __, destination, **kwargs):
                    destination.mkdir()
                    w.m.write_json(destination / "evidence-manifest.json", w.c.read_bounded_json(root / "profile.json"))

                client = bridge.Bridge(controller, launch=self.launch, identify=identify)
                with mock.patch.object(w.c, "collect_profile", side_effect=collect):
                    self.assertTrue(client.investigate(message("start_profile"))["ok"])
                received = client.investigate(message("status"))["status"]["diagnostics"]
                self.assertIn("FSharp.Compiler.Service", received["binlog"]["projects"])
                self.assertTrue(client.investigate(message("select_hypothesis", self.selection(repo)))["ok"])
                self.assertTrue(client.investigate(message("submit_patch", {"patch": patch.decode()}))["ok"])
                if mode == "retention-failure":
                    (controller.root / "private" / "profile" / "compiler.nettrace").write_bytes(b"Nettrace-private")
                if mode == "correctness-failure":
                    with mock.patch.object(w.e, "run_correctness",
                                           side_effect=ValueError("Required check failed: regression-0")):
                        result = client.investigate(message("start_validation"))
                else:
                    result = client.investigate(message("start_validation"))
                self.assertTrue(result["ok"], result)
                result = client.investigate(message("status"))["status"]
                expected = {"synthetic-improvement": "measured_improvement",
                            "retention-failure": "blocked_collection", "correctness-failure": "rejected"}[mode]
                self.assertEqual(result["result"]["outcome"], expected, result)
                self.assertEqual(result["revisions"][0]["outcome"], expected)
                if mode == "synthetic-improvement":
                    payload = controller.validate_output()
                    self.assertEqual(len(payload["items"]), 1)
                    self.assertTrue(payload["items"][0]["draft"])
                    self.assertEqual(len([a for a in transport.read()[0]["attempts"].values()
                                          if a["publication"]]), 1)
                elif mode == "retention-failure":
                    self.assertEqual(controller.session["retention"]["status"], "blocked_retention")
                    with self.assertRaisesRegex(ValueError, "Only an active accepted experiment"):
                        controller.validate_output()
                    self.assertFalse((controller.root / "output").exists())
                else:
                    self.assertFalse(client.investigate(message("finish_no_finding", {"reason": "Hide failure"}))["ok"])
                    self.assertTrue(client.investigate(message(
                        "select_hypothesis", self.selection(repo), identifier="retry-unchanged"))["ok"])
                    skipped = client.investigate(message("status"))["status"]["selection_result"]
                    self.assertEqual(skipped["action"], "skip", skipped)
                    self.assertTrue(client.investigate(message("finish_rejected", {"reason": "Retain failed revision"}))["ok"])
                    self.assertFalse((controller.root / "output").exists())
                    self.assertFalse((controller.candidate_root() / "tree").exists())
                    self.assertEqual((controller.candidate_root() / "proposal.patch").read_bytes(), patch)
                self.assertEqual(api.effects, [])
                client.close()

    def test_real_http_sdk_client_receives_profile_and_rejects_unsafe_requests(self):
        try:
            import httpx
            import uvicorn
            from mcp import ClientSession
            from mcp.client.streamable_http import streamable_http_client
        except ImportError:
            self.skipTest("Install the isolated requirements-agent.txt to exercise HTTP MCP")
        with self.ready() as (_, controller, _, _):
            controller.start_active()
            controller.profile()
            client = bridge.Bridge(controller, launch=self.launch, identify=identify)
            token = "a" * 64
            with self.assertNoLogs("uvicorn.error", level="ERROR"), socket.socket() as listener:
                listener.bind(("127.0.0.1", 0))
                port = listener.getsockname()[1]
                app = bridge.create_app(client, token, allowed_hosts=[f"127.0.0.1:{port}"])
                server = uvicorn.Server(uvicorn.Config(app, log_level="error", lifespan="on"))
                thread = threading.Thread(target=server.run, kwargs={"sockets": [listener]})
                thread.start()
                try:
                    end = time.monotonic() + 10
                    while not server.started and thread.is_alive() and time.monotonic() < end:
                        time.sleep(.02)
                    self.assertTrue(server.started, "Owned HTTP server did not become responsive")

                    async def script():
                        url = f"http://127.0.0.1:{port}/mcp"
                        async with httpx.AsyncClient(headers={"Authorization": "Bearer " + token},
                                                     timeout=60) as http:
                            denied = await http.post(url, headers={"Authorization": "Bearer wrong"}, content="{}")
                            self.assertEqual(denied.status_code, 401)
                            for headers, code in (({"Host": "untrusted.invalid"}, 403),
                                                  ({"Origin": "https://untrusted.invalid"}, 403),
                                                  ([("Authorization", "Bearer " + token),
                                                    ("Authorization", "Bearer " + token)], 400)):
                                denied = await http.post(url, headers=headers, content="{}")
                                self.assertEqual(denied.status_code, code)
                            oversized = await http.post(url, content=b"x" * (w.e.MAX_PATCH * 8))
                            self.assertEqual(oversized.status_code, 413)
                            async with streamable_http_client(url, http_client=http) as (read, write, _):
                                async with ClientSession(read, write) as session:
                                    await session.initialize()
                                    tools = await session.list_tools()
                                    self.assertEqual([tool.name for tool in tools.tools], ["investigate"])
                                    result = await session.call_tool("investigate", {"message": message("status")})
                                    data = result.structuredContent
                                    self.assertIsNotNone(data, result)
                                    self.assertTrue(data["ok"], data)
                                    self.assertTrue(data["status"]["diagnostics"]["stacks"])
                                    extra = await http.post(url, json={
                                        "jsonrpc": "2.0", "id": 17, "method": "tools/call",
                                        "params": {"name": "investigate", "arguments": {
                                            "message": message("status"), "command": "whoami"}}})
                                    self.assertEqual(extra.status_code, 400)
                    asyncio.run(script())
                    self.assertTrue(client.investigate(message(
                        "finish_no_finding", {"reason": "Profile reviewed; no change"}))["ok"])
                finally:
                    server.should_exit = True
                    thread.join(timeout=10)
                    self.assertFalse(thread.is_alive(), "Owned HTTP server failed to stop")


if __name__ == "__main__":
    unittest.main()
