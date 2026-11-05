"""Exercise the generated graph, not just the workflow's Markdown."""

import copy
import json
from pathlib import Path
import re
import unittest

import yaml

ROOT = Path(__file__).resolve().parents[2]
WORKFLOW = ROOT / ".github" / "workflows" / "compiler-self-profile"
GUARDS = ("github.repository == 'dotnet/fsharp'",
          "github.ref == format('refs/heads/{0}', github.event.repository.default_branch)",
          "(github.event_name != 'workflow_dispatch' || inputs.aw_context == '')")
READ = {"actions": "read", "contents": "read", "pull-requests": "read"}


def load_graph():
    # BaseLoader preserves Actions' `on`, rather than YAML 1.1's boolean True.
    return yaml.load(WORKFLOW.with_suffix(".lock.yml").read_text(encoding="utf-8"), Loader=yaml.BaseLoader)


def step(graph, identifier):
    return next(s for s in graph["jobs"]["agent"]["steps"] if s.get("id") == identifier)


def code(item):
    return item.get("run", "") + item.get("with", {}).get("script", "")


def assert_graph(test, graph):
    test.assertEqual(set(graph["on"]), {"schedule", "workflow_dispatch"})
    test.assertEqual(graph["on"]["schedule"], [{"cron": "17 3 * * 1"}])
    inputs = graph["on"]["workflow_dispatch"]["inputs"]
    test.assertEqual(set(inputs), {"aw_context"})
    test.assertEqual(inputs["aw_context"]["default"], "")
    test.assertEqual(graph["concurrency"], {"group": "compiler-self-profile", "cancel-in-progress": "false"})
    test.assertEqual(graph["permissions"], {})
    jobs = graph["jobs"]
    test.assertEqual(set(jobs), {"activation", "agent", "safe_outputs", "conclusion"})
    test.assertEqual(" ".join(jobs["activation"].get("if", "").split()), " && ".join(GUARDS))
    test.assertEqual(jobs["agent"]["needs"], "activation")
    test.assertFalse(jobs["agent"].get("if"))
    test.assertEqual(jobs["agent"]["permissions"], READ)
    test.assertEqual(int(jobs["agent"].get("timeout-minutes", 360)), 360)
    test.assertNotIn("detection", jobs, "No separately timed optimizer/detection worker")
    for name, job in jobs.items():
        test.assertFalse(any(v == "write" for v in job.get("permissions", {}).values()), name)
        for key, value in job.get("env", {}).items():
            test.assertNotIn(key, ("GH_TOKEN", "GITHUB_TOKEN", "ACTIONS_RUNTIME_TOKEN"))
            test.assertNotIn("OWNER_TOKEN", value)
    steps = jobs["agent"]["steps"]
    ordered = [step(graph, key) for key in ("provision", "controller", "agentic_execution", "handoff", "publisher", "quarantine")]
    test.assertEqual([steps.index(s) for s in ordered], sorted(steps.index(s) for s in ordered))
    provision, controller, engine, handoff, publisher, quarantine = ordered
    for item in (provision, controller, engine):
        test.assertFalse(item.get("if"), "Required entry point must run after successful guarded activation")
        test.assertNotEqual(item.get("continue-on-error"), "true")
    owners = {(job_name, item.get("id", item.get("name"))) for job_name, job in jobs.items() for item in job["steps"]
              if "COMPILER_PERF_OWNER_TOKEN" in yaml.dump(item.get("env", {}))}
    test.assertEqual(owners, {("agent", key) for key in
                             ("provision", "controller", "handoff", "publisher", "Redact secrets in logs")})
    redactor = next(s for s in steps if s.get("name") == "Redact secrets in logs")
    test.assertEqual(redactor["uses"], "actions/github-script@3a2844b7e9c422d3c10d287c895573f7108da1b3")
    test.assertEqual(code(redactor), (
        "const { setupGlobals } = require('${{ runner.temp }}/gh-aw/actions/setup_globals.cjs');\n"
        "setupGlobals(core, github, context, exec, io, getOctokit);\n"
        "const { main } = require('${{ runner.temp }}/gh-aw/actions/redact_secrets.cjs');\nawait main();\n"))
    test.assertEqual(int(engine["timeout-minutes"]), 250)
    test.assertIn("listJobsForWorkflowRunAttempt", code(provision))
    test.assertIn("started_at", code(provision))
    test.assertIn("process.env.GITHUB_SHA", code(provision))
    test.assertIn("'provision'", code(provision))
    test.assertIn(" start-service ", code(controller))
    test.assertIn(" handoff ", code(handoff))
    test.assertIn("'finalize'", code(publisher))
    test.assertIn("'/usr/bin/mv'", code(quarantine))
    test.assertIn("`${source}/agent_output.json`", code(quarantine))
    test.assertEqual(handoff["env"]["PERF_ENGINE_OUTCOME"], "${{ steps.agentic_execution.outcome }}")
    for item in (provision, controller, handoff, publisher):
        test.assertEqual(item["env"]["GH_TOKEN"], "${{ secrets.COMPILER_PERF_OWNER_TOKEN }}")
    for item in (handoff, publisher, quarantine):
        test.assertEqual(item["if"], "always() && " + " && ".join(GUARDS))
    for key in engine["env"]:
        test.assertNotIn(key, ("GH_TOKEN", "GITHUB_TOKEN", "ACTIONS_RUNTIME_TOKEN"))
        test.assertNotIn("SESSION_TIMEOUT", key)
    test.assertIn("compiler_perf_harness.cjs", code(engine))
    test.assertEqual(engine["env"]["COMPILER_PERF_EXECUTION_DEADLINE_MS"],
                     "${{ steps.controller.outputs.execution_deadline_ms }}")
    for tool in ("shell", "write", "safeoutputs", "task"):
        test.assertRegex(code(engine), r"--deny-tool (?:'|\")?" + tool + r"(?:'|\")?(?:\s|$)")
    test.assertNotIn("--allow-all-tools", code(engine))
    test.assertNotIn("--max-turns", code(engine))
    test.assertNotIn("--max-autopilot-continues", code(engine))
    for item in steps:
        test.assertNotIn("actions/checkout@", item.get("uses", ""))
        test.assertNotIn("github.event.issue.body", code(item))
        test.assertNotIn("github.event.pull_request.body", code(item))
        for key in item.get("env", {}):
            test.assertNotIn("SESSION_TIMEOUT", key)
            test.assertNotIn(key, ("COPILOT_TIMEOUT", "GH_AW_TIMEOUT"))
    for item in (handoff, publisher):
        test.assertNotIn("GH_AW_SAFE_OUTPUTS", code(item))
        test.assertNotIn("agent_output.json", code(item))
    gateway = next(s for s in steps if s.get("id") == "start-mcp-gateway")
    match = re.search(r'cat << (GH_AW_MCP_CONFIG_\w+_EOF).*?\n(.*?)\n\s*\1', code(gateway), re.S)
    test.assertIsNotNone(match, "Rendered gateway JSON absent")
    config = json.loads(match[2].replace("\\${", "${").replace("$MCP_GATEWAY_PORT", "8080"))
    test.assertEqual(set(config["mcpServers"]), {"controller", "safeoutputs"})
    test.assertEqual(config["gateway"]["toolTimeout"], 300)
    test.assertNotIn("sessionTimeout", config["gateway"])
    test.assertEqual(config["mcpServers"]["controller"]["url"], "http://host.docker.internal:8765/mcp")
    test.assertEqual(config["mcpServers"]["controller"]["headers"]["Authorization"],
                     "Bearer ${{ steps.controller.outputs.token }}")
    native = jobs["safe_outputs"]
    test.assertEqual(native["env"]["GH_AW_SAFE_OUTPUTS_STAGED"], "true")
    test.assertEqual(native["permissions"], {})
    test.assertNotIn("OWNER_TOKEN", yaml.dump(native))
    output = next(s for s in native["steps"] if s.get("id") == "process_safe_outputs")
    handlers = json.loads(output["env"]["GH_AW_SAFE_OUTPUTS_HANDLER_CONFIG"])
    pr = handlers["create_pull_request"]
    test.assertEqual((pr["max"], pr["draft"], pr["patch_format"], pr["fallback_as_issue"]), (1, True, "am", False))
    test.assertFalse(set(handlers) & {"create_issue", "merge_pull_request", "add_comment", "submit_pull_request_review"})
    upload = next(i for i, s in enumerate(steps) if s.get("name") == "Upload agent artifacts")
    test.assertLess(steps.index(quarantine), upload)
    test.assertNotIn(str(ROOT), yaml.dump(graph))


class CompiledWorkflowTests(unittest.TestCase):
    def test_generated_graph(self):
        self.assertTrue(WORKFLOW.with_suffix(".md").is_file(), "Weekly workflow Markdown is absent")
        metadata = json.loads(WORKFLOW.with_suffix(".lock.yml").read_text(encoding="utf-8").splitlines()[0].split(": ", 1)[1])
        self.assertEqual(metadata["compiler_version"], "v0.76.1")
        assert_graph(self, load_graph())

    def test_local_model_defaults_preserve_shared_selection(self):
        def frontmatter(path):
            return yaml.load(path.read_text(encoding="utf-8").split("---", 2)[1], Loader=yaml.BaseLoader)
        local = frontmatter(WORKFLOW.with_suffix(".md"))
        shared = frontmatter(WORKFLOW.parent / "shared" / "model-defaults.md")
        self.assertEqual(local["models"], shared["models"])
        self.assertEqual(local["engine"]["model"], shared["engine"]["model"])
        self.assertEqual(local["engine"]["env"]["GH_AW_REASONING_EFFORT"],
                         shared["engine"]["env"]["GH_AW_REASONING_EFFORT"])
        self.assertEqual(local["engine"]["args"][:2], shared["engine"]["args"])

    def test_graph_negative_controls(self):
        graph = load_graph()
        mutations = {
            "short-job": lambda g: g["jobs"]["agent"].update({"timeout-minutes": "90"}),
            "missing-guard": lambda g: g["jobs"]["activation"].pop("if"),
            "manual-context": lambda g: g["jobs"]["activation"].update({"if": " && ".join(GUARDS[:2])}),
            "guard-bypass": lambda g: g["jobs"]["activation"].update({"if": " && ".join(GUARDS) + " || true"}),
            "missing-concurrency": lambda g: g.update(concurrency={}),
            "agent-writer": lambda g: g["jobs"]["agent"]["permissions"].update(contents="write"),
            "short-engine": lambda g: step(g, "agentic_execution").update({"timeout-minutes": "60"}),
            "short-session": lambda g: step(g, "agentic_execution")["env"].update(MCP_GATEWAY_SESSION_TIMEOUT="1h"),
            "wrong-outcome": lambda g: step(g, "handoff")["env"].update(PERF_ENGINE_OUTCOME="success"),
            "agent-output": lambda g: step(g, "publisher").update(run='cat "$GH_AW_SAFE_OUTPUTS"'),
            "owner-token-in-agent": lambda g: step(g, "agentic_execution")["env"].update(
                GH_TOKEN="${{ secrets.COMPILER_PERF_OWNER_TOKEN }}"),
            "native-writer": lambda g: g["jobs"]["safe_outputs"]["env"].update(GH_AW_SAFE_OUTPUTS_STAGED="false"),
            "unprotected-finalizer": lambda g: step(g, "publisher").update({"if": "always()"}),
            "raw-upload": lambda g: step(g, "quarantine").update({"with": {"script": "noop"}}),
            "disabled-controller": lambda g: step(g, "controller").update({"if": "false"}),
            "skipped-provision": lambda g: step(g, "provision").update({"if": "false"}),
            "unguarded-owner-job": lambda g: g["jobs"].update(other_publisher={
                "runs-on": "ubuntu-24.04", "permissions": {"contents": "read"},
                "steps": [{"run": "gh api --method POST repos/dotnet/fsharp/pulls",
                           "env": {"GH_TOKEN": "${{ secrets.COMPILER_PERF_OWNER_TOKEN }}"}}]}),
            "alternate-owner-step": lambda g: g["jobs"]["activation"]["steps"].append({
                "run": "gh api --method POST repos/dotnet/fsharp/pulls",
                "env": {"GH_TOKEN": "${{ secrets.COMPILER_PERF_OWNER_TOKEN }}"}}),
        }
        assert_graph(self, graph)
        for name, mutate in mutations.items():
            with self.subTest(mutation=name):
                changed = copy.deepcopy(graph)
                mutate(changed)
                with self.assertRaises(AssertionError):
                    assert_graph(self, changed)


if __name__ == "__main__":
    unittest.main()
