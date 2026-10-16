---
name: reviewing-vsintegration
description: "Routes a Visual Studio integration or compiler-service review to the file for that tree. Use when reviewing a PR, pre-merge change, or edit under vsintegration, FSharp.Editor, the legacy language service, FSI, the project system, MEF, src/Compiler/Service, src/Compiler/Symbols, FCS snapshots, the incremental builder, workspace, navigation, completion, or symbols. Invokes @ide-integration. Does not review compiler type checking, IL, pickle, or FSharp.Core."
---

# Reviewing VS integration

Tree checks live in `vsintegration.md` and `compiler-service.md`; the three boundary checks live in `@ide-integration`.

`@expert-reviewer` stays the compiler reviewer. Do not send `vsintegration` or `src/Compiler/Service` findings there.

## Which file

| Diff touches | Read |
|---|---|
| `vsintegration/**` | [vsintegration.md](vsintegration.md) |
| `src/Compiler/Service/**` or `src/Compiler/Symbols/**` | [compiler-service.md](compiler-service.md) |
| Both | Both files |
| `vsintegration/tests/**` | [vsintegration.md](vsintegration.md) |
| `tests/FSharp.Compiler.Service.Tests/**` | [compiler-service.md](compiler-service.md) |

Use the Both trees rules only when the diff touches both trees or adds a reference from `src/Compiler/Service` to `vsintegration`.

## Folder routing

| Path | File | Select |
|---|---|---|
| `vsintegration/src/FSharp.Editor/**` | vsintegration.md | UI thread, MEF, caches, quick info, navigation, completion, diagnostics, options, classification |
| `vsintegration/src/FSharp.LanguageService/**` | vsintegration.md | Legacy language service, UI thread, solution lifetime |
| `vsintegration/src/FSharp.ProjectSystem*/**` | vsintegration.md | Project lifetime, UI thread, generator identity |
| `vsintegration/src/FSharp.VS.FSI/**` | vsintegration.md | Interactive session startup and cancellation |
| `vsintegration/tests/**` | vsintegration.md | Editor tests must call the host provider |
| `vsintegration/**` other | vsintegration.md | Do not push host types into the compiler service |
| `src/Compiler/Service/**` | compiler-service.md | Cache identity, incremental results, locks, public FCS API, diagnostics, readers |
| `src/Compiler/Symbols/**` | compiler-service.md | Symbol results, ranges, completion, walks |
| `tests/FSharp.Compiler.Service.Tests/**` | compiler-service.md | Assert the observable service result. Isolate process-wide state |

## Do not route here

- `src/Compiler/Checking/`, `src/Compiler/Optimize/`, `src/Compiler/CodeGen/`, `src/Compiler/TypedTree/`, `src/Compiler/SyntaxTree/`, `src/Compiler/Driver/`
- `src/FSharp.Core/**`
- `src/FSharp.Compiler.LanguageServer/**` is not this corpus. Compiler concurrency stays with `@expert-reviewer`.
