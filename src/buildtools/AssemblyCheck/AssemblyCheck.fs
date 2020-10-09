// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Reflection.PortableExecutable
open System.Text.RegularExpressions

module AssemblyCheck =

    let private versionZero = Version(0, 0, 0, 0)
    let private versionOne = Version(1, 0, 0, 0)
    let private commitHashPattern = new Regex(@"Commit Hash: (<developer build>)|([0-9a-fA-F]{40})", RegexOptions.Compiled)
    let private devVersionPattern = new Regex(@"-(ci|dev)", RegexOptions.Compiled)

    let verifyEmbeddedPdb (filename:string)  =
        use fileStream = File.OpenRead(filename)
        let reader = new PEReader(fileStream)
        let mutable hasEmbeddedPdb = false

        try
            for entry in reader.ReadDebugDirectory() do
                match entry.Type with
                | DebugDirectoryEntryType.CodeView ->
                    let _ = reader.ReadCodeViewDebugDirectoryData(entry)
                    ()

                | DebugDirectoryEntryType.EmbeddedPortablePdb ->
                    let _ = reader.ReadEmbeddedPortablePdbDebugDirectoryData(entry)
                    hasEmbeddedPdb <- true
                    ()

                | DebugDirectoryEntryType.PdbChecksum ->
                    let _ = reader.ReadPdbChecksumDebugDirectoryData(entry)
                    ()

                | _ -> ()
        with | e -> printfn "Error validating assembly %s\nMessage: %s" filename (e.ToString())
        hasEmbeddedPdb

    let verifyAssemblies (binariesPath:string) =

        let excludedAssemblies =
            [ ] |> Set.ofList

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

        let fsharpExecutingWithEmbeddedPdbs =
            fsharpAssemblies
            |> List.filter (fun p -> not (p.Contains(@"\Proto\") || p.Contains(@"\Bootstrap\") || p.Contains(@".resources.") || p.Contains(@"\FSharpSdk\") || p.Contains(@"\tmp\") || p.Contains(@"\obj\")))

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
            |> List.filter (fun p -> not (p.Contains(@"\FSharpSdk\")))
            |> List.filter (fun a ->
                let fileProductVersion = FileVersionInfo.GetVersionInfo(a).ProductVersion
                not (commitHashPattern.IsMatch(fileProductVersion) || devVersionPattern.IsMatch(fileProductVersion)))

        if failedCommitHash.Length > 0 then
            printfn "The following assemblies don't have a commit hash set"
            printfn "%s\r\n" <| String.Join("\r\n", failedCommitHash)
        else
            printfn "All shipping assemblies had an appropriate commit hash."

        // verify that all assemblies have an embedded pdb
        let failedVerifyEmbeddedPdb =
            fsharpExecutingWithEmbeddedPdbs
            |> List.filter (fun a -> not (verifyEmbeddedPdb a))

        if failedVerifyEmbeddedPdb.Length > 0 then
            printfn "The following assemblies don't have an embedded pdb"
            printfn "%s\r\n" <| String.Join("\r\n", failedVerifyEmbeddedPdb)
        else
            printfn "All shipping assemblies had an embedded PDB."

        // return code is the number of failures
        failedVersionCheck.Length + failedCommitHash.Length + failedVerifyEmbeddedPdb.Length 


[<EntryPoint>]
let main (argv:string array) =
    if argv.Length <> 1 then
        printfn "Usage: dotnet AssemblyCheck.dll -- path/to/binaries"
        1
    else
        AssemblyCheck.verifyAssemblies argv.[0]
