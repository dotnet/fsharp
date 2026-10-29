# F# Apex integration tests on DartLab-1ES

This pipeline installs Visual Studio from `DD-CB-ReleaseVS` on a DartLab-1ES
CloudTest machine, builds and deploys the PR's F# VSIX, then runs the Apex tests in Release.
It avoids the dependency skew of older preinstalled CI images. The chosen VS drop
must still support the PR's package pins; a recent drop is not a compatibility guarantee.

Before checking out source, the bootstrap uses `DartLab.Modules.Git`'s `Install-Git`
to copy the Git distribution bundled with the selected DartLab Production package
to `C:\Test\Git`. No separate installer or latest-version download is used.
Bootstrap and `run-apex.ps1` each prepend `C:\Test\Git\cmd` to their process PATH;
the Apex build and test children inherit it. CloudTest operations run in independent
processes, so setup does not rely on a previous operation's PATH changes.
Provisioning failures, an unusable Git executable, or a missing installation stop
the run rather than falling back to an unverified machine installation.

The runner also sets process-local `AGENT_WORKFOLDER` to the repository root.
CloudTest supplies `BUILD_BUILDID` without an Azure Pipelines agent workspace;
MicroBuild's `AddSBOM` task needs that workspace path when packaging VS manifests.
The build children inherit the value; the caller's environment is unchanged.

After setup/build execution, the runner copies the contents of
`artifacts\TestResults\<Configuration>` into CloudTest's required `LoggingDirectory`,
preserving attachment subdirectories. Handoff also runs on failure without replacing
the original build error; copy errors are reported separately. A successful build
without TRX results fails explicitly. No placeholder results are generated.

## Requesting a run

Post **`/dart <sha>`** or **`/pr-val <sha>`** as the entire comment on an open
`dotnet/fsharp` PR targeting `main`. Replace `<sha>` with the full 40-character
lowercase SHA of a commit listed in the PR after reviewing that revision, including
its build scripts and tests. Older listed commits are supported;
short, unlisted or force-pushed-away SHAs are rejected. GitHub exposes at most 250
PR commits, so a revision absent from that list cannot be authorized by this flow.
Both commands use the same protected-main orchestration for same-repository and
fork PRs. Fork code execution requires lab approval as described below.
The commenter must have repository write, maintain or admin access **and** be a
member of the `microsoft` GitHub organization. Supplying the SHA is the requester's
explicit authorization of that revision, not an automatic check of GitHub review status.

The [GitHub workflow](../../../.github/workflows/pr-validation.yml) validates the
requested commit and queues Azure DevOps over OIDC. It posts a run link and the
reviewed revision plus the separately pinned main orchestration revision to the PR.
New pushes, PR events and VS builds do **not** request runs.
Administrators can still queue runs manually, subject to pipeline permissions.

The [pipeline](../../../azure-pipelines-integration-dartlab.yml) is registered
directly against GitHub `dotnet/fsharp`; its `self` is **not** the dnceng mirror.
The Actions workflow and authorization helper run from the default branch.
The workflow resolves `refs/heads/main` once and pins pipeline YAML and local
templates to that immutable commit. This is independent of the reviewed PR SHA:
main advancing after queueing neither changes nor invalidates the orchestration.
The pipeline source is not a fork PR ref.

After VM provisioning, bootstrap fetches the pinned main commit into
`C:\Test\FSharp` and verifies its identity. It preserves main's
`setup-pr-validation.ps1` and `run-apex.ps1` in `C:\Test\FSharpControl`, outside
the PR checkout. The preserved verifier fetches and verifies the reviewed PR
commit and checks that the PR is still open and targeting `main`. The preserved
runner builds the selected source via an explicit `-SourceRoot C:\Test\FSharp`;
PR build scripts, hive setup, product and tests still execute from that workspace.
PR copies of the verifier and CloudTest runner are not invoked.

PR changes to pipeline YAML, provisioning or those control scripts are not tested
by ordinary `/dart` commands. They must land on protected main before this flow
uses them; experimental orchestration needs a separately approved validation run.

If the requested commit is still the current PR head, setup fetches GitHub's
generated PR merge and verifies that its second parent is the reviewed commit.
The merge's base parent is logged, not compared with a captured or live `main`
revision. Advancing `main` does not invalidate the run. A missing or stale merge
while the requested commit is still current fails explicitly rather than silently
dropping integration coverage.

If the PR head has advanced, setup verifies the requested commit is still listed
in the PR and tests that exact commit directly, without requiring a merge ref.
This also handles a push during merge fetching: no merge containing a newer,
unreviewed head is executed. API/Git failures and invalid PR membership remain
errors. A push after selection does not alter the detached commit being tested.

Immediately before checkout in either mode, setup refreshes the PR metadata and
requires the PR to remain open and target `dotnet/fsharp`'s `main`. Observed
retargeting or closure during selection is rejected without reintroducing a
base-SHA freshness check. GitHub's API and Git refs are not an atomic snapshot;
this recheck cannot prove that a fetched merge matches a concurrently changing
target when GitHub serves stale observations.

Setup logs record `mode=merge` or `mode=commit`, the actual tested SHA
(`FSharp.TestedCommitSha`), and the mode (`FSharp.TestedCommitMode`). Azure DevOps
`Build.SourceVersion` identifies the main orchestration revision, not the reviewed
or tested source revision. Reproduction requires that orchestration SHA, the
reviewed SHA, actual tested SHA/mode and selected VS/DartLab resource versions.
Results apply to that recorded snapshot,
not automatically to newer PR revisions. Direct-commit mode does not validate
integration with newer `main`; normal PR CI provides that coverage.

For manual runs, set `prNumber` and `headSha`; the latter means the requested
reviewed commit, even if it is older than the current PR head. Select pipeline
source `main` (`refs/heads/main`) and pin Commit to the main orchestration SHA,
not the PR SHA. There is no `baseSha` input and no equality requirement between
orchestration and tested source. Historical PR revisions need not contain current
pipeline/control scripts, but must support the expected product build/test entry
points and installed VS. Incompatibility fails explicitly rather than falling
back to a PR-provided verifier or runner.

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
5. Microsoft-org membership lookup uses the repository **`GITHUB_TOKEN`**, matching
   Roslyn's comment-command flow. No separate GitHub App or membership secrets are
   required. Private memberships may not be visible to this token, so a legitimate
   member with hidden membership may be rejected. Verify lookup for the intended
   requesters before rollout. Lookup errors fail closed; they do not prove nonmembership.
6. Have the lab owners approve execution of fork PR code and verify effective
   credential/network isolation. This is a protected-main orchestration build
   that later fetches reviewed fork code: **do not assume automatic fork-build
   secret restrictions apply**. Trusted YAML is not a sandbox. Build scripts and
   tests run on the VM with its effective managed identity and network access;
   copied control files prevent replacement by checkout, not deliberate writes
   by code executing on the same machine.
   Review job tokens, feed credentials, service connections and internal
   network access. Git fetches use the public repository, and the Actions OIDC and
   GitHub tokens are not sent to the test machine, but these measures alone do
   not sandbox a build. Do not enable this pipeline if lab policy cannot support it.

The Azure Pipelines GitHub source connection and the Entra queue identity are
separate; neither replaces the other or the Actions `GITHUB_TOKEN`.
Protect `main`, restrict who can edit/queue this privileged pipeline,
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
powershell -NoProfile -ExecutionPolicy Bypass -File eng\tests\SetupPrValidation.Tests.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File eng\tests\ApexResults.Tests.ps1
```

The script tests use temporary local Git repositories and a mocked GitHub PR response.
They cover merge identity, advancing main, current/older reviewed commits, head
races, closed PRs and failure before checkout.
Result handoff tests cover copied TRX/attachments, missing results, and preserving
the primary setup/build error when copying also fails.

After provisioning, use the Runs API's `previewRun` with the same repository
ref/version, reviewed-commit parameters, `DD-CB-ReleaseVS` resource pin and confirmed
`testMachineLab` to verify actual private-template expansion. Fix every template,
parameter, expression, service-connection and operation-expansion error before
allocating a CloudTest machine. Verify GitHub `self.version` pinning against the
resolved main SHA for same-repository and fork PRs, and the test-machine source
bootstrap; local YAML parsing cannot establish either. Land the Actions
workflow, templates and control scripts on main before activation, and configure the
environment and real pipeline ID together.

Explicitly request same-repository and fork PR smoke runs. Confirm the exact merge
SHA and mode, installed VS, VSIX load, Azure DevOps test counts, Apex results and
CloudTest TRX/build/setup log artifacts. Exercise an advancing main, a newer PR
push and an unauthorized request that queues no run. Exercise missing, short and
unlisted command SHAs. Verify YAML/local templates and copied control scripts come
from pinned main, while product code comes from the reviewed PR commit or its
verified merge. Confirm the main-sourced run has no fork-source classification
and necessary service endpoints are available; consult VSEng if policy still
blocks them rather than broadening fork permissions. Verify no UI overrides,
schedules or resource triggers introduce automatic runs. Completion is not reported
back as a new required GitHub check; the PR comment links to DevDiv results.

Source-connection precedent: [NuGet/NuGet.Client#5936][nuget] introduced a self-contained
VS-test pipeline for manual PR-branch queueing. F# retains its VS-drop selection and
Apex build path while using DartLab-1ES CloudTest operations rather than legacy DTL
agent step lists.

[github]: https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/github?view=azure-devops
[runs]: https://learn.microsoft.com/en-us/rest/api/azure/devops/pipelines/runs/run-pipeline?view=azure-devops-rest-7.1
[nuget]: https://github.com/NuGet/NuGet.Client/pull/5936
