module ``FSharp-Tests-Tools``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers
open SingleTest

[<Test>]
let bundle () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "tools/bundle"

    do! fsc cfg "%s --progress --standalone -o:test-one-fsharp-module.exe -g" cfg.fsc_flags ["test-one-fsharp-module.fs"]
   
    do! peverify cfg "test-one-fsharp-module.exe"
   
    do! fsc cfg "%s -a -o:test_two_fsharp_modules_module_1.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_1.fs"]
   
    do! peverify cfg "test_two_fsharp_modules_module_1.dll"
   
    do! fsc cfg "%s --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
    do! peverify cfg "test_two_fsharp_modules_module_2.exe"
   
    do! fsc cfg "%s -a --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
    do! peverify cfg "test_two_fsharp_modules_module_2_as_dll.dll"
                
    })

[<Test>]
let eval () = singleTestBuildAndRun "tools/eval" FSC_OPT_PLUS_DEBUG
