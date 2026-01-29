# F# Codegen Bug Fix Campaign - FOCUSED SCOPE

## Goal

Fix **24 doable codegen bugs** out of 62 total issues, focusing on issues that:
- Can be fixed with surgical changes to IlxGen.fs, ilwrite.fs, Optimizer.fs, or NicePrint.fs
- Don't require type checker modifications
- Don't require multi-sprint architectural changes
- Have clear reproduction cases and expected behavior

## Current Progress (2026-01-29)

- ✅ **26 issues FIXED** (21 actual fixes + 5 OUT_OF_SCOPE feature requests)
- ✅ **4 issues marked KNOWN_LIMITATION** (require major architectural work)
- 🎯 **24 issues REMAINING** (doable with surgical fixes)
- ⏭️ **8 issues DEFERRED** (require type checker changes or multi-sprint work)

**Total: 26 + 4 + 24 + 8 = 62 issues**

---

## 24 DOABLE Issues (Grouped by Area)

### Group A: Metadata & Attributes (8 issues - EASY)

**IlxGen.fs - Metadata/Attributes (4 issues)**
1. [#19020](https://github.com/dotnet/fsharp/issues/19020) - `[<return:>]` not respected on class members
2. [#18125](https://github.com/dotnet/fsharp/issues/18125) - Wrong StructLayoutAttribute.Size for struct unions  
3. [#15352](https://github.com/dotnet/fsharp/issues/15352) - User symbols get CompilerGeneratedAttribute incorrectly
4. [#11935](https://github.com/dotnet/fsharp/issues/11935) - `unmanaged` constraint not recognized by C#

**ilwrite.fs - Interop/Metadata (4 issues)**
5. [#13108](https://github.com/dotnet/fsharp/issues/13108) - Static linking FS2009 warnings
6. [#12460](https://github.com/dotnet/fsharp/issues/12460) - F# and C# produce Version info differently
7. [#7861](https://github.com/dotnet/fsharp/issues/7861) - Missing assembly reference for types in attributes
8. [#5464](https://github.com/dotnet/fsharp/issues/5464) - F# ignores custom modifiers modreq/modopt

**Why EASY:** Simple metadata/attribute additions or corrections. No complex logic.

---

### Group B: Cosmetic Fixes (4 issues - EASY)

**NicePrint.fs - Signature Generation (2 issues)**
9. [#14712](https://github.com/dotnet/fsharp/issues/14712) - Signature generation should use F# Core alias
10. [#14706](https://github.com/dotnet/fsharp/issues/14706) - Signature generation WhereTyparSubtypeOfType

**IlxGen.fs - Naming (1 issue)**
11. [#12366](https://github.com/dotnet/fsharp/issues/12366) - Rethink names for compiler-generated closures

**Symbols.fs - API (1 issue)**
12. [#17641](https://github.com/dotnet/fsharp/issues/17641) - IsMethod/IsProperty incorrect for generated members

**Why EASY:** Don't change semantics, just improve naming/output quality.

---

### Group C: Performance - IlxGen (6 issues - MEDIUM)

13. [#16378](https://github.com/dotnet/fsharp/issues/16378) - DU logging causes significant allocations
14. [#16362](https://github.com/dotnet/fsharp/issues/16362) - Extension methods with CompiledName C# incompatible
15. [#16245](https://github.com/dotnet/fsharp/issues/16245) - Span IL gen produces 2 get_Item calls
16. [#12546](https://github.com/dotnet/fsharp/issues/12546) - Implicit boxing produces extraneous closure
17. [#11556](https://github.com/dotnet/fsharp/issues/11556) - Better IL output for property/field initializers
18. [#9348](https://github.com/dotnet/fsharp/issues/9348) - Performance of Comparing and Ordering

**Why MEDIUM:** IL generation optimizations, localized to IlxGen.fs.

---

### Group D: Performance - Optimizer (6 issues - MEDIUM)

19. [#18753](https://github.com/dotnet/fsharp/issues/18753) - CE inlining prevented by DU constructor  
20. [#16037](https://github.com/dotnet/fsharp/issues/16037) - Tuple pattern in lambda suboptimal
21. [#15326](https://github.com/dotnet/fsharp/issues/15326) - InlineIfLambda delegates not inlined
22. [#12416](https://github.com/dotnet/fsharp/issues/12416) - Optimization inlining inconsistent with piping
23. [#12139](https://github.com/dotnet/fsharp/issues/12139) - Improve string null check IL codegen
24. [#12137](https://github.com/dotnet/fsharp/issues/12137) - Reduce emit of `tail.` prefix

**Why MEDIUM:** Optimizer pass improvements, localized to Optimizer.fs.

---

## DEFERRED Issues (8 issues)

### Known Limitations (4 issues - require architectural changes)
- [#12136](https://github.com/dotnet/fsharp/issues/12136) - `use fixed` does not unpin (requires scope tracking)
- [#16292](https://github.com/dotnet/fsharp/issues/16292) - SRTP debug mutable struct (requires defensive copy analysis)
- [#16546](https://github.com/dotnet/fsharp/issues/16546) - Debug recursive reference null (requires type checker)
- [#15627](https://github.com/dotnet/fsharp/issues/15627) - Async before EntryPoint hangs (CLR deadlock)

### Out of Scope (1 issue)
- [#13218](https://github.com/dotnet/fsharp/issues/13218) - Compilation time performance (not codegen)

### Requires Type Checker (3 issues - beyond surgical scope)
- [#14707](https://github.com/dotnet/fsharp/issues/14707) - Signature files become unusable
- [#6379](https://github.com/dotnet/fsharp/issues/6379) - FS2014 when using tupled args
- [#11114](https://github.com/dotnet/fsharp/issues/11114) - Record StackOverflow
- [#6750](https://github.com/dotnet/fsharp/issues/6750) - Mutually recursive values uninitialized

---

## Strategy

### Phase 1: Quick Wins (Groups A + B = 12 issues)
Start with EASY metadata, attribute, and cosmetic fixes.

### Phase 2: Performance (Groups C + D = 12 issues)  
Move to MEDIUM performance and optimization issues.

### Principles
1. **One issue at a time** - Full test verification between fixes
2. **Surgical changes** - Minimal edits (< 50 lines per issue)
3. **Quick abandon** - If harder than expected, mark DEFERRED
4. **Document fixes** - Update CODEGEN_REGRESSIONS.md

---

## File Map

- **Tests:** `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs`
- **Docs:** `CODEGEN_REGRESSIONS.md`
- **Code:**
  - `src/Compiler/CodeGen/IlxGen.fs` (14 issues)
  - `src/Compiler/CodeGen/ilwrite.fs` (4 issues)
  - `src/Compiler/Optimize/Optimizer.fs` (6 issues)
  - `src/Compiler/Driver/NicePrint.fs` (2 issues)
  - `src/Compiler/Symbols/Symbols.fs` (1 issue)
