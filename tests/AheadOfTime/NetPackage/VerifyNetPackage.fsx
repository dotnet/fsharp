// Structural gate on the shipped FSharp.Core package: the pack must ship a non-degenerate lib/<pin>
// (dll + xml) alongside the netstandard legs, a satellite, and a matching nuspec dependency <group>.
// Portable: dotnet fsi tests/AheadOfTime/NetPackage/VerifyNetPackage.fsx

open System
open System.IO
open System.IO.Compression
open System.Text.RegularExpressions

let repoRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", ".."))
let mutable errors = []
let fail m = errors <- m :: errors

// The pack lands in different sub-lanes (top-level Shipping locally, Dependency/Shipping on CI); take the newest.
let nupkg =
    [ "artifacts/packages/Release/Shipping"; "artifacts/packages/Release/Dependency/Shipping" ]
    |> List.collect (fun d -> let d = Path.Combine(repoRoot, d) in if Directory.Exists d then List.ofArray (Directory.GetFiles(d, "FSharp.Core.*.nupkg")) else [])
    |> List.filter (fun f -> not (f.EndsWith ".symbols.nupkg"))
    |> List.sortByDescending File.GetLastWriteTimeUtc
    |> List.tryHead

match nupkg with
| None -> eprintfn "verify: no FSharp.Core.*.nupkg under artifacts/packages/Release — pack first."; exit 2
| Some nupkgPath ->

printfn "verify: %s" (Path.GetFileName nupkgPath)
use zip = ZipFile.OpenRead nupkgPath
let entries = zip.Entries |> Seq.map (fun e -> e.FullName.Replace('\\', '/')) |> Seq.toList
let sizeOf (path: string) =
    zip.Entries |> Seq.tryFind (fun e -> e.FullName.Replace('\\', '/').Equals(path, StringComparison.OrdinalIgnoreCase)) |> Option.map (fun e -> e.Length)

let pin =
    match entries |> List.choose (fun e -> let m = Regex.Match(e, @"^lib/(net\d+\.0)/") in if m.Success then Some m.Groups.[1].Value else None) |> List.distinct with
    | [ p ] -> printfn "verify: shipped net TFM lib = %s" p; p
    | [] -> fail "no lib/netNN.0 folder (shipped net TFM lib missing)"; "net?"
    | many -> fail (sprintf "expected exactly one lib/netNN.0 folder, found: %s" (String.Join(", ", many))); List.head many

for tfm in [ "netstandard2.0"; "netstandard2.1"; pin ] do
    match sizeOf (sprintf "lib/%s/FSharp.Core.dll" tfm) with
    | Some n when n >= 100000L -> ()
    | Some n -> fail (sprintf "lib/%s/FSharp.Core.dll is degenerate (%d bytes)" tfm n)
    | None -> fail (sprintf "missing lib/%s/FSharp.Core.dll" tfm)
    match sizeOf (sprintf "lib/%s/FSharp.Core.xml" tfm) with
    | Some n when n >= 1000L -> ()
    | _ -> fail (sprintf "missing or degenerate lib/%s/FSharp.Core.xml" tfm)

if not (entries |> List.exists (Regex(sprintf @"^lib/%s/[^/]+/FSharp\.Core\.resources\.dll$" (Regex.Escape pin)).IsMatch)) then
    fail (sprintf "no satellite FSharp.Core.resources.dll under lib/%s" pin)

match zip.Entries |> Seq.tryFind (fun e -> e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)) with
| None -> fail "no .nuspec in the package"
| Some e ->
    use r = new StreamReader(e.Open())
    if not (Regex.IsMatch(r.ReadToEnd(), sprintf @"<group\s+targetFramework=""%s""" (Regex.Escape pin), RegexOptions.IgnoreCase)) then
        fail (sprintf "nuspec has no <group targetFramework=\"%s\"> dependency group" pin)

match errors with
| [] -> printfn "verify: OK — lib/{netstandard2.0,netstandard2.1,%s} (dll+xml), satellite, nuspec group." pin
| es ->
    eprintfn "verify: FSharp.Core PACKAGE GATE FAILED:"
    for e in List.rev es do eprintfn "  - %s" e
    exit 1
