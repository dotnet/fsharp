// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler

module CustomAttributes_CallerArgumentExpression =

    [<Fact>]
    let ``Can consume CallerArgumentExpression in BCL methods`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
try System.ArgumentNullException.ThrowIfNullOrWhiteSpace(Seq.init 50 (fun _ -> " ")
  (* comment *) 
  |> String.concat " ")
with :? System.ArgumentException as ex -> 
  assertEqual true (ex.Message.Contains("(Parameter 'Seq.init 50 (fun _ -> \" \")\n  (* comment *) \n  |> String.concat \" \""))
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Can define in F#`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type A() =
  static member aa (
    a,
    [<CallerMemberName; Optional; DefaultParameterValue "no value">]b: string, 
    [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
    [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
    a,b,c,e

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 13, "stringABC")
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Can define in F# with F#-style optional arguments`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
type A() =
  static member aa (
    a,
    [<CallerMemberName>] ?b: string, 
    [<CallerLineNumber>] ?c: int, 
    [<CallerArgumentExpressionAttribute("a")>] ?e: string) = 
    let b = defaultArg b "no value"
    let c = defaultArg c 0
    let e = defaultArg e "no value"
    a,b,c,e

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 15, "stringABC")
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Can define in F# - with #line`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
# 1 "C:\\Program.fs"
type A() =
  static member aa (
    a,
    [<CallerMemberName; Optional; DefaultParameterValue "no value">]b: string, 
    [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
    [<CallerFilePath; Optional; DefaultParameterValue "no value">]d: string, 
    [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
    a,b,c,d,e

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 11, "C:\\Program.fs", "stringABC")
# 1 "C:\\Program.fs"
assertEqual (A.aa(stringABC : string)) ("abc", ".cctor", 1, "C:\\Program.fs", "stringABC : string")
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

