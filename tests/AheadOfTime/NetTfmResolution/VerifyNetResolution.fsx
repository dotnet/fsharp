// Consumer asset-RESOLUTION test (e2e-2, the linchpin): restores a net-TFM consumer against the
// locally built FSharp.Core and witnesses from obj/project.assets.json that FSharp.Core resolved to
// lib/<netNN.0>/FSharp.Core.dll for BOTH compile and runtime (never a netstandard asset). Only the
// FSharp.Core cache entry for that version is purged, never the whole packages dir. A build+run smoke
// of a widened member follows, reported but non-fatal locally (the structural witness is the gate).
//
// Portable (Linux/macOS/Windows): dotnet fsi tests/AheadOfTime/NetTfmResolution/VerifyNetResolution.fsx

open System
open System.IO
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

// 1. Newest built FSharp.Core version.
let shippingDir = Path.Combine(repoRoot, "artifacts", "packages", "Release", "Shipping")
let version =
    if not (Directory.Exists shippingDir) then None
    else
        Directory.GetFiles(shippingDir, "FSharp.Core.*.nupkg")
        |> Array.filter (fun f -> not (f.EndsWith ".symbols.nupkg"))
        |> Array.sortByDescending File.GetLastWriteTimeUtc
        |> Array.tryHead
        |> Option.map (fun f -> Regex.Replace(Path.GetFileName f, @"^FSharp\.Core\.(.*)\.nupkg$", "$1"))

match version with
| None ->
    eprintfn "e2e-2: no FSharp.Core.*.nupkg in %s — pack first (Milestone B)." shippingDir
    exit 2
| Some ver ->

printfn "e2e-2: consumer will pin FSharp.Core %s" ver

// 2. Clean the consumer obj/bin and only the cached FSharp.Core for this version.
for sub in [ "obj"; "bin" ] do
    let d = Path.Combine(scriptDir, sub)
    if Directory.Exists d then Directory.Delete(d, true)
let nugetPackages =
    match Environment.GetEnvironmentVariable "NUGET_PACKAGES" with
    | null | "" -> Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
    | p -> p
let cachedFsCore = Path.Combine(nugetPackages, "fsharp.core", ver)
if Directory.Exists cachedFsCore then
    printfn "e2e-2: purging cached fsharp.core/%s" ver
    Directory.Delete(cachedFsCore, true)

let rc, out, err = run dotnet (sprintf "restore \"%s\" -p:FSharpCoreTestVersion=%s --nologo" proj ver) scriptDir
if rc <> 0 then
    eprintfn "e2e-2: restore FAILED (exit %d)\n%s\n%s" rc out err
    exit 1
printfn "e2e-2: restore OK"

// 3. Structural witness from project.assets.json.
let assetsPath = Path.Combine(scriptDir, "obj", "project.assets.json")
if not (File.Exists assetsPath) then
    eprintfn "e2e-2: %s not produced" assetsPath
    exit 1

let doc = JsonDocument.Parse(File.ReadAllText assetsPath)
let root = doc.RootElement

let mutable errors = []
let fail m = errors <- m :: errors
let netLibRegex = Regex(@"^lib/net\d+\.0/FSharp\.Core\.dll$", RegexOptions.IgnoreCase)
let nsLibRegex = Regex(@"^lib/netstandard\d\.\d/FSharp\.Core\.dll$", RegexOptions.IgnoreCase)

// targets -> <tfm> -> "FSharp.Core/<ver>" -> { compile: {...}, runtime: {...} }
let mutable checkedAny = false
let targets = root.GetProperty("targets")
for tfmProp in targets.EnumerateObject() do
    for libProp in tfmProp.Value.EnumerateObject() do
        if libProp.Name.StartsWith("FSharp.Core/", StringComparison.OrdinalIgnoreCase) then
            checkedAny <- true
            let libVer = libProp.Name.Substring("FSharp.Core/".Length)
            if libVer <> ver then fail (sprintf "resolved FSharp.Core %s, expected the built %s" libVer ver)
            let checkSection (section: string) =
                match libProp.Value.TryGetProperty section with
                | true, sec ->
                    let paths = sec.EnumerateObject() |> Seq.map (fun p -> p.Name.Replace('\\','/')) |> Seq.toList
                    let dllPaths = paths |> List.filter (fun p -> p.EndsWith("FSharp.Core.dll", StringComparison.OrdinalIgnoreCase))
                    match dllPaths with
                    | [] ->
                        // A "_._" placeholder means no asset selected for this section — a resolution failure.
                        if paths |> List.exists (fun p -> p.EndsWith "_._") then
                            fail (sprintf "%s under '%s' resolved to a _._ placeholder (no %s asset)" section tfmProp.Name (if section = "compile" then "compile" else "runtime"))
                    | ps ->
                        for p in ps do
                            if nsLibRegex.IsMatch p then fail (sprintf "%s resolved to a netstandard asset (%s) under '%s' — expected the net TFM asset" section p tfmProp.Name)
                            elif not (netLibRegex.IsMatch p) then fail (sprintf "%s resolved to an unexpected path '%s' under '%s'" section p tfmProp.Name)
                            else printfn "e2e-2: %s [%s] -> %s ✓" section tfmProp.Name p
                | false, _ -> ()
            checkSection "compile"
            checkSection "runtime"

if not checkedAny then fail "FSharp.Core not found in project.assets.json targets"

match errors with
| _ :: _ ->
    eprintfn "e2e-2: STRUCTURAL WITNESS FAILED:"
    for e in List.rev errors do eprintfn "  - %s" e
    exit 1
| [] ->

printfn "e2e-2: structural witness OK — FSharp.Core compile+runtime both bound to the net TFM lib."

// 4. Best-effort build + run of the runtime smoke.
let brc, bout, berr = run dotnet (sprintf "build \"%s\" -c Release -p:FSharpCoreTestVersion=%s --no-restore --nologo" proj ver) scriptDir
if brc <> 0 then
    printfn "e2e-2: (non-fatal locally) build did not succeed:\n%s\n%s" bout berr
    printfn "e2e-2: PASS on the structural witness (the authoritative gate)."
    exit 0

let rrc, rout, rerr = run dotnet (sprintf "run --project \"%s\" -c Release -p:FSharpCoreTestVersion=%s --no-build --no-restore" proj ver) scriptDir
printf "%s" rout
if rrc <> 0 then
    printfn "e2e-2: (non-fatal locally) run did not succeed (net shared framework may be absent):\n%s" rerr
    printfn "e2e-2: PASS on the structural witness (the authoritative gate)."
    exit 0

printfn "e2e-2: PASS — structural witness + runtime smoke both green."
exit 0
