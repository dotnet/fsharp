from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
import json
import os
from pathlib import Path
import shutil
import stat
import subprocess
import tempfile
import threading
import time
import unittest
import zipfile


SCRIPT = Path(__file__).resolve().parents[1] / "scripts" / "fetch-build-binlogs.fsx"
LOAD_SCRIPT = f'#load @"{SCRIPT}"\nopen FSharp.Data\nopen ``Fetch-build-binlogs``\n'
VALIDATE = LOAD_SCRIPT + r"""
let cases = System.IO.File.ReadAllText(fsi.CommandLineArgs[1]) |> JsonValue.Parse
for case in cases.AsArray() do
    let accepted =
        try
            let head, merge = validateBuild "20727" "42" case?build
            validatePr head merge case?pr |> ignore
            true
        with Skip _ -> false
    printfn "%s %b" (case?name.AsString()) accepted
"""
FETCH = LOAD_SCRIPT + r"""
try
    fetch false fsi.CommandLineArgs[1] 2. (fun response cancellation ->
        response.content.ReadAsStringAsync(cancellation).GetAwaiter().GetResult())
    |> printfn "%s"
with error ->
    eprintfn "%s" error.Message
    exit 1
"""


def write_archive(path, entries):
    with zipfile.ZipFile(path, "w") as archive:
        for name, mode, content in entries:
            info = zipfile.ZipInfo(name)
            info.create_system = 3
            info.external_attr = mode << 16
            archive.writestr(info, content)


class FetchBuildBinlogsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.dotnet = shutil.which("dotnet")
        if not cls.dotnet:
            raise RuntimeError("dotnet is required.")

    def run_script(self, directory, *arguments, script=SCRIPT, env=None):
        return subprocess.run(
            [self.dotnet, "fsi", "--exec", str(script), "--", *map(str, arguments)],
            capture_output=True,
            text=True,
            cwd=directory,
            env=os.environ if env is None else env,
            timeout=120,
        )

    def extract(self, entries, budget=1024 * 1024, label="../../Leg"):
        with tempfile.TemporaryDirectory(prefix="fsharp-binlog-extract-") as directory:
            root = Path(directory)
            archive = root / "artifact.zip"
            destination = root / "output"
            write_archive(archive, entries)
            result = self.run_script(root, "--extract", archive, destination, "7", budget, label)
            files = {
                path.name: path.read_bytes()
                for path in destination.glob("*")
                if path.is_file()
            } if destination.exists() else {}
            return result, files

    def validate_url(self, url):
        with tempfile.TemporaryDirectory(prefix="fsharp-binlog-url-") as directory:
            return self.run_script(directory, "--validate-url", url)

    def test_extracts_regular_binlogs_to_generated_names(self):
        result, files = self.extract(
            [
                ("nested/build.binlog", stat.S_IFREG | 0o644, b"binlog"),
                ("nested/build.proto.binlog", stat.S_IFREG | 0o644, b"proto"),
                ("nested/readme.txt", stat.S_IFREG | 0o644, b"text"),
            ]
        )
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertEqual(result.stdout.strip(), "1 6")
        self.assertEqual(files, {"7_0_Leg.binlog": b"binlog"})

    def test_rejects_unsafe_paths_and_entry_types(self):
        cases = (
            ("traversal", "../escape.binlog", stat.S_IFREG | 0o644),
            ("absolute", "/escape.binlog", stat.S_IFREG | 0o644),
            ("drive", r"C:\escape.binlog", stat.S_IFREG | 0o644),
            ("backslash-root", r"\escape.binlog", stat.S_IFREG | 0o644),
            ("backslash-traversal", r"nested\..\escape.binlog", stat.S_IFREG | 0o644),
            ("symlink", "link.binlog", stat.S_IFLNK | 0o777),
            ("device", "device.binlog", stat.S_IFCHR | 0o600),
            ("fifo", "pipe.binlog", stat.S_IFIFO | 0o600),
            ("ignored-traversal", "../readme.txt", stat.S_IFREG | 0o644),
        )
        for name, entry, mode in cases:
            with self.subTest(name=name):
                result, files = self.extract([(entry, mode, b"target")])
                self.assertNotEqual(result.returncode, 0)
                self.assertEqual(files, {})

    def test_budget_failure_removes_partial_outputs(self):
        result, files = self.extract(
            [
                ("first.binlog", stat.S_IFREG | 0o644, b"ok"),
                ("second.binlog", stat.S_IFREG | 0o644, b"too-large"),
            ],
            budget=4,
        )
        self.assertNotEqual(result.returncode, 0)
        self.assertEqual(files, {})

    def test_archive_limits_are_enforced_before_extraction(self):
        cases = (
            ("entries", 65537, lambda i: f"file-{i}.txt"),
            ("binlogs", 257, lambda i: f"file-{i}.binlog"),
            ("metadata", 260, lambda i: "x" * 65000 + str(i)),
        )
        for limit, count, name in cases:
            with self.subTest(limit=limit):
                result, files = self.extract(
                    [(name(i), stat.S_IFREG | 0o644, b"") for i in range(count)]
                )
                self.assertNotEqual(result.returncode, 0)
                self.assertIn(limit, result.stderr.lower())
                self.assertEqual(files, {})

    def test_accepts_documented_artifact_hosts_only(self):
        artifact_path = (
            "/A6fcc92e5-73a7-4f88-8d13-d9045b45fb27/"
            "cbb18261-c48f-4abb-8651-8cdcb5474649/artifact"
        )
        cases = (
            ("compact-region", f"https://artprodeus21.artifacts.visualstudio.com{artifact_path}", True),
            ("dotted-region", f"https://artprod.eus21.artifacts.visualstudio.com{artifact_path}", True),
            ("hyphenated-region", f"https://artprod-weu.artifacts.visualstudio.com{artifact_path}", True),
            ("azure-project", "https://dev.azure.com/dnceng-public/public/_apis/build/builds/1/artifacts", True),
            ("azure-project-id", "https://dev.azure.com/dnceng-public/cbb18261-c48f-4abb-8651-8cdcb5474649/_apis/build/builds/1/artifacts", True),
            ("http", f"http://artprodeus21.artifacts.visualstudio.com{artifact_path}", False),
            ("nondefault-port", f"https://artprodeus21.artifacts.visualstudio.com:444{artifact_path}", False),
            ("wrong-organization", "https://dev.azure.com/other/public/_apis/build", False),
            ("project-lookalike", "https://dev.azure.com/dnceng-public/public.evil/_apis/build", False),
            ("wrong-project", "https://artprod.eus21.artifacts.visualstudio.com/A6fcc92e5-73a7-4f88-8d13-d9045b45fb27/00000000-0000-0000-0000-000000000000/artifact", False),
            ("lookalike-host", f"https://artprod.eus21.artifacts.visualstudio.com.evil.example{artifact_path}", False),
            ("wrong-prefix", f"https://evilartprod.eus21.artifacts.visualstudio.com{artifact_path}", False),
        )
        for name, url, accepted in cases:
            with self.subTest(name=name):
                result = self.validate_url(url)
                self.assertEqual(result.returncode == 0, accepted, result.stderr)

    def test_build_and_current_revision_must_match(self):
        head, merge = "a" * 40, "b" * 40
        baseline = {
            "build": {
                "id": 42, "definition": {"id": 90},
                "repository": {"id": "dotnet/fsharp"},
                "status": "completed", "result": "failed",
                "sourceBranch": "refs/pull/20727/merge",
                "triggerInfo": {"pr.number": "20727", "pr.sourceSha": head},
                "sourceVersion": merge,
            },
            "pr": {
                "state": "open",
                "head": {"sha": head, "ref": "fix-branch", "repo": {"full_name": "dotnet/fsharp"}},
                "merge_commit_sha": merge,
            },
        }
        cases = (
            ("current", None, [], None),
            ("wrong-id", "build", ["id"], 43),
            ("wrong-definition", "build", ["definition", "id"], 91),
            ("wrong-repo", "build", ["repository", "id"], "dotnet/runtime"),
            ("running", "build", ["status"], "inProgress"),
            ("succeeded", "build", ["result"], "succeeded"),
            ("wrong-branch", "build", ["sourceBranch"], "refs/pull/1/merge"),
            ("wrong-pr", "build", ["triggerInfo", "pr.number"], "1"),
            ("missing-built-head", "build", ["triggerInfo", "pr.sourceSha"], None),
            ("closed", "pr", ["state"], "closed"),
            ("moved-head", "pr", ["head", "sha"], "c" * 40),
            ("moved-merge", "pr", ["merge_commit_sha"], "d" * 40),
            ("missing-merge", "pr", ["merge_commit_sha"], None),
            ("output-injection", "pr", ["head", "ref"], "branch\nbinlog-found=true"),
        )
        fixtures = []
        for name, section, path, value in cases:
            fixture = json.loads(json.dumps(baseline))
            fixture["name"] = name
            if section:
                node = fixture[section]
                for key in path[:-1]:
                    node = node[key]
                node[path[-1]] = value
            fixtures.append(fixture)
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            harness, inputs = root / "validate.fsx", root / "inputs.json"
            harness.write_text(VALIDATE, encoding="utf-8")
            inputs.write_text(json.dumps(fixtures), encoding="utf-8")
            result = self.run_script(root, inputs, script=harness)
            self.assertEqual(result.returncode, 0, result.stderr)
            self.assertEqual(
                result.stdout.splitlines(),
                [f"{name} {str(name == 'current').lower()}" for name, *_ in cases],
            )

    def test_http_redirect_retry_and_body_deadline(self):
        for scenario in ("redirect", "retry", "deadline"):
            with self.subTest(scenario=scenario), tempfile.TemporaryDirectory() as directory:
                requests = []

                class Handler(BaseHTTPRequestHandler):
                    def do_GET(self):
                        requests.append(dict(self.headers))
                        if scenario == "redirect" and self.path != "/escaped":
                            self.send_response(302)
                            self.send_header("Location", f"http://127.0.0.1:{self.server.server_port}/escaped")
                        elif scenario == "retry" and len(requests) < 3:
                            self.send_response(503)
                        else:
                            self.send_response(200)
                        self.end_headers()
                        if scenario == "deadline":
                            self.wfile.flush()
                            time.sleep(3)
                        elif scenario != "redirect" or self.path == "/escaped":
                            self.wfile.write(b"recovered")

                    def log_message(self, *_):
                        pass

                server = ThreadingHTTPServer(("127.0.0.1", 0), Handler)
                worker = threading.Thread(target=server.serve_forever, daemon=True)
                worker.start()
                try:
                    root = Path(directory)
                    harness = root / "fetch.fsx"
                    harness.write_text(FETCH, encoding="utf-8")
                    result = self.run_script(
                        root, f"http://127.0.0.1:{server.server_port}/start",
                        script=harness, env={**os.environ, "GH_TOKEN": "fixture-token"},
                    )
                    self.assertEqual(result.returncode == 0, scenario == "retry", result.stderr)
                    self.assertEqual(len(requests), 1 if scenario == "redirect" else 3)
                    self.assertTrue(all("Authorization" not in headers for headers in requests))
                    if scenario == "redirect":
                        self.assertIn("outside dnceng-public/public", result.stderr)
                    elif scenario == "retry":
                        self.assertEqual(result.stdout.strip(), "recovered")
                finally:
                    server.shutdown()
                    server.server_close()
                    worker.join()


if __name__ == "__main__":
    unittest.main()
