﻿module FSharp.Compiler.Service.Tests.TooltipTests


#nowarn "57"

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Test
open Xunit

let testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource (expectedContent: string) =
    let context = Checker.getResolveContext implSource

    let files =
        Map.ofArray
            [| "A.fsi", SourceText.ofString sigSource
               "A.fs",  SourceText.ofString context.Source |]

    let documentSource fileName = Map.tryFind fileName files |> async.Return

    let projectOptions =
        let _, projectOptions = mkTestFileAndOptions Array.empty
        { projectOptions with SourceFiles = [| "A.fsi"; "A.fs" |] }

    let checker =
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler)

    let checkResult =
        checker.ParseAndCheckFileInProject("A.fs", 0, Map.find "A.fs" files, projectOptions)
        |> Async.RunImmediate

    match checkResult with
    | _, FSharpCheckFileAnswer.Succeeded(checkResults) ->
        // Get the tooltip for (line, colAtEndOfNames) in the implementation file
        let (ToolTipText tooltipElements) = checkResults.GetTooltip(context)
        match tooltipElements with
        | ToolTipElement.Group [ element ] :: _ ->
            match element.XmlDoc with
            | FSharpXmlDoc.FromXmlText xmlDoc ->
                Assert.True xmlDoc.NonEmpty
                Assert.True (xmlDoc.UnprocessedLines[0].Contains(expectedContent))
            | xmlDoc -> failwith $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> failwith $"Expected at least one tooltip group element, got {elements}"
    | _ -> failwith "Expected checking to succeed."


[<Fact>]
let ``Display XML doc of signature file for let if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Comment
val bar: a: int -> b: int -> int
"""

    let implSource =
                   """
module Foo

let bar{caret} a b = a - b
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment"
    

[<Fact>]
let ``Display XML doc of signature file for partial AP if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Comment
val (|IsThree|_|): x: int -> int option
"""

    let implSource =
        """
module Foo

let (|IsThr{caret}ee|_|) x = if x = 3 then Some x else None
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment"
    

[<Fact>]
let ``Display XML doc of signature file for DU if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Comment
type Bar =
    | Case1 of int * string
    | Case2 of string
"""
               
    let implSource =
        """
module Foo

type Bar{caret} =
    | Case1 of int * string
    | Case2 of string
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment"


[<Fact>]
let ``Display XML doc of signature file for DU case if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

type Bar =
    | BarCase1 of int * string
    /// CommentSig
    | BarCase2 of string
"""

    let implSource =
        """
module Foo

type Bar =
    | BarCase1 of int * string
    // CommentImpl
    | BarCase2{caret} of string
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "CommentSig"


[<Fact>]
let ``Display XML doc of signature file for record type if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

/// Comment
type Bar = {
    SomeField: int
}
"""

    let implSource =
        """
module Foo

type B{caret}ar = {
    SomeField: int
}
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment"


[<Fact>]
let ``Display XML doc of signature file for record field if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

type Bar = {
    /// Comment
    SomeField: int
}
"""

    let implSource =
        """
module Foo

type Bar = {
    SomeFiel{caret}d: int
}
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment"


[<Fact>]
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

type B{caret}ar() =
    member val Foo = "bla" with get, set
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Some sig comment on class type"


[<Fact>]
let ``Display XML doc of signature file for class member if implementation doesn't have one`` () =
    let sigSource =
        """
module Foo

type Bar =
    new: unit -> Bar
    /// Comment1
    member Foo: string
    /// Comment2
    member Func: int -> int -> int
"""

    let implSource =
        """
module Foo

type Bar() =
    member val Foo = "bla" with get, set
    member _.Func{caret} x y = x * y
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment2"


[<Fact>]
let ``Display XML doc of signature file for module if implementation doesn't have one`` () =
    let sigSource =
        """
/// Comment
module Foo

val a: int
"""
               
    let implSource =
        """
module Fo{caret}o

let a = 23
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource "Comment"


let testToolTipSquashing source =
    let context = Checker.getResolveContext source
    let files = Map.ofArray [| "A.fs", SourceText.ofString context.Source |]
    
    let documentSource fileName = Map.tryFind fileName files |> async.Return

    let projectOptions =
        let _, projectOptions = mkTestFileAndOptions Array.empty

        { projectOptions with
            SourceFiles = [| "A.fs" |] }

    let checker =
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler)

    let checkResult =
        checker.ParseAndCheckFileInProject("A.fs", 0, Map.find "A.fs" files, projectOptions)
        |> Async.RunImmediate
        
    match checkResult with
    | _, FSharpCheckFileAnswer.Succeeded(checkResults) ->
        // Get the tooltip for `bar`
        let (ToolTipText tooltipElements) = checkResults.GetTooltip(context)
        let (ToolTipText tooltipElementsSquashed) = checkResults.GetTooltip(context, 10)

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
                    
            Assert.True(breaks < squashedBreaks)
    | _ -> failwith "Expected checking to succeed."


[<Fact>]
let ``Squashed tooltip of long function signature should have newlines added`` () =
    testToolTipSquashing """
module Foo

let bar{caret} (fileName: string) (fileVersion: int) (sourceText: string)  (options: int) (userOpName: string) = 0
"""


[<Fact>]
let ``Squashed tooltip of record with long field signature should have newlines added`` () =
    testToolTipSquashing """
module Foo

type Fo{caret}o =
    { Field1: string
      Field2: (string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string) }
"""


[<Fact>]
let ``Squashed tooltip of DU with long case signature should have newlines added`` () =
    testToolTipSquashing """
module Foo

type SomeDis{caret}cUnion =
    | Case1 of string
    | Case2 of (string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string)
"""


[<Fact>]
let ``Squashed tooltip of constructor with long signature should have newlines added`` () =
    testToolTipSquashing """
module Foo

type Some{caret}Class(a1: int, a2: int, a3: int, a4: int, a5: int, a6: int, a7: int, a8: int, a9: int, a10: int, a11: int, a12: int, a13: int, a14: int, a15: int, a16: int, a17: int, a18: int, a19: int, a20: int) =
    member _.A = a1
"""


[<Fact>]
let ``Squashed tooltip of property with long signature should have newlines added`` () =
    testToolTipSquashing """
module Foo

type SomeClass() =
    member _.Abc: (int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int) = 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20

let c = SomeClass()
c.Ab{caret}c
"""


let getCheckResults source options =
    let fileName, options =
        mkTestFileAndOptions
            options
    let _, checkResults = parseAndCheckFile fileName source options
    checkResults


let taggedTextsToString (t: TaggedText array) =
    t
    |> Array.map (fun taggedText -> taggedText.Text)
    |> String.concat ""

let assertAndExtractTooltip (ToolTipText(items)) =
    Assert.Equal(1,items.Length)
    match items[0] with
    | ToolTipElement.Group [ singleElement ] ->
        let toolTipText =
            singleElement.MainDescription
            |> taggedTextsToString
        toolTipText, singleElement.XmlDoc, singleElement.Remarks |> Option.map taggedTextsToString
    | _ -> failwith $"Expected group, got {items[0]}"
    
let assertAndGetSingleToolTipText items =
    let text,_xml,_remarks = assertAndExtractTooltip items
    text

let normalize (s: string) = s.Replace("\r\n", "\n").Replace("\n\n", "\n")

[<Fact>]
let ``Auto property should display a single tool tip`` () =
    Checker.getTooltip """
namespace Foo

/// Some comment on class
type Bar() =
    /// Some comment on class member
    member val Fo{caret}o = "bla" with get, set
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo "property Bar.Foo: string with get, set"

[<FactForNETCOREAPP>]
let ``Should display nullable Csharp code analysis annotations on method argument`` () =
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

let exists() = System.IO.Path.Exist{caret}s(null:string)
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo "System.IO.Path.Exists([<NotNullWhenAttribute (true)>] path: string | null) : bool"
    
[<FactForNETCOREAPP>]
let ``Should display xml doc on a nullable BLC method`` () =
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

let exists() = System.IO.Path.Exi{caret}sts(null:string)
"""
    |> assertAndExtractTooltip
    |> fun (text,xml,_remarks) ->
            text |> Assert.shouldBeEquivalentTo "System.IO.Path.Exists([<NotNullWhenAttribute (true)>] path: string | null) : bool"
            match xml with
            | FSharpXmlDoc.FromXmlFile (_dll,sigPath) -> sigPath |> Assert.shouldBeEquivalentTo "M:System.IO.Path.Exists(System.String)"
            | _ -> failwith $"Xml wrong type %A{xml}"

            
[<FactForNETCOREAPP>]
let ``Should display xml doc on fsharp hosted nullable function`` () =
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

/// This is a xml doc above myFunc
let myFunc(x:string|null) : string | null = x

let exists() = myFu{caret}nc(null)
"""
    |> assertAndExtractTooltip
    |> fun (text,xml,remarks) ->
            match xml with
            | FSharpXmlDoc.FromXmlText t ->
                 t.UnprocessedLines |> Assert.shouldBeEquivalentTo [|" This is a xml doc above myFunc"|]
            | _ -> failwith $"xml was %A{xml}"
            text |> Assert.shouldBeEquivalentTo "val myFunc: x: string | null -> string | null"            
            remarks |> Assert.shouldBeEquivalentTo (Some "Full name: Foo.myFunc")


[<FactForNETCOREAPP>]
let ``Should display nullable Csharp code analysis annotations on method return type`` () =
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

let getPath() = System.IO.Path.GetFile{caret}Name(null:string)
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""[<return:NotNullIfNotNullAttribute ("path")>]
System.IO.Path.GetFileName(path: string | null) : string | null""" |> normalize)

[<FactForNETCOREAPP>]
let ``Should display nullable Csharp code analysis annotations on TryParse pattern`` () =   
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

let success,version = System.Version.TryPar{caret}se(null)
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo """System.Version.TryParse([<NotNullWhenAttribute (true)>] input: string | null, [<NotNullWhenAttribute (true)>] result: byref<System.Version | null>) : bool"""

[<FactForNETCOREAPP>]
let ``Display with nullable annotations can be squashed`` () =   
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

let success,version = System.Version.Try{caret}Parse(null)
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""System.Version.TryParse([<NotNullWhenAttribute (true)>] input: string | null, [<NotNullWhenAttribute (true)>] result: byref<System.Version | null>) : bool""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is shown on BCL interface declaration`` () =   
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

open System
let myAction : Acti{caret}on<int> | null = null
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldStartWith ("""type Action<'T (allows ref struct)>""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is shown for each T on BCL interface declaration`` () =   
    Checker.getTooltipWithOptions [|"--checknulls+";"--langversion:preview"|] """
module Foo

open System
let myAction : Acti{caret}on<int,_,_,_> | null = null
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldStartWith ("""type Action<'T1,'T2,'T3,'T4 (allows ref struct and allows ref struct and allows ref struct and allows ref struct)>""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is shown on BCL method usage`` () =
    Checker.getTooltip """
module Foo

open System
open System.Collections.Generic
let doIt (dict:Dictionary<'a,'b>) = dict.GetAltern{caret}ateLookup<'a,'b,ReadOnlySpan<char>>()
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldContain ("""'TAlternateKey (allows ref struct)""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is not shown on BCL interface usage`` () =   
    Checker.getTooltip """
module Foo

open System
let doIt(myAction : Action<int>) = myAc{caret}tion.Invoke(42)
"""
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""val myAction: Action<int>""" |> normalize)
    
// Tests for direct interfaces in tooltips
[<Fact>]
let ``Tooltip for class with multiple direct interfaces shows all direct interfaces`` () =
    let source = """
module DirectInterfaces

// Define multiple interfaces
type IA =
    abstract DoA: unit -> unit

type IB =
    abstract DoB: unit -> unit

// Class implementing multiple interfaces
type ClassWithMultipleInterfaces() =
    interface IA with
        member _.DoA() = ()
    interface IB with
        member _.DoB() = ()

// Create an instance
let obj = ClassWithMultipleInterfaces()
"""
    let checkResults = getCheckResults source Array.empty
    let tooltip = checkResults.GetToolTip(12, 30, "type ClassWithMultipleInterfaces() =", [ "ClassWithMultipleInterfaces" ], FSharpTokenTag.Identifier)
    let tooltipText = tooltip |> assertAndGetSingleToolTipText
    
    // Verify both direct interfaces are shown
    Assert.contains "inherit IA" tooltipText
    Assert.contains "inherit IB" tooltipText

[<Fact>]
let ``Tooltip for class implementing interface chain shows only direct interface`` () =
    let source = """
module InterfaceChain

// Define chained interfaces
type IX =
    abstract DoX: unit -> unit

type IY =
    inherit IX
    abstract DoY: unit -> unit

// Class implementing the most derived interface
type ClassWithChainedInterface() =
    interface IY with
        member _.DoY() = ()
        member _.DoX() = ()

// Create an instance
let obj = ClassWithChainedInterface()
"""
    let checkResults = getCheckResults source Array.empty
    let tooltip = checkResults.GetToolTip(12, 31, "type ClassWithChainedInterface() =", [ "ClassWithChainedInterface" ], FSharpTokenTag.Identifier)
    let tooltipText = tooltip |> assertAndGetSingleToolTipText
    
    // Verify only direct interface is shown
    Assert.contains "inherit IY" tooltipText
    Assert.doesNotContain "inherit IX" tooltipText

[<Fact>]
let ``Tooltip for class with combined interface hierarchy shows only direct interfaces`` () =
    let source = """
module CombinedHierarchy

// Base interfaces
type IBase1 =
    abstract DoBase1: unit -> unit

type IBase2 =
    abstract DoBase2: unit -> unit

// Derived interfaces
type IDerived1 =
    inherit IBase1
    abstract DoDerived1: unit -> unit

type IDerived2 =
    inherit IBase2
    abstract DoDerived2: unit -> unit

// Class implementing multiple interfaces, some in a chain
type ComplexClass() =
    interface IDerived1 with
        member _.DoDerived1() = ()
        member _.DoBase1() = ()
    interface IBase2 with
        member _.DoBase2() = ()

// Create an instance
let obj = ComplexClass()
"""
    let checkResults = getCheckResults source Array.empty
    let tooltip = checkResults.GetToolTip(21, 19, "type ComplexClass() =", [ "ComplexClass" ], FSharpTokenTag.Identifier)
    let tooltipText = tooltip |> assertAndGetSingleToolTipText
    
    // Verify only direct interfaces are shown
    Assert.contains "inherit IDerived1" tooltipText
    Assert.contains "inherit IBase2" tooltipText
    Assert.doesNotContain "inherit IBase1" tooltipText