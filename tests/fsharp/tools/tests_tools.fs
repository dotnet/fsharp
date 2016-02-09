module ``FSharp-Tests-Tools``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

let testContext = FSharpTestSuite.testContext


module Bundle = 

    [<Test; FSharpSuiteTest("tools/bundle")>]
    let bundle () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"

        // "%FSC%" %fsc_flags% --progress --standalone -o:test-one-fsharp-module.exe -g test-one-fsharp-module.fs
        do! fsc "%s --progress --standalone -o:test-one-fsharp-module.exe -g" cfg.fsc_flags ["test-one-fsharp-module.fs"]
   
        // "%PEVERIFY%"  test-one-fsharp-module.exe
        do! peverify "test-one-fsharp-module.exe"
   
        // "%FSC%" %fsc_flags% -a -o:test_two_fsharp_modules_module_1.dll -g test_two_fsharp_modules_module_1.fs
        do! fsc "%s -a -o:test_two_fsharp_modules_module_1.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_1.fs"]
   
        // "%PEVERIFY%"  test_two_fsharp_modules_module_1.dll
        do! peverify "test_two_fsharp_modules_module_1.dll"
   
   
        // "%FSC%" %fsc_flags% --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g test_two_fsharp_modules_module_2.fs
        do! fsc "%s --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
        // "%PEVERIFY%"  test_two_fsharp_modules_module_2.exe
        do! peverify "test_two_fsharp_modules_module_2.exe"
   
        // "%FSC%" %fsc_flags% -a --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g test_two_fsharp_modules_module_2.fs
        do! fsc "%s -a --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
        // "%PEVERIFY%"  test_two_fsharp_modules_module_2_as_dll.dll
        do! peverify "test_two_fsharp_modules_module_2_as_dll.dll"
                
        })



module Eval = 

    [<Test; FSharpSuiteScriptPermutations("tools/eval")>]
    let eval p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })
