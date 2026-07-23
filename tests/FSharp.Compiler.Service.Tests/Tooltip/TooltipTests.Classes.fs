module FSharp.Compiler.Service.Tests.TooltipClassesTests

open System
open System.IO
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.IO
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization
open TestFramework

let private assertCrossFileTooltipContains
    (expected: string)
    (file1Name: string)
    (file1Source: string)
    (file2RelativePath: string)
    (markedFile2: string)
    =
    let context = Checker.getResolveContext markedFile2
    let root = createTemporaryDirectory ()
    let projDir = Path.Combine(root.FullName, "proj")
    Directory.CreateDirectory(projDir) |> ignore
    let file1Path = Path.Combine(projDir, file1Name)
    let file2LogicalPath = Path.Combine(projDir, file2RelativePath)
    let file2PhysicalPath = Path.GetFullPath file2LogicalPath
    Directory.CreateDirectory(Path.GetDirectoryName file2PhysicalPath) |> ignore
    FileSystem.OpenFileForWriteShim(file1Path).Write(file1Source)
    FileSystem.OpenFileForWriteShim(file2PhysicalPath).Write(context.Source)

    let dllName = Path.Combine(projDir, "CrossFile.dll")
    let projName = Path.Combine(projDir, "CrossFile.fsproj")
    let args = mkProjectCommandLineArgs(dllName, [])

    let options =
        { checker.GetProjectOptionsFromCommandLineArgs(projName, args) with
            SourceFiles = [| file1Path; file2LogicalPath |] }

    let _, checkResults = parseAndCheckFile file2LogicalPath context.Source options

    checkResults.GetTooltip(context)
    |> foldToolTip
    |> assertFoldedTooltipContains true "cross-file tooltip" expected

let private assertProjectTooltipContains (projectName: string) (expected: string) (markedSource: string) =
    foldedProjectTooltip [] [] markedSource
    |> assertFoldedTooltipContains true (sprintf "tooltip in project %A" projectName) expected

[<Fact>]
let ``QuickInfo.LetBindingsInTypes`` () =
    assertTooltipContains
        "val fff: n: int -> int"
        """type A() =
    let ff{caret}f n = n + 1"""

[<Fact>]
let ``Basic`` () =
    assertTooltipContains
        "Bob ="
        """type (*bob*)Bob{caret}() =
    let x = 1"""

[<Fact>]
let ``TauStarter`` () =
    assertTooltipContains
        "Bob ="
        """type (*Scenario01*)Bob() =
    let x = 1
type (*Scenario021*)Bob{caret} =
    class
    public new() = { }
end
type (*Scenario022*)Alice =
    class
    public new() = { }
end"""

    assertTooltipContains
        "Alice ="
        """type (*Scenario01*)Bob() =
    let x = 1
type (*Scenario021*)Bob =
    class
    public new() = { }
end
type (*Scenario022*)Alice{caret} =
    class
    public new() = { }
end"""

[<Fact>]
let ``MemberIdentifiers`` () =
    let source =
        String.concat
            "\n"
            [ "type TestType() ="
              "     member (*test6*) xx.PPPP = 1"
              "     member (*test7*) xx.QQQQ(x) = 3.0"
              "let test8 = (TestType()).PPPP" ]

    let walk = EditorServiceAsserts.walk source
    walk "member (*test6*) " "xx" "TestType"
    walk "member (*test6*) xx." "PPPP" "PPPP"
    walk "member (*test7*) " "xx" "TestType"
    walk "member (*test7*) xx." "QQQQ" "float"
    walk "let test8 = (TestType())." "PPPP" "PPPP"

[<Fact>]
let ``Regression.StaticVsInstance.Bug3626`` () =
    let staticCall =
        """type Foo() =
    member this.Bar () = "hllo"
    static member Bar() = 13
let z = (*int*) Foo.Ba{caret}r()
let Hoo = new Foo()
let y = (*string*) Hoo.Bar()"""

    assertTooltipContains "Foo.Bar" staticCall
    assertTooltipContains "-> int" staticCall

    let instanceCall =
        """type Foo() =
    member this.Bar () = "hllo"
    static member Bar() = 13
let z = (*int*) Foo.Bar()
let Hoo = new Foo()
let y = (*string*) Hoo.Ba{caret}r()"""

    assertTooltipContains "Foo.Bar" instanceCall
    assertTooltipContains "-> string" instanceCall

[<Fact>]
let ``Regression.Classes.Bug4066`` () =
    let source = "type Foo() as this =\n    do this |> ignore\n    member this.Bar() = this"

    for marker in [ "as thi"; "do thi"; "member thi"; "Bar() = thi" ] do
        let marked = markAtEndOfMarker source marker
        assertTooltipContains "val this: Foo" marked
        assertTooltipDoesNotContain "ref" marked

[<Fact>]
let ``AcrossTwoProjects`` () =
    assertProjectTooltipContains
        "testproject1"
        "Bob1 ="
        """type (*bob*)Bob{caret}1() =
    let x = 1"""

    assertProjectTooltipContains
        "testproject2"
        "Bob2 ="
        """type (*bob*)Bob{caret}2() =
    let x = 1"""

[<Theory>]
[<InlineData("File2.fs")>]
[<InlineData("../File2.fs")>]
let ``AcrossMultipleFiles`` (file2RelativePath: string) =
    assertCrossFileTooltipContains
        "File1.Bob"
        "File1.fs"
        "type Bob() =\n    let x = 1\n"
        file2RelativePath
        "let bo{caret}b = new File1.Bob()"

[<Fact>]
let ``AcrossLinkedFiles`` () =
    assertCrossFileTooltipContains
        "Link.Bob"
        "link.fs"
        "type Bob() =\n    let x = 1\n"
        "File2.fs"
        "let bo{caret}b = new Link.Bob()"

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_9`` () =
    assertTooltipContainsInOrder
        [ "type Class"; "A comment" ]
        """module Module =
    /// A comment
    type Class = class end
let _ = typeof<Module.Cla{caret}ss>"""

[<Fact>]
let ``Regression.Class.Printing.CSharp.Classes.Only.Bug4592`` () =
    assertTooltipContainsInOrder
        [ "type Random ="
          "  new: unit -> unit + 1 overload"
          "  member Next: unit -> int + 2 overloads"
          "  member NextBytes: buffer: byte array -> unit"
          "  member NextDouble: unit -> float" ]
        "let _ = typeof<System.Rand{caret}om>"

#if !NETCOREAPP
let private getWinFormsTooltip (markedSource: string) =
    getTooltipWithReferences
        "WinFormsTooltip"
        [ fsCoreDefaultReference ()
          sysLib "mscorlib"
          sysLib "System"
          sysLib "System.Core"
          sysLib "System.Drawing"
          sysLib "System.Windows.Forms" ]
        markedSource

[<Fact>]
let ``Regression.CompListItemInfo.Bug5694`` () =
    let actual =
        getWinFormsTooltip
            """type Form2() as self =
    inherit System.Windows.Forms.Form()
    member _.M() = self.AcceptB{caret}utton"""
        |> foldToolTip

    let expected =
        "<summary>Gets or sets the button on the form that is clicked when the user presses the ENTER key.</summary>"

    if not (actual.Contains expected) then
        failwithf "Expected tooltip to contain %A, but the actual tooltip was:\n%s" expected actual

[<Fact>]
let ``Regression.Class.Printing.CSharp.Classes.Bug4624`` () =
    assertTooltipContainsInOrder
        [ "type CodeConnectAccess ="
          "  new: allowScheme: string * allowPort: int -> unit"
          "  member Equals: o: obj -> bool"
          "  member GetHashCode: unit -> int"
          "  static member CreateAnySchemeAccess: allowPort: int -> CodeConnectAccess"
          "  static member CreateOriginSchemeAccess: allowPort: int -> CodeConnectAccess"
          "  static val AnyScheme: string"
          "  static val DefaultPort: int"
          "  ..." ]
        "let _ = typeof<System.Security.Policy.CodeConnectAcc{caret}ess>"
#endif
