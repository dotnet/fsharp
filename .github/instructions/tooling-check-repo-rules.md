# Repo-specific rules for PR Tooling Safety Check
#
# This file is read by the labelops-pr-security-scan workflow.
# It defines additional categories beyond the generic .NET ones.
# Edit this file to add repo-specific paths that matter for YOUR repo.

## ⚠️ Affects-Bootstrap

PR modifies the F# compiler bootstrap chain. The compiler builds itself:
PROTO compiler → new compiler → everything else. A compromised bootstrap
produces a compromised compiler that compiles all user code.

**Trigger on:** `proto.proj`, `FSharpBuild.Directory.Build.*`, `buildtools/fslex/**`, `buildtools/fsyacc/**`, files referencing `Configuration==Proto` or `BUILDING_USING_DOTNET` or `ProtoOutputPath`.

## ⚠️ Affects-Compiler-Output

PR modifies the IL emission or code generation pipeline. Compiled binaries
could behave differently than source review suggests.

**Trigger on:** `src/Compiler/AbstractIL/ilwrite*`, `src/Compiler/CodeGen/**`, `src/Compiler/AbstractIL/ilreflect*`, `src/Compiler/TypedTree/TypedTreePickle*`, `src/FSharp.Build/**`.

## ⚠️ Affects-Design-Time

PR modifies type provider infrastructure, the `#r "nuget:..."` dependency
manager, or IDE integration that executes code at design time in VS or FSI.

**Trigger on:** `src/Compiler/TypedTree/TypeProviders.fs`, `src/FSharp.DependencyManager.Nuget/**`, `vsintegration/tests/MockTypeProviders/**`.

## ⚠️ Affects-Test-Tooling

PR modifies test build configuration, test runner setup, or test
infrastructure that spawns external processes.

**Trigger on:** `tests/FSharp.Test.Utilities/FSharp.Test.Utilities.fsproj`, `tests/FSharp.Test.Utilities/TestFramework.fs`, `tests/FSharp.Test.Utilities/ProjectGeneration.fs`, `tests/EndToEndBuildTests/**`, `*.runsettings`.

**Does NOT trigger on:** test helper methods (`Compiler.fs`, `CompilerAssert.fs`, `Assert.fs`, `SurfaceArea.fs`, `XunitHelpers.fs`).

## Trusted authors

`T-Gro`, `abonie`, `dotnet-bot`, `dotnet-maestro`, `dotnet-maestro[bot]`, `copilot`, `copilot-swe-agent`, `github-actions`, `github-actions[bot]`

## Non-fork bypass

PRs with head repository `dotnet/fsharp` (not a fork) are auto-cleaned — pushed by someone with write access.
