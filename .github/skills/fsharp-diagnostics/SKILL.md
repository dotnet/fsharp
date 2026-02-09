---
name: fsharp-diagnostics
description: "After modifying any F# file, use this to get quick parse errors and typecheck warnings+errors. Also finds symbol references and inferred type hints."
---

# F# Diagnostics

**Scope:** `src/Compiler/` files only (`FSharp.Compiler.Service.fsproj`, Release, net10.0).

## Setup (run once per shell session)

```bash
GetErrors() { "$(git rev-parse --show-toplevel)/.github/skills/fsharp-diagnostics/scripts/get-fsharp-errors.sh" "$@"; }
```

## Parse first, typecheck second

```bash
GetErrors --parse-only src/Compiler/Checking/CheckBasics.fs
```
If errors → fix syntax. Do NOT typecheck until parse is clean.
```bash
GetErrors src/Compiler/Checking/CheckBasics.fs
```

## Find references for a single symbol (line 1-based, col 0-based)

Before renaming or to understand call sites:
```bash
GetErrors --find-refs src/Compiler/Checking/CheckBasics.fs 30 5
```

## Type hints for a range selection (begin and end line numbers, 1-based)

To see inferred types as inline `// (name: Type)` comments:
```bash
GetErrors --type-hints src/Compiler/TypedTree/TypedTreeOps.fs 1028 1032
```

## Other

```bash
GetErrors --check-project   # typecheck entire project
GetErrors --ping
GetErrors --shutdown
```

First call starts server (~70s cold start, set initial_wait=600). Auto-shuts down after 4h idle. ~3 GB RAM.
