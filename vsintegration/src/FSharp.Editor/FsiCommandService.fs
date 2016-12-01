namespace Microsoft.VisualStudio.FSharp.Editor
open System
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.OLE.Interop
open System.ComponentModel.Composition
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Interactive
open EnvDTE

type IMenuCommand =
    inherit IOleCommandTarget
    abstract IsAdded: bool with get, set
    abstract NextTarget: IOleCommandTarget with get, set

type FsiCommandFilter(serviceProvider: System.IServiceProvider) =
    let projectSystemPackage =
      lazy(
        let shell = serviceProvider.GetService(typeof<SVsShell>) :?> IVsShell
        let packageToBeLoadedGuid = ref (Guid "{91a04a73-4f2c-4e7c-ad38-c1a68e7da05c}")  // FSharp ProjectSystem guid
        let pkg = ref null 
        shell.LoadPackage(packageToBeLoadedGuid, pkg) |> ignore
        !pkg :?> Package)
    let getProjectSystemPackage() = projectSystemPackage.Value

    interface IMenuCommand with
        member val IsAdded = false with get, set
        member val NextTarget = null with get, set
        member x.Exec (pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) =
            if pguidCmdGroup = VSConstants.VsStd11 then
                if nCmdId = uint32 VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive then
                    Hooks.OnMLSend (getProjectSystemPackage()) FsiEditorSendAction.ExecuteSelection null null
                elif nCmdId = uint32 VSConstants.VSStd11CmdID.ExecuteLineInInteractive then
                    Hooks.OnMLSend (getProjectSystemPackage()) FsiEditorSendAction.ExecuteLine null null
                VSConstants.S_OK
            elif pguidCmdGroup = Guids.guidInteractive then
                if nCmdId = uint32 Guids.cmdIDDebugSelection then
                    Hooks.OnMLSend (getProjectSystemPackage()) FsiEditorSendAction.DebugSelection null null
                VSConstants.S_OK
            else
                let x = x :> IMenuCommand
                x.NextTarget.Exec(&pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut)

        member x.QueryStatus (pguidCmdGroup, cCmds, prgCmds, pCmdText) =
            if pguidCmdGroup = VSConstants.VsStd11 then
                if prgCmds.[0].cmdID = uint32 VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive then
                    prgCmds.[0].cmdf <- uint32 (OLECMDF.OLECMDF_SUPPORTED ||| OLECMDF.OLECMDF_ENABLED)
                elif prgCmds.[0].cmdID = uint32 VSConstants.VSStd11CmdID.ExecuteLineInInteractive then
                    prgCmds.[0].cmdf <- uint32 (OLECMDF.OLECMDF_SUPPORTED ||| OLECMDF.OLECMDF_ENABLED ||| OLECMDF.OLECMDF_DEFHIDEONCTXTMENU)
                VSConstants.S_OK
            elif pguidCmdGroup = Guids.guidInteractive then
                if prgCmds.[0].cmdID = uint32 Guids.cmdIDDebugSelection then
                    let dbgState = Hooks.GetDebuggerState(getProjectSystemPackage())
                    if dbgState = FsiDebuggerState.AttachedNotToFSI then
                        prgCmds.[0].cmdf <- uint32 OLECMDF.OLECMDF_INVISIBLE
                    else
                        prgCmds.[0].cmdf <- uint32 (OLECMDF.OLECMDF_SUPPORTED ||| OLECMDF.OLECMDF_ENABLED)
                VSConstants.S_OK
            else
                let x = x :> IMenuCommand
                x.NextTarget.QueryStatus(&pguidCmdGroup, cCmds, prgCmds, pCmdText)

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType("F#")>]
[<TextViewRole(PredefinedTextViewRoles.PrimaryDocument)>]
type FsiCommandFilterProvider [<ImportingConstructor>] 
    ([<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
     editorFactory: IVsEditorAdaptersFactoryService) =
  
    let addCommandFilter (viewAdapter: IVsTextView) (commandFilter: IMenuCommand) =
        if not commandFilter.IsAdded then
            match viewAdapter.AddCommandFilter(commandFilter) with
            | VSConstants.S_OK, next ->
                commandFilter.IsAdded <- true
                match next with
                | null -> ()
                | _ -> commandFilter.NextTarget <- next
            | _ -> ()
            
    interface IWpfTextViewCreationListener with
        member __.TextViewCreated(textView) = 
            let textViewAdapter = editorFactory.GetViewAdapter(textView)
            match textViewAdapter with
            | null -> ()
            | _ ->
                addCommandFilter textViewAdapter (FsiCommandFilter(serviceProvider))
        