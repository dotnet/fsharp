// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using VsShell = Microsoft.VisualStudio.Shell.VsShellUtilities;
using Microsoft.VisualStudio.FSharp.LanguageService.Resources;
using Microsoft.VisualStudio.FSharp.LanguageService; 

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    /// <summary>
    /// ViewFilter provides a default implementation of IVsTextViewFilter providing a
    /// handling of the following VS commands:
    /// 
    /// VsCommands.GotoDefn
    /// VsCommands.GotoDecl
    /// VsCommands.GotoRef
    /// VsCommands2K.COMMENT_BLOCK
    /// VsCommands2K.UNCOMMENT_BLOCK
    /// VsCommands2K.SHOWMEMBERLIST
    /// VsCommands2K.COMPLETEWORD
    /// VsCommands2K.PARAMINFO
    /// VsCommands2K.QUICKINFO
    /// VsCommands2K.OUTLN_STOP_HIDING_ALL
    /// VsCommands2K.OUTLN_START_AUTOHIDING
    /// VsCommands2K.SHOWCONTEXTMENU
    /// 
    /// Most of the work is delegated to the Source object.
    /// </summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ViewFilter : IVsTextViewFilter, IVsTextViewEvents, IOleCommandTarget, IDisposable, IVsExpansionEvents {
        private CodeWindowManager mgr;
        private NativeMethods.ConnectionPointCookie textViewEvents;
        private LanguageService_DEPRECATED service;
        private IVsTextView textView;
        private IOleCommandTarget nextTarget;
        private TextTipData textTipData;
        private ISource source;
        // the current dataTipText info...
        private int quickInfoRequestLine;
        private int quickInfoRequestCol;
        private TextSpan quickInfoSpan;
        private string quickInfoText;
        private bool gotQuickInfo;
        private bool wasCompletionActive;
        private bool commentSupported;
        private bool autoCompleted;
        private bool autoCompletedNothing;
        private bool autoCompleteTypeChar;
        private IntPtr pvaChar;
        private bool gotEnterKey;
        private bool snippetBound;
        private VsCommands gotoCmd;
        private readonly Guid guidInteractive = new Guid("8B9BF77B-AF94-4588-8847-2EB2BFFD29EB");
        private readonly uint cmdIDDebugSelection = 0x01;

        protected bool SnippetBound {
            get { return snippetBound; }
            set { snippetBound = value; }
        }

        private NativeMethods.ConnectionPointCookie expansionEvents;

        private Microsoft.VisualStudio.Shell.Package projectSystemPackage = null;

        private Microsoft.VisualStudio.Shell.Package GetProjectSystemPackage()
        {
            // Ideally the FsiToolWindow would be a part of the language service, but right now its code lives in the
            // project system, and it would require setup authoring changes to update it.  So for now, we just force-load the
            // project system when FSI commands are first used, which is good enough.
            if (this.projectSystemPackage == null)
            {
                IVsShell shell = this.service.Site.GetService(typeof(SVsShell)) as IVsShell;
                Guid PackageToBeLoadedGuid = new Guid("{91a04a73-4f2c-4e7c-ad38-c1a68e7da05c}");  // FSharp ProjectSystem guid
                Microsoft.VisualStudio.Shell.Interop.IVsPackage pkg;
                shell.LoadPackage(ref PackageToBeLoadedGuid, out pkg);
                this.projectSystemPackage = (Microsoft.VisualStudio.Shell.Package)pkg; // we know our object is an instance of this type
            }
            return this.projectSystemPackage;
        }

        internal ViewFilter(CodeWindowManager mgr, IVsTextView view) {
            this.pvaChar = IntPtr.Zero;
            this.mgr = mgr;
            this.service = mgr.LanguageService;
            this.source = mgr.Source;
            this.commentSupported = this.service.Preferences.EnableCommenting;
            this.textView = view;
            NativeMethods.ThrowOnFailure(view.AddCommandFilter(this, out nextTarget));
            this.textViewEvents = new NativeMethods.ConnectionPointCookie(view, this, typeof(IVsTextViewEvents));
            if (this.service != null) {
                IVsExpansionManager emgr = this.service.Site.GetService(typeof(SVsExpansionManager)) as IVsExpansionManager;
                if (emgr != null) {
                    int fBound;
                    emgr.GetSnippetShortCutKeybindingState(out fBound);
                    this.snippetBound = fBound == 0 ? false : true;
                    this.expansionEvents = new NativeMethods.ConnectionPointCookie(emgr, this, typeof(IVsExpansionEvents));
                }
            }
        }

        ~ViewFilter() {
            Dispose();
#if LANGTRACE
            Trace.WriteLine("~ViewFilter");
#endif
        }

        public virtual void Dispose() {
            this.textView = null;
            this.service = null;
            this.nextTarget = null;
            this.textTipData = null;
            this.mgr = null;
            if (this.pvaChar != IntPtr.Zero) {
                Marshal.FreeCoTaskMem(pvaChar);
                pvaChar = IntPtr.Zero;
            }
            GC.SuppressFinalize(this); // REVIEW: Why this?
        }

        public virtual void Close() {
#if LANGTRACE
            Trace.WriteLine("ViewFilter::Close");
#endif
            if (this.expansionEvents != null) {
                expansionEvents.Dispose();
                expansionEvents = null;
            }
            if (textViewEvents != null) {
                textViewEvents.Dispose();
                textViewEvents = null;
            }

            if (textView == this.service.LastActiveTextView)
                this.service.OnActiveViewChanged(null);

            textView.RemoveCommandFilter(this); // do not care about HRESULT.
            if (textTipData != null) {
                textTipData.Close(textView);
                textTipData = null;
            }

            Dispose();
        }
        const int SizeOfVariant = 16;

        public virtual int OnAfterSnippetsKeyBindingChange(uint dwCmdGuid, uint dwCmdId, int fBound) {
            this.snippetBound = fBound == 0 ? false : true;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public virtual int OnAfterSnippetsUpdate() {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>Returnt the CodeWindowManager that created this view filter.</summary>
        public CodeWindowManager CodeWindowManager {
            get { return this.mgr; }
        }

        /// <summary>Return the Source object encapsulating the text buffer.</summary>
        internal ISource Source {
            get { return this.source; }
        }

        /// <summary>Get or set the TextTipData object used for displaying tool tips.</summary>
        public TextTipData TextTipData {
            get {
                if (this.textTipData == null) {
                    this.textTipData = this.CreateTextTipData();
                }
                return this.textTipData;
            }
            set { this.textTipData = value; }
        }

        /// <summary>Return the IVsTextView associated with this filter.</summary>
        public IVsTextView TextView {
            get { return this.textView; }
        }

        public virtual bool IsExpansionUIActive {
            get {
                IVsTextViewEx tve = this.textView as IVsTextViewEx;
                if (tve != null && tve.IsExpansionUIActive() == Microsoft.VisualStudio.VSConstants.S_OK) {
                    return true;
                }
                return false;
            }
        }

        /// <summary>Returns the result of Source.GetWordExtent.</summary>
        public virtual int GetWordExtent(int line, int index, uint flags, TextSpan[] span) {
            Debug.Assert(line >= 0 && index >= 0);
            if (span == null) NativeHelpers.RaiseComError(NativeMethods.E_INVALIDARG);
            else
                span[0] = new TextSpan();

            span[0].iStartLine = span[0].iEndLine = line;
            span[0].iStartIndex = span[0].iEndIndex = index;

            int start, end;

            if (!this.source.GetWordExtent(line, index, (WORDEXTFLAGS)flags, out start, out end)) {
                return NativeMethods.S_FALSE;
            }

            span[0].iStartIndex = start;
            span[0].iEndIndex = end;
            TextSpanHelper.MakePositive(ref span[0]);
            return NativeMethods.S_OK;
        }

        /// <summary>
        /// If Preferences.EnableQuickInfo is true then this method kicks of a parse with the 
        /// reason BackgroundRequestReason.QuickInfo to find information about the current token.  If the
        /// parse finds something (returned via the IntellisenseInfo.GetDataTipText) then it is
        /// displayed using the TextTipData object.  When the asynchronous parse is finished
        /// GetFullDataTipText is called to pop up the tip.
        /// </summary>
        public virtual int GetDataTipText(TextSpan[] aspan, out string textValue) {
            textValue = null;

            TextSpan span = aspan[0];

            if (!service.Preferences.EnableQuickInfo) {
                return NativeMethods.E_FAIL;
            }

            if (span.iEndLine == this.quickInfoRequestLine && span.iEndIndex == this.quickInfoRequestCol) {
                if (!gotQuickInfo) {
                    // still parsing on the background thread, so return E_PENDING.
                    return (int)NativeMethods.E_PENDING;
                }

                this.quickInfoRequestLine = -1;
                int hr = this.GetFullDataTipText(this.quickInfoText, quickInfoSpan, out textValue);
                aspan[0] = this.quickInfoSpan;
                if (hr != NativeMethods.S_OK)
                    return hr;
            } else {
                // kick off the background parse to get this information...
                this.quickInfoText = null;
                this.gotQuickInfo = false;
                this.quickInfoRequestLine = span.iEndLine;
                this.quickInfoRequestCol = span.iEndIndex;
                this.source.BeginBackgroundRequest(span.iEndLine, span.iEndIndex, new TokenInfo(), BackgroundRequestReason.QuickInfo, this.textView, RequireFreshResults.No, new BackgroundRequestResultHandler(GetDataTipResponse));
                return (int)NativeMethods.E_PENDING;
            }
            // This return code means that regardless of whether we found any tooltip or not
            // the error associated with a code marker takes precedence (see DocumentTask.cs).
            return (int)TipSuccesses.TIP_S_ONLYIFNOMARKER;
        }


        internal void GetDataTipResponse(BackgroundRequest_DEPRECATED req)
        {
            if (req == null || req.Source == null || req.Source.IsClosed) return;

            if ((req.ResultQuickInfoText != null) && TextSpanHelper.ContainsInclusive(req.ResultQuickInfoSpan, this.quickInfoRequestLine, this.quickInfoRequestCol))
            {
                this.quickInfoText = req.ResultQuickInfoText;
                this.gotQuickInfo = true;
                this.quickInfoSpan = req.ResultQuickInfoSpan;

            }
        }

        public virtual int GetPairExtents(int line, int index, TextSpan[] span) {
            Debug.Assert(line >= 0 && index >= 0);
            // This call from VS does not support returning E_PENDING.
            if (span == null) return NativeMethods.E_INVALIDARG;

            this.source.GetPairExtents(this.textView, line, index, out span[0]);
            TextSpanHelper.MakePositive(ref span[0]);
            return NativeMethods.S_OK;
        }

        public virtual void OnChangeCaretLine(IVsTextView view, int line, int col) {
        }

        public virtual void OnChangeScrollInfo(IVsTextView view, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit) {
        }

        public virtual void OnKillFocus(IVsTextView view) {
            this.service.OnActiveViewLostFocus(view);
            this.mgr.OnKillFocus(view);
        }

        public virtual void OnSetBuffer(IVsTextView view, IVsTextLines buffer) {
            Debug.Assert(buffer == this.mgr.Source.GetTextLines());
        }

        public virtual void OnSetFocus(IVsTextView view) {
            this.service.OnActiveViewChanged(view);
            if (this.mgr != null) this.mgr.OnSetFocus(view); // is null during shutdown.
        }

        /// <summary>
        /// Override this method to intercept the IOleCommandTarget::QueryStatus call.
        /// </summary>
        /// <param name="guidCmdGroup"></param>
        /// <param name="nCmdId"></param>
        /// <returns>Usually returns a combination of OLECMDF flags, for example
        /// OLECMDF_ENABLED | OLECMDF_SUPPORTED.  
        /// Return E_FAIL if want to delegate to the next command target.
        /// </returns>
        protected virtual int QueryCommandStatus(ref Guid guidCmdGroup, uint nCmdId)
        {
            ExpansionProvider ep = GetExpansionProvider();
            if (ep != null && ep.InTemplateEditingMode)
            {
                int hr = 0;
                if (ep.HandleQueryStatus(ref guidCmdGroup, nCmdId, out hr))
                    return hr;
            }
            if (guidCmdGroup == typeof(VsCommands).GUID)
            {
                VsCommands cmd = (VsCommands)nCmdId;

                switch (cmd)
                {
                case VsCommands.GotoDefn:
                case VsCommands.GotoDecl:
                case VsCommands.GotoRef:
                case VsCommands.Goto:
                    return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                }
            }
            else if (guidCmdGroup == typeof(VsCommands2K).GUID)
            {
                VsCommands2K cmd = (VsCommands2K)nCmdId;

                switch (cmd)
                {
                case VsCommands2K.FORMATDOCUMENT:
                    if (this.CanReformat())
                        return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    break;
                case VsCommands2K.FORMATSELECTION:
                    if (this.CanReformat())
                        return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    break;

                case VsCommands2K.COMMENT_BLOCK:
                case VsCommands2K.UNCOMMENT_BLOCK:
                    if (this.commentSupported)
                        return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    break;

                case VsCommands2K.SHOWMEMBERLIST:
                case VsCommands2K.COMPLETEWORD:
                case VsCommands2K.PARAMINFO:
                    return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;

                case VsCommands2K.QUICKINFO:
                    if (this.service.Preferences.EnableQuickInfo)
                    {
                        return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    }
                    break;

                //                    case VsCommands2K.HANDLEIMEMESSAGE:
                //                        return 0;

                // Let the core editor handle this.  Stop outlining also removes user
                // defined hidden sections so it is handy to keep this enabled.
                //                    case VsCommands2K.OUTLN_STOP_HIDING_ALL: 
                //                        int rc = (int)OLECMDF.OLECMDF_SUPPORTED;
                //                        if (this.source.OutliningEnabled) {
                //                            rc |= (int)OLECMDF.OLECMDF_ENABLED;
                //                        }
                //                        return rc;

                case VsCommands2K.OUTLN_START_AUTOHIDING:
                    if (this.source.OutliningEnabled)
                    {
                        return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
                    }
                    return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;

                case VsCommands2K.OUTLN_STOP_HIDING_ALL: //"stop outlining" on context menu
                    if (this.source.OutliningEnabled)
                    {
                        return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    }
                    return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
                }
            }
            else if (guidCmdGroup == Microsoft.VisualStudio.VSConstants.VsStd11)
            {
                if (nCmdId == (uint)Microsoft.VisualStudio.VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive)
                {
                    return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                }
                else if (nCmdId == (uint)Microsoft.VisualStudio.VSConstants.VSStd11CmdID.ExecuteLineInInteractive)
                {
                    return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED | (int)OLECMDF.OLECMDF_DEFHIDEONCTXTMENU;
                }
            }
            else if (guidCmdGroup == guidInteractive)
            {
                if (nCmdId == cmdIDDebugSelection)
                {
                    var dbgState = Interactive.Hooks.GetDebuggerState(GetProjectSystemPackage());

                    if (dbgState == Interactive.FsiDebuggerState.AttachedNotToFSI)
                        return (int)OLECMDF.OLECMDF_INVISIBLE;
                    else
                        return (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                }
            }

            return (int)NativeMethods.E_FAIL; // delegate to next command target.
        }

        /// <summary>
        /// The parameter list of a command is queried by calling Exec with the LOWORD
        /// of nCmdexecopt set to OLECMDEXECOPT_SHOWHELP (instead of the more usual
        /// OLECMDEXECOPT_DODEFAULT), the HIWORD of nCmdexecopt set to
        /// VSCmdOptQueryParameterList, pvaIn set to NULL, and pvaOut pointing to an
        /// empty VARIANT ready to receive the result BSTR.  This should be done only
        /// for commands that are marked with the ALLOWPARAMS flags in the command
        /// table.        
        /// </summary>
        /// <returns>Usually returns 0 if ok, or OLECMDERR_E_NOTSUPPORTED</returns>
        protected virtual int QueryParameterList(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
#if LANGTRACE
            Trace.WriteLine(String.Format("QueryParameterList({0},{1})", guidCmdGroup.ToString(), nCmdId));
#endif
            if (guidCmdGroup == typeof(VsCommands).GUID)
            {
                VsCommands cmd = (VsCommands)nCmdId;
                switch (cmd)
                {
                    case VsCommands.Goto:
                        Marshal.GetNativeVariantForObject("~", pvaOut);  // No clue what this is, just copying from  env\Editor\Pkg\Impl\VsTextViewAdapter_Commands.cs
                        return Microsoft.VisualStudio.VSConstants.S_OK;
                }
            }
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        public virtual bool HandlePreExec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            
            this.wasCompletionActive = this.Source.IsCompletorActive;
            this.gotEnterKey = false;

            int line;
            int col;
            var hr = textView.GetCaretPos(out line, out col);
            if (NativeMethods.Failed(hr))
                return false;

            if (guidCmdGroup == typeof(VsCommands).GUID) {
                VsCommands cmd = (VsCommands)nCmdId;
#if TRACE_EXEC
                if (cmd != VsCommands.SolutionCfg && cmd != VsCommands.SearchCombo) {
                    Trace.WriteLine(String.Format("ExecCommand: {0}", cmd.ToString()));
                }
#endif

                
                switch (cmd) {
                    case VsCommands.GotoDefn:
                    case VsCommands.GotoDecl:
                    case VsCommands.GotoRef:
                        HandleGoto(cmd, line, col);
                        return true;
                }
            } else if (guidCmdGroup == typeof(VsCommands2K).GUID) {

                VsCommands2K cmd = (VsCommands2K)nCmdId;

                switch (cmd) {
                    case VsCommands2K.FORMATDOCUMENT:
                        this.ReformatDocument();
                        return true;

                    case VsCommands2K.FORMATSELECTION:
                        this.ReformatSelection();
                        return true;

                    case VsCommands2K.COMMENT_BLOCK:
                        this.CommentSelection();
                        return true;

                    case VsCommands2K.UNCOMMENT_BLOCK:
                        this.UncommentSelection();
                        return true;

                    case VsCommands2K.COMPLETEWORD:
                        this.source.Completion(this.textView, this.source.GetTokenInfo(line, col), BackgroundRequestReason.CompleteWord, RequireFreshResults.No);
                        return true;

                    case VsCommands2K.SHOWMEMBERLIST:
                        this.source.Completion(this.textView, this.source.GetTokenInfo(line, col), BackgroundRequestReason.DisplayMemberList, RequireFreshResults.No);
                        return true;

                    case VsCommands2K.PARAMINFO:
                        this.source.MethodTip(this.textView, line, col, this.source.GetTokenInfo(line, col), MethodTipMiscellany_DEPRECATED.ExplicitlyInvokedViaCtrlShiftSpace, RequireFreshResults.No);
                        return true;

                    case VsCommands2K.QUICKINFO: 
                        HandleQuickInfo(line, col);
                        return true;

                    case VsCommands2K.SHOWCONTEXTMENU:
                        this.ShowContextMenu(Microsoft.VisualStudio.Shell.VsMenus.IDM_VS_CTXT_CODEWIN, Microsoft.VisualStudio.Shell.VsMenus.guidSHLMainMenu, null);
                        return true;

                    //                    case VsCommands2K.HANDLEIMEMESSAGE:
                    //                        if (pvaOut != IntPtr.Zero) {
                    //                            Marshal.GetNativeVariantForObject(false, pvaOut); //debug this make sure it's right ...
                    //                        }
                    //                        break;

                    case VsCommands2K.OUTLN_STOP_HIDING_ALL:
                        this.source.OutliningEnabled = false;
                        break;

                    case VsCommands2K.OUTLN_START_AUTOHIDING:
                        this.source.OutliningEnabled = true;
                        break;

                }
            }
            else if (guidCmdGroup == Microsoft.VisualStudio.VSConstants.VsStd11)
            {
                if (nCmdId == (uint) Microsoft.VisualStudio.VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive)
                {
                    Interactive.Hooks.OnMLSend(GetProjectSystemPackage(), Interactive.FsiEditorSendAction.ExecuteSelection);
                    return true;
                }
                else if (nCmdId == (uint) Microsoft.VisualStudio.VSConstants.VSStd11CmdID.ExecuteLineInInteractive)
                {
                    Interactive.Hooks.OnMLSend(GetProjectSystemPackage(), Interactive.FsiEditorSendAction.ExecuteLine);
                    return true;
                }
            }
            else if (guidCmdGroup == guidInteractive)
            {
                if (nCmdId == cmdIDDebugSelection)
                {
                    Interactive.Hooks.OnMLSend(GetProjectSystemPackage(), Interactive.FsiEditorSendAction.DebugSelection);
                    return true;
                }
            }

            return false;
        }

        /// <summary>This method hooks up HandleSmartIndent and Source.OnCommand.  </summary>
        public virtual void HandlePostExec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut, bool bufferWasChanged) {

            if (guidCmdGroup == typeof(VsCommands2K).GUID) {
                Microsoft.VisualStudio.VSConstants.VSStd2KCmdID cmd = (VsCommands2K)nCmdId;
                char ch = '\0';
                if (cmd == VsCommands2K.TYPECHAR && pvaIn != IntPtr.Zero) {
                    Variant v = Variant.ToVariant(pvaIn);
                    ch = v.ToChar();

#if TRACE_EXEC
                    Trace.WriteLine(String.Format("ExecCommand: {0}, '{1}', {2}", cmd.ToString(), ch.ToString(), (int)ch));
#endif
                }

                switch (cmd) {
                    case Microsoft.VisualStudio.VSConstants.VSStd2KCmdID.RETURN:
                        gotEnterKey = true;
                        // Handle smart-indentation after core text editor has
                        // actually performed the newline operation.
                        if (bufferWasChanged && !this.wasCompletionActive && this.service.Preferences.IndentStyle == IndentingStyle.Smart) {
                            if (HandleSmartIndent())
                                break;
                        }
                        break;

                    case VsCommands2K.TYPECHAR:
                    case VsCommands2K.BACKSPACE:
                    case VsCommands2K.TAB:
                    case VsCommands2K.BACKTAB:
                    case VsCommands2K.DELETE:
                        // check general trigger characters for intellisense
                        if (bufferWasChanged) {
                        this.source.OnCommand(this.textView, cmd, ch);
                        }
                        break;
                    // these commands are important for parameter info and parentheses matching
                    case VsCommands2K.LEFT:
                    case VsCommands2K.LEFT_EXT:
                    case VsCommands2K.RIGHT:
                    case VsCommands2K.RIGHT_EXT:
                    case VsCommands2K.WORDNEXT:
                    case VsCommands2K.WORDNEXT_EXT:
                    case VsCommands2K.WORDPREV:
                    case VsCommands2K.WORDPREV_EXT:
                    // handle keys that change the location as well to make sure that parameter info disappears
                    case VsCommands2K.UP:
                    case VsCommands2K.UP_EXT:
                    case VsCommands2K.DOWN:
                    case VsCommands2K.DOWN_EXT:
                    case VsCommands2K.HOME:
                    case VsCommands2K.HOME_EXT:
                    case VsCommands2K.END:
                    case VsCommands2K.END_EXT:
                        // check general trigger characters for intellisense
                        this.source.OnCommand(this.textView, cmd, ch);
                        break;
                    case VsCommands2K.OUTLN_STOP_HIDING_ALL:
                        this.source.DisableOutlining();
                        break;
                    case VsCommands2K.OUTLN_START_AUTOHIDING:
                        // subtle way of calling this.source.OnUntypedParseInfoUpdate(..);
                        this.source.RecordChangeToView();
                        break;
                    case VsCommands2K.OUTLN_TOGGLE_ALL:
                        this.source.ToggleRegions();
                        break;
                }
            }
            return;
        }

        [DllImport("oleaut32")]
        static extern void VariantInit(IntPtr pObject);
        [DllImport("oleaut32", PreserveSig = false)]
        static extern void VariantClear(IntPtr pObject);

        /// <summary>
        /// Override this method to intercept the IOleCommandTarget::Exec call.
        /// </summary>
        /// <returns>Usually returns 0 if ok, or OLECMDERR_E_NOTSUPPORTED</returns>
        protected virtual int ExecCommand(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {

            this.autoCompleted = false;
            int rc = 0;
            if (this.IsExpansionUIActive) {
                // Pass it along to the expansion UI picker.
                return this.InnerExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
            }

            ExpansionProvider ep = GetExpansionProvider();
            if (ep != null && ep.InTemplateEditingMode ) {
                if (ep.HandlePreExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut))
                    return rc;
            }

            if (!HandlePreExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) &&
                this.nextTarget != null) {

                // Pass it along the chain.
                int count = this.source.ChangeCount;
                rc = this.InnerExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
                if (!Microsoft.VisualStudio.ErrorHandler.Succeeded(rc))
                    return rc;

                bool bufferWasChanged = count != source.ChangeCount;

                if (this.autoCompleted) {
                    this.autoCompleted = false;
                    // See if completion set just completed, so we can do auto-completion synchronously.
                    CompletionSet cset = this.source.CompletionSet;
                    if (cset != null) {
                        if (this.autoCompletedNothing &&
                            this.wasCompletionActive && guidCmdGroup == typeof(VsCommands2K).GUID &&
                            nCmdId == (uint)VsCommands2K.RETURN) {
                            // This happens if the user typed ENTER while the CompletionSet was active,
                            // but selected nothing from the list.  In this case the ENTER key was gobbled up
                            // by the CompletionSet, and we want the ENTER key inserted into the buffer instead.
                            this.wasCompletionActive = false;
                            this.InnerExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
                        }
                    }
                    if (this.autoCompleteTypeChar) {
                        // Auto-complete typed a different char so we skip HandlePostExec in this case.
                        return rc;
                    }
                }

                if (ep != null && ep.InTemplateEditingMode ) {
                    if (ep.HandlePostExec(ref guidCmdGroup, nCmdId, nCmdexecopt, gotEnterKey, pvaIn, pvaOut))
                        return rc;
                }
                HandlePostExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut, bufferWasChanged);
            }
            return rc;
        }

        protected virtual int InnerExec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            return this.nextTarget.Exec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
        }

        public virtual void OnAutoComplete() {
            this.autoCompletedNothing = false;
            this.autoCompleteTypeChar = false;
            CompletionSet cset = this.source.CompletionSet;
            if (cset != null) {
                char ch = cset.OnAutoComplete();
                if (ch != '\0') {
                    this.autoCompleteTypeChar = true;
                    bool wca = this.wasCompletionActive;
                    TypeChar(ch);
                    this.wasCompletionActive = wca;
                } else {
                    this.autoCompletedNothing = string.IsNullOrEmpty(cset.OnCommitText);
                }
            }
            // This must be placed after the above TypeChar because TypeChar re-enters
            // ExecCommand and we do not want to do the autoCompleted block until after
            // this method returns.
            this.autoCompleted = true;
        }

        // Executes a VsCommands2K.TYPECHAR command on the current command chain.
        public int TypeChar(char ch) {

            if (this.pvaChar == IntPtr.Zero) {
                this.pvaChar = Marshal.AllocCoTaskMem(SizeOfVariant);
            }
            if (pvaChar == IntPtr.Zero)
                return NativeMethods.E_OUTOFMEMORY;

            VariantInit(pvaChar);
            Marshal.GetNativeVariantForObject(ch, pvaChar);
            int rc = 0;
            try {
				Guid cmdGroup = typeof(VsCommands2K).GUID;
                rc = ExecCommand(ref cmdGroup, (uint)VsCommands2K.TYPECHAR, 0, pvaChar, IntPtr.Zero);
            } finally {
                VariantClear(pvaChar);
            }
            return rc;
        }

        /// Override this method if you want to support smart indenting.
        /// This will only be called if Preferences.Indenting == IndentingStyle.Smart which is
        /// only available if you set your language service registry key ShowSmartIndent to 1.
        public virtual bool HandleSmartIndent() {
            return false;
        }

        /// <internalonly/>
        int IOleCommandTarget.QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            for (uint i = 0; i < cCmds; i++) {
                int rc = QueryCommandStatus(ref guidCmdGroup, (uint)prgCmds[i].cmdID);

                if (rc == NativeMethods.E_FAIL) {
                    if (nextTarget != null) {
                        try {
                            return this.nextTarget.QueryStatus(ref guidCmdGroup, cCmds, prgCmds, pCmdText);
                        } catch (Exception) {
                            // We are going to return the failed return code below
                        }
                    }

                    return rc;
                }

                prgCmds[i].cmdf = (uint)rc;
            }

            return NativeMethods.S_OK;
        }

        /// <internalonly/>
        int IOleCommandTarget.Exec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {

            ushort lo = (ushort)(nCmdexecopt & (uint)0xffff);
            ushort hi = (ushort)(nCmdexecopt >> 16);
            switch (lo) {
                case (ushort)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP:
                    if ((nCmdexecopt >> 16) == Microsoft.VisualStudio.Shell.VsMenus.VSCmdOptQueryParameterList) {
                        return QueryParameterList(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
                    }
                    break;
                default:
                    // On every command, update the tip window if it's active.
                    if (this.textTipData != null && this.textTipData.IsActive())
                        textTipData.CheckCaretPosition(this.textView);

                    int rc = 0;
                    try {
                        rc = ExecCommand(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
                    } catch (COMException e) {
                        int hr = e.ErrorCode;
                        // We silently fail on the following errors because the user has
                        // most likely already been prompted with things like source control checkout
                        // dialogs and so forth.
                        if (hr != (int)TextBufferErrors.BUFFER_E_LOCKED &&
                            hr != (int)TextBufferErrors.BUFFER_E_READONLY &&
                            hr != (int)TextBufferErrors.BUFFER_E_READONLY_REGION &&
                            hr != (int)TextBufferErrors.BUFFER_E_SCC_READONLY) {
                            throw;
                        }
                    }

                    return rc;
            }
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        /// <summary>This method is called to handle the VsCommands2K.QUICKINFO command.</summary>
        public virtual void HandleQuickInfo(int line, int col) {
            // Get the tip text at that location. 
            this.source.BeginBackgroundRequest(line, col, new TokenInfo(), BackgroundRequestReason.QuickInfo, this.textView, RequireFreshResults.No, 
                new BackgroundRequestResultHandler(HandleQuickInfoResponse));
        }

        void HandleQuickInfoResponse(BackgroundRequest_DEPRECATED req){
            if (req == null || req.Source == null || req.Source.IsClosed) return;

            int line;
            int col;
            // Get the caret position
            NativeMethods.ThrowOnFailure(this.textView.GetCaretPos(out line, out col));

            string text = null;
            if (req.ResultIntellisenseInfo != null) {
                text = req.ResultQuickInfoText;
                if (text == null || !TextSpanHelper.ContainsInclusive(req.ResultQuickInfoSpan, line, col))
                {
                    return; // caret has moved.
                }
            } else {
                return;
            }

            string fullText;
            TextSpan ts = new TextSpan();
            ts.iStartLine = ts.iEndLine = line;
            ts.iStartIndex = ts.iEndIndex = col;
            NativeMethods.ThrowOnFailure(GetFullDataTipText(text, ts, out fullText));

            if (String.IsNullOrEmpty(fullText)) {
                return;
            }

            int iPos, iSpace, iLength;

            // Calculate the stream position
            NativeMethods.ThrowOnFailure(textView.GetNearestPosition(ts.iStartLine, ts.iStartIndex, out iPos, out iSpace));
            iLength = 0;

            // Tear down the method tip window if it's there
            this.source.DismissCompletor();

            // Update the text tip window
            TextTipData textTipData = this.TextTipData;

            textTipData.Update(fullText, iPos, iLength, this.textView);
        }

        /// <summary>This method checks to see if the IVsDebugger is running, and if so, 
        /// calls it to get additional information about the current token and returns a combined result.
        /// You can return an HRESULT here like TipSuccesses2.TIP_S_NODEFAULTTIP.</summary>
        public virtual int GetFullDataTipText(string textValue, TextSpan ts, out string fullTipText) {
            IVsTextLines textLines;
            fullTipText = textValue;

            NativeMethods.ThrowOnFailure(this.textView.GetBuffer(out textLines));

            // Now, check if the debugger is running and has anything to offer
            try {
                Microsoft.VisualStudio.Shell.Interop.IVsDebugger debugger = this.service.GetIVsDebugger();
                if (debugger != null && this.mgr.LanguageService.IsDebugging) {
                    TextSpan[] tsdeb = new TextSpan[1] { new TextSpan() };
                    if (!TextSpanHelper.IsEmpty(ts)) {
                        // While debugging we always want to evaluate the expression user is hovering over
                        // TODO: This is failing, but it's not critical. We will enter into the if block
                        // below and return false.
                        if (NativeMethods.Failed(textView.GetWordExtent(ts.iStartLine, ts.iStartIndex, (uint)WORDEXTFLAGS.WORDEXT_FINDEXPRESSION, tsdeb))) {
                            textView.GetWordExtent(ts.iStartLine, ts.iStartIndex, (uint)WORDEXTFLAGS.WORDEXT_FINDTOKEN, tsdeb);
                        }
                        // If it failed to find something, then it means their is no expression so return S_FALSE
                        if (TextSpanHelper.IsEmpty(tsdeb[0])) {
                            return NativeMethods.S_FALSE;
                        }
                    }
                    string debugTextTip = null;

                    var expressionIsland = this.source.GetExpressionAtPosition(ts.iStartLine, ts.iStartIndex);
                    int hr = debugger.GetDataTipValue(textLines, tsdeb, expressionIsland, out debugTextTip);

                    fullTipText = debugTextTip;
                    if (hr == (int)TipSuccesses2.TIP_S_NODEFAULTTIP) {
                        return hr;
                    }
                    if (!string.IsNullOrEmpty(debugTextTip) && debugTextTip != textValue) {
                        // The debugger in this case returns "=value [type]" which we can
                        // append to the variable name so we get "x=value[type]" as the full tip.
                        int i = debugTextTip.IndexOf('=');
                        if (i >= 0) {
                            string spacer = (i < debugTextTip.Length - 1 && debugTextTip[i + 1] == ' ') ? " " : "";
                            fullTipText = textValue + spacer + debugTextTip.Substring(i);
                        }
                    }
                }
#if LANGTRACE
            } catch (COMException e) {
                Trace.WriteLine("COMException: GetDataTipValue, errorcode=" + e.ErrorCode);
#else
            } catch (COMException) {
#endif
            }
            if (string.IsNullOrEmpty(fullTipText)) {
                fullTipText = textValue;
            }
            return NativeMethods.S_OK;

        }

        /// <summary>Creates the TextTipData object and returns it</summary>
        public virtual TextTipData CreateTextTipData() {
            // create it 
            return new TextTipData(this.service.Site);

        }

        /// <summary>Handles VsCommands.GotoDefn, VsCommands.GotoDecl and VsCommands.GotoRef by
        /// calling OnSyncGoto on the Source object and opening the text editor on the resulting
        /// URL, then scrolling to the resulting span.</summary>
        public virtual void HandleGoto(VsCommands cmd, int line, int col) {

            // Get the tip text at that location. 
            this.gotoCmd = cmd;
            this.source.BeginBackgroundRequest(line, col, new TokenInfo(), BackgroundRequestReason.Goto, this.textView, RequireFreshResults.No, 
                new BackgroundRequestResultHandler(HandleGotoResponse));
        }

        void HandleGotoResponse(BackgroundRequest_DEPRECATED req) {
            if (req == null || req.Source == null || req.Source.IsClosed) return;

            // Make sure caret hasn't moved since we kicked this off.
            TextSpan[] aSpan = new TextSpan[1];
            int line, col;
            NativeMethods.ThrowOnFailure(this.textView.GetCaretPos(out line, out col));
            if (req.Line != line || req.Col != col)
                return; // caret has moved.

            string url = null;
            TextSpan span;
            IntellisenseInfo_DEPRECATED scope = req.ResultIntellisenseInfo;
            if (scope != null && gotoCmd == Microsoft.VisualStudio.VSConstants.VSStd97CmdID.GotoDefn)
            {
                var gotoResult = scope.Goto(textView, line, col);
                if (!gotoResult.Success)
                {
                    ShowErrorMessageBox(gotoResult.ErrorDescription);
                    return;
                }
                url = gotoResult.Url;
                span = gotoResult.Span;
            } else {
                return;
            }
            if (url == null || url.Trim().Length == 0) { // nothing to show
                return;
            }

            // Open the referenced document, and scroll to the given location.
            IVsUIHierarchy hierarchy;
            uint itemID;
            IVsWindowFrame frame;
            IVsTextView view;

            VsShell.OpenDocument(this.service.Site, url, NativeMethods.LOGVIEWID_Code, out hierarchy, out itemID, out frame, out view);
            if (view != null) {
                TextSpanHelper.MakePositive(ref span);
                NativeMethods.ThrowOnFailure(view.SendExplicitFocus());
                NativeMethods.ThrowOnFailure(view.EnsureSpanVisible(span));
                NativeMethods.ThrowOnFailure(view.SetSelection(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex));
            }
        }

        private void ShowErrorMessageBox(string message)
        {
            var uiManager = this.service.Site.GetService(typeof(SOleComponentUIManager)) as IOleComponentUIManager;

            if (uiManager == null)
                NativeHelpers.RaiseComError(NativeMethods.E_FAIL);

            int dialogResult;
            var clsidNull = Guid.Empty;

            var hr = uiManager.ShowMessage(
                (uint)OLEROLE.OLEROLE_MAINCOMPONENT,
                ref clsidNull,
                null,
                message,
                null,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_CRITICAL,
                0,
                out dialogResult);

            NativeMethods.ThrowOnFailure(hr);
        }

        public virtual ExpansionProvider GetExpansionProvider() {
            return this.source.GetExpansionProvider();
        }

        public virtual void ShowContextMenu(int menuId, Guid groupGuid, IOleCommandTarget target) {
            IVsUIShell uiShell = this.service.GetService(typeof(SVsUIShell)) as IVsUIShell;
            if (uiShell != null && !this.service.IsMacroRecordingOn()) { // disable context menu while recording macros.
                System.Drawing.Point pt = System.Windows.Forms.Cursor.Position;
                POINTS[] pnts = new POINTS[1];
                pnts[0].x = (short)pt.X;
                pnts[0].y = (short)pt.Y;
                int hr = uiShell.ShowContextMenu(0, ref groupGuid, menuId, pnts, target);
                if (!NativeMethods.Succeeded(hr)) {
                    Debug.Assert(false, "uiShell.ShowContextMenu returned " + hr);
                }
            }
            uiShell = null;
        }

        // Special View filter command handling.
        public virtual void CommentSelection() {
            if (this.service == null) return;

            TextSpan span = GetSelection();
            span = this.source.CommentSpan(span);

            NativeMethods.ThrowOnFailure(this.textView.SetSelection(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex));
        }

        /// <summary>Returns the current selection, adjusted to become a positive text span</summary>
        public TextSpan GetSelection() {
            //get text range
            TextSpan[] aspan = new TextSpan[1];
            NativeMethods.ThrowOnFailure(this.textView.GetSelectionSpan(aspan));
            if (!TextSpanHelper.IsPositive(aspan[0])) {
                TextSpanHelper.MakePositive(ref aspan[0]);
            }
            return aspan[0];
        }

        public virtual void UncommentSelection() {
            //get text range
            TextSpan span = GetSelection();
            
            span = this.source.UncommentSpan(span);

            NativeMethods.ThrowOnFailure(textView.SetSelection(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex));

        }

        public virtual void ReformatDocument() {
            if (this.CanReformat()) {
                Debug.Assert(this.source != null);
                if (this.source != null) {
                    TextSpan span = this.source.GetDocumentSpan();
                    using (EditArray mgr = new EditArray(this.source, this.TextView, true, SR.GetString(SR.FormatSpan))) {
                        this.source.ReformatSpan(mgr, span);
                        mgr.ApplyEdits();
                    }
                }
            }
        }

        public virtual void ReformatSelection() {
            if (this.CanReformat()) {
                Debug.Assert(this.source != null);
                if (this.source != null) {
                    TextSpan ts = GetSelection();
                    if (TextSpanHelper.IsEmpty(ts)) {
                        // format just this current line.
                        ts.iStartIndex = 0;
                        ts.iEndLine = ts.iStartLine;
                        ts.iEndIndex = this.source.GetLineLength(ts.iStartLine);
                    }
                    using (EditArray mgr = new EditArray(this.source, this.TextView, true, SR.GetString(SR.FormatSpan))) {
                        this.source.ReformatSpan(mgr, ts);
                        mgr.ApplyEdits();
                    }
                }
            }
        }

        /// <summary>This method returns true if the FormatDocument and FormatSelection commands
        /// are to be enabled.  Default returns false if debugging, otherwise it returns
        /// the result for Preferences.EnableFormatSelection.</summary>
        public virtual bool CanReformat() {
            if (this.service == null) return false;
            if (this.service.IsDebugging) return false;
            return this.service.Preferences.EnableFormatSelection;
        }
    }

    /// <summary>This class provides a default implementation of IVsTextTipData for
    /// use in the IVsTextTipWindow for displaying tool tips.</summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class TextTipData : IVsTextTipData {
        IVsTextTipWindow textTipWindow;
        int pos;
        int len;
        string text;
        bool isWindowUp;

        internal TextTipData(IServiceProvider site) {
            if (site == null)
                throw new System.ArgumentNullException("site");

            //this.textView = view;
            // Create our method tip window (through the local registry)
            Type t = typeof(IVsTextTipWindow);
            Guid riid = t.GUID;

            Guid clsid = typeof(VsTextTipWindowClass).GUID;
            Microsoft.VisualStudio.Shell.Package pkg = (Microsoft.VisualStudio.Shell.Package)site.GetService(typeof(Microsoft.VisualStudio.Shell.Package));
            if (pkg == null) {
                throw new NullReferenceException(typeof(Microsoft.VisualStudio.Shell.Package).FullName);
            }
            this.textTipWindow = (IVsTextTipWindow)pkg.CreateInstance(ref clsid, ref riid, t);
            if (this.textTipWindow == null)
                NativeHelpers.RaiseComError(NativeMethods.E_FAIL);
            else
                NativeMethods.ThrowOnFailure(textTipWindow.SetTextTipData(this));
        }

        public void Close(IVsTextView textView) {
            if (this.textTipWindow != null) {
                if (this.isWindowUp)
                    NativeMethods.ThrowOnFailure(textView.UpdateTipWindow(this.textTipWindow, (uint)TipWindowFlags.UTW_DISMISS));

                this.textTipWindow = null;
            }
        }

        public bool IsActive() { return this.isWindowUp; }

        public void Update(string textValue, int pos, int len, IVsTextView textView) {
            if (textView == null) return;

            this.pos = pos;
            this.len = len;
            this.text = textValue;
            if (textValue == null || textValue.Length == 0)
                NativeHelpers.RaiseComError(NativeMethods.E_FAIL);

            int hr = textView.UpdateTipWindow(textTipWindow, (uint)TipWindowFlags.UTW_CONTEXTCHANGED | (uint)TipWindowFlags.UTW_CONTENTCHANGED);
            Debug.Assert(NativeMethods.Succeeded(hr), "UpdateTipWindow");
            this.isWindowUp = true;
        }

        public void CheckCaretPosition(IVsTextView textView)
        {
            if (textView == null) return;

            int line, col, pos, space;

            var hr = textView.GetCaretPos(out line, out col);
            if (NativeMethods.Failed(hr))
                return;

            NativeMethods.ThrowOnFailure(textView.GetNearestPosition(line, col, out pos, out space));
            if (pos < this.pos || pos > this.pos + this.len)
            {
                NativeMethods.ThrowOnFailure(textView.UpdateTipWindow(this.textTipWindow, (uint)TipWindowFlags.UTW_DISMISS));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        public virtual int GetTipText(string[] pbstrText, out int pfFontData) {
            pfFontData = 0; // TODO: Do whatever formatting we might want...
            if (pbstrText == null || pbstrText.Length == 0)
                return NativeMethods.E_INVALIDARG;

            pfFontData = 0; // TODO: Do whatever formatting we might want...
            pbstrText[0] = this.text;
            return NativeMethods.S_OK;
        }

        public virtual int GetTipFontInfo(int iChars, uint[] pdwFontInfo) {
            return NativeMethods.E_NOTIMPL;
        }

        public virtual int GetContextStream(out int piPos, out int piLen) {
            piPos = this.pos;
            piLen = this.len;
            return NativeMethods.S_OK;
        }

        public virtual void OnDismiss() {
            this.isWindowUp = false;
        }

        public virtual void UpdateView() {
        }
    }
}
