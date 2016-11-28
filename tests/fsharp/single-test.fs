module SingleTest

open System
open System.IO
open System.Diagnostics
open NUnit.Framework
open TestFramework


type Permutation = 
    | FSC_CORECLR
    | FSI_CORECLR
#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    | FSI_FILE
    | FSI_STDIN
    | GENERATED_SIGNATURE
    | FSC_OPT_MINUS_DEBUG
    | FSC_OPT_PLUS_DEBUG
    | AS_DLL
#endif

let singleTestBuildAndRunAux cfg p = 
    //remove FSharp.Core.dll from the target directory to ensure that compiler uses the correct FSharp.Core.dll
    do if fileExists cfg "FSharp.Core.dll" then rm cfg "FSharp.Core.dll"

    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter (fileExists cfg)


    match p with 
    | FSC_CORECLR -> 
        let testName = getBasename cfg.Directory
        let extraSource = (__SOURCE_DIRECTORY__  ++ "coreclr_utilities.fs")
        let outFile = (__SOURCE_DIRECTORY__ ++ sprintf @"../testbin/%s/coreclr/fsharp/core/%s/output/test.exe" cfg.BUILD_CONFIG testName)
        let coreRunExe = (__SOURCE_DIRECTORY__ ++ sprintf @"../testbin/%s/coreclr/%s/corerun.exe" cfg.BUILD_CONFIG defaultPlatform)
        makeDirectory (getDirectoryName outFile)
        let fscArgs = 
            sprintf """--debug:portable --debug+ --out:%s  --target:exe -g --define:FX_RESHAPED_REFLECTION --define:NETSTANDARD1_6 --define:FSCORE_PORTABLE_NEW --define:FX_PORTABLE_OR_NETSTANDARD "%s" %s """
               outFile
               extraSource
               (String.concat " " sources)

        let fsccArgs = sprintf """--verbose:repro %s""" fscArgs

        fsi cfg "--exec %s %s %s"
               cfg.fsi_flags
               (__SOURCE_DIRECTORY__ ++ @"../scripts/fscc.fsx")
               fsccArgs
               []
               
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")
        exec cfg  coreRunExe outFile

        testOkFile.CheckExists()

    | FSI_CORECLR -> 
        let extraSource = (__SOURCE_DIRECTORY__  ++ "coreclr_utilities.fs")
        let fsiArgs = 
            sprintf """ --define:NETSTANDARD1_6 --define:FSCORE_PORTABLE_NEW --define:FX_RESHAPED_REFLECTION --define:FX_PORTABLE_OR_NETSTANDARD "%s" %s """
               extraSource
               (String.concat " " sources)

        let fsciArgs = sprintf """--verbose:repro %s""" fsiArgs

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsi cfg "--exec %s %s %s"
               cfg.fsi_flags
               (__SOURCE_DIRECTORY__ ++ @"../scripts/fsci.fsx")
               fsciArgs
               []
               
        testOkFile.CheckExists()

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    | FSI_FILE -> 
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsi cfg "%s" cfg.fsi_flags sources

        testOkFile.CheckExists()

    | FSI_STDIN -> 
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        fsiStdin cfg (sources |> List.rev |> List.head) "" [] //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        testOkFile.CheckExists()

    | GENERATED_SIGNATURE -> 

        let source1 = 
            ["test.ml"; "test.fs"; "test.fsx"] 
            |> List.rev
            |> List.tryFind (fileExists cfg)

        source1 |> Option.iter (fun from -> copy_y cfg from "tmptest.fs")

        log "Generated signature file..."
        fsc cfg "%s --sig:tmptest.fsi --define:TESTS_AS_APP" cfg.fsc_flags ["tmptest.fs"]

        log "Compiling against generated signature file..."
        fsc cfg "%s -o:tmptest1.exe  --define:TESTS_AS_APP" cfg.fsc_flags ["tmptest.fsi";"tmptest.fs"]

        peverify cfg "tmptest1.exe"

    | FSC_OPT_MINUS_DEBUG -> 
        fsc cfg "%s --optimize- --debug -o:test--optminus--debug.exe -g" cfg.fsc_flags sources

        peverify cfg "test--optminus--debug.exe"
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("." ++ "test--optminus--debug.exe") ""

        testOkFile.CheckExists()

    | FSC_OPT_PLUS_DEBUG -> 
        fsc cfg "%s --optimize+ --debug -o:test--optplus--debug.exe -g" cfg.fsc_flags sources

        peverify cfg "test--optplus--debug.exe"

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("." ++ "test--optplus--debug.exe") ""

        testOkFile.CheckExists()

    | AS_DLL -> 
        // Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
        // THe second compilation will not utilize the information from the first in any meaningful way, but the
        // compiler will unpickle the interface and optimization data, so we test unpickling as well.

        fsc cfg "%s --optimize -a -o:test--optimize-lib.dll -g" cfg.fsc_flags sources

        fsc cfg "%s --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g" cfg.fsc_flags sources

        peverify cfg "test--optimize-lib.dll"

        peverify cfg "test--optimize-client-of-lib.exe"

        use testOkFile = new FileGuard (getfullpath cfg "test.ok")

        exec cfg ("." ++ "test--optimize-client-of-lib.exe") ""

        testOkFile.CheckExists()
#endif

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

