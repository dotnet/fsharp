// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Interactive
open System.ComponentModel.Design
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Text


[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.PrimaryDocument)>]
type internal DumpCommandFilterProvider 
    [<ImportingConstructor>] 
    (checkerProvider: FSharpCheckerProvider,
    [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
     projectInfoManager: FSharpProjectOptionsManager,
     workspace: VisualStudioWorkspaceImpl,
     textDocumentFactoryService: ITextDocumentFactoryService,
     editorFactory: IVsEditorAdaptersFactoryService) =

    let projectSystemPackage =
      lazy(
        let shell = serviceProvider.GetService(typeof<SVsShell>) :?> IVsShell
        let packageToBeLoadedGuid = ref (Guid "{91a04a73-4f2c-4e7c-ad38-c1a68e7da05c}")  // FSharp ProjectSystem guid        
        match shell.LoadPackage packageToBeLoadedGuid with
        | VSConstants.S_OK, pkg ->
            pkg :?> Package
        | _ -> null)

    let FSharpDump (this:Package) (sender:obj) (e:EventArgs) =
        System.Diagnostics.Debbuger.Launch() |> ignore
        checkerProvider.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        ()

    interface IWpfTextViewCreationListener with
        member __.TextViewCreated(textView) = 
            let commandService = serviceProvider.GetService(typeof<IMenuCommandService>) :?> OleMenuCommandService // FSI-LINKAGE-POINT
            let id  = new CommandID(Guids.guidFsiPackageCmdSet,int32 Guids.cmdIDFSharpDump)
            let cmd = new MenuCommand(new EventHandler(FSharpDump projectSystemPackage.Value), id)
            commandService.AddCommand(cmd)