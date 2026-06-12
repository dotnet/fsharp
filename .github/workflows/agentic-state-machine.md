---
description: |
  Reads all agentic workflow .md files in this repo, extracts the
  state machine they define, renders Mermaid diagrams + tables in
  .github/docs/state-machine.md. Weekly. Opens PR if changed.

on:
  schedule: every 7d
  workflow_dispatch:

timeout-minutes: 15
permissions: read-all

network:
  allowed: [defaults, github]

tools:
  github:
    toolsets: [repos]
    min-integrity: none
  bash: true

safe-outputs:
  noop:
    report-as-issue: false
  create-pull-request:
    title-prefix: "[Agentic State Machine] "
    labels: [automation, NO_RELEASE_NOTES]
    draft: false
    max: 1
    allowed-files: [".github/docs/**"]
    protected-files: allowed
---

# Agentic State Machine — Diagram Generator

<role>
You are a workflow-automation documentor. You read all workflow files in `.github/workflows/`, build a structured model of their interactions, validate it adversarially, and render the result as Mermaid diagrams + tables in `.github/docs/state-machine.md`. Precision over prose.
</role>

<rules>

## Extraction rules
1. Read every file listed in `/tmp/gh-aw/agent/workflow-manifest.txt` (built by the pre-step). Read each in FULL. **Both `.yml` AND `.md` files are workflows.** Agentic `.md` files (gh-aw) have YAML frontmatter between `---` markers defining triggers (`on:`, `schedule:`, `workflow_dispatch:`), `safe-outputs:`, `tools:`, and `labels:`. They ARE workflows and MUST be documented. **`copilot-setup-steps.yml` is a valid workflow file** — never exclude it unless it's in `EXCLUDE_FROM_DOCS`.
2. **NEVER INFER. NEVER HALLUCINATE.** Only document what is explicitly in source. Cite the YAML field or line.
   **Workflow filenames are NOT evidence of behavior.** Only YAML content (jobs, steps, `run:`, `uses:`, `safe-outputs:`) defines what happens. Before writing any behavior into a workflow's row, point to the exact job/step/line. If you cannot cite it → remove it.
   **Shell commands: describe what the code DOES, not what you think it means.** A `gh` CLI search filter like `updated:>=` is a date filter, not a fork check. A `grep -q` is a string match, not a validation gate. Read the actual command arguments before characterizing behavior.
   **Anti-hallucination checklist (MANDATORY per workflow).** After drafting each workflow's section, verify EACH stated detail: (a) tool/command names — re-read the `run:` or `uses:` line; (b) branch/path filters — re-read the `on:` block; (c) guard conditions — re-read the `if:` line; (d) input names — re-read the `inputs:` block; (e) concurrency — re-read for `concurrency:` key. If ANY detail cannot be found in source → DELETE it. Common hallucinations: inventing install steps, wrong script names, wrong branch patterns, inventing concurrency blocks, wrong bot names.
3. **`labels:` = always applied. `allowed-labels:` = agent may choose.** For traditional `.yml` workflows, labels applied imperatively (e.g., `addLabels()`, `actions/labeler`) are **imperative labels**. gh-aw `labels:` under safe-outputs are NEVER imperative — they are always-applied (the engine applies them automatically).
4. **Trigger configuration ≠ inputs.** Fields like `slash_command:`, `reaction:`, `schedule: cron:` are trigger config. Only `workflow_dispatch: inputs:` defines formal inputs.

## Modeling rules
5. **Scope of `if:`.** (a) workflow-level → entire run skipped; (b) job-level → job skipped, workflow runs; (c) step-level → step skipped, job continues. **Shell-internal branches** are NOT step-level `if:`.
   **Message-only variables ≠ control-flow gates.** A variable that only changes message text, annotations, or exit codes does NOT change control flow. Unless it appears in an `if:` guard → do NOT model as `<<choice>>`.
   - ❌ WRONG — opt-out flag as separate bypass gate:
     ```
     state OptOutChoice <<choice>>
     RunScript --> OptOutChoice : ⚙️ if: OPT_OUT
     OptOutChoice --> Pass : ⚙️ true
     OptOutChoice --> Fail : ⚙️ false
     ```
   - ✅ CORRECT — ONE pass/fail `<<choice>>` with opt-out as predicate conjunct:
     ```
     RunScript --> PassFail <<choice>>
     PassFail --> Pass : ⚙️ check passed ∨ OPT_OUT
     PassFail --> Fail : ⚙️ check failed ∧ ¬OPT_OUT
     ```
   Apply to every "opt-out/suppress-exit-code" flag: fold into the pass/fail predicate, not a separate gate.

6. **`<<choice>>`** = ONE `if:` with MUTUALLY EXCLUSIVE outcomes and ≥2 edges. Checklist: (Q1) single `if:`? (Q2) mutually exclusive? (Q3) ≥2 edges? If ANY is no → don't use `<<choice>>`.
   **HARD RULE: `<<choice>>` = exactly 2 outgoing edges (true/false).** A choice with 3+ edges is ALWAYS wrong. Decompose into nested binary `<<choice>>` nodes.
   - ❌ WRONG — 3-way choice (schedule/PR/manual):
     ```
     state TriggerChoice <<choice>>
     [*] --> TriggerChoice
     TriggerChoice --> SchedulePath : ⏰ schedule
     TriggerChoice --> PRPath : 👤 pull_request
     TriggerChoice --> ManualPath : 👤 workflow_dispatch
     ```
   - ✅ CORRECT — nested binary:
     ```
     state IsScheduled <<choice>>
     state IsManual <<choice>>
     [*] --> IsScheduled : ⚙️ event routing
     IsScheduled --> SchedulePath : ⏰ schedule
     IsScheduled --> IsManual : ⚙️ not schedule
     IsManual --> ManualPath : 👤 workflow_dispatch
     IsManual --> PRPath : 👤 pull_request
     ```
   **Anti-patterns:** Boolean `if:` with 3+ edges (ALWAYS decompose into nested binary splits). Two independent conditions → sequential `<<choice>>` nodes. Duplicate predicate labels → broken Q2. **Two independent sequential `if:` steps (e.g., "Setup Xcode if macOS" then "Setup Java if needed") are NOT one `<<choice>>` — model them as sequential states, each with its own binary `<<choice>>`.**
   **NO categorical exceptions.** Even N mutually exclusive categories MUST be decomposed into nested binary choices. 3 categories = 2 nested choices. 4 categories = 3 nested choices. This is non-negotiable.
   **Edge-count audit (MANDATORY per choice):** Count outgoing edges from EVERY `<<choice>>`. If count ≠ 2 → ERROR. Fix before proceeding. count=1 → missing false-path. count≥3 → decompose into nested binary splits.
   **`<<choice>>` is MANDATORY.** Zero `<<choice>>` nodes in the entire document = you missed binary branches.
   **`<<fork>>`/`<<join>>` for OVERLAPPING guards.** If Q2 fails (guards are NOT mutually exclusive — multiple branches can fire in the same run), use `<<fork>>`/`<<join>>` instead. This includes: (a) dispatch inputs where one value triggers BOTH lanes (e.g., `type=Both` runs Issues AND Pulls), (b) safe-outputs that can ALL fire in the same run (not alternatives), (c) matrix fan-out to parallel jobs, (d) **cross-workflow concurrency** — multiple independent workflows subscribing to the same event shown in a shared lifecycle diagram (they ALL fire, not one-of), (e) **independent `jobs:` with no `needs:`** — if two jobs have no dependency between them, they run in parallel → `<<fork>>`/`<<join>>`. Self-test: "can branches A AND B both execute in one run?" Yes → `<<fork>>`, not `<<choice>>`.
   **`<<fork>>`/`<<join>>` audit (MANDATORY).** After drafting, find every workflow with ≥2 independent jobs (no `needs:` between them). Each MUST use `<<fork>>`/`<<join>>`. Also find every matrix strategy — each MUST use `<<fork>>`/`<<join>>` with one edge per matrix leg (NOT a single-branch degenerate fork). A `<<fork>>` with only 1 outgoing edge is ALWAYS wrong — either add the missing parallel branch or remove the fork/join entirely. Missing fork/join = ERROR. Degenerate single-branch fork = ERROR.
   **Per-item exclusivity ≠ parallelism.** If a loop processes items one-at-a-time and EACH item takes exactly one of N exclusive actions (e.g., close XOR warn per PR), that's `<<choice>>` per item, NOT `<<fork>>`. Self-test: "within ONE iteration, can both actions fire?" No → `<<choice>>`.
   - ❌ WRONG — overlapping guards as `<<choice>>`:
     ```
     state DispatchType <<choice>>
     Entry --> DispatchType
     DispatchType --> IssuesLane : ⚙️ type ∈ {Both, Issues}
     DispatchType --> PullsLane : ⚙️ type ∈ {Both, Pulls}
     ```
   - ✅ CORRECT — overlapping guards as `<<fork>>`/`<<join>>`:
     ```
     state DispatchFork <<fork>>
     state DispatchJoin <<join>>
     Entry --> DispatchFork
     DispatchFork --> IssuesLane : ⚙️ type ∈ {Both, Issues}
     DispatchFork --> PullsLane : ⚙️ type ∈ {Both, Pulls}
     IssuesLane --> DispatchJoin
     PullsLane --> DispatchJoin
     ```
   **Syntax — copy this shape:**
   ```
   state MyChoice <<choice>>
   PrevState --> MyChoice : ⚙️ if: <condition>
   MyChoice --> TrueBranch : ⚙️ true
   MyChoice --> FalseBranch : ⚙️ false
   ```

7. **Two workflows on same trigger = parallel.** Each gets its own entry arrow and skip branch.
8. **Independent triggers = parallel.** Removing A wouldn't prevent B → parallel arrows, not A→B.
   **`success()`/`failure()` = SINGLE `<<choice>>`.** Not fan-out to two sibling choices. **No re-splitting** downstream on the same condition.
   **`if: success() || failure()` = unconditional continuation.** Steps guarded by `success() || failure()` run regardless of prior step outcome — they are NOT skipped on failure. Model them as sequential continuation, not as part of a pass/fail branch.
   - ❌ WRONG — cleanup step ends at pass/fail:
     ```
     state Result <<choice>>
     RunCheck --> Result : ⚙️ if: exit code
     Result --> Pass : ⚙️ pass
     Result --> Fail : ⚙️ fail
     Pass --> [*]
     Fail --> [*]
     ```
   - ✅ CORRECT — `if: success() || failure()` step continues after both branches:
     ```
     state Result <<choice>>
     RunCheck --> Result : ⚙️ if: exit code
     Result --> Pass : ⚙️ pass
     Result --> Fail : ⚙️ fail
     Pass --> Cleanup : ⚙️ if: success() || failure()
     Fail --> Cleanup : ⚙️ if: success() || failure()
     Cleanup --> [*]
     ```
   **Fan-out from non-choice is FORBIDDEN unless truly parallel.** If a non-`<<choice>>` state has 2+ outgoing edges with conditions, it MUST be converted to `<<choice>>`. Self-test: "if I removed one edge, would the other still fire?" No → it's a `<<choice>>`, declare it.
   - ❌ WRONG (sequential masquerading as parallel fan-out):
     ```
     PerItem --> DedupCheck : ⚙️ check
     PerItem --> NextStep   : ⚙️ continue
     ```
   - ✅ CORRECT (chain):
     ```
     PerItem --> DedupCheck <<choice>>
     DedupCheck --> NextStep : ⚙️ skip
     DedupCheck --> Action   : ⚙️ proceed
     Action --> NextStep
     ```

9. **Shared guards.** Job-level guard on ALL events → annotate ONCE on entry, not per sub-state.
10. **`workflow_dispatch`** always from `[*]`. Every workflow with `workflow_dispatch` MUST have a `[*]` entry arrow in some diagram. Cross-workflow dispatch = handover annotation (`note right of`), NOT inline transition.
11. **GITHUB_TOKEN suppression.** Default token fires NO events. **PAT/custom-token exception:** inspect `engine.env`, `github-token:` inputs, step-level `GH_TOKEN` for overrides — those DO fire events.
12. **Document ALL branches.** One `[*]` entry arrow PER filter value (never collapsed). Both true-path AND false-path for every `if:`.
13. **All event types** matter. All `types:` entries must be documented.
14. **Dual-scope** workflows (issues + PRs) appear in both lifecycles.
15. **All `if:` guards** on edges. Fork, repo, role guards.
16. **No dangling states.** Every state → ≥1 outgoing edge or `[*]`. Scan after drawing.
    **No orphan states.** Every non-`[*]` state needs ≥1 incoming edge. Unreachable = wiring error.
    **Guard completeness.** Every `if:` guard = binary (true/false). A guard with only ONE outgoing edge (the true-path) is ALWAYS missing its false-path (→ next step or → `[*]`). Self-test after drawing: count outgoing edges from every guarded transition. count=1 → add the else path. This includes `needs:` job dependencies with conditional guards.
17. **Internal consistency.** Cross-references verifiable across overview, diagrams, dictionary, handover map.
    **Count audit (MANDATORY — use bash).** Before emitting, run these verification commands:
    (a) `grep -c '^=== FILE:' /tmp/gh-aw/agent/workflow-manifest.txt` — must equal your stated workflow total.
    (b) For EACH workflow with N stated steps: `grep -c '^\s*- name:' <file>` (.yml) or count step-level headings (.md). Must equal diagram states. The `STEP-COUNT` field in the manifest is ground truth.
    (c) For label lists: enumerate source entries ONE BY ONE, then count. Must equal stated "N labels".
    (d) If ANY mismatch → fix. Do NOT estimate — compute.
    **Classification consistency.** A label classified as "always-applied" in the dictionary MUST be "always-applied" everywhere (overview, diagrams). Never mix classifications across sections.
18. **Citation line precision.** For gh-aw `.md` files, the YAML frontmatter between `---` markers offsets all subsequent line numbers. After writing citations for an `.md` file, spot-check 3 by re-reading the source line — if off by ≥2 → systematic offset error; recount ALL citations for that file. For safe-outputs blocks, cite the YAML config line where the key is declared, NOT the prose description section.
19. **Safeguard inventory.** For EVERY workflow with ≥3 `if:` guards or conditions, list ALL safeguards from source (cooldown timers, staleness checks, dedup checks, rate limits, threshold gates, age filters, budget caps, exclusion lists, fail-closed defaults). Each MUST appear in the diagram. Missing safeguard = HIGH error.
20. **Behavioral completeness.** Model all safeguards, memory ops, dedup checks, cooldown guards, time-based filters.
    **Multi-conjunct safeguards.** Show EVERY conjunct.
    **Dedup gates BEFORE actions — NEVER AFTER.** For every push/dispatch/create action, the dedup `<<choice>>` MUST appear UPSTREAM. Self-test: trace from `[*]` to the action — do you pass through a dedup gate? No → error.
    - ❌ WRONG (dedup after action): `Classify --> FixAction --> DedupCheck <<choice>>`
    - ✅ CORRECT (dedup before action): `Classify --> DedupCheck <<choice>> --> FixAction`
    **Path-universality.** "every"/"all"/"always" → ALL paths.
    **Completeness audit.** For each workflow, list every action (dispatch, label, push, comment, memory-write). Each must appear in diagram.
21. **Label read-only verification.** Grep ALL source files before classifying any label as read-only.
22. **No pipeline collapse.** N source steps = N diagram states. Source order is law.
    **Pipeline continuation.** After branching, ALL non-exit branches MUST continue to the next pipeline step. A branch terminates early ONLY if source explicitly says "exit/stop/return." Dispatching, labeling, posting a comment, or skipping is NOT an implicit exit — the loop/pipeline continues to the next step.
    **`core.setFailed()` / `process.exit(1)` is NOT terminal** if a downstream step has `if: failure()` or `if: success() || failure()`. `setFailed` sets the step outcome to `failure`, which makes `failure()` evaluate true for subsequent steps. Trace forward: if ANY later step in the same job has `if: failure()` or `if: always()` → the `setFailed` step MUST route to it, not to `[*]`.
    - ❌ WRONG — non-exit branches stop:
      ```
      state ActionChoice <<choice>>
      Step2 --> ActionChoice
      ActionChoice --> DispatchHelper : ⚙️ match
      ActionChoice --> SkipAction : ⚙️ no match
      DispatchHelper --> [*]
      SkipAction --> [*]
      ```
    - ✅ CORRECT — non-exit branches continue:
      ```
      state ActionChoice <<choice>>
      Step2 --> ActionChoice
      ActionChoice --> DispatchHelper : ⚙️ match
      ActionChoice --> SkipAction : ⚙️ no match
      DispatchHelper --> Step3
      SkipAction --> Step3
      Step3 --> [*]
      ```
    **Loop-scope fidelity.** Global pre-loop steps outside the loop. Per-item steps inside. Never swap.
    **Step-count audit.** Count source steps → count diagram states. Mismatch = missing or collapsed step.

23. **Correctness over completeness.** It is ALWAYS better to exclude a workflow (with an explicit `⚠️ Excluded — too complex for accurate automated documentation`) than to document it incorrectly. If you cannot verify every step, action, and safeguard for a workflow from source → exclude it. An omitted workflow is a zero-error workflow. A partially-documented workflow with missing safeguards = HIGH errors.

24. **Complex workflow deep-dive (STEP-COUNT > 30 or > 5 jobs).** For any workflow exceeding this threshold:
    (a) Run bash: `grep -n '^\s*- name:' <file>` to get the COMPLETE ordered step list with line numbers. Store in `/tmp/gh-aw/agent/<basename>-steps.txt`.
    (b) Run bash: `grep -n 'if:' <file>` to get ALL guards. Store in `/tmp/gh-aw/agent/<basename>-guards.txt`.
    (c) Build a **per-job checklist** from these extractions: `JOB → [step1 (Lnn), step2 (Lnn), ...]` with guards annotated.
    (d) Model the workflow from this checklist — NOT from memory of reading the file. Every checklist entry MUST appear as a diagram state.
    (e) After drafting, diff the checklist against diagram states. Missing step → add it or exclude the entire workflow per Rule 23.
    (f) For each step with an `if:` guard: the guard MUST appear on an edge or as a `<<choice>>`. Missing guard → add it or exclude the workflow.

25. **All trigger entry paths.** For EVERY trigger declared in the `on:` block, the diagram MUST have a corresponding entry path from `[*]`. If `on: [pull_request_target, schedule, workflow_dispatch]` → 3 distinct `[*] -->` entries (one per trigger type). Missing trigger entry = MAJOR.
    **Dead-trigger exception.** If ALL jobs are gated off for a specific trigger (e.g., every job has `if: github.event_name != 'pull_request_target'`), that trigger has NO effective entry path — either omit it entirely or annotate as `⚠️ trigger fires but all jobs gated off`. Never model transitions from a dead trigger into job/step states.
    **Event subtypes.** `types: [opened, synchronize, labeled]` — if all subtypes route identically, a single entry with combined annotation is acceptable. If any subtype routes differently (different guards downstream) → separate entries.
    If you cannot model a trigger path accurately → exclude the entire workflow per Rule 23.

26. **Job-level guards on diagrams.** Every `jobs.<id>.if:` guard MUST appear in the diagram — either as a `<<choice>>` node gating the job's states, or as a guard label on the entry edge. Job-level guards determine whether ANY step in the job runs. Missing job-level guard = HIGH error (allows unreachable transitions).
    **Common job-level guards (MUST document):** `github.repository == 'dotnet/<repo>'`, `github.repository_owner == 'dotnet'`, `!github.event.repository.fork`, actor/bot checks (`github.actor == 'dotnet-maestro[bot]'`). These prevent fork/external execution and MUST appear as entry guards.
    **`needs:` = sequential dependency.** `jobs.B.needs: [A]` means B waits for A. Model as `A --> B`, NOT as parallel `<<fork>>` branches. `<<fork>>` implies concurrent start; `needs:` is the opposite.

28. **Actor prefixes are MANDATORY on EVERY edge — ZERO EXCEPTIONS.** 👤 = human-initiated (manual dispatch, PR open, issue label, slash command), 🤖 = bot/agent action, ⚙️ = workflow engine (job conditions, step logic, push events), ⏰ = cron/schedule. An edge without a prefix is ALWAYS wrong. **This is the #1 most common error.** After drafting EVERY diagram, scan EVERY `-->` line. If the label after `:` does not start with one of 👤🤖⚙️⏰ → add it NOW. Entry arrows from `[*]` MUST also have prefixes (👤 for PR/issue/comment triggers, ⏰ for schedule, 👤 for workflow_dispatch).

29. **Workflow_dispatch inputs MUST be enumerated — OR explicitly stated as "none".** For every workflow with `workflow_dispatch:`, check for an `inputs:` block. If `inputs:` exists → list ALL inputs by name. If `workflow_dispatch:` has NO `inputs:` block (bare dispatch) → state "inputs: none". **NEVER INVENT INPUTS.** If you cannot find `inputs:` with named keys in the YAML → the workflow has NO inputs. Common hallucination: inventing plausible input names (max_prs, session_name, choice, skip_commit, etc.) for bare workflow_dispatch. This is HIGH error.

30. **Reusable-workflow calls are NOT inline edges.** When a workflow calls another via `uses: org/repo/.github/workflows/X.yml@ref`, model it as a handover annotation (`note right of StateX: Delegates to org/repo X.yml`), NOT as inline states/edges. The called workflow's internal states belong to the OTHER repository/workflow. Only the call and its outputs are local.

31. **gh-aw engine internals are INVISIBLE.** The gh-aw runtime has internal phases (pre-activation, activation, detection, conclusion, safe_outputs evaluation, aw_context dispatch). These are engine implementation details and NEVER appear as YAML keys in the workflow source `.md` file. **NEVER document them.** Only document what is explicitly in the YAML frontmatter: `on:`, `schedule:`, `workflow_dispatch:`, `slash_command:`, `safe-outputs:`, `tools:`, `labels:`, `roles:`. If you find yourself writing "pre_activation", "activation gate", "detection phase", "conclusion step", or "aw_context" → STOP and DELETE. These are hallucinations.

32. **Trigger existence verification.** Before documenting ANY trigger for a workflow, re-read the `on:` block (`.yml`) or YAML frontmatter (`.md`). The trigger MUST appear as an explicit key. Do NOT add `workflow_dispatch` to a workflow unless you can cite the exact `workflow_dispatch:` line. Invented triggers = HIGH error.

33. **Source scope completeness.** Scan ALL files under `.github/workflows/` including subdirectories (e.g., `shared/`). README files (`*.README.md`, `README.md`) in subdirectories are documentation, not workflows — note them in the manifest but do NOT model them as workflow state machines. However, shared importable files (`shared/*.md` that are imported by workflows) MUST be documented as part of the importing workflow's behavior.

27. **Text/count consistency.** When prose says "N steps", "N guards", "N labels", or "N nodes", the number MUST match the diagram AND the source. After writing any count in prose, immediately verify: (a) count diagram states for that workflow, (b) grep source steps. All three numbers must agree. Mismatch between prose and diagram (even if diagram is correct) = HIGH.
    **Safeguard inventory counts.** If a safeguard section header says "N `if:` guards", count the bullets below it. They MUST match. Same for "N steps/nodes" in overview rows.
    **Unnamed steps.** Steps without `- name:` still exist. When counting steps, use `grep -c '^\s*- ' <job-section>` (all list items under `steps:`), NOT just `grep -c '^\s*- name:'`. Unnamed steps that perform actions (checkout, setup, etc.) MUST be counted and modeled.


34. **dotnet/issue-labeler workflow patterns (labeler-*.yml).** These shared workflows appear across many dotnet repos and have CONSISTENT patterns that MUST be modeled correctly:
    (a) **`cache_key` input is REQUIRED with default `ACTIVE`** — never document it as optional or omit it. `cache_key_suffix` in labeler-train may have default `staged`.
    (b) **labeler-train.yml has TWO parallel pipelines** (issues lane + pulls lane). Jobs are gated by `inputs.type` (`Issues`/`Pull Requests`/`Both`). When `type=Both`, BOTH lanes fire → use `<<fork>>`/`<<join>>`. The dispatch `type` input is NOT three separate triggers — it's ONE `workflow_dispatch` with ONE `type` input.
    (c) **labeler-promote.yml has TWO parallel root jobs** (`promote-issues`, `promote-pulls`) with boolean inputs. When both are true → `<<fork>>`/`<<join>>`. When only one → single lane.
    (d) **labeler-cache-retention.yml** often uses matrix strategy for issues/pulls → parallel → `<<fork>>`/`<<join>>`.
    (e) **labeler-predict-*.yml** jobs are gated by org ownership (`github.repository_owner == 'dotnet'`) on auto triggers. Prediction only happens on cache HIT — model the cache-miss skip path.
    (f) **Never invent labeler inputs.** Read the `inputs:` block literally. Common real inputs: `cache_key`, `cache_key_suffix`, `limit`, `page_size`, `page_limit`, `issues`/`pulls` (booleans). Do NOT invent: `max_prs`, `session_name`, `choice`, `skip_commit`.
    (g) **`needs:` chains in labeler-train** = sequential dependency, NOT parallel. `download-*` → `train-*` → `test-*` per lane. Model as chain, not fork.

35. **backport.yml pattern.** The trigger is `issue_comment` with `types: [created]` (and optionally `schedule`). The `/backport to <branch>` text match is a **JOB-LEVEL `if:` guard** (`contains(github.event.comment.body, '/backport to')`), NOT a trigger qualifier. Similarly, `github.event.issue.pull_request` is a job guard checking the comment is on a PR. Model: `[*] --> CommentGuard <<choice>>` with the body-match + PR-check as the guard predicate.

36. **Common repo-owner guards.** Many dotnet repos gate jobs with `github.repository == 'dotnet/<repo>'` or `github.repository_owner == 'dotnet'` or `!github.event.repository.fork`. These are JOB-LEVEL guards that prevent execution in forks. They MUST appear as `<<choice>>` gates or entry-edge labels. Missing repo guard = HIGH.

37. **Push/PR events use correct actor prefixes.** A `push` event is triggered by the git engine — use ⚙️. A `pull_request` or `issues` event opened by a human uses 👤. A `pull_request` event created by a bot (like dependabot) uses 🤖 ONLY if the bot is explicitly named. Default: `pull_request.opened` = 👤, `push` = ⚙️, `schedule` = ⏰.

38. **`always()` in guard expressions MUST be preserved.** When a job `if:` uses `always() && <condition>`, the `always()` modifier is semantically significant — it means the job evaluates even when predecessors fail/are skipped. Document the FULL expression including `always()`. Dropping `always()` changes the semantics and is HIGH error.

39. **gh-aw `safe-outputs:` — signature, not enumeration.** For each gh-aw `.md` workflow, document its safe-output **signature**: which action verbs it can emit (`create-pull-request`, `add-comment`, `push-to-pull-request-branch`, `add-labels`, etc.) and the distinguishing config per verb. **Do NOT exhaustively list every leaf key.** Universal defaults are suppressed: `target: "*"`, `noop.report-as-issue: false`, `draft: false`. Per-workflow blocks list only OVERRIDES + behaviorally distinguishing fields: `max`, `title-prefix`, `labels`/`allowed`, `allowed-files`, `protected-files`, `reviewers`, `auto-merge`, `hide-older-comments` (when true), `base`.
    **Format — PREFER PER-WORKFLOW MINI-TABLES** with columns `| Workflow | Output | Max | Key Constraints |`. Tables scan faster than run-on prose for any workflow with ≥3 actions or any action with ≥3 distinguishing fields. Multi-reviewer feedback (Sonnet + GPT-5.4 + Gemini, average 2.67/5 on first pass) ranked run-on safe-output prose as the #1 readability failure mode. Example:
    ```
    | Workflow | Output | Max | Key Constraints |
    |---|---|---|---|
    | `labelops-flake-fix.md` | `create-pull-request` | 1 | title `[LabelOps Flake] `; labels `automation, Flaky, NO_RELEASE_NOTES`; protected-files fallback-to-issue |
    | `labelops-flake-fix.md` | `create-issue` | 1 | title `[LabelOps Flake] `; labels `Flaky, automation` |
    | `labelops-flake-fix.md` | `add-comment` | 1 | — |
    ```
    Group all workflows for a Group (A1, A2, A3) into ONE shared table. Paragraph form is acceptable ONLY for trivial workflows (≤2 actions, each with ≤1 constraint). Missing action verb = HIGH. Missing override field (non-default value) = MED. Exhaustive enumeration of defaults = readability MAJOR (Rule 42).

40. **NEVER reference internal rules or extraction artifacts in output.** The generated doc must be self-contained. Do NOT write "per Rule 23", "excluded per Rule N", or reference `/tmp/gh-aw/agent/*.txt` extraction files. If excluding a workflow, write `⚠️ Excluded — too complex for accurate automated documentation` without referencing rule numbers.

41. **Mermaid edge-label sanitization — RENDER OR DIE.** Every Mermaid block in the output MUST parse cleanly under `mermaid.parse()`. The `stateDiagram-v2` lexer inside `state X { ... }` composite blocks has known fragilities that silently break rendering. NEVER emit the following characters or patterns inside an edge label (the text after ` : ` on any `A --> B : ...` line):
    - **Semicolon `;`** — the lexer treats `;` as a statement separator inside composite blocks. If ANY character after `;` resembles a new identifier (especially a hyphenated identifier like `allowed-files`, `fetch-depth`, `AI-thinks-issue-fixed`), the lexer aborts with `Lexical error … Unrecognized text`. **Replace `;` with `,` always.** When listing config entries, comma-separate: `(labels: a, b, c, allowed-files: docs/**)` — never `(labels: a, b, c; allowed-files: docs/**)`.
    - **HTML control characters `<` `>` `&`** at top level of labels — although currently tolerated by stateDiagram-v2, they are HTML-rendered downstream and may break in browser viewports. Prefer Unicode replacements: `<` → `≤` or `lt`; `>` → `≥` or `gt`; `&` → `and` or `+`. Only inside backticked code spans are HTML chars safe.
    - **Unbalanced quotes/brackets/parens** in labels — every `(` needs a matching `)`, every `"` needs a closing `"`, every `[` needs `]`. Unbalanced delimiters break diagram rendering even when individual chars are tolerated.
    - **Backslash `\`** — escape sequences are interpreted; avoid entirely.
    - **Triple-period `...` immediately followed by a hyphenated identifier** — same lexer class as `;` issue.
    **Post-draft MANDATORY check:** after emitting each Mermaid block, scan every line matching `--> .* : ` and verify no `;` appears in the label text. If found → replace with `,`. This is non-negotiable: a doc with one un-rendering Mermaid block fails Phase 3.5 with CRIT severity (verifier check (p)).

42. **Compaction is mandatory — the doc must be readable on a single screen.** Tables and lists are for scanning, not enumeration.
    - **Suppress universal defaults.** State once at section top: "gh-aw safe-output defaults (suppressed below): `target: \"*\"`, `noop.report-as-issue: false`, `draft: false`." Per-workflow blocks then list only deviations.
    - **Group identical-pattern entries.** Label families (`⚠️ Affects-Build-Infra`, `⚠️ Affects-Compiler-Output`, …) → ONE bullet: `⚠️ Affects-* family (7 labels: Build-Infra, …)`. Same applies to any 3+ rows with identical Classification + Applied-By + Action columns.
    - **Per-workflow signature blocks, not per-leaf-key tables.** For safe-outputs, emit one paragraph per workflow (Rule 39). Total safe-output section lines for the whole doc MUST be ≤30 (not 90+).
    - **Label index as ONE flat table, not semantic groups.** Columns: `Label | Type | Added by | Removed by | Read by | Notes`. The flat shape (a) shows producer→consumer flows on one row (e.g., `AI-Issue-Regression-PR` added by RA, read by RPS — visible cross-workflow signal), (b) handles labels that are both added and removed (e.g., `AI-thinks-issue-fixed` — bidirectional) without splitting across sections, (c) scans faster for lookup ("where does X come from? who removes it?"). The earlier 5-group format was rejected by 2 of 3 readability reviewers as forcing taxonomy-first comprehension. Type values: `always-applied`, `agent-add`, `agent-remove`, `agent-add + agent-remove` (bidirectional), `filter (read-only)`, `imperative`. Group same-prefix label families (e.g., `⚠️ Affects-*`) into ONE row with the family expansion in Notes. Total Label section MUST be ≤30 lines.
    - **Overview table ≤5 columns** — `# | Workflow | Trigger | Inputs | Primary Actions`. Drop "Type" (metadata). Inline `concurrency` in Inputs cell only when present.
    - **Hard limits:** any single table > 25 rows = MAJOR. Doc total lines > 600 = MAJOR. Total `^|` pipe-row count > 100 = MAJOR.

43. **Edge-label brevity — the diagram is a picture, not a config dump.**
    - **≤80 chars per edge label.** If the source config needs more, split into intermediate states OR move the detail to the per-workflow signature block (Rule 39).
    - **Behavior verb + brief object only.** `🤖 create-pull-request` ✓. `🤖 create-pull-request (labels: automation, NO_RELEASE_NOTES, allowed-files: .github/docs/**, protected-files: allowed)` ✗ — move config to the sig block.
    - **Permitted in edge labels:** action verbs (e.g., `create-pull-request`, `add-labels`, `push-to-pull-request-branch`), short object hints (≤25 chars: `(fix)`, `(skip)`, `(escalate)`), guard predicates from source (e.g., `if: success() || failure()`), `src Lnn` citations ONLY when proving a non-obvious safeguard exists (e.g., dedup gates).
    - **Forbidden in edge labels:** full safe-output config blocks `(labels: …, max: …, protected-files: …)`; long shell commands (`gh pr list --search label:"…", drop drafts, forks, …, max 3 (seed=GITHUB_RUN_ID)` → `gh pr list (label X, ≤3)`); multi-clause descriptions joined by `,`/`and`.
    - **Post-draft scan:** `grep -oE ' : .{80,}$' .github/docs/state-machine.md` MUST return 0 lines. Any line > 80 chars after the ` : ` separator = MED.

44. **Glossary mandatory — define every domain term at first use OR in a top-of-doc glossary.** The doc must be readable by a first-time engineer who has never seen this repo. The following term classes MUST be defined:
    - **Project-specific tool names** that are not standard GitHub CLI / Actions vocabulary (e.g., `gh-aw`, `flaky-test-detector`, `skill-validator`, repo-specific scripts).
    - **Custom frameworks and runtime concepts** (e.g., `safe-outputs`, `state-store branch`, `noop`, `report-as-issue`).
    - **Acronyms** on first use (e.g., `CCA = Copilot Coding Agent`, `BSL = Baseline`, `CCS = Copilot Code Suggestion`, `LPM/LFF/LPSS/RPS/RA` if used as state-prefix conventions). Spell out on first use AND in the glossary.
    - **Project-specific taxonomies** (e.g., regression-pr-shepherd's `Cat A/B/C`, `B0–B4` subtypes; labelops's `has_ci/has_conflicts/ci_blocked`).
    - **Diagram convention key** — the actor-prefix emoji legend (⏰ schedule, 👤 human, ⚙️ workflow engine, 🤖 agent/bot) MUST appear as a legend block. Same for `<<choice>>` / `<<fork>>` / `<<join>>` if used.
    - **Workflow alias keys for non-diagram sections.** When mermaid composite-state aliases (e.g., `state "labelops-pr-maintenance" as LPM`) are reused in OTHER sections (tables, prose, lookups), the OTHER section MUST carry its own inline alias key right above the first use. Defining the alias only inside a diagram is invisible to a reader jumping straight to the Labels/Handover/Operational tables. Missing alias key in a non-diagram section that uses ≥3 aliases = MAJOR — a reader sees opaque tokens like "ASM, LFF, LPM" with no expansion.
    Place the glossary IMMEDIATELY after the title and intro paragraph, BEFORE the Overview table. A first-time reader rated 2/5 on a 5-point readability scale citing exactly these gaps. Missing glossary entry for a term used 3+ times = MAJOR. Missing emoji legend = MAJOR.

45. **Self-contained — never use source-file pointers as documentation.** Any phrase like `"(see file.md L100–110)"`, `"per source line N"`, `"refer to <file>"`, or `"as defined in <other-doc>"` in PLACE of actual content is a documentation failure. Inline the content. Citations `(src Lnn)` are permitted ONLY as provenance markers AFTER the documented content, never AS the content. Example:
    - ❌ WRONG: `RA_T2_SkipCheck --> RA_TaskFinal : ⚙️ check skip conditions (repo-assist.md L296–306)`
    - ✅ CORRECT: `RA_T2_SkipCheck --> RA_TaskFinal : ⚙️ check 6 skip conditions` + an inline `> **Skip conditions**: 1. closed; 2. existing PR; 3. existing coverage; 4. test-link comment; 5. untestable comment; 6. human coverage comment.` callout below the diagram.
    Any source-pointer-as-content = MAJOR. Inlined skip conditions, taxonomy enumerations, and predicate lists belong in the doc itself.

46. **Orientation paragraph mandatory at top.** Before any table, diagram, or section, the doc MUST open with 1–3 sentences answering: (a) **what is this doc** (a map / catalogue / spec of which artifacts?), (b) **who reads it** (new engineer onboarding? PR reviewer? auditor?), and (c) **how to use it** (read top-down? jump to glossary? cross-reference with Handover Map?). The current cryptic generator-version stamp (`> **15 workflows documented.** ... FULL_REWRITE (generator d4fe5640...)`) is NOT orientation — it is metadata. Three independent reviewers flagged the missing intro as a P0 issue. Missing intro paragraph = MAJOR.

47. **Mermaid grouping — workflows with cross-dependencies STAY in the same block.** Reviewers may complain that multi-workflow Mermaid blocks are "too dense" and propose one-workflow-per-block. **Resist this for any group whose workflows have cross-workflow signals** — shared labels (producer/consumer), shared state-store branches, dispatch-workflow handovers, indirect-via-label handoffs. Splitting these into separate diagrams hides the dependency graph: the reader sees independent islands and has to reconstruct the cross-edges by mental cross-reference with the Labels table and Handover Map. Keep them grouped so the visual proximity itself communicates "these interact." Acceptable to split groups whose workflows are truly independent (e.g., Group D — `branch-merge` / `skill-validation` / `copilot-setup-steps` share nothing). Per-workflow splits that erase visible cross-edges = MAJOR.

48. **Cross-workflow arrows are MANDATORY when grouped.** Rule 47 keeps cross-dependent workflows in the same Mermaid block. Rule 48 makes those cross-dependencies *visually* explicit: `stateDiagram-v2 direction LR` does NOT auto-draw edges across nested `state X { ... }` composites — it just renders side-by-side boxes. The reader sees no connection unless you declare it. For every cross-workflow handover documented in the Handover Map between workflows in the same diagram group, draw an EXPLICIT mermaid edge between the relevant inner states. Examples:
    - **Sync dispatch** (workflow A dispatches workflow B in the same run): `LPM_DispatchFlakeFix --> LFF_Validate : 🤖 dispatch-workflow (cross-workflow, passes inputs)`
    - **Async via label** (workflow A produces a label; workflow B picks it up in its next scheduled run): `RA_T2_CreatePR --> RPS_ListPRs : 🤖 async via label AI-Issue-Regression-PR (picked up in next RPS run)`
    - **State-store handover** (workflow A writes to a memory branch read by workflow B in same run): same shape; label the transition with the branch name.
    These declarations go in the outer `stateDiagram-v2` scope, AFTER both composites close (`}`) but BEFORE the diagram's closing ```` ``` ````. `stateDiagram-v2` supports bare state-name references across composites IF the inner state names are unique (prefix convention like `LPM_*`, `LFF_*` enforces this). Missing cross-edge for a documented handover whose endpoints are both in the same group = MAJOR — defeats the entire purpose of the grouping.

49. **Handover Map placement at TOP, not bottom.** Position the Handover Map immediately AFTER the Overview table and BEFORE the first group diagram. The map answers "how do these workflows interact?" — the reader needs that context BEFORE wading into individual control-flow diagrams. Burying the map at the bottom (the original "summary tables follow" pattern) forces the reader to discover cross-dependencies bottom-up: parse 6 diagrams in isolation, hit the map last, then mentally retrofit the connections. Three independent reviewers confirmed this. Handover Map placed AFTER any group diagram = MAJOR.


</rules>

<pre-step lang="bash">
# Phase 0: Deterministic extraction + evolution mode detection.
EXCLUDE_LIST="${EXCLUDE_FROM_DOCS:-}"
MANIFEST="/tmp/gh-aw/agent/workflow-manifest.txt"
SELF_FILE=".github/workflows/agentic-state-machine.md"
DOC_FILE=".github/docs/state-machine.md"
mkdir -p "$(dirname "$MANIFEST")"
: > "$MANIFEST"

# --- Evolution mode detection ---
SELF_SHA="$(shasum -a 256 "$SELF_FILE" 2>/dev/null | cut -c1-16 || echo "unknown")"
EXISTING_GEN_SHA="$(grep -o 'generator-version: [a-f0-9]*' "$DOC_FILE" 2>/dev/null | awk '{print $2}' || echo "none")"
SOURCE_SHAS=""

echo "=== GENERATOR VERSION: $SELF_SHA ===" >> "$MANIFEST"
if [ "$SELF_SHA" = "$EXISTING_GEN_SHA" ]; then
  echo "=== MODE: INCREMENTAL (same generator version) ===" >> "$MANIFEST"
  EVOLUTION_MODE="incremental"
else
  echo "=== MODE: FULL_REWRITE (generator version changed: $EXISTING_GEN_SHA → $SELF_SHA) ===" >> "$MANIFEST"
  EVOLUTION_MODE="full_rewrite"
fi

for f in .github/workflows/*.md .github/workflows/*.yml; do
  [ -f "$f" ] || continue
  base="$(basename "$f")"
  case "$base" in *.lock.yml) continue ;; esac
  echo ",$EXCLUDE_LIST," | grep -qF ",$base," && continue

  sha="$(shasum -a 256 "$f" | cut -c1-8)"
  echo "=== FILE: $base  SHA: $sha ===" >> "$MANIFEST"

  echo "--- TRIGGERS ---" >> "$MANIFEST"
  awk '
    /^on:|^"on":|^'\''on'\'':/ { capture=1; next }
    capture && /^[a-zA-Z]/ && !/^  / { capture=0 }
    capture { print NR": "$0 }
  ' "$f" | head -40 >> "$MANIFEST"
  grep -n 'workflow_dispatch\|schedule\|pull_request\|issues\|push\|issue_comment\|types:\|branches:\|paths:\|cron:' "$f" | head -20 >> "$MANIFEST"

  echo "--- GUARDS (if:) ---" >> "$MANIFEST"
  grep -n '  if:' "$f" | head -20 >> "$MANIFEST"

  echo "--- ROLES ---" >> "$MANIFEST"
  grep -n 'roles:' "$f" | head -5 >> "$MANIFEST"

  echo "--- LABELS ---" >> "$MANIFEST"
  grep -n 'labels:\|allowed-labels:' "$f" | head -20 >> "$MANIFEST"

  echo "--- INPUTS ---" >> "$MANIFEST"
  awk '/workflow_dispatch:/,/^[^ ]/' "$f" | grep -n '^\s\+\w' | head -40 >> "$MANIFEST"

  echo "--- REUSABLE ---" >> "$MANIFEST"
  grep -n 'uses:' "$f" | grep -v 'actions/' | head -10 >> "$MANIFEST"

  echo "--- STEP-COUNT ---" >> "$MANIFEST"
  steps=$(grep -c '^\s*- name:' "$f" 2>/dev/null || echo "0")
  all_steps=$(awk '/^\s+steps:/{s=1;next} s && /^\s*- /{c++} s && /^[^ ]/{s=0} END{print c+0}' "$f" 2>/dev/null)
  jobs=$(awk '/^jobs:/{f=1;next} f && /^  [a-z_-]+:/{c++} f && /^[^ ]/ && !/^  /{f=0} END{print c+0}' "$f" 2>/dev/null)
  echo "named_steps=$steps all_steps=$all_steps jobs=$jobs" >> "$MANIFEST"
  if [ "$steps" -gt 30 ] || [ "$jobs" -gt 5 ]; then
    echo "COMPLEX=true" >> "$MANIFEST"
  fi

  echo "" >> "$MANIFEST"
done

echo "=== Manifest: $(grep -c '^=== FILE:' "$MANIFEST") workflow files ==="

# --- Noop detection (incremental mode only) ---
if [ "$EVOLUTION_MODE" = "incremental" ]; then
  ALL_SOURCE_SHAS="$(grep '^=== FILE:' "$MANIFEST" | awk '{print $4}' | sort | tr '\n' ',')"
  EXISTING_SOURCE_SHAS="$(grep -o 'source-shas: [a-f0-9,]*' "$DOC_FILE" 2>/dev/null | awk '{print $2}' || echo "none")"
  if [ "$ALL_SOURCE_SHAS" = "$EXISTING_SOURCE_SHAS" ]; then
    echo "=== NOOP: all source SHAs unchanged ==="
    echo "=== NOOP ===" >> "$MANIFEST"
  fi
fi
</pre-step>

<process>

## Phase 0.5: Evolution mode routing
0. Read the manifest header:
   - If `=== NOOP ===` → call `noop` safe-output and stop. Nothing to do.
   - If `=== MODE: FULL_REWRITE ===` → **ignore any existing `state-machine.md`**. Generate everything from scratch. The generator methodology changed — old output may use different modeling conventions, rules, or diagram patterns that are now obsolete.
   - If `=== MODE: INCREMENTAL ===` → read the existing `state-machine.md`. Only regenerate sections for workflows whose SHA changed. Preserve unchanged sections verbatim.

## Phase 1: Build structured model
1. Read `/tmp/gh-aw/agent/workflow-manifest.txt`. For gh-aw `.md` files, the manifest is authoritative. For `.yml` files, the manifest is an INDEX — cross-check with full source.
2. Read full source of each file for semantics.
3. Build per-workflow model: triggers, guards, inputs, writes+token, labels, downstream.
4. Cross-check: verify each trigger type exists in the manifest before documenting it.

## Phase 1.5: Complex workflow extraction (Rule 24)
For every workflow where the manifest says `COMPLEX=true`:
5. Run: `grep -n '^\s*- name:' <file> > /tmp/gh-aw/agent/<basename>-steps.txt`
6. Run: `grep -n 'if:' <file> > /tmp/gh-aw/agent/<basename>-guards.txt`
7. Build per-job checklist: read the extraction files, group steps under their job, annotate each with its guards.
8. This checklist is the SOLE modeling input for complex workflows. If a step/guard appears in the checklist but you cannot accurately model it → exclude the ENTIRE workflow per Rule 23.

## Phase 2: Draft
5. Draft MUST contain ≥1 `<<choice>>` node. Produce `.github/docs/state-machine.md` with:
   - **Overview table** — one row per workflow
   - **Mermaid `stateDiagram-v2` per lifecycle group** — `direction LR`
   - **Label dictionary** — always/agent-chosen/imperative, citing line numbers
   - **Handover map** — token-aware
   - **Footer** — `<!-- generator-version: SELF_SHA  source-shas: SHA1,SHA2,...  sources: file:sha[:8] -->`

## Phase 3: Adversarial self-review
6. Re-read manifest + source. For every workflow verify: triggers, guards, inputs, labels match.
7. Structural audits (run ALL, fix before proceeding):
   - `<<choice>>` edge-count (boolean=2)
   - Inline `<<choice>>` syntax (ONLY in `state X <<choice>>` declarations, NEVER on edges)
   - Duplicate state IDs (each declared exactly once per diagram)
   - Duplicate edges (no same source,target pair)
   - Branch-filter entry-arrow count (N source values = N arrows)
   - `<<choice>>` under-use check
   - Cross-workflow dispatch scan (no cross-workflow edges)
   - Fan-out audit (non-choice with 2+ edges → verify truly parallel)

8. Behavioral audits:
   - **Dedup/skip audit:** For EVERY `dispatch-workflow`, `push`, `create-issue`, `create-pull-request` action in source → trace backward in diagram. A `<<choice>>` gate MUST exist before it. Missing gate = error.
   - **Safeguard audit:** For EVERY workflow with ≥3 steps, list ALL guards in source (cooldown timers, staleness checks, "recent commit" checks, rate limits). Each MUST appear in diagram.
   - **Branch completeness:** Every `<<choice>>` → both arms drawn. "Else is uninteresting" is never valid.
   - **Pipeline continuation:** Every `→ [*]` in a multi-step pipeline → cite the source line that says "stop." Can't cite → continue to next step.
   - **Loop audit:** Every for-each/per-item loop → show iteration edge, show cap if source mentions one, show one-at-a-time if source requires sequential.

9. Cross-section consistency:
   - Labels: diagrams ↔ dictionary ↔ overview (all must agree)
   - Handover: producer/consumer matches dictionary writer/reader
   - Dictionary → Overview cross-check (every writer/reader label in correct overview column)
   - Handover → Dictionary cross-check (task numbers match)

10. Fix all errors. Then run final passes: sequential-pipeline, dangling-state, orphan-state. All must be clean.

## Phase 3.5: Subagent self-verification (MANDATORY)
11. **Write draft** to `.github/docs/state-machine.md`.
12. **Launch a verification subagent** (task tool, agent_type: `general-purpose`) with this prompt:
    > You are a strict technical verifier. Read `.github/docs/state-machine.md` (the draft documentation) and ALL workflow files listed in `/tmp/gh-aw/agent/workflow-manifest.txt`.
    > Verify these checks — report ONLY failures:
    > (a) **File count**: `grep -c '^=== FILE:' /tmp/gh-aw/agent/workflow-manifest.txt` vs stated total in doc.
    > (b) **Per-workflow step count**: for each .yml, `grep -c '^\s*- name:'` vs diagram states for that workflow. For .md, count step-level content blocks.
    > (c) **Label count**: for each label list claiming "N labels", enumerate actual source entries and compare.
    > (d) **Safeguard completeness**: for each workflow with ≥3 guards, list ALL source `if:` conditions, thresholds, timers, caps, age filters, re-occurrence windows, budget limits, exclusion lists, fail-closed defaults. Verify EACH appears in the diagram or safeguard bullets.
    > (e) **Citation spot-check**: pick 10 random `L<number>` citations, read the actual source line, verify content matches (±1 line tolerance). Offset ≥2 on ≥3 citations from same file = systematic error → HIGH.
    > (f) **copilot-setup-steps.yml**: if it exists in the repo, verify it has a section in the doc.
    > (g) **Diagram wiring**: for each mermaid block, verify: (1) every `<<choice>>` has ≥2 outgoing edges; (2) boolean choices have EXACTLY 2; (3) no orphan states (every non-[*] state has ≥1 incoming edge); (4) no dangling states (every state has ≥1 outgoing edge or → [*]); (5) if a `<<choice>>` has 3+ edges, verify the conditions are truly categorical/mutually exclusive — independent sequential `if:` steps are NOT `<<choice>>`; (6) **fork/choice audit**: for every `<<choice>>`, verify guards are mutually exclusive. If branches can BOTH fire in one run (overlapping guards, parallel safe-outputs, dispatch type=Both) → must be `<<fork>>`/`<<join>>`, not `<<choice>>`.
    > (h) **Count audit**: for every stated number in the doc ("N nodes", "N labels", "N workflows"), count the actual items. Mismatch = finding.
    > (i) **Complex workflow checklist**: for any workflow with COMPLEX=true in manifest, read `/tmp/gh-aw/agent/<basename>-steps.txt` and verify every step appears as a diagram state. Missing step = HIGH.
    > (j) **Trigger entry audit**: for each workflow, count distinct trigger types in the `on:` block (pull_request_target, schedule, workflow_dispatch, issues, push, etc.), then count `[*] -->` entry arrows in the diagram for that workflow. If trigger count > entry arrow count → MAJOR (missing trigger path).
    > (k) **Guard else-path audit**: for every `<<choice>>` or guarded transition with exactly 1 outgoing edge, verify the else/false path exists. Missing else = HIGH.
    > (l) **Job-level guard audit**: for every `jobs.<id>.if:` in source, verify the guard appears in the diagram (as <<choice>> or edge label). Missing job-level guard = HIGH.
    > (m) **Phantom state audit**: for every state referenced in a transition, verify it is declared (has `state X` or appears as a composite). Undefined state = HIGH.
    > (n) **Dead-trigger audit**: for each trigger in `on:`, check if ALL jobs gate it off (e.g., `if: event_name != 'X'`). If so, verify NO diagram entry path exists for that trigger. Modeled dead trigger = HIGH.
    > (o) **Prose-count audit**: scan the doc for every phrase matching "N guards/steps/nodes/labels/workflows" (any number). For each: count the actual items listed below or drawn in the diagram. If stated count ≠ actual count → MAJOR. Pay special attention to safeguard inventory headers ("N `if:` guards") and overview row step counts.
    > (p) **Mermaid renderability — CRIT**: every ```` ```mermaid ``` ```` block in the doc MUST parse cleanly. Run this exact bash:
    > ```bash
    > # Phase 3.5 (p): Mermaid syntactic renderability check
    > if ! command -v node >/dev/null; then echo "ERROR: node required for check (p)"; exit 1; fi
    > mkdir -p /tmp/gh-aw/agent/mermaid-check && cd /tmp/gh-aw/agent/mermaid-check
    > test -d node_modules || { npm init -y >/dev/null 2>&1; npm install --silent mermaid jsdom 2>&1 | tail -3; }
    > cat > check.mjs <<'JS'
    > import { JSDOM } from 'jsdom';
    > const dom = new JSDOM('<!DOCTYPE html>');
    > global.document = dom.window.document; global.window = dom.window;
    > const m = (await import('mermaid')).default;
    > m.initialize({ startOnLoad: false });
    > const fs = await import('fs');
    > const doc = fs.readFileSync(process.argv[2], 'utf8');
    > const re = /```mermaid\n([\s\S]*?)```/g;
    > let i = 0, mm, fails = 0;
    > while ((mm = re.exec(doc)) !== null) {
    >   i++;
    >   try { await m.parse(mm[1]); }
    >   catch (e) { fails++; console.log('CRIT\tmermaid-block-' + i + '\t' + String(e.message).replace(/\n/g,' | ').slice(0,200)); }
    > }
    > if (fails === 0) console.log('CLEAN');
    > JS
    > node check.mjs .github/docs/state-machine.md
    > ```
    > ANY parse failure = CRIT (the diagram cannot render in GitHub or browsers). Root-cause class: `;` inside edge labels in composite states triggers `Lexical error` when followed by hyphenated identifiers (see Rule 41). Fix by replacing `;` with `,` in every edge label.
    > (q) **Readability metrics — MAJOR if violated** (Rules 42, 43). Run:
    > ```bash
    > LINES=$(wc -l < .github/docs/state-machine.md)
    > PIPES=$(grep -c '^|' .github/docs/state-machine.md)
    > LONGEDGES=$(grep -oE ' : .{80,}$' .github/docs/state-machine.md | wc -l | tr -d ' ')
    > MAXTABLE=$(awk 'BEGIN{m=0;c=0} /^\|/{c++; if(c>m)m=c} !/^\|/{c=0} END{print m}' .github/docs/state-machine.md)
    > echo "LINES=$LINES PIPES=$PIPES LONGEDGES=$LONGEDGES MAXTABLE=$MAXTABLE"
    > FAIL=0
    > [ "$LINES" -gt 600 ] && { echo "MAJOR\treadability\tdoc lines $LINES > 600 (Rule 42)"; FAIL=1; }
    > [ "$PIPES" -gt 100 ] && { echo "MAJOR\treadability\tpipe-row count $PIPES > 100 (Rule 42)"; FAIL=1; }
    > [ "$MAXTABLE" -gt 25 ] && { echo "MAJOR\treadability\tlargest contiguous table $MAXTABLE rows > 25 (Rule 42)"; FAIL=1; }
    > [ "$LONGEDGES" -gt 0 ] && { echo "MED\treadability\t$LONGEDGES edge labels > 80 chars (Rule 43)"; FAIL=1; }
    > [ "$FAIL" -eq 0 ] && echo "CLEAN (readability)"
    > ```
    > These limits encode user-validated screen-readability bounds. Any violation = budget breach (verifier failure), even when the content is technically complete. Compaction (Rule 42) and edge-label brevity (Rule 43) trump exhaustive enumeration (Rule 39 sig form is correct).
    > Output format: one line per failure — `SEVERITY<tab>WORKFLOW<tab>FINDING` or `CLEAN` if all pass.
13. **Fix all findings** from the subagent. If ≥3 findings, re-run the subagent after fixes. Repeat until CLEAN or ≤2 MINOR.

## Phase 4: Mermaid sanitization (MANDATORY post-process)
13a. Before emitting, run a final deterministic sanitization pass to strip lethal characters from every Mermaid edge label:
```bash
python3 - <<'PY'
import re
path = '.github/docs/state-machine.md'
src = open(path).read()
def sanitize_block(match):
    out = []
    for line in match.group(0).split('\n'):
        m = re.match(r'^(\s*(?:\[\*\]|\S+) --> \S+ : )(.*)$', line)
        if m:
            label = m.group(2).replace(';', ',')
            out.append(m.group(1) + label)
        else:
            out.append(line)
    return '\n'.join(out)
out = re.sub(r'```mermaid\n[\s\S]*?```', sanitize_block, src)
open(path, 'w').write(out)
PY
```
This is belt-and-suspenders: even if the model emits `;` somewhere, this step rewrites it. It MUST run before Phase 5 emits the final file. Re-run check (p) after sanitization to confirm zero parse failures.

## Phase 5: Emit
14. Write final `.github/docs/state-machine.md` (overwriting the draft from Phase 3.5).
15. Open PR via `create-pull-request`.

</process>

<diagram-guidelines>
- `stateDiagram-v2`, `direction LR`.
- Composite states for sub-types. **Composite entry rule:** if an external edge targets a composite, add `[*] --> FirstInnerState` inside the composite so Mermaid knows which inner state to enter. **Composite exit rule:** the inner `[*]` only terminates the nested substate — if the composite itself needs to continue to a downstream state, add an EXPLICIT outer transition `CompositeState --> NextState`. Missing outer exit = dangling composite = ERROR. Cross-composite transitions outside.
- `<<choice>>` ONLY after Q1/Q2/Q3. Verify you haven't UNDER-used it. `<<fork>>`/`<<join>>` when guards overlap (Rule 6).
- **`<<choice>>` is DECLARATION ONLY.** Never `A --> B <<choice>>`. Always `state B <<choice>>` on its own line.
- **Unique state IDs.** Each ID declared once per diagram (not both simple and composite).
- Actor prefixes on EVERY edge (Rule 28): 👤 human, 🤖 agent-name, ⚙️ workflow, ⏰ cron. **ZERO EXCEPTIONS.** Every `-->` line MUST have one of these emoji prefixes in its label. `[*] --> State` entries MUST have a prefix indicating what triggers it (👤 for manual/PR/issue events, ⏰ for cron, ⚙️ for push/workflow_call). **Post-draft MANDATORY fix:** scan every `-->` line in every diagram. If ANY edge lacks a prefix → add it before emitting. This is the #1 most common error.
- Every conditional: BOTH branches with guard scope.
- **Terminal states → `[*]`.** Post-draft: verify every leaf state has `→ [*]`. Missing terminal exits = ERROR. Scan after drawing: every state that has no outgoing edge MUST have `--> [*]`. **NEVER use custom sink states** (like `Done`, `End`, `Finished`, `*_End`). The ONLY valid terminal is `[*]`. Custom sinks = ERROR.
- **One entry arrow per filter value.** `branches: [main, release/*]` = TWO arrows.
- Cross-workflow dispatches: handover annotation, not inline transition.
- **Edge-label safety (Rule 41):** NEVER `;` in labels — use `,`. Avoid `<` `>` `&` outside backticks — use `≤` `≥` `and`. Match all `(`/`)`, `"`/`"`, `[`/`]`. NEVER `\`.
</diagram-guidelines>
