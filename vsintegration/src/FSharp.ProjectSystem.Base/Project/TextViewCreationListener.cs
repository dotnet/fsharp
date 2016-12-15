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
    [ContentType("F#")]
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
