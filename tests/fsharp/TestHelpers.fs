module Tests.TestHelpers

open System.IO


let assembleDiffMessage actual expected =
  
    let getLines text =
        use reader = new StringReader(text)
        Seq.initInfinite (fun _ -> reader.ReadLine()) 
        |> Seq.takeWhile (not << isNull)
        |> set
    let actual   = getLines actual  
    let expected = getLines expected
    //
    // Find types/members which exist in exactly one of the expected or actual surface areas.
    //

    /// Surface area types/members which were expected to be found but missing from the actual surface area.
    let unexpectedlyMissing = Set.difference expected actual

    /// Surface area types/members present in the actual surface area but weren't expected to be.
    let unexpectedlyPresent = Set.difference actual expected

    // If both sets are empty, the surface areas match so allow the test to pass.
    if Set.isEmpty unexpectedlyMissing
      && Set.isEmpty unexpectedlyPresent then
        None
    else
        // The surface areas don't match; prepare an easily-readable output message.
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
