// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System.Threading
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.QuickInfo
open Xunit
open FSharp.Editor.Tests.Helpers
open Internal.Utilities.CancellableTasks

module QuickInfo =
    open FSharp.Compiler.EditorServices

    let private GetCaretPosition (codeWithCaret: string) =
        let caretSentinel = "$$"
        let mutable cursorInfo: (int * string) = (0, null)

        // find the '$$' sentinel that represents the cursor location
        let caretPosition = codeWithCaret.IndexOf(caretSentinel)

        if caretPosition >= 0 then
            let newContents =
                codeWithCaret.Substring(0, caretPosition)
                + codeWithCaret.Substring(caretPosition + caretSentinel.Length)

            cursorInfo <- caretPosition, newContents

        cursorInfo

    let internal GetQuickInfo (code: string) caretPosition =
        asyncMaybe {
            let document =
                RoslynTestHelpers.CreateSolution(code) |> RoslynTestHelpers.GetSingleDocument

            let! _, _, _, tooltip =
                FSharpAsyncQuickInfoSource.TryGetToolTip(document, caretPosition)
                |> CancellableTask.start CancellationToken.None
                |> Async.AwaitTask

            return tooltip
        }
        |> Async.RunSynchronously

    let GetQuickInfoTextFromCode (codeWithCaret: string) =
        let caretPosition, code = GetCaretPosition codeWithCaret
        let sigHelp = GetQuickInfo code caretPosition

        match sigHelp with
        | Some (ToolTipText elements) when not elements.IsEmpty ->
            let documentationBuilder =
                { new IDocumentationBuilder with
                    override _.AppendDocumentationFromProcessedXML(_, _, _, _, _, _) = ()
                    override _.AppendDocumentation(_, _, _, _, _, _, _) = ()
                }

            let _, mainDescription, docs =
                XmlDocumentation.BuildSingleTipText(documentationBuilder, elements.Head, XmlDocumentation.DefaultLineLimits)

            let mainTextItems = mainDescription |> Seq.map (fun x -> x.Text)
            let docTextItems = docs |> Seq.map (fun x -> x.Text)
            System.String.Join(System.String.Empty, (Seq.concat [ mainTextItems; docTextItems ]))
        | _ -> ""

    let expectedLines (lines: string list) = System.String.Join("\n", lines)

    [<Fact>]
    let ``Automation.EnumDUInterfacefromFSBrowse.InsideComputationExpression`` () =
        let code =
            """
namespace FsTest

type MyColors =
        | Red = 0
        | Green = 1
        | Blue = 2

module Test =
    let test() =
        let x =
            seq {
                for i in 1..10 do
                    let f = MyColors.Re$$d
                    yield f
                }
        ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "MyColors.Red: MyColors = 0"
        Assert.Equal(expected, quickInfo)

    [<Fact>]
    let ``Automation.EnumDUInterfacefromFSBrowse.InsideMatch`` () =
        let code =
            """
namespace FsTest

type MyDistance =
    | Kilometers of float
    | Miles of float
    | NauticalMiles of float

module Test =
    let test() =
        let myDuList = (fun x ->
            match x with
            | 0 -> MyDistanc$$e.Kilometers
            | 1 -> MyDistance.Miles
            | _ -> MyDistance.NauticalMiles
            )
        ()
"""

        let quickInfo = GetQuickInfoTextFromCode code

        let expected =
            expectedLines
                [
                    "type MyDistance ="
                    "  | Kilometers of float"
                    "  | Miles of float"
                    "  | NauticalMiles of float"
                    "Full name: FsTest.MyDistance"
                ]

        Assert.Equal(expected, quickInfo)

    [<Fact>]
    let ``Automation.EnumDUInterfacefromFSBrowse.InsideLambda`` () =
        let code =
            """
namespace FsTest

type IMyInterface =
    interface
        abstract Represent : unit -> string
    end

type MyTestType() =
    [<DefaultValue>]
    val mutable field : int

    interface IMyInterface with
        member this.Represent () = "Implement Interface"

module Test =
    let test() =
        let s = new MyTestType()
                |> fun (x:MyTestType) -> x :> IMyInterface
                |> fun (x:IMyInterface) -> x.Represen$$t()
        ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "abstract IMyInterface.Represent: unit -> string"
        Assert.Equal(expected, quickInfo)

    [<Fact>]
    let ``Automation.RecordAndInterfaceFromFSProj.InsideComputationExpression`` () =
        let code =
            """
namespace FsTest

type MyEmployee = 
    { mutable Name  : string;
        mutable Age   : int;
        mutable IsFTE : bool }

module Test =
    let test() =
        let construct =
            seq {
                for i in 1..10 do
                let a = MyEmploye$$e.MakeDummy()
                ()
            }
    ()
"""

        let quickInfo = GetQuickInfoTextFromCode code

        let expected =
            expectedLines
                [
                    "type MyEmployee ="
                    "  {"
                    "    mutable Name: string"
                    "    mutable Age: int"
                    "    mutable IsFTE: bool"
                    "  }"
                    "Full name: FsTest.MyEmployee"
                ]

        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.RecordAndInterfaceFromFSProj.InsideQuotation`` () =
        let code =
            """
namespace FsTest

type MyEmployee = 
    { mutable Name  : string;
        mutable Age   : int;
        mutable IsFTE : bool }

module Test =
    let test() =
        let aa = { Name: "name";
                    Age: 1;
                    IsFTE: false; }
        let b = <@ a$$a.Name @>
    ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val aa: MyEmployee"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.RecordAndInterfaceFromFSProj.InsideLambda`` () =
        let code =
            """
namespace FsTest

type MyEmployee = 
    { mutable Name  : string;
        mutable Age   : int;
        mutable IsFTE : bool }

module Test =
    let test() =
        let aa = { Name: "name";
                    Age: 1;
                    IsFTE: false; }
        let b =
            [ aa ]
            |> List.filter (fun e -> e.IsFT$$E)
    ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "MyEmployee.IsFTE: bool"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.TupleRecordFromFSBrowse.InsideComputationExpression`` () =
        let code =
            """
namespace FsTest

module Test =
    let GenerateTuple =
        fun x ->
            let tuple = (x, x.ToString(), (float)x, (fun y -> (y.ToString(), y + 1)))
            tuple
    let test() =
        let mySeq =
            seq {
                for i in 1..9 do
                    let my$$Tuple = GenerateTuple i
                    yield myTuple
            }
    ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val myTuple: int * string * float * (int -> string * int)"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.TupleRecordFromFSBrowse.SequenceOfMethods`` () =
        let code =
            """
namespace FsTest

module Test =
    let GenerateTuple =
        fun x ->
            let tuple = (x, x.ToString(), (float)x, (fun y -> (y.ToString(), y + 1)))
            tuple
    let GetTupleMethod tuple =
        let (_intInTuple, _stringInTuple, _floatInTuple, methodInTuple) = tuple
        methodInTuple
    let test() =
        let mySeq =
            seq {
                for i in 1..9 do
                    let myTuple = GenerateTuple i
                    yield myTuple
            }
        let method$$Seq = Seq.map GetTupleMethod mySeq
    ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val methodSeq: seq<(int -> string * int)>"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.MatchExpression`` () =
        let code =
            """
namespace FsTest

[<Struct>]
type MyPoint(x:int, y:int) =
    member this.X = x
    member this.Y = y

module Test =
    let test() =
        let p1 = MyPoint(1, 2)
        match p$$1 with
        | p3 when p3.X = 1 -> 0
        | _ -> 1
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val p1: MyPoint"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.MatchPattern`` () =
        let code =
            """
namespace FsTest

[<Struct>]
type MyPoint(x:int, y:int) =
    member this.X = x
    member this.Y = y

module Test =
    let test() =
        let p1 = MyPoint(1, 2)
        match p1 with
        | p$$3 when p3.X = 1 -> 0
        | _ -> 1
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val p3: MyPoint"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.UnionIfPredicate`` () =
        let code =
            """
namespace FsTest

type MyDistance =
    | Kilometers of float
    | Miles of float
    | NauticalMiles of float

module Test =
    let test() =
        let dd = MyDistance.Kilometers 1.0
        if MyDistance.toMiles d$$d > 0 then ()
        else ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val dd: MyDistance"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.UnionForPattern`` () =
        let code =
            """
namespace FsTest

type MyDistance =
    | Kilometers of float
    | Miles of float
    | NauticalMiles of float

module Test =
    let test() =
        let distances = [ MyDistance.Kilometers 1.0; MyDistance.Miles 1.0 ]
        for dist$$ance in distances do
            ()
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "val distance: MyDistance"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.UnionMethodPatternMatch`` () =
        let code =
            """
namespace FsTest

type MyDistance =
    | Kilometers of float
    | Miles of float
    | NauticalMiles of float
    static member toMiles x =
        Miles(
            match x with
            | Miles x -> x
            | Kilometers x -> x / 1.6
            | NauticalMiles x -> x * 1.15
        )

module Test =
    let test() =
        let dd = MyDistance.Kilometers 1.0
        match MyDistance.to$$Miles dd with
        | Miles x -> 0
        | _ -> 1
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "static member MyDistance.toMiles: x: MyDistance -> MyDistance"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.UnionMethodPatternMatchBody`` () =
        let code =
            """
namespace FsTest

type MyDistance =
    | Kilometers of float
    | Miles of float
    | NauticalMiles of float
    member this.IncreaseBy dist =
        match this with
        | Kilometers x -> Kilometers (x + dist)
        | Miles x -> Miles (x + dist)
        | NauticalMiles x -> NauticalMiles (x + dist)

module Test =
    let test() =
        let dd = MyDistance.Kilometers 1.0
        match dd.toMiles() with
        | Miles x -> dd.Increase$$By 1.0
        | _ -> dd
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "member MyDistance.IncreaseBy: dist: float -> MyDistance"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.UnionAndStructFromFSProj.UnionPropertyInComputationExpression`` () =
        let code =
            """
namespace FsTest

type MyDistance =
    | Kilometers of float
    | Miles of float
    | NauticalMiles of float
    member this.asNautical =
        NauticalMiles(
            match this with
            | Kilometers x -> x / 1.852
            | Miles x -> x / 1.15
            | NauticalMiles x -> x
        )

module Test =
    let test() =
        let dd = MyDistance.Kilometers 1.0
        async {
            let r = dd.as$$Nautical
            return r
        }
"""

        let quickInfo = GetQuickInfoTextFromCode code
        let expected = "property MyDistance.asNautical: MyDistance with get"
        Assert.Equal(expected, quickInfo)
        ()

    [<Fact>]
    let ``Automation.LetBindings.Module`` () =
        let code =
            """
namespace FsTest

module Test =
    let fu$$nc x = ()
"""

        let expectedSignature = "val func: x: 'a -> unit"

        let tooltip = GetQuickInfoTextFromCode code

        Assert.StartsWith(expectedSignature, tooltip)
        ()

    [<Fact>]
    let ``Automation.LetBindings.InsideType.Instance`` () =
        let code =
            """
namespace FsTest

module Test =
    type T() =
        let fu$$nc x = ()
"""

        let expectedSignature = "val func: x: 'a -> unit"

        let tooltip = GetQuickInfoTextFromCode code

        Assert.StartsWith(expectedSignature, tooltip)

    [<Fact>]
    let ``Automation.LetBindings.InsideType.Static`` () =
        let code =
            """
namespace FsTest

module Test =
    type T() =
        static let fu$$nc x = ()
"""

        let expectedSignature = "val func: x: 'a -> unit"

        let tooltip = GetQuickInfoTextFromCode code

        Assert.StartsWith(expectedSignature, tooltip)
        ()

    [<Fact>]
    let ``Automation.LetBindings`` () =
        let code =
            """
namespace FsTest

module Test =
    do
        let fu$$nc x = ()
        ()
"""

        let expectedSignature = "val func: x: 'a -> unit"
        let tooltip = GetQuickInfoTextFromCode code
        Assert.StartsWith(expectedSignature, tooltip)

    [<Fact>]
    let ``quick info for IWSAM property get`` () =
        let code =
            """
type IStaticProperty<'T when 'T :> IStaticProperty<'T>> =
    static abstract StaticProperty: 'T

let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) =
    'T.StaticPr$$operty
"""

        let expectedSignature = "property IStaticProperty.StaticProperty: 'T with get"
        let tooltip = GetQuickInfoTextFromCode code
        Assert.StartsWith(expectedSignature, tooltip)

    [<Fact>]
    let ``quick info for IWSAM method call`` () =
        let code =
            """
type IStaticMethod<'T when 'T :> IStaticMethod<'T>> =
    static abstract StaticMethod: unit -> 'T

let f (x: #IStaticMethod<'T>) =
    'T.StaticMe$$thod()
"""

        let expectedSignature = "static abstract IStaticMethod.StaticMethod: unit -> 'T"
        let tooltip = GetQuickInfoTextFromCode code
        Assert.StartsWith(expectedSignature, tooltip)

    [<Fact>]
    let ``quick info for SRTP property get`` () =
        let code =
            """

let inline f_StaticProperty_SRTP<'T when 'T : (static member StaticProperty: 'T) >() =
    'T.StaticPr$$operty
"""

        let expectedSignature = "'T: (static member StaticProperty: 'T)"
        let tooltip = GetQuickInfoTextFromCode code
        Assert.StartsWith(expectedSignature, tooltip)

    [<Fact>]
    let ``quick info for SRTP method call`` () =
        let code =
            """

let inline f_StaticProperty_SRTP<'T when 'T : (static member StaticMethod: unit -> 'T) >() =
    'T.StaticMe$$thod()
"""

        let expectedSignature = "'T: (static member StaticMethod: unit -> 'T)"
        let tooltip = GetQuickInfoTextFromCode code
        Assert.StartsWith(expectedSignature, tooltip)

    [<Fact>]
    let ``Display names for exceptions with backticks preserve backticks`` () =
        let code =
            """
exception SomeError of ``thing wi$$th space``: string
"""

        let expected = "``thing with space``"

        let actual = GetQuickInfoTextFromCode code
        Assert.Contains(expected, actual)
        ()

    [<Fact>]
    let ``Display names for anonymous record fields with backticks preserve backticks`` () =
        let code =
            """
type R = {| ``thing wi$$th space``: string |}
"""

        let expected = "``thing with space``"

        let actual = GetQuickInfoTextFromCode code

        Assert.Contains(expected, actual)
        ()

    [<Fact>]
    let ``Display names identifiers for active patterns with backticks preserve backticks`` () =
        let code =
            """
let (|``Thing with space``|_|) x = if x % 2 = 0 then Some (x/2) else None

match 4 with
| ``Thing wi$$th space`` _ -> "yes"
| _ -> "no"
"""

        let expected = "``Thing with space``"

        let actual = GetQuickInfoTextFromCode code

        Assert.Contains(expected, actual)
        ()
