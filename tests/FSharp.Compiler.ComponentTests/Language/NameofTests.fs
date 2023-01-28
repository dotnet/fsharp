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


    [<Fact>]
    let ``nameof works for pattern matching of DU case names`` () =  
        let source = """
/// Simplified version of EventStore's API
type RecordedEvent = { EventType: string; Data: string }

/// My concrete type:
type MyEvent =
    | A of string
    | B of string

let deserialize (e: RecordedEvent) : MyEvent =
    printfn "EventType is '%s'" e.EventType
    printfn "Nameof A is '%s'" (nameof A)
    printfn "Nameof B is '%s'" (nameof B)
    match e.EventType with
    | nameof A -> A e.Data
    | nameof B -> B e.Data
    | t -> failwithf "Invalid EventType: '%s'" t

let getData event =
    match event with
    | A amsg -> amsg
    | B bmsg -> bmsg

let re1 = { EventType = nameof A; Data = "hello" }
let re2 = { EventType = nameof B; Data = "world" }

let a = deserialize re1
let b = deserialize re2

if not((getData a) = re1.Data) then
    failwith $"Record1 mismatch;; {getData a} <> {re1.Data}"

if not((getData b) = re2.Data) then
    failwith $"Record1 mismatch;; {getData b} <> {re2.Data}"
        """
        Fsx source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``nameof works for modules`` () =
        let source = """
let actual = nameof(FSharp.Core.LanguagePrimitives)
let expected = "LanguagePrimitives"
if actual <> expected then failwith $"Expected type to be '{expected}', but got '{actual}'"
        """
        Fsx source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed
