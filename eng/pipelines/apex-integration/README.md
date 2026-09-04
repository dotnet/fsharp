# F# Apex VS integration tests — DartLab pipeline (scaffold)

This directory contains an **internal DevDiv DartLab** pipeline that runs the F# Apex VS
integration tests (`vsintegration/tests/FSharp.Editor.Apex.IntegrationTests`) against a
Visual Studio build that **matches** what this repo targets, by *installing* that VS on a
fresh test machine via the VS bootstrapper.

It is modeled directly on dotnet/roslyn's
[`azure-pipelines-integration-dartlab.yml`](https://github.com/dotnet/roslyn/blob/main/azure-pipelines-integration-dartlab.yml)
and its `eng/pipelines/test-gates/*` stage templates.

## Why this exists

The F# editor VSIX is pinned (Roslyn, and the dotnet/runtime `System.*` family) to the VS it
inserts into (F# `main` → VS `main`, ~18.10). Visual Studio has **no binding redirects for our
assemblies**, so a VSIX built against those pins only loads in a matching (or newer) VS. The
public PR CI scout image trails that VS (e.g. 18.7), and the mismatch cannot be worked around
by downgrading dependencies (the VS-SDK imposes a floor — e.g. `Microsoft.VisualStudio.RpcContracts`
requires `System.Composition >= 10.0.8`, so a downgrade fails restore with NU1605).

The industry-standard fix (used by both NuGet/NuGet.Client and dotnet/roslyn) is to **install a
matching VS via the bootstrapper on internal DartLab test machines**, not to use a pre-baked older
public image.

## How it is triggered

The pipeline does **not** run automatically — neither on F# GitHub pushes/PRs (`trigger: none`,
`pr: none`) nor on VS builds (the `VisualStudioBuildUnderTest` pipeline resource is `trigger: none`,
so it only *supplies* the matching VS drop, it does not start the pipeline).

Instead it is started **on demand from a PR comment**, mirroring dotnet/roslyn's `/dart` / `/pr-val`
flow, via [`.github/workflows/pr-validation.yml`](../../.github/workflows/pr-validation.yml):

- An F# team member comments **`/pr-val`** (or **`/dart`**) on a PR.
- The workflow authorizes the commenter (repo **write** access **and** `microsoft` org membership),
  then triggers the DevDiv pipeline over an OIDC-authenticated Azure DevOps REST call, passing
  `prNumber` / `sha` / `EnforceLatestCommit`.
- External-authored PRs must pass an explicit reviewed commit: `/pr-val <commit-hash>`.
- The stage checks out the internal mirror, runs `eng/setup-pr-validation.ps1` to fetch the PR's
  merge commit, then builds + deploys the VSIX + runs Apex against the installed VS.
- A comment with the pipeline-run link is posted back to the PR.

## Prerequisites that must be provisioned before this can run (P0 — external)

These are **not** contained in this repo and require coordination with the DevDiv / DartLab / VS
team (as Roslyn did). The YAML here uses `# TODO(P0):` markers wherever a real value is required.

1. **DartLab + VS pipeline templates access** — the pipeline `extends`/references `DevDiv/DartLab`,
   `DevDiv/DartLab.Templates` and `DevDiv/VS.Templates` (internal AzDO repos).
2. **Internal source mirror** — the `dotnet/fsharp` mirror already exists at
   [`dnceng/internal/dotnet-fsharp`](https://dev.azure.com/dnceng/internal/_git/dotnet-fsharp)
   (referenced as `internal/dotnet-fsharp` via the `dnceng-internal-code-access` service
   connection, the same endpoint Roslyn uses). Remaining: confirm that service connection is
   authorized for this pipeline once it is registered.
3. **VS-Platform test lab pool** + **1ES** onboarding for a new internal pipeline definition.
4. **F# DevDiv area path / owner** for `templateContext` (Roslyn uses
   `mlinfraswat` / `DevDiv\NET Developer Experience\CSharp and VB IDE`).
5. **VS-build-under-test source** — a `VisualStudioBuildUnderTest` pipeline resource
   (`source: DD-CB-ReleaseVS`, `trigger: none`) supplies the bootstrapper; `stage.yml` downloads its
   artifacts and computes the drop name with DartLab's `Get-VisualStudioDropName.ps1`, which the
   `(default)` `visualStudioBootstrapperURI` expands to. When the pipeline is started via `/pr-val`,
   the resource resolves to the latest such VS build. **Confirm with the DartLab/VS team whether
   `DD-CB-ReleaseVS` is the correct VS build for F#** (the VS that consumes the F# insertion), or
   substitute the right one. *(Interim alternative: drop the resource + the two
   `preTestMachineConfigurationStepList` steps and hard-code `visualStudioBootstrapperURI` to a
   pinned `int.main` Products drop.)*
6. **Comment-trigger wiring** — register `azure-pipelines-integration-dartlab.yml` as a DevDiv
   pipeline and set its ID in `.github/workflows/pr-validation.yml` (`FSHARP_APEX_PIPELINE_ID`);
   configure the `AZURE_CLIENT_ID` / `AZURE_TENANT_ID` / `AZURE_SUBSCRIPTION_ID` OIDC secrets and the
   `fsharp_pr_validation` GitHub environment (the OIDC identity must be allowed to queue the pipeline).

## Files

- `../../.github/workflows/pr-validation.yml` — GitHub Actions workflow: authorizes a `/pr-val`
  (or `/dart`) PR comment and triggers the DevDiv pipeline over OIDC.
- `../../azure-pipelines-integration-dartlab.yml` — pipeline entry (`trigger: none`), extends the VS
  `build.yml` DartLab template and wires the DartLab/VS template repos, the internal F# mirror, and
  the `VisualStudioBuildUnderTest` pipeline resource.
- `stage.yml` — the DartLab VS test stage: provisions a `VS-Platform` machine, installs VS via the
  bootstrapper with an F#-minimal component set, then deploys + runs.
- `integration-job.yml` — deploy the F# VSIX + run the Apex tests via `eng/Build.ps1 -testApex`
  against the freshly installed VS; publish the TRX.
- `../../eng/setup-pr-validation.ps1` — on the test machine, fetch + check out the PR's merge commit.

## Interim public-CI workarounds (removed)

The old public-scout Apex leg (`WindowsApexIntegration` in `azure-pipelines-PR.yml`),
`eng/SetApexRoslynVersion.ps1`, and the `FSHARP_APEX_ROSLYN_VERSION` override group in
`eng/Versions.props` have been removed: DartLab installs a matching VS via bootstrapper, so the
VSIX pins and the VS-under-test align by construction and no Roslyn version override is needed.
`eng/SetupVSHive.ps1` (first-launch/registry prep) and the `/NoSigninPrompt` launch argument are kept.

## Concrete request to send to the DartLab / VS test team

Onboarding is the same shape Roslyn used. Ask for / decide:

1. Read access for the F# pipeline's service identity to the internal AzDO repos
   `DevDiv/DartLab`, `DevDiv/DartLab.Templates`, and `DevDiv/VS.Templates`.
2. Confirm the `dnceng-internal-code-access` service connection (to the existing
   `dnceng/internal/dotnet-fsharp` mirror) is authorized for this pipeline. The mirror itself
   already exists — no new mirror needs to be created.
3. Use of the `VS-Platform` test-lab pool for the new pipeline, and 1ES registration of
   `azure-pipelines-integration-dartlab.yml` as an internal pipeline.
4. The F# `templateContext.owner` and `areaPath` to record against test results
   (fill into `eng/pipelines/apex-integration/stage.yml`).
5. Confirm the VS build that supplies the matching VS: the scaffold uses Roslyn's `DD-CB-ReleaseVS`
   as the `VisualStudioBuildUnderTest` pipeline resource (`trigger: none` — it does not gate on VS
   builds). Verify that is the correct VS build for F# (the one that consumes the F# insertion) or
   provide the right pipeline name.
6. For the `/pr-val` comment trigger: the DevDiv pipeline ID (to put in
   `.github/workflows/pr-validation.yml`), and an Entra (AAD) identity whose OIDC federation is
   trusted by the `fsharp_pr_validation` GitHub environment and permitted to queue that pipeline
   (populates the `AZURE_CLIENT_ID` / `AZURE_TENANT_ID` / `AZURE_SUBSCRIPTION_ID` secrets).

After that: fill in every `# TODO(P0):` marker, register the pipeline, and iterate to green.
(The old public `WindowsApexIntegration` leg and its Roslyn-version overrides have already been removed.)

