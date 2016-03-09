module SingleTestRun

open System
open System.IO
open NUnit.Framework

open PlatformHelpers
open NUnitConf
open FSharpTestSuiteTypes

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

    let exec p = Command.exec testDir cfg.EnvironmentVariables { Output = Inherit; Input = None } p >> checkResult

    let fsi = Printf.ksprintf (fun flags l -> Commands.fsi exec cfg.FSI flags l)
    let ``exec <`` l p = Command.exec testDir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
    let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])

    let fsi_flags = cfg.fsi_flags

    let createTestOkFile () = NUnitConf.FileGuard.create (getfullpath "test.ok")

    let skipIfExists file = attempt {
        if fileExists file
        then return! NUnitConf.skip (sprintf "file '%s' found" file)
        }

    let skipIfNotExists file = attempt {
        if not (fileExists file)
        then return! NUnitConf.skip (sprintf "file '%s' not found" file)
        }


    // :FSI_STDIN
    // @echo do :FSI_STDIN
    let runFSI_STDIN () = attempt {
        // if NOT EXIST dont.pipe.to.stdin (
        do! skipIfExists "dont.pipe.to.stdin"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% "%FSI%" %fsi_flags% < %sources% && (
        do! ``fsi <`` "%s" fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only
        // dir test.ok > NUL 2>&1 ) || (
        // @echo FSI_STDIN failed;
        // set ERRORMSG=%ERRORMSG% FSI_STDIN failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        // )
        }

    // :FSI_STDIN_OPT
    // @echo do :FSI_STDIN_OPT
    let runFSI_STDIN_OPT () = attempt {
        // if NOT EXIST dont.pipe.to.stdin (
        do! skipIfExists "dont.pipe.to.stdin"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% "%FSI%" %fsi_flags% --optimize < %sources% && (
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
        // %CLIX% "%FSI%" %fsi_flags% --gui < %sources% && (
        do! ``fsi <`` "%s --gui" fsi_flags (sources |> List.rev |> List.head) //use last file, because `cmd < a.txt b.txt` redirect b.txt only
        // dir test.ok > NUL 2>&1 ) || (
        // @echo FSI_STDIN_GUI failed;
        // set ERRORMSG=%ERRORMSG% FSI_STDIN_GUI failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        // )
        }

    // :FSI_FILE
    // @echo do :FSI_FILE
    let runFSI_FILE () = attempt {
        // if NOT EXIST dont.run.as.script (
        do! skipIfExists "dont.run.as.script"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% "%FSI%" %fsi_flags% %sources% && (
        do! fsi "%s" fsi_flags sources
        // dir test.ok > NUL 2>&1 ) || (
        // @echo FSI_FILE failed
        // set ERRORMSG=%ERRORMSG% FSI_FILE failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        // )
        }

    // :FSC_BASIC
    // @echo do :FSC_BASIC
    let runFSC_BASIC () = attempt {
        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\test.exe && (
        do! exec ("."/"test.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :FSC_BASIC failed
        // set ERRORMSG=%ERRORMSG% FSC_BASIC failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :FSC_BASIC_64
    // @echo do :FSC_BASIC_64
    let runFSC_BASIC_64 () = attempt {
        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\testX64.exe && (
        do! exec ("."/"testX64.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :FSC_BASIC_64 failed
        // set ERRORMSG=%ERRORMSG% FSC_BASIC_64 failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :FSC_O3
    // @echo do :FSC_O3
    let runFSC_O3 () = attempt {
        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\test--optimize.exe && (
        do! exec ("."/"test--optimize.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :FSC_O3 failed
        // set ERRORMSG=%ERRORMSG% FSC_03 failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :FSC_OPT_MINUS_DEBUG
    // @echo do :FSC_OPT_MINUS_DEBUG
    let runFSC_OPT_MINUS_DEBUG () = attempt {
        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\test--optminus--debug.exe && (
        do! exec ("."/"test--optminus--debug.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :FSC_OPT_MINUS_DEBUG failed
        // set ERRORMSG=%ERRORMSG% FSC_OPT_MINUS_DEBUG failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :FSC_OPT_PLUS_DEBUG
    // @echo do :FSC_OPT_PLUS_DEBUG
    let runFSC_OPT_PLUS_DEBUG () = attempt {
        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\test--optplus--debug.exe && (
        do! exec ("."/"test--optplus--debug.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :FSC_OPT_PLUS_DEBUG failed
        // set ERRORMSG=%ERRORMSG% FSC_OPT_PLUS_DEBUG failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :GENERATED_SIGNATURE
    // @echo do :GENERATED_SIGNATURE
    let runGENERATED_SIGNATURE () = attempt {
        // if NOT EXIST dont.use.generated.signature (
        do! skipIfExists "dont.use.generated.signature"

        // if exist test.fs (
        do! skipIfNotExists "test.fs"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% tmptest1.exe && (
        do! exec ("."/"tmptest1.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :GENERATED_SIGNATURE failed
        // set ERRORMSG=%ERRORMSG% FSC_GENERATED_SIGNATURE failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        // )
        //)
        }

    // :SPANISH
    // @echo do :SPANISH
    let runSPANISH () = attempt {
        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\test.exe es-ES && (
        do! exec ("."/"test.exe") "es-ES"
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :SPANISH failed
        // set ERRORMSG=%ERRORMSG% SPANISH failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    // :AS_DLL
    // @echo do :AS_DLL
    let runAS_DLL () = attempt {
        //if NOT EXIST dont.compile.test.as.dll (
        do! skipIfExists "dont.compile.test.as.dll"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = createTestOkFile () 
        // %CLIX% .\test--optimize-client-of-lib.exe && (
        do! exec ("."/"test--optimize-client-of-lib.exe") ""
        // dir test.ok > NUL 2>&1 ) || (
        // @echo :AS_DLL failed
        // set ERRORMSG=%ERRORMSG% AS_DLL failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists
        //)
        }

    let run = function
        | FSI_FILE -> runFSI_FILE
        | FSI_STDIN -> runFSI_STDIN
        | FSI_STDIN_OPT -> runFSI_STDIN_OPT
        | FSI_STDIN_GUI -> runFSI_STDIN_GUI
        | SPANISH -> runSPANISH
        | FSC_BASIC -> runFSC_BASIC
        | FSC_BASIC_64 -> runFSC_BASIC_64
        | GENERATED_SIGNATURE -> runGENERATED_SIGNATURE
        | FSC_OPT_MINUS_DEBUG -> runFSC_OPT_MINUS_DEBUG
        | FSC_OPT_PLUS_DEBUG -> runFSC_OPT_PLUS_DEBUG
        | AS_DLL -> runAS_DLL

    run

let singleTestRun config testDir =
    let fileExists = Commands.fileExists testDir >> Option.isSome

    //@if "%_echo%"=="" echo off
    //setlocal
    ignore "unused"

    //set ERRORMSG=
    ignore "unused"

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

    let tests config p = attempt {
        //dir build.ok > NUL ) || (
        //  @echo 'build.ok' not found.
        //  set ERRORMSG=%ERRORMSG% Skipped because 'build.ok' not found.
        //  goto :ERROR
        //)
        do! skipIfNotExists "build.ok"

        // call %~d0%~p0..\config.bat
        let cfg = config
        // if errorlevel 1 (
        //   set ERRORMSG=%ERRORMSG% config.bat failed;
        //   goto :ERROR
        // )

        // if not exist "%FSC%" (
        //   set ERRORMSG=%ERRORMSG% fsc.exe not found at the location "%FSC%"
        //   goto :ERROR
        // )
        ignore "already checked at test suite startup"

        // if not exist "%FSI%" (
        //   set ERRORMSG=%ERRORMSG% fsi.exe not found at the location "%FSI%"
        //   goto :ERROR
        // )
        ignore "already checked at test suite startup"

        do! singleTestRun' cfg testDir p ()
        }

    let flow p () =    
        tests config p
        |> Attempt.Run
        |> function
            | Success () -> doneOK ()
            | Failure (Skipped msg) -> doneSkipped msg
            | Failure (GenericError msg) -> doneError (GenericError msg) msg
            | Failure (ProcessExecError (_,_,msg) as err) -> doneError err msg


    flow
