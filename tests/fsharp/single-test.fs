module SingleTest

open System
open System.IO
open System.Diagnostics
open NUnit.Framework
open TestFramework


type Permutation = 
    | FSC_CORECLR
    | FSI_CORECLR_SCRIPT
#if INCLUDE_NETFX_TESTS
    | FSI_NETFX_SCRIPT
    | FSI_NETFX_SCRIPT_STDIN
    | FSC_NETFX_DEBUG
    | FSC_NETFX
    | FSC_NETFX_AS_DLL
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
        printfn "------ %s: --------" filename
        printfn "%s" body
        use sw = File.CreateText(filename)
        sw.WriteLine(body)
    with | _ -> ()

let copyFilesToDest sourceDir destDir =
    let filenames = Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly)
    for file in filenames do
        let dest = Path.Combine(destDir, Path.GetFileName(file))
        File.Copy(file, dest)

type CompileItem = Reference | Compile | UseSource | LoadSource

type OutputType = Exe | Script

type ProjectConfiguration = {
    OutputType: OutputType
    Framework: string
    SourceDirectory: string
    SourceItems: string list
    UtilitySourceItems: string list
    ReferenceItems: string list
    LoadSources: string list
    UseSources: string list
    Optimize: bool
}


let generateProps testCompilerVersion configuration =
    let template = @"<Project>
  <PropertyGroup>
    <Configuration Condition=""'$(Configuration)' == ''"">$(TESTCONFIGURATION)</Configuration>
    <FSharpTestCompilerVersion>$(TESTCOMPILERVERSION)</FSharpTestCompilerVersion>
  </PropertyGroup>
  <Import Project=""$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(PROJECTDIRECTORY)'))"" />
</Project>"
    template
      .Replace("$(TESTCONFIGURATION)", configuration)
      .Replace("$(PROJECTDIRECTORY)", Path.GetFullPath(__SOURCE_DIRECTORY__))
      .Replace("$(TESTCOMPILERVERSION)", testCompilerVersion)

let targetsBody =
    let template = @"<Project>
  <Import Project=""$([MSBuild]::GetPathOfFileAbove('Directory.Build.targets', '$(PROJECTDIRECTORY)'))"" />
  <Import Project=""$(MSBuildThisFileDirectory)Directory.Overrides.targets"" Condition=""'$(OutputType)'=='Script'"" />
</Project>"
    template
     .Replace("$(PROJECTDIRECTORY)", Path.GetFullPath(__SOURCE_DIRECTORY__))

let overridesBody =
    let template = @"<Project>
  <Target Name=""Build"" DependsOnTargets=""RunFSharpScript"" />
  <Target Name=""Rebuild"" DependsOnTargets=""RunFSharpScript"" />
</Project>"
    template

let computeSourceItems (pc: ProjectConfiguration) addDirectory addCondition (compileItem: CompileItem) sources =
    [ for src in sources do
        let fileName = if addDirectory then Path.Combine(pc.SourceDirectory, src) else src
        let condition = if addCondition then " Condition=\"Exists('" + fileName + "')\"" else ""
        match compileItem with
        | CompileItem.Compile ->
            yield "\n    <Compile Include=\"" + fileName + "\"" + condition + " />"
        | CompileItem.Reference ->
            yield "\n    <Reference Include=\"" + fileName + "\"" + condition + " />"
        | CompileItem.UseSource ->
            yield "\n    <UseSource Include=\"" + fileName + "\"" + condition + " />"
        | CompileItem.LoadSource ->
            yield "\n    <LoadSource Include=\"" + fileName + "\"" + condition + " />" ]
    |> String.concat ""

let generateProjectArtifacts (pc:ProjectConfiguration) targetFramework configuration =

    let outputType =
        match pc.OutputType with
        | OutputType.Script -> "Script"
        | OutputType.Exe -> "Exe"
    let optimize = if pc.Optimize then "True" else "False"
    let debug = if pc.Optimize then "True" else "False"
    let template = @"<Project Sdk='Microsoft.NET.Sdk'>
        
  <PropertyGroup>
    <OutputType>$(OUTPUTTYPE)</OutputType>
    <TargetFramework>$(TARGETFRAMEWORK)</TargetFramework>
    <IsPackable>false</IsPackable>
    <DebugSymbols>$(DEBUG)</DebugSymbols>
    <DebugType>portable</DebugType>
    <Optimize>$(OPTIMIZE)</Optimize>
    <SignAssembly>false</SignAssembly>
    <DefineConstants>FX_RESHAPED_REFLECTION</DefineConstants>
    <DefineConstants Condition=""'$(OutputType)' == 'Script' and '$(FSharpTestCompilerVersion)' == 'coreclr'"">NETCOREAPP</DefineConstants>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <RestoreAdditionalProjectSources Condition = "" '$(RestoreAdditionalProjectSources)' == ''"">$(RestoreFromArtifactsPath)</RestoreAdditionalProjectSources>
    <RestoreAdditionalProjectSources Condition = "" '$(RestoreAdditionalProjectSources)' != ''"">$(RestoreAdditionalProjectSources);$(RestoreFromArtifactsPath)</RestoreAdditionalProjectSources>
  </PropertyGroup>

  <!-- Utility sources -->
  <ItemGroup>$(UTILITYSOURCEITEMS)
  </ItemGroup>

  <!-- Sources -->
  <ItemGroup>$(SOURCEITEMS)
  </ItemGroup>

  <!-- References -->
  <ItemGroup>$(REFERENCEITEMS)
    <Reference Condition=""$(TargetFramework.StartsWith('net4'))"" Include=""System.Windows.Forms"" />
    <Reference Condition=""$(TargetFramework.StartsWith('net4'))"" Include=""System.Web"" />
  </ItemGroup>

  <Target Name='CopyCustomContentOnPublish' AfterTargets='Build'>
    <ItemGroup>
        <Libraries Include='*.dll' />
    </ItemGroup>
    <Copy SourceFiles='@(Libraries)' DestinationFolder='$(OutputPath)' SkipUnchangedFiles='false' />
  </Target>

</Project>"
    let projBody = 
        template
         .Replace("$(UTILITYSOURCEITEMS)", computeSourceItems pc false false CompileItem.Compile pc.UtilitySourceItems)
         .Replace("$(SOURCEITEMS)", computeSourceItems pc true true CompileItem.Compile pc.SourceItems)
         .Replace("$(REFERENCEITEMS)", computeSourceItems pc true true CompileItem.Reference pc.ReferenceItems)
         .Replace("$(LOADSOURCEITEMS)", computeSourceItems pc true true CompileItem.LoadSource pc.LoadSources)
         .Replace("$(USESOURCEITEMS)", computeSourceItems pc true true CompileItem.UseSource pc.UseSources)
         .Replace("$(DIRECTORYBUILDLOCATION)", Path.GetFullPath(__SOURCE_DIRECTORY__))
         .Replace("$(OUTPUTTYPE)", outputType)
         .Replace("$(OPTIMIZE)", optimize)
         .Replace("$(DEBUG)", debug)
         .Replace("$(TARGETFRAMEWORK)", targetFramework)
         .Replace("$(RestoreFromArtifactsPath)", Path.GetFullPath(__SOURCE_DIRECTORY__) + "/../../artifacts/packages/" + configuration)

    projBody

let potentialSources = ["testlib.fsi";"testlib.fs";"test.mli";"test.ml";"test.fsi";"test.fs";"test2.fsi";"test2.fs";"test.fsx";"test2.fsx"]

let executeSingleTestBuildAndRun cfg outputType testCompilerVersion targetFramework optimize extraRef =
    let loadSources = []
    let useSources = []
    let extraSources = potentialSources |> List.filter (fileExists cfg)
    let utilitySources = [__SOURCE_DIRECTORY__  ++ "coreclr_utilities.fs"]
    let referenceItems =  if String.IsNullOrEmpty(extraRef) then [] else [extraRef]
    let mutable result = false
    let directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() )
    let pc = {
        OutputType = outputType
        Framework = targetFramework
        SourceDirectory = cfg.Directory
        SourceItems = extraSources
        UtilitySourceItems = utilitySources
        ReferenceItems = referenceItems
        LoadSources = loadSources
        UseSources = useSources
        Optimize = optimize
    }

    let targetsFileName = Path.Combine(directory, "Directory.Build.targets")
    let propsFileName = Path.Combine(directory, "Directory.Build.props")
    let overridesFileName = Path.Combine(directory, "Directory.Overrides.targets")
    let projectFileName = Path.Combine(directory, Path.GetRandomFileName() + ".fsproj")
    try
        // Clean up directory
        Directory.CreateDirectory(directory) |> ignore
        copyFilesToDest cfg.Directory directory
        try File.Delete(Path.Combine(directory, "FSharp.Core.dll")) with _ -> ()
        emitFile targetsFileName targetsBody
        emitFile overridesFileName overridesBody
        let propsBody = generateProps testCompilerVersion cfg.BUILD_CONFIG
        emitFile propsFileName propsBody
        let projectBody = generateProjectArtifacts pc targetFramework cfg.BUILD_CONFIG
        emitFile projectFileName projectBody
        use testOkFile = new FileGuard(Path.Combine(directory, "test.ok"))
        if outputType = OutputType.Exe then
            exec { cfg with Directory = directory }  cfg.DotNetExe (sprintf "run -f %s -v:n" targetFramework)
        else
            exec { cfg with Directory = directory }  cfg.DotNetExe "build /t:RunFSharpScript"
        testOkFile.CheckExists()
        result <- true
    finally
        if result <> false then
            Directory.Delete(directory, true)
        else
            printfn "Configuration: %s" cfg.Directory
            printfn "Directory: %s" directory
            printfn "Filename: %s" projectFileName

let singleTestBuildAndRunCore cfg extraRef p =

    match p with
    | FSC_CORECLR -> executeSingleTestBuildAndRun cfg OutputType.Exe "coreclr" "netcoreapp2.0" true extraRef
    | FSI_CORECLR_SCRIPT -> executeSingleTestBuildAndRun cfg OutputType.Script "coreclr" "netcoreapp2.0" true extraRef
#if INCLUDE_NETFX_TESTS
    | FSC_NETFX -> executeSingleTestBuildAndRun cfg OutputType.Exe "netfx" "net472" true extraRef
    | FSC_NETFX_DEBUG -> executeSingleTestBuildAndRun cfg OutputType.Exe "netfx" "net472" false extraRef
    | FSI_NETFX_SCRIPT -> executeSingleTestBuildAndRun cfg OutputType.Script "netfx" "net472" true extraRef

    | FSI_NETFX_SCRIPT_STDIN -> 
        use cleanup = cleanUpFSharpCore cfg
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")
        let sources = potentialSources |> List.filter (fileExists cfg)

        fsiStdin cfg (sources |> List.rev |> List.head) "" [] //use last file, because `cmd < a.txt b.txt` redirect b.txt only

        testOkFile.CheckExists()

    | FSC_NETFX_AS_DLL -> 
        // Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
        // THe second compilation will not utilize the information from the first in any meaningful way, but the
        // compiler will unpickle the interface and optimization data, so we test unpickling as well.
        use cleanup = cleanUpFSharpCore cfg
        use testOkFile = new FileGuard (getfullpath cfg "test.ok")
        
        let sources = potentialSources |> List.filter (fileExists cfg)

        fsc cfg "%s --optimize -a -o:test--optimize-lib.dll -g" cfg.fsc_flags sources
        fsc cfg "%s --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g" cfg.fsc_flags sources

        peverify cfg "test--optimize-lib.dll"
        peverify cfg "test--optimize-client-of-lib.exe"

        exec cfg ("." ++ "test--optimize-client-of-lib.exe") ""

        testOkFile.CheckExists()
#endif

let singleTestBuildAndRunAux cfg p = 
    singleTestBuildAndRunCore cfg "" p 

let singleTestBuildAndRunWithExtraRef  cfg extraRef p = 
    singleTestBuildAndRunCore cfg extraRef p

let singleTestBuildAndRun dir p = 
    let cfg = testConfig dir
    singleTestBuildAndRunAux cfg p

#if INCLUDE_NETFX_TESTS
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
#endif
