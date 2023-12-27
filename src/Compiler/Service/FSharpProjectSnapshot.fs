// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

module internal FSharp.Compiler.CodeAnalysis.ProjectSnapshot

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
open System.Runtime.CompilerServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger

type IFileSnapshot =
    abstract member FileName: string
    abstract member Version: byte array
    abstract member IsSignatureFile: bool

[<AutoOpen>]
module internal Helpers =

    let isSignatureFile (fileName: string) =
        // TODO: is this robust enough?
        fileName[fileName.Length - 1] = 'i'

    let addFileName (file: IFileSnapshot) = Md5Hasher.addString file.FileName

    let addFileNameAndVersion (file: IFileSnapshot) =
        addFileName file >> Md5Hasher.addBytes file.Version

    let signatureHash projectCoreVersion (sourceFiles: IFileSnapshot seq) =
        let mutable lastFile = ""

        ((projectCoreVersion, Set.empty), sourceFiles)
        ||> Seq.fold (fun (res, sigs) file ->
            if file.IsSignatureFile then
                lastFile <- file.FileName
                res |> addFileNameAndVersion file, sigs |> Set.add file.FileName
            else
                let sigFileName = $"{file.FileName}i"

                if sigs.Contains sigFileName then
                    res |> addFileName file, sigs |> Set.remove sigFileName
                else
                    lastFile <- file.FileName
                    res |> addFileNameAndVersion file, sigs)
        |> fst,
        lastFile

type FSharpFileSnapshot(FileName: string, Version: string, GetSource: unit -> Task<ISourceTextNew>) =

    static member Create(fileName: string, version: string, getSource: unit -> Task<ISourceTextNew>) =
        FSharpFileSnapshot(fileName, version, getSource)

    static member CreateFromFileSystem(fileName: string) =
        FSharpFileSnapshot(
            fileName,
            FileSystem.GetLastWriteTimeShim(fileName).Ticks.ToString(),
            fun () -> fileName |> File.ReadAllText |> SourceTextNew.ofString |> Task.FromResult
        )

    member public _.FileName = FileName
    member _.Version = Version
    member _.GetSource() = GetSource()

    member val IsSignatureFile = FileName |> isSignatureFile

    member _.GetFileName() = FileName

    override this.Equals(o) =
        match o with
        | :? FSharpFileSnapshot as o -> o.FileName = this.FileName && o.Version = this.Version
        | _ -> false

    override this.GetHashCode() =
        this.FileName.GetHashCode() + this.Version.GetHashCode()

    interface IFileSnapshot with
        member this.FileName = this.FileName
        member this.Version = this.Version |> System.Text.Encoding.UTF8.GetBytes
        member this.IsSignatureFile = this.IsSignatureFile

type FSharpFileSnapshotWithSource
    (FileName: string, SourceHash: ImmutableArray<byte>, Source: ISourceText, IsLastCompiland: bool, IsExe: bool) =

    let version = lazy (SourceHash.ToBuilder().ToArray())
    let stringVersion = lazy (version.Value |> BitConverter.ToString)

    member val Version = version.Value
    member val StringVersion = stringVersion.Value
    member val IsSignatureFile = FileName |> isSignatureFile

    member _.FileName = FileName
    member _.Source = Source
    member _.IsLastCompiland = IsLastCompiland
    member _.IsExe = IsExe

    interface IFileSnapshot with
        member this.FileName = this.FileName
        member this.Version = this.Version
        member this.IsSignatureFile = this.IsSignatureFile

type FSharpParsedFile
    (
        FileName: string,
        SyntaxTreeHash: byte array,
        SourceText: ISourceText,
        ParsedInput: ParsedInput,
        ParseErrors: (PhasedDiagnostic * FSharpDiagnosticSeverity)[]
    ) =

    member _.FileName = FileName
    member _.SourceText = SourceText
    member _.ParsedInput = ParsedInput
    member _.ParseErrors = ParseErrors

    member val IsSignatureFile = FileName |> isSignatureFile

    interface IFileSnapshot with
        member this.FileName = this.FileName
        member this.Version = SyntaxTreeHash
        member this.IsSignatureFile = this.IsSignatureFile

type ReferenceOnDisk =
    { Path: string; LastModified: DateTime }

type ProjectSnapshotKey = string * string

type ProjectSnapshotBase<'T when 'T :> IFileSnapshot>(projectCore: ProjectCore, sourceFiles: 'T list) =

    let noFileVersionsHash =
        lazy
            (projectCore.Version
             |> Md5Hasher.addStrings (sourceFiles |> Seq.map (fun x -> x.FileName)))

    let noFileVersionsKey =
        lazy
            ({ new ICacheKey<_, _> with
                 member this.GetLabel() = projectCore.Label
                 member this.GetKey() = projectCore.Key

                 member this.GetVersion() =
                     noFileVersionsHash.Value |> Md5Hasher.toString
             })

    let fullHash =
        lazy
            (projectCore.Version
             |> Md5Hasher.addStrings (
                 sourceFiles
                 |> Seq.collect (fun x ->
                     seq {
                         x.FileName
                         x.Version |> Md5Hasher.toString
                     })
             ))

    let fullKey =
        lazy
            ({ new ICacheKey<_, _> with
                 member this.GetLabel() = projectCore.Label
                 member this.GetKey() = projectCore.Key
                 member this.GetVersion() = fullHash.Value |> Md5Hasher.toString
             })

    let addHash (file: 'T) hash =
        hash |> Md5Hasher.addString file.FileName |> Md5Hasher.addBytes file.Version

    let signatureHash =
        lazy (signatureHash projectCore.Version (sourceFiles |> Seq.map (fun x -> x :> IFileSnapshot)))

    let signatureKey =
        lazy (projectCore.CacheKeyWith("Signature", signatureHash.Value |> fst |> Md5Hasher.toString))

    let lastFileHash =
        lazy
            (let lastFile = sourceFiles |> List.last
             let sigHash, f = signatureHash.Value

             (if f = lastFile.FileName then
                  sigHash
              else
                  sigHash |> Md5Hasher.addBytes lastFile.Version),
             lastFile)

    let lastFileKey =
        lazy
            (let hash, f = lastFileHash.Value

             { new ICacheKey<_, _> with
                 member _.GetLabel() = $"{f.FileName} ({projectCore.Label})"
                 member _.GetKey() = f.FileName, projectCore.Key
                 member _.GetVersion() = hash |> Md5Hasher.toString
             })

    let sourceFileNames = lazy (sourceFiles |> List.map (fun x -> x.FileName))

    member _.ProjectFileName = projectCore.ProjectFileName
    member _.ProjectId = projectCore.ProjectId
    member _.ReferencesOnDisk = projectCore.ReferencesOnDisk
    member _.OtherOptions = projectCore.OtherOptions
    member _.ReferencedProjects = projectCore.ReferencedProjects

    member _.IsIncompleteTypeCheckEnvironment =
        projectCore.IsIncompleteTypeCheckEnvironment

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

    member _.Label = projectCore.Label

    member _.IndexOf fileName =
        sourceFiles
        |> List.tryFindIndex (fun x -> x.FileName = fileName)
        |> Option.defaultWith (fun () -> failwith (sprintf "Unable to find file %s in project %s" fileName projectCore.ProjectFileName))

    member private _.With(sourceFiles: 'T list) =
        ProjectSnapshotBase(projectCore, sourceFiles)

    member this.UpTo fileIndex = this.With sourceFiles[..fileIndex]

    member this.UpTo fileName = this.UpTo(this.IndexOf fileName)

    member this.OnlyWith fileIndexes =
        this.With(
            fileIndexes
            |> Set.toList
            |> List.sort
            |> List.choose (fun x -> sourceFiles |> List.tryItem x)
        )

    member this.WithoutSourceFiles = this.With []

    override this.ToString() =
        Path.GetFileNameWithoutExtension this.ProjectFileName
        |> sprintf "FSharpProjectSnapshot(%s)"

    member this.GetLastModifiedTimeOnDisk() =
        // TODO:
        DateTime.Now

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
    member this.FileKey(index: FileIndex) = this.UpTo(index).LastFileKey

and ProjectSnapshot = ProjectSnapshotBase<FSharpFileSnapshot>
and ProjectSnapshotWithSources = ProjectSnapshotBase<FSharpFileSnapshotWithSource>

and ProjectCore
    (
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
        Stamp: int64 option
    ) as self =

    let hashForParsing =
        lazy
            (Md5Hasher.empty
             |> Md5Hasher.addString ProjectFileName
             |> Md5Hasher.addStrings OtherOptions
             |> Md5Hasher.addBool IsIncompleteTypeCheckEnvironment
             |> Md5Hasher.addBool UseScriptResolutionRules)

    let fullHash =
        lazy
            (hashForParsing.Value
             |> Md5Hasher.addStrings (ReferencesOnDisk |> Seq.map (fun r -> r.Path))
             |> Md5Hasher.addBytes' (
                 ReferencedProjects
                 |> Seq.map (fun (FSharpReference(_name, p)) -> p.ProjectSnapshot.SignatureVersion)
             ))

    let fullHashString = lazy (fullHash.Value |> Md5Hasher.toString)

    let commandLineOptions =
        lazy
            (seq {
                for r in ReferencesOnDisk do
                    $"-r:{r.Path}"

                yield! OtherOptions
             }
             |> Seq.toList)

    let outputFileName =
        lazy
            (OtherOptions
             |> List.tryFind (fun x -> x.StartsWith("-o:"))
             |> Option.map (fun x -> x.Substring(3)))

    let key = lazy (ProjectFileName, outputFileName.Value |> Option.defaultValue "")

    let cacheKey =
        lazy
            ({ new ICacheKey<_, _> with
                 member _.GetLabel() = self.Label
                 member _.GetKey() = self.Key
                 member _.GetVersion() = fullHashString.Value
             })

    member val ProjectDirectory = Path.GetDirectoryName(ProjectFileName)
    member _.OutputFileName = outputFileName.Value
    member _.Key = key.Value
    member _.Version = fullHash.Value
    member _.Label = ProjectFileName |> shortPath
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

    member _.CacheKeyWith(label, version) =
        { new ICacheKey<_, _> with
            member _.GetLabel() = $"{label} ({self.Label})"
            member _.GetKey() = self.Key
            member _.GetVersion() = fullHashString.Value, version
        }

    member _.CacheKeyWith(label, key, version) =
        { new ICacheKey<_, _> with
            member _.GetLabel() = $"{label} ({self.Label})"
            member _.GetKey() = key, self.Key
            member _.GetVersion() = fullHashString.Value, version
        }

    member _.CacheKey = cacheKey.Value

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


and FSharpProjectSnapshot internal(projectSnapshot) =

    member internal _.ProjectSnapshot: ProjectSnapshot = projectSnapshot

    static member Create
        (
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
            stamp: int64 option
        ) =

        let projectCore =
            ProjectCore(
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
                stamp
            )

        ProjectSnapshotBase(projectCore, sourceFiles) |> FSharpProjectSnapshot

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
                                    let! snapshot =
                                        FSharpProjectSnapshot.FromOptions(options, getFileSnapshot, snapshotAccumulator)

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
                FSharpFileSnapshot.Create(
                    fileName,
                    timeStamp.Ticks.ToString(),
                    (fun () -> Task.FromResult(SourceTextNew.ofString contents))
                )
        }

    static member FromOptions(options: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(options, FSharpProjectSnapshot.GetFileSnapshotFromDisk)

    static member FromOptions(options: FSharpProjectOptions, fileName: string, fileVersion: int, sourceText: ISourceText) =

        let getFileSnapshot _ fName =
            if fName = fileName then
                async.Return(
                    FSharpFileSnapshot.Create(
                        fileName,
                        $"{fileVersion}{sourceText.GetHashCode().ToString()}",
                        fun () -> Task.FromResult(SourceTextNew.ofISourceText sourceText)
                    )
                )
            else
                FSharpProjectSnapshot.GetFileSnapshotFromDisk () fName

        FSharpProjectSnapshot.FromOptions(options, getFileSnapshot)

let rec snapshotToOptions (projectSnapshot: ProjectSnapshotBase<_>) =
    {
        ProjectFileName = projectSnapshot.ProjectFileName
        ProjectId = projectSnapshot.ProjectId
        SourceFiles = projectSnapshot.SourceFiles |> Seq.map (fun x -> x.FileName) |> Seq.toArray
        OtherOptions = projectSnapshot.CommandLineOptions |> List.toArray
        ReferencedProjects =
            projectSnapshot.ReferencedProjects
            |> Seq.map (function
                | FSharpReference(name, opts) -> FSharpReferencedProject.FSharpReference(name, opts.ProjectSnapshot |> snapshotToOptions))
            |> Seq.toArray
        IsIncompleteTypeCheckEnvironment = projectSnapshot.IsIncompleteTypeCheckEnvironment
        UseScriptResolutionRules = projectSnapshot.UseScriptResolutionRules
        LoadTime = projectSnapshot.LoadTime
        UnresolvedReferences = projectSnapshot.UnresolvedReferences
        OriginalLoadReferences = projectSnapshot.OriginalLoadReferences
        Stamp = projectSnapshot.Stamp
    }


[<Extension>]
type Extensions =

    [<Extension>]
    static member ToOptions(this: ProjectSnapshot) =
        this |> snapshotToOptions

    [<Extension>]
    static member ToOptions(this: FSharpProjectSnapshot) =
        this.ProjectSnapshot |> snapshotToOptions