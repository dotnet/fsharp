// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System.Diagnostics;
using System.ComponentModel.Design;

namespace Microsoft.VisualStudio.FSharp.LanguageService {

    /// <summary>
    /// This class View provides an abstract base class for simple editor views
    /// that follow the VS simple embedding model.
    /// </summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class SimpleEditorView : IOleCommandTarget, IVsWindowPane, IVsToolboxUser, IVsStatusbarUser, IVsWindowPaneCommit, IOleComponent // for idle processing.
        //IServiceProvider,
        //IVsMultiViewDocumentView,
        //IVsFindTarget,
        //IVsWindowFrameNotify,
        //IVsCodeWindow,
        //IVsBroadcastMessageEvents,
        //IVsDocOutlineProvider,
        //IVsDebuggerEvents,
        // ??? VxDTE::IExtensibleObject,
        //IVsBackForwardNavigation
        // ??? public ISelectionContainer,
    {

        IServiceProvider site;
        IVsTextLines buffer;
        IOleComponentManager componentManager;
        uint componentID;

        internal SimpleEditorView() {}

        protected IServiceProvider Site {
            get { return this.site; }
            set { this.site = value; }
        }
        protected IVsTextLines Buffer {
            get { return this.buffer; }
            set { this.buffer = value; }
        }
        protected IOleComponentManager ComponentManager {
            get { return this.componentManager; }
            set { this.componentManager = value; }
        }
        protected uint ComponentId {
            get { return this.componentID; }
            set { this.componentID = value; }
        }

        protected SimpleEditorView(IVsTextLines buffer) {
            this.buffer = buffer;
        }

        /// <summary>
        /// Override this method to provide custom command status, 
        /// e.g. (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED
        /// </summary>
        protected virtual int QueryCommandStatus(ref Guid guidCmdGroup, uint cmdId) {
            IServiceProvider sp = this.Site;
            if (sp != null) {
                // Delegate to menu command service just in case the child control registered some MenuCommands with it.
                IMenuCommandService svc = sp.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
                if (svc != null) {
                    MenuCommand cmd = svc.FindCommand(new CommandID(guidCmdGroup, (int)cmdId));
                    if (cmd != null) {
                        return cmd.OleStatus;
                    }
                }
            }
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }
        /// <summary>
        /// Override this method to intercept the IOleCommandTarget::Exec call.
        /// </summary>
        /// <returns>Usually returns 0 if ok, or OLECMDERR_E_NOTSUPPORTED</returns>
        protected virtual int ExecCommand(ref Guid guidCmdGroup, uint cmdId, uint cmdExecOptions, IntPtr pvaIn, IntPtr pvaOut) {
            IServiceProvider sp = this.Site;
            if (sp != null) {
                // Delegate to menu command service just in case the child control registered some MenuCommands with it.
                IMenuCommandService svc = sp.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
                if (svc != null) {
                    MenuCommand cmd = svc.FindCommand(new CommandID(guidCmdGroup, (int)cmdId));
                    if (cmd != null) {
                        cmd.Invoke();
                    }
                }
            }
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }
        /// <summary>
        /// This method is called when IOleCommandTarget.Exec is called with 
        /// nCmdexecopt equal to MAKELONG(OLECMDEXECOPT_SHOWHELP, VSCmdOptQueryParameterList).
        /// </summary>
        /// <returns>Usually returns 0 if ok, or OLECMDERR_E_NOTSUPPORTED</returns>
        protected virtual int QueryParameterList(ref Guid guidCmdGroup, uint id, uint options, IntPtr pvaIn, IntPtr pvaOut) {
#if LANGTRACE
            Trace.WriteLine(String.Format("QueryParameterList({0},{1})", guidCmdGroup.ToString(), id));
#endif
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }
        /// <internalonly/>
        /// <summary>
        /// IOleCommandTarget implementation
        /// </summary>
        public virtual int QueryStatus(ref Guid guidCmdGroup, uint cmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            for (uint i = 0; i < cmds; i++) {
                int rc = QueryCommandStatus(ref guidCmdGroup, (uint)prgCmds[i].cmdID);

                if (rc < 0) return rc;
            }

            return 0;
        }
        /// <internalonly/>
        public virtual int Exec(ref Guid guidCmdGroup, uint id, uint options, IntPtr pvaIn, IntPtr pvaOut) {
            ushort lo = (ushort)(options & (uint)0xffff);
            ushort hi = (ushort)(options >> 16);
            switch (lo) {
                case (ushort)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP:
                    if ((options >> 16) == VsMenus.VSCmdOptQueryParameterList) {
                        return QueryParameterList(ref guidCmdGroup, id, options, pvaIn, pvaOut);
                    }
                    break;
                case (ushort)OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER: // todo
                case (ushort)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER: // todo
                    return NativeMethods.E_NOTIMPL;
                    
                case (ushort)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT:
                default:
                    return ExecCommand(ref guidCmdGroup, id, options, pvaIn, pvaOut);
            }
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }


        public virtual int ClosePane() {
            return this.componentManager.FRevokeComponent(this.componentID);
        }
        public abstract int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd);
        public virtual int GetDefaultSize(SIZE[] size) {
            size[0].cx = 100;
            size[0].cy = 100;
            return NativeMethods.S_OK;
        }
        public virtual int LoadViewState(Microsoft.VisualStudio.OLE.Interop.IStream stream) {
            return NativeMethods.S_OK;
        }
        public virtual int SaveViewState(Microsoft.VisualStudio.OLE.Interop.IStream stream) {
            return NativeMethods.S_OK;
        }
        public virtual int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider site) {
            this.site = new ServiceProvider(site);

            if (this.buffer != null) {
                // register our independent view with the IVsTextManager so that it knows
                // the user is working with a view over the text buffer. this will trigger
                // the text buffer to prompt the user whether to reload the file if it is
                // edited outside of the development Environment.
                IVsTextManager textManager = (IVsTextManager)this.site.GetService(typeof(SVsTextManager));
                // NOTE: NativeMethods.ThrowOnFailure is removed from this method because you are not allowed
                // to fail a SetSite call, see debug assert at f:\dd\env\msenv\core\docwp.cpp line 87.
                int hr = 0;
                if (textManager != null) {
                    IVsWindowPane windowPane = (IVsWindowPane)this;
                    hr = textManager.RegisterIndependentView(this, this.buffer);
                    if (!NativeMethods.Succeeded(hr))
                        Debug.Assert(false, "RegisterIndependentView failed");
                }
            }

            //register with ComponentManager for Idle processing
            this.componentManager = (IOleComponentManager)this.site.GetService(typeof(SOleComponentManager));
            if (componentID == 0) {
                OLECRINFO[] crinfo = new OLECRINFO[1];

                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime | (uint)_OLECRF.olecrfNeedPeriodicIdleTime | (uint)_OLECRF.olecrfNeedAllActiveNotifs | (uint)_OLECRF.olecrfNeedSpecActiveNotifs;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal | (uint)_OLECADVF.olecadvfRedrawOff | (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                int hr = this.componentManager.FRegisterComponent(this, crinfo, out this.componentID);
                if (!NativeMethods.Succeeded(hr))
                    Debug.Assert(false, "FRegisterComponent failed");
            }
            return NativeMethods.S_OK;
        }
        public virtual int TranslateAccelerator(MSG[] msg) {
            return (int)NativeMethods.S_FALSE;
        }

        public virtual int IsSupported(Microsoft.VisualStudio.OLE.Interop.IDataObject data) {
            return (int)NativeMethods.S_FALSE;
        }
        public virtual int ItemPicked(Microsoft.VisualStudio.OLE.Interop.IDataObject data) {
            return NativeMethods.S_OK;
        }

        public virtual int SetInfo() {
            return NativeMethods.S_OK;
        }

        public virtual int CommitPendingEdit(out int fCommitFailed) {
            fCommitFailed = 0;
            return NativeMethods.S_OK;
        }

        public virtual int FDoIdle(uint grfidlef) {
            return 0;
        }
        public virtual void Terminate() {
        }
        public virtual int FPreTranslateMessage(MSG[] msg) {
            return 0;
        }
        public virtual void OnEnterState(uint uStateID, int fEnter) {
        }
        public virtual void OnAppActivate(int fActive, uint dwOtherThreadID) {
        }
        public virtual void OnLoseActivation() {
        }
        public virtual void OnActivationChange(Microsoft.VisualStudio.OLE.Interop.IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved) {
        }
        public virtual int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked) {
            return 1;
        }
        public virtual int FQueryTerminate(int fPromptUser) {
            return 1;
        }
        public virtual IntPtr HwndGetWindow(uint dwWhich, uint dwReserved) {
            return IntPtr.Zero;
        }
        public virtual int FReserved1(uint reserved, uint message, IntPtr wParam, IntPtr lParam) {
            return 1;
        }
    }

    /// <summary>
    /// This class wraps a managed WinForm control and uses that as the editor window.
    /// </summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class EditorControl : SimpleEditorView {
        Control control;
        
        internal EditorControl(IServiceProvider site, IVsTextLines buffer, Control ctrl) : base(buffer) {
            this.control = ctrl;
            this.Site = site;
        }

        protected Control Control {
            get { return this.control; }
            set { this.control = value; }
        }

        public override int ClosePane() {
            if (control != null) {
                control.Dispose();
                control = null;
            }

            return base.ClosePane();
        }
        public override int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd) {
            control.SuspendLayout();
            control.Left = x;
            control.Top = y;
            control.Width = cx;
            control.Height = cy;
            control.ResumeLayout();
            control.CreateControl();

            //HACK: For some VS throws debug asserts if WS_MAXIMIZEBOX is set
            //so we'll just turn off this window style here.
            int windowStyle = (int)UnsafeNativeMethods.GetWindowLong(this.control.Handle, NativeMethods.GWL_STYLE);
            windowStyle = windowStyle & ~(0x00010000); //WS_MAXIMIZEBOX;
            NativeMethods.SetWindowLong(this.Control.Handle, NativeMethods.GWL_STYLE, windowStyle);
            //End of workaround

            NativeMethods.SetParent(control.Handle, hwndParent);
            hwnd = control.Handle;
            return NativeMethods.S_OK;
        }
        public override int CommitPendingEdit(out int fCommitFailed) {
            fCommitFailed = 0;
            return NativeMethods.S_OK;
        }
        public override int FDoIdle(uint grfidlef) {
            return 0;
        }
        public override void OnAppActivate(int fActive, uint dwOtherThreadID) {
        }
        public override int FQueryTerminate(int fPromptUser) {
            return 1;
        }
        public override void OnLoseActivation() {
        }
        public override IntPtr HwndGetWindow(uint dwWhich, uint dwReserved) {
            return control.Handle;
        }
        
    }


}
