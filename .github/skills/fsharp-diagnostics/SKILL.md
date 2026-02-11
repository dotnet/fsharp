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

## Rules

1. **After every edit** to a `src/Compiler/*.fs` file → typecheck it before proceeding. This catches errors in ~2s vs ~35s for a full build. Do NOT attempt `dotnet build` or `dotnet test` until the file typechecks clean.
2. **Use `--find-refs` instead of grep** for finding usages of a symbol (function, type, member, field). Returns semantically resolved references — no false positives from comments, strings, or similarly-named symbols.
3. **Use `--type-hints` to read code blocks** — F# infers most types, so bindings like `env`, `state`, `x` are opaque without it.
   - ⚠️ Output has `// (name: Type)` annotations. These are **read-only overlays**. When editing, use `view` to get the real unannotated source.
4. **Parse first, typecheck second** — fix `--parse-only` errors before running a full typecheck.

## Commands

```bash
GetErrors --parse-only src/Compiler/path/File.fs        # parse errors only
GetErrors src/Compiler/path/File.fs                     # full typecheck
GetErrors --find-refs src/Compiler/path/File.fs 30 5    # references (line 1-based, col 0-based)
GetErrors --type-hints src/Compiler/path/File.fs 50 60  # annotated code (line range, 1-based)
GetErrors --check-project                               # typecheck entire project
GetErrors --ping                                        # server alive?
GetErrors --shutdown                                    # stop server
```

## Cached test runs

No separate `dotnet build` of FSharp.Compiler.Service needed — `dotnet test` builds all dependencies automatically.

```bash
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj -c Release /p:FastBuildFromCache=true
```

First call starts server (~70s cold start, set initial_wait=600). Auto-shuts down after 4h idle. ~3 GB RAM.
