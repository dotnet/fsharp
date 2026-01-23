## Subtask 2 - Implement iteration 1 (2026-01-23 17:57:42)
-    **VERIFY_FAILED**

   The task is **incomplete**. According to VISION.md:
   - The goal is to document **all 62 open `Area-Compiler-CodeGen` bugs**
   - Organized in 6 sprints of ~10 issues each

   **Current Status:**
   - ✅ Test folder created: `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/`
   - ✅ Test file created: `CodeGenRegressions.fs` with proper structure
   - ✅ Documentation created: `CODEGEN_REGRESSIONS.md` with proper format

## Subtask 3 - Implement iteration 1 (2026-01-23 18:10:03)
-    **VERIFY_FAILED**

   The task is incomplete. According to VISION.md, the goal is to document all 62 open
   `Area-Compiler-CodeGen` bugs, but currently only 30 of 62 issues have been implemented:

   - **Tests created:** 30 of 62 (48%)
   - **Documentation entries:** 30 of 62 (48%)
   - **Missing:** Issues #14707, #14706, #14508, #14492, #14392, #14321, #13468, #13447, #13223,
   #13218, #13108, #13100, #12546, #12460, #12416, #12384, #12366, #12139, #12137, #12136, #11935,
   #11556, #11132, #11114, #9348, #9176, #7861, #6750, #6379, #5834, #5464, #878

## Subtask 3 - Implement iteration 2 (2026-01-23 18:12:38)
-    VERIFY_FAILED

   **Issue:** Only 30 of 62 required issues are documented. The following 32 issues from VISION.md
   are missing:
   - #14707, #14706, #14508, #14492, #14392, #14321, #13468, #13447, #13223, #13218
   - #13108, #13100, #12546, #12460, #12416, #12384, #12366, #12139, #12137, #12136
   - #11935, #11556, #11132, #11114, #9348, #9176, #7861, #6750, #6379, #5834, #5464, #878

   Sprints 4-6 as outlined in VISION.md need to be completed.


## Subtask 1 - Implement iteration 1 (2026-01-23 19:47:59)
-    **VERIFY_FAILED**

   The task is incomplete per the Final Sprint Focus criteria in VISION.md:

   1. **OUT_OF_SCOPE markers missing** - Test file has 0 `OUT_OF_SCOPE` markers. Issues #14392,
   #13223, #9176, #15467, #15092 should be marked.

   2. **3 stub tests remain unfixed**:
      - `#5464` - Still just `let f x = x + 1` with no IL check for modreq/modopt
      - `#9176` - Still just `let inline f x = x + 1` with no marker/explanation

## Subtask 1 - Implement iteration 1 (2026-01-23 21:04:03)
-    **VERIFY_FAILED**

   The task is not complete. According to VISION.md, all 62 codegen regression tests should have
   their `[<Fact>]` attributes uncommented after being fixed. Currently:
   - 62 tests remain with `// [<Fact>]` (commented out)
   - Only 1 test has `[<Fact>]` uncommented

   No progress has been made on fixing the codegen bugs - the task is at the starting point as
   described in "Current State" section of VISION.md.


## Subtask 1 - Review iteration 1 (2026-01-23 21:11:42)
-    **VERIFY_FAILED**

   The task is **not complete**. According to the VISION.md, the goal is to fix all 62 documented
   codegen bugs. Currently:
   - Only **1 test** (Issue #18319) has been fixed
   - **61 tests** remain to be fixed (the document mentions 62 original tests with 1 fixed = 61
   remaining, but there appear to be 62 still commented meaning there may be 63 total tests)

   The campaign is approximately **1.6% complete** (1 of 62 issues fixed). The vast majority of the
    work remains to be done.

