// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using Microsoft.Win32;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using System.Diagnostics;
using System.Xml;
using System.Text;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using Microsoft.VisualStudio.FSharp.LanguageService.Resources;

namespace Microsoft.VisualStudio.FSharp.LanguageService {

    internal class DefaultFieldValue {
        private string field;
        private string value;

        internal DefaultFieldValue(string field, string value) {
            this.field = field;
            this.value = value;
        }

        internal string Field {
            get { return this.field; }
        }

        internal string Value {
            get { return this.value; }
        }
    }

    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ExpansionProvider : IDisposable, IVsExpansionClient {
        IVsTextView view;
        ISource source;
        IVsExpansion vsExpansion;
        IVsExpansionSession expansionSession;
        bool expansionActive;
        bool expansionPrepared;
        bool completorActiveDuringPreExec;
        ArrayList fieldDefaults; // CDefaultFieldValues
        string titleToInsert;
        string pathToInsert;

        internal ExpansionProvider(ISource src) {
            if (src == null){
                throw new ArgumentNullException("src");
            }
            this.fieldDefaults = new ArrayList();
            if (src == null)
                throw new System.ArgumentNullException();

            this.source = src;
            this.vsExpansion = null; // do we need a Close() method here?

            // QI for IVsExpansion
            IVsTextLines buffer = src.GetTextLines();
            this.vsExpansion = (IVsExpansion)buffer;
            if (this.vsExpansion == null) {
                throw new ArgumentNullException("(IVsExpansion)src.GetTextLines()");
            }
        }

        ~ExpansionProvider() {
#if LANGTRACE
            Trace.WriteLine("~ExpansionProvider");
#endif
        }

        public virtual void Dispose() {
            EndTemplateEditing(true);
            this.source = null;
            this.vsExpansion = null;
            this.view = null;
            GC.SuppressFinalize(this);
        }

        internal ISource Source {
            get { return this.source; }
        }

        internal IVsTextView TextView {
            get { return this.view; }
        }

        internal IVsExpansion Expansion {
            get { return this.vsExpansion; }
        }

        internal IVsExpansionSession ExpansionSession {
            get { return this.expansionSession; }
        }

        internal virtual bool HandleQueryStatus(ref Guid guidCmdGroup, uint nCmdId, out int hr) {
            // in case there's something to conditinally support later on...
            hr = 0;
            return false;
        }

        internal virtual bool InTemplateEditingMode {
            get {
                return this.expansionActive;
            }
        }

        internal virtual TextSpan GetExpansionSpan() {
            if (this.expansionSession == null){
                throw new System.InvalidOperationException(SR.GetString(SR.NoExpansionSession));
            }
            TextSpan[] pts = new TextSpan[1];
            int hr = this.expansionSession.GetSnippetSpan(pts);
            if (NativeMethods.Succeeded(hr)) {
                return pts[0];
            }
            return new TextSpan();
        }


        internal virtual bool HandlePreExec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (!this.expansionActive || this.expansionSession == null) {
				return false;
            }

            this.completorActiveDuringPreExec = this.IsCompletorActive(this.view);            

            if (guidCmdGroup == typeof(VsCommands2K).GUID) {
                VsCommands2K cmd = (VsCommands2K)nCmdId;
#if TRACE_EXEC
                Trace.WriteLine(String.Format("ExecCommand: {0}", cmd.ToString()));
#endif
                switch (cmd) {
                    case VsCommands2K.CANCEL:
                        if (this.completorActiveDuringPreExec)
                            return false;
                        EndTemplateEditing(true);
                        return true;
                    case VsCommands2K.RETURN:
                        bool leaveCaret = false;
                        int line = 0, col = 0;
                        if (NativeMethods.Succeeded(this.view.GetCaretPos(out line, out col))) {
                            TextSpan span = GetExpansionSpan();
                            if (!TextSpanHelper.ContainsExclusive(span, line, col)) {
                                leaveCaret = true;
                            }
                        }
                        if (this.completorActiveDuringPreExec)
                            return false;
                        if (this.completorActiveDuringPreExec)
                            return false;
                        EndTemplateEditing(leaveCaret);
                        if (leaveCaret)
                            return false;
                        return true;
                    case VsCommands2K.BACKTAB:
                        if (this.completorActiveDuringPreExec)
                            return false;
                        this.expansionSession.GoToPreviousExpansionField();
                        return true;
                    case VsCommands2K.TAB:
                        if (this.completorActiveDuringPreExec)
                            return false;
                        this.expansionSession.GoToNextExpansionField(0); // fCommitIfLast=false
                        return true;
#if TRACE_EXEC
                    case VsCommands2K.TYPECHAR:
                        if (pvaIn != IntPtr.Zero) {
                            Variant v = Variant.ToVariant(pvaIn);
                            char ch = v.ToChar();
                            Trace.WriteLine(String.Format("TYPECHAR: {0}, '{1}', {2}", cmd.ToString(), ch.ToString(), (int)ch));
                        }
                        return true;
#endif
                }
            }
            return false;
        }

        internal virtual bool HandlePostExec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, bool commit, IntPtr pvaIn, IntPtr pvaOut) {
            if (guidCmdGroup == typeof(VsCommands2K).GUID) {
                VsCommands2K cmd = (VsCommands2K)nCmdId;
                switch (cmd) {
                    case VsCommands2K.RETURN:
                        if (this.completorActiveDuringPreExec && commit) {
                            // if the completor was active during the pre-exec we want to let it handle the command first
                            // so we didn't deal with this in pre-exec. If we now get the command, we want to end
                            // the editing of the expansion. We also return that we handled the command so auto-indenting doesn't happen
                            EndTemplateEditing(false);
                            this.completorActiveDuringPreExec = false;
                            return true;
                        }
                        break;
                }
            }
            this.completorActiveDuringPreExec = false;
            return false;
        }

        internal virtual bool DisplayExpansionBrowser(IVsTextView view, string prompt, string[] types, bool includeNullType, string[] kinds, bool includeNullKind) {
            if (this.expansionActive) this.EndTemplateEditing(true);

            if (this.source.IsCompletorActive) {
                this.source.DismissCompletor();
            }

            this.view = view;
            IServiceProvider site = this.source.LanguageService.Site;
            IVsTextManager2 textmgr = site.GetService(typeof(SVsTextManager)) as IVsTextManager2;
            if (textmgr == null) return false;
            IVsExpansionManager exmgr;
            textmgr.GetExpansionManager(out exmgr);
            Guid languageSID = this.source.LanguageService.GetLanguageServiceGuid();
            int hr = 0;
            if (exmgr != null) {
                hr = exmgr.InvokeInsertionUI(view, // pView
                    this, // pClient
                    languageSID, // guidLang
                    types, // bstrTypes
                    (types == null) ? 0 : types.Length, // iCountTypes
                    includeNullType ? 1 : 0,  // fIncludeNULLType
                    kinds, // bstrKinds
                    (kinds == null) ? 0 : kinds.Length, // iCountKinds
                    includeNullKind ? 1 : 0, // fIncludeNULLKind
                    prompt, // bstrPrefixText
                    ">" //bstrCompletionChar
                    );
                if (NativeMethods.Succeeded(hr)) {
                    return true;
                }
            }
            return false;
        }

        internal virtual bool InsertSpecificExpansion(IVsTextView view, XmlElement snippet, TextSpan pos, string relativePath) {
            if (this.expansionActive) this.EndTemplateEditing(true);

            if (this.source.IsCompletorActive) {
                this.source.DismissCompletor();
            }

            this.view = view;
            MSXML.IXMLDOMDocument doc = (MSXML.IXMLDOMDocument)new MSXML.DOMDocumentClass();
            if (!doc.loadXML(snippet.OuterXml)) {
                throw new ArgumentException(doc.parseError.reason);
            }
            Guid guidLanguage = this.source.LanguageService.GetLanguageServiceGuid();

            int hr = this.vsExpansion.InsertSpecificExpansion(doc, pos, this, guidLanguage, relativePath, out this.expansionSession);
            if (hr != NativeMethods.S_OK || this.expansionSession == null) {
                this.EndTemplateEditing(true);
            } else {
                this.expansionActive = true;
                return true;
            }
            return false;
        }

        bool IsCompletorActive(IVsTextView view){
            if (this.source.IsCompletorActive)
                return true;

            IVsTextViewEx viewex = view as IVsTextViewEx;
            if (viewex  != null) {
                return viewex.IsCompletorWindowActive() == Microsoft.VisualStudio.VSConstants.S_OK;
            }

            return false;
        }

        internal virtual bool InsertNamedExpansion(IVsTextView view, string title, string path, TextSpan pos, bool showDisambiguationUI) {

            if (this.source.IsCompletorActive) {
                this.source.DismissCompletor();
            }

            this.view = view;
            if (this.expansionActive) this.EndTemplateEditing(true);

            Guid guidLanguage = this.source.LanguageService.GetLanguageServiceGuid();

            int hr = this.vsExpansion.InsertNamedExpansion(title, path, pos, this, guidLanguage, showDisambiguationUI ? 1 : 0, out this.expansionSession);

            if (hr != NativeMethods.S_OK || this.expansionSession == null) {
                this.EndTemplateEditing(true);
                return false;
            } else if (hr == NativeMethods.S_OK) {
                this.expansionActive = true;
                return true;
            }
            return false;
        }

        /// <summary>Returns S_OK if match found, S_FALSE if expansion UI is shown, and error otherwise</summary>
        internal virtual int FindExpansionByShortcut(IVsTextView view, string shortcut, TextSpan span, bool showDisambiguationUI, out string title, out string path) {
            if (this.expansionActive) this.EndTemplateEditing(true);
            this.view = view;
            title = path = null;

            LanguageService_DEPRECATED svc = this.source.LanguageService;
            IVsExpansionManager mgr = svc.Site.GetService(typeof(SVsExpansionManager)) as IVsExpansionManager;
            if (mgr == null) return NativeMethods.E_FAIL ;
            Guid guidLanguage = svc.GetLanguageServiceGuid();

            TextSpan[] pts = new TextSpan[1];
            pts[0] = span;
            int hr = mgr.GetExpansionByShortcut(this, guidLanguage, shortcut, this.TextView, pts, showDisambiguationUI ? 1 : 0, out path, out title);
            return hr;
        }

        public virtual IVsExpansionFunction GetExpansionFunction(XmlElement xmlFunctionNode, string fieldName) {
            string functionName = null;
            ArrayList rgFuncParams = new ArrayList();

            // first off, get the function string from the node
            string function = xmlFunctionNode.InnerText;

            if (function == null || function.Length == 0)
                return null;

            bool inIdent = false;
            bool inParams = false;
            int token = 0;

            // initialize the vars needed for our super-complex function parser :-)
            for (int i = 0, n = function.Length; i < n; i++) {
                char ch = function[i];

                // ignore and skip whitespace
                if (!Char.IsWhiteSpace(ch)) {
                    switch (ch) {
                        case ',':
                            if (!inIdent || !inParams)
                                i = n; // terminate loop
                            else {
                                // we've hit a comma, so end this param and move on...
                                string name = function.Substring(token, i - token);
                                rgFuncParams.Add(name);
                                inIdent = false;
                            }
                            break;
                        case '(':
                            if (!inIdent || inParams)
                                i = n; // terminate loop
                            else {
                                // we've hit the (, so we know the token before this is the name of the function
                                functionName = function.Substring(token, i - token);
                                inIdent = false;
                                inParams = true;
                            }
                            break;
                        case ')':
                            if (!inParams)
                                i = n; // terminate loop
                            else {
                                if (inIdent) {
                                    // save last param and stop
                                    string name = function.Substring(token, i - token);
                                    rgFuncParams.Add(name);
                                    inIdent = false;
                                }
                                i = n; // terminate loop
                            }
                            break;
                        default:
                            if (!inIdent) {
                                inIdent = true;
                                token = i;
                            }
                            break;
                    }
                }
            }

            if (functionName != null && functionName.Length > 0) {
                ExpansionFunction func = this.source.LanguageService.CreateExpansionFunction(this, functionName);
                if (func != null) {
                    func.FieldName = fieldName;
                    func.Arguments = (string[])rgFuncParams.ToArray(typeof(string));
                    return func;
                }
            }
            return null;
        }

        internal virtual void PrepareTemplate(string title, string path) {            
            if (title == null)
                throw new System.ArgumentNullException("title");

            // stash the title and path for when we actually insert the template
            this.titleToInsert = title;
            this.pathToInsert = path;
            this.expansionPrepared = true;
        }

        void SetFieldDefault(string field, string value) {
            if (!this.expansionPrepared) {
                throw new System.InvalidOperationException(SR.GetString(SR.TemplateNotPrepared));
            }
            if (field == null) throw new System.ArgumentNullException("field");
            if (value == null) throw new System.ArgumentNullException("value");

            // we have an expansion "prepared" to insert, so we can now save this
            // field default to set when the expansion is actually inserted
            this.fieldDefaults.Add(new DefaultFieldValue(field, value));
        }

        internal virtual void BeginTemplateEditing(int line, int col) {
            if (!this.expansionPrepared) {
                throw new System.InvalidOperationException(SR.GetString(SR.TemplateNotPrepared));
            }

            TextSpan tsInsert = new TextSpan();
            tsInsert.iStartLine = tsInsert.iEndLine = line;
            tsInsert.iStartIndex = tsInsert.iEndIndex = col;

            Guid languageSID = this.source.LanguageService.GetType().GUID;

            int hr = this.vsExpansion.InsertNamedExpansion(this.titleToInsert,
                                                            this.pathToInsert,
                                                            tsInsert,
                                                            (IVsExpansionClient)this,
                                                            languageSID,
                                                            0, // fShowDisambiguationUI,
                out this.expansionSession);

            if (hr != NativeMethods.S_OK) {
                this.EndTemplateEditing(true);
            }
            this.pathToInsert = null;
            this.titleToInsert = null;
        }

        internal virtual void EndTemplateEditing(bool leaveCaret) {
            if (!this.expansionActive || this.expansionSession == null) {
                this.expansionActive = false;
                return;
            }

            this.expansionSession.EndCurrentExpansion(leaveCaret ? 1 : 0); // fLeaveCaret=true
            this.expansionSession = null;
            this.expansionActive = false;
        }

        internal virtual bool GetFieldSpan(string field, out TextSpan pts) {
            if (this.expansionSession == null) {
                throw new System.InvalidOperationException(SR.GetString(SR.NoExpansionSession));
            }
            if (this.expansionSession != null) {
                TextSpan[] apt = new TextSpan[1];
                this.expansionSession.GetFieldSpan(field, apt);
                pts = apt[0];
                return true;
            } else {
                pts = new TextSpan();
                return false;
            }
        }

        internal virtual bool GetFieldValue(string field, out string value) {
            if (this.expansionSession == null) {
                throw new System.InvalidOperationException(SR.GetString(SR.NoExpansionSession));
            }
            if (this.expansionSession != null) {
                this.expansionSession.GetFieldValue(field, out value);
            } else {
                value = null;
            }
            return value != null;
        }

        public int EndExpansion() {
            this.expansionActive = false;
            this.expansionSession = null;
            return NativeMethods.S_OK;
        }

        public virtual int FormatSpan(IVsTextLines buffer, TextSpan[] ts) {
            if (this.source.GetTextLines() != buffer) {
                throw new System.ArgumentException(SR.GetString(SR.UnknownBuffer), "buffer");
            }
            int rc = NativeMethods.E_NOTIMPL;
            if (ts != null) {
                for (int i = 0, n = ts.Length; i < n; i++) {
                    if (this.source.LanguageService.Preferences.EnableFormatSelection) {
                        TextSpan span = ts[i];
                        // We should not merge edits in this case because it might clobber the
                        // $varname$ spans which are markers for yellow boxes.
                        using (EditArray edits = new EditArray(this.source, this.view, false, SR.GetString(SR.FormatSpan))) {
                            this.source.ReformatSpan(edits, span);
                            edits.ApplyEdits();
                        }
                        rc = NativeMethods.S_OK;
                    }
                }
            }
            return rc;
        }

        public virtual int IsValidKind(IVsTextLines buffer, TextSpan[] ts, string bstrKind, out int /*BOOL*/ fIsValid)
        {
            fIsValid = 0;
            if (this.source.GetTextLines() != buffer)
            {
                throw new System.ArgumentException(SR.GetString(SR.UnknownBuffer), "buffer");
            }

            fIsValid = 1;
            return NativeMethods.S_OK;
        }

        public virtual int IsValidType(IVsTextLines buffer, TextSpan[] ts, string[] rgTypes, int iCountTypes, out int /*BOOL*/ fIsValid)
        {
            fIsValid = 0;
            if (this.source.GetTextLines() != buffer) {
                throw new System.ArgumentException(SR.GetString(SR.UnknownBuffer), "buffer");
            }
            
            fIsValid = 1;
            return NativeMethods.S_OK;
        }

        public virtual int OnItemChosen(string pszTitle, string pszPath) {
            TextSpan ts;
            view.GetCaretPos(out ts.iStartLine, out ts.iStartIndex);
            ts.iEndLine = ts.iStartLine;
            ts.iEndIndex = ts.iStartIndex;

            if (this.expansionSession != null) { // previous session should have been ended by now!
                EndTemplateEditing(true);
            }

            Guid languageSID = this.source.LanguageService.GetType().GUID;

            // insert the expansion

            int hr = this.vsExpansion.InsertNamedExpansion(pszTitle,
                pszPath,
                ts,
                (IVsExpansionClient)this,
                languageSID,
                0, // fShowDisambiguationUI, (FALSE)
                out this.expansionSession);

            return hr;
        }

        public virtual int PositionCaretForEditing(IVsTextLines pBuffer, TextSpan[] ts) {
            // NOP
            return NativeMethods.S_OK;
        }

        public virtual int OnAfterInsertion(IVsExpansionSession session) {
            return NativeMethods.S_OK;
        }

        public virtual int OnBeforeInsertion(IVsExpansionSession session) {
            if (session == null)
                return NativeMethods.E_UNEXPECTED;

            this.expansionPrepared = false;
            this.expansionActive = true;

            // stash the expansion session pointer while the expansion is active
            if (this.expansionSession == null) {
                this.expansionSession = session;
            } else {
                // these better be the same!
                Debug.Assert(this.expansionSession == session);
            }

            // now set any field defaults that we have.
            foreach (DefaultFieldValue dv in this.fieldDefaults) {
                this.expansionSession.SetFieldDefault(dv.Field, dv.Value);
            }
            this.fieldDefaults.Clear();
            return NativeMethods.S_OK;
        }

        public virtual int GetExpansionFunction(MSXML.IXMLDOMNode xmlFunctionNode, string fieldName, out IVsExpansionFunction func) {

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            using(StringReader stream = new StringReader(xmlFunctionNode.xml))
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
            {
                doc.Load(reader);
                func = GetExpansionFunction(doc.DocumentElement, fieldName);
            }
            return NativeMethods.S_OK;
        }
    }


    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class ExpansionFunction : IVsExpansionFunction {
        ExpansionProvider provider;
        string fieldName;
        string[] args;
        string[] list;

        /// <summary>You must construct this object with an ExpansionProvider</summary>
        private ExpansionFunction() {
        }

        internal ExpansionFunction(ExpansionProvider provider) {
            this.provider = provider;
        }

        public ExpansionProvider ExpansionProvider {
            get { return this.provider; }
        }

        public string[] Arguments {
            get { return this.args; }
            set { this.args = value; }
        }

        public string FieldName {
            get { return this.fieldName; }
            set { this.fieldName = value; }
        }

        public abstract string GetCurrentValue();

        public virtual string GetDefaultValue() {
            // This must call GetCurrentValue sincs during initialization of the snippet
            // VS will call GetDefaultValue and not GetCurrentValue.
            return GetCurrentValue();
        }

        /// <summary>Override this method if you want intellisense drop support on a list of possible values.</summary>
        public virtual string[] GetIntellisenseList() {
            return null;
        }

        /// <summary>
        /// Gets the value of the specified argument, resolving any fields referenced in the argument.
        /// In the substitution, "$$" is replaced with "$" and any floating '$' signs are left unchanged,
        /// for example "$US 23.45" is returned as is.  Only if the two dollar signs enclose a string of
        /// letters or digits is this considered a field name (e.g. "$foo123$").  If the field is not found
        /// then the unresolved string "$foo" is returned.
        /// </summary>
        public string GetArgument(int index) {
            if (args == null || args.Length == 0 || index > args.Length) return null;
            string arg = args[index];
            if (arg == null) return null;
            int i = arg.IndexOf('$');
            if (i >= 0) {
                StringBuilder sb = new StringBuilder();
                int len = arg.Length;
                int start = 0;

                while (i >= 0 && i + 1 < len) {
                    sb.Append(arg.Substring(start, i - start));
                    start = i;
                    i++;
                    if (arg[i] == '$') {
                        sb.Append('$');
                        start = i + 1; // $$ is resolved to $.
                    } else {
                        // parse name of variable.
                        int j = i;
                        for (; j < len; j++) {
                            if (!Char.IsLetterOrDigit(arg[j]))
                                break;
                        }
                        if (j == len) {
                            // terminating '$' not found.
                            sb.Append('$');
                            start = i;
                            break;
                        } else if (arg[j] == '$') {
                            string name = arg.Substring(i, j - i);
                            string value;
                            if (GetFieldValue(name, out value)) {
                                sb.Append(value);
                            } else {
                                // just return the unresolved variable.
                                sb.Append('$');
                                sb.Append(name);
                                sb.Append('$');
                            }
                            start = j + 1;
                        } else {
                            // invalid syntax, e.g. "$US 23.45" or some such thing                            
                            sb.Append('$');
                            sb.Append(arg.Substring(i, j - i));
                            start = j;
                        }
                    }
                    i = arg.IndexOf('$', start);
                }
                if (start < len) {
                    sb.Append(arg.Substring(start, len - start));
                }
                arg = sb.ToString();
            }
            // remove quotes around string literals.
            if (arg.Length > 2 && arg[0] == '"' && arg[arg.Length - 1] == '"') {
                arg = arg.Substring(1, arg.Length - 2);
            } else if (arg.Length > 2 && arg[0] == '\'' && arg[arg.Length - 1] == '\'') {
                arg = arg.Substring(1, arg.Length - 2);
            }
            return arg;
        }

        public bool GetFieldValue(string name, out string value) {
            value = null;
            if (this.provider != null && this.provider.ExpansionSession != null) {
                int hr = this.provider.ExpansionSession.GetFieldValue(name, out value);
                return NativeMethods.Succeeded(hr);
            }
            return false;
        }

        public TextSpan GetSelection() {
            TextSpan result = new TextSpan();
            ExpansionProvider provider = this.ExpansionProvider;
            if (provider != null && provider.TextView != null) {
                NativeMethods.ThrowOnFailure(provider.TextView.GetSelection(out result.iStartLine,
                    out result.iStartIndex, out result.iEndLine, out result.iEndIndex));
            }
            return result;
        }

        public virtual int FieldChanged(string bstrField, out int fRequeryValue) {
            // Returns true if we care about this field changing.
            // We care if the field changes if one of the arguments refers to it.
            if (this.args != null) {
                string var = "$" + bstrField + "$";
                foreach (string arg in this.args) {
                    if (arg == var) {
                        fRequeryValue = 1; // we care!
                        return NativeMethods.S_OK;
                    }
                }
            }
            fRequeryValue = 0;
            return NativeMethods.S_OK;
        }

        public int GetCurrentValue(out string bstrValue, out int hasDefaultValue) {
            try {
                bstrValue = this.GetCurrentValue();
            } catch {
                bstrValue = String.Empty;
            }
            hasDefaultValue = (bstrValue == null) ? 0 : 1;
            return NativeMethods.S_OK;
        }

        public int GetDefaultValue(out string bstrValue, out int hasCurrentValue) {
            try {
                bstrValue = this.GetDefaultValue();
            } catch {
                bstrValue = String.Empty;
            }
            hasCurrentValue = (bstrValue == null) ? 0 : 1;
            return NativeMethods.S_OK;
        }

        public virtual int GetFunctionType(out uint pFuncType) {
            if (this.list == null) {
                this.list = this.GetIntellisenseList();
            }
            pFuncType = (this.list == null) ? (uint)_ExpansionFunctionType.eft_Value : (uint)_ExpansionFunctionType.eft_List;
            return NativeMethods.S_OK;
        }

        public virtual int GetListCount(out int iListCount) {
            if (this.list == null) {
                this.list = this.GetIntellisenseList();
            }
            if (this.list != null) {
                iListCount = this.list.Length;
            } else {
                iListCount = 0;
            }
            return NativeMethods.S_OK;
        }

        public virtual int GetListText(int iIndex, out string ppszText) {
            if (this.list == null) {
                this.list = this.GetIntellisenseList();
            }
            if (this.list != null) {
                ppszText = this.list[iIndex];
            } else {
                ppszText = null;
            }
            return NativeMethods.S_OK;
        }

        public virtual int ReleaseFunction() {
            this.provider = null;
            return NativeMethods.S_OK;
        }
    }

    // todo: for some reason VsExpansionManager is wrong.
    [Guid("4970C2BC-AF33-4a73-A34F-18B0584C40E4")]
    internal class SVsExpansionManager {
    }

}
