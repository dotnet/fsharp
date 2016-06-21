module ``FSharpQA-Tests``

open NUnit.Framework
open NUnitConf
open RunPlTest

module Simple = 

    [<Test; FSharpQASuiteTest("Simple")>]
    let Simple () = runpl |> check 

module EntryPoint = 

    [<Test; FSharpQASuiteTest("EntryPoint")>]
    let EntryPoint () = runpl |> check 

module Globalization = 

    [<Test; FSharpQASuiteTest("Globalization")>]
    let Globalization () = runpl |> check 

module Import = 

    [<Test; FSharpQASuiteTest("Import")>]
    let Import () = runpl |> check 

module Misc = 

    [<Test; FSharpQASuiteTest("Misc")>]
    let Misc () = runpl |> check 

module MultiTargeting = 

    [<Test; FSharpQASuiteTest("MultiTargeting")>]
    let MultiTargeting () = runpl |> check 

module OCamlCompat = 

    [<Test; FSharpQASuiteTest("OCamlCompat")>]
    let OCamlCompat () = runpl |> check 

module Printing = 

    [<Test; FSharpQASuiteTest("Printing")>]
    let Printing () = runpl |> check 

module Stress = 

    [<Test; FSharpQASuiteTest("Stress")>]
    let Stress () = runpl |> check 

module Warnings = 

    [<Test; FSharpQASuiteTest("Warnings")>]
    let Warnings () = runpl |> check 
