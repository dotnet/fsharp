#load @"../../../../../src/buildtools/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO

let Sources = argv |> Seq.filter(fun t -> printfn "%s" t; t.StartsWith("--source:")) |> Seq.map(fun t -> t.Remove(0, 9).Trim()) |> Seq.distinct
let Defines = argv |> Seq.filter(fun t -> printfn "%s" t; t.StartsWith("--define:")) |> Seq.map(fun t -> t.Remove(0, 9).Trim())

let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    @"tests\fsharp\project.lock.json"
let PackagesDir        = getCmdLineArg "--packagesDir:"        @"packages"
let TargetPlatformName = getCmdLineArg "--targetPlatformName:" @"DNXCore,Version=v5.0/win7-x64"
let FSharpCore         = getCmdLineArg "--fsharpCore:"         @"Release\coreclr\bin\FSharp.Core.dll"
let Output             = getCmdLineArg "--output:"             @"."
let Verbosity          = getCmdLineArg "--v:"                  @"quiet"
let CompilerPath       = getCmdLineArg "--compilerPath:"       @"."
let TestKeyFile        = getCmdLineArg "--keyfile:"            @""
let TestDelaySign      = getCmdLineArg "--delaysign:"          @""
let TestPublicSign     = getCmdLineArg "--publicsign:"          @""
let ExtraDefines       = getCmdLineArg "--ExtraDefines:"       @""

let KeyFileOption = if isNullOrEmpty TestKeyFile then "" else sprintf "--keyfile:%s" TestKeyFile
let DelaySignOption = if isNullOrEmpty TestDelaySign then "" else "--delaysign"
let PublicSignOption = if isNullOrEmpty TestPublicSign then "" else "--publicsign"

let FSharpCoreDir = getDirectoryName FSharpCore
let Win32Manifest = FSharpCoreDir ++ "default.win32manifest"
let FSharpCompilerFiles =
    [ FSharpCoreDir ++ "fsc.exe"
      FSharpCoreDir ++ "FSharp.Compiler.dll"
      FSharpCoreDir ++ "FSharp.Core.sigdata"
      FSharpCoreDir ++ "FSharp.Core.optdata"
      Win32Manifest
      FSharpCoreDir ++ "fsi.exe"
      FSharpCoreDir ++ "FSharp.Compiler.Interactive.Settings.dll"]

let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, TargetPlatformName, ProjectJsonLock, false)

let executeCompiler sources references =
    let Win32manifest=Path.Combine(CompilerPath, "default.win32manifest")
    let listToPrefixedSpaceSeperatedString prefix list = list |> Seq.fold(fun a t -> sprintf "%s %s%s" a prefix t) ""
    let listToSpaceSeperatedString list = list |> Seq.fold(fun a t -> sprintf "%s %s" a t) ""
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "--reference:%s" i)
    printfn ">%s<" (KeyFileOption)
    printfn ">%s<" (DelaySignOption)
    let arguments = sprintf @"%s --debug:portable --debug+ --noframework --simpleresolution  --out:%s --define:BASIC_TEST --targetprofile:netcore --target:exe -g --times --win32manifest:%s %s -r:%s %s %s %s %s %s %s"
                            (CompilerPath ++ "fsc.exe")
                            Output
                            Win32Manifest
                            (listToSpaceSeperatedString (addReferenceSwitch references)) 
                            FSharpCore
                            (listToPrefixedSpaceSeperatedString "--define:" Defines) 
                            KeyFileOption
                            DelaySignOption
                            PublicSignOption
                            ExtraDefines
                            (listToSpaceSeperatedString sources)
    File.WriteAllText("coreclr.fsc.cmd",(CompilerPath ++ "CoreRun.exe") + arguments)
    executeProcess (CompilerPath ++ "CoreRun.exe") arguments

makeDirectory (getDirectoryName Output)
exit (executeCompiler Sources dependencies)
