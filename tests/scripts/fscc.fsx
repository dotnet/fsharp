// Simulate fsc.exe on .NET Core

#load @"../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO

let root = Path.GetFullPath (__SOURCE_DIRECTORY__ ++ ".." ++ "..")
let Platform           = getCmdLineArg "--platform:"    "win7-x64"
let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    (root ++ "tests" ++ "fsharp" ++ "project.lock.json")
let PackagesDir        = getCmdLineArg "--packagesDir:"        (root ++ "packages")
let FrameworkName      = getCmdLineArg "--framework:"          ".NETStandard,Version=v1.6"
let Verbosity          = getCmdLineArg "--verbose:"                  "quiet"
let CompilerPathOpt    = getCmdLineArgOptional "--compilerPath:"       
let Flavour            = getCmdLineArg "--flavour:"       "release"
let ExtraArgs          = getCmdLineExtraArgs  (fun x -> List.exists x.StartsWith ["--platform:";"--projectJsonLock:";"--packagesDir:";"--framework:";"--verbose:";"--compilerPath:";"--flavour:"])

let CompilerPath       = defaultArg CompilerPathOpt (root ++ "tests" ++ "testbin" ++ Flavour ++ "coreclr" ++ "fsc" ++ Platform)
let Win32Manifest = CompilerPath ++ "default.win32manifest"

let isRepro = Verbosity = "repro" || Verbosity = "verbose"
let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, false, false)

let executeCompiler references =
    let Win32manifest=Path.Combine(CompilerPath, "default.win32manifest")
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "-r:%s" i)
    let arguments = 
        [ yield "--noframework"
          yield "--simpleresolution"
          yield "--targetprofile:netcore"
          yield "--win32manifest:" + Win32Manifest
          yield "-r:"+ (CompilerPath ++ "FSharp.Core.dll")
          yield! addReferenceSwitch references 
          yield! ExtraArgs ]
    
    let coreRunExe = (CompilerPath ++ "CoreRun.exe")
    let fscExe = (CompilerPath ++ "fsc.exe")
    let arguments2 = sprintf @"%s %s" fscExe (String.concat " " arguments)
    if isRepro then 
        File.WriteAllLines("fsc.cmd.args", arguments)
        log "%s %s" coreRunExe arguments2
        log "%s %s @fsc.cmd.args" coreRunExe fscExe
    executeProcess coreRunExe arguments2

exit (executeCompiler dependencies)
