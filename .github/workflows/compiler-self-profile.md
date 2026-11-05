---
name: Compiler Self Profile
description: Weekly controller-owned compiler-source profiling and independently gated draft optimization.

models:
  gpt-5.6-sol: [copilot/gpt-5.6-sol, openai/gpt-5.6-sol]

engine:
  id: copilot
  model: ${{ (github.job == 'detection' && vars.GH_AW_MODEL_DETECTION_COPILOT) || vars.GH_AW_MODEL_AGENT_COPILOT || 'gpt-5.6-sol' }}
  env:
    GH_AW_REASONING_EFFORT: ${{ (github.job == 'detection' && vars.GH_AW_REASONING_EFFORT_DETECTION) || vars.GH_AW_REASONING_EFFORT || 'high' }}
    COMPILER_PERF_EXECUTION_DEADLINE_MS: ${{ steps.controller.outputs.execution_deadline_ms }}
  args: ["--reasoning-effort", "${{ env.GH_AW_REASONING_EFFORT }}", "--deny-tool", "shell", "--deny-tool", "safeoutputs", "--deny-tool", "write", "--deny-tool", "task"]
  version: "1.0.52"
  bare: true
  harness: compiler_perf_harness.cjs
  mcp:
    tool-timeout: 5m

on:
  schedule:
    - cron: "17 3 * * 1"
  workflow_dispatch:

if: github.repository == 'dotnet/fsharp' && github.ref == format('refs/heads/{0}', github.event.repository.default_branch) && (github.event_name != 'workflow_dispatch' || inputs.aw_context == '')
concurrency:
  group: compiler-self-profile
  cancel-in-progress: false

runs-on: ubuntu-24.04
timeout-minutes: 250
permissions:
  actions: read
  contents: read
  pull-requests: read
checkout: false

network:
  allowed:
    - defaults

tools:
  github: false
  bash: false
  edit: false

# The pin injects an issue writer without a non-builtin output. Staging and a
# read-only token disable native effects; only sealed host output is published.
safe-outputs:
  staged: true
  github-token: ${{ secrets.GITHUB_TOKEN }}
  threat-detection: false
  report-failure-as-issue: false
  noop:
    report-as-issue: false
  create-pull-request:
    max: 1
    draft: true
    patch-format: am
    fallback-as-issue: false
    preserve-branch-name: true
  missing-tool:
    create-issue: false
  missing-data:
    create-issue: false

mcp-servers:
  controller:
    type: http
    url: http://127.0.0.1:8765/mcp
    headers:
      Authorization: "Bearer ${{ steps.controller.outputs.token }}"
    allowed: [investigate]

pre-steps:
  - name: Provision protected controller and reserve canonical discovery
    id: provision
    uses: actions/github-script@3a2844b7e9c422d3c10d287c895573f7108da1b3
    env:
      GH_TOKEN: ${{ secrets.COMPILER_PERF_OWNER_TOKEN }}
      COMPILER_PERF_OWNER_LOGIN: ${{ vars.COMPILER_PERF_OWNER_LOGIN }}
      COMPILER_PERF_AGE_RECIPIENT: ${{ vars.COMPILER_PERF_AGE_RECIPIENT }}
    with:
      script: |
        const {execFileSync} = require('node:child_process');
        const fs = require('node:fs');
        if (!process.env.GH_TOKEN) throw new Error('blocked_setup: COMPILER_PERF_OWNER_TOKEN is required');
        const jobs = await github.paginate(github.rest.actions.listJobsForWorkflowRunAttempt, {
          ...context.repo, run_id: context.runId, attempt_number: Number(process.env.GITHUB_RUN_ATTEMPT), per_page: 100
        });
        const owned = jobs.filter(j => j.name === process.env.GITHUB_JOB &&
          j.runner_name === process.env.RUNNER_NAME && j.status === 'in_progress');
        if (owned.length !== 1 || !owned[0].started_at) throw new Error('blocked_setup: actual job start unavailable');
        const deadline = Date.parse(owned[0].started_at) + 3600000;
        function run(file, args) {
          const remaining = deadline - Date.now();
          if (!(remaining > 0)) throw new Error('blocked_setup: setup admission deadline');
          execFileSync(file, args, {stdio: 'inherit', timeout: remaining});
        }
        const root = `/var/lib/compiler-perf/${context.runId}-${process.env.GITHUB_RUN_ATTEMPT}`;
        if (fs.existsSync(root)) throw new Error('blocked_setup: existing owner requires reconciliation');
        run('/usr/bin/sudo', ['-n', '/usr/bin/install', '-d', '-m', '0700', '-o', String(process.getuid()), root]);
        run('/usr/bin/git', ['clone', '--quiet', '--no-checkout', '--template=', 'https://github.com/dotnet/fsharp.git', `${root}/source`]);
        run('/usr/bin/git', ['-C', `${root}/source`, 'checkout', '--quiet', '--detach', process.env.GITHUB_SHA]);
        run('/usr/bin/python3', ['-I', `${root}/source/eng/scripts/compiler_perf/actions_adapter.py`,
          'provision', '--base', root, '--job-started-at', owned[0].started_at]);

pre-agent-steps:
  - name: Prepare frozen compiler inputs and start authenticated controller
    id: controller
    env:
      GH_TOKEN: ${{ secrets.COMPILER_PERF_OWNER_TOKEN }}
    run: |
      set -eu
      base="/var/lib/compiler-perf/$GITHUB_RUN_ID-$GITHUB_RUN_ATTEMPT"
      "$base/python/bin/python3" -I "$base/source/eng/scripts/compiler_perf/runtime.py" start-service --root "$base/controller" --repository "$base/source" --journal "$base/journal.git" --workspace "$GITHUB_WORKSPACE"

post-steps:
  - name: Checkpoint, stop owned work and export sealed publisher inventory
    id: handoff
    if: always() && github.repository == 'dotnet/fsharp' && github.ref == format('refs/heads/{0}', github.event.repository.default_branch) && (github.event_name != 'workflow_dispatch' || inputs.aw_context == '')
    env:
      GH_TOKEN: ${{ secrets.COMPILER_PERF_OWNER_TOKEN }}
      PERF_ENGINE_OUTCOME: ${{ steps.agentic_execution.outcome }}
    run: |
      set -eu
      base="/var/lib/compiler-perf/$GITHUB_RUN_ID-$GITHUB_RUN_ATTEMPT"
      "$base/python/bin/python3" -I "$base/source/eng/scripts/compiler_perf/runtime.py" handoff --root "$base/controller" --repository "$base/source" --journal "$base/journal.git" --workspace "$GITHUB_WORKSPACE" --destination "$base/publisher" --engine-outcome "$PERF_ENGINE_OUTCOME"
  - name: Acknowledge checked evidence and publish at most one sealed draft
    id: publisher
    if: always() && github.repository == 'dotnet/fsharp' && github.ref == format('refs/heads/{0}', github.event.repository.default_branch) && (github.event_name != 'workflow_dispatch' || inputs.aw_context == '')
    uses: actions/github-script@3a2844b7e9c422d3c10d287c895573f7108da1b3
    env:
      GH_TOKEN: ${{ secrets.COMPILER_PERF_OWNER_TOKEN }}
    with:
      script: |
        const {spawnSync} = require('node:child_process');
        const fs = require('node:fs');
        const base = `/var/lib/compiler-perf/${context.runId}-${process.env.GITHUB_RUN_ATTEMPT}`;
        if (!fs.existsSync(`${base}/controller/key`)) throw new Error('blocked_setup: no authenticated controller custody');
        const result = spawnSync(`${base}/python/bin/python3`, ['-I',
          `${base}/source/eng/scripts/compiler_perf/runtime.py`, 'finalize',
          '--root', `${base}/controller`, '--repository', `${base}/source`,
          '--journal', `${base}/journal.git`, '--workspace', process.env.GITHUB_WORKSPACE,
          '--destination', `${base}/publisher`], {stdio: 'inherit', timeout: 2400000});
        if (result.error || result.status !== 0) throw new Error('Finalization blocked; outstanding work preserved');
  - name: Quarantine untrusted stock logs before stock artifact collection
    id: quarantine
    if: always() && github.repository == 'dotnet/fsharp' && github.ref == format('refs/heads/{0}', github.event.repository.default_branch) && (github.event_name != 'workflow_dispatch' || inputs.aw_context == '')
    uses: actions/github-script@3a2844b7e9c422d3c10d287c895573f7108da1b3
    with:
      script: |
        const {execFileSync} = require('node:child_process');
        const fs = require('node:fs');
        const base = `/var/lib/compiler-perf/${context.runId}-${process.env.GITHUB_RUN_ATTEMPT}`;
        const source = '/tmp/gh-aw';
        execFileSync('/usr/bin/sudo', ['-n', '/usr/bin/install', '-d', '-m', '0700', '-o', String(process.getuid()), base], {timeout: 60000});
        if (fs.existsSync(source)) {
          if (fs.lstatSync(source).isSymbolicLink() || !fs.lstatSync(source).isDirectory()) throw new Error('Unsafe stock log root');
          const destination = `${base}/stock-logs`;
          if (fs.existsSync(destination)) throw new Error('Existing stock quarantine preserved');
          execFileSync('/usr/bin/sudo', ['-n', '/usr/bin/mv', '--', source, destination], {timeout: 60000});
        }
        fs.mkdirSync(source, {mode: 0o700});
        // Native staged consumers receive no agent decision or patch.
        fs.writeFileSync(`${source}/agent_output.json`, '{"items":[]}\n', {flag: 'wx', mode: 0o600});
        core.info('quarantine: stock transport contains only the fixed empty envelope');
---

# Weekly compiler-source investigation

Use only `controller.investigate`, the authenticated data endpoint. Send JSON text
with exactly `schema_version: 1`, a unique `id`, `operation`, and `data`.
Begin with `status`, then `start_profile` (`data: {}`); poll `status` while the
owned worker runs. All waits, revisions and workers share the controller's full
240-minute active allowance, starting at authenticated readiness, not tool dispatch.
Do not run shell commands, delegate an optimizer or use GitHub writers.

Develop insights from CPU stacks, sampled allocations, GC/heap/RSS distinctions
and compiler binlog timings. This is the pinned Release compiler-source workload,
not FSharp.Core or generated-code runtime benchmarks. Keep `global.json` intact.
Consult retained hypotheses, negative results and explicit retry dependencies.
Do not repeat an unchanged rejected scope or claim missing/expired evidence exists.

Select one observed hot operation using `select_hypothesis`: `compiler_paths`,
`regressions` (`path` and exact `method`), `hypothesis`, `complexity`, and the
displayed integer `hot_operation`. Explain the smallest focused compiler change.
Submit bounded UTF-8 Git diff bytes with `submit_patch` (`data: {patch: ...}`),
then `start_validation` (`data: {}`). The host applies the patch, checks actual
paths/modes/tree, runs its required regression/area/compatibility plan, and obtains
fresh independent unprofiled A/A and A/B measurements. Neither a passing timing
nor your own assertion authorizes publication.

For a specific retention/heap-growth/unexplained-RSS hypothesis, `request_dump`
accepts only `reason` and `hypothesis`. Interpret the returned bounded evidence.
Use `finish_rejected` for a tested negative/inconclusive result, `finish_no_finding`
only for completed untested discovery, or `cancel` for incomplete work; each takes
`data: {reason: ...}`. Preserve a short measured conclusion or material blocker.
The host alone retains canonical memory, acknowledges encrypted Actions evidence,
and may publish one independently approved draft using a sealed `am` patch.
Do not create issues, comments, alternate branches or output files, request
ready-for-review, approve, merge, or change tools, policy, SDK or permissions.
