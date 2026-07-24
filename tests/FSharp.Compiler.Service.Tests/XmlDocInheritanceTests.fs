module FSharp.Compiler.Service.Tests.XmlDocInheritanceTests

open FSharp.Compiler.Symbols
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.XmlDocInheritance
open Xunit

let expandWith (crefMap: (string * string) list) (implicitTarget: string option) (xml: string) : string =
    let map = Map.ofList crefMap
    let resolve cref = Map.tryFind cref map
    expandInheritDocFromXmlText resolve implicitTarget range0 Set.empty xml

let getTooltipXml (markedSource: string) =
    let _, xml, _ = Checker.getTooltip markedSource |> TooltipTests.assertAndExtractTooltip
    xml

let getCompletionXml name markedSource =
    let completionInfo = Checker.getCompletionInfo markedSource

    let item =
        completionInfo.Items
        |> Array.find (fun item -> item.NameInCode = name)

    let _, xml, _ = item.Description |> TooltipTests.assertAndExtractTooltip
    xml

let getSymbolXml name markedSource =
    let _, checkResults = Checker.getCheckedResolveContext markedSource
    let symbol = XmlDocTests.findSymbolByName name checkResults

    match symbol with
    | :? FSharpEntity as entity -> entity.XmlDoc
    | :? FSharpMemberOrFunctionOrValue as value -> value.XmlDoc
    | :? FSharpUnionCase as unionCase -> unionCase.XmlDoc
    | :? FSharpField as field -> field.XmlDoc
    | :? FSharpActivePatternCase as activePatternCase -> activePatternCase.XmlDoc
    | _ -> failwith $"Unexpected symbol type {symbol.GetType()}"

let xmlText (xml: FSharpXmlDoc) =
    match xml with
    | FSharpXmlDoc.FromXmlText xmlDoc -> xmlDoc.GetXmlText()
    | other -> failwith $"Expected FromXmlText, got {other}"

[<Fact(Skip = "RED: blocked on Phase 3 IDE wiring; proves inheritdoc not expanded in tooltip today (G1)")>]
let ``tooltip expands explicit cref inheritdoc to a same-file type`` () =
    let xml =
        getTooltipXml
            """
module Test
/// <summary>Base summary text</summary>
type Base() = class end
/// <inheritdoc cref="T:Test.Base"/>
type Derive{caret}d() = class end
"""

    Assert.Contains("Base summary text", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)
