#load "../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO
open FSharp.Data
open FSharp.Data.JsonExtensions 
let root = Path.GetFullPath (__SOURCE_DIRECTORY__ ++ ".." ++ "..")

try 
    let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    (root ++ "tests" ++ "fsharp" ++ "project.lock.json")
    let PackagesDir        = getCmdLineArg "--packagesDir:"        (root ++ "packages")
    let Framework           = getCmdLineArg "--framework:"          ".NETStandard,Version=v1.6"
    let Platform           = getCmdLineArg "--platform:"           defaultPlatform
    let FSharpCore         = getCmdLineArg "--fsharpCore:"         @"release/coreclr/bin/FSharp.Core.dll"
    let Output             = getCmdLineArg "--output:"             @"."
    let Verbosity          = getCmdLineArg "--v:"                  @"quiet"
    let CopyCompiler       = getCmdLineArg "--copyCompiler:"       @"no"

    let FSharpCompilerFiles =
        let FSharpCoreDir = getDirectoryName FSharpCore
        [ FSharpCoreDir ++ "fsc.exe"
          FSharpCoreDir ++ "FSharp.Compiler.dll"
          FSharpCoreDir ++ "FSharp.Core.sigdata"
          FSharpCoreDir ++ "FSharp.Core.optdata"
          FSharpCoreDir ++ "default.win32manifest"
          FSharpCoreDir ++ "fsi.exe"
          FSharpCoreDir ++ "FSharp.Compiler.Interactive.Settings.dll" ]

    let isVerbose = Verbosity = "verbose"

    let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, Framework + "/" + Platform, ProjectJsonLock, true, true)

    //Okay copy everything
    makeDirectory Output
    dependencies |> Seq.iter(fun source -> copyFile source Output)
    if CopyCompiler = "yes" then 
        copyFile FSharpCore Output
        FSharpCompilerFiles |> Seq.iter(fun source -> copyFile source Output)
    exit 0
with e -> 
    printfn "%s" (e.ToString())
    exit 1