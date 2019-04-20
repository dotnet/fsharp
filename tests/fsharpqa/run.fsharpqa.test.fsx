// Work In Progress
// this script helps run a subset of the fsharpqa tests without calling a full build.cmd

open System.IO
open System.Diagnostics
type Env = System.Environment
let releaseOrDebug = "Debug"
let setEnvVar name value =
  System.Environment.SetEnvironmentVariable(name, value)

let addToPath path =
  let currentPath = System.Environment.GetEnvironmentVariable "PATH"
  
  let splits = currentPath.Split(Path.PathSeparator)
  if not(Array.contains path splits) then
    setEnvVar "PATH" (path + (string Path.PathSeparator) + currentPath)

let rootFolder = Path.Combine(__SOURCE_DIRECTORY__, "..", "..")
let compilerBinFolder = Path.Combine(rootFolder, releaseOrDebug, "net40", "bin")
setEnvVar "CSC_PIPE"      (Path.Combine(rootFolder, "packages", "Microsoft.Net.Compilers.2.7.0", "tools", "csc.exe"))
setEnvVar "FSC"           (Path.Combine(compilerBinFolder, "fsc.exe"))
setEnvVar "FSCOREDLLPATH" (Path.Combine(compilerBinFolder, "FSharp.Core.dll"))
addToPath compilerBinFolder

let runPerl arguments =
  // a bit expeditive, but does the deed
  Process.GetProcessesByName("perl") |> Array.iter (fun p -> p.Kill())
  use perlProcess = new Process()
  let userRoot = Env.SpecialFolder.UserProfile |> Env.GetFolderPath

  let perlExe = 
    [| userRoot
       ".nuget"
       "packages"
       "strawberryperl64"
       "5.22.2.1"
       "Tools"
       "perl"
       "bin"
       "perl.exe" |] 
    |> Path.Combine
    |> FileInfo
    
  if not perlExe.Exists then failwithf "couldn't find %s, please adjust this script" perlExe.FullName else

  perlProcess.StartInfo.set_FileName (perlExe.FullName)
  perlProcess.StartInfo.set_Arguments (arguments |> Array.map(fun a -> @"""" + a + @"""") |> String.concat " ")
  perlProcess.StartInfo.set_WorkingDirectory (Path.Combine(rootFolder, "tests", "fsharpqa", "source"))
  perlProcess.StartInfo.set_RedirectStandardOutput true
  perlProcess.StartInfo.set_RedirectStandardError true
  perlProcess.StartInfo.set_UseShellExecute false
  perlProcess.Start() |> ignore
  while (not perlProcess.StandardOutput.EndOfStream) do
    perlProcess.StandardOutput.ReadLine() |> printfn "%s" 
  while (not perlProcess.StandardError.EndOfStream) do
    perlProcess.StandardError.ReadLine() |> printfn "%s" 
  perlProcess.WaitForExit()
  if perlProcess.ExitCode <> 0 then
    failwithf "exit code: %i" perlProcess.ExitCode

let testResultDir = Path.Combine(rootFolder, "tests", "TestResults")
let perlScript = Path.Combine(rootFolder, "tests", "fsharpqa", "testenv", "bin", "runall.pl")
runPerl [|perlScript; "-resultsroot";testResultDir ;"-ttags:Conformance06"|]