module ``FSharp-Tests-Regression``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

let testContext = FSharpTestSuite.testContext


module ``26`` = 

    [<Test; FSharpSuiteFscCodePermutation("regression/26")>]
    let ``26`` p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``321`` = 

    [<Test; FSharpSuiteFscCodePermutation("regression/321")>]
    let ``321`` p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``655`` = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"

        // "%FSC%" %fsc_flags% -a -o:pack.dll xlibC.ml
        do! fsc "%s -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

        // "%PEVERIFY%" pack.dll
        do! peverify "pack.dll"

        // "%FSC%" %fsc_flags%    -o:test.exe -r:pack.dll main.fs
        do! fsc "%s    -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

        // "%PEVERIFY%" test.exe
        do! peverify "test.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"

        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""

        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    [<Test; FSharpSuiteTest("regression/655")>]
    let ``655`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module ``656`` = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"

        // "%FSC%" %fsc_flags% -o:pack.exe misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs
        do! fsc "%s -o:pack.exe" cfg.fsc_flags ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

        // "%PEVERIFY%" pack.exe
        do! peverify "pack.exe"
        }

    [<Test; FSharpSuiteTest("regression/656")>]
    let ``656`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir
                
        })



module ``83`` = 

    [<Test; FSharpSuiteFscCodePermutation("regression/83")>]
    let ``83`` p = check (attempt {
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

    [<Test; FSharpSuiteFscCodePermutation("regression/84")>]
    let ``84`` p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``85`` = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"

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

    [<Test; FSharpSuiteTest("regression/85")>]
    let ``85`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        // REM build.bat produces only dll's. Nothing to run
                
        })


module ``86`` = 

    [<Test; FSharpSuiteFscCodePermutation("regression/86")>]
    let ``86`` p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module ``Tuple-bug-1`` = 

    [<Test; FSharpSuiteFscCodePermutation("regression/tuple-bug-1")>]
    let ``tuple-bug-1`` p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })
