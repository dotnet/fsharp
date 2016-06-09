module ``FSharpQA-Tests-Conformance-TypesAndTypeConstraints``

open NUnit.Framework

open NUnitConf
open RunPlTest

module CheckingSyntacticTypes =

    [<Test; FSharpQASuiteTest("Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes")>]
    let CheckingSyntacticTypes () = runpl |> check


module LogicalPropertiesOfTypes =

    [<Test; FSharpQASuiteTest("Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes")>]
    let LogicalPropertiesOfTypes () = runpl |> check

module TypeConstraints =

    [<Test; FSharpQASuiteTest("Conformance/TypesAndTypeConstraints/TypeConstraints")>]
    let TypeConstraints () = runpl |> check


module TypeParameterDefinitions =

    [<Test; FSharpQASuiteTest("Conformance/TypesAndTypeConstraints/TypeParameterDefinitions")>]
    let TypeParameterDefinitions () = runpl |> check
