---
name: vsintegration-ide-debugging
description: Fix F# debugging issues (breakpoints, .pdb, sequence points). Build, run VS integration tests, inspect IL/PDB.
---

# VS Integration Debugging

## Build
`.\Build.cmd -c Debug`

## Run tests
xUnit2 VS Extensibility — not `dotnet test`.
```powershell
$runner = (Get-ChildItem "$env:NUGET_PACKAGES\xunit.runner.console" -Recurse -Filter xunit.console.exe | Where-Object { $_.FullName -like '*net472*' } | Select-Object -First 1).FullName
& $runner artifacts\bin\FSharp.Editor.IntegrationTests\Debug\net472\FSharp.Editor.IntegrationTests.exe -parallel none -nologo
```

## Key paths
- Tests + infra: `vsintegration/tests/FSharp.Editor.IntegrationTests/`
- Sequence point emit: `src/Compiler/CodeGen/IlxGen.fs` → `EmitDebugPoint`, `EnsureNopBetweenDebugPoints`
- PDB writer: `src/Compiler/AbstractIL/ilwritepdb.fs`
- Collection lowering: `src/Compiler/Optimize/LowerComputedCollections.fs`
- Local compiler setup: `UseLocalCompiler.Directory.Build.props`

## Core rule
VS debugger binds breakpoints only at **stack-empty** IL offsets. Move the sequence point — never add runtime instructions.
