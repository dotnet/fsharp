module SingleTest

open System
open System.IO
open System.Diagnostics
open NUnit.Framework

open PlatformHelpers
open NUnitConf
open FSharpTestSuiteTypes


let singleTestBuild (cfg:TestConfig) = 

    let testDir = cfg.Directory
    let fileExists = Commands.fileExists testDir >> Option.isSome
    let del = Commands.rm testDir

    //if EXIST build.ok DEL /f /q build.ok
    do if fileExists "build.ok" then del "build.ok"

    //remove FSharp.Core.dll from the target directory to ensure that compiler uses the correct FSharp.Core.dll
    do if fileExists "FSharp.Core.dll" then del "FSharp.Core.dll"

    //set source1=
    //if exist test.ml (set source1=test.ml)
    //if exist test.fs (set source1=test.fs)
    let source1 = 
        ["test.ml"; "test.fs"] 
        |> List.rev
        |> List.tryFind fileExists

    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter fileExists

    let exec p = Command.exec testDir cfg.EnvironmentVariables { Output = Inherit; Input = None } p >> checkResult

    let copy_y f = Commands.copy_y testDir f >> checkResult
    let fsi = Printf.ksprintf (fun flags l -> Commands.fsi exec cfg.FSI flags l)
    let fsi_flags = cfg.fsi_flags
    let fsc = Printf.ksprintf (fun flags -> Commands.fsc exec cfg.FSC flags)
    let fsc_flags = cfg.fsc_flags
    let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
    let ``echo._tofile`` = Commands.``echo._tofile`` testDir

    //:Ok
    let doneOk x =
        //echo Built fsharp %~f0 ok.
        log "Built fsharp %s ok." testDir
        //echo. > build.ok
        ``echo._tofile`` " " "build.ok"
        //endlocal
        //exit /b 0
        Success x

    //:Skip
    let doneSkipped msg x =
        //echo Skipped %~f0
        log "Skipped build '%s' reason: %s" testDir msg
        //endlocal
        ``echo._tofile`` " " "build.ok"
        //exit /b 0
        Success x

    //:Error
    let doneError err msg =
        //echo Test Script Failed (perhaps test did not emit test.ok signal file?)
        log "%s" msg
        //endlocal
        //exit /b %ERRORLEVEL%
        Failure (err)

    //:SETERROR
    //set NonexistentErrorLevel 2> nul
    //goto Error

    let skipIfExists file = attempt {
        if fileExists file
        then return! NUnitConf.skip (sprintf "file '%s' found" file)
        }

    let skipIfNotExists file = attempt {
        if not (fileExists file)
        then return! NUnitConf.skip (sprintf "file '%s' not found" file)
        }
    
    /// <summary>
    /// if NOT EXIST dont.run.peverify (    <para/>
    ///    "%PEVERIFY%" test.exe            <para/>
    ///    @if ERRORLEVEL 1 goto Error      <para/>
    /// )                                   <para/>
    /// </summary>
    let doPeverify cmd = attempt {
        do! skipIfExists "dont.run.peverify"
        
        do! peverify cmd
        }

    let doNOOP () = attempt {
        //@echo No build action to take for this permutation
        log "No build action to take for this permutation"
        }

    let doBasic () = attempt { 
        // FSC %fsc_flags% --define:BASIC_TEST -o:test.exe -g %sources%
        //if ERRORLEVEL 1 goto Error
        do! fsc "%s --define:BASIC_TEST -o:test.exe -g" fsc_flags sources 

        //if NOT EXIST dont.run.peverify (
        //    "%PEVERIFY%" test.exe
        //    @if ERRORLEVEL 1 goto Error
        //)
        do! doPeverify "test.exe"
        }

    let doBasic64 () = attempt {
        // "%FSC%" %fsc_flags% --define:BASIC_TEST --platform:x64 -o:testX64.exe -g %sources%
        do! fsc "%s --define:BASIC_TEST --platform:x64 -o:testX64.exe -g" fsc_flags sources

        // if NOT EXIST dont.run.peverify (
        //     "%PEVERIFY%" testX64.exe
        // )
        do! doPeverify "testX64.exe"
        }

    let doBasicCoreCLR () = attempt {
        let platform = "win7-x64"
        //let For %%A in ("%cd%") do (Set TestCaseName=%%~nxA)
        do! fsi """%s --targetPlatformName:.NETStandard,Version=v1.6/%s --source:"coreclr_utilities.fs" --source:"%s" --packagesDir:..\..\packages --projectJsonLock:%s --fsharpCore:%s --define:CoreClr --define:NetCore --compilerPath:%s --copyCompiler:yes --verbose:verbose --exec """
               fsi_flags
               platform
               (String.concat " " sources)
               (__SOURCE_DIRECTORY__ ++ "project.lock.json")
               (__SOURCE_DIRECTORY__ ++ sprintf @"..\testbin\%s\coreclr\fsc\%s\FSharp.Core.dll" cfg.BUILD_CONFIG platform)
               (__SOURCE_DIRECTORY__ ++ sprintf @"..\testbin\%s\coreclr\fsc\%s" cfg.BUILD_CONFIG  platform)
               [__SOURCE_DIRECTORY__ ++ "..\fsharpqa\testenv\src\deployProj\CompileProj.fsx"]
        }


    let doGeneratedSignature () = attempt {
        //if NOT EXIST dont.use.generated.signature (
        do! skipIfExists "dont.use.generated.signature"

        // if exist test.ml (
        do! skipIfNotExists "test.fs"

        //  echo Generating interface file...
        log "Generating interface file..."
        //  copy /y %source1% tmptest.ml
        do! source1 |> Option.map (fun from -> copy_y from "tmptest.fs")
        //  REM NOTE: use --generate-interface-file since results may be in Unicode
        //  "%FSC%" %fsc_flags% --sig:tmptest.mli tmptest.ml
        do! fsc "%s --sig:tmptest.fsi" fsc_flags ["tmptest.fs"]

        //  echo Compiling against generated interface file...
        log "Compiling against generated interface file..."
        //  "%FSC%" %fsc_flags% -o:tmptest1.exe tmptest.fsi tmptest.fs
        do! fsc "%s -o:tmptest1.exe" fsc_flags ["tmptest.fsi";"tmptest.fs"]

        do! doPeverify "tmptest1.exe"
        }

    let doOptFscMinusDebug () = attempt {
        // "%FSC%" %fsc_flags% --optimize- --debug -o:test--optminus--debug.exe -g %sources%
        do! fsc "%s --optimize- --debug -o:test--optminus--debug.exe -g" fsc_flags sources

        do! doPeverify "test--optminus--debug.exe"
        }

    let doOptFscPlusDebug () = attempt {
        // "%FSC%" %fsc_flags% --optimize+ --debug -o:test--optplus--debug.exe -g %sources%
        do! fsc "%s --optimize+ --debug -o:test--optplus--debug.exe -g" fsc_flags sources

        // if NOT EXIST dont.run.peverify (
        //     "%PEVERIFY%" test--optplus--debug.exe
        // )
        do! doPeverify "test--optplus--debug.exe"
        }

    let doAsDLL () = attempt {
        //REM Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
        //REM THe second compilation will not utilize the information from the first in any meaningful way, but the
        //REM compiler will unpickle the interface and optimization data, so we test unpickling as well.

        //if NOT EXIST dont.compile.test.as.dll (
        do! skipIfExists "dont.compile.test.as.dll"

        // "%FSC%" %fsc_flags% --optimize -a -o:test--optimize-lib.dll -g %sources%
        do! fsc "%s --optimize -a -o:test--optimize-lib.dll -g" fsc_flags sources

        // "%FSC%" %fsc_flags% --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g %sources%
        do! fsc "%s --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g" fsc_flags sources

        // if NOT EXIST dont.run.peverify (
        //     "%PEVERIFY%" test--optimize-lib.dll
        // )
        do! doPeverify "test--optimize-lib.dll"

        // if NOT EXIST dont.run.peverify (
        //     "%PEVERIFY%" test--optimize-client-of-lib.exe
        // )
        do! doPeverify "test--optimize-client-of-lib.exe"
        }

    let build = function
        | FSI_FILE -> doNOOP
        | FSI_STDIN -> doNOOP
        | FSI_STDIN_OPT -> doNOOP
        | FSI_STDIN_GUI -> doNOOP
        | SPANISH -> doBasic
        | FSC_CORECLR -> doBasicCoreCLR
        | FSC_BASIC -> doBasic
        | FSC_BASIC_64 -> doBasic64
        | GENERATED_SIGNATURE -> doGeneratedSignature
        | FSC_OPT_MINUS_DEBUG -> doOptFscMinusDebug
        | FSC_OPT_PLUS_DEBUG -> doOptFscPlusDebug
        | AS_DLL -> doAsDLL

    let flow p () =
        build p () 
        |> Attempt.Run 
        |> function 
            | Success () -> doneOk () 
            | Failure (Skipped msg) -> doneSkipped msg ()
            | Failure (GenericError msg as err) -> doneError err msg
            | Failure (ProcessExecError (_,_,msg) as err) -> doneError err msg
    
    flow

let private singleTestRun' cfg testDir =

    let getfullpath = Commands.getfullpath testDir
    let fileExists = Commands.fileExists testDir >> Option.isSome

    // set sources=
    // if exist testlib.fsi (set sources=%sources% testlib.fsi)
    // if exist testlib.fs (set sources=%sources% testlib.fs)
    // if exist test.mli (set sources=%sources% test.mli)
    // if exist test.ml (set sources=%sources% test.ml)
    // if exist test.fsi (set sources=%sources% test.fsi)
    // if exist test.fs (set sources=%sources% test.fs)
    // if exist test2.fsi (set sources=%sources% test2.fsi)
    // if exist test2.fs (set sources=%sources% test2.fs)
    // if exist test.fsx (set sources=%sources% test.fsx)
    // if exist test2.fsx (set sources=%sources% test2.fsx)
    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter fileExists

    // :START

    // set PERMUTATIONS_LIST=FSI_FILE FSI_STDIN FSI_STDIN_OPT FSI_STDIN_GUI FSC_BASIC %FSC_BASIC_64% FSC_HW FSC_O3 GENERATED_SIGNATURE FSC_OPT_MINUS_DEBUG FSC_OPT_PLUS_DEBUG SPANISH AS_DLL 
    // 
    // if "%REDUCED_RUNTIME%"=="1" (
    //     echo REDUCED_RUNTIME set
    //     
    //     if not defined PERMUTATIONS (
    //         powershell.exe %PSH_FLAGS% -command "&{& '%~d0%~p0\PickPermutations.ps1' '%cd%' '%FSC%' '%PERMUTATIONS_LIST%'}" > _perm.txt
    //         if errorlevel 1 (
    //             set ERRORMSG=%ERRORMSG% PickPermutations.ps1 failed;
    //             goto :ERROR
    //         )
    //         set /p PERMUTATIONS=<_perm.txt
    //     )
    // )
    ignore "test is parametrized"

    // if not defined PERMUTATIONS (
    //     echo "PERMUTATIONS not defined. Running everything."
    //     set PERMUTATIONS=%PERMUTATIONS_LIST%
    // )
    ignore "test is parametrized"

    // for %%A in (%PERMUTATIONS%) do (
    //     call :%%A
    //     IF ERRORLEVEL 1 EXIT /B 1
    // )
    ignore "test is parametrized"

    // if "%ERRORMSG%"==""  goto Ok

    // set NonexistentErrorLevel 2> nul
    // goto :ERROR

    // :END

    // :EXIT_PATHS

    // REM =========================================
    // REM THE TESTS
    // REM =========================================

    let ``exec <`` l p = Command.exec testDir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
    let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])

    let fsi_flags = cfg.fsi_flags

    let createTestOkFile () = NUnitConf.FileGuard.create (getfullpath "test.ok")

    let skipIfExists file = attempt {
        if fileExists file then 
            return! NUnitConf.skip (sprintf "file '%s' found" file)
        }

    let skipIfNotExists file = attempt {
        if not (fileExists file) then 
            return! NUnitConf.skip (sprintf "file '%s' not found" file)
        }


    // :FSI_STDIN
    // @echo do :FSI_STDIN
    let runFSI_STDIN () = attempt {
        // if NOT EXIST dont.pipe.to.stdin (
        do! skipIfExists "dont.pipe.to.stdin"

        use testOkFile = createTestOkFile () 

        do! ``fsi <`` "%s" fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :FSI_STDIN_OPT
    // @echo do :FSI_STDIN_OPT
    let runFSI_STDIN_OPT () = attempt {
        // if NOT EXIST dont.pipe.to.stdin (
        do! skipIfExists "dont.pipe.to.stdin"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // "%FSI%" %fsi_flags% --optimize < %sources% && (
        do! ``fsi <`` "%s --optimize" fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only
        // dir test.ok > NUL 2>&1 ) || (
        // @echo FSI_STDIN_OPT failed
        // set ERRORMSG=%ERRORMSG% FSI_STDIN_OPT failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        // )
        }

    // :FSI_STDIN_GUI
    // @echo do :FSI_STDIN_GUI
    let runFSI_STDIN_GUI () = attempt {
        // if NOT EXIST dont.pipe.to.stdin (
        do! skipIfExists "dont.pipe.to.stdin"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // "%FSI%" %fsi_flags% --gui < %sources% && (
        do! ``fsi <`` "%s --gui" fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only
        // dir test.ok > NUL 2>&1 ) || (
        // @echo FSI_STDIN_GUI failed;
        // set ERRORMSG=%ERRORMSG% FSI_STDIN_GUI failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        // )
        }

    let runFSI_FILE () = attempt {
        do! skipIfExists "dont.run.as.script"

        use testOkFile = createTestOkFile () 

        do! fsi cfg "%s" fsi_flags sources

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let runFSC_BASIC () = attempt {
        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"test.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let runFSC_CORECLR () = attempt {
        use testOkFile = createTestOkFile () 
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
        }

    let runFSC_BASIC_64 () = attempt {
        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"testX64.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let runFSC_OPT_MINUS_DEBUG () = attempt {
        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"test--optminus--debug.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let runFSC_OPT_PLUS_DEBUG () = attempt {
        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"test--optplus--debug.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let runGENERATED_SIGNATURE () = attempt {
        do! skipIfExists "dont.use.generated.signature"

        do! skipIfNotExists "test.fs"

        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"tmptest1.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists

        }

    let runSPANISH () = attempt {

        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"test.exe") "es-ES"

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let runAS_DLL () = attempt {
        do! skipIfExists "dont.compile.test.as.dll"

        use testOkFile = createTestOkFile () 

        do! exec cfg ("."/"test--optimize-client-of-lib.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    let run = function
        | FSI_FILE -> runFSI_FILE
        | FSI_STDIN -> runFSI_STDIN
        | FSI_STDIN_OPT -> runFSI_STDIN_OPT
        | FSI_STDIN_GUI -> runFSI_STDIN_GUI
        | SPANISH -> runSPANISH
        | FSC_CORECLR -> runFSC_CORECLR
        | FSC_BASIC -> runFSC_BASIC
        | FSC_BASIC_64 -> runFSC_BASIC_64
        | GENERATED_SIGNATURE -> runGENERATED_SIGNATURE
        | FSC_OPT_MINUS_DEBUG -> runFSC_OPT_MINUS_DEBUG
        | FSC_OPT_PLUS_DEBUG -> runFSC_OPT_PLUS_DEBUG
        | AS_DLL -> runAS_DLL

    run

let singleTestRun (cfg:TestConfig) = 
    let testDir = cfg.Directory
    let fileExists = Commands.fileExists testDir >> Option.isSome

    //:Ok
    let doneOK x =
        //echo Ran fsharp %~f0 ok.
        log "Ran fsharp %s ok." testDir
        //exit /b 0
        Success x

    //:Skip
    let doneSkipped msg =
        //echo Skipped %~f0
        log "Skipped run '%s' reason: %s" testDir msg
        //exit /b 0
        Failure (Skipped msg)

    //:Error
    let doneError err msg =
        //echo %ERRORMSG%
        log "%s" msg
        //exit /b %ERRORLEVEL% 
        Failure (err)

    let skipIfNotExists file = attempt {
        if not (fileExists file)
        then return! NUnitConf.skip (sprintf "file '%s' not found" file)
        }

    let tests cfg p = attempt {
        do! skipIfNotExists "build.ok"

        do! singleTestRun' cfg testDir p ()
        }

    let flow p () =    
        tests cfg p
        |> Attempt.Run
        |> function
            | Success () -> doneOK ()
            | Failure (Skipped msg) -> doneSkipped msg
            | Failure (GenericError msg) -> doneError (GenericError msg) msg
            | Failure (ProcessExecError (_,_,msg) as err) -> doneError err msg


    flow


let private singleNegTestAux (cfg: TestConfig) testname = attempt {

    let fullpath = Commands.getfullpath cfg.Directory
    let fileExists = fullpath >> Commands.fileExists cfg.Directory >> Option.isSome

    // REM == Set baseline (fsc vs vs, in case the vs baseline exists)
    let VSBSLFILE = 
        // IF     EXIST %testname%.vsbsl (set BSLFILE=%testname%.vsbsl)
        // IF NOT EXIST %testname%.vsbsl (set BSLFILE=%testname%.bsl)
        if (sprintf "%s.vsbsl" testname) |> fileExists 
        then sprintf "%s.vsbsl" testname
        else sprintf "%s.bsl" testname

    let sources = [
        let src = [ testname + ".mli"; testname + ".fsi"; testname + ".ml"; testname + ".fs"; testname +  ".fsx";
                    testname + "a.mli"; testname + "a.fsi"; testname + "a.ml"; testname + "a.fs"; 
                    testname + "b.mli"; testname + "b.fsi"; testname + "b.ml"; testname + "b.fs"; ]

        yield! src |> List.filter fileExists
    
        if fileExists "helloWorldProvider.dll" then 
            yield "-r:helloWorldProvider.dll"

        if fileExists (testname + "-pre.fs") then 
            yield (sprintf "-r:%s-pre.dll" testname)

        ]

    do! if fileExists (testname + "-pre.fs")
    //     "%FSC%" %fsc_flags% -a -o:%testname%-pre.dll  "%testname%-pre.fs" 
        then fsc cfg "%s -a -o:%s-pre.dll" cfg.fsc_flags testname [testname + "-pre.fs"] 
        else Success ()

    // echo Negative typechecker testing: %testname%
    log "Negative typechecker testing: %s" testname

    let ``fail fsc 2> a`` = 
        // "%FSC%" %fsc_flags% --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% 2> %testname%.err
        let ``exec 2>`` errPath = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Error(Overwrite(errPath)); Input = None }
        let checkErrorLevel1 = function 
            | CmdResult.ErrorLevel (_,1) -> Success
            | CmdResult.Success | CmdResult.ErrorLevel _ -> NUnitConf.genericError (sprintf "FSC passed unexpectedly for  %A" sources)

        Printf.ksprintf (fun flags sources errPath -> Commands.fsc (``exec 2>`` errPath) cfg.FSC flags sources |> checkErrorLevel1)
        
    let fsdiff a b = attempt {
        let out = new ResizeArray<string>()
        let redirectOutputToFile path args =
            log "%s %s" path args
            use toLog = redirectToLog ()
            Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } cfg.Directory cfg.EnvironmentVariables path args
        do! (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> checkResult
        return out.ToArray() |> List.ofArray
        }

    // "%FSC%" %fsc_flags% --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% 2> %testname%.err
    do! ``fail fsc 2> a`` """%s --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%s.dll""" cfg.fsc_flags testname sources (sprintf "%s.err" testname)

    // %FSDIFF% %testname%.err %testname%.bsl > %testname%.diff
    let! testnameDiff = fsdiff (sprintf "%s.err" testname) (sprintf "%s.bsl" testname)

    // for /f %%c IN (%testname%.diff) do (
    do! match testnameDiff with
        | [] -> Success
        | l ->
            // echo ***** %testname%.err %testname%.bsl differed: a bug or baseline may neeed updating
            log "***** %s.err %s.bsl differed: a bug or baseline may neeed updating" testname testname
            // set ERRORMSG=%ERRORMSG% %testname%.err %testname%.bsl differ;
            NUnitConf.genericError (sprintf "%s.err %s.bsl differ; %A" testname testname l)

    // echo Good, output %testname%.err matched %testname%.bsl
    log "Good, output %s.err matched %s.bsl" testname testname

    // "%FSC%" %fsc_flags% --test:ContinueAfterParseFailure --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% 2> %testname%.vserr
    do! ``fail fsc 2> a`` "%s --test:ContinueAfterParseFailure --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%s.dll" cfg.fsc_flags testname sources (sprintf "%s.vserr" testname)

    // %FSDIFF% %testname%.vserr %BSLFILE% > %testname%.vsdiff
    let! testnameDiff = fsdiff (sprintf "%s.vserr" testname) VSBSLFILE

    // for /f %%c IN (%testname%.vsdiff) do (
    do! match testnameDiff with
        | [] -> Success
        | l ->
            // echo ***** %testname%.vserr %BSLFILE% differed: a bug or baseline may neeed updating
            log "***** %s.vserr %s differed: a bug or baseline may neeed updating" testname VSBSLFILE
            // set ERRORMSG=%ERRORMSG% %testname%.vserr %BSLFILE% differ;
            NUnitConf.genericError (sprintf "%s.vserr %s differ; %A" testname VSBSLFILE l)

    log "Good, output %s.vserr matched %s" testname VSBSLFILE
    }

let singleNegTest =

    let doneOK x =
        log "Ran fsharp %%~f0 ok"
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
           | Success () -> doneOK ()
           | Failure (Skipped msg) -> doneSkipped cfg.Directory msg ()
           | Failure (GenericError msg) -> doneError (GenericError msg) msg
           | Failure (ProcessExecError (_,_,msg) as err) -> doneError err msg
    flow

let singleTestBuildAndRun p = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()
        
    do! singleTestBuild cfg p
        
    do! singleTestRun cfg p
    })


