module ``FSharpQA-Tests-EnvLst``

open System
open System.IO
open NUnit.Framework

open EnvLst

module ParseLine =

    let permutations () =
        [ TestCaseData("\tSOURCE=a", [ "SOURCE", "a" ])
          TestCaseData("\tSOURCE=\"a file\"", [ "SOURCE", "a file" ])
          TestCaseData("\tSOURCE=\"a file with \\\"escaped values\\\" \"", [ "SOURCE", "a file with \\\"escaped values\\\" " ])
          TestCaseData("\tV1=parse V2=multiple V3=\"values works\"", [ "V1","parse"; "V2","multiple"; "V3","values works" ])
        ]

    [<Test; TestCaseSource("permutations")>]
    let ``parse should split vars`` line expected =
        match line |> EnvLst.parseLine with
        | Choice2Of2 error -> Assert.Fail(sprintf "expected valid line, but got error '%s'" error)
        | Choice1Of2 None -> Assert.Fail("expected some vars parsed, but noone found")
        | Choice1Of2 (Some l) -> 
            let emptyData = { Tags = []; Vars = []; Comment = None } 
            let expectedData = EnvLstLine.Data { emptyData with Vars = expected }
            Assert.IsTrue((l = expectedData), (sprintf "Expected '%A', but was '%A'" expected l))





