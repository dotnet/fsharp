module FSharp.Compiler.Service.Tests.CompletionExceptionsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``IncompleteStatement.Try_B`` () =
    let info =
        Checker.getCompletionInfo
            """let x = "1"
try (x).{caret} finally ()"""

    assertHasItemWithNames [ "Contains" ] info

[<Fact>]
let ``IncompleteStatement.Try_C`` () =
    let info =
        Checker.getCompletionInfo
            """let x = "1"
try (x).{caret} with e -> () """

    assertHasItemWithNames [ "Contains" ] info

[<Fact>]
let ``Duplicates.Bug4103b`` () =
    let source marker =
        sprintf
            """namespace A
module Test =
  let foo n = n + 1
  let (|Pat|) x = x + 1
  exception Failed
  type Del = delegate of int -> int
  type A = | Foo
  type B = | Bar = 0
type TestType =
  static member Prop = 0
  static member Event = (new Event<_>()).Publish
namespace B
open A
open A
%s"""
            marker

    for marker, shortName, fullName in
        [ "Test.", "foo", "foo"
          "Test.", "Pat", "Pat"
          "Test.", "Failed", "exception Failed"
          "Test.", "Del", "type Del"
          "Test.", "Foo", "Test.A.Foo"
          "Test.B.", "Bar", "Test.B.Bar"
          "TestType.", "Prop", "TestType.Prop"
          "TestType.", "Event", "TestType.Event" ] do
        let info = Checker.getCompletionInfo (source (marker + "{caret}"))

        assertItemDescriptionContainsExactlyOnce shortName fullName info

[<Fact>]
let ``NoDupException.Postive`` () =
    let info = Checker.getCompletionInfo """let x = Match{caret}"""

    assertHasItemWithNames [ "MatchFailureException" ] info

[<Fact>]
let ``DotNetException.Negative`` () =
    let info = Checker.getCompletionInfo """let x = Match{caret}"""

    assertHasNoItemsWithNames [ "MatchFailure" ] info
