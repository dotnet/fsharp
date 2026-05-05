# INTERNAL.md — release/dev15.9.x branch

> This branch revives the F# 15.9 product (VS 2017 servicing) for builds in modern dnceng AzDO. **Infrastructure-only branch — no product code changes**. Created from tag `Visual-Studio-2017-Version-15.9` (SHA `6e26c5bacc8c4201e962f5bdde0a177f82f88691`, 29 Oct 2018).

## Why this branch differs from `main`, `release/dev16.11`, etc.

This branch follows the **Roslyn `dev15.9.x` revival pattern** (NOT Arcade), modeled on `dotnet/roslyn` PR #83282 by @phil-allen-msft (merged 30 Apr 2026). Justification in `plan.md` (session workspace).

Key differences from the rest of `dotnet/fsharp`:
- **No Arcade SDK** — pre-Arcade era source; full Arcade adoption rejected by adversarial review (5/5 R1 + 3/3 R3 unanimous).
- **No `eng/common/`**, no `eng/Versions.props`, no `eng/Version.Details.xml`, no darc/Maestro.
- **Dual-SDK acquisition** — `init-tools.cmd` keeps installing legacy CLIs (1.0.0-preview3-003886 + 2.1.300-rtm-008707) because 11 `project.json` files at the tag (incl. `src/fsharp/FSharp.Core/project.json` for netstandard1.6) cannot be restored by SDK 2.1.526. Modern SDK 2.1.526 (per `global.json`) is for orchestration only.
- **Pipeline cloned from Roslyn 15.9.x def** — NOT F# pipeline 499 (Arcade-driven).
- **Pool / image**: `NetCore1ESPool-Svc-Internal` + `windows.vs2017.amd64`.
- **Insertion via Roslyn-style `Insertion.*` pipeline variables** (no helper scripts) — see `plan.md §11`.

## Verified facts about the source tree (do not re-discover)

### Versions
- `FSCoreVersion = 4.5.0.0` (assembly version) — from `build/targets/AssemblyVersions.props:22`
- `FSCorePackageVersion = 4.5.3` (the nupkg version we DO NOT publish per Constraint 4)
- `packages.config:22` references already-published `FSharp.Core 4.5.2` for bootstrap
- `RoslynPackageVersion.txt = 2.9.0-beta8-63208-01`
- `BuildToolsVersion.txt = 1.0.27-prerelease-01001-04`
- `DotnetCLIVersion.txt = 1.0.0-preview3-003886` (the .NET CLI itself)
- `DotnetCLIToolsVersion.txt = 2.1.300-rtm-008707` (the CLI Tools companion)

### Source projects in `src/fsharp/`
`FSharp.Build`, `FSharp.Build-proto`, `FSharp.Compiler.Interactive.Settings`, `FSharp.Compiler.Private`, `FSharp.Compiler.Server.Shared`, `FSharp.Core`, `Fsc`, `Fsc-proto`, `fsi`, `fsiAnyCpu`. (10 projects.)

### `vsintegration/` includes a single `.vbproj`
`vsintegration/src/FSharp.ProjectSystem.PropertyPages/FSharp.PropertiesPages.vbproj`. Three-language repo (F# + C# + VB).

### Locales (14)
`SetupLanguages` ItemGroup in `setup/FSharp.Setup.props` lists 14:
`CHS, CHT, CSY, DEU, ENU, ESN, FRA, ITA, JPN, KOR, PLK, PTB, RUS, TRK`.
Each has:
- `setup/resources/eula/VF_EULA.<LOCALE>.rtf` (verified — all 14 present)
- `src/fsharp/FSharp.Core/xlf/FSCore.<lang>.xlf` (14 locales: cs, de, en, es, fr, it, ja, ko, pl, pt-BR, ru, tr, zh-Hans, zh-Hant)
- `src/fsharp/FSharp.Build/xlf/FSBuild.txt.<lang>.xlf` (same 14)
- 490 XLF files total across all locales × components

### Insertion payload — full driver chain
The 6 swixprojs are built via TWO separate orchestrators (do not assume `setup/build-insertion.proj` builds all of them):

| Driver | Builds |
|---|---|
| `build-everything.proj` line 49 → `<SetupProjects Include="setup/fsharp-setup-build.proj" />` (when `BUILD_SETUP=1`) | `setup/fsharp-setup-build.proj` builds: `Microsoft.FSharp.{IDE, Dependencies, Vsix.Resources}.swixproj` + `VisualFSharpFull.csproj` + `VisualFSharpTemplates.csproj` |
| `build.cmd:819-820` → `setup/build-insertion.proj` (when `BUILD_SETUP=1`) | Builds `Microsoft.FSharp.{SDK, Compiler, Compiler.Resources}.swixproj` (Compiler.Resources per locale) |
| `build.cmd:833-834` → `setup/Swix/Microsoft.FSharp.vsmanproj` | Merges all 8 per-component `.json` manifests into final `Microsoft.FSharp.vsman`; copies `VisualFSharpFull.vsix` + `VisualFSharpTemplate.vsix` + 2 .json into `Release\insertion\` |

**Total ship surface in `Release\insertion\`**:
- 1 × `Microsoft.FSharp.vsman` (merged manifest)
- 8 per-component `.json` manifests: `Microsoft.FSharp.{Compiler, Compiler.Resources.<lang>, SDK, Vsix.Full.Core, Vsix.Full.Resources.<lang>, VSIX.Templates, Dependencies, IDE}.json` (lang-bearing × 14 locales)
- 6 swixproj VSIXes: `Microsoft.FSharp.{Compiler, Compiler.Resources, Dependencies, IDE, SDK, Vsix.Resources}.vsix`
- 2 vsintegration-Vsix VSIXes: `VisualFSharpFull.vsix` + `VisualFSharpTemplate.vsix`
- **NO MSI**. F# 15.9 swixprojs all set `<OutputType>vsix</OutputType>`.

### Critical build.cmd lines (microbuild mode)

| Line | Purpose |
|---|---|
| 82 | `set SIGN_TYPE=%PB_SIGNTYPE%` — pipeline must set `PB_SIGNTYPE` env var (NOT `SignType`) |
| 200/213/253/356 | `BUILD_SETUP=%FSC_BUILD_SETUP%` (default 1) |
| 205-220 | `microbuild` mode block — sets `BUILD_NUGET=1` and `BUILD_MICROBUILD=1` |
| 214 | **`set BUILD_NUGET=1`** — triggers nupkg production we MUST suppress (B10 patch) |
| 446-449 | `if /i "%PB_SKIPTESTS%" == "true"` — pipeline must set `PB_SKIPTESTS` (NOT `SkipTests`) |
| 506/509 | `echo BUILD_NUGET=%BUILD_NUGET%` / `BUILD_MICROBUILD=` (diagnostic) |
| 800-803 | First sign call: `AssemblySignToolData.json` |
| 808-809 | **`build-nuget-packages.proj`** invocation — produces `FSharp.Core.nupkg` + `Microsoft.FSharp.Compiler.nupkg`; B10 patch suppresses |
| 812-815 | Second sign call: `PackageSignToolData.json` (signs the nupkgs we no longer produce) |
| 819-820 | `setup/build-insertion.proj` — builds 3 swixprojs (SDK, Compiler, Compiler.Resources) |
| 824-827 | Third sign call: `InsertionSignToolData.json` |
| 832-835 | `setup/Swix/Microsoft.FSharp.vsmanproj` — final merge |

### Cert names in sign tool data
- `build/config/AssemblySignToolData.json:4-6` — `"certificate": "Microsoft", "strongName": "StrongName"`. Used by `RoslynTools.SignTool 1.0.0-beta2-dev3` from `build/config/packages.config`.
- `build/config/InsertionSignToolData.json` — `"certificate": "VsixSHA2"`.
- `build/config/PackageSignToolData.json` — `"certificate": "NuGet"`. Signs `artifacts\*.nupkg`. **Will not run** after B10 patch removes nupkg production.

**Open: cert mapping** — modern ESRP renamed `Microsoft` → `Microsoft400`. Verify `RoslynTools.SignTool 1.0.0-beta2-dev3` translates the literal `Microsoft` cert name to current ESRP via `signtool verify /pa /v` post-test-sign.

### `Experimental="true"` in VisualFSharpFull manifest
- `vsintegration/Vsix/VisualFSharpFull/Source.extension.vsixmanifest` has `<Installation Experimental="true">` at the tag.
- Strip target: **assumed handled by `MicroBuild.Plugins.SwixBuild`'s `Finalize*` target** when `<FinalizeManifest>true</FinalizeManifest>` is set in `vsmanproj`. NO explicit strip target found in the source tree.
- G4 must explicitly assert produced VSIX manifests do NOT contain `Experimental="true"`. If assertion fails, a strip step must be added to the `vsmanproj` finalize chain.

### NuGet feed mapping
Per-package audit saved to `Q:\source\fsharp\refs\fsharp-15.9-package-inventory.csv` (103 unique package id+version pairs). C1 to mark each as resolvable from the new feeds (`myget-legacy`, `dnceng/dotnet-eng`, `azure-public/vssdk`, etc.). All `dotnet.myget.org/F/*` feeds are dead and replaced by `myget-legacy` mirror.

## Decisions made (defaults, when not specified by user)

- **SDK pin**: `2.1.526` per `global.json` (matches Roslyn 15.9.x).
- **Pool/image**: `NetCore1ESPool-Svc-Internal` + `windows.vs2017.amd64` (matches Roslyn 15.9.x).
- **MicroBuildSigningPlugin**: `@4`. **MicroBuildSwixPlugin**: `@4`. **NuGetTool**: `6.2.4`.
- **Symbol publish**: `PublishSymbols@2 SymbolServerType: TeamServices` (internal AzDO; matches Roslyn). Add SymWeb later if VS partner debugging needs it.
- **SBOM**: `sdl.sbom.enabled: false` (matches Roslyn — VS-internal-only insertion).
- **Insertion**: Roslyn-style `Insertion.*` pipeline variables (NOT F# main's helper scripts).
- **Auto-completion of insertion PR**: explicitly `false` — DevDiv release management decides.
- **Localization**: vendor existing `setup/resources/` and `xlf/*.xlf` files; NO OneLocBuild for v1.

## Tickets / external dependencies (track here as filed)

| Item | Owner | Status | Notes |
|---|---|---|---|
| dnceng helpdesk: clone Roslyn 15.9.x pipeline def | (TBD) | pending | A2 |
| dnceng helpdesk: variable groups (`DotNet-FSharp-Insertion-Variables`, etc.) | (TBD) | pending | A4 |
| dnceng helpdesk: DevDiv push access for `dn-bot-devdiv-build-rw-code-rw-release-rw` | (TBD) | pending | A9 |
| ESRP/MicroBuild: cert mapping verification (`Microsoft` → `Microsoft400`?) | (TBD) | pending | B9 |
| 1ES helpdesk: `MicroBuildInsertVsPayload` task input contract | (TBD) | pending | B12 |
| Lab access: `\\cpvsbuild\drops\FSharp\Visual-Studio-2017-Version-15.9\<latest>\insertion` UNC | (TBD) | pending | F1 |

## Contact / escalation

- Branch initial author: T-Gro (`tomasgrosup@microsoft.com`)
- Plan: in session workspace `C:\Users\tomasgrosup\.copilot\session-state\05b7ee33-b7c7-4a2f-92db-469833aa3dd7\plan.md`
- Local refs: `Q:\source\fsharp\refs\` (Roslyn 15.9.x sparse clone, Arcade reference, RoslynPR #83282 patch)

## Status (running log — append, do not edit)

- 2026-05-05: Branch created from tag SHA. C1 feed audit baseline committed (103 unique packages). C5 verified (14 locales × all components). C11 swixproj driver chain documented (build-everything.proj → fsharp-setup-build.proj for IDE/Dependencies/Vsix.Resources; build-insertion.proj for SDK/Compiler/Compiler.Resources; vsmanproj for merge). global.json, eng/TSAConfig.gdntsa, eng/config/guardian/.gdnsuppress added.
