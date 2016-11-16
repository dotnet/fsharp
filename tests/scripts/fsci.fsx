#load @"../../src/scripts/scriptlib.fsx"
#load "crackProjectJson.fsx"

open System
open System.IO
open System.Diagnostics

let root = Path.GetFullPath(__SOURCE_DIRECTORY__ ++ ".." ++ "..")
let Platform = getCmdLineArg "--platform:"    "win7-x64"
let ProjectJsonLock    = getCmdLineArg "--projectJsonLock:"    (root ++ "tests" ++ "fsharp" ++ "project.lock.json")
let PackagesDir        = getCmdLineArg "--packagesDir:"        (root ++ "packages")
let FrameworkName      = getCmdLineArg "--frameworkName:"      ".NETStandard,Version=v1.6"
let Verbosity          = getCmdLineArg "--v:"                  "quiet"
let CompilerPathOpt    = getCmdLineArgOptional "--compilerPath:"       
let ExtraArgs          = getCmdLineExtraArgs "--"       

let CompilerPath       = defaultArg CompilerPathOpt (root ++ "tests" ++ "testbin" ++ "release" ++ "coreclr" ++ "fsc" ++ Platform)
let Win32Manifest = CompilerPath ++ "default.win32manifest"

let isVerbose = Verbosity = "verbose"

let dependencies = CrackProjectJson.collectReferences (isVerbose, PackagesDir, FrameworkName + "/" + Platform, ProjectJsonLock, false)

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
    let Win32manifest=Path.Combine(CompilerPath, "default.win32manifest")
    let listToPrefixedSpaceSeperatedString prefix list = list |> Seq.fold(fun a t -> sprintf "%s %s%s" a prefix t) ""
    let listToSpaceSeperatedString list = list |> Seq.fold(fun a t -> sprintf "%s %s" a t) ""
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "--reference:%s" i)
    let arguments = sprintf @"%s --noframework %s -r:%s %s"
                            (CompilerPath ++ "fsi.exe")
                            (listToSpaceSeperatedString (addReferenceSwitch references)) 
                            (CompilerPath ++ "FSharp.Core.dll")
                            (String.concat " " ExtraArgs)

    executeProcessNoRedirect (CompilerPath ++ "CoreRun.exe") arguments

exit (executeCompiler dependencies)
