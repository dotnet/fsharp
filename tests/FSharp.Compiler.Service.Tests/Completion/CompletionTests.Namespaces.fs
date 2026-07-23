module FSharp.Compiler.Service.Tests.CompletionNamespacesTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``List.AfterAddLinqNamespace.Bug3754`` () =
    let info =
        Checker.getCompletionInfo
            """open System.Xml.Linq
List.{caret}"""

    assertHasItemWithNames [ "map"; "filter" ] info

[<Fact>]
let ``Global`` () =
    let info = Checker.getCompletionInfo "global.{caret}"

    assertHasItemWithNames [ "System"; "Microsoft" ] info

[<Fact>]
let ``Identifier.NonDottedNamespace.Bug1347`` () =
    let info =
        Checker.getCompletionInfo
            """open System
let x = Mic{caret}
let p7 =
    let sieve limit =
        let isPrime = Array.create (limit+1) true
        for n in"""

    assertHasItemWithNames [ "Microsoft" ] info

[<Fact>]
let ``OpenNamespaceOrModule.CompletionOnlyContainsNamespaceOrModule.Case2`` () =
    let info = Checker.getCompletionInfo "open Microsoft.FSharp.Collections.Array.{caret}"

    assertHasItemWithNames [ "Parallel" ] info
    assertHasNoItemsWithNames [ "map" ] info

[<Fact>]
let ``AtNamespaceDot`` () =
    let info = Checker.getCompletionInfo "let y=new System.{caret}String()"

    assertHasItemWithNames [ "String"; "Console" ] info

[<Fact>]
let ``SystemNamespace`` () =
    let info = Checker.getCompletionInfo "let y = System.{caret}"

    assertHasItemWithNames [ "Action"; "Collections" ] info

    assertItemGlyph "Action" FSharpGlyph.Delegate info
    assertItemGlyph "Collections" FSharpGlyph.NameSpace info

[<Fact>]
let ``WithoutOpenNamespace`` () =
    let info =
        Checker.getCompletionInfo
            """
module CodeAccessibility
let x = S{caret}"""

    assertHasNoItemsWithNames [ "Single" ] info

[<Fact>]
let ``Namespace.System`` () =
    let info =
        Checker.getCompletionInfo
            """
// Test '.' after System
open System.{caret}
let str = "a string"
// Test '.' after str
let _ = str(*usage*)"""

    assertHasItemWithNames [ "IO"; "Collections" ] info

[<Fact>]
let ``Identifier.AsNamespace`` () =
    let info = Checker.getCompletionInfo "namespace Namespace1.{caret}"

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``ReopenNamespace.Module`` () =
    let info =
        Checker.getCompletionInfo
            """
namespace A
module Test =
  let foo n = n + 1
namespace B
open A
open A
Test.{caret}"""

    assertHasItemWithNames [ "foo" ] info
