// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Collections.Immutable
open System.Diagnostics
open System.IO
open System.Linq
open System.Text
open System.Runtime.InteropServices
open System.Reflection.PortableExecutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.FindSymbols
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.VisualStudio.ComponentModelHost

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Threading
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop

module internal MetadataAsSource =

    let generateTemporaryDocument (asmIdentity: AssemblyIdentity, name: string, metadataReferences) =
        let rootPath = Path.Combine(Path.GetTempPath(), "MetadataAsSource")
        let extension = ".fsi"
        let directoryName = Guid.NewGuid().ToString("N")
        let temporaryFilePath = Path.Combine(rootPath, directoryName, name + extension)

        let projectId = ProjectId.CreateNewId()

        let generatedDocumentId = DocumentId.CreateNewId(projectId)
        let documentInfo = 
            DocumentInfo.Create(
                generatedDocumentId,
                Path.GetFileName(temporaryFilePath),
                filePath = temporaryFilePath,
                loader = FileTextLoader(temporaryFilePath, Encoding.UTF8))

        let projectInfo = 
            ProjectInfo.Create(
                projectId,
                VersionStamp.Default,
                name = asmIdentity.Name,
                assemblyName = asmIdentity.Name,
                language = LanguageNames.FSharp,
                documents = [|documentInfo|],
                metadataReferences = metadataReferences)

        (projectInfo, documentInfo)

    let showDocument (filePath, name, serviceProvider: IServiceProvider) =
        let vsRunningDocumentTable4 = serviceProvider.GetService<SVsRunningDocumentTable, IVsRunningDocumentTable4>()
        let fileAlreadyOpen = vsRunningDocumentTable4.IsMonikerValid(filePath)

        let openDocumentService = serviceProvider.GetService<SVsUIShellOpenDocument, IVsUIShellOpenDocument>()

        let (_, _, _, _, windowFrame) = openDocumentService.OpenDocumentViaProject(filePath, ref VSConstants.LOGVIEWID.TextView_guid)

        let componentModel = serviceProvider.GetService<SComponentModel, IComponentModel>()
        let editorAdaptersFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();
        let documentCookie = vsRunningDocumentTable4.GetDocumentCookie(filePath)
        let vsTextBuffer = vsRunningDocumentTable4.GetDocumentData(documentCookie) :?> IVsTextBuffer
        let textBuffer = editorAdaptersFactory.GetDataBuffer(vsTextBuffer)

        if not fileAlreadyOpen then
            ErrorHandler.ThrowOnFailure(windowFrame.SetProperty(int __VSFPROPID5.VSFPROPID_IsProvisional, true)) |> ignore
            ErrorHandler.ThrowOnFailure(windowFrame.SetProperty(int __VSFPROPID5.VSFPROPID_OverrideCaption, name)) |> ignore
            ErrorHandler.ThrowOnFailure(windowFrame.SetProperty(int __VSFPROPID5.VSFPROPID_OverrideToolTip, name)) |> ignore

        windowFrame.Show() |> ignore

        let textContainer = textBuffer.AsTextContainer()
        let mutable workspace = Unchecked.defaultof<_>
        if Workspace.TryGetWorkspace(textContainer, &workspace) then
            let solution = workspace.CurrentSolution
            let documentId = workspace.GetDocumentIdInCurrentContext(textContainer)
            match box documentId with
            | null -> None
            | _ -> solution.GetDocument(documentId) |> Some
        else
            None

[<Sealed>]
type internal FSharpMetadataAsSourceService() =

    member val Files = System.Collections.Concurrent.ConcurrentDictionary(StringComparer.OrdinalIgnoreCase)

    member this.ShowDocument(projInfo: ProjectInfo, docInfo: DocumentInfo, text: Text.SourceText) =
        let _ =
            let directoryName = Path.GetDirectoryName(docInfo.FilePath)
            if Directory.Exists(directoryName) |> not then
                Directory.CreateDirectory(directoryName) |> ignore
            use fileStream = new FileStream(docInfo.FilePath, IO.FileMode.Create)
            use writer = new StreamWriter(fileStream)
            text.Write(writer)

        this.Files.[docInfo.FilePath] <- (projInfo, docInfo)

        MetadataAsSource.showDocument(docInfo.FilePath, docInfo.Name, ServiceProvider.GlobalProvider)