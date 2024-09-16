// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Ole = Microsoft.VisualStudio.OLE.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using Microsoft.VisualStudio.FSharp.LanguageService.Resources;

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    /// <summary>
    /// CodeWindowManager provides a default implementation of the VSIP interface IVsCodeWindowManager
    /// and manages the LanguageService, Source, ViewFilter, and DocumentProperties objects associated
    /// with the given IVsCodeWindow.  It calls CreateViewFilter on your LanguageService for each new
    /// IVsTextView created by Visual Studio and installs the resulting filter into the command chain.
    /// You do not have to override this method, since a default view filter will be created.
#if DOCUMENT_PROPERTIES
    /// If your LanguageService returns an object from CreateDocumentProperties then you will have
    /// properties in the Properties Window associated with your source files.
#endif
    /// The CodeWindowManager also provides support for optional drop down combos in the IVsDropdownBar for 
    /// listing types and members by installing the TypeAndMemberDropdownBars object returned from your 
    /// LanguageService CreateDropDownHelper method.  The default return from CreateDropDownHelper is null, 
    /// which results in no drop down combos.
    /// </summary>
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class CodeWindowManager : IVsCodeWindowManager {
        TypeAndMemberDropdownBars dropDownHelper;
        IVsCodeWindow codeWindow;
        ArrayList viewFilters;
        LanguageService_DEPRECATED service;
        ISource source;
#if DOCUMENT_PROPERTIES
        DocumentProperties properties;
#endif

        /// <summary>
        /// The CodeWindowManager is constructed by the base LanguageService class when VS calls
        /// the IVsLanguageInfo.GetCodeWindowManager method.  You can override CreateCodeWindowManager
        /// on your LanguageService if you want to plug in a different CodeWindowManager.
        /// </summary>
        internal CodeWindowManager(LanguageService_DEPRECATED service, IVsCodeWindow codeWindow, ISource source) {
            this.service = service;
            this.codeWindow = codeWindow;
            this.viewFilters = new ArrayList();
            this.source = source;
#if DOCUMENT_PROPERTIES
            this.properties = service.CreateDocumentProperties(this);
#endif
        }

        ~CodeWindowManager() {
#if	LANGTRACE
            Trace.WriteLine("~CodeWindowManager");
#endif
        }

        /// <summary>Closes all view filters, and the document properties window</summary>
        internal void Close() {
#if	LANGTRACE
            Trace.WriteLine("CodeWindowManager::Close");
#endif
#if DOCUMENT_PROPERTIES
            if (this.properties != null) this.properties.Close();
#endif
            CloseFilters();
            this.viewFilters = null;
#if DOCUMENT_PROPERTIES
            properties = null;
#endif
            service = null;
            source = null;
            this.codeWindow = null;
        }

        void CloseFilters() {
            if (this.viewFilters != null) {
                foreach (ViewFilter f in this.viewFilters) {
                    f.Close();
                }
                this.viewFilters.Clear();
            }
        }

        /// <summary>Returns the LanguageService object that created this code window manager</summary>
        internal LanguageService_DEPRECATED LanguageService {
            get { return this.service; }
        }
        /// <summary>returns the Source object associated with the IVsTextLines buffer for this code window</summary>
        internal ISource Source {
            get { return this.source; }
        }
        /// <summary>
        /// Returns the ViewFilter for the given view or null if no matching filter is found.
        /// </summary>
        internal ViewFilter GetFilter(IVsTextView view) {
            if (this.viewFilters != null) {
                foreach (ViewFilter f in this.viewFilters) {
                    if (f.TextView == view)
                        return f;
                }
            }
            return null;
        }

#if DOCUMENT_PROPERTIES
        /// <summary>Returns the DocumentProperties, if any.  You can update this property if you want to 
        /// change the document properties on the fly.</summary>
        internal DocumentProperties Properties {
            get { return this.properties; }
            set {
                if (this.properties != value) {
                    if (this.properties != null) this.properties.Close();
                    this.properties = value;
                    if (value != null) value.Refresh();
                }
            }
        }
#endif
        /// <summary>Return the optional TypeAndMemberDropdownBars object for the drop down combos</summary>
        internal TypeAndMemberDropdownBars DropDownHelper {
            get { return this.dropDownHelper; }
        }
        /// <summary>Return the IVsCodeWindow associated with this code window manager.</summary>
        internal IVsCodeWindow CodeWindow {
            get { return this.codeWindow; }
        }
      
        /// <summary>Install the optional TypeAndMemberDropdownBars, and primary and secondary view filters</summary>
        public virtual int AddAdornments() {
            int hr = 0;
            this.service.AddCodeWindowManager(this);

            this.source.Open();

            IVsTextView textView;
            NativeMethods.ThrowOnFailure(this.codeWindow.GetPrimaryView(out textView));

            this.dropDownHelper = this.service.CreateDropDownHelper(textView);
            if (this.dropDownHelper != null) {
                IVsDropdownBar pBar;
                IVsDropdownBarManager dbm = (IVsDropdownBarManager)this.codeWindow;
                int rc = dbm.GetDropdownBar(out pBar);
                if (rc == 0 && pBar != null)
                    NativeMethods.ThrowOnFailure(dbm.RemoveDropdownBar());
                //this.dropDownHelper = new TypeAndMemberDropdownBars(this.service);
                this.dropDownHelper.SynchronizeDropdowns(textView, 0, 0);
                NativeMethods.ThrowOnFailure(dbm.AddDropdownBar(2, this.dropDownHelper));
            }
            // attach view filter to primary view.
            if (textView != null)
                this.OnNewView(textView); // always returns S_OK.

            // attach view filter to secondary view.
            textView = null;
            hr = this.codeWindow.GetSecondaryView(out textView);
            if (hr == NativeMethods.S_OK && textView != null)
                this.OnNewView(textView); // always returns S_OK.

            return NativeMethods.S_OK;
        }

        /// <summary>Remove drop down combos, view filters, and notify the LanguageService that the Source and
        /// CodeWindowManager is now closed</summary>
        public virtual int RemoveAdornments() {
            try {
                if (this.dropDownHelper != null) {
                    IVsDropdownBarManager dbm = (IVsDropdownBarManager)this.codeWindow;
                    NativeMethods.ThrowOnFailure(dbm.RemoveDropdownBar());
                    this.dropDownHelper.Done();
                    this.dropDownHelper = null;
                }
            } finally {
                CloseFilters();

                if (this.source != null && this.source.Close()) {
                    this.service.OnCloseSource(this.source);
                    this.source.Dispose();
                }
                this.source = null;

                service.RemoveCodeWindowManager(this);
                this.codeWindow = null;
                this.Close();
            }
            return NativeMethods.S_OK;
        }

        /// <summary>Install a new view filter for the given view. This method calls your
        /// CreateViewFilter method.</summary>
        public virtual int OnNewView(IVsTextView newView) {
            ViewFilter filter = this.service.CreateViewFilter(this, newView);
            if (filter != null) this.viewFilters.Add(filter);
            return NativeMethods.S_OK;
        }

        public virtual void OnKillFocus(IVsTextView textView) {
        }

        /// <summary>Refresh the document properties</summary>
        public virtual void OnSetFocus(IVsTextView textView) {
#if DOCUMENT_PROPERTIES
            if (this.properties != null) {
                this.properties.Refresh();
            }
#endif
        }

    }



    /// <summary>
    /// Represents the two drop down bars on the top of a text editor window that allow 
    /// types and type members to be selected by name.
    /// </summary>
    internal abstract class TypeAndMemberDropdownBars : IVsDropdownBarClient {
        /// <summary>The language service object that created this object and calls its SynchronizeDropdowns method</summary>
        private LanguageService_DEPRECATED languageService;

        /// <summary>The corresponding VS object that represents the two drop down bars. The VS object uses call backs to pull information from
        /// this object and makes itself known to this object by calling SetDropdownBar</summary>
        private IVsDropdownBar dropDownBar;

        /// <summary>The icons that prefix the type names and member signatures</summary>
        private ImageList imageList;

        /// <summary>The current text editor window</summary>
        private IVsTextView textView;

        /// <summary>The list of types that appear in the type drop down list.</summary>
        private ArrayList dropDownTypes;

        /// <summary>The list of types that appear in the member drop down list. </summary>
        private ArrayList dropDownMembers;

        private int selectedType = -1;
        private int selectedMember = -1;

        const int DropClasses = 0;
        const int DropMethods = 1;

        protected TypeAndMemberDropdownBars(LanguageService_DEPRECATED languageService) {
            this.languageService = languageService;
            this.dropDownTypes = new ArrayList();
            this.dropDownMembers = new ArrayList();
        }

        public void Done() { //TODO: use IDisposable pattern
            if (this.imageList != null) {
                imageList.Dispose();
                imageList = null;
            }
        }


        internal void SynchronizeDropdowns(IVsTextView textView, int line, int col) {
            if (this.dropDownBar == null) return;
            this.textView = textView;
            if (OnSynchronizeDropdowns(languageService, textView, line, col, this.dropDownTypes, this.dropDownMembers, ref this.selectedType, ref this.selectedMember)) {
                NativeMethods.ThrowOnFailure(this.dropDownBar.RefreshCombo(TypeAndMemberDropdownBars.DropClasses, this.selectedType));
                NativeMethods.ThrowOnFailure(this.dropDownBar.RefreshCombo(TypeAndMemberDropdownBars.DropMethods, this.selectedMember));
            }
        }



        /// <summary>
        /// This method is called to update the drop down bars to match the current contents of the text editor window. 
        /// It is called during OnIdle when the caret position changes.  You can provide new drop down members here.
        /// It is up to you to sort the ArrayLists if you want them sorted in any particular order.
        /// </summary>
        /// <param name="languageService">The language service</param>
        /// <param name="textView">The editor window</param>
        /// <param name="line">The line on which the cursor is now positioned</param>
        /// <param name="col">The column on which the cursor is now position</param>
        /// <param name="dropDownTypes">The current list of types (you can update this)</param>
        /// <param name="dropDownMembers">The current list of members (you can update this)</param>
        /// <param name="selectedType">The selected type (you can update this)</param>
        /// <param name="selectedMember">The selected member (you can update this)</param>
        /// <returns>true if something was updated</returns>
        public abstract bool OnSynchronizeDropdowns(LanguageService_DEPRECATED languageService, IVsTextView textView, int line, int col, ArrayList dropDownTypes, ArrayList dropDownMembers, ref int selectedType, ref int selectedMember);


        // IVsDropdownBarClient methods
        public virtual int GetComboAttributes(int combo, out uint entries, out uint entryType, out IntPtr iList) {
            entries = 0;
            entryType = 0;
            if (combo == TypeAndMemberDropdownBars.DropClasses && this.dropDownTypes != null)
                entries = (uint)this.dropDownTypes.Count;
            else if (this.dropDownMembers != null)
                entries = (uint)this.dropDownMembers.Count;
            entryType = (uint)(DropDownItemType.HasText | DropDownItemType.HasFontAttribute | DropDownItemType.HasImage);
            if (this.imageList == null)
                this.imageList = this.languageService.GetImageList();
            iList = this.imageList.Handle;
            return NativeMethods.S_OK;
        }

        private enum DropDownItemType {
            HasText = 1,
            HasFontAttribute = 2,
            HasImage = 4
        }

        public virtual int GetComboTipText(int combo, out string text) {
            if (combo == TypeAndMemberDropdownBars.DropClasses)
                text = SR.GetString(SR.ComboTypesTip);
            else
                text = SR.GetString(SR.ComboMembersTip);
            return NativeMethods.S_OK;
        }

        public virtual int GetEntryAttributes(int combo, int entry, out uint fontAttrs) {
            fontAttrs = (uint)DROPDOWNFONTATTR.FONTATTR_PLAIN;
            DropDownMember member = GetMember(combo, entry);
            if (!Object.ReferenceEquals(member,null)) {
                fontAttrs = (uint)member.FontAttr;
            }
            return NativeMethods.S_OK;
        }

        public virtual int GetEntryImage(int combo, int entry, out int imgIndex) {
            // this happens during drawing and has to be fast 
            imgIndex = -1;
            DropDownMember member = GetMember(combo, entry);
            if (!Object.ReferenceEquals(member,null)) {
                imgIndex = member.Glyph;
            }
            return NativeMethods.S_OK;
        }

        public virtual int GetEntryText(int combo, int entry, out string text) {
            text = null;
            DropDownMember member = GetMember(combo, entry);
            if (!Object.ReferenceEquals(member,null)) {
                text = member.Label;
            }
            return NativeMethods.S_OK;
        }

        public virtual int OnComboGetFocus(int combo) {
            return NativeMethods.S_OK;
        }

        public DropDownMember GetMember(int combo, int entry) {
            if (combo == TypeAndMemberDropdownBars.DropClasses) {
                if (this.dropDownTypes != null && entry >= 0 && entry < this.dropDownTypes.Count)
                    return (DropDownMember)this.dropDownTypes[entry];
            } else {
                if (this.dropDownMembers != null && entry >= 0 && entry < this.dropDownMembers.Count)
                    return (DropDownMember)this.dropDownMembers[entry];
            }
            return null;
        }

        public virtual int OnItemChosen(int combo, int entry) {
            DropDownMember member = GetMember(combo, entry);
            if (!Object.ReferenceEquals(member,null)) {
                if (this.textView != null) {
                    int line = member.Span.iStartLine;
                    int col = member.Span.iStartIndex;
                    try {
                        // Here we don't want to throw or to check the return value.
                        textView.CenterLines(line, 16);
                    } catch (COMException) { }
                    NativeMethods.ThrowOnFailure(this.textView.SetCaretPos(line, col));
                    NativeMethods.SetFocus(this.textView.GetWindowHandle());
                    this.SynchronizeDropdowns(this.textView, line, col);
                }
            }
            return NativeMethods.S_OK;
        }

        [DllImport("user32.dll")]
        static extern void SetFocus(IntPtr hwnd);

        public int OnItemSelected(int combo, int index) {
            //nop
            return NativeMethods.S_OK;
        }

        public int SetDropdownBar(IVsDropdownBar bar) {
            this.dropDownBar = bar;
            return NativeMethods.S_OK;
        }
    }

    internal class DropDownMember : IComparable {

        private string label;
        private TextSpan span;
        private int glyph;
        private DROPDOWNFONTATTR fontAttr;

        public string Label {
            get {
                return this.label;
            }
            set {
                this.label = value;
            }
        }

        public TextSpan Span {
            get {
                return this.span;
            }
            set {
                this.span = value;
            }
        }
        public int Glyph {
            get {
                return this.glyph;
            }
            set {
                this.glyph = value;
            }
        }
        public DROPDOWNFONTATTR FontAttr {
            get {
                return this.fontAttr;
            }
            set {
                this.fontAttr = value;
            }
        }

        public DropDownMember(string label, TextSpan span, int glyph, DROPDOWNFONTATTR fontAttribute) {
            if (label == null) {
                throw new ArgumentNullException("label");
            }
            this.Label = label;
            this.Span = span;
            this.Glyph = glyph;
            this.FontAttr = fontAttribute;
        }

        public int CompareTo(object obj) {
            // if this overload is used then it assumes a case-sensitive current culture comparison
            // which allows for case-sensitive languages to work
            return CompareTo(obj, StringComparison.CurrentCulture);
        }

        public int CompareTo(object obj, StringComparison stringComparison)
        {
            if (obj is DropDownMember)
            {
                return String.Compare(this.Label, ((DropDownMember)obj).Label, stringComparison);
            }
            return -1;
        }

        // Omitting Equals violates FxCop rule: IComparableImplementationsOverrideEquals.
        public override bool Equals(Object obj) {
            if (!(obj is DropDownMember))
                return false;
            return (this.CompareTo(obj, StringComparison.CurrentCulture) == 0);
        }

        // Omitting getHashCode violates FxCop rule: EqualsOverridesRequireGetHashCodeOverride.
        public override int GetHashCode() {
            return this.Label.GetHashCode();
        }


        // Omitting any of the following operator overloads
        // violates FxCop rule: IComparableImplementationsOverrideOperators.
        public static bool operator ==(DropDownMember m1, DropDownMember m2) {
            return m1.Equals(m2);
        }
        public static bool operator !=(DropDownMember m1, DropDownMember m2) {
            return !(m1 == m2);
        }
        public static bool operator <(DropDownMember m1, DropDownMember m2) {
            return (m1.CompareTo(m2, StringComparison.CurrentCulture) < 0);
        }
        public static bool operator >(DropDownMember m1, DropDownMember m2) {
            return (m1.CompareTo(m2, StringComparison.CurrentCulture) > 0);
        }
    }
}
