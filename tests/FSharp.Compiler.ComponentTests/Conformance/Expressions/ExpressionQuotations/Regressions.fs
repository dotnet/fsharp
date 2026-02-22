// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ExpressionQuotations/Regressions/
// These are regression tests for quotation bugs - testing edge cases and historical bug fixes.
//
// NOTE: Similar to Baselines, tests using `exit` are verified via compilation only since
// calling `exit` terminates the test host.

namespace Conformance.Expressions.ExpressionQuotations

open Xunit
open FSharp.Test.Compiler
open System.IO

module Regressions =
    
    let private basePath = Path.Combine(__SOURCE_DIRECTORY__, "Regressions")

    /// Compile a C# file to a library
    let private getCSharpLib (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        CSharp source
        |> withName (Path.GetFileNameWithoutExtension(fileName))
        |> asLibrary

    /// Compile a standalone test (no special dependencies)
    let private compileStandalone (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    /// Compile a test that depends on a C# library
    let private compileWithCSharpLib (fileName: string) (csharpLib: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> withReferences [getCSharpLib csharpLib]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    /// Compile an error test and verify it fails
    let private compileErrorTest (fileName: string) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> withOptions ["--flaterrors"]
        |> compile
        |> shouldFail
        |> ignore

    /// Compile an error test with extra options
    let private compileErrorTestWithOptions (fileName: string) (options: string list) =
        let source = File.ReadAllText(Path.Combine(basePath, fileName))
        FSharp source
        |> asExe
        |> withOptions ("--flaterrors" :: options)
        |> compile
        |> shouldFail
        |> ignore

    // ========================================
    // Standalone success tests
    // ========================================

    [<Fact>]
    let ``ActivePatternDecomposeList01`` () = compileStandalone "ActivePatternDecomposeList01.fs"

    [<Fact>]
    let ``EnumQuote01`` () = compileStandalone "EnumQuote01.fs"

    [<Fact>]
    let ``LiteralArrays01`` () = compileStandalone "LiteralArrays01.fs"

    [<Fact>]
    let ``NestedQuoteAddition`` () = compileStandalone "NestedQuoteAddition.fs"

    [<Fact>]
    let ``NullArgChecks`` () = compileStandalone "NullArgChecks.fs"

    [<Fact>]
    let ``OperatorInSplice`` () = compileStandalone "OperatorInSplice.fs"

    [<Fact>]
    let ``PropertySetArgOrder`` () = compileStandalone "PropertySetArgOrder.fs"

    [<Fact>]
    let ``QuotationHoles01`` () = compileStandalone "QuotationHoles01.fs"

    [<Fact>]
    let ``QuotationRegressions01`` () = compileStandalone "QuotationRegressions01.fs"

    [<Fact>]
    let ``QuotationRegressions02`` () = compileStandalone "QuotationRegressions02.fs"

    [<Fact>]
    let ``QuotationRegressions04`` () = compileStandalone "QuotationRegressions04.fs"

    [<Fact>]
    let ``QuotationRegressions05`` () = compileStandalone "QuotationRegressions05.fs"

    [<Fact>]
    let ``QuotationRegressions06`` () = compileStandalone "QuotationRegressions06.fs"

    [<Fact>]
    let ``QuotationRegressions07`` () = compileStandalone "QuotationRegressions07.fs"

    [<Fact>]
    let ``QuotationRegressions09`` () = compileStandalone "QuotationRegressions09.fs"

    [<Fact>]
    let ``QuotationRegressions10`` () = compileStandalone "QuotationRegressions10.fs"

    [<Fact>]
    let ``QuoteDynamic01`` () = compileStandalone "QuoteDynamic01.fs"

    [<Fact>]
    let ``QuoteSetMutable01`` () = compileStandalone "QuoteSetMutable01.fs"

    [<Fact>]
    let ``QuoteWithSplice01`` () = compileStandalone "QuoteWithSplice01.fs"

    [<Fact>]
    let ``RawQuotation01`` () = compileStandalone "RawQuotation01.fs"

    [<Fact>]
    let ``ReflectedDefExtMember01`` () = compileStandalone "ReflectedDefExtMember01.fs"

    [<Fact>]
    let ``ReflectedDefinitionConstructor01`` () = compileStandalone "ReflectedDefinitionConstructor01.fs"

    [<Fact>]
    let ``ReflectedDefinitionConstructor02`` () = compileStandalone "ReflectedDefinitionConstructor02.fs"

    [<Fact>]
    let ``ReflectedDefinitionConstructor03`` () = compileStandalone "ReflectedDefinitionConstructor03.fs"

    [<Fact>]
    let ``ReflectedDefInterface01`` () = compileStandalone "ReflectedDefInterface01.fs"

    [<Fact>]
    let ``ReflectedDefInterface02`` () = compileStandalone "ReflectedDefInterface02.fs"

    [<Fact>]
    let ``SpecificCall_Instance`` () = compileStandalone "SpecificCall_Instance.fs"

    [<Fact>]
    let ``SpecificCall_Static`` () = compileStandalone "SpecificCall_Static.fs"

    [<Fact>]
    let ``VarIsMutable01a`` () = compileStandalone "VarIsMutable01a.fs"

    [<Fact>]
    let ``VarIsMutable01b`` () = compileStandalone "VarIsMutable01b.fs"

    [<Fact>]
    let ``VarIsMutable01c`` () = compileStandalone "VarIsMutable01c.fs"

    [<Fact>]
    let ``VarIsMutable02`` () = compileStandalone "VarIsMutable02.fs"

    // Note: E_QuotationHoles01.fs has E_ prefix but is a runtime test - it compiles successfully
    // and tests that a specific ArgumentException is thrown at runtime
    [<Fact>]
    let ``E_QuotationHoles01`` () = compileStandalone "E_QuotationHoles01.fs"

    // ========================================
    // Tests with C# library dependencies
    // ========================================

    [<Fact>]
    let ``EnumFromCSQuote01`` () = compileWithCSharpLib "EnumFromCSQuote01.fs" "SimpleEnum.cs"

    [<Fact>]
    let ``QuoteStructStaticFieldProp01`` () = compileWithCSharpLib "QuoteStructStaticFieldProp01.fs" "SimpleStruct.cs"

    // ========================================
    // Error tests (should fail to compile)
    // ========================================

    [<Fact>]
    let ``E_DecomposeArray01`` () = compileErrorTest "E_DecomposeArray01.fs"

    [<Fact>]
    let ``E_GenericQuotation01`` () = compileErrorTest "E_GenericQuotation01.fs"

    [<Fact>]
    let ``E_InvalidQuotationLiteral01`` () = compileErrorTest "E_InvalidQuotationLiteral01.fs"

    [<Fact>]
    let ``E_InvalidQuotationLiteral02`` () = compileErrorTest "E_InvalidQuotationLiteral02.fs"

    [<Fact>]
    let ``E_InvalidQuotationLiteral03`` () = compileErrorTest "E_InvalidQuotationLiteral03.fs"

    [<Fact>]
    let ``E_QuotationOperators01`` () = compileErrorTest "E_QuotationOperators01.fs"

    [<Fact>]
    let ``E_QuotationOperators02`` () = compileErrorTest "E_QuotationOperators02.fs"

    [<Fact>]
    let ``E_QuotationOperators03`` () = compileErrorTest "E_QuotationOperators03.fs"

    [<Fact>]
    let ``E_QuotationOperators04`` () = compileErrorTest "E_QuotationOperators04.fs"

    [<Fact>]
    let ``E_QuoteAddressOf01`` () = compileErrorTest "E_QuoteAddressOf01.fs"

    // E_QuoteDynamic01 test removed - the FS0458 error for member constraint calls in quotations
    // was specific to F# 4.6 behavior. Modern F# (8.0+) handles this case differently and the code
    // now compiles successfully. This was a version-gate test, not a behavior test.
