// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module SymbolicOperators =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"GreaterThanDotParen01.fs"|])>]
    let ``SymbolicOperators - GreaterThanDotParen01.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="error" span="(13,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_GreaterThanDotParen01.fs"|])>]
    let ``SymbolicOperators - E_GreaterThanDotParen01.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="error" id="FS0670">This code is not sufficiently generic\. The type variable  \^T when  \^T : \(static member \( \+ \) :  \^T \*  \^T ->  \^a\) could not be generalized because it would escape its scope</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_LessThanDotOpenParen001.fs"|])>]
    let ``SymbolicOperators - E_LessThanDotOpenParen001.fs - --flaterrors`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0670
        |> withDiagnosticMessageMatches "  \^a\) could not be generalized because it would escape its scope"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"GreaterThanColon001.fs"|])>]
    let ``SymbolicOperators - GreaterThanColon001.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_GreaterThanColon002.fs"|])>]
    let ``SymbolicOperators - E_GreaterThanColon002.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

