#load "../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO
open System.Diagnostics

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

let executeFsi references =
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "-r:%s" i)
    let arguments = 
        [ yield "--noframework"
          yield "--simpleresolution"
          yield "--targetprofile:netcore"
          yield! addReferenceSwitch references
          yield! ExtraArgs ]

    let coreRunExe = (root ++ "Tools" ++ "dotnetcli" ++ "dotnet.exe")
    let fsiExe = (CompilerPath ++ "fsi.exe")
    let arguments2 = sprintf @"%s %s" fsiExe (String.concat " " arguments)
    if isVerbose then 
        log "%s %s" coreRunExe arguments2
        log "%s %s @fsi.cmd.args" coreRunExe fsiExe
    File.WriteAllLines(OutputDir ++ "fsi.cmd.args", arguments)
    File.WriteAllLines(OutputDir ++ (TestName + ".runtimeconfig.json"), runtimeConfigLines)
    CopyDlls.Split(';') |> Array.iter(fun s -> if not (System.String.IsNullOrWhiteSpace(s)) then File.Copy(s, OutputDir ++ Path.GetFileName(s), true))
    executeProcess coreRunExe arguments2

exit (executeFsi dependencies)
