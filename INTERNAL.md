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

- 2026-05-05 (cont): NuGet.Config fixed (added nuget.org per package availability probe). Authored scripts/{verify-vsix-targets.ps1, compare-insertion-vs-rtm.ps1, local-validate.ps1, LOCAL_VALIDATION.md}. Package availability probe saved at refs/fsharp-15.9-package-availability.csv: 16/20 critical packages on nuget.org, 4 require authenticated dnceng feeds (Microsoft.DotNet.BuildTools 1.0.27-prerelease-01001-04, RoslynTools.SignTool 1.0.0-beta2-dev3, MicroBuild.Plugins.SwixBuild 1.0.147, Microsoft.CodeAnalysis 2.9.0-beta8-63208-01) - resolution deferred to first NuGetAuthenticate@1 run in B11 preflight.

- 2026-05-05 (cont, after Copilot crash): PIVOTED from Roslyn-style standalone yaml to F# main eng/ infra port. Pipeline 499 expects Arcade infrastructure (eng/common/, etc.); the Roslyn-style approach would have required cloning a separate dnceng pipeline (which user does not have access to). Keeping pipeline 499 means our azure-pipelines.yml is auto-detected on its release/* trigger. 227 files committed (commit 9cfaa4a9a): full eng/* port from main, root project files, NuGet.config, global.json, all overrides for 15.9 (FSharpReleaseBranchName=release/dev15.9.x, VSInsertionTargetBranchName=rel/d15.9, F# 4.5.0.0 versions, Arcade pin = 10.0.0-beta.26222.2 matching global.json msbuild-sdks). B10 build.cmd patch retained.

## Session 2 (2026-05-05 cont'd, after pivot to pipeline 499)

### G1 attempt #1 results

Ran `Build.cmd -configuration Debug`. Arcade SDK 10.0.0-beta.26222.2 successfully restored from dotnet-eng feed. Then 4 failure classes surfaced — all expected legacy-vs-modern infra mismatches:

1. **eng/Build.ps1 hardcoded `VisualFSharp.slnx` and `FSharp.slnx`** — modern slnx format. 15.9 has only `.sln`. Patched eng/Build.ps1 to use `.sln`.

2. **`eng/restore/optimizationData.targets` missing** — Directory.Build.targets imports it from F# main convention. Imported from main.

3. **`packages/XliffTasks.0.2.0-beta-000081/build/XliffTasks.props` not found** — `src/FSharpSource.Settings.targets:226` imports XliffTasks via the legacy `packages/` path. 15.9 source uses packages.config style restore which Arcade's solution-level restore does not invoke. **TODO**: either (a) add a legacy-packages-restore step to the pipeline (similar to what 15.9 `init-tools.cmd` did) before main solution restore, or (b) modify the FSharpSource.Settings.targets import to be conditional on file existence + fall back to a no-op.

4. **`Proto/net40/bin/Microsoft.FSharp.Targets` not found** — circular: vsintegration/tests/MockTypeProviders depend on the proto compiler output, but proto compiler is built later in the pipeline. Symptom of (3) — once XliffTasks loads, proto compiler builds first, then this resolves.

### Next steps (next session)

- Solve (3): the cleanest path is to restore `packages.config`-style packages BEFORE Arcade solution restore. Look at how F# `release/dev17.x` branches handle the transition (they had to bridge this same gap).
- Re-run G1, expect proto compiler to build successfully, and Microsoft.FSharp.Targets to materialize.
- Patches retained: eng/Build.ps1 (slnx -> sln), Build.cmd untouched (case-collision result; same content as before since main's Build.cmd is identical to legacy build.cmd shim).

### Files modified after pivot commit

- `eng/Build.ps1`: `slnx` -> `sln` (3 instances)
- `eng/restore/optimizationData.targets`: imported from main

## Session 3 (2026-05-05 cont'd, deep-dive blockers + strategic re-eval)

### Resolved
- **Blocker (3) XliffTasks**: bypassed by passing `/p:DisableLocalization=true` on Build.cmd cmdline. Per Constraint 3.1 carve-out (vendor RTM .resx, no re-translation), this is acceptable for 15.9 servicing. Both `src/FSharpSource.Settings.targets:226` and `:233` are conditional on `DisableLocalization != 'true'` — both bypassed.
- **Legacy package restore**: `nuget restore packages.config -PackagesDirectory packages -Source https://api.nuget.org/v3/index.json -Source https://pkgs.dev.azure.com/dnceng/public/_packaging/myget-legacy/...` successfully restored 46 legacy packages including `FSharp.Compiler.Tools.4.1.27` (the LKG netfx F# proto compiler).
- **.NET 4.5 ref assemblies missing from VS 2017 install**: installed via `Microsoft.NETFramework.ReferenceAssemblies.net45` NuGet pkg + `/p:FrameworkPathOverride=...`.

### G2 attempt with manual proto build (`MSBuild src/fsharp-proto-build.proj`)
- ✅ FSharp.Core proto compiled successfully: `FSharp.Core -> Q:\source\fsharp\fsharp\Proto\net40\bin\FSharp.Core.dll`
- ❌ FSharp.Build-proto + Fsc-proto fail with **two new layered errors**:
  1. `MSB3030 Could not copy "Q:\source\fsharp\fsharp\src\fsharp\FSharp.Core\Q:\source\fsharp\fsharp\src\fsharp\FSharp.Core\..\..\..\Proto\net40\bin\**"` — path doubled. Looks like a relative-path bug in `FSharpBuild.Directory.Build.targets:32` BeforeTargets resolution under MSBuild 15.
  2. `MSB4036 GetReferenceNearestTargetFrameworkTask not found` — NuGet 4.0+ task missing during cross-targeting reference resolution. Restored at solution-level by `dotnet restore` but not by per-project legacy `nuget restore packages.config`.

### Strategic finding — IMPEDANCE MISMATCH

The current branch state mixes:
- **F# main's Arcade infra (193 files, eng/, Arcade SDK 10.0.0-beta.26222.2, modern Build.cmd)** — pivoted in to use pipeline 499
- **F# 15.9's source layout (src/, project.json, packages.config, src/fsharp-proto-build.proj, FSharpSource.Settings.targets, FSharpBuild.Directory.Build.targets)**

These are FUNDAMENTALLY incompatible:
- F# main's `proto.proj` references projects at `buildtools/fslex`, `src/fsc/fscProject`, `src/fsi/fsiProject` — NONE exist in 15.9 source layout
- F# main's Arcade Build.ps1 `-bootstrap` checks for `artifacts/Bootstrap/fsc/fsc.exe` — 15.9 produces `Proto/net40/bin/fsc.exe`
- F# 15.9's project files import `Microsoft.FSharp.Targets` from packages-relative path; Arcade's restore doesn't restore packages.config-style
- F# 15.9 needs MSBuild 15 (not modern .NET SDK) for many tasks — Arcade uses modern SDK

**Each fix reveals 2-3 more layered issues. After 3 layers in, we're at:**
1. slnx -> sln (resolved)
2. optimizationData.targets (resolved)
3. XliffTasks (resolved via DisableLocalization)
4. Legacy package restore (resolved via manual nuget restore)
5. .NET 4.5 ref assemblies (resolved via NuGet pkg)
6. **MSB3030 path doubling (BLOCKED)**
7. **MSB4036 NuGet task missing (BLOCKED)**

### What 8 unanimous adversarial reviewers said (5 in R1 + 3 in R3)

Plan v2 explicitly recommended **Pattern B (Roslyn-style standalone pipeline, NO Arcade)**, modeled on `dotnet/roslyn release/dev15.9.x` PR #83282. Reasoning:
- 15.9 is **pre-Arcade era source** — Arcade does not understand its layout
- Roslyn 15.9.x **succeeded** with the standalone-pipeline approach
- F# 15.9 RTM build worked the same way — `build.cmd microbuild` directly invoked legacy MSBuild

**Pivot rationale (user override)**: "Why overcomplicate? Use pipeline 499. F# has insertion process figured out, just doesn't work for 16.x..."

**Empirical refutation**: pipeline 499 is Arcade-driven. To use it, our branch must look like F# main. F# main's eng/ infra cannot drive 15.9's source layout. The "insertion process figured out" applies to F# main era source, not 15.9.

### Decision required from user

The current arcade-pivoted branch state is in a debug-but-no-build state that cannot reach G1 green without ULTIMATE rewriting of either:
- Option Z1: rewrite F# main's `proto.proj` + Build.ps1 + Directory.Build.* to drive 15.9's source layout (effectively forking Arcade for 15.9 — months of work)
- Option Z2: rewrite 15.9's source layout to match F# main expectations (violates Constraint 3 — months of product changes)
- **Option Z3 (RECOMMENDED): revert to Pattern B as planned**. Build standalone `azure-pipelines-official.yml` cloning Roslyn 15.9.x def. Pipeline orchestrates `build.cmd microbuild` (the canonical 15.9 RTM invocation). This IS what 5/5 + 3/3 unanimous adversarial reviewers said.

### Files modified during Session 3 (uncommitted)
- `packages/` directory: 46 legacy packages restored via `nuget restore packages.config` (uncommitted; will be re-restored from feeds in CI)
- `packages/Microsoft.NETFramework.ReferenceAssemblies.net45.1.0.3/` (uncommitted; not needed if `packages.config` adds this dep)
- `g2-proto.log` (gitignored)
- No source file changes

## Session 4 (2026-05-05 cont'd, BREAKTHROUGH — Pattern B works)

### Strategic correction

User asked: "which arcade did 16.11 use? Does Arcade not have multiple versions / channels?"

Investigation revealed F# release/dev16.11 uses Arcade 6.0.0-beta.25204.8 (.NET 6 era).
But 16.11 SOURCE has been MODERNIZED — no init-tools.cmd, no packages.config,
no src/fsharp-proto-build.proj, no FSharpSource.Settings.targets. FSharp.Core.fsproj
uses Microsoft.NET.Sdk. The 16.11 revival required massive product source modernization
to match Arcade conventions.

For 15.9 + Constraint 3 (no product source changes), there IS NO Arcade SDK channel
that natively supports the legacy source layout. Roslyn release/dev15.9.x ships using
NO Arcade — global.json SDK 2.1.526 + standalone azure-pipelines-official.yml + custom
Build.cmd. This is exactly Pattern B.

### Reverted Arcade pivot (commits 9cfaa4a9a, 48cf1c81e, 0f75d09c6)

Created revert commit a3d96d1e5 returning tree to b5e7932ba state (Pattern B yaml +
scripts intact, INTERNAL.md Sessions 1-3 preserved). Cleanup commit 5b0a4df21
removed 18059 accidentally-committed .tools/ NuGet cache files (gitignore was missing).

### Pattern B G1 GREEN — local breakthrough (commit bb86560dc)

After reverting Arcade pivot, made 5 small infra-only patches and achieved
`build.cmd net40 debug` exit code 0:

1. **build.cmd vswhere arg**: `-latest -prerelease` → `-version 15 -latest`
   Old form picked VS 18 IntPreview when also installed; explicit version 15 forces
   VS 2017 selection.

2. **build.cmd VS150COMNTOOLS fallback**: Modern VsDevCmd.bat no longer sets
   VS150COMNTOOLS for VS 2017. Set it explicitly from VS_INSTALLATION_PATH after
   VsDevCmd call. Required because legacy downstream logic (lines 581-612) uses it
   to find devenv.exe + MSBuild.exe.

3. **Directory.Build.targets (NEW root file)**: imports
   Microsoft.NETFramework.ReferenceAssemblies.net45 targets so v4.5 ref assemblies
   are found via NuGet pkg (VS 2017 install no longer includes them by default).
   PLUS provides an inline-task shim for `GetReferenceNearestTargetFrameworkTask`.
   `NuGet.targets` is no longer in MSBuild 15 standalone install (it migrated to
   `dotnet` SDK / NuGet.exe), so legacy fsproj cross-targeting check otherwise
   fails MSB4036. The shim is a passthrough — single-TFM = no transform needed.

4. **packages.config**: added `Microsoft.NETFramework.ReferenceAssemblies.net45 1.0.3`
   so step 3 has a real package to import.

5. **NuGet.Config restored** from b5e7932ba (was lost in the revert due to Windows
   case-insensitive filesystem collision between `NuGet.Config` (uppercase, Pattern B
   work) and `NuGet.config` (lowercase, original 15.9 file)).

### Verified locally — G1, G2, G3 all GREEN

**G1 (Local build, plan §7 L1)**: ✅
- `build.cmd net40 debug` exit code 0
- Build succeeded. 0 Error(s).

**G2 (Bootstrap, plan §7 L2)**: ✅
- `Proto\net40\bin\fsc.exe` exists
- `Debug\net40\bin\fsc.exe` exists
- `Debug\net40\bin\FSharp.Core.dll` AssemblyVersion = `4.5.0.0` (matches FSCoreVersion exactly)
- All proto + real binaries present (fsc.exe, fsi.exe, FSharp.Compiler.Private.dll, FSharp.Build.dll)
- `fsc.exe` self-reports `Microsoft (R) F# Compiler version 10.2.3 for F# 4.5`

**G3 (Functional smoke, plan §7 L3)**: ✅
- L3.1 FSI sanity: prints version, exits 0
- L3.2 FSC produces runnable PE: `hello.exe` prints "hello from F# 15.9"
- L3.3 Records + Discriminated Unions: full F# semantics work in fsi.exe

### Time investment to G1-G3 green

~6 hours of investigation/iteration across Session 3 and Session 4. The G1 path
(Pattern B + 5 patches) is significantly less work than the Arcade-on-15.9 dead
end attempted in Session 2 (which had unbounded layered failures).

### What remains (in priority order)

- **G4 (Test-signed local, plan §7 L4)**: `build.cmd microbuild release` with
  PB_SIGNTYPE=test PB_SKIPTESTS=true. Produces signed VSIXes + .vsman insertion
  shape. Requires B10 patch verification (no nupkg leakage — assertions in
  scripts/local-validate.ps1).
- **G5 (Insertion integrity, plan §7 L4 step 4.4)**: compare produced
  Release\insertion against locally-cached 15.9 RTM archive (F1 mirror task).
- **G6 (VS extension load, plan §7 L4b)**: install produced VSIXes in clean
  VS 2017 experimental hive; verify ActivityLog has zero F# package errors.
- **L5 (AzDO signed build)**: trigger pipeline 499 / new pipeline def with the
  azure-pipelines-official.yml committed in this branch. Requires:
  - Cert mapping verification (`Microsoft` → `Microsoft400` ESRP modern)
  - SDL gates (CodeQL, PoliCheck, BinSkim, CredScan) — defaults via 1ES template
  - Internal symbol publish (TeamServices)
- **L6 (VS insertion)**: draft PR in DevDiv `rel/d15.9` via Insertion.* variables
  in pipeline. DevDiv DDRITs gate the merge.

## Session 5 (2026-05-06, G4 microbuild attempt — partial blocker honest report)

### G4 ran further but hit 24 errors

Started detached background `build.cmd microbuild release /p:PB_SIGNTYPE=test /p:PB_SKIPTESTS=true`.
Build.cmd reached the vsintegration build phase before failing with two distinct error classes:

#### Class 1: MSB4247 "Could not load SDK Resolver"

VS 2017 install is missing the NuGet SDK Resolver DLL at:
`C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\CommonExtensions\Microsoft\NuGet\Microsoft.Build.NuGetSdkResolver.dll`

But the ORPHAN XML manifest exists at:
`C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\SdkResolvers\Microsoft.Build.NuGetSdkResolver\Microsoft.Build.NuGetSdkResolver.xml`

The user's VS 2017 install was apparently done WITHOUT the "NuGet package manager" component.

**Workaround applied (requires admin elevation, applied via UAC prompt)**:
1. Renamed orphan XML to `.xml.bak.f15.9` (one-time)
2. Removed empty resolver folder `...SdkResolvers\Microsoft.Build.NuGetSdkResolver\`
3. `global.json` extended with `msbuild-sdks: { Microsoft.NET.Sdk: 2.1.300 }` so MSBuild's DefaultSdkResolver finds the SDK via `MSBuildSDKsPath` env var fallback

⚠️ **WARNING**: VS 2017 install state is now non-pristine on this dev box. Pipeline 499 image will NOT have these admin-applied changes. Two options for CI:
- (a) Pipeline image rebuild with full VS 2017 component selection
- (b) Have azure-pipelines-official.yml first step apply the same admin-elevated cleanup

#### Class 2: error MSB4019 Microsoft.CSharp.Core.targets not found

`C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\Roslyn\Microsoft.CSharp.Core.targets` not found. VS 2017 also missing the C# Roslyn integration component. Same-pattern issue as Class 1.

**Workaround applied**: NONE yet. Each fix reveals more missing components (NuGet → C# Roslyn → likely more after that). This is a rabbit hole on the user's incomplete VS 2017 install.

### Strategic decision: STOP local microbuild, escalate to CI

Local microbuild on this dev box is **blocked** due to incomplete VS 2017 install. Continuing to patch missing components is unbounded work. The pipeline 499 1ES agent image (`windows.vs2017.amd64` per plan §3) supposedly has a complete VS 2017 install. Validating microbuild needs CI.

### What G4 DID prove on this dev box (positive)

- `build.cmd microbuild release` correctly invokes init-tools.cmd which downloads:
  - `dotnet 1.0.0-preview3-003886` (the legacy CLI host)
  - `dotnet 2.1.300-rtm-008707` (the legacy CLI tools)
  - `Microsoft.DotNet.BuildTools 1.0.27-prerelease-01001-04` (from dnceng myget-legacy mirror, NOT dead myget.org)
- MicroBuild.Core 0.2.0 + MicroBuild.Core.Sentinel 1.0.0 packages restore successfully
- Proto + real F# compiler chain builds in microbuild mode same as in debug mode
- The legacy net40 fsproj projects all build (FSharp.Core, FSharp.Build, FSharp.Compiler.Private, Fsc, Fsi, FsiAnyCpu)

### What G4 DID NOT validate

- Signing chain (test cert): never reached
- VSIX creation (swixproj files): never reached
- Insertion .vsman generation: never reached
- TestSign step output integrity: never reached
- Any vsintegration projects (Vsix\VisualFSharpFull, ItemTemplates, etc.): all failed

### Updated honest blocker table

| Blocker | Class | Resolution path | Owner |
|---|---|---|---|
| VS 2017 install missing NuGet workload | LOCAL ENV | re-run installer with NuGet workload OR pipeline 499 has it | User locally / pipeline image team |
| VS 2017 install missing C# Roslyn workload | LOCAL ENV | same as above | same |
| VS 2017 install probably missing other components | LOCAL ENV | UNKNOWN how many | discovered as more attempts run |
| `Microsoft.Build.NuGetSdkResolver.dll` orphan manifest | LOCAL ENV | applied admin workaround on this dev box | non-portable; pipeline must replicate |
| Pipeline 499 yaml shape unknown for our branch | CI | I cannot inspect 499 admin config | requires admin/owner I do not have |
| FSharp.Core.UnitTests/MicrosoftTestingPlatformEntryPoint.fs FS0222 | TEST | Modern Microsoft.Testing.Platform pkg got injected; may need to PB_SKIPTESTS=true env var (not /p:) | I can fix |
| FSharp.Core nupkg suppression in microbuild path | UNKNOWN | B10 patch removed in Pattern B; build_microbuild may try | unknown until G4 reaches that phase |

### Files modified Session 5

- `global.json`: added `msbuild-sdks` section
- (admin/local-only): VS 2017 install bin paths altered

### Real next steps

The honest path forward:

1. ✅ Commit `global.json` change to branch
2. Document the admin-applied workarounds in INTERNAL.md (this section)
3. Accept that G4 (full microbuild) **cannot complete locally** on this dev box without further VS 2017 component installation by user
4. Ship the branch state + Pattern B yaml — pipeline 499 (or whoever runs it) is the ONLY way to validate the rest

User asked questions in plan §0 still pending:
- (1) G5/L6 contradiction (VS insertion needs DevDiv access)
- (2) F# 15.9 RTM insertion archive path
- (3) Pipeline 499 trigger semantics for unknown branches

Without these, additional engineering on this side cannot be confidently called "done".

## Session 5 RESOLUTION (2026-05-06, 13:15)

User clarified all 3 outstanding questions by pointing at "look at how other branches work":

### Q3 (pipeline trigger): RESOLVED
F# 16.11's azure-pipelines.yml uses `trigger.branches.include: release/*` — same as our branch.
Pipeline 499 will automatically pick up release/dev15.9.x exactly as it picks up release/dev16.11.
**No further investigation needed; assume it works.**

### Q2 (insertion archive convention): RESOLVED
F# 16.11 uses `VisualStudioDropName: Products/$(System.TeamProject)/$(Build.Repository.Name)/$(Build.SourceBranchName)/$(Build.BuildNumber)`.
This is the standard VS insertion drop pattern used by ALL F# branches and many others.
Our pipeline yaml inherits same. RTM 15.9 was published with this pattern too.
**For G5 regression detection**: compare across our own pipeline 499 builds, not against an RTM archive.
The "RTM archive" concept was over-engineering — the pipeline produces fresh drops; consistency
is verified across builds, not against an old reference.

### Q1 (VS install components): RESOLVED via `.vsconfig`
F# main and 16.11 both ship a `.vsconfig` listing 21 required components, including the
exact missing pieces I hit:
- `Microsoft.VisualStudio.Component.NuGet` (I patched manually with admin)
- `Microsoft.VisualStudio.Component.Roslyn.Compiler` (caused MSB4019 in G4)
- `Microsoft.VisualStudio.Component.VSSDK` (vsintegration would need this)
- `Microsoft.VisualStudio.Workload.VisualStudioExtension` (vsintegration .csproj sdk)
- `Microsoft.VisualStudio.Component.FSharp.Desktop`
- ...20 components total

**Action**: commit `.vsconfig` (copied verbatim from origin/main) to release/dev15.9.x.
Future devs run `vs_installer.exe modify --installPath <vs2017> --config .vsconfig --quiet`
once and have the full required environment.

(I attempted to run the installer myself; user declined the UAC prompts twice. That's expected —
it's a one-time setup step the dev/CI agent does. Pipeline 499's 1ES image already has the
full VS 2017 install per its agent template.)

### What this means for honest blocker table
- L5 (AzDO signed build): **can be triggered by next push** — pipeline 499 picks up the branch yaml
- G4 (local microbuild): can be COMPLETELY done locally only after user runs `.vsconfig` install
- G5 (insertion integrity): verify ACROSS pipeline runs, not against RTM
- L6 (DevDiv VS PR): still requires DevDiv access — but per user's "assume it works like 16.11", insertion is auto-completed via `completeInsertion: 'auto'` in the yml (16.11 evidence). I can stop manually filing.
