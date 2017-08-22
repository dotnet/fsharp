// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"
open System
open Fake.AppVeyor
open Fake
open Fake.Git
open Fake.ReleaseNotesHelper
open Fake.UserInputHelper

#if MONO
// prevent incorrect output encoding (e.g. https://github.com/fsharp/FAKE/issues/1196)
System.Console.OutputEncoding <- System.Text.Encoding.UTF8
#endif

// --------------------------------------------------------------------------------------
// Utilities
// --------------------------------------------------------------------------------------

let assertExitCodeZero x = if x = 0 then () else failwithf "Command failed with exit code %i" x
let runCmdIn mono workDir exe = Printf.ksprintf (fun args ->
    if mono then
        printfn "mono %s/%s %s" workDir exe args
        Shell.Exec("mono", sprintf "%s %s" exe args, workDir)
        |> assertExitCodeZero
    else
        printfn "%s/%s %s" workDir exe args
        Shell.Exec(exe, args, workDir)
        |> assertExitCodeZero
)
let run mono exe = runCmdIn mono "." exe

// --------------------------------------------------------------------------------------
// Information about the project to be used at NuGet 
// --------------------------------------------------------------------------------------

let project = "FSharp.Compiler.Service"
let authors = ["Microsoft Corporation, Dave Thomas, Anh-Dung Phan, Tomas Petricek"]

let gitOwner = "fsharp"
let gitHome = "https://github.com/" + gitOwner

let gitName = "FSharp.Compiler.Service"
let gitRaw = environVarOrDefault "gitRaw" "https://raw.githubusercontent.com/fsharp"

let netFrameworks = [(* "v4.0"; *) "v4.5"]

// --------------------------------------------------------------------------------------
// The rest of the code is standard F# build script
// --------------------------------------------------------------------------------------

let releaseDir = "../Release"


// Read release notes & version info from RELEASE_NOTES.md
let release = LoadReleaseNotes (__SOURCE_DIRECTORY__ + "/RELEASE_NOTES.md")
let isAppVeyorBuild = buildServer = BuildServer.AppVeyor
let isVersionTag tag = Version.TryParse tag |> fst
let hasRepoVersionTag = isAppVeyorBuild && AppVeyorEnvironment.RepoTag && isVersionTag AppVeyorEnvironment.RepoTagName
let assemblyVersion = if hasRepoVersionTag then AppVeyorEnvironment.RepoTagName else release.NugetVersion
let nugetVersion = release.NugetVersion
open SemVerHelper
let nugetDebugVersion =
    let semVer = SemVerHelper.parse nugetVersion
    let debugPatch, debugPreRelease =
        match semVer.PreRelease with
        | None -> semVer.Patch + 1, { Origin = "alpha001"; Name = "alpha"; Number = Some 1; Parts = [AlphaNumeric "alpha001"] }
        | Some pre ->
            let num = match pre.Number with Some i -> i + 1 | None -> 1
            let name = pre.Name
            let newOrigin = sprintf "%s%03d" name num
            semVer.Patch, { Origin = newOrigin; Name = name; Number = Some num; Parts = [AlphaNumeric newOrigin] }
    let debugVer =
        { semVer with
            Patch = debugPatch
            PreRelease = Some debugPreRelease }
    debugVer.ToString()
let buildDate = DateTime.UtcNow
let buildVersion =
    if hasRepoVersionTag then assemblyVersion
    else if isAppVeyorBuild then sprintf "%s-b%s" assemblyVersion AppVeyorEnvironment.BuildNumber
    else assemblyVersion

let netstdsln = gitName + ".netstandard.sln";

Target "BuildVersion" (fun _ ->
    Shell.Exec("appveyor", sprintf "UpdateBuild -Version \"%s\"" buildVersion) |> ignore
)

// --------------------------------------------------------------------------------------
// Clean build results & restore NuGet packages


Target "Build.NetFx" (fun _ ->
    !! (project + ".sln")
    |> MSBuild "" "Build" ["Configuration","Release" ]
    |> Log (".NETFxBuild-Output: ")
)


// --------------------------------------------------------------------------------------
// Run the unit tests using test runner

Target "RunTests.NetFx" (fun _ ->
    !! (releaseDir + "/fcs/net45/FSharp.Compiler.Service.Tests.dll")
    |> NUnit (fun p ->
        { p with
            Framework = "v4.0.30319"
            DisableShadowCopy = true
            TimeOut = TimeSpan.FromMinutes 20.
            OutputFile = "TestResults.xml" })
)

// --------------------------------------------------------------------------------------
// Build a NuGet package
Target "NuGet.NetFx" (fun _ ->
    run false @"..\.nuget\nuget.exe" @"pack nuget\FSharp.Compiler.Service.nuspec -OutputDirectory %s" releaseDir
    run false @"..\.nuget\nuget.exe" @"pack nuget\FSharp.Compiler.Service.MSBuild.v12.nuspec -OutputDirectory %s" releaseDir
    run false @"..\.nuget\nuget.exe" @"pack nuget\FSharp.Compiler.Service.ProjectCracker.nuspec -OutputDirectory %s" releaseDir
)



// --------------------------------------------------------------------------------------
// .NET Core and .NET Core SDK

let isDotnetSDKInstalled =
    match Fake.EnvironmentHelper.environVarOrNone "FCS_DNC" with
    | Some flag ->
        match bool.TryParse flag with
        | true, result -> result
        | _ -> false
    | None ->
        try
            Shell.Exec("dotnet", "--info") = 0
        with
        _ -> false

Target "CodeGen.NetStd" (fun _ ->
    let lexArgs = "--lexlib Internal.Utilities.Text.Lexing"
    let yaccArgs = "--internal --parslib Internal.Utilities.Text.Parsing"
    let module1 = "--module Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiParser"
    let module2 = "--module Microsoft.FSharp.Compiler.Parser"
    let module3 = "--module Microsoft.FSharp.Compiler.PPParser"
    let open1 = "--open Microsoft.FSharp.Compiler.AbstractIL"
    let open2 = "--open Microsoft.FSharp.Compiler"
    let open3 = "--open Microsoft.FSharp.Compiler"

    // restore all the required tools, declared in each fsproj
    run false "dotnet" "restore %s" netstdsln
    run false "dotnet" "restore %s" "tools.fsproj"

    // run tools
    let toolDir = "../packages/FsLexYacc.7.0.6/build"
    let fsLex fsl out = runCmdIn isMono "." (sprintf "%s/fslex.exe" toolDir) "%s --unicode %s -o %s" fsl lexArgs out
    let fsYacc fsy out m o = runCmdIn isMono "." (sprintf "%s/fsyacc.exe" toolDir) "%s %s %s %s %s -o %s" fsy lexArgs yaccArgs m o out

    run false "dotnet" "fssrgen ../src/fsharp/FSComp.txt FSharp.Compiler.Service.netstandard/FSComp.fs FSharp.Compiler.Service.netstandard/FSComp.resx"
    run false "dotnet" "fssrgen ../src/fsharp/fsi/FSIstrings.txt FSharp.Compiler.Service.netstandard/FSIstrings.fs FSharp.Compiler.Service.netstandard/FSIstrings.resx"
    fsLex "../src/fsharp/lex.fsl" "FSharp.Compiler.Service.netstandard/lex.fs"
    fsLex "../src/fsharp/pplex.fsl" "FSharp.Compiler.Service.netstandard/pplex.fs"
    fsLex "../src/absil/illex.fsl" "FSharp.Compiler.Service.netstandard/illex.fs"
    fsYacc "../src/absil/ilpars.fsy" "FSharp.Compiler.Service.netstandard/ilpars.fs" module1 open1
    fsYacc "../src/fsharp/pars.fsy" "FSharp.Compiler.Service.netstandard/pars.fs" module2 open2
    fsYacc "../src/fsharp/pppars.fsy" "FSharp.Compiler.Service.netstandard/pppars.fs" module3 open3
)

Target "Build.NetStd" (fun _ ->
    run false "dotnet" "pack %s -v n -c Release" netstdsln
)


Target "RunTests.NetStd" (fun _ ->
    run false "dotnet" "run -p FSharp.Compiler.Service.Tests.netcore/FSharp.Compiler.Service.Tests.netcore.fsproj -c Release -- --result:TestResults.NetStd.xml;format=nunit3"
)


//use dotnet-mergenupkg to merge the .NETstandard nuget package into the default one
Target "Nuget.AddNetStd" (fun _ ->
    do
        let nupkg = sprintf "%s/FSharp.Compiler.Service.%s.nupkg" releaseDir release.AssemblyVersion
        let netcoreNupkg = sprintf "FSharp.Compiler.Service.netstandard/bin/Release/FSharp.Compiler.Service.%s.nupkg" release.AssemblyVersion
        runCmdIn false "." "dotnet" "mergenupkg --source %s --other %s --framework netstandard1.6" nupkg netcoreNupkg
)


// --------------------------------------------------------------------------------------
// Generate the documentation

Target "GenerateDocs" (fun _ ->
    executeFSIWithArgs "docsrc/tools" "generate.fsx" ["--define:RELEASE"] [] |> ignore
)

Target "GenerateDocsJa" (fun _ ->
    executeFSIWithArgs "docsrc/tools" "generate.ja.fsx" ["--define:RELEASE"] [] |> ignore
)

// --------------------------------------------------------------------------------------
// Release Scripts

Target "PublishNuGet" (fun _ ->
    Paket.Push (fun p ->
        let apikey =
            match getBuildParam "nuget-apikey" with
            | s when not (String.IsNullOrWhiteSpace s) -> s
            | _ -> getUserInput "Nuget API Key: "
        { p with
            ApiKey = apikey
            WorkingDir = releaseDir })
)

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "Clean" DoNothing
Target "CleanDocs" DoNothing
Target "Release" DoNothing
Target "NuGet" DoNothing
Target "All" DoNothing
Target "All.NetStd" DoNothing
Target "All.NetFx" DoNothing

"Clean"
  =?> ("BuildVersion", isAppVeyorBuild)
  ==> "CodeGen.NetStd"
  ==> "Build.NetStd"
//  ==> "RunTests.NetStd"
  ==> "All.NetStd"

"Clean"
  =?> ("BuildVersion", isAppVeyorBuild)
  ==> "Build.NetFx"
//  ==> "RunTests.NetFx"
  ==> "All.NetFx"

"All.NetFx"
  =?> ("All.NetStd", isDotnetSDKInstalled)
  ==> "All"

"All.NetStd"
  ==> "Nuget.AddNetStd"

"All.NetFx"
  ==> "NuGet.NetFx"
  =?> ("Nuget.AddNetStd", isDotnetSDKInstalled)
  ==> "NuGet"

"All"
  ==> "NuGet"
  ==> "PublishNuGet"
  ==> "Release"

"CleanDocs"
  ==> "GenerateDocsJa"
  ==> "GenerateDocs"
  ==> "Release"

RunTargetOrDefault "All"
