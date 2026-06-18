module FSharp.Compiler.Service.Tests.OpenDeclarationInsertionTests

open Xunit
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common

/// Computes the line an `open` would be inserted at, mirroring the real editor consumer
/// (RoslynHelpers.insertContext): `FindNearestPointToInsertOpenDeclaration` followed by
/// `AdjustInsertionPoint`. Asserting on the adjusted position is what the user actually sees.
let private findOpenInsertionLine (fileName: string) (source: string) (entity: string) : int =
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

    let getLineStr line =
        if line >= 0 && line < lines.Length then lines.[line].Trim() else ""

    (ParsedInput.AdjustInsertionPoint getLineStr ctx).Line

[<Fact>]
let ``Open placed after r directives in fsx`` () =
    let source = """#r "nuget: Newtonsoft.Json"
#r "nuget: FSharp.Data"

let x : Newtonsoft.Json.JsonConvert = failwith ""
"""
    let line = findOpenInsertionLine "test.fsx" source "Newtonsoft.Json"
    // Right after the last #r directive (line 2), never before it.
    Assert.Equal(3, line)

[<Fact>]  // NEGATIVE: .fs files unaffected
let ``Open in fs file placed at top`` () =
    let source = """module Test
let x = System.IO.File.ReadAllText "a"
"""
    let line = findOpenInsertionLine "test.fs" source "System.IO"
    Assert.Equal(2, line)  // right after module decl

[<Fact>]
let ``Open after r and load directives`` () =
    let source = """#r "nuget: FSharp.Data"
#load "helper.fsx"

let x = 1
"""
    let line = findOpenInsertionLine "test.fsx" source "SomeNs"
    Assert.Equal(3, line)

[<Fact>]
let ``No r directives in fsx inserts at top`` () =
    let source = """let x = System.IO.File.ReadAllText "a"
"""
    let line = findOpenInsertionLine "test.fsx" source "System.IO"
    Assert.Equal(1, line)

[<Fact>]
let ``No r directives in multiline fsx inserts at top`` () =
    let source = """let y = 1

let x = System.IO.File.ReadAllText "a"
"""
    let line = findOpenInsertionLine "test.fsx" source "System.IO"
    Assert.Equal(1, line)

[<Fact>]
let ``Open after r with existing opens`` () =
    let source = """#r "nuget: FSharp.Data"
open System

let x : System.IO.File = failwith ""
"""
    let line = findOpenInsertionLine "test.fsx" source "System.IO"
    // Grouped with the existing `open System` (line 2), still after the #r directive.
    Assert.Equal(2, line)

[<Fact>]
let ``Open grouped with existing opens below header comment`` () =
    let source = """// My script header
open System

let x = System.IO.File.ReadAllText "a"
"""
    let line = findOpenInsertionLine "test.fsx" source "System.IO"
    // Grouped after existing `open System` (line 2), not forced to line 1.
    Assert.Equal(3, line)

[<Fact>]
let ``Open placed after r directives in fsscript`` () =
    let source = """#r "nuget: Newtonsoft.Json"
#r "nuget: FSharp.Data"

let x : Newtonsoft.Json.JsonConvert = failwith ""
"""
    let line = findOpenInsertionLine "test.fsscript" source "Newtonsoft.Json"
    Assert.Equal(3, line)

[<Fact>]  // Regression: must not force the open above a named module (would not compile)
let ``Open not forced above named module in fsx`` () =
    let source = """module Foo

let x = System.IO.File.ReadAllText "a"
"""
    let line = findOpenInsertionLine "test.fsx" source "System.IO"
    // Below the `module Foo` header (line 1), inside the module.
    Assert.Equal(2, line)

[<Fact>]  // Only #r/#load drive placement; other directives (#time/#help/#I/...) must not
let ``Non reference directive does not drive placement in fsx`` () =
    let source = """#time "on"

let x = System.IO.File.ReadAllText "a"
"""
    let line = findOpenInsertionLine "test.fsx" source "System.IO"
    // #time is not a reference directive, so the open is placed by the normal top-of-file logic.
    Assert.Equal(1, line)

[<Fact>]  // #reference is an alias of #r; the open must still land after it
let ``Open placed after reference directive in fsx`` () =
    let source = """#reference "nuget: Newtonsoft.Json"

let x : Newtonsoft.Json.JsonConvert = failwith ""
"""
    let line = findOpenInsertionLine "test.fsx" source "Newtonsoft.Json"
    Assert.Equal(2, line)

[<Fact>]  // #I precedes #r; the open still lands after the last #r, not merely after #I
let ``Open after r preceded by I directive in fsx`` () =
    let source = """#I "/tmp"
#r "nuget: FSharp.Data"

let x : Newtonsoft.Json.JsonConvert = failwith ""
"""
    let line = findOpenInsertionLine "test.fsx" source "Newtonsoft.Json"
    Assert.Equal(3, line)
[<Fact>]  // comments / blank lines between directives are trivia, not decls: scan still spans both
let ``Open after r and load separated by comment and blank line in fsx`` () =
    let source = """#r "nuget: FSharp.Data"

// pull in a helper
#load "helper.fsx"

let x = SomeNs.foo
"""
    let line = findOpenInsertionLine "test.fsx" source "SomeNs"
    Assert.Equal(5, line)  // after #load on line 4

[<Fact>]  // a non-reference directive between two references must not stop the scan
let ``Open after last reference with interleaved I directive in fsx`` () =
    let source = """#r "nuget: A"
#I "/tmp"
#load "b.fsx"

let x = SomeNs.foo
"""
    let line = findOpenInsertionLine "test.fsx" source "SomeNs"
    Assert.Equal(4, line)  // after #load on line 3, not after #I on line 2

[<Fact>]  // CRLF line endings must not shift the computed line
let ``Open placed after r directive with CRLF line endings`` () =
    let source = "#r \"nuget: Newtonsoft.Json\"\r\n\r\nlet x : Newtonsoft.Json.JsonConvert = failwith \"\"\r\n"
    let line = findOpenInsertionLine "test.fsx" source "Newtonsoft.Json"
    Assert.Equal(2, line)
