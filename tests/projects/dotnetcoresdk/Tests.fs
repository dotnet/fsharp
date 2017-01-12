module Tests

open System
open System.IO
open Xunit

open Scripting
open TestFramework
open DotnetCoreTestConfig

let assetsDirectory = __SOURCE_DIRECTORY__ ++ ".." ++ "assets"
let getPackageToTest cfg =
    match cfg.EnvironmentVariables |> Map.tryFind "TEST_COMPILERSDK_PACKAGE" with
    | Some pkg -> pkg
    | None -> failwith "TEST_COMPILERSDK_PACKAGE env var is not set, should be the package version to test"
let cloneAssets (major,name) toDir =
    let dir = assetsDirectory ++ major ++ name
    dir |> Commands.copyDirectory toDir

[<Fact>]
let ``console app should build`` () =

    let cfg = testConfig (Commands.createTempDir ())

    use packagesDir = helperDir cfg (Commands.createTempDir () ++ "packages")

    let pkg = getPackageToTest cfg

    cloneAssets ("dotnetcoresdk","console") cfg.Directory

    dotnet cfg "--version"

    let feeds = 
        assetsDirectory ++ "dotnetcoresdk" ++ "dotnetcoresdk.NuGet.Config"
        |> readFeedsFromNugetConfig
        |> List.append [ getArtifactsDir cfg  ]

    //using --packages directory and --no-cache is slower but it's done to be sure the right package is used
    // because one with same version can be already saved in local cache, and nuget (dotnet restore) use always
    // the package in local cache if version match
    //the --no-cache just ignore cache for http requests, not the locally cached package in packages directory

    dotnet cfg """restore --packages "%s" --no-cache %s /p:CompilerSdkPackageVersion=%s -v n""" (packagesDir.Path) (feeds |> List.map (sprintf """--source "%s" """) |> String.concat " ") pkg

    dotnet cfg "build -c Debug -v n"

    dotnet cfg "run -c Debug -v n"

    dotnet cfg "build -c Release -v n"

    dotnet cfg "run -c Relase -v n"


[<Fact>]
let ``lib should build`` () =

    let cfg = testConfig (Commands.createTempDir ())

    use packagesDir = helperDir cfg (Commands.createTempDir () ++ "packages")

    let pkg = getPackageToTest cfg

    cloneAssets ("dotnetcoresdk","lib") cfg.Directory

    dotnet cfg "--version"

    let feeds = 
        assetsDirectory ++ "dotnetcoresdk" ++ "dotnetcoresdk.NuGet.Config"
        |> readFeedsFromNugetConfig
        |> List.append [ getArtifactsDir cfg  ]

    //using --packages directory and --no-cache is slower but it's done to be sure the right package is used
    // because one with same version can be already saved in local cache, and nuget (dotnet restore) use always
    // the package in local cache if version match
    //the --no-cache just ignore cache for http requests, not the locally cached package in packages directory

    dotnet cfg """restore --packages "%s" --no-cache %s /p:CompilerSdkPackageVersion=%s -v n""" (packagesDir.Path) (feeds |> List.map (sprintf """--source "%s" """) |> String.concat " ") pkg

    dotnet cfg "build -c Debug -v n"

    dotnet cfg "pack -c Debug /p:PackageVersion=1.0.0-alpha1 -v n"

    dotnet cfg "build -c Release -v n"

    dotnet cfg "pack -c Release /p:PackageVersion=1.0.0-alpha2 -v n"

[<EntryPoint>]
let main argv = 
    ``console app should build`` ()
    ``lib should build`` ()
    0
