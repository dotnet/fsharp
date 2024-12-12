// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SymbolicOperators =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"GreaterThanDotParen01.fs"|])>]
    let ``SymbolicOperators - GreaterThanDotParen01_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="error" span="(13,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_GreaterThanDotParen01.fs"|])>]
    let ``SymbolicOperators - E_GreaterThanDotParen01_fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="error" id="FS0670">This code is not sufficiently generic\. The type variable  \^T when  \^T : \(static member \( \+ \) :  \^T \*  \^T ->  \^a\) could not be generalized because it would escape its scope</Expects>
    [<InlineData(true)>]            // RealSig
    [<InlineData(false)>]           // Regular
    [<Theory>]
    let ``SymbolicOperators_E_LessThanDotOpenParen001_fs`` (realsig) =
        Fsx """

type public TestType<'T,'S>() =
    
    member public s.Value with get() = Unchecked.defaultof<'T>
    static member public (+++) (a : TestType<'T,'S>, b : TestType<'T,'S>) = a.Value
    static member public (+++) (a : TestType<'T,'S>, b : 'T) = b
    static member public (+++) (a : 'T, b : TestType<'T,'S>) = a
    static member public (+++) (a : TestType<'T,'S>, b : 'T -> 'S) = a.Value
    static member public (+++) (a : 'S -> 'T, b : TestType<'T,'S>) = (a 17) + b.Value

let inline (+++) (a : ^a) (b : ^b) = ((^a or ^b): (static member (+++): ^a * ^b -> ^c) (a,b) )

let tt0 = TestType<int, string>()
let tt1 = TestType<int, string>()

let f (x : string) = 18

let a0 = tt0 +++ tt1
let a1 = tt0 +++ 11
let a2 =  12 +++ tt1
let a3 = tt0 +++ (fun x -> "18")
let a4 = f +++ tt0

let a5 = TestType<int, string>.(+++)(f, tt0)
let a6 = TestType<int, string>.(+++)((fun (x : string) -> 18), tt0)"""
        |> withOptions ["--flaterrors"]
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withErrorCode 0670
        |> withDiagnosticMessageMatches " 'a\) could not be generalized because it would escape its scope"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"GreaterThanColon001.fs"|])>]
    let ``SymbolicOperators - GreaterThanColon001_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_GreaterThanColon002.fs"|])>]
    let ``SymbolicOperators - E_GreaterThanColon002_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

