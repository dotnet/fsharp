module FSharp.Compiler.Service.Tests.TooltipRecordsTests

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization

let private assertTooltipTrimmedContainsInFsFile (expected: string) (markedSource: string) =
    let actual = foldedTooltip FsFile markedSource
    let trimmed = actual.Replace("\r", "").Replace("\n", "")

    if not (trimmed.Contains expected) then
        failwithf "Expected newline-stripped .fs-file tooltip to contain %A, but the actual tooltip was:\n%s" expected actual

[<Theory>]
[<InlineData("""open System.ComponentModel
type TypeU = { Element : string }
    with
      [<EditorBrowsableAttribute(EditorBrowsableState.Never)>]
      [<CompilerMessageAttribute("This method is intended for use in generated code only.", 10001, IsHidden=true, IsError=false)>]
      member x._Print = x.Element.ToString()
let u = { Element = "abc" }
""",
            "member _Print", "")>]
[<InlineData("""type TypeU = { Element : string }
    with
        [<System.ObsoleteAttribute("This is replaced with Print2")>]
        member x.Print1 = x.Element.ToString()
        member x.Print2 = x.Element.ToString()
let u = { Element = "abc" }
""",
            "member Print1", "member Print2")>]
let ``Hidden record members are omitted from the type tooltip`` (source: string) (notExpected: string) (alsoExpected: string) =
    let marked = markAtStartOfMarker source "ypeU ="
    assertTooltipDoesNotContain notExpected marked

    if alsoExpected <> "" then
        assertTooltipContains alsoExpected marked

[<Fact>]
let ``TypeRecordQuickInfo`` () =
    let source =
        """namespace NS
                   type Re(*MarkerRecord*) = { X : int } """

    assertTooltipTrimmedContainsInFsFile "type Re =  { X: int }" (markAtStartOfMarker source "(*MarkerRecord*)")

[<Fact>]
let ``Regression.InDeclaration.Bug3176a`` () =
    let source = """type T<'a> = { aaaa : 'a; bbbb : int } """
    assertTooltipContains "aaaa: 'a" (markAtEndOfMarker source "aa")

[<Fact>]
let ``IdentifiersForFields`` () =
    let source =
        String.concat "\n" [ "type TestType9 = { XXX : int }"; "let test11 = { XXX = 1 }" ]

    walk source "type TestType9 = { " "XXX" "XXX: int"
    walk source "let test11 = { " "XXX" "XXX"

[<Fact>]
let ``ArgumentAndPropertyNames`` () =
    let source =
        String.concat
            "\n"
            [ "type R = { mutable AAA : int }"
              "         static member M() = { AAA = 1 }"
              "let test13 = R.M(AAA=3)"
              "type R2() = "
              "    static member M() = System.Reflection.InterfaceMapping()"
              ""
              "let test14 = R2.M(InterfaceMethods= [| |])"
              ""
              "let test15 = new System.Reflection.AssemblyName(Name=\"Foo\")"
              "let test16 = new System.Reflection.AssemblyName(assemblyName=\"Foo\")" ]

    walk source "let test13 = R.M(" "AAA" "R.AAA: int"
    walk source "let test14 = R2.M(" "InterfaceMethods" "field System.Reflection.InterfaceMapping.InterfaceMethods"
    walk source "let test15 = new System.Reflection.AssemblyName(" "Name" "property System.Reflection.AssemblyName.Name"
    walk source "let test16 = new System.Reflection.AssemblyName(" "assemblyName" "argument assemblyName"

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_7`` () =
    assertTooltipContainsInOrder
        [ "Record.field: int"; "A comment" ]
        """type Record = {
    /// A comment
    field : int
    }

let record = {field = 1}
let x() = record.fie{caret}ld"""
