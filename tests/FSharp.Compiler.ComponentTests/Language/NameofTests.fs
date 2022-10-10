// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module NameofTests =

    [<Theory>]
    [<InlineData("+")>]
    [<InlineData("-")>]
    [<InlineData("/")>]
    [<InlineData("*")>]
    [<InlineData(".. ..")>]
    let ``nameof() with operator should return demangled name`` operator =
        let source = $"""
let expected = "{operator}"
let actual = nameof({operator})
if actual <> expected then failwith $"Expected nameof({{expected}}) to be '{{expected}}', but got '{{actual}}'"
        """
        Fsx source
        |> asExe
        |> withLangVersion50
        |> compileAndRun
        |> shouldSucceed
        
    [<Fact>]
    let ``nameof() with member of a generic type should return the name without types provided`` () =    
        let source = $"""
type A<'a>() =
    static member P = ()

let expected = "P"
let actual = nameof(A.P)
if actual <> expected then failwith $"Expected nameof({{expected}}) to be '{{expected}}', but got '{{actual}}'"
        """
        Fsx source
        |> asExe
        |> withLangVersion50
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed
        
    [<Theory>]
    [<InlineData(1)>]
    [<InlineData(2)>]
    [<InlineData(3)>]
    let ``nameof() with member of a generic type should return the name`` numberOfGenericParameters =    
        let source = $"""
type A<{(seq {for i in 1 .. numberOfGenericParameters -> "'a" + string i } |> String.concat ", ")}>() =
    static member P = ()

let expected = "P"
let actual = nameof(A<{(seq {for _ in 1 .. numberOfGenericParameters -> "_" } |> String.concat ", ")}>.P)
if actual <> expected then failwith $"Expected nameof({{expected}}) to be '{{expected}}', but got '{{actual}}'"
        """
        Fsx source
        |> asExe
        |> withLangVersion50
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``nameof() in a pattern should return the correct type`` () =    
        let source = $"""
open Microsoft.FSharp.Reflection
let f x = match x with nameof x -> true | _ -> false

let expected = "System.String -> System.Boolean"
let elms s = if FSharpType.IsFunction s then
               let domain, range = FSharpType.GetFunctionElements s
               $"{{domain}} -> {{range}}"
             else
               ""
let fType = f.GetType()
let actual = elms fType
if actual <> expected then failwith $"Expected type to be '{{expected}}', but got '{{actual}}'"
        """
        Fsx source
        |> asExe
        |> withLangVersion50
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed
