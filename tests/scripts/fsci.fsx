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

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, true, false) |> Seq.toArray

let executeProcessNoRedirect filename arguments =
    if isVerbose then 
       printfn "%s %s" filename arguments
    let info = ProcessStartInfo(Arguments=arguments, UseShellExecute=false, 
                                RedirectStandardOutput=true, RedirectStandardError=true,RedirectStandardInput=true,
                                CreateNoWindow=true, FileName=filename)
    let p = new Process(StartInfo=info)
    if p.Start() then

        async { try 
                  let buffer = Array.zeroCreate 4096
                  while not p.StandardOutput.EndOfStream do 
                    let n = p.StandardOutput.Read(buffer, 0, buffer.Length)
                    if n > 0 then System.Console.Out.Write(buffer, 0, n)
                with _ -> () } |> Async.Start
        async { try 
                  let buffer = Array.zeroCreate 4096
                  while not p.StandardError.EndOfStream do 
                    let n = p.StandardError.Read(buffer, 0, buffer.Length)
                    if n > 0 then System.Console.Error.Write(buffer, 0, n)
                with _ -> () } |> Async.Start
        async { try 
                  while true do 
                    let c = System.Console.In.ReadLine()
                    p.StandardInput.WriteLine(c)
                with _ -> () } |> Async.Start
        p.WaitForExit()
        p.ExitCode
    else
        0

let executeCompiler references =
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

exit (executeCompiler dependencies)

