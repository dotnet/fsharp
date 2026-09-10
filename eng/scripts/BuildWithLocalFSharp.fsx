// Build an unmodified repo with this checkout's F# compiler and FSharp.Core, on any OS (needs only the .NET SDK).
//   dotnet fsi <fsharp-repo>/eng/scripts/BuildWithLocalFSharp.fsx --build-script "dotnet build MySolution.sln"
// Prerequisite: build this checkout with `-c Release -pack`.

open System
open System.IO
open System.Diagnostics

let fail (msg: string) : 'a = eprintfn "ERROR: %s" msg; exit 1

let opts = System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)

let rec parseArgs = function
    | (key: string) :: value :: rest when key.StartsWith "--" && not (value.StartsWith "--") ->
        opts.[key.Substring 2] <- value
        parseArgs rest
    | key :: rest when key.StartsWith "--" ->
        opts.[key.Substring 2] <- "true"
        parseArgs rest
    | _ :: rest -> parseArgs rest
    | [] -> ()

fsi.CommandLineArgs |> Array.tail |> Array.toList |> parseArgs

let tryOpt k = match opts.TryGetValue k with | true, v -> Some v | _ -> None
let opt k d = defaultArg (tryOpt k) d

let fsharpRoot = opt "fsharp-root" (Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..")))
let configuration = opt "configuration" "Release"
let compilerPath = opt "compiler-path" fsharpRoot
let props = opt "props" (Path.Combine(fsharpRoot, "UseLocalCompiler.Directory.Build.props"))
let targets = opt "targets" (Path.Combine(fsharpRoot, "UseLocalCompiler.Directory.Build.targets"))
let corePackagesDir = opt "core-packages-dir" (Path.Combine(compilerPath, "artifacts", "packages", configuration))
let repoDir = opt "repo-dir" (Directory.GetCurrentDirectory())
let buildScript = match tryOpt "build-script" with Some s -> s | None -> fail "--build-script is required"
let verify = (tryOpt "verify").IsSome

if not (File.Exists props) then fail (sprintf "props file not found: %s" props)
if not (File.Exists targets) then fail (sprintf "targets file not found: %s" targets)
if not (Directory.Exists corePackagesDir) then
    fail (sprintf "FSharp.Core package folder not found: %s (build the compiler with `-c %s -pack`)" corePackagesDir configuration)

let nupkg =
    // Arcade routes FSharp.Core to a `Shipping` leaf that varies by layout (Release/Shipping locally,
    // Dependency/Shipping on CI), so search recursively and prefer that folder, then newest.
    Directory.GetFiles(corePackagesDir, "FSharp.Core.*.nupkg", SearchOption.AllDirectories)
    |> Array.filter (fun f -> not (f.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase)))
    |> Array.sortByDescending (fun f -> Path.GetFileName(Path.GetDirectoryName f) = "Shipping", File.GetLastWriteTimeUtc f)
    |> Array.tryHead
    |> Option.defaultWith (fun () -> fail (sprintf "no FSharp.Core.*.nupkg under %s" corePackagesDir))

let version = Path.GetFileNameWithoutExtension(nupkg).Substring("FSharp.Core.".Length)

let setEnv k v = Environment.SetEnvironmentVariable(k, v)
setEnv "LoadLocalFSharpBuild" "True"
setEnv "LocalFSharpCompilerPath" compilerPath
setEnv "LocalFSharpCompilerConfiguration" configuration
setEnv "CustomAfterDirectoryBuildProps" props
setEnv "CustomAfterDirectoryBuildTargets" targets
setEnv "RegressionLocalCore" "true"
setEnv "RegressionLocalCoreVersion" version
setEnv "RegressionLocalCorePackagesDir" corePackagesDir
tryOpt "nuget-packages" |> Option.iter (setEnv "NUGET_PACKAGES")

// NuGet caches by id+version, so a repacked same-version local FSharp.Core would be served stale; evict it first.
let globalPackages =
    match Environment.GetEnvironmentVariable "NUGET_PACKAGES" with
    | null | "" -> Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
    | p -> p
let cachedCore = Path.Combine(globalPackages, "fsharp.core", version)
if Directory.Exists cachedCore then
    try Directory.Delete(cachedCore, true)
    with e -> eprintfn "WARN: could not evict cached %s: %s" cachedCore e.Message

printfn "Local F# compiler: %s (%s)" compilerPath configuration
printfn "Local FSharp.Core:  %s from %s" version corePackagesDir

let run (command: string) =
    let psi = ProcessStartInfo(WorkingDirectory = repoDir, UseShellExecute = false)
    let launch =
        if OperatingSystem.IsWindows() then
            psi.FileName <- "cmd.exe"
            psi.ArgumentList.Add "/c"
            if command.StartsWith("dotnet", StringComparison.OrdinalIgnoreCase) then command else ".\\" + command
        else
            psi.FileName <- "/bin/bash"
            psi.ArgumentList.Add "-c"
            // Escape bare ';' so MSBuild's `-t:Build;Test` stays one argument, and run non-dotnet scripts
            // through bash instead of `chmod +x` so the checked-out repo is never modified.
            let escaped = command.Replace(";", "\\;")
            if command.StartsWith("dotnet", StringComparison.OrdinalIgnoreCase) then escaped else "bash " + escaped
    psi.ArgumentList.Add launch
    printfn "==> %s" command
    use p = Process.Start psi
    p.WaitForExit()
    p.ExitCode

for cmd in buildScript.Split([| ";;" |], StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries) do
    let code = run cmd
    if code <> 0 then fail (sprintf "build command failed with exit code %d" code)

if verify then
    // Fail if any project resolved a non-local FSharp.Core; match the exact quoted identity so a longer
    // prerelease can't satisfy a prefix.
    let rx = System.Text.RegularExpressions.Regex("\"FSharp\\.Core/([^\"]+)\"")
    let options = EnumerationOptions(RecurseSubdirectories = true, IgnoreInaccessible = true)
    let mutable usedLocal = false
    let others = System.Collections.Generic.SortedSet<string>()
    for f in Directory.EnumerateFiles(repoDir, "project.assets.json", options) do
        let text = try File.ReadAllText f with _ -> ""
        for m in rx.Matches text do
            if m.Groups.[1].Value = version then usedLocal <- true
            else others.Add m.Groups.[1].Value |> ignore
    if others.Count > 0 then
        fail (sprintf "expected local FSharp.Core %s but some projects resolved: %s" version (String.Join(", ", others)))
    if not usedLocal then
        fail (sprintf "expected local FSharp.Core %s in project.assets.json but found none; built against a different FSharp.Core" version)
    printfn "Verified: local FSharp.Core %s was consumed." version
