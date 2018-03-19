// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Diagnostics
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Interactive
open System.ComponentModel.Design
open Microsoft.Diagnostics.Runtime
open EnvDTE80


[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.PrimaryDocument)>]
type internal DumpCommandFilterProvider 
    [<ImportingConstructor>] 
    (checkerProvider: FSharpCheckerProvider,
    [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider) =

    let projectSystemPackage =
      lazy(
        let shell = serviceProvider.GetService(typeof<SVsShell>) :?> IVsShell
        let packageToBeLoadedGuid = ref (Guid "{91a04a73-4f2c-4e7c-ad38-c1a68e7da05c}")  // FSharp ProjectSystem guid        
        match shell.LoadPackage packageToBeLoadedGuid with
        | VSConstants.S_OK, pkg ->
            pkg :?> Package
        | _ -> null)

    let FSharpDump (this:Package) (sender:obj) (e:EventArgs) =
        checkerProvider.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        use target = DataTarget.CreateSnapshotAndAttach(Process.GetCurrentProcess().Id)
        let runtime = target.ClrVersions.[0].CreateRuntime();
        let outputPane = projectSystemPackage.Value.GetOutputPane(VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid, "FSharp Dump")
        runtime.Heap.EnumerateObjects() 
        |> Seq.groupBy (fun x -> x.Type.Name) 
        |> Seq.map (fun (ty, os) -> ty, os |> Seq.sumBy (fun x -> x.Size), os |> Seq.length)
        |> Seq.sortBy (fun (_, size, _) -> -int64 size)
        |> Seq.take 100
        |> Seq.iter (fun (ty, size, count) -> outputPane.OutputString (sprintf "Type: %s, Total size: %d, Instance count: %d\r\n" ty size count) |> ignore)

    interface IWpfTextViewCreationListener with
        member __.TextViewCreated(textView) = 
            let commandService = (projectSystemPackage.Value :> System.IServiceProvider).GetService(typeof<IMenuCommandService>) :?> OleMenuCommandService
            let id  = new CommandID(Guids.guidFsiPackageCmdSet,int32 Guids.cmdIDFSharpDump)
            let cmd = new MenuCommand(new EventHandler(FSharpDump projectSystemPackage.Value), id)
            commandService.AddCommand(cmd)