---
name: "Build Failure Analysis"
description: >-
  When the Azure Pipelines PR build (`fsharp-ci`) fails, downloads binary logs
  produced by its failed or canceled jobs without rebuilding and asks the
  read-only build-failure analyst to post a root-cause summary and high-
  confidence inline suggestions on the validated PR revision.

on:
  check_run:
    types: [completed]
  roles: all
  workflow_dispatch:
    inputs:
      ado-build-id:
        description: "Azure DevOps build id to analyze (dnceng-public/public)."
        required: true
        type: string
      pr-number:
        description: "PR number to receive the analysis."
        required: true
        type: string
  needs: [fetch-binlog]

if: needs.fetch-binlog.outputs.binlog-found == 'true'

permissions:
  contents: read
  pull-requests: read

# Automatic and manual analyses for one PR supersede older automatic work.
# Unrelated check runs use their run id and remain cheap no-ops.
concurrency:
  group: ${{ (github.event_name == 'check_run' && github.event.check_run.name == 'fsharp-ci' && format('build-failure-analysis-{0}', github.event.check_run.pull_requests[0].number || github.event.check_run.head_sha)) || (github.event_name == 'workflow_dispatch' && github.ref == format('refs/heads/{0}', github.event.repository.default_branch) && format('build-failure-analysis-{0}', inputs['pr-number'])) || format('build-failure-analysis-run-{0}', github.run_id) }}
  cancel-in-progress: true
  job-discriminator: ${{ github.run_id }}

timeout-minutes: 30

imports:
  - shared/build-failure-analysis-shared.md

engine: copilot

jobs:
  fetch-binlog:
    name: Fetch binlogs (Azure Pipelines)
    runs-on: ubuntu-latest
    timeout-minutes: 15
    if: >-
      github.event.repository.fork == false &&
      (github.event_name == 'workflow_dispatch' ||
       (github.event_name == 'check_run' &&
        github.event.check_run.name == 'fsharp-ci' &&
        github.event.check_run.conclusion == 'failure'))
    permissions:
      contents: read
      pull-requests: read
    outputs:
      binlog-found: ${{ steps.upload.outcome == 'success' && steps.upload.outputs.artifact-id != '' }}
      pr-number: ${{ steps.fetch.outputs.pr-number }}
      pr-head-sha: ${{ steps.fetch.outputs.pr-head-sha }}
      pr-merge-sha: ${{ steps.fetch.outputs.pr-merge-sha }}
      ado-build-id: ${{ steps.fetch.outputs.ado-build-id }}
      ado-build-url: ${{ steps.fetch.outputs.ado-build-url }}
    steps:
      - name: Require the default branch for manual dispatch
        if: github.event_name == 'workflow_dispatch'
        shell: bash
        env:
          DEFAULT_BRANCH: ${{ github.event.repository.default_branch }}
        run: |
          if [ "$GITHUB_REF" != "refs/heads/${DEFAULT_BRANCH}" ]; then
            echo "::error::Manual build-failure analysis must run from the repository default branch."
            exit 1
          fi

      # The script and analyst instructions always come from the trusted
      # default branch, never from the PR being analyzed.
      - name: Check out analysis scripts
        uses: actions/checkout@v7.0.1
        with:
          ref: refs/heads/${{ github.event.repository.default_branch }}
          sparse-checkout: |
            .github/agents
            .github/workflows/scripts
          persist-credentials: false

      - name: Download binlogs from the failed Azure Pipelines build
        id: fetch
        shell: bash
        continue-on-error: true
        env:
          GH_TOKEN: ${{ github.token }}
          GH_AW_REPO: ${{ github.repository }}
          RESOLVE_MODE: ${{ github.event_name == 'workflow_dispatch' && 'dispatch' || 'check_run' }}
          PR_NUMBER: ${{ github.event.check_run.pull_requests[0].number || inputs['pr-number'] }}
          CHECK_HEAD_SHA: ${{ github.event.check_run.head_sha }}
          CHECK_DETAILS_URL: ${{ github.event.check_run.details_url }}
          DISPATCH_BUILD_ID: ${{ inputs['ado-build-id'] }}
          BINLOG_DIR: /tmp/binlogs
          SCRIPT_DIR: ${{ github.workspace }}/.github/workflows/scripts
        # Run outside the checkout so repository build customization cannot
        # affect the trusted file-based C# fetcher.
        run: cd "${RUNNER_TEMP:-/tmp}" && timeout 600 dotnet run --file "${SCRIPT_DIR}/fetch-build-binlogs.cs" -p:ImportDirectoryBuildProps=false -p:ImportDirectoryBuildTargets=false -p:ImportDirectoryPackagesProps=false

      - name: Report an incomplete fetch
        if: steps.fetch.outcome != 'success'
        run: echo "::warning::Binlog fetch did not complete (${{ steps.fetch.outcome }}); skipping analysis for this build."

      - name: Upload analysis artifact
        id: upload
        if: steps.fetch.outputs.binlog-found == 'true'
        uses: actions/upload-artifact@v7.0.1
        with:
          name: build-failure-analysis-data
          path: /tmp/binlogs
          if-no-files-found: error
          retention-days: 1

safe-outputs:
  needs: [fetch-binlog]
  messages:
    footer: "> **Automated content by GitHub Copilot.** Generated by the [{workflow_name}]({agentic_workflow_url}) workflow.{ai_credits_suffix} | [History]({history_link})"
  data:
    type: object
    properties:
      workflow_artifact:
        type: string
        enum: [build-failure-analysis]
      artifact_kind:
        type: string
        enum: [analysis]
    required: [workflow_artifact, artifact_kind]
    additionalProperties: false
  report-failure-as-issue: false
  add-comment:
    max: 1
    target: ${{ needs.fetch-binlog.outputs.pr-number }}
    hide-older-comments: true
  create-pull-request-review-comment:
    max: 25
    target: ${{ needs.fetch-binlog.outputs.pr-number }}
    commit-id: ${{ needs.fetch-binlog.outputs.pr-head-sha }}
  noop:
    max: 1
    report-as-issue: false
---

<!-- Body provided by shared/build-failure-analysis-shared.md. -->
