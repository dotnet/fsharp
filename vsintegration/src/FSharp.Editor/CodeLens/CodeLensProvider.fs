// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.Shared.Utilities

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<Export(typeof<IViewTaggerProvider>)>]
[<TagType(typeof<CodeLensGeneralTag>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
type internal CodeLensProvider  
    [<ImportingConstructor>]
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        textDocumentFactory: ITextDocumentFactoryService,
        metadataAsSource: FSharpMetadataAsSourceService,
        typeMap : FSharpClassificationTypeMap Lazy,
        settings: EditorOptions
    ) =

    let tryGetTextDocument (buffer: ITextBuffer) (factory: ITextDocumentFactoryService) = 
        match factory.TryGetTextDocument buffer with
        | true, document -> Some document
        | _ -> None

    let taggers = ResizeArray()
    let lineLensProviders = ResizeArray()
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    let tryGetCodeLensTagger wpfView buffer =
        taggers 
        |> Seq.tryFind (fun (view, _) -> view = wpfView) 
        |> Option.map (fun (_, (tagger, _)) -> tagger)
        |> Option.orElse
            (textDocumentFactory 
            |> tryGetTextDocument buffer
            |> Option.map (fun document -> workspace.CurrentSolution.GetDocumentIdsWithFilePath document.FilePath)
            |> Option.bind Seq.tryHead
            |> Option.map (fun documentId -> 
                let tagger = CodeLensGeneralTagger(wpfView, buffer)
                let service = FSharpCodeLensService(serviceProvider, workspace, documentId, buffer, metadataAsSource, componentModel.GetService(), typeMap, tagger, settings)
                let provider = (wpfView, (tagger, service))
                wpfView.Closed.Add (fun _ -> taggers.Remove provider |> ignore)
                taggers.Add provider
                tagger))

    let addLineLensProvider wpfView buffer =
        textDocumentFactory
        |> tryGetTextDocument buffer
        |> Option.map (fun document -> workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath))
        |> Option.bind Seq.tryHead
        |> Option.map (fun documentId ->
            let service = FSharpCodeLensService(serviceProvider, workspace, documentId, buffer, metadataAsSource, componentModel.GetService(), typeMap, LineLensDisplayService(wpfView, buffer), settings)
            let provider = (wpfView, service)
            wpfView.Closed.Add (fun _ -> lineLensProviders.Remove provider |> ignore)
            lineLensProviders.Add(provider))

    [<Export(typeof<AdornmentLayerDefinition>); Name("CodeLens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]
    member val CodeLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set
    
    [<Export(typeof<AdornmentLayerDefinition>); Name("LineLens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]
    member val LineLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IViewTaggerProvider with
        override _.CreateTagger(view, buffer) =
            if settings.CodeLens.Enabled && not settings.CodeLens.ReplaceWithLineLens then
                let wpfView =
                    match view with
                    | :? IWpfTextView as view -> view
                    | _ -> failwith "error"
            
                match tryGetCodeLensTagger wpfView buffer with
                | Some tagger -> box tagger :?> _
                | None -> null
            else
                null

    interface IWpfTextViewCreationListener with
        override _.TextViewCreated view =
            if settings.CodeLens.Enabled && settings.CodeLens.ReplaceWithLineLens then
                let provider = 
                    lineLensProviders 
                    |> Seq.tryFind (fun (v, _) -> v = view)

                if provider.IsNone then
                    addLineLensProvider view (view.TextBuffer) |> ignore