# Claims Coverage Report for PR #19297

## Findings

### Implemented Claims
1.  **Fixes #19296 - State machines: low-level resumable code not always expanded correctly**
    -   **Location**: `src/Compiler/Optimize/LowerStateMachines.fs:178` (`BindResumableCodeDefinitions`), `tests/FSharp.Compiler.ComponentTests/Language/StateMachineTests.fs:57`
    -   **Description**: Implemented logic to eliminate `IfUseResumableStateMachinesExpr` by taking the static branch and recursively binding resumable code definitions, including looking through debug points. Verified by `Nested __useResumableCode is expanded correctly` test case.
    -   **Severity**: **None (Verified)**

2.  **Fixes #12839 - Unexpected FS3511 warning for big records in tasks**
    -   **Location**: `src/Compiler/Optimize/LowerStateMachines.fs:242` (`TryReduceApp`), `tests/FSharp.Compiler.ComponentTests/Language/StateMachineTests.fs:274`
    -   **Description**: Enhanced `TryReduceApp` to better track and resolve resumable code variables and push applications through, enabling static compilation for complex constructs (like big records and nested matches) that previously fell back to dynamic code (triggering FS3511). Verified by `Big record` test case.
    -   **Severity**: **None (Verified)**

3.  **Includes test cases from #14930**
    -   **Location**: `tests/FSharp.Compiler.ComponentTests/Language/StateMachineTests.fs:225`
    -   **Description**: Added `Task with for loop over tuples compiles statically` test case, which was the repro for #14930. The code changes in `LowerStateMachines.fs` support this scenario.
    -   **Severity**: **None (Verified)**

4.  **Issue #13404 - Referenced in NestedTaskFailures.fs**
    -   **Location**: `tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Control/NestedTaskFailures.fs`
    -   **Description**: Removed `#nowarn "3511"` from the file, indicating that the nested task failures are now statically compiled correctly and don't trigger the warning/error.
    -   **Severity**: **None (Verified)**

### Orphan Claims
- None found. All claims in the PR description and linked issues have corresponding code changes and tests.

### Partial Implementations
- None found. The implementations appear to fully address the described issues based on the provided repro cases.

### Orphan Changes
- None found. All code changes in `LowerStateMachines.fs` are directly related to improving the static compilation of state machines as described in the PR.

## Summary
The PR covers all its claims with appropriate code changes and regression tests. The changes in `LowerStateMachines.fs` logically address the issues of incorrect expansion and fallback to dynamic code. The added tests cover the reported reproduction scenarios.
