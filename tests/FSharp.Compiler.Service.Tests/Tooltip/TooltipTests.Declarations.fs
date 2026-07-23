module FSharp.Compiler.Service.Tests.TooltipDeclarationsTests

open System
open Xunit
open FSharp.Test
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization

[<Fact>]
let ``Regression.ImportedEvent.138110`` () =
    let source =
        """
open Microsoft.FSharp.Core.CompilerServices
let f (tp:ITypeProvider(*$$$*)) = tp.Invalidate
"""

    assertTooltipContains "Invalidate" (markAtStartOfMarker source "Provider(*$$$*)")

[<Fact>]
let ``OrphanFs.BaselineIntellisenseStillWorks`` () =
    assertTooltipContains "val astring: string" (markAtEndOfMarker """let astring = "Hello" """ "let astr")

[<Fact>]
let ``Global.LongPaths`` () =
    let source =
        String.concat
            "\n"
            [ "let test0 = global.System.Console.In"
              "let test0b = global.System.Collections.Generic.List<int>()"
              "let test0c = global.System.Collections.Generic.KeyNotFoundException()"
              "type Test0d = global.System.Collections.Generic.List<int>"
              "type Test0e = global.System.Collections.Generic.KeyNotFoundException" ]

    walk source "let test0 = global.System." "Console" "Console ="
    walk source "let test0 = global.System.Console." "In" "System.Console.In"
    walk source "let test0 = global.System.Console." "In" "TextReader"
    walk source "let test0b = global.System." "Collections" "namespace System.Collections"
    walk source "let test0b = global.System.Collections." "Generic" "namespace System.Collections.Generic"
    walk source "let test0b = global.System.Collections.Generic." "List" "List()"
    walk source "let test0c = global.System." "Collections" "namespace System.Collections"
    walk source "let test0c = global.System.Collections." "Generic" "namespace System.Collections.Generic"
    walk source "let test0c = global.System.Collections.Generic." "KeyNotFoundException" "KeyNotFoundException()"
    walk source "type Test0d = global.System." "Collections" "namespace System.Collections"
    walk source "type Test0d = global.System.Collections." "Generic" "namespace System.Collections.Generic"
    walk source "type Test0d = global.System.Collections.Generic." "List" "Generic.List"
    walk source "type Test0e = global.System." "Collections" "namespace System.Collections"
    walk source "type Test0e = global.System.Collections." "Generic" "namespace System.Collections.Generic"
    walk source "type Test0e = global.System.Collections.Generic." "KeyNotFoundException" "Generic.KeyNotFoundException"

[<Fact>]
let ``MethodAndPropTooltip`` () =
    let source =
        """
open System
do
    Console.Clear()
    Console.BackgroundColor |> ignore"""

    assertIdentifierInTooltipExactlyOnce "Clear" (markAtEndOfMarker source "Console.Cle")
    assertIdentifierInTooltipExactlyOnce "BackgroundColor" (markAtEndOfMarker source "Console.Back")

[<Fact>]
let ``Automation.Regression.AccessibilityOnTypeMembers.Bug4168`` () =
    let source =
        """module Test
type internal Foo2(*Marker*) () =
    member public this.Prop1 = 12
    member internal this.Prop2 = 12
    member private this.Prop3 = 12
    public new(x: int) = new Foo2()
    internal new(x: int, y: int) = new Foo2()
    private new(x: int, y: int, z: int) = new Foo2()"""

    assertTooltipContains "type internal Foo2" (markAtStartOfMarker source "(*Marker*)")

[<Fact(Skip = "DocComment issue")>]
let ``Automation.AutoOpenMyNamespace`` () =
    let source =
        """namespace System.Numerics
type t = BigInteger(*Marker1*)"""

    assertTooltipContainsInFsFile "type BigInteger" (markAtStartOfMarker source "r(*Marker1*)")

[<Fact>]
let ``Automation.Regression.TupleException.Bug3723`` () =
    let source =
        """namespace TestQuickinfo
exception E3(*Marker1*) of int * int
exception E4(*Marker2*) of (int * int)
exception E5(*Marker3*) = E4"""

    assertTooltipContainsInFsFile "exception E3 of int * int" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContainsInFsFile "Full name: TestQuickinfo.E3" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContainsInFsFile "exception E4 of (int * int)" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipContainsInFsFile "Full name: TestQuickinfo.E4" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipContainsInFsFile "exception E5 = E4" (markAtStartOfMarker source "(*Marker3*)")

[<Fact>]
let ``Automation.Regression.XmlDocComments.Bug3157`` () =
    let source =
        """namespace TestQuickinfo
module XmlComment =
    /// XmlComment J
    let func(*Marker*) x =
        /// XmlComment K
        let rec g x = 1
        g x"""

    let marked = markAtStartOfMarker source "(*Marker*)"
    assertTooltipContainsInFsFile "val func: x: 'a -> int" marked
    assertTooltipContainsInFsFile "XmlComment J" marked
    assertTooltipContainsInFsFile "Full name: TestQuickinfo.XmlComment.func" marked
    assertTooltipDoesNotContainInFsFile "XmlComment K" marked

let private referenceTooltipAtCaret (markedSource: string) =
    let context = SourceContext.fromMarkedSource markedSource
    let _, checkResults = getParseAndCheckResultsUniqueName context.Source
    checkResults.GetToolTip(context.CaretPos.Line, context.CaretPos.Column, context.LineText, ([]: string list), FSharpTokenTag.String)
    |> foldToolTip

let private assertReferenceTooltipContains (expected: string) (markedSource: string) =
    referenceTooltipAtCaret markedSource
    |> assertFoldedTooltipContains true "#r reference tooltip" expected

let private assertReferenceTooltipDoesNotContain (notExpected: string) (markedSource: string) =
    referenceTooltipAtCaret markedSource
    |> assertFoldedTooltipContains false "#r reference tooltip" notExpected

[<FactForDESKTOP>]
let ``Fsx.Bug4311HoverOverReferenceInFirstLine`` () =
    let source = "#r \"PresentationFramework.dll\"\n\n#r \"PresentationCore.dll\" "
    assertReferenceTooltipContains "PresentationFramework.dll" (markAtEndOfMarker source "#r \"PresentationFrame")
    assertReferenceTooltipDoesNotContain "multiple results" (markAtEndOfMarker source "#r \"PresentationFrame")

[<FactForDESKTOP>]
let ``Fsx.Bug5073`` () =
    let source = "#r \"System\" "
    assertReferenceTooltipContains @"Reference Assemblies\Microsoft" (markAtEndOfMarker source "#r \"Sys")
    assertReferenceTooltipContains ".NETFramework" (markAtEndOfMarker source "#r \"Sys")

[<FactForDESKTOP>]
let ``Fsx.HashR_QuickInfo.BugDefaultReferenceFileIsAlsoResolved`` () =
    assertReferenceTooltipContains "System.dll" (markAtEndOfMarker "#r \"System\" " "#r \"Syst")

[<FactForDESKTOP>]
let ``Fsx.HashR_QuickInfo.DoubleReference`` () =
    let source = "#r \"System\" // Mark1\n#r \"System\" // Mark2 "
    assertReferenceTooltipContains "System.dll" (markAtStartOfMarker source "tem\" // Mark1")
    assertReferenceTooltipContains "System.dll" (markAtStartOfMarker source "tem\" // Mark2")

[<Fact(Skip = "non-FCS: #r \"CustomMarshalers\" name-based resolution is non-deterministic in the FCS test host (intermittently empty tooltip)")>]
let ``Fsx.HashR_QuickInfo.ResolveFromGAC`` () =
    let marked = markAtEndOfMarker "#r \"CustomMarshalers\" " "#r \"Custo"
    assertReferenceTooltipContains ".NETFramework" marked
    assertReferenceTooltipContains "CustomMarshalers.dll" marked

[<FactForDESKTOP>]
let ``Fsx.HashR_QuickInfo.ResolveFromFullyQualifiedPath`` () =
    let path = System.IO.Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "System.configuration.dll")
    let source = sprintf "#r @\"%s\"" path
    let marker = "#r @\"" + path.Substring(0, path.Length / 2)
    let marked = markAtEndOfMarker source marker
    assertReferenceTooltipContains path marked
    assertReferenceTooltipContains (System.Reflection.AssemblyName.GetAssemblyName(path).ToString()) marked
