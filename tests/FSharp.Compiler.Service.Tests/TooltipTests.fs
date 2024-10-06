module FSharp.Compiler.Service.Tests.TooltipTests


#nowarn "57"

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Xml
open FSharp.Test
open Xunit

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
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler)

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
            | xmlDoc -> failwith $"Expected FSharpXmlDoc.FromXmlText, got {xmlDoc}"
        | elements -> failwith $"Expected at least one tooltip group element, got {elements}"
    | _ -> failwith "Expected checking to succeed."

    
[<Fact>]
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
    

[<Fact>]
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
    

[<Fact>]
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


[<Fact>]
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


[<Fact>]
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


[<Fact>]
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

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 5 13 "    SomeField: int" [ "SomeField" ] "Some sig comment on record field"


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

type Bar() =
    member val Foo = "bla" with get, set
"""

    testXmlDocFallbackToSigFileWhileInImplFile sigSource implSource 3 9 "type Bar() =" [ "Bar" ] "Some sig comment on class type"


[<Fact>]
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


[<Fact>]
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
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler)

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
                    
            Assert.True(breaks < squashedBreaks)
    | _ -> failwith "Expected checking to succeed."


[<Fact>]
let ``Squashed tooltip of long function signature should have newlines added`` () =
    let source =
        """
module Foo

let bar (fileName: string) (fileVersion: int) (sourceText: string)  (options: int) (userOpName: string) = 0
"""

    testToolTipSquashing source 3 6 "let bar (fileName: string) (fileVersion: int) (sourceText: string)  (options: int) (userOpName: string) = 0;" [ "bar" ] FSharpTokenTag.Identifier


[<Fact>]
let ``Squashed tooltip of record with long field signature should have newlines added`` () =
    let source =
        """
module Foo

type Foo =
    { Field1: string
      Field2: (string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string) }
"""

    testToolTipSquashing source 3 7 "type Foo =" [ "Foo" ] FSharpTokenTag.Identifier


[<Fact>]
let ``Squashed tooltip of DU with long case signature should have newlines added`` () =
    let source =
        """
module Foo

type SomeDiscUnion =
    | Case1 of string
    | Case2 of (string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string)
"""

    testToolTipSquashing source 3 7 "type SomeDiscUnion =" [ "SomeDiscUnion" ] FSharpTokenTag.Identifier


[<Fact>]
let ``Squashed tooltip of constructor with long signature should have newlines added`` () =
    let source =
        """
module Foo

type SomeClass(a1: int, a2: int, a3: int, a4: int, a5: int, a6: int, a7: int, a8: int, a9: int, a10: int, a11: int, a12: int, a13: int, a14: int, a15: int, a16: int, a17: int, a18: int, a19: int, a20: int) =
    member _.A = a1
"""

    testToolTipSquashing source 3 7 "type SomeClass(a1: int, a2: int, a3: int, a4: int, a5: int, a6: int, a7: int, a8: int, a9: int, a10: int, a11: int, a12: int, a13: int, a14: int, a15: int, a16: int, a17: int, a18: int, a19: int, a20: int) =" [ "SomeClass" ] FSharpTokenTag.Identifier


[<Fact>]
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

let getCheckResults source options =
    let fileName, options =
        mkTestFileAndOptions
            source
            options
    let _, checkResults = parseAndCheckFile fileName source options
    checkResults


let taggedTextsToString (t: TaggedText array) =
    t
    |> Array.map (fun taggedText -> taggedText.Text)
    |> String.concat ""
let assertAndExtractTooltip (ToolTipText(items)) =
    Assert.Equal(1,items.Length)
    match items.[0] with
    | ToolTipElement.Group [ singleElement ] ->
        let toolTipText =
            singleElement.MainDescription
            |> taggedTextsToString
        toolTipText, singleElement.XmlDoc, singleElement.Remarks |> Option.map taggedTextsToString
    | _ -> failwith $"Expected group, got {items.[0]}"
    
let assertAndGetSingleToolTipText items =
    let text,_xml,_remarks = assertAndExtractTooltip items
    text

let normalize (s:string) = s.Replace("\r\n", "\n").Replace("\n\n", "\n")

[<Fact>]
let ``Auto property should display a single tool tip`` () =
    let source = """
namespace Foo

/// Some comment on class
type Bar() =
    /// Some comment on class member
    member val Foo = "bla" with get, set
"""
    let checkResults = getCheckResults source Array.empty
    checkResults.GetToolTip(7, 18, "    member val Foo = \"bla\" with get, set", [ "Foo" ], FSharpTokenTag.Identifier)
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo "property Bar.Foo: string with get, set"

[<FactForNETCOREAPP>]
let ``Should display nullable Csharp code analysis annotations on method argument`` () =
    
    let source = """module Foo
let exists() = System.IO.Path.Exists(null:string)
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(2, 36, "let exists() = System.IO.Path.Exists(null:string)", [ "Exists" ], FSharpTokenTag.Identifier)
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo "System.IO.Path.Exists([<NotNullWhenAttribute (true)>] path: string | null) : bool"
    
[<FactForNETCOREAPP>]
let ``Should display xml doc on a nullable BLC method`` () =
    
    let source = """module Foo
let exists() = System.IO.Path.Exists(null:string)
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(2, 36, "let exists() = System.IO.Path.Exists(null:string)", [ "Exists" ], FSharpTokenTag.Identifier)
    |> assertAndExtractTooltip
    |> fun (text,xml,remarks) ->
            text |> Assert.shouldBeEquivalentTo "System.IO.Path.Exists([<NotNullWhenAttribute (true)>] path: string | null) : bool"
            match xml with
            | FSharpXmlDoc.FromXmlFile (_dll,sigPath) -> sigPath |> Assert.shouldBeEquivalentTo "M:System.IO.Path.Exists(System.String)"
            | _ -> failwith $"Xml wrong type %A{xml}"

            
[<FactForNETCOREAPP>]
let ``Should display xml doc on fsharp hosted nullable function`` () =
    
    let source = """module Foo
/// This is a xml doc above myFunc
let myFunc(x:string|null) : string | null = x

let exists() = myFunc(null)
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(5, 21, "let exists() = myFunc(null)", [ "myFunc" ], FSharpTokenTag.Identifier)
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
    
    let source = """module Foo
let getPath() = System.IO.Path.GetFileName(null:string)
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(2, 42, "let getPath() = System.IO.Path.GetFileName(null:string)", [ "GetFileName" ], FSharpTokenTag.Identifier)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""[<return:NotNullIfNotNullAttribute ("path")>]
System.IO.Path.GetFileName(path: string | null) : string | null""" |> normalize)

[<FactForNETCOREAPP>]
let ``Should display nullable Csharp code analysis annotations on TryParse pattern`` () =   
    let source = """module Foo
let success,version = System.Version.TryParse(null)
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(2, 45, "let success,version = System.Version.TryParse(null)", [ "TryParse" ], FSharpTokenTag.Identifier)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""System.Version.TryParse([<NotNullWhenAttribute (true)>] input: string | null, [<NotNullWhenAttribute (true)>] result: byref<System.Version | null>) : bool""")

[<FactForNETCOREAPP>]
let ``Display with nullable annotations can be squashed`` () =   
    let source = """module Foo
let success,version = System.Version.TryParse(null)
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(2, 45, "let success,version = System.Version.TryParse(null)", [ "TryParse" ], FSharpTokenTag.Identifier,width=100)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""System.Version.TryParse([<NotNullWhenAttribute (true)>] input: string | null,
                        [<NotNullWhenAttribute (true)>] result: byref<System.Version | null>) : bool""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is shown on BCL interface declaration`` () =   
    let source = """module Foo
open System
let myAction : Action<int> | null = null
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(3, 21, "let myAction : Action<int> | null = null", [ "Action" ], FSharpTokenTag.Identifier)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldStartWith ("""type Action<'T (allows ref struct)>""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is shown for each T on BCL interface declaration`` () =   
    let source = """module Foo
open System
let myAction : Action<int,_,_,_> | null = null
"""
    let checkResults = getCheckResults source [|"--checknulls+";"--langversion:preview"|]
    checkResults.GetToolTip(3, 21, "let myAction : Action<int,_,_,_> | null = null", [ "Action" ], FSharpTokenTag.Identifier)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldStartWith ("""type Action<'T1,'T2,'T3,'T4 (allows ref struct and allows ref struct and allows ref struct and allows ref struct)>""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is shown on BCL method usage`` () =
    let source = """module Foo
open System
open System.Collections.Generic
let doIt (dict:Dictionary<'a,'b>) = dict.GetAlternateLookup<'a,'b,ReadOnlySpan<char>>()
"""
    let checkResults = getCheckResults source [|"--langversion:preview"|]
    checkResults.GetToolTip(4, 59, "let doIt (dict:Dictionary<'a,'b>) = dict.GetAlternateLookup<'a,'b,ReadOnlySpan<char>>()", [ "GetAlternateLookup" ], FSharpTokenTag.Identifier)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldContain ("""'TAlternateKey (allows ref struct)""" |> normalize)
    
[<FactForNETCOREAPP>]
let ``Allows ref struct is not shown on BCL interface usage`` () =   
    let source = """module Foo
open System
let doIt(myAction : Action<int>) = myAction.Invoke(42)
"""
    let checkResults = getCheckResults source [|"--langversion:preview"|]
    checkResults.GetToolTip(3, 43, "let doIt(myAction : Action<int>) = myAction.Invoke(42)", [ "myAction" ], FSharpTokenTag.Identifier)   
    |> assertAndGetSingleToolTipText
    |> Assert.shouldBeEquivalentTo ("""val myAction: Action<int>""" |> normalize)