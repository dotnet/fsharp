module FSharp.Compiler.Service.Tests.TooltipTests

#nowarn "57"

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open NUnit.Framework

let testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource line colAtEndOfNames lineText names (expectedContent: string) =
    let files =
        Map.ofArray
            [| "A.fsi",
               SourceText.ofString sigSource


               "A.fs",
               SourceText.ofString implSource |]

    let documentSource fileName = Map.tryFind fileName files |> async.Return

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
        // Get the tooltip for (line, colAtEndOfNames) in the implementation file
        let (ToolTipText tooltipElements) =
            checkResults.GetToolTip(line, colAtEndOfNames, lineText, names, FSharpTokenTag.Identifier)

        match tooltipElements with
        | ToolTipElement.Group [ element ] :: _ ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains(expectedContent))
            | xmlDoc -> Assert.Fail $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> Assert.Fail $"Expected at least one tooltip group element, got {elements}"
    | _ -> Assert.Fail "Expected checking to succeed."

    
[<Test>]
let ``Display XML doc of signature file for let if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Great XML doc comment
val bar: a: int -> b: int -> int
"""

    let implSource =
                   """
module Foo

// No XML doc here because the signature file has one right?
let bar a b = a - b
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 4 4 "let bar a b = a - b" [ "bar" ] "Great XML doc comment"
    

[<Test>]
let ``Display XML doc of signature file for partial AP if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Some Sig Doc on IsThree
val (|IsThree|_|): x: int -> int option
"""

    let implSource =
        """
module Foo

// No XML doc here because the signature file has one right?
let (|IsThree|_|) x = if x = 3 then Some x else None
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 4 4 "let (|IsThree|_|) x = if x = 3 then Some x else None" [ "IsThree" ] "Some Sig Doc on IsThree"
    

[<Test>]
let ``Display XML doc of signature file for DU if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Some sig comment on the disc union type
type Bar =
    | Case1 of int * string
    | Case2 of string
"""
               
    let implSource =
        """
module Foo

// No XML doc here because the signature file has one right?
type Bar =
    | Case1 of int * string
    | Case2 of string
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 4 7 "type Bar =" [ "Bar" ] "Some sig comment on the disc union type"


[<Test>]
let ``Display XML doc of signature file for DU case if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

type Bar =
    | BarCase1 of int * string
    /// Some sig comment on the disc union case
    | BarCase2 of string
"""

    let implSource =
        """
module Foo

type Bar =
    | BarCase1 of int * string
    // No XML doc here because the signature file has one right?
    | BarCase2 of string
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 7 14 "    | BarCase2 of string" [ "BarCase2" ] "Some sig comment on the disc union case"


[<Test>]
let ``Display XML doc of signature file for record type if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Some sig comment on record type
type Bar = {
    SomeField: int
}
"""

    let implSource =
        """
module Foo

type Bar = {
    SomeField: int
}
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 3 9 "type Bar = {" [ "Bar" ] "Some sig comment on record type"


[<Test>]
let ``Display XML doc of signature file for record field if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

type Bar = {
    /// Some sig comment on record field
    SomeField: int
}
"""

    let implSource =
        """
module Foo

type Bar = {
    SomeField: int
}
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 5 9 "    SomeField: int" [ "SomeField" ] "Some sig comment on record field"


[<Test>]
let ``Display XML doc of signature file for class type if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Some sig comment on class type
type Bar =
    new: unit -> Bar
    member Foo: string
"""
               
    let implSource =
        """
module Foo

type Bar() =
    member val Foo = "bla" with get, set
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 3 9 "type Bar() =" [ "Bar" ] "Some sig comment on class type"


[<Test>]
let ``Display XML doc of signature file for class member if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

type Bar =
    new: unit -> Bar
    /// Some sig comment on auto property
    member Foo: string
    /// Some sig comment on class member
    member Func: int -> int -> int
"""

    let implSource =
        """
module Foo

type Bar() =
    member val Foo = "bla" with get, set
    member _.Func x y = x * y
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 6 30 "    member _.Func x y = x * y" [ "_"; "Func" ] "Some sig comment on class member"


[<Test>]
let ``Display XML doc of signature file for module if implementation doesn't have one`` () =
    let sigSource =
        """
/// Some sig comment on module
module Foo

val a: int
"""
               
    let implSource =
        """
module Foo

let a = 23
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 2 10 "module Foo" [ "Foo" ] "Some sig comment on module"


let testToolTipSquashing source line colAtEndOfNames lineText names tokenTag =
    let files =
        Map.ofArray
            [| "A.fs",
               SourceText.ofString source |]
    
    let documentSource fileName = Map.tryFind fileName files |> async.Return

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
