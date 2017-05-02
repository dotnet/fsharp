// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Security.Permissions;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsShell = Microsoft.VisualStudio.Shell.VsShellUtilities;
using Microsoft.VisualStudio.FSharp.LanguageService.Resources;


namespace Microsoft.VisualStudio.FSharp.LanguageService
{
    internal enum BackgroundRequestReason
    {
        MemberSelect, // ".", also triggered by some some other tokens like ".."
        MemberSelectAndHighlightBraces, // unused? No F# tokens have both MatchBraces and MemberSelect
        MatchBracesAndMethodTip, // close-paren
        MatchBraces, // moving cursor etc.
        FullTypeCheck,  // triggered on idle
        CompleteWord, //Ctrl-space
        DisplayMemberList, //Ctrl-J
        QuickInfo, // mouse hover
        MethodTip, // open-paren, close-paren or comma
        Goto, // F12
        /// <summary>
        /// This reason is used when we want to trigger only 'parsing' (without type-checking) to 
        /// update the untyped AST information (e.g. when a different file is opened). After updating
        /// the untyped scope (in F# LS), a 'null' can be returned as the result of 'ExecuteBackgroundRequest'.
        /// </summary>
        ParseFile
    };

    [CLSCompliant(false), ComVisible(true)]
    public abstract class LanguageService : IDisposable, 
        IVsLanguageContextProvider, IOleServiceProvider,
        IObjectWithSite, IVsDebuggerEvents,
        IVsFormatFilterProvider,
        ILanguageServiceTestHelper
    {

        private IServiceProvider site;
        private ArrayList codeWindowManagers;
        private LanguagePreferences preferences;
        internal ArrayList sources;
        private bool disposed;
        private IVsDebugger debugger;
        private uint cookie;
        private DBGMODE dbgMode;
        private int lcid;
        private bool isServingBackgroundRequest; // used to stop the OnIdle thread making new background requests when a request is already running

        protected LanguageService()
        {
            this.codeWindowManagers = new ArrayList();
            this.sources = new ArrayList();
        }

        internal abstract void Initialize();

        internal IServiceProvider Site
        {
            get { return this.site; }
        }

        internal LanguagePreferences Preferences
        {
            get
            {
                if (this.preferences == null && !disposed)
                {
                    this.preferences = this.GetLanguagePreferences();
                }
                return this.preferences;
            }
            set
            {
                this.preferences = value;
            }
        }

        /// <summary>
        /// Cleanup the sources, uiShell, shell, preferences and imageList objects
        /// and unregister this language service with VS.
        /// </summary>
        public virtual void Dispose()
        {
            OnActiveViewChanged(null);
            this.disposed = true;
            this.StopBackgroundThread();
            this.lastActiveView = null;
            if (this.sources != null)
            {
                foreach (ISource s in this.sources)
                {
                    s.Dispose();
                }
                this.sources.Clear();
                this.sources = null;
            }

            if (this.codeWindowManagers != null)
            {
                foreach (CodeWindowManager m in this.codeWindowManagers)
                {
                    m.Close();
                }
                this.codeWindowManagers.Clear();
                this.codeWindowManagers = null;
            }

            if (this.preferences != null)
            {
                this.preferences.Dispose();
                this.preferences = null;
            }
            if (this.debugger != null && this.cookie != 0)
            {
                NativeMethods.ThrowOnFailure(this.debugger.UnadviseDebuggerEvents(this.cookie));
                this.cookie = 0;
                this.debugger = null;
            }
            this.site = null;
        }

        // Methods implemented by subclass.
        /// It is expected that you will have one static language preferences object
        /// for your package.
        internal abstract LanguagePreferences GetLanguagePreferences();

        internal abstract void ExecuteBackgroundRequest(BackgroundRequest req);

        /// If this returns true we can reuse a recent IntellisenseInfo if its available
        internal abstract bool IsRecentScopeSufficientForBackgroundRequest(BackgroundRequestReason req);

        internal Guid GetLanguageServiceGuid()
        {
            return this.GetType().GUID;
        }

		// Provides context from the language service to the Visual Studio core editor.
		int IVsLanguageContextProvider.UpdateLanguageContext(uint dwHint, IVsTextLines buffer, TextSpan[] ptsSelection, object ptr)
        {
            if (ptr != null && ptr is IVsUserContext && buffer is IVsTextBuffer)
                return UpdateLanguageContext((LanguageContextHint)dwHint, buffer, ptsSelection, (IVsUserContext)ptr);
            else
                return NativeMethods.E_FAIL;
        }

        /// <summary>
        /// Call this method if you want UpdateLanguageContext to be called again.
        /// </summary>
        internal void SetUserContextDirty(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            IVsWindowFrame windowFrame = null;
            uint itemID = Microsoft.VisualStudio.VSConstants.VSITEMID_NIL;
            IVsUIHierarchy hierarchy = null;
            if (VsShell.IsDocumentOpen(this.Site, fileName, Guid.Empty, out hierarchy, out itemID, out windowFrame))
            {
                IVsUserContext context;
                if (windowFrame != null)
                {
                    object prop;
                    int hr = windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_UserContext, out prop);
                    context = (IVsUserContext)prop;
                    if (NativeMethods.Succeeded(hr) && context != null)
                    {
                        context.SetDirty(1);
                    }
                }
            }
        }

        internal virtual int UpdateLanguageContext(LanguageContextHint hint, IVsTextLines buffer, TextSpan[] ptsSelection, IVsUserContext context)
        {
            // From the docs: Any failure code: means the implementer is "passing" on this opportunity to provide context and the text editor will fall back to other mechanisms.
            if (ptsSelection == null || ptsSelection.Length != 1) return NativeMethods.E_FAIL;
            context.RemoveAttribute(null, null);
            TextSpan span = ptsSelection[0];
            IVsTextLines lastActiveBuffer;
            IVsTextView lastAciveView = this.LastActiveTextView;
            if (lastActiveView == null) return NativeMethods.E_FAIL;
            NativeMethods.ThrowOnFailure(lastActiveView.GetBuffer(out lastActiveBuffer));
            if (lastActiveBuffer != buffer) return NativeMethods.E_FAIL;
            ISource source = GetSource(buffer);
            if (source == null) return NativeMethods.E_FAIL;

            var req = source.BeginBackgroundRequest(span.iStartLine, span.iStartIndex, new TokenInfo(), BackgroundRequestReason.FullTypeCheck, lastActiveView, RequireFreshResults.Yes, new BackgroundRequestResultHandler(this.HandleUpdateLanguageContextResponse));

            if (req == null || req.Result == null) return NativeMethods.E_FAIL;

            if ((req.IsSynchronous ||
                    ((req.Result != null) && req.Result.TryWaitForBackgroundRequestCompletion(1000))))
            {
                if (req.IsAborted) return NativeMethods.E_FAIL;
                if (req.ResultIntellisenseInfo != null)
                {
                    req.ResultIntellisenseInfo.GetF1KeywordString(span, context);
                    return NativeMethods.S_OK;
                }
            }
            else // result is asynchronous and have not completed within 1000 ms
            {
                context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter, "devlang", "fsharp");
                context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1_CaseSensitive, "keyword", "fsharp.typechecking.incomplete");
                return NativeMethods.S_OK;
            }
            return NativeMethods.E_FAIL;

        }
        internal void HandleUpdateLanguageContextResponse(BackgroundRequest req)
        {
        }

        internal virtual ImageList GetImageList()
        {
            ImageList ilist = new ImageList();
            ilist.ImageSize = new Size(16, 16);
            ilist.TransparentColor = Color.FromArgb(255, 0, 255);
            Stream stream = typeof(LanguageService).Assembly.GetManifestResourceStream("Resources.completionset.bmp");
            ilist.Images.AddStrip(new Bitmap(stream));
            return ilist;
        }

        internal bool IsMacroRecordingOn()
        {
            IVsShell shell = this.GetService(typeof(SVsShell)) as IVsShell;
            if (shell != null)
            {
                object pvar;
                NativeMethods.ThrowOnFailure(shell.GetProperty((int)__VSSPROPID.VSSPROPID_RecordState, out pvar));
                shell = null;
                if (pvar != null)
                {
                    return ((VSRECORDSTATE)pvar == VSRECORDSTATE.VSRECORDSTATE_ON);
                }
            }
            return false;
        }

        internal IVsDebugger GetIVsDebugger()
        {
            if (this.debugger == null)
            {
                Guid guid = typeof(Microsoft.VisualStudio.Shell.Interop.IVsDebugger).GUID;
                this.debugger = this.GetService(typeof(IVsDebugger)) as IVsDebugger;
                if (this.debugger != null)
                {
                    NativeMethods.ThrowOnFailure(debugger.AdviseDebuggerEvents(this, out this.cookie));
                    DBGMODE[] mode = new DBGMODE[1];
                    NativeMethods.ThrowOnFailure(debugger.GetMode(mode));
                    this.dbgMode = mode[0];
                }
            }
            return debugger;
        }

        internal IVsTextMacroHelper GetIVsTextMacroHelperIfRecordingOn()
        {
            if (IsMacroRecordingOn())
            {
                IVsTextManager textmgr = (IVsTextManager)this.GetService(typeof(SVsTextManager));
                return (IVsTextMacroHelper)textmgr;
            }
            return null;
        }

        internal void OpenDocument(string path)
        {
            VsShell.OpenDocument(this.site, path);
        }

        // State used for OnIdle synchronization of dropdown menu and other visual elements dependent on the active view
        internal int lastLine = -1;
        internal int lastCol = -1;
        internal string lastFileName;
        internal IVsTextView lastActiveView;
        // STATIC ROOT INTO PROJECT BUILD
        internal IntellisenseInfo recentFullTypeCheckResults = null;
        internal string recentFullTypeCheckFile = null;

        /// <devdoc>
        /// Returns the last active IVsTextView that is managed by this language service.
        /// </devdoc>
        internal IVsTextView LastActiveTextView
        {
            get { return this.lastActiveView; }
        }

        /// Returns the last active successful fetch of an IntellisenseInfo that is managed by this language service.
        /// This is only relevant to the active text view and is cleared each time the text view is switched. If it
        /// is null we must make a background request to the language service to get the recent full typecheck results.
        /// If a file is dirty, an OnIdle call will kick in to refresh the recent results.
        internal IntellisenseInfo RecentFullTypeCheckResults
        {
            get { return this.recentFullTypeCheckResults; }
            set { this.recentFullTypeCheckResults = value; }
        }

        internal string RecentFullTypeCheckFile
        {
            get { return this.recentFullTypeCheckFile; }
            set { this.recentFullTypeCheckFile = value; }
        }


        /// <devdoc>
        /// Return whether or not the last active text view is one of ours or not.
        /// </devdoc>
        internal bool IsActive
        {
            get
            {
                if (disposed) return false;
                if (this.lastActiveView == null) return false;
                return this.GetSource(this.lastActiveView) != null;
            }
        }

        internal virtual int OnIdle(bool periodic, IOleComponentManager mgr)
        {
            if (!this.IsActive)
                return 0;

            // here's our chance to synchronize combo's and so on, 
            // first we see if the caret has moved.                
            IVsTextView view = this.lastActiveView;
            if (view == null) return 0;
            ISource s = this.GetSource(view);
            if (s == null) return 0;

            int line = -1, col = -1;
            var hr = view.GetCaretPos(out line, out col);
            if (NativeMethods.Failed(hr))
                return 0;

            if (line != this.lastLine || col != this.lastCol || this.lastFileName == null)
            {
                this.lastLine = line;
                this.lastCol = col;
                this.lastFileName = s.GetFilePath();
                CodeWindowManager cwm = this.GetCodeWindowManagerForView(view);
                if (cwm != null)
                {
                    this.OnCaretMoved(cwm, view, line, col);
                }
            }
            s.OnIdle(periodic);  // do idle processing for currently-focused file
            bool moreToDo = false;
#if CHECK_ALL_DIRTY_FILES_ON_PERIODIC_IDLE
            if (periodic && mgr.FContinueIdle() != 0)
            {
                // while there is spare idle time, pick a dirty file (if there is one) and do idle processing for it
                for (int i = 0; i < this.sources.Count; ++i)
                {
                    Source so = this.sources[i] as Source;
                    if (so != null && so.IsDirty)
                    {
                        so.OnIdle(periodic);
                        if (mgr.FContinueIdle() == 0)
                        {
                            moreToDo = true;
                            break;
                        }
                    }
                }
            }
#endif
            return moreToDo ? 1 : 0;
        }

        internal abstract TypeAndMemberDropdownBars CreateDropDownHelper(IVsTextView forView);

        internal virtual void OnActiveViewChanged(IVsTextView textView)
        {
            this.lastActiveView = textView;
            this.lastFileName = null;
            this.recentFullTypeCheckResults = null;
            this.recentFullTypeCheckFile = null;
        }
        internal virtual void OnActiveViewLostFocus(IVsTextView textView)
        {
            FSharpSourceBase s = (FSharpSourceBase)this.GetSource(textView);
            if (s != null) s.HandleLostFocus();
        }
        internal virtual void OnCaretMoved(CodeWindowManager mgr, IVsTextView textView, int line, int col)
        {
            if (mgr.DropDownHelper != null)
                mgr.DropDownHelper.SynchronizeDropdowns(textView, line, col);
        }

        internal virtual void SynchronizeDropdowns()
        {
            IVsTextView textView = this.LastActiveTextView;
            if (textView != null)
            {
                CodeWindowManager mgr = this.GetCodeWindowManagerForView(textView);
                if (mgr != null && mgr.DropDownHelper != null)
                {
                    try
                    {
                        int line = -1, col = -1;
                        if (NativeMethods.Failed(textView.GetCaretPos(out line, out col)))
                            return;
                        mgr.DropDownHelper.SynchronizeDropdowns(textView, line, col);
                    }
                    catch { }
                }
            }
        }

        protected virtual void OnChangesCommitted(uint flags, Microsoft.VisualStudio.TextManager.Interop.TextSpan[] ptsChanged)
        {
        }

        internal abstract Colorizer GetColorizer(IVsTextLines buffer);

        // We have to make sure we return the same colorizer for each text buffer,
        // so we keep a hashtable of IVsTextLines -> Source objects, the Source
        // object owns the Colorizer for that buffer.  If this method returns null
        // then it means the text buffer does not belong to this language service.
        internal ISource GetSource(IVsTextLines buffer)
        {
            if (buffer == null) return null;
            foreach (ISource src in this.sources)
            {
                if (src.GetTextLines() == buffer)
                {
                    return src;
                }
            }
            return null;
        }

        internal ISource GetSource(IVsTextView view)
        {
            if (view == null) return null;
            IVsTextLines buffer;
            NativeMethods.ThrowOnFailure(view.GetBuffer(out buffer));
            return GetSource(buffer);
        }

        internal ISource GetSource(string fname)
        {
            if (this.sources != null)
            {
                foreach (ISource s in this.sources)
                {
                    if (NativeMethods.IsSamePath(s.GetFilePath(), fname))
                        return s;
                }
            }
            return null;
        }

        internal virtual void OnCloseSource(ISource source)
        {
            // JAF: Consider using cancellation token to stop the real (non-MLS) background thread.
            // StopBackgroundThread();  
            if (this.sources != null)
            {
                if (this.sources.Contains(source))
                {
                    this.sources.Remove(source);
                }
            }
        }

        internal virtual bool IsSourceOpen(ISource src)
        {
            return (this.sources != null) && this.sources.Contains(src);
        }

        internal bool IsDebugging
        {
            get
            {
                if (this.debugger == null)
                {
                    this.debugger = GetIVsDebugger();
                }
                return this.dbgMode != DBGMODE.DBGMODE_Design;
            }
        }

#if DOCUMENT_PROPERTIES
        // Override this method to create your own custom document properties for
        // display in the Properties Window when the editor for this Source is active.
        // Default is null which means there will be no document properties.
        internal virtual DocumentProperties CreateDocumentProperties(CodeWindowManager mgr)
        {
            return null;
        }
#endif

        /// If the functionName is supported, return a new IVsExpansionFunction object.
        internal virtual ExpansionFunction CreateExpansionFunction(ExpansionProvider provider, string functionName)
        {
            return null;
        }
        internal virtual ExpansionProvider CreateExpansionProvider(ISource src)
        {
            return new ExpansionProvider(src);
        }

        internal virtual CodeWindowManager CreateCodeWindowManager(IVsCodeWindow codeWindow, ISource source)
        {
            return new CodeWindowManager(this, codeWindow, source);
        }



        internal object GetService(Type serviceType)
        {
            if (this.site != null)
            {
                return this.site.GetService(serviceType);
            }
            return null;
        }

        public virtual int QueryService(ref Guid guidService, ref Guid iid, out IntPtr obj)
        {
            obj = IntPtr.Zero;
            if (this.site != null)
            {
                IOleServiceProvider psp = this.GetService(typeof(IOleServiceProvider)) as IOleServiceProvider;
                if (psp != null)
                    NativeMethods.ThrowOnFailure(psp.QueryService(ref guidService, ref iid, out obj));
                return 0;
            }
            return (int)NativeMethods.E_UNEXPECTED;
        }

        // Override this method if you want to insert your own view filter
        // into the command chain.  
        internal virtual ViewFilter CreateViewFilter(CodeWindowManager mgr, IVsTextView newView)
        {
            return new ViewFilter(mgr, newView);
        }

        internal void AddCodeWindowManager(CodeWindowManager m)
        {
            this.codeWindowManagers.Add(m);
        }

        internal void RemoveCodeWindowManager(CodeWindowManager m)
        {
            this.codeWindowManagers.Remove(m);
        }

        internal CodeWindowManager GetCodeWindowManagerForView(IVsTextView view)
        {
            if (view == null) return null;
            foreach (CodeWindowManager m in this.codeWindowManagers)
            {
                if (m.CodeWindow != null)
                {
                    IVsTextView pView;
                    int hr = m.CodeWindow.GetLastActiveView(out pView);
                    if (hr == NativeMethods.S_OK && pView == view)
                        return m;
                }
            }
            return null;
        }

        internal CodeWindowManager GetCodeWindowManagerForSource(ISource src)
        {
            if (src == null) return null;
            foreach (CodeWindowManager m in this.codeWindowManagers)
            {
                if (m.Source == src)
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>Executes the given command if it is enabled and supported using the
        /// current SUIHostCommandDispatcher.</summary>
        internal int DispatchCommand(Guid cmdGuid, uint cmdId, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hr = NativeMethods.E_FAIL;
            IOleCommandTarget cmdTarget = this.Site.GetService(typeof(SUIHostCommandDispatcher)) as IOleCommandTarget;
            if (cmdTarget != null)
            {
                OLECMD[] prgCmds = new OLECMD[1];
                prgCmds[0].cmdID = cmdId;
                hr = cmdTarget.QueryStatus(ref cmdGuid, 1, prgCmds, IntPtr.Zero);
                if (hr == NativeMethods.S_OK &&
                    ((prgCmds[0].cmdf & (uint)OLECMDF.OLECMDF_ENABLED) == (uint)OLECMDF.OLECMDF_ENABLED))
                {
                    hr = cmdTarget.Exec(ref cmdGuid, cmdId, 0, IntPtr.Zero, IntPtr.Zero);
                }
            }
            return hr;
        }

        internal void ScrollToEnd(IVsWindowFrame frame)
        {
            IVsTextView view = VsShell.GetTextView(frame);
            if (view != null)
            {
                ScrollToEnd(view);
            }
        }
        internal void ScrollToEnd(IVsTextView view)
        {
            IVsTextLines buffer;
            NativeMethods.ThrowOnFailure(view.GetBuffer(out buffer));
            int lines;
            NativeMethods.ThrowOnFailure(buffer.GetLineCount(out lines));
            int lineHeight;
            NativeMethods.ThrowOnFailure(view.GetLineHeight(out lineHeight));
            NativeMethods.RECT bounds = new NativeMethods.RECT();
            NativeMethods.GetClientRect(view.GetWindowHandle(), ref bounds);
            int visibleLines = ((bounds.bottom - bounds.top) / lineHeight) - 1;
            // The line number needed to be passed to SetTopLine is ZERO based, so need to subtract ONE from number of total lines
            int top = Math.Max(0, lines - visibleLines - 1);
            Debug.Assert(lines > top, "Cannot set top line to be greater than total number of lines");
#if XMLTRACE
            Trace.WriteLine("ScrollToEnd: lines=" + lines + ", visibleLines=" + visibleLines + ", top=" + top);
#endif
            NativeMethods.ThrowOnFailure(view.SetTopLine(top));
        }

        internal BackgroundRequestAsyncResult BeginBackgroundRequest(BackgroundRequest request, BackgroundRequestResultHandler handler)
        {
            EnsureBackgroundThreadStarted();
            lock (this)
            {
                request.Callback = handler;
                this.requests.Enqueue(request);
                this.isServingBackgroundRequest = true;
                this.backgroundRequestPending.Set();
                this.backgroundRequestDone.Reset();
                // Return a capability to wait on the completion of the background request
                return new BackgroundRequestAsyncResult(request, this.backgroundRequestDone);
            }
        }


        internal BackgroundRequest CreateBackgroundRequest(FSharpSourceBase s, int line, int idx, TokenInfo info, string sourceText, ITextSnapshot snapshot, MethodTipMiscellany methodTipMiscellany, string fname, BackgroundRequestReason reason, IVsTextView view)
        {
            // We set this to "false" because we are effectively abandoning any currently executing background request, e.g. an OnIdle request
            this.isServingBackgroundRequest = false;
            bool sync = false;
            if (!this.Preferences.EnableAsyncCompletion)
            {
                sync = true; //unless registry value indicates that sync ops always prefer async 
            }
            return CreateBackgroundRequest(line, idx, info, sourceText, snapshot, methodTipMiscellany, fname, reason, view, s.CreateAuthoringSink(reason, line, idx), s, s.ChangeCount, sync);
        }

        // Implemented in FSharpLanguageService.fs
        internal abstract BackgroundRequest CreateBackgroundRequest(int line, int col, TokenInfo info, string sourceText, ITextSnapshot snapshot, MethodTipMiscellany methodTipMiscellany, string fname, BackgroundRequestReason reason, IVsTextView view,AuthoringSink sink, ISource source, int timestamp, bool synchronous);

		// Implemented in FSharpLanguageService.fs
		internal abstract void OnParseFileOrCheckFileComplete(BackgroundRequest req);

        internal void EnsureBackgroundThreadStarted()
        {
            if (this.backgroundThread == null && !disposed)
            {
                this.backgroundRequestPending = new ManualResetEvent(false);
                this.backgroundThreadTerminated = new ManualResetEvent(false);
                this.backgroundRequestDone = new ManualResetEvent(false);
                this.backgroundThread = new Thread(new ThreadStart(BackgroundRequestThread));
                this.backgroundThread.Start();
            }
        }

        internal void StopBackgroundThread()
        {
            if (this.backgroundThread != null)
            {
                requests.Set(new BackgroundRequest(true));
                ManualResetEvent ptt = this.backgroundThreadTerminated;
                this.backgroundRequestPending.Set();
                if (!ptt.WaitOne(10, false))
                { // give it a few milliseconds...
                    // Then kill it right away so devenv.exe shuts down quickly and so that
                    // the parse thread doesn't try to access services that are already shutdown.
                    try
                    {
                        this.backgroundThread.Abort();
                        this.backgroundRequestDone.Set(); // make sure this gets set!
                    }
                    catch
                    {
                    }
                    this.backgroundThread = null;
                }
            }
            CleanupThread();
        }

        internal void CleanupThread()
        {
            this.backgroundRequestPending = null;
            this.backgroundThreadTerminated = null;
            this.backgroundThread = null;
            this.backgroundRequestDone = null;
        }

        internal bool IsServingBackgroundRequest
        {
            get { return this.isServingBackgroundRequest; }
        }

        internal PendingRequests requests = new PendingRequests();
        internal ManualResetEvent backgroundRequestPending;
        internal ManualResetEvent backgroundThreadTerminated = new ManualResetEvent(false);
        private ManualResetEvent backgroundRequestDone;

        internal Thread backgroundThread;

        internal void BackgroundRequestThread()
        {
            try
            {

                // Initialize this thread's culture info with that of the shell's LCID
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.lcid);
                bool stop = false;
                while (!stop)
                {
                    if (!backgroundRequestPending.WaitOne(10000, true))
                    {
                        break;
                    }
                    BackgroundRequest req = null;
                    lock (this)
                    {
                        req = this.requests.Dequeue();
                        backgroundRequestPending.Reset();
                    }
                    if (req.Terminate)
                        break;

                    try
                    {
                        // Ensure that no new OnIdle requests are issued during the execution of this request
                        this.isServingBackgroundRequest = true;

                        this.ExecuteBackgroundRequest(req);

                        // If another parse request has already come in then the
                        // user must be typing really fast (e.g. macros) and 
                        // so we throw this response away, and go right on to the
                        // next request.
                        // Note this must be asynchronous (do NOT call invoke).
                        // Reason being that the UI thread may then want to call
                        // StopBackgroundThread, which would deadlock if this was synchronous.
                        if (!requests.ContainsSimilarRequest(req) || req.Reason == BackgroundRequestReason.FullTypeCheck)
                        {
                            UIThread.Run(
                                    delegate()
                                    {
                                        req.Callback(req);
                                    }
                                );
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        stop = true;
                    }
                    catch
                    {
                        // prevent a stray exception from ending the LS processing thread; without this we'll exit the while loop and
                        // this thread will quietly exit and there will no longer be a LS running
                    }
                    finally
                    {
                        if (this.backgroundRequestDone != null) //thread cleanup might have set this to null
                        {
                            this.backgroundRequestDone.Set();
                        }
                    }
                    this.isServingBackgroundRequest = false;
                }
                ManualResetEvent ptt = backgroundThreadTerminated;
                CleanupThread();
                ptt.Set();
                this.isServingBackgroundRequest = false;
            }
            catch
            {
                // final exception handler for the thread, to make sure the whole VS process does not come down due to stray exception
#if LANGTRACE
                Trace.WriteLine("Background Parse Thread Aborted");
#endif
            }
            this.isServingBackgroundRequest = false;
        }


        public void GetSite(ref Guid iid, out IntPtr ptr)
        {
            IntPtr pUnk = Marshal.GetIUnknownForObject(this.site);
            try
            {
                Marshal.QueryInterface(pUnk, ref iid, out ptr);
            }
            finally
            {
                // This is to release the reference from GetIUnknownForObject.
                // There may be an additional reference from the QueryInterface that will be owned by the caller of GetSite
                Marshal.Release(pUnk);
            }
        }
        public void SetSite(object site)
        {
            if (site is IServiceProvider)
            {
                this.site = (IServiceProvider)site;
            }
            else if (site is IOleServiceProvider)
            {
                this.site = new Microsoft.VisualStudio.Shell.ServiceProvider ((IOleServiceProvider)site);
            }
            Microsoft.VisualStudio.Shell.Package pkg = (Microsoft.VisualStudio.Shell.Package)this.site.GetService(typeof(Microsoft.VisualStudio.Shell.Package));
            this.lcid = pkg.GetProviderLocale();
        }

        public virtual int OnModeChange(DBGMODE dbgmodeNew)
        {
            this.dbgMode = dbgmodeNew;
            return NativeMethods.S_OK;
        }

        /// Return true if the given encoding information is invalid for your language service
        /// Default always returns false.  If you return true, then also return an error
        /// message to display to the user.
        internal virtual bool QueryInvalidEncoding(__VSTFF format, out string errorMessage)
        {
            errorMessage = null;
            return false;
        }

		// Provides the list of available extensions for Save As.
		// The following default filter string is automatically added
		// by Visual Studio:
		// "All Files (*.*)\n*.*\nText Files (*.txt)\n*.txt\n"
		internal abstract string GetFormatFilterList();

		// Provides the index to the filter matching the extension of the file passed in.
		internal abstract int CurFileExtensionFormat(string fileName);

        int IVsFormatFilterProvider.QueryInvalidEncoding(uint format, out string pbstrMessage)
        {
            if (QueryInvalidEncoding((__VSTFF)format, out pbstrMessage))
            {
                return NativeMethods.S_OK;
            }
            return NativeMethods.S_FALSE;
        }

        int IVsFormatFilterProvider.CurFileExtensionFormat(string bstrFileName, out uint pdwExtnIndex)
        {
            pdwExtnIndex = 0;
            if (!string.IsNullOrEmpty(bstrFileName))
            {
                int i = CurFileExtensionFormat(bstrFileName);
                if (i >= 0)
                {
                    pdwExtnIndex = (uint)i;
                    return NativeMethods.S_OK;
                }
            }
            return NativeMethods.E_FAIL; // return 0 - but no match found.
        }

        int IVsFormatFilterProvider.GetFormatFilterList(out string pbstrFilterList)
        {
            pbstrFilterList = GetFormatFilterList();
            if (pbstrFilterList.Contains("|"))
            {
                string[] sa = pbstrFilterList.Split('|');
                pbstrFilterList = string.Join("\n", sa);
            }
            if (pbstrFilterList == null)
                return NativeMethods.E_FAIL;

            // Must be terminated with a new line character.
            // (since inside VS this results in the proper Win32 saveas dialog double null 
            // termination format since the new lines are replaced with nulls).
            // (See dlgsave.cpp line 163 in the InvokeSaveAsDlg function).
            if (!pbstrFilterList.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
                pbstrFilterList = pbstrFilterList + "\n";

            return NativeMethods.S_OK;
        }

        /// <summary>
        /// Version number will increment to indicate a change in the semantics of preexisting methods. 
        /// </summary>
        int ILanguageServiceTestHelper.GetSemanticsVersion()
        {
            return 3;
        }

    } // end class LanguageService

    internal class BackgroundRequestAsyncResult
    {
        ManualResetEvent globalRequestCompletedEvent;
        BackgroundRequest req;

        internal BackgroundRequestAsyncResult(BackgroundRequest req, ManualResetEvent globalRequestCompletedEvent)
        {
            this.globalRequestCompletedEvent = globalRequestCompletedEvent;
            this.req = req;
        }
        internal bool TryWaitForBackgroundRequestCompletion(int millisecondsTimeout)
        {
            return this.globalRequestCompletedEvent.WaitOne(millisecondsTimeout, false);
        }

    }

    internal delegate void BackgroundRequestResultHandler(BackgroundRequest request);

    internal enum MethodTipMiscellany
    {
        Typing,                                 // OnCommand TYPECHAR nothing special refresh already-displayed tip
        ExplicitlyInvokedViaCtrlShiftSpace,     // ViewFilter PARAMINFO
        JustPressedBackspace,                   // OnCommand BACKSPACE
        JustPressedOpenParen,                   // OnCommand TYPECHAR TokenTriggers ParamStart
        JustPressedComma,                       // OnCommand TYPECHAR TokenTriggers ParamNext
        JustPressedCloseParen,                  // OnCommand TYPECHAR TokenTriggers ParamEnd
    }

    internal class BackgroundRequest
    {
        int line, col;
        ISource source;
        TextSpan dirtySpan;
        string fileName;
        string text;
        BackgroundRequestReason reason;
        IVsTextView view;
        ITextSnapshot snapshot;
        bool terminate;
        BackgroundRequestResultHandler callback;
        AuthoringSink sink;
        IntellisenseInfo scope;
        bool isFreshFullTypeCheck;
        int startTimeForOnIdleRequest;
        string quickInfoText;
        TextSpan quickInfoSpan;
        TokenInfo tokenInfo;
        int timestamp;
        int resultTimestamp;
        RequireFreshResults requireFreshResults;
        bool isSynchronous;
        internal BackgroundRequestAsyncResult result;

        internal MethodTipMiscellany MethodTipMiscellany { get; set; }

        internal RequireFreshResults RequireFreshResults
        {
            get { return requireFreshResults; }
            set { requireFreshResults = value; }
        }

        internal bool IsSynchronous
        {
            get { return isSynchronous; }
            set { isSynchronous = value; }
        }

        internal BackgroundRequestAsyncResult Result
        {
            get { return result; }
        }

        internal int Line
        {
            get { return this.line; }
            set { this.line = value; }
        }

        internal int Col
        {
            get { return this.col; }
            set { this.col = value; }
        }

        internal TextSpan DirtySpan
        {
            get { return this.dirtySpan; }
            set { this.dirtySpan = value; }
        }

        internal string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        internal string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        internal BackgroundRequestReason Reason
        {
            get { return this.reason; }
            set { this.reason = value; }
        }

        internal IVsTextView View
        {
            get { return this.view; }
            set { this.view = value; }
        }

        internal ITextSnapshot Snapshot
        {
            get { return this.snapshot; }
            set { this.snapshot = value; }
        }

        internal bool Terminate
        {
            get { return this.terminate; }
            set { this.terminate = value; }
        }

        internal BackgroundRequestResultHandler Callback
        {
            get { return this.callback; }
            set { this.callback = value; }
        }

        internal AuthoringSink ResultSink
        {
            get { return this.sink; }
            set { this.sink = value; }
        }

        internal IntellisenseInfo ResultIntellisenseInfo
        {
            get { return this.scope; }
            set { this.scope = value; }
        }

        internal bool ResultClearsDirtinessOfFile
        {
            get { return this.isFreshFullTypeCheck; }
            set { this.isFreshFullTypeCheck = value; }
        }

        internal int StartTimeForOnIdleRequest
        {
            get { return this.startTimeForOnIdleRequest; }
            set { this.startTimeForOnIdleRequest = value; }
        }

        internal string ResultQuickInfoText
        {
            get { return this.quickInfoText; }
            set { this.quickInfoText = value; }
        }

        internal TextSpan ResultQuickInfoSpan
        {
            get { return this.quickInfoSpan; }
            set { this.quickInfoSpan = value; }
        }

        internal TokenInfo TokenInfo
        {
            get { return this.tokenInfo; }
            set { this.tokenInfo = value; }
        }

        internal int Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// File timestamp (ChangeCount) that the results correspond to. This will be different than Timestamp in the case
        /// that stale result were used.
        internal int ResultTimestamp
        {
            get { return this.resultTimestamp; }
            set { this.resultTimestamp = value; }
        }

        internal BackgroundRequest(bool terminate)
        {
            this.Terminate = terminate;
        }

        /// <summary>
        /// Source that represents the opened file for which the parse request was created. 
        /// This can be used for accessing information like file name, line lengths etc.
        /// </summary>
        internal ISource Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        internal bool IsAborted { get; set; }

        internal BackgroundRequest(int line, int col, TokenInfo info, string src, ITextSnapshot snapshot, MethodTipMiscellany methodTipMiscellany, string fname,
                                 BackgroundRequestReason reason, IVsTextView view,
                                 AuthoringSink sink, ISource source, int timestamp, bool synchronous)
        {
            this.Source = source;
            this.Timestamp = timestamp;
            this.Line = line;
            this.Col = col;
            this.FileName = fname;
            this.Text = src;
            this.Reason = reason;
            this.View = view;
            this.Snapshot = snapshot;
            this.MethodTipMiscellany = methodTipMiscellany;
            this.ResultSink = sink;
            this.TokenInfo = info;
            this.isSynchronous = synchronous;

            this.ResultIntellisenseInfo = null;
            this.ResultClearsDirtinessOfFile = false;
        }
    }

    /// <summary>
    /// Represents result returned from scope.Goto
    /// If Success = true, then Url\Span should be filled, ErrorDescription will be null
    /// If Success = false - then only ErrorDescription will have value, Url and Span will have default values
    /// </summary>
    internal class GotoDefinitionResult
    {
        private GotoDefinitionResult(bool success, string url, TextSpan span, string errorDescription)
        {
            Success = success;
            Url = url;
            ErrorDescription = errorDescription;
            Span = span;
        }

        public bool Success { get; private set; }
        public string Url { get; private set; }
        public TextSpan Span { get; private set; }
        public string ErrorDescription { get; private set; }

        /// <summary>
        /// Creates instance of GotoDefinitionResult that will have Success = true
        /// </summary>
        /// <param name="url">Path to source file</param>
        /// <param name="span">Location in source file</param>
        /// <returns>New instance of GotoDefinitionResult</returns>
        public static GotoDefinitionResult MakeSuccess(string url, TextSpan span)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new GotoDefinitionResult(true, url, span, null);
        }

        /// <summary>
        /// Creates instance of GotoDefinitionResult that will have Success = false
        /// </summary>
        /// <param name="errorDescription">Error message</param>
        /// <returns>New instance of GotoDefinitionResult</returns>
        public static GotoDefinitionResult MakeError(string errorDescription)
        {
            if (String.IsNullOrWhiteSpace(errorDescription))
                throw new ArgumentNullException("errorDescription");
            return new GotoDefinitionResult(false, null, default(TextSpan), errorDescription);
        }
    }

    /// <summary>
    /// Stores incoming requests
    /// Maintains queue using following rules
    /// 1. Max amount of items in queue = 2
    /// 2. requests are divided to UI and non-UI
    /// 3. UI request discards all requests that were enqueued before
    /// 4. non-UI request replaces old non-UI request that was enqueued before
    /// 5. if non-UI request is enqueued after UI request -> nothing happens and they will be dequeued subsequently
    /// 
    /// </summary>
    internal class PendingRequests
    {
        private enum RequestType
        {
            Ui,
            NonUi
        }

        private readonly object syncRoot = new object();

        // invariants: (first == null && second == null) || (first != null && second == null) || (first != null && second != null)
        // first == null && second == null => count == 0
        // first != null && second == null => count == 1
        // first != null && second != null => count == 2
        private BackgroundRequest first;
        private BackgroundRequest second;

        public void Enqueue(BackgroundRequest newRequest)
        {
            if (newRequest == null)
                throw new ArgumentNullException("newRequest");

            lock (syncRoot)
            {
                if (first == null)
                {
                    // 0-items                    
                    // just add request to the queue
                    first = newRequest;
                }
                else if (second == null)
                {
                    // 1-item                    
                    var currentType = GetRequestType(newRequest);
                    var previousType = GetRequestType(first);

                    if (currentType == RequestType.Ui || (currentType == previousType))
                    {
                        // - Ui requests discard everything
                        // - non-Ui request discard non-Ui request
                        first = newRequest;
                    }
                    else
                    {
                        // we got here if current type is non-Ui and prev type is Ui
                        second = newRequest;
                    }

                }
                else
                {
                    // 2 items

                    // the only situation with we can have 2 requests in queue is [Ui; Non-Ui]
                    Debug.Assert(GetRequestType(first) == RequestType.Ui);
                    Debug.Assert(GetRequestType(second) == RequestType.NonUi);

                    var requestType = GetRequestType(newRequest);
                    if (requestType == RequestType.Ui)
                    {
                        // discard both old requests
                        first = newRequest;
                        second = null;
                    }
                    else
                    {
                        // replace non-Ui request with a new one
                        second = newRequest;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if request queue contains request similar to the given one.
        /// </summary>
        public bool ContainsSimilarRequest(BackgroundRequest request)
        {
            var requestType = GetRequestType(request); 
            lock (syncRoot)
            {
                if (first == null)
                {
                    // 0 items
                    return false;
                }
                else if (second == null)
                {
                    // 1 item
                    return requestType == GetRequestType(first);
                }
                else
                {
                    // 2 items (both Ui and non-Ui)
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets request from queue
        /// </summary>
        /// <returns></returns>
        public BackgroundRequest Dequeue()
        {
            lock (syncRoot)
            {
                Debug.Assert(first != null);
                var tmp = first;
                
                first = second;
                second = null;
                
                return tmp;
            }
        }

        /// <summary>
        /// Discards all requests added so far and enqueues specified request.
        /// </summary>
        public void Set(BackgroundRequest r)
        {
            lock (syncRoot)
            {
                first = null;
                second = null;

                Enqueue(r);
            }
        }

        /// for tests only!!!
        internal int Count
        {
            get
            {
                if (first == null)
                    return 0;
                if (second == null)
                    return 1;
                return 2;
            }
        }

        private static RequestType GetRequestType(BackgroundRequest r)
        {
            switch (r.Reason)
            {
                case BackgroundRequestReason.ParseFile:
                case BackgroundRequestReason.FullTypeCheck:
                    return RequestType.NonUi;
                default:
                    return RequestType.Ui;        
            }
        }
    }

    internal abstract class IntellisenseInfo
    {
        internal abstract System.Tuple<string,TextSpan> GetDataTipText(int line, int col);

        internal abstract Microsoft.FSharp.Control.FSharpAsync<Declarations> GetDeclarations(ITextSnapshot textSnapshot, int line, int col, BackgroundRequestReason reason);

        internal abstract Microsoft.FSharp.Core.FSharpOption<MethodListForAMethodTip> GetMethodListForAMethodTip();

        internal abstract GotoDefinitionResult Goto(IVsTextView textView, int line, int col);

        internal abstract void GetF1KeywordString(TextSpan span, IVsUserContext context);
    }

    // Note, this class is only implemented once in the F# Language Service implementation, in the F# code which implements the
    // declaration set. It would be better if all the implementation details in this code were put in the F# code.
    internal abstract class Declarations
    {
        internal abstract bool IsEmpty();

        internal abstract int GetCount(string filterText);

        internal abstract string GetDisplayText(string filterText, int index);

        internal abstract String GetName(string filterText, int index);

        internal abstract string GetNameInCode(string filterText, int index);

        internal abstract String GetDescription(string filterText, int index);

        internal abstract int GetGlyph(string filterText, int index);

        internal abstract BackgroundRequestReason Reason { get; }

        // return whether this is a uniqueMatch or not
        internal abstract void GetBestMatch(string filterText, String value, out int index, out bool uniqueMatch, out bool shouldSelectItem);

        internal abstract bool IsCommitChar(char commitCharacter);

        internal abstract string OnCommit(string filterText, int index);

        // This method allows the implementer to do something after completion is finished, for example,
        // in the XML editor the when the user selects a start tag name "<foo", this method is used to
        // insert the end tag automatically "></foo>".  The framework makes sure this method is called at
        // the right time, after VS has actually inserted the result from OnCommit, in this case "foo".
        // It returns one more character to process, which may itself be a trigger for more intellisense.
        internal abstract char OnAutoComplete(IVsTextView textView, string committedText, char commitCharacter, int index);
    }

    //-------------------------------------------------------------------------------------

    // represents all the information necessary to display and navigate withing a method tip (e.g. param info, overloads, ability to move thru overloads and params)
    internal abstract class MethodListForAMethodTip
    {

        internal abstract string GetName(int index);

        internal abstract int GetCount();

        internal abstract string GetDescription(int index);

        internal abstract string GetReturnTypeText(int index);

        internal abstract int GetParameterCount(int index);

        internal abstract void GetParameterInfo(int index, int parameter, out string name, out string display, out string description);

        internal abstract int GetColumnOfStartOfLongId();  // 0-based - this is used for left aligning the tip that gets drawn on the screen

        internal abstract bool IsThereACloseParen();  // false if either this is a call without parens "f x" or the parser recovered as in "f(x,y"

        internal abstract Tuple<int, int>[] GetNoteworthyParamInfoLocations(); // 0-based: longId start, longId end, open paren, <tuple ends> (see below) - relative to the ITextSnapshot this was created against
        //          let resultVal = some.functionOrMethod.call   (   arg1 ,  arg2 )
        //                          ^                        ^   ^        ^       ^
        // start of call identifier ^                        ^   ^        ^       ^
        //                            end of call identifier ^   ^        ^       ^
        //        open paren (or start of first arg if no paren) ^        ^       ^
        //                                                       end of   ^       ^
        //                                                          each arg      ^
        //
        // and thus arg ranges are e.g. computed to be:          |--------|-------|
        // and so when in those regions, we bold that param

        internal abstract ITrackingSpan[] GetParameterRanges();  // GetNoteworthyParamInfoLocations above is for unit testing; VS uses GetParameterRanges instead, to track changes as user types // TODO can we remove GetNoteworthyParamInfoLocations and move unit tests to GetParameterRanges?

        internal abstract string[] GetParameterNames(); // an entry for each actual parameter, either null, or the parameter name if this is a named parameter (e.g. "f(0,y=4)" has [|null;"y"|] )

        internal virtual string OpenBracket
        {
            get { return "("; }
        }
        internal virtual string CloseBracket
        {
            get { return ")"; }
        }
        internal virtual string Delimiter
        {
            get { return ", "; }
        }
        internal virtual bool TypePrefixed
        {
            get { return false; }
        }
        internal virtual string TypePrefix
        {
            get { return null; }
        }
        internal virtual string TypePostfix
        {
            get { return null; }
        }
    }

    internal class BraceMatch
    {
        internal TextSpan a;
        internal TextSpan b;
        internal int priority;

        internal BraceMatch(TextSpan a, TextSpan b, int priority)
        {
            this.a = a;
            this.b = b;
            this.priority = priority;
        }

    }

    internal class TripleMatch : BraceMatch
    {
        internal TextSpan c;

        internal TripleMatch(TextSpan a, TextSpan b, TextSpan c, int priority)
            : base(a, b, priority)
        {
            this.c = c;
        }
    }

    internal delegate void OnErrorAddedHandler(string path, string subcategory, string message, TextSpan context, Severity sev);
    /// <summary>
    /// AuthoringSink is used to gather information from the parser to help in the following:
    /// - error reporting
    /// - matching braces (ctrl-])
    /// - intellisense: Member Selection, CompleteWord, QuickInfo, MethodTips
    /// - management of the autos window in the debugger
    /// - breakpoint validation
    /// </summary>
    /// 
    internal class AuthoringSink
    {
        internal BackgroundRequestReason reason;
        internal int line;
        internal int col;
        internal ArrayList Spans;
        internal ArrayList Braces;
        internal bool foundMatchingBrace;
        internal ArrayList errors;
        private int[] errorCounts;
        private int maxErrors;

        public event OnErrorAddedHandler OnErrorAdded;

        internal AuthoringSink(BackgroundRequestReason reason, int line, int col, int maxErrors)
        {
            this.reason = reason;
            this.errors = new ArrayList();
            this.line = line;
            this.col = col;
            this.Spans = new ArrayList();
            this.Braces = new ArrayList();
            this.errorCounts = new int[4];
            this.maxErrors = maxErrors;
        }

        internal int Line
        {
            get { return this.line; }
        }

        internal int Column
        {
            get { return this.col; }
        }

        internal BackgroundRequestReason Reason
        {
            get { return this.reason; }
        }

        internal bool FoundMatchingBrace
        {
            get { return this.foundMatchingBrace; }
            set { this.foundMatchingBrace = value; }
        }


        private void AddBraces(BraceMatch b)
        {
            this.foundMatchingBrace = true;
            int i = 0;
            for (int n = this.Braces.Count; i < n; i++)
            {
                BraceMatch a = (BraceMatch)this.Braces[i];
                if (a.priority < b.priority)
                    break;
            }
            this.Braces.Insert(i, b);
        }

        /// <summary>Use this property to find if your parser should call MatchPair or MatchTriple</summary>
        internal bool BraceMatching
        {
            get
            {
                switch (this.reason)
                {
                    case BackgroundRequestReason.MatchBraces:
                    case BackgroundRequestReason.MatchBracesAndMethodTip:
                    case BackgroundRequestReason.MemberSelectAndHighlightBraces:
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Whenever a matching pair is parsed, e.g. '{' and '}', this method is called
        /// with the text span of both the left and right item. The
        /// information is used when a user types "ctrl-]" in VS
        /// to find a matching brace and when auto-highlight matching
        /// braces is enabled.  A priority can also be given so that multiple overlapping pairs 
        /// can be prioritized for brace matching.  The matching pair with the highest priority 
        /// (largest integer value) wins.
        /// </summary>
        internal virtual void MatchPair(TextSpan span, TextSpan endContext, int priority)
        {
            if (BraceMatching)
            {
                TextSpanHelper.MakePositive(ref span);
                TextSpanHelper.MakePositive(ref endContext);
                if (TextSpanHelper.ContainsInclusive(span, this.line, this.col) ||
                    TextSpanHelper.ContainsInclusive(endContext, this.line, this.col))
                {
                    this.Spans.Add(span);
                    this.Spans.Add(endContext);
                    AddBraces(new BraceMatch(span, endContext, priority));
                }
            }
        }

        /// <summary>
        /// Matching tripples are used to highlight in bold a completed statement.  For example
        /// when you type the closing brace on a foreach statement VS highlights in bold the statement
        /// that was closed.  The first two source contexts are the beginning and ending of the statement that
        /// opens the block (for example, the span of the "foreach(...){" and the third source context
        /// is the closing brace for the block (e.g., the "}").  A priority can also be given so that
        /// multiple overlapping pairs can be prioritized for brace matching.  
        /// The matching pair with the highest priority  (largest integer value) wins.
        /// </summary>
        internal virtual void MatchTriple(TextSpan startSpan, TextSpan middleSpan, TextSpan endSpan, int priority)
        {
            if (BraceMatching)
            {
                TextSpanHelper.MakePositive(ref startSpan);
                TextSpanHelper.MakePositive(ref middleSpan);
                TextSpanHelper.MakePositive(ref endSpan);
                if (TextSpanHelper.ContainsInclusive(startSpan, this.line, this.col) ||
                    TextSpanHelper.ContainsInclusive(middleSpan, this.line, this.col) ||
                    TextSpanHelper.ContainsInclusive(endSpan, this.line, this.col))
                {
                    this.Spans.Add(startSpan);
                    this.Spans.Add(middleSpan);
                    this.Spans.Add(endSpan);
                    AddBraces(new TripleMatch(startSpan, middleSpan, endSpan, priority));
                }
            }
        }

        /// <summary>
        /// AutoExpression is in support of IVsLanguageDebugInfo.GetProximityExpressions.
        /// It is called for each expression that might be interesting for
        /// a user in the "Auto Debugging" window. All names that are
        /// set using StartName and QualifyName are already automatically
        /// added to the "Auto" window! This means that AutoExpression
        /// is rarely used.
        /// </summary>
        internal virtual void AutoExpression(TextSpan expr)
        {
        }

        /// <summary>
        /// CodeSpan is in support of IVsLanguageDebugInfo.ValidateBreakpointLocation.
        /// It is called for each region that contains "executable" code.
        /// This is used to validate breakpoints. Comments are
        /// automatically taken care of based on TokenInfo returned from scanner. 
        /// Normally this method is called when a procedure is started/ended.
        /// </summary>
        internal virtual void CodeSpan(TextSpan span)
        {
        }

        /// <summary>
        /// Add an error message. This method also filters out duplicates so you only
        /// see the unique errors in the error list window.
        /// </summary>
        internal virtual void AddError(string path, string subcategory, string message, TextSpan context, Severity sev)
        {
            if (context.iStartLine < 0 || context.iEndLine < context.iStartLine || context.iStartIndex < 0 || (context.iEndLine == context.iStartLine && context.iEndIndex < context.iStartIndex))
            {
                //TODO: reenable this!
                //Debug.Assert(false);
                return;
            }
            int i = (int)sev;
            if (this.errorCounts[i] == this.maxErrors)
                return; // reached maximum

            // Make sure the error is unique.
            foreach (ErrorNode n in this.errors)
            {
                if ((TextSpanHelper.IsSameSpan(n.context, context) ||
                     TextSpanHelper.IsEmbedded(n.context, context) ||
                     TextSpanHelper.IsEmbedded(context, n.context)) &&
                    n.message == message &&
                    n.severity == sev &&
                    n.uri == path)
                {
                    return; // then it's a duplicate!
                }
            }
            this.errorCounts[i]++;
            this.errors.Add(new ErrorNode(path, subcategory, message, context, sev));
            if (this.OnErrorAdded != null)
                this.OnErrorAdded.Invoke(path, subcategory, message, context, sev);
        }

    }; // AuthoringSink

    internal class ErrorNode
    {
        internal string uri;
        internal string message;
        internal string subcategory;
        internal TextSpan context;
        internal Severity severity;
        internal ErrorNode(string uri, string subcategory, string message, TextSpan context, Severity severity)
        {
            this.uri = uri;
            this.subcategory = subcategory;
            this.message = message;
            this.context = context;
            this.severity = severity;
        }
    }


}
