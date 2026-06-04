---
name: fsharp-diagnostics
description: "Always invoke after editing .fs files. Provides fast parse/typecheck feedback without a full dotnet build. Prefer this over dotnet build for iterative changes. Also finds symbol references and inferred type hints."
---

# F# Diagnostics

**Scope:** `src/Compiler/` files only (`FSharp.Compiler.Service.fsproj`, Release, net10.0).

## Setup (run once per shell session)

Works on macOS, Linux, and Windows — requires pwsh 7+ (`brew install powershell` / `winget install Microsoft.PowerShell` / `apt install powershell`).

```pwsh
function GetErrors { & "$(git rev-parse --show-toplevel)/.github/skills/fsharp-diagnostics/scripts/get-fsharp-errors.ps1" @args }
```

If your shell is bash/zsh and you don't want to switch, the script also runs as `pwsh -File <path>/get-fsharp-errors.ps1 ...`.

## Parse first, typecheck second

```pwsh
GetErrors -ParseOnly src/Compiler/Checking/CheckBasics.fs
```
If errors → fix syntax. Do NOT typecheck until parse is clean.
```pwsh
GetErrors src/Compiler/Checking/CheckBasics.fs
```

## Find references for a single symbol (line 1-based, col 0-based)

Before renaming or to understand call sites:
```pwsh
GetErrors -FindRefs src/Compiler/Checking/CheckBasics.fs 30 5
```

## Type hints for a range selection (begin and end line numbers, 1-based)

To see inferred types as inline `// (name: Type)` comments:
```pwsh
GetErrors -TypeHints src/Compiler/TypedTree/TypedTreeOps.Transforms.fs 100 120
```

## Other

```pwsh
GetErrors -CheckProject   # typecheck entire project
GetErrors -Ping
GetErrors -Shutdown
```

First call starts server (~70s cold start, set initial_wait=600). Auto-shuts down after 4h idle. ~3 GB RAM.
