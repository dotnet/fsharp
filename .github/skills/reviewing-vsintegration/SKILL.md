---
name: reviewing-vsintegration
description: "Selects Pack A and Pack B review dimensions for Visual Studio integration and the host-neutral F# compiler service. Use when reviewing a PR, pre-merge change, or edit under vsintegration, FSharp.Editor, the legacy language service, FSI, the project system, MEF, src/Compiler/Service, src/Compiler/Symbols, FCS snapshots, the incremental builder, workspace, navigation, completion, or symbols. Invokes @vsintegration-reviewer. Does not review compiler type checking, IL, pickle, or FSharp.Core."
---

# Reviewing VS integration

Full CHECK rules and the five-wave workflow live in `@vsintegration-reviewer`. This skill only routes folders to a pack. Do not copy `@vs-integration-perf`. Invoke it for JTF, retained memory, and the perf execution map.

`@expert-reviewer` stays the compiler reviewer. Do not send Service or `vsintegration` findings there.

## When to invoke

- Diff touches `vsintegration/**` → Pack A
- Diff touches `src/Compiler/Service/**` or `src/Compiler/Symbols/**` → Pack B
- Diff touches both → both packs
- Diff touches `tests/FSharp.Compiler.Service.Tests/**` or `vsintegration/tests/**` → the pack those tests belong to, not a third pack
- Boundary, cancellation ownership, or a Service reference to `vsintegration` → the three shared dimensions in the agent

## Folder routing

| Path | Pack | Select |
|---|---|---|
| `vsintegration/src/FSharp.Editor/**` | A | UI thread, MEF, caches, quick info, navigation, completion, diagnostics scope, options, classification, feature failures |
| `vsintegration/src/FSharp.LanguageService/**` | A | Legacy language service, UI thread, solution lifetime |
| `vsintegration/src/FSharp.ProjectSystem*/**` | A | Project lifetime, UI thread, generator identity |
| `vsintegration/src/FSharp.VS.FSI/**` | A | Interactive session startup and cancellation |
| `vsintegration/tests/**` | A | Editor tests must call the host provider |
| `vsintegration/**` other | A | Same pack. Do not push host types into FCS |
| `src/Compiler/Service/**` | B | Cache identity, incremental results, locks, public FCS API, diagnostics, readers |
| `src/Compiler/Symbols/**` | B | Symbol results, ranges, completion, walks |
| `tests/FSharp.Compiler.Service.Tests/**` | B | Assert the observable service result. Isolate process-wide state |

## Shared, and only these

- Snapshot identity across the host/service boundary
- Cancellation ownership
- Service must not depend on `vsintegration`

## Do not route here

- `src/Compiler/Checking/`, `Optimize/`, `CodeGen/`, `TypedTree/`, `SyntaxTree/`, `Driver/` except a Service caller that is already in Pack B
- `src/FSharp.Core/**`
- `src/FSharp.Compiler.LanguageServer/**` — not this corpus. IDE responsiveness checks live in `@vsintegration-reviewer` because that dimension moved; do not add LanguageServer rules here
