module FSharp.Compiler.Service.Tests.TooltipPropertiesTests

open Xunit

let private propSource =
    """namespace CountChocula
               type BooBerry() =
                   let get() = ""
                   member source.Prop
                       with get() : int = 0
                       and set(value:int) : unit = ()"""

[<Fact>]
let ``Regression.AccessorMutator.Bug4903a`` () =
    assertTooltipDoesNotContain "string" (markAtEndOfMarker propSource "with g")
    assertTooltipContains "int" (markAtEndOfMarker propSource "with g")

[<Fact>]
let ``Regression.AccessorMutator.Bug4903d`` () =
    assertTooltipDoesNotContain
        "string"
        """namespace CountChocula
               type BooBerry() =
                   member source.AMetho{caret}d() = ()
                   member source.AProperty
                       with get() : int = 0
                       and set(value:int) : unit = ()"""

[<Fact>]
let ``Regression.AccessorMutator.Bug4903b`` () =
    assertTooltipDoesNotContain "seq" (markAtEndOfMarker propSource "and s")
    assertTooltipContains "int" (markAtEndOfMarker propSource "and s")

[<Fact>]
let ``Regression.AccessorMutator.Bug4903c`` () =
    assertTooltipContains "string" (markAtEndOfMarker propSource "let g")

[<Theory>]
[<InlineData("member source.Pr", "Prop")>]
[<InlineData("member source.Pr", "int")>]
[<InlineData("member sou", "source")>]
let ``Regression.AccessorMutator.Bug4903efg`` (marker: string) (expected: string) =
    assertTooltipContains expected (markAtEndOfMarker propSource marker)

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_8`` () =
    assertTooltipContainsInOrder
        [ "property Foo.Property: string"; "A comment" ]
        """type Foo =
    /// A comment
    static member Property
        with get() = ""

let x() = Foo.Prop{caret}erty"""
