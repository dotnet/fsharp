// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Diagnostics
open Microsoft.CodeAnalysis
open FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices
open FSharp.NativeInterop
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

#nowarn "9" // NativePtr.toNativeInt

// Exposes FSharpChecker as MEF export
[<Export(typeof<FSharpCheckerProvider>); Composition.Shared>]
type internal FSharpCheckerProvider 
    [<ImportingConstructor>]
    (
        analyzerService: IFSharpDiagnosticAnalyzerService,
        [<Import(typeof<VisualStudioWorkspace>)>] workspace: VisualStudioWorkspace,
        settings: EditorOptions
    ) =

    let tryGetMetadataSnapshot (path, timeStamp) = 
        try
            let md = Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetMetadata(workspace, path, timeStamp)
            let amd = (md :?> AssemblyMetadata)
            let mmd = amd.GetModules().[0]
            let mmr = mmd.GetMetadataReader()

            // "lifetime is timed to Metadata you got from the GetMetadata(...). As long as you hold it strongly, raw 
            // memory we got from metadata reader will be alive. Once you are done, just let everything go and 
            // let finalizer handle resource rather than calling Dispose from Metadata directly. It is shared metadata. 
            // You shouldn't dispose it directly."

            let objToHold = box md

            // We don't expect any ilread WeakByteFile to be created when working in Visual Studio
            Debug.Assert((FSharp.Compiler.AbstractIL.ILBinaryReader.GetStatistics().weakByteFileCount = 0), "Expected weakByteFileCount to be zero when using F# in Visual Studio. Was there a problem reading a .NET binary?")

            Some (objToHold, NativePtr.toNativeInt mmr.MetadataPointer, mmr.MetadataLength)
        with ex -> 
            // We catch all and let the backup routines in the F# compiler find the error
            Assert.Exception(ex)
            None 


    let checker = 
        lazy
            let checker = 
                FSharpChecker.Create(
                    projectCacheSize = settings.LanguageServicePerformance.ProjectCheckCacheSize, 
                    keepAllBackgroundResolutions = false,
                    // Enabling this would mean that if devenv.exe goes above 2.3GB we do a one-off downsize of the F# Compiler Service caches
                    (* , MaxMemory = 2300 *)
                    legacyReferenceResolver=LegacyMSBuildReferenceResolver.getResolver(),
                    tryGetMetadataSnapshot = tryGetMetadataSnapshot)

            // This is one half of the bridge between the F# background builder and the Roslyn analysis engine.
            // When the F# background builder refreshes the background semantic build context for a file,
            // we request Roslyn to reanalyze that individual file.
            checker.BeforeBackgroundFileCheck.Add(fun (fileName, _extraProjectInfo) ->  
                async {
                    try 
                        let solution = workspace.CurrentSolution
                        let documentIds = solution.GetDocumentIdsWithFilePath(fileName)
                        if not documentIds.IsEmpty then 
                            let documentIdsFiltered = documentIds |> Seq.filter workspace.IsDocumentOpen |> Seq.toArray
                            for documentId in documentIdsFiltered do
                                Trace.TraceInformation("{0:n3} Requesting Roslyn reanalysis of {1}", DateTime.Now.TimeOfDay.TotalSeconds, documentId)
                            if documentIdsFiltered.Length > 0 then 
                                analyzerService.Reanalyze(workspace,documentIds=documentIdsFiltered)
                    with ex -> 
                        Assert.Exception(ex)
                } |> Async.StartImmediate
            )
            checker

    member this.Checker = checker.Value

