// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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

    /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView"]/*' />
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

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.Site;"]/*' />
        protected IServiceProvider Site {
            get { return this.site; }
            set { this.site = value; }
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.Buffer;"]/*' />
        protected IVsTextLines Buffer {
            get { return this.buffer; }
            set { this.buffer = value; }
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.ComponentManager;"]/*' />
        protected IOleComponentManager ComponentManager {
            get { return this.componentManager; }
            set { this.componentManager = value; }
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.ComponentId;"]/*' />
        protected uint ComponentId {
            get { return this.componentID; }
            set { this.componentID = value; }
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.SimpleEditorView"]/*' />
        protected SimpleEditorView(IVsTextLines buffer) {
            this.buffer = buffer;
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.QueryCommandStatus"]/*' />
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
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.ExecCommand"]/*' />
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
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.QueryParameterList"]/*' />
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
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.IOleCommandTarget.QueryStatus"]/*' />
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
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.IOleCommandTarget.Exec"]/*' />
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


        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.ClosePane"]/*' />
        public virtual int ClosePane() {
            return this.componentManager.FRevokeComponent(this.componentID);
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.CreatePaneWindow"]/*' />
        public abstract int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd);
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.GetDefaultSize"]/*' />
        public virtual int GetDefaultSize(SIZE[] size) {
            size[0].cx = 100;
            size[0].cy = 100;
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.LoadViewState"]/*' />
        public virtual int LoadViewState(Microsoft.VisualStudio.OLE.Interop.IStream stream) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.SaveViewState"]/*' />
        public virtual int SaveViewState(Microsoft.VisualStudio.OLE.Interop.IStream stream) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.SetSite"]/*' />
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
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.TranslateAccelerator"]/*' />
        public virtual int TranslateAccelerator(MSG[] msg) {
            return (int)NativeMethods.S_FALSE;
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.IsSupported"]/*' />
        public virtual int IsSupported(Microsoft.VisualStudio.OLE.Interop.IDataObject data) {
            return (int)NativeMethods.S_FALSE;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.ItemPicked"]/*' />
        public virtual int ItemPicked(Microsoft.VisualStudio.OLE.Interop.IDataObject data) {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.SetInfo"]/*' />
        public virtual int SetInfo() {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.CommitPendingEdit"]/*' />
        public virtual int CommitPendingEdit(out int fCommitFailed) {
            fCommitFailed = 0;
            return NativeMethods.S_OK;
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.FDoIdle"]/*' />
        public virtual int FDoIdle(uint grfidlef) {
            return 0;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.Terminate"]/*' />
        public virtual void Terminate() {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.FPreTranslateMessage"]/*' />
        public virtual int FPreTranslateMessage(MSG[] msg) {
            return 0;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.OnEnterState"]/*' />
        public virtual void OnEnterState(uint uStateID, int fEnter) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.OnAppActivate"]/*' />
        public virtual void OnAppActivate(int fActive, uint dwOtherThreadID) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.OnLoseActivation"]/*' />
        public virtual void OnLoseActivation() {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.OnActivationChange"]/*' />
        public virtual void OnActivationChange(Microsoft.VisualStudio.OLE.Interop.IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.FContinueMessageLoop"]/*' />
        public virtual int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked) {
            return 1;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.FQueryTerminate"]/*' />
        public virtual int FQueryTerminate(int fPromptUser) {
            return 1;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.HwndGetWindow"]/*' />
        public virtual IntPtr HwndGetWindow(uint dwWhich, uint dwReserved) {
            return IntPtr.Zero;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="SimpleEditorView.FReserved1"]/*' />
        public virtual int FReserved1(uint reserved, uint message, IntPtr wParam, IntPtr lParam) {
            return 1;
        }
    }

#if CUT

    /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView"]/*' />
    /// <summary>
    /// This class View provides an abstract base class for custom editor views that
    /// support Ole Inplace activation (ActiveX controls).
    /// </summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class OleEditorView : SimpleEditorView, IOleCommandTarget, IVsWindowPane, IVsToolboxUser, IVsStatusbarUser, IOleObject, IOleInPlaceActiveObject, IOleInPlaceObject, IOleInPlaceComponent
        //IServiceProvider,
        //IOleDocumentView,
        //IOleDocument,
        //IOleInPlaceUIWindow,
        //IVsMultiViewDocumentView,
        //IVsFindTarget,
        //IVsWindowFrameNotify,
        //IVsCodeWindow,
        //IVsWindowPaneCommit,
        //IVsBroadcastMessageEvents,
        //IVsDocOutlineProvider,
        //IVsDebuggerEvents,
        // ??? VxDTE::IExtensibleObject,
        //IVsBackForwardNavigation
        // ??? public IVsTextLinesEvents,
        // ??? public ISelectionContainer,
        // ??? public IVsTextBufferDataEvents,
        {
        internal EventSinkCollection eventSinks = new EventSinkCollection();
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.pCompUIMgr;"]/*' />
        protected IOleComponentUIManager pCompUIMgr;
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.pIPCompSite;"]/*' />
        protected IOleInPlaceComponentSite pIPCompSite;
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.pClientSite;"]/*' />
        protected IOleClientSite pClientSite;
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.monikers"]/*' />
        protected Hashtable monikers = new Hashtable();
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.OleEditorView"]/*' />
        protected OleEditorView(IVsTextLines buffer) : base(buffer) {
        }
    
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.Advise"]/*' />
        public virtual void Advise(IAdviseSink sink, out uint cookie) {
            cookie = eventSinks.Add(sink);
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.Close"]/*' />
        public virtual void Close(uint dwSaveOption) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.DoVerb"]/*' />
        public virtual int DoVerb(int iVerb, MSG[] msg, IOleClientSite site, int index, IntPtr hwndParent, RECT[] posRect) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.EnumAdvise"]/*' />
        public virtual void EnumAdvise(out IEnumSTATDATA ppEnumAdvise) {
            ppEnumAdvise = null;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.EnumVerbs"]/*' />
        public virtual int EnumVerbs(out IEnumOLEVERB ppEnumVerbs) {
            ppEnumVerbs = null;
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetClientSite"]/*' />
        public virtual void GetClientSite(out IOleClientSite site) {
            site = this.pClientSite;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetClipboardData"]/*' />
        public virtual void GetClipboardData(uint reserved, out Microsoft.VisualStudio.OLE.Interop.IDataObject obj) {
            obj = null;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetExtent"]/*' />
        public virtual void GetExtent(uint dwDrawAspect, SIZEL[] size) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetMiscStatus"]/*' />
        public virtual int GetMiscStatus(uint dwAspect, out uint status) {
            status = 0;
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetMoniker"]/*' />
        public virtual void GetMoniker(uint iAssign, uint whichMoniker, out IMoniker moniker) {
            object key = (object)whichMoniker;

            moniker = (IMoniker)monikers[key];
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetUserClassID"]/*' />
        public virtual void GetUserClassID(out Guid pClsid) {
            pClsid = this.GetType().GUID;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetUserType"]/*' />
        public virtual int GetUserType(uint formOfType, IntPtr userType) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.InitFromData"]/*' />
        public virtual int InitFromData(Microsoft.VisualStudio.OLE.Interop.IDataObject data, int fCreation, uint reserved) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.IsUpToDate"]/*' />
        public virtual int IsUpToDate() {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.SetClientSite"]/*' />
        public virtual void SetClientSite(IOleClientSite site) {
            this.pClientSite = site;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.SetColorScheme"]/*' />
        public virtual void SetColorScheme(LOGPALETTE[] logicalPalette) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.SetExtent"]/*' />
        public virtual void SetExtent(uint drawAspect, SIZEL[] size) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.SetHostNames"]/*' />
        public virtual void SetHostNames(string containerApp, string containerObj) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.SetMoniker"]/*' />
        public virtual void SetMoniker(uint whichMoniker, IMoniker moniker) {
            object key = (object)whichMoniker;

            if (monikers.Contains(key)) monikers.Remove(key);

            monikers.Add(key, moniker);
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.Unadvise"]/*' />
        public virtual void Unadvise(uint dwCookie) {
            eventSinks.RemoveAt(dwCookie);
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.Update"]/*' />
        public virtual int Update() {
            return NativeMethods.S_OK;
        }
    

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.EnableModeless"]/*' />
        public virtual void EnableModeless(int fEnable) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.OnDocWindowActivate"]/*' />
        public virtual void OnDocWindowActivate(int fActivate) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.OnFrameWindowActivate"]/*' />
        public virtual void OnFrameWindowActivate(int fActivate) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.ResizeBorder"]/*' />
        public virtual void ResizeBorder(RECT[] border, ref Guid iid, IOleInPlaceUIWindow window, int fFrameWindow) {
        }



        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.ContextSensitiveHelp"]/*' />
        public virtual void ContextSensitiveHelp(int fEnterHelp) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetWindow"]/*' />
        public virtual void GetWindow(out IntPtr hwnd) {
            hwnd = IntPtr.Zero;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.InPlaceDeactivate"]/*' />
        public virtual void InPlaceDeactivate() {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.ReactivateAndUndo"]/*' />
        public virtual void ReactivateAndUndo() {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.SetObjectRects"]/*' />
        public virtual void SetObjectRects(RECT[] posRect, RECT[] clipRect) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.UIDeactivate"]/*' />
        public virtual void UIDeactivate() {
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.FQueryClose"]/*' />
        public virtual int FQueryClose(int fPromptUser) {
            return 0;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetCntrContextMenu"]/*' />
        public virtual int GetCntrContextMenu(uint dwRoleActiveObject, ref Guid clsidActiveObject, int nMenuIdActiveObject, POINTS[] pos, out Guid clsidCntr, OLEMENUID[] menuid, out uint pgrf) {
            clsidCntr = Guid.Empty;
            pgrf = 0;
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetCntrHelp"]/*' />
        public virtual int GetCntrHelp(ref uint pdwRole, ref Guid pclsid, POINT posMouse, uint dwHelpCmd, string pszHelpFileIn, out string pwsHelpFileOut, uint dwDataIn, out uint dwDataOut) {
            pwsHelpFileOut = pszHelpFileIn;
            dwDataOut = dwDataIn;
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.GetCntrMessage"]/*' />
        public virtual int GetCntrMessage(ref uint pdwRolw, ref Guid clsid, string titleIn, string textIn, string helpFileIn, out string titleOut, out string textOut, out string helpFileOut, ref uint dwHelpContextId, OLEMSGBUTTON[] msgbutton, OLEMSGDEFBUTTON[] msgdefbutton, OLEMSGICON[] msgicon, ref int sysAlert) {
            titleOut = titleIn;
            textOut = textIn;
            helpFileOut = helpFileIn;
            return NativeMethods.S_OK;
        }
        void IOleInPlaceComponent.OnEnterState(uint dwStateId, int fEnter) {
            ((IOleComponent)this).OnEnterState(dwStateId, fEnter);
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.OnWindowActivate"]/*' />
        public virtual int OnWindowActivate(uint windowType, int fActivate) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.TranslateCntrAccelerator"]/*' />
        public virtual int TranslateCntrAccelerator(MSG[] msg) {
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="OleEditorView.UseComponentUIManager"]/*' />
        public virtual int UseComponentUIManager(uint dwCompRole, out uint pgrfCompFlags, IOleComponentUIManager pCompUIMgr, IOleInPlaceComponentSite pIPCompSite) {
            pgrfCompFlags = 0;
            this.pCompUIMgr = pCompUIMgr;
            this.pIPCompSite = pIPCompSite;
            return NativeMethods.S_OK;
        }
    }
#endif
    /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl"]/*' />
    /// <summary>
    /// This class wraps a managed WinForm control and uses that as the editor window.
    /// </summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class EditorControl : SimpleEditorView {
        Control control;
        
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.EditorControl"]/*' />
        internal EditorControl(IServiceProvider site, IVsTextLines buffer, Control ctrl) : base(buffer) {
            this.control = ctrl;
            this.Site = site;
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.Control;"]/*' />
        protected Control Control {
            get { return this.control; }
            set { this.control = value; }
        }

        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.ClosePane"]/*' />
        public override int ClosePane() {
            if (control != null) {
                control.Dispose();
                control = null;
            }

            return base.ClosePane();
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.CreatePaneWindow"]/*' />
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
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.CommitPendingEdit"]/*' />
        public override int CommitPendingEdit(out int fCommitFailed) {
            fCommitFailed = 0;
            return NativeMethods.S_OK;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.FDoIdle"]/*' />
        public override int FDoIdle(uint grfidlef) {
            return 0;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.OnAppActivate"]/*' />
        public override void OnAppActivate(int fActive, uint dwOtherThreadID) {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.FQueryTerminate"]/*' />
        public override int FQueryTerminate(int fPromptUser) {
            return 1;
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.OnLoseActivation"]/*' />
        public override void OnLoseActivation() {
        }
        /// <include file='doc\EditorView.uex' path='docs/doc[@for="EditorControl.HwndGetWindow"]/*' />
        public override IntPtr HwndGetWindow(uint dwWhich, uint dwReserved) {
            return control.Handle;
        }
        
    }


}
