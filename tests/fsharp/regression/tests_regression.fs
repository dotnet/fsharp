module ``FSharp-Tests-Regression``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers
open SingleTest

[<Test; FSharpSuiteFscCodePermutation("regression/26")>]
let ``26`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteFscCodePermutation("regression/321")>]
let ``321`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("regression/655")>]
let ``655`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -a -o:pack.dll xlibC.ml
    do! fsc cfg "%s -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

    // "%PEVERIFY%" pack.dll
    do! peverify cfg "pack.dll"

    // "%FSC%" %fsc_flags%    -o:test.exe -r:pack.dll main.fs
    do! fsc cfg "%s    -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

    // "%PEVERIFY%" test.exe
    do! peverify cfg "test.exe"

    // if exist test.ok (del /f /q test.ok)
    use testOkFile = fileguard cfg "test.ok"

    // %CLIX% test.exe
    do! exec cfg ("."/"test.exe") ""

    // if NOT EXIST test.ok goto SetError
    do! testOkFile |> NUnitConf.checkGuardExists
                
    })


[<Test; FSharpSuiteTest("regression/656")>]
let ``656`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:pack.exe misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs
    do! fsc cfg "%s -o:pack.exe" cfg.fsc_flags ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

    // "%PEVERIFY%" pack.exe
    do! peverify cfg  "pack.exe"
                
    })



[<Test; FSharpSuiteFscCodePermutation("regression/83")>]
let ``83`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteFscCodePermutation("regression/84")>]
let ``84`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("regression/85")>]
let ``85`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -r:Category.dll -a -o:petshop.dll Category.ml
    do! fsc cfg "%s -r:Category.dll -a -o:petshop.dll" cfg.fsc_flags ["Category.ml"]

    do! peverify cfg "petshop.dll"
                
    })

[<Test; FSharpSuiteFscCodePermutation("regression/86")>]
let ``86`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteFscCodePermutation("regression/tuple-bug-1")>]
let ``tuple-bug-1`` p = singleTestBuildAndRun p
