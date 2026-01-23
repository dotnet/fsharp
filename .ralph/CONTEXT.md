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

## Sprint 4: Issues 31-40

**Summary:** Improved 10 tests for issues #14707, #14706, #14508, #14492, #14392, #14321, #13468, #13447, #13223, #13218

**Issues covered:**
- #14707: Signature files become unusable with wildcards (Signature Generation)
- #14706: Signature generation WhereTyparSubtypeOfType (Signature Generation)
- #14508: nativeptr in interfaces leads to TypeLoadException (Runtime Error)
- #14492: Release config TypeLoadException with inline constraints (Runtime Error)
- #14392: OpenApi Swashbuckle support (Feature Request - OUT OF SCOPE)
- #14321: DU constructors and IWSAM names conflict (Compile Error)
- #13468: outref compiled as byref (Wrong IL Metadata)
- #13447: Extra tail instruction corrupts stack (Runtime Crash)
- #13223: FSharp.Build reference assemblies (Feature Request - OUT OF SCOPE)
- #13218: Compilation performance 13000 members (Performance - NOT CODEGEN BUG)

**Non-codegen issues noted:**
- #14392: Feature request for OpenAPI/Swashbuckle interop
- #13223: Feature request for FSharp.Build reference assembly support
- #13218: Compilation performance issue, not a codegen bug

**Files modified:**
- `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs` - 10 tests updated with accurate repros
- `CODEGEN_REGRESSIONS.md` - 10 issue entries updated with detailed analysis

**Total tests:** 40 (30 from Sprints 1-3 + 10 from Sprint 4)

---

## Sprint 5: Issues 41-50

**Summary:** Improved 10 tests for issues #13108, #13100, #12546, #12460, #12416, #12384, #12366, #12139, #12137, #12136

**Issues covered:**
- #13108: Static linking FS2009 warnings (Compile Warning)
- #13100: --platform:x64 sets 32 bit characteristic (Wrong Behavior)
- #12546: Implicit boxing produces extraneous closure (Performance)
- #12460: F# C# Version info values different (Metadata)
- #12416: Optimization inlining inconsistent with piping (Performance)
- #12384: Mutually recursive values initialization wrong (Wrong Behavior)
- #12366: Rethink names for compiler-generated closures (Cosmetic)
- #12139: Improve string null check IL codegen (Performance)
- #12137: Improve analysis to reduce emit of tail (Performance)
- #12136: use fixed does not unpin at end of scope (Wrong Behavior)

**Files modified:**
- `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs` - 10 tests updated with accurate repros
- `CODEGEN_REGRESSIONS.md` - 10 issue entries updated with detailed analysis

**Total tests:** 50 (40 from Sprints 1-4 + 10 from Sprint 5)

---

## Sprint 6: Issues 51-62 + Final Review

**Summary:** Final sprint completing all 62 issues with documentation polish

**Issues covered (12):**
- #11935: unmanaged constraint not recognized by C# (Interop)
- #11556: Better IL output for property/field initializers (Performance)
- #11132: TypeloadException delegate with voidptr parameter (Runtime Error)
- #11114: Record with hundreds of members StackOverflow (Compile Crash)
- #9348: Performance of Comparing and Ordering (Performance)
- #9176: Decorate inline function code with attribute (Feature Request)
- #7861: Missing assembly reference for type in attributes (Compile Error)
- #6750: Mutually recursive values leave fields uninitialized (Wrong Behavior)
- #6379: FS2014 when using tupled args (Compile Warning)
- #5834: Obsolete on abstract generates accessors without specialname (Wrong Behavior)
- #5464: F# ignores custom modifiers modreq/modopt (Interop)
- #878: Serialization of F# exception variants doesn't serialize fields (Wrong Behavior)

**Final Review completed:**
- ✅ All 62 issues have tests in CodeGenRegressions.fs
- ✅ All 62 issues documented in CODEGEN_REGRESSIONS.md
- ✅ Table of Contents added
- ✅ Summary Statistics added (categories, risk levels, fix locations)
- ✅ Consistent formatting verified across all 62 entries
- ✅ Build succeeds with 0 errors

**Files modified:**
- `CODEGEN_REGRESSIONS.md` - Added TOC, summary statistics, risk assessment, fix location breakdown

**Total tests:** 62 (complete)

---

## Sprint 6: Issues 51-62 + Final Review

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 1: Fix
    #878 + #5834 tests

**Summary:** Completed in 3 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Fix #5464 + #11556 tests

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 3: Fix #9176 +
    #12366 + #12137 + #12139

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 4: Add OUT_OF_SCOPE markers

**Summary:** Verified that all 5 feature request tests already have OUT_OF_SCOPE markers from previous work.

**Issues verified:**
- #14392: OpenApi Swashbuckle support - [OUT_OF_SCOPE: Feature Request]
- #13223: FSharp.Build reference assemblies - [OUT_OF_SCOPE: Feature Request]  
- #9176: Inline function attribute - [OUT_OF_SCOPE: Feature Request]
- #15467: Include language version in metadata - [OUT_OF_SCOPE: Feature Request]
- #15092: DebuggerProxies in release builds - [OUT_OF_SCOPE: Feature Request]

**DoD verification:**
- ✅ Build succeeds with 0 errors
- ✅ All 5 feature request tests have [OUT_OF_SCOPE: Feature Request] markers
- ✅ CODEGEN_REGRESSIONS.md summary table shows Feature Request | 5
- ✅ Test file comments explain why each is not a codegen bug

**Files touched:** None (work already completed in Sprint 3)

---
