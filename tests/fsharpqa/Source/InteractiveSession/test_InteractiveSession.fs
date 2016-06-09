module ``FSharpQA-Tests-InteractiveSession``

open NUnit.Framework

open NUnitConf
open RunPlTest

module Misc =

    [<Test; FSharpQASuiteTest("InteractiveSession/Misc")>]
    let Misc () = runpl |> check
