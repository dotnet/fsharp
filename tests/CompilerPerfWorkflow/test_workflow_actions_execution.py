"""Production adapter paths with local tools, Git and fake network boundaries."""

from contextlib import ExitStack
from datetime import timedelta
import io
import json
import os
from pathlib import Path
import shutil
import subprocess
import tarfile
import tempfile
from types import SimpleNamespace
import unittest
from unittest import mock

import test_workflow_actions as fixtures
from test_experiment import SOURCE

actions, w = fixtures.actions, fixtures.w


class ActionsExecutionTests(unittest.TestCase):
    fixture = fixtures.ActionsAdapterTests.fixture
    candidate = fixtures.ActionsAdapterTests.candidate
    discovery = fixtures.ActionsAdapterTests.discovery

    def test_transfer_executes_node_with_current_run_credentials_and_preserves_upload_clock(self):
        with self.discovery(run_id="123") as (_, controller, _, _):
            controller.finish("blocked_setup", "Fixture compact conclusion")
            staged = controller.session["artifacts"][0]
            plan = w.artifacts.retention_result(staged["root"], w.read_input(staged["manifest"]["path"]), controller.now())
            module = controller.root.parent / "transport" / "node_modules" / "@actions" / "artifact"
            module.mkdir(parents=True)
            shutil.copyfile(Path(__file__).parent / "fixtures" / "ActionsArtifact.cjs", module / "index.js")
            (module / "effects.json.lose-ack").touch()
            environment = {"ACTIONS_RUNTIME_TOKEN": "fixture-only", "GITHUB_REPOSITORY": w.mem.REPOSITORY,
                           "ACTIONS_RESULTS_URL": "https://fixture.actions.githubusercontent.com",
                           "GITHUB_RUN_ID": "123"}
            sink = actions.ActionsArtifactSink(controller, environment=environment)
            with tempfile.TemporaryDirectory(dir=controller.root) as directory:
                with self.assertRaisesRegex(ValueError, "upload/readback failed"):
                    sink.transfer(plan, Path(directory))
                started = w.read_input(controller.root / ("artifact-upload-" + plan["manifest_sha256"] + ".json"))
                receipt = sink.transfer(plan, Path(directory))
                self.assertEqual(receipt, {"id": 42, "retention_started_at": started["retention_started_at"]})
                actions.verify_inventory(directory, plan)
                self.assertEqual(w.read_input(module / "effects.json")["uploads"], 1)
                for change in ({"GITHUB_RUN_ID": "124"}, {"GITHUB_REPOSITORY": "fork/fsharp"},
                               {"ACTIONS_RUNTIME_TOKEN": ""}, {"ACTIONS_RESULTS_URL": "https://external.invalid"}):
                    with self.subTest(change=change), self.assertRaises(ValueError):
                        actions.ActionsArtifactSink(controller, environment=dict(environment, **change)).transfer(
                            plan, Path(directory))

    def test_client_retention_clock_survives_server_delay_and_canonical_validation(self):
        with self.discovery(run_id="123") as (_, controller, _, _):
            controller.finish("blocked_setup", "Fixture compact conclusion")
            staged = controller.session["artifacts"][0]
            manifest = w.read_input(staged["manifest"]["path"])
            start = controller.now() + timedelta(minutes=1, microseconds=123456)
            controller.now = lambda: start + timedelta(minutes=2)
            plan = w.artifacts.retention_result(staged["root"], manifest, controller.now())

            def transfer(inventory, destination):
                for record in inventory["files"]:
                    shutil.copyfile(record["path"], destination / Path(record["path"]).name)
                return {"id": 42, "retention_started_at": start.isoformat()}

            sink = actions.ActionsArtifactSink(controller, transport=transfer)
            for delay in (0, 1, 60):
                metadata = {"id": 42, "name": "compiler-perf-" + plan["manifest_sha256"], "expired": False,
                            "workflow_run": {"id": 123}, "size_in_bytes": plan["total_bytes"],
                            "created_at": (start + timedelta(seconds=delay)).replace(microsecond=0).isoformat(),
                            "expires_at": (start + timedelta(days=14)).replace(microsecond=0).isoformat()}
                for change in ({}, {"expires_at": (start + timedelta(days=13)).isoformat()},
                               {"expires_at": (start + timedelta(days=15)).isoformat()},
                               {"created_at": (controller.now() + timedelta(seconds=1)).isoformat()}):
                    with self.subTest(delay=delay, change=change), \
                            mock.patch.object(controller.transport, "scope", "github"), \
                            mock.patch.object(controller.api, "get", create=True, return_value=dict(metadata, **change)):
                        if change:
                            with self.assertRaises(ValueError):
                                w.artifacts.acknowledge_artifacts(staged["root"], manifest, controller.now, sink)
                        else:
                            records = w.artifacts.acknowledge_artifacts(staged["root"], manifest, controller.now, sink)
                            self.assertEqual(records[0]["availability"], "available")
                            self.assertEqual(records[0]["expires_at"], manifest["expires_at"])
                            w.mem.validate_artifact(records[0])
                            w.mem.evidence_extends(manifest["artifacts"], records)
                            self.assertFalse(w.mem.artifact_availability(
                                records[0], w.mem.timestamp(manifest["expires_at"]))["available"])
                            self.assertEqual(controller.acknowledge_evidence(sink)["status"], "available")
                            restarted = w.Controller(controller.root, controller.repository, controller.transport,
                                                     controller.api, now=controller.now)
                            no_upload = mock.Mock(side_effect=AssertionError("Acknowledged evidence must not upload again"))
                            self.assertEqual(restarted.acknowledge_evidence(no_upload)["status"], "available")
                            no_upload.assert_not_called()

    def test_provision_executes_setup_before_installing_active_dependencies(self):
        for failure in (None, "owner", "checksum", "preflight", "pip", "sdk", "deadline"):
            with self.subTest(failure=failure), tempfile.TemporaryDirectory() as directory, ExitStack() as patches:
                base = Path(directory)
                repo, controller, api, transport = self.fixture(base)
                workspace = base / "public"
                workspace.mkdir()
                base.chmod(0o700)
                controller.now = lambda: w.datetime.now(w.timezone.utc)
                started = controller.now() - timedelta(seconds=3601 if failure == "deadline" else 1)
                events = []
                archives = {}
                for name, member in (("python", "python/bin/python3"), ("age", "age/age")):
                    data = io.BytesIO()
                    with tarfile.open(fileobj=data, mode="w:gz") as archive:
                        entry = tarfile.TarInfo(member)
                        entry.size = 7
                        archive.addfile(entry, io.BytesIO(b"fixture"))
                    archives[name] = data.getvalue()
                    patches.enter_context(mock.patch.object(actions, name.upper() + "_SHA",
                        "0" * 64 if failure == "checksum" else actions.hashlib.sha256(data.getvalue()).hexdigest()))

                def download(url, **_):
                    name = "python" if url == actions.PYTHON_URL else "age"
                    events.append(name)
                    return io.BytesIO(archives[name])

                original_run, original_match, is_file, stat = subprocess.run, actions.re.fullmatch, Path.is_file, Path.stat

                def protected_mode(path, **kwargs):
                    info = stat(path, **kwargs)
                    if path != base:
                        return info
                    return SimpleNamespace(**{key: 0o40700 if key == "st_mode" else getattr(info, key)
                                              for key in dir(info) if key.startswith("st_")})

                def run(command, **kwargs):
                    if command[0] in ("git", "powershell.exe"):
                        return original_run(command, **kwargs)
                    if command[0] == str(base / "python" / "bin" / "python3") and "-c" in command:
                        return subprocess.CompletedProcess(command, 0, w.mem.encoded({
                            "executable": str(base / "python" / "bin" / "python3"),
                            "prefix": str(base / "python"), "base_prefix": str(base / "python"),
                            "stdlib": str(base / "python" / "lib")}), b"")
                    events.append("setup" if "setup" in command else "pip" if "pip" in command else command[0])
                    if "setup" in command:
                        self.assertEqual(events, ["python", "setup"])
                        self.assertIn("-I", command)
                        self.assertEqual(command[-1], started.isoformat())
                        if failure == "preflight":
                            api.failure = "403 denied"
                        result = fixtures.runtime.Runtime(controller, workspace).setup(
                            w.read_input(base / "input.json"), started)
                        if result["action"] != "reserve":
                            raise subprocess.CalledProcessError(2, command)
                    elif "pip" in command and failure == "pip":
                        raise subprocess.CalledProcessError(1, command)
                    elif command[0] == "bash":
                        self.assertEqual(command[1:2], ["-c"])
                        self.assertIn("InitializeDotNetCli true", command[2])
                        self.assertIn("InstallDotNetSdk", command[2])
                        self.assertEqual(command[-2:], [str(repo.root / ".dotnet"), "9.0.300"])
                        if failure == "sdk":
                            raise subprocess.CalledProcessError(1, command)
                    return subprocess.CompletedProcess(command, 0)

                environment = {"GH_TOKEN": "fixture", "COMPILER_PERF_OWNER_LOGIN": "profile-owner[bot]",
                    "COMPILER_PERF_AGE_RECIPIENT": "age1" + "a" * 58, "GITHUB_REPOSITORY": w.mem.REPOSITORY,
                    "GITHUB_SHA": repo.sha, "GITHUB_RUN_ID": "123", "GITHUB_WORKSPACE": str(workspace)}
                api.get = lambda _: {"login": "other" if failure == "owner" else "profile-owner[bot]"}
                patches.enter_context(mock.patch.dict(os.environ, environment))
                patches.enter_context(mock.patch.object(actions, "sys", SimpleNamespace(
                    platform="linux", flags=SimpleNamespace(isolated=1))))
                # Only the Linux root spelling/tool presence is mapped for this portable fixture.
                patches.enter_context(mock.patch.object(actions.re, "fullmatch", side_effect=lambda pattern, value:
                    True if pattern == r"/var/lib/compiler-perf/[0-9]+-[0-9]+" and value == str(base)
                    else original_match(pattern, value)))
                patches.enter_context(mock.patch.object(Path, "is_file", lambda path:
                    True if path == Path("/usr/bin/bwrap") else is_file(path)))
                patches.enter_context(mock.patch.object(Path, "stat", protected_mode))
                patches.enter_context(mock.patch.object(actions, "urlopen", side_effect=download))
                patches.enter_context(mock.patch.object(actions.subprocess, "run", side_effect=run))
                github = patches.enter_context(mock.patch.object(w, "GitHub", return_value=api))
                github.get.side_effect = api.get
                patches.enter_context(mock.patch("transport.JournalTransport", return_value=transport))
                encryption = patches.enter_context(mock.patch.object(actions, "verify_retention"))
                if failure:
                    with self.assertRaises((ValueError, subprocess.CalledProcessError)):
                        actions.provision(base, started.isoformat())
                    self.assertNotIn("active", controller.load("clocks") if (controller.root / "clocks.json").exists() else {})
                    if failure != "sdk":
                        self.assertNotIn("npm", events)
                    if failure in ("pip", "sdk"):
                        saved = transport.read()[0]["discoveries"]["123.discovery"]
                        self.assertEqual(saved["outcome"], "blocked_setup")
                    else:
                        encryption.assert_not_called()
                else:
                    actions.provision(base, started.isoformat())
                    self.assertEqual(events, ["python", "setup", "age", "pip", "npm", "bash"])
                    self.assertTrue((workspace / "source" / SOURCE).is_file())
                    saved = w.Controller(controller.root, repo.root, transport, api)
                    self.assertEqual(saved.load("publisher-owner"), {"login": "profile-owner[bot]"})
                    self.assertEqual(saved.load("runtime")["job_started_at"], started.isoformat())
                    self.assertIsNone(saved.session["discovery"]["outcome"])
                    encryption.assert_called_once()

    def test_production_publisher_uses_create_only_push_and_reconciles_configured_owner(self):
        for crash in ("after-branch", "after-pr", None):
            with self.subTest(crash=crash), self.candidate() as (controller, api, transport):
                payload = controller.validate_output()
                controller.save("publisher-owner", {"login": "profile-owner[bot]"})
                controller.save("runtime-container", {"mounts": []})
                adapter = fixtures.runtime.Runtime(controller, controller.root.parent / "public")
                publisher = actions.ActionsPublisher(adapter, controller.root / "publisher")
                original_run, publish = subprocess.run, publisher.publish
                remote, failed = transport.remote, False
                calls = []

                def run(command, **kwargs):
                    nonlocal failed
                    if command[:2] == ["git", "-C"] and "push" in command:
                        item = payload["items"][0]
                        ref = "refs/heads/" + item["branch"]
                        self.assertIn("--force-with-lease=" + ref + ":", command)
                        self.assertEqual(command[-2], "https://github.com/dotnet/fsharp.git")
                        self.assertEqual(command[-1], controller.load("approval")["head"] + ":" + ref)
                        local = list(command)
                        local[-2] = remote
                        with mock.patch.object(transport, "scope", "local_fixture"):
                            kwargs["env"] = transport._environment(network=True)
                        result = original_run(local, **kwargs)
                        self.assertEqual(result.returncode, 0, result.stderr)
                        api.branches[item["branch"]] = controller.load("approval")["head"]
                        api.effects.append("branch")
                        if crash == "after-branch" and not failed:
                            failed = True
                            raise OSError("branch acknowledgment lost")
                        return result
                    if command[:2] == ["gh", "api"]:
                        calls.append(command)
                        endpoint = command[3].split("repos/dotnet/fsharp/", 1)[1]
                        if endpoint.startswith("git/ref/heads/"):
                            head = api.branches.get(endpoint.removeprefix("git/ref/heads/"))
                            status, body = (200, {"object": {"sha": head}}) if head else (404, {})
                        else:
                            self.assertEqual(endpoint, "pulls")
                            item = payload["items"][0]
                            self.assertEqual(json.loads(kwargs["input"]), {
                                **{key: item[key] for key in ("title", "body", "base", "draft")}, "head": item["branch"]})
                            api.publish(payload, controller.load("approval")["head"], controller.session["attempt"]["publication"])
                            api.items[0]["author"] = "profile-owner[bot]"
                            status, body = 201, {"draft": True, "head": {"sha": api.items[0]["head_sha"]},
                                                 "user": {"login": "profile-owner[bot]"}}
                            if crash == "after-pr" and not failed:
                                failed = True
                                raise OSError("PR acknowledgment lost")
                        return subprocess.CompletedProcess(command, 0 if status < 300 else 1,
                            f"HTTP/2.0 {status}\r\n\r\n".encode() + w.mem.encoded(body), b"")
                    if command[0] == "git" and any(op in command for op in ("ls-remote", "fetch", "push")):
                        self.assertFalse(any("://" in arg and arg != "https://github.com/dotnet/fsharp.git"
                                             for arg in command))
                        command = [remote if arg == "https://github.com/dotnet/fsharp.git" else arg for arg in command]
                        with mock.patch.object(transport, "scope", "local_fixture"):
                            kwargs["env"] = transport._environment(network=True)
                    return original_run(command, **kwargs)

                def production(*args):
                    with mock.patch.object(transport, "scope", "github"), \
                            mock.patch.object(transport, "remote", "https://github.com/dotnet/fsharp.git"), \
                            mock.patch.dict(os.environ, {"GH_TOKEN": "fixture-owner-token"}):
                        return publish(*args)

                with mock.patch.object(actions.subprocess, "run", side_effect=run), \
                        mock.patch.object(publisher, "publish", side_effect=production):
                    if crash:
                        with self.assertRaisesRegex(OSError, "acknowledgment lost"):
                            controller.finalize(publisher=publisher)
                        controller = w.Controller(controller.root, controller.repository, transport, api, now=controller.now)
                        adapter.controller = controller
                    self.assertEqual(controller.finalize(publisher=publisher)["action"], "acknowledge")
                    controller.finalize(publisher=publisher)
                self.assertEqual(api.effects, ["branch", "pr"])
                self.assertEqual(len([c for c in calls if c[3].endswith("/pulls")]), 1)
                self.assertEqual(transport.read()[0]["attempts"]["attempt-1"]["publication"]["pr"]["number"], 42)


if __name__ == "__main__":
    unittest.main()
