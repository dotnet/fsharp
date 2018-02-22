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
CleanDir (__SOURCE_DIRECTORY__ + "/../tests/TestResults") 
File.WriteAllText(__SOURCE_DIRECTORY__ + "/../tests/TestResults/notestsyet.txt","No tests yet")
#endif

// --------------------------------------------------------------------------------------
// Utilities
// --------------------------------------------------------------------------------------

let dotnetExePath = DotNetCli.InstallDotNetSDK "2.1.4"

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

let releaseDir = Path.Combine(__SOURCE_DIRECTORY__, "../Release/fcs")


// Read release notes & version info from RELEASE_NOTES.md
let release = LoadReleaseNotes (__SOURCE_DIRECTORY__ + "/RELEASE_NOTES.md")
let isAppVeyorBuild = buildServer = BuildServer.AppVeyor
let isVersionTag tag = Version.TryParse tag |> fst
let hasRepoVersionTag = isAppVeyorBuild && AppVeyorEnvironment.RepoTag && isVersionTag AppVeyorEnvironment.RepoTagName
let assemblyVersion = if hasRepoVersionTag then AppVeyorEnvironment.RepoTagName else release.NugetVersion
let nugetVersion = release.NugetVersion
open SemVerHelper

let buildVersion =
    if hasRepoVersionTag then assemblyVersion
    else if isAppVeyorBuild then sprintf "%s-b%s" assemblyVersion AppVeyorEnvironment.BuildNumber
    else assemblyVersion

Target "Clean" (fun _ ->
    CleanDir releaseDir
)

Target "Restore" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ (sprintf "restore %s -v n"  "FSharp.Compiler.Service.sln")
    for p in (!! "./../**/packages.config") do
        let result =
            ExecProcess (fun info ->
                info.FileName <- FullName @"./../.nuget/NuGet.exe"
                info.WorkingDirectory <- FullName @"./.."
                info.Arguments <- sprintf "restore %s -PackagesDirectory \"%s\" -ConfigFile \"%s\""   (FullName p) (FullName "./../packages") (FullName "./../.nuget/NuGet.Config")) TimeSpan.MaxValue
        if result <> 0 then failwithf "nuget restore %s failed" p
)

Target "BuildVersion" (fun _ ->
    Shell.Exec("appveyor", sprintf "UpdateBuild -Version \"%s\"" buildVersion) |> ignore
)

Target "Build" (fun _ ->
#if MONO
    // Using 'dotnet' to build .NET Framework projects fails on Mono, see https://github.com/dotnet/sdk/issues/335
    // Use 'msbuild' instead
    MSBuild "" "Build" ["Configuration","Release" ] 
        [ "FSharp.Compiler.Service/FSharp.Compiler.Service.fsproj"
          "FSharp.Compiler.Service.MSBuild.v12/FSharp.Compiler.Service.MSBuild.v12.fsproj"
          "FSharp.Compiler.Service.ProjectCracker/FSharp.Compiler.Service.ProjectCracker.fsproj"
          "FSharp.Compiler.Service.ProjectCrackerTool/FSharp.Compiler.Service.ProjectCrackerTool.fsproj"
          "FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj" ]
    |> Log (".NETFxBuild-Output: ")

#else
    runDotnet __SOURCE_DIRECTORY__ (sprintf "build  %s -v n -c Release /maxcpucount:1"  "FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj")
#endif
)

Target "Test" (fun _ ->
#if MONO
    ()
#else
    runDotnet __SOURCE_DIRECTORY__ (sprintf "restore %s -v n"  "../tests/projects/Sample_NETCoreSDK_FSharp_Library_netstandard2_0/Sample_NETCoreSDK_FSharp_Library_netstandard2_0.fsproj")
    runDotnet __SOURCE_DIRECTORY__ (sprintf "build  %s -v n"  "../tests/projects/Sample_NETCoreSDK_FSharp_Library_netstandard2_0/Sample_NETCoreSDK_FSharp_Library_netstandard2_0.fsproj")
    runDotnet __SOURCE_DIRECTORY__ (sprintf "test %s -v n -c Release /maxcpucount:1" "FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj")
#endif
)


Target "NuGet" (fun _ ->
#if MONO
    ()
#else
    runDotnet __SOURCE_DIRECTORY__ (sprintf "pack %s -v n -c Release /maxcpucount:1" "FSharp.Compiler.Service/FSharp.Compiler.Service.fsproj")
    runDotnet __SOURCE_DIRECTORY__ (sprintf "build %s -v n -c Release /maxcpucount:1" "FSharp.Compiler.Service.ProjectCrackerTool/FSharp.Compiler.Service.ProjectCrackerTool.fsproj")
    runDotnet __SOURCE_DIRECTORY__ (sprintf "pack %s -v n -c Release /maxcpucount:1" "FSharp.Compiler.Service.ProjectCracker/FSharp.Compiler.Service.ProjectCracker.fsproj")
    runDotnet __SOURCE_DIRECTORY__ (sprintf "pack %s -v n -c Release /maxcpucount:1" "FSharp.Compiler.Service.MSBuild.v12/FSharp.Compiler.Service.MSBuild.v12.fsproj")
#endif
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

Target "Release" DoNothing
Target "GenerateDocs" DoNothing
Target "TestAndNuGet" DoNothing

"Clean"
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
