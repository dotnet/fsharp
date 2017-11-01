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

let dotnetExePath = DotNetCli.InstallDotNetSDK "2.0.2"

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue

    if result <> 0 then failwithf "dotnet %s failed" args

// --------------------------------------------------------------------------------------
// Utilities
// --------------------------------------------------------------------------------------

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

let releaseDir = Path.Combine(__SOURCE_DIRECTORY__, "../Release")


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

Target "Clean" (fun _ ->
    CleanDir releaseDir
)

Target "Restore" (fun _ ->
    for p in (!! "./../**/packages.config") do
        let result =
            ExecProcess (fun info ->
                info.FileName <- FullName @"./../.nuget/NuGet.exe"
                info.WorkingDirectory <- FullName @"./.."
                info.Arguments <- sprintf "restore %s -PackagesDirectory \"%s\" -ConfigFile \"%s\""   (FullName p) (FullName "./../packages") (FullName "./../.nuget/NuGet.Config")) TimeSpan.MaxValue
        if result <> 0 then failwithf "nuget restore %s failed" p

    runDotnet __SOURCE_DIRECTORY__ "restore tools.fsproj"
)

Target "BuildVersion" (fun _ ->
    Shell.Exec("appveyor", sprintf "UpdateBuild -Version \"%s\"" buildVersion) |> ignore
)

// --------------------------------------------------------------------------------------
// Clean build results & restore NuGet packages


Target "Build.NetFx" (fun _ ->
    !! "FSharp.Compiler.Service.sln"
    |> MSBuild "" "Build" ["Configuration","Release" ]
    |> Log (".NETFxBuild-Output: ")
)


// --------------------------------------------------------------------------------------
// Run the unit tests using test runner

Target "Test.NetFx" (fun _ ->
    let testDir = __SOURCE_DIRECTORY__ + "/../tests/fcs"
    CleanDir testDir

    let outDir = releaseDir + "/fcs"

    !! (outDir + "/**/*.*")
    |> CopyFiles testDir

    let toolPath = __SOURCE_DIRECTORY__ + "/../packages/NUnit.Console.3.0.0/tools"
    !! (toolPath + "/*.*")
    |> CopyFiles testDir

    !! (testDir + "/**/FSharp.Compiler.Service.Tests.dll")
    |>  Fake.Testing.NUnit3.NUnit3 (fun p ->
        { p with
            ToolPath = testDir + "/nunit3-console.exe"
            ShadowCopy = false
            WorkingDir = FullName testDir
            TimeOut = TimeSpan.FromMinutes 20. })
)

// --------------------------------------------------------------------------------------
// Build a NuGet package
Target "NuGet.NetFx" (fun _ ->
    runCmdIn __SOURCE_DIRECTORY__  "../.nuget/NuGet.exe" @"pack nuget/FSharp.Compiler.Service.nuspec -OutputDirectory %s" releaseDir
    runCmdIn __SOURCE_DIRECTORY__  "../.nuget/NuGet.exe" @"pack nuget/FSharp.Compiler.Service.MSBuild.v12.nuspec -OutputDirectory %s" releaseDir
    runCmdIn __SOURCE_DIRECTORY__  "../.nuget/NuGet.exe" @"pack nuget/FSharp.Compiler.Service.ProjectCracker.nuspec -OutputDirectory %s" releaseDir
)



// --------------------------------------------------------------------------------------
// .NET Core and .NET Core SDK


Target "Build.NetStd" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ (sprintf "pack %s -v n -c Release" "FSharp.Compiler.Service.netstandard.sln")
)


Target "Test.NetStd" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ (sprintf "run -p FSharp.Compiler.Service.Tests.netcore/FSharp.Compiler.Service.Tests.netcore.fsproj -c Release -- --result:TestResults.NetStd.xml;format=nunit3")
)


//use dotnet-mergenupkg to merge the .NETstandard nuget package into the default one
Target "Nuget.AddNetStd" (fun _ ->
    let nupkg = sprintf "%s/FSharp.Compiler.Service.%s.nupkg" releaseDir release.AssemblyVersion
    let netcoreNupkg = sprintf "FSharp.Compiler.Service.netstandard/bin/Release/FSharp.Compiler.Service.%s.nupkg" release.AssemblyVersion
    runDotnet __SOURCE_DIRECTORY__ (sprintf "mergenupkg --source %s --other %s --framework netstandard1.6" nupkg netcoreNupkg)
)


// --------------------------------------------------------------------------------------
// Generate the documentation

Target "GenerateDocsEn" (fun _ ->
    executeFSIWithArgs "docsrc/tools" "generate.fsx" [] [] |> ignore
)

Target "GenerateDocsJa" (fun _ ->
    executeFSIWithArgs "docsrc/tools" "generate.ja.fsx" [] [] |> ignore
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

Target "Release" DoNothing
Target "NuGet" DoNothing
Target "Build" DoNothing
Target "GenerateDocs" DoNothing
Target "TestAndNuGet" DoNothing

"Clean"
  =?> ("BuildVersion", isAppVeyorBuild)
  ==> "Restore"
  ==> "Build.NetStd"

"Clean"
  =?> ("BuildVersion", isAppVeyorBuild)
  ==> "Restore"
  ==> "Build.NetFx"

"Build.NetFx"
  ==> "Test.NetFx"

"Build.NetStd"
  ==> "Test.NetStd"

"Build.NetFx"
  ==> "Build.NetStd"
  ==> "Build"

"Build.NetStd"
  ==> "Nuget.AddNetStd"

"Build.NetFx"
  ==> "NuGet.NetFx"
  ==> "Nuget.AddNetStd"
  ==> "NuGet"

"Test.NetFx"
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
