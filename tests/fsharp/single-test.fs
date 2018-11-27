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
    | FSIANYCPU_FILE
    | FSI_STDIN
    | GENERATED_SIGNATURE
    | FSC_OPT_MINUS_DEBUG
    | FSC_OPT_PLUS_DEBUG
    | AS_DLL
#endif

// Because we build programs ad dlls the compiler will copy an fsharp.core.dll into the build directory
// peverify will fail if fsharp.core.dll is not found or is the wrong one.
// This ensures that we delete any fsharp.core.dlls when we start the build and also when the singleTestBuild and run is finished.
let cleanUpFSharpCore cfg =
    let removeFSharpCore () =
        if fileExists cfg "FSharp.Core.dll" then rm cfg "FSharp.Core.dll"
    removeFSharpCore ()
    { new System.IDisposable with member x.Dispose() = removeFSharpCore () }

// Generate a project files
let emitFile filename (body:string) =
    try
        // Create a file to write to
        use sw = File.CreateText(filename)
        sw.WriteLine(body)
    with | _ -> ()

let copyFilesToDest sourceDir destDir compilerPath =
    // copy existing files
    let filenames = Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly)
    for file in filenames do
        let dest = Path.Combine(destDir, Path.GetFileName(file))
        File.Copy(file, dest)
    // generate Directory.Build.props that points to the built compiler
    let lines =
        [ "<Project>"
          "  <PropertyGroup>"
          sprintf "    <CompilerTestPath>%s</CompilerTestPath>" compilerPath
          "  </PropertyGroup>"
          sprintf "  <Import Project=\"%s\\Test.Directory.Build.props\" />" __SOURCE_DIRECTORY__
          "</Project>" ]
    let dest = Path.Combine(destDir, "Directory.Build.props")
    File.WriteAllLines(dest, lines)

let generateProjectArtifacts (framework:string) (sourceDirectory:string) (sourceItems:string list) (extraSourceItems:string list) (utilitySourceItems:string list) (referenceItems:string list) =
    let computeSourceItems addDirectory addCondition isCompileItem sources =
        let computeInclude src =
            let fileName = if addDirectory then Path.Combine(sourceDirectory, src) else src
            let condition = if addCondition then " Condition=\"Exists('" + fileName + "')\"" else ""
            if isCompileItem then
                "\n    <Compile Include='" + fileName + "'" + condition + " />"
            else
                "\n    <Reference Include='" + fileName + "'" + condition + " />"
        sources
        |> List.map(fun src -> computeInclude src)
        |> List.fold (fun acc s -> acc + s) ""

    let replace tag items addDirectory addCondition isCompileItem (template:string) = template.Replace(tag, computeSourceItems addDirectory addCondition isCompileItem items)

    let generateProjBody =
        let template = @"<Project Sdk='Microsoft.NET.Sdk'>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>$(TARGETFRAMEWORK)</TargetFramework>
    <IsPackable>false</IsPackable>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>portable</DebugType>
    <Optimize>true</Optimize>
    <DefineConstants>$(DefineConstants);FX_RESHAPED_REFLECTION;NETSTANDARD</DefineConstants>
  </PropertyGroup>

  <!-- Utility sources -->
  <ItemGroup>$(UTILITYSOURCEITEMS)
  </ItemGroup>

  <!-- Sources -->
  <ItemGroup>$(SOURCEITEMS)
  </ItemGroup>

  <!-- Extra sources -->
  <ItemGroup>$(EXTRASOURCEITEMS)
  </ItemGroup>

  <!-- References -->
  <ItemGroup>$(REFERENCEITEMS)
  </ItemGroup>

</Project>"
        template.Replace("$(TARGETFRAMEWORK)", framework)
        |> replace "$(UTILITYSOURCEITEMS)" utilitySourceItems false false true
        |> replace "$(SOURCEITEMS)" sourceItems true false true
        |> replace "$(EXTRASOURCEITEMS)" extraSourceItems true true true
        |> replace "$(REFERENCEITEMS)" referenceItems true true false

    generateProjBody

let singleTestBuildAndRunCore cfg copyFiles p =
    printfn "singleTestBuildAndRunCore: %A : %A" copyFiles p
    let sources =
        ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]
        |> List.filter (fileExists cfg)
    let extraSources = []
    let utilitySources =
        [__SOURCE_DIRECTORY__  ++ "coreclr_utilities.fs"]
        |> List.filter (File.Exists)
    let referenceItems =  if String.IsNullOrEmpty(copyFiles) then [] else [copyFiles]
    let framework = "netcoreapp2.1"
    match p with
    | FSC_CORECLR ->
        let mutable result = false
        let directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() )
        let projectBody = generateProjectArtifacts framework cfg.Directory sources extraSources utilitySources referenceItems
        let projectFileName = Path.Combine(directory, Path.GetRandomFileName() + ".fsproj")
        try
            use testOkFile = new FileGuard(Path.Combine(directory, "test.ok"))
            printfn "Configuration: %s" cfg.Directory
            printfn "Directory: %s" directory
            printfn "Filename: %s" projectFileName
            Directory.CreateDirectory(directory) |> ignore
            copyFilesToDest cfg.Directory directory cfg.BinPath
            emitFile projectFileName projectBody
            exec { cfg with Directory = directory }  cfg.DotNet20Exe "run"
            testOkFile.CheckExists()
            result <- true
        finally
            if result <> false then Directory.Delete(directory, true)
        ()
    | _ ->
        match p with 
        | FSC_CORECLR ->
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            let testName = getBasename cfg.Directory
            let extraSource = (__SOURCE_DIRECTORY__  ++ "coreclr_utilities.fs")
            let outDir =  (__SOURCE_DIRECTORY__ ++ sprintf @"../testbin/%s/coreclr/fsharp/core/%s" cfg.BUILD_CONFIG testName)
            let outFile = (__SOURCE_DIRECTORY__ ++ sprintf @"../testbin/%s/coreclr/fsharp/core/%s/test.exe" cfg.BUILD_CONFIG testName)

            makeDirectory (getDirectoryName outFile)
            let fscArgs = 
                sprintf """--debug:portable --debug+ --out:%s  --target:exe -g --define:FX_RESHAPED_REFLECTION --define:NETCOREAPP1_0 "%s" %s """
                   outFile
                   extraSource
                   (String.concat " " sources)

            let fsccArgs = sprintf """--OutputDir:%s --CopyDlls:%s %s""" outDir copyFiles fscArgs

            fsi_script cfg "--exec %s %s %s"
                   cfg.fsi_flags
                   (__SOURCE_DIRECTORY__ ++ @"../scripts/fscc.fsx")
                   fsccArgs
                   []

            exec cfg  cfg.DotNetExe outFile

            testOkFile.CheckExists()

        | FSI_CORECLR -> 
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            let testName = getBasename cfg.Directory
            let extraSource = (__SOURCE_DIRECTORY__  ++ "coreclr_utilities.fs")
            let outDir =  (__SOURCE_DIRECTORY__ ++ sprintf @"../testbin/%s/coreclr/fsharp/core/%s" cfg.BUILD_CONFIG testName)
            let fsiArgs = 
                sprintf """ --define:NETCOREAPP1_0 --define:FX_RESHAPED_REFLECTION "%s" %s """
                   extraSource
                   (String.concat " " sources)

            let fsciArgs = sprintf """--verbose:repro --OutputDir:%s --CopyDlls:%s %s""" outDir copyFiles fsiArgs

            fsi_script cfg "--exec %s %s %s"
                   cfg.fsi_flags
                   (__SOURCE_DIRECTORY__ ++ @"../scripts/fsci.fsx")
                   fsciArgs
                   []

            testOkFile.CheckExists()

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
        | FSI_FILE -> 
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            fsi cfg "%s" cfg.fsi_flags sources

            testOkFile.CheckExists()

        | FSIANYCPU_FILE -> 
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            fsiAnyCpu cfg "%s" cfg.fsi_flags sources

            testOkFile.CheckExists()

        | FSI_STDIN -> 
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            fsiStdin cfg (sources |> List.rev |> List.head) "" [] //use last file, because `cmd < a.txt b.txt` redirect b.txt only

            testOkFile.CheckExists()

        | GENERATED_SIGNATURE -> 
            use cleanup = (cleanUpFSharpCore cfg)

            let source1 = 
                ["test.ml"; "test.fs"; "test.fsx"] 
                |> List.rev
                |> List.tryFind (fileExists cfg)

            source1 |> Option.iter (fun from -> copy_y cfg from "tmptest.fs")

            log "Generated signature file..."
            fsc cfg "%s --sig:tmptest.fsi --define:TESTS_AS_APP" cfg.fsc_flags ["tmptest.fs"]
            (if File.Exists("FSharp.Core.dll") then log "found fsharp.core.dll after build" else log "found fsharp.core.dll after build") |> ignore

            log "Compiling against generated signature file..."
            fsc cfg "%s -o:tmptest1.exe  --define:TESTS_AS_APP" cfg.fsc_flags ["tmptest.fsi";"tmptest.fs"]
            (if File.Exists("FSharp.Core.dll") then log "found fsharp.core.dll after build" else log "found fsharp.core.dll after build") |> ignore

            log "Verifying built .exe..."
            peverify cfg "tmptest1.exe"

        | FSC_OPT_MINUS_DEBUG -> 
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            fsc cfg "%s --optimize- --debug -o:test--optminus--debug.exe -g" cfg.fsc_flags sources
            peverify cfg "test--optminus--debug.exe"
            exec cfg ("." ++ "test--optminus--debug.exe") ""

            testOkFile.CheckExists()

        | FSC_OPT_PLUS_DEBUG -> 
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            fsc cfg "%s --optimize+ --debug -o:test--optplus--debug.exe -g" cfg.fsc_flags sources
            peverify cfg "test--optplus--debug.exe"
            exec cfg ("." ++ "test--optplus--debug.exe") ""

            testOkFile.CheckExists()

        | AS_DLL -> 
            // Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
            // THe second compilation will not utilize the information from the first in any meaningful way, but the
            // compiler will unpickle the interface and optimization data, so we test unpickling as well.
            use cleanup = (cleanUpFSharpCore cfg)
            use testOkFile = new FileGuard (getfullpath cfg "test.ok")

            fsc cfg "%s --optimize -a -o:test--optimize-lib.dll -g" cfg.fsc_flags sources
            fsc cfg "%s --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g" cfg.fsc_flags sources

            peverify cfg "test--optimize-lib.dll"
            peverify cfg "test--optimize-client-of-lib.exe"

            exec cfg ("." ++ "test--optimize-client-of-lib.exe") ""

            testOkFile.CheckExists()
#endif
    
let singleTestBuildAndRunAux cfg p = 
    singleTestBuildAndRunCore cfg "" p 

let singleTestBuildAndRunWithCopyDlls  cfg copyFiles p = 
    singleTestBuildAndRunCore cfg copyFiles p

let singleTestBuildAndRun dir p = 
    let cfg = testConfig dir
    singleTestBuildAndRunAux cfg p


let singleNegTest (cfg: TestConfig) testname = 

    let cfg = { cfg with fsc_flags = sprintf "%s --define:NEGATIVE" cfg.fsc_flags }

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

    if fileExists cfg (testname + "-pre.fsx") then
        fsi_script cfg "--exec %s %s %s"
               cfg.fsi_flags
               (cfg.Directory ++ (testname + "-pre.fsx"))
               ""
               []

    log "Negative typechecker testing: %s" testname

    let warnaserror =
        if cfg.fsc_flags.Contains("--warnaserror-") then String.Empty
        else "--warnaserror"

    fscAppendErrExpectFail cfg  (sprintf "%s.err" testname) """%s --vserrors %s --nologo --maxerrors:10000 -a -o:%s.dll""" cfg.fsc_flags warnaserror testname sources

    let diff = fsdiff cfg (sprintf "%s.err" testname) (sprintf "%s.bsl" testname)

    fscAppendErrExpectFail cfg (sprintf "%s.vserr" testname) "%s --test:ContinueAfterParseFailure --vserrors %s --nologo --maxerrors:10000 -a -o:%s.dll" cfg.fsc_flags warnaserror testname sources

    let vbslDiff = fsdiff cfg (sprintf "%s.vserr" testname) VSBSLFILE

    match diff,vbslDiff with
    | "","" -> 
        log "Good, output %s.err matched %s.bsl" testname testname
        log "Good, output %s.vserr matched %s" testname VSBSLFILE
    | l,"" ->        
        log "***** %s.err %s.bsl differed: a bug or baseline may need updating" testname testname        
        failwithf "%s.err %s.bsl differ; %A" testname testname l
    | "",l ->
        log "Good, output %s.err matched %s.bsl" testname testname
        log "***** %s.vserr %s differed: a bug or baseline may need updating" testname VSBSLFILE
        failwithf "%s.vserr %s differ; %A" testname VSBSLFILE l
    | l1,l2 ->    
        log "***** %s.err %s.bsl differed: a bug or baseline may need updating" testname testname 
        log "***** %s.vserr %s differed: a bug or baseline may need updating" testname VSBSLFILE
        failwithf "%s.err %s.bsl differ; %A; %s.vserr %s differ; %A" testname testname l1 testname VSBSLFILE l2
