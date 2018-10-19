// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Collections.Immutable
open System.ComponentModel.Composition
open System.IO
open System.Linq
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.SiteProvider
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell
open System.Threading
open System.Collections

module private FSharpProjectOptions =

    /// Get the exact options for a single-file script
    let computeSingleFileOptions fileName loadTime fileContents (checkerProvider: FSharpCheckerProvider) (settings: EditorOptions) projectOptionsTable serviceProvider =
        async {
            // NOTE: we don't use a unique stamp for single files, instead comparing options structurally.
            // This is because we repeatedly recompute the options.
            let extraProjectInfo = None
            let optionsStamp = None 
            let! options, _diagnostics = checkerProvider.Checker.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo, ?optionsStamp=optionsStamp) 
            // NOTE: we don't use FCS cross-project references from scripts to projects.  THe projects must have been
            // compiled and #r will refer to files on disk
            let referencedProjectFileNames = [| |] 
            let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, referencedProjectFileNames, options)
            let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, site, serviceProvider, None, fileName, options.ExtraProjectInfo, Some projectOptionsTable)
            let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
            return (deps, parsingOptions, projectOptions)
        }

[<RequireQualifiedAccess>]
type private FSharpProjectOptionsMessage =
    | GetSingleFileOptions of Document * AsyncReplyChannel<FSharpParsingOptions * FSharpProjectOptions>

[<Sealed>]
type internal FSharpProjectOptionsCache (checkerProvider, settings, projectOptionsTable, serviceProvider) =

    let cancellationTokenSource = new CancellationTokenSource()

    let singleFileOptionsCache = ConcurrentDictionary<DocumentId, VersionStamp * FSharpParsingOptions * FSharpProjectOptions>()

    let cacheSingleFileOptions (document: Document) =
        async {
            let! textVersion = document.GetTextVersionAsync() |> Async.AwaitTask
            let! sourceText = document.GetTextAsync() |> Async.AwaitTask
            let timeStamp = DateTime.UtcNow
            let! _referencedProjectFileNames, parsingOptions, projectOptions = 
                FSharpProjectOptions.computeSingleFileOptions document.FilePath timeStamp (sourceText.ToString()) checkerProvider settings projectOptionsTable serviceProvider
            singleFileOptionsCache.[document.Id] <- (textVersion, parsingOptions, projectOptions)
            return (parsingOptions, projectOptions)
        }

    let tryGetSingleFileOptions (document: Document) f =
        async {
            let! textVersion = document.GetTextVersionAsync() |> Async.AwaitTask

            match singleFileOptionsCache.TryGetValue(document.Id) with
            | true, (lastTextVersion, _, _) when textVersion <> lastTextVersion ->
                return! f 
            | false, _ ->
                return! f
            | true, (_, parsingOptions, projectOptions) -> 
                return (parsingOptions, projectOptions)
        }

    let loop (agent: MailboxProcessor<FSharpProjectOptionsMessage>) =
        async {
            while true do
                try
                    match! agent.Receive() with
                    | FSharpProjectOptionsMessage.GetSingleFileOptions(document, reply) ->
                        let! (parsingOptions, projectOptions) = tryGetSingleFileOptions document (cacheSingleFileOptions document) 
                        reply.Reply(parsingOptions, projectOptions)
                with
                | _ -> ()
        }

    let agent = MailboxProcessor.Start((fun agent -> loop agent), cancellationToken = cancellationTokenSource.Token)

    member this.TryGetProjectOptionsAsync(document: Document) =
        match projectOptionsTable.TryGetOptionsForProject(document.Project.Id) with
        | Some(result) when not (isScriptFile document.FilePath) -> async { return Some(result) }
        | _ ->
            async {
                let! (parsingOptions, projectOptions) = 
                    tryGetSingleFileOptions document (agent.PostAndAsyncReply(fun reply -> FSharpProjectOptionsMessage.GetSingleFileOptions(document, reply)))
                return Some(parsingOptions, None, projectOptions)
            }

    member __.ClearSingleDocumentCacheById(documentId: DocumentId) =
        singleFileOptionsCache.TryRemove(documentId) |> ignore

    interface IDisposable with

        member __.Dispose() = 
            (agent :> IDisposable).Dispose()
            cancellationTokenSource.Cancel()
            cancellationTokenSource.Dispose()