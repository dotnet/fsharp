// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

module CompletionProviderTests =

    open System
    open System.Linq
    open System.Threading
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.Completion
    open Microsoft.CodeAnalysis.Text
    open Microsoft.VisualStudio.FSharp.Editor
    open FSharp.Editor.Tests.Helpers
    open Xunit
    open FSharp.Test

    let filePath = "C:\\test.fs"

    let mkGetInfo documentId =
        fun () -> documentId, filePath, [], (Some "preview"), None

    let formatCompletions (completions: string seq) =
        "\n\t" + String.Join("\n\t", completions)

    let VerifyCompletionListWithOptions (fileContents: string, marker: string, expected: string list, unexpected: string list, opts) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length

        let document =
            RoslynTestHelpers.CreateSolution(fileContents, extraFSharpProjectOtherOptions = Array.ofSeq opts)
            |> RoslynTestHelpers.GetSingleDocument

        let results =
            let task =
                FSharpCompletionProvider.ProvideCompletionsAsyncAux(document, caretPosition, (fun _ -> [||]))
                |> CancellableTask.start CancellationToken.None

            task.Result |> Seq.map (fun result -> result.DisplayText)

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
            RoslynTestHelpers.CreateSolution(fileContents, extraFSharpProjectOtherOptions = Array.ofSeq opts)
            |> RoslynTestHelpers.GetSingleDocument

        let actual =
            let task =
                FSharpCompletionProvider.ProvideCompletionsAsyncAux(document, caretPosition, (fun _ -> [||]))
                |> CancellableTask.start CancellationToken.None

            task.Result
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
            CompletionUtils.getDefaultCompletionListSpan (
                sourceText,
                caretPosition,
                documentId,
                filePath,
                [],
                None,
                None,
                CancellationToken.None
            )

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

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    CompletionTriggerKind.Insertion,
                    mkGetInfo documentId,
                    IntelliSenseOptions.Default,
                    CancellationToken.None
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

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    triggerKind,
                    mkGetInfo documentId,
                    IntelliSenseOptions.Default,
                    CancellationToken.None
                )

            Assert.False(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

    [<Fact>]
    let ShouldNotTriggerCompletionInStringLiterals () =
        let fileContents = "let literal = \"System.Console.WriteLine()\""
        let caretPosition = fileContents.IndexOf("System.")
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    CompletionTriggerKind.Insertion,
                    mkGetInfo documentId,
                    IntelliSenseOptions.Default,
                    CancellationToken.None
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

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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

            let triggered =
                FSharpCompletionProvider.ShouldTriggerCompletionAux(
                    SourceText.From(fileContents),
                    caretPosition,
                    CompletionTriggerKind.Insertion,
                    mkGetInfo documentId,
                    IntelliSenseOptions.Default,
                    CancellationToken.None
                )

            Assert.True(triggered, "Completion should trigger after typing an identifier that follows a mathematical operation")

    [<Fact>]
    let ShouldTriggerCompletionAtStartOfFileWithInsertion () =
        let fileContents =
            """
l"""

        let marker = "l"
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())

        let triggered =
            FSharpCompletionProvider.ShouldTriggerCompletionAux(
                SourceText.From(fileContents),
                caretPosition,
                CompletionTriggerKind.Insertion,
                mkGetInfo documentId,
                IntelliSenseOptions.Default,
                CancellationToken.None
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
    let ``No completion on nested module identifier, incomplete`` () =
        let fileContents =
            """
    module Namespace.Top

    module Nest

    let a = ()
    """

        VerifyNoCompletionList(fileContents, "Nest")

    [<Fact>]
    let ``No completion on nested module identifier`` () =
        let fileContents =
            """
    namespace N

    module Nested =
        do ()
    """

        VerifyNoCompletionList(fileContents, "Nested")

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
        VerifyNoCompletionList(fileContents, "member this.M(p")

    [<Fact>]
    let ``Completion list on abstract member type signature contains modules and types but not keywords or functions`` () =
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
        VerifyNoCompletionList(fileContents, "type T(p")

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
    let ``Completion list at record declaration site contains type parameter and record`` () =
        let fileContents =
            """
type ARecord<'keyType> = {
    Field: key
    Field2: AR
    Field3: ARecord<ke
}
    with
        static member Create () = { F }
        member x.F () = typeof<AR
        member _.G = typeof<ke

let x = { F }
"""

        VerifyCompletionList(fileContents, ": key", [ "keyType" ], [])
        VerifyCompletionList(fileContents, ": AR", [ "ARecord" ], [])
        VerifyCompletionList(fileContents, ": ARecord<ke", [ "ARecord" ], [])
        VerifyCompletionList(fileContents, "typeof<AR", [ "ARecord" ], [])
        VerifyCompletionList(fileContents, "typeof<ke", [ "keyType" ], [])
        VerifyCompletionList(fileContents, "Create () = { F", [ "Field"; "Field2"; "Field3" ], [])
        VerifyCompletionList(fileContents, "let x = { F", [ "Field"; "Field2"; "Field3" ], [])

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
    let ``Completion list at union declaration site contains type parameter and union`` () =
        let fileContents =
            """
type AUnion<'keyType> =
    | Case of key
    | Case2 of AU
    | Case3 of AUnion<ke

    with
        static member Create () = Cas
        member x.F () = typeof<AU
        member _.G = typeof<ke

let x = c
"""

        VerifyCompletionList(fileContents, "of key", [ "keyType" ], [])
        VerifyCompletionList(fileContents, "of AU", [ "AUnion" ], [])
        VerifyCompletionList(fileContents, "of AUnion<ke", [ "keyType" ], [])
        VerifyCompletionList(fileContents, "typeof<AU", [ "AUnion" ], [])
        VerifyCompletionList(fileContents, "typeof<ke", [ "keyType" ], [])
        VerifyCompletionList(fileContents, "= Cas", [ "Case"; "Case2"; "Case3" ], [])
        VerifyCompletionList(fileContents, "let x = c", [ "Case"; "Case2"; "Case3" ], [])

    [<Fact>]
    let ``Completion list on a union identifier and a dot in a match clause contains union cases`` () =
        let fileContents =
            """
type DU =
    | A
    | B
    | C

match A with
|  DU. -> ()

match A with
| B
|   DU.
| A -> ()

match A with
| DU.
"""

        VerifyCompletionListExactly(fileContents, "| DU.", [ "A"; "B"; "C" ])
        VerifyCompletionListExactly(fileContents, "|  DU.", [ "A"; "B"; "C" ])
        VerifyCompletionListExactly(fileContents, "|   DU.", [ "A"; "B"; "C" ])

    [<Fact>]
    let ``Completion list on a union identifier and a dot in a match clause contains union cases2`` () =
        let fileContents =
            """
type DU =
    | A
    | B
    | C

match None with
| Some DU.

match A, () with
| DU.

match A, () with
|  Some DU., _ -> ()

match (), A with
| _, DU.
"""

        VerifyCompletionListExactly(fileContents, "| Some DU.", [ "A"; "B"; "C" ])
        VerifyCompletionListExactly(fileContents, "| DU.", [ "A"; "B"; "C" ])
        VerifyCompletionListExactly(fileContents, "|  Some DU.", [ "A"; "B"; "C" ])
        VerifyCompletionListExactly(fileContents, "| _, DU.", [ "A"; "B"; "C" ])

    [<Fact>]
    let ``Completion list on a module identifier and a dot in a match clause contains module contents`` () =
        let fileContents =
            """
module M =
    type DU =
        | A

match M.A with
| M. -> ()

match M.A with
|  M.DU. ->
"""

        VerifyCompletionListExactly(fileContents, "| M.", [ "A"; "DU" ])
        VerifyCompletionListExactly(fileContents, "|  M.DU.", [ "A" ])

    [<Fact>]
    let ``Completion list on type alias contains modules and types but not keywords or functions`` () =
        let fileContents =
            """
type A = l
"""

        VerifyCompletionList(fileContents, "= l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``Completion list on return type annotation contains modules and types but not keywords or functions`` () =
        let fileContents =
            """
let a: l
let b (): l

type X =
    member _.c: l
    member x.d (): l
    static member e: l
"""

        VerifyCompletionList(fileContents, "let a: l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])
        VerifyCompletionList(fileContents, "let b (): l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])
        VerifyCompletionList(fileContents, "member _.c: l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])
        VerifyCompletionList(fileContents, "member x.d (): l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])
        VerifyCompletionList(fileContents, "static member e: l", [ "LanguagePrimitives"; "List" ], [ "let"; "log" ])

    [<Fact>]
    let ``No completion on enum case identifier at declaration site`` () =
        let fileContents =
            """
let [<Literal>] lit = 1

type A =
    | C = 0
    | D = l
"""

        VerifyNoCompletionList(fileContents, "| C")
        VerifyCompletionList(fileContents, "| D = l", [ "lit" ], [])

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

type NestedRecTy = { B: string; C: AnotherNestedRecTy }

module F =
    type RecTy = { D: NestedRecTy; E: string option }

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

let t11 = { t2 with NestedRecTy.C. }

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

        VerifyNoCompletionList(fileContents, "let t11 = { t2 with NestedRecTy.C.")

        VerifyCompletionListExactly(fileContents, "let t12 x = { x with F.RecTy.d", [ "D"; "E" ])

        VerifyCompletionListExactly(fileContents, "let t13 x = { x with RecTy.D.", [ "B"; "C" ])

    [<Fact>]
    let ``Completion list for nested copy and update contains correct record fields, mixed nominal and anonymous`` () =
        let fileContents =
            """
type AnotherNestedRecTy = { A: int }

type NestedRecTy = { B: string; C: {| C: AnotherNestedRecTy |} }

type RecTy = { D: NestedRecTy; E: {| a: string |} }

let t1 x = { x with D.C.C.A = 12; E.a = "a" }

let t2 (x: {| D: NestedRecTy; E: {| a: string |} |}) = {| x with E.a = "a"; D.B = "z" |}
"""

        VerifyCompletionListExactly(fileContents, "let t1 x = { x with D.", [ "B"; "C" ])
        VerifyCompletionListExactly(fileContents, "let t1 x = { x with D.C.", [ "C" ])
        VerifyCompletionListExactly(fileContents, "let t1 x = { x with D.C.C.", [ "A" ])
        VerifyCompletionListExactly(fileContents, "let t1 x = { x with D.C.C.A = 12; ", [ "D"; "E" ])
        VerifyCompletionListExactly(fileContents, "let t1 x = { x with D.C.C.A = 12; E.", [ "a" ])

        VerifyCompletionListExactly(fileContents, "let t2 (x: {| D: NestedRecTy; E: {| a: string |} |}) = {| x with ", [ "D"; "E" ])
        VerifyCompletionListExactly(fileContents, "let t2 (x: {| D: NestedRecTy; E: {| a: string |} |}) = {| x with E.", [ "a" ])

        VerifyCompletionListExactly(
            fileContents,
            "let t2 (x: {| D: NestedRecTy; E: {| a: string |} |}) = {| x with E.a = \"a\"; ",
            [ "D"; "E" ]
        )

        VerifyCompletionListExactly(
            fileContents,
            "let t2 (x: {| D: NestedRecTy; E: {| a: string |} |}) = {| x with E.a = \"a\"; D.",
            [ "B"; "C" ]
        )

    [<Fact>]
    let ``Completion list for nested copy and update contains correct record fields, nominal, recursive, generic`` () =
        let fileContents =
            """
type RecordA<'a> = { Foo: 'a; Bar: int; Zoo: RecordA<'a> }

let fz (a: RecordA<int>) = { a with Zoo.F = 1; Zoo.Zoo.B = 2; F } 
"""

        VerifyCompletionListExactly(fileContents, "with Zoo.F", [ "Bar"; "Foo"; "Zoo" ])
        VerifyCompletionListExactly(fileContents, "Zoo.Zoo.B", [ "Bar"; "Foo"; "Zoo" ])
        VerifyCompletionListExactly(fileContents, "; F", [ "Bar"; "Foo"; "Zoo" ])

    [<Fact>]
    let ``Anonymous record fields have higher priority than methods`` () =
        let fileContents =
            """
let x = [ {| Goo = 1; Foo = "foo" |} ]
x[0].
"""

        VerifyCompletionListExactly(fileContents, "x[0].", [ "Foo"; "Goo"; "Equals"; "GetHashCode"; "GetType"; "ToString" ])

    [<Fact>]
    let ``Completion list contains suggested names for union case field pattern with one field, and no valrefs other than literals`` () =
        let fileContents =
            """
let logV = 1
let [<Literal>] logLit = 1

type DU = A of logField: int

let (|Even|Odd|) input = Odd

match A 1 with
| A l -> ()
"""

        VerifyCompletionList(fileContents, "| A", [ "A"; "DU"; "logLit"; "Even"; "Odd"; "System" ], [ "logV"; "failwith"; "false" ])
        VerifyCompletionList(fileContents, "| A l", [ "logField"; "logLit"; "num" ], [ "logV"; "log" ])

    [<Fact>]
    let ``Completion list contains suggested names for union case field pattern in a match clause unless the field name is generated`` () =
        let fileContents =
            """
type Du =
    | C of first: Du * rest: Du list
    | D of int

let x du =
    match du with
    | C (f, [ D i; C (first = s) ]) -> ()
    | C (rest = r) -> ()
    | _ -> ()
"""

        VerifyCompletionList(fileContents, "| C (f", [ "first"; "du" ], [ "rest"; "item"; "num" ])
        VerifyCompletionList(fileContents, "| C (f, [ D i", [ "num" ], [ "rest"; "first"; "du"; "item" ])
        VerifyCompletionList(fileContents, "| C (f, [ D i; C (first = s", [ "first"; "du" ], [ "rest"; "num" ])
        VerifyCompletionList(fileContents, "| C (rest = r", [ "rest"; "list" ], [ "first"; "du"; "item"; "num" ])

    [<Fact>]
    let ``Completion list does not contain suggested names which are already used in the same pattern`` () =
        let fileContents =
            """
type Du =
    | C of first: string option * rest: Du list

let x (du: Du list) =
    match du with
    | [ C (first = first); C (first = f) ] -> ()
    | [ C (first, rest); C (f, l) ] -> ()
    | _ -> ()
"""

        VerifyCompletionList(fileContents, "| [ C (first = first); C (first = f", [ "option" ], [ "first" ])
        VerifyCompletionList(fileContents, "| [ C (first, rest); C (f", [ "option" ], [ "first" ])
        VerifyCompletionList(fileContents, "| [ C (first, rest); C (f, l", [ "list" ], [ "rest" ])

    [<Fact>]
    let ``Completion list contains relevant items for the correct union case field pattern before its identifier has been typed`` () =
        let fileContents =
            """
type Du =
    | C of first: Du * second: Result<int, string>

let x du =
    match du with
    | C () -> ()
    | C  (ff, ) -> ()
    | C  (first = f;) -> ()
"""

        // This has the potential to become either a positional field pattern or a named field identifier, so we want to see completions for both:
        // - suggested name based on the first field's identifier and a suggested name based on the first field's type
        // - names of all fields
        VerifyCompletionList(fileContents, "| C (", [ "first"; "du"; "second" ], [ "result" ])
        VerifyCompletionList(fileContents, "| C  (ff, ", [ "second"; "result" ], [ "first"; "du" ])
        VerifyCompletionListExactly(fileContents, "| C  (first = f;", [ "second" ])

    [<Fact>]
    let ``Completion list contains suggested names for union case field pattern in a let binding, lambda and member`` () =
        let fileContents =
            """
type Ids = Ids of customerId: int * orderId: string option

let x (Ids (c)) = ()
let xy (Ids (c, o)) = ()
let xyz (Ids c) = ()

fun (Ids (c, o)) -> ()
fun (Some v) -> ()

type C =
    member _.M (Ids (c, o)) = ()


type MyAlias = int

type Id2<'a> = Id2 of fff: 'a

let r: Id2<MyAlias> = Id2 3

match r with
| Id2 (a) -> ()
"""

        VerifyCompletionList(fileContents, "let x (Ids (c", [ "customerId"; "num" ], [])
        VerifyCompletionList(fileContents, "let xy (Ids (c", [ "customerId"; "num" ], [])
        VerifyCompletionList(fileContents, "let xy (Ids (c, o", [ "orderId"; "option" ], [])
        VerifyCompletionList(fileContents, "let xyz (Ids c", [ "option" ], [ "customerId"; "orderId"; "num" ]) // option is on the list as a type
        VerifyCompletionList(fileContents, "fun (Ids (c", [ "customerId"; "num" ], [])
        VerifyCompletionList(fileContents, "fun (Ids (c, o", [ "orderId"; "option" ], [])
        VerifyCompletionList(fileContents, "fun (Some v", [ "value" ], [])
        VerifyCompletionList(fileContents, "member _.M (Ids (c", [ "customerId"; "num" ], [ "orderId" ])

        // Respecting the type alias
        VerifyCompletionList(fileContents, "| Id2 (a", [ "fff"; "myAlias" ], [ "num" ])

    [<Fact>]
    let ``Completion list does not contain suggested names in tuple deconstruction`` () =
        let fileContents =
            """
match Some (1, 2) with
| Some v -> ()
| Some (a, b) -> ()
| Some (c) -> ()
"""

        VerifyCompletionList(fileContents, "| Some v", [ "value" ], [])
        VerifyCompletionList(fileContents, "| Some (a", [], [ "value" ])
        VerifyCompletionList(fileContents, "| Some (a, b", [], [ "value" ])

        // Binding the whole tuple here, so the field name should be present
        VerifyCompletionList(fileContents, "| Some (c", [ "value" ], [])

    [<Fact>]
    let ``Completion list contains suggested names for union case field pattern based on the name of the generic type's solution`` () =
        let fileContents =
            """
type Tab =
    | A
    | B

match Some A with
| Some a -> ()

type G<'x, 'y> =
    | U1 of xxx: 'x * yyy: 'y
    | U2 of fff: string

match U1 (1, A) with
| U2 s -> ()
| U1 (x, y) -> ()
"""

        VerifyCompletionList(fileContents, "| Some a", [ "value"; "tab" ], [])
        VerifyCompletionList(fileContents, "| U2 s", [ "fff"; "string" ], [ "tab"; "xxx"; "yyy" ])
        VerifyCompletionList(fileContents, "| U1 (x", [ "xxx"; "num" ], [ "tab"; "yyy"; "fff" ])
        VerifyCompletionList(fileContents, "| U1 (x, y", [ "yyy"; "tab" ], [ "xxx"; "num"; "fff" ])

    [<Fact>]
    let ``Completion list for union case field identifier in a pattern contains available fields`` () =
        let fileContents =
            """
type PatternContext =
    | PositionalUnionCaseField of fieldIndex: int option * isTheOnlyField: bool * caseIdRange: range
    | NamedUnionCaseField of fieldName: string * caseIdRange: range
    | UnionCaseFieldIdentifier of referencedFields: string list * caseIdRange: range
    | Other

match PositionalUnionCaseField (None, 0, range0) with
| PositionalUnionCaseField (fieldIndex = _; a)
| NamedUnionCaseField (fieldName = a; z)
| NamedUnionCaseField (x)
"""

        VerifyCompletionListExactly(fileContents, "PositionalUnionCaseField (fieldIndex = _; a", [ "caseIdRange"; "isTheOnlyField" ])
        VerifyCompletionListExactly(fileContents, "NamedUnionCaseField (fieldName = a; z", [ "caseIdRange" ])

        // This has the potential to become either a positional field pattern or a named field identifier, so we want to see completions for both:
        // - suggested name based on the first field's identifier and a suggested name based on the first field's type
        // - names of all fields
        VerifyCompletionList(
            fileContents,
            "NamedUnionCaseField (x",
            [ "string"; "fieldName"; "caseIdRange" ],
            [ "range"; "fieldIndex"; "referencedFields"; "isTheOnlyField" ]
        )

    [<Fact>]
    let ``Completion list does not contain methods and non-literals when dotting into a type or module in a pattern`` () =
        let fileContents =
            """
module G =
    let a = 1

    [<Literal>]
    let b = 1

    let c () = ()

type A =
    | B of a: int
    | C of float

    static member Aug () = ()

for G. in [] do

let y x =
    match x with
    | [ B G. ] -> ()
    | A.

for Some ((0, C System.Double. ))
"""

        VerifyCompletionListExactly(fileContents, "for G.", [ "b" ])
        VerifyCompletionListExactly(fileContents, "| [ B G.", [ "b" ])
        VerifyCompletionListExactly(fileContents, "| A.", [ "B"; "C" ])
        VerifyCompletionList(fileContents, "for Some ((0, C System.Double.", [ "Epsilon"; "MaxValue" ], [ "Abs" ])

    [<Fact>]
    let ``Completion list for override does not contain virtual method if there is a sealed override higher up in the hierarchy`` () =
        let fileContents =
            """
[<AbstractClass>]
type A () =
    inherit System.Dynamic.SetIndexBinder (null)

    override _.a

[<AbstractClass>]
type B () =
    inherit System.Dynamic.DynamicMetaObjectBinder ()

    override x.

let _ =
    { new System.Dynamic.SetIndexBinder (null) with
        member x.
    }

let _ =
    { new System.Dynamic.DynamicMetaObjectBinder () with
        member this.
    }
"""

        // SetIndexBinder inherits from DynamicMetaObjectBinder, but overrides and seals Bind and the ReturnType property
        [ "override _.a"; "member x." ]
        |> List.iter (fun i ->
            VerifyCompletionListExactly(
                fileContents,
                i,
                [
                    "BindDelegate (site: System.Runtime.CompilerServices.CallSite<'T>, args: obj array): 'T"
                    "Equals (obj: obj): bool"
                    "FallbackSetIndex (target: System.Dynamic.DynamicMetaObject, indexes: System.Dynamic.DynamicMetaObject array, value: System.Dynamic.DynamicMetaObject, errorSuggestion: System.Dynamic.DynamicMetaObject): System.Dynamic.DynamicMetaObject"
                    "Finalize (): unit"
                    "GetHashCode (): int"
                    "ToString (): string"
                ]
            ))

        [ "override x."; "member this." ]
        |> List.iter (fun i ->
            VerifyCompletionListExactly(
                fileContents,
                i,
                [
                    "ReturnType with get (): System.Type"
                    "Bind (target: System.Dynamic.DynamicMetaObject, args: System.Dynamic.DynamicMetaObject array): System.Dynamic.DynamicMetaObject"
                    "BindDelegate (site: System.Runtime.CompilerServices.CallSite<'T>, args: obj array): 'T"
                    "Equals (obj: obj): bool"
                    "Finalize (): unit"
                    "GetHashCode (): int"
                    "ToString (): string"
                ]
            ))

    [<Fact>]
    let ``Completion list for override does not contain virtual method if it is already overridden in the same type`` () =
        let fileContents =
            """
type G<'a> () =
    override _.

    override x.ToString () = ""

[<AbstractClass]
type A () =
    abstract member A1: unit -> unit
    abstract member A1: string -> unit
    abstract member A2: unit -> unit

    member NotVirtual () = ()

type B () =
    inherit A ()

    override A1 () = ()
    override x.b

type C () =
    inherit A () =

    override A1 () = ()
    override x.c
    override A1 s = ()
"""

        VerifyCompletionListExactly(fileContents, "override _.", [ "Equals (obj: obj): bool"; "Finalize (): unit"; "GetHashCode (): int" ])

        VerifyCompletionListExactly(
            fileContents,
            "override x.b",
            [
                "A1 (arg: string): unit"
                "A2 (): unit"
                "Equals (obj: obj): bool"
                "Finalize (): unit"
                "GetHashCode (): int"
                "ToString (): string"
            ]
        )

        VerifyCompletionListExactly(
            fileContents,
            "override x.c",
            [
                "A2 (): unit"
                "Equals (obj: obj): bool"
                "Finalize (): unit"
                "GetHashCode (): int"
                "ToString (): string"
            ]
        )

    [<Fact>]
    let ``Completion list for override in interface implements does not contain method which is already overridden in the same type`` () =
        let fileContents =
            """
type IA =
    static abstract member A3: unit -> unit
    static abstract member A4: unit -> unit
    static abstract member P1: value: int -> int with get, set
    static abstract member P2: int with get, set
    
type IB =
    abstract member A1: unit -> unit
    abstract member A1: string -> unit
    abstract member A2: unit -> unit
    abstract member P1: int * bool -> int with get, set
    abstract member P2: int with get, set

type TA() =
    interface IA with
        static member 
        static member A3 (): unit = ()
        static member P2
            with get (): int = raise (System.NotImplementedException())
    interface IB with
        member this.A1 (arg1: string): unit = ()
        member this.P2
            with get (): int = raise (System.NotImplementedException())
        member thisTA.
"""

        VerifyCompletionListExactly(
            fileContents,
            "static member ",
            [
                "P1 with get (value: int): int and set (value: int) (value_1: int)"
                "P2 with set (value: int)"
                "A4 (): unit"
            ]
        )

        VerifyCompletionListExactly(
            fileContents,
            "member thisTA.",
            [
                "P1 with get (arg: int, arg_1: bool): int and set (arg: int, arg_1: bool) (value: int)"
                "P2 with set (value: int)"
                "A1 (): unit"
                "A2 (): unit"
            ]
        )

    [<Fact>]
    let ``Completion list for override is empty when the caret is on the self identifier`` () =
        let fileContents =
            """
type A () =
    override a

type B () =
    override _

type C () =
    override c.b () = ()
"""

        VerifyNoCompletionList(fileContents, "override a")
        VerifyNoCompletionList(fileContents, "override _")
        VerifyNoCompletionList(fileContents, "override c")

    [<Fact>]
    let ``Completion list for record field identifier in a pattern contains available fields, modules, namespaces and record types`` () =
        let fileContents =
            """
open System

type DU =
    | X

type R1 = { A: int; B: int }
type R2 = { C: int; D: int }

match [] with
| [ { A = 2; l = 2 } ] -> ()

match { A = 1; B = 2 } with
| { A = 1;  } -> ()
| { A = 2; s } -> ()
| { B = } -> ()
| { X = ; A = 3 } -> ()
| {   } -> ()
"""

        VerifyCompletionList(
            fileContents,
            "| [ { A = 2; l",
            [ "B"; "R1"; "R2"; "System"; "LanguagePrimitives" ],
            [ "A"; "C"; "D"; "DU"; "X"; "log"; "let"; "Lazy" ]
        )

        VerifyCompletionList(fileContents, "| { A = 1; ", [ "B"; "R1"; "R2" ], [ "C"; "D" ])
        VerifyCompletionList(fileContents, "| { A = 2; s", [ "B"; "R1"; "R2" ], [ "C"; "D" ])
        VerifyCompletionList(fileContents, "| { B =", [ "R1"; "R2"; "Some"; "None"; "System"; "DU" ], [ "A"; "B"; "C"; "D" ])
        VerifyCompletionList(fileContents, "| { B = ", [ "R1"; "R2"; "Some"; "None"; "System"; "DU" ], [ "A"; "B"; "C"; "D" ])
        VerifyCompletionList(fileContents, "| { X =", [ "R1"; "R2"; "Some"; "None"; "System"; "DU" ], [ "A"; "B"; "C"; "D" ])
        VerifyCompletionList(fileContents, "| { X = ", [ "R1"; "R2"; "Some"; "None"; "System"; "DU" ], [ "A"; "B"; "C"; "D" ])

        // Ideally C and D should not be present here, but right now we're not able to filter fields in an empty record pattern stub
        VerifyCompletionList(fileContents, "| {  ", [ "A"; "B"; "C"; "D"; "R1"; "R2" ], [])

    [<Fact>]
    let ``Completion list for record field identifier in a pattern contains fields of all records in scope when the record type is not known yet``
        ()
        =
        let fileContents =
            """
type R1 = { A: int; B: int }
type R2 = { C: int; D: int }

match { A = 1; B = 2 } with
| { f = () }
"""

        VerifyCompletionList(fileContents, "| { f = ()", [ "A"; "B"; "C"; "D" ], [])

    [<Fact>]
    let ``issue #16260 [TO-BE-IMPROVED] operators are fumbling for now`` () =
        let fileContents =
            """
module Ops =
let (|>>) a b = a + b
module Foo =
  let (|>>) a b = a + b
Ops.Foo.()
Ops.Foo.(
Ops.(
Ops.()
"""

        VerifyCompletionList(fileContents, "Ops.Foo.(", [], [ "|>>"; "(|>>)" ])
        VerifyCompletionList(fileContents, "Ops.(", [], [ "|>>"; "(|>>)" ])
