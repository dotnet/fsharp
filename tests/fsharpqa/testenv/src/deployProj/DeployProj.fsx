#load "../../../../../src/buildtools/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System.IO
open FSharp.Data
open FSharp.Data.JsonExtensions 

let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    @"tests\fsharp\project.lock.json"
let PackagesDir        = getCmdLineArg "--packagesDir:"        @"packages"
let TargetPlatformName = getCmdLineArg "--targetPlatformName:" @".NETStandard,Version=v1.6/win7-x64"
let FSharpCore         = getCmdLineArg "--fsharpCore:"         @"release\coreclr\bin\FSharp.Core.dll"
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

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, TargetPlatformName, ProjectJsonLock, true)

//Okay copy everything
makeDirectory Output
dependencies |> Seq.iter(fun source -> copyFile source Output)
if CopyCompiler = "yes" then 
    copyFile FSharpCore Output
    FSharpCompilerFiles |> Seq.iter(fun source -> copyFile source Output)
