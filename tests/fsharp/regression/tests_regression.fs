module ``FSharp-Tests-Regression``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

let setTestDataInfo name = FSharpTestSuite.setTestDataInfo ("regression", name)

let testContext () =
    { Directory = NUnit.Framework.TestContext.CurrentContext.Test.Properties.["DIRECTORY"] :?> string;
      Config = suiteHelpers.Value }


module ``26`` = 
    let permutations = 
        FSharpTestSuite.allPermutation
        |> List.map (fun p -> (new TestCaseData (p)).SetCategory(sprintf "%A" p) |> setTestDataInfo "26")

    [<Test; TestCaseSource("permutations")>]
    let ``26`` p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``321`` = 
    let permutations = 
        FSharpTestSuite.allPermutation
        |> List.map (fun p -> (new TestCaseData (p)).SetCategory(sprintf "%A" p) |> setTestDataInfo "321")

    [<Test; TestCaseSource("permutations")>]
    let ``321`` p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``655`` = 

    let build cfg dir = processor {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY

        // "%FSC%" %fsc_flags% -a -o:pack.dll xlibC.ml
        do! fsc "%s -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

        // "%PEVERIFY%" pack.dll
        do! peverify "pack.dll"

        // "%FSC%" %fsc_flags%    -o:test.exe -r:pack.dll main.fs
        do! fsc "%s    -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

        // "%PEVERIFY%" test.exe
        do! peverify "test.exe"

        }

    let run cfg dir = processor {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"

        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""

        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let testData = [ (new TestCaseData()) |> setTestDataInfo "655" ]

    [<Test; TestCaseSource("testData")>]
    let ``655`` () = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


[<Category("fail_new"); Category("fail_reason_ILX_CONFIG")>]
module ``656`` = 

    let build cfg dir = processor {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Printf.ksprintf (Commands.peverify exec cfg.PEVERIFY)

        //REVIEW ILX_CONFIG?
        let ILX_CONFIG = ""

        // "%FSC%" %fsc_flags% -o:pack%ILX_CONFIG%.exe misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs
        do! fsc "%s -o:pack%s.exe" cfg.fsc_flags ILX_CONFIG ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

        // "%PEVERIFY%" pack%ILX_CONFIG%.exe
        do! peverify "pack%s.exe" ILX_CONFIG
        }

    let run cfg dir = processor {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        //REVIEW ILX_CONFIG?
        let ILX_CONFIG = ""

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"

        // %CLIX% pack%ILX_CONFIG%.exe
        do! exec ("."/(sprintf "pack%s.exe" ILX_CONFIG)) ""

        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        return! NUnitConf.genericError "env var 'ILX_CONFIG' not found, using '' as default the test pass"
        }

    let testData = [ (new TestCaseData()) |> setTestDataInfo "656" ]

    [<Test; TestCaseSource("testData")>]
    let ``656`` () = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })



module ``83`` = 
    let permutations = 
        FSharpTestSuite.allPermutation
        |> List.map (fun p -> (new TestCaseData (p)).SetCategory(sprintf "%A" p) |> setTestDataInfo "83")

    [<Test; TestCaseSource("permutations")>]
    let ``83`` p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()

        // if "%CLR_SUPPORTS_WINFORMS%"=="false" ( goto Skip)
        do! match cfg.EnvironmentVariables |> Map.tryFind "CLR_SUPPORTS_WINFORMS" |> Option.map (fun s -> s.ToLower()) with
            | Some "false" -> NUnitConf.skip "env var CLR_SUPPORTS_WINFORMS is false"
            | Some _ | None -> Success
           
        // call %~d0%~p0..\..\single-test-build.bat
        do! SingleTestBuild.singleTestBuild cfg dir p

        // if "%CLR_SUPPORTS_WINFORMS%"=="false" ( goto Skip )
        ignore "already skipped if CLR_SUPPORTS_WINFORMS == false"

        // if "%COMPLUS_Version%"=="v1.0.3705" ( goto Skip )
        do! match cfg.EnvironmentVariables |> Map.tryFind "COMPLUS_Version"  |> Option.map (fun s -> s.ToLower()) with
            | Some "v1.0.3705" -> NUnitConf.skip "env var COMPLUS_Version is v1.0.3705"
            | Some _ | None -> Success

        // call %~d0%~p0..\..\single-test-run.bat
        do! SingleTestRun.singleTestRun cfg dir p
        })



module ``84`` = 
    let permutations = 
        FSharpTestSuite.allPermutation
        |> List.map (fun p -> (new TestCaseData (p)).SetCategory(sprintf "%A" p) |> setTestDataInfo "84")

    [<Test; TestCaseSource("permutations")>]
    let ``84`` p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``85`` = 

    let build cfg dir = processor {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY

        // if "%CLR_SUPPORTS_GENERICS%"=="false" ( goto Skip)
        do! match cfg.EnvironmentVariables |> Map.tryFind "CLR_SUPPORTS_GENERICS" |> Option.map (fun s -> s.ToLower()) with
            | Some "false" -> NUnitConf.skip "env var CLR_SUPPORTS_GENERICS is false"
            | Some _ | None -> Success

        // if "%CLR_SUPPORTS_SYSTEM_WEB%"=="false" ( goto Skip)
        do! match cfg.EnvironmentVariables |> Map.tryFind "CLR_SUPPORTS_SYSTEM_WEB" |> Option.map (fun s -> s.ToLower()) with
            | Some "false" -> NUnitConf.skip "env var CLR_SUPPORTS_SYSTEM_WEB is false"
            | Some _ | None -> Success

        // "%FSC%" %fsc_flags% -r:Category.dll -a -o:petshop.dll Category.ml
        do! fsc "%s -r:Category.dll -a -o:petshop.dll" cfg.fsc_flags ["Category.ml"]

        // "%PEVERIFY%" petshop.dll
        do! peverify "petshop.dll"

        }

    let testData = [ (new TestCaseData()) |> setTestDataInfo "85" ]

    [<Test; TestCaseSource("testData")>]
    let ``85`` () = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        // REM build.bat produces only dll's. Nothing to run
                
        })


module ``86`` = 
    let permutations = 
        FSharpTestSuite.allPermutation
        |> List.map (fun p -> (new TestCaseData (p)).SetCategory(sprintf "%A" p) |> setTestDataInfo "86")

    [<Test; TestCaseSource("permutations")>]
    let ``86`` p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``Tuple-bug-1`` = 
    let permutations = 
        FSharpTestSuite.allPermutation
        |> List.map (fun p -> (new TestCaseData (p)).SetCategory(sprintf "%A" p) |> setTestDataInfo "tuple-bug-1")

    [<Test; TestCaseSource("permutations")>]
    let ``tuple-bug-1`` p = check (processor {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })
