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


type internal CodeLensAdornment
    (
        workspace: Workspace,
        documentId: Lazy<DocumentId>,
        view: IWpfTextView, 
        checker: FSharpChecker,
        projectInfoManager: ProjectInfoManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
    ) as self =
    
    let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"
    let codeLensLines = ConcurrentDictionary()

    do assert (documentId <> null)

    let mutable cancellationTokenSource = new CancellationTokenSource()
    let mutable cancellationToken = cancellationTokenSource.Token

    /// Get the interline layer. CodeLens belong there.
    let interlineLayer = view.GetAdornmentLayer(PredefinedAdornmentLayers.InterLine)
    do view.LayoutChanged.AddHandler (fun _ e -> self.OnLayoutChanged e)
    
    let layoutTagToFormatting (layoutTag: LayoutTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> ClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties

    let executeCodeLenseAsync () =
        asyncMaybe {
            try 
                let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
                let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                let hyperlinkStyles = HyperlinkStyles.getCurrent()

                let applyCodeLens bufferPosition (taggedText: seq<Layout.TaggedText>) m =
                    let DoUI () = 
                        try
                            let line = view.TextViewLines.GetTextViewLineContainingBufferPosition(bufferPosition)
                            
                            let offset = 
                                [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
                                |> Option.defaultValue 0

                            let realStart = line.Start.Add(offset)
                            let span = SnapshotSpan(line.Snapshot, Span.FromBounds(int realStart, int line.End))
                            let geometry = view.TextViewLines.GetMarkerGeometry(span)
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
                            
                            textBox.Resources.[typeof<Documents.Hyperlink>] <- hyperlinkStyles
                            Canvas.SetLeft(textBox, geometry.Bounds.Left)
                            Canvas.SetTop(textBox, geometry.Bounds.Top - 15.)
                            let tag = new IntraTextAdornmentTag(textBox, (fun _ _ -> ()))
                            let adornment = new SpaceNegotiatingAdornmentTag(0., 0., 0., 0., 0., PositionAffinity.Predecessor, tag, tag)

                            interlineLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, Nullable (span), adornment, textBox, null) |> ignore
                            if line.VisibilityState = VisibilityState.Unattached then view.DisplayTextLineContainingBufferPosition(line.Start, 0., ViewRelativePosition.Top)
                        with
                        | _ -> ()
                    
                    Application.Current.Dispatcher.Invoke(fun _ -> DoUI())
                
                let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue) =
                    async {
                        try
                            let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                            
                            if (lineNumber >= 0 || lineNumber < view.TextSnapshot.LineCount) && 
                                not func.IsPropertyGetterMethod && 
                                not func.IsPropertySetterMethod then
                        
                                match func.FullTypeSafe with
                                | Some ty ->
                                    let bufferPosition = view.TextSnapshot.GetLineFromLineNumber(lineNumber).Start
                                    if not (codeLensLines.ContainsKey lineNumber) then
                                        let! displayEnv = checkFileResults.GetDisplayEnvForPos(func.DeclarationLocation.Start)
                                        
                                        let displayContext =
                                            match displayEnv with
                                            | Some denv -> FSharpDisplayContext(fun _ -> denv)
                                            | None -> displayContext
                                             
                                        let typeLayout = ty.FormatLayout(displayContext)
                                        let taggedText = ResizeArray()
                                        Layout.renderL (Layout.taggedTextListR taggedText.Add) typeLayout |> ignore
                                        codeLensLines.[lineNumber] <- taggedText
                                        applyCodeLens bufferPosition taggedText func.DeclarationLocation
                                | None -> ()
                        with
                        | _ -> () // suppress any exception according wrong line numbers -.-
                    }
                
                //let forceReformat () =
                //    view.VisualSnapshot.Lines
                //    |> Seq.iter(fun line -> view.DisplayTextLineContainingBufferPosition(line.Start, 25., ViewRelativePosition.Top))

                for symbolUse in symbolUses do
                    if symbolUse.IsFromDefinition then
                        match symbolUse.Symbol with
                        | :? FSharpEntity as entity ->
                            for func in entity.MembersFunctionsAndValues do
                                do! useResults (symbolUse.DisplayContext, func) |> liftAsync
                        | _ -> ()
            with
            | _ -> () // TODO: Should report error
        }

    /// Handles required transformation depending on whether CodeLens are required or not required
    interface ILineTransformSource with
        override __.GetLineTransform(line, _, _) =
            let applyCodeLens = codeLensLines.ContainsKey(view.TextSnapshot.GetLineNumberFromPosition(line.Start.Position))
            if applyCodeLens then
                // Give us space for CodeLens
                LineTransform(15., 1., 1.)
            else
                // Restore old transformation
                line.DefaultLineTransform

    member __.OnLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        // Non expensive computations which have to be done immediate
        for line in e.NewOrReformattedLines do
            let lineNumber = view.TextSnapshot.GetLineNumberFromPosition(line.Start.Position)
            codeLensLines.TryRemove(lineNumber) |> ignore //All changed lines are supposed to be now No-CodeLens-Lines (Reset)
            if line.VisibilityState = VisibilityState.Unattached then 
                view.DisplayTextLineContainingBufferPosition(line.Start, 0., ViewRelativePosition.Top) //Force refresh (works partly...)

        //for line in view.TextViewLines.WpfTextViewLines do
        //    if line.VisibilityState = VisibilityState.Unattached then 
        //        view.DisplayTextLineContainingBufferPosition(line.Start, 0., ViewRelativePosition.Top) //Force refresh (works partly...)
        
        cancellationTokenSource.Cancel() // Stop all ongoing async workflow. 
        cancellationTokenSource.Dispose()
        cancellationTokenSource <- new CancellationTokenSource()
        cancellationToken <- cancellationTokenSource.Token
        executeCodeLenseAsync() |> Async.Ignore |> RoslynHelpers.StartAsyncSafe cancellationToken

[<Export(typeof<ILineTransformSourceProvider>)>]
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
    ) =
    let TextAdornments = ResizeArray<IWpfTextView * CodeLensAdornment>()
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()
    
    /// Returns an provider for the textView if already one has been created. Else create one.
    let getSuitableAdornmentProvider (textView: IWpfTextView) =
        let res = TextAdornments |> Seq.tryFind(fun (view, _) -> view = textView)
        match res with
        | Some (_, res) -> res
        | None ->
            let documentId = 
                lazy(
                    match textDocumentFactory.TryGetTextDocument(textView.TextBuffer) with
                    | true, textDocument ->
                         Seq.tryHead (workspace.CurrentSolution.GetDocumentIdsWithFilePath(textDocument.FilePath))
                    | _ -> None
                    |> Option.get
                )

            let provider = CodeLensAdornment(workspace, documentId, textView, checkerProvider.Checker, projectInfoManager, typeMap, gotoDefinitionService)
            TextAdornments.Add((textView, provider))
            provider

    interface ILineTransformSourceProvider with
        override __.Create textView = 
            getSuitableAdornmentProvider(textView) :> ILineTransformSource