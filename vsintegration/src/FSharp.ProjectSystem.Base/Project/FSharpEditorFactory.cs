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
    //TODO: Where should this be put? Constants file?
    [Guid(Constants.FSharpEditorFactoryIdString)]
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

        private string ContentTypeName => "FSharp";

        public FSharpEditorFactory(Package parentPackage)
        {
            _parentPackage = _parentPackage ?? throw new ArgumentNullException(nameof(_parentPackage));
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
            pguidCmdUI = Guid.Empty;
            pgrfCDW = 0;    //TODO: What is this?

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
                var contentType = _contentTypeRegistryService.GetContentType(ContentTypeName);
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
            pguidCmdUI = Constants.FSharpEditorFactoryGuid;

            ppunkDocData = Marshal.GetIUnknownForObject(textBuffer);

            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null; //TODO: What's a physical view vs logical view?

            if(rguidLogicalView.Equals(VSConstants.LOGVIEWID.Designer_guid) || rguidLogicalView.Equals(VSConstants.LOGVIEWID.Primary_guid))
            {
                return VSConstants.S_OK;
            }

            //TODO: What scenarios trigger this?
            return VSConstants.E_NOTIMPL;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider packageServiceProvider)
        {
            _oleServiceProvider = packageServiceProvider;
            return VSConstants.S_OK;
        }
    }
}
