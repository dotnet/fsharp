using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(Constants.FSharpContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class TextViewCreationListener : IVsTextViewCreationListener
    {
        internal readonly IVsEditorAdaptersFactoryService _adaptersFactory;

        [ImportingConstructor]
        public TextViewCreationListener(IVsEditorAdaptersFactoryService adaptersFactory)
        {
            _adaptersFactory = adaptersFactory;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = _adaptersFactory.GetWpfTextView(textViewAdapter);
            InitKeyBindings(textViewAdapter);
        }

        /// <summary>
        /// The <see cref="FSharpEditorFactory"/> initializes the pguidCmdUI to an empty Guid. This means that our buffer does not receive the normal text editor command bindings.
        /// In order to handle this, we tell the IVsWindowFrame in which our editor lives to inherit the keybindinds from the text editor factory.
        /// This allows us to specify the TextEditor keybindings at a lower priority than our F# Editor Factory keybindings and allows us to handle Alt+Enter
        /// </summary>
        public void InitKeyBindings(IVsTextView vsTextView)
        {
            var os = vsTextView as IObjectWithSite;
            if (os == null)
            {
                return;
            }

            IntPtr unkSite = IntPtr.Zero;
            IntPtr unkFrame = IntPtr.Zero;

            try
            {
                os.GetSite(typeof(VisualStudio.OLE.Interop.IServiceProvider).GUID, out unkSite);
                var sp = Marshal.GetObjectForIUnknown(unkSite) as VisualStudio.OLE.Interop.IServiceProvider;

                sp.QueryService(typeof(SVsWindowFrame).GUID, typeof(IVsWindowFrame).GUID, out unkFrame);

                //When calling Peek Definition, the editor creates an IVsTextView within another view.
                //Therefore this new view won't exist as the direct child of an IVsWindowFrame and we will return.
                //We don't need to worry about inheriting key bindings in this situation, because the
                //parent IVsTextView will have already set this value during its creation.
                if(unkFrame == IntPtr.Zero)
                {
                    return;
                }

                var frame = Marshal.GetObjectForIUnknown(unkFrame) as IVsWindowFrame;
                frame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, VSConstants.GUID_TextEditorFactory);
            }
            finally
            {
                if (unkSite != IntPtr.Zero)
                {
                    Marshal.Release(unkSite);
                }
                if (unkFrame != IntPtr.Zero)
                {
                    Marshal.Release(unkFrame);
                }
            }
        }
    }
}
