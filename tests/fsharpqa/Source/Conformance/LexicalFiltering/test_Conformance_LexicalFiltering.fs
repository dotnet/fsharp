module ``FSharpQA-Tests-Conformance-LexicalFiltering``

open NUnit.Framework

open NUnitConf
open RunPlTest


module Basic =

    [<Test; FSharpQASuiteTest("Conformance/LexicalFiltering/Basic/ByExample")>]
    let ByExample () = runpl |> check

    [<Test; FSharpQASuiteTest("Conformance/LexicalFiltering/Basic/OffsideExceptions")>]
    let OffsideExceptions () = runpl |> check


module HashLight =

    [<Test; FSharpQASuiteTest("Conformance/LexicalFiltering/HashLight")>]
    let HashLight () = runpl |> check


module HighPrecedenceApplication =

    [<Test; FSharpQASuiteTest("Conformance/LexicalFiltering/HighPrecedenceApplication")>]
    let HighPrecedenceApplication () = runpl |> check


module LexicalAnalysisOfTypeApplications =

    [<Test; FSharpQASuiteTest("Conformance/LexicalFiltering/LexicalAnalysisOfTypeApplications")>]
    let LexicalAnalysisOfTypeApplications () = runpl |> check
