namespace FSharp.Test

module Assert =
    open FluentAssertions
    open System.Collections
    open System.Text
    open System.IO

    let inline shouldBeEqualWith (expected : ^T) (message: string) (actual: ^U) =
        actual.Should().BeEquivalentTo(expected, message) |> ignore

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

    let shouldBeSameMultilineStringSets expectedText actualText =      
      
        let getLines text =
            use reader = new StringReader(text)
            Seq.initInfinite (fun _ -> reader.ReadLine()) 
            |> Seq.takeWhile (not << isNull)
            |> set
        let actual   = getLines actualText  
        let expected = getLines expectedText
       
        let unexpectedlyMissing = Set.difference expected actual       
        let unexpectedlyPresent = Set.difference actual expected

        // If both sets are empty, the surface areas match so allow the test to pass.
        if Set.isEmpty unexpectedlyMissing
          && Set.isEmpty unexpectedlyPresent then
            None
        else           
            let msg =
                let inline newLine (sb : System.Text.StringBuilder) = sb.AppendLine () |> ignore
                let sb = System.Text.StringBuilder ()
                sb.Append "Unexpectedly missing (expected, not actual):" |> ignore
                for s in unexpectedlyMissing do
                    newLine sb
                    sb.Append "    " |> ignore
                    sb.Append s |> ignore
                newLine sb
                newLine sb
                sb.Append "Unexpectedly present (actual, not expected):" |> ignore
                for s in unexpectedlyPresent do
                    newLine sb
                    sb.Append "    " |> ignore
                    sb.Append s |> ignore
                newLine sb
                sb.ToString ()

            Some msg

    /// Same as 'shouldBe' but goes pairwise over the collections. Lengths must be equal.
    let shouldBePairwiseEqual (x: seq<_>) (y: seq<_>) =
        // using enumerators, because Seq.iter2 allows different lengths silently
        let ex = x.GetEnumerator()
        let ey = y.GetEnumerator()
        let mutable countx = 0
        let mutable county = 0
        while ex.MoveNext() do
            countx <- countx + 1
            if ey.MoveNext() then
                county <- county + 1
                ey.Current |> shouldBe ex.Current

        while ex.MoveNext() do countx <- countx + 1
        while ey.MoveNext() do county <- county + 1
        if countx <> county then
            failwithf $"Collections are of unequal lengths, expected length {countx}, actual length is {county}."
