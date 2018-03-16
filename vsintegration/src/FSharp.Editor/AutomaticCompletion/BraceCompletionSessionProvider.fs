// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Diagnostics
open System.Threading
open System.ComponentModel.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Classification
open Microsoft.VisualStudio.Text.BraceCompletion
open Microsoft.VisualStudio.Text.Operations
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

[<AutoOpen>]
module private FSharpBraceCompletionSessionProviderHelpers =

    let getLanguageService<'T when 'T :> ILanguageService and 'T : null> (document: Document) =
        match document.Project with
        | null -> null
        | project ->
            match project.LanguageServices with
            | null -> null
            | languageServices ->
                languageServices.GetService<'T>()

    let getCaretPoint (buffer: ITextBuffer) (session: IBraceCompletionSession) =
        session.TextView.Caret.Position.Point.GetPoint(buffer, PositionAffinity.Predecessor)

    let getCaretPosition session =
        session |> getCaretPoint session.SubjectBuffer

    [<Literal>] 
    let BraceCompletion = "Brace_Completion"

[<AllowNullLiteral>]
type internal IEditorBraceCompletionSession =
    inherit ILanguageService

    abstract CheckOpeningPoint : IBraceCompletionSession * CancellationToken -> bool

    abstract AfterStart : IBraceCompletionSession * CancellationToken -> unit

    abstract AllowOverType : IBraceCompletionSession * CancellationToken -> bool

    abstract AfterReturn : IBraceCompletionSession * CancellationToken -> unit

[<AllowNullLiteral>]
type internal IEditorBraceCompletionSessionFactory =
    inherit ILanguageService

    abstract TryCreateSession : Document * openingPosition: int * openingBrace: char * CancellationToken -> IEditorBraceCompletionSession

type internal FSharpBraceCompletionSession 
    (
        textView: ITextView,
        subjectBuffer: ITextBuffer,
        openingPoint: SnapshotPoint,
        openingBrace: char,
        closingBrace: char,
        undoHistory: ITextUndoHistory,
        editorOperationsFactoryService: IEditorOperationsFactoryService,
        session: IEditorBraceCompletionSession
    ) =

    let mutable closingPoint = subjectBuffer.CurrentSnapshot.CreateTrackingPoint(openingPoint.Position, PointTrackingMode.Positive)
    let mutable openingPoint : ITrackingPoint = null
    let editorOperations = editorOperationsFactoryService.GetEditorOperations(textView)

    member this.EndSession() =
        closingPoint <- null
        openingPoint <- null

    member __.CreateUndoTransaction() =
        undoHistory.CreateTransaction(BraceCompletion)

    member this.Start (cancellationToken: CancellationToken) =
        // this is where the caret should go after the change
        let pos = textView.Caret.Position.BufferPosition
        let beforeTrackingPoint = pos.Snapshot.CreateTrackingPoint(pos.Position, PointTrackingMode.Negative)

        let mutable snapshot = subjectBuffer.CurrentSnapshot
        let closingSnapshotPoint = closingPoint.GetPoint(snapshot)

        if closingSnapshotPoint.Position < 1 then
            Debug.Fail("The closing point was not found at the expected position.")
            this.EndSession()
        else

            let openingSnapshotPoint = closingSnapshotPoint.Subtract(1)

            if openingSnapshotPoint.GetChar() <> openingBrace then
                // there is a bug in editor brace completion engine on projection buffer that already fixed in vs_pro. until that is FIed to use
                // I will make this not to assert
                // Debug.Fail("The opening brace was not found at the expected position.");
                this.EndSession()
            else

                openingPoint <- snapshot.CreateTrackingPoint(openingSnapshotPoint.Position, PointTrackingMode.Positive)
                let _document = snapshot.GetOpenDocumentInCurrentContextWithChanges()

                if not (session.CheckOpeningPoint(this, cancellationToken)) then
                    this.EndSession()
                else

                    use undo = this.CreateUndoTransaction()

                    snapshot <-
                        use edit = subjectBuffer.CreateEdit()

                        edit.Insert(closingSnapshotPoint.Position, closingBrace.ToString()) |> ignore
                    
                        if edit.HasFailedChanges then
                            Debug.Fail("Unable to insert closing brace")

                            // exit without setting the closing point which will take us off the stack
                            edit.Cancel()
                            undo.Cancel()
                            snapshot
                        else
                            edit.Apply() // FIXME: perhaps, it should be ApplyAndLogExceptions()

                    let beforePoint = beforeTrackingPoint.GetPoint(textView.TextSnapshot)

                    // switch from positive to negative tracking so it stays against the closing brace
                    closingPoint <- subjectBuffer.CurrentSnapshot.CreateTrackingPoint(closingPoint.GetPoint(snapshot).Position, PointTrackingMode.Negative)

                    Debug.Assert(closingPoint.GetPoint(snapshot).Position > 0 && (SnapshotSpan(closingPoint.GetPoint(snapshot).Subtract(1), 1)).GetText().Equals(closingBrace.ToString()),
                        "The closing point does not match the closing brace character")

                    // move the caret back between the braces
                    textView.Caret.MoveTo(beforePoint) |> ignore

                    session.AfterStart(this, cancellationToken)

                    undo.Complete()

    member this.HasNoForwardTyping(caretPoint: SnapshotPoint, endPoint: SnapshotPoint) =
        Debug.Assert(caretPoint.Snapshot = endPoint.Snapshot, "snapshots do not match")

        if caretPoint.Snapshot = endPoint.Snapshot then
            if caretPoint = endPoint then
                true
            else
                if caretPoint.Position < endPoint.Position then
                    let span = SnapshotSpan(caretPoint, endPoint)
                    String.IsNullOrWhiteSpace(span.GetText())
                else
                    false
        else
            false

    member this.HasForwardTyping =
        let closingSnapshotPoint = closingPoint.GetPoint(subjectBuffer.CurrentSnapshot)

        if closingSnapshotPoint.Position > 0 then
            let caretPos = getCaretPosition this

            if caretPos.HasValue && not (this.HasNoForwardTyping(caretPos.Value, closingSnapshotPoint.Subtract(1))) then
                true
            else
                false
        else
            false

    member __.MoveCaretToClosingPoint() =
        let closingSnapshotPoint = closingPoint.GetPoint(subjectBuffer.CurrentSnapshot)

        // find the position just after the closing brace in the view's text buffer
        let afterBrace = 
            textView.BufferGraph.MapUpToBuffer(closingSnapshotPoint, PointTrackingMode.Negative, PositionAffinity.Predecessor, textView.TextBuffer)

        Debug.Assert(afterBrace.HasValue, "Unable to move caret to closing point")

        if afterBrace.HasValue then
            textView.Caret.MoveTo(afterBrace.Value) |> ignore

    interface IBraceCompletionSession with

        member this.Start() =
            // Brace completion is not cancellable.
            this.Start(CancellationToken.None)
                        
        member this.PreBackspace handledCommand =
            handledCommand <- false

            let caretPos = getCaretPosition this
            let snapshot = subjectBuffer.CurrentSnapshot

            if caretPos.HasValue && caretPos.Value.Position > 0 &&
                    caretPos.Value.Position - 1 = openingPoint.GetPoint(snapshot).Position &&
                    not this.HasForwardTyping then
                
                use undo = this.CreateUndoTransaction()
                use edit = subjectBuffer.CreateEdit()

                let span = SnapshotSpan(openingPoint.GetPoint(snapshot), closingPoint.GetPoint(snapshot))

                edit.Delete(span.Span) |> ignore

                if edit.HasFailedChanges then
                    edit.Cancel()
                    undo.Cancel()
                    Debug.Fail("Unable to clear braces")
                else
                    // handle the command so the backspace does 
                    // not go through since we've already cleared the braces
                    handledCommand <- true
                    edit.Apply() |> ignore // FIXME: ApplyAndLogExceptions()
                    undo.Complete()
                    this.EndSession()
           
        member __.PostBackspace() = ()

        member this.PreOverType handledCommand =
            handledCommand <- false

            if closingPoint <> null then
                // Brace completion is not cancellable.
                let cancellationToken = CancellationToken.None
                let snapshot = subjectBuffer.CurrentSnapshot
                let _document = snapshot.GetOpenDocumentInCurrentContextWithChanges()

                let closingSnapshotPoint = closingPoint.GetPoint(snapshot)
                if not this.HasForwardTyping && session.AllowOverType(this, cancellationToken) then
                    let caretPos = getCaretPosition this

                    Debug.Assert(caretPos.HasValue && caretPos.Value.Position < closingSnapshotPoint.Position)

                    // ensure that we are within the session before clearing
                    if caretPos.HasValue && caretPos.Value.Position < closingSnapshotPoint.Position && closingSnapshotPoint.Position > 0 then

                        use undo = this.CreateUndoTransaction()

                        let span = SnapshotSpan(caretPos.Value, closingSnapshotPoint.Subtract(1))

                        use edit = subjectBuffer.CreateEdit()

                        edit.Delete(span.Span) |> ignore

                        if edit.HasFailedChanges then
                            Debug.Fail("Unable to clear closing brace")
                            edit.Cancel()
                            undo.Cancel()
                        else
                            handledCommand <- true
                            edit.Apply() |> ignore // FIXME: ApplyAndLogExceptions()
                            this.MoveCaretToClosingPoint()
                            editorOperations.AddAfterTextBufferChangePrimitive()
                            undo.Complete()
                
        member __.PostOverType() = ()

        member this.PreTab handledCommand =
            handledCommand <- false

            if not this.HasForwardTyping then
                handledCommand <- true

                use undo = this.CreateUndoTransaction()

                editorOperations.AddBeforeTextBufferChangePrimitive()
                this.MoveCaretToClosingPoint()
                editorOperations.AddAfterTextBufferChangePrimitive()
                undo.Complete()

        member __.PreReturn handledCommand =
            handledCommand <- false

        member this.PostReturn() =
            let caretPos = getCaretPosition this

            if caretPos.HasValue then
                let closingSnapshotPoint = closingPoint.GetPoint(subjectBuffer.CurrentSnapshot)

                if closingSnapshotPoint.Position > 0 && this.HasNoForwardTyping(caretPos.Value, closingSnapshotPoint.Subtract(1)) then
                    session.AfterReturn(this, CancellationToken.None)
                
        member __.Finish() = ()

        member __.PostTab() = ()

        member __.PreDelete handledCommand =
            handledCommand <- false
        
        member __.PostDelete() = ()

        member __.OpeningBrace = openingBrace

        member __.ClosingBrace = closingBrace

        member __.OpeningPoint = openingPoint

        member __.ClosingPoint = closingPoint

        member __.SubjectBuffer = subjectBuffer

        member __.TextView = textView

module private Parenthesis =

    [<Literal>]
    let OpenCharacter = '('

    [<Literal>]
    let CloseCharacter = ')'

module private CurlyBrackets =

    [<Literal>]
    let OpenCharacter = '{'

    [<Literal>]
    let CloseCharacter = '}'

module private SquareBrackets =

    [<Literal>]
    let OpenCharacter = '['

    [<Literal>]
    let CloseCharacter = ']'

module private DoubleQuote =

    [<Literal>]
    let OpenCharacter = '"'

    [<Literal>]
    let CloseCharacter = '"'

module private VerticalBar =

    [<Literal>]
    let OpenCharacter = '|'

    [<Literal>]
    let CloseCharacter = '|'

type internal ParenthesisCompletionSession () =
    
    interface IEditorBraceCompletionSession with

        member this.AfterReturn(_session, _cancellationToken) = 
            ()

        member this.AfterStart(_session, _cancellationToken) = 
            ()

        member this.AllowOverType(_session, _cancellationToken) = 
            // TODO: Implement this for F#
            true

        member this.CheckOpeningPoint(_session, _cancellationToken) = 
            // TODO: Implement this for F#
            true 

type internal DoubleQuoteCompletionSession () =
    
    interface IEditorBraceCompletionSession with

        member this.AfterReturn(_session, _cancellationToken) = 
            ()

        member this.AfterStart(_session, _cancellationToken) = 
            ()

        member this.AllowOverType(_session, _cancellationToken) = 
            // TODO: Implement this for F#
            true

        member this.CheckOpeningPoint(_session, _cancellationToken) = 
            // TODO: Implement this for F#
            true 

type internal VerticalBarCompletionSession () =
    
    interface IEditorBraceCompletionSession with

        member this.AfterReturn(_session, _cancellationToken) = 
            ()

        member this.AfterStart(_session, _cancellationToken) = 
            ()

        member this.AllowOverType(_session, _cancellationToken) = 
            // TODO: Implement this for F#
            true

        member this.CheckOpeningPoint(_session, _cancellationToken) = 
            let sourceCode = _session.TextView.TextSnapshot
            //let document = _session.TextView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges()
            let position = _session.TextView.Caret.Position.BufferPosition.Position
            let ret = sourceCode.GetText(position-2,1) = "{" && sourceCode.GetText(position,1) = "}"
                        || sourceCode.GetText(position-2,1) = "[" && sourceCode.GetText(position,1) = "]"
            ret
            
[<ExportLanguageService(typeof<IEditorBraceCompletionSessionFactory>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpEditorBraceCompletionSessionFactory () =
    inherit ForegroundThreadAffinitizedObject ()

    member __.IsSupportedOpeningBrace openingBrace =
        match openingBrace with
        | Parenthesis.OpenCharacter | CurlyBrackets.OpenCharacter | SquareBrackets.OpenCharacter
        | DoubleQuote.OpenCharacter | VerticalBar.OpenCharacter -> true
        | _ -> false

    member this.CheckCodeContext(_document : Document, _position : int, _openingBrace, _cancellationToken) =
        this.AssertIsForeground()
    // TODO: We need to know if we are inside a F# comment. If we are, then don't do automatic completion.
        let sourceCodeTask = _document.GetTextAsync(_cancellationToken)
        sourceCodeTask.Wait(_cancellationToken)
        let sourceCode = sourceCodeTask.Result
        let colorizationData = Tokenizer.getColorizationData(_document.Id, sourceCode, TextSpan(_position-1, 1), Some (_document.FilePath), [], _cancellationToken)
        colorizationData.Exists (fun classifiedSpan -> 
            classifiedSpan.TextSpan.IntersectsWith _position &&
            (
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment
                | ClassificationTypeNames.StringLiteral -> false
                | _ -> true // anything else is a valid classification type
            ))                

    member this.CreateEditorSession(_document: Document, _openingPosition: int, openingBrace: char, _cancellationToken: CancellationToken) =
        match openingBrace with
        | Parenthesis.OpenCharacter -> ParenthesisCompletionSession() :> IEditorBraceCompletionSession
        | CurlyBrackets.OpenCharacter -> ParenthesisCompletionSession() :> IEditorBraceCompletionSession
        | SquareBrackets.OpenCharacter -> ParenthesisCompletionSession() :> IEditorBraceCompletionSession
        | VerticalBar.OpenCharacter -> VerticalBarCompletionSession() :> IEditorBraceCompletionSession
        | DoubleQuote.OpenCharacter -> DoubleQuoteCompletionSession() :> IEditorBraceCompletionSession
        | _ -> null

    interface IEditorBraceCompletionSessionFactory with

        member this.TryCreateSession(document, openingPosition, openingBrace, cancellationToken) : IEditorBraceCompletionSession = 
            this.AssertIsForeground()

            if this.IsSupportedOpeningBrace(openingBrace) && this.CheckCodeContext(document, openingPosition, openingBrace, cancellationToken) then
                this.CreateEditorSession(document, openingPosition, openingBrace, cancellationToken)
            else
                null

[<Export(typeof<IBraceCompletionSessionProvider>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<BracePair(Parenthesis.OpenCharacter, Parenthesis.CloseCharacter)>]
[<BracePair(CurlyBrackets.OpenCharacter, CurlyBrackets.CloseCharacter)>]
[<BracePair(SquareBrackets.OpenCharacter, SquareBrackets.CloseCharacter)>]
[<BracePair(DoubleQuote.OpenCharacter, DoubleQuote.CloseCharacter)>]
[<BracePair(VerticalBar.OpenCharacter, VerticalBar.CloseCharacter)>]
type internal FSharpBraceCompletionSessionProvider
    [<ImportingConstructor>] 
    (
        undoManager: ITextBufferUndoManagerProvider,
        editorOperationsFactoryService: IEditorOperationsFactoryService
    ) =

    inherit ForegroundThreadAffinitizedObject ()

    interface IBraceCompletionSessionProvider with

        member this.TryCreateSession(textView, openingPoint, openingBrace, closingBrace, session) =
            this.AssertIsForeground();

            let textSnapshot = openingPoint.Snapshot

            let newSession =
                match textSnapshot.GetOpenDocumentInCurrentContextWithChanges() with
                | null -> null
                | document ->
                    match getLanguageService<IEditorBraceCompletionSessionFactory> document with
                    | null -> null
                    | editorSessionFactory ->
                        // Brace completion is (currently) not cancellable.
                        let cancellationToken = CancellationToken.None
                        
                        match editorSessionFactory.TryCreateSession(document, openingPoint.Position, openingBrace, cancellationToken) with
                        | null -> null
                        | editorSession ->
                            let undoHistory = undoManager.GetTextBufferUndoManager(textView.TextBuffer).TextBufferUndoHistory
                            FSharpBraceCompletionSession(
                                textView,
                                openingPoint.Snapshot.TextBuffer,
                                openingPoint, 
                                openingBrace, 
                                closingBrace, 
                                undoHistory, 
                                editorOperationsFactoryService, 
                                editorSession) :> IBraceCompletionSession

            session <- newSession

            match session with
            | null -> false
            | _ -> true