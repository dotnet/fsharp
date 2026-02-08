---
name: fsharp-diagnostics
description: "Get F# compiler errors/warnings for src/Compiler/FSharp.Compiler.Service.fsproj (Release, net10.0). Parse-only or full typecheck."
---

# F# Diagnostics

**Project:** `src/Compiler/FSharp.Compiler.Service.fsproj` | Release | net10.0 | BUILDING_USING_DOTNET=true

## Setup

```bash
alias get-fsharp-errors='$(git rev-parse --show-toplevel)/.github/skills/fsharp-diagnostics/scripts/get-fsharp-errors.sh'
```

## Two-Phase Workflow

**Step 1 — Parse-only.** Always run first.
```bash
get-fsharp-errors --parse-only src/Compiler/Checking/CheckBasics.fs
```
If errors → fix syntax. Do NOT proceed to Step 2.

**Step 2 — Full typecheck.** Only after Step 1 is clean.
```bash
get-fsharp-errors src/Compiler/Checking/CheckBasics.fs
```

## Other Commands

```bash
get-fsharp-errors --check-project   # typecheck entire project
get-fsharp-errors --ping            # check server is alive
get-fsharp-errors --shutdown        # stop server
```

## Output

Clean: `OK`

Errors (one per line):
```
ERROR FS0039 (12,5-12,15) The value or constructor 'foo' is not defined | let x = foo
WARNING FS0020 (15,1-15,5) The result is implicitly ignored | doSomething()
```

## Notes

- Server auto-starts on first call, one per repo copy, 4h idle timeout.
- ~3 GB RSS after full project load. First project typecheck ~65s, subsequent file checks <500ms.
- Log: `~/.fsharp-diag/<hash>.log`
