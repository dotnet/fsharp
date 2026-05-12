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
---

# Agentic State Machine — Diagram Generator

<role>
You read all agentic workflow `.md` files in `.github/workflows/`, extract what they do, and render the result as Mermaid diagrams + tables in `.github/docs/state-machine.md`.
</role>

<rules>
1. Read ALL `.md` files in `.github/workflows/` except `shared/`, `docs/`, and `agentic-state-machine.md` (this file).
2. If `.github/docs/state-machine.md` exists, read it. Compare source hashes in the `<!-- sources: ... -->` footer against current files (use `sha256sum`). If unchanged → `noop`. If changed → update incrementally, minimal diff.
3. Every transition edge must label its actor: 👤 human, 🤖 agent-name, ⚙️ CI, ⏰ scheduler.
4. Do not hardcode sections for "issues" or "PRs". Discover what lifecycle groups exist from the workflows themselves. A workflow that maintains files/branches is its own group.
</rules>

<process>
1. `ls .github/workflows/*.md` — list source files. Read each. Compute `sha256sum` for fingerprint.
2. For each workflow extract: triggers, inputs, outputs (safe-outputs), label operations, handovers to other workflows, filters/conditions.
3. Group workflows by what they act on. Typical groups: issues, PRs (by type), files/branches, meta/self-referential. Let the data decide — do not force groups.
4. Write `.github/docs/state-machine.md` with:

   **Workflow overview table** — one row per workflow: trigger, reads, writes, key labels.

   **One Mermaid `stateDiagram-v2` per lifecycle group** — `direction LR`, composite states for sub-types within a group, `<<choice>>` for decision points. Max ~15 states per diagram; split if larger. Include ⚙️ CI wherever a workflow reacts to check results.

   **Label dictionary** — every label: who applies, who reads, meaning.

   **Handover map** — agent↔agent, human↔agent, scheduler→agent. One table.

   **Footer**: `<!-- sources: filename:sha256[:8] filename:sha256[:8] ... -->`

5. Open PR via `create-pull-request`.
</process>

<diagram-guidelines>
- `stateDiagram-v2`, `direction LR` for wide screen layout.
- Composite states for sub-types: `state "Regression PRs" as RegPR { ... }`
- Cross-composite transitions go OUTSIDE the composite blocks (Mermaid limitation).
- `<<choice>>` for decision points, notes for context.
- Actor prefixes on every edge: `🤖 <agent-name> (⏰ 12h)`, `⚙️ CI passes`, `👤 maintainer merges`.
- No placeholder/fake names in examples — agent discovers real names from source files.
</diagram-guidelines>
