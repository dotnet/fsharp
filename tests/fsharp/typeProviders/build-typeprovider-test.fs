module BuildTypeProviderTest

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open PlatformHelpers
open NUnitConf

let build (cfg: TestConfig) (dir: string) p = attempt {
    let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
    let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
    let del = Commands.rm dir

    // if EXIST provided.dll del provided.dll
    del "provided.dll"

    // "%FSC%" --out:provided.dll -a ..\helloWorld\provided.fs
    do! fsc "--out:provided.dll -a" [".."/"helloWorld"/"provided.fs"]

    // if EXIST providedJ.dll del providedJ.dll
    del "providedJ.dll"

    // "%FSC%" --out:providedJ.dll -a ..\helloWorld\providedJ.fs
    do! fsc "--out:providedJ.dll -a" [".."/"helloWorld"/"providedJ.fs"]

    // if EXIST providedK.dll del providedK.dll
    del "providedK.dll"

    // "%FSC%" --out:providedK.dll -a ..\helloWorld\providedK.fs
    do! fsc "--out:providedK.dll -a" [".."/"helloWorld"/"providedK.fs"]

    // if EXIST provider.dll del provider.dll
    del "provider.dll"

    // "%FSC%" --out:provider.dll -a provider.fsx
    do! fsc "--out:provider.dll -a" ["provider.fsx"]

    // call %~d0%~p0..\single-test-build.bat
    do! SingleTestBuild.singleTestBuild cfg dir p

    }
