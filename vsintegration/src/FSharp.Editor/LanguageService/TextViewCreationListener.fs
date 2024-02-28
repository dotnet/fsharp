namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.InteropServices
open System.ComponentModel.Composition
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.OLE.Interop

[<Export(typeof<IVsTextViewCreationListener>)>]
[<ContentType(Constants.FSharpContentType)>]
[<TextViewRole(PredefinedTextViewRoles.Editable)>]
type TextViewCreationListener [<ImportingConstructor>] (adaptersFactory: IVsEditorAdaptersFactoryService) =

    /// <summary>
    /// The <see cref="FSharpEditorFactory"/> initializes the pguidCmdUI to an empty Guid. This means that our buffer does not receive the normal text editor command bindings.
    /// In order to handle this, we tell the IVsWindowFrame in which our editor lives to inherit the keybindings from the text editor factory.
    /// This allows us to specify the TextEditor keybindings at a lower priority than our F# Editor Factory keybindings and allows us to handle Alt+Enter
    /// </summary>
    let initKeyBindings (vsTextView: IVsTextView) =
        match vsTextView with
        | :? IObjectWithSite as os ->
            let mutable unkSite = IntPtr.Zero
            let mutable unkFrame = IntPtr.Zero

            try
                os.GetSite(ref typeof<IServiceProvider>.GUID, &unkSite)
                let sp = Marshal.GetObjectForIUnknown(unkSite) :?> IServiceProvider

                sp.QueryService(ref typeof<SVsWindowFrame>.GUID, ref typeof<IVsWindowFrame>.GUID, &unkFrame)
                |> ignore

                //When calling Peek Definition, the editor creates an IVsTextView within another view.
                //Therefore this new view won't exist as the direct child of an IVsWindowFrame and we will return.
                //We don't need to worry about inheriting key bindings in this situation, because the
                //parent IVsTextView will have already set this value during its creation.
                if unkFrame <> IntPtr.Zero then
                    let frame = Marshal.GetObjectForIUnknown(unkFrame) :?> IVsWindowFrame

                    frame.SetGuidProperty(
                        LanguagePrimitives.EnumToValue __VSFPROPID.VSFPROPID_InheritKeyBindings,
                        ref VSConstants.GUID_TextEditorFactory
                    )
                    |> ignore

            finally
                if unkSite <> IntPtr.Zero then
                    Marshal.Release(unkSite) |> ignore

                if unkFrame <> IntPtr.Zero then
                    Marshal.Release(unkFrame) |> ignore

        | _ -> ()

    interface IVsTextViewCreationListener with

        member _.VsTextViewCreated(textViewAdapter) =
            let _textView = adaptersFactory.GetWpfTextView(textViewAdapter)
            initKeyBindings textViewAdapter
