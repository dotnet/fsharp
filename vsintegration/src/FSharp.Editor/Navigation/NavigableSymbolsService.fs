// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Linq
open System.Threading
open System.Threading.Tasks
open System.ComponentModel.Composition

open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Text.Shared.Extensions
open Microsoft.CodeAnalysis.Navigation

open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Shell

[<AllowNullLiteral>]
type internal FSharpNavigableSymbol(item: INavigableItem, statusBar: StatusBar, span: SnapshotSpan) =
    interface INavigableSymbol with
        member __.Navigate(_: INavigableRelationship) =
            GoToDefinitionHelpers.tryNavigateToItem statusBar (Some item) |> ignore

        member __.Relationships = seq { yield PredefinedNavigableRelationships.Definition }

        member __.SymbolSpan = span

type internal FSharpNavigableSymbolSource(checkerProvider: FSharpCheckerProvider, projectInfoManager: FSharpProjectOptionsManager, serviceProvider: IServiceProvider) =
    
    let mutable disposed = false
    let gtd = GoToDefinition(checkerProvider.Checker, projectInfoManager)
    let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar,IVsStatusbar>())

    interface INavigableSymbolSource with
        member __.GetNavigableSymbolAsync(triggerSpan: SnapshotSpan, cancellationToken: CancellationToken) =
            // Yes, this is a code smell. But this is how the editor API accepts what we would treat as None.
            if disposed then null
            else
                asyncMaybe {
                    let snapshot = triggerSpan.Snapshot
                    let position = triggerSpan.Start.Position
                    let document = snapshot.GetOpenDocumentInCurrentContextWithChanges()
                    let! sourceText = document.GetTextAsync () |> liftTaskAsync
                    
                    statusBar.Message (SR.LocatingSymbol())
                    use _ = statusBar.Animate()

                    let gtdTask = GoToDefinitionHelpers.findDefinitionsTask checkerProvider projectInfoManager document position gtd cancellationToken

                    // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled
                    // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
                    let gtdCompletedOrError =
                        try
                            // This call to Wait() is fine because we want to be able to provide the error message.
                            gtdTask.Wait()
                            Ok gtdTask
                        with exc -> 
                            Error(Exception.flattenMessage exc)

                    match gtdCompletedOrError with
                    | Ok task ->
                        if task.Status = TaskStatus.RanToCompletion && task.Result <> null && task.Result.Any() then
                            let (navigableItem, range) = task.Result.First() // F# API provides only one INavigableItem

                            let declarationTextSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)
                            let declarationSpan = Span(declarationTextSpan.Start, declarationTextSpan.Length)
                            let symbolSpan = SnapshotSpan(snapshot, declarationSpan)

                            return FSharpNavigableSymbol(navigableItem, statusBar, symbolSpan) :> INavigableSymbol
                        else 
                            statusBar.TempMessage (SR.CannotDetermineSymbol())

                            // The NavigableSymbols API accepts 'null' when there's nothing to navigate to.
                            return null
                    | Error message ->
                        statusBar.TempMessage (String.Format(SR.NavigateToFailed(), message))

                        // The NavigableSymbols API accepts 'null' when there's nothing to navigate to.
                        return null
                }
                |> Async.map Option.toObj
                |> RoslynHelpers.StartAsyncAsTask cancellationToken
        
        member __.Dispose() =
            disposed <- true

[<Export(typeof<INavigableSymbolSourceProvider>)>]
[<Name("F# Navigable Symbol Service")>]
[<ContentType(Constants.FSharpContentType)>]
[<Order>]
type internal FSharpNavigableSymbolService
    [<ImportingConstructor>]
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    interface INavigableSymbolSourceProvider with
        member __.TryCreateNavigableSymbolSource(_: ITextView, _: ITextBuffer) =
            new FSharpNavigableSymbolSource(checkerProvider, projectInfoManager, serviceProvider) :> INavigableSymbolSource