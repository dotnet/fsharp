---
name: reviewing-vsintegration
description: "Routes a Visual Studio integration or compiler-service review to the file for that tree. Use when reviewing a PR, pre-merge change, or edit under vsintegration, FSharp.Editor, the legacy language service, FSI, the project system, MEF, src/Compiler/Service, src/Compiler/Symbols, FCS snapshots, the incremental builder, workspace, navigation, completion, or symbols. Invokes @vsintegration-reviewer. Does not review compiler type checking, IL, pickle, or FSharp.Core."
---

# Reviewing VS integration

CHECK rules live in two files. The five-wave workflow lives in `@vsintegration-reviewer`. Do not copy `@vs-integration-perf`. Invoke it for JTF, retained memory, and the perf execution map.

`@expert-reviewer` stays the compiler reviewer. Do not send `vsintegration` or `src/Compiler/Service` findings there.

## Which file

| Diff touches | Read |
|---|---|
| `vsintegration/**` | [vsintegration.md](vsintegration.md) |
| `src/Compiler/Service/**` or `src/Compiler/Symbols/**` | [compiler-service.md](compiler-service.md) |
| Both | Both files |
| `vsintegration/tests/**` | [vsintegration.md](vsintegration.md) |
| `tests/FSharp.Compiler.Service.Tests/**` | [compiler-service.md](compiler-service.md) |

A change that crosses the two trees, owns cancellation, or could make `src/Compiler/Service` depend on `vsintegration` also uses the three rules under Both trees in `@vsintegration-reviewer`.

## Folder routing

| Path | File | Select |
|---|---|---|
| `vsintegration/src/FSharp.Editor/**` | vsintegration.md | UI thread, MEF, caches, quick info, navigation, completion, diagnostics scope, options, classification, feature failures |
| `vsintegration/src/FSharp.LanguageService/**` | vsintegration.md | Legacy language service, UI thread, solution lifetime |
| `vsintegration/src/FSharp.ProjectSystem*/**` | vsintegration.md | Project lifetime, UI thread, generator identity |
| `vsintegration/src/FSharp.VS.FSI/**` | vsintegration.md | Interactive session startup and cancellation |
| `vsintegration/tests/**` | vsintegration.md | Editor tests must call the host provider |
| `vsintegration/**` other | vsintegration.md | Do not push host types into the compiler service |
| `src/Compiler/Service/**` | compiler-service.md | Cache identity, incremental results, locks, public FCS API, diagnostics, readers |
| `src/Compiler/Symbols/**` | compiler-service.md | Symbol results, ranges, completion, walks |
| `tests/FSharp.Compiler.Service.Tests/**` | compiler-service.md | Assert the observable service result. Isolate process-wide state |

## Do not route here

- `src/Compiler/Checking/`, `Optimize/`, `CodeGen/`, `TypedTree/`, `SyntaxTree/`, `Driver/`
- `src/FSharp.Core/**`
- `src/FSharp.Compiler.LanguageServer/**` — not this corpus. The IDE responsiveness section in both files still names that hotspot because that section was moved intact.
