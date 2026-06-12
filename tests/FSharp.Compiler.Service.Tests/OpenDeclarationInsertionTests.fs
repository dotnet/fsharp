module FSharp.Compiler.Service.Tests.OpenDeclarationInsertionTests

open Xunit
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Service.Tests.Common

let private findOpenInsertionPoint (fileName: string) (source: string) (entity: string) : pos =
    let parseTree = parseSourceCode (fileName, source)

    let lines = source.Replace("\r\n", "\n").Split('\n')

    let currentLine =
        let mutable i = lines.Length - 1
        while i > 0 && System.String.IsNullOrWhiteSpace(lines.[i]) do
            i <- i - 1
        i + 1

    let idents = entity.Split('.')

    let ctx =
        ParsedInput.FindNearestPointToInsertOpenDeclaration
            currentLine
            parseTree
            idents
            OpenStatementInsertionPoint.TopLevel

    ctx.Pos

[<Fact>]
let ``Open placed after r directives in fsx`` () =
    let source = """#r "nuget: Newtonsoft.Json"
#r "nuget: FSharp.Data"

let x : Newtonsoft.Json.JsonConvert = failwith ""
"""
    let insertionPoint = findOpenInsertionPoint "test.fsx" source "Newtonsoft.Json"
    // Must be after line 2 (#r directives), not before
    Assert.Equal(3, insertionPoint.Line)

[<Fact>]  // NEGATIVE: .fs files unaffected
let ``Open in fs file placed at top`` () =
    let source = """module Test
let x = System.IO.File.ReadAllText "a"
"""
    let insertionPoint = findOpenInsertionPoint "test.fs" source "System.IO"
    Assert.Equal(2, insertionPoint.Line)  // right after module decl

[<Fact>]
let ``Open after r and load directives`` () =
    let source = """#r "nuget: FSharp.Data"
#load "helper.fsx"

let x = 1
"""
    let insertionPoint = findOpenInsertionPoint "test.fsx" source "SomeNs"
    Assert.Equal(3, insertionPoint.Line)

[<Fact>]
let ``No r directives in fsx inserts at top`` () =
    let source = """let x = System.IO.File.ReadAllText "a"
"""
    let insertionPoint = findOpenInsertionPoint "test.fsx" source "System.IO"
    Assert.Equal(1, insertionPoint.Line)

[<Fact>]
let ``No r directives in multiline fsx inserts at top`` () =
    let source = """let y = 1

let x = System.IO.File.ReadAllText "a"
"""
    let insertionPoint = findOpenInsertionPoint "test.fsx" source "System.IO"
    Assert.Equal(1, insertionPoint.Line)

[<Fact>]
let ``Open after r with existing opens`` () =
    let source = """#r "nuget: FSharp.Data"
open System

let x : System.IO.File = failwith ""
"""
    let insertionPoint = findOpenInsertionPoint "test.fsx" source "System.IO"
    // Should be near existing opens (after #r, near open System)
    Assert.Equal(2, insertionPoint.Line)

[<Fact>]
let ``Open placed after r directives in fsscript`` () =
    let source = """#r "nuget: Newtonsoft.Json"
#r "nuget: FSharp.Data"

let x : Newtonsoft.Json.JsonConvert = failwith ""
"""
    let insertionPoint = findOpenInsertionPoint "test.fsscript" source "Newtonsoft.Json"
    Assert.Equal(3, insertionPoint.Line)
