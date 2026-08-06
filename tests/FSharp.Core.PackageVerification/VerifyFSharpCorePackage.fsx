// End-to-end verification of the SHIPPED FSharp.Core NuGet package (e2e-1).
//
// Asserts that the produced FSharp.Core.*.nupkg:
//   * contains lib/<pin>/FSharp.Core.dll + .xml (non-degenerate) for the shipped net TFM, where
//     <pin> is DISCOVERED from the package (a lib/netNN.0 folder), never hard-coded to a literal;
//   * still contains lib/netstandard2.0 and lib/netstandard2.1 (no regression);
//   * carries at least one satellite **/FSharp.Core.resources.dll under lib/<pin>;
//   * declares a <group targetFramework="netNN.0"> dependency group in the .nuspec;
//   * has the SAME AssemblyVersion across all three lib assemblies (TFM must not fork identity).
//
// Portable: pure .NET (System.IO.Compression + Reflection), runnable on Linux/macOS/Windows in CI.
// Usage: dotnet fsi tests/FSharp.Core.PackageVerification/VerifyFSharpCorePackage.fsx [<packagesDir>]

open System
open System.IO
open System.IO.Compression
open System.Reflection
open System.Text.RegularExpressions

let repoRoot =
    let here = __SOURCE_DIRECTORY__
    Path.GetFullPath(Path.Combine(here, "..", ".."))

let searchDirs =
    match fsi.CommandLineArgs |> Array.tryItem 1 with
    | Some d -> [ d ]
    | None ->
        // Prefer the shipping lane, then the embedded dependency lanes.
        [ "artifacts/packages/Release/Shipping"
          "artifacts/packages/Release/Dependency/Shipping"
          "artifacts/packages/Debug/Shipping" ]
        |> List.map (fun d -> Path.Combine(repoRoot, d))

let mutable errors = []
let fail msg = errors <- msg :: errors
let netTfmRegex = Regex(@"^lib/(net\d+\.0)/", RegexOptions.Compiled)

let findPackage () =
    searchDirs
    |> List.collect (fun d -> if Directory.Exists d then Directory.GetFiles(d, "FSharp.Core.*.nupkg") |> List.ofArray else [])
    |> List.filter (fun f -> not (f.EndsWith(".symbols.nupkg")))
    |> List.sortByDescending File.GetLastWriteTimeUtc
    |> List.tryHead

match findPackage () with
| None ->
    eprintfn "e2e-1: no FSharp.Core.*.nupkg found under: %s" (String.Join("; ", searchDirs))
    eprintfn "       Pack first, e.g.: dotnet msbuild src/FSharp.Core/FSharp.Core.fsproj -t:Pack -p:Configuration=Release -p:DISABLE_ARCADE=false -p:Restore=true"
    exit 2
| Some pkg ->

printfn "e2e-1: verifying %s" (Path.GetFileName pkg)
use zip = ZipFile.OpenRead pkg
let entries = zip.Entries |> Seq.map (fun e -> e.FullName.Replace('\\', '/')) |> Seq.toList

// Discover the shipped net TFM folder from the package itself.
let discoveredNetTfms =
    entries
    |> List.choose (fun e -> let m = netTfmRegex.Match e in if m.Success then Some m.Groups.[1].Value else None)
    |> List.distinct

match discoveredNetTfms with
| [] -> fail "no lib/netNN.0 folder found in the package (the shipped net TFM lib is missing)"
| [ pin ] -> printfn "e2e-1: discovered shipped net TFM lib = %s" pin
| many -> fail (sprintf "expected exactly one lib/netNN.0 folder, found: %s" (String.Join(", ", many)))

let pin = discoveredNetTfms |> List.tryHead |> Option.defaultValue "net?"

let entryExists (path: string) = entries |> List.exists (fun e -> e.Equals(path, StringComparison.OrdinalIgnoreCase))
let entrySize (path: string) =
    zip.Entries |> Seq.tryFind (fun e -> e.FullName.Replace('\\','/').Equals(path, StringComparison.OrdinalIgnoreCase))
    |> Option.map (fun e -> e.Length) |> Option.defaultValue 0L

// Required lib layout.
let requiredLibs = [ "netstandard2.0"; "netstandard2.1"; pin ]
for tfm in requiredLibs do
    let dll = sprintf "lib/%s/FSharp.Core.dll" tfm
    if not (entryExists dll) then fail (sprintf "missing %s" dll)
    elif entrySize dll < 100000L then fail (sprintf "%s is degenerate (%d bytes)" dll (entrySize dll))
    let xml = sprintf "lib/%s/FSharp.Core.xml" tfm
    if not (entryExists xml) then fail (sprintf "missing %s" xml)
    elif entrySize xml < 1000L then fail (sprintf "%s is degenerate (%d bytes)" xml (entrySize xml))

// Satellite resources for the shipped net TFM.
let satellitePattern = Regex(sprintf @"^lib/%s/[^/]+/FSharp\.Core\.resources\.dll$" (Regex.Escape pin))
if not (entries |> List.exists satellitePattern.IsMatch) then
    fail (sprintf "no satellite **/FSharp.Core.resources.dll under lib/%s" pin)

// Nuspec dependency group for the shipped net TFM.
let nuspecEntry = zip.Entries |> Seq.tryFind (fun e -> e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
match nuspecEntry with
| None -> fail "no .nuspec in the package"
| Some e ->
    use r = new StreamReader(e.Open())
    let nuspec = r.ReadToEnd()
    let groupRegex = Regex(sprintf @"<group\s+targetFramework=""%s""" (Regex.Escape pin), RegexOptions.IgnoreCase)
    if not (groupRegex.IsMatch nuspec) then
        fail (sprintf "nuspec has no <group targetFramework=\"%s\"> dependency group" pin)

// AssemblyVersion equality across all three lib assemblies.
let tempDir = Path.Combine(Path.GetTempPath(), "fscore-e2e1-" + Guid.NewGuid().ToString("N"))
Directory.CreateDirectory tempDir |> ignore
try
    let versions =
        requiredLibs
        |> List.choose (fun tfm ->
            let src = sprintf "lib/%s/FSharp.Core.dll" tfm
            match zip.Entries |> Seq.tryFind (fun e -> e.FullName.Replace('\\','/').Equals(src, StringComparison.OrdinalIgnoreCase)) with
            | None -> None
            | Some e ->
                let dst = Path.Combine(tempDir, tfm + "-FSharp.Core.dll")
                e.ExtractToFile(dst, true)
                Some (tfm, AssemblyName.GetAssemblyName(dst).Version))
    for (tfm, v) in versions do printfn "e2e-1: lib/%s AssemblyVersion = %O" tfm v
    match versions |> List.map snd |> List.distinct with
    | [ _ ] -> ()
    | vs -> fail (sprintf "AssemblyVersion mismatch across lib TFMs: %s" (String.Join(", ", vs)))
finally
    try Directory.Delete(tempDir, true) with _ -> ()

match errors with
| [] ->
    printfn "e2e-1: OK — lib/{netstandard2.0,netstandard2.1,%s} present, satellites + nuspec group present, AssemblyVersion uniform." pin
    exit 0
| es ->
    eprintfn "e2e-1: FAILED:"
    for e in List.rev es do eprintfn "  - %s" e
    exit 1
