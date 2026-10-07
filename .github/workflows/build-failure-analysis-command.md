---
name: "Build Failure Analysis (command)"
run-name: "Build failure analysis command ${{ github.event.comment.id }}"
description: >-
  Reruns build-failure analysis when a maintainer comments
  `/analyze-build-failure` on a pull request. It analyzes only that PR's newest
  `fsharp-ci` build, and only when the newest build is completed and failed.

on:
  slash_command:
    name: analyze-build-failure
    events: [pull_request_comment]
  roles: [admin, maintainer, write]
  reaction: "eyes"
  needs: [fetch-binlog]

if: needs.fetch-binlog.outputs.binlog-found == 'true'

permissions:
  contents: read
  pull-requests: read

# Queue command analyses for one PR. A successful safe-output publication is
# deduplicated by the exact comment id below; a new comment requests a new run.
concurrency:
  group: ${{ (github.event_name == 'issue_comment' && !startsWith(github.event.comment.body, '/analyze-build-failure') && format('build-failure-analysis-cmd-run-{0}', github.run_id)) || format('build-failure-analysis-cmd-{0}', github.event.issue.number || github.event.pull_request.number || fromJSON(github.event.inputs.aw_context || github.event.client_payload.aw_context || '{}').item_number || github.run_id) }}
  cancel-in-progress: false
  queue: max

timeout-minutes: 30

imports:
  - shared/build-failure-analysis-shared.md

engine: copilot

jobs:
  fetch-binlog:
    name: Fetch binlogs (Azure Pipelines)
    if: >-
      github.event.repository.fork == false &&
      github.event.issue.pull_request &&
      contains(fromJSON('["OWNER","MEMBER","COLLABORATOR"]'), github.event.comment.author_association) &&
      startsWith(github.event.comment.body, '/analyze-build-failure')
    runs-on: ubuntu-latest
    timeout-minutes: 15
    permissions:
      actions: read
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
      # author_association cannot distinguish an organization member with read
      # access from a repository maintainer. Resolve the actual base permission
      # before downloading any artifacts and fail closed on API errors.
      - name: Verify the command and commenter permission
        id: permission
        shell: bash
        env:
          GH_TOKEN: ${{ github.token }}
          COMMENTER: ${{ github.event.comment.user.login }}
          COMMENT_BODY: ${{ github.event.comment.body }}
          COMMAND_NAME: "analyze-build-failure"
        run: |
          set +e
          case "${COMMENT_BODY}" in
            "/${COMMAND_NAME}" | "/${COMMAND_NAME} "* | "/${COMMAND_NAME}"$'\n'*) ;;
            *)
              echo "Comment does not match the slash-command activation predicate."
              echo "authorized=false" >> "$GITHUB_OUTPUT"
              exit 0
              ;;
          esac
          if [[ ! "${COMMENTER}" =~ ^[A-Za-z0-9-]+$ ]]; then
            echo "::warning::Commenter login is missing or malformed."
            echo "authorized=false" >> "$GITHUB_OUTPUT"
            exit 0
          fi
          response=$(gh api "repos/${GITHUB_REPOSITORY}/collaborators/${COMMENTER}/permission" 2>/dev/null)
          permission=$(printf '%s' "${response}" | jq -r '.permission // empty' 2>/dev/null)
          case "${permission}" in
            admin|write) authorized=true ;;
            *) authorized=false ;;
          esac
          if [ "${authorized}" = "true" ]; then
            echo "'${COMMENTER}' has '${permission}' access; proceeding."
          else
            echo "::warning::'${COMMENTER}' does not have write access (resolved permission '${permission:-none}')."
          fi
          echo "authorized=${authorized}" >> "$GITHUB_OUTPUT"

      - name: Check for completed command publication
        id: command
        if: steps.permission.outputs.authorized == 'true'
        uses: actions/github-script@v9.0.0
        env:
          WORKFLOW_FILE: build-failure-analysis-command.lock.yml
        with:
          script: |
            const comment = context.payload.comment;
            const title = `Build failure analysis command ${comment.id}`;
            for (let page = 1; ; page++) {
              const { data } = await github.rest.actions.listWorkflowRuns({
                ...context.repo,
                workflow_id: process.env.WORKFLOW_FILE,
                event: "issue_comment",
                status: "completed",
                created: `>=${comment.created_at}`,
                per_page: 100,
                page,
              });
              if (data.total_count > 1000) {
                throw new Error("Command history exceeds the GitHub search limit; post a new command.");
              }
              for (const run of data.workflow_runs) {
                if (run.display_title !== title) {
                  continue;
                }
                const jobs = await github.paginate(
                  github.rest.actions.listJobsForWorkflowRunAttempt,
                  {
                    ...context.repo,
                    run_id: run.id,
                    attempt_number: run.run_attempt,
                    per_page: 100,
                  });
                const agentCompleted = jobs.some(job =>
                  job.name === "agent" &&
                  job.conclusion === "success" &&
                  job.steps?.some(step =>
                    step.name === "Execute GitHub Copilot CLI" &&
                    step.conclusion === "success"));
                const outputsCompleted = jobs.some(job =>
                  job.name === "safe_outputs" &&
                  job.conclusion === "success" &&
                  job.steps?.some(step =>
                    step.name === "Process Safe Outputs" &&
                    step.conclusion === "success"));
                if (agentCompleted && outputsCompleted) {
                  core.notice("This command completed publication; post a new command to rerun.");
                  core.setOutput("completed", "true");
                  return;
                }
              }
              if (data.workflow_runs.length < 100 || page * 100 >= data.total_count) {
                break;
              }
            }
            core.setOutput("completed", "false");

      - name: Check out analysis scripts
        if: steps.command.outputs.completed == 'false'
        uses: actions/checkout@v7.0.1
        with:
          ref: refs/heads/${{ github.event.repository.default_branch }}
          sparse-checkout: |
            .github/agents
            .github/workflows/scripts
          persist-credentials: false

      - name: Download binlogs from the PR's latest failed Azure Pipelines build
        id: fetch
        if: steps.command.outputs.completed == 'false'
        shell: bash
        continue-on-error: true
        env:
          GH_TOKEN: ${{ github.token }}
          GH_AW_REPO: ${{ github.repository }}
          RESOLVE_MODE: latest
          PR_NUMBER: ${{ github.event.issue.number }}
          BINLOG_DIR: /tmp/binlogs
          SCRIPT_DIR: ${{ github.workspace }}/.github/workflows/scripts
        run: cd "${RUNNER_TEMP:-/tmp}" && timeout 600 dotnet run --file "${SCRIPT_DIR}/fetch-build-binlogs.cs" -p:ImportDirectoryBuildProps=false -p:ImportDirectoryBuildTargets=false -p:ImportDirectoryPackagesProps=false

      - name: Report an incomplete fetch
        if: steps.fetch.outcome == 'failure'
        run: echo "::warning::Binlog fetch did not complete; skipping analysis for this command."

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
    footer: "> **Automated content by GitHub Copilot.** Generated by the [{workflow_name}]({agentic_workflow_url}) workflow.{ai_credits_suffix} | [History]({history_link}) | [Request](${{ github.event.comment.html_url }})"
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
