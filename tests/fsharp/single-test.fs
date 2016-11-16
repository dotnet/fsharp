module SingleTest

open System
open System.IO
open System.Diagnostics
open NUnit.Framework

open PlatformHelpers
open NUnitConf
open FSharpTestSuiteTypes


let singleTestBuildAndRunAux cfg p = 

    //remove FSharp.Core.dll from the target directory to ensure that compiler uses the correct FSharp.Core.dll
    do if fileExists cfg "FSharp.Core.dll" then rm cfg "FSharp.Core.dll"

    let source1 = 
        ["test.ml"; "test.fs"] 
        |> List.rev
        |> List.tryFind (fileExists cfg)

    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter (fileExists cfg)


    let doPeverify file = peverify cfg file 

    match p with 
    | FSI_FILE -> 
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsi cfg "%s" cfg.fsi_flags sources

        testOkFile.CheckExists()

    | FSI_STDIN -> 
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsiStdin cfg (sources |> List.rev |> List.head) "" [] //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        testOkFile.CheckExists()
    | FSI_STDIN_OPT -> 
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsiStdin cfg (sources |> List.rev |> List.head) "--optimize" [] //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        testOkFile.CheckExists()
    | FSI_STDIN_GUI -> 
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsiStdin cfg (sources |> List.rev |> List.head) "--gui" [] //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        testOkFile.CheckExists()
    | FSC_CORECLR -> 
        let platform = "win7-x64"
        fsi cfg """%s --targetPlatformName:.NETStandard,Version=v1.6/%s --source:"coreclr_utilities.fs" --source:"%s" --packagesDir:..\..\packages --projectJsonLock:%s --fsharpCore:%s --define:NETSTANDARD1_6 --define:FSCORE_PORTABLE_NEW --define:FX_PORTABLE_OR_NETSTANDARD --compilerPath:%s --copyCompiler:yes --verbose:verbose --exec """
               cfg.fsi_flags
               platform
               (String.concat " " sources)
               (__SOURCE_DIRECTORY__ ++ "project.lock.json")
               (__SOURCE_DIRECTORY__ ++ sprintf @"..\testbin\%s\coreclr\fsc\%s\FSharp.Core.dll" cfg.BUILD_CONFIG platform)
               (__SOURCE_DIRECTORY__ ++ sprintf @"..\testbin\%s\coreclr\fsc\%s" cfg.BUILD_CONFIG  platform)
               [__SOURCE_DIRECTORY__ ++ "..\fsharpqa\testenv\src\deployProj\CompileProj.fsx"]

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")
(*
:FSC_CORECLR
@echo do :FSC_CORECLR
    set platform=win7-x64
    set packagesDir=%~d0%~p0..\..\packages
    For %%A in ("%cd%") do ( Set TestCaseName=%%~nxA)
    echo   %~d0%~p0..\testbin\%flavor%\coreclr\%platform%\corerun.exe %~d0%~p0..\testbin\%flavor%\coreclr\fsharp\core\%TestCaseName%\output\test.exe > coreclr.run.cmd
    %~d0%~p0..\testbin\%flavor%\coreclr\%platform%\corerun.exe %~d0%~p0..\testbin\%flavor%\coreclr\fsharp\core\%TestCaseName%\output\test.exe
    )
*)
        testOkFile.CheckExists()

    | FSC_BASIC -> 
        fsc cfg "%s --define:BASIC_TEST -o:test.exe -g" cfg.fsc_flags sources 

        doPeverify "test.exe"
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("."/"test.exe") ""

        testOkFile.CheckExists()

    | FSC_BASIC_64 -> 
        fsc cfg "%s --define:BASIC_TEST --platform:x64 -o:testX64.exe -g" cfg.fsc_flags sources

        doPeverify "testX64.exe"

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("."/"testX64.exe") ""

        testOkFile.CheckExists()
    | GENERATED_SIGNATURE -> 
      if fileExists cfg "test.fs" then 

        log "Generating signature file..."

        source1 |> Option.iter (fun from -> copy_y cfg from "tmptest.fs")

        fsc cfg "%s --sig:tmptest.fsi" cfg.fsc_flags ["tmptest.fs"]

        log "Compiling against generated interface file..."
        fsc cfg "%s -o:tmptest1.exe" cfg.fsc_flags ["tmptest.fsi";"tmptest.fs"]

        doPeverify "tmptest1.exe"

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("."/"tmptest1.exe") ""

        testOkFile.CheckExists()
    | FSC_OPT_MINUS_DEBUG -> 
        fsc cfg "%s --optimize- --debug -o:test--optminus--debug.exe -g" cfg.fsc_flags sources

        doPeverify "test--optminus--debug.exe"
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("."/"test--optminus--debug.exe") ""

        testOkFile.CheckExists()
    | FSC_OPT_PLUS_DEBUG -> 
        fsc cfg "%s --optimize+ --debug -o:test--optplus--debug.exe -g" cfg.fsc_flags sources

        doPeverify "test--optplus--debug.exe"

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("."/"test--optplus--debug.exe") ""

        testOkFile.CheckExists()
    | AS_DLL -> 
        // Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
        // THe second compilation will not utilize the information from the first in any meaningful way, but the
        // compiler will unpickle the interface and optimization data, so we test unpickling as well.

        fsc cfg "%s --optimize -a -o:test--optimize-lib.dll -g" cfg.fsc_flags sources

        fsc cfg "%s --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g" cfg.fsc_flags sources

        doPeverify "test--optimize-lib.dll"

        doPeverify "test--optimize-client-of-lib.exe"

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("."/"test--optimize-client-of-lib.exe") ""

        testOkFile.CheckExists()

let singleTestBuildAndRun dir p = 
    let cfg = testConfig dir
        
    singleTestBuildAndRunAux cfg p



let singleNegTest (cfg: TestConfig) testname = 

    // REM == Set baseline (fsc vs vs, in case the vs baseline exists)
    let VSBSLFILE = 
        if (sprintf "%s.vsbsl" testname) |> (fileExists cfg)
        then sprintf "%s.vsbsl" testname
        else sprintf "%s.bsl" testname

    let sources = [
        let src = [ testname + ".mli"; testname + ".fsi"; testname + ".ml"; testname + ".fs"; testname +  ".fsx";
                    testname + "a.mli"; testname + "a.fsi"; testname + "a.ml"; testname + "a.fs"; 
                    testname + "b.mli"; testname + "b.fsi"; testname + "b.ml"; testname + "b.fs"; ]

        yield! src |> List.filter (fileExists cfg)
    
        if fileExists cfg "helloWorldProvider.dll" then 
            yield "-r:helloWorldProvider.dll"

        if fileExists cfg (testname + "-pre.fs") then 
            yield (sprintf "-r:%s-pre.dll" testname)

        ]

    if fileExists cfg (testname + "-pre.fs")
        then fsc cfg "%s -a -o:%s-pre.dll" cfg.fsc_flags testname [testname + "-pre.fs"] 
        else ()

    // echo Negative typechecker testing: %testname%
    log "Negative typechecker testing: %s" testname

    let fsdiff a b = 
        let out = new ResizeArray<string>()
        let redirectOutputToFile path args =
            log "%s %s" path args
            use toLog = redirectToLog ()
            Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } cfg.Directory cfg.EnvironmentVariables path args
        (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> checkResult
        out.ToArray() |> List.ofArray

    fscAppendErrExpectFail cfg  (sprintf "%s.err" testname) """%s --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%s.dll""" cfg.fsc_flags testname sources

    let testnameDiff = fsdiff (sprintf "%s.err" testname) (sprintf "%s.bsl" testname)

    match testnameDiff with
    | [] -> ()
    | l ->
        log "***** %s.err %s.bsl differed: a bug or baseline may neeed updating" testname testname
        failwith (sprintf "%s.err %s.bsl differ; %A" testname testname l)

    log "Good, output %s.err matched %s.bsl" testname testname

    fscAppendErrExpectFail cfg (sprintf "%s.vserr" testname) "%s --test:ContinueAfterParseFailure --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%s.dll" cfg.fsc_flags testname sources

    let testnameDiff = fsdiff (sprintf "%s.vserr" testname) VSBSLFILE

    match testnameDiff with
        | [] -> ()
        | l ->
            log "***** %s.vserr %s differed: a bug or baseline may neeed updating" testname VSBSLFILE
            failwith (sprintf "%s.vserr %s differ; %A" testname VSBSLFILE l)

    log "Good, output %s.vserr matched %s" testname VSBSLFILE

