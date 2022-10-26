// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

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
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
type internal LensProvider  
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

    let lensProviders = ResizeArray()
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    let addLensProvider wpfView buffer =
        textDocumentFactory
        |> tryGetTextDocument buffer
        |> Option.map (fun document -> workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath))
        |> Option.bind Seq.tryHead
        |> Option.map (fun documentId ->
            let service = LensService(serviceProvider, workspace, documentId, buffer, metadataAsSource, componentModel.GetService(), typeMap, LensDisplayService(wpfView, buffer), settings)
            let provider = (wpfView, service)
            wpfView.Closed.Add (fun _ -> lensProviders.Remove provider |> ignore)
            lensProviders.Add(provider))

    [<Export(typeof<AdornmentLayerDefinition>); Name("Lens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]
    member val LensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IWpfTextViewCreationListener with
        override _.TextViewCreated view =
            if settings.Lens.Enabled then
                let provider = 
                    lensProviders 
                    |> Seq.tryFind (fun (v, _) -> v = view)

                if provider.IsNone then
                    addLensProvider view (view.TextBuffer) |> ignore