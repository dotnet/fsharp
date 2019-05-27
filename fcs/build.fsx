// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"
open System
open System.IO
open Fake
open Fake.AppVeyor
open Fake.ReleaseNotesHelper

#if MONO
// prevent incorrect output encoding (e.g. https://github.com/fsharp/FAKE/issues/1196)
System.Console.OutputEncoding <- System.Text.Encoding.UTF8
CleanDir (__SOURCE_DIRECTORY__ + "/../artifacts/TestResults") 
File.WriteAllText(__SOURCE_DIRECTORY__ + "/../artifacts/TestResults/notestsyet.txt","No tests yet")
let isMono = true
#else
let isMono = false
#endif

// --------------------------------------------------------------------------------------
// Utilities
// --------------------------------------------------------------------------------------

let dotnetExePath =
    // Build.cmd normally downloads a dotnet cli to: <repo-root>\artifacts\toolset\dotnet
    // check if there is one there to avoid downloading an additional one here
    let pathToCli = Path.Combine(__SOURCE_DIRECTORY__, @"..\artifacts\toolset\dotnet\dotnet.exe")
    if File.Exists(pathToCli) then
        pathToCli
    else
        DotNetCli.InstallDotNetSDK "2.2.105"

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue

    if result <> 0 then failwithf "dotnet %s failed" args

let assertExitCodeZero x = if x = 0 then () else failwithf "Command failed with exit code %i" x

let runCmdIn workDir (exe:string) = Printf.ksprintf (fun (args:string) ->
#if MONO
        let exe = exe.Replace("\\","/")
        let args = args.Replace("\\","/")
        printfn "[%s] mono %s %s" workDir exe args
        Shell.Exec("mono", sprintf "%s %s" exe args, workDir)
#else
        printfn "[%s] %s %s" workDir exe args
        Shell.Exec(exe, args, workDir)
#endif
        |> assertExitCodeZero
)

// --------------------------------------------------------------------------------------
// The rest of the code is standard F# build script
// --------------------------------------------------------------------------------------

let releaseDir = Path.Combine(__SOURCE_DIRECTORY__, "../artifacts/bin/fcs/Release")

// Read release notes & version info from RELEASE_NOTES.md
let release = LoadReleaseNotes (__SOURCE_DIRECTORY__ + "/RELEASE_NOTES.md")
let isAppVeyorBuild = buildServer = BuildServer.AppVeyor
let isJenkinsBuild = buildServer = BuildServer.Jenkins
let isVersionTag tag = Version.TryParse tag |> fst
let hasRepoVersionTag = isAppVeyorBuild && AppVeyorEnvironment.RepoTag && isVersionTag AppVeyorEnvironment.RepoTagName
let assemblyVersion = if hasRepoVersionTag then AppVeyorEnvironment.RepoTagName else release.NugetVersion

let buildVersion =
    if hasRepoVersionTag then assemblyVersion
    else if isAppVeyorBuild then sprintf "%s-b%s" assemblyVersion AppVeyorEnvironment.BuildNumber
    else assemblyVersion

Target "Clean" (fun _ ->
    CleanDir releaseDir
)

Target "Restore" (fun _ ->
    // We assume a paket restore has already been run
    runDotnet __SOURCE_DIRECTORY__ "restore ../src/buildtools/buildtools.proj -v n"
    runDotnet __SOURCE_DIRECTORY__ "restore FSharp.Compiler.Service.sln -v n"
)

Target "BuildVersion" (fun _ ->
    Shell.Exec("appveyor", sprintf "UpdateBuild -Version \"%s\"" buildVersion) |> ignore
)

Target "Build" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "build ../src/buildtools/buildtools.proj -v n -c Proto"
    let fslexPath = __SOURCE_DIRECTORY__ + "/../artifacts/bin/fslex/Proto/netcoreapp2.1/fslex.dll"
    let fsyaccPath = __SOURCE_DIRECTORY__ + "/../artifacts/bin/fsyacc/Proto/netcoreapp2.1/fsyacc.dll"
    runDotnet __SOURCE_DIRECTORY__ (sprintf "build FSharp.Compiler.Service.sln -v n -c Release /p:FsLexPath=%s /p:FsYaccPath=%s" fslexPath fsyaccPath)
)

Target "Test" (fun _ ->
    // This project file is used for the netcoreapp2.0 tests to work out reference sets
    runDotnet __SOURCE_DIRECTORY__ "build ../tests/projects/Sample_NETCoreSDK_FSharp_Library_netstandard2_0/Sample_NETCoreSDK_FSharp_Library_netstandard2_0.fsproj -v n /restore /p:DisableCompilerRedirection=true"

    // Now run the tests
    let logFilePath = Path.Combine(__SOURCE_DIRECTORY__, "..", "artifacts", "TestResults", "Release", "FSharp.Compiler.Service.Test.xml")
    runDotnet __SOURCE_DIRECTORY__ (sprintf "test FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --no-restore --no-build -v n -c Release --test-adapter-path . --logger \"nunit;LogFilePath=%s\"" logFilePath)
)

Target "NuGet" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "pack FSharp.Compiler.Service.sln -v n -c Release"
)

Target "GenerateDocsEn" (fun _ ->
    executeFSIWithArgs "docsrc/tools" "generate.fsx" [] [] |> ignore
)

Target "GenerateDocsJa" (fun _ ->
    executeFSIWithArgs "docsrc/tools" "generate.ja.fsx" [] [] |> ignore
)

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

Target "Start" DoNothing
Target "Release" DoNothing
Target "GenerateDocs" DoNothing
Target "TestAndNuGet" DoNothing

"Start"
  =?> ("BuildVersion", isAppVeyorBuild)
  ==> "Restore"
  ==> "Build"

"Build"
  ==> "Test"

"Build"
  ==> "NuGet"

"Test"
  ==> "TestAndNuGet"

"NuGet"
  ==> "TestAndNuGet"
  
"Build"
  ==> "NuGet"
  ==> "PublishNuGet"
  ==> "Release"

"Build"
  ==> "GenerateDocsEn"
  ==> "GenerateDocs"

"Build"
  ==> "GenerateDocsJa"
  ==> "GenerateDocs"

"GenerateDocs"
  ==> "Release"

RunTargetOrDefault "Build"
