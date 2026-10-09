#!/usr/bin/env dotnet fsi
#r "System.Text.Json"
#r "System.Collections.Immutable"
open System
open System.IO
open System.Collections.Generic
open System.Collections.Immutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Text.Json

let usage () =
    printfn "Usage: dotnet fsi compare_roslyn.fsx <scenario> <metadata-path>"
    printfn "Scenario names match tools/baselines/roslyn_tables.json (e.g., AsyncUpdate, PropertyUpdate)."

let args =
    Environment.GetCommandLineArgs()
    |> Array.skip 2

if args.Length <> 2 then
    usage ()
    Environment.Exit 1

let scenario = args[0]
let metadataPath = Path.GetFullPath args[1]
if not (File.Exists metadataPath) then
    eprintfn "error: metadata file not found: %s" metadataPath
    Environment.Exit 2

let baselineJsonPath =
    Path.Combine(__SOURCE_DIRECTORY__, "../../../tools/baselines/roslyn_tables.json")
    |> Path.GetFullPath
if not (File.Exists baselineJsonPath) then
    eprintfn "error: roslyn baseline file not found: %s" baselineJsonPath
    Environment.Exit 3

let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
let baselines =
    let json = File.ReadAllText baselineJsonPath
    JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json, options)

let scenarioTables =
    match baselines.TryGetValue scenario with
    | true, table -> table
    | _ ->
        eprintfn "error: scenario '%s' not found in roslyn_tables.json" scenario
        eprintfn "available scenarios: %s" (String.Join(", ", baselines.Keys))
        Environment.Exit 4
        Unchecked.defaultof<_>

use provider =
    MetadataReaderProvider.FromMetadataImage(
        ImmutableArray.CreateRange<byte>(File.ReadAllBytes metadataPath))
let reader = provider.GetMetadataReader()
let actualCounts =
    [ for i in 0 .. MetadataTokens.TableCount - 1 do
          let table = LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte i)
          let count = reader.GetTableRowCount table
          yield table, count ]
    |> dict

let tryParseTable (name: string) =
    match Enum.TryParse<TableIndex>(name, true) with
    | true, value -> Some value
    | _ -> None

printfn "Comparing scenario '%s' to metadata '%s'\n" scenario metadataPath
printfn "Table                    Roslyn   Actual   Status"
printfn "-----------------------------------------------"
for kvp in scenarioTables do
    let tableName = kvp.Key
    let baselineCount = kvp.Value
    match tryParseTable tableName with
    | Some table ->
        let actualCount = actualCounts[table]
        let status =
            if actualCount = baselineCount then "OK"
            elif actualCount > baselineCount then "EXTRA"
            else "MISSING"
        printfn "%-24s %6d %8d   %s" tableName baselineCount actualCount status
    | None ->
        printfn "%-24s %6d %8s   %s" tableName baselineCount "?" "UNKNOWN"

let extraTables =
    actualCounts
    |> Seq.choose (fun (KeyValue(table, count)) ->
        if count > 0 && not (scenarioTables.ContainsKey(table.ToString())) then
            Some(table, count)
        else
            None)
    |> Seq.toList

if not (List.isEmpty extraTables) then
    printfn "\nTables present in metadata but not in Roslyn baseline:"
    for (table, count) in extraTables do
        printfn "  %-20A %d" table count

printfn "\nDone."
