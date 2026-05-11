namespace FSharp.Test

module Assert =
    open System.Collections
    open Xunit
    open System.IO

    // Equivalent, with message
    let inline shouldBeEqualWith (expected : ^T) (message: string) (actual: ^U) =
        try
            Assert.Equivalent(expected, actual)
        with e ->
            Assert.True(false, message);

    let inline shouldBeEquivalentTo (expected : ^T) (actual : ^U) =
        Assert.Equivalent(expected, actual)

    let inline shouldBe (expected : ^T) (actual : ^U) =
        Assert.Equal(expected :> obj, actual :> obj)

    let inline shouldStartWith (expected : string) (actual : string) =
        Assert.StartsWith(expected, actual)

    let inline shouldContain (needle : string) (haystack : string) =
        Assert.Contains(needle, haystack)

    let inline areEqual (expected: ^T) (actual: ^T) =
        Assert.Equal<^T>(expected, actual)

    let inline shouldEqual (x: 'a) (y: 'a) =
        Assert.Equal<'a>(x, y)

    let inline shouldBeEmpty (actual : ^T when ^T :> IEnumerable) =
        Assert.Empty(actual)

    let inline shouldNotBeEmpty (actual : ^T when ^T :> IEnumerable) =
        Assert.NotEmpty(actual)

    let shouldBeFalse (actual: bool) =
        Assert.False(actual)

    let shouldBeTrue (actual: bool) =
        Assert.True(actual)

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

    let shouldBeEqualCollections (expected: seq<'T>) (actual: seq<'T>) =
        let expectedList = Seq.toList expected
        let actualList = Seq.toList actual
        if expectedList = actualList then
            ()
        else
            let unexpectedlyMissing = List.except actualList expectedList
            let unexpectedlyPresent = List.except expectedList actualList
            let inline newLine (sb: System.Text.StringBuilder) = sb.AppendLine() |> ignore
            let sb = System.Text.StringBuilder()
            if not unexpectedlyMissing.IsEmpty then
                sb.Append "Unexpectedly missing (expected, not actual):" |> ignore
                for item in unexpectedlyMissing do
                    newLine sb
                    sb.Append "    " |> ignore
                    sb.Append(sprintf "%A" item) |> ignore
                newLine sb
                newLine sb
            if not unexpectedlyPresent.IsEmpty then
                sb.Append "Unexpectedly present (actual, not expected):" |> ignore
                for item in unexpectedlyPresent do
                    newLine sb
                    sb.Append "    " |> ignore
                    sb.Append(sprintf "%A" item) |> ignore
                newLine sb
                newLine sb
            if unexpectedlyMissing.IsEmpty && unexpectedlyPresent.IsEmpty then
                sb.Append "Same elements but in different order. Positional differences:" |> ignore
                for i in 0 .. expectedList.Length - 1 do
                    if expectedList[i] <> actualList[i] then
                        newLine sb
                        sb.Append(sprintf "    [%d] expected %A but was %A" i expectedList[i] actualList[i]) |> ignore
                newLine sb
            Assert.True(false, sb.ToString())

