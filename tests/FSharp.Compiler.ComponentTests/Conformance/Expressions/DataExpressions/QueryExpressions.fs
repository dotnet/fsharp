// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/Expressions/DataExpressions/QueryExpressions/
// These tests verify F# query expressions (`query { ... }`).
// Utils.fs provides shared utilities (Product, Customer types) for the Linq101 tests.
// Customers.xml provides test data loaded by Utils.fs.
//
// Test patterns from env.lst:
// - Utils.fs: Compiled as library (-a -r:System.Xml.Linq)
// - Linq101* tests: Reference Utils.dll
// - E_* tests: Error tests that should fail to compile
// - Other tests: Standalone success tests
//
// NOTE: The original tests used `exit` to return success/failure codes. Since the ComponentTests
// framework runs tests in-process, calling `exit` terminates the test host. These tests are
// currently set up to verify compilation only. Full runtime verification would require either:
// 1. Modifying test files to throw exceptions instead of calling exit
// 2. Running tests in a separate process (slower)
// 3. Using FSI to execute tests

namespace Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Compiler
open System.IO

module QueryExpressions =
    
    let private basePath = Path.Combine(__SOURCE_DIRECTORY__, "QueryExpressions")
    
    /// Create Utils.fs as a library CompilationUnit
    let private getUtilsLib () =
        let source = File.ReadAllText(Path.Combine(basePath, "Utils.fs"))
        FSharp source
        |> withName "Utils"
        |> asLibrary
    
    /// Compile a test that depends on Utils.dll (compile-only, no run)
    let private compileWithUtils (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> withReferences [getUtilsLib()]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    /// Compile a standalone test (compile-only, no run)
    let private compileStandalone (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    /// Compile a standalone test as library (compile-only, no run) 
    let private compileAsLibrary (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asLibrary
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed
        |> ignore

    /// Compile an error test and verify it fails
    let private compileErrorTest (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> ignore

    /// Compile an error test that references Utils.dll
    let private compileErrorTestWithUtils (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> withReferences [getUtilsLib()]
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> ignore

    // ========================================
    // Linq101 tests (require Utils.dll)
    // ========================================

    [<Fact>]
    let ``Linq101Aggregates01`` () = compileWithUtils "Linq101Aggregates01.fs"

    [<Fact>]
    let ``Linq101ElementOperators01`` () = compileWithUtils "Linq101ElementOperators01.fs"

    [<Fact>]
    let ``Linq101Grouping01`` () = compileWithUtils "Linq101Grouping01.fs"

    [<Fact>]
    let ``Linq101Joins01`` () = compileWithUtils "Linq101Joins01.fs"

    [<Fact>]
    let ``Linq101Ordering01`` () = compileWithUtils "Linq101Ordering01.fs"

    [<Fact>]
    let ``Linq101Partitioning01`` () = compileWithUtils "Linq101Partitioning01.fs"

    [<Fact>]
    let ``Linq101Quantifiers01`` () = compileWithUtils "Linq101Quantifiers01.fs"

    [<Fact>]
    let ``Linq101Select01`` () = compileWithUtils "Linq101Select01.fs"

    [<Fact>]
    let ``Linq101SetOperators01`` () = compileWithUtils "Linq101SetOperators01.fs"

    [<Fact>]
    let ``Linq101Where01`` () = compileWithUtils "Linq101Where01.fs"

    // ========================================
    // Standalone success tests (compile-only due to exit calls)
    // ========================================

    [<Fact>]
    let ``ForWhereJoin01`` () = compileStandalone "ForWhereJoin01.fs"

    [<Fact>]
    let ``FunctionAsTopLevelLet01`` () = compileStandalone "FunctionAsTopLevelLet01.fs"

    [<Fact>]
    let ``FunctionsDefinedOutsideQuery01`` () = compileStandalone "FunctionsDefinedOutsideQuery01.fs"

    [<Fact>]
    let ``FunctionWithinTopLevelLet01`` () = compileStandalone "FunctionWithinTopLevelLet01.fs"

    [<Fact>]
    let ``JoinsWithInterveningExpressions01`` () = compileStandalone "JoinsWithInterveningExpressions01.fs"

    [<Fact>]
    let ``MatchInQuery01`` () = compileStandalone "MatchInQuery01.fs"

    [<Fact>]
    let ``MatchInQuery02`` () = compileStandalone "MatchInQuery02.fs"

    [<Fact>]
    let ``OperatorsOverClass01`` () = compileStandalone "OperatorsOverClass01.fs"

    [<Fact>]
    let ``OperatorsOverRecords01`` () = compileStandalone "OperatorsOverRecords01.fs"

    [<Fact>]
    let ``OperatorsOverTuples01`` () = compileStandalone "OperatorsOverTuples01.fs"

    [<Fact>]
    let ``WhereRequiresParens01`` () = compileStandalone "WhereRequiresParens01.fs"

    [<Fact>]
    let ``YieldOrSelect01`` () = compileStandalone "YieldOrSelect01.fs"

    // ========================================
    // Compile-only success tests (library, no run)
    // ========================================

    [<Fact>]
    let ``UppercaseIdentifier04`` () = compileAsLibrary "UppercaseIdentifier04.fs"

    // ========================================
    // Error tests (should fail to compile)
    // ========================================

    [<Fact>]
    let ``E_BadGroupValBy01`` () = compileErrorTest "E_BadGroupValBy01.fs"

    [<Fact>]
    let ``E_BadGroupValBy02`` () = compileErrorTest "E_BadGroupValBy02.fs"

    [<Fact>]
    let ``E_CustomOperatorAsIllegalIdentifier01`` () = compileErrorTest "E_CustomOperatorAsIllegalIdentifier01.fs"

    [<Fact>]
    let ``E_FirstOrDefaultWithNulls01`` () = compileErrorTestWithUtils "E_FirstOrDefaultWithNulls01.fs"

    [<Fact>]
    let ``E_FunctionAsTopLevelLet01`` () = compileErrorTest "E_FunctionAsTopLevelLet01.fs"

    [<Fact>]
    let ``E_FunctionAsTopLevelLet02`` () = compileErrorTest "E_FunctionAsTopLevelLet02.fs"

    [<Fact>]
    let ``E_FunctionWithinTopLevelLet01`` () = compileErrorTest "E_FunctionWithinTopLevelLet01.fs"

    [<Fact>]
    let ``E_MatchInQuery01`` () = compileErrorTest "E_MatchInQuery01.fs"

    [<Fact>]
    let ``E_MatchInQuery02`` () = compileErrorTest "E_MatchInQuery02.fs"

    [<Fact>]
    let ``E_MismatchedConditionalBranches01`` () = compileErrorTest "E_MismatchedConditionalBranches01.fs"

    [<Fact>]
    let ``E_MismatchedConditionalBranches02`` () = compileErrorTest "E_MismatchedConditionalBranches02.fs"

    [<Fact>]
    let ``E_Sequential01`` () = compileErrorTest "E_Sequential01.fs"

    [<Fact>]
    let ``E_WhereRequiresParens01`` () = compileErrorTest "E_WhereRequiresParens01.fs"

    [<Fact>]
    let ``E_WhitespaceErrors01`` () = compileErrorTest "E_WhitespaceErrors01.fs"

    [<Fact>]
    let ``E_WhitespaceErrors02`` () = compileErrorTest "E_WhitespaceErrors02.fs"

    [<Fact>]
    let ``E_WhitespaceErrors03`` () = compileErrorTest "E_WhitespaceErrors03.fs"

    [<Fact>]
    let ``E_Zip01`` () = compileErrorTest "E_Zip01.fs"
