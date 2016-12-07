#load @"../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO
open System.Diagnostics

let root = Path.GetFullPath(__SOURCE_DIRECTORY__ ++ ".." ++ "..")
let Platform = getCmdLineArg "--platform:"    "win7-x64"
let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    (root ++ "tests" ++ "fsharp" ++ "project.lock.json")
let PackagesDir        = getCmdLineArg "--packagesDir:"        (root ++ "packages")
let FrameworkName      = getCmdLineArg "--framework:"      ".NETStandard,Version=v1.6"
let Verbosity          = getCmdLineArg "--verbose:"                  "quiet"
let CompilerPathOpt    = getCmdLineArgOptional "--compilerPath:"       
let Flavour            = getCmdLineArg "--flavour:"       "release"
let ExtraArgs          = getCmdLineExtraArgs  (fun x -> List.exists x.StartsWith ["--platform:";"--projectJsonLock:";"--packagesDir:";"--framework:";"--verbose:";"--compilerPath:";"--flavour:"])
let CompilerPath       = defaultArg CompilerPathOpt (root ++ "tests" ++ "testbin" ++ Flavour ++ "coreclr" ++ "fsc" ++ Platform)
let Win32Manifest = CompilerPath ++ "default.win32manifest"

let isRepro = Verbosity = "repro" || Verbosity = "verbose"
let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, false, false) |> Seq.toArray


let executeFsi references =
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "-r:%s" i)
    let arguments = 
        [ yield "--noframework"
          yield! addReferenceSwitch references
          yield "-r:" + (CompilerPath ++ "FSharp.Core.dll")
          yield! ExtraArgs ]

    let coreRunExe = (CompilerPath ++ "CoreRun.exe")
    let fsiExe = (CompilerPath ++ "fsi.exe")
    let arguments2 = sprintf @"%s %s" fsiExe (String.concat " " arguments)

    if isRepro then 
        File.WriteAllLines("fsi.cmd.args", arguments)
        log "%s %s" coreRunExe arguments2
        log "%s %s @fsi.cmd.args" coreRunExe fsiExe 

    executeProcessNoRedirect coreRunExe arguments2

executeFsi dependencies // ignore exit code for now since FailFast gives negative error code

