import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest

import yaml


WORKFLOWS = Path(__file__).resolve().parents[1]
WORKFLOW_NAME = "build-failure-analysis-command"
SHARED = WORKFLOWS / "shared" / "build-failure-analysis-shared.md"
HARNESS = r"""
const fs = require("node:fs");
const input = JSON.parse(fs.readFileSync(0, "utf8"));
const outputs = {};
const context = {
  repo: {owner: "dotnet", repo: "test"},
  payload: {comment: {id: 123, created_at: "2026-10-01T00:00:00Z"}},
};
const core = {setOutput: (key, value) => outputs[key] = value, notice() {}};
const github = {
  rest: {actions: {listJobsForWorkflowRunAttempt: "jobs",
    async listWorkflowRuns() {
      return {data: {total_count: input.runs.length, workflow_runs: input.runs}};
    }}},
  async paginate(method, options) {
    return input.jobs[`${options.run_id}:${options.attempt_number}`] || [];
  },
};
const AsyncFunction = Object.getPrototypeOf(async function() {}).constructor;
(async () => {
  await new AsyncFunction("context", "github", "core", input.script)(context, github, core);
  console.log(JSON.stringify(outputs));
})().catch(error => {
  console.error(error);
  process.exitCode = 1;
});
"""


def load(path):
    lines = path.read_text(encoding="utf-8").splitlines()
    return yaml.safe_load("\n".join(lines[1:lines.index("---", 1)]))


def publication_jobs(agent="success", cli="success", safe="success", process="success"):
    return [
        {
            "name": "agent",
            "conclusion": agent,
            "steps": [{"name": "Execute GitHub Copilot CLI", "conclusion": cli}],
        },
        {
            "name": "safe_outputs",
            "conclusion": safe,
            "steps": [{"name": "Process Safe Outputs", "conclusion": process}],
        },
    ]


class CommandPublicationTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.workflow = load(WORKFLOWS / f"{WORKFLOW_NAME}.md")
        cls.check = next(
            step
            for step in cls.workflow["jobs"]["fetch-binlog"]["steps"]
            if step.get("id") == "command"
        )
        cls.metadata = load(SHARED)["safe-outputs"]["steps"][0]["with"]["script"]

    def run_check(self, jobs, title="Build failure analysis command 123"):
        fixture = {
            "runs": [{"id": 9, "run_attempt": 2, "display_title": title}],
            "jobs": {"9:2": jobs},
            "script": self.check["with"]["script"],
        }
        result = subprocess.run(
            ["node", "-e", HARNESS],
            input=json.dumps(fixture),
            capture_output=True,
            text=True,
            env={**os.environ, "WORKFLOW_FILE": f"{WORKFLOW_NAME}.lock.yml"},
            timeout=15,
        )
        self.assertEqual(result.returncode, 0, result.stderr)
        return json.loads(result.stdout)

    def test_completion_requires_agent_cli_and_safe_output_success(self):
        cases = (
            (publication_jobs(), True),
            (publication_jobs(agent="failure", cli="failure"), False),
            (publication_jobs(cli="failure"), False),
            (publication_jobs(safe="failure", process="failure"), False),
            (publication_jobs(process="failure"), False),
            (publication_jobs()[1:], False),
        )
        for jobs, expected in cases:
            with self.subTest(jobs=jobs):
                self.assertEqual(
                    self.run_check(jobs),
                    {"completed": str(expected).lower()},
                )

    def test_completion_is_bound_to_the_exact_command(self):
        self.assertEqual(
            self.run_check(publication_jobs(), "Build failure analysis command 1234"),
            {"completed": "false"},
        )

    def test_compiled_workflow_preserves_the_source_predicate(self):
        lock = yaml.safe_load(
            (WORKFLOWS / f"{WORKFLOW_NAME}.lock.yml").read_text(encoding="utf-8")
        )
        compiled = next(
            step
            for step in lock["jobs"]["fetch-binlog"]["steps"]
            if step.get("id") == "command"
        )
        self.assertEqual(
            compiled["with"]["script"].strip(),
            self.check["with"]["script"].strip(),
        )

    def test_fix_publication_requires_checked_same_repository_changes(self):
        def summary(status):
            return {
                "type": "add_comment",
                "body": "Build failure",
                "data": {
                    "workflow_artifact": "build-failure-analysis",
                    "artifact_kind": "analysis",
                    "fix_status": status,
                },
            }

        push = {"type": "push_to_pull_request_branch"}
        cases = (
            ("blocked-analysis", [summary("blocked")], "dotnet/test", True),
            ("checked-fix", [push, summary("validated")], "dotnet/test", True),
            ("unverified-fix", [push, summary("failed")], "dotnet/test", False),
            ("fork-fix", [push, summary("validated")], "contributor/test", False),
            ("missing-push", [summary("validated")], "dotnet/test", False),
            ("missing-summary", [push], "dotnet/test", False),
            ("multiple-pushes", [push, push, summary("validated")], "dotnet/test", False),
            ("missing-metadata", [{"type": "add_comment", "body": "Missing"}], "dotnet/test", False),
        )
        for name, items, head_repo, accepted in cases:
            with self.subTest(name=name), tempfile.TemporaryDirectory() as directory:
                output = Path(directory) / "output.json"
                output.write_text(json.dumps({"items": items}), encoding="utf-8")
                result = subprocess.run(
                    ["node", "-e", HARNESS],
                    input=json.dumps({"script": self.metadata}),
                    capture_output=True,
                    text=True,
                    env={
                        **os.environ,
                        "GH_AW_AGENT_OUTPUT": str(output),
                        "EXPECTED_HEAD_REPO": head_repo,
                        "GITHUB_REPOSITORY": "dotnet/test",
                    },
                    timeout=15,
                )
                self.assertEqual(result.returncode == 0, accepted, result.stderr)
                if accepted:
                    published = json.loads(output.read_text(encoding="utf-8"))
                    self.assertIn("Structured data:", published["items"][-1]["body"])


if __name__ == "__main__":
    unittest.main()
