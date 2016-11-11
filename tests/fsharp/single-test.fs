module SingleTest

open System
open System.IO
open System.Diagnostics
open NUnit.Framework

open PlatformHelpers
open NUnitConf
open FSharpTestSuiteTypes

let skipIfExists cfg file = attempt {
    if fileExists cfg file then 
        return! NUnitConf.skip (sprintf "file '%s' found" file)
    }


let skipIfNotExists cfg file = attempt {
    if not (fileExists cfg file) then 
        return! NUnitConf.skip (sprintf "file '%s' not found" file)
    }


let singleTestBuildAndRunAux cfg p = attempt {

    //remove FSharp.Core.dll from the target directory to ensure that compiler uses the correct FSharp.Core.dll
    do if fileExists cfg "FSharp.Core.dll" then rm cfg "FSharp.Core.dll"

    let source1 = 
        ["test.ml"; "test.fs"] 
        |> List.rev
        |> List.tryFind (fileExists cfg)

    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter (fileExists cfg)


    let doPeverify file = attempt {
        do! skipIfExists cfg "dont.run.peverify"
        
        do! peverify cfg file 
        }

    match p with 
    | FSI_FILE -> 
        do! skipIfExists cfg "dont.run.as.script"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! fsi cfg "%s" cfg.fsi_flags sources

        do! testOkFile |> NUnitConf.checkGuardExists

    | FSI_STDIN -> 
        do! skipIfExists cfg "dont.pipe.to.stdin"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! ``fsi <`` cfg "%s" cfg.fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        do! testOkFile |> NUnitConf.checkGuardExists
    | FSI_STDIN_OPT -> 
        do! skipIfExists cfg "dont.pipe.to.stdin"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! ``fsi <`` cfg "%s --optimize" cfg.fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        do! testOkFile |> NUnitConf.checkGuardExists
    | FSI_STDIN_GUI -> 
        do! skipIfExists cfg "dont.pipe.to.stdin"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! ``fsi <`` cfg "%s --gui" cfg.fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        do! testOkFile |> NUnitConf.checkGuardExists
    | FSC_CORECLR -> 
        let platform = "win7-x64"
        do! fsi cfg """%s --targetPlatformName:.NETStandard,Version=v1.6/%s --source:"coreclr_utilities.fs" --source:"%s" --packagesDir:..\..\packages --projectJsonLock:%s --fsharpCore:%s --define:CoreClr --define:PortableNew --compilerPath:%s --copyCompiler:yes --verbose:verbose --exec """
               cfg.fsi_flags
               platform
               (String.concat " " sources)
               (__SOURCE_DIRECTORY__ ++ "project.lock.json")
               (__SOURCE_DIRECTORY__ ++ sprintf @"..\testbin\%s\coreclr\fsc\%s\FSharp.Core.dll" cfg.BUILD_CONFIG platform)
               (__SOURCE_DIRECTORY__ ++ sprintf @"..\testbin\%s\coreclr\fsc\%s" cfg.BUILD_CONFIG  platform)
               [__SOURCE_DIRECTORY__ ++ "..\fsharpqa\testenv\src\deployProj\CompileProj.fsx"]

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")
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
        do! testOkFile |> NUnitConf.checkGuardExists
        return ()

    | FSC_BASIC -> 
        do! fsc cfg "%s --define:BASIC_TEST -o:test.exe -g" cfg.fsc_flags sources 

        do! doPeverify "test.exe"
        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! exec cfg ("."/"test.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists

    | FSC_BASIC_64 -> 
        do! fsc cfg "%s --define:BASIC_TEST --platform:x64 -o:testX64.exe -g" cfg.fsc_flags sources

        do! doPeverify "testX64.exe"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! exec cfg ("."/"testX64.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
    | GENERATED_SIGNATURE -> 
        do! skipIfExists cfg "dont.use.generated.signature"

        do! skipIfNotExists cfg "test.fs"

        log "Generating signature file..."

        do! source1 |> Option.map (fun from -> copy_y cfg from "tmptest.fs")

        do! fsc cfg "%s --sig:tmptest.fsi" cfg.fsc_flags ["tmptest.fs"]

        log "Compiling against generated interface file..."
        do! fsc cfg "%s -o:tmptest1.exe" cfg.fsc_flags ["tmptest.fsi";"tmptest.fs"]

        do! doPeverify "tmptest1.exe"

        do! skipIfExists cfg "dont.use.generated.signature"

        do! skipIfNotExists cfg "test.fs"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! exec cfg ("."/"tmptest1.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
    | FSC_OPT_MINUS_DEBUG -> 
        do! fsc cfg "%s --optimize- --debug -o:test--optminus--debug.exe -g" cfg.fsc_flags sources

        do! doPeverify "test--optminus--debug.exe"
        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! exec cfg ("."/"test--optminus--debug.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
    | FSC_OPT_PLUS_DEBUG -> 
        do! fsc cfg "%s --optimize+ --debug -o:test--optplus--debug.exe -g" cfg.fsc_flags sources

        do! doPeverify "test--optplus--debug.exe"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! exec cfg ("."/"test--optplus--debug.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
    | AS_DLL -> 
        // Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
        // THe second compilation will not utilize the information from the first in any meaningful way, but the
        // compiler will unpickle the interface and optimization data, so we test unpickling as well.

        do! skipIfExists cfg "dont.compile.test.as.dll"

        do! fsc cfg "%s --optimize -a -o:test--optimize-lib.dll -g" cfg.fsc_flags sources

        do! fsc cfg "%s --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g" cfg.fsc_flags sources

        do! doPeverify "test--optimize-lib.dll"

        do! doPeverify "test--optimize-client-of-lib.exe"

        use testOkFile = FileGuard.create (getfullpath cfg "test.ok")

        do! exec cfg ("."/"test--optimize-client-of-lib.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists



    }

let singleTestBuildAndRun dir p = check (attempt {
    let cfg = FSharpTestSuite.testConfig dir
        
    do! singleTestBuildAndRunAux cfg p
    })



let private singleNegTestAux (cfg: TestConfig) testname = attempt {

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

    do! if fileExists cfg (testname + "-pre.fs")
        then fsc cfg "%s -a -o:%s-pre.dll" cfg.fsc_flags testname [testname + "-pre.fs"] 
        else Success ()

    // echo Negative typechecker testing: %testname%
    log "Negative typechecker testing: %s" testname

    let ``fail fsc 2> a`` = 
        let ``exec 2>`` errPath = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Error(Overwrite(errPath)); Input = None }
        let checkErrorLevel1 = function 
            | CmdResult.ErrorLevel (_,1) -> Success
            | CmdResult.Success | CmdResult.ErrorLevel _ -> NUnitConf.genericError (sprintf "FSC passed unexpectedly for  %A" sources)

        Printf.ksprintf (fun flags sources errPath -> Commands.fsc cfg.Directory (``exec 2>`` errPath) cfg.FSC flags sources |> checkErrorLevel1)
        
    let fsdiff a b = attempt {
        let out = new ResizeArray<string>()
        let redirectOutputToFile path args =
            log "%s %s" path args
            use toLog = redirectToLog ()
            Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } cfg.Directory cfg.EnvironmentVariables path args
        do! (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> checkResult
        return out.ToArray() |> List.ofArray
        }

    do! ``fail fsc 2> a`` """%s --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%s.dll""" cfg.fsc_flags testname sources (sprintf "%s.err" testname)

    let! testnameDiff = fsdiff (sprintf "%s.err" testname) (sprintf "%s.bsl" testname)

    do! match testnameDiff with
        | [] -> Success
        | l ->
            log "***** %s.err %s.bsl differed: a bug or baseline may neeed updating" testname testname
            NUnitConf.genericError (sprintf "%s.err %s.bsl differ; %A" testname testname l)

    log "Good, output %s.err matched %s.bsl" testname testname

    do! ``fail fsc 2> a`` "%s --test:ContinueAfterParseFailure --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%s.dll" cfg.fsc_flags testname sources (sprintf "%s.vserr" testname)

    let! testnameDiff = fsdiff (sprintf "%s.vserr" testname) VSBSLFILE

    do! match testnameDiff with
        | [] -> Success
        | l ->
            log "***** %s.vserr %s differed: a bug or baseline may neeed updating" testname VSBSLFILE
            NUnitConf.genericError (sprintf "%s.vserr %s differ; %A" testname VSBSLFILE l)

    log "Good, output %s.vserr matched %s" testname VSBSLFILE
    }

let singleNegTest =

    let doneOK workDir x =
        log "Ran %s ok" workDir
        Success x

    let doneSkipped workDir msg x =
        log "Skipped neg run '%s' reason: %s" workDir msg
        Success x

    let doneError err msg =
        log "%s" msg
        Failure err

    let flow cfg testname () =    
        singleNegTestAux cfg testname
        |> Attempt.Run
        |> function
           | Success () -> doneOK cfg.Directory ()
           | Failure (Skipped msg) -> doneSkipped cfg.Directory msg ()
           | Failure (GenericError msg) -> doneError (GenericError msg) msg
           | Failure (ProcessExecError (_,_,msg) as err) -> doneError err msg
    flow

