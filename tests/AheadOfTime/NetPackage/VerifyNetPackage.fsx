open System
open System.IO
open System.IO.Compression
open System.Text.RegularExpressions

let fail message =
    eprintfn "Package verification failed: %s" message
    exit 1

let requireExactlyOne description items =
    match Array.ofSeq items with
    | [| item |] -> item
    | items -> fail $"{description}: expected exactly one, found {items.Length}"

let packagesRoot =
    match fsi.CommandLineArgs with
    | [| _; path |] -> Path.GetFullPath path
    | _ -> fail "expected the artifacts/packages directory"

let packagePath =
    Directory.EnumerateFiles(packagesRoot, "FSharp.Core.*.nupkg", SearchOption.AllDirectories)
    |> Seq.filter (fun path ->
        DirectoryInfo(Path.GetDirectoryName path).Name = "Shipping"
        && not (Path.GetFileName(path).EndsWith(".symbols.nupkg", StringComparison.Ordinal)))
    |> requireExactlyOne "FSharp.Core package"

let package = ZipFile.OpenRead packagePath

let entries =
    package.Entries
    |> Seq.map (fun entry -> entry.FullName, entry)
    |> Map.ofSeq

let netTfm =
    entries.Keys
    |> Seq.choose (fun path ->
        let matched = Regex.Match(path, "^lib/(net\\d+\\.0)/FSharp\\.Core\\.dll$")
        if matched.Success then Some matched.Groups[1].Value else None)
    |> Seq.distinct
    |> requireExactlyOne "versioned net target"

let requireNonEmpty path =
    match entries.TryFind path with
    | Some entry when entry.Length > 0L -> ()
    | Some _ -> fail $"{path} is empty"
    | None -> fail $"{path} is missing"

for tfm in [ "netstandard2.0"; "netstandard2.1"; netTfm ] do
    requireNonEmpty $"lib/{tfm}/FSharp.Core.dll"
    requireNonEmpty $"lib/{tfm}/FSharp.Core.xml"

let satellites tfm =
    entries
    |> Seq.choose (fun (KeyValue(path, entry)) ->
        let matched = Regex.Match(path, $"^lib/{Regex.Escape tfm}/([^/]+)/FSharp\\.Core\\.resources\\.dll$")
        if matched.Success then Some(matched.Groups[1].Value, entry) else None)
    |> Map.ofSeq

let referenceSatellites = satellites "netstandard2.1"
let netSatellites = satellites netTfm

if referenceSatellites.IsEmpty || Set.ofSeq referenceSatellites.Keys <> Set.ofSeq netSatellites.Keys then
    fail $"{netTfm} satellite assemblies do not match netstandard2.1"

netSatellites.Values
|> Seq.iter (fun entry -> if entry.Length = 0L then fail $"{entry.FullName} is empty")

package.Dispose()
printfn "Verified %s with %s assets." (Path.GetFileName packagePath) netTfm
