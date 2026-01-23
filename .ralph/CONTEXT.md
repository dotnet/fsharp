# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Infrastructure + Issues 1-10

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Issues 11-20

**Summary:** Added 10 tests for issues #18319, #18263, #18140, #18135, #18125, #17692, #17641, #16565, #16546, #16378

**Issues covered:**
- #18319: Literal upcast missing box instruction (Invalid IL)
- #18263: DU .Is* properties duplicate method (Compile Error)
- #18140: Callvirt on value type ILVerify error (Invalid IL)
- #18135: Static abstract with byref params error (Compile Error)
- #18125: Wrong StructLayoutAttribute.Size for struct unions (Incorrect Metadata)
- #17692: Mutual recursion duplicate param name (Invalid IL)
- #17641: IsMethod/IsProperty incorrect for generated (API Issue)
- #16565: DefaultAugmentation(false) duplicate entry (Compile Error)
- #16546: Debug build recursive reference null (Wrong Behavior)
- #16378: DU logging allocations (Performance)

**Files modified:**
- `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs` - 10 new tests
- `CODEGEN_REGRESSIONS.md` - 10 new issue entries with analysis

**Total tests:** 20 (10 from Sprint 1 + 10 from Sprint 2)

---

## Sprint 3: Issues 21-30

**Summary:** Added 10 tests for issues #16362, #16292, #16245, #16037, #15627, #15467, #15352, #15326, #15092, #14712

**Issues covered:**
- #16362: Extension methods with CompiledName generate C# incompatible names (C# Interop)
- #16292: Debug SRTP mutable struct incorrect codegen (Wrong Behavior)
- #16245: Span IL gen produces 2 get_Item calls (Performance)
- #16037: Tuple pattern in lambda suboptimal (Performance)
- #15627: Async before EntryPoint hangs program (Wrong Behavior)
- #15467: Include language version in metadata (Feature Request)
- #15352: User code gets CompilerGeneratedAttribute (Incorrect Attribute)
- #15326: InlineIfLambda delegates not inlined (Optimization Regression)
- #15092: DebuggerProxies in release builds (Feature Request)
- #14712: Signature generation uses System.Int32 instead of int (Cosmetic)

**Files modified:**
- `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs` - 10 new tests
- `CODEGEN_REGRESSIONS.md` - 10 new issue entries with analysis

**Total tests:** 30 (20 from Sprints 1-2 + 10 from Sprint 3)

---
