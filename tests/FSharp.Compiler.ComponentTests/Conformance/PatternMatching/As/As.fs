// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module As =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/As)
    [<Theory; FileInlineData("asPattern01.fs")>]
    let ``Simple - asPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/As)
    [<Theory; FileInlineData("asPattern02.fs")>]
    let ``Simple - asPattern02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``As patterns``() =
        Fsx """
        let (|Id|) = id
        let a = [1..4]
        match a with
        | 1 | 1 as b::(Id 2 as c as c2)::[d as 3; Id e & Id _ as Id 4] as Id f when b = 1 && c = 2 && c2 = 2 && d = 3 && e = 4 && a = f -> ()
        | _ -> failwith "Match failed"
        """
        |> asExe
        |> withLangVersion60
        |> compileExeAndRun
        |> shouldSucceed
        
    [<Theory>]
    [<InlineData("DateTime", "DateTime.Now")>]
    [<InlineData("int", "1")>]
    [<InlineData("Guid", "(Guid.NewGuid())")>]
    [<InlineData("Byte", "0x1")>]
    [<InlineData("Decimal", "1m")>]
    let ``Test type matching for subtypes and interfaces`` typ value =
        Fsx $"""
open System
let classify (o: obj) =
    match o with
    | :? {typ} as d when d = Unchecked.defaultof<_> -> "default"
    | :? IFormattable -> "formattable"
    | _ -> "not a {typ}"

let res = classify {value}
if res <> "formattable" then
    failwith $"Unexpected result: {{res}}"
         """
         |> asExe
         |> compileAndRun
         |> shouldSucceed