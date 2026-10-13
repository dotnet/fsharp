---
title: Extension SRTP evidence
category: Postmortems
categoryindex: 550
index: 700
---
# Regression: Extension SRTP evidence lost across files

## Summary

Extension-SRTP calls failed across files in the released RC1 compiler-service package with preview mode enabled. Copied constraints lost their definition scope or selected witness identity.

## Error Manifestation

[#20683](https://github.com/dotnet/fsharp/issues/20683) and [#20684](https://github.com/dotnet/fsharp/issues/20684) report failures on fresh `main`. The members exist in the library source, but compilation fails in the consumer:

```text
FS0001: None of the types 'int, Library.Marker' support the operator 'Add'
FS0971: Undefined value '*'
```

`Library.add 1 2` should produce `3`. `Library.A.mul "a" 3` should produce `"aaa"`.

A local check reproduced both errors with released [FSharp.Compiler.Service `43.13.101-rc1.26425.128`](https://www.nuget.org/packages/FSharp.Compiler.Service/43.13.101-rc1.26425.128).

The released compiler accepted the [#20685](https://github.com/dotnet/fsharp/issues/20685) sample with `--optimize-` and produced `1`.

## Root Cause

A witness is the member selected to satisfy an SRTP constraint. Equal argument types do not imply equal witnesses.

[#19602](https://github.com/dotnet/fsharp/pull/19602) captured definition scope without remapping its type and value identities. Its operator replay cache used types and source ranges. A copied body can retain its definition range instead of its caller's range.

**Invariant:** scope and selected evidence must follow each constraint through copying and code generation.

| Constraint state | Required evidence |
| --- | --- |
| Unresolved, same assembly | Definition scope |
| Unresolved, imported | Consumer scope |
| Selected, imported | Retain the valid selected witness |

Metadata can store a selected witness. It does not store the captured type-checking environment (`TcEnv`).

## Why It Escaped

The introducing tests already covered file and assembly boundaries. The cross-file inline test opened `Lib.Ext` in the consumer and enabled optimization. The definition-scope test used one file.

**Coverage inference:** those separate tests did not establish that definition-only scope or competing witnesses survive a file boundary.

## Fix

[#20717](https://github.com/dotnet/fsharp/pull/20717) retains selected witnesses on fresh application type-argument cells. It remaps captured scope identities and retrieves selected evidence before type normalization or unification discards those cells.

Quotation translation preserves witness order and built-in operator priority. The type-only specialization cache excludes trait-bearing calls when the feature is enabled. The quotation and cache corrections arose during repair review, not from separate public escape reports.

## Timeline

| Date | Event |
| --- | --- |
| 2026-08-21 | [#19602](https://github.com/dotnet/fsharp/pull/19602) merges as `7727673ea13f`. |
| 2026-09-08 | [SDK `11.0.100-rc.1.26425.128`](https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/11.0/releases.json) and the compiler-service package ship. Their [VMR manifest](https://github.com/dotnet/dotnet/blob/3551975be08744f0418857c5bed8ab1545c5dd47/src/source-manifest.json) selects F# commit `c251d06dc38d`, which contains #19602. |
| 2026-10-01 | #20683 and #20684 report the cross-file failures on fresh `main`. |
| 2026-10-09 | #20717 merges as `6cde2b16ef45`. |
| 2026-10-13 | Released-bit checks reproduce FS0001 and FS0971. The #20685 sample compiles with `--optimize-` and produces `1`. |

## Prevention

Existing [regression tests](https://github.com/dotnet/fsharp/blob/main/tests/FSharp.Compiler.ComponentTests/Conformance/Types/TypeConstraints/ExtensionConstraints/ExtensionConstraintsTests.fs) cover definition scope, competing witnesses, and imported calls with equal types.

The [path-scoped instruction](https://github.com/dotnet/fsharp/blob/main/.github/instructions/ExtensionConstraintEvidence.instructions.md) requires application-owned evidence, remapped scope, and tests across file/import boundaries.
