namespace FSharp.Test.Utilities

module Assert =
    open FluentAssertions
    open System.Collections

    let inline shouldBeEquivalentTo (expected : ^T) (actual : ^U) =
        actual.Should().BeEquivalentTo(expected, "") |> ignore

    let inline shouldBe (expected : ^T) (actual : ^U) =
        actual.Should().Be(expected, "") |> ignore

    let inline shouldBeEmpty (actual : ^T when ^T :> IEnumerable) =
        actual.Should().BeEmpty("") |> ignore

    let inline shouldNotBeEmpty (actual : ^T when ^T :> IEnumerable) =
        actual.Should().NotBeEmpty("") |> ignore

    let shouldBeFalse (actual: bool) =
        actual.Should().BeFalse("") |> ignore

    let shouldBeTrue (actual: bool) =
        actual.Should().BeTrue("") |> ignore
