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

## Prerequisites that must be provisioned before this can run (P0 — external)

These are **not** contained in this repo and require coordination with the DevDiv / DartLab / VS
team (as Roslyn did). The YAML here uses `# TODO(P0):` markers wherever a real value is required.

1. **DartLab + VS pipeline templates access** — the pipeline `extends`/references
   `DevDiv/DartLab.Templates` and `DevDiv/VS.Templates` (internal AzDO repos).
2. **Internal source mirror** — the `dotnet/fsharp` mirror already exists at
   [`dnceng/internal/dotnet-fsharp`](https://dev.azure.com/dnceng/internal/_git/dotnet-fsharp)
   (referenced as `internal/dotnet-fsharp` via the `dnceng-internal-code-access` service
   connection, the same endpoint Roslyn uses). Remaining: confirm that service connection is
   authorized for this pipeline once it is registered.
3. **VS-Platform test lab pool** + **1ES** onboarding for a new internal pipeline definition.
4. **F# DevDiv area path / owner** for `templateContext` (Roslyn uses
   `mlinfraswat` / `DevDiv\NET Developer Experience\CSharp and VB IDE`).
5. **VS-build-under-test source** — the pipeline now wires this the way Roslyn does: a
   `VisualStudioBuildUnderTest` pipeline resource (`source: DD-CB-ReleaseVS`, `trigger: true`) whose
   drop supplies the bootstrapper; `stage.yml` downloads its artifacts and computes the drop name
   with DartLab's `Get-VisualStudioDropName.ps1`, which the `(default)` `visualStudioBootstrapperURI`
   expands to. **Confirm with the DartLab/VS team whether `DD-CB-ReleaseVS` is the correct VS build
   for F# to gate on** (the VS build that consumes the F# insertion), or substitute the right one.
   *(Interim alternative: drop the resource + the two `preTestMachineConfigurationStepList` steps and
   hard-code `visualStudioBootstrapperURI` to a pinned `int.main` Products drop.)*

## Files

- `../../azure-pipelines-integration-dartlab.yml` — pipeline entry (`trigger: none`), extends the VS
  `build.yml` DartLab template and wires the DartLab/VS template repos, the internal F# mirror, and
  the `VisualStudioBuildUnderTest` pipeline resource.
- `stage.yml` — the DartLab VS test stage: provisions a `VS-Platform` machine, installs VS via the
  bootstrapper with an F#-minimal component set, then deploys + runs.
- `integration-job.yml` — deploy the F# VSIX + run the Apex tests via `eng/Build.ps1 -testApex`
  against the freshly installed VS; publish the TRX.

## Once matching-VS runs are green

The interim public-CI workarounds become unnecessary and should be retired (see plan P4):
- `eng/SetApexRoslynVersion.ps1` and the `FSHARP_APEX_ROSLYN_VERSION` override group in
  `eng/Versions.props`.
Keep `eng/SetupVSHive.ps1` (first-launch/registry prep) and the `/NoSigninPrompt` launch argument.

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
5. Confirm the VS build to gate on: the scaffold uses Roslyn's `DD-CB-ReleaseVS` as the
   `VisualStudioBuildUnderTest` pipeline resource. Verify that is the correct VS build for F# (the
   one that consumes the F# insertion) or provide the right pipeline name.

After that: fill in every `# TODO(P0):` marker, register the pipeline, and iterate to green. Then do
plan P4 (retire the version overrides) and P5 (disable the public `WindowsApexIntegration` job).

