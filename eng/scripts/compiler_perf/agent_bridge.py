"""Data-only MCP boundary for the protected compiler-performance controller.

The HTTP endpoint uses the official MCP SDK, imported only by create_app(). The
agent sees a disposable Git export, never the controller/helper/journal mounts.
The pinned GH AW/AWF lifecycle is connected by runtime.py. Portable tests are
not evidence that Linux isolation has executed.
"""

import argparse
import asyncio
import copy
import hashlib
import hmac
import os
from pathlib import Path, PurePosixPath
import re
import secrets
import shutil
import signal
import stat
import subprocess
import sys
import threading
import time
from typing import Any

if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import workflow

mem, e, c, require = workflow.mem, workflow.e, workflow.c, workflow.require
MAX_MESSAGE = e.MAX_PATCH + 8192
MAX_HTTP_BODY = 6 * MAX_MESSAGE + 8192
MAX_MESSAGES = 128
MAX_RESPONSE = mem.LIMITS["file_bytes"]
STOP_SECONDS = 20
DISPATCH_SECONDS = 300
PORT = 8765
FIELDS = {
    "status": set(), "start_profile": set(), "start_validation": set(),
    "select_hypothesis": {"compiler_paths", "regressions", "hypothesis", "complexity", "hot_operation"},
    "submit_patch": {"patch"}, "request_dump": {"reason", "hypothesis"},
    "finish_no_finding": {"reason"}, "finish_rejected": {"reason"}, "cancel": {"reason"},
}
SOURCE_ROOTS = ("src/Compiler/", "src/FSharp.Core/", "tests/", "docs/")
SOURCE_SUFFIXES = {".fs", ".fsi", ".fsx", ".cs", ".vb", ".il", ".md", ".txt"}
SOURCE_LIMITS = {"files": 25000, "file_bytes": 4 * 1024 * 1024, "total_bytes": 128 * 1024 * 1024}


def _text(value, limit=4096):
    require(mem.text(value) and len(value.encode("utf-8")) <= limit
            and c.sanitize(value) == value, "Invalid bounded domain text")


def parse_message(raw):
    """Decode a bounded envelope; no command, path-to-patch, clock, or approval inputs."""
    try:
        require(type(raw) in (str, bytes), "Message must be JSON text")
        data = raw.encode("utf-8") if type(raw) is str else raw
        require(0 < len(data) <= MAX_MESSAGE, "Message budget exceeded")
        message = mem.decoded(data.decode("utf-8"))
        require(type(message) is dict and set(message) == {"schema_version", "id", "operation", "data"}
                and type(message["schema_version"]) is int and message["schema_version"] == 1
                and mem.identifier(message["id"]), "Invalid message envelope")
        operation, value = message["operation"], message["data"]
        require(type(operation) is str and operation in FIELDS and type(value) is dict
                and set(value) == FIELDS[operation], "Invalid operation fields")
        if operation == "select_hypothesis":
            require(type(value["hot_operation"]) is int and 0 <= value["hot_operation"] < 256,
                    "Invalid observed stack index")
            request = {k: v for k, v in value.items() if k != "hot_operation"}
            request.update(schema_version=1, attempt_id="input", run_id="input",
                           base_sha="0" * 40, primary_metric=mem.m.PRIMARY_METRICS[0])
            e.validate_request(request)
            _text(value["hypothesis"])
            _text(value["complexity"])
        elif operation == "submit_patch":
            patch = value["patch"]
            require(type(patch) is str and 0 < len(patch.encode("utf-8")) <= e.MAX_PATCH
                    and "\0" not in patch and patch.startswith("diff --git "),
                    "Invalid UTF-8 Git patch")
        elif operation == "request_dump":
            require(value["reason"] in c.REASONS, "Unsupported dump reason")
            _text(value["hypothesis"], 2048)
        elif operation.startswith("finish_") or operation == "cancel":
            _text(value["reason"])
        return message
    except (ValueError, TypeError, KeyError, OverflowError, RecursionError, UnicodeError) as error:
        raise ValueError("Invalid investigation message") from error


def worker_command(controller, operation):
    require(operation in ("profile", "validate", "dump", "select", "finish", "authorize"), "Invalid worker operation")
    return [sys.executable, "-I", str(Path(workflow.__file__).resolve()), "worker",
            "--root", str(controller.root), "--repository", str(controller.repository),
            "--journal", str(controller.transport.journal.repository), "--operation", operation]


def trusted_environment(controller):
    """Host configuration and scratch space must not share AWF's writable home."""
    root = controller.root / "host"
    for path in (root, root / "home", root / "home" / ".config", root / "tmp", root / "packages"):
        require(e.unredirected(path, missing_ok=True), "Redirected trusted process environment")
        if not path.exists():
            c.private_directory(path)
        require(path.is_dir(), "Invalid trusted process environment")
    environment = c.tool_environment()
    environment.update({key: str(root / directory) for key, directory in (
        ("HOME", "home"), ("USERPROFILE", "home"), ("DOTNET_CLI_HOME", "home"),
        ("APPDATA", "home/.config"), ("LOCALAPPDATA", "home/.config"),
        ("XDG_CONFIG_HOME", "home/.config"), ("GH_CONFIG_DIR", "home/.config/gh"),
        ("NUGET_PACKAGES", "packages"), ("TMP", "tmp"), ("TEMP", "tmp"), ("TMPDIR", "tmp"))})
    if sys.platform == "linux":
        environment.update(PATH=os.pathsep.join((str(Path(sys.executable).resolve().parent),
                           "/usr/local/sbin", "/usr/local/bin", "/usr/sbin", "/usr/bin", "/sbin", "/bin")),
                           DOTNET_ROOT=str(controller.repository / ".dotnet"),
                           DOTNET_HOST_PATH=str(controller.repository / ".dotnet" / "dotnet"))
    environment.update({key: os.environ[key] for key in
                        (*workflow.m.RUNNER_KEYS, "PROCESSOR_IDENTIFIER", "GH_TOKEN", "GITHUB_TOKEN",
                         "GITHUB_STEP_SUMMARY", "RUNNER_TEMP")
                        if key in os.environ})
    return environment


def _launch(arguments, controller):
    environment = trusted_environment(controller)
    return subprocess.Popen(arguments, cwd=controller.repository, env=environment, stdin=subprocess.DEVNULL,
                            stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL,
                            start_new_session=True)


class Bridge:
    """One owned child, serialized controller writes, and sealed request receipts."""

    def __init__(self, controller, *, launch=_launch, identify=c.snapshot_process, ready=None):
        require(type(controller.session) is dict, "Prepared controller session is required")
        self.controller, self.launch, self.identify = controller, launch, identify
        self.ready = ready
        self.lock, self.worker = threading.RLock(), None
        self.shutdown_lock = threading.Lock()
        self.stopping, self.thread = threading.Event(), None
        self.closed, self.blocked = False, False
        self.messages = (controller.load("messages") if (controller.root / "messages.json").exists() else {})
        require(type(self.messages) is dict and len(self.messages) <= MAX_MESSAGES,
                "Invalid message receipt inventory")
        for identifier, receipt in self.messages.items():
            require(mem.identifier(identifier) and type(receipt) is dict
                    and set(receipt) == {"digest", "response"} and mem.digest(receipt["digest"])
                    and (receipt["response"] is None or type(receipt["response"]) is dict),
                    "Invalid message receipt")
        self.receipt = (controller.load("worker") if (controller.root / "worker.json").exists() else None)
        require(self.receipt is None or type(self.receipt) is dict
                and self.receipt.get("state") in
                ("launching", "running", "stopping", "completed", "launch_failed", "worker_failed", "cleanup_failed"),
                "Invalid worker receipt")
        # A saved PID is not an owned process handle. Never reattach or signal it.
        # A launch without a completed receipt is deliberately not retried.
        if self.receipt is not None and self.receipt.get("state") != "completed":
            self.blocked = True
        if any(r["response"] is None for r in self.messages.values()):
            self.blocked = True

    @staticmethod
    def error(code):
        return {"ok": False, "error": code}

    def _save_messages(self):
        require(len(mem.encoded(self.messages)) <= mem.LIMITS["file_bytes"] - 2048,
                "Message receipt budget exceeded")
        self.controller.save("messages", self.messages)

    def _worker_receipt(self, state, **fields):
        self.receipt = dict(self.receipt or {}, state=state, **fields)
        self.controller.save("worker", self.receipt)

    def _reload(self):
        self.controller.session = self.controller.load("session")

    def _status(self):
        result = self.controller.status()
        require(type(result) is dict and len(mem.encoded(result)) <= MAX_RESPONSE,
                "Status response budget exceeded")
        response = {"ok": not self.blocked, "status": result, "worker": self.worker is not None,
                    "worker_operation": self.receipt["operation"] if self.worker is not None else None,
                    "recovery_required": self.blocked}
        if self.blocked:
            response["error"] = "owner_reconciliation_required"
        return response

    def _ack(self):
        return {"ok": True, "phase": self.controller.session["phase"], "worker": self.worker is not None}

    def _terminal(self):
        session = self.controller.session
        record = session.get("discovery") or session.get("attempt") or {}
        return session.get("phase") in ("accepted", "finished") or record.get("outcome") is not None

    def _reap(self):
        if self.worker is None or self.worker.poll() is None:
            return
        code = self.worker.wait(timeout=0)
        self.worker = None
        self._reload()
        previous = self.receipt
        operation, session = previous["operation"], self.controller.session
        phase = session["phase"]
        completed = (self._terminal() and operation != "authorize") or (code == 0 and {
            "profile": phase == "awaiting_decision" and type(session.get("profile")) is dict,
            "dump": phase == "awaiting_decision" and type(session.get("dump_result")) is dict,
            "validate": phase in ("accepted", "awaiting_decision")
                        and len(session.get("revisions", [])) > previous["revision_count"],
            "select": phase in ("selected", "awaiting_decision")
                      and type(session.get("selection_result")) is dict,
            "finish": False,
            "authorize": phase == "accepted" and (self.controller.root / "approval.json").exists(),
        }[operation])
        if previous["state"] == "cleanup_failed":
            self.blocked = False
        elif self.blocked:
            self._worker_receipt("worker_failed", exit_status=code)
            return
        pending = previous.get("pending_finish")
        if completed:
            self._worker_receipt("completed", exit_status=code)
        else:
            self._worker_receipt("worker_failed", exit_status=code)
        if self._terminal():
            return
        if pending is not None:
            if self.closed:
                self.blocked = True
                self._worker_receipt("worker_failed", exit_status=code)
            else:
                self._queue_finish(pending["outcome"], pending["reason"], pending["message_id"])
        elif not completed:
            if operation == "finish" or self.closed:
                self.blocked = True
            else:
                self._queue_finish("blocked_setup", "Trusted worker exited before completing its operation")

    def _queue_finish(self, outcome, reason, message_id=None):
        if self._terminal() and not (self.controller.session["phase"] == "accepted"
                                    and not (self.controller.root / "approval.json").exists()):
            return self._ack()
        require(self.worker is None and not self.closed, "Worker still owns the session")
        self.controller.session["finish_request"] = {"outcome": outcome, "reason": reason}
        self.controller.save("session", self.controller.session)
        try:
            self.controller.remaining("finalization", workflow.FINALIZATION_SECONDS, create=True)
        except (KeyboardInterrupt, ValueError, OSError, KeyError, TypeError):
            self.blocked = True
            self.receipt = {"operation": "finish", "message_id": message_id}
            self._worker_receipt("worker_failed")
            return self.error("finalization_unavailable")
        return self._start("finish", message_id)

    def _request_finish(self, outcome, reason, message_id=None):
        if self.worker is not None:
            if self._finalizing():
                return self._ack()
            self._worker_receipt(self.receipt["state"], pending_finish={
                "outcome": outcome, "reason": reason, "message_id": message_id})
            if not self._stop():
                return self.error("cleanup_pending")
            # Reaping may already have launched the queued finalizer.
            if self.worker is not None or self._terminal():
                return self._ack()
        return self._queue_finish(outcome, reason, message_id)

    def _start(self, operation, message_id):
        require(self.worker is None and not self.blocked and not self.closed, "Another worker owns the session")
        self.receipt = {"operation": operation, "message_id": message_id,
                        "revision_count": len(self.controller.session.get("revisions", []))}
        self._worker_receipt("launching")
        try:
            self.worker = self.launch(worker_command(self.controller, operation), self.controller)
            identity = self.identify(self.worker.pid)
            require(identity["pid"] == self.worker.pid and identity["parent_pid"] == os.getpid()
                    and mem.text(identity["start_time"]), "Worker ownership mismatch")
            self._worker_receipt("running", identity=identity)
        except BaseException:
            self.blocked = True
            if self.worker is not None:
                self._stop()
            else:
                self._worker_receipt("launch_failed")
            raise
        return self._ack()

    def _stop(self, *, interrupt=True):
        if self.worker is None:
            return True
        timeout = STOP_SECONDS
        if self._finalizing():
            try:
                timeout = min(timeout, self._read_deadline(
                    self.controller.load("clocks")["finalization"], "finalization", workflow.FINALIZATION_SECONDS))
                interrupt = False
            except KeyboardInterrupt:
                timeout, interrupt = 0, True
        try:
            if self.worker.poll() is None and interrupt and not self.receipt.get("interrupt_sent"):
                # Signal only the original Popen handle. The worker unwinds its
                # candidate and collectors; their separate sessions are not ours.
                self.worker.send_signal(signal.SIGINT)
                self._worker_receipt("stopping", interrupt_sent=True)
            self.worker.wait(timeout=timeout)
        except (OSError, subprocess.TimeoutExpired) as error:
            if isinstance(error, subprocess.TimeoutExpired) and self._finalizing():
                try:
                    self._read_deadline(self.controller.load("clocks")["finalization"],
                                        "finalization", workflow.FINALIZATION_SECONDS)
                except KeyboardInterrupt:
                    pass
                else:
                    # A short dispatch/stop wait is not the worker's cleanup deadline.
                    return False
            self.blocked = True
            self._worker_receipt("cleanup_failed")
            return False
        self._reap()
        return True

    def _read_deadline(self, record, name, allowance):
        start, deadline, last = (mem.timestamp(record[k]) for k in ("start", "deadline", "last_seen"))
        require((deadline - start).total_seconds() == allowance, "Protected deadline differs")
        require(type(record["remaining_seconds"]) in (int, float)
                and 0 <= record["remaining_seconds"] <= allowance, "Invalid protected remaining time")
        now, tick = self.controller.now(), self.controller.clock()
        remaining = min((deadline - now).total_seconds(),
                        record["remaining_seconds"] - (now - last).total_seconds())
        if name == "finalization" and (self.controller.root / "runtime-engine-exit.json").exists():
            exited = mem.timestamp(self.controller.load("runtime-engine-exit")["at"])
            remaining = min(remaining, allowance - (now - exited).total_seconds())
        origin, initial = self.controller.timers.setdefault(name, (tick, remaining))
        remaining = min(remaining, initial - (tick - origin))
        if now < last or now < start or tick < origin or remaining <= 0:
            raise KeyboardInterrupt
        return remaining

    def _finalizing(self, clocks=None):
        if self.worker is None:
            return False
        if self.receipt["operation"] in ("finish", "authorize"):
            return True
        if clocks is None:
            clocks = (self.controller.load("clocks")
                      if (self.controller.root / "clocks.json").exists() else {})
        return "finalization" in clocks

    def _poll(self):
        self._reap()
        if self.blocked:
            return
        clocks = (self.controller.load("clocks")
                  if (self.controller.root / "clocks.json").exists() else {})
        if self._finalizing(clocks):
            try:
                self._read_deadline(clocks["finalization"], "finalization", workflow.FINALIZATION_SECONDS)
            except KeyboardInterrupt:
                self._stop()
            return
        if self._terminal():
            return
        if "active" not in clocks:
            if self.controller.session["phase"] == "ready":
                ready = mem.timestamp(self.controller.session["ready_at"])
                elapsed = (self.controller.now() - ready).total_seconds()
                if not 0 <= elapsed < workflow.STARTUP_SECONDS:
                    self._queue_finish("blocked_setup", "Agent readiness exhausted startup allowance")
            return
        try:
            if self.worker is None:
                self.controller.poll_active()
            else:
                # This branch only reads clocks: the worker owns clock/session
                # writes until it has exited and its sealed session is reloaded.
                self._read_deadline(clocks["active"], "active", workflow.ACTIVE_SECONDS)
        except KeyboardInterrupt:
            self._request_finish("cancelled", "Active investigation deadline or clock anomaly")

    def tick(self):
        with self.lock:
            if self.closed:
                return
            try:
                self._poll()
            except (ValueError, OSError, KeyError, TypeError, RecursionError, subprocess.SubprocessError):
                self.blocked = True
                self._stop()

    def start_watchdog(self):
        with self.lock:
            require(self.thread is None and not self.closed, "Watchdog already started")
            def watch():
                while not self.stopping.wait(1):
                    self.tick()
            self.thread = threading.Thread(target=watch, name="compiler-perf-deadline", daemon=True)
            self.thread.start()

    def close(self):
        self.stopping.set()
        with self.lock:
            self.closed = True
            try:
                interrupt = not self._finalizing()
            except (ValueError, OSError, KeyError, TypeError):
                self.blocked, interrupt = True, True
            stopped = self._stop(interrupt=interrupt)
            if not stopped:
                self.blocked = True
                self._worker_receipt("cleanup_failed")
        if self.thread is not None and self.thread is not threading.current_thread():
            self.thread.join(timeout=STOP_SECONDS + 2)
        return stopped and not self.blocked

    def shutdown(self, reason):
        """Owned service cancellation, not adoption or signalling of a saved PID."""
        with self.shutdown_lock:
            if self.closed:
                return not self.blocked
            with self.lock:
                self._reload()
                self._reap()
                unfinished = (not self._terminal() or self.controller.session["phase"] == "accepted"
                              and not (self.controller.root / "approval.json").exists())
                if unfinished and not self.blocked:
                    self._request_finish("cancelled", reason)
            while not self.blocked:
                with self.lock:
                    self._reap()
                    if self.worker is None:
                        if (self.controller.session["phase"] == "accepted"
                                and not (self.controller.root / "approval.json").exists()):
                            self._queue_finish("cancelled", reason)
                        else:
                            break
                self.tick()
                if not self.blocked:
                    time.sleep(.1)
            return self.close()

    def investigate(self, message):
        try:
            envelope = parse_message(message)
        except ValueError:
            return self.error("invalid_message")
        digest = hashlib.sha256(message.encode("utf-8") if type(message) is str else message).hexdigest()
        identifier, operation, data = envelope["id"], envelope["operation"], envelope["data"]
        with self.lock:
            if self.closed:
                return self.error("endpoint_closed")
            previous = self.messages.get(identifier)
            if previous is not None:
                if previous["digest"] != digest:
                    return self.error("message_id_reused")
                return copy.deepcopy(previous["response"] or self.error("incomplete_request"))
            try:
                if self.ready is not None and not self.blocked and not self._terminal():
                    self.ready()
                    self.ready = None
                self._poll()
                # Status is an observation, not a persistent mutation receipt.
                # Fresh status IDs never consume the bounded mutation history.
                if operation == "status":
                    return self._status()
                if self.blocked:
                    return self.error("owner_reconciliation_required")
                if self._terminal():
                    return self.error("investigation_finished")
                if self.worker is not None and operation != "cancel":
                    return self.error("worker_busy")
                require(len(self.messages) < MAX_MESSAGES
                        and len(mem.encoded(self.messages)) + 8192
                        < mem.LIMITS["file_bytes"], "Message receipt budget exceeded")
                self.messages[identifier] = {"digest": digest, "response": None}
                self._save_messages()
                if operation == "start_profile":
                    self.controller.start_active()
                    response = self._start("profile", identifier)
                elif operation == "start_validation":
                    require(self.controller.session["phase"] == "patch_submitted", "Submit a patch before validation")
                    response = self._start("validate", identifier)
                elif operation == "request_dump":
                    require(self.controller.session["phase"] == "awaiting_decision"
                            and not self.controller.session.get("dump_result"), "Dump replay is unavailable")
                    self.controller.session["dump_request"] = copy.deepcopy(data)
                    self.controller.save("session", self.controller.session)
                    response = self._start("dump", identifier)
                elif operation == "select_hypothesis":
                    require(self.controller.session["phase"] == "awaiting_decision",
                            "Complete diagnostics before selection")
                    self.controller.session["selection"] = copy.deepcopy(data)
                    self.controller.session.pop("selection_result", None)
                    self.controller.save("session", self.controller.session)
                    response = self._start("select", identifier)
                elif operation == "submit_patch":
                    selection = self.controller.session.get("selection_result")
                    require(selection is None or type(selection) is dict and selection.get("action") == "selected",
                            "A blocked selection cannot submit a patch")
                    self.controller.submit_patch(data["patch"])
                    response = self._ack()
                else:
                    if operation == "finish_rejected":
                        outcome = (self.controller.session.get("attempt") or {}).get("outcome")
                        require(outcome in mem.OUTCOMES - {"measured_improvement", "completed_no_finding"},
                                "A recorded negative attempt is required")
                    else:
                        outcome = {"finish_no_finding": "completed_no_finding", "cancel": "cancelled"}[operation]
                        if operation == "finish_no_finding":
                            require(self.controller.session["phase"] == "awaiting_decision"
                                    and self.controller.session.get("attempt") is None
                                    and not self.controller.session.get("revisions"),
                                    "No-finding requires an untested completed profile")
                    response = self._request_finish(outcome, data["reason"], identifier)
            except KeyboardInterrupt:
                self._request_finish("cancelled", "Active investigation deadline or cancellation", identifier)
                response = self.error("deadline_reached")
            except (ValueError, OSError, KeyError, TypeError, OverflowError, RecursionError,
                    UnicodeError, subprocess.SubprocessError):
                response = self.error("operation_rejected")
            if identifier in self.messages:
                self.messages[identifier]["response"] = copy.deepcopy(response)
                try:
                    self._save_messages()
                except (ValueError, OSError, TypeError, RecursionError):
                    self.blocked = True
                    return self.error("receipt_persistence_failed")
            return response


def _overlap(left, right):
    return left == right or left.is_relative_to(right) or right.is_relative_to(left)


def _local_directory(path, *, missing=False):
    path = Path(path).absolute()
    require(e.unredirected(path, missing_ok=missing), "Redirected isolation path")
    require(missing or path.is_dir(), "Isolation path must be a directory")
    return path.resolve()


def validate_mounts(source, protected_roots, mounts):
    """Validate the actual mount inventory, not Unix permission bits."""
    source = _local_directory(source)
    protected = [_local_directory(p, missing=True) for p in protected_roots]
    require(len(protected) >= 4 and all(not _overlap(source, p) for p in protected),
            "Agent source overlaps protected custody")
    require(type(mounts) is list and len(mounts) == 2, "Unexpected agent mount inventory")
    bind, scratch = mounts
    require(type(bind) is dict and set(bind) == {"type", "source", "target"}
            and bind["type"] == "bind" and bind["target"] == "/workspace"
            and _local_directory(bind["source"]) == source, "Only disposable source may be bound")
    require(type(scratch) is dict and scratch == {"type": "tmpfs", "target": "/agent-tmp"},
            "Agent scratch must be a private tmpfs")
    require(not any(character in str(source) for character in ",\r\n\0"),
            "Unsafe Docker mount source")
    count, total = 0, 0
    for path in source.rglob("*"):
        info = path.lstat()
        require(not stat.S_ISLNK(info.st_mode)
                and not getattr(info, "st_file_attributes", 0) & stat.FILE_ATTRIBUTE_REPARSE_POINT,
                "Redirected agent source entry")
        relative = path.relative_to(source).as_posix()
        require(not any(p.startswith(".") and p != ".FSharp.Compiler.Service"
                        for p in PurePosixPath(relative).parts), "Private agent source entry")
        if stat.S_ISDIR(info.st_mode):
            continue
        count, total = count + 1, total + info.st_size
        require(stat.S_ISREG(info.st_mode) and _public_path(relative)
                and info.st_size <= SOURCE_LIMITS["file_bytes"] and total <= SOURCE_LIMITS["total_bytes"]
                and count <= SOURCE_LIMITS["files"], "Agent source contains nonpublic or oversized files")
    return mounts


def _public_path(path):
    try:
        e.relative_path(path)
    except ValueError:
        return False
    parts = PurePosixPath(path).parts
    return (path.startswith(SOURCE_ROOTS) and PurePosixPath(path).suffix in SOURCE_SUFFIXES
            and not any(p.startswith(".") and p != ".FSharp.Compiler.Service" for p in parts)
            and not any(p.lower() in {"credentials", "secrets", "node_modules", "bin", "obj"}
                        for p in parts))


def export_source(repository, revision, destination, protected_roots):
    """Export allowlisted regular Git blobs; dirty, untracked and Git data never enter."""
    repository = _local_directory(repository)
    destination = _local_directory(destination, missing=True)
    protected = [_local_directory(p, missing=True) for p in (*protected_roots, repository)]
    require(mem.digest(revision, 40) and not destination.exists()
            and all(not _overlap(destination, p) for p in protected), "Unsafe source export")
    environment = c.tool_environment()
    environment.update(GIT_CONFIG_NOSYSTEM="1", GIT_CONFIG_GLOBAL=os.devnull, GIT_NO_REPLACE_OBJECTS="1")

    def git(*arguments):
        result = subprocess.run(
            ["git", "-c", "core.hooksPath=" + os.devnull, "-c", "core.fsmonitor=false",
             "-c", "credential.helper=", *arguments], cwd=repository, env=environment,
            stdin=subprocess.DEVNULL, capture_output=True, timeout=60, check=False)
        require(result.returncode == 0, "Source export Git read failed")
        return result.stdout

    tree = git("ls-tree", "-r", "-z", "-l", revision, "--", *SOURCE_ROOTS)
    require(len(tree) <= 8 * 1024 * 1024, "Source tree inventory budget exceeded")
    records, total = [], 0
    for record in filter(None, tree.split(b"\0")):
        header, raw_path = record.split(b"\t", 1)
        path = raw_path.decode("utf-8")
        if not _public_path(path):
            continue
        mode, kind, digest, raw_size = header.decode("ascii").split()
        require(mode in ("100644", "100755") and kind == "blob" and mem.digest(digest, 40),
                "Source export requires regular Git blobs")
        size = int(raw_size)
        total += size
        require(0 <= size <= SOURCE_LIMITS["file_bytes"] and total <= SOURCE_LIMITS["total_bytes"]
                and len(records) < SOURCE_LIMITS["files"], "Source export budget exceeded")
        records.append((path, digest, size))
    require(records, "Source export is empty")
    destination.mkdir(mode=0o755)
    try:
        for path, digest, size in records:
            content = git("cat-file", "blob", digest)
            require(len(content) == size, "Source blob size changed")
            target = destination.joinpath(*PurePosixPath(path).parts)
            target.parent.mkdir(parents=True, exist_ok=True)
            with target.open("xb") as stream:
                stream.write(content)
            target.chmod(0o644)
    except BaseException:
        shutil.rmtree(destination)
        raise
    return {"files": len(records), "bytes": total, "revision": revision}


class HttpBoundary:
    """Authenticate and cap the complete HTTP body before the SDK JSON decoder."""

    def __init__(self, app, bridge, token, allowed_hosts, shutdown=None):
        require(type(token) is str and re.fullmatch(r"[A-Za-z0-9_-]{32,128}", token),
                "Invalid endpoint capability")
        self.app, self.bridge, self.token = app, bridge, token
        self.allowed_hosts = frozenset(allowed_hosts)
        self.shutdown = shutdown or bridge.close

    async def _reject(self, send, code):
        body = b'{"error":"request_rejected"}'
        await send({"type": "http.response.start", "status": code,
                    "headers": [(b"content-type", b"application/json"), (b"content-length", str(len(body)).encode())]})
        await send({"type": "http.response.body", "body": body})

    async def __call__(self, scope, receive, send):
        if scope["type"] == "lifespan":
            self.bridge.start_watchdog()
            cleanup = None

            async def close_owned_bridge():
                nonlocal cleanup
                if cleanup is None:
                    cleanup = asyncio.create_task(asyncio.to_thread(self.shutdown))
                return await asyncio.shield(cleanup)

            async def lifespan_send(event):
                if event["type"] in ("lifespan.shutdown.complete", "lifespan.shutdown.failed",
                                     "lifespan.startup.failed"):
                    stopped = await close_owned_bridge()
                    if event["type"] == "lifespan.shutdown.complete" and not stopped:
                        event = {"type": "lifespan.shutdown.failed", "message": "Owned worker cleanup incomplete"}
                await send(event)

            try:
                await self.app(scope, receive, lifespan_send)
            finally:
                if cleanup is None or not cleanup.done():
                    await close_owned_bridge()
            return
        if scope["type"] != "http":
            await send({"type": "websocket.close", "code": 1008})
            return
        if scope.get("path") != "/mcp" or scope.get("query_string") or scope.get("method") not in ("POST", "GET", "DELETE"):
            await self._reject(send, 404)
            return
        headers = {}
        for key, value in scope.get("headers", []):
            key = key.lower()
            if key in headers and key in (b"host", b"authorization", b"content-length", b"origin"):
                await self._reject(send, 400)
                return
            headers[key] = value
        if b"origin" in headers or headers.get(b"host", b"").decode("latin-1") not in self.allowed_hosts:
            await self._reject(send, 403)
            return
        if not hmac.compare_digest(headers.get(b"authorization", b""), ("Bearer " + self.token).encode("ascii")):
            await self._reject(send, 401)
            return
        length = headers.get(b"content-length")
        if length is not None and (not re.fullmatch(rb"[0-9]{1,9}", length) or int(length) > MAX_HTTP_BODY):
            await self._reject(send, 413)
            return
        buffer, size, deadline = bytearray(), 0, time.monotonic() + 30
        try:
            while True:
                event = await asyncio.wait_for(receive(), max(0, deadline - time.monotonic()))
                if event["type"] == "http.disconnect":
                    return
                part = event.get("body", b"")
                size += len(part)
                if size > MAX_HTTP_BODY:
                    await self._reject(send, 413)
                    return
                buffer.extend(part)
                if not event.get("more_body", False):
                    break
        except (TimeoutError, asyncio.TimeoutError):
            await self._reject(send, 408)
            return
        if length is not None and int(length) != size:
            await self._reject(send, 400)
            return
        body, delivered = bytes(buffer), False
        if scope["method"] == "POST":
            try:
                request = mem.decoded(body)
                require(type(request) is dict, "Invalid MCP envelope")
                if request.get("method") == "tools/call":
                    parameters = request.get("params")
                    require(type(parameters) is dict and set(parameters) <= {"name", "arguments", "_meta"}
                            and parameters.get("name") == "investigate", "Invalid MCP tool")
                    arguments = parameters.get("arguments")
                    require(type(arguments) is dict and set(arguments) == {"message"}
                            and type(arguments["message"]) is str, "Invalid MCP tool arguments")
            except (ValueError, TypeError, UnicodeError, OverflowError, RecursionError):
                await self._reject(send, 400)
                return

        async def replay():
            nonlocal delivered
            if not delivered:
                delivered = True
                return {"type": "http.request", "body": body, "more_body": False}
            return await receive()

        await self.app(scope, replay, send)


def create_app(bridge, token, *, allowed_hosts=None, shutdown=None):
    from mcp.server.fastmcp import FastMCP
    from mcp.server.transport_security import TransportSecuritySettings

    # GH AW v0.76.1 rewrites loopback URLs to this alias inside its host-network gateway.
    hosts = allowed_hosts or (f"127.0.0.1:{PORT}", f"localhost:{PORT}", f"host.docker.internal:{PORT}")
    server = FastMCP("compiler-performance", stateless_http=True, json_response=True,
                     streamable_http_path="/mcp", max_request_body_size=MAX_HTTP_BODY,
                     transport_security=TransportSecuritySettings(
                         enable_dns_rebinding_protection=True, allowed_hosts=list(hosts), allowed_origins=[]))
    pending = set()

    @server.tool(structured_output=True)
    async def investigate(message: str) -> dict[str, Any]:
        """Send one bounded compiler-investigation JSON envelope; never commands or credentials."""
        if len(pending) >= 8:
            return Bridge.error("dispatch_busy")
        dispatch = asyncio.create_task(asyncio.to_thread(bridge.investigate, message))
        pending.add(dispatch)

        def completed(task):
            pending.discard(task)
            if not task.cancelled():
                task.exception()

        dispatch.add_done_callback(completed)
        try:
            return await asyncio.wait_for(asyncio.shield(dispatch), DISPATCH_SECONDS)
        except (TimeoutError, asyncio.TimeoutError):
            # The original dispatch and sealed message receipt remain owned.
            # Retry the identical ID or observe status; never launch a replacement.
            return Bridge.error("dispatch_pending")
        except Exception:
            return Bridge.error("operation_failed")

    return HttpBoundary(server.streamable_http_app(), bridge, token, hosts, shutdown)


def endpoint_token(root):
    path = Path(root) / "endpoint-key"
    require(e.unredirected(path, missing_ok=True), "Redirected endpoint capability")
    if not path.exists():
        descriptor = os.open(path, os.O_CREAT | os.O_EXCL | os.O_WRONLY, 0o600)
        with os.fdopen(descriptor, "w", encoding="ascii") as stream:
            stream.write(secrets.token_urlsafe(32))
    require(path.is_file() and path.stat().st_size <= 128, "Invalid endpoint capability")
    if os.name != "nt":
        require(path.stat().st_uid == os.getuid() and stat.S_IMODE(path.stat().st_mode) == 0o600,
                "Endpoint capability must remain protected")
    token = path.read_text(encoding="ascii")
    require(re.fullmatch(r"[A-Za-z0-9_-]{32,128}", token), "Invalid endpoint capability")
    return token


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", required=True)
    parser.add_argument("--repository", required=True)
    parser.add_argument("--journal", required=True)
    args = parser.parse_args(argv)
    try:
        require(sys.platform == "linux", "Production agent endpoint requires Linux")
        from transport import JournalTransport
        import uvicorn

        controller = workflow.Controller(args.root, args.repository, JournalTransport(args.journal), workflow.GitHub())
        server_root = controller.root / "agent-server"
        require(e.unredirected(server_root, missing_ok=True), "Redirected endpoint owner directory")
        server_root.mkdir(mode=0o700, exist_ok=True)
        with e.owned_lock(server_root):
            bridge = Bridge(controller)
            app = create_app(bridge, endpoint_token(controller.root))
            uvicorn.run(app, host="127.0.0.1", port=PORT, access_log=False, log_level="warning",
                        proxy_headers=False, server_header=False, limit_concurrency=8)
            return 0 if bridge.close() else 2
    except KeyboardInterrupt:
        return 130
    except (ValueError, OSError, KeyError, TypeError, ImportError, RecursionError, subprocess.SubprocessError):
        print('{"ok":false,"error":"endpoint_unavailable"}')
        return 2


if __name__ == "__main__":
    sys.exit(main())
