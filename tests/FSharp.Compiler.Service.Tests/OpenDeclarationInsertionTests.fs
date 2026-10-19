module FSharp.Compiler.Service.Tests.OpenDeclarationInsertionTests

open Xunit
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common

/// Computes the position an `open` would be inserted at, mirroring the real editor consumer
/// (RoslynHelpers.insertContext): `FindNearestPointToInsertOpenDeclaration` followed by
/// `AdjustInsertionPoint`. Asserting on the adjusted position is what the user actually sees.
let private findOpenInsertionAt currentLine insertionPoint fileName (source: string) (entity: string) =
    let parseTree = parseSourceCode (fileName, source)

    let lines = source.Replace("\r\n", "\n").Split('\n')

    let idents = entity.Split('.')

    let ctx =
        ParsedInput.FindNearestPointToInsertOpenDeclaration currentLine parseTree idents insertionPoint

    let getLineStr line =
        if line >= 0 && line < lines.Length then lines.[line].Trim() else ""

    let pos = ParsedInput.AdjustInsertionPoint getLineStr ctx
    pos.Line, pos.Column

let private findOpenInsertion insertionPoint fileName (source: string) entity =
    findOpenInsertionAt (source.TrimEnd().Split('\n').Length) insertionPoint fileName source entity

let private findOpenInsertionLine fileName source entity =
    findOpenInsertion OpenStatementInsertionPoint.TopLevel fileName source entity |> fst

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
    Assert.Equal(3, line)

[<Theory>]
[<InlineData("""[<AutoOpen>] module Foo

let x = System.IO.File.ReadAllText "a"
""", 3)>]
[<InlineData("""module Foo
let x = System.IO.File.ReadAllText "a"
""", 2)>]
[<InlineData("""namespace Ns
type T() =
    member _.M() = System.IO.File.ReadAllText "a"
""", 2)>]
let ``Open goes below the header of a module or namespace, with or without a blank line`` (source: string, expectedLine: int) =
    Assert.Equal(expectedLine, findOpenInsertionLine "test.fs" source "System.IO")

[<Fact>]
let ``Open placed at the first declaration of a nested module declared on one line`` () =
    let source = """module Top

module M = let x = System.IO.File.ReadAllText "a"
"""
    let pos = findOpenInsertion OpenStatementInsertionPoint.Nearest "test.fs" source "System.IO"
    Assert.Equal((3, 11), pos)

[<Theory>]
[<InlineData("module M = let y = 1", 2)>]
[<InlineData("[<Obsolete>] module M = let y = 1", 2)>]
[<InlineData("[<Obsolete>] module M = let y = 1", 3)>]
let ``Open stays out of a nested module declared on one line that the use is not in`` (nestedModule: string, useLine: int) =
    let source = $"module Top\nlet x = Console.WriteLine 42\n{nestedModule}\n"
    let pos = findOpenInsertionAt useLine OpenStatementInsertionPoint.Nearest "test.fs" source "System"
    Assert.Equal((2, 0), pos)

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

[<Fact>]  // Nearest insertion inside a nested module keeps the open's indentation (not forced to col 0,
          // which would break the module's offside) while still landing after the leading #r.
let ``Open under Nearest keeps indentation inside nested module after r`` () =
    let source = """#r "nuget: FSharp.Data"
module Outer =
    module Inner =
        let _z = FSharp.Data.JsonValue.Parse "1"
"""
    let line, col = findOpenInsertion OpenStatementInsertionPoint.Nearest "test.fsx" source "FSharp.Data"
    Assert.Equal((4, 8), (line, col))
