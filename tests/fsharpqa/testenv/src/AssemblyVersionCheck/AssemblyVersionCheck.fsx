// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// this points to assemblies that were restored by packages.config in the root
#load "assemblies.fsx"

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Text.RegularExpressions
open Newtonsoft.Json.Linq

module AssemblyVersionCheck =

    let private versionZero = Version(0, 0, 0, 0)
    let private commitHashPattern = new Regex(@"Commit Hash: (<developer build>)|([0-9a-fA-F]{40})", RegexOptions.Compiled)

    let verifyAssemblyVersions (signToolData:string) (binariesPath:string) =
        let json = File.ReadAllText(signToolData)
        let jobject = JObject.Parse(json)

        // could either contain things like 'net40\bin\FSharp.Core.dll' or patterns like 'net40\bin\*\FSharp.Core.resources.dll'
        let assemblyPatterns =
            (jobject.["sign"] :?> JArray)
            |> Seq.map (fun a -> (a :?> JObject).["values"] :?> JArray)
            |> Seq.map (fun a -> a :> seq<JToken>)
            |> Seq.collect (fun t -> t)
            |> Seq.map (fun t -> t.ToString())
            |> Seq.filter (fun p -> p.EndsWith(".dll") || p.EndsWith(".exe")) // only check assemblies

        // map the assembly patterns to actual files on disk
        let actualAssemblies =
            assemblyPatterns
            |> Seq.map (fun a ->
                if not (a.Contains("*")) then
                    [a] // just a raw file name
                else
                    let parts = a.Split([|'\\'|])
                    let mutable candidatePaths = [binariesPath]
                    for p in parts do
                        match p with
                        | "*" ->
                            // expand all candidates into multiples
                            let expansions =
                                candidatePaths
                                |> List.filter Directory.Exists
                                |> List.map (Directory.EnumerateDirectories >> Seq.toList)
                                |> List.collect (fun x -> x)
                            candidatePaths <- expansions
                        | _ ->
                            // regular path part, just append it to all candidates
                            candidatePaths <- List.map (fun d -> Path.Combine(d, p)) candidatePaths
                    candidatePaths)
            |> Seq.collect (fun a -> a)
            |> Seq.map (fun a -> Path.Combine(binariesPath, a))
            |> Seq.filter (fun p -> File.Exists(p)) // not all test runs produce all files
            |> Seq.toList

        // verify that all assemblies have a version number other than 0.0.0.0
        let failedVersionCheck =
            actualAssemblies
            |> List.filter (fun a ->
                let assemblyVersion = AssemblyName.GetAssemblyName(a).Version
                printfn "Checking version: %s (%A)" a assemblyVersion
                assemblyVersion = versionZero)
        if failedVersionCheck.Length > 0 then
            printfn "The following assemblies had a version of %A" versionZero
            printfn "%s\r\n" <| String.Join("\r\n", failedVersionCheck)

        // verify that all assemblies have a commit hash
        let failedCommitHash =
            actualAssemblies
            |> List.filter (fun a ->
                let fileProductVersion = FileVersionInfo.GetVersionInfo(a).ProductVersion
                printfn "Checking commit hash: %s (%s)" a fileProductVersion
                not <| commitHashPattern.IsMatch(fileProductVersion))
        if failedCommitHash.Length > 0 then
            printfn "The following assemblies don't have a commit hash set"
            printfn "%s\r\n" <| String.Join("\r\n", failedCommitHash)

        // return code is the number of failures
        failedVersionCheck.Length + failedCommitHash.Length

let main (argv:string array) =
    if argv.Length <> 2 then
        printfn "Usage: fsi.exe AssemblyVersionCheck.fsx -- SignToolData.json path/to/binaries"
        1
    else
        AssemblyVersionCheck.verifyAssemblyVersions argv.[0] argv.[1]

Environment.GetCommandLineArgs()
|> Seq.skipWhile ((<>) "--")
|> Seq.skip 1
|> Array.ofSeq
|> main
