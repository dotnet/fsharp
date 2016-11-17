// Simulate fsc.exe on .NET Core

#load @"../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO

let root = Path.GetFullPath (__SOURCE_DIRECTORY__ ++ ".." ++ "..")
let Platform = getCmdLineArg "--platform:"    "win7-x64"
let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    (root ++ "tests" ++ "fsharp" ++ "project.lock.json")
let PackagesDir        = getCmdLineArg "--packagesDir:"        (root ++ "packages")
let FrameworkName      = getCmdLineArg "--frameworkName:"      ".NETStandard,Version=v1.6"
let Verbosity          = getCmdLineArg "--v:"                  "quiet"
let CompilerPathOpt    = getCmdLineArgOptional "--compilerPath:"       
let Flavour            = getCmdLineArg "--flavour:"       "release"
let ExtraArgs          = getCmdLineExtraArgs "--"       

let CompilerPath       = defaultArg CompilerPathOpt (root ++ "tests" ++ "testbin" ++ Flavour ++ "coreclr" ++ "fsc" ++ Platform)
let Win32Manifest = CompilerPath ++ "default.win32manifest"

let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, false)

let executeCompiler references =
    let Win32manifest=Path.Combine(CompilerPath, "default.win32manifest")
    let listToPrefixedSpaceSeperatedString prefix list = list |> Seq.fold(fun a t -> sprintf "%s %s%s" a prefix t) ""
    let listToSpaceSeperatedString list = list |> Seq.fold(fun a t -> sprintf "%s %s" a t) ""
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "--reference:%s" i)
    let arguments = sprintf @"%s --noframework --simpleresolution  --targetprofile:netcore --win32manifest:%s %s -r:%s %s"
                            (CompilerPath ++ "fsc.exe")
                            Win32Manifest
                            (listToSpaceSeperatedString (addReferenceSwitch references)) 
                            (CompilerPath ++ "FSharp.Core.dll")
                            (String.concat " " ExtraArgs)
    log "%s %s" (CompilerPath ++ "CoreRun.exe") arguments
    executeProcess (CompilerPath ++ "CoreRun.exe") arguments

exit (executeCompiler dependencies)
