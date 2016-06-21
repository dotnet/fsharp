module ``FSharpQA-Tests-ClrFx-PseudoCustomAttributes``

open NUnit.Framework

open NUnitConf
open RunPlTest


module AssemblyAlgorithmId =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyAlgorithmId")>]
    let AssemblyAlgorithmId () = runpl |> check


module AssemblyCompany =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyCompany")>]
    let AssemblyCompany () = runpl |> check

module AssemblyConfiguration =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyConfiguration")>]
    let AssemblyConfiguration () = runpl |> check


module AssemblyCopyright =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyCopyright")>]
    let AssemblyCopyright () = runpl |> check


module AssemblyDescription =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyDescription")>]
    let AssemblyDescription () = runpl |> check


module AssemblyFileVersion =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyFileVersion")>]
    let AssemblyFileVersion () = runpl |> check


module AssemblyInformationalVersion =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyInformationalVersion")>]
    let AssemblyInformationalVersion () = runpl |> check


module AssemblyProduct =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyProduct")>]
    let AssemblyProduct () = runpl |> check


module AssemblyTitle =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyTitle")>]
    let AssemblyTitle () = runpl |> check


module AssemblyTrademark =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyTrademark")>]
    let AssemblyTrademark () = runpl |> check


module AssemblyVersion =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/AssemblyVersion")>]
    let AssemblyVersion () = runpl |> check


module NYI_AssemblyCulture =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/NYI_AssemblyCulture")>]
    let NYI_AssemblyCulture () = runpl |> check


module NYI_AssemblyDefaultAlias =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/NYI_AssemblyDefaultAlias")>]
    let NYI_AssemblyDefaultAlias () = runpl |> check


module NYI_AssemblyDelaySign =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/NYI_AssemblyDelaySign")>]
    let NYI_AssemblyDelaySign () = runpl |> check


module NYI_AssemblyFlags =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/NYI_AssemblyFlags")>]
    let NYI_AssemblyFlags () = runpl |> check


module NYI_AssemblyKeyFile =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/NYI_AssemblyKeyFile")>]
    let NYI_AssemblyKeyFile () = runpl |> check


module NYI_AssemblyKeyName =

    [<Test; FSharpQASuiteTest("ClrFx/PseudoCustomAttributes/NYI_AssemblyKeyName")>]
    let NYI_AssemblyKeyName () = runpl |> check
