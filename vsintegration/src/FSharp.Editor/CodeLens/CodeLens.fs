// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows.Controls
open System.Windows.Media
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis
open System.Threading
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open System.Windows
open System.Collections.Generic
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Classification
open Internal.Utilities.StructuredFormat
open Microsoft.VisualStudio.Text.Tagging
open System.Collections.Concurrent
open System.Collections


//type internal CodeLensAdornment
//    (
//        workspace: Workspace,
//        documentId: Lazy<DocumentId>,
//        view: IWpfTextView, 
//        checker: FSharpChecker,
//        projectInfoManager: ProjectInfoManager,
//        typeMap: Lazy<ClassificationTypeMap>,
//        gotoDefinitionService: FSharpGoToDefinitionService
//    ) as self =
    
//    let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"
//    let codeLensLines = ConcurrentDictionary()

//    do assert (documentId <> null)

//    let mutable cancellationTokenSource = new CancellationTokenSource()
//    let mutable cancellationToken = cancellationTokenSource.Token

//    /// Get the interline layer. CodeLens belong there.
//    let interlineLayer = view.GetAdornmentLayer(PredefinedAdornmentLayers.InterLine)
//    do view.LayoutChanged.AddHandler (fun _ e -> self.OnLayoutChanged e)
    
//    let layoutTagToFormatting (layoutTag: LayoutTag) =
//        layoutTag
//        |> RoslynHelpers.roslynTag
//        |> ClassificationTags.GetClassificationTypeName
//        |> typeMap.Value.GetClassificationType
//        |> formatMap.Value.GetTextProperties

//    let executeCodeLenseAsync () =
//        let uiContext = SynchronizationContext.Current
//        asyncMaybe {
//            try 
//                let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
//                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
//                let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
//                let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
//                do! Async.SwitchToContext uiContext |> liftAsync

//                let applyCodeLens bufferPosition (taggedText: seq<Layout.TaggedText>) m =
//                    let line = view.TextViewLines.GetTextViewLineContainingBufferPosition(bufferPosition)
                    
//                    let offset = 
//                        [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
//                        |> Option.defaultValue 0

//                    let realStart = line.Start.Add(offset)
//                    let span = SnapshotSpan(line.Snapshot, Span.FromBounds(int realStart, int line.End))
//                    let geometry = view.TextViewLines.GetMarkerGeometry(span)
//                    let textBox = TextBlock(Width = 500., Background = Brushes.Transparent, Opacity = 0.5, TextTrimming = TextTrimming.WordEllipsis)
//                    DependencyObjectExtensions.SetDefaultTextProperties(textBox, formatMap.Value)
//                    let navigation = QuickInfoNavigation(gotoDefinitionService, document, m)
//                    for text in taggedText do
//                        let run = Documents.Run text.Text
//                        DependencyObjectExtensions.SetTextProperties (run, layoutTagToFormatting text.Tag)
//                        let inl =
//                           match text with
//                           | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->                        
//                               let h = Documents.Hyperlink(run, ToolTip = nav.Range.FileName)
//                               h.Click.Add (fun _ -> navigation.NavigateTo nav.Range)
//                               h :> Documents.Inline
//                           | _ -> run :> _
//                        textBox.Inlines.Add inl
                    
//                    Canvas.SetLeft(textBox, geometry.Bounds.Left)
//                    Canvas.SetTop(textBox, geometry.Bounds.Top - 15.)
//                    let tag = IntraTextAdornmentTag(textBox, (fun _ _ -> ()))
//                    let adornment = SpaceNegotiatingAdornmentTag(0., 0., 0., 0., 0., PositionAffinity.Predecessor, tag, tag)
//                    interlineLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, Nullable span, adornment, textBox, null) |> ignore
//                    if line.VisibilityState = VisibilityState.Unattached then view.DisplayTextLineContainingBufferPosition(line.Start, 0., ViewRelativePosition.Top)
                
//                let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue) =
//                    async {
//                        let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                        
//                        if (lineNumber >= 0 || lineNumber < view.TextSnapshot.LineCount) && 
//                            not func.IsPropertyGetterMethod && 
//                            not func.IsPropertySetterMethod then
                        
//                            match func.FullTypeSafe with
//                            | Some ty ->
//                                let bufferPosition = view.TextSnapshot.GetLineFromLineNumber(lineNumber).Start
//                                if not (codeLensLines.ContainsKey lineNumber) then
//                                    let! displayEnv = checkFileResults.GetDisplayEnvForPos(func.DeclarationLocation.Start)
                                    
//                                    let displayContext =
//                                        match displayEnv with
//                                        | Some denv -> FSharpDisplayContext(fun _ -> denv)
//                                        | None -> displayContext
                                         
//                                    let typeLayout = ty.FormatLayout(displayContext)
//                                    let taggedText = ResizeArray()
//                                    Layout.renderL (Layout.taggedTextListR taggedText.Add) typeLayout |> ignore
//                                    codeLensLines.[lineNumber] <- taggedText
//                                    applyCodeLens bufferPosition taggedText func.DeclarationLocation
//                            | None -> ()
//                    }
                
//                //let forceReformat () =
//                //    view.VisualSnapshot.Lines
//                //    |> Seq.iter(fun line -> view.DisplayTextLineContainingBufferPosition(line.Start, 25., ViewRelativePosition.Top))

//                for symbolUse in symbolUses do
//                    if symbolUse.IsFromDefinition then
//                        match symbolUse.Symbol with
//                        | :? FSharpEntity as entity ->
//                            for func in entity.MembersFunctionsAndValues do
//                                do! useResults (symbolUse.DisplayContext, func) |> liftAsync
//                        | _ -> ()
//            with
//            | _ -> () // TODO: Should report error
//        }

//    /// Handles required transformation depending on whether CodeLens are required or not required
//    interface ILineTransformSource with
//        override __.GetLineTransform(line, _, _) =
//            let applyCodeLens = codeLensLines.ContainsKey(view.TextSnapshot.GetLineNumberFromPosition(line.Start.Position))
//            if applyCodeLens then
//                // Give us space for CodeLens
//                LineTransform(15., 1., 1.)
//            else
//                // Restore old transformation
//                line.DefaultLineTransform

//    member __.OnLayoutChanged (e:TextViewLayoutChangedEventArgs) =
//        // Non expensive computations which have to be done immediate
//        for line in e.NewOrReformattedLines do
//            let lineNumber = view.TextSnapshot.GetLineNumberFromPosition(line.Start.Position)
//            codeLensLines.TryRemove(lineNumber) |> ignore //All changed lines are supposed to be now No-CodeLens-Lines (Reset)
//            if line.VisibilityState = VisibilityState.Unattached then 
//                view.DisplayTextLineContainingBufferPosition(line.Start, 0., ViewRelativePosition.Top) //Force refresh (works partly...)

//        //for line in view.TextViewLines.WpfTextViewLines do
//        //    if line.VisibilityState = VisibilityState.Unattached then 
//        //        view.DisplayTextLineContainingBufferPosition(line.Start, 0., ViewRelativePosition.Top) //Force refresh (works partly...)
        
//        cancellationTokenSource.Cancel() // Stop all ongoing async workflow. 
//        cancellationTokenSource.Dispose()
//        cancellationTokenSource <- new CancellationTokenSource()
//        cancellationToken <- cancellationTokenSource.Token
//        executeCodeLenseAsync() |> Async.Ignore |> RoslynHelpers.StartAsyncSafe cancellationToken

 

type CodeLensTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, identityTag:obj, providerTag:obj, text) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, identityTag, providerTag)
    
    new (width, topSpace, baseline, textHeight, bottomSpace, affinity, identityTag:obj, providerTag:obj) =
        new CodeLensTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, identityTag, providerTag, "Lalala")

    member val Text = text with get, set


type internal CodeLensTagger  
    (
        workspace: Workspace,
        documentId: Lazy<DocumentId>,
        buffer: ITextBuffer, 
        checker: FSharpChecker,
        projectInfoManager: ProjectInfoManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
     ) as self =
        let tagsChanged = new Event<EventHandler<SnapshotSpanEventArgs>,SnapshotSpanEventArgs>()

        let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"
        let codeLensLines = ConcurrentDictionary()

        do assert (documentId <> null)

        let mutable cancellationTokenSourceBufferChanged = new CancellationTokenSource()
        let mutable cancellationTokenBufferChanged = cancellationTokenSourceBufferChanged.Token
        let mutable cancellationTokenSourceLayoutChanged = new CancellationTokenSource()
    
        let layoutTagToFormatting (layoutTag: LayoutTag) =
            layoutTag
            |> RoslynHelpers.roslynTag
            |> ClassificationTags.GetClassificationTypeName
            |> typeMap.Value.GetClassificationType
            |> formatMap.Value.GetTextProperties

        let executeCodeLenseAsync () =  
            let uiContext = SynchronizationContext.Current
            asyncMaybe {
                try 
                    let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
                    let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                    let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
                    let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                    
                    do! Async.SwitchToContext uiContext |> liftAsync
                    codeLensLines.Clear()
                    let view = self.WpfTextView.Value;
                    let textSnapshot = view.TextSnapshot.TextBuffer.CurrentSnapshot
                    Logging.Logging.logInfof "Updating code lens due to buffer edit!"
                    let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue) =
                        async {
                            let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                        
                            if (lineNumber >= 0 || lineNumber < textSnapshot.LineCount) && 
                                not func.IsPropertyGetterMethod && 
                                not func.IsPropertySetterMethod then
                        
                                match func.FullTypeSafe with
                                | Some ty ->
                                    let! displayEnv = checkFileResults.GetDisplayEnvForPos(func.DeclarationLocation.Start)
                                    
                                    let displayContext =
                                        match displayEnv with
                                        | Some denv -> FSharpDisplayContext(fun _ -> denv)
                                        | None -> displayContext
                                         
                                    let typeLayout = ty.FormatLayout(displayContext)
                                    let taggedText = ResizeArray()
                                    Layout.renderL (Layout.taggedTextListR taggedText.Add) typeLayout |> ignore

                                    let m = func.DeclarationLocation
                                    
                                    let textBox = TextBlock(Width = 500., Background = Brushes.Transparent, Opacity = 0.5, TextTrimming = TextTrimming.WordEllipsis)
                                    DependencyObjectExtensions.SetDefaultTextProperties(textBox, formatMap.Value)
                                    let navigation = QuickInfoNavigation(gotoDefinitionService, document, m)
                                    for text in taggedText do
                                        let run = Documents.Run text.Text
                                        DependencyObjectExtensions.SetTextProperties (run, layoutTagToFormatting text.Tag)
                                        let inl =
                                            match text with
                                            | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->                        
                                                let h = Documents.Hyperlink(run, ToolTip = nav.Range.FileName)
                                                h.Click.Add (fun _ -> navigation.NavigateTo nav.Range)
                                                h :> Documents.Inline
                                            | _ -> run :> _
                                        textBox.Inlines.Add inl
                                        
                                    codeLensLines.[lineNumber] <- textBox

                                | None -> ()
                        }

                    for symbolUse in symbolUses do
                        if symbolUse.IsFromDefinition then
                            match symbolUse.Symbol with
                            | :? FSharpEntity as entity ->
                                for func in entity.MembersFunctionsAndValues do
                                    do! useResults (symbolUse.DisplayContext, func) |> liftAsync
                            | _ -> ()
                    
                    Logging.Logging.logInfof "Finished updating code lens."
                    view.DisplayTextLineContainingBufferPosition(view.TextViewLines.FirstVisibleLine.Start, 0., ViewRelativePosition.Top)
                with
                | e -> Logging.Logging.logErrorf "Error occured: %A" e
                       ()
            }
    
    

        do buffer.Changed.AddHandler(fun _ e -> (self.BufferChanged e))



        member __.BufferChanged _ =
            cancellationTokenSourceBufferChanged.Cancel() // Stop all ongoing async workflow. 
            cancellationTokenSourceBufferChanged.Dispose()
            cancellationTokenSourceBufferChanged <- new CancellationTokenSource()
            cancellationTokenBufferChanged <- cancellationTokenSourceBufferChanged.Token
            executeCodeLenseAsync() |> Async.Ignore |> RoslynHelpers.StartAsyncSafe cancellationTokenBufferChanged
                
        member val WpfTextView : Lazy<IWpfTextView> = Lazy<IWpfTextView>() with get,set 

        member val CodeLensLayer = Lazy<IAdornmentLayer>() with get, set

        member __.LayoutChanged (e:TextViewLayoutChangedEventArgs) = 
            let uiContext = SynchronizationContext.Current
            cancellationTokenSourceLayoutChanged.Cancel()
            cancellationTokenSourceLayoutChanged.Dispose()
            cancellationTokenSourceLayoutChanged <- new CancellationTokenSource()
            async{
                do Async.Sleep(50) |> ignore
                do! Async.SwitchToContext uiContext
                for line in e.NewOrReformattedLines do
                    let tags = line.GetAdornmentTags self
                    let tag = tags |> Seq.tryHead
                    match tag with
                    | None -> ()
                    | Some t ->
                        try
                            if not self.CodeLensLayer.IsValueCreated then
                                self.CodeLensLayer.Force() |> ignore
                            let layer = self.CodeLensLayer.Value
                            let offset = 
                                [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
                                |> Option.defaultValue 0
                            let view = self.WpfTextView.Value
                            let realStart = line.Start.Add(offset)
                            let span = SnapshotSpan(line.Snapshot, Span.FromBounds(int realStart, int line.End))
                            let geometry = view.TextViewLines.GetMarkerGeometry(span)
                            let ui = codeLensLines.[buffer.CurrentSnapshot.GetLineNumberFromPosition(line.Start.Position)]
                            Canvas.SetLeft(ui, geometry.Bounds.Left)
                            Canvas.SetTop(ui, geometry.Bounds.Top - 15.)
                            layer.AddAdornment(line.Extent, t, ui) |> ignore
                        with
                        | e -> Logging.Logging.logErrorf "Error occured: %A" e
                        ()
            } |> Async.Ignore |> RoslynHelpers.StartAsyncSafe cancellationTokenSourceLayoutChanged.Token
            ()

        interface ITagger<CodeLensTag> with
            [<CLIEvent>]
            member this.TagsChanged = tagsChanged.Publish

            override __.GetTags spans =
                    seq{
                        let translatedSpans = 
                            new NormalizedSnapshotSpanCollection(
                                spans 
                                |> Seq.map (fun span -> span.TranslateTo(buffer.CurrentSnapshot, SpanTrackingMode.EdgeExclusive)))
                        Logging.Logging.logMsgf "Iterating over %A spans" spans.Count
                        for tagSpan in translatedSpans do
                            if self.WpfTextView.IsValueCreated then
                                let lineNumber = buffer.CurrentSnapshot.GetLineNumberFromPosition(tagSpan.Start.Position)
                                let codeLens = codeLensLines
                                if codeLens.ContainsKey(lineNumber) then
                                    yield TagSpan(tagSpan, CodeLensTag(0., 15., 0., 0., 0., PositionAffinity.Predecessor, self, self)):> ITagSpan<CodeLensTag>
                       }


[<Export(typeof<IWpfTextViewCreationListener>)>]
[<Export(typeof<ITaggerProvider>)>]
[<TagType(typeof<CodeLensTag>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]

type internal CodeLensProvider  
    [<ImportingConstructor>]
    (
        textDocumentFactory: ITextDocumentFactoryService,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
    ) as __ =
    let CodeLens = ResizeArray<_ * _>()
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    /// Returns an provider for the textView if already one has been created. Else create one.
    let getSuitableAdornmentProvider (buffer) =
        let res = CodeLens |> Seq.tryFind(fun (view, _) -> view = buffer)
        match res with
        | Some (_, res) -> res
        | None ->
            let documentId = 
                lazy(
                    match textDocumentFactory.TryGetTextDocument(buffer) with
                    | true, textDocument ->
                         Seq.tryHead (workspace.CurrentSolution.GetDocumentIdsWithFilePath(textDocument.FilePath))
                    | _ -> None
                    |> Option.get
                )

            let provider = CodeLensTagger(workspace, documentId, buffer, checkerProvider.Checker, projectInfoManager, typeMap, gotoDefinitionService)
            CodeLens.Add((buffer, provider))
            provider

    
    
    [<Export(typeof<AdornmentLayerDefinition>); Name("CodeLens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]
    member val CodeLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IWpfTextViewCreationListener with
        override __.TextViewCreated view =
            let tagger = getSuitableAdornmentProvider view.TextBuffer
            tagger.WpfTextView <- Lazy<_>.CreateFromValue view
            tagger.CodeLensLayer <- Lazy<_>.CreateFromValue(view.GetAdornmentLayer "CodeLens")
            view.LayoutChanged.AddHandler(fun _ e -> tagger.LayoutChanged e)
            view.DisplayTextLineContainingBufferPosition(view.TextViewLines.FirstVisibleLine.Start, 0., ViewRelativePosition.Top)
            ()

    interface ITaggerProvider with
        override __.CreateTagger(buffer) =
            let tagger = getSuitableAdornmentProvider buffer
            box (tagger) :?> _