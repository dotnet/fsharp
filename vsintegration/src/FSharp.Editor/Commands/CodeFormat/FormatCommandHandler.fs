// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.OLE.Interop
open System.ComponentModel.Composition
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.LanguageService
open System.Runtime.InteropServices
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Host
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.Text.Operations
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Commands
open Microsoft.CodeAnalysis.Editor.Implementation.Formatting
open System.Threading
open Microsoft.CodeAnalysis.Text
open System.Threading.Tasks
open System.Collections.Generic
open Microsoft.CodeAnalysis.Host.Mef
open FormatConfig

[<ExportCommandHandler(PredefinedCommandHandlerNames.FormatDocument, FSharpCommonConstants.FSharpLanguageName)>]
[<Order(After = PredefinedCommandHandlerNames.Rename, Before = PredefinedCommandHandlerNames.Completion)>]
type internal FSharpFormatCommandHandler
    [<ImportingConstructor>]
    (
        waitIndicator: IWaitIndicator,
        undoHistoryRegistry: ITextUndoHistoryRegistry,
        editorOperationsFactoryService: IEditorOperationsFactoryService
    ) =

    inherit FormatCommandHandler(waitIndicator, undoHistoryRegistry, editorOperationsFactoryService)

[<Shared>]
[<ExportLanguageService(typeof<IEditorFormattingService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpEditorFormattingService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    interface IEditorFormattingService with
        member __.SupportsFormatDocument = true
        member __.SupportsFormatSelection = false
        member __.SupportsFormatOnPaste = false
        member __.SupportsFormatOnReturn = false
        member __.SupportsFormattingOnTypedCharacter(_document, _ch) = false
        member __.GetFormattingChangesOnPasteAsync(_document, _textSpan, _cancellationToken) = Task.FromResult ([||] :> IList<_>)
        member __.GetFormattingChangesAsync(_document, _typedChar, _position, _cancellationToken) = Task.FromResult ([||] :> IList<_>)
        member __.GetFormattingChangesOnReturnAsync(_document, _position, _cancellationToken) = Task.FromResult ([||] :> IList<_>)
 
        member __.GetFormattingChangesAsync(document, textSpan, cancellationToken) =
            asyncMaybe {
                let! sourceText = document.GetTextAsync(cancellationToken)
                let span = if textSpan.HasValue then textSpan.Value else TextSpan(0, sourceText.Length)
                let textToFormat = sourceText.ToString(span)
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! newText = CodeFormatter.FormatDocumentAsync(document.FilePath, textToFormat, FormatConfig.Default, options, checkerProvider.Checker) |> liftAsync
                return TextChange(span, newText)
            } 
            |> Async.map (fun x -> (match x with Some x -> [|x|] | None -> [||]) :> IList<_>)
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)


//[<ExportCommandHandler(PredefinedCommandHandlerNames.FormatDocument, ContentTypeNames.RoslynContentType)>]
//[<Order(After = PredefinedCommandHandlerNames.Rename)>]
//[<Order(Before = PredefinedCommandHandlerNames.Completion)>]
//type internal FormatCommandHandler
//    [<ImportingConstructor>]
//    (
//        waitIndicator: IWaitIndicator,
//        undoHistoryRegistry: ITextUndoHistoryRegistry,
//        editorOperationsFactoryService: IEditorOperationsFactoryService
//    ) =
     
//    let format(textView: ITextView, document: Document, selectionOpt: TextSpan option, cancellationToken: CancellationToken) =
//        use transaction = new CaretPreservingEditTransaction(EditorFeaturesResources.Formatting, textView, undoHistoryRegistry, editorOperationsFactoryService)
 
//        match selectionOpt with
//        | Some selection -> ()
//        | None ->
//            let textChange = TextChange.NoChanges
//            IWorkspaceExtensions.ApplyTextChanges(document.Project.Solution.Workspace, document.Id, textChange, cancellationToken)
 
//        transaction.Complete()
        
//    //ICommandHandler<FormatSelectionCommandArgs>
//    //ICommandHandler<PasteCommandArgs>
    
//    let getCommandState(buffer: ITextBuffer, nextHandler: Func<CommandState>) =
//        if not (Extensions.CanApplyChangeDocumentToWorkspace(buffer)) then
//            nextHandler.Invoke()
//        else CommandState.Available

//    let tryExecuteCommand(args: FormatDocumentCommandArgs) =
//        if not (Extensions.CanApplyChangeDocumentToWorkspace(args.SubjectBuffer)) then false
//        else
//            let document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges()
//            if isNull document then false
//            else
//                let formattingService = document.GetLanguageService<IEditorFormattingService>();
//                if (formattingService == null || !formattingService.SupportsFormatDocument)
//                {
//                    return false;
//                }
                
//                var result = false;
//                _waitIndicator.Wait(
//                    title: EditorFeaturesResources.Format_Document,
//                    message: EditorFeaturesResources.Formatting_document,
//                    allowCancel: true,
//                    action: waitContext =>
//                    {
//                        Format(args.TextView, document, null, waitContext.CancellationToken);
//                        result = true;
//                    });
                
//                // We don't call nextHandler, since we have handled this command.
//                return result;

//    interface ICommandHandler<FormatDocumentCommandArgs> with
//        member __.GetCommandState(args, nextHandler) =
//            getCommandState(args.SubjectBuffer, nextHandler)
 
//        member __.ExecuteCommand(args, nextHandler) =
//            if not (TryExecuteCommand(args)) then
//                nextHandler.Invoke()