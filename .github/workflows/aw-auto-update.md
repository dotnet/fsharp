---
description: |
  Keeps agentic workflows up to date by running `gh aw upgrade` and `gh aw compile` daily.
  If changes are detected, pushes them to a long-lived branch and creates or updates a PR.

on:
  schedule: every 24h
  workflow_dispatch:

timeout-minutes: 15

permissions: read-all

network:
  allowed:
  - defaults

checkout:
  ref: main

tools:
  github:
    toolsets: [pull_requests]
    min-integrity: none
  bash: true

safe-outputs:
  create-pull-request:
    draft: false
    title-prefix: "[Auto Update] "
    labels: [automation]
    max: 1
  push-to-pull-request-branch:
    target: "*"
    title-prefix: "[Auto Update] "
    labels: [automation]
    max: 1
---

# Agentic Workflow Auto-Update

You are a maintenance bot that keeps the repository's agentic workflow infrastructure current.

## Task

1. **Upgrade**: Run `gh aw upgrade` to update the gh-aw CLI version and apply any codemods.
2. **Compile**: Run `gh aw compile` to recompile all workflows and verify 0 errors.
3. **Check for changes**: Run `git diff` to see if anything changed.
4. **If no changes**: Report "Already up to date" and exit.
5. **If changes exist**:
   - Check if an open PR titled `[Auto Update] Agentic workflows` already exists (search open PRs).
   - If a PR exists, push the changes to its branch (`agentics/auto-update-gh-aw`) to update it. Leave a brief comment noting what changed (e.g. "Updated gh-aw-actions/setup from vX to vY").
   - If no PR exists, create a new PR from branch `agentics/auto-update-gh-aw` to `main` with title `[Auto Update] Agentic workflows` and a body summarizing the changes.

## Rules

- Only commit changes to files managed by `gh aw`: `.github/workflows/`, `.github/aw/`, `.github/agents/`.
- Use a single commit with message: `Update agentic workflows via gh aw upgrade`.
- The branch name must always be `agentics/auto-update-gh-aw`.
- If `gh aw compile` reports errors, do **not** create or update a PR. Instead, report the errors and exit.
- Be concise in PR descriptions and comments.
