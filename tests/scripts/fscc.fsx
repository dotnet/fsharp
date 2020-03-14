// Simulate fsc.exe on .NET Core

#load "../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO

let root            = Path.GetFullPath                   (__SOURCE_DIRECTORY__ ++ ".." ++ "..")

let Platform        = getCmdLineArg "--platform:"        "win7-x64"
let ProjectJsonLock = getCmdLineArg "--projectJsonLock:" (root ++ "tests" ++ "fsharp" ++ "FSharp.Tests.FSharpSuite.DrivingCoreCLR" ++ "project.lock.json")
let PackagesDir     = getCmdLineArg "--packagesDir:"     (root ++ "packages")
let FrameworkName   = getCmdLineArg "--framework:"       ".NETCoreApp,Version=v1.0"
let Verbosity       = getCmdLineArg "--verbose:"         "quiet"
let CompilerPathOpt = getCmdLineArgOptional              "--compilerPath:"
let Flavour         = getCmdLineArg "--flavour:"         "release"
let TestName        = getCmdLineArg "--TestName:"        "test"
let OutputDir       = getCmdLineArg "--OutputDir:"       ("bin" ++ Flavour)
let CopyDlls        = getCmdLineArg "--CopyDlls:"        ""
let ExtraArgs       = getCmdLineExtraArgs                (fun x -> List.exists x.StartsWith ["--platform:";"--projectJsonLock:";"--packagesDir:";"--framework:";"--verbose:";"--compilerPath:";"--flavour:";"--TestName:";"--OutputDir:";"--CopyDlls:"])

let CompilerPath    = defaultArg CompilerPathOpt (root ++ "tests" ++ "testbin" ++ Flavour ++ "coreclr" ++ "fsc")
let Win32Manifest   = CompilerPath ++ "default.win32manifest"

let isRepro = Verbosity = "repro" || Verbosity = "verbose"
let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, false, false)

let runtimeConfigLines =
    [| "{";
       "  \"runtimeOptions\": {";
       "    \"framework\": {";
       "         \"name\": \"Microsoft.NETCore.App\",";
       "         \"version\": \"1.0.1\"";
       "    }";
       "  }";
       "}" |]

let executeCompiler references =
    let Win32manifest=Path.Combine(CompilerPath, "default.win32manifest")
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "-r:%s" i)
    let arguments = 
        [ yield "--noframework"
          yield "--simpleresolution"
          yield "--targetprofile:netcore"
          yield "--win32manifest:" + Win32Manifest
          yield! addReferenceSwitch references 
          yield! ExtraArgs ]

    let coreRunExe = (root ++ "Tools" ++ "dotnetcli" ++ "dotnet.exe")
    let fscExe = (CompilerPath ++ "fsc.exe")
    let arguments2 = sprintf @"%s %s" fscExe (String.concat " " arguments)
    if isVerbose then 
        log "%s %s" coreRunExe arguments2
        log "%s %s @fsc.cmd.args" coreRunExe fscExe
    File.WriteAllLines(OutputDir ++ "fsc.cmd.args", arguments)
    File.WriteAllLines(OutputDir ++ (TestName + ".runtimeconfig.json"), runtimeConfigLines)
    CopyDlls.Split(';') |> Array.iter(fun s -> if not (System.String.IsNullOrWhiteSpace(s)) then File.Copy(s, OutputDir ++ Path.GetFileName(s), true))
    executeProcess coreRunExe arguments2

exit (executeCompiler dependencies)
