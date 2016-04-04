module SingleTestBuild

open System
open System.IO
open System.Diagnostics
open NUnit.Framework

open PlatformHelpers
open NUnitConf
open FSharpTestSuiteTypes


let singleTestBuild cfg testDir =

    let fileExists = Commands.fileExists testDir >> Option.isSome
    let del = Commands.rm testDir

    //if EXIST build.ok DEL /f /q build.ok
    let buildOkPath = testDir / "build.ok"
    do if fileExists "build.ok" then del "build.ok"

    //call %~d0%~p0..\config.bat
    ignore "param"

    //if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" (
    //  goto Skip
    //)
    ignore "already checked fsc/fsi exists"

    //set source1=
    //if exist test.ml (set source1=test.ml)
    //if exist test.fs (set source1=test.fs)
    let source1 = 
        ["test.ml"; "test.fs"] 
        |> List.rev
        |> List.tryFind fileExists

    //set sources=
    //if exist testlib.fsi (set sources=%sources% testlib.fsi)
    //if exist testlib.fs (set sources=%sources% testlib.fs)
    //if exist test.mli (set sources=%sources% test.mli)
    //if exist test.ml (set sources=%sources% test.ml)
    //if exist test.fsi (set sources=%sources% test.fsi)
    //if exist test.fs (set sources=%sources% test.fs)
    //if exist test2.fsi (set sources=%sources% test2.fsi)
    //if exist test2.fs (set sources=%sources% test2.fs)
    //if exist test.fsx (set sources=%sources% test.fsx)
    //if exist test2.fsx (set sources=%sources% test2.fsx)
    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter fileExists


    //rem to run the 64 bit version of the code set FSC_BASIC_64=FSC_BASIC_64
    //set PERMUTATIONS_LIST=FSI_FILE FSI_STDIN FSI_STDIN_OPT FSI_STDIN_GUI FSC_BASIC %FSC_BASIC_64% GENERATED_SIGNATURE FSC_OPT_MINUS_DEBUG FSC_OPT_PLUS_DEBUG SPANISH AS_DLL 

    //if "%REDUCED_RUNTIME%"=="1" (
    //    echo REDUCED_RUNTIME set
    //    
    //    if not defined PERMUTATIONS (
    //        powershell.exe %PSH_FLAGS% -command "&{& '%~d0%~p0\PickPermutations.ps1' '%cd%' '%FSC%' '%PERMUTATIONS_LIST%'}" > _perm.txt
    //        if errorlevel 1 (
    //            set ERRORMSG=%ERRORMSG% PickPermutations.ps1 failed;
    //            goto :ERROR
    //        )
    //        set /p PERMUTATIONS=<_perm.txt
    //    )
    //    
    //    powershell.exe %PSH_FLAGS% -command "&{& '%~d0%~p0\DecidePEVerify.ps1' '%cd%' '%FSC%'}"
    //    if errorlevel 1 (
    //        set ERRORMSG=%ERRORMSG% DecidePEVerify.ps1 failed;
    //        goto :ERROR
    //    )
    //)

    //if not defined PERMUTATIONS (
    //    echo "PERMUTATIONS not defined. Building everything."
    //    set PERMUTATIONS=%PERMUTATIONS_LIST%
    //)

    //for %%A in (%PERMUTATIONS%) do (
    //    call :%%A
    //    IF ERRORLEVEL 1 EXIT /B 1
    //)
    ignore "permutations useless because build type is an input"

    let exec p = Command.exec testDir cfg.EnvironmentVariables { Output = Inherit; Input = None } p >> checkResult

    let echo_tofile = Commands.echo_tofile testDir
    let copy_y f = Commands.copy_y testDir f >> checkResult
    let type_append_tofile = Commands.type_append_tofile testDir
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

    let genericErrorMessage = "Test Script Failed (perhaps test did not emit test.ok signal file?)"

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

    let doEmptySignature () = attempt {
        //if NOT EXIST dont.use.empty.signature (
        do! skipIfExists "dont.use.empty.signature"

        // if exist test.fs ( 
        do! skipIfNotExists "test.fs"

        // echo Compiling against empty interface file...
        log "Compiling against empty interface file..."
        // echo // empty file  > tmptest2.fsi
        echo_tofile "// empty file  " "tmptest2.fsi"
        // copy /y %source1% tmptest2.fs
        do! source1 |> Option.map (fun from -> copy_y from "tmptest2.fs")
        // "%FSC%" %fsc_flags% --define:COMPILING_WITH_EMPTY_SIGNATURE -o:tmptest2.exe tmptest2.fsi tmptest2.fs
        do! fsc "%s --define:COMPILING_WITH_EMPTY_SIGNATURE -o:tmptest2.exe" fsc_flags ["tmptest2.fsi";"tmptest2.fs"]

        do! doPeverify "tmptest2.exe"
        }


    let doEmptySignatureOpt () = attempt {
        //if NOT EXIST dont.use.empty.signature (
        do! skipIfExists "dont.use.empty.signature"

        // if exist test.fs ( 
        do! skipIfNotExists "test.fs"

        // echo Compiling against empty interface file...
        log "Compiling against empty interface file..."
        // echo // empty file  > tmptest2.fsi
        echo_tofile "// empty file  " "tmptest2.fsi"
        // copy /y %source1% tmptest2.fs
        do! source1 |> Option.map (fun from -> copy_y from "tmptest2.fs")
        // "%FSC%" %fsc_flags% --define:COMPILING_WITH_EMPTY_SIGNATURE --optimize -o:tmptest2--optimize.exe tmptest2.fsi tmptest2.fs
        do! fsc "%s --define:COMPILING_WITH_EMPTY_SIGNATURE --optimize -o:tmptest2--optimize.exe" fsc_flags ["tmptest2.fsi";"tmptest2.fs"]

        do! doPeverify "tmptest2--optimize.exe"
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

    let doWrapperNamespace () = attempt {
        // if NOT EXIST dont.use.wrapper.namespace (
        do! skipIfExists "dont.use.wrapper.namespace"

        do! skipIfNotExists "test.fs"
         
        // echo Compiling when wrapped in a namespace declaration...
        log "Compiling when wrapped in a namespace declaration..."
        // echo module TestNamespace.TestModule > tmptest3.fs
        echo_tofile "module TestNamespace.TestModule " "tmptest3.fs"
        // type %source1%  >> tmptest3.fs
        source1 |> Option.iter (fun from -> type_append_tofile from "tmptest3.fs")
        // "%FSC%" %fsc_flags% -o:tmptest3.exe tmptest3.fs
        do! fsc "%s -o:tmptest3.exe" fsc_flags ["tmptest3.fs"]

        do! doPeverify "tmptest3.exe"
        }

    let doWrapperNamespaceOpt () = attempt {
        //if NOT EXIST dont.use.wrapper.namespace (
        do! skipIfExists "dont.use.wrapper.namespace"

        // if exist test.fs (
        do! skipIfNotExists "test.fs"

        // echo Compiling when wrapped in a namespace declaration...
        log "Compiling when wrapped in a namespace declaration..."
        // echo module TestNamespace.TestModule > tmptest3.fs
        echo_tofile "module TestNamespace.TestModule " "tmptest3.fs"
        // type %source1%  >> tmptest3.fs
        source1 |> Option.iter (fun from -> type_append_tofile from "tmptest3.fs")
        // "%FSC%" %fsc_flags% --optimize -o:tmptest3--optimize.exe tmptest3.fs
        do! fsc "%s --optimize -o:tmptest3--optimize.exe" fsc_flags ["tmptest3.fs"]

        do! doPeverify "tmptest3--optimize.exe"
        }

    let build = function
        | FSI_FILE -> doNOOP
        | FSI_STDIN -> doNOOP
        | FSI_STDIN_OPT -> doNOOP
        | FSI_STDIN_GUI -> doNOOP
        | SPANISH -> doBasic
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
