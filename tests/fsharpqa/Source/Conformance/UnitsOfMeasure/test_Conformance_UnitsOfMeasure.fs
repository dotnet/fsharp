module ``FSharpQA-Tests-Conformance-UnitsOfMeasure``

open NUnit.Framework

open NUnitConf
open RunPlTest



module Basic =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Basic")>]
    let Basic () = runpl |> check


module Bounds =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Bounds")>]
    let Bounds () = runpl |> check


module Constants =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Constants")>]
    let Constants () = runpl |> check


module Diagnostics =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Diagnostics")>]
    let Diagnostics () = runpl |> check


module Operators =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Operators")>]
    let Operators () = runpl |> check


module Parenthesis =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Parenthesis")>]
    let Parenthesis () = runpl |> check


module Parsing =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/Parsing")>]
    let Parsing () = runpl |> check



module TypeChecker =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/TypeChecker")>]
    let TypeChecker () = runpl |> check


module WithOOP =

    [<Test; FSharpQASuiteTest("Conformance/UnitsOfMeasure/WithOOP")>]
    let WithOOP () = runpl |> check
