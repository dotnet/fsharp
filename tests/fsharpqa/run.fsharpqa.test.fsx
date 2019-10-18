// Work In Progress
// this script helps run a subset of the fsharpqa tests without calling a full build.cmd

open System.IO
open System.Diagnostics

let releaseOrDebug = "Debug"
let setEnvVar name value =
    System.Environment.SetEnvironmentVariable(name, value)

let addToPath path =
    let currentPath = System.Environment.GetEnvironmentVariable "PATH"

    let splits = currentPath.Split(Path.PathSeparator)
    if not(Array.contains path splits) then
        setEnvVar "PATH" (path + (string Path.PathSeparator) + currentPath)

let nugetCache = Path.Combine(System.Environment.GetEnvironmentVariable "USERPROFILE", ".nuget", "packages")
let rootFolder = Path.Combine(__SOURCE_DIRECTORY__, "..", "..")
let compilerBinFolder = Path.Combine(rootFolder, "artifacts", "bin", "fsc", releaseOrDebug, "net472")
setEnvVar "CSC_PIPE"      (Path.Combine(nugetCache, "Microsoft.Net.Compilers", "2.7.0", "tools", "csc.exe"))
setEnvVar "FSC"           (Path.Combine(compilerBinFolder, "fsc.exe"))
setEnvVar "FSCOREDLLPATH" (Path.Combine(compilerBinFolder, "FSharp.Core.dll"))
addToPath compilerBinFolder

let runPerl arguments =
    // Kill all Perl processes, and their children
    ProcessStartInfo(
        FileName = "taskkill",
        Arguments = "/im perl.exe /f /t",
        CreateNoWindow = true,
        UseShellExecute = false
    )
    |> Process.Start
    |> fun p -> p.WaitForExit()

    use perlProcess =
        ProcessStartInfo(
            FileName = Path.Combine(nugetCache, "StrawberryPerl64", "5.22.2.1", "Tools", "perl", "bin", "perl.exe"),
            Arguments = (arguments |> Array.map(fun a -> @"""" + a + @"""") |> String.concat " "),
            WorkingDirectory = Path.Combine(rootFolder, "tests", "fsharpqa", "source"),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        )
        |> Process.Start

    while (not perlProcess.StandardOutput.EndOfStream) do
        perlProcess.StandardOutput.ReadLine() |> printfn "%s" 
    while (not perlProcess.StandardError.EndOfStream) do
        perlProcess.StandardError.ReadLine() |> printfn "%s" 
    perlProcess.WaitForExit()
    if perlProcess.ExitCode <> 0 then
        failwithf "exit code: %i" perlProcess.ExitCode

let testResultDir = Path.Combine(rootFolder, "tests", "TestResults")
let perlScript = Path.Combine(rootFolder, "tests", "fsharpqa", "testenv", "bin", "runall.pl")
runPerl [|perlScript; "-resultsroot";testResultDir ;"-ttags:Determinism"|]