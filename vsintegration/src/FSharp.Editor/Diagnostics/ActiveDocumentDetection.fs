// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

/// Helpers for determining whether a Roslyn Document corresponds to the document
/// currently active (focused) in the Visual Studio shell.
///
/// Background expensive analyzers (UnusedOpens, UnusedDeclarations, SimplifyName,
/// InlayHints) should run only for the active document, mirroring how C# restricts
/// "remove unnecessary usings" and similar live analyzers.
///
/// Roslyn's BackgroundAnalysisScope lets a host choose "open documents" or
/// "entire solution" but has no built-in "active document only" tier, so we
/// determine the truly active document ourselves via the VS shell.
[<RequireQualifiedAccess>]
module internal ActiveDocumentDetection =

    /// Returns the document moniker (full file path) of the currently focused
    /// editor window, or ValueNone if it cannot be determined.
    let tryGetActiveDocumentMoniker (serviceProvider: IServiceProvider) : string voption =
        match serviceProvider.GetService(typeof<SVsShellMonitorSelection>) with
        | :? IVsMonitorSelection as monitorSelection ->
            let mutable frameObj = null

            if
                ErrorHandler.Succeeded(
                    monitorSelection.GetCurrentElementValue(uint32 VSConstants.VSSELELEMID.SEID_DocumentFrame, &frameObj)
                )
            then
                match frameObj with
                | :? IVsWindowFrame as frame ->
                    let mutable monikerObj = null

                    if
                        ErrorHandler.Succeeded(frame.GetProperty(int32 __VSFPROPID.VSFPROPID_pszMkDocument, &monikerObj))
                    then
                        match monikerObj with
                        | :? string as moniker -> ValueSome moniker
                        | _ -> ValueNone
                    else
                        ValueNone
                | _ -> ValueNone
            else
                ValueNone
        | _ -> ValueNone

    /// Returns true when the given document is the currently active editor document.
    ///
    /// Falls back to true (= do not suppress analysis) when the active document
    /// cannot be determined, so analysis is never silently lost.
    let isActiveDocument (serviceProvider: IServiceProvider) (document: Document) : bool =
        match document.FilePath with
        | null -> true
        | filePath ->
            match tryGetActiveDocumentMoniker serviceProvider with
            | ValueNone -> true // couldn't determine the active document, don't suppress analysis
            | ValueSome activeMoniker -> String.Equals(activeMoniker, filePath, StringComparison.OrdinalIgnoreCase)
