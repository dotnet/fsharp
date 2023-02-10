module FSharp.Compiler.Service.Tests.TooltipTests

#nowarn "57"

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open NUnit.Framework

[<Test>]
let ``Display XML doc of signature file if implementation doesn't have one`` () =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString
                   """
module Foo

/// Great XML doc comment
val bar: a: int -> b: int -> int
"""

               "A.fs",
               SourceText.ofString
                   """
module Foo

// No XML doc here because the signature file has one right?
let bar a b = a - b
""" |]

    let documentSource fileName = Map.tryFind fileName files

    let projectOptions =
        let _, projectOptions = mkTestFileAndOptions "" Array.empty

        { projectOptions with
            SourceFiles = [| "A.fsi"; "A.fs" |] }

    let checker =
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource)

    let checkResult =
        checker.ParseAndCheckFileInProject("A.fs", 0, Map.find "A.fs" files, projectOptions)
        |> Async.RunImmediate

    match checkResult with
    | _, FSharpCheckFileAnswer.Succeeded(checkResults) ->
        let barSymbol = findSymbolByName "bar" checkResults

        match barSymbol with
        | :? FSharpMemberOrFunctionOrValue as mfv -> Assert.True mfv.HasSignatureFile
        | _ -> Assert.Fail "Expected to find a symbol FSharpMemberOrFunctionOrValue that HasSignatureFile"

        // Get the tooltip for `bar` in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(4, 4, "let bar a b = a - b", [ "bar" ], FSharpTokenTag.Identifier)

        match tooltipElements with
        | [ ToolTipElement.Group [ element ] ] ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc -> Assert.True xmlDoc.NonEmpty
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected a single tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."
