﻿namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<UseCulture("en-US")>]
module HatDesugaringTests =

    [<Fact>]
    let ``Hat operator should be overloadable in infix context``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
module X
open System

let (^) (x: int) (y: int) = x + y

if 1 ^ 2 <> 3 then failwithf "expected result to be 3 but got %i" (1 ^ 2)
Console.WriteLine()
            """)
    
    [<Fact>]
    let ``Reverse slicing should work with overloaded infix hat``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
module X
open System

let (^) (x: int) (y: int) = x + y
let result = [1;2][^1..]
if result <> [1;2] then failwithf "expected result to be [1;2] but got %A" result
Console.WriteLine()
            """)

    [<Fact>]
    let ``At operator should not be usable in prefix context``() =
        CompilerAssert.ParseWithErrors
           ("""
module X

let x = @1
            """, langVersion="preview")
            [|
                FSharpDiagnosticSeverity.Error, 1208, (4,9,4,10), "Invalid prefix operator"
            |]

    [<Fact>]
    let ``Hat operator should not be overloadable as prefix operator``() = 
        CompilerAssert.ParseWithErrors
            """
module X
open System

let (~^) (x: int) (y:int) = x + y
            """
            [|
                FSharpDiagnosticSeverity.Error, 1208, (5,6,5,8), "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.";
            |]

    [<Fact>]
    let ``Reverse slicing should not work with at symbol in 1st slice index``() =
        CompilerAssert.ParseWithErrors
           ("""
module X
open System

let list = [1;2;3]
Console.WriteLine(list[@1..])
            """, langVersion="preview")
            [|
                FSharpDiagnosticSeverity.Error, 1208, (6,24,6,25), "Invalid prefix operator"
            |]

    [<Fact>]
    let ``Reverse slicing should not work with at symbol in 2nd slice index``() =
        CompilerAssert.ParseWithErrors
           ("""
module X
open System

let list = [1;2;3]
Console.WriteLine(list[..@1])
            """, langVersion="preview")
            [|
                FSharpDiagnosticSeverity.Error, 1208, (6,24,6,27), "Invalid prefix operator"
            |]

    [<Fact>]
    let ``Reverse slicing should not work with at symbol in both slice index``() =
        CompilerAssert.ParseWithErrors
           ("""
module X
open System

let list = [1;2;3]
Console.WriteLine(list[@1..@1])
            """, langVersion="preview")
            [|
                FSharpDiagnosticSeverity.Error, 1208, (6,24,6,25), "Invalid prefix operator";
                FSharpDiagnosticSeverity.Error, 1208, (6,28,6,29), "Invalid prefix operator"
            |]

    [<Fact>]
    let ``Reverse indexing should not work with at symbol``() =
        CompilerAssert.ParseWithErrors
           ("""
module X
open System

let list = [1;2;3]
Console.WriteLine(list[@11])
            """, langVersion="preview")
            [|
                FSharpDiagnosticSeverity.Error, 1208, (6,24,6,25), "Invalid prefix operator"
            |]
