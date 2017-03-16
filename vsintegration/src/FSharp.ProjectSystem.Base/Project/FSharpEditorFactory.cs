using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [Guid(Constants.FSharpEditorFactoryIdString)]
    [ProvideEditorFactory(typeof(FSharpEditorFactory), 101, CommonPhysicalViewAttributes = (int)__VSPHYSICALVIEWATTRIBUTES.PVA_SupportsPreview)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fs", 32)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fsi", 32)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fsscript", 32)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fsx", 32)]
    public class FSharpEditorFactory : IVsEditorFactory
    {
        private Package _parentPackage;
        private IOleServiceProvider _oleServiceProvider;
        private IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        private IContentTypeRegistryService _contentTypeRegistryService;
        private IComponentModel _componentModel;

        private IServiceProvider ServiceProvider
        {
            get
            {
                return _parentPackage;
            }
        }

        public FSharpEditorFactory(Package parentPackage)
        {
            if (parentPackage == null)
            {
                throw new ArgumentNullException(nameof(parentPackage));
            }

            _parentPackage = parentPackage;
            _componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
            _editorAdaptersFactoryService = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
            _contentTypeRegistryService = _componentModel.GetService<IContentTypeRegistryService>();
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pbstrEditorCaption = String.Empty;

            //pguidCmdUI is the highest priority Guid that Visual Studio Shell looks at when translating key strokes into editor commands.
            //Here we intentionally set it to Guid.Empty so it will not play a part in translating keystrokes at all. The next highest priority 
            //will be commands tied to this FSharpEditorFactory (such as Alt-Enter).
            //However, because we are setting pguidCmdUI, we are not going to get typical text editor commands bound to this editor unless we inherit 
            //those keybindings on the IVsWindowFrame in which our editor lives.
            pguidCmdUI = Guid.Empty;
            pgrfCDW = 0;

            IVsTextBuffer textBuffer = null;

            // Is this document already open? If so, let's see if it's a IVsTextBuffer we should re-use. This allows us
            // to properly handle multiple windows open for the same document.
            if (punkDocDataExisting != IntPtr.Zero)
            {
                object docDataExisting = Marshal.GetObjectForIUnknown(punkDocDataExisting);

                textBuffer = docDataExisting as IVsTextBuffer;

                if (textBuffer == null)
                {
                    // We are incompatible with the existing doc data
                    return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
                }
            }

            // Do we need to create a text buffer?
            if (textBuffer == null)
            {
                var contentType = _contentTypeRegistryService.GetContentType(Constants.FSharpContentType);
                textBuffer = _editorAdaptersFactoryService.CreateVsTextBufferAdapter(_oleServiceProvider, contentType);
            }

            // If the text buffer is marked as read-only, ensure that the padlock icon is displayed
            // next the new window's title and that [Read Only] is appended to title.
            READONLYSTATUS readOnlyStatus = READONLYSTATUS.ROSTATUS_NotReadOnly;
            uint textBufferFlags;
            if (ErrorHandler.Succeeded(textBuffer.GetStateFlags(out textBufferFlags)) &&
                0 != (textBufferFlags & ((uint)BUFFERSTATEFLAGS.BSF_FILESYS_READONLY | (uint)BUFFERSTATEFLAGS.BSF_USER_READONLY)))
            {
                readOnlyStatus = READONLYSTATUS.ROSTATUS_ReadOnly;
            }

            var codeWindow = _editorAdaptersFactoryService.CreateVsCodeWindowAdapter(_oleServiceProvider);
            codeWindow.SetBuffer((IVsTextLines)textBuffer);
            codeWindow.GetEditorCaption(readOnlyStatus, out pbstrEditorCaption);

            ppunkDocView = Marshal.GetIUnknownForObject(codeWindow);
            ppunkDocData = Marshal.GetIUnknownForObject(textBuffer);

            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null; 

            if(rguidLogicalView == VSConstants.LOGVIEWID.Primary_guid ||
                rguidLogicalView == VSConstants.LOGVIEWID.Debugging_guid ||
                rguidLogicalView == VSConstants.LOGVIEWID.Code_guid ||
                rguidLogicalView == VSConstants.LOGVIEWID.TextView_guid)
            {
                return VSConstants.S_OK;
            }

            return VSConstants.E_NOTIMPL;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider packageServiceProvider)
        {
            _oleServiceProvider = packageServiceProvider;
            return VSConstants.S_OK;
        }
    }
}
