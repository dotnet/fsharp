---
name: fsharp-diagnostics
description: Always invoke after editing `.fs` files under `src/Compiler/`. Fast parse/typecheck without `dotnet build`, plus symbol references and inferred type hints. Use whenever the user asks about F# errors, compile errors, type inference, finding usages, or renaming a symbol in the compiler tree.
---

# F# Diagnostics

**Scope:** `src/Compiler/` files only.

## Setup (once per session)

Requires pwsh 7+ (`brew install powershell` / `winget install Microsoft.PowerShell` / `apt install powershell`).

```pwsh
function GetErrors { & "$(git rev-parse --show-toplevel)/.github/skills/fsharp-diagnostics/scripts/get-fsharp-errors.ps1" @args }
```

From bash/zsh without a function: `pwsh -File <repo>/.github/skills/fsharp-diagnostics/scripts/get-fsharp-errors.ps1 <args>`.

## Parse first, typecheck second

```pwsh
GetErrors -ParseOnly src/Compiler/Checking/CheckBasics.fs   # syntax only
GetErrors            src/Compiler/Checking/CheckBasics.fs   # full typecheck
```
Fix all parse errors before typechecking; type errors on top of bad syntax are noise.

## Symbol references (line 1-based, col 0-based)

```pwsh
GetErrors -FindRefs src/Compiler/Checking/CheckBasics.fs 30 5
```
Use before any rename.

## Type hints (line range, 1-based)

Returns the range with inferred types as inline `// (name: Type)` comments:
```pwsh
GetErrors -TypeHints src/Compiler/TypedTree/TypedTreeOps.Transforms.fs 100 120
```

## Other

```pwsh
GetErrors -CheckProject   # typecheck entire project
GetErrors -Ping           # liveness check, no side effects
GetErrors -Shutdown
```

## Timing

- First real call after a fresh clone: server build + in-memory warmup, 5–15 min → `initial_wait=1200`.
- After warmup: real commands answer in seconds → `initial_wait=180`.
- `-Ping` / `-Shutdown`: sub-second; never trigger build or warmup.

Auto-shuts down after 4h idle; ~3 GB RAM while running.
