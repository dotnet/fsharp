# Last Validator Output - Subtask 6

_Updated: 2026-01-29 15:33:12_

● Read tests/fsharp/single-test.fs
  └ 415 lines read

● Grep "net10\.0" (/Users/tomasgrosup/code/RFCs/tiebreakers/impl/fsharp/tests)
  └ 32 lines found

● Now let me check if `single-test.fs` uses the centralized TFM. I see it uses `productTfm` on
  lines 222, 286, 287. Let me verify that `productTfm` is the centralized value from
  TestFramework.

● Glob "tests/fsharp/single-test*.fsproj" in /Users/tomasgrosup/code/RFCs/tiebreakers/impl/fsharp
  └ No matches found

● Grep "productTfm" (/Users/tomasgrosup/code/RFCs/tiebreakers/impl/fsharp/tests/fsharp)
  └ 3 lines found

● Grep "let productTfm" (/Users/tomasgrosup/code/RFCs/tiebreakers/impl/fsharp/tests)
  └ 1 line found

● Good - `single-test.fs` uses `productTfm` from TestFramework. Let me verify the definition in
  TestFramework.fs:

● Read tests/FSharp.Test.Utilities/TestFramework.fs lines 270-290
  └ 26 lines read

● The centralized TFM is correctly read from `eng/productTfm.txt`. Now let me check if there is a
  `single-test-net.fsproj` file to build:

● Glob "**/*.fsproj" in /Users/tomasgrosup/code/RFCs/tiebreakers/impl/fsharp/tests/fsharp
  └ 2 files found

● There's no `single-test-net.fsproj` file, so that DoD item is not applicable. Now let me verify
  the `net10.0` occurrences in `tests/` - I need to check which are baselines/expected output
  files vs functional test code.

● Find net10.0 in test code files
  $ cd /Users/tomasgrosup/code/RFCs/tiebreakers/impl/fsharp && grep -r "net10\.0" tests/
  --include="*.fs" --include="*.fsx" --include="*.fsproj" 2>/dev/null | grep -v "\.bsl" | grep -v
  "baseline" | head -50
   └ 21 lines...

● I see there are still hardcoded `net10.0` references in test project files. Let me categorize
  which are:
  1. Comments (allowed)
  2. Baselines/expected output (allowed)
  3. Functional test code (NOT allowed per DoD)

  Let me analyze:
  - `tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj` - This is a comment
  - `tests/FSharp.Test.Utilities/CompilerAssert.fs` - This is a comment in documentation
  - `tests/FSharp.Compiler.ComponentTests/CompilerCompatibilityTests.fsx` - This is a comment
  - `tests/scripts/identifierAnalysisByType.fsx` - This is a script with hardcoded path
  - `tests/benchmarks/...` - In a notebook

  But the key ones are the `.fsproj` files that have hardcoded TFMs. Let me check if these are
  End-to-End tests or core functional test code:

● List directory tests/EndToEndBuildTests
  └ 14 files found

● Read tests/fsharp/SDKTests/tests/FSharpCoreVersionTest.props
  └ 28 lines read

● Let me check the DoD again:
  - **DoD item 1**: `tests/fsharp/single-test.fs uses centralized TFM` - ✅ PASS (uses
  `productTfm`)
  - **DoD item 2**: `grep for 'net10.0' in tests/ returns only baseline/expected files` - ❌ There
   are hardcoded `net10.0` in project files
  - **DoD item 3**: `dotnet build tests/fsharp/single-test-net.fsproj succeeds (if applicable)` -
  ✅ N/A (file doesn't exist)
  - **DoD item 4**: `No functional test code has hardcoded 'net10.0'` - ❌ There are hardcoded
  refs

  Let me verify - the Sprint scope specifically mentions:
  - Update `tests/fsharp/single-test.fs` to use centralized TFM ✅
  - Search for remaining 'net10.0' references in test files and update them
  - Do NOT update files that are test baselines or expected output

  The hardcoded `net10.0` references in:
  - `tests/EndToEndBuildTests/*/...fsproj` - These are E2E build test projects
  - `tests/AheadOfTime/*/...fsproj` - Ahead of time test projects
  - `tests/fsharp/SDKTests/tests/FSharpCoreVersionTest.props` - SDK test props

  Let me check if these are part of the expected scope or if they can be considered outside the
  main test infrastructure:

● Read tests/ILVerify/ilverify.ps1 lines 45-60
  └ 20 lines read

● The `ilverify.ps1` has a hardcoded `net10.0` at line 50, mixing with `$default_tfm`. This is
  inconsistent - it should use `$default_tfm` consistently or reference the centralized TFM.

  Based on the DoD criteria:
  - "grep for 'net10.0' in tests/ returns only baseline/expected files" - **FAILED** - there are
  hardcoded `net10.0` in functional scripts/projects
  - "No functional test code has hardcoded 'net10.0'" - **FAILED** - `ilverify.ps1` line 50, many
  `.fsproj` files

  **VERIFY_FAILED**

  The following issues need to be addressed:

  1. **`tests/ILVerify/ilverify.ps1` line 50**: Has hardcoded `net10.0` instead of using
  `$default_tfm`

  2. **Multiple EndToEndBuildTests `.fsproj` files**: Have hardcoded `net10.0` TargetFramework(s)
     - `tests/EndToEndBuildTests/BasicProvider/*.fsproj`
     - `tests/EndToEndBuildTests/ComboProvider/*.fsproj`
     - `tests/EndToEndBuildTests/DesignTimeProviderPackaging/*.fsproj`

  3. **AheadOfTime test projects** with hardcoded paths:
     - `tests/AheadOfTime/Equality/Equality.fsproj`
     - `tests/AheadOfTime/Trimming/*/*.fsproj`

  4. **`tests/fsharp/SDKTests/tests/FSharpCoreVersionTest.props`** line 6-7: Has hardcoded
  `net10.0`

  5. **`tests/scripts/identifierAnalysisByType.fsx`** line 5: Has hardcoded path with `net10.0`

