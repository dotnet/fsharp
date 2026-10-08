# F# Apex integration tests on DartLab-1ES

This pipeline installs Visual Studio from `DD-CB-ReleaseVS` on a DartLab-1ES
CloudTest machine, builds and deploys the PR's F# VSIX, then runs the Apex tests in Release.
It avoids the dependency skew of older preinstalled CI images. The chosen VS drop
must still support the PR's package pins; a recent drop is not a compatibility guarantee.

## Requesting a run

Post **`/dart <sha>`** or **`/pr-val <sha>`** as the entire comment on an open
`dotnet/fsharp` PR targeting `main`. Replace `<sha>` with the full 40-character
lowercase SHA of the current PR head after reviewing that revision, including its
pipeline YAML, templates and scripts. Short, stale or non-head SHAs are rejected.
Both commands run the same pipeline. Fork PRs are supported.
The commenter must have repository write, maintain or admin access **and** be a
member of the `microsoft` GitHub organization. Supplying the SHA is the requester's
explicit authorization of that revision, not an automatic check of GitHub review status.

The [GitHub workflow](../../../.github/workflows/pr-validation.yml) captures the PR
head and base SHAs and queues Azure DevOps over OIDC. It posts a run link and those
revisions to the PR. New pushes, PR events and VS builds do **not** request runs.
Administrators can still queue runs manually, subject to pipeline permissions.

The [pipeline](../../../azure-pipelines-integration-dartlab.yml) is registered
directly against GitHub `dotnet/fsharp`; its `self` is **not** the dnceng mirror.
The Actions workflow and authorization helper run from the default branch.
The workflow pins pipeline YAML and local templates to `refs/pull/<number>/head`
at the explicitly requested head SHA, including for fork PRs. On the test machine,
[`setup-pr-validation.ps1`](../../setup-pr-validation.ps1) fetches the public PR
merge ref from a checkout of that reviewed head, verifies both parents against
the captured head/base SHAs, checks the PR is still open and observes current
head/base refs before checking out that exact merge.

A conflict, missing/stale merge, changed head/base, or API/Git failure stops the run
before building PR code in the provided pipeline. Review the updated head and
post a new command with its SHA after resolving the problem. A push
after verification does not alter the detached commit being tested. The setup and
test logs record the actual merge SHA; Azure DevOps `Build.SourceVersion` identifies
the reviewed PR head/YAML revision, **not** the tested PR merge. Results apply only
to the recorded snapshot, not automatically to later PR revisions.

## Registration and authorization

Provision these before enabling requests:

1. In **devdiv/DevDiv -> Pipelines -> New pipeline -> GitHub**, select `dotnet/fsharp`
   and the existing `azure-pipelines-integration-dartlab.yml`, default branch `main`.
   Use an approved Azure Pipelines GitHub App connection restricted to the necessary
   repository; authorize this pipeline specifically. No `dnceng-internal-code-access`
   connection is needed. Leave the existing mirror and dnceng pipelines unchanged.
2. Authorize `DevDiv/DartLab`, `DevDiv/VS.Templates`, the `DD-CB-ReleaseVS`
   pipeline/artifacts, required feeds, and the `DartLab-1ES` service connection.
   The stage uses the production CloudTest templates from `DevDiv/DartLab`; it has
   no dependency on the legacy `DevDiv/DartLab.Templates` DTL repository.
3. The DartLab team assigned the F# pipeline to the
   **`DotNet-Project-System-AMD64`** CloudTest lab. The owner notification alias
   is `fsharp`. Confirm the VS drop and installed components support F#.
4. Create the GitHub **`fsharp_pr_validation`** environment. Set its variable
   **`FSHARP_APEX_PIPELINE_ID`** to the new DevDiv pipeline's numeric ID.
   Set **`AZURE_CLIENT_ID`** and **`AZURE_TENANT_ID`** secrets for an Entra identity
   federated to `repo:dotnet/fsharp:environment:fsharp_pr_validation`.
   Grant it permission to queue this pipeline, not edit pipeline definitions.
   This flow does not require an Azure subscription.
5. Provide **`MICROSOFT_MEMBERS_APP_ID`** and **`MICROSOFT_MEMBERS_APP_PRIVATE_KEY`**
   environment secrets for an approved GitHub App installed in `microsoft`, with
   organization **Members: read** permission. The workflow requests a short-lived
   membership-only token. The dotnet/fsharp `GITHUB_TOKEN` alone cannot be assumed
   to see private memberships in another organization. Verify lookup with a private
   member before rollout. Lookup errors fail closed; they do not prove nonmembership.
6. Have the lab owners approve execution of fork PR code and verify effective
   credential/network isolation. This is a manually queued build that executes
   reviewed PR pipeline YAML: **do not assume automatic fork-build secret
   restrictions apply**. PR YAML can change steps and resource usage before the
   provided source-verification script runs, so the SHA gate is not a sandbox.
   Review job tokens, feed credentials, service connections and internal
   network access. Git fetches use the public repository, and the Actions OIDC and
   membership tokens are not sent to the test machine, but these measures alone do
   not sandbox a build. Do not enable this pipeline if lab policy cannot support it.

These are three separate identities: the Azure Pipelines GitHub source connection,
the Entra queue identity, and the Microsoft-org membership lookup App. None replaces
the others. Protect `main`, restrict who can edit/queue this privileged pipeline,
keep `testMachineHoldStrategy: None` as the default, and authorize no
signing/publishing credentials. Use `Failure` or `Completion` only for a bounded
investigation; CloudTest held machines are accessed through the generated Bastion link.

Microsoft supports [GitHub pipelines in multiple Azure DevOps organizations][github].
Only the first organization receives automatic GitHub push/PR triggers; manual queueing
remains supported in secondary organizations. This workflow uses the [Runs REST API][runs],
so the restriction does not require moving existing dnceng CI or enabling PR triggers.

## Validation and activation

Local regression commands (no product build or DartLab access required):

```powershell
pwsh -NoProfile -File .github\scripts\pr-validation.Tests.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File eng\tests\SetupPrValidation.Tests.ps1
```

The script tests use temporary local Git repositories and a mocked GitHub PR response.
They cover merge identity, ref races, closed/changed PRs and failure before checkout.
The PowerShell 7 tests run the Actions helper offline with a fake HTTP boundary.
They cover command syntax, authorization of the original commenter (also on reruns),
private membership lookup with the separate App token, the queued snapshot and PR feedback.

After provisioning, use the Runs API's `previewRun` with the same repository
ref/version, snapshot parameters, `DD-CB-ReleaseVS` resource pin and confirmed
`testMachineLab` to verify actual private-template expansion. Fix every template,
parameter, expression, service-connection and operation-expansion error before
allocating a CloudTest machine. Verify GitHub `self.version` pinning against the
requested head SHA for same-repository and fork PRs, and the test-machine source
bootstrap; local YAML parsing cannot establish either. Land the Actions
workflow/authorization helper on main before activation, and configure the
environment and real pipeline ID together.

Explicitly request same-repository and fork PR smoke runs. Confirm the exact merge
SHA, installed VS, VSIX load, Azure DevOps test counts, Apex results and CloudTest
TRX/build/setup log artifacts. Exercise changed head/base rejection and an
unauthorized request that queues no run. Exercise missing, short, mismatched and
stale command SHAs. Verify the compiled YAML/local templates come from the
explicitly requested PR head rather than main or a newer push, and no UI overrides,
schedules or resource triggers introduce automatic runs. Completion is not reported
back as a new required GitHub check; the PR comment links to DevDiv results.

Source-connection precedent: [NuGet/NuGet.Client#5936][nuget] introduced a self-contained
VS-test pipeline for manual PR-branch queueing. F# retains its VS-drop selection and
Apex build path while using DartLab-1ES CloudTest operations rather than legacy DTL
agent step lists.

[github]: https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/github?view=azure-devops
[runs]: https://learn.microsoft.com/en-us/rest/api/azure/devops/pipelines/runs/run-pipeline?view=azure-devops-rest-7.1
[nuget]: https://github.com/NuGet/NuGet.Client/pull/5936
