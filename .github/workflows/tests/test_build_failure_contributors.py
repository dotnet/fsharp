import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest

import yaml

from test_command_publication import HARNESS, SHARED, WORKFLOWS, load


ROOT = WORKFLOWS.parent.parent
NOTIFICATIONS = WORKFLOWS / "build-failure-analysis-test-failure.yml"
NOTIFICATION_HARNESS = r"""
const fs = require("node:fs");
const input = JSON.parse(fs.readFileSync(0, "utf8"));
const calls = [];
const context = {
  repo: {owner: "dotnet", repo: "fsharp"},
  serverUrl: "https://github.com",
  payload: {workflow_run: input.run, repository: {default_branch: "main"}},
};
const github = {rest: {
  actions: {async getWorkflowRun() {return {data: input.current_run || input.run};}},
  repos: {listPullRequestsAssociatedWithCommit: "associated"},
  pulls: {async get() {return {data: input.pr};}},
  issues: {
    listComments: "comments", listForRepo: "issues",
    async createComment(value) {calls.push(["comment", value]);},
    async updateComment(value) {calls.push(["update-comment", value]);},
    async create(value) {calls.push(["issue", value]);},
    async update(value) {calls.push(["update-issue", value]);},
  },
}, async paginate(method) {return input[method] || [];}};
const core = {info() {}, notice() {}, warning() {}};
const AsyncFunction = Object.getPrototypeOf(async function() {}).constructor;
(async () => {
  await new AsyncFunction("context", "github", "core", input.script)(context, github, core);
  console.log(JSON.stringify(calls));
})().catch(error => {console.error(error); process.exitCode = 1;});
"""
LOCAL_HARNESS = r"""
param([string]$Script, [string]$Fixture)
$ErrorActionPreference = 'Stop'
$global:collectorFixture = Get-Content -LiteralPath $Fixture -Raw | ConvertFrom-Json
function gh { $global:LASTEXITCODE = 0; 'fixture-token' }
function dotnet {
    if ($args[0] -ne 'fsi' -or $args[1] -ne '--exec' -or
        [IO.Path]::GetFileName($args[2]) -ne 'fetch-build-binlogs.fsx') {
        throw 'The helper did not invoke the shared collector.'
    }
    Set-Variable -Name LASTEXITCODE -Value $global:collectorFixture.exit -Scope 1
    if ($global:collectorFixture.found) {
        New-Item -ItemType Directory -Path $env:BINLOG_DIR | Out-Null
        if ($global:collectorFixture.regular -eq $false) {
            New-Item -ItemType Directory -Path (Join-Path $env:BINLOG_DIR '1.binlog') | Out-Null
        } else {
            Set-Content -LiteralPath (Join-Path $env:BINLOG_DIR '1.binlog') -Value 'fixture'
        }
        @"
binlog-found=false
binlog-found=true
pr-number=7
pr-head-sha=$('a' * 40)
pr-merge-sha=$('b' * 40)
pr-head-ref=contributor-fix
pr-head-repo=contributor/fsharp
ado-build-id=123
ado-build-url=https://dev.azure.com/dnceng-public/public/_build/results?buildId=123
"@ | Set-Content -LiteralPath $env:GITHUB_OUTPUT
    } else {
        'binlog-found=false' | Set-Content -LiteralPath $env:GITHUB_OUTPUT
    }
    "collector-progress: $env:RESOLVE_MODE"
}
$before = @{}
foreach ($name in 'GH_AW_REPO', 'RESOLVE_MODE', 'PR_NUMBER', 'DISPATCH_BUILD_ID', 'BINLOG_DIR', 'GITHUB_OUTPUT', 'GH_TOKEN') {
    $before[$name] = [Environment]::GetEnvironmentVariable($name)
}
$parameters = @{ PrNumber = 7 }
if ($global:collectorFixture.build) { $parameters.BuildId = $global:collectorFixture.build }
try { $context = & $Script @parameters }
finally {
    foreach ($name in $before.Keys) {
        if ([Environment]::GetEnvironmentVariable($name) -ne $before[$name]) {
            throw "The helper did not restore $name."
        }
    }
}
$context
"""


class ContributorTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.steps = load(SHARED)["safe-outputs"]["steps"]
        cls.patch_check = next(step["with"]["script"] for step in cls.steps if step["name"] == "Check the contributor patch")
        cls.link = next(step["with"]["script"] for step in cls.steps if step["name"] == "Link the checked patch in the summary")
        cls.retry = next(step["with"]["script"] for step in cls.steps if step["name"] == "Prepare retry-safe command outputs")

    def run_script(self, script, env, directory, **fixture):
        return subprocess.run(
            ["node", "-e", HARNESS],
            input=json.dumps({"script": script, **fixture}),
            cwd=directory,
            env={**os.environ, **env},
            capture_output=True,
            text=True,
            timeout=15,
        )

    def test_patch_is_complete_applicable_bounded_and_not_executed(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            def git(*args):
                return subprocess.check_output(["git", *args], cwd=root, text=True).strip()
            git("init", "--quiet")
            paths = ["Source.fs", *[f"File{number}.fs" for number in range(25)],
                     "nested/AGENTS.md", ".github/guard.yml"]
            for name in paths:
                path = root / name
                path.parent.mkdir(parents=True, exist_ok=True)
                path.write_text("let value = missing\n", encoding="utf-8")
            git("add", ".")
            git("-c", "user.name=Fixture", "-c", "user.email=fixture@example.test", "commit", "--quiet", "-m", "Base")
            head = git("rev-parse", "HEAD")
            def diff_for(names):
                for name in names:
                    (root / name).write_text("let value = 1\n", encoding="utf-8")
                diff = git("diff", "--no-ext-diff", "--no-renames") + "\n"
                for name in names:
                    (root / name).write_text("let value = missing\n", encoding="utf-8")
                return diff
            diff = diff_for(["Source.fs"])
            large = diff.replace("+let value = 1\n", "+let value = 1" + "x" * (65536 - len(diff.encode())) + "\n")
            self.assertEqual(len(large.encode()), 65536)
            symlink = "diff --git a/Link b/Link\nnew file mode 120000\n--- /dev/null\n+++ b/Link\n@@ -0,0 +1 @@\n+Source.fs\n"
            patch = root / "fix.patch"
            output = root / "output.json"
            cases = (
                ("valid", diff, head, [], True),
                ("stale", diff, "f" * 40, [], False),
                ("not-applicable", diff.replace("missing", "other"), head, [], False),
                ("empty", "", head, [], False),
                ("size-limit", large, head, [], True),
                ("oversize", large + "\n", head, [], False),
                ("file-limit", diff_for(paths[:25]), head, [], True),
                ("too-many-files", diff_for(paths[:26]), head, [], False),
                ("protected-instructions", diff_for(["nested/AGENTS.md"]), head, [], False),
                ("workflow-change", diff_for([".github/guard.yml"]), head, [], False),
                ("symlink", symlink, head, [], False),
                ("crlf-symlink", symlink.replace("\n", "\r\n"), head, [], False),
                ("submodule", symlink.replace("120000", "160000"), head, [], False),
                ("wrong-suggestion", diff, head, [{"type": "create_pull_request_review_comment", "path": "Other.fs"}], False),
            )
            for name, content, base, items, accepted in cases:
                with self.subTest(name=name):
                    patch.write_bytes(content.encode("utf-8"))
                    output.write_text(json.dumps({"items": items}), encoding="utf-8")
                    result = self.run_script(self.patch_check, {
                        "PATCH_PATH": str(patch), "GH_AW_AGENT_OUTPUT": str(output), "EXPECTED_HEAD": base,
                    }, root)
                    self.assertEqual(result.returncode == 0, accepted, result.stderr)
                    for name in paths:
                        self.assertEqual((root / name).read_text(encoding="utf-8"), "let value = missing\n")

    def test_command_retries_deduplicate_bot_summaries_and_reviews_for_the_exact_request(self):
        items = [{"type": "add_comment", "body": "Checked patch"},
                 {"type": "create_pull_request_review_comment", "path": "Source.fs", "line": 2,
                  "body": "```suggestion\nlet value = 1\n```"}]
        bot = {"login": "github-actions[bot]", "type": "Bot"}
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / "output.json"
            env = {"GH_AW_AGENT_OUTPUT": str(output), "EXPECTED_HEAD": "a" * 40}
            output.write_text(json.dumps({"items": items}), encoding="utf-8")
            first = self.run_script(self.retry, env, directory)
            self.assertEqual(first.returncode, 0, first.stderr)
            published = json.loads(output.read_text(encoding="utf-8"))["items"]
            self.assertTrue(all(item["body"].startswith("Build-analysis output: `123:") for item in published))
            for name, head, request, user, expected in (
                ("retry", "a" * 40, 123, bot, 0),
                ("other-head", "b" * 40, 123, bot, 2),
                ("other-command", "a" * 40, 124, bot, 2),
                ("forged-marker", "a" * 40, 123, {"login": "contributor", "type": "User"}, 2),
            ):
                with self.subTest(name=name):
                    output.write_text(json.dumps({"items": items}), encoding="utf-8")
                    result = self.run_script(self.retry, {**env, "EXPECTED_HEAD": head}, directory,
                                             request=request, comments=[{**published[0], "user": user}],
                                             reviews=[{**published[1], "user": user}])
                    self.assertEqual(result.returncode, 0, result.stderr)
                    self.assertEqual(len(json.loads(output.read_text(encoding="utf-8"))["items"]), expected)

    def test_patch_link_requires_a_successful_upload(self):
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / "output.json"
            for url, accepted in (("", False), ("https://github.com/dotnet/fsharp/actions/runs/9/artifacts/7", True)):
                with self.subTest(url=url):
                    output.write_text(json.dumps({"items": [{"type": "add_comment", "body": "Checked"}]}), encoding="utf-8")
                    result = self.run_script(self.link, {
                        "GH_AW_AGENT_OUTPUT": str(output), "PATCH_URL": url, "EXPECTED_HEAD": "a" * 40,
                    }, directory)
                    self.assertEqual(result.returncode == 0, accepted, result.stderr)
                    if accepted:
                        body = json.loads(output.read_text(encoding="utf-8"))["items"][0]["body"]
                        self.assertIn(url, body)
                        self.assertIn("not applied to your branch", body)
                        self.assertIn("git apply --check", body)

    def test_source_and_compiled_patch_guards_precede_publication(self):
        for name in ("build-failure-analysis", "build-failure-analysis-command"):
            with self.subTest(workflow=name):
                source = load(WORKFLOWS / f"{name}.md")["safe-outputs"]
                review = source["create-pull-request-review-comment"]
                self.assertEqual(review["max"], 3)
                self.assertEqual(review["commit-id"], "${{ needs.fetch-binlog.outputs.pr-head-sha }}")
                lock = yaml.safe_load((WORKFLOWS / f"{name}.lock.yml").read_text(encoding="utf-8"))
                steps = lock["jobs"]["safe_outputs"]["steps"]
                names = [step.get("name") for step in steps]
                ordered = ["Ensure build-analysis output metadata", "Prepare retry-safe command outputs",
                           "Revalidate the latest failed build and PR revision",
                           "Check out the verified patch base without credentials",
                           "Check the contributor patch", "Upload the checked contributor patch",
                           "Link the checked patch in the summary", "Process Safe Outputs"]
                self.assertEqual([names.index(name) for name in ordered], sorted(names.index(name) for name in ordered))
                compiled = next(step for step in steps if step.get("name") == "Check the contributor patch")
                self.assertEqual(compiled["with"]["script"].strip(), self.patch_check.strip())
                artifact = next(step for step in lock["jobs"]["agent"]["steps"] if step.get("name") == "Upload agent artifacts")
                self.assertIn("/tmp/gh-aw/aw-*.patch", artifact["with"]["path"])
                self.assertTrue(all(path.lstrip("!").startswith("/tmp/gh-aw/")
                                    for path in artifact["with"]["path"].splitlines()))
                download = next(step for step in steps if step.get("id") == "download-agent-output")
                self.assertEqual(download["with"]["path"], "/tmp/gh-aw/")
                checkout = next(step for step in steps if step.get("name") == "Check out the verified patch base without credentials")
                self.assertIs(checkout["with"]["persist-credentials"], False)
                self.assertEqual(checkout["with"]["ref"], "${{ needs.fetch-binlog.outputs.pr-head-sha }}")

    def test_local_agent_reuses_the_pinned_mcp_and_validated_collector(self):
        agent = load(ROOT / ".github" / "agents" / "build-failure-analyst.agent.md")
        server = agent["mcp-servers"]["binlog-mcp"]
        self.assertEqual((server["command"], server["args"]), ("dotnet", ["tool", "run", "binlog-mcp"]))
        manifest = json.loads((ROOT / ".config" / "dotnet-tools.json").read_text(encoding="utf-8"))
        self.assertIn("binlog-mcp", manifest["tools"]["microsoft.aitools.binlogmcp"]["commands"])
        script = WORKFLOWS / "scripts" / "prepare-build-failure-analysis.ps1"
        cases = (
            ("latest", {"found": True, "exit": 0}, None),
            ("explicit-build", {"found": True, "exit": 0, "build": 123}, None),
            ("existing-token", {"found": True, "exit": 0, "token": "existing-fixture-token"}, None),
            ("empty-token", {"found": True, "exit": 0, "token": ""}, None),
            ("incomplete", {"found": False, "exit": 0}, "No complete, current binlog evidence"),
            ("failed", {"found": True, "exit": 1}, "Binlog collection failed (exit 1)"),
            ("not-a-file", {"found": True, "exit": 0, "regular": False}, "has no regular binlogs"),
        )
        for name, data, error in cases:
            with self.subTest(name=name), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                harness = root / "harness.ps1"
                harness.write_text(LOCAL_HARNESS, encoding="utf-8")
                fixture = root / "fixture.json"
                fixture.write_text(json.dumps(data), encoding="utf-8")
                env = {**os.environ, "TEMP": directory, "TMP": directory}
                env.pop("GH_TOKEN", None)
                if "token" in data:
                    env["GH_TOKEN"] = data["token"]
                result = subprocess.run(
                    ["pwsh", "-NoProfile", "-File", str(harness), str(script), str(fixture)],
                    env=env,
                    capture_output=True,
                    text=True,
                    timeout=30,
                )
                self.assertEqual(result.returncode == 0, error is None, result.stderr)
                if error is not None:
                    self.assertIn(error, result.stderr)
                else:
                    context = json.loads(Path(result.stdout.strip()).read_text(encoding="utf-8-sig"))
                    self.assertEqual(context["GH_AW_PR_NUMBER"], "7")
                    self.assertEqual(context["GH_AW_PR_HEAD_REPO"], "contributor/fsharp")
                    self.assertTrue(Path(context["GH_AW_BINLOG_PATH"]).is_file())
                    self.assertNotIn("fixture-token", json.dumps(context))
                    self.assertIn("collector-progress: " + ("dispatch" if data.get("build") else "latest"), result.stderr)


class RegressionOwnershipTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.workflow = yaml.safe_load(NOTIFICATIONS.read_text(encoding="utf-8"))
        cls.script = cls.workflow["jobs"]["notify"]["steps"][0]["with"]["script"]

    def notify(self, event="pull_request", expected_error=None, **extra):
        run = {
            "id": 9, "path": ".github/workflows/build-failure-analysis-tests.yml",
            "head_sha": "a" * 40, "head_branch": "main", "pull_requests": [],
            "event": event, "conclusion": "failure", "status": "completed", "run_attempt": 1,
        }
        pr = {"number": 7, "state": "open", "head": {"sha": "a" * 40},
              "base": {"repo": {"full_name": "dotnet/fsharp"}}, "user": {"login": "contributor"}}
        fixture = {"script": self.script, "run": run, "pr": pr, "associated": [{"number": 7}], **extra}
        result = subprocess.run(
            ["node", "-e", NOTIFICATION_HARNESS], input=json.dumps(fixture),
            capture_output=True, text=True, timeout=15,
        )
        self.assertEqual(result.returncode == 0, expected_error is None, result.stderr)
        if expected_error is not None:
            self.assertIn(expected_error, result.stderr)
            return
        return json.loads(result.stdout)

    def test_fork_pr_failures_notify_the_author_without_trusting_pr_code(self):
        calls = self.notify()
        self.assertEqual(calls[0][0], "comment")
        self.assertEqual(calls[0][1]["issue_number"], 7)
        self.assertIn("@contributor", calls[0][1]["body"])
        self.assertIn("@dotnet/fsharp-team-msft", calls[0][1]["body"])
        self.assertNotIn("checkout", self.script)
        self.assertNotIn("downloadArtifact", self.script)

    def test_default_branch_failures_have_one_owner_triaged_issue(self):
        bot = {"login": "github-actions[bot]", "type": "Bot"}
        report = {"number": 1, "body": "<!-- build-failure-analysis-test-failure -->", "user": bot}
        for event in ("push", "schedule", "workflow_dispatch"):
            with self.subTest(event=event):
                self.assertEqual(self.notify(event)[0][0], "issue")
                self.assertEqual(self.notify(event, issues=[report])[0][0], "update-issue")

    def test_stale_closed_prs_are_not_blamed_and_missing_association_is_escalated(self):
        stale = {"number": 7, "state": "open", "head": {"sha": "b" * 40},
                 "base": {"repo": {"full_name": "dotnet/fsharp"}}, "user": {"login": "contributor"}}
        self.assertEqual(self.notify(pr=stale), [])
        self.assertEqual(self.notify(pr={**stale, "state": "closed"}), [])
        self.assertEqual(self.notify(associated=[])[0][0], "issue")

    def test_recovered_or_superseded_attempts_do_not_notify_and_wrong_workflows_fail(self):
        for current in (
            {"run_attempt": 1, "status": "completed", "conclusion": "success"},
            {"run_attempt": 2, "status": "completed", "conclusion": "failure"},
            {"run_attempt": 2, "status": "in_progress", "conclusion": None},
        ):
            with self.subTest(current=current):
                self.assertEqual(self.notify(current_run=current), [])
        self.notify(run={"id": 9, "run_attempt": 1, "head_sha": "a" * 40, "path": "other.yml"},
                    expected_error="Unexpected regression workflow or run identity")

    def test_pr_failure_notifications_update_only_the_existing_bot_owned_report(self):
        report = {"id": 5, "body": "<!-- build-failure-analysis-test-failure -->",
                  "user": {"login": "github-actions[bot]", "type": "Bot"}}
        calls = self.notify(comments=[report])
        self.assertEqual(calls[0][0], "update-comment")
        self.assertEqual(calls[0][1]["comment_id"], 5)
        calls = self.notify(comments=[{**report, "user": {"login": "contributor", "type": "User"}}])
        self.assertEqual(calls[0][0], "comment")

    def test_regression_runs_on_every_pr_and_notifications_are_least_privileged(self):
        tests = yaml.safe_load((WORKFLOWS / "build-failure-analysis-tests.yml").read_text(encoding="utf-8"))
        # PyYAML's YAML 1.1 parser recognizes the unquoted Actions "on" key as True.
        self.assertIsNone(tests[True]["pull_request"])
        self.assertNotIn("paths", tests[True]["push"])
        permissions = self.workflow["jobs"]["notify"]["permissions"]
        self.assertEqual(permissions["contents"], "read")
        self.assertEqual(permissions["pull-requests"], "read")
        self.assertEqual(permissions["issues"], "write")
        self.assertEqual(self.workflow[True]["workflow_run"]["types"], ["completed"])


if __name__ == "__main__":
    unittest.main()
