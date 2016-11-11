module ``FSharp-Tests-Regression``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers
open SingleTest

[<Test>]
let ``26`` () = singleTestBuildAndRun "regression/26" FSC_OPT_PLUS_DEBUG

[<Test >]
let ``321`` () = singleTestBuildAndRun "regression/321" FSC_OPT_PLUS_DEBUG

[<Test>]
let ``655`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "regression/655"

    do! fsc cfg "%s -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

    do! peverify cfg "pack.dll"

    do! fsc cfg "%s    -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

    do! peverify cfg "test.exe"

    use testOkFile = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })


[<Test >]
let ``656`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "regression/656"

    do! fsc cfg "%s -o:pack.exe" cfg.fsc_flags ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

    do! peverify cfg  "pack.exe"
                
    })



[<Test>]
let ``83`` () = singleTestBuildAndRun "regression/83" FSC_OPT_PLUS_DEBUG

[<Test >]
let ``84`` () = singleTestBuildAndRun "regression/84" FSC_OPT_PLUS_DEBUG

[<Test >]
let ``85`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "regression/85"

    do! fsc cfg "%s -r:Category.dll -a -o:petshop.dll" cfg.fsc_flags ["Category.ml"]

    do! peverify cfg "petshop.dll"
                
    })

[<Test >]
let ``86`` () = singleTestBuildAndRun "regression/86" FSC_OPT_PLUS_DEBUG

[<Test >]
let ``tuple-bug-1`` () = singleTestBuildAndRun "regression/tuple-bug-1" FSC_OPT_PLUS_DEBUG
