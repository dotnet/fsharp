// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Text.RegularExpressions

module AssemblyVersionCheck =

    let private versionZero = Version(0, 0, 0, 0)
    let private versionOne = Version(1, 0, 0, 0)
    let private commitHashPattern = new Regex(@"Commit Hash: (<developer build>)|([0-9a-fA-F]{40})", RegexOptions.Compiled)
    let private devVersionPattern = new Regex(@"-(ci|dev)", RegexOptions.Compiled)

    let verifyAssemblyVersions (binariesPath:string) =
        let excludedAssemblies =
            [ "FSharp.Data.TypeProviders.dll" ]
            |> Set.ofList
        let fsharpAssemblies =
            [ "FSharp*.dll"
              "fsc.exe"
              "fsc.dll"
              "fsi*.exe"
              "fsi*.dll" ]
            |> List.map (fun p -> Directory.EnumerateFiles(binariesPath, p, SearchOption.AllDirectories))
            |> Seq.concat
            |> List.ofSeq
            |> List.filter (fun p -> (Set.contains (Path.GetFileName(p)) excludedAssemblies) |> not)

        // verify that all assemblies have a version number other than 0.0.0.0 or 1.0.0.0
        let failedVersionCheck =
            fsharpAssemblies
            |> List.filter (fun a ->
                let assemblyVersion = AssemblyName.GetAssemblyName(a).Version
                assemblyVersion = versionZero || assemblyVersion = versionOne)
        if failedVersionCheck.Length > 0 then
            printfn "The following assemblies had a version of %A or %A" versionZero versionOne
            printfn "%s\r\n" <| String.Join("\r\n", failedVersionCheck)
        else
            printfn "All shipping assemblies had an appropriate version number."

        // verify that all assemblies have a commit hash
        let failedCommitHash =
            fsharpAssemblies
            |> List.filter (fun a ->
                let fileProductVersion = FileVersionInfo.GetVersionInfo(a).ProductVersion
                not (commitHashPattern.IsMatch(fileProductVersion) || devVersionPattern.IsMatch(fileProductVersion)))
        if failedCommitHash.Length > 0 then
            printfn "The following assemblies don't have a commit hash set"
            printfn "%s\r\n" <| String.Join("\r\n", failedCommitHash)
        else
            printfn "All shipping assemblies had an appropriate commit hash."

        // return code is the number of failures
        failedVersionCheck.Length + failedCommitHash.Length

let main (argv:string array) =
    if argv.Length <> 1 then
        printfn "Usage: fsi.exe AssemblyVersionCheck.fsx -- path/to/binaries"
        1
    else
        AssemblyVersionCheck.verifyAssemblyVersions argv.[0]

Environment.GetCommandLineArgs()
|> Seq.skipWhile ((<>) "--")
|> Seq.skip 1
|> Array.ofSeq
|> main
