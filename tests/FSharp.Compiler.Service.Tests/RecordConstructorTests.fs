module FSharp.Compiler.Service.Tests.RecordConstructorTests

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open Xunit

// IDE smoke tests for the RecordConstructorSyntax feature (FS-1073): a positional record-constructor
// call should behave like any other constructor for tooling - go-to-definition lands on the record type,
// the tooltip shows the type, and the use is linked back to the type declaration.

let private source = """
module M
type MyRecord = { A: int; B: int }
let r = MyRecord(1, 2)
"""

let private tooltipToString (ToolTipText items) =
    items
    |> List.collect (function
        | ToolTipElement.Group elements -> elements |> List.collect (fun e -> List.ofArray e.MainDescription)
        | _ -> [])
    |> List.map (fun t -> t.Text)
    |> String.concat ""

[<Fact>]
let ``GoToDefinition on a record constructor call navigates to the record type`` () =
    let _, checkResults = parseAndCheckScriptPreview("Test.fsx", source)
    // 'MyRecord' constructor use is on line 4; ask for its declaration.
    let location = checkResults.GetDeclarationLocation(4, 16, "let r = MyRecord(1, 2)", [ "MyRecord" ])
    match location with
    | FindDeclResult.DeclFound r -> Assert.Equal(3, r.StartLine) // 'type MyRecord = ...'
    | other -> failwith $"Expected the record type declaration, got {other}"

[<Fact>]
let ``Tooltip on a record constructor call mentions the record type`` () =
    let tooltip =
        Checker.getTooltipWithOptions [| "--langversion:preview" |] """
module M
type MyRecord = { A: int; B: int }
let r = MyReco{caret}rd(1, 2)
"""
    Assert.Contains("MyRecord", tooltipToString tooltip)

[<Fact>]
let ``A record constructor call is linked to the record type declaration`` () =
    let _, checkResults = parseAndCheckScriptPreview("Test.fsx", source)
    // The constructor use is on line 4 starting at column 8 ('let r = ').
    let ctorUse =
        checkResults.GetAllUsesOfAllSymbolsInFile()
        |> Seq.find (fun u -> u.Range.StartLine = 4 && u.Range.StartColumn = 8)
    // The symbol resolves back to the record type declaration on line 3.
    match ctorUse.Symbol.DeclarationLocation with
    | Some loc -> Assert.Equal(3, loc.StartLine)
    | None -> failwith "Expected a declaration location for the record constructor symbol"
    Assert.True(checkResults.GetUsesOfSymbolInFile(ctorUse.Symbol).Length >= 1)
