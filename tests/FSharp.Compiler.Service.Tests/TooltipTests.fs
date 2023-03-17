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
let ``Display XML doc of signature file for let if implementation doesn't have one`` () =
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
    
    
[<Test>]
let ``Display XML doc of signature file for partial AP if implementation doesn't have one`` () =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString
                   """
module Foo

/// Some Sig Doc on IsThree
val (|IsThree|_|): x: int -> int option
"""

               "A.fs",
               SourceText.ofString
                   """
module Foo

// No XML doc here because the signature file has one right?
let (|IsThree|_|) x = if x = 3 then Some x else None
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
        // Get the tooltip for `IsThree` in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(4, 4, "let (|IsThree|_|) x = if x = 3 then Some x else None", [ "IsThree" ], FSharpTokenTag.Identifier)

        match tooltipElements with
        | [ ToolTipElement.Group [ element ] ] ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains("Some Sig Doc on IsThree"))
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected a single tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."
    

[<Test>]
let ``Display XML doc of signature file for DU if implementation doesn't have one`` () =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString
                   """
module Foo

/// Some sig comment on the disc union type
type Bar =
    | Case1 of int * string
    | Case2 of string
"""

               "A.fs",
               SourceText.ofString
                   """
module Foo

// No XML doc here because the signature file has one right?
type Bar =
    | Case1 of int * string
    | Case2 of string
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
        // Get the tooltip for `Bar` in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(4, 7, "type Bar =", [ "Bar" ], FSharpTokenTag.Identifier)

        match tooltipElements with
        | [ ToolTipElement.Group [ element ] ] ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains("Some sig comment on the disc union type"))
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected a single tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."


[<Test>]
let ``Display XML doc of signature file for DU case if implementation doesn't have one`` () =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString
                   """
module Foo

type Bar =
    | BarCase1 of int * string
    /// Some sig comment on the disc union case
    | BarCase2 of string
"""

               "A.fs",
               SourceText.ofString
                    """
module Foo

type Bar =
    | BarCase1 of int * string
    // No XML doc here because the signature file has one right?
    | BarCase2 of string
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
        // Get the tooltip for `BarCase2` in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(7, 14, "    | BarCase2 of string", [ "BarCase2" ], FSharpTokenTag.Identifier)   // ToDo: Why line 7?

        match tooltipElements with
        | [ ToolTipElement.Group [ element ] ] ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains("Some sig comment on the disc union case"))
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected a single tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."


[<Test>]
let ``Display XML doc of signature file for record type if implementation doesn't have one`` () =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString
                   """
module Foo

/// Some sig comment on record type
type Bar = {
    SomeField: int
}
"""

               "A.fs",
               SourceText.ofString
                    """
module Foo

type Bar = {
    SomeField: int
}
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
        // Get the tooltip for `Bar` in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(3, 9, "type Bar = {", [ "Bar" ], FSharpTokenTag.Identifier)

        match tooltipElements with
        | [ ToolTipElement.Group [ element ] ] ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains("Some sig comment on record type"))
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected a single tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."


[<Test>]
let ``Display XML doc of signature file for record field if implementation doesn't have one`` () =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString
                   """
module Foo

type Bar = {
    /// Some sig comment on record field
    SomeField: int
}
"""

               "A.fs",
               SourceText.ofString
                    """
module Foo

type Bar = {
    SomeField: int
}
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
        // Get the tooltip for `Bar` in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(5, 9, "    SomeField: int", [ "SomeField" ], FSharpTokenTag.Identifier) // ToDo: Why ling 5?

        match tooltipElements with
        | [ ToolTipElement.Group [ element ] ] ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains("Some sig comment on record field"))
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected a single tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."


let testToolTipSquashing source line colAtEndOfNames lineText names tokenTag =
    let files =
        Map.ofArray
            [| "A.fs",
               SourceText.ofString source |]
    
    let documentSource fileName = Map.tryFind fileName files

    let projectOptions =
        let _, projectOptions = mkTestFileAndOptions "" Array.empty

        { projectOptions with
            SourceFiles = [| "A.fs" |] }

    let checker =
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource)

    let checkResult =
        checker.ParseAndCheckFileInProject("A.fs", 0, Map.find "A.fs" files, projectOptions)
        |> Async.RunImmediate
        
    match checkResult with
    | _, FSharpCheckFileAnswer.Succeeded(checkResults) ->

        // Get the tooltip for `bar`
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(line, colAtEndOfNames, lineText, names, tokenTag)

        let (ToolTipText tooltipElementsSquashed) =
            checkResults.GetToolTip(line, colAtEndOfNames, lineText, names, tokenTag, 10)
        match tooltipElements, tooltipElementsSquashed with
        | groups, groupsSquashed ->
            let breaks =
                groups
                |> List.map
                       (fun g ->
                                match g with
                                | ToolTipElement.Group gr -> gr |> List.map (fun g -> g.MainDescription)
                                | _ -> failwith "expected TooltipElement.Group")
                |> List.concat
                |> Array.concat
                |> Array.sumBy (fun t -> if t.Tag = TextTag.LineBreak then 1 else 0)
            let squashedBreaks =
                groupsSquashed
                |> List.map
                       (fun g ->
                                match g with
                                | ToolTipElement.Group gr -> gr |> List.map (fun g -> g.MainDescription)
                                | _ -> failwith "expected TooltipElement.Group")
                |> List.concat
                |> Array.concat
                |> Array.sumBy (fun t -> if t.Tag = TextTag.LineBreak then 1 else 0)
                    
            Assert.Less(breaks, squashedBreaks)
    | _ -> Assert.Fail "Expected checking to succeed."


[<Test>]
let ``Squashed tooltip of long function signature should have newlines added`` () =
    let source =
        """
module Foo

let bar (fileName: string) (fileVersion: int) (sourceText: string)  (options: int) (userOpName: string) = 0
"""

    testToolTipSquashing source 3 6 "let bar (fileName: string) (fileVersion: int) (sourceText: string)  (options: int) (userOpName: string) = 0;" [ "bar" ] FSharpTokenTag.Identifier


[<Test>]
let ``Squashed tooltip of record with long field signature should have newlines added`` () =
    let source =
        """
module Foo

type Foo =
    { Field1: string
      Field2: (string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string) }
"""

    testToolTipSquashing source 3 7 "type Foo =" [ "Foo" ] FSharpTokenTag.Identifier


[<Test>]
let ``Squashed tooltip of DU with long case signature should have newlines added`` () =
    let source =
        """
module Foo

type SomeDiscUnion =
    | Case1 of string
    | Case2 of (string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string)
"""

    testToolTipSquashing source 3 7 "type SomeDiscUnion =" [ "SomeDiscUnion" ] FSharpTokenTag.Identifier


[<Test>]
let ``Squashed tooltip of constructor with long signature should have newlines added`` () =
    let source =
        """
module Foo

type SomeClass(a1: int, a2: int, a3: int, a4: int, a5: int, a6: int, a7: int, a8: int, a9: int, a10: int, a11: int, a12: int, a13: int, a14: int, a15: int, a16: int, a17: int, a18: int, a19: int, a20: int) =
    member _.A = a1
"""

    testToolTipSquashing source 3 7 "type SomeClass(a1: int, a2: int, a3: int, a4: int, a5: int, a6: int, a7: int, a8: int, a9: int, a10: int, a11: int, a12: int, a13: int, a14: int, a15: int, a16: int, a17: int, a18: int, a19: int, a20: int) =" [ "SomeClass" ] FSharpTokenTag.Identifier


[<Test>]
let ``Squashed tooltip of property with long signature should have newlines added`` () =
    let source =
        """
module Foo

type SomeClass() =
    member _.Abc: (int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int) = 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20

let c = SomeClass()
c.Abc
"""

    testToolTipSquashing source 7 5 "c.Abc" [ "c"; "Abc" ] FSharpTokenTag.Identifier
