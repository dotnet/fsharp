module FSharp.Compiler.Service.Tests.CompletionOpenDirectivesTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LambdaOverloads.Completion`` () =
    let info =
        Checker.getCompletionInfo
            """open System.Linq
let _ = [""].Sum(fun x -> x.Len{caret})"""

    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``Duplicates.Bug4103a`` () =
    let info = Checker.getCompletionInfo "open Microsoft.FSharp.Quotations\nExpr.{caret}"

    assertItemDescriptionContainsExactlyOnce "WhileLoop" "WhileLoop" info

[<Fact>]
let ``StandardTypes.Bug4403`` () =
    let info =
        Checker.getCompletionInfo
            """open System
let x={caret}"""

    assertHasItemWithNames [ "int8"; "int16"; "int32"; "string"; "SByte"; "Int16"; "Int32"; "String" ] info

[<Fact>]
let ``NameSpace.InFsiFile.Bug882304_2`` () =
    let info =
        Checker.getCompletionInfoOfSignatureFile
            """module BasicTest
open System.{caret}"""

    assertHasItemWithNames [ "Action"; "Activator"; "Collections"; "IConvertible" ] info

[<Fact>]
let ``Duplicates.Bug4103c`` () =
    let info =
        Checker.getCompletionInfo
            """open System.IO
open System.IO
File.{caret}"""

    let expectedOverloads =
        typeof<System.IO.File>.GetMethods()
        |> Array.filter (fun m -> m.Name = "Open")
        |> Array.length

    assertItemDescriptionOccurrences expectedOverloads "Open" "File.Open" info

[<Fact>]
let ``Duplicates.Bug2094`` () =
    let info =
        Checker.getCompletionInfo
            """open Microsoft.FSharp.Control
let b = MailboxProcessor.{caret}"""

    assertItemDescriptionOccurrences 2 "Start" "Start" info

[<Fact>]
let ``Identifier.String.Positive`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System
                let str = "a string"
                // Test '.' after str
                let _ = str.{caret}
                """

    assertHasItemWithNames [ "Chars"; "ToString"; "Length"; "GetHashCode" ] info

[<Fact>]
let ``Identifier.String.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System
                let str = "a string"
                // Test '.' after str
                let _ = str.{caret}
                """

    assertHasNoItemsWithNames [ "Parse"; "op_Addition"; "op_Subtraction" ] info

[<Fact>]
let ``ImportStatement.System.ImportDirectly`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System.{caret}
                open IO = System(*Mimportstatement2*)"""

    assertHasItemWithNames [ "Collections" ] info

[<Fact>]
let ``ImportStatement.System.ImportAsIdentifier`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System(*Mimportstatement1*)
                open IO = System.{caret}"""

    assertHasItemWithNames [ "IO" ] info

[<Fact>]
let ``ObjInstance.ExtensionMethods.WithoutDef.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System
                let rnd = new System.Random()
                rnd.{caret}"""

    assertHasNoItemsWithNames [ "NextDice"; "DiceValue" ] info

[<Fact(Skip = "non-FCS: completion suppression inside a comment is an editor-layer concern (CompletionUtils.shouldProvideCompletion), not reproducible via the FCS GetDeclarationListInfo API which resolves the qualified 'System.' island regardless of the enclosing '//' comment (empirically returns 251 items here).")>]
let ``Expression.InComment`` () =
    let info =
        Checker.getCompletionInfo
            """
                //open System
                //open IO = System.{caret}"""

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "I don't understand why this test doesn't work, but the product works")>]
let ``ShortFormSeqExpr.Bug229610`` () =
    let info =
        Checker.getCompletionInfo
            """module test

open System.Text.RegularExpressions

let getLinks (txt: string) =
    [ for m in Regex.Matches(txt, "pattern") -> m.Groups.Item(1).{caret} ]"""

    assertHasItemWithNames [ "Value" ] info

[<Fact>]
let ``ReOpenNameSpace.SystemLibrary`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                open System.IO
                open System.IO

                File.{caret}
                """

    assertHasItemWithNames [ "Open" ] info

[<Fact>]
let ``ReOpenNameSpace.MailboxProcessor`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                open Microsoft.FSharp.Control
                open Microsoft.FSharp.Control
                let counter =
                    MailboxProcessor.{caret}"""

    assertHasItemWithNames [ "Start" ] info

[<Fact>]
let ``Seq.NearTheEndOfFile`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                open Microsoft.FSharp.Math

                let trianglenumbers = Seq.init_infinite (fun i -> let i = BigInt(i) in i * (i+1I) / 2I)

                (trianglenumbers |> Seq.{caret})"""

    assertHasItemWithNames [ "cache"; "find" ] info

[<Theory>]
[<InlineData(true, "append;choose;isEmpty")>]
[<InlineData(false, "<Note>")>]
let ``Regression3754.TypeOfListForward`` (shouldContain: bool) (names: string) =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test for bug 3754
                // tupe forwarder bug? intellisense bug?

                open System.IO
                open System.Xml
                open System.Xml.Linq
                let xmlStr = @"<?xml version='1.0' encoding='UTF-8'?><doc>    <blah>Blah</blah>    <a href='urn:foo' />    <yadda>        <blah>Blah</blah>        <a href='urn:bar' />    </yadda></doc>"
                let xns = XNamespace.op_Implicit ""
                let a = xns + "a"
                let reader = new StringReader(xmlStr)
                let xdoc = XDocument.Load(reader)
                let aElements = [for x in xdoc.Root.Elements() do
                                    if x.Name = a then
                                        yield x]
                let href = xns + "href"
                aElements |> List.{caret}"""

    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``NonApplicableExtensionMembersDoNotAppear.Bug40379`` () =
    let source (decl: string) =
        sprintf
            """open System.Xml.Linq
type MyType() =
    static member Foo(actual: XElement) = actual.Name
    member public this.Bar() =
        let actual: %s = failwith ""
        actual.{caret}"""
            decl

    let info1 = Checker.getCompletionInfo (source "int[]")
    assertHasNoItemsWithNames [ "Ancestors"; "AncestorsAndSelf" ] info1

    let info2 = Checker.getCompletionInfo (source "XNode[]")
    assertHasItemWithNames [ "Ancestors" ] info2
    assertHasNoItemsWithNames [ "AncestorsAndSelf" ] info2

    let info3 = Checker.getCompletionInfo (source "XElement[]")
    assertHasItemWithNames [ "Ancestors"; "AncestorsAndSelf" ] info3

[<Fact>]
let ``Verify no completion in hash directives`` () =
    let info =
        Checker.getCompletionInfo
            """
                #r {caret}

                let foo x = x
                let bar = 1"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Fsx.HashLoad.Conditionals`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "--define:INTERACTIVE" |]
            FSharpCodeCompletionOptions.Default
            """module InDifferentFS =
#if INTERACTIVE
    let x = 1
#else
    let y = 2
#endif
#if RELEASE
    let A = 3
#else
    let B = 4
#endif

InDifferentFS.{caret}"""

    assertHasItemWithNames [ "x"; "B" ] info
    assertHasNoItemsWithNames [ "y"; "A" ] info
    Assert.Equal(2, info.Items.Length)

[<Fact>]
let ``Fsx.BugAllowExplicitReferenceToMsCorlib`` () =
    let serviceDll = typeof<FSharpChecker>.Assembly.Location

    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| sprintf "-r:%s" serviceDll |]
            FSharpCodeCompletionOptions.Default
            """#r "mscorlib"
open FSharp.Compiler.Interactive.Shell.Settings
fsi.{caret}"""

    assertHasItemWithNames [ "CommandLineArgs" ] info

[<Fact>]
let ``Fsx.HashReferenceAgainstStrongName`` () =
    let source =
        sprintf
            "#reference \"System.Core, Version=%s, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nopen System.{caret}"
            (System.Environment.Version.ToString())

    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "Linq" ] info

[<Fact>]
let ``Fsx.ShouldBeAbleToReference30Assemblies.Bug2050`` () =
    let info =
        Checker.getCompletionInfo
            """#r "System.Core.dll"
open System.{caret}"""

    assertHasItemWithNames [ "Linq" ] info
