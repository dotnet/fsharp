// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

module CompletionProviderTests =

    open System
    open System.Linq
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.Completion
    open Microsoft.CodeAnalysis.Text
    open Microsoft.VisualStudio.FSharp.Editor
    open FSharp.Editor.Tests.Helpers
    open Xunit
    open FSharp.Test

    let filePath = "C:\\test.fs"

    let formatCompletions (completions: string seq) =
        "\n\t" + String.Join("\n\t", completions)

    let VerifyCompletionListWithOptions (fileContents: string, marker: string, expected: string list, unexpected: string list, opts) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length

        let document =
            RoslynTestHelpers.CreateSolution(fileContents)
            |> RoslynTestHelpers.GetSingleDocument

        let results =
            FSharpCompletionProvider.ProvideCompletionsAsyncAux(document, caretPosition, (fun _ -> []))
            |> Async.RunSynchronously
            |> Option.defaultValue (ResizeArray())
            |> Seq.map (fun result -> result.DisplayText)

        let expectedFound = expected |> List.filter results.Contains

        let expectedNotFound = expected |> List.filter (expectedFound.Contains >> not)

        let unexpectedNotFound = unexpected |> List.filter (results.Contains >> not)

        let unexpectedFound = unexpected |> List.filter (unexpectedNotFound.Contains >> not)

        // If either of these are true, then the test fails.
        let hasExpectedNotFound = not (List.isEmpty expectedNotFound)
        let hasUnexpectedFound = not (List.isEmpty unexpectedFound)

        if hasExpectedNotFound || hasUnexpectedFound then
            let expectedNotFoundMsg =
                if hasExpectedNotFound then
                    sprintf "\nExpected completions not found:%s\n" (formatCompletions expectedNotFound)
                else
                    String.Empty

            let unexpectedFoundMsg =
                if hasUnexpectedFound then
                    sprintf "\nUnexpected completions found:%s\n" (formatCompletions unexpectedFound)
                else
                    String.Empty

            let completionsMsg = sprintf "\nin Completions:%s" (formatCompletions results)

            let msg = sprintf "%s%s%s" expectedNotFoundMsg unexpectedFoundMsg completionsMsg

            failwith msg

    let VerifyCompletionList (fileContents, marker, expected, unexpected) =
        VerifyCompletionListWithOptions(fileContents, marker, expected, unexpected, [||])

    let VerifyCompletionListExactlyWithOptions (fileContents: string, marker: string, expected: string list, opts) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length

        let document =
            RoslynTestHelpers.CreateSolution(fileContents)
            |> RoslynTestHelpers.GetSingleDocument

        let actual =
            FSharpCompletionProvider.ProvideCompletionsAsyncAux(document, caretPosition, (fun _ -> []))
            |> Async.RunSynchronously
            |> Option.defaultValue (ResizeArray())
            |> Seq.toList
            // sort items as Roslyn do - by `SortText`
            |> List.sortBy (fun x -> x.SortText)

        let actualNames = actual |> List.map (fun x -> x.DisplayText)

        if actualNames <> expected then
            failwithf
                "Expected:\n%s,\nbut was:\n%s\nactual with sort text:\n%s"
                (String.Join("; ", expected |> List.map (sprintf "\"%s\"")))
                (String.Join("; ", actualNames |> List.map (sprintf "\"%s\"")))
                (String.Join("\n", actual |> List.map (fun x -> sprintf "%s => %s" x.DisplayText x.SortText)))

    let VerifyCompletionListExactly (fileContents: string, marker: string, expected: string list) =
        VerifyCompletionListExactlyWithOptions(fileContents, marker, expected, [||])

    let VerifyNoCompletionList (fileContents: string, marker: string) =
        VerifyCompletionListExactly(fileContents, marker, [])

    let VerifyCompletionListSpan (fileContents: string, marker: string, expected: string) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let sourceText = SourceText.From(fileContents)

        let resultSpan =
            CompletionUtils.getDefaultCompletionListSpan (sourceText, caretPosition, documentId, filePath, [])

        Assert.Equal(expected, sourceText.ToString(resultSpan))

    [<Fact>]
    let ShouldTriggerCompletionAtCorrectMarkers () =
        let testCases =
            [
                ("x", true)
                ("y", true)
                ("1", false)
                ("2", false)
                ("x +", false)
                ("Console.Write", false)
                ("System.", true)
                ("Console.", true)
            ]

        for (marker, shouldBeTriggered) in testCases do
            let fileContents =
                """
let x = 1
let y = 2
System.Console.WriteLine(x + y)
"""

            let caretPosition = fileContents.IndexOf(marker) + marker.Length
            let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
            let getInfo () = documentId, filePath, []

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    CompletionTriggerKind.Insertion,
                    getInfo,
                    IntelliSenseOptions.Default
                )

            triggered
            |> Assert.shouldBeEqualWith
                shouldBeTriggered
                "FSharpCompletionProvider.ShouldTriggerCompletionAux() should compute the correct result"

    [<Fact>]
    let ShouldNotTriggerCompletionAfterAnyTriggerOtherThanInsertionOrDeletion () =
        for triggerKind in [ CompletionTriggerKind.Invoke; CompletionTriggerKind.Snippets ] do
            let fileContents = "System.Console.WriteLine(123)"
            let caretPosition = fileContents.IndexOf("rite")
            let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
            let getInfo () = documentId, filePath, []

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    triggerKind,
                    getInfo,
                    IntelliSenseOptions.Default
                )

            Assert.False(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

    [<Fact>]
    let ShouldNotTriggerCompletionInStringLiterals () =
        let fileContents = "let literal = \"System.Console.WriteLine()\""
        let caretPosition = fileContents.IndexOf("System.")
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.False(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

    [<Fact>]
    let ShouldNotTriggerCompletionInComments () =
        let fileContents =
            """
(*
This is a comment
System.Console.WriteLine()
*)
"""

        let caretPosition = fileContents.IndexOf("System.")
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.False(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

    [<Fact>]
    let ShouldTriggerCompletionInInterpolatedString () =
        let fileContents =
            """

let x = 1
let y = 2
let z = $"abc  {System.Console.WriteLine(x + y)} def"
"""

        let testCases =
            [
                ("x", true)
                ("y", true)
                ("1", false)
                ("2", false)
                ("x +", false)
                ("Console.Write", false)
                ("System.", true)
                ("Console.", true)
            ]

        for (marker, shouldBeTriggered) in testCases do
            let caretPosition = fileContents.IndexOf(marker) + marker.Length
            let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
            let getInfo () = documentId, filePath, []

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    CompletionTriggerKind.Insertion,
                    getInfo,
                    IntelliSenseOptions.Default
                )

            triggered
            |> Assert.shouldBeEqualWith
                shouldBeTriggered
                $"FSharpCompletionProvider.ShouldTriggerCompletionAux() should compute the correct result for marker '{marker}"

    [<Fact>]
    let ShouldNotTriggerCompletionInExcludedCode () =
        let fileContents =
            """
#if UNDEFINED
System.Console.WriteLine()
#endif
"""

        let caretPosition = fileContents.IndexOf("System.")
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.False(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

    [<Fact>]
    let ShouldNotTriggerCompletionInOperatorWithDot () =
        // Simulate mistyping '|>' as '|.'
        let fileContents =
            """
let f() =
    12.0 |. sqrt
"""

        let caretPosition = fileContents.IndexOf("|.")
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.False(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger on operators")

    [<Fact>]
    let ShouldTriggerCompletionInAttribute () =
        let fileContents =
            """
[<A
module Foo = module end
"""

        let marker = "A"
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.True(triggered, "Completion should trigger on Attributes.")

    [<Fact>]
    let ShouldTriggerCompletionAfterDerefOperator () =
        let fileContents =
            """
let foo = ref 12
printfn "%d" !f
"""

        let marker = "!f"
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.True(triggered, "Completion should trigger after typing an identifier that follows a dereference operator (!).")

    [<Fact>]
    let ShouldTriggerCompletionAfterAddressOfOperator () =
        let fileContents =
            """
type Point = { mutable X: int; mutable Y: int }
let pnt = { X = 1; Y = 2 }
use ptr = fixed &p
"""

        let marker = "&p"
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.True(triggered, "Completion should trigger after typing an identifier that follows an addressOf operator (&).")

    [<Fact>]
    let ShouldTriggerCompletionAfterArithmeticOperation () =
        let fileContents =
            """
let xVal = 1.0
let yVal = 2.0
let zVal

xVal+y
xVal-y
xVal*y
xVal/y
xVal%y
xVal**y
"""

        let markers = [ "+y"; "-y"; "*y"; "/y"; "%y"; "**y" ]

        for marker in markers do
            let caretPosition = fileContents.IndexOf(marker) + marker.Length
            let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
            let getInfo () = documentId, filePath, []

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    CompletionTriggerKind.Insertion,
                    getInfo,
                    IntelliSenseOptions.Default
                )

            Assert.True(triggered, "Completion should trigger after typing an identifier that follows a mathematical operation")

    [<Fact>]
    let ShouldTriggerCompletionAtStartOfFileWithInsertion =
        let fileContents =
            """
l"""

        let marker = "l"
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo () = documentId, filePath, []

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                getInfo,
                IntelliSenseOptions.Default
            )

        Assert.True(
            triggered,
            "Completion should trigger after typing an Insertion character at the top of the file, e.g. a function definition in a new script file."
        )

    [<Fact>]
    let ShouldDisplayTypeMembers () =
        let fileContents =
            """
type T1() =
    member this.M1 = 5
    member this.M2 = "literal"

[<EntryPoint>]
let main argv =
    let obj = T1()
    obj.
"""

        VerifyCompletionList(fileContents, "obj.", [ "M1"; "M2" ], [ "System" ])

    [<Fact>]
    let ShouldDisplaySystemNamespace () =
        let fileContents =
            """
type T1 =
    member this.M1 = 5
    member this.M2 = "literal"
System.Console.WriteLine()
"""

        VerifyCompletionList(fileContents, "System.", [ "Console"; "Array"; "String" ], [ "T1"; "M1"; "M2" ])

    [<Fact>]
    let ShouldDisplaySystemNamespaceInInterpolatedString () =
        let fileContents =
            """
type T1 =
    member this.M1 = 5
    member this.M2 = "literal"
let x = $"1 not the same as {System.Int32.MaxValue} is it"
"""

        VerifyCompletionList(fileContents, "System.", [ "Console"; "Array"; "String" ], [ "T1"; "M1"; "M2" ])

    [<Fact>]
    let ``Class instance members are ordered according to their kind and where they are defined (simple case, by a variable)`` () =
        let fileContents =
            """
type Base() =
    member _.BaseMethod() = 1
    member _.BaseProp = 1

type Class() = 
    inherit Base()
    member this.MineMethod() = 1
    member this.MineProp = 1

let x = Class()
x.
"""

        let expected =
            [
                "MineProp"
                "BaseProp"
                "MineMethod"
                "BaseMethod"
                "Equals"
                "GetHashCode"
                "GetType"
                "ToString"
            ]

        VerifyCompletionListExactly(fileContents, "x.", expected)

    [<Fact>]
    let ``Class instance members are ordered according to their kind and where they are defined (simple case, by a constructor)`` () =
        let fileContents =
            """
type Base() =
    member _.BaseMethod() = 1
    member _.BaseProp = 1

type Class() = 
    inherit Base()
    member this.MineMethod() = 1
    member this.MineProp = 1

let x = Class().
"""

        let expected =
            [
                "MineProp"
                "BaseProp"
                "MineMethod"
                "BaseMethod"
                "Equals"
                "GetHashCode"
                "GetType"
                "ToString"
            ]

        VerifyCompletionListExactly(fileContents, "let x = Class().", expected)

    [<Fact>]
    let ``Class static members are ordered according to their kind and where they are defined`` () =
        let fileContents =
            """
type Base() =
    static member BaseStaticMethod() = 1
    static member BaseStaticProp = 1

type Class() = 
    inherit Base()
    static member MineStaticMethod() = 1
    static member MineStaticProp = 2

Class.
"""

        let expected =
            [ "MineStaticProp"; "BaseStaticProp"; "MineStaticMethod"; "BaseStaticMethod" ]

        VerifyCompletionListExactly(fileContents, "Class.", expected)

    [<Fact>]
    let ``Class instance members are ordered according to their kind and where they are defined (complex case)`` () =
        let fileContents =
            """
type Base() =
    inherit System.Collections.Generic.List<int>
    member _.BaseMethod() = 1
    member _.BaseProp = 1

type Class() = 
    inherit Base()
    member this.MineMethod() = 1
    member this.MineProp = 1

let x = Class()
x.
"""

        let expected =
            [
                "MineProp"
                "BaseProp"
                "Capacity"
                "Count"
                "Item"
                "MineMethod"
                "Add"
                "AddRange"
                "AsReadOnly"
                "BaseMethod"
                "BinarySearch"
                "Clear"
                "Contains"
                "ConvertAll"
                "CopyTo"
                "Equals"
                "Exists"
                "Find"
                "FindAll"
                "FindIndex"
                "FindLast"
                "FindLastIndex"
                "ForEach"
                "GetEnumerator"
                "GetHashCode"
                "GetRange"
                "GetType"
                "IndexOf"
                "Insert"
                "InsertRange"
                "LastIndexOf"
                "Remove"
                "RemoveAll"
                "RemoveAt"
                "RemoveRange"
                "Reverse"
                "Sort"
                "ToArray"
                "ToString"
                "TrimExcess"
                "TrueForAll"
            ]

        VerifyCompletionListExactly(fileContents, "x.", expected)

    [<Fact>]
    let ``Constructing a new class with object initializer syntax`` () =
        let fileContents =
            """
type A() =
    member val SettableProperty = 1 with get, set
    member val AnotherSettableProperty = 1 with get, set
    member val NonSettableProperty = 1
    
let _ = new A(Setta)
"""

        let expected = [ "SettableProperty"; "AnotherSettableProperty" ]
        let notExpected = [ "NonSettableProperty" ]
        VerifyCompletionList(fileContents, "(Setta", expected, notExpected)

    [<Fact>]
    let ``Constructing a new class with object initializer syntax and verifying 'at' character doesn't exist.`` () =
        let fileContents =
            """
type A() =
    member val SettableProperty = 1 with get, set
    member val AnotherSettableProperty = 1 with get, set
    member val NonSettableProperty = 1
    
let _ = new A(Setta)
"""

        let expected = []

        let notExpected =
            [ "SettableProperty@"; "AnotherSettableProperty@"; "NonSettableProperty@" ]

        VerifyCompletionList(fileContents, "(Setta", expected, notExpected)

    [<Fact(Skip = "https://github.com/dotnet/fsharp/issues/3954")>]
    let ``Constructing a new fully qualified class with object initializer syntax without ending paren`` () =
        let fileContents =
            """
module M =
    type A() =
        member val SettableProperty = 1 with get, set
        member val AnotherSettableProperty = 1 with get, set
        member val NonSettableProperty = 1
    
let _ = new M.A(Setta
"""

        let expected = [ "SettableProperty"; "AnotherSettableProperty" ]
        let notExpected = [ "NonSettableProperty" ]
        VerifyCompletionList(fileContents, "(Setta", expected, notExpected)

    [<Fact>]
    let ``Extension methods go after everything else, extension properties are treated as normal ones`` () =
        let fileContents =
            """
open System.Collections.Generic

type List<'a> with
    member _.ExtensionProp = 1
    member _.ExtensionMeth() = 1

List().
"""

        let expected =
            [
                "Capacity"
                "Count"
                "Item"
                "ExtensionProp"
                "Add"
                "AddRange"
                "AsReadOnly"
                "BinarySearch"
                "Clear"
                "Contains"
                "ConvertAll"
                "CopyTo"
                "Exists"
                "Find"
                "FindAll"
                "FindIndex"
                "FindLast"
                "FindLastIndex"
                "ForEach"
                "GetEnumerator"
                "GetRange"
                "IndexOf"
                "Insert"
                "InsertRange"
                "LastIndexOf"
                "Remove"
                "RemoveAll"
                "RemoveAt"
                "RemoveRange"
                "Reverse"
                "Sort"
                "ToArray"
                "TrimExcess"
                "TrueForAll"
                "Equals"
                "GetHashCode"
                "GetType"
                "ToString"
                "ExtensionMeth"
            ]

        VerifyCompletionListExactly(fileContents, "List().", expected)

    [<Fact>]
    let ``Completion for open contains namespaces and static types`` () =
        let fileContents =
            """
open type System.Ma
"""

        let expected = [ "Management"; "Math" ] // both namespace and static type
        VerifyCompletionList(fileContents, "System.Ma", expected, [])

    [<Fact>]
    let ``No completion on type name at declaration site`` () =
        let fileContents =
            """
type T

"""

        VerifyNoCompletionList(fileContents, "type T")

    [<Fact>]
    let ``No completion on name of unfinished function declaration`` () =
        let fileContents =
            """
let f

"""

        VerifyNoCompletionList(fileContents, "let f")

    [<Fact>]
    let ``No completion on name of value declaration`` () =
        let fileContents =
            """
let xyz = 1

"""

        VerifyNoCompletionList(fileContents, "let xy")

    [<Fact>]
    let ``No completion on name of function declaration`` () =
        let fileContents =
            """
let foo x = 1

"""

        VerifyNoCompletionList(fileContents, "let fo")

    [<Fact>]
    let ``No completion on name of tupled function declaration`` () =
        let fileContents =
            """
let foo (x, y) = 1

"""

        VerifyNoCompletionList(fileContents, "let fo")

    [<Fact>]
    let ``No completion on member name at declaration site`` () =
        let fileContents =
            """
type T() =
    member this.M
"""

        VerifyNoCompletionList(fileContents, "member this.M")

    [<Fact>]
    let ``No completion on function first argument name`` () =
        let fileContents =
            """
let func (p
"""

        VerifyNoCompletionList(fileContents, "let func (p")

    [<Fact>]
    let ``No completion on function subsequent argument name`` () =
        let fileContents =
            """
let func (p, h
"""

        VerifyNoCompletionList(fileContents, "let func (p, h")

    [<Fact>]
    let ``No completion on curried function subsequent argument name`` () =
        let fileContents =
            """
let func (p) (h
"""

        VerifyNoCompletionList(fileContents, "let func (p) (h")

    [<Fact>]
    let ``No completion on method first argument name`` () =
        let fileContents =
            """
type T() =
    member this.M(p) = ()
"""

        VerifyNoCompletionList(fileContents, "member this.M(p")

    [<Fact>]
    let ``No completion on method subsequent argument name`` () =
        let fileContents =
            """
type T() =
    member this.M(p:int, h ) = ()
"""

        VerifyNoCompletionList(fileContents, "member this.M(p:int, h")

    [<Fact>]
    let ``Completion list on abstract member type signature contains modules and types but not keywords or functions`` =
        let fileContents =
            """
type Interface =
    abstract member Eat: l
"""

        VerifyCompletionList(fileContents, "Eat: l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``Provide completion on first function argument type hint`` () =
        let fileContents =
            """
let func (p:i
"""

        VerifyCompletionList(fileContents, "let func (p:i", [ "int" ], [])

    [<Fact>]
    let ``Provide completion on subsequent function argument type hint`` () =
        let fileContents =
            """
let func (p:int, h:f
"""

        VerifyCompletionList(fileContents, "let func (p:int, h:f", [ "float" ], [])

    [<Fact>]
    let ``Provide completion on local function argument type hint`` () =
        let fileContents =
            """
let top () =
    let func (p:i
"""

        VerifyCompletionList(fileContents, "let func (p:i", [ "int" ], [])

    [<Fact>]
    let ``No completion on implicit constructor first argument name`` () =
        let fileContents =
            """
type T(p) =
"""

        VerifyNoCompletionList(fileContents, "type T(p")

    [<Fact>]
    let ``No completion on implicit constructor subsequent argument name`` () =
        let fileContents =
            """
type T(p:int, h) =
"""

        VerifyNoCompletionList(fileContents, "type T(p:int, h")

    [<Fact>]
    let ``Provide completion on implicit constructor argument type hint`` () =
        let fileContents =
            """
type T(p:i) =
"""

        VerifyCompletionList(fileContents, "type T(p:i", [ "int" ], [])

    [<Fact>]
    let ``No completion on lambda argument name`` () =
        let fileContents =
            """
let _ = fun (p) -> ()
"""

        VerifyNoCompletionList(fileContents, "let _ = fun (p")

    [<Fact>]
    let ``No completion on lambda argument name2`` () =
        let fileContents =
            """
let _ = fun (p: int) -> ()
"""

        VerifyNoCompletionList(fileContents, "let _ = fun (p")

    [<Fact>]
    let ``Completions on lambda argument type hint contain modules and types but not keywords or functions`` () =
        let fileContents =
            """
let _ = fun (p:l) -> ()
"""

        VerifyCompletionList(fileContents, "let _ = fun (p:l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``Completions in match clause type test contain modules and types but not keywords or functions`` () =
        let fileContents =
            """
match box 5 with
| :? l as x -> ()
| _ -> ()
"""

        VerifyCompletionList(fileContents, ":? l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``Completions in catch clause type test contain modules and types but not keywords or functions`` () =
        let fileContents =
            """
try
    ()
with :? l as x ->
    ()
"""

        VerifyCompletionList(fileContents, ":? l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``Extensions.Bug5162`` () =
        let fileContents =
            """
module Extensions =
    type System.Object with
        member x.P = 1
module M2 =
    let x = 1
    Ext
"""

        VerifyCompletionList(fileContents, "    Ext", [ "Extensions"; "ExtraTopLevelOperators" ], [])

    [<Fact>]
    let ``Custom operations should be at the top of completion list inside computation expression`` () =
        let fileContents =
            """
let joinLocal = 1

let _ =
    query {
        for i in 1..10 do
        select i
        join
    }
"""

        VerifyCompletionList(fileContents, "        join", [ "groupJoin"; "join"; "leftOuterJoin"; "joinLocal" ], [])

    [<Fact>]
    let ``Byref Extension Methods`` () =
        let fileContents =
            """
module Extensions =
    open System
    open System.Runtime.CompilerServices

    [<Struct>]
    type Message = Message of String

    [<Sealed; AbstractClass; Extension>]
    type MessageExtensions private () =
        let (|Message|) (Message message) = message

        [<Extension>]
        static member Print (Message message : Message) =
            printfn "%s" message

        [<Extension>]
        static member PrintRef (Message message : inref<Message>) =
            printfn "%s" message

    let wrappedMessage = Message "Hello World"

    wrappedMessage.
"""

        VerifyCompletionList(fileContents, "wrappedMessage.", [ "PrintRef" ], [])

    [<Fact>]
    let ``Completion list span works with underscore in identifier`` () =
        let fileContents =
            """
let x = A.B_C
"""

        VerifyCompletionListSpan(fileContents, "A.B_C", "B_C")

    [<Fact>]
    let ``Completion list span works with digit in identifier`` () =
        let fileContents =
            """
let x = A.B1C
"""

        VerifyCompletionListSpan(fileContents, "A.B1C", "B1C")

    [<Fact>]
    let ``Completion list span works with enclosed backtick identifier`` () =
        let fileContents =
            """
let x = A.``B C``
"""

        VerifyCompletionListSpan(fileContents, "A.``B C``", "``B C``")

    [<Fact>]
    let ``Completion list span works with partial backtick identifier`` () =
        let fileContents =
            """
let x = A.``B C
"""

        VerifyCompletionListSpan(fileContents, "A.``B C", "``B C")

    [<Fact>]
    let ``Completion list span works with first of multiple enclosed backtick identifiers`` () =
        let fileContents =
            """
let x = A.``B C`` + D.``E F``
"""

        VerifyCompletionListSpan(fileContents, "A.``B C``", "``B C``")

    [<Fact>]
    let ``Completion list span works with last of multiple enclosed backtick identifiers`` () =
        let fileContents =
            """
let x = A.``B C`` + D.``E F``
"""

        VerifyCompletionListSpan(fileContents, "D.``E F``", "``E F``")

    [<Fact>]
    let ``No completion on record field identifier at declaration site`` () =
        let fileContents =
            """
type A = { le: string }
"""

        VerifyNoCompletionList(fileContents, "le")

    [<Fact>]
    let ``Completion list on record field type at declaration site contains modules, types and type parameters but not keywords or functions``
        ()
        =
        let fileContents =
            """
type A<'lType> = { Field: l }
"""

        VerifyCompletionList(fileContents, "Field: l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``No completion on record stub with no fields at declaration site`` () =
        let fileContents =
            """
type A = {  }
"""

        VerifyNoCompletionList(fileContents, "{ ")

    [<Fact>]
    let ``No completion on record outside of all fields at declaration site`` () =
        let fileContents =
            """
type A = { Field: string; }
"""

        VerifyNoCompletionList(fileContents, "; ")

    [<Fact>]
    let ``No completion on union case identifier at declaration site`` () =
        let fileContents =
            """
type A =
    | C of string
"""

        VerifyNoCompletionList(fileContents, "| C")

    [<Fact>]
    let ``No completion on union case field identifier at declaration site`` () =
        let fileContents =
            """
type A =
    | Case of blah: int * str: int
"""

        VerifyNoCompletionList(fileContents, "str")

    [<Fact>]
    let ``Completion list on union case type at declaration site contains modules, types and type parameters but not keywords or functions``
        ()
        =
        let fileContents =
            """
type A<'lType> =
    | Case of blah: int * str: l
"""

        VerifyCompletionList(fileContents, "str: l", [ "LanguagePrimitives"; "List"; "lType" ], [ "let"; "log" ])

    [<Fact>]
    let ``Completion list on union case type at declaration site contains modules, types and type parameters but not keywords or functions2``
        ()
        =
        let fileContents =
            """
type A<'lType> =
    | Case of l
"""

        VerifyCompletionList(fileContents, "of l", [ "LanguagePrimitives"; "List"; "lType" ], [ "let"; "log" ])

    [<Fact>]
    let ``Completion list on union case type at declaration site contains type parameter`` () =
        let fileContents =
            """
type A<'keyType> =
    | Case of key
"""

        VerifyCompletionList(fileContents, "of key", [ "keyType" ], [])

    [<Fact>]
    let ``Completion list on type alias contains modules and types but not keywords or functions`` () =
        let fileContents =
            """
type A = l
"""

        VerifyCompletionList(fileContents, "= l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``No completion on enum case identifier at declaration site`` () =
        let fileContents =
            """
type A =
    | C = 0
"""

        VerifyNoCompletionList(fileContents, "| C")

    [<Fact>]
    let ``Completion list in generic function body contains type parameter`` () =
        let fileContents =
            """
let Null<'wrappedType> () =
    Unchecked.defaultof<wrapp>
"""

        VerifyCompletionList(fileContents, "defaultof<wrapp", [ "wrappedType" ], [])

    [<Fact>]
    let ``Completion list in generic method body contains type parameter`` () =
        let fileContents =
            """
type A () =
    member _.Null<'wrappedType> () = Unchecked.defaultof<wrapp>
"""

        VerifyCompletionList(fileContents, "defaultof<wrapp", [ "wrappedType" ], [])

    [<Fact>]
    let ``Completion list in generic class method body contains type parameter`` () =
        let fileContents =
            """
type A<'wrappedType> () =
    member _.Null () = Unchecked.defaultof<wrapp>
"""

        VerifyCompletionList(fileContents, "defaultof<wrapp", [ "wrappedType" ], [])

    [<Fact>]
    let ``Completion list in type application contains modules, types and type parameters but not keywords or functions`` () =
        let fileContents =
            """
let emptyMap<'keyType, 'lValueType> () =
    Map.empty<'keyType, l>
"""

        VerifyCompletionList(fileContents, ", l", [ "LanguagePrimitives"; "List"; "lValueType" ], [ "let"; "log" ])

    [<Fact>]
    let ``Completion list for interface with static abstract method type invocation contains static property with residue`` () =
        let fileContents =
            """
type IStaticProperty<'T when 'T :> IStaticProperty<'T>> =
    static abstract StaticProperty: 'T

let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) =
    'T.StaticProperty
"""

        VerifyCompletionListWithOptions(fileContents, "'T.Stati", [ "StaticProperty" ], [], [| "/langversion:preview" |])

    [<Fact>]
    let ``Completion list for interface with static abstract method type invocation contains static property after dot`` () =
        let fileContents =
            """
type IStaticProperty<'T when 'T :> IStaticProperty<'T>> =
    static abstract StaticProperty: 'T

let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) =
    'T.StaticProperty
"""

        VerifyCompletionListWithOptions(fileContents, "'T.", [ "StaticProperty" ], [], [| "/langversion:preview" |])

    [<Fact>]
    let ``Completion list for SRTP invocation contains static property with residue`` () =
        let fileContents =
            """
let inline f_StaticProperty_SRTP<'T when 'T : (static member StaticProperty: 'T) >() =
    'T.StaticProperty

"""

        VerifyCompletionListWithOptions(fileContents, "'T.Stati", [ "StaticProperty" ], [], [| "/langversion:preview" |])

    [<Fact>]
    let ``Completion list for SRTP invocation contains static property after dot`` () =
        let fileContents =
            """
let inline f_StaticProperty_SRTP<'T when 'T : (static member StaticProperty: 'T) >() =
    'T.StaticProperty

"""

        VerifyCompletionListWithOptions(fileContents, "'T.", [ "StaticProperty" ], [], [| "/langversion:preview" |])

    [<Fact>]
    let ``Completion list for attribute application contains settable members and ctor parameters`` () =
        let fileContents =
            """
type LangAttribute (langParam: int) =
    inherit System.Attribute ()

    member val LangMember1 = 0 with get, set
    member val LangMember2 = 0 with get, set

[<Lang(1)>]
module X =
    [< Lang(2, LangMember1 = 2)>]
    let a = ()

[<  Lang(3, LangMember1 = 3, L)>]
type B () =
    [<   Lang(la)>]
    member _.M = ""

type G = { [<Lang(l)>] f: string }

type A =
    | [<Lang(l)>] A = 1
"""

        // Attribute on module, completing attribute name - settable properties omitted
        VerifyCompletionList(fileContents, "[<La", [ "Lang" ], [ "LangMember1"; "langParam" ])

        // Attribute on let-bound value - LangMember2 is already set, so it's omitted
        VerifyCompletionList(fileContents, "[< Lang(2", [ "langParam"; "LangMember2" ], [ "LangMember1" ])

        // Attribute on type - LangMember1 is already set, so it's omitted
        VerifyCompletionList(fileContents, "[<  Lang(3, LangMember1 = 3, L", [ "LangMember2" ], [ "LangMember1" ])

        // Attribute on member - All settable properties available
        VerifyCompletionList(fileContents, "[<   Lang(l", [ "langParam"; "LangMember1"; "LangMember2" ], [])

        // Attribute on record field - All settable properties available
        VerifyCompletionList(fileContents, "{ [<Lang(l", [ "langParam"; "LangMember1"; "LangMember2" ], [])

        // Attribute on enum case - All settable properties available
        VerifyCompletionList(fileContents, "| [<Lang(l", [ "langParam"; "LangMember1"; "LangMember2" ], [])

    [<Fact>]
    let ``Completion list for nested copy and update contains correct record fields, nominal`` () =
        let fileContents =
            """
type AnotherNestedRecTy = { A: int }

type NestdRecTy = { B: string; C: AnotherNestedRecTy }

module F =
    type RecTy = { D: NestdRecTy; E: string option }

open F

let t1 = { D = { B = "t1"; C = { A = 1; } }; E = None; }

let t2 = { t1 with D.B = "12" }

let t3 = { t2 with F.RecTy.d }

let t4 = { t2 with F.RecTy.D. }

let t5 = { t2 with F.RecTy.D.C. }

let t6 = { t2 with E. }

let t7 = { t2 with D.B. }

let t8 = { t2 with F. }

let t9 = { t2 with d }

let t10 x = { x with d } 

let t11 = { t2 with NestdRecTy.C. }

let t12 x = { x with F.RecTy.d }

let t13 x = { x with RecTy.D. }
"""

        VerifyCompletionListExactly(fileContents, "t1 with ", [ "D"; "E" ])
        VerifyCompletionListExactly(fileContents, "t1 with D.", [ "B"; "C" ])

        VerifyCompletionListExactly(fileContents, "let t3 = { t2 with F.RecTy.d", [ "D"; "E" ])

        VerifyCompletionListExactly(fileContents, "let t4 = { t2 with F.RecTy.D.", [ "B"; "C" ])

        VerifyCompletionListExactly(fileContents, "let t5 = { t2 with F.RecTy.D.C.", [ "A" ])

        VerifyNoCompletionList(fileContents, "let t6 = { t2 with E.")

        VerifyNoCompletionList(fileContents, "let t7 = { t2 with D.B.")

        VerifyCompletionListExactly(fileContents, "let t8 = { t2 with F.", [ "D"; "E"; "RecTy" ])

        VerifyCompletionListExactly(fileContents, "let t9 = { t2 with d", [ "D"; "E" ])

        // The type of `x` is not known, so show fields of records in scope
        VerifyCompletionList(fileContents, "let t10 x = { x with d", [ "A"; "B"; "C"; "D"; "E" ], [])

        VerifyNoCompletionList(fileContents, "let t11 = { t2 with NestdRecTy.C.")

        VerifyCompletionListExactly(fileContents, "let t12 x = { x with F.RecTy.d", [ "D"; "E" ])

        VerifyCompletionListExactly(fileContents, "let t13 x = { x with RecTy.D.", [ "B"; "C" ])
