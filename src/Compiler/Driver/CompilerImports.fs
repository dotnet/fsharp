// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Contains logic to coordinate assembly resolution and manage the TcImports table of referenced
/// assemblies.
module internal FSharp.Compiler.CompilerImports

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Diagnostics
open System.IO
open System.IO.Compression
open System.Reflection

open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.FSharpEnvironment
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Diagnostics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Import
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTreePickle
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.BuildGraph

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
open FSharp.Core.CompilerServices
#endif

let (++) x s = x @ [ s ]

//----------------------------------------------------------------------------
// Signature and optimization data blobs
//--------------------------------------------------------------------------

let IsSignatureDataResource (r: ILResource) =
    r.Name.StartsWithOrdinal FSharpSignatureDataResourceName
    || r.Name.StartsWithOrdinal FSharpSignatureCompressedDataResourceName
    || r.Name.StartsWithOrdinal FSharpSignatureDataResourceName2

let IsOptimizationDataResource (r: ILResource) =
    r.Name.StartsWithOrdinal FSharpOptimizationDataResourceName
    || r.Name.StartsWithOrdinal FSharpOptimizationCompressedDataResourceName
    || r.Name.StartsWithOrdinal FSharpOptimizationDataResourceName2

let decompressResource (r: ILResource) =
    use raw = r.GetBytes().AsStream()
    use decompressed = new MemoryStream()
    use deflator = new DeflateStream(raw, CompressionMode.Decompress)
    deflator.CopyTo decompressed
    deflator.Close()
    ByteStorage.FromByteArray(decompressed.ToArray()).GetByteMemory()

let GetResourceNameAndSignatureDataFunc (r: ILResource) =
    let resourceType, ccuName =
        if r.Name.StartsWithOrdinal FSharpSignatureDataResourceName then
            FSharpSignatureDataResourceName, String.dropPrefix r.Name FSharpSignatureDataResourceName
        elif r.Name.StartsWithOrdinal FSharpSignatureCompressedDataResourceName then
            FSharpSignatureCompressedDataResourceName, String.dropPrefix r.Name FSharpSignatureCompressedDataResourceName
        elif r.Name.StartsWithOrdinal FSharpSignatureDataResourceName2 then
            FSharpSignatureDataResourceName2, String.dropPrefix r.Name FSharpSignatureDataResourceName2
        else
            failwith "GetSignatureDataResourceName"

    if resourceType = FSharpSignatureCompressedDataResourceName then
        ccuName, (fun () -> decompressResource (r))
    else
        ccuName, (fun () -> r.GetBytes())

let GetResourceNameAndOptimizationDataFunc (r: ILResource) =
    let resourceType, ccuName =
        if r.Name.StartsWithOrdinal FSharpOptimizationDataResourceName then
            FSharpOptimizationDataResourceName, String.dropPrefix r.Name FSharpOptimizationDataResourceName
        elif r.Name.StartsWithOrdinal FSharpOptimizationCompressedDataResourceName then
            FSharpOptimizationCompressedDataResourceName, String.dropPrefix r.Name FSharpOptimizationCompressedDataResourceName
        elif r.Name.StartsWithOrdinal FSharpOptimizationDataResourceName2 then
            FSharpOptimizationDataResourceName2, String.dropPrefix r.Name FSharpOptimizationDataResourceName2
        else
            failwith "GetOptimizationDataResourceName"

    if resourceType = FSharpOptimizationCompressedDataResourceName then
        ccuName, (fun () -> decompressResource (r))
    else
        ccuName, (fun () -> r.GetBytes())

let IsReflectedDefinitionsResource (r: ILResource) =
    r.Name.StartsWithOrdinal(QuotationPickler.SerializedReflectedDefinitionsResourceNameBase)

let MakeILResource rName bytes =
    {
        Name = rName
        Location = ILResourceLocation.Local(ByteStorage.FromByteArray(bytes))
        Access = ILResourceAccess.Public
        CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
        MetadataIndex = NoMetadataIdx
    }

let PickleToResource inMem file (g: TcGlobals) compress scope rName p x =
    let file = PathMap.apply g.pathMap file

    let bytes =
        use bytes = pickleObjWithDanglingCcus inMem file g scope p x

        if compress then
            let raw = new MemoryStream(bytes.AsMemory().ToArray())
            let compressed = new MemoryStream()
            use deflator = new DeflateStream(compressed, CompressionLevel.Optimal)
            raw.CopyTo deflator
            deflator.Close()
            compressed.ToArray()
        else
            bytes.AsMemory().ToArray()

    let byteStorage = ByteStorage.FromByteArray(bytes)

    {
        Name = rName
        Location = ILResourceLocation.Local(byteStorage)
        Access = ILResourceAccess.Public
        CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
        MetadataIndex = NoMetadataIdx
    }

let GetSignatureData (file, ilScopeRef, ilModule, byteReader) : PickledDataWithReferences<PickledCcuInfo> =
    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleCcuInfo (byteReader ())

let WriteSignatureData (tcConfig: TcConfig, tcGlobals, exportRemapping, ccu: CcuThunk, fileName, inMem) : ILResource =
    let mspec = ApplyExportRemappingToEntity tcGlobals exportRemapping ccu.Contents

    // For historical reasons, we use a different resource name for FSharp.Core, so older F# compilers
    // don't complain when they see the resource.
    let rName, compress =
        if tcConfig.compressMetadata then
            FSharpSignatureCompressedDataResourceName, true
        elif ccu.AssemblyName = getFSharpCoreLibraryName then
            FSharpSignatureDataResourceName2, false
        else
            FSharpSignatureDataResourceName, false

    let includeDir =
        if String.IsNullOrEmpty tcConfig.implicitIncludeDir then
            ""
        else
            tcConfig.implicitIncludeDir
            |> FileSystem.GetFullPathShim
            |> PathMap.applyDir tcGlobals.pathMap

    PickleToResource
        inMem
        fileName
        tcGlobals
        compress
        ccu
        (rName + ccu.AssemblyName)
        pickleCcuInfo
        {
            mspec = mspec
            compileTimeWorkingDir = includeDir
            usesQuotations = ccu.UsesFSharp20PlusQuotations
        }

let GetOptimizationData (file, ilScopeRef, ilModule, byteReader) =
    unpickleObjWithDanglingCcus file ilScopeRef ilModule Optimizer.u_CcuOptimizationInfo (byteReader ())

let WriteOptimizationData (tcConfig: TcConfig, tcGlobals, fileName, inMem, ccu: CcuThunk, modulInfo) =
    // For historical reasons, we use a different resource name for FSharp.Core, so older F# compilers
    // don't complain when they see the resource.
    let rName, compress =
        if tcConfig.compressMetadata then
            FSharpOptimizationCompressedDataResourceName, true
        elif ccu.AssemblyName = getFSharpCoreLibraryName then
            FSharpOptimizationDataResourceName2, false
        else
            FSharpOptimizationDataResourceName, false

    PickleToResource inMem fileName tcGlobals compress ccu (rName + ccu.AssemblyName) Optimizer.p_CcuOptimizationInfo modulInfo

let EncodeSignatureData (tcConfig: TcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, isIncrementalBuild) =
    if tcConfig.GenerateSignatureData then
        let resource =
            WriteSignatureData(tcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, isIncrementalBuild)

        let resources = [ resource ]

        let sigAttr =
            mkSignatureDataVersionAttr tcGlobals (parseILVersion FSharpBinaryMetadataFormatRevision)

        [ sigAttr ], resources
    else
        [], []

let EncodeOptimizationData (tcGlobals, tcConfig: TcConfig, outfile, exportRemapping, data, isIncrementalBuild) =
    if tcConfig.GenerateOptimizationData then
        let data = map2Of2 (Optimizer.RemapOptimizationInfo tcGlobals exportRemapping) data

        let ccu, optData =
            if tcConfig.onlyEssentialOptimizationData then
                map2Of2 Optimizer.AbstractOptimizationInfoToEssentials data
            else
                data

        [
            WriteOptimizationData(tcConfig, tcGlobals, outfile, isIncrementalBuild, ccu, optData)
        ]
    else
        []

exception AssemblyNotResolved of originalName: string * range: range

exception MSBuildReferenceResolutionWarning of message: string * warningCode: string * range: range

exception MSBuildReferenceResolutionError of message: string * warningCode: string * range: range

let OpenILBinary (fileName, reduceMemoryUsage, pdbDirPath, shadowCopyReferences, tryGetMetadataSnapshot) =
    let opts: ILReaderOptions =
        {
            metadataOnly = MetadataOnlyFlag.Yes
            reduceMemoryUsage = reduceMemoryUsage
            pdbDirPath = pdbDirPath
            tryGetMetadataSnapshot = tryGetMetadataSnapshot
        }

    let location =
        // In order to use memory mapped files on the shadow copied version of the Assembly, we `preload the assembly
        // We swallow all exceptions so that we do not change the exception contract of this API
        if shadowCopyReferences && not isRunningOnCoreClr then
            try
                Assembly.ReflectionOnlyLoadFrom(fileName).Location
            with _ ->
                fileName
        else
            fileName

    AssemblyReader.GetILModuleReader(location, opts)

[<RequireQualifiedAccess>]
type ResolveAssemblyReferenceMode =
    | Speculative
    | ReportErrors

#if !NO_TYPEPROVIDERS
type ResolvedExtensionReference = ResolvedExtensionReference of string * AssemblyReference list * Tainted<ITypeProvider> list
#endif

#if DEBUG
[<DebuggerDisplay("AssemblyResolution({resolvedPath})")>]
#endif
type AssemblyResolution =
    {
        /// The original reference to the assembly.
        originalReference: AssemblyReference

        /// Path to the resolvedFile
        resolvedPath: string

        /// Create the tooltip text for the assembly reference
        prepareToolTip: unit -> string

        /// Whether or not this is an installed system assembly (for example, System.dll)
        sysdir: bool

        /// Lazily populated ilAssemblyRef for this reference.
        mutable ilAssemblyRef: ILAssemblyRef option
    }

    override this.ToString() =
        sprintf "%s%s" (if this.sysdir then "[sys]" else "") this.resolvedPath

    member this.ProjectReference = this.originalReference.ProjectReference

    /// Compute the ILAssemblyRef for a resolved assembly. This is done by reading the binary if necessary. The result
    /// is cached.
    ///
    /// Only used in F# Interactive
    member this.GetILAssemblyRef(reduceMemoryUsage, tryGetMetadataSnapshot) =
        match this.ilAssemblyRef with
        | Some assemblyRef -> assemblyRef
        | None ->
            match this.ProjectReference with
            | Some _ -> failwith "IProjectReference is not allowed to be used in GetILAssemblyRef"
            | None -> ()

            let assemblyRef =
                let readerSettings: ILReaderOptions =
                    {
                        pdbDirPath = None
                        reduceMemoryUsage = reduceMemoryUsage
                        metadataOnly = MetadataOnlyFlag.Yes
                        tryGetMetadataSnapshot = tryGetMetadataSnapshot
                    }

                use reader = OpenILModuleReader this.resolvedPath readerSettings
                mkRefToILAssembly reader.ILModuleDef.ManifestOfAssembly

            this.ilAssemblyRef <- Some assemblyRef
            assemblyRef

type ImportedBinary =
    {
        FileName: string
        RawMetadata: IRawFSharpAssemblyData
#if !NO_TYPEPROVIDERS
        ProviderGeneratedAssembly: Assembly option
        IsProviderGenerated: bool
        ProviderGeneratedStaticLinkMap: ProvidedAssemblyStaticLinkingMap option
#endif
        ILAssemblyRefs: ILAssemblyRef list
        ILScopeRef: ILScopeRef
    }

type ImportedAssembly =
    {
        ILScopeRef: ILScopeRef
        FSharpViewOfMetadata: CcuThunk
        AssemblyAutoOpenAttributes: string list
        AssemblyInternalsVisibleToAttributes: string list
#if !NO_TYPEPROVIDERS
        IsProviderGenerated: bool
        mutable TypeProviders: Tainted<ITypeProvider> list
#endif
        FSharpOptimizationData: Microsoft.FSharp.Control.Lazy<Option<Optimizer.LazyModuleInfo>>
    }

type AvailableImportedAssembly =
    | ResolvedImportedAssembly of ImportedAssembly * range
    | UnresolvedImportedAssembly of string * range

type CcuLoadFailureAction =
    | RaiseError
    | ReturnNone

type TcImportsLockToken() =
    interface LockToken

type TcImportsLock = Lock<TcImportsLockToken>

let RequireTcImportsLock (_tcitok: TcImportsLockToken, _thingProtected: 'T) = ()

// if this is a #r reference (not from dummy range), make sure the directory of the declaring
// file is included in the search path. This should ideally already be one of the search paths, but
// during some global checks it won't be. We append to the end of the search list so that this is the last
// place that is checked.
let isHashRReference (r: range) =
    not (equals r range0)
    && not (equals r rangeStartup)
    && not (equals r rangeCmdArgs)
    && FileSystem.IsPathRootedShim r.FileName

let IsNetModule fileName =
    let ext = Path.GetExtension fileName
    String.Compare(ext, ".netmodule", StringComparison.OrdinalIgnoreCase) = 0

let IsDLL fileName =
    let ext = Path.GetExtension fileName
    String.Compare(ext, ".dll", StringComparison.OrdinalIgnoreCase) = 0

let IsExe fileName =
    let ext = Path.GetExtension fileName
    String.Compare(ext, ".exe", StringComparison.OrdinalIgnoreCase) = 0

type TcConfig with

    member tcConfig.TryResolveLibWithDirectories(r: AssemblyReference) =
        let m, nm = r.Range, r.Text
        use _ = UseBuildPhase BuildPhase.Parameter

        // See if the language service has already produced the contents of the assembly for us, virtually
        match r.ProjectReference with
        | Some _ ->
            let resolved = r.Text
            let sysdir = tcConfig.IsSystemAssembly resolved

            Some
                {
                    originalReference = r
                    resolvedPath = resolved
                    prepareToolTip = (fun () -> resolved)
                    sysdir = sysdir
                    ilAssemblyRef = None
                }
        | None ->

            // Only want to resolve certain extensions (otherwise, 'System.Xml' is ambiguous).
            // MSBuild resolution is limited to .exe and .dll so do the same here.
            if IsDLL nm || IsExe nm || IsNetModule nm then

                let searchPaths =
                    seq {
                        yield! tcConfig.GetSearchPathsForLibraryFiles()

                        if isHashRReference m then
                            Path.GetDirectoryName(m.FileName)
                    }

                let resolved = TryResolveFileUsingPaths(searchPaths, m, nm)

                match resolved with
                | Some resolved ->
                    let sysdir = tcConfig.IsSystemAssembly resolved

                    Some
                        {
                            originalReference = r
                            resolvedPath = resolved
                            prepareToolTip =
                                (fun () ->
                                    let fusionName = AssemblyName.GetAssemblyName(resolved).ToString()
                                    let line (append: string) = append.Trim(' ') + "\n"
                                    line resolved + line fusionName)
                            sysdir = sysdir
                            ilAssemblyRef = None
                        }
                | None -> None
            else
                None

    member tcConfig.ResolveLibWithDirectories(ccuLoadFailureAction, r: AssemblyReference) =
        let m, nm = r.Range, r.Text
        use _ = UseBuildPhase BuildPhase.Parameter

        let rs =
            if IsExe nm || IsDLL nm || IsNetModule nm then
                [ r ]
            else
                [
                    AssemblyReference(m, nm + ".dll", None)
                    AssemblyReference(m, nm + ".exe", None)
                    AssemblyReference(m, nm + ".netmodule", None)
                ]

        match rs |> List.tryPick (fun r -> tcConfig.TryResolveLibWithDirectories(r)) with
        | Some res -> Some res
        | None ->
            match ccuLoadFailureAction with
            | CcuLoadFailureAction.RaiseError ->
                let searchMessage = String.concat "\n " (tcConfig.GetSearchPathsForLibraryFiles())
                raise (FileNameNotResolved(nm, searchMessage, m))
            | CcuLoadFailureAction.ReturnNone -> None

    member tcConfig.MsBuildResolve(references, _, errorAndWarningRange, showMessages) =
        let logMessage showMessages =
            if showMessages && tcConfig.showReferenceResolutions then
                (fun (message: string) -> printfn "%s" message)
            else
                ignore

        let logDiagnostic _ =
            (fun (_: bool) (_: string) (_: string) -> ())

        let targetProcessorArchitecture =
            match tcConfig.platform with
            | None -> "MSIL"
            | Some X86 -> "x86"
            | Some AMD64 -> "amd64"
            | Some ARM -> "arm"
            | Some ARM64 -> "arm64"
            | Some IA64 -> "ia64"

        try
            tcConfig.legacyReferenceResolver.Impl.Resolve(
                tcConfig.resolutionEnvironment,
                references,
                tcConfig.targetFrameworkVersion,
                tcConfig.GetTargetFrameworkDirectories(),
                targetProcessorArchitecture,
                tcConfig.fsharpBinariesDir, // FSharp binaries directory
                tcConfig.includes, // Explicit include directories
                tcConfig.implicitIncludeDir, // Implicit include directory (likely the project directory)
                logMessage showMessages,
                logDiagnostic showMessages
            )
        with LegacyResolutionFailure ->
            error (Error(FSComp.SR.buildAssemblyResolutionFailed (), errorAndWarningRange))

    // NOTE!! if mode=Speculative then this method must not report ANY warnings or errors through 'warning' or 'error'. Instead
    // it must return warnings and errors as data
    //
    // NOTE!! if mode=ReportErrors then this method must not raise exceptions. It must just report the errors and recover
    static member TryResolveLibsUsingMSBuildRules
        (
            tcConfig: TcConfig,
            originalReferences: AssemblyReference list,
            errorAndWarningRange: range,
            mode: ResolveAssemblyReferenceMode
        ) : AssemblyResolution list * UnresolvedAssemblyReference list =

        use _ = UseBuildPhase BuildPhase.Parameter

        if tcConfig.useSimpleResolution then
            failwith "MSBuild resolution is not supported."

        if originalReferences = [] then
            [], []
        else
            // Group references by name with range values in the grouped value list.
            // In the grouped reference, store the index of the last use of the reference.
            let groupedReferences =
                originalReferences
                |> List.indexed
                |> List.groupBy (fun (_, reference) -> reference.Text)
                |> List.map (fun (assemblyName, assemblyAndIndexGroup) ->
                    let assemblyAndIndexGroup = assemblyAndIndexGroup |> List.ofSeq
                    let highestPosition = assemblyAndIndexGroup |> List.maxBy fst |> fst
                    let assemblyGroup = assemblyAndIndexGroup |> List.map snd
                    assemblyName, highestPosition, assemblyGroup)
                |> Array.ofList

            // First, try to resolve everything as a file using simple resolution
            let resolvedAsFile =
                [|
                    for (_filename, maxIndexOfReference, references) in groupedReferences do
                        let assemblyResolution =
                            references |> List.choose (fun r -> tcConfig.TryResolveLibWithDirectories r)

                        if not assemblyResolution.IsEmpty then
                            (maxIndexOfReference, assemblyResolution)
                |]

            let toMsBuild =
                [|
                    for i in 0 .. groupedReferences.Length - 1 do
                        let ref, i0, _ = groupedReferences[i]

                        if resolvedAsFile |> Array.exists (fun (i1, _) -> i0 = i1) |> not then
                            ref, string i
                |]

            let resolutions =
                tcConfig.MsBuildResolve(toMsBuild, mode, errorAndWarningRange, true)

            // Map back to original assembly resolutions.
            let resolvedByMsbuild =
                [|
                    for resolvedFile in resolutions do
                        let i = int resolvedFile.baggage
                        let _, maxIndexOfReference, ms = groupedReferences[i]

                        let assemblyResolutions =
                            [
                                for originalReference in ms do
                                    let canonicalItemSpec = FileSystem.GetFullPathShim(resolvedFile.itemSpec)

                                    {
                                        originalReference = originalReference
                                        resolvedPath = canonicalItemSpec
                                        prepareToolTip = (fun () -> resolvedFile.prepareToolTip (originalReference.Text, canonicalItemSpec))
                                        sysdir = tcConfig.IsSystemAssembly canonicalItemSpec
                                        ilAssemblyRef = None
                                    }
                            ]

                        (maxIndexOfReference, assemblyResolutions)
                |]

            // When calculating the resulting resolutions, we're going to use the index of the reference
            // in the original specification and resort it to match the ordering that we had.
            let resultingResolutions =
                [ resolvedByMsbuild; resolvedAsFile ]
                |> Array.concat
                |> Array.sortBy fst
                |> Array.map snd
                |> List.ofArray
                |> List.concat

            // O(N^2) here over a small set of referenced assemblies.
            let IsResolved (originalName: string) =
                if
                    resultingResolutions
                    |> List.exists (fun resolution -> resolution.originalReference.Text = originalName)
                then
                    true
                else
                    // MSBuild resolution may have unified the result of two duplicate references. Try to re-resolve now.
                    // If re-resolution worked then this was a removed duplicate.
                    let references = [| (originalName, "") |]

                    let resolutions =
                        tcConfig.MsBuildResolve(references, mode, errorAndWarningRange, false)

                    resolutions.Length <> 0

            let unresolvedReferences =
                groupedReferences |> Array.filter (p13 >> IsResolved >> not) |> List.ofArray

            let unresolved =
                [
                    for (name, _, r) in unresolvedReferences -> UnresolvedAssemblyReference(name, r)
                ]

            // If mode=Speculative, then we haven't reported any errors.
            // We report the error condition by returning an empty list of resolutions
            if
                mode = ResolveAssemblyReferenceMode.Speculative
                && unresolvedReferences.Length > 0
            then
                [], unresolved
            else
                resultingResolutions, unresolved

[<Sealed>]
type TcAssemblyResolutions(tcConfig: TcConfig, results: AssemblyResolution list, unresolved: UnresolvedAssemblyReference list) =

    let originalReferenceToResolution =
        results |> List.map (fun r -> r.originalReference.Text, r) |> Map.ofList

    let resolvedPathToResolution =
        results |> List.map (fun r -> r.resolvedPath, r) |> Map.ofList

    /// Add some resolutions to the map of resolution results.
    member _.AddResolutionResults newResults =
        TcAssemblyResolutions(tcConfig, results @ newResults, unresolved)

    /// Add some unresolved results.
    member _.AddUnresolvedReferences newUnresolved =
        TcAssemblyResolutions(tcConfig, results, unresolved @ newUnresolved)

    /// Get information about referenced DLLs
    member _.GetAssemblyResolutions() = results

    member _.GetUnresolvedReferences() = unresolved

    member _.TryFindByOriginalReference(assemblyReference: AssemblyReference) =
        originalReferenceToResolution.TryFind assemblyReference.Text

    /// Only used by F# Interactive
    member _.TryFindByExactILAssemblyRef assemblyRef =
        results
        |> List.tryFind (fun ar ->
            let r =
                ar.GetILAssemblyRef(tcConfig.reduceMemoryUsage, tcConfig.tryGetMetadataSnapshot)

            r = assemblyRef)

    /// Only used by F# Interactive
    member _.TryFindBySimpleAssemblyName simpleAssemName =
        results
        |> List.tryFind (fun ar ->
            let r =
                ar.GetILAssemblyRef(tcConfig.reduceMemoryUsage, tcConfig.tryGetMetadataSnapshot)

            r.Name = simpleAssemName)

    member _.TryFindByResolvedPath nm = resolvedPathToResolution.TryFind nm

    member _.TryFindByOriginalReferenceText nm =
        originalReferenceToResolution.TryFind nm

    static member ResolveAssemblyReferences
        (
            tcConfig: TcConfig,
            assemblyList: AssemblyReference list,
            knownUnresolved: UnresolvedAssemblyReference list
        ) : TcAssemblyResolutions =
        let resolved, unresolved =
            if tcConfig.useSimpleResolution then
                let resolutions =
                    assemblyList
                    |> List.map (fun assemblyReference ->
                        try
                            let resolutionOpt =
                                tcConfig.ResolveLibWithDirectories(CcuLoadFailureAction.RaiseError, assemblyReference)

                            Choice1Of2 resolutionOpt.Value
                        with e ->
                            errorRecovery e assemblyReference.Range
                            Choice2Of2 assemblyReference)

                let successes =
                    resolutions
                    |> List.choose (function
                        | Choice1Of2 x -> Some x
                        | _ -> None)

                let failures =
                    resolutions
                    |> List.choose (function
                        | Choice2Of2 x -> Some(UnresolvedAssemblyReference(x.Text, [ x ]))
                        | _ -> None)

                successes, failures
            else
                // we don't want to do assembly resolution concurrently, we assume MSBuild doesn't handle this
                TcConfig.TryResolveLibsUsingMSBuildRules(tcConfig, assemblyList, rangeStartup, ResolveAssemblyReferenceMode.ReportErrors)

        TcAssemblyResolutions(tcConfig, resolved, unresolved @ knownUnresolved)

    static member GetAllDllReferences(tcConfig: TcConfig) =
        [
            let primaryReference = tcConfig.PrimaryAssemblyDllReference()

            let assumeDotNetFramework = primaryReference.SimpleAssemblyNameIs("mscorlib")

            if not tcConfig.compilingFSharpCore then
                tcConfig.CoreLibraryDllReference()

                if assumeDotNetFramework then
                    // When building desktop then we need these additional dependencies
                    AssemblyReference(rangeStartup, "System.Numerics.dll", None)
                    AssemblyReference(rangeStartup, "System.dll", None)
                    let asm = AssemblyReference(rangeStartup, "netstandard.dll", None)

                    let found =
                        if tcConfig.useSimpleResolution then
                            match tcConfig.ResolveLibWithDirectories(CcuLoadFailureAction.ReturnNone, asm) with
                            | Some _ -> true
                            | None -> false
                        else
                            let references = [| (asm.Text, "") |]

                            let resolutions =
                                tcConfig.MsBuildResolve(references, ResolveAssemblyReferenceMode.Speculative, rangeStartup, false)

                            resolutions.Length = 1

                    if found then
                        asm

            if tcConfig.implicitlyReferenceDotNetAssemblies then
                let references, _useDotNetFramework =
                    tcConfig.FxResolver.GetDefaultReferences(tcConfig.useFsiAuxLib)

                for s in references do
                    let referenceText =
                        if s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) then
                            s
                        else
                            s + ".dll"

                    AssemblyReference(rangeStartup, referenceText, None)

            yield! tcConfig.referencedDLLs
        ]

    static member SplitNonFoundationalResolutions(tcConfig: TcConfig) =
        let assemblyList = TcAssemblyResolutions.GetAllDllReferences tcConfig

        let resolutions =
            TcAssemblyResolutions.ResolveAssemblyReferences(tcConfig, assemblyList, tcConfig.knownUnresolvedReferences)

        let frameworkDLLs, nonFrameworkReferences =
            resolutions.GetAssemblyResolutions() |> List.partition (fun r -> r.sysdir)

        let unresolved = resolutions.GetUnresolvedReferences()
#if DEBUG
        let mutable itFailed = false

        let addedText =
            "\nIf you want to debug this right now, attach a debugger, and put a breakpoint in 'CompileOps.fs' near the text '!itFailed', and you can re-step through the assembly resolution logic."

        for UnresolvedAssemblyReference (referenceText, _ranges) in unresolved do
            if referenceText.Contains("mscorlib") then
                Debug.Assert(false, sprintf "whoops, did not resolve mscorlib: '%s'%s" referenceText addedText)
                itFailed <- true

        for x in frameworkDLLs do
            if not (FileSystem.IsPathRootedShim(x.resolvedPath)) then
                Debug.Assert(false, sprintf "frameworkDLL should be absolute path: '%s'%s" x.resolvedPath addedText)
                itFailed <- true

        for x in nonFrameworkReferences do
            if not (FileSystem.IsPathRootedShim(x.resolvedPath)) then
                Debug.Assert(false, sprintf "nonFrameworkReference should be absolute path: '%s'%s" x.resolvedPath addedText)
                itFailed <- true

        if itFailed then
            // idea is, put a breakpoint here and then step through
            let assemblyList = TcAssemblyResolutions.GetAllDllReferences tcConfig

            let resolutions =
                TcAssemblyResolutions.ResolveAssemblyReferences(tcConfig, assemblyList, [])

            let _frameworkDLLs, _nonFrameworkReferences =
                resolutions.GetAssemblyResolutions() |> List.partition (fun r -> r.sysdir)

            ()
#endif
        frameworkDLLs, nonFrameworkReferences, unresolved

    static member BuildFromPriorResolutions(tcConfig: TcConfig, resolutions, knownUnresolved) =
        let references = resolutions |> List.map (fun r -> r.originalReference)
        TcAssemblyResolutions.ResolveAssemblyReferences(tcConfig, references, knownUnresolved)

    static member GetAssemblyResolutionInformation(tcConfig: TcConfig) =
        use _ = UseBuildPhase BuildPhase.Parameter
        let assemblyList = TcAssemblyResolutions.GetAllDllReferences tcConfig

        let resolutions =
            TcAssemblyResolutions.ResolveAssemblyReferences(tcConfig, assemblyList, [])

        resolutions.GetAssemblyResolutions(), resolutions.GetUnresolvedReferences()

//----------------------------------------------------------------------------
// Abstraction for project reference

let GetNameOfILModule (m: ILModuleDef) =
    match m.Manifest with
    | Some manifest -> manifest.Name
    | None -> m.Name

let MakeScopeRefForILModule (ilModule: ILModuleDef) =
    match ilModule.Manifest with
    | Some m -> ILScopeRef.Assembly(mkRefToILAssembly m)
    | None -> ILScopeRef.Module(mkRefToILModule ilModule)

let GetCustomAttributesOfILModule (ilModule: ILModuleDef) =
    let attrs =
        match ilModule.Manifest with
        | Some m -> m.CustomAttrs
        | None -> ilModule.CustomAttrs

    attrs.AsList()

let GetAutoOpenAttributes ilModule =
    ilModule |> GetCustomAttributesOfILModule |> List.choose TryFindAutoOpenAttr

let GetInternalsVisibleToAttributes ilModule =
    ilModule
    |> GetCustomAttributesOfILModule
    |> List.choose TryFindInternalsVisibleToAttr

type RawFSharpAssemblyDataBackedByFileOnDisk(ilModule: ILModuleDef, ilAssemblyRefs) =
    let externalSigAndOptData = [ "FSharp.Core" ]

    interface IRawFSharpAssemblyData with

        member _.GetAutoOpenAttributes() = GetAutoOpenAttributes ilModule

        member _.GetInternalsVisibleToAttributes() =
            GetInternalsVisibleToAttributes ilModule

        member _.TryGetILModuleDef() = Some ilModule

        member _.GetRawFSharpSignatureData(m, ilShortAssemName, fileName) =
            let resources = ilModule.Resources.AsList()

            let sigDataReaders =
                [
                    for r in resources do
                        if IsSignatureDataResource r then
                            GetResourceNameAndSignatureDataFunc r
                ]

            let sigDataReaders =
                if sigDataReaders.IsEmpty && List.contains ilShortAssemName externalSigAndOptData then
                    let sigFileName = Path.ChangeExtension(fileName, "sigdata")

                    if not (FileSystem.FileExistsShim sigFileName) then
                        error (Error(FSComp.SR.buildExpectedSigdataFile (FileSystem.GetFullPathShim sigFileName), m))

                    [
                        (ilShortAssemName,
                         fun () ->
                             FileSystem
                                 .OpenFileForReadShim(sigFileName, useMemoryMappedFile = true, shouldShadowCopy = true)
                                 .AsByteMemory()
                                 .AsReadOnly())
                    ]
                else
                    sigDataReaders

            sigDataReaders

        member _.GetRawFSharpOptimizationData(m, ilShortAssemName, fileName) =
            let optDataReaders =
                ilModule.Resources.AsList()
                |> List.choose (fun r ->
                    if IsOptimizationDataResource r then
                        Some(GetResourceNameAndOptimizationDataFunc r)
                    else
                        None)

            // Look for optimization data in a file
            let optDataReaders =
                if optDataReaders.IsEmpty && List.contains ilShortAssemName externalSigAndOptData then
                    let optDataFile = Path.ChangeExtension(fileName, "optdata")

                    if not (FileSystem.FileExistsShim optDataFile) then
                        let fullPath = FileSystem.GetFullPathShim optDataFile
                        error (Error(FSComp.SR.buildExpectedFileAlongSideFSharpCore (optDataFile, fullPath), m))

                    [
                        (ilShortAssemName,
                         (fun () ->
                             FileSystem
                                 .OpenFileForReadShim(optDataFile, useMemoryMappedFile = true, shouldShadowCopy = true)
                                 .AsByteMemory()
                                 .AsReadOnly()))
                    ]
                else
                    optDataReaders

            optDataReaders

        member _.GetRawTypeForwarders() =
            match ilModule.Manifest with
            | Some manifest -> manifest.ExportedTypes
            | None -> mkILExportedTypes []

        member _.ShortAssemblyName = GetNameOfILModule ilModule

        member _.ILScopeRef = MakeScopeRefForILModule ilModule

        member _.ILAssemblyRefs = ilAssemblyRefs

        member _.HasAnyFSharpSignatureDataAttribute =
            let attrs = GetCustomAttributesOfILModule ilModule
            List.exists IsSignatureDataVersionAttr attrs

        member _.HasMatchingFSharpSignatureDataAttribute =
            let attrs = GetCustomAttributesOfILModule ilModule
            List.exists (IsMatchingSignatureDataVersionAttr(parseILVersion FSharpBinaryMetadataFormatRevision)) attrs

[<Sealed>]
type RawFSharpAssemblyData(ilModule: ILModuleDef, ilAssemblyRefs) =

    interface IRawFSharpAssemblyData with

        member _.GetAutoOpenAttributes() = GetAutoOpenAttributes ilModule

        member _.GetInternalsVisibleToAttributes() =
            GetInternalsVisibleToAttributes ilModule

        member _.TryGetILModuleDef() = Some ilModule

        member _.GetRawFSharpSignatureData(_, _, _) =
            let resources = ilModule.Resources.AsList()

            [
                for r in resources do
                    if IsSignatureDataResource r then
                        GetResourceNameAndSignatureDataFunc r
            ]

        member _.GetRawFSharpOptimizationData(_, _, _) =
            ilModule.Resources.AsList()
            |> List.choose (fun r ->
                if IsOptimizationDataResource r then
                    Some(GetResourceNameAndOptimizationDataFunc r)
                else
                    None)

        member _.GetRawTypeForwarders() =
            match ilModule.Manifest with
            | Some manifest -> manifest.ExportedTypes
            | None -> mkILExportedTypes []

        member _.ShortAssemblyName = GetNameOfILModule ilModule

        member _.ILScopeRef = MakeScopeRefForILModule ilModule

        member _.ILAssemblyRefs = ilAssemblyRefs

        member _.HasAnyFSharpSignatureDataAttribute =
            let attrs = GetCustomAttributesOfILModule ilModule
            List.exists IsSignatureDataVersionAttr attrs

        member _.HasMatchingFSharpSignatureDataAttribute =
            let attrs = GetCustomAttributesOfILModule ilModule
            List.exists (IsMatchingSignatureDataVersionAttr(parseILVersion FSharpBinaryMetadataFormatRevision)) attrs

//----------------------------------------------------------------------------
// TcImports
//--------------------------------------------------------------------------

[<Sealed>]
type TcImportsSafeDisposal
    (
        tciLock: TcImportsLock,
        disposeActions: ResizeArray<unit -> unit>,
        disposeTypeProviderActions: ResizeArray<unit -> unit>
    ) =

    let mutable isDisposed = false

    let dispose () =
        tciLock.AcquireLock(fun tcitok ->

            RequireTcImportsLock(tcitok, isDisposed)
            RequireTcImportsLock(tcitok, disposeTypeProviderActions)
            RequireTcImportsLock(tcitok, disposeActions)

            // disposing deliberately only closes this tcImports, not the ones up the chain
            isDisposed <- true

            let actions1 = disposeTypeProviderActions |> Seq.toArray
            let actions2 = disposeActions |> Seq.toArray

            disposeTypeProviderActions.Clear()
            disposeActions.Clear()

            for action in actions1 do
                action ()

            for action in actions2 do
                action ())

    override _.Finalize() = dispose ()

    interface IDisposable with

        member this.Dispose() =
            if not isDisposed then
                GC.SuppressFinalize this
                dispose ()

#if !NO_TYPEPROVIDERS

// TcImports is held as a weak reference inside a TypeProviderConfig.
//
// Due to various historical bugs with the ReferencedAssemblies property of TypeProviderConfig,
// type providers compiled using the TypeProvider SDK have picked up the unfortunate habit of using
// private reflection on the TypeProviderConfig to correctly determine the ReferencedAssemblies.
// These types thus also act as a stable facade supporting exactly the private reflection that is
// used by type providers built with the TypeProvider SDK.
//
// The use of private reflection is highly unfortunate but has historically been the only way to
// unblock several important type providers such as FSharp.Data when the weaknesses in reported
// ReferencedAssemblies were determined.
//
// The use of private reflection was removed from the TypeProvider SDK, see
//     https://github.com/fsprojects/FSharp.TypeProviders.SDK/pull/305. But we still need to work on older type providers.
//
// however it was then reinstated
//
//    https://github.com/fsprojects/FSharp.TypeProviders.SDK/pull/388
//
// All known issues TypeProviderConfig::ReferencedAssemblies are now fixed, meaning one day
// we can remove the use of private reflection from the TPSDK (that is, once the F# tooling fixes
// can be assumed to be shipped in all F# tooling where type providers have to load). After that,
// once all type providers are updated, we will no longer need to have this fixed facade.

/// This acts as a stable type supporting the
type TcImportsDllInfoFacade = { FileName: string }

type TcImportsWeakFacade(tciLock: TcImportsLock, tcImportsWeak: WeakReference<TcImports>) =

    let mutable dllInfos: TcImportsDllInfoFacade list = []

    // The name of these fields must not change, see above
    do assert (nameof (dllInfos) = "dllInfos")

    member _.SetDllInfos(value: ImportedBinary list) =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, dllInfos)

            let infos =
                [
                    for x in value do
                        let info = { FileName = x.FileName }
                        // The name of this field must not change, see above
                        assert (nameof (info.FileName) = "FileName")
                        info
                ]

            dllInfos <- infos)

    member this.Base: TcImportsWeakFacade option =
        // The name of this property msut not change, see above
        assert (nameof (this.Base) = "Base")

        match tcImportsWeak.TryGetTarget() with
        | true, tcImports ->
            match tcImports.Base with
            | Some (baseTcImports: TcImports) -> Some baseTcImports.Weak
            | _ -> None
        | _ -> None

    member _.SystemRuntimeContainsType typeName =
        match tcImportsWeak.TryGetTarget() with
        | true, tcImports -> tcImports.SystemRuntimeContainsType typeName
        | _ -> false

    member _.AllAssemblyResolutions() =
        match tcImportsWeak.TryGetTarget() with
        | true, tcImports -> tcImports.AllAssemblyResolutions()
        | _ -> []

#endif
/// Represents a table of imported assemblies with their resolutions.
/// Is a disposable object, but it is recommended not to explicitly call Dispose unless you absolutely know nothing will be using its contents after the disposal.
/// Otherwise, simply allow the GC to collect this and it will properly call Dispose from the finalizer.
and [<Sealed>] TcImports
    (
        tcConfigP: TcConfigProvider,
        initialResolutions: TcAssemblyResolutions,
        importsBase: TcImports option,
        dependencyProviderOpt: DependencyProvider option
    ) as this
#if !NO_TYPEPROVIDERS
#endif
 =

    let tciLock = TcImportsLock()

    //---- Start protected by tciLock -------
    let mutable resolutions = initialResolutions
    let mutable dllInfos: ImportedBinary list = []
    let mutable dllTable: NameMap<ImportedBinary> = NameMap.empty
    let mutable ccuInfos: ImportedAssembly list = []
    let mutable ccuTable: NameMap<ImportedAssembly> = NameMap.empty
    let mutable ccuThunks = ResizeArray<CcuThunk * (unit -> unit)>()
    let disposeActions = ResizeArray()
    let disposeTypeProviderActions = ResizeArray()

#if !NO_TYPEPROVIDERS
    let mutable generatedTypeRoots =
        Dictionary<ILTypeRef, int * ProviderGeneratedType>()

    let tcImportsWeak = TcImportsWeakFacade(tciLock, WeakReference<_> this)
#endif

    let disposal =
        new TcImportsSafeDisposal(tciLock, disposeActions, disposeTypeProviderActions)
    //---- End protected by tciLock -------

    let mutable disposed = false // this doesn't need locking, it's only for debugging
    let mutable tcGlobals = None // this doesn't need locking, it's set during construction of the TcImports

    let CheckDisposed () =
        if disposed then
            assert false

    let dispose () =
        CheckDisposed()
        (disposal :> IDisposable).Dispose()

    //   Get a snapshot of the current unFixedUp ccuThunks.
    //   for each of those thunks, remove them from the dictionary, so any parallel threads can't do this work
    //      If it successfully removed it from the dictionary then do the fixup
    //          If the thunk remains unresolved add it back to the ccuThunks dictionary for further processing
    //      If not then move on to the next thunk
    let fixupOrphanCcus () =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, ccuThunks)
            let contents = ccuThunks |> Seq.toArray

            let unsuccessful =
                [
                    for ccuThunk, func in contents do
                        if ccuThunk.IsUnresolvedReference then
                            func ()

                        if ccuThunk.IsUnresolvedReference then
                            (ccuThunk, func)
                ]

            ccuThunks <- ResizeArray unsuccessful)

    let availableToOptionalCcu =
        function
        | ResolvedCcu ccu -> Some ccu
        | UnresolvedCcu _ -> None

    static let ccuHasType (ccu: CcuThunk) (nsname: string list) (tname: string) =
        let matchNameSpace (entityOpt: Entity option) n =
            match entityOpt with
            | None -> None
            | Some entity -> entity.ModuleOrNamespaceType.AllEntitiesByCompiledAndLogicalMangledNames.TryFind n

        match (Some ccu.Contents, nsname) ||> List.fold matchNameSpace with
        | Some ns ->
            match Map.tryFind tname ns.ModuleOrNamespaceType.TypesByMangledName with
            | Some _ -> true
            | None -> false
        | None -> false

    member internal _.Base =
        CheckDisposed()
        importsBase

    member _.CcuTable =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, ccuTable)
            CheckDisposed()
            ccuTable)

    member _.DllTable =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, dllTable)
            CheckDisposed()
            dllTable)

#if !NO_TYPEPROVIDERS
    member _.Weak =
        CheckDisposed()
        tcImportsWeak
#endif

    member _.RegisterCcu ccuInfo =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, ccuInfos)
            RequireTcImportsLock(tcitok, ccuTable)
            ccuInfos <- ccuInfos ++ ccuInfo
            // Assembly Ref Resolution: remove this use of ccu.AssemblyName
            ccuTable <- NameMap.add ccuInfo.FSharpViewOfMetadata.AssemblyName ccuInfo ccuTable)

    member _.RegisterDll dllInfo =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, dllInfos)
            RequireTcImportsLock(tcitok, dllTable)
            dllInfos <- dllInfos ++ dllInfo
#if !NO_TYPEPROVIDERS
            tcImportsWeak.SetDllInfos dllInfos
#endif
            dllTable <- NameMap.add (getNameOfScopeRef dllInfo.ILScopeRef) dllInfo dllTable)

    member _.GetDllInfos() : ImportedBinary list =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, dllInfos)

            match importsBase with
            | Some importsBase -> importsBase.GetDllInfos() @ dllInfos
            | None -> dllInfos)

    member _.AllAssemblyResolutions() =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, resolutions)
            let ars = resolutions.GetAssemblyResolutions()

            match importsBase with
            | Some importsBase -> importsBase.AllAssemblyResolutions() @ ars
            | None -> ars)

    member tcImports.TryFindDllInfo(ctok: CompilationThreadToken, m, assemblyName, lookupOnly) =
        CheckDisposed()

        let rec look (t: TcImports) =
            match NameMap.tryFind assemblyName t.DllTable with
            | Some res -> Some res
            | None ->
                match t.Base with
                | Some t2 -> look t2
                | None -> None

        match look tcImports with
        | Some res -> Some res
        | None ->
            tcImports.ImplicitLoadIfAllowed(ctok, m, assemblyName, lookupOnly)
            look tcImports

    member tcImports.FindDllInfo(ctok, m, assemblyName) =
        match tcImports.TryFindDllInfo(ctok, m, assemblyName, lookupOnly = false) with
        | Some res -> res
        | None -> error (Error(FSComp.SR.buildCouldNotResolveAssembly assemblyName, m))

    member _.GetImportedAssemblies() =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, ccuInfos)

            match importsBase with
            | Some importsBase -> List.append (importsBase.GetImportedAssemblies()) ccuInfos
            | None -> ccuInfos)

    member _.GetCcusExcludingBase() =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, ccuInfos)
            ccuInfos |> List.map (fun x -> x.FSharpViewOfMetadata))

    member tcImports.GetCcusInDeclOrder() =
        CheckDisposed()
        List.map (fun x -> x.FSharpViewOfMetadata) (tcImports.GetImportedAssemblies())

    // This is the main "assembly reference --> assembly" resolution routine.
    member tcImports.FindCcuInfo(ctok, m, assemblyName, lookupOnly) =
        CheckDisposed()

        let rec look (t: TcImports) =
            match NameMap.tryFind assemblyName t.CcuTable with
            | Some res -> Some res
            | None ->
                match t.Base with
                | Some t2 -> look t2
                | None -> None

        match look tcImports with
        | Some res -> ResolvedImportedAssembly(res, m)
        | None ->
            tcImports.ImplicitLoadIfAllowed(ctok, m, assemblyName, lookupOnly)

            match look tcImports with
            | Some res -> ResolvedImportedAssembly(res, m)
            | None -> UnresolvedImportedAssembly(assemblyName, m)

    member tcImports.FindCcu(ctok, m, assemblyName, lookupOnly) =
        CheckDisposed()

        match tcImports.FindCcuInfo(ctok, m, assemblyName, lookupOnly) with
        | ResolvedImportedAssembly (importedAssembly, _) -> ResolvedCcu(importedAssembly.FSharpViewOfMetadata)
        | UnresolvedImportedAssembly (assemblyName, _) -> UnresolvedCcu assemblyName

    member tcImports.FindCcuFromAssemblyRef(ctok, m, assemblyRef: ILAssemblyRef) =
        CheckDisposed()

        match tcImports.FindCcuInfo(ctok, m, assemblyRef.Name, lookupOnly = false) with
        | ResolvedImportedAssembly (importedAssembly, _) -> ResolvedCcu(importedAssembly.FSharpViewOfMetadata)
        | UnresolvedImportedAssembly _ -> UnresolvedCcu(assemblyRef.QualifiedName)

    member tcImports.TryFindXmlDocumentationInfo(assemblyName: string) =
        CheckDisposed()

        let rec look (t: TcImports) =
            match NameMap.tryFind assemblyName t.CcuTable with
            | Some res -> Some res
            | None ->
                match t.Base with
                | Some t2 -> look t2
                | None -> None

        match look tcImports with
        | Some res -> res.FSharpViewOfMetadata.Deref.XmlDocumentationInfo
        | _ -> None

#if !NO_TYPEPROVIDERS
    member tcImports.GetProvidedAssemblyInfo(ctok, m, assembly: Tainted<ProvidedAssembly MaybeNull>) =
        match assembly with
        | Tainted.Null -> false, None
        | Tainted.NonNull assembly ->
            let aname = assembly.PUntaint((fun a -> a.GetName()), m)
            let ilShortAssemName = aname.Name

            match tcImports.FindCcu(ctok, m, ilShortAssemName, lookupOnly = true) with
            | ResolvedCcu ccu ->
                if ccu.IsProviderGenerated then
                    let dllinfo = tcImports.FindDllInfo(ctok, m, ilShortAssemName)
                    true, dllinfo.ProviderGeneratedStaticLinkMap
                else
                    false, None

            | UnresolvedCcu _ ->
                let g = tcImports.GetTcGlobals()
                let ilScopeRef = ILScopeRef.Assembly(ILAssemblyRef.FromAssemblyName aname)
                let fileName = aname.Name + ".dll"

                let bytes =
                    assembly
                        .PApplyWithProvider((fun (assembly, provider) -> assembly.GetManifestModuleContents provider), m)
                        .PUntaint(id, m)

                let tcConfig = tcConfigP.Get ctok

                let ilModule, ilAssemblyRefs =
                    let opts: ILReaderOptions =
                        {
                            reduceMemoryUsage = tcConfig.reduceMemoryUsage
                            pdbDirPath = None
                            metadataOnly = MetadataOnlyFlag.Yes
                            tryGetMetadataSnapshot = tcConfig.tryGetMetadataSnapshot
                        }

                    let reader = OpenILModuleReaderFromBytes fileName bytes opts
                    reader.ILModuleDef, reader.ILAssemblyRefs

                let theActualAssembly = assembly.PUntaint((fun x -> x.Handle), m)

                let dllinfo =
                    {
                        RawMetadata = RawFSharpAssemblyDataBackedByFileOnDisk(ilModule, ilAssemblyRefs)
                        FileName = fileName
                        ProviderGeneratedAssembly = Some theActualAssembly
                        IsProviderGenerated = true
                        ProviderGeneratedStaticLinkMap =
                            if g.isInteractive then
                                None
                            else
                                Some(ProvidedAssemblyStaticLinkingMap.CreateNew())
                        ILScopeRef = ilScopeRef
                        ILAssemblyRefs = ilAssemblyRefs
                    }

                tcImports.RegisterDll dllinfo

                let ccuContents =
                    Construct.NewCcuContents ilScopeRef m ilShortAssemName (Construct.NewEmptyModuleOrNamespaceType(Namespace true))

                let ccuData: CcuData =
                    {
                        IsFSharp = false
                        UsesFSharp20PlusQuotations = false
                        InvalidateEvent = (Event<_>()).Publish
                        IsProviderGenerated = true
                        QualifiedName = Some(assembly.PUntaint((fun a -> a.FullName), m))
                        Contents = ccuContents
                        ILScopeRef = ilScopeRef
                        Stamp = newStamp ()
                        SourceCodeDirectory = ""
                        FileName = Some fileName
                        MemberSignatureEquality = (fun ty1 ty2 -> typeEquivAux EraseAll g ty1 ty2)
                        ImportProvidedType = (fun ty -> ImportProvidedType (tcImports.GetImportMap()) m ty)
                        TryGetILModuleDef = (fun () -> Some ilModule)
                        TypeForwarders = CcuTypeForwarderTable.Empty
                        XmlDocumentationInfo =
                            match tcConfig.xmlDocInfoLoader with
                            | Some xmlDocInfoLoader -> xmlDocInfoLoader.TryLoad(fileName)
                            | _ -> None
                    }

                let ccu = CcuThunk.Create(ilShortAssemName, ccuData)

                let ccuinfo =
                    {
                        FSharpViewOfMetadata = ccu
                        ILScopeRef = ilScopeRef
                        AssemblyAutoOpenAttributes = []
                        AssemblyInternalsVisibleToAttributes = []
                        IsProviderGenerated = true
                        TypeProviders = []
                        FSharpOptimizationData = notlazy None
                    }

                tcImports.RegisterCcu ccuinfo
                // Yes, it is generative
                true, dllinfo.ProviderGeneratedStaticLinkMap

    member _.RecordGeneratedTypeRoot root =
        tciLock.AcquireLock(fun tcitok ->
            // checking if given ProviderGeneratedType was already recorded before (probably for another set of static parameters)
            let (ProviderGeneratedType (_, ilTyRef, _)) = root

            let index =
                RequireTcImportsLock(tcitok, generatedTypeRoots)

                match generatedTypeRoots.TryGetValue ilTyRef with
                | true, (index, _) -> index
                | false, _ -> generatedTypeRoots.Count

            generatedTypeRoots[ilTyRef] <- (index, root))

    member _.ProviderGeneratedTypeRoots =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, generatedTypeRoots)
            generatedTypeRoots.Values |> Seq.sortBy fst |> Seq.map snd |> Seq.toList)
#endif

    member private tcImports.AttachDisposeAction action =
        tciLock.AcquireLock(fun tcitok ->
            CheckDisposed()
            RequireTcImportsLock(tcitok, disposeActions)
            disposeActions.Add action)

#if !NO_TYPEPROVIDERS
    member private tcImports.AttachDisposeTypeProviderAction action =
        CheckDisposed()
        disposeTypeProviderActions.Add action
#endif

    // Note: the returned binary reader is associated with the tcImports, i.e. when the tcImports are closed
    // then the reader is closed.
    member tcImports.OpenILBinaryModule(ctok, fileName, m) =
        try
            CheckDisposed()
            let tcConfig = tcConfigP.Get ctok

            let pdbDirPath =
                // We open the pdb file if one exists parallel to the binary we
                // are reading, so that --standalone will preserve debug information.
                if tcConfig.openDebugInformationForLaterStaticLinking then
                    let pdbDir =
                        try
                            FileSystem.GetDirectoryNameShim fileName
                        with _ ->
                            "."

                    let pdbFile =
                        (try
                            FileSystemUtils.chopExtension fileName
                         with _ ->
                             fileName)
                        + ".pdb"

                    if FileSystem.FileExistsShim pdbFile then
                        Some pdbDir
                    else
                        None
                else
                    None

            let ilILBinaryReader =
                OpenILBinary(
                    fileName,
                    tcConfig.reduceMemoryUsage,
                    pdbDirPath,
                    tcConfig.shadowCopyReferences,
                    tcConfig.tryGetMetadataSnapshot
                )

            tcImports.AttachDisposeAction(fun _ -> (ilILBinaryReader :> IDisposable).Dispose())
            ilILBinaryReader.ILModuleDef, ilILBinaryReader.ILAssemblyRefs
        with e ->
            error (Error(FSComp.SR.buildErrorOpeningBinaryFile (fileName, e.Message), m))

    (* auxModTable is used for multi-module assemblies *)
    member tcImports.MkLoaderForMultiModuleILAssemblies ctok m =
        CheckDisposed()
        let auxModTable = HashMultiMap(10, HashIdentity.Structural)

        fun viewedScopeRef ->

            let tcConfig = tcConfigP.Get ctok

            match viewedScopeRef with
            | ILScopeRef.Module modref ->
                let key = modref.Name

                if not (auxModTable.ContainsKey key) then
                    let resolution =
                        tcConfig.ResolveLibWithDirectories(CcuLoadFailureAction.RaiseError, AssemblyReference(m, key, None))
                        |> Option.get

                    let ilModule, _ = tcImports.OpenILBinaryModule(ctok, resolution.resolvedPath, m)
                    auxModTable[key] <- ilModule

                auxModTable[key]

            | _ -> error (InternalError("Unexpected ILScopeRef.Local or ILScopeRef.Assembly in exported type table", m))

    member tcImports.IsAlreadyRegistered nm =
        CheckDisposed()

        tcImports.GetDllInfos()
        |> List.exists (fun dll ->
            match dll.ILScopeRef with
            | ILScopeRef.Assembly a -> a.Name = nm
            | _ -> false)

    member _.DependencyProvider =
        CheckDisposed()

        match dependencyProviderOpt with
        | None ->
            Debug.Assert(false, "this should never be called on FrameworkTcImports")
            new DependencyProvider()
        | Some dependencyProvider -> dependencyProvider

    member tcImports.GetImportMap() =
        CheckDisposed()

        let loaderInterface =
#if NO_TYPEPROVIDERS
            { new AssemblyLoader with
                member _.FindCcuFromAssemblyRef(ctok, m, ilAssemblyRef) =
                    tcImports.FindCcuFromAssemblyRef(ctok, m, ilAssemblyRef)

                member _.TryFindXmlDocumentationInfo assemblyName =
                    tcImports.TryFindXmlDocumentationInfo(assemblyName)
            }
#else
            { new AssemblyLoader with
                member _.FindCcuFromAssemblyRef(ctok, m, ilAssemblyRef) =
                    tcImports.FindCcuFromAssemblyRef(ctok, m, ilAssemblyRef)

                member _.TryFindXmlDocumentationInfo assemblyName =
                    tcImports.TryFindXmlDocumentationInfo(assemblyName)

                member _.GetProvidedAssemblyInfo(ctok, m, assembly) =
                    tcImports.GetProvidedAssemblyInfo(ctok, m, assembly)

                member _.RecordGeneratedTypeRoot root = tcImports.RecordGeneratedTypeRoot root
            }
#endif
        ImportMap(tcImports.GetTcGlobals(), loaderInterface)

    // Note the tcGlobals are only available once mscorlib and fslib have been established. For TcImports,
    // they are logically only needed when converting AbsIL data structures into F# data structures, and
    // when converting AbsIL types in particular, since these types are normalized through the tables
    // in the tcGlobals (E.g. normalizing 'System.Int32' to 'int'). On the whole ImportILAssembly doesn't
    // actually convert AbsIL types - it only converts the outer shell of type definitions - the vast majority of
    // types such as those in method signatures are currently converted on-demand. However ImportILAssembly does have to
    // convert the types that are constraints in generic parameters, which was the original motivation for making sure that
    // ImportILAssembly had a tcGlobals available when it really needs it.
    member _.GetTcGlobals() : TcGlobals =
        CheckDisposed()

        match tcGlobals with
        | Some g -> g
        | None ->
            match importsBase with
            | Some b -> b.GetTcGlobals()
            | None -> failwith "unreachable: GetGlobals - are the references to mscorlib.dll and FSharp.Core.dll valid?"

    member private tcImports.SetTcGlobals g =
        CheckDisposed()
        tcGlobals <- Some g

#if !NO_TYPEPROVIDERS
    member private tcImports.InjectProvidedNamespaceOrTypeIntoEntity
        (
            typeProviderEnvironment,
            tcConfig: TcConfig,
            m,
            entity: Entity,
            injectedNamespace,
            remainingNamespace,
            provider,
            st: Tainted<ProvidedType> option
        ) =
        match remainingNamespace with
        | next :: rest ->
            // Inject the namespace entity
            match entity.ModuleOrNamespaceType.ModulesAndNamespacesByDemangledName.TryFind next with
            | Some childEntity ->
                tcImports.InjectProvidedNamespaceOrTypeIntoEntity(
                    typeProviderEnvironment,
                    tcConfig,
                    m,
                    childEntity,
                    next :: injectedNamespace,
                    rest,
                    provider,
                    st
                )
            | None ->
                // Build up the artificial namespace if there is not a real one.
                let cpath =
                    CompPath(
                        ILScopeRef.Local,
                        injectedNamespace
                        |> List.rev
                        |> List.map (fun n -> (n, ModuleOrNamespaceKind.Namespace true))
                    )

                let mid = ident (next, rangeStartup)
                let mty = Construct.NewEmptyModuleOrNamespaceType(Namespace true)

                let newNamespace =
                    Construct.NewModuleOrNamespace (Some cpath) taccessPublic mid XmlDoc.Empty [] (MaybeLazy.Strict mty)

                entity.ModuleOrNamespaceType.AddModuleOrNamespaceByMutation newNamespace

                tcImports.InjectProvidedNamespaceOrTypeIntoEntity(
                    typeProviderEnvironment,
                    tcConfig,
                    m,
                    newNamespace,
                    next :: injectedNamespace,
                    rest,
                    provider,
                    st
                )
        | [] ->
            match st with
            | Some st ->
                // Inject the wrapper type into the provider assembly.
                //
                // Generated types get properly injected into the provided (i.e. generated) assembly CCU in tc.fs

                let importProvidedType t =
                    ImportProvidedType (tcImports.GetImportMap()) m t

                let isSuppressRelocate =
                    tcConfig.isInteractive || st.PUntaint((fun st -> st.IsSuppressRelocate), m)

                let newEntity =
                    Construct.NewProvidedTycon(typeProviderEnvironment, st, importProvidedType, isSuppressRelocate, m)

                entity.ModuleOrNamespaceType.AddProvidedTypeEntity newEntity
            | None -> ()

            entity.entity_tycon_repr <-
                match entity.TypeReprInfo with
                // This is the first extension
                | TNoRepr -> TProvidedNamespaceRepr(typeProviderEnvironment, [ provider ])

                // Add to the existing list of extensions
                | TProvidedNamespaceRepr (resolutionFolder, prior) as repr ->
                    if not (prior |> List.exists (fun r -> Tainted.EqTainted r provider)) then
                        TProvidedNamespaceRepr(resolutionFolder, provider :: prior)
                    else
                        repr

                | _ -> failwith "Unexpected representation in namespace entity referred to by a type provider"

    member tcImportsStrong.ImportTypeProviderExtensions
        (
            ctok,
            tcConfig: TcConfig,
            fileNameOfRuntimeAssembly,
            ilScopeRefOfRuntimeAssembly,
            runtimeAssemblyAttributes: ILAttribute list,
            entityToInjectInto,
            invalidateCcu: Event<_>,
            m
        ) =

        let startingErrorCount = DiagnosticsThreadStatics.DiagnosticsLogger.ErrorCount

        // Find assembly level TypeProviderAssemblyAttributes. These will point to the assemblies that
        // have class which implement ITypeProvider and which have TypeProviderAttribute on them.
        let designTimeAssemblyNames =
            runtimeAssemblyAttributes
            |> List.choose TryDecodeTypeProviderAssemblyAttr
            // If no design-time assembly is specified, use the runtime assembly
            |> List.map (function
                | Null -> fileNameOfRuntimeAssembly
                | NonNull s -> s)
            // For each simple name of a design-time assembly, we take the first matching one in the order they are
            // specified in the attributes
            |> List.distinctBy (fun s ->
                try
                    Path.GetFileNameWithoutExtension s
                with _ ->
                    s)

        if not (List.isEmpty designTimeAssemblyNames) then

            // Find the SystemRuntimeAssemblyVersion value to report in the TypeProviderConfig.
            let primaryAssemblyVersion =
                let primaryAssemblyRef = tcConfig.PrimaryAssemblyDllReference()

                let resolution =
                    tcConfig.ResolveLibWithDirectories(CcuLoadFailureAction.RaiseError, primaryAssemblyRef)
                    |> Option.get
                // MSDN: this method causes the file to be opened and closed, but the assembly is not added to this domain
                let name = AssemblyName.GetAssemblyName(resolution.resolvedPath)
                name.Version

            // Note, this only captures systemRuntimeContainsTypeRef (which captures tcImportsWeak, using name tcImports)
            let systemRuntimeContainsType =
                let tcImports = tcImportsWeak

                // The name of this captured value must not change, see comments on TcImportsWeakFacade above
                assert (nameof (tcImports) = "tcImports")

                let mutable systemRuntimeContainsTypeRef =
                    (fun typeName -> tcImports.SystemRuntimeContainsType typeName)

                // When the tcImports is disposed the systemRuntimeContainsTypeRef thunk is replaced
                // with one raising an exception.
                tcImportsStrong.AttachDisposeTypeProviderAction(fun () ->
                    systemRuntimeContainsTypeRef <- fun _ -> raise (ObjectDisposedException("The type provider has been disposed")))

                (fun arg -> systemRuntimeContainsTypeRef arg)

            // Note, this only captures tcImportsWeak
            let mutable getReferencedAssemblies =
                (fun () -> [| for r in tcImportsWeak.AllAssemblyResolutions() -> r.resolvedPath |])

            // When the tcImports is disposed the getReferencedAssemblies thunk is replaced
            // with one raising an exception.
            tcImportsStrong.AttachDisposeTypeProviderAction(fun () ->
                getReferencedAssemblies <- fun _ -> raise (ObjectDisposedException("The type provider has been disposed")))

            let typeProviderEnvironment =
                {
                    ResolutionFolder = tcConfig.implicitIncludeDir
                    OutputFile = tcConfig.outputFile
                    ShowResolutionMessages = tcConfig.showExtensionTypeMessages
                    GetReferencedAssemblies = (fun () -> [| for r in tcImportsStrong.AllAssemblyResolutions() -> r.resolvedPath |])
                    TemporaryFolder = FileSystem.GetTempPathShim()
                }

            let providers =
                [
                    for designTimeAssemblyName in designTimeAssemblyNames do
                        yield!
                            GetTypeProvidersOfAssembly(
                                fileNameOfRuntimeAssembly,
                                ilScopeRefOfRuntimeAssembly,
                                designTimeAssemblyName,
                                typeProviderEnvironment,
                                tcConfig.isInvalidationSupported,
                                tcConfig.isInteractive,
                                systemRuntimeContainsType,
                                primaryAssemblyVersion,
                                tcConfig.compilerToolPaths,
                                m
                            )
                ]
            // Note, type providers are disposable objects. The TcImports owns the provider objects - when/if it is disposed, the providers are disposed.
            // We ignore all exceptions from provider disposal.
            for provider in providers do
                tcImportsStrong.AttachDisposeTypeProviderAction(fun () ->
                    try
                        provider.PUntaintNoFailure(fun x -> x.Dispose())
                    with e ->
                        ())

            // Add the invalidation signal handlers to each provider
            for provider in providers do
                provider.PUntaint(
                    (fun tp ->

                        // Register the type provider invalidation handler.
                        //
                        // We are explicit about what the handler closure captures to help reason about the
                        // lifetime of captured objects, especially in case the type provider instance gets leaked
                        // or keeps itself alive mistakenly, e.g. via some global state in the type provider instance.
                        //
                        // The closure captures
                        //   1. an Event value, ultimately this is made available in all CCus as ccu.InvalidateEvent
                        //   2. any handlers registered to ccu.InvalidateEvent
                        //   3. a message string
                        //
                        // Note that the invalidation handler does not explicitly capture the TcImports.
                        // The only place where handlers are registered is to ccu.InvalidateEvent is in IncrementalBuilder.fs.

                        let capturedInvalidateCcu = invalidateCcu

                        let capturedMessage =
                            "The provider '" + fileNameOfRuntimeAssembly + "' reported a change"

                        let handler =
                            tp.Invalidate.Subscribe(fun _ -> capturedInvalidateCcu.Trigger capturedMessage)

                        // When the TcImports is disposed we detach the invalidation callback
                        tcImportsStrong.AttachDisposeTypeProviderAction(fun () ->
                            try
                                handler.Dispose()
                            with _ ->
                                ())),
                    m
                )

            match providers with
            | [] ->
                let typeName = typeof<TypeProviderAssemblyAttribute>.FullName
                warning (Error(FSComp.SR.etHostingAssemblyFoundWithoutHosts (fileNameOfRuntimeAssembly, typeName), m))
            | _ ->

#if DEBUG
                if typeProviderEnvironment.ShowResolutionMessages then
                    dprintfn "Found extension type hosting hosting assembly '%s' with the following extensions:" fileNameOfRuntimeAssembly

                    providers
                    |> List.iter (fun provider -> dprintfn " %s" (DisplayNameOfTypeProvider(provider.TypeProvider, m)))
#endif

                for provider in providers do
                    try
                        // Inject an entity for the namespace, or if one already exists, then record this as a provider
                        // for that namespace.
                        let rec loop (providedNamespace: Tainted<IProvidedNamespace>) =
                            let path =
                                GetProvidedNamespaceAsPath(m, provider, providedNamespace.PUntaint((fun r -> r.NamespaceName), m))

                            tcImportsStrong.InjectProvidedNamespaceOrTypeIntoEntity(
                                typeProviderEnvironment,
                                tcConfig,
                                m,
                                entityToInjectInto,
                                [],
                                path,
                                provider,
                                None
                            )

                            // Inject entities for the types returned by provider.GetTypes().
                            //
                            // NOTE: The types provided by GetTypes() are available for name resolution
                            // when the namespace is "opened". This is part of the specification of the language
                            // feature.
                            let tys =
                                providedNamespace.PApplyArray((fun provider -> provider.GetTypes()), "GetTypes", m)

                            let ptys =
                                [|
                                    for ty in tys -> ty.PApply((fun ty -> ty |> ProvidedType.CreateNoContext), m)
                                |]

                            for st in ptys do
                                tcImportsStrong.InjectProvidedNamespaceOrTypeIntoEntity(
                                    typeProviderEnvironment,
                                    tcConfig,
                                    m,
                                    entityToInjectInto,
                                    [],
                                    path,
                                    provider,
                                    Some st
                                )

                            for providedNestedNamespace in
                                providedNamespace.PApplyArray((fun provider -> provider.GetNestedNamespaces()), "GetNestedNamespaces", m) do
                                loop providedNestedNamespace

                        RequireCompilationThread ctok // IProvidedType.GetNamespaces is an example of a type provider call

                        let providedNamespaces =
                            provider.PApplyArray((fun r -> r.GetNamespaces()), "GetNamespaces", m)

                        for providedNamespace in providedNamespaces do
                            loop providedNamespace
                    with e ->
                        errorRecovery e m

                if startingErrorCount < DiagnosticsThreadStatics.DiagnosticsLogger.ErrorCount then
                    error (Error(FSComp.SR.etOneOrMoreErrorsSeenDuringExtensionTypeSetting (), m))

            providers
        else
            []
#endif

    /// Query information about types available in target system runtime library
    member tcImports.SystemRuntimeContainsType(typeName: string) : bool =
        let ns, typeName = splitILTypeName typeName
        let tcGlobals = tcImports.GetTcGlobals()
        tcGlobals.TryFindSysTyconRef ns typeName |> Option.isSome

    // Add a referenced assembly
    //
    // Retargetable assembly refs are required for binaries that must run
    // against DLLs supported by multiple publishers. For example
    // Compact Framework binaries must use this. However it is not
    // clear when else it is required, e.g. for Mono.

    member tcImports.PrepareToImportReferencedILAssembly(ctok, m, fileName, dllinfo: ImportedBinary) =
        CheckDisposed()
        let tcConfig = tcConfigP.Get ctok
        assert dllinfo.RawMetadata.TryGetILModuleDef().IsSome
        let ilModule = dllinfo.RawMetadata.TryGetILModuleDef().Value
        let ilScopeRef = dllinfo.ILScopeRef
        let auxModuleLoader = tcImports.MkLoaderForMultiModuleILAssemblies ctok m
        let invalidateCcu = Event<_>()

        let ccu =
            ImportILAssembly(
                tcImports.GetImportMap,
                m,
                auxModuleLoader,
                tcConfig.xmlDocInfoLoader,
                ilScopeRef,
                tcConfig.implicitIncludeDir,
                Some fileName,
                ilModule,
                invalidateCcu.Publish
            )

        let ccuinfo =
            {
                FSharpViewOfMetadata = ccu
                ILScopeRef = ilScopeRef
                AssemblyAutoOpenAttributes = GetAutoOpenAttributes ilModule
                AssemblyInternalsVisibleToAttributes = GetInternalsVisibleToAttributes ilModule
#if !NO_TYPEPROVIDERS
                IsProviderGenerated = false
                TypeProviders = []
#endif
                FSharpOptimizationData = notlazy None
            }

        tcImports.RegisterCcu ccuinfo

        let phase2 () =
#if !NO_TYPEPROVIDERS
            let attrs = ilModule.ManifestOfAssembly.CustomAttrs.AsList()

            ccuinfo.TypeProviders <-
                tcImports.ImportTypeProviderExtensions(ctok, tcConfig, fileName, ilScopeRef, attrs, ccu.Contents, invalidateCcu, m)
#endif
            [ ResolvedImportedAssembly(ccuinfo, m) ]

        phase2

    member tcImports.PrepareToImportReferencedFSharpAssembly(ctok, m, fileName, dllinfo: ImportedBinary) =
        CheckDisposed()
#if !NO_TYPEPROVIDERS
        let tcConfig = tcConfigP.Get ctok
#endif
        let ilModule = dllinfo.RawMetadata
        let ilScopeRef = dllinfo.ILScopeRef
        let ilShortAssemName = getNameOfScopeRef ilScopeRef

        let optDataReaders =
            ilModule.GetRawFSharpOptimizationData(m, ilShortAssemName, fileName)

        let ccuRawDataAndInfos =
            ilModule.GetRawFSharpSignatureData(m, ilShortAssemName, fileName)
            |> List.map (fun (ccuName, sigDataReader) ->
                let data =
                    GetSignatureData(fileName, ilScopeRef, ilModule.TryGetILModuleDef(), sigDataReader)

                let optDatas = Map.ofList optDataReaders

                let minfo: PickledCcuInfo = data.RawData
                let mspec = minfo.mspec

#if !NO_TYPEPROVIDERS
                let invalidateCcu = Event<_>()
#endif

                let codeDir = minfo.compileTimeWorkingDir

                // note: for some fields we fix up this information later
                let ccuData: CcuData =
                    {
                        ILScopeRef = ilScopeRef
                        Stamp = newStamp ()
                        FileName = Some fileName
                        QualifiedName = Some(ilScopeRef.QualifiedName)
                        SourceCodeDirectory = codeDir
                        IsFSharp = true
                        Contents = mspec
#if !NO_TYPEPROVIDERS
                        InvalidateEvent = invalidateCcu.Publish
                        IsProviderGenerated = false
                        ImportProvidedType = (fun ty -> ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
                        TryGetILModuleDef = ilModule.TryGetILModuleDef
                        UsesFSharp20PlusQuotations = minfo.usesQuotations
                        MemberSignatureEquality = (fun ty1 ty2 -> typeEquivAux EraseAll (tcImports.GetTcGlobals()) ty1 ty2)
                        TypeForwarders = ImportILAssemblyTypeForwarders(tcImports.GetImportMap, m, ilModule.GetRawTypeForwarders())
                        XmlDocumentationInfo =
                            match tcConfig.xmlDocInfoLoader with
                            | Some xmlDocInfoLoader -> xmlDocInfoLoader.TryLoad(fileName)
                            | _ -> None
                    }

                let ccu = CcuThunk.Create(ccuName, ccuData)

                let optdata =
                    lazy
                        (match Map.tryFind ccuName optDatas with
                         | None -> None
                         | Some info ->
                             let data =
                                 GetOptimizationData(fileName, ilScopeRef, ilModule.TryGetILModuleDef(), info)

                             let fixupThunk () =
                                 data.OptionalFixup(fun nm -> availableToOptionalCcu (tcImports.FindCcu(ctok, m, nm, lookupOnly = false)))

                             // Make a note of all ccuThunks that may still need to be fixed up when other dlls are loaded
                             tciLock.AcquireLock(fun tcitok ->
                                 RequireTcImportsLock(tcitok, ccuThunks)

                                 for ccuThunk in data.FixupThunks do
                                     if ccuThunk.IsUnresolvedReference then
                                         ccuThunks.Add(ccuThunk, (fun () -> fixupThunk () |> ignore)))

                             Some(fixupThunk ()))

                let ccuinfo =
                    {
                        FSharpViewOfMetadata = ccu
                        AssemblyAutoOpenAttributes = ilModule.GetAutoOpenAttributes()
                        AssemblyInternalsVisibleToAttributes = ilModule.GetInternalsVisibleToAttributes()
                        FSharpOptimizationData = optdata
#if !NO_TYPEPROVIDERS
                        IsProviderGenerated = false
                        TypeProviders = []
#endif
                        ILScopeRef = ilScopeRef
                    }

                let phase2 () =
#if !NO_TYPEPROVIDERS
                    match ilModule.TryGetILModuleDef() with
                    | None -> () // no type providers can be used without a real IL Module present
                    | Some ilModule ->
                        let attrs = ilModule.ManifestOfAssembly.CustomAttrs.AsList()

                        let tps =
                            tcImports.ImportTypeProviderExtensions(
                                ctok,
                                tcConfig,
                                fileName,
                                ilScopeRef,
                                attrs,
                                ccu.Contents,
                                invalidateCcu,
                                m
                            )

                        ccuinfo.TypeProviders <- tps
#else
                    ()
#endif
                data, ccuinfo, phase2)

        // Register all before relinking to cope with mutually-referential ccus
        ccuRawDataAndInfos |> List.iter (p23 >> tcImports.RegisterCcu)

        let phase2 () =
            // Relink
            ccuRawDataAndInfos
            |> List.iter (fun (data, _, _) ->
                let fixupThunk () =
                    data.OptionalFixup(fun nm -> availableToOptionalCcu (tcImports.FindCcu(ctok, m, nm, lookupOnly = false)))
                    |> ignore

                fixupThunk ()

                for ccuThunk in data.FixupThunks do
                    if ccuThunk.IsUnresolvedReference then
                        tciLock.AcquireLock(fun tcitok ->
                            RequireTcImportsLock(tcitok, ccuThunks)
                            ccuThunks.Add(ccuThunk, fixupThunk)))
#if !NO_TYPEPROVIDERS
            ccuRawDataAndInfos |> List.iter (fun (_, _, phase2) -> phase2 ())
#endif
            ccuRawDataAndInfos
            |> List.map p23
            |> List.map (fun asm -> ResolvedImportedAssembly(asm, m))

        phase2

    // NOTE: When used in the Language Service this can cause the transitive checking of projects. Hence it must be cancellable.
    member tcImports.TryRegisterAndPrepareToImportReferencedDll
        (
            ctok,
            r: AssemblyResolution
        ) : NodeCode<(_ * (unit -> AvailableImportedAssembly list)) option> =
        node {
            CheckDisposed()
            let m = r.originalReference.Range
            let fileName = r.resolvedPath

            let! contentsOpt =
                node {
                    match r.ProjectReference with
                    | Some ilb -> return! ilb.EvaluateRawContents()
                    | None -> return ProjectAssemblyDataResult.Unavailable true
                }

            // If we have a project reference but did not get any valid contents,
            //     just return None and do not attempt to read elsewhere.
            match contentsOpt with
            | ProjectAssemblyDataResult.Unavailable false -> return None
            | _ ->

                let assemblyData =
                    match contentsOpt with
                    | ProjectAssemblyDataResult.Available ilb -> ilb
                    | ProjectAssemblyDataResult.Unavailable _ ->
                        let ilModule, ilAssemblyRefs = tcImports.OpenILBinaryModule(ctok, fileName, m)
                        RawFSharpAssemblyDataBackedByFileOnDisk(ilModule, ilAssemblyRefs) :> IRawFSharpAssemblyData

                let ilShortAssemName = assemblyData.ShortAssemblyName
                let ilScopeRef = assemblyData.ILScopeRef

                if tcImports.IsAlreadyRegistered ilShortAssemName then
                    let dllinfo = tcImports.FindDllInfo(ctok, m, ilShortAssemName)

                    let phase2 () =
                        [ tcImports.FindCcuInfo(ctok, m, ilShortAssemName, lookupOnly = true) ]

                    return Some(dllinfo, phase2)
                else
                    let dllinfo =
                        {
                            RawMetadata = assemblyData
                            FileName = fileName
#if !NO_TYPEPROVIDERS
                            ProviderGeneratedAssembly = None
                            IsProviderGenerated = false
                            ProviderGeneratedStaticLinkMap = None
#endif
                            ILScopeRef = ilScopeRef
                            ILAssemblyRefs = assemblyData.ILAssemblyRefs
                        }

                    tcImports.RegisterDll dllinfo

                    let phase2 =
                        if assemblyData.HasAnyFSharpSignatureDataAttribute then
                            if not assemblyData.HasMatchingFSharpSignatureDataAttribute then
                                errorR (Error(FSComp.SR.buildDifferentVersionMustRecompile fileName, m))
                                tcImports.PrepareToImportReferencedILAssembly(ctok, m, fileName, dllinfo)
                            else
                                try
                                    tcImports.PrepareToImportReferencedFSharpAssembly(ctok, m, fileName, dllinfo)
                                with e ->
                                    error (Error(FSComp.SR.buildErrorOpeningBinaryFile (fileName, e.Message), m))
                        else
                            tcImports.PrepareToImportReferencedILAssembly(ctok, m, fileName, dllinfo)

                    return Some(dllinfo, phase2)
        }

    // NOTE: When used in the Language Service this can cause the transitive checking of projects. Hence it must be cancellable.
    member tcImports.RegisterAndImportReferencedAssemblies(ctok, nms: AssemblyResolution list) =
        node {
            CheckDisposed()

            let tcConfig = tcConfigP.Get ctok

            let runMethod =
                match tcConfig.parallelReferenceResolution with
                | ParallelReferenceResolution.On -> NodeCode.Parallel
                | ParallelReferenceResolution.Off -> NodeCode.Sequential

            let! results =
                nms
                |> List.map (fun nm ->
                    node {
                        try
                            return! tcImports.TryRegisterAndPrepareToImportReferencedDll(ctok, nm)
                        with e ->
                            errorR (Error(FSComp.SR.buildProblemReadingAssembly (nm.resolvedPath, e.Message), nm.originalReference.Range))
                            return None
                    })
                |> runMethod

            let _dllinfos, phase2s = results |> Array.choose id |> List.ofArray |> List.unzip
            fixupOrphanCcus ()
            let ccuinfos = List.collect (fun phase2 -> phase2 ()) phase2s
            return ccuinfos
        }

    /// Note that implicit loading is not used for compilations from MSBuild, which passes ``--noframework``
    /// Implicit loading is done in non-cancellation mode. Implicit loading is never used in the language service, so
    /// no cancellation is needed.
    member tcImports.ImplicitLoadIfAllowed(ctok, m, assemblyName, lookupOnly) =
        CheckDisposed()
        // If the user is asking for the default framework then also try to resolve other implicit assemblies as they are discovered.
        // Using this flag to mean 'allow implicit discover of assemblies'.
        let tcConfig = tcConfigP.Get ctok

        if not lookupOnly && tcConfig.implicitlyResolveAssemblies then
            let tryFile speculativeFileName =
                let foundFile =
                    tcImports.TryResolveAssemblyReference(
                        ctok,
                        AssemblyReference(m, speculativeFileName, None),
                        ResolveAssemblyReferenceMode.Speculative
                    )

                match foundFile with
                | OkResult (warns, res) ->
                    ReportWarnings warns

                    tcImports.RegisterAndImportReferencedAssemblies(ctok, res)
                    |> NodeCode.RunImmediateWithoutCancellation
                    |> ignore

                    true
                | ErrorResult (_warns, _err) ->
                    // Throw away warnings and errors - this is speculative loading
                    false

            if tryFile (assemblyName + ".dll") then
                ()
            else
                tryFile (assemblyName + ".exe") |> ignore

#if !NO_TYPEPROVIDERS
    member tcImports.TryFindProviderGeneratedAssemblyByName(ctok, assemblyName: string) : Assembly option =
        // The assembly may not be in the resolutions, but may be in the load set including EST injected assemblies
        match tcImports.TryFindDllInfo(ctok, range0, assemblyName, lookupOnly = true) with
        | Some res ->
            // Provider-generated assemblies don't necessarily have an on-disk representation we can load.
            res.ProviderGeneratedAssembly
        | _ -> None
#endif

    /// Only used by F# Interactive
    member _.TryFindExistingFullyQualifiedPathBySimpleAssemblyName simpleAssemName : string option =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, resolutions)

            resolutions.TryFindBySimpleAssemblyName simpleAssemName
            |> Option.map (fun r -> r.resolvedPath))

    /// Only used by F# Interactive
    member _.TryFindExistingFullyQualifiedPathByExactAssemblyRef(assemblyRef: ILAssemblyRef) : string option =
        tciLock.AcquireLock(fun tcitok ->
            RequireTcImportsLock(tcitok, resolutions)

            resolutions.TryFindByExactILAssemblyRef assemblyRef
            |> Option.map (fun r -> r.resolvedPath))

    member _.TryResolveAssemblyReference
        (
            ctok,
            assemblyReference: AssemblyReference,
            mode: ResolveAssemblyReferenceMode
        ) : OperationResult<AssemblyResolution list> =
        tciLock.AcquireLock(fun tcitok ->
            let tcConfig = tcConfigP.Get ctok

            RequireTcImportsLock(tcitok, resolutions)
            // First try to lookup via the original reference text.
            match resolutions.TryFindByOriginalReference assemblyReference with
            | Some assemblyResolution -> ResultD [ assemblyResolution ]
            | None ->
#if NO_MSBUILD_REFERENCE_RESOLUTION
                try
                    ResultD [ tcConfig.ResolveLibWithDirectories assemblyReference ]
                with e ->
                    ErrorD e
#else
                // Next try to lookup up by the exact full resolved path.
                match resolutions.TryFindByResolvedPath assemblyReference.Text with
                | Some assemblyResolution -> ResultD [ assemblyResolution ]
                | None ->
                    if tcConfigP.Get(ctok).useSimpleResolution then
                        let action =
                            match mode with
                            | ResolveAssemblyReferenceMode.ReportErrors -> CcuLoadFailureAction.RaiseError
                            | ResolveAssemblyReferenceMode.Speculative -> CcuLoadFailureAction.ReturnNone

                        match tcConfig.ResolveLibWithDirectories(action, assemblyReference) with
                        | Some resolved ->
                            resolutions <- resolutions.AddResolutionResults [ resolved ]
                            ResultD [ resolved ]
                        | None -> ErrorD(AssemblyNotResolved(assemblyReference.Text, assemblyReference.Range))
                    else
                        // This is a previously unencountered assembly. Resolve it and add it to the list.
                        // But don't cache resolution failures because the assembly may appear on the disk later.
                        let resolved, unresolved =
                            TcConfig.TryResolveLibsUsingMSBuildRules(tcConfig, [ assemblyReference ], assemblyReference.Range, mode)

                        match resolved, unresolved with
                        | assemblyResolution :: _, _ ->
                            resolutions <- resolutions.AddResolutionResults resolved
                            ResultD [ assemblyResolution ]
                        | _, _ :: _ ->
                            resolutions <- resolutions.AddUnresolvedReferences unresolved
                            ErrorD(AssemblyNotResolved(assemblyReference.Text, assemblyReference.Range))
                        | [], [] ->
                            // Note, if mode=ResolveAssemblyReferenceMode.Speculative and the resolution failed then TryResolveLibsUsingMSBuildRules returns
                            // the empty list and we convert the failure into an AssemblyNotResolved here.
                            ErrorD(AssemblyNotResolved(assemblyReference.Text, assemblyReference.Range))
#endif
        )

    member tcImports.ResolveAssemblyReference(ctok, assemblyReference, mode) : AssemblyResolution list =
        CommitOperationResult(tcImports.TryResolveAssemblyReference(ctok, assemblyReference, mode))

    // Note: This returns a TcImports object. However, framework TcImports are not currently disposed. The only reason
    // we dispose TcImports is because we need to dispose type providers, and type providers are never included in the framework DLL set.
    // If a framework set ever includes type providers, you will not have to worry about explicitly calling Dispose as the Finalizer will handle it.
    static member BuildFrameworkTcImports(tcConfigP: TcConfigProvider, frameworkDLLs, nonFrameworkDLLs) =
        node {
            let ctok = CompilationThreadToken()
            let tcConfig = tcConfigP.Get ctok

            let tcResolutions =
                TcAssemblyResolutions.BuildFromPriorResolutions(tcConfig, frameworkDLLs, [])

            let tcAltResolutions =
                TcAssemblyResolutions.BuildFromPriorResolutions(tcConfig, nonFrameworkDLLs, [])

            let frameworkTcImports = new TcImports(tcConfigP, tcResolutions, None, None)

            // Fetch the primaryAssembly from the referenced assemblies otherwise
            let primaryAssemblyReference =
                let path =
                    frameworkDLLs
                    |> List.tryFind (fun dll ->
                        let baseName = Path.GetFileNameWithoutExtension(dll.resolvedPath)

                        let res =
                            String.Compare(baseName, tcConfig.primaryAssembly.Name, StringComparison.OrdinalIgnoreCase)

                        res = 0)

                match path with
                | Some p -> AssemblyReference(range0, p.resolvedPath, None)
                | None -> tcConfig.PrimaryAssemblyDllReference()

            let primaryAssemblyResolution =
                frameworkTcImports.ResolveAssemblyReference(ctok, primaryAssemblyReference, ResolveAssemblyReferenceMode.ReportErrors)

            let! primaryAssem = frameworkTcImports.RegisterAndImportReferencedAssemblies(ctok, primaryAssemblyResolution)

            let primaryScopeRef =
                match primaryAssem with
                | [ ResolvedImportedAssembly (ccu, _) ] -> ccu.FSharpViewOfMetadata.ILScopeRef
                | _ -> failwith "primaryScopeRef - unexpected"

            let resolvedAssemblies = tcResolutions.GetAssemblyResolutions()

            let primaryAssemblyResolvedPath =
                match primaryAssemblyResolution with
                | [ primaryAssemblyResolution ] -> primaryAssemblyResolution.resolvedPath
                | _ -> failwith "primaryAssemblyResolvedPath - unexpected"

            let readerSettings: ILReaderOptions =
                {
                    pdbDirPath = None
                    reduceMemoryUsage = tcConfig.reduceMemoryUsage
                    metadataOnly = MetadataOnlyFlag.Yes
                    tryGetMetadataSnapshot = tcConfig.tryGetMetadataSnapshot
                }

            let tryFindEquivPrimaryAssembly (resolvedAssembly: AssemblyResolution) =
                if primaryAssemblyResolvedPath = resolvedAssembly.resolvedPath then
                    None
                else
                    let reader = OpenILModuleReader resolvedAssembly.resolvedPath readerSettings
                    let mdef = reader.ILModuleDef

                    // We check the exported types of all assemblies, since many may forward System.Object,
                    // but only check the actual type definitions for specific assemblies that we know
                    // might actually declare System.Object.
                    match mdef.Manifest with
                    | Some manifest when
                        manifest.ExportedTypes.TryFindByName "System.Object" |> Option.isSome
                        || PrimaryAssembly.IsPossiblePrimaryAssembly resolvedAssembly.resolvedPath
                           && mdef.TypeDefs.ExistsByName "System.Object"
                        ->
                        mkRefToILAssembly manifest |> Some
                    | _ -> None

            // Find assemblies which also declare System.Object
            let equivPrimaryAssemblyRefs =
                resolvedAssemblies |> List.choose tryFindEquivPrimaryAssembly

            let! fslibCcu, fsharpCoreAssemblyScopeRef =
                node {
                    if tcConfig.compilingFSharpCore then
                        // When compiling FSharp.Core.dll, the fslibCcu reference to FSharp.Core.dll is a delayed ccu thunk fixed up during type checking
                        return CcuThunk.CreateDelayed getFSharpCoreLibraryName, ILScopeRef.Local
                    else
                        let coreLibraryReference = tcConfig.CoreLibraryDllReference()

                        let resolvedAssemblyRef =
                            match tcResolutions.TryFindByOriginalReference coreLibraryReference with
                            | Some resolution -> Some resolution
                            | _ ->
                                // Are we using a "non-canonical" FSharp.Core?
                                match tcAltResolutions.TryFindByOriginalReference coreLibraryReference with
                                | Some resolution -> Some resolution
                                | _ -> tcResolutions.TryFindByOriginalReferenceText getFSharpCoreLibraryName // was the ".dll" elided?

                        match resolvedAssemblyRef with
                        | Some coreLibraryResolution ->
                            match! frameworkTcImports.RegisterAndImportReferencedAssemblies(ctok, [ coreLibraryResolution ]) with
                            | [ ResolvedImportedAssembly (fslibCcuInfo, _) ] ->
                                return fslibCcuInfo.FSharpViewOfMetadata, fslibCcuInfo.ILScopeRef
                            | _ ->
                                return
                                    error (
                                        InternalError(
                                            $"no import of {coreLibraryResolution.resolvedPath}",
                                            coreLibraryResolution.originalReference.Range
                                        )
                                    )
                        | None -> return error (InternalError($"no resolution of '{coreLibraryReference.Text}'", rangeStartup))
                }

            // Load the rest of the framework DLLs all at once (they may be mutually recursive)
            let! _assemblies = frameworkTcImports.RegisterAndImportReferencedAssemblies(ctok, resolvedAssemblies)

            // These are the DLLs we can search for well-known types
            let sysCcus =
                [|
                    for ccu in frameworkTcImports.GetCcusInDeclOrder() do
                        ccu
                |]

            let tryFindSysTypeCcu path typeName =
                sysCcus |> Array.tryFind (fun ccu -> ccuHasType ccu path typeName)

            let ilGlobals =
                mkILGlobals (primaryScopeRef, equivPrimaryAssemblyRefs, fsharpCoreAssemblyScopeRef)

            // OK, now we have both mscorlib.dll and FSharp.Core.dll we can create TcGlobals
            let tcGlobals =
                TcGlobals(
                    tcConfig.compilingFSharpCore,
                    ilGlobals,
                    fslibCcu,
                    tcConfig.implicitIncludeDir,
                    tcConfig.mlCompatibility,
                    tcConfig.isInteractive,
                    tcConfig.useReflectionFreeCodeGen,
                    tryFindSysTypeCcu,
                    tcConfig.emitDebugInfoInQuotations,
                    tcConfig.noDebugAttributes,
                    tcConfig.pathMap,
                    tcConfig.langVersion
                )

#if DEBUG
            // the global_g reference cell is used only for debug printing
            global_g <- Some tcGlobals
#endif
            frameworkTcImports.SetTcGlobals tcGlobals
            return tcGlobals, frameworkTcImports
        }

    member _.ReportUnresolvedAssemblyReferences knownUnresolved =
        // Report that an assembly was not resolved.
        let reportAssemblyNotResolved (file, originalReferences: AssemblyReference list) =
            originalReferences
            |> List.iter (fun originalReference -> errorR (AssemblyNotResolved(file, originalReference.Range)))

        knownUnresolved
        |> List.map (function
            | UnresolvedAssemblyReference (file, originalReferences) -> file, originalReferences)
        |> List.iter reportAssemblyNotResolved

    static member BuildNonFrameworkTcImports
        (
            tcConfigP: TcConfigProvider,
            baseTcImports,
            nonFrameworkReferences,
            knownUnresolved,
            dependencyProvider
        ) =

        node {
            let ctok = CompilationThreadToken()
            let tcConfig = tcConfigP.Get ctok

            let tcResolutions =
                TcAssemblyResolutions.BuildFromPriorResolutions(tcConfig, nonFrameworkReferences, knownUnresolved)

            let references = tcResolutions.GetAssemblyResolutions()

            let tcImports =
                new TcImports(tcConfigP, tcResolutions, Some baseTcImports, Some dependencyProvider)

            let! _assemblies = tcImports.RegisterAndImportReferencedAssemblies(ctok, references)
            tcImports.ReportUnresolvedAssemblyReferences knownUnresolved
            return tcImports
        }

    static member BuildTcImports(tcConfigP: TcConfigProvider, dependencyProvider) =
        node {
            let ctok = CompilationThreadToken()
            let tcConfig = tcConfigP.Get ctok

            let frameworkDLLs, nonFrameworkReferences, knownUnresolved =
                TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)

            let! tcGlobals, frameworkTcImports = TcImports.BuildFrameworkTcImports(tcConfigP, frameworkDLLs, nonFrameworkReferences)

            let! tcImports =
                TcImports.BuildNonFrameworkTcImports(
                    tcConfigP,
                    frameworkTcImports,
                    nonFrameworkReferences,
                    knownUnresolved,
                    dependencyProvider
                )

            return tcGlobals, tcImports
        }

    interface IDisposable with
        member _.Dispose() = dispose ()

    override tcImports.ToString() = "TcImports(...)"

/// Process #r in F# Interactive.
/// Adds the reference to the tcImports and add the ccu to the type checking environment.
let RequireReferences (ctok, tcImports: TcImports, tcEnv, thisAssemblyName, resolutions) =

    let ccuinfos =
        tcImports.RegisterAndImportReferencedAssemblies(ctok, resolutions)
        |> NodeCode.RunImmediateWithoutCancellation

    let asms =
        ccuinfos
        |> List.map (function
            | ResolvedImportedAssembly (asm, m) -> asm, m
            | UnresolvedImportedAssembly (assemblyName, m) -> error (Error(FSComp.SR.buildCouldNotResolveAssembly (assemblyName), m)))

    let g = tcImports.GetTcGlobals()
    let amap = tcImports.GetImportMap()

    let _openDecls, tcEnv =
        (tcEnv, asms)
        ||> List.collectFold (fun tcEnv (asm, m) ->
            AddCcuToTcEnv(
                g,
                amap,
                m,
                tcEnv,
                thisAssemblyName,
                asm.FSharpViewOfMetadata,
                asm.AssemblyAutoOpenAttributes,
                asm.AssemblyInternalsVisibleToAttributes
            ))

    let asms = asms |> List.map fst

    tcEnv, asms
