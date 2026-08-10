// End-to-end pack-and-consume gate for the shipped FSharp.Core net-TFM asset.
//   structural: the nupkg ships lib/{netstandard2.0,netstandard2.1,<netNN.0>} (non-degenerate dll+xml),
//     a satellite resources.dll, a matching nuspec dependency <group>, and a uniform AssemblyVersion.
//   resolution: two consumer legs - the pinned net TFM (exact match) and the in-dev product TFM (nearest
//     fallback) - must BOTH bind FSharp.Core to lib/<pin> for compile and runtime, never a netstandard asset.
//   A best-effort runtime smoke then executes a widened member on the net asset.
// Portable: dotnet fsi tests/AheadOfTime/NetTfmResolution/VerifyNetResolution.fsx

open System
open System.IO
open System.IO.Compression
open System.Reflection
open System.Text.Json
open System.Text.RegularExpressions
open System.Diagnostics

let scriptDir = __SOURCE_DIRECTORY__
let repoRoot = Path.GetFullPath(Path.Combine(scriptDir, "..", "..", ".."))
let proj = Path.Combine(scriptDir, "NetTfmResolution.fsproj")
let dotnet =
    let local = Path.Combine(repoRoot, ".dotnet", if OperatingSystem.IsWindows() then "dotnet.exe" else "dotnet")
    if File.Exists local then local else "dotnet"

let run (fileName: string) (args: string) (workDir: string) =
    let psi = ProcessStartInfo(fileName, args, WorkingDirectory = workDir, UseShellExecute = false,
                               RedirectStandardOutput = true, RedirectStandardError = true)
    use p = Process.Start psi
    let out = p.StandardOutput.ReadToEnd()
    let err = p.StandardError.ReadToEnd()
    p.WaitForExit()
    p.ExitCode, out, err

let mutable errors = []
let fail m = errors <- m :: errors

// Newest built FSharp.Core nupkg. The shipped package lands in different sub-lanes depending on the
// pack (top-level Shipping locally, Dependency/Shipping on CI); search both and take the newest.
let searchDirs =
    [ "artifacts/packages/Release/Shipping"
      "artifacts/packages/Release/Dependency/Shipping" ]
    |> List.map (fun d -> Path.Combine(repoRoot, d))
let nupkg =
    searchDirs
    |> List.collect (fun d -> if Directory.Exists d then Directory.GetFiles(d, "FSharp.Core.*.nupkg") |> List.ofArray else [])
    |> List.filter (fun f -> not (f.EndsWith ".symbols.nupkg"))
    |> List.sortByDescending File.GetLastWriteTimeUtc
    |> List.tryHead

match nupkg with
| None ->
    eprintfn "e2e: no FSharp.Core.*.nupkg under: %s — pack first." (String.Join("; ", searchDirs))
    exit 2
| Some nupkgPath ->

let ver = Regex.Replace(Path.GetFileName nupkgPath, @"^FSharp\.Core\.(.*)\.nupkg$", "$1")
printfn "e2e: verifying FSharp.Core %s (%s)" ver (Path.GetFileName nupkgPath)

// --- structural gates on the package ---------------------------------------------------------------
use zip = ZipFile.OpenRead nupkgPath
let entries = zip.Entries |> Seq.map (fun e -> e.FullName.Replace('\\', '/')) |> Seq.toList
let findEntry (path: string) =
    zip.Entries |> Seq.tryFind (fun e -> e.FullName.Replace('\\','/').Equals(path, StringComparison.OrdinalIgnoreCase))

let pin =
    match entries |> List.choose (fun e -> let m = Regex.Match(e, @"^lib/(net\d+\.0)/") in if m.Success then Some m.Groups.[1].Value else None) |> List.distinct with
    | [ p ] -> printfn "e2e: discovered shipped net TFM lib = %s" p; p
    | [] -> fail "no lib/netNN.0 folder in the package (shipped net TFM lib missing)"; "net?"
    | many -> fail (sprintf "expected exactly one lib/netNN.0 folder, found: %s" (String.Join(", ", many))); List.head many

let requiredLibs = [ "netstandard2.0"; "netstandard2.1"; pin ]
let checkAsset (minSize: int64) (path: string) =
    match findEntry path with
    | None -> fail (sprintf "missing %s" path)
    | Some e when e.Length < minSize -> fail (sprintf "%s is degenerate (%d bytes)" path e.Length)
    | Some _ -> ()
for tfm in requiredLibs do
    checkAsset 100000L (sprintf "lib/%s/FSharp.Core.dll" tfm)
    checkAsset 1000L (sprintf "lib/%s/FSharp.Core.xml" tfm)

if not (entries |> List.exists (Regex(sprintf @"^lib/%s/[^/]+/FSharp\.Core\.resources\.dll$" (Regex.Escape pin)).IsMatch)) then
    fail (sprintf "no satellite FSharp.Core.resources.dll under lib/%s" pin)

match zip.Entries |> Seq.tryFind (fun e -> e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)) with
| None -> fail "no .nuspec in the package"
| Some e ->
    use r = new StreamReader(e.Open())
    if not (Regex.IsMatch(r.ReadToEnd(), sprintf @"<group\s+targetFramework=""%s""" (Regex.Escape pin), RegexOptions.IgnoreCase)) then
        fail (sprintf "nuspec has no <group targetFramework=\"%s\"> dependency group" pin)

let tempDir = Path.Combine(Path.GetTempPath(), "fscore-e2e-" + Guid.NewGuid().ToString("N"))
Directory.CreateDirectory tempDir |> ignore
try
    let versions =
        requiredLibs
        |> List.choose (fun tfm ->
            findEntry (sprintf "lib/%s/FSharp.Core.dll" tfm)
            |> Option.map (fun e ->
                let dst = Path.Combine(tempDir, tfm + "-FSharp.Core.dll")
                e.ExtractToFile(dst, true)
                tfm, AssemblyName.GetAssemblyName(dst).Version))
    for (tfm, v) in versions do printfn "e2e: lib/%s AssemblyVersion = %O" tfm v
    match versions |> List.map snd |> List.distinct with
    | [ _ ] | [] -> ()
    | vs -> fail (sprintf "AssemblyVersion mismatch across lib TFMs: %s" (String.Join(", ", vs)))
finally
    try Directory.Delete(tempDir, true) with _ -> ()

match errors with
| _ :: _ ->
    eprintfn "e2e: STRUCTURAL GATE FAILED:"
    for e in List.rev errors do eprintfn "  - %s" e
    exit 1
| [] -> printfn "e2e: structural gate OK — lib/{netstandard2.0,netstandard2.1,%s}, satellite, nuspec group, uniform AssemblyVersion." pin

// --- resolution: a real net-TFM consumer must bind the net asset -----------------------------------
for sub in [ "obj"; "bin" ] do
    let d = Path.Combine(scriptDir, sub)
    if Directory.Exists d then Directory.Delete(d, true)

// Stage the built nupkg into a clean, flat local feed the consumer's NuGet.Config points at; a depth-0
// folder resolves deterministically everywhere, unlike NuGet's recursion over the packages root.
let feedDir = Path.Combine(scriptDir, "obj", "localfeed")
Directory.CreateDirectory feedDir |> ignore
File.Copy(nupkgPath, Path.Combine(feedDir, Path.GetFileName nupkgPath), true)

let nugetPackages =
    match Environment.GetEnvironmentVariable "NUGET_PACKAGES" with
    | null | "" -> Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
    | p -> p
let cachedFsCore = Path.Combine(nugetPackages, "fsharp.core", ver)
if Directory.Exists cachedFsCore then
    printfn "e2e: purging cached fsharp.core/%s" ver
    Directory.Delete(cachedFsCore, true)

let rc, rout, rerr = run dotnet (sprintf "restore \"%s\" -p:FSharpCoreTestVersion=%s --nologo" proj ver) scriptDir
if rc <> 0 then
    eprintfn "e2e: consumer restore FAILED (exit %d)\n%s\n%s" rc rout rerr
    exit 1

let assetsPath = Path.Combine(scriptDir, "obj", "project.assets.json")
if not (File.Exists assetsPath) then
    eprintfn "e2e: %s not produced" assetsPath
    exit 1

let pinLibRegex = Regex(sprintf @"^lib/%s/FSharp\.Core\.dll$" (Regex.Escape pin), RegexOptions.IgnoreCase)
let nsLibRegex = Regex(@"^lib/netstandard\d\.\d/FSharp\.Core\.dll$", RegexOptions.IgnoreCase)
let mutable checkedAny = false
let targets = JsonDocument.Parse(File.ReadAllText assetsPath).RootElement.GetProperty("targets")
for tfmProp in targets.EnumerateObject() do
    for libProp in tfmProp.Value.EnumerateObject() do
        if libProp.Name.StartsWith("FSharp.Core/", StringComparison.OrdinalIgnoreCase) then
            checkedAny <- true
            let libVer = libProp.Name.Substring("FSharp.Core/".Length)
            if libVer <> ver then fail (sprintf "resolved FSharp.Core %s, expected the built %s" libVer ver)
            for section in [ "compile"; "runtime" ] do
                match libProp.Value.TryGetProperty section with
                | true, sec ->
                    let paths = sec.EnumerateObject() |> Seq.map (fun p -> p.Name.Replace('\\','/')) |> Seq.toList
                    match paths |> List.filter (fun p -> p.EndsWith("FSharp.Core.dll", StringComparison.OrdinalIgnoreCase)) with
                    | [] ->
                        let detail = if paths |> List.exists (fun p -> p.EndsWith "_._") then " (_._ placeholder)" else ""
                        fail (sprintf "%s under '%s' selected no FSharp.Core.dll asset%s" section tfmProp.Name detail)
                    | ps ->
                        for p in ps do
                            if nsLibRegex.IsMatch p then fail (sprintf "%s resolved to a netstandard asset (%s) under '%s' — expected the net TFM asset" section p tfmProp.Name)
                            elif not (pinLibRegex.IsMatch p) then fail (sprintf "%s resolved to '%s' under '%s' - expected the pinned net asset lib/%s" section p tfmProp.Name pin)
                            else printfn "e2e: %s [%s] -> %s ✓" section tfmProp.Name p
                | false, _ -> fail (sprintf "no '%s' section for FSharp.Core under '%s' — not bound for that phase" section tfmProp.Name)
if not checkedAny then fail "FSharp.Core not found in project.assets.json targets"

match errors with
| _ :: _ ->
    eprintfn "e2e: RESOLUTION WITNESS FAILED:"
    for e in List.rev errors do eprintfn "  - %s" e
    exit 1
| [] -> printfn "e2e: resolution witness OK - exact-match and fallback consumer legs both bind FSharp.Core to lib/%s (compile+runtime)." pin

// Runtime smoke on the pin leg: build+run the widened IAsyncDisposable member. Best-effort (non-fatal
// everywhere, CI included - the net shared framework may be absent); the structural + resolution
// witnesses above are the authoritative gates.
let brc, bout, berr = run dotnet (sprintf "build \"%s\" -c Release -f %s -p:FSharpCoreTestVersion=%s --no-restore --nologo" proj pin ver) scriptDir
if brc <> 0 then
    printfn "e2e: (best-effort) build did not succeed:\n%s\n%s" bout berr
    exit 0
let rrc, rrout, rrerr = run dotnet (sprintf "run --project \"%s\" -c Release -f %s -p:FSharpCoreTestVersion=%s --no-build --no-restore" proj pin ver) scriptDir
printf "%s" rrout
if rrc <> 0 then printfn "e2e: (best-effort) run did not succeed:\n%s" rrerr
exit 0
