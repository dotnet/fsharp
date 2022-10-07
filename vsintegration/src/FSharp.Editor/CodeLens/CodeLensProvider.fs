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

    let lineLensProviders = ResizeArray()
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    let addLineLensProvider wpfView buffer =
        textDocumentFactory
        |> tryGetTextDocument buffer
        |> Option.map (fun document -> workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath))
        |> Option.bind Seq.tryHead
        |> Option.map (fun documentId ->
            let service = FSharpCodeLensService(serviceProvider, workspace, documentId, buffer, metadataAsSource, componentModel.GetService(), typeMap, CodeLensDisplayService(wpfView, buffer), settings)
            let provider = (wpfView, service)
            wpfView.Closed.Add (fun _ -> lineLensProviders.Remove provider |> ignore)
            lineLensProviders.Add(provider))

    [<Export(typeof<AdornmentLayerDefinition>); Name("LineLens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]
    member val LineLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IWpfTextViewCreationListener with
        override _.TextViewCreated view =
            if settings.CodeLens.Enabled then
                let provider = 
                    lineLensProviders 
                    |> Seq.tryFind (fun (v, _) -> v = view)

                if provider.IsNone then
                    addLineLensProvider view (view.TextBuffer) |> ignore