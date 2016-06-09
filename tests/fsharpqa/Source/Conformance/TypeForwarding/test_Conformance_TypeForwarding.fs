module ``FSharpQA-Tests-Conformance-TypeForwarding``

open NUnit.Framework

open NUnitConf
open RunPlTest



module Class =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Class")>]
    let Class () = runpl |> check


module Cycle =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Cycle")>]
    let Cycle () = runpl |> check


module Delegate =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Delegate")>]
    let Delegate () = runpl |> check


module Interface =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Interface")>]
    let Interface () = runpl |> check


module Nested =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Nested")>]
    let Nested () = runpl |> check


module Struct =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Struct")>]
    let Struct () = runpl |> check

