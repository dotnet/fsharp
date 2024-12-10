// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.CodeAnalysis.ProjectSnapshot

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open FSharp.Compiler.IO
open Internal.Utilities.Library
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

type internal ProjectIdentifier = string * string

/// A common interface for an F# source file snapshot that can be used across all stages (lazy, source loaded, parsed)
type internal IFileSnapshot =
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

    let signatureHash projectBaseVersion (sourceFiles: IFileSnapshot seq) =
        let mutable lastFile = ""

        ((projectBaseVersion, Set.empty), sourceFiles)
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

    let findOutputFileName options =
        options
        |> Seq.tryFind (fun (x: string) -> x.StartsWith("-o:"))
        |> Option.map (fun x -> x.Substring(3))

/// A snapshot of an F# source file.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpFileSnapshot(FileName: string, Version: string, GetSource: unit -> Task<ISourceTextNew>) =

    static member Create(fileName: string, version: string, getSource: unit -> Task<ISourceTextNew>) =
        FSharpFileSnapshot(fileName, version, getSource)

    static member CreateFromString(filename: string, content: string) =
        FSharpFileSnapshot(
            filename,
            Md5Hasher.hashString content |> Md5Hasher.toString,
            fun () -> Task.FromResult(SourceTextNew.ofString content)
        )

    static member CreateFromFileSystem(fileName: string) =
        FSharpFileSnapshot(
            fileName,
            FileSystem.GetLastWriteTimeShim(fileName).Ticks.ToString(),
            fun () ->
                FileSystem.OpenFileForReadShim(fileName).ReadAllText()
                |> SourceTextNew.ofString
                |> Task.FromResult
        )

    static member CreateFromDocumentSource(fileName: string, documentSource: DocumentSource) =

        match documentSource with
        | DocumentSource.Custom f ->
            let version = DateTime.Now.Ticks.ToString()

            FSharpFileSnapshot(
                fileName,
                version,
                fun () ->
                    task {
                        match! f fileName |> Async.StartAsTask with
                        | Some source -> return SourceTextNew.ofISourceText source
                        | None -> return failwith $"Couldn't get source for file {f}"
                    }
            )

        | DocumentSource.FileSystem -> FSharpFileSnapshot.CreateFromFileSystem fileName

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

/// A source file snapshot with loaded source text.
type internal FSharpFileSnapshotWithSource
    (FileName: string, SourceHash: ImmutableArray<byte>, Source: ISourceTextNew, IsLastCompiland: bool, IsExe: bool) =

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

/// A source file snapshot with parsed syntax tree
type internal FSharpParsedFile
    (
        FileName: string,
        SyntaxTreeHash: byte array,
        SourceText: ISourceText,
        ParsedInput: ParsedInput,
        ParseDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity)[]
    ) =

    member _.FileName = FileName
    member _.SourceText = SourceText
    member _.ParsedInput = ParsedInput
    member _.ParseDiagnostics = ParseDiagnostics

    member val IsSignatureFile = FileName |> isSignatureFile

    interface IFileSnapshot with
        member this.FileName = this.FileName
        member this.Version = SyntaxTreeHash
        member this.IsSignatureFile = this.IsSignatureFile

/// An on-disk reference needed for project compilation.
[<Experimental("This FCS API is experimental and subject to change.")>]
type ReferenceOnDisk =
    { Path: string; LastModified: DateTime }

/// A snapshot of an F# project. The source file type can differ based on which stage of compilation the snapshot is used for.
type internal ProjectSnapshotBase<'T when 'T :> IFileSnapshot>
    (projectConfig: ProjectConfig, referencedProjects: FSharpReferencedProjectSnapshot list, sourceFiles: 'T list) =

    // Version of project without source files
    let baseVersion =
        lazy
            (projectConfig.Version
             |> Md5Hasher.addBytes' (referencedProjects |> Seq.map _.Version))

    let baseVersionString = lazy (baseVersion.Value |> Md5Hasher.toString)

    let baseCacheKeyWith (label, version) =
        { new ICacheKey<_, _> with
            member _.GetLabel() = $"{label} ({projectConfig.Label})"
            member _.GetKey() = projectConfig.Identifier
            member _.GetVersion() = baseVersionString.Value, version
        }

    let noFileVersionsHash =
        lazy
            (baseVersion.Value
             |> Md5Hasher.addStrings (sourceFiles |> Seq.map (fun x -> x.FileName)))

    let noFileVersionsKey =
        lazy
            ({ new ICacheKey<_, _> with
                 member _.GetLabel() = projectConfig.Label
                 member _.GetKey() = projectConfig.Identifier

                 member _.GetVersion() =
                     noFileVersionsHash.Value |> Md5Hasher.toString

             })

    let fullHash =
        lazy
            (baseVersion.Value
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
                 member _.GetLabel() = projectConfig.Label
                 member _.GetKey() = projectConfig.Identifier
                 member _.GetVersion() = fullHash.Value |> Md5Hasher.toString
             })

    let addHash (file: 'T) hash =
        hash |> Md5Hasher.addString file.FileName |> Md5Hasher.addBytes file.Version

    let signatureHash =
        lazy (signatureHash baseVersion.Value (sourceFiles |> Seq.map (fun x -> x :> IFileSnapshot)))

    let signatureKey =
        lazy (baseCacheKeyWith ("Signature", signatureHash.Value |> fst |> Md5Hasher.toString))

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
                 member _.GetLabel() = $"{f.FileName} ({projectConfig.Label})"
                 member _.GetKey() = f.FileName, projectConfig.Identifier
                 member _.GetVersion() = hash |> Md5Hasher.toString
             })

    let sourceFileNames = lazy (sourceFiles |> List.map (fun x -> x.FileName))

    member _.ProjectFileName = projectConfig.ProjectFileName
    member _.ProjectId = projectConfig.ProjectId
    member _.Identifier = projectConfig.Identifier
    member _.ReferencesOnDisk = projectConfig.ReferencesOnDisk
    member _.OtherOptions = projectConfig.OtherOptions
    member _.ReferencedProjects = referencedProjects

    member _.IsIncompleteTypeCheckEnvironment =
        projectConfig.IsIncompleteTypeCheckEnvironment

    member _.UseScriptResolutionRules = projectConfig.UseScriptResolutionRules
    member _.LoadTime = projectConfig.LoadTime
    member _.UnresolvedReferences = projectConfig.UnresolvedReferences
    member _.OriginalLoadReferences = projectConfig.OriginalLoadReferences
    member _.Stamp = projectConfig.Stamp
    member _.CommandLineOptions = projectConfig.CommandLineOptions
    member _.ProjectDirectory = projectConfig.ProjectDirectory

    member _.OutputFileName = projectConfig.OutputFileName

    member _.ProjectConfig = projectConfig

    member _.SourceFiles = sourceFiles

    member _.SourceFileNames = sourceFileNames.Value

    member _.Label = projectConfig.Label

    member _.IndexOf fileName =
        sourceFiles
        |> List.tryFindIndex (fun x -> x.FileName = fileName)
        |> Option.defaultWith (fun () -> failwith (sprintf "Unable to find file %s in project %s" fileName projectConfig.ProjectFileName))

    member private _.With(sourceFiles: 'T list) =
        ProjectSnapshotBase(projectConfig, referencedProjects, sourceFiles)

    /// Create a new snapshot with given source files replacing files in this snapshot with the same name. Other files remain unchanged.
    member this.Replace(changedSourceFiles: 'T list) =
        // TODO: validate if changed files are not present in the original list?

        let sourceFiles =
            sourceFiles
            |> List.map (fun x ->
                match changedSourceFiles |> List.tryFind (fun y -> y.FileName = x.FileName) with
                | Some y -> y
                | None -> x)

        this.With sourceFiles

    /// Create a new snapshot with source files only up to the given index (inclusive)
    member this.UpTo fileIndex = this.With sourceFiles[..fileIndex]

    /// Create a new snapshot with source files only up to the given file name (inclusive)
    member this.UpTo fileName = this.UpTo(this.IndexOf fileName)

    /// Create a new snapshot with only source files at the given indexes
    member this.OnlyWith fileIndexes =
        this.With(
            fileIndexes
            |> Set.toList
            |> List.sort
            |> List.choose (fun x -> sourceFiles |> List.tryItem x)
        )

    override this.ToString() =
        Path.GetFileNameWithoutExtension this.ProjectFileName
        |> sprintf "FSharpProjectSnapshot(%s)"

    /// The newest last modified time of any file in this snapshot including the project file
    member _.GetLastModifiedTimeOnDisk() =
        seq {
            projectConfig.ProjectFileName

            yield!
                sourceFiles
                |> Seq.filter (fun x -> not (x.FileName.EndsWith(".AssemblyInfo.fs"))) // TODO: is this safe? any better way of doing this?
                |> Seq.filter (fun x -> not (x.FileName.EndsWith(".AssemblyAttributes.fs")))
                |> Seq.map (fun x -> x.FileName)
        }
        |> Seq.map FileSystem.GetLastWriteTimeShim
        |> Seq.max

    member _.FullVersion = fullHash.Value
    member _.SignatureVersion = signatureHash.Value |> fst
    member _.LastFileVersion = lastFileHash.Value |> fst

    /// Version for parsing - doesn't include any references because they don't affect parsing (...right?)
    member _.ParsingVersion = projectConfig.VersionForParsing |> Md5Hasher.toString

    /// A key for this snapshot but without file versions. So it will be the same across any in-file changes.
    member _.NoFileVersionsKey = noFileVersionsKey.Value

    /// A full key for this snapshot, any change will cause this to change.
    member _.FullKey = fullKey.Value

    /// A key including the public surface or signature for this snapshot
    member _.SignatureKey = signatureKey.Value

    /// A key including the public surface or signature for this snapshot and the last file (even if it's not a signature file)
    member _.LastFileKey = lastFileKey.Value

    //TODO: cache it here?
    member this.FileKey(fileName: string) = this.UpTo(fileName).LastFileKey
    member this.FileKey(index: FileIndex) = this.UpTo(index).LastFileKey

    member this.FileKeyWithExtraFileSnapshotVersion(fileName: string) =
        let fileKey = this.FileKey fileName
        let fileSnapshot = this.SourceFiles |> Seq.find (fun f -> f.FileName = fileName)

        fileKey.WithExtraVersion(fileSnapshot.Version |> Md5Hasher.toString)

    /// Cache key for the project without source files
    member this.BaseCacheKeyWith(label, version) = baseCacheKeyWith (label, version)

/// Project snapshot with filenames and versions given as initial input
and internal ProjectSnapshot = ProjectSnapshotBase<FSharpFileSnapshot>

/// Project snapshot with file sources loaded
and internal ProjectSnapshotWithSources = ProjectSnapshotBase<FSharpFileSnapshotWithSource>

/// All required information for compiling a project except the source files and referenced projects. It's kept separate so it can be reused
/// for different stages of a project snapshot and also between changes to the source files.
and [<Experimental("This FCS API is experimental and subject to change.")>] ProjectConfig
    internal
    (
        projectFileName: string,
        outputFileName: string option,
        referencesOnDisk: ReferenceOnDisk list,
        otherOptions: string list,
        isIncompleteTypeCheckEnvironment: bool,
        useScriptResolutionRules: bool,
        unresolvedReferences,
        originalLoadReferences: (range * string * string) list,
        loadTime: DateTime,
        stamp: int64 option,
        projectId: string option
    ) =

    let hashForParsing =
        lazy
            (Md5Hasher.empty
             |> Md5Hasher.addString projectFileName
             |> Md5Hasher.addStrings otherOptions
             |> Md5Hasher.addBool isIncompleteTypeCheckEnvironment
             |> Md5Hasher.addBool useScriptResolutionRules)

    let fullHash =
        lazy
            (hashForParsing.Value
             |> Md5Hasher.addStrings (referencesOnDisk |> Seq.map (fun r -> r.Path))
             |> Md5Hasher.addDateTimes (referencesOnDisk |> Seq.map (fun r -> r.LastModified)))

    let commandLineOptions =
        lazy
            (seq {
                yield! otherOptions

                for r in referencesOnDisk do
                    $"-r:{r.Path}"
             }
             |> Seq.toList)

    let outputFileNameValue =
        lazy
            (outputFileName
             |> Option.orElseWith (fun () -> otherOptions |> findOutputFileName))

    let identifier =
        lazy
            ((projectFileName, outputFileNameValue.Value |> Option.defaultValue "")
             |> FSharpProjectIdentifier)

    new(projectFileName: string,
        outputFileName: string option,
        referencesOnDisk: string seq,
        otherOptions: string seq,
        ?isIncompleteTypeCheckEnvironment: bool,
        ?useScriptResolutionRules: bool,
        ?loadTime: DateTime,
        ?stamp: int64,
        ?projectId: string) =

        let referencesOnDisk =
            referencesOnDisk
            |> Seq.map (fun path ->
                {
                    Path = path
                    LastModified = FileSystem.GetLastWriteTimeShim path
                })
            |> Seq.toList

        ProjectConfig(
            projectFileName,
            outputFileName,
            referencesOnDisk,
            otherOptions |> Seq.toList,
            isIncompleteTypeCheckEnvironment = defaultArg isIncompleteTypeCheckEnvironment false,
            useScriptResolutionRules = defaultArg useScriptResolutionRules false,
            unresolvedReferences = None,
            originalLoadReferences = [],
            loadTime = defaultArg loadTime DateTime.Now,
            stamp = stamp,
            projectId = projectId
        )

    member val ProjectDirectory = !! Path.GetDirectoryName(projectFileName)
    member _.OutputFileName = outputFileNameValue.Value
    member _.Identifier = identifier.Value
    member _.Version = fullHash.Value
    member _.Label = projectFileName |> shortPath
    member _.VersionForParsing = hashForParsing.Value

    member _.CommandLineOptions = commandLineOptions.Value

    member _.ProjectFileName = projectFileName
    member _.ProjectId = projectId
    member _.ReferencesOnDisk = referencesOnDisk
    member _.OtherOptions = otherOptions

    member _.IsIncompleteTypeCheckEnvironment = isIncompleteTypeCheckEnvironment
    member _.UseScriptResolutionRules = useScriptResolutionRules
    member _.LoadTime = loadTime
    member _.Stamp = stamp
    member _.UnresolvedReferences = unresolvedReferences
    member _.OriginalLoadReferences = originalLoadReferences

    /// Creates a copy of this project config with a new set of references
    member internal _.With(newReferencesOnDisk) =
        ProjectConfig(
            projectFileName,
            outputFileName,
            newReferencesOnDisk,
            otherOptions,
            isIncompleteTypeCheckEnvironment,
            useScriptResolutionRules,
            unresolvedReferences,
            originalLoadReferences,
            loadTime,
            stamp,
            projectId
        )

and [<NoComparison; CustomEquality; Experimental("This FCS API is experimental and subject to change.")>] FSharpReferencedProjectSnapshot =
    /// <summary>
    /// A reference to an F# project. The physical data for it is stored/cached inside of the compiler service.
    /// </summary>
    /// <param name="projectOutputFile">The fully qualified path to the output of the referenced project. This should be the same value as the <c>-r</c> reference in the project options for this referenced project.</param>
    /// <param name="snapshot">Snapshot of the referenced F# project</param>
    | FSharpReference of projectOutputFile: string * snapshot: FSharpProjectSnapshot
    /// <summary>
    /// A reference to any portable executable, including F#. The stream is owned by this reference.
    /// The stream will be automatically disposed when there are no references to FSharpReferencedProject and is GC collected.
    /// Once the stream is evaluated, the function that constructs the stream will no longer be referenced by anything.
    /// If the stream evaluation throws an exception, it will be automatically handled.
    /// </summary>
    /// <param name="getStamp">A function that calculates a last-modified timestamp for this reference. This will be used to determine if the reference is up-to-date.</param>
    /// <param name="delayedReader">A function that opens a Portable Executable data stream for reading.</param>
    | PEReference of getStamp: (unit -> DateTime) * delayedReader: DelayedILModuleReader

    /// <summary>
    /// A reference to an ILModuleReader.
    /// </summary>
    /// <param name="projectOutputFile">The fully qualified path to the output of the referenced project. This should be the same value as the <c>-r</c> reference in the project options for this referenced project.</param>
    /// <param name="getStamp">A function that calculates a last-modified timestamp for this reference. This will be used to determine if the reference is up-to-date.</param>
    /// <param name="getReader">A function that creates an ILModuleReader for reading module data.</param>
    | ILModuleReference of
        projectOutputFile: string *
        getStamp: (unit -> DateTime) *
        getReader: (unit -> FSharp.Compiler.AbstractIL.ILBinaryReader.ILModuleReader)

    /// <summary>
    /// The fully qualified path to the output of the referenced project. This should be the same value as the <c>-r</c>
    /// reference in the project options for this referenced project.
    /// </summary>
    member this.OutputFile =
        match this with
        | FSharpReference(projectOutputFile = projectOutputFile)
        | ILModuleReference(projectOutputFile = projectOutputFile) -> projectOutputFile
        | PEReference(delayedReader = reader) -> reader.OutputFile

    /// <summary>
    /// Creates a reference for an F# project. The physical data for it is stored/cached inside of the compiler service.
    /// </summary>
    /// <param name="projectOutputFile">The fully qualified path to the output of the referenced project. This should be the same value as the <c>-r</c> reference in the project options for this referenced project.</param>
    /// <param name="snapshot">The project snapshot for this F# project</param>
    static member CreateFSharp(projectOutputFile, snapshot: FSharpProjectSnapshot) =
        FSharpReference(projectOutputFile, snapshot)

    member this.Version =
        match this with
        | FSharpReference(_name, p) -> p.ProjectSnapshot.SignatureVersion
        | PEReference(getStamp, _) -> Md5Hasher.empty |> Md5Hasher.addDateTime (getStamp ())
        | ILModuleReference(_name, getStamp, _) -> Md5Hasher.empty |> Md5Hasher.addDateTime (getStamp ())

    override this.Equals(o) =
        match o with
        | :? FSharpReferencedProjectSnapshot as o ->
            match this, o with
            | FSharpReference(projectOutputFile1, options1), FSharpReference(projectOutputFile2, options2) ->
                projectOutputFile1 = projectOutputFile2 && options1 = options2
            | PEReference(getStamp1, reader1), PEReference(getStamp2, reader2) ->
                reader1.OutputFile = reader2.OutputFile && (getStamp1 ()) = (getStamp2 ())
            | ILModuleReference(projectOutputFile1, getStamp1, _), ILModuleReference(projectOutputFile2, getStamp2, _) ->
                projectOutputFile1 = projectOutputFile2 && (getStamp1 ()) = (getStamp2 ())
            | _ -> false

        | _ -> false

    override this.GetHashCode() = this.OutputFile.GetHashCode()

/// An identifier of an F# project. This serves to identify the same project as it changes over time and enables us to clear obsolete data from caches.
and [<Experimental("This FCS API is experimental and subject to change.")>] FSharpProjectIdentifier =
    | FSharpProjectIdentifier of projectFileName: string * outputFileName: string

    member this.OutputFileName =
        match this with
        | FSharpProjectIdentifier(_, outputFileName) -> outputFileName

    member this.ProjectFileName =
        match this with
        | FSharpProjectIdentifier(projectFileName, _) -> projectFileName

    override this.ToString() =
        $"{shortPath this.ProjectFileName} 🡒 {shortPath this.OutputFileName}"

/// A snapshot of an F# project. This type contains all the necessary information for type checking a project.
and [<Experimental("This FCS API is experimental and subject to change.")>] FSharpProjectSnapshot internal (projectSnapshot) =

    member internal _.ProjectSnapshot: ProjectSnapshot = projectSnapshot

    /// Create a new snapshot with given source files replacing files in this snapshot with the same name. Other files remain unchanged.
    member _.Replace(changedSourceFiles: FSharpFileSnapshot list) =
        projectSnapshot.Replace(changedSourceFiles) |> FSharpProjectSnapshot

    member _.Label = projectSnapshot.Label
    member _.Identifier = projectSnapshot.ProjectConfig.Identifier
    member _.ProjectFileName = projectSnapshot.ProjectFileName
    member _.ProjectId = projectSnapshot.ProjectId
    member _.SourceFiles = projectSnapshot.SourceFiles
    member _.ReferencesOnDisk = projectSnapshot.ReferencesOnDisk
    member _.OtherOptions = projectSnapshot.OtherOptions
    member _.ReferencedProjects = projectSnapshot.ReferencedProjects

    member _.IsIncompleteTypeCheckEnvironment =
        projectSnapshot.IsIncompleteTypeCheckEnvironment

    member _.UseScriptResolutionRules = projectSnapshot.UseScriptResolutionRules
    member _.LoadTime = projectSnapshot.LoadTime
    member _.UnresolvedReferences = projectSnapshot.UnresolvedReferences
    member _.OriginalLoadReferences = projectSnapshot.OriginalLoadReferences
    member _.Stamp = projectSnapshot.Stamp
    member _.OutputFileName = projectSnapshot.OutputFileName

    static member Create
        (
            projectFileName: string,
            outputFileName: string option,
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

        let projectConfig =
            ProjectConfig(
                projectFileName,
                outputFileName,
                referencesOnDisk,
                otherOptions,
                isIncompleteTypeCheckEnvironment,
                useScriptResolutionRules,
                unresolvedReferences,
                originalLoadReferences,
                loadTime,
                stamp,
                projectId
            )

        ProjectSnapshotBase(projectConfig, referencedProjects, sourceFiles)
        |> FSharpProjectSnapshot

    static member FromOptions(options: FSharpProjectOptions, getFileSnapshot, ?snapshotAccumulator) =
        let snapshotAccumulator = defaultArg snapshotAccumulator (Dictionary())

        async {

            // TODO: check if options is a good key here
            if not (snapshotAccumulator.ContainsKey options) then

                let! sourceFiles =
                    options.SourceFiles
                    |> Seq.map (getFileSnapshot options)
                    |> MultipleDiagnosticsLoggers.Parallel

                let! referencedProjects =
                    options.ReferencedProjects
                    |> Seq.map (function
                        | FSharpReferencedProject.FSharpReference(outputName, options) ->
                            async {
                                let! snapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot, snapshotAccumulator)

                                return FSharpReferencedProjectSnapshot.FSharpReference(outputName, snapshot)
                            }
                        | FSharpReferencedProject.PEReference(getStamp, reader) ->
                            async.Return <| FSharpReferencedProjectSnapshot.PEReference(getStamp, reader)
                        | FSharpReferencedProject.ILModuleReference(outputName, getStamp, getReader) ->
                            async.Return
                            <| FSharpReferencedProjectSnapshot.ILModuleReference(outputName, getStamp, getReader))

                    |> MultipleDiagnosticsLoggers.Sequential

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
                        outputFileName = None,
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

    static member FromOptions(options: FSharpProjectOptions, documentSource: DocumentSource) =
        FSharpProjectSnapshot.FromOptions(
            options,
            fun _ fileName ->
                FSharpFileSnapshot.CreateFromDocumentSource(fileName, documentSource)
                |> async.Return
        )

    static member FromOptions
        (
            options: FSharpProjectOptions,
            fileName: string,
            fileVersion: int,
            sourceText: ISourceText,
            documentSource: DocumentSource
        ) =

        let getFileSnapshot _ fName =
            if fName = fileName then
                FSharpFileSnapshot.Create(
                    fileName,
                    $"{fileVersion}{sourceText.GetHashCode().ToString()}",
                    fun () -> Task.FromResult(SourceTextNew.ofISourceText sourceText)
                )
            else
                FSharpFileSnapshot.CreateFromDocumentSource(fName, documentSource)
            |> async.Return

        FSharpProjectSnapshot.FromOptions(options, getFileSnapshot)

let internal snapshotTable =
    ConditionalWeakTable<ProjectSnapshot, FSharpProjectOptions>()

let rec internal snapshotToOptions (projectSnapshot: ProjectSnapshot) =
    snapshotTable.GetValue(
        projectSnapshot,
        fun projectSnapshot ->
            {
                ProjectFileName = projectSnapshot.ProjectFileName
                ProjectId = projectSnapshot.ProjectId
                SourceFiles = projectSnapshot.SourceFiles |> Seq.map (fun x -> x.FileName) |> Seq.toArray
                OtherOptions = projectSnapshot.CommandLineOptions |> List.toArray
                ReferencedProjects =
                    projectSnapshot.ReferencedProjects
                    |> Seq.map (function
                        | FSharpReference(name, opts) ->
                            FSharpReferencedProject.FSharpReference(name, opts.ProjectSnapshot |> snapshotToOptions)
                        | PEReference(getStamp, reader) -> FSharpReferencedProject.PEReference(getStamp, reader)
                        | ILModuleReference(name, getStamp, getReader) ->
                            FSharpReferencedProject.ILModuleReference(name, getStamp, getReader))
                    |> Seq.toArray
                IsIncompleteTypeCheckEnvironment = projectSnapshot.IsIncompleteTypeCheckEnvironment
                UseScriptResolutionRules = projectSnapshot.UseScriptResolutionRules
                LoadTime = projectSnapshot.LoadTime
                UnresolvedReferences = projectSnapshot.UnresolvedReferences
                OriginalLoadReferences = projectSnapshot.OriginalLoadReferences
                Stamp = projectSnapshot.Stamp
            }
    )

[<Extension>]
type internal Extensions =

    [<Extension>]
    static member ToOptions(this: ProjectSnapshot) = this |> snapshotToOptions

    [<Extension>]
    static member ToOptions(this: FSharpProjectSnapshot) =
        this.ProjectSnapshot |> snapshotToOptions

    [<Extension>]
    static member GetProjectIdentifier(this: FSharpProjectOptions) : ProjectIdentifier =
        this.ProjectFileName, this.OtherOptions |> findOutputFileName |> Option.defaultValue ""
