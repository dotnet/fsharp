// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

module FSharp.Compiler.CodeAnalysis.ProjectSnapshot

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open FSharp.Compiler.IO
open Internal.Utilities.Library.Extras
open FSharp.Core.Printf
open FSharp.Compiler.Text

open Internal.Utilities.Collections
open System.Threading.Tasks
open Internal.Utilities.Hashing
open System.Collections.Immutable


[<AutoOpen>]
module internal Helpers =

    let isSignatureFile (fileName: string) =
        // TODO: is this robust enough?
        fileName[fileName.Length - 1] = 'i'


[<NoComparison; CustomEquality>]
type internal FSharpFileSnapshot =
    {
        FileName: string
        Version: string
        GetSource: unit -> Task<ISourceText>
    }

    member this.IsSignatureFile = this.FileName |> isSignatureFile

    override this.Equals(o) =
        match o with
        | :? FSharpFileSnapshot as o -> o.FileName = this.FileName && o.Version = this.Version
        | _ -> false

    override this.GetHashCode() = this.Key.GetHashCode()

    member this.Key = this :> ICacheKey<_, _>

    interface ICacheKey<string, string> with
        member this.GetLabel() = this.FileName |> shortPath
        member this.GetKey() = this.FileName
        member this.GetVersion() = this.Version

type FSharpFileSnapshotWithSource =
    {
        FileName: string
        SourceHash: ImmutableArray<byte>
        Source: ISourceText
        IsLastCompiland: bool
        IsExe: bool
    }

    member this.IsSignatureFile = this.FileName |> isSignatureFile

type ReferenceOnDisk =
    { Path: string; LastModified: DateTime }

type ProjectSnapshotKey = string * string

[<NoComparison>]
type internal ProjectCore(
    ProjectFileName: string,
    ProjectId: string option,
    ReferencesOnDisk: ReferenceOnDisk list,
    OtherOptions: string list,
    ReferencedProjects: FSharpReferencedProjectSnapshot list,
    IsIncompleteTypeCheckEnvironment: bool,
    UseScriptResolutionRules: bool,
    LoadTime: DateTime,
    UnresolvedReferences: FSharpUnresolvedReferencesSet option,
    OriginalLoadReferences: (range * string * string) list,
    Stamp: int64 option) =

    let hashForParsing = lazy (
        Md5Hasher.empty
        |> Md5Hasher.addString ProjectFileName
        |> Md5Hasher.addStrings OtherOptions
        |> Md5Hasher.addBool IsIncompleteTypeCheckEnvironment
        |> Md5Hasher.addBool UseScriptResolutionRules)

    let fullHash = lazy (
        hashForParsing.Value
        |> Md5Hasher.addStrings (ReferencesOnDisk |> Seq.map (fun r -> r.Path))
        |> Md5Hasher.addBytes' (
            ReferencedProjects
            |> Seq.map (fun (FSharpReference (_name, p)) -> p.SignatureVersion)
        ))

    let commandLineOptions = lazy (
        seq {
            for r in ReferencesOnDisk do
                $"-r:{r.Path}"

            yield! OtherOptions
        }
        |> Seq.toList)

    let outputFileName = lazy (
        OtherOptions
        |> List.tryFind (fun x -> x.StartsWith("-o:"))
        |> Option.map (fun x -> x.Substring(3)))

    let key = lazy (ProjectFileName, outputFileName.Value |> Option.defaultValue "")

    member val ProjectDirectory = Path.GetDirectoryName(ProjectFileName)
    member _.OutputFileName = outputFileName.Value
    member _.Key = key.Value

    member _.Version = fullHash.Value
    member _.VersionForParsing = hashForParsing.Value

    member _.CommandLineOptions = commandLineOptions.Value

    member _.ProjectFileName = ProjectFileName
    member _.ProjectId = ProjectId
    member _.ReferencesOnDisk = ReferencesOnDisk
    member _.OtherOptions = OtherOptions
    member _.ReferencedProjects = ReferencedProjects
    member _.IsIncompleteTypeCheckEnvironment = IsIncompleteTypeCheckEnvironment
    member _.UseScriptResolutionRules = UseScriptResolutionRules
    member _.LoadTime = LoadTime
    member _.UnresolvedReferences = UnresolvedReferences
    member _.OriginalLoadReferences = OriginalLoadReferences
    member _.Stamp = Stamp

and [<NoComparison>] internal FSharpProjectSnapshot private (projectCore: ProjectCore, sourceFiles: FSharpFileSnapshot list) =

    let noFileVersionsHash = lazy (
        projectCore.Version
        |> Md5Hasher.addStrings (sourceFiles |> Seq.map (fun x -> x.FileName)))

    let noFileVersionsKey = lazy (
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = projectCore.Key
            member this.GetVersion() = noFileVersionsHash.Value |> Md5Hasher.toString
        })

    let fullHash = lazy (
        projectCore.Version
        |> Md5Hasher.addStrings (sourceFiles |> Seq.collect (fun x -> seq { x.FileName; x.Version })))

    let fullKey = lazy (
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = projectCore.Key
            member this.GetVersion() = fullHash.Value |> Md5Hasher.toString
        })

    let addHash (file: FSharpFileSnapshot) hash =
        hash
        |> Md5Hasher.addString file.FileName
        |> Md5Hasher.addString file.Version

    let signatureHash = lazy (

        let mutable lastFile = ""

        ((projectCore.Version, Set.empty), sourceFiles)
        ||> Seq.fold (fun (res, sigs) file ->
            if file.IsSignatureFile then
                lastFile <- file.FileName
                res |> addHash file, sigs |> Set.add file.FileName
            else
                let sigFileName = $"{file.FileName}i"

                if sigs.Contains sigFileName then
                    res, sigs |> Set.remove sigFileName
                else
                    lastFile <- file.FileName
                    res |> addHash file, sigs)
        |> fst,
        lastFile)

    let signatureKey = lazy (
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = projectCore.Key
            member this.GetVersion() = signatureHash.Value |> fst |> Md5Hasher.toString
        })

    let lastFileHash = lazy (
        let lastFile = sourceFiles |> List.last
        let sigHash, f = signatureHash.Value
        (if f = lastFile.FileName then
            sigHash
        else
            sigHash |> addHash lastFile),
        lastFile)

    let lastFileKey = lazy (
        let hash, f = lastFileHash.Value
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = f.FileName, projectCore.Key
            member this.GetVersion() = hash |> Md5Hasher.toString
        })

    let sourceFileNames = lazy (sourceFiles |> List.map (fun x -> x.FileName))

    static member Create(
        projectFileName: string,
        projectId: string option,
        sourceFiles: FSharpFileSnapshot list,
        referencesOnDisk: ReferenceOnDisk list,
        otherOptions: string list,
        referencedProjects: FSharpReferencedProjectSnapshot list,
        isIncompleteTypeCheckEnvironment: bool,
        useScriptResolutionRules: bool,
        loadTime: DateTime,
        unresolvedReferences: FSharpUnresolvedReferencesSet option,
        originalLoadReferences: (range * string * string) list,
        stamp: int64 option) =

        let projectCore = ProjectCore(
            projectFileName,
            projectId,
            referencesOnDisk,
            otherOptions,
            referencedProjects,
            isIncompleteTypeCheckEnvironment,
            useScriptResolutionRules,
            loadTime,
            unresolvedReferences,
            originalLoadReferences,
            stamp)
        FSharpProjectSnapshot(projectCore, sourceFiles)

    member _.ProjectFileName = projectCore.ProjectFileName
    member _.ProjectId = projectCore.ProjectId
    member _.ReferencesOnDisk = projectCore.ReferencesOnDisk
    member _.OtherOptions = projectCore.OtherOptions
    member _.ReferencedProjects = projectCore.ReferencedProjects
    member _.IsIncompleteTypeCheckEnvironment = projectCore.IsIncompleteTypeCheckEnvironment
    member _.UseScriptResolutionRules = projectCore.UseScriptResolutionRules
    member _.LoadTime = projectCore.LoadTime
    member _.UnresolvedReferences = projectCore.UnresolvedReferences
    member _.OriginalLoadReferences = projectCore.OriginalLoadReferences
    member _.Stamp = projectCore.Stamp
    member _.CommandLineOptions = projectCore.CommandLineOptions
    member _.ProjectDirectory = projectCore.ProjectDirectory

    member _.OutputFileName = projectCore.OutputFileName

    member _.ProjectCore = projectCore

    member _.SourceFiles = sourceFiles

    member _.SourceFileNames = sourceFileNames.Value

    member _.IndexOf fileName =
        sourceFiles
        |> List.tryFindIndex (fun x -> x.FileName = fileName)
        |> Option.defaultWith (fun () -> failwith (sprintf "Unable to find file %s in project %s" fileName projectCore.ProjectFileName))

    member private _.With(sourceFiles: FSharpFileSnapshot list) = FSharpProjectSnapshot(projectCore, sourceFiles)

    member this.UpTo fileIndex =
        this.With sourceFiles[..fileIndex]

    member this.UpTo fileName = this.UpTo(this.IndexOf fileName)

    member this.OnlyWith fileIndexes =
        this.With(
                fileIndexes
                |> Set.toList
                |> List.sort
                |> List.choose (fun x -> sourceFiles |> List.tryItem x))

    member this.WithoutImplFilesThatHaveSignatures =
        let files =
            (([], Set.empty), sourceFiles)
            ||> Seq.fold (fun (res, sigs) file ->
                if file.IsSignatureFile then
                    file :: res, sigs |> Set.add file.FileName
                else
                    let sigFileName = $"{file.FileName}i"

                    if sigs.Contains sigFileName then
                        res, sigs |> Set.remove sigFileName
                    else
                        file :: res, sigs)
            |> fst
            |> List.rev

        this.With files

    member this.WithoutImplFilesThatHaveSignaturesExceptLastOne =
        let lastFile = sourceFiles |> List.last

        let snapshotWithoutImplFilesThatHaveSignatures =
            this.WithoutImplFilesThatHaveSignatures

        if
            lastFile.IsSignatureFile
            || snapshotWithoutImplFilesThatHaveSignatures.SourceFiles |> List.last = lastFile
        then
            this.WithoutImplFilesThatHaveSignatures
        else
            snapshotWithoutImplFilesThatHaveSignatures.With(
                 snapshotWithoutImplFilesThatHaveSignatures.SourceFiles @ [ lastFile ])


    member this.WithoutFileVersions =
        this.With(sourceFiles |> List.map (fun x -> { x with Version = "" }))

    member this.WithoutSourceFiles = this.With []

    override this.ToString() =
        Path.GetFileNameWithoutExtension this.ProjectFileName
        |> sprintf "FSharpProjectSnapshot(%s)"

    member this.GetLastModifiedTimeOnDisk() =
        // TODO:
        DateTime.Now

    member this.GetDebugVersion() : FSharpProjectSnapshotDebugVersion =
        {
            ProjectFileName = this.ProjectFileName
            SourceFiles = [ for f in this.SourceFiles -> f.FileName, f.Version ]
            ReferencesOnDisk = this.ReferencesOnDisk
            OtherOptions = this.OtherOptions
            ReferencedProjects =
                [
                    for FSharpReference(_, p) in this.ReferencedProjects -> p.WithoutImplFilesThatHaveSignatures.GetDebugVersion()
                ]
            IsIncompleteTypeCheckEnvironment = this.IsIncompleteTypeCheckEnvironment
            UseScriptResolutionRules = this.UseScriptResolutionRules
        }

    member this.FullVersion = fullHash.Value
    member this.SignatureVersion = signatureHash.Value |> fst
    member this.LastFileVersion = lastFileHash.Value |> fst

    /// Version for parsing - doesn't include any references because they don't affect parsing (...right?)
    member this.ParsingVersion = projectCore.VersionForParsing |> Md5Hasher.toString

    /// A key for this snapshot but without file versions. So it will be the same across any in-file changes.
    member this.NoFileVersionsKey = noFileVersionsKey.Value

    /// A full key for this snapshot, any change will cause this to change.
    member this.FullKey = fullKey.Value

    /// A key including the public surface or signature for this snapshot
    member this.SignatureKey = signatureKey.Value

    /// A key including the public surface or signature for this snapshot and the last file (even if it's not a signature file)
    member this.LastFileKey = lastFileKey.Value

    //TODO: cache it here?
    member this.FileKey(fileName: string) = this.UpTo(fileName).LastFileKey


and internal FSharpProjectSnapshotWithSources(
        projectSnapshot: FSharpProjectSnapshot,
        sourceFiles: FSharpFileSnapshotWithSource list
    ) =

    let projectCore = projectSnapshot.ProjectCore

    let hashedVersion = lazy (
        projectCore.Version
        |> Md5Hasher.addStrings (sourceFiles |> Seq.map (fun x -> x.FileName))
        |> Md5Hasher.addBytes' (sourceFiles |> Seq.map (fun x -> x.SourceHash.ToBuilder().ToArray())))

    let fullKey = lazy (
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = projectCore.Key
            member this.GetVersion() = hashedVersion.Value |> Md5Hasher.toString
        })

    let addHash (file: FSharpFileSnapshotWithSource) hash =
        hash
        |> Md5Hasher.addString file.FileName
        |> Md5Hasher.addBytes <| file.SourceHash.ToBuilder().ToArray()

    let signatureHash = lazy (

        let mutable lastFile = ""

        ((projectCore.Version, Set.empty), sourceFiles)
        ||> Seq.fold (fun (res, sigs) file ->
            if file.IsSignatureFile then
                lastFile <- file.FileName
                res |> addHash file, sigs |> Set.add file.FileName
            else
                let sigFileName = $"{file.FileName}i"

                if sigs.Contains sigFileName then
                    res, sigs |> Set.remove sigFileName
                else
                    lastFile <- file.FileName
                    res |> addHash file, sigs)
        |> fst,
        lastFile)

    let signatureKey = lazy (
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = projectCore.Key
            member this.GetVersion() = signatureHash.Value |> fst |> Md5Hasher.toString
        })

    let lastFileHash = lazy (
        let lastFile = sourceFiles |> List.last
        let sigHash, f = signatureHash.Value
        (if f = lastFile.FileName then
            sigHash
        else
            sigHash |> addHash lastFile),
        lastFile)

    let lastFileKey = lazy (
        let hash, f = lastFileHash.Value
        { new ICacheKey<_, _> with
            member this.GetLabel() = this.ToString()
            member this.GetKey() = f.FileName, projectCore.Key
            member this.GetVersion() = hash |> Md5Hasher.toString
        })

    member _.ProjectSnapshot = projectSnapshot
    member _.SourceFiles = sourceFiles

    member this.IndexOf fileName =
        this.SourceFiles
        |> List.tryFindIndex (fun x -> x.FileName = fileName)
        |> Option.defaultWith (fun () ->
            failwith (sprintf "Unable to find file %s in project %s" fileName this.ProjectSnapshot.ProjectFileName))

    member private _.With(sourceFiles: FSharpFileSnapshotWithSource list) = FSharpProjectSnapshotWithSources(projectSnapshot, sourceFiles)

    member this.UpTo(fileIndex: FileIndex) = this.With sourceFiles[..fileIndex]

    member this.UpTo fileName = this.UpTo(this.IndexOf fileName)

    member this.WithoutImplFilesThatHaveSignatures =
        let files =
            (([], Set.empty), this.SourceFiles)
            ||> Seq.fold (fun (res, sigs) file ->
                if file.IsSignatureFile then
                    file :: res, sigs |> Set.add file.FileName
                else
                    let sigFileName = $"{file.FileName}i"

                    if sigs.Contains sigFileName then
                        res, sigs |> Set.remove sigFileName
                    else
                        file :: res, sigs)
            |> fst
            |> List.rev

        this.With files

    member this.WithoutImplFilesThatHaveSignaturesExceptLastOne =
        let lastFile = this.SourceFiles |> List.last

        let snapshotWithoutImplFilesThatHaveSignatures =
            this.WithoutImplFilesThatHaveSignatures

        if
            lastFile.IsSignatureFile
            || snapshotWithoutImplFilesThatHaveSignatures.SourceFiles |> List.last = lastFile
        then
            this.WithoutImplFilesThatHaveSignatures
        else
            this.With( snapshotWithoutImplFilesThatHaveSignatures.SourceFiles @ [ lastFile ])

    member this.GetDebugVersion() =
        {
            ProjectSnapshotVersion = this.ProjectSnapshot.WithoutFileVersions.GetDebugVersion()
            SourceVersions = this.SourceFiles |> List.map (fun x -> x.SourceHash.ToBuilder().ToArray() |> BitConverter.ToString)
        }

    member this.FullKey = fullKey.Value
    member this.SignatureKey = signatureKey.Value
    member this.LastFileKey = lastFileKey.Value
    member this.FileKey(fileName: string) = this.UpTo(fileName).LastFileKey
    

and FSharpProjectSnapshotWithSourcesDebugVersion =
    {
        ProjectSnapshotVersion: FSharpProjectSnapshotDebugVersion
        SourceVersions: string list
    }

and FSharpProjectSnapshotWithSourcesVersion = FSharpProjectSnapshotWithSourcesDebugVersion

and FSharpProjectSnapshotDebugVersion =
    {
        ProjectFileName: string
        SourceFiles: (string * string) list
        ReferencesOnDisk: ReferenceOnDisk list
        OtherOptions: string list
        ReferencedProjects: FSharpProjectSnapshotDebugVersion list
        IsIncompleteTypeCheckEnvironment: bool
        UseScriptResolutionRules: bool
    }

and FSharpProjectSnapshotVersion = string

and [<NoComparison; CustomEquality>] internal FSharpReferencedProjectSnapshot =
    internal
    | FSharpReference of projectOutputFile: string * options: FSharpProjectSnapshot
    //| PEReference of projectOutputFile: string * getStamp: (unit -> DateTime) * delayedReader: DelayedILModuleReader
    //| ILModuleReference of
    //    projectOutputFile: string *
    //    getStamp: (unit -> DateTime) *
    //    getReader: (unit -> ILModuleReader)

    /// <summary>
    /// The fully qualified path to the output of the referenced project. This should be the same value as the <c>-r</c>
    /// reference in the project options for this referenced project.
    /// </summary>
    member this.OutputFile =
        match this with
        | FSharpReference(projectOutputFile, _) -> projectOutputFile

    /// <summary>
    /// Creates a reference for an F# project. The physical data for it is stored/cached inside of the compiler service.
    /// </summary>
    /// <param name="projectOutputFile">The fully qualified path to the output of the referenced project. This should be the same value as the <c>-r</c> reference in the project options for this referenced project.</param>
    /// <param name="options">The Project Options for this F# project</param>
    static member CreateFSharp(projectOutputFile, options: FSharpProjectSnapshot) =
        FSharpReference(projectOutputFile, options)

    override this.Equals(o) =
        match o with
        | :? FSharpReferencedProjectSnapshot as o ->
            match this, o with
            | FSharpReference(projectOutputFile1, options1), FSharpReference(projectOutputFile2, options2) ->
                projectOutputFile1 = projectOutputFile2 && options1 = options2

        | _ -> false

    override this.GetHashCode() = this.OutputFile.GetHashCode()

type FSharpProjectSnapshot with

    member this.ToOptions() : FSharpProjectOptions =
        {
            ProjectFileName = this.ProjectFileName
            ProjectId = this.ProjectId
            SourceFiles = this.SourceFiles |> Seq.map (fun x -> x.FileName) |> Seq.toArray
            OtherOptions = this.CommandLineOptions |> List.toArray
            ReferencedProjects =
                this.ReferencedProjects
                |> Seq.map (function
                    | FSharpReference(name, opts) -> FSharpReferencedProject.FSharpReference(name, opts.ToOptions()))
                |> Seq.toArray
            IsIncompleteTypeCheckEnvironment = this.IsIncompleteTypeCheckEnvironment
            UseScriptResolutionRules = this.UseScriptResolutionRules
            LoadTime = this.LoadTime
            UnresolvedReferences = this.UnresolvedReferences
            OriginalLoadReferences = this.OriginalLoadReferences
            Stamp = this.Stamp
        }

    static member FromOptions(options: FSharpProjectOptions, getFileSnapshot, ?snapshotAccumulator) =
        let snapshotAccumulator = defaultArg snapshotAccumulator (Dictionary())

        async {

            // TODO: check if options is a good key here
            if not (snapshotAccumulator.ContainsKey options) then

                let! sourceFiles = options.SourceFiles |> Seq.map (getFileSnapshot options) |> Async.Parallel

                let! referencedProjects =
                    options.ReferencedProjects
                    |> Seq.choose (function
                        | FSharpReferencedProject.FSharpReference(outputName, options) ->
                            Some(
                                async {
                                    let! snapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot, snapshotAccumulator)
                                    return FSharpReferencedProjectSnapshot.FSharpReference(outputName, snapshot)
                                }
                            )
                        // TODO: other types
                        | _ -> None)
                    |> Async.Sequential

                let referencesOnDisk, otherOptions =
                    options.OtherOptions
                    |> Array.partition (fun x -> x.StartsWith("-r:"))
                    |> map1Of2 (
                        Array.map (fun x ->
                            let path = x.Substring(3)

                            {
                                Path = path
                                LastModified = FileSystem.GetLastWriteTimeShim(path)
                            })
                    )

                let snapshot =
                    FSharpProjectSnapshot.Create(
                        projectFileName = options.ProjectFileName,
                        projectId = options.ProjectId,
                        sourceFiles = (sourceFiles |> List.ofArray),
                        referencesOnDisk = (referencesOnDisk |> List.ofArray),
                        otherOptions = (otherOptions |> List.ofArray),
                        referencedProjects = (referencedProjects |> List.ofArray),
                        isIncompleteTypeCheckEnvironment = options.IsIncompleteTypeCheckEnvironment,
                        useScriptResolutionRules = options.UseScriptResolutionRules,
                        loadTime = options.LoadTime,
                        unresolvedReferences = options.UnresolvedReferences,
                        originalLoadReferences = options.OriginalLoadReferences,
                        stamp = options.Stamp
                    )

                snapshotAccumulator.Add(options, snapshot)

            return snapshotAccumulator[options]
        }

    static member GetFileSnapshotFromDisk _ fileName =
        async {
            let timeStamp = FileSystem.GetLastWriteTimeShim(fileName)
            let contents = FileSystem.OpenFileForReadShim(fileName).ReadAllText()

            return
                {
                    FileName = fileName
                    Version = timeStamp.Ticks.ToString()
                    GetSource = fun () -> Task.FromResult(SourceText.ofString contents)
                }
        }

    static member FromOptions(options: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(options, FSharpProjectSnapshot.GetFileSnapshotFromDisk)

    static member FromOptions(options: FSharpProjectOptions, fileName: string, fileVersion: int, sourceText: ISourceText) =

        let getFileSnapshot _ fName =
            if fName = fileName then
                async.Return
                    {
                        FileName = fileName
                        GetSource = fun () -> Task.FromResult sourceText
                        Version = $"{fileVersion}{sourceText.GetHashCode().ToString()}"
                    }
            else
                FSharpProjectSnapshot.GetFileSnapshotFromDisk () fName

        FSharpProjectSnapshot.FromOptions(options, getFileSnapshot)