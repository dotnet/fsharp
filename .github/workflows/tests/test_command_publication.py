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
  payload: {comment: {id: input.request ?? 123, created_at: "2026-10-01T00:00:00Z"}, issue: {number: 7}},
};
const core = {setOutput: (key, value) => outputs[key] = value, notice() {}, info() {}};
const github = {
  rest: {issues: {listComments: "comments"}, pulls: {listReviewComments: "reviews"},
    actions: {listJobsForWorkflowRunAttempt: "jobs",
    async listWorkflowRuns() {
      return {data: {total_count: input.runs.length, workflow_runs: input.runs}};
    }}},
  async paginate(method, options) {
    if (method === "comments" || method === "reviews") {
      return input[method] || [];
    }
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

    def test_fix_delivery_requires_validation_and_blocks_fork_pushes(self):
        def summary(status, delivery=None):
            return {
                "type": "add_comment",
                "body": "Build failure",
                "data": {
                    "workflow_artifact": "build-failure-analysis",
                    "artifact_kind": "analysis",
                    "fix_status": status,
                    "fix_delivery": delivery or ("push" if status == "validated" else "none"),
                },
            }

        push = {"type": "push_to_pull_request_branch"}
        cases = (
            ("blocked-analysis", [summary("blocked")], "dotnet/test", True),
            ("checked-fix", [push, summary("validated")], "dotnet/test", True),
            ("unverified-fix", [push, summary("failed")], "dotnet/test", False),
            ("fork-fix", [push, summary("validated")], "contributor/test", False),
            ("fork-patch", [summary("validated", "patch")], "contributor/test", True),
            ("same-repo-patch", [summary("validated", "patch")], "dotnet/test", True),
            ("unverified-patch", [summary("failed", "patch")], "contributor/test", False),
            ("mixed-delivery", [push, summary("validated", "patch")], "dotnet/test", False),
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

    def test_suggestions_require_one_validated_surgical_patch(self):
        data = {
            "workflow_artifact": "build-failure-analysis",
            "artifact_kind": "analysis",
            "fix_status": "validated",
            "fix_delivery": "patch",
        }
        summary = {"type": "add_comment", "body": "Checked patch", "data": data}
        suggestion = {
            "type": "create_pull_request_review_comment",
            "path": "Source.fs",
            "line": 2,
            "body": "```suggestion\nlet value = 1\n```",
            "data": data,
        }
        cases = (
            ("single", [summary, suggestion], True),
            ("multi-line", [summary, {**suggestion, "line": 3, "start_line": 2}], True),
            ("twenty-lines", [summary, {**suggestion, "line": 21, "start_line": 2,
                                        "body": "```suggestion\n" + "\n".join(["x"] * 20) + "\n```"}], True),
            ("missing-summary", [suggestion], False),
            ("missing-metadata", [summary, {**suggestion, "data": None}], False),
            ("multiple-files", [summary, suggestion, {**suggestion, "path": "Other.fs"}], False),
            ("too-many", [summary, *[suggestion] * 4], False),
            ("old-side", [summary, {**suggestion, "side": "LEFT"}], False),
            ("invalid-line", [summary, {**suggestion, "line": 0}], False),
            ("too-wide", [summary, {**suggestion, "line": 22, "start_line": 2}], False),
            ("too-large", [summary, {**suggestion, "body": "```suggestion\n" + "x\n" * 21 + "```"}], False),
            ("not-a-suggestion", [summary, {**suggestion, "body": "Please fix this"}], False),
        )
        for name, items, accepted in cases:
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
                        "EXPECTED_HEAD_REPO": "contributor/test",
                        "GITHUB_REPOSITORY": "dotnet/test",
                    },
                    timeout=15,
                )
                self.assertEqual(result.returncode == 0, accepted, result.stderr)


if __name__ == "__main__":
    unittest.main()
